/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Reflection;
using System.Threading;

namespace VPrinting
{
    [Obfuscation]
    public static class ThreadBase
    {
        public static event ThreadExceptionEventHandler Error;

        [Obfuscation]
        private class TargetInfo
        {
            internal TargetInfo(Delegate d, object[] args)
            {
                Target = d;
                Args = args;
            }

            [Obfuscation]
            internal readonly Delegate Target;

            [Obfuscation]
            internal readonly object[] Args;
        }

        [Obfuscation]
        private static WaitCallback dynamicInvokeShim = new WaitCallback(DynamicInvokeShim);

        ////    SomeMethodDelegate smd = new SomeMethodDelegate(SomeMethod);
        ////    smd.FireAndForget(smd, "hello", 43);
        public static void FireAndForget(this Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(dynamicInvokeShim, new TargetInfo(d, args));
        }

        public static void FireAndForgetSafe(this Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(DynamicInvokeShimSafe, new TargetInfo(d, args));
        }

        [Obfuscation]
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

        [Obfuscation]
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
            }
        }

        /// <summary>
        /// A thread can be in Unstarted, WaitSleepJoin ot Stopped state
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public static ThreadState GetState(this Thread thread)
        {
            return thread.ThreadState & (ThreadState.Unstarted | ThreadState.WaitSleepJoin | ThreadState.Stopped);
        }

        public static void AbortSafe(this Thread th)
        {
            try
            {
                if (th != null)
                {
                    th.Abort();
                    th.Join(TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
            }
        }

        public static void AbortSafe(this Thread th, ref bool RunFlag)
        {
            try
            {
                RunFlag = false;
                if (th != null)
                {
                    th.Abort();
                    th.Join();
                }
            }
            catch
            {
            }
        }
    }
}