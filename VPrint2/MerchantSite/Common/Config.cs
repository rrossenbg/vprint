using System.Web;
using System.Web.Configuration;

namespace MerchantSite.Common
{
    public class Config
    {
        public static string SITEROOT
        {
            get
            {
                return WebConfigurationManager.AppSettings["SITEROOT"];
            }
        }

        public static string FILECACHEFOLDER
        {
            get
            {
                return WebConfigurationManager.AppSettings["FILECACHEFOLDER"];
            }
        }

        public static string WEBVOUCHERFOLDER
        {
            get
            {
                string webVoucherRootPath = HttpContext.Current.Server.MapPath("~/WEBVOUCHERFOLDER");
                return webVoucherRootPath;
            }
        }

        public static string WEBVOUCHERFOLDER_SHARE
        {
            get
            {
                return WebConfigurationManager.AppSettings["WEBVOUCHERFOLDER_SHARE"];
            }
        }

        public static int FILETRANSFER_TIMEOUT
        {
            get
            {
                return 700;
            }
        }
    }
}