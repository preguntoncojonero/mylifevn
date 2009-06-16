using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Collection;
using MyLife.Serialization;
using MyLife.Web.Security;

namespace MyLife.Web.Blogs
{
    public class Post : BizObject<Post, int>
    {
        private static readonly ICacheManager CacheManager;

        static Post()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
        }

        public Post()
        {
            Categories = new List<Category>();
        }

        public Post(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [ScriptIgnore]
        public int BlogId { get; set; }

        [ScriptIgnore]
        public string Content { get; set; }

        public bool Published { get; set; }

        public DateTime PublishedDate { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề bài viết")]
        [StringLength(255)]
        public string Title { get; set; }

        public bool CommentsEnabled { get; set; }

        public int ViewCount { get; set; }

        public bool Sticky { get; set; }

        [ScriptIgnore]
        public List<Comment> Comments { get; private set; }

        [ScriptIgnore]
        public List<Category> Categories { get; set; }

        public string RelativeUrl
        {
            get { return string.Format("/{0}/blog/post/{1}", CreatedBy, Slug); }
        }

        public void IncreaseViewCount()
        {
            ViewCount++;
            ServiceManager.GetService<BlogsProvider>().IncreaseViewCount(Id);
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<BlogsProvider>().InsertPost(this);
        }

        protected override void OnDataInserting()
        {
            base.OnDataInserting();
            Slug = ConvertTitleToSlug(Title);
            var post = GetPostBySlug(BlogId, Slug);
            if ((IsNew && post != null) || (post != null && post.Id != Id))
            {
                throw new ArgumentException(Messages.DuplicatePost);
            }

            if (PublishedDate != DateTime.MinValue)
            {
                CreatedDate = PublishedDate;
            }
        }

        public static string ConvertTitleToSlug(string title)
        {
            return new NonUnicodeEncoding(title).ToString(true).ToLowerInvariant().Trim('-');
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<BlogsProvider>().UpdatePost(this);
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<BlogsProvider>().DeletePost(Id);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new MyLifeSecurityException();
            }

            if (!IsNew)
            {
                if (
                    !User.Identity.Name.Equals(CreatedBy) &&
                    !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new MyLifeSecurityException();
                }
            }
        }

        public static Post GetPostById(int id)
        {
            var key = "Blogs_GetPostById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Post) cache[key];
            }
            var post = ServiceManager.GetService<BlogsProvider>().GetPostById(id);
            if (post != null)
            {
                post.Categories = Category.GetCategoriesOfPost(post.Id);
                post.Comments = Comment.GetCommentsOfPost(post.Id);
                AddToCache(cache, post);
            }
            return post;
        }

        public static IList<Post> GetPostsOfBlog(int blogId, int indexOfPage, int sizeOfPage, out int total)
        {
            var key = string.Format("Blogs_GetPostsOfBlog_{0}_{1}_{2}", blogId, indexOfPage, sizeOfPage);
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);

            if (cache.Contains(key))
            {
                var container = (ContainerCollection<Post>) cache[key];
                total = container.Total;
                return container.List;
            }

            var posts = ServiceManager.GetService<BlogsProvider>().GetPostsOfBlog(blogId, PostOptions.Published,
                                                                                  indexOfPage - 1,
                                                                                  sizeOfPage, out total);
            if (posts != null)
            {
                cache.Add(key, new ContainerCollection<Post> {Total = total, List = posts});
            }
            return posts;
        }

        public static Post GetPostBySlug(int blogId, string slug)
        {
            var key = string.Format("Blogs_GetPostBySlug_{0}_{1}", blogId, slug.ToLowerInvariant());
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Post) cache[key];
            }
            var post = ServiceManager.GetService<BlogsProvider>().GetPostBySlug(blogId, slug);
            if (post != null)
            {
                post.Categories = Category.GetCategoriesOfPost(post.Id);
                post.Comments = Comment.GetCommentsOfPost(post.Id);
                AddToCache(cache, post);
            }
            return post;
        }

        public static IList<Post> GetPostsOfCategory(int categoryId, int indexOfPage, int sizeOfPage, out int total)
        {
            var key = string.Format("Blogs_GetPostsOfCategory_{0}_{1}_{2}", categoryId, indexOfPage, sizeOfPage);
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                var container = (ContainerCollection<Post>) cache[key];
                total = container.Total;
                return container.List;
            }
            var posts = ServiceManager.GetService<BlogsProvider>().GetPostsOfCategory(categoryId, PostOptions.Published,
                                                                                      indexOfPage - 1, sizeOfPage,
                                                                                      out total);
            if (posts != null)
            {
                cache.Add(key, new ContainerCollection<Post> {Total = total, List = posts});
            }
            return posts;
        }

        public override string ToString()
        {
            return Title;
        }

        public static IList<Post> GetRecentPosts(int numberOfPosts)
        {
            if (numberOfPosts <= 0)
            {
                return null;
            }
            var key = string.Format("Blogs_GetRecentPosts_{0}", numberOfPosts);
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);

            if (cache.Contains(key))
            {
                return (IList<Post>) cache[key];
            }
            var posts = ServiceManager.GetService<BlogsProvider>().GetRecentPosts(numberOfPosts);
            if (posts != null)
            {
                cache.Add(key, posts);
            }
            return posts;
        }

        public static List<Post> GetRecentPosts(int blogId, int numberOfPosts)
        {
            if (numberOfPosts <= 0)
            {
                return new List<Post>();
            }

            var key = string.Format("Blogs_GetRecentPosts_{0}_{1}", blogId, numberOfPosts);
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);

            if (cache.Contains(key))
            {
                return (List<Post>) cache[key];
            }

            var posts = ServiceManager.GetService<BlogsProvider>().GetRecentPosts(blogId, numberOfPosts);
            if (posts != null)
            {
                cache.Add(key, posts);
            }
            return posts;
        }

        public static IList<Post> GetAllPosts(int blogId)
        {
            var key = "Blogs_GetAllPosts_" + blogId;
            if (CacheManager.Contains(key))
            {
                return (IList<Post>) CacheManager[key];
            }
            var posts = ServiceManager.GetService<BlogsProvider>().GetAllPosts(blogId);
            CacheManager.Add(key, posts);
            return posts;
        }

        public static IList<Post> Search(int blogId, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return new List<Post>();
            }

            var key = "Blogs_Search_" + blogId + "_" + keyword;
            if (CacheManager.Contains(key))
            {
                return (IList<Post>) CacheManager[key];
            }
            var posts = ServiceManager.GetService<BlogsProvider>().Search(blogId, keyword);
            CacheManager.Add(key, posts);
            return posts;
        }

        private static void AddToCache(ICacheManager cacheManager, Post post)
        {
            cacheManager.Add(string.Format("Blogs_GetPostBySlug_{0}", post.Slug), post);
            cacheManager.Add(string.Format("Blogs_GetPostById_{0}", post.Id), post);
        }
    }
}