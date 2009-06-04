using System.Web;

namespace MyLife.Web.HttpHandlers
{
    public class SilverlightXapHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.TransmitFile(context.Request.PhysicalPath);
        }

        #endregion
    }
}