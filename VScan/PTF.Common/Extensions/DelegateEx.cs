/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.ComponentModel;
using System.Security;
using System.Security.Principal;
using PremierTaxFree.PTFLib.Native;
using System.Threading;

namespace PremierTaxFree.PTFLib
{
    public static class DelegateEx
    {
        /// <summary>
        /// Creates a delegate chain
        /// </summary>
        /// <param name="del"></param>
        /// <param name="dels"></param>
        /// <returns></returns>
        /// <example>
        /// private void Button_Click(object sender, EventArgs e)
        /// {
        ///     new EventHandler((s, e1) => { MessageBox.Show(s.GetHashCode().ToString()); }).Add(
        ///        new EventHandler((s, e1) => { MessageBox.Show(s.GetHashCode().ToString()); }),
        ///        new EventHandler((s, e1) => { MessageBox.Show(s.GetHashCode().ToString()); })).DynamicInvoke(this, e);
        /// }
        /// </example>
        public static Delegate Add(this Delegate del, params Delegate[] dels)
        {
            return Delegate.Combine(del, Delegate.Combine(dels));
        }

        /// <summary>
        /// Removes all receivers from a delegate chain
        /// </summary>
        /// <param name="delegate"></param>
        public static void RemoveAll(this EventHandler @delegate) 
        {
            if (@delegate == null)
                return;

            foreach (EventHandler del in @delegate.GetInvocationList())
                @delegate -= del;
        }

        /// <summary>
        /// Removes all receivers from a delegate chain by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegate"></param>
        public static void RemoveAll<T>(this EventHandler<T> @delegate) where T : EventArgs
        {
            if (@delegate == null)
                return;

            foreach (EventHandler<T> del in @delegate.GetInvocationList())
                @delegate -= del;
        }

        /// <summary>
        /// Removes all receivers from a ThreadExceptionEventHandler delegate chain
        /// </summary>
        /// <param name="delegate"></param>
        public static void RemoveAll(this ThreadExceptionEventHandler @delegate)
        {
            if (@delegate == null)
                return;

            foreach (ThreadExceptionEventHandler del in @delegate.GetInvocationList())
                @delegate -= del;
        }

        /// <summary>
        /// Runs action delegate synchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        /// <param name="data"></param>
        /// <param name="completedCallback"></param>
        public static void RunAsync<T>(this Action<T> del, T data, AsyncCallback completedCallback)
        {
            lock (typeof(T))
            {
                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                del.BeginInvoke(data, completedCallback, async);
            }
        }

        /// <summary>
        /// Invokes delegate dynamicly with paramethers
        /// </summary>
        /// <param name="del"></param>
        /// <param name="list"></param>
        public static void Fire(this Delegate del, params object[] list)
        {
            if (del != null)
                del.DynamicInvoke(list);
        }

        /// <summary>
        /// Web mainly. Impersonate the Authenticating User
        /// </summary>
        /// <param name="identity">((WindowsIdentity)User.Identity)</param>
        /// <param name="data"></param>
        public static void RunAsUser(this Delegate del, WindowsIdentity identity, params object[] data)
        {
            WindowsImpersonationContext impersonationContext = identity.Impersonate();
            try
            {
                if (del != null)
                    del.DynamicInvoke(data);
            }
            finally
            {
                impersonationContext.Undo();
            }
        }

        public static void RunAsUser(this Delegate del, string userName, string domain, string password, params object[] data)
        {
            IntPtr lnToken;
            int result = advapi32.LogonUser(userName, domain, password, advapi32.LOGON32_LOGON_NETWORK, advapi32.LOGON32_PROVIDER_DEFAULT, out lnToken);
            if (result <= 0)
                throw new SecurityException("Can authenticate as user");
            try
            {
                if (del != null)
                    del.DynamicInvoke(data);
            }
            finally
            {
                advapi32.RevertToSelf();
            }
        }
    }
}
