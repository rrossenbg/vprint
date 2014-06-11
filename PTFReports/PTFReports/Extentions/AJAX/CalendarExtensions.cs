using System.Text;
using System.Web.Mvc;

namespace PTF.Reports.Ajax
{
    public static class CalendarExtensions
    {
        public static string RegisterCalendar(this AjaxHelper helper, string elementId)
        {
            var sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>");
            sb.Append("Sys.Application.add_init(function () { $create(AjaxControlToolkit.CalendarBehavior, null, null, null, $get('");
            sb.Append(elementId);
            sb.Append("')) });");
            sb.Append("</script>");
            return sb.ToString();
        }

        public static string RegisterCalendarStyle(this AjaxHelper helper)
        {
            const string s =
                "<script type='text/javascript'>" +
                "var link = document.createElement('link');" +
                "link.setAttribute('rel', 'stylesheet');" +
                "link.setAttribute('type', 'text/css');" +
                "link.setAttribute('href', '/Content/AjaxControlToolkit/AjaxControlToolkit.Calendar.Calendar.css');" +
                "var head = document.getElementsByTagName('head')[0]; head.appendChild(link);" +
                "</script>";
            return s;
        }
    }
}


////// Add Microsoft Ajax library
////sb.AppendLine(helper.MicrosoftAjaxLibraryInclude());

////// Add toolkit scripts
////sb.AppendLine(helper.ToolkitInclude
////    (
////        "AjaxControlToolkit.ExtenderBase.BaseScripts.js",
////        "AjaxControlToolkit.Common.Common.js",
////        "AjaxControlToolkit.Common.DateTime.js",
////        "AjaxControlToolkit.Animation.Animations.js",
////        "AjaxControlToolkit.PopupExtender.PopupBehavior.js",
////        "AjaxControlToolkit.Animation.AnimationBehavior.js",
////        "AjaxControlToolkit.Common.Threading.js",
////        "AjaxControlToolkit.Compat.Timer.Timer.js",
////        "AjaxControlToolkit.Calendar.CalendarBehavior.js"
////    ));


////// Add Calendar CSS file
////sb.AppendLine(helper.DynamicToolkitCssInclude("AjaxControlToolkit.Calendar.Calendar.css"));

////// Perform $create
////sb.AppendLine(helper.Create("AjaxControlToolkit.CalendarBehavior", elementId));
