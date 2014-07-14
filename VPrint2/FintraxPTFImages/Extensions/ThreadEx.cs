using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace FintraxPTFImages
{
    public static class ThreadEx
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

        /// <summary>
        /// Aborts a thread safely
        /// </summary>
        /// <param name="th"></param>
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

        /// <summary>
        /// Joins a thread safely
        /// </summary>
        /// <param name="th"></param>
        public static void JoinSafe(this Thread th, TimeSpan timeout)
        {
            try
            {
                if (th != null)
                    th.Join(timeout);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Aborts a thread safely
        /// </summary>
        /// <param name="th"></param>
        /// <param name="RunFlag"></param>
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