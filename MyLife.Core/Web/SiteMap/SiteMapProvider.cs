using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.SiteMap
{
    public abstract class SiteMapProvider : ProviderBase
    {
        public abstract long InsertSiteMap(SiteMap sitemap);
        public abstract void UpdateSiteMap(SiteMap sitemap);
        public abstract void DeleteSiteMap(long id);
        public abstract IList<SiteMap> GetSiteMap(string user);
        public abstract SiteMap GetSiteMapById(long id);
    }
}