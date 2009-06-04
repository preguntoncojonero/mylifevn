using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Web.Script.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Serialization;
using MyLife.Web.DynamicData;
using xVal.ServerSide;

namespace MyLife.Web.Blogs
{
    public class Category : BizObject<Category, int>
    {
        public Category()
        {
        }

        public Category(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [ScriptIgnore]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên chủ đề")]
        [DynamicTextField(FriendlyName = "Chủ đề")]
        public string Name { get; set; }

        public string Slug { get; set; }

        public string RelativeUrl
        {
            get { return string.Format("/{0}/blog/category/{1}", CreatedBy, Slug); }
        }

        protected override void OnDataInserting()
        {
            base.OnDataInserting();
            Slug = new NonUnicodeEncoding(Name).ToString(true).ToLowerInvariant();
            var blog = Blog.GetBlogByName(MyLifeContext.Current.User.Identity.Name);
            BlogId = blog.Id;
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<BlogsProvider>().InsertCategory(this);
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<BlogsProvider>().UpdateCategory(this);
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<BlogsProvider>().DeleteCategory(Id);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider).Flush();
        }

        public override void Validate()
        {
            base.Validate();
            var category = GetCategoryBySlug(BlogId, Slug);
            if (IsNew && category != null)
            {
                throw new RulesException("Name", Messages.CategoryExist);
            }
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new SecurityException();
            }

            if (!IsNew)
            {
                if (!CreatedBy.Equals(User.Identity.Name) && !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new SecurityException();
                }
            }
        }

        public static Category GetCategoryBySlug(int blogId, string slug)
        {
            var key = "Blogs_GetCategoryBySlug_" + slug;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Category) cache[key];
            }
            var category = ServiceManager.GetService<BlogsProvider>().GetCategoryBySlug(blogId, slug);
            if (category != null)
            {
                cache.Add(key, category);
            }
            return category;
        }

        public static List<Category> GetCategoriesOfBlog(int blogId)
        {
            var key = "Blogs_GetCategoriesOfBlog_" + blogId;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Category>) cache[key];
            }
            var categories = ServiceManager.GetService<BlogsProvider>().GetCategoriesOfBlog(blogId);
            if (categories != null)
            {
                cache.Add(key, categories);
            }
            return categories;
        }

        public static List<Category> GetCategoriesOfPost(int postId)
        {
            var key = "Blogs_GetCategoriesOfPost_" + postId;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (List<Category>) cache[key];
            }
            var categories = ServiceManager.GetService<BlogsProvider>().GetCategoriesOfPost(postId);
            if (categories != null)
            {
                cache.Add(key, categories);
            }
            return categories;
        }

        public static Category GetCategoryById(int id)
        {
            var key = "Blogs_GetCategoryById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Blogs.CacheProvider);
            if (cache.Contains(key))
            {
                return (Category) cache[key];
            }
            var category = ServiceManager.GetService<BlogsProvider>().GetCategoryById(id);
            if (category != null)
            {
                cache.Add(key, category);
            }
            return category;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}