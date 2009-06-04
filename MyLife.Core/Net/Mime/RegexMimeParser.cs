using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLife.Net.Mime
{
    /// <summary>
    /// MIME Parser whose parsing algorithm is based on Regular Expressions
    /// </summary>
    public class RegexMimeParser : IMimeParser
    {
        #region IMimeParser Members

        /// <summary>
        /// Parse all headers of the MIME message
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the headers of the MIME message</returns>
        public string[] ParseHeaders(string mimeData)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(@"(\r\n\s*){2,}");
            var headerPart = r.Split(mimeData)[0];

            r = new Regex(@"(?<Header>.+:\s+[^\r\n]*(\r\n[\t ]+[^\r\n]+)*)");
            var mc = r.Matches(headerPart);
            IList<string> headers = new List<string>();
            foreach (Match match in mc)
            {
                headers.Add(match.Groups["Header"].Value);
            }
            return headers.ToArray();
        }

        /// <summary>
        /// Extracts the value of a header from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <param name="headerName">A string containing the name of the header to parse</param>
        /// <returns>A String array containing the value of the specific header of the MIME message</returns>
        public string ParseHeaderValue(string mimeData, string headerName)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(headerName + @":\s+(?<Header>[^\r\n]*(\r\n[\t\x20]+[^\r\n]+)*)");
            var m = r.Match(mimeData);
            return m.Groups["Header"].Value.Trim();
        }

        /// <summary>
        /// Extracts the Content-Type from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the Content-Type of the MIME message</returns>
        public string ParseContentType(string mimeData)
        {
            var ContentType = ParseHeaderValue(mimeData, "Content-Type");
            if (ContentType.Contains(Environment.NewLine))
            {
                ContentType = ContentType.Substring(0, ContentType.IndexOf(Environment.NewLine)).Trim().TrimEnd(';');
            }

            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(@"Content-Type:[\s]+(?<ContentType>.*)");
            var m = r.Match(mimeData);
            return m.Groups["ContentType"].Value.Trim().Replace(";", "");
        }

        /// <summary>
        /// Extracts the charset from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the charset of the MIME message</returns>
        public string ParseCharset(string mimeData)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(@"[\s\t]+charset=\x22(?<Charset>.*)\x22");
            var m = r.Match(mimeData);
            return m.Groups["Charset"].Value;
        }

        /// <summary>
        /// Extracts the boundary string from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the boundary string of the MIME message</returns>
        public string ParseBoundary(string mimeData)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(@"[\s\t]+boundary=\x22(?<Boundary>.*)\x22");
            var m = r.Match(mimeData);
            return m.Groups["Boundary"].Value;
        }

        /// <summary>
        /// Extracts the Content part from MIME data
        /// </summary>
        /// <param name="mimeData">A string containing the text data of a MIME message</param>
        /// <returns>A String array containing the Content part of the MIME message</returns>
        public string ParseContent(string mimeData)
        {
            if (mimeData == null)
            {
                throw new ArgumentNullException("mimeData", "mimeData cannot be null");
            }

            var r = new Regex(Environment.NewLine + @"\s*" + Environment.NewLine);
            var m = r.Match(mimeData);
            return mimeData.Substring(m.Index + m.Length);
        }

        #endregion
    }
}