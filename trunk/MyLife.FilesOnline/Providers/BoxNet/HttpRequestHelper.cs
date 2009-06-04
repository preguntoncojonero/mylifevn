using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace MyLife.FilesOnline.Providers.BoxNet
{
    internal class HttpRequestHelper
    {
        private static string UserAgent
        {
            get
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location);
                var str2 = string.Format("{0}.{1}", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart);
                return string.Format("{0}/{1}.{2}", "Box.net.API", str2, versionInfo.ProductPrivatePart);
            }
        }

        public static string DumpResponse(HttpWebResponse resp)
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            writer.WriteLine(
                string.Format("{0}/{1} {2} {3}",
                              new object[] {"HTTP", resp.ProtocolVersion, (int) resp.StatusCode, resp.StatusDescription}));
            foreach (var str in resp.Headers.AllKeys)
            {
                writer.WriteLine(string.Format("{0}: {1}", str, resp.Headers[str]));
            }
            using (var stream = resp.GetResponseStream())
            {
                writer.WriteLine("");
                var str2 = new StreamReader(stream).ReadToEnd();
                writer.WriteLine(str2);
            }
            writer.Close();
            return sb.ToString();
        }

        public static HttpWebResponse SendRequest(string requestUri, bool allowAutoRedirect, int timeoutMs,
                                                  string contentType, byte[] postData, IWebProxy webProxy)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            if (timeoutMs > 0)
            {
                request.Timeout = timeoutMs;
                request.ReadWriteTimeout = timeoutMs;
            }
            request.AllowAutoRedirect = allowAutoRedirect;
            request.UserAgent = UserAgent;
            if (contentType != null)
            {
                request.ContentType = contentType;
            }

            if (webProxy != null)
            {
                request.Proxy = webProxy;    
            }

            request.Pipelined = false;
            request.ProtocolVersion = HttpVersion.Version10;
            if (postData != null)
            {
                request.Method = "POST";
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            try
            {
                var response2 = (HttpWebResponse) request.GetResponse();
                if (response2.StatusCode > ((HttpStatusCode) 0x12b))
                {
                    throw new WebException(response2.StatusCode + ": " + response2.StatusDescription, null,
                                           WebExceptionStatus.UnknownError, response2);
                }
                response = response2;
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.Timeout)
                {
                    throw new ResponseTimeoutException(exception);
                }
                throw;
            }
            return response;
        }
    }
}