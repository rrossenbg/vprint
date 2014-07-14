/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.Runtime;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using FintraxPTFImages.Common;

namespace FintraxPTFImages
{
    public static class MVCEx
    {
        [TargetedPatchingOptOut("na")]
        public static MvcHtmlString IIF<T>(this HtmlHelper<T> value, bool condition, Func<string> value1, Func<string> value2)
        {
            return (condition ? new MvcHtmlString(value1()) : new MvcHtmlString(value2()));
        }

        /// <summary>
        /// Url.AbsoluteContent("~/static/images/image.jpg")
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static string AbsoluteContent(this UrlHelper url, string contentPath)
        {
            var requestUrl = url.RequestContext.HttpContext.Request.Url;
            return string.Format(
                "{0}{1}",
                requestUrl.GetLeftPart(UriPartial.Authority),
                url.Content(contentPath)
            );
        }

        [TargetedPatchingOptOut("na")]
        public static Uri FullyQualifiedUri(this UrlHelper url, string relativeOrAbsolutePath)
        {
            Uri baseUri = HttpContext.Current.Request.Url;
            string path = UrlHelper.GenerateContentUrl(relativeOrAbsolutePath, new HttpContextWrapper(HttpContext.Current));
            Uri instance = null;
            bool ok = Uri.TryCreate(baseUri, path, out instance);
            return instance; // instance will be null if the uri could not be created
        }

        #region SESSION

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpSessionState state, string name, bool throwIfNotFound = true) where T : class
        {
            if (throwIfNotFound && state[name] == default(T))
                throw new ArgumentNullException("Cannot find " + name);

            return (T)state[name];
        }

        [TargetedPatchingOptOut("na")]
        public static void Set(this HttpSessionState state, string name, object data)
        {
            state[name] = data;
        }

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpSessionStateBase state, string name, bool throwIfNotFound = true) where T : class
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);

            if (throwIfNotFound && state[name] == default(T))
                throw new ArgumentNullException("Cannot find " + name);

            return (T)state[name];
        }

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpSessionStateBase state, string name, Func<T> createFunc) where T : class
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);
            Debug.Assert(createFunc != null);

            if (state[name] == default(T))
                state[name] = createFunc();

            return (T)state[name];
        }

        [TargetedPatchingOptOut("na")]
        public static void Set(this HttpSessionStateBase state, string name, object data)
        {
            state[name] = data;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Contains(this HttpSessionState state, string name)
        {
            return state[name] != null;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Contains(this HttpSessionStateBase state, string name)
        {
            return state[name] != null;
        }

        #endregion //SESSION

        #region APPLICATION

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpApplicationState state, string name, bool throwIfNotFound = true) where T : class
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);

            try
            {
                state.Lock();
                if (throwIfNotFound && state[name] == default(T))
                    throw new ArgumentNullException("Cannot find " + name);

                return (T)state[name];
            }
            finally
            {
                state.UnLock();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void Set(this HttpApplicationState state, string name, object data)
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);

            try
            {
                state.Lock();
                state[name] = data;
            }
            finally
            {
                state.UnLock();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpApplicationStateBase state, string name, bool throwIfNotFound = true) where T : class
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);

            try
            {
                state.Lock();

                if (throwIfNotFound && state[name] == default(T))
                    throw new ArgumentNullException("Cannot find " + name);

                return (T)state[name];
            }
            finally
            {
                state.UnLock();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this HttpApplicationStateBase state, string name, Func<T> createFunc) where T : class
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);
            Debug.Assert(createFunc != null);

            try
            {
                state.Lock();

                if (state[name] == default(T))
                    state[name] = createFunc();

                return (T)state[name];
            }
            finally
            {
                state.UnLock();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void Set(this HttpApplicationStateBase state, string name, object data)
        {
            Debug.Assert(state != null);
            Debug.Assert(name != null);

            try
            {
                state.Lock();
                state[name] = data;
            }
            finally
            {
                state.UnLock();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T GetCached<T>(this HttpApplicationState state, string name, T data, TimeSpan time)
        {
            if (HttpRuntime.Cache[name] != null)
                return (T)HttpRuntime.Cache[name];

            HttpRuntime.Cache.Insert(name, data, null, DateTime.Now.Add(time), TimeSpan.Zero);
            return data;
        }

        [TargetedPatchingOptOut("na")]
        public static void SetCached<T>(this HttpApplicationState state, string name, TimeSpan time, T data)
        {
            HttpRuntime.Cache.Insert(name, data, null, DateTime.Now.Add(time), TimeSpan.Zero);
        }

        #endregion //APPLICATION

        /// <summary>
        /// Shows a pager control - Creates a list of links that jump to each page
        /// </summary>
        /// <param name="page">The ViewPage instance this method executes on.</param>
        /// <param name="pagedList">A PagedList instance containing the data for the paged control</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action on the controller.</param>
        /// <example>@this.ShowPagerControl(Model, "Bids", "Page")</example>
        public static void ShowPagerControl(this ViewPage page, IPagedList pagedList, string controllerName, string actionName)
        {
            var writer = new HtmlTextWriter(page.Response.Output);
            if (writer != null)
            {
                for (int pageNum = 1; pageNum <= pagedList.TotalPages; pageNum++)
                {
                    if (pageNum != pagedList.CurrentPage)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("/{0}/{1}/{2}", controllerName, actionName, pageNum));
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, string.Concat("Page ", pageNum));
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class,
                                        pageNum == pagedList.CurrentPage ?
                                                            "pageLinkCurrent" :
                                                            "pageLink");

                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(pageNum);
                    writer.RenderEndTag();

                    if (pageNum != pagedList.CurrentPage)
                        writer.RenderEndTag();

                    writer.Write("&nbsp;");
                }

                writer.Write("(");
                writer.Write(pagedList.TotalItems);
                writer.Write(" items in all)");
            }
        }
    }
}