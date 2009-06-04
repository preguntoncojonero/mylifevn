using System.IO;
using System.Web;

namespace MyLife.Web.HttpHandlers
{
    public class DynamicHtmlHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var file = Path.GetFileName(context.Request.PhysicalPath);
            context.Response.TransmitFile(Path.Combine(MyLifeContext.WorkingFolder, "Content\\DynamicHtml\\" + file));
        }

        #endregion
    }
}