/***************************************************
//  Copyright (c) Premium Tax Free 2013
***************************************************/

using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;

namespace BtRetryService
{
    public static class Threading
    {
        public static event ThreadExceptionEventHandler Error;

        [TargetedPatchingOptOut("na")]
        public static void RunSafe(this Action act)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(null, new ThreadExceptionEventArgs(ex));
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe<T>(this Action<T> act, T arg)
        {
            try
            {
                act(arg);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(null, new ThreadExceptionEventArgs(ex));
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe<T1, T2>(this Action<T1, T2> act, T1 arg1, T2 arg2)
        {
            try
            {
                act(arg1, arg2);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(null, new ThreadExceptionEventArgs(ex));
            }
        }

        /// <summary>
        /// Wait until time: Example 5am or 9pm
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="time"></param>
        [TargetedPatchingOptOut("na")]
        public static void WaitUntil(this Thread thread, TimeSpan time)
        {
            Debug.Assert(thread != null);
            var t = time.Subtract(DateTime.Now.TimeOfDay);
            if (t < TimeSpan.Zero)
                t = new TimeSpan(24, 0, 0).Subtract(DateTime.Now.TimeOfDay).Add(time);
            Thread.Sleep(t);
        }
    }
}
