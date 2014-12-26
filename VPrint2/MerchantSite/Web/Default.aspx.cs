using System;
using System.Collections;
using System.Web.Services;
using System.Web.UI.WebControls;

////
////routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
////
////------------------------------------------------------------------
////<script type="text/javascript">
////    //Code Starts
////    $(document).ready(function () {
////        $.ajax({
////            type: "POST",
////            url: "http://localhost/MerchantSite/Web/Default.aspx/GetLanguageList",
////            data: '',
////            contentType: "application/json; charset=utf-8",
////            dataType: "json",
////            success: function (msg) {
////                $("#Retailer").empty().append($("<option></option>").val("[-]").html("Please select"));
////                $.each(msg.d, function () {
////                    $("#Retailer").append($("<option></option>").val(this['Value']).html(this['Text']));
////                });
////            },
////            error: function () {
////                alert("An error has occurred during processing your request.");
////            }
////        });
////    });
////    //Code Ends
////</script>
namespace MerchantSite
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod()]
        public static ArrayList GetLanguageList()
        {
            var results = new ArrayList();
            results.Add(new ListItem("C#", "1"));
            results.Add(new ListItem("Java", "2"));
            results.Add(new ListItem("PHP", "3"));
            results.Add(new ListItem("VB.NET", "4"));
            results.Add(new ListItem("JavaScript", "5"));
            results.Add(new ListItem("jQuery", "6"));
            return results;
        }
    }
}