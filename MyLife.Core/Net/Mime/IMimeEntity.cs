using System.Collections.Generic;

namespace MyLife.Net.Mime
{
    public interface IMimeEntity
    {
        /// <summary>
        /// Gets or sets the Content-Type
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Transfer-Encoding
        /// </summary>
        string ContentTransferEncoding { get; set; }

        /// <summary>
        /// Gets or sets the Content-Disposition
        /// </summary>
        string ContentDisposition { get; set; }

        /// <summary>
        /// Collection of MIME headers
        /// </summary>
        IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Content of the entity (not decoded)
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Collection of child MimeEntities
        /// </summary>
        MimeEntityCollection Entities { get; set; }

        /// <summary>
        /// Gets a value indicating whether there are any items in the Entities collection
        /// </summary>
        bool HasEntities { get; }

        /// <summary>
        /// Gets the IMimeParser that is used to parse the MIME data
        /// </summary>
        IMimeParser MimeParser { get; }

        /// <summary>
        /// Gets the MIME data.
        /// </summary>
        string GetMimeData();

        /// <summary>
        /// Sets the MIME data. This changes the properties of the IMimeEntity
        /// </summary>
        void SetMimeData(string mimeData);

        /// <summary>
        /// Get the value of a MIME header
        /// </summary>
        /// <param name="headerName">The name of the header (e.g. "Content-Type")</param>
        /// <returns>A String containing the value of the header or an empty string if the header is not present</returns>
        string GetHeaderValue(string headerName);
    }
}