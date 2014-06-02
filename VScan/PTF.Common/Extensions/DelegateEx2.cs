/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace PremierTaxFree.PTFLib
{
    public static class DelegateBase
    {
        public static event ThreadExceptionEventHandler Error;
        private static readonly Hashtable ms_Callbacks = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Async delegate executor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="del"></param>
        /// <param name="done"></param>
        /// <param name="data"></param>
        /// <example>
        ///   new Func<string, int>((s) => { MessageBox.Show(s); return DateTime.Now.Millisecond; }).RunAsync(
        ///         new Action<int>((i) => MessageBox.Show(i.ToString())),
        ///         "Test");
        /// </example>
        public static void RunAsync<T, U>(this Func<T, U> del, Action<U> done, T data)
        {
            Debug.Assert(del != null);

            ms_Callbacks[del] = done;

            // [OneWay()]
            AsyncCallback callback = new AsyncCallback((ar) =>
            {
                Func<T, U> synchMethod = (Func<T, U>)((AsyncResult)ar).AsyncDelegate;

                U result = default(U);

                try
                {
                    result = synchMethod.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(null, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    Action<U> doneMethod = (Action<U>)ms_Callbacks[synchMethod];
                    ms_Callbacks.Remove(synchMethod);

                    //WARNING: This delegate may throw errors as well
                    //Those errors are intentionally unhandled
                    if (doneMethod != null)
                        doneMethod(result);
                }
            });

            del.BeginInvoke(data, callback, null);
        }

        /// <summary>
        /// Async delegate executor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="del"></param>
        /// <param name="done"></param>
        /// <param name="data"></param>
        /// <param name="state"></param>
        public static void RunAsync<T, U>(this Func<T, U> del, Action<U, object> done, T data, object state)
        {
            Debug.Assert(del != null);

            ms_Callbacks[del] = done;

            // [OneWay()]
            AsyncCallback callback = new AsyncCallback((ar) =>
            {
                Func<T, U> synchMethod = (Func<T, U>)((AsyncResult)ar).AsyncDelegate;

                object synchState = ((AsyncResult)ar).AsyncState;

                U result = default(U);

                try
                {
                    result = synchMethod.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(null, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    Action<U, object> doneMethod = (Action<U, object>)ms_Callbacks[synchMethod];
                    ms_Callbacks.Remove(synchMethod);

                    //WARNING: This delegate may throw errors as well
                    //Those errors are intentionally unhandled
                    if (doneMethod != null)
                        doneMethod(result, synchState);
                }
            });

            del.BeginInvoke(data, callback, state);
        }
    }
}