/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Http;

namespace MerchantSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
