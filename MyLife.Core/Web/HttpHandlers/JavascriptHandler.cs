using System.IO.Compression;
using System.Web;

namespace MyLife.Web.HttpHandlers
{
    public class JavascriptHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLowerInvariant();
                if (acceptEncoding.Contains("gzip"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }

            try
            {
                response.TransmitFile(request.PhysicalPath);
            }
            catch
            {
                throw new HttpException(404, "The Url you requested was not found.");
            }
        }

        #endregion
    }
}