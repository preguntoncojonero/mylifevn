using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.News
{
    public abstract class NewsProvider : ProviderBase
    {
        public abstract int InsertNews(News news);
        public abstract void UpdateNews(News news);
        public abstract void DeleteNews(int id);
        public abstract News GetNewsById(int id);
        public abstract News GetNewsBySlug(string slug);
        public abstract IList<News> GetNews();
        public abstract IList<News> GetNews(int numberOfNews);
    }
}