using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace MyLife.Web.Blogs
{
    public class Blog : BizObject<Blog, int>
    {
        private static readonly ICacheManager CacheManager;

        static Blog()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
        }

        public Blog()
        {
        }

        public Blog(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        public bool AnonymousCommentEnabled { get; set; }

        [Required(ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 365")]
        [Range(0, 365, ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 365")]
        public int DaysCommentEnabled { get; set; }

        public bool ModerationCommentEnable { get; set; }

        public bool CommentsEnabled { get; set; }

        [Required(ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        [Range(0, 100, ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        public int NumberOfRecentPosts { get; set; }

        [Required(ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        public int NumberOfRecentComments { get; set; }

        [Required(ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        public int PostsPerFeed { get; set; }

        [Required(ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị chỉ chấp nhận từ 0 đến 100")]
        public int PostsPerPage { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên của blog")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string Theme { get; set; }

        public string AboutMe { get; set; }

        [StringLength(160, ErrorMessage = "Giá trị không được vượt quá 160 ký tự")]
        public string Status { get; set; }

        public IList<string> Friends { get; set; }

        public static Blog New()
        {
            var blog = new Blog
                           {
                               DaysCommentEnabled = 0,
                               Description = "Your blog slogan here",
                               ModerationCommentEnable = false,
                               CommentsEnabled = true,
                               Name = "Your blog name here",
                               NumberOfRecentPosts = 10,
                               NumberOfRecentComments = 10,
                               PostsPerFeed = 10,
                               PostsPerPage = 10,
                               AnonymousCommentEnabled = true,
                               Theme = MyLifeContext.Settings.Blogs.DefaultTheme
                           };
            return blog;
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<BlogsProvider>().InsertBlog(this);
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<BlogsProvider>().UpdateBlog(this);
        }

        protected override void DataDelete()
        {
            throw new NotImplementedException();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new SecurityException(Web.Messages.NotAuthorization);
            }

            if (!IsNew)
            {
                if (!User.Identity.Name.Equals(CreatedBy) || !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new SecurityException(Web.Messages.NotAuthorization);
                }
            }
        }

        public static Blog GetBlogByName(string name)
        {
            var key = "Blogs_GetBlogByName_" + name.ToLowerInvariant();
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Blog) cache[key];
            }
            var blog = ServiceManager.GetService<BlogsProvider>().GetBlogByName(name);
            AddToCache(cache, blog);
            return blog;
        }

        private static void AddToCache(ICacheManager cacheManager, Blog blog)
        {
            if (blog != null)
            {
                cacheManager.Add(string.Format("Blogs_GetBlogByName_{0}", blog.CreatedBy), blog);
                cacheManager.Add(string.Format("Blogs_GetBlogById_{0}", blog.Id), blog);
            }
        }

        public static Blog GetBlogById(int id)
        {
            var key = "Blogs_GetBlogById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Blog) cache[key];
            }
            var blog = ServiceManager.GetService<BlogsProvider>().GetBlogById(id);
            AddToCache(cache, blog);
            return blog;
        }

        public static string[] GetThemes()
        {
            var key = "Blogs_GetThemes";
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (string[]) cache[key];
            }
            var path = Path.Combine(MyLifeContext.WorkingFolder, "Content\\Themes\\Blogs");
            var themes = Directory.GetDirectories(path).Select(item => Path.GetFileName(item)).ToArray();
            cache.Add(key, themes);
            return themes;
        }

        public override string ToString()
        {
            return Name;
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        public static IList<Blog> GetNewesUsers(int numberOfUsers)
        {
            var key = "Blogs_GetNewesUsers" + numberOfUsers;
            if (CacheManager.Contains(key))
            {
                return (IList<Blog>) CacheManager[key];
            }
            var users = ServiceManager.GetService<BlogsProvider>().GetNewesUsers(numberOfUsers);
            CacheManager.Add(key, users);
            return users;
        }
    }
}