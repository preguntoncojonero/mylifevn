using System;
using System.Web;

namespace MyLife.Web
{
    public class UrlRewriter : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }

        #endregion

        private static void context_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            var request = application.Request;
            var host = request.Url.Host.ToLowerInvariant();
            if (request.IsLocal || host.StartsWith("www.")) return;
            if (request.Url.IsDefaultPort)
            {
                application.Response.Redirect(
                    string.Format("{0}://www.{1}{2}", request.Url.Scheme, host, request.RawUrl), true);
            }
            else
            {
                application.Response.Redirect(
                    string.Format("{0}://www.{1}:{2}{3}", request.Url.Scheme, host, request.Url.Port,
                                  request.RawUrl), true);
            }
        }
    }
}