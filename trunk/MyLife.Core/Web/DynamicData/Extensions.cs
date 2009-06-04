using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using xVal;
using xVal.Html;
using xVal.RuleProviders;

namespace MyLife.Web.DynamicData
{
    public static class Extensions
    {
        public static DynamicForm DynamicForm(this HtmlHelper helper, Type target)
        {
            return FormHelper(helper, target, FormMethod.Post, new RouteValueDictionary());
        }

        public static DynamicForm DynamicForm(this HtmlHelper helper, Type target, object htmlAttributes)
        {
            return FormHelper(helper, target, FormMethod.Post, new RouteValueDictionary(htmlAttributes));
        }

        private static DynamicForm FormHelper(this HtmlHelper helper, Type target, FormMethod method,
                                              IDictionary<string, object> htmlAttributes)
        {
            var builder = new TagBuilder("form");
            builder.MergeAttribute("action", "#");
            builder.MergeAttribute("method", GetFormMethodString(method), true);
            builder.MergeAttribute("id", string.Format("fAddOrEdit{0}", target.Name), true);
            builder.MergeAttributes(htmlAttributes, true);

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var writer = new HtmlTextWriter(textWriter);
            var properties =
                target.GetProperties().Where(item => Utils.GetCustomAttribute<DynamicFieldAttribute>(item) != null);
            var elementPrefix = target.Name.ToLowerInvariant();
            var hiddenProperties = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                var dynamicFieldAttribute = Utils.GetCustomAttribute<DynamicFieldAttribute>(property);
                if (dynamicFieldAttribute is DynamicIdentityFieldAttribute)
                {
                    hiddenProperties.Add(property);
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // Render label
                    var label = dynamicFieldAttribute.FriendlyName;
                    if (string.IsNullOrEmpty(label))
                    {
                        label = property.Name;
                    }
                    var requiredAttribute = Utils.GetCustomAttribute<RequiredAttribute>(property);
                    writer.AddAttribute(HtmlTextWriterAttribute.For,
                                        string.Format("{0}_{1}", elementPrefix, property.Name));
                    writer.RenderBeginTag(HtmlTextWriterTag.Label);
                    writer.Write(label);
                    if (requiredAttribute != null)
                    {
                        writer.Write("<span class=\"required\"> *</span>");
                    }
                    writer.RenderEndTag(); // End label

                    // Render input
                    if (dynamicFieldAttribute is DynamicTextFieldAttribute)
                    {
                        var dynamicTextFieldAttribute = dynamicFieldAttribute as DynamicTextFieldAttribute;
                        if (dynamicTextFieldAttribute.Multiple)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Id,
                                                string.Format("{0}_{1}", elementPrefix, property.Name));
                            writer.AddAttribute(HtmlTextWriterAttribute.Name,
                                                string.Format("{0}.{1}", elementPrefix, property.Name));
                            writer.RenderBeginTag(HtmlTextWriterTag.Textarea);
                            writer.RenderEndTag();
                        }
                        else
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Id,
                                                string.Format("{0}_{1}", elementPrefix, property.Name));
                            writer.AddAttribute(HtmlTextWriterAttribute.Name,
                                                string.Format("{0}.{1}", elementPrefix, property.Name));
                            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength,
                                                dynamicTextFieldAttribute.MaxLength > 0
                                                    ? dynamicTextFieldAttribute.MaxLength.ToString()
                                                    : "255");
                            writer.RenderBeginTag(HtmlTextWriterTag.Input);
                            writer.RenderEndTag();
                        }
                    }
                    else if (dynamicFieldAttribute is DynamicChoiceFieldAttribute)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "checkbox");
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, "true");
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();

                        writer.AddAttribute(HtmlTextWriterAttribute.Name,
                                            string.Format("{0}.{1}", elementPrefix, property.Name));
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                        writer.AddAttribute(HtmlTextWriterAttribute.Value, "false");
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag(); // End Div
                }
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "buttons");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach (var property in hiddenProperties)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id,
                                    string.Format("{0}_{1}", elementPrefix, property.Name));
                writer.AddAttribute(HtmlTextWriterAttribute.Name,
                                    string.Format("{0}.{1}", elementPrefix, property.Name));
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "button add");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, string.Format("btnAddOrEdit{0}", target.Name));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write("Thêm mới");
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Id, string.Format("btnCancelAddOrEdit{0}", target.Name));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "button cancel");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write("Hủy bỏ");
            writer.RenderEndTag();

            writer.RenderEndTag(); //End Div

            // Render client validation
            writer.Write(ClientSideValidation(target, elementPrefix));

            var response = helper.ViewContext.HttpContext.Response;
            response.Write(builder.ToString(TagRenderMode.StartTag));
            response.Write(sb.ToString());
            return new DynamicForm(response);
        }

        private static string ClientSideValidation(Type modelType, string elementPrefix)
        {
            var rules = ActiveRuleProviders.GetRulesForType(modelType);
            var builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            elementPrefix = (elementPrefix != null) ? string.Format("\"{0}\"", elementPrefix) : "null";
            builder.InnerHtml = string.Format("xVal.AttachValidator({0}, {1})", elementPrefix,
                                              ClientSideValidationRules(rules));
            return builder.ToString(TagRenderMode.Normal);
        }

        private static string ClientSideValidationRules(RuleSet rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }
            var formatter = new JsonValidationConfigFormatter();
            return formatter.FormatRules(rules);
        }

        private static string GetFormMethodString(FormMethod method)
        {
            switch (method)
            {
                case FormMethod.Get:
                    return "get";
                case FormMethod.Post:
                    return "post";
            }
            return "post";
        }
    }
}