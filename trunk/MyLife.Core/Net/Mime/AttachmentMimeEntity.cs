using System.IO;

namespace MyLife.Net.Mime
{
    public class AttachmentMimeEntity : MimeEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the AttachmentMimeEntity class
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is used for parsing the MIME data</param>
        /// <param name="mimeData">A string containing the MIME data of the attachment</param>
        public AttachmentMimeEntity(IMimeParser mimeParser, string mimeData)
            : base(mimeParser, mimeData)
        {
        }

        /// <summary>
        /// Returns the decoded content of the attachment as a Stream
        /// </summary>
        /// <returns>A Stream containing the decoded attachment</returns>
        public Stream GetContent()
        {
            return MimeEncoding.Base64.Decode(Content.Trim());
        }
    }
}