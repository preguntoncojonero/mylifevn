using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MyLife.Net.Mail
{
    public class XmlMailTemplate : IMailTemplate
    {
        public XmlMailTemplate(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; set; }

        #region IMailTemplate Members

        public string Subject { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }

        public void Process()
        {
            var xmlDocument = XDocument.Load(Uri);

            var query = from element in xmlDocument.Descendants("Subject") select element;
            foreach (var element in query)
            {
                Subject = element.Value;
            }

            query = from element in xmlDocument.Descendants("Body") select element;
            foreach (var element in query)
            {
                Body = element.Value;
            }

            if (Data != null)
            {
                var regex = new Regex("##[a-zA-Z0-9_]+##", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var data = new Hashtable();
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Data))
                {
                    data.Add(descriptor.Name.ToLowerInvariant(), descriptor.GetValue(Data));
                }
                Body = regex.Replace(Body,
                                     new MatchEvaluator(
                                         math => data[math.Value.Trim(new[] {'#'}).ToLowerInvariant()].ToString()));
            }
        }

        #endregion
    }
}