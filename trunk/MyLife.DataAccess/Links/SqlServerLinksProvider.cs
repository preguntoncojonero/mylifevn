using System.Collections.Generic;
using System.Linq;
using MyLife.Web;
using MyLife.Web.Links;

namespace MyLife.DataAccess.Links
{
    public class SqlServerLinksProvider : LinksProvider
    {
        public override int InsertLink(Link link)
        {
            var context = new MyLifeEntities();
            var obj = new tblLinks();
            BizObject<Link, int>.CopyToObject(link, obj);
            context.AddTotblLinks(obj);
            context.SaveChanges();
            return obj.Id;
        }

        public override void UpdateLink(Link link)
        {
            var context = new MyLifeEntities();
            var obj = new tblLinks {Id = link.Id};
            context.AttachTo("tblLinks", obj);
            BizObject<Link, int>.CopyToObject(link, obj);
            context.SaveChanges();
        }

        public override void DeleteLink(int id)
        {
            var context = new MyLifeEntities();
            var obj = new tblLinks {Id = id};
            context.AttachTo("tblLinks", obj);
            context.DeleteObject(obj);
            context.SaveChanges();
        }

        public override IList<Link> GetLinks(string user)
        {
            var context = new MyLifeEntities();
            var list = context.tblLinks.Where(item => item.CreatedBy == user).OrderBy(item => item.Id).ToList();
            return Convert(list);
        }

        private static IList<Link> Convert(IEnumerable<tblLinks> list)
        {
            var retval = new List<Link>();
            foreach (var obj in list)
            {
                retval.Add(Convert(obj));
            }
            return retval;
        }

        private static Link Convert(tblLinks obj)
        {
            if (obj == null)
            {
                return null;
            }
            var link = new Link(obj.Id, obj.CreatedDate, obj.CreatedBy, obj.ModifiedDate, obj.ModifiedBy);
            BizObject<Link, int>.CopyFromObject(link, obj);
            return link;
        }

        public override Link GetLinkById(int id)
        {
            var context = new MyLifeEntities();
            var obj = context.tblLinks.Where(item => item.Id == id).FirstOrDefault();
            return Convert(obj);
        }
    }
}