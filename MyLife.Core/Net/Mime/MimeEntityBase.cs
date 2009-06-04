using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLife.Net.Mime
{
    /// <summary>
    /// Base class for IMimeEntity classes
    /// </summary>
    public abstract class MimeEntityBase : IMimeEntity
    {
        private MimeEntityCollection _entities;
        private IDictionary<string, string> _headers;
        protected IMimeParser _mimeParser;

        /// <summary>
        /// Initializes a new instance of the MimeEntityBase class with empty MIME data
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        public MimeEntityBase(IMimeParser mimeParser) : this(mimeParser, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MimeEntityBase class
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        /// <param name="mimeData">A string containing the MIME data for the MIME entity</param>
        public MimeEntityBase(IMimeParser mimeParser, string mimeData)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeParser", "mimeParser cannot be null");
            }

            _mimeParser = mimeParser;
            SetMimeData(mimeData);
            var contentType = GetHeaderValue("Content-Type");
            if (contentType.Contains(";"))
                contentType = contentType.Substring(0, contentType.IndexOf(";"));
            ContentType = contentType;
            ContentTransferEncoding = GetHeaderValue("Content-Transfer-Encoding");
        }

        #region IMimeEntity Members

        /// <summary>
        /// Gets or sets the Content-Type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Transfer-Encoding
        /// </summary>
        public string ContentTransferEncoding { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Collection of MIME headers
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new Dictionary<string, string>();
                return _headers;
            }
            set { _headers = value; }
        }

        /// <summary>
        /// Content of the entity (not decoded)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Collection of child MimeEntities
        /// </summary>
        public MimeEntityCollection Entities
        {
            get
            {
                if (_entities == null)
                    _entities = new MimeEntityCollection();
                return _entities;
            }
            set { _entities = value; }
        }

        /// <summary>
        /// Gets a value indicating whether there are any items in the Entities collection
        /// </summary>
        public bool HasEntities
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the IMimeParser that is used to parse the MIME data
        /// </summary>
        public IMimeParser MimeParser
        {
            get { return _mimeParser; }
        }

        /// <summary>
        /// Gets or sets the MIME data
        /// </summary>
        public virtual string GetMimeData()
        {
            var mimeData = new StringBuilder();
            mimeData.AppendLine(GetHeaders());
            mimeData.AppendLine();
            mimeData.Append(Content);
            return mimeData.ToString();
        }

        /// <summary>
        /// Sets the MIME data. This changes the properties of the IMimeEntity
        /// </summary>
        public virtual void SetMimeData(string mimeData)
        {
            var r = new Regex(@"(\r\n\s*){2}");
            var m = r.Match(mimeData);
            if (m.Index <= 1)
            {
                Content = mimeData.Trim();
            }
            else
            {
                Content = mimeData.Substring(m.Index + m.Length).Trim();

                var headers = mimeData.Substring(0, m.Index);
                SetHeaders(headers);
            }
        }

        /// <summary>
        /// Get the value of a MIME header
        /// </summary>
        /// <param name="headerName">The name of the header (e.g. "Content-Type")</param>
        /// <returns>A String containing the value of the header or an empty string if the header is not present</returns>
        public string GetHeaderValue(string headerName)
        {
            var header = Headers.FirstOrDefault(h => h.Key == headerName).Value;
            return header ?? String.Empty;
        }

        #endregion

        /// <summary>
        /// Converts a string containing header data and adds items to Headers property
        /// </summary>
        /// <param name="headerData"></param>
        protected void SetHeaders(string headerData)
        {
            var r = new Regex(@"(?<Name>.+):\s+(?<Value>[^\r\n]*(\r\n[\t ]+[^\r\n]+)*)");
            var mc = r.Matches(headerData);
            foreach (Match match in mc)
            {
                Headers[match.Groups["Name"].Value] = match.Groups["Value"].Value;
            }
        }

        /// <summary>
        /// Gets the Headers collection as a string
        /// </summary>
        /// <returns>A string containing the MIME header data</returns>
        protected string GetHeaders()
        {
            var headers = new StringBuilder();
            foreach (var header in Headers)
            {
                headers.AppendLine(String.Format("{0}: {1}", header.Key, header.Value));
            }
            return headers.ToString().TrimEnd();
        }
    }
}