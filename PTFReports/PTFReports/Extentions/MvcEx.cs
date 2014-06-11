/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace PTF.Reports
{
    public static class MvcEx
    {
        public static string Table(this HtmlHelper helper, string name, IList items, IDictionary<string, object> attributes = null)
        {
            if (items == null || items.Count == 0 || string.IsNullOrEmpty(name))
                return string.Empty;
            return BuildTable(name, items, attributes);
        }

        private static string BuildTable(string name, IList items, IDictionary<string, object> attributes)
        {
            StringBuilder sb = new StringBuilder();
            BuildTableHeader(sb, items[0].GetType());
            foreach (var item in items)
                BuildTableRow(sb, item);

            TagBuilder builder = new TagBuilder("table");

            if (attributes != null)
                builder.MergeAttributes(attributes);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = sb.ToString();
            return builder.ToString(TagRenderMode.Normal);
        }

        private static void BuildTableRow(StringBuilder sb, object obj)
        {
            Type objType = obj.GetType();
            sb.AppendLine("\t<tr>");

            foreach (var property in objType.GetProperties())
                sb.AppendFormat("\t\t<td>{0}</td>\n", property.GetValue(obj, null));

            sb.AppendLine("\t</tr>");
        }

        private static void BuildTableHeader(StringBuilder sb, Type p)
        {
            sb.AppendLine("\t<tr>");
            foreach (var property in p.GetProperties())
                sb.AppendFormat("\t\t<th>{0}</th>\n", property.Name);
            sb.AppendLine("\t</tr>");
        }

        public static MvcHtmlString Replace(this MvcHtmlString str, string oldVlaue, string value)
        {
            var html = str.ToHtmlString();
            if (html != null)
            {
                var newhtml = html.Replace(oldVlaue, value);
                return new MvcHtmlString(newhtml);
            }
            return str;
        }

        public static MvcHtmlString ToSsl(this MvcHtmlString text, int port)
        {
            var linktag = text.ToHtmlString();
            var value = Regex.Matches(linktag, "(?!\")[^\"]+", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            var str = value.Count < 2 ? "" : value[1].Value;
            var url = HttpContext.Current.Request.Url;
            UriBuilder b = new UriBuilder("https", url.Host, port, str);
            return new MvcHtmlString(linktag.Replace(str, b.Uri.OriginalString));
        }

        public static MvcHtmlString WriteDebug(this MvcHtmlString str, string category = null)
        {
            Debug.WriteLine(str.ToHtmlString(), category);
            return str;
        }

        public static MvcHtmlString Retry(this HtmlHelper helper, string text, int count)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < count; i++)
                b.Append(text);
            return new MvcHtmlString(b.ToString());
        }

        /// <summary>
        /// dinamic d = new {x=4, y=6}.ToExpando();
        /// </summary>
        /// <param name="anonymousObject"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static string RenderPartialToString(this Controller controller, string partialViewName, object model = null)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View != null)
            {
                controller.ViewData.Model = model;
                
                var sb = new StringBuilder();

                using (var sw = new StringWriter(sb))
                {
                    using (var output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(
                            controller.ControllerContext, 
                            result.View,
                            controller.ViewData,
                            controller.TempData, output);
                        result.View.Render(viewContext, output);
                    }
                }
                return sb.ToString();
            }

            return String.Empty;
        }
    }
}
