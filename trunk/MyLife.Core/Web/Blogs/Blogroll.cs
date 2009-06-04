using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Web.Script.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Web.DynamicData;

namespace MyLife.Web.Blogs
{
    public class Blogroll : BizObject<Blogroll, int>
    {
        public Blogroll(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        public Blogroll()
        {
        }

        [ScriptIgnore]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên blogroll")]
        [StringLength(255)]
        [DynamicTextField(FriendlyName = "Blogroll")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập liên kết")]
        [StringLength(255)]
        [RegularExpression(Constants.Regulars.Website, ErrorMessage = "Url của bạn không hợp lệ. Ex: http://")]
        [DynamicTextField(FriendlyName = "Liên kết")]
        public string Link { get; set; }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<BlogsProvider>().InsertBlogroll(this);
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<BlogsProvider>().UpdateBlogroll(this);
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<BlogsProvider>().DeleteBlogroll(Id);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new SecurityException(Web.Messages.NotAuthorization);
            }

            if (!IsNew)
            {
                if (!CreatedBy.Equals(User.Identity.Name) && !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new SecurityException(Web.Messages.NotAuthorization);
                }
            }
        }

        protected override void OnDataInserting()
        {
            base.OnDataInserting();
            var blog = Blog.GetBlogByName(MyLifeContext.Current.User.Identity.Name);
            BlogId = blog.Id;
        }

        public static List<Blogroll> GetBlogrolls(int blogId)
        {
            var key = "Blogs_GetBlogrolls_" + blogId;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Blogroll>) cache[key];
            }
            var blogrolls = ServiceManager.GetService<BlogsProvider>().GetBlogrolls(blogId);
            if (blogrolls != null)
            {
                cache.Add(key, blogrolls);
            }
            return blogrolls;
        }

        public static Blogroll GetBlogrollById(int id)
        {
            var key = "Blogs_GetBlogrollById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Blogroll) cache[key];
            }
            var blogroll = ServiceManager.GetService<BlogsProvider>().GetBlogrollById(id);
            if (blogroll != null)
            {
                cache.Add(key, blogroll);
            }
            return blogroll;
        }
    }
}