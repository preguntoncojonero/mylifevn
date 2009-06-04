using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Web.DynamicData;

namespace MyLife.Web.Blogs
{
    public class Comment : BizObject<Comment, int>
    {
        public Comment()
        {
        }

        public Comment(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn hãy nhập tên của bạn")]
        [StringLength(255, ErrorMessage = "Tên của bạn quá dài")]
        [DynamicTextField(FriendlyName = "Người gửi")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn hãy nhập nội dung phản hồi của bạn")]
        [DynamicTextField(FriendlyName = "Nội dung phản hồi", Multiple = true)]
        public string Content { get; set; }

        [Required(ErrorMessage = "Bạn hãy nhập địa chỉ email")]
        [StringLength(255, ErrorMessage = "Địa chỉ email của bạn quá dài")]
        [RegularExpression(Constants.Regulars.Email, ErrorMessage = "Địa chỉ email không hợp lệ")]
        [DynamicTextField]
        public string Email { get; set; }

        public string IP { get; set; }

        [DynamicChoiceField(FriendlyName = "Chấp thuận")]
        public bool IsApproved { get; set; }

        public int PostId { get; set; }

        public int BlogId { get; set; }

        [RegularExpression(Constants.Regulars.Website, ErrorMessage = "Địa chỉ trang web của bạn không hợp lệ")]
        public string Website { get; set; }

        public string RelativeUrl
        {
            get { return string.Format("/{0}/blog/permalink/{1}#comment-{2}", CreatedBy, PostId, Id); }
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<BlogsProvider>().InsertComment(this);
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<BlogsProvider>().UpdateComment(this);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void OnDataUpdated()
        {
            base.OnDataUpdated();
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<BlogsProvider>().DeleteComment(Id);
        }

        protected override void VerifyAuthorization()
        {
            if (IsNew)
            {
                var post = Post.GetPostById(PostId);
                var blog = Blog.GetBlogById(post.BlogId);
                VerifyCommentsEnabled(blog, post, true);
                CreatedBy = blog.CreatedBy;
            }
            else
            {
                // For edit and delete action
                if (!User.Identity.IsAuthenticated)
                {
                    throw new SecurityException(Web.Messages.NotAuthorization);
                }

                if (!CreatedBy.Equals(User.Identity.Name) && !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new SecurityException(Web.Messages.NotAuthorization);
                }
            }
        }

        public static List<Comment> GetCommentsOfPost(int postId)
        {
            var key = "Blogs_GetCommentsOfPost_" + postId;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Comment>) cache[key];
            }
            var comments = ServiceManager.GetService<BlogsProvider>().GetCommentsOfPost(postId);
            if (comments != null)
            {
                cache.Add(key, comments);
            }
            return comments;
        }

        public static List<Comment> GetCommentsOfBlog(int blogId)
        {
            var key = "Blogs_GetCommentsOfBlog_" + blogId;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Comment>) cache[key];
            }
            var comments = ServiceManager.GetService<BlogsProvider>().GetCommentsOfBlog(blogId);
            if (comments != null)
            {
                cache.Add(key, comments);
            }
            return comments;
        }

        public override string ToString()
        {
            return Name;
        }

        protected override void OnDataInserting()
        {
            CreatedDate = DateTime.UtcNow;
            var post = Post.GetPostById(PostId);
            var blog = Blog.GetBlogById(post.BlogId);
            IsApproved = !blog.ModerationCommentEnable;
            IP = MyLifeContext.Current.RequestIP;
            BlogId = blog.Id;
        }

        protected override void OnDataInserted()
        {
            base.OnDataInserted();
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        public static bool VerifyCommentsEnabled(Blog blog, Post post, bool throwEx)
        {
            Exception ex = null;
            if (!blog.CommentsEnabled || !post.CommentsEnabled)
            {
                ex = new SecurityException(Messages.CommentsLock);
                goto Return;
            }

            if (!MyLifeContext.Current.User.Identity.IsAuthenticated && !blog.AnonymousCommentEnabled)
            {
                ex = new SecurityException(Messages.CommentsAnonymousLock);
                goto Return;
            }

            if (blog.DaysCommentEnabled > 0 &&
                (post.CreatedDate - DateTime.Now).Days > blog.DaysCommentEnabled)
            {
                ex = new SecurityException(Messages.CommentsLock);
                goto Return;
            }

            Return:
            if (ex != null)
            {
                if (throwEx)
                {
                    throw ex;
                }
                return false;
            }
            return true;
        }

        public static Comment GetCommentById(int id)
        {
            var key = "Blogs_GetCommentById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Comment) cache[key];
            }
            var comment = ServiceManager.GetService<BlogsProvider>().GetCommentById(id);
            if (comment != null)
            {
                cache.Add(key, comment);
            }
            return comment;
        }

        public static List<Comment> GetRecentComments(int blogId, int numberOfComments)
        {
            if (numberOfComments <= 0)
            {
                return new List<Comment>();
            }

            var key = string.Format("Blogs_GetRecentComments_{0}_{1}", blogId, numberOfComments);
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Comment>) cache[key];
            }
            var comments = ServiceManager.GetService<BlogsProvider>().GetRecentComments(blogId, numberOfComments);
            if (comments != null)
            {
                cache.Add(key, comments);
            }
            return comments;
        }
    }
}