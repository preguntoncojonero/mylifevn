using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using MyLife.Web.Links;

namespace MyLife.Web
{
    public static class Extensions
    {
        #region HtmlHelper

        public static string Navigation(this HtmlHelper helper, string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return null;
            }

            var links = Link.GetLinksOfUser(user);

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var writer = new HtmlTextWriter(textWriter);
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (var link in links)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, link.Url);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(string.Format("<span>{0}</span>", link.Title));
                writer.RenderEndTag(); // End a

                writer.RenderEndTag(); // End li
            }

            writer.RenderEndTag(); // End ul
            return sb.ToString();
        }

        public static string News(this HtmlHelper helper, IList<News.News> newses)
        {
            if (newses == null || newses.Count == 0)
            {
                return null;
            }
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var news in newses)
            {
                sb.AppendFormat("<li>{0}: <a href=\"/news/{2}\">{1}</a></li>", news.CreatedDate.ToString("d/M"), news.Title, news.Slug);
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        public static string Paging(this HtmlHelper helper, int indexOfPage, int sizeOfPage, int totalItems, string next, string previous)
        {
            var totalPages = Utils.CalcTotalPages(totalItems, sizeOfPage);
            
            var sb = new StringBuilder();
            if (totalPages > 1)
            {
                sb.Append("<div class=\"paging\">");

                if (indexOfPage > 0)
                {
                    sb.Append(GeneratePageLink(helper, previous, indexOfPage, "previous"));
                }

                if (indexOfPage < totalPages - 1)
                {
                    sb.Append(GeneratePageLink(helper, next, indexOfPage + 2, "next"));
                }

                sb.Append("</div>");
            }
            return sb.ToString();
        }

        private static string GeneratePageLink(HtmlHelper helper, string linkText, int indexOfPage, string @class)
        {
            var linkValueDictionary = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            linkValueDictionary["indexOfPage"] = indexOfPage;
            var virtualPathData = RouteTable.Routes.GetVirtualPath(helper.ViewContext.RequestContext, linkValueDictionary);
            if (virtualPathData != null)
            {
                var linkFormat = "<a href=\"{0}\" class=\"{2}\">{1}</a>";
                return string.Format(linkFormat, virtualPathData.VirtualPath, linkText, @class);
            }
            return null;
        }

        #endregion

        #region String

        public static string HtmlEncode(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : HttpUtility.HtmlEncode(str);
        }

        public static string UrlEncode(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : HttpUtility.UrlEncode(str);
        }

        public static IList<string> ToList(this string source, string separate)
        {
            var retval = new List<string>();
            if (string.IsNullOrEmpty(source))
            {
                return retval;
            }

            retval.AddRange(source.Split(new[] { separate }, StringSplitOptions.None));
            return retval;
        }

        #endregion
    }
}