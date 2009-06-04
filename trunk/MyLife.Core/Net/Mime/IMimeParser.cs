namespace MyLife.Net.Mime
{
    public interface IMimeParser
    {
        /// <summary>
        /// Parse all headers of the MIME message
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the headers of the MIME message</returns>
        string[] ParseHeaders(string mimeData);

        /// <summary>
        /// Extracts the value of a header from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <param name="headerName">A string containing the name of the header to parse</param>
        /// <returns>A String containing the value of the specific header of the MIME message</returns>
        string ParseHeaderValue(string mimeData, string headerName);

        /// <summary>
        /// Extracts the Content-Type from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the Content-Type of the MIME message</returns>
        string ParseContentType(string mimeData);

        /// <summary>
        /// Extracts the charset from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the charset of the MIME message</returns>
        string ParseCharset(string mimeData);

        /// <summary>
        /// Extracts the boundary string from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the boundary string of the MIME message</returns>
        string ParseBoundary(string mimeData);

        /// <summary>
        /// Extracts the Content part from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the Content part of the MIME message</returns>
        string ParseContent(string mimeData);
    }
}