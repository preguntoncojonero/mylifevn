using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace MyLife.Web.News
{
    public class News : BizObject<News, int>
    {
        public News()
        {
        }

        public News(int id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : base(id, createdDate, createdBy, modifiedDate, modifiedBy)
        {
        }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }

        public string RelativeUrl
        {
            get { return string.Format("/news/{0}", Slug); }
        }

        protected override int DataInsert()
        {
            return ServiceManager.GetService<NewsProvider>().InsertNews(this);
        }

        protected override void OnDataInserted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.News.CacheProvider).Flush();
        }

        protected override void DataUpdate()
        {
            ServiceManager.GetService<NewsProvider>().UpdateNews(this);
        }

        protected override void OnDataUpdated()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.News.CacheProvider).Flush();
        }

        protected override void DataDelete()
        {
            ServiceManager.GetService<NewsProvider>().DeleteNews(Id);
        }

        protected override void OnDataDeleted()
        {
            CacheFactory.GetCacheManager(MyLifeContext.Settings.News.CacheProvider).Flush();
        }

        protected override void VerifyAuthorization()
        {
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                throw new SecurityException(Messages.NotAuthorization);
            }

            if (!User.IsInRole(Constants.Roles.Administrators))
            {
                throw new SecurityException(Messages.NotAuthorization);
            }
        }

        public static IList<News> GetNews(int numberOfNews)
        {
            var key = "News_GetNews_" + numberOfNews;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.News.CacheProvider);
            if (cache.Contains(key))
            {
                return (IList<News>) cache[key];
            }
            var news = ServiceManager.GetService<NewsProvider>().GetNews(numberOfNews);
            cache.Add(key, news);
            return news;
        }

        public static News GetNewsBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var key = "News_GetNewsBySlug_" + slug;
            var cache = CacheFactory.GetCacheManager(MyLifeContext.Settings.News.CacheProvider);
            if (cache.Contains(key))
            {
                return (News) cache[key];
            }
            var news = ServiceManager.GetService<NewsProvider>().GetNewsBySlug(slug);
            if (news != null)
            {
                cache.Add(key, news);
            }
            return news;
        }
    }
}