using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Routing;

namespace FintraxPTFImages.Handler
{
    /// <summary>
    /// http://www.hanselman.com/blog/BackToBasicsDynamicImageGenerationASPNETControllersRoutingIHttpHandlersAndRunAllManagedModulesForAllRequests.aspx
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        public bool IsReusable { get { return false; } }
        protected RequestContext RequestContext { get; set; }

        public ImageHandler() : base() { }

        public ImageHandler(RequestContext requestContext)
        {
            this.RequestContext = requestContext;
        }

        public void ProcessRequest(HttpContext context)
        {
            using (var rectangleFont = new Font("Arial", 14, FontStyle.Bold))
            using (var bitmap = new Bitmap(320, 110, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = Color.Bisque;
                g.Clear(backgroundColor);
                g.DrawString("This PNG was totally generated", rectangleFont, SystemBrushes.WindowText, new PointF(10, 40));
                context.Response.ContentType = "image/png";
                bitmap.Save(context.Response.OutputStream, ImageFormat.Png);
            }
        }
    }
}