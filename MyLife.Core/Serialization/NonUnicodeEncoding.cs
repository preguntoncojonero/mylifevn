using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLife.Serialization
{
    public class NonUnicodeEncoding : ASCIIEncoding
    {
        private readonly string[,] array;
        private readonly Byte[] encodedBytes;

        public NonUnicodeEncoding()
        {
            array = new string[14,0x12];
            var text = "aAeEoOuUiIdDyY";
            var Lower_a = "\x00e1\x00e0ạả\x00e3\x00e2ấầậẩẫăắằặẳẵ";
            var Upper_A = "\x00c1\x00c0ẠẢ\x00c3\x00c2ẤẦẬẨẪĂẮẰẶẲẴ";
            var Lower_e = "\x00e9\x00e8ẹẻẽ\x00eaếềệểễeeeeee";
            var Upper_E = "\x00c9\x00c8ẸẺẼ\x00caẾỀỆỂỄEEEEEE";
            var Lower_o = "\x00f3\x00f2ọỏ\x00f5\x00f4ốồộổỗơớờợởỡ";
            var Upper_O = "\x00d3\x00d2ỌỎ\x00d5\x00d4ỐỒỘỔỖƠỚỜỢỞỠ";
            var Lower_u = "\x00fa\x00f9ụủũưứừựửữuuuuuu";
            var Upper_U = "\x00da\x00d9ỤỦŨƯỨỪỰỬỮUUUUUU";
            var Lower_i = "\x00ed\x00ecịỉĩiiiiiiiiiiii";
            var Upper_I = "\x00cd\x00ccỊỈĨIIIIIIIIIIII";
            var Lower_d = "đdddddddddddddddd";
            var Upper_D = "ĐDDDDDDDDDDDDDDDD";
            var Lower_y = "\x00fdỳỵỷỹyyyyyyyyyyyy";
            var Upper_Y = "\x00ddỲỴỶỸYYYYYYYYYYYY";
            byte i = 0;
            do
            {
                array[i, 0] = Mid(text, i + 1, 1);
                i = (byte) (i + 1);
            } while (i <= 13);
            byte j = 1;
            do
            {
                i = 1;
                do
                {
                    array[0, i] = Mid(Lower_a, i, 1);
                    array[1, i] = Mid(Upper_A, i, 1);
                    array[2, i] = Mid(Lower_e, i, 1);
                    array[3, i] = Mid(Upper_E, i, 1);
                    array[4, i] = Mid(Lower_o, i, 1);
                    array[5, i] = Mid(Upper_O, i, 1);
                    array[6, i] = Mid(Lower_u, i, 1);
                    array[7, i] = Mid(Upper_U, i, 1);
                    array[8, i] = Mid(Lower_i, i, 1);
                    array[9, i] = Mid(Upper_I, i, 1);
                    array[10, i] = Mid(Lower_d, i, 1);
                    array[11, i] = Mid(Upper_D, i, 1);
                    array[12, i] = Mid(Lower_y, i, 1);
                    array[13, i] = Mid(Upper_Y, i, 1);
                    i = (byte) (i + 1);
                } while (i <= 0x11);
                j = (byte) (j + 1);
            } while (j <= 0x11);
        }

        public NonUnicodeEncoding(string input)
            : this()
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            byte j = 0;
            do
            {
                byte i = 1;
                do
                {
                    input = input.Replace(array[j, i], array[j, 0]);
                    i = (byte) (i + 1);
                } while (i <= 0x11);
                j = (byte) (j + 1);
            } while (j <= 13);

            encodedBytes = GetBytess(input);
        }

        private byte[] GetBytess(string input)
        {
            return GetBytes(input);
        }

        private static string Mid(string str, int start, int length)
        {
            if (start <= 0)
            {
                throw new ArgumentException("start");
            }
            if (length < 0)
            {
                throw new ArgumentException("length");
            }
            if ((length == 0) || (str == null))
            {
                return "";
            }
            var l = str.Length;
            if (start > l)
            {
                return "";
            }
            if ((start + length) > l)
            {
                return str.Substring(start - 1);
            }
            return str.Substring(start - 1, length);
        }

        public override string ToString()
        {
            var retval = base.GetString(encodedBytes);
            if (string.IsNullOrEmpty(retval))
            {
                return retval;
            }

            return retval;
        }

        public string ToString(bool removeIlegalCharacters)
        {
            var retval = ToString();
            if (removeIlegalCharacters && !string.IsNullOrEmpty(retval))
            {
                return RemoveIlegalCharacters(retval);
            }
            return retval;
        }

        /// <summary>
        /// Strips all illegal characters from the specified title.
        /// </summary>
        public static string RemoveIlegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = Regex.Replace(text, Constants.Regulars.IlegalCharacters, " ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "\\s+", " ");
            return Regex.Replace(text, " ", "-");
        }
    }
}