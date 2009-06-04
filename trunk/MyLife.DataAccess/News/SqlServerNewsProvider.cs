using System.Collections.Generic;
using System.Linq;
using MyLife.Web.News;

namespace MyLife.DataAccess.News
{
    public class SqlServerNewsProvider : NewsProvider
    {
        public override int InsertNews(Web.News.News news)
        {
            var context = new MyLifeEntities();
            var obj = new tblNews();
            news.CopyToObject(obj);
            context.AddTotblNews(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateNews(Web.News.News news)
        {
            var context = new MyLifeEntities();
            var obj = new tblNews {Id = news.Id};
            context.AttachTo("tblNews", obj);
            news.CopyToObject(obj);
            context.SaveChanges();
        }

        public override void DeleteNews(int id)
        {
            var context = new MyLifeEntities();
            var obj = new tblNews {Id = id};
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override Web.News.News GetNewsById(int id)
        {
            var context = new MyLifeEntities();
            var obj = context.tblNews.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }

        private static Web.News.News Convert(tblNews obj)
        {
            if (obj == null)
            {
                return null;
            }

            var news = new Web.News.News(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            news.CopyFromObject(obj);
            return news;
        }

        public override IList<Web.News.News> GetNews()
        {
            var context = new MyLifeEntities();
            var list = context.tblNews.OrderByDescending(item => item.CreatedDate).ToList();
            return Convert(list);
        }

        private static IList<Web.News.News> Convert(List<tblNews> list)
        {
            var retval = new List<Web.News.News>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        public override IList<Web.News.News> GetNews(int numberOfNews)
        {
            var context = new MyLifeEntities();
            var list = context.tblNews.OrderByDescending(item => item.CreatedDate).Skip(0).Take(numberOfNews).ToList();
            return Convert(list);
        }

        public override Web.News.News GetNewsBySlug(string slug)
        {
            var context = new MyLifeEntities();
            var obj = context.tblNews.Where(item => item.Slug == slug).FirstOrDefault();
            return Convert(obj);
        }
    }
}