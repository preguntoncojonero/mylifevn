using System.Collections.Generic;
using System.Configuration.Provider;

namespace MyLife.Web.Links
{
    public abstract class LinksProvider : ProviderBase
    {
        public abstract int InsertLink(Link link);
        public abstract void UpdateLink(Link link);
        public abstract void DeleteLink(int id);
        public abstract IList<Link> GetLinks(string user);
        public abstract Link GetLinkById(int id);
    }
}