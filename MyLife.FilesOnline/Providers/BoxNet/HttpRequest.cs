using System.IO;
using System.Net;

namespace MyLife.FilesOnline.Providers.BoxNet
{
    internal class HttpRequest
    {
        private readonly string requestUrl;

        public HttpRequest(string requestUrl)
            : this(requestUrl, null)
        {
        }

        public HttpRequest(string requestUrl, IWebProxy webProxy)
        {
            AllowAutoRedirect = true;
            ContentType = null;
            this.requestUrl = requestUrl;
            Proxy = webProxy;
        }

        public bool AllowAutoRedirect { get; set; }

        public string ContentType { get; set; }

        public byte[] PostData { get; set; }

        public IWebProxy Proxy { get; set; }

        public Stream GetResponse()
        {
            return GetResponse(-1);
        }

        public Stream GetResponse(int timeoutMs)
        {
            var responseStream =
                HttpRequestHelper.SendRequest(requestUrl, AllowAutoRedirect, timeoutMs, ContentType, PostData, Proxy).
                    GetResponseStream();
            return responseStream;
        }
    }
}