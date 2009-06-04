using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace MyLife.FilesOnline.Providers.BoxNet
{
    public class FileUploader
    {
        private const string CONTENT_DISP = "Content-Disposition: form-data; name=";
        private readonly Hashtable coFormFields;
        private readonly BoxNetFilesOnlineProvider provider;
        private string beginBoundary;
        private Stream coFileStream;
        protected HttpWebRequest coRequest;

        public FileUploader(string url, BoxNetFilesOnlineProvider provider)
        {
            URL = url;
            coFormFields = new Hashtable();
            ResponseText = new StringBuilder();
            BufferSize = 0x2800;
            BeginBoundary = "ou812--------------8c405ee4e38917c";
            TransferHttpVersion = HttpVersion.Version10;
            FileContentType = "text/xml";
            this.provider = provider;
        }

        public string BeginBoundary
        {
            get { return beginBoundary; }
            set
            {
                beginBoundary = value;
                ContentBoundary = "--" + BeginBoundary;
                EndingBoundary = ContentBoundary + "--";
            }
        }

        public int BufferSize { get; set; }

        protected string ContentBoundary { get; set; }

        protected string EndingBoundary { get; set; }

        public string FileContentType { get; set; }

        public IWebProxy Proxy { get; set; }

        public StringBuilder ResponseText { get; set; }

        public Version TransferHttpVersion { get; set; }

        public string URL { get; set; }

        public string GetFileHeader(string aFilename)
        {
            return
                (ContentBoundary + "\r\n" + CONTENT_DISP + "\"sendfile\"; filename=\"" + Path.GetFileName(aFilename) +
                 "\"\r\nContent-type: " + FileContentType + "\r\n\r\n");
        }

        public string GetFileTrailer()
        {
            return ("\r\n" + EndingBoundary);
        }

        public string GetFormFields()
        {
            var str = "";
            var enumerator = coFormFields.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object obj2 = str;
                str =
                    string.Concat(
                        new[]
                            {
                                obj2, ContentBoundary, "\r\n", CONTENT_DISP, '"', enumerator.Key, "\"\r\n\r\n",
                                enumerator.Value, "\r\n"
                            });
            }
            return str;
        }

        public virtual void GetResponse()
        {
            if (coFileStream == null)
            {
                WebResponse response;
                string str;
                try
                {
                    response = coRequest.GetResponse();
                }
                catch (WebException exception)
                {
                    response = exception.Response;
                }
                if (response == null)
                {
                    throw new Exception("MultipartForm: Error retrieving server response");
                }
                var reader = new StreamReader(response.GetResponseStream());
                ResponseText.Length = 0;
                while ((str = reader.ReadLine()) != null)
                {
                    ResponseText.Append(str);
                }
                response.Close();
            }
        }

        public virtual Stream GetStream()
        {
            return coFileStream ?? coRequest.GetRequestStream();
        }

        public void SendFile(string filename)
        {
            coRequest = (HttpWebRequest) WebRequest.Create(URL);
            coRequest.ProtocolVersion = TransferHttpVersion;
            coRequest.Method = "POST";
            coRequest.ContentType = "multipart/form-data; boundary=" + BeginBoundary;
            coRequest.Headers.Add("Cache-Control", "no-cache");
            coRequest.KeepAlive = true;
            if (Proxy != null)
            {
                coRequest.Proxy = Proxy;    
            }
            var str = GetFormFields();
            var str2 = GetFileHeader(filename);
            var str3 = GetFileTrailer();
            var info = new FileInfo(filename);
            coRequest.ContentLength = ((str.Length + str2.Length) + str3.Length) + info.Length;
            var io = GetStream();
            WriteString(io, str);
            WriteString(io, str2);
            WriteFile(io, filename);
            WriteString(io, str3);
            GetResponse();
            io.Close();
            coRequest = null;
            provider.RaiseFileUploadComplete();
        }

        public void SendFile(string filename, byte[] data)
        {
            coRequest = (HttpWebRequest)WebRequest.Create(URL);
            coRequest.ProtocolVersion = TransferHttpVersion;
            coRequest.Method = "POST";
            coRequest.ContentType = "multipart/form-data; boundary=" + BeginBoundary;
            coRequest.Headers.Add("Cache-Control", "no-cache");
            coRequest.KeepAlive = true;
            coRequest.Proxy = Proxy ?? WebRequest.GetSystemWebProxy();
            var str = GetFormFields();
            var str2 = GetFileHeader(filename);
            var str3 = GetFileTrailer();
            coRequest.ContentLength = ((str.Length + str2.Length) + str3.Length) + data.Length;
            var io = GetStream();
            WriteString(io, str);
            WriteString(io, str2);
            WriteFile(io, data);
            WriteString(io, str3);
            GetResponse();
            io.Close();
            coRequest = null;
            provider.RaiseFileUploadComplete();
        }

        public void SetField(string key, string str)
        {
            coFormFields[key] = str;
        }

        public void SetFilename(string path)
        {
            coFileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        public void WriteFile(Stream io, string aFilename)
        {
            int num;
            var stream = new FileStream(aFilename, FileMode.Open, FileAccess.Read);
            stream.Seek(0L, SeekOrigin.Begin);
            var buffer = new byte[BufferSize];
            var bytesSent = 0L;
            while ((num = stream.Read(buffer, 0, BufferSize)) > 0)
            {
                io.Write(buffer, 0, num);
                bytesSent += num;
                provider.RaiseFileUploading(bytesSent, stream.Length);
            }
            stream.Close();
        }

        public void WriteFile(Stream io, byte[] data)
        {
            int num;
            var stream = new MemoryStream(data);
            stream.Seek(0L, SeekOrigin.Begin);
            var buffer = new byte[BufferSize];
            var bytesSent = 0L;
            while ((num = stream.Read(buffer, 0, BufferSize)) > 0)
            {
                io.Write(buffer, 0, num);
                bytesSent += num;
                provider.RaiseFileUploading(bytesSent, stream.Length);
            }
            stream.Close();
        }

        public void WriteString(Stream io, string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            io.Write(bytes, 0, bytes.Length);
        }
    }
}