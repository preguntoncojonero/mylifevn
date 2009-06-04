using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web.UI;

namespace MyLife.Web
{
    public static class HtmlTextRender
    {
        public static string RenderCheckBox(object source, string name, string dataMemberText, string dataMemberValue)
        {
            if (source == null || string.IsNullOrEmpty(dataMemberText) || string.IsNullOrEmpty(dataMemberValue))
            {
                return null;
            }

            if (!(source is IEnumerable))
            {
                throw new InvalidOperationException();
            }

            var list = source as IEnumerable;

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var writer = new HtmlTextWriter(textWriter);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "checkboxlist");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (var obj in list)
            {
                var text = Utils.GetPropertyValue(obj, dataMemberText);
                var value = Utils.GetPropertyValue(obj, dataMemberValue);

                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute("dataMemberValue", dataMemberValue);
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "checkbox");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, value.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();

                writer.Write(text);

                writer.RenderEndTag(); // End li
            }

            writer.RenderEndTag(); // End ul

            return sb.ToString();
        }
    }
}