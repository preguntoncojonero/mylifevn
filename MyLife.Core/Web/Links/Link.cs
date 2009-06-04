using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using MyLife.Web.DynamicData;

namespace MyLife.Web.Links
{
    public class Link : BizObject<Link, int>
    {
        public Link()
        {
        }

        public Link(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề")]
        [DynamicTextField(FriendlyName = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập liên kết")]
        [RegularExpression(Constants.Regulars.Website, ErrorMessage = "Liên kết không hợp lệ")]
        [DynamicTextField(FriendlyName = "Liên kết")]
        public string Url { get; set; }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<LinksProvider>().InsertLink(this);
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Links.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<LinksProvider>().UpdateLink(this);
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Links.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<LinksProvider>().DeleteLink(Id);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.Links.CacheProvider).Flush();
        }

        protected override void VerifyAuthorization()
        {
        }

        public static IList<Link> GetLinksOfUser(string user)
        {
            var key = "Links_GetLinksOfUser_" + user;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Links.CacheProvider);
            if (cache.Contains(key))
            {
                return (IList<Link>) cache[key];
            }
            var links = ServiceManager.GetService<LinksProvider>().GetLinks(user);
            cache.Add(key, links);
            return links;
        }

        public static Link GetLinkById(int id)
        {
            var key = "Links_GetLinkById_" + id;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.Links.CacheProvider);
            if (cache.Contains(key))
            {
                return (Link)cache[key];
            }
            var link = ServiceManager.GetService<LinksProvider>().GetLinkById(id);
            if (link != null)
            {
                cache.Add(key, link);    
            }
            
            return link;
        }
    }
}