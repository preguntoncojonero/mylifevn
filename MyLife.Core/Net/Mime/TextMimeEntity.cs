using System.Diagnostics;

namespace MyLife.Net.Mime
{
    /// <summary>
    /// MIME Entity for text content
    /// </summary>
    public class TextMimeEntity : MimeEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the TextMimeEntity class
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        /// <param name="mimeData">A string containing the MIME data for the MIME entity</param>
        public TextMimeEntity(IMimeParser mimeParser, string mimeData)
            : base(mimeParser, mimeData)
        {
            Charset = _mimeParser.ParseCharset(mimeData);
            Debug.WriteLine(ContentType);
            if (ContentType.Contains("\r\n"))
                ContentType = ContentType.Substring(0, ContentType.IndexOf('\r')).TrimEnd(';');
        }

        /// <summary>
        /// Sets or gets the charset of this entity's text
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// Returns the text content
        /// </summary>
        /// <returns>A String containing the text of the TextMimeEntity</returns>
        public string GetContent()
        {
            return ContentTransferEncoding == Mime.ContentTransferEncoding.QuotedPrintable ? MimeEncoding.QuotedPrintable.Decode(Content.Trim()) : Content.Trim();
        }
    }
}