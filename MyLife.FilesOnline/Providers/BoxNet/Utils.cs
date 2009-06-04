using System.Text;
using System.Xml;

namespace MyLife.FilesOnline.Providers.BoxNet
{
    internal static class Utils
    {
        public static bool GetAttributeValueBool(XmlNode node, string attribute)
        {
            return
                ((((node != null) && (node.Attributes[attribute] != null)) &&
                  (node.Attributes[attribute].InnerText.Length > 0)) &&
                 XmlConvert.ToBoolean(node.Attributes[attribute].InnerText));
        }

        public static int GetAttributeValueInt(XmlNode node, string attribute)
        {
            if (((node != null) && (node.Attributes[attribute] != null)) &&
                (node.Attributes[attribute].InnerText.Length > 0))
            {
                return XmlConvert.ToInt32(node.Attributes[attribute].InnerText);
            }
            return -1;
        }

        public static long GetAttributeValueLong(XmlNode node, string attribute)
        {
            if (((node != null) && (node.Attributes[attribute] != null)) &&
                (node.Attributes[attribute].InnerText.Length > 0))
            {
                return XmlConvert.ToInt64(node.Attributes[attribute].InnerText);
            }
            return -1L;
        }

        public static string GetAttributeValueString(XmlNode node, string attribute)
        {
            if (((node != null) && (node.Attributes[attribute] != null)) &&
                (node.Attributes[attribute].InnerText.Length > 0))
            {
                return node.Attributes[attribute].InnerText;
            }
            return string.Empty;
        }

        public static string GetFileSharedId(string sharedLink)
        {
            if ((sharedLink != null) && (sharedLink.LastIndexOf("/") > 1))
            {
                return sharedLink.Substring(sharedLink.LastIndexOf("/") + 1);
            }
            return "";
        }

        public static bool GetNodeTextBool(XmlNode node)
        {
            return
                ((((node != null) && (node.InnerText != null)) && (node.InnerText.Length > 0)) &&
                 XmlConvert.ToBoolean(node.InnerText));
        }

        public static int GetNodeTextInt(XmlNode node)
        {
            if (((node != null) && (node.InnerText != null)) && (node.InnerText.Length > 0))
            {
                return XmlConvert.ToInt32(node.InnerText);
            }
            return -1;
        }

        public static long GetNodeTextLong(XmlNode node)
        {
            if (((node != null) && (node.InnerText != null)) && (node.InnerText.Length > 0))
            {
                return XmlConvert.ToInt64(node.InnerText);
            }
            return -1L;
        }

        public static string GetNodeTextString(XmlNode node)
        {
            if (((node != null) && (node.InnerText != null)) && (node.InnerText.Length > 0))
            {
                return node.InnerText.Trim();
            }
            return string.Empty;
        }

        public static byte[] StrToByteArray(string str)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string GetFileExtension(string fileName)
        {
            if ((fileName != null) && (fileName.LastIndexOf(".") > 1))
            {
                return fileName.Substring(fileName.LastIndexOf(".") + 1);
            }
            return "";
        }
    }
}