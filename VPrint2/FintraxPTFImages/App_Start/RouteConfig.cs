/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Mvc;
using System.Web.Routing;

namespace FintraxPTFImages
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{file}.html");

            routes.IgnoreRoute("{*filter}", new { filter = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*filter}", new { filter = @".*\.jpeg(/.*)?" });
            routes.IgnoreRoute("{*filter}", new { filter = @".*\.tif(/.*)?" });
            routes.IgnoreRoute("{*filter}", new { filter = @".*\.tiff(/.*)?" });
            routes.IgnoreRoute("{*filter}", new { filter = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*filter}", new { filter = @".*\.bmp(/.*)?" });
            
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");

            routes.RouteExistingFiles = true;

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}