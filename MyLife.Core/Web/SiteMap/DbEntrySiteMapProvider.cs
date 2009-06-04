using System.Collections.Generic;
using Lephone.Data.Definition;
using Lephone.Linq;

namespace MyLife.Web.SiteMap
{
    public class DbEntrySiteMapProvider : SiteMapProvider
    {
        public override long InsertSiteMap(SiteMap sitemap)
        {
            var obj = DbObjectModelBase<DbSiteMap, long>.New();
            BizObject<SiteMap, long>.CopyToObject(sitemap, obj);
            obj.Save();
            return obj.Id;
        }

        public override void UpdateSiteMap(SiteMap sitemap)
        {
            var obj = DbObjectModelBase<DbSiteMap, long>.FindById(sitemap.Id);
            if (obj == null)
            {
                return;
            }

            BizObject<SiteMap, long>.CopyToObject(sitemap, obj);
            obj.Save();
        }

        public override void DeleteSiteMap(long id)
        {
            DbSiteMap.Delete(id);
        }

        public override IList<SiteMap> GetSiteMap(string user)
        {
            var dbSiteMaps = LinqObjectModel<DbSiteMap, long>.Find(item => item.CreatedBy == user);
            var retval = new List<SiteMap>();
            foreach (var dbSiteMap in dbSiteMaps)
            {
                retval.Add(Convert(dbSiteMap));
            }
            return retval;
        }

        private static SiteMap Convert(DbSiteMap dbSiteMap)
        {
            if (dbSiteMap == null)
            {
                return null;
            }

            var category = new SiteMap(dbSiteMap.Id, dbSiteMap.CreatedDate, dbSiteMap.CreatedBy,
                                       dbSiteMap.ModifiedDate, dbSiteMap.ModifiedBy);
            BizObject<SiteMap, long>.CopyFromObject(category, dbSiteMap);
            return category;
        }

        public override SiteMap GetSiteMapById(long id)
        {
            return Convert(DbObjectModelBase<DbSiteMap, long>.FindById(id));
        }
    }
}