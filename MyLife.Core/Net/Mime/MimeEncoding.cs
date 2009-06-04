using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLife.Net.Mime
{
    /// <summary>
    /// Methods for encoding and decoding MIME content
    /// </summary>
    public static class MimeEncoding
    {
        #region Nested type: Base64

        /// <summary>
        /// Encode and decode data with base64 encoding
        /// </summary>
        public static class Base64
        {
            /// <summary>
            /// Encode data with base64 encoding
            /// </summary>
            /// <param name="obj">Object to encode</param>
            /// <returns>Base64 encoded string</returns>
            public static string Encode(object obj)
            {
                byte[] byteObj;
                if (obj is String)
                {
                    byteObj = Encoding.UTF8.GetBytes((obj as String));
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(memoryStream, obj);
                        byteObj = memoryStream.GetBuffer();
                    }
                }
                return Convert.ToBase64String(byteObj, Base64FormattingOptions.InsertLineBreaks);
            }

            /// <summary>
            /// Decode base64 encoded content
            /// </summary>
            /// <param name="s">Base64 encoded string</param>
            /// <returns>Decoded object</returns>
            public static Stream Decode(string s)
            {
                return Decode(s, "utf-8");
            }

            /// <summary>
            /// Decode base64 encoded content
            /// </summary>
            /// <param name="s">Base64 encoded string</param>
            /// <param name="charset"></param>
            /// <returns>Decoded object</returns>
            public static Stream Decode(string s, string charset)
            {
                MemoryStream stream;
                try
                {
                    stream = new MemoryStream(Convert.FromBase64String(s));
                }
                catch (Exception)
                {
                    stream = new MemoryStream(Encoding.GetEncoding(charset).GetBytes(s));
                }
                return stream;
            }
        }

        #endregion

        #region Nested type: QuotedPrintable

        /// <summary>
        /// Encode and decode data with quoted-printable
        /// </summary>
        public static class QuotedPrintable
        {
            /// <summary>
            /// Decode quoted-printable encoded content
            /// </summary>
            /// <param name="s">Quoted-printable encoded string</param>
            /// <returns>Decoded string</returns>
            public static string Decode(string s)
            {
                var decodedText = s;
                decodedText = decodedText.Replace("=" + Environment.NewLine, "");
                var hexRegex = new Regex(@"(\=([0-9A-F][0-9A-F]))", RegexOptions.IgnoreCase);
                decodedText = hexRegex.Replace(decodedText, new MatchEvaluator(HexDecodeMatchEvaluator));
                return decodedText;
            }

            private static string HexDecodeMatchEvaluator(Match m)
            {
                var dec = Convert.ToInt32(m.Groups[2].Value, 16);
                var character = Convert.ToChar(dec);
                return character.ToString();
            }

            /// <summary>
            /// Encode string with quoted-printable encoding
            /// </summary>
            /// <param name="s">String to encode</param>
            /// <returns>Quoted-printable encoded string</returns>
            public static string Encode(string s)
            {
                var r = new Regex(@"[^\r\n\x20-\x7E]|\x3D|([\ \t](?=[\r\n]))");
                var encodedText = r.Replace(s, new MatchEvaluator(HexEncodeMatchEvaluator));
                encodedText = LimitLineLength(encodedText, 76);
                return encodedText;
            }

            private static string HexEncodeMatchEvaluator(Match m)
            {
                var character = m.Value[0];
                int dec = character;
                return "=" + dec.ToString("X");
            }

            /// <summary>
            /// Limits the line length of a string and inserts linebreaks after the given maximum line length
            /// </summary>
            /// <param name="s">A String which is to be processed</param>
            /// <param name="maxLineLength">An Int32 declaring the maximum line length</param>
            /// <returns></returns>
            private static string LimitLineLength(string s, int maxLineLength)
            {
                var lines = s.Split(new[] {"\r\n"}, StringSplitOptions.None);
                var formattedText = new StringBuilder();
                foreach (var line in lines)
                {
                    if (line.Length > maxLineLength)
                    {
                        var currentLine = line;
                        while (currentLine.Length > maxLineLength)
                        {
                            var splitPosition = maxLineLength;
                            // Do not split encoded characters (e.g. =3D)
                            if (currentLine.Substring(splitPosition - 3, splitPosition).Contains("="))
                            {
                                splitPosition = currentLine.LastIndexOf('=', maxLineLength);
                            }
                            formattedText.AppendLine(currentLine.Substring(0, splitPosition) + "=");
                            currentLine = currentLine.Substring(splitPosition);
                        }
                    }
                    else
                    {
                        formattedText.AppendLine(line);
                    }
                }
                // Remove line break at the end of the text
                var formatted = formattedText.ToString();
                formatted = formatted.Remove(formatted.Length - 2);
                return formatted;
            }
        }

        #endregion
    }
}