/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

namespace PTF.Reports
{
    public static class WebEx
    {
        public static T Parse<T>(this HttpRequestBase request, string key, T defaultValue = default(T))
        {
            try
            {
                return (string.IsNullOrWhiteSpace(request.Form[key]) ?
                     defaultValue :
                    (T)Convert.ChangeType(request.Form[key], typeof(T)));
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return defaultValue;
            }
        }

        public static T Get<T>(this HttpSessionStateBase session, string name, T defaultValue = default(T)) 
        {
            if (session[name] == null || !(session[name] is T))
                session[name] = defaultValue;
            return (T)session[name];
        }

        public static T Get<T>(this HttpContextBase ctx, string name)
        {
            Debug.Assert(ctx != null);
            return (T) ctx.Cache[name];
        }

        public static void Set(this HttpContextBase ctx, string name, object obj)
        {
            Debug.Assert(ctx != null);
            ctx.Cache.Remove(name);
            ctx.Cache.Add(name, obj, null, DateTime.Now.AddMinutes(1), TimeSpan.MaxValue, CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// "Microsoft.Reporting.WebForms.ReportHierarchy"
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="key"></param>
        public static void Clear(this HttpContext ctx, string key)
        {
            lock (((ICollection)ctx.Session.Keys).SyncRoot)
            {
                for (int i = 0; i < ctx.Session.Keys.Count; )
                {
                    if (ctx.Session[i].ToString() == key)
                        ctx.Session.RemoveAt(i);
                    else
                        i++;
                }
            }
        }

        public static bool IsViaProxy(this HttpRequest request)
        {
            string proxyIPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            return (!string.IsNullOrEmpty(proxyIPAddress));
        }

        public static bool IsViaProxy(this HttpRequestBase request)
        {
            string proxyIPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            return (!string.IsNullOrEmpty(proxyIPAddress));
        }

        public static void Remove(this HttpSessionStateBase session, params string[] keys)
        {
            foreach (string key in keys)
                session.Remove(key);
        }
    }
}