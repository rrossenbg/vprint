/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Threading;

namespace PTF.Reports
{
    public static class DelegateEx
    {
        public static event ThreadExceptionEventHandler Error;

        private class TargetInfo
        {
            internal TargetInfo(Delegate d, object[] args)
            {
                Target = d;
                Args = args;
            }

            internal readonly Delegate Target;
            internal readonly object[] Args;
        }

        private static WaitCallback dynamicInvokeShim = new WaitCallback(DynamicInvokeShim);

        ////    SomeMethodDelegate smd = new SomeMethodDelegate(SomeMethod);
        ////    smd.FireAndForget(smd, "hello", 43);
        public static void FireAndForget(this Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(dynamicInvokeShim, new TargetInfo(d, args));
        }

        /// <summary>
        /// Fires and forgets a delegate safely
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void FireAndForgetSafe(this Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(DynamicInvokeShimSafe, new TargetInfo(d, args));
        }

        private static void DynamicInvokeShim(object o)
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                TargetInfo ti = (TargetInfo)o;
                ti.Target.DynamicInvoke(ti.Args);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(o, new ThreadExceptionEventArgs(ex));
            }
        }

        private static void DynamicInvokeShimSafe(object o)
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                TargetInfo ti = (TargetInfo)o;
                ti.Target.DynamicInvoke(ti.Args);
            }
            catch
            {
                //Do nothing here. Err is ignored
            }
        }
    }
}
