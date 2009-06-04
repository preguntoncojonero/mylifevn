using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Web.Security;

namespace MyLife.Web.MoneyBox
{
    public class Category : BizObject<Category, int>
    {
        private static readonly ICacheManager CacheManager;

        static Category()
        {
            CacheManager = CacheFactory.GetCacheManager(MyLifeContext.Settings.MoneyBox.CacheProvider);
        }

        public Category()
        {
        }

        public Category(int id) : base(id)
        {
        }

        public Category(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn chưa nhập tên danh mục")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(6)]
        public string ColorHex { get; set; }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<MoneyBoxProvider>().InsertCategory(this);
        }

        protected override void OnDataInserted()
        {
            base.OnDataInserted();
            CacheManager.Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<MoneyBoxProvider>().UpdateCategory(this);
        }

        protected override void OnDataUpdated()
        {
            base.OnDataUpdated();
            CacheManager.Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<MoneyBoxProvider>().DeleteCategory(Id);
        }

        protected override void OnDataDeleted()
        {
            base.OnDataDeleted();
            CacheManager.Flush();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new MyLifeSecurityException();
            }

            if (!IsNew)
            {
                if (!User.Identity.Name.Equals(CreatedBy) && !User.IsInRole(Constants.Roles.Administrators))
                {
                    throw new MyLifeSecurityException();
                }
            }
        }

        public static IList<Category> GetCategories(string user)
        {
            var key = "MoneyBox_GetCategories_" + user;
            if (CacheManager.Contains(key))
            {
                return (IList<Category>) CacheManager[key];
            }
            var categories = ServiceManager.GetService<MoneyBoxProvider>().GetCategories(user);
            CacheManager.Add(key, categories);
            return categories;
        }

        public static Category GetCategoryById(int id)
        {
            if (id == 0)
            {
                return null;
            }

            var key = "MoneyBox_GetCategoryById_" + id;
            if (CacheManager.Contains(key))
            {
                return (Category) CacheManager[key];
            }
            var category = ServiceManager.GetService<MoneyBoxProvider>().GetCategoryById(id);
            if (category != null)
            {
                CacheManager.Add(key, category);
            }
            return category;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}