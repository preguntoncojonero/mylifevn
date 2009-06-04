using System.Web.Mvc;

namespace MyLife.Web.SiteMap
{
    public interface ISiteMapController
    {
        [AcceptVerbs("POST")]
        ActionResult GetSiteMapById();

        [AcceptVerbs("POST")]
        ActionResult AddOrEditSiteMap();

        [AcceptVerbs("POST")]
        ActionResult DeleteSiteMap();
    }
}