using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLife.Net.Mime
{
    /// <summary>
    /// MIME Entity which holds other entities (e.g. "multipart/mixed", "multipart/alternative") 
    /// </summary>
    public class MultipartMimeEntity : MimeEntityBase, IMimeEntity
    {
        private string _boundary;

        /// <summary>
        /// Initializes a new instance of the MultipartMimeEntity class with empty MIME data
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        public MultipartMimeEntity(IMimeParser mimeParser) : this(mimeParser, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipartMimeEntity class
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        /// <param name="mimeData">A string containing the MIME data for the MIME entity</param>
        public MultipartMimeEntity(IMimeParser mimeParser, string mimeData)
            : base(mimeParser, mimeData)
        {
        }

        /// <summary>
        /// Gets the boundary string for the multipart entity
        /// </summary>
        public string Boundary
        {
            get
            {
                if (String.IsNullOrEmpty(_boundary))
                    _boundary = "---=_NextPart_" + new Guid();
                return _boundary;
            }
            private set { _boundary = value; }
        }

        #region IMimeEntity Members

        /// <summary>
        /// Gets the MIME data.
        /// </summary>
        public override string GetMimeData()
        {
            var mimeData = new StringBuilder();
            var headers = GetHeaders();
            if (!String.IsNullOrEmpty(headers))
            {
                mimeData.AppendLine(headers);
                mimeData.AppendLine();
            }
            if (!String.IsNullOrEmpty(Content))
            {
                mimeData.AppendLine(Content);
                mimeData.AppendLine();
            }
            foreach (var entity in Entities)
            {
                mimeData.AppendLine("--" + Boundary);
                mimeData.AppendLine(entity.GetMimeData());
                mimeData.AppendLine();
            }
            if (HasEntities)
                mimeData.Append("--" + Boundary);
            return mimeData.ToString();
        }

        /// <summary>
        /// Sets the MIME data. This changes the properties of the IMimeEntity
        /// </summary>
        public override void SetMimeData(string mimeData)
        {
            if (string.IsNullOrEmpty(mimeData))
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null or empty");
            }

            var headersAndContent = String.Empty;
            Regex r;
            Boundary = _mimeParser.ParseBoundary(mimeData);
            if (!String.IsNullOrEmpty(Boundary))
            {
                r = new Regex(@"--" + Boundary, RegexOptions.IgnoreCase);
                var entities = r.Split(mimeData);
                headersAndContent = entities[0].Trim();

                // Entities
                Entities.Clear();
                for (var i = 1; i < entities.Length; i++)
                {
                    if (entities[i].Trim().Length > 0)
                    {
                        var mimeEntity = MimeEntity.GetInstance(_mimeParser, entities[i].Trim());
                        Entities.Add(mimeEntity);
                    }
                }
            }

            // Headers & Content
            r = new Regex(@"(\r\n\s*){2}");
            var m = r.Match(headersAndContent);
            if (m.Index > 1)
            {
                SetHeaders(headersAndContent.Substring(0, m.Index));
                Content = headersAndContent.Substring(m.Index + m.Length);
            }
            else
            {
                SetHeaders(headersAndContent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are any items in the Entities collection
        /// </summary>
        public new bool HasEntities
        {
            get { return Entities.Count > 0; }
        }

        #endregion
    }
}