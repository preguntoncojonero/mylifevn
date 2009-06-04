using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Microsoft.Web.Mvc;

namespace MyLife.Web.Mvc
{
    public static class Extensions
    {
        #region Ordered & Unordered

        /// <summary>
        /// Creates a formatted list of items based on the passed in format
        /// </summary>
        public static string Ordered(this HtmlHelper helper, IEnumerable list)
        {
            return string.Format("<ol>{0}</ol>", ToFormattedList(list, "<li>{0}</li>"));
        }

        /// <summary>
        /// Creates a formatted list of items based on the passed in format
        /// </summary>
        public static string Unordered(this HtmlHelper helper, IEnumerable list)
        {
            return string.Format("<ul>{0}</ul>", ToFormattedList(list, "<li>{0}</li>"));
        }

        public static string Unordered<T>(this HtmlHelper helper, IEnumerable list, Func<T, string> predicate)
        {
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var obj in list)
            {
                sb.AppendFormat("<li>{0}</li>", predicate((T) obj));
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        public static string UnorderedLinks<T>(this HtmlHelper helper, object list, Func<T, string> textPredicate,
                                               Func<T, string> linkPredicate)
        {
            if (list == null || !(list is IEnumerable<T>))
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var obj in (IEnumerable<T>) list)
            {
                sb.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", textPredicate(obj), linkPredicate(obj));
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a formatted list of items based on the passed in format
        /// </summary>
        /// <param name="list">The item list</param>
        /// <param name="format">The single-place format string to use</param>
        private static string ToFormattedList(IEnumerable list, string format)
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendFormat(format, item);
            }

            return sb.ToString();
        }

        #endregion

        #region Link

        public static string Link(this HtmlHelper helper, string text, string link)
        {
            return Link(helper, text, link, null);
        }

        public static string Link(this HtmlHelper helper, string text, string link, object htmlAttributes)
        {
            if (string.IsNullOrEmpty(link))
            {
                return text;
            }

            var tagBuilder = new TagBuilder("a");
            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tagBuilder.MergeAttribute("href", link);
            tagBuilder.InnerHtml = text;
            return tagBuilder.ToString(TagRenderMode.Normal);
        }

        /// <summary>
        /// Creates a client-resolvable Url based on the passed-in value
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="virtualUrl">Relative Url to evaluate. Use ~/ to resolve from the root</param>
        /// <returns></returns>
        public static string ResolveUrl(this HtmlHelper helper, string virtualUrl)
        {
            var ctx = helper.ViewContext.HttpContext;
            var result = virtualUrl;

            if (virtualUrl.StartsWith("~/"))
            {
                virtualUrl = virtualUrl.Remove(0, 2);

                //get the site root
                var siteRoot = ctx.Request.ApplicationPath;

                if (siteRoot == string.Empty)
                    siteRoot = "/";

                result = siteRoot + virtualUrl;
            }
            return result;
        }

        public static string LinkTag(this HtmlHelper helper, string rel, string type, string title, string href)
        {
            if (href.Contains("~/"))
            {
                href = ResolveUrl(helper, href);
            }
            return string.Format("<link rel=\"{0}\" type=\"{1}\" title=\"{2}\" href=\"{3}\" />", rel, type, title, href);
        }

        #endregion

        #region String

        public static string RemoveHtmlTags(this string str)
        {
            return Utils.RemoveHtmlTags(str);
        }

        public static string Ellipsis(this string str, int length)
        {
            return str.Length <= length ? str : str.Substring(0, length).Trim() + "...";
        }

        #endregion

        #region List

        public static string FormatAsLinks<T>(this IEnumerable<T> list, Func<T, string> textPredicate,
                                              Func<T, string> linkPredicate, string separate)
        {
            if (list == null || !list.Any())
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var obj in list)
            {
                sb.AppendFormat("<a href=\"{1}\">{0}</a>", textPredicate(obj), linkPredicate(obj));
                sb.Append(separate);
            }
            return sb.ToString(0, sb.Length - separate.Length);
        }

        #endregion

        #region HtmlHelper

        public static void RenderPartialIfNotNull(this HtmlHelper helper, string view, object model)
        {
            if (model != null)
            {
                helper.RenderPartial(view, model);
            }
        }

        public static void RenderPartialIf(this HtmlHelper helper, bool condition, string view, object model)
        {
            if (condition)
            {
                helper.RenderPartial(view, model);
            }
        }

        public static string RenderIf(this HtmlHelper helper, bool condition, object obj)
        {
            return condition ? obj.ToString() : null;
        }

        public static ConditionalHtmlRender If(this HtmlHelper helper, bool condition, Func<HtmlHelper, string> ifAction)
        {
            return new ConditionalHtmlRender(helper, condition, ifAction);
        }

        public static ConditionRenderPartial WriteIf(this HtmlHelper helper, bool condition, Action<HtmlHelper> ifAction)
        {
            return new ConditionRenderPartial(helper, condition, ifAction);
        }

        public static string Div(this HtmlHelper helper, string innerHtml)
        {
            return Div(helper, innerHtml, null);
        }

        public static string Div(this HtmlHelper helper, string innerHtml, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tagBuilder.InnerHtml = innerHtml;
            return tagBuilder.ToString(TagRenderMode.Normal);
        }

        public static string Avatar(this HtmlHelper helper, string user, string email)
        {
            return user ?? email;
        }

        #region Nested type: ConditionalHtmlRender

        public class ConditionalHtmlRender
        {
            private readonly Dictionary<bool, Func<HtmlHelper, string>> elseActions =
                new Dictionary<bool, Func<HtmlHelper, string>>();

            private readonly HtmlHelper helper;
            private readonly Func<HtmlHelper, string> ifAction;
            private readonly bool ifCondition;

            public ConditionalHtmlRender(HtmlHelper helper, bool ifCondition, Func<HtmlHelper, string> ifAction)
            {
                this.helper = helper;
                this.ifCondition = ifCondition;
                this.ifAction = ifAction;
            }

            public ConditionalHtmlRender Else(Func<HtmlHelper, string> renderAction)
            {
                return ElseIf(true, renderAction);
            }

            public ConditionalHtmlRender ElseIf(bool condition, Func<HtmlHelper, string> renderAction)
            {
                elseActions.Add(condition, renderAction);
                return this;
            }

            public override string ToString()
            {
                if (ifCondition)
                {
                    return ifAction.Invoke(helper);
                }

                foreach (var elseAction in elseActions)
                {
                    if (elseAction.Key)
                    {
                        return elseAction.Value.Invoke(helper);
                    }
                }

                return string.Empty;
            }
        }

        #endregion

        #region Nested type: ConditionRenderPartial

        public class ConditionRenderPartial
        {
            private readonly Dictionary<bool, Action<HtmlHelper>> elseActions =
                new Dictionary<bool, Action<HtmlHelper>>();

            private readonly HtmlHelper helper;
            private readonly Action<HtmlHelper> ifAction;
            private readonly bool ifCondition;

            public ConditionRenderPartial(HtmlHelper helper, bool ifCondition, Action<HtmlHelper> ifAction)
            {
                this.helper = helper;
                this.ifCondition = ifCondition;
                this.ifAction = ifAction;
            }

            public ConditionRenderPartial Else(Action<HtmlHelper> renderAction)
            {
                return ElseIf(true, renderAction);
            }

            public ConditionRenderPartial ElseIf(bool condition, Action<HtmlHelper> renderAction)
            {
                elseActions.Add(condition, renderAction);
                return this;
            }

            public void Render()
            {
                if (ifCondition)
                {
                    ifAction.Invoke(helper);
                }

                foreach (var elseAction in elseActions)
                {
                    if (elseAction.Key)
                    {
                        elseAction.Value.Invoke(helper);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Avatar & Gravatar

        public static string Avatar(this HtmlHelper helper, string user)
        {
            return string.Format("<img src=\"/avatar/{0}\" alt=\"{0}\" />", user);
        }

        public static string Gravatar(this HtmlHelper helper, string email)
        {
            return Utils.Gravatar(email, 80);
        }

        public static string Gravatar(this HtmlHelper helper, string email, int size)
        {
            return Utils.Gravatar(email, size);
        }

        #endregion

        #region Anti Forgery Extensions

        public static string MyLifeAntiForgeryToken(this HtmlHelper helper)
        {
            return MyLifeAntiForgeryToken(helper, null);
        }

        public static string MyLifeAntiForgeryToken(this HtmlHelper helper, string salt)
        {
            if (helper.ViewContext.HttpContext.Request.HttpMethod == "GET")
            {
                helper.ViewContext.HttpContext.Request.Cookies.Clear();
            }

            return AntiForgeryExtensions.AntiForgeryToken(helper, salt);
        }

        #endregion

        #region Checkboxlist

        public static string CheckBoxList<T>(this HtmlHelper htmlHelper, string name, IEnumerable<T> source,
                                             IEnumerable<T> selected, Func<T, object> textSelector,
                                             Func<T, object> valueSelector, int column)
        {
            if (source == null)
            {
                return null;
            }
            var id = name.Replace(".", "_");
            var texts = new List<object>(source.Select(textSelector));
            var values = new List<object>(source.Select(valueSelector));
            var selectedValues = selected == null
                                     ? new List<object>()
                                     : new List<object>(selected.Select(valueSelector));
            var sb = new StringBuilder();
            sb.Append("<ul class=\"checkboxlist\">");
            var index = 0;
            foreach (var value in values)
            {
                sb.Append("<li>");
                var check = selectedValues.Contains(value);
                sb.AppendFormat("<input id=\"{4}_{2}\" name=\"{0}\" type=\"checkbox\" value=\"{1}\" {3} />{5}", name,
                                value,
                                index, check ? "checked=\"checked\"" : "", id, texts[index]);
                sb.Append("</li>");
                index++;
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        #endregion

        #region Page Navigator

        public static string PageNavigator(this HtmlHelper helper, string baseUrl, int indexOfPage, int totalPage)
        {
            return PageNavigator(helper, baseUrl, indexOfPage, totalPage, "Next", "Previous");
        }

        public static string PageNavigator(this HtmlHelper helper, string baseUrl, int indexOfPage, int totalPage,
                                           string next, string previous)
        {
            if (totalPage <= 1)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.Append("<div class=\"page-navigator\">");

            sb.AppendFormat("<span class=\"pages alignright\">Page {0} of {1}</span>", indexOfPage, totalPage);

            sb.Append("<span class=\"alignleft\">");

            if (indexOfPage > 1)
            {
                sb.AppendFormat("<a class=\"page\" href=\"{1}/{2}\">{0}</a>", previous, baseUrl, indexOfPage - 1);
            }

            for (var i = 1; i < indexOfPage; i++)
            {
                sb.AppendFormat("<a class=\"page\" href=\"{1}/{2}\">{0}</a>", i, baseUrl, i);
            }

            sb.AppendFormat("<span class=\"current\">{0}</span>", indexOfPage);

            for (var i = indexOfPage + 1; i <= totalPage; i++)
            {
                sb.AppendFormat("<a class=\"page\" href=\"{1}/{2}\">{0}</a>", i, baseUrl, i);
            }

            if (indexOfPage < totalPage)
            {
                sb.AppendFormat("<a class=\"page\" href=\"{1}/{2}\">{0}</a>", next, baseUrl, indexOfPage + 1);
            }

            sb.Append("</span>");

            sb.Append("</div>");
            return sb.ToString();
        }

        #endregion

        #region Google

        public static string GoogleAnalytics(this HtmlHelper helper)
        {
            return
                "<script type=\"text/javascript\" src=\"http://www.google-analytics.com/ga.js\"></script><script type=\"text/javascript\">try {var pageTracker = _gat._getTracker(\"UA-7365222-1\");pageTracker._trackPageview();} catch (err) {}</script>";
        }

        #endregion

        #region Html Meta

        public static string DescriptionMeta(this HtmlHelper htmlHelper, string description)
        {
            return string.IsNullOrEmpty(description)
                       ? string.Empty
                       : string.Format("<meta name=\"description\" content=\"{0}\">", description);
        }

        public static string KeywordsMeta(this HtmlHelper htmlHelper, string keywords)
        {
            return string.IsNullOrEmpty(keywords)
                       ? string.Empty
                       : string.Format("<meta name=\"keywords\" content=\"{0}\">", keywords);
        }

        #endregion

        #region DropDownList

        public static string DropDownList<T>(this HtmlHelper htmlHelper, string name, object source,
                                             Func<T, object> textSelector, Func<T, object> valueSelector)
        {
            if (source == null || !(source is IEnumerable<T>))
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendFormat("<select name=\"{0}\" id=\"{1}\">", name, name.Replace(".", "_"));
            foreach (var item in source as IEnumerable<T>)
            {
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", valueSelector(item), textSelector(item));
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        #endregion
    }
}