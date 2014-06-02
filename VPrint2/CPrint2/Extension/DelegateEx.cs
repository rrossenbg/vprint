    /***************************************************
    //  Copyright (c) Premium Tax Free 2011
    /***************************************************/

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

namespace CPrint2
{
    public static class DelegateBase
    {
        /// <summary>
        /// 500 ms
        /// </summary>
        private const int TIMEOUT = 500;
        /// <summary>
        /// 5
        /// </summary>
        private const int MAX_COUNT = 5;
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
        [TargetedPatchingOptOut("na")]
        public static void RunAsync<T, R>(this Func<T, R> del, Action<R> done, T data)
        {
            Debug.Assert(del != null);

            ms_Callbacks[del] = done;

            // [OneWay()]
            AsyncCallback callback = new AsyncCallback((ar) =>
            {
                Func<T, R> synchMethod = (Func<T, R>)((AsyncResult)ar).AsyncDelegate;

                R result = default(R);

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

                    Action<R> doneMethod = (Action<R>)ms_Callbacks[synchMethod];
                    ms_Callbacks.Remove(synchMethod);

                    //WARNING: This delegate may throw errors
                    //Those errors are intentionally unhandled
                    if (doneMethod != null)
                        doneMethod(result);
                }
            });

            del.BeginInvoke(data, callback, null);
        }

        [TargetedPatchingOptOut("na")]
        public static void RunAsync<T, R>(this Func<T, R> del, Action<R, object> done, T data, object state)
        {
            Debug.Assert(del != null);

            ms_Callbacks[del] = done;

            // [OneWay()]
            AsyncCallback callback = new AsyncCallback((ar) =>
            {
                Func<T, R> synchMethod = (Func<T, R>)((AsyncResult)ar).AsyncDelegate;

                object synchState = ((AsyncResult)ar).AsyncState;

                R result = default(R);

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
                    Action<R, object> doneMethod = (Action<R, object>)ms_Callbacks[synchMethod];
                    ms_Callbacks.Remove(synchMethod);

                    //WARNING: This delegate may throw errors
                    //Those errors are intentionally unhandled
                    if (doneMethod != null)
                        doneMethod(result, synchState);
                }
            });

            del.BeginInvoke(data, callback, state);
        }

        [TargetedPatchingOptOut("na")]
        public static void RunAsync<T, U, R>(this Func<T, U, R> del, Action<R, object> done, T data1, U data2, object state)
        {
            Debug.Assert(del != null);

            ms_Callbacks[del] = done;

            // [OneWay()]
            AsyncCallback callback = new AsyncCallback((ar) =>
            {
                Func<T, U, R> synchMethod = (Func<T, U, R>)((AsyncResult)ar).AsyncDelegate;

                object synchState = ((AsyncResult)ar).AsyncState;

                R result = default(R);

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
                    Action<R, object> doneMethod = (Action<R, object>)ms_Callbacks[synchMethod];
                    ms_Callbacks.Remove(synchMethod);

                    //WARNING: This delegate may throw errors
                    //Those errors are intentionally unhandled
                    if (doneMethod != null)
                        doneMethod(result, synchState);
                }
            });

            del.BeginInvoke(data1, data2, callback, state);
        }

        [TargetedPatchingOptOut("na")]
        public static void ReTry(this EventHandler handle, object sender, EventArgs e)
        {
            int count = 0;
        go:
            try
            {
                handle(sender, e);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return;

                if (Global.Instance.ExitSignal)
                    return;

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ReTry<T, U>(this Action<T, U> handle, T t, U u)
        {
            int count = 0;
        go:
            try
            {
                handle(t, u);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return;

                if (Global.Instance.ExitSignal)
                    return;

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ReTry<T, U, V>(this Action<T, U, V> handle, T t, U u, V v)
        {
            int count = 0;
        go:
            try
            {
                handle(t, u, v);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return;

                if (Global.Instance.ExitSignal)
                    return;

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static R ReTry<T, R>(this Func<T, R> handle, T t)
        {
            int count = 0;
        go:
            try
            {
                return handle(t);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "VPRINT");

                if (count++ > MAX_COUNT)
                    return default(R);

                if (Environment.HasShutdownStarted || Global.Instance.ExitSignal)
                    return default(R);

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static R ReTry<T, U, R>(this Func<T, U, R> handle, T t, U u)
        {
            int count = 0;
        go:
            try
            {
                return handle(t, u);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return default(R);

                if (Global.Instance.ExitSignal)
                    return default(R);

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ReTry<T, U, V, R>(this Action<T, U, V> handle, T t, U u, V v)
        {
            int count = 0;
        go:
            try
            {
                handle(t, u, v);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return;

                if (Global.Instance.ExitSignal)
                    return;

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static R ReTry<T, U, V, R>(this Func<T, U, V, R> handle, T t, U u, V v)
        {
            int count = 0;
        go:
            try
            {
                return handle(t, u, v);
            }
            catch
            {
                if (count++ > MAX_COUNT)
                    return default(R);

                if (Global.Instance.ExitSignal)
                    return default(R);

                Thread.Sleep(TIMEOUT);
                goto go;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe(this Action handle)
        {
            try
            {
                handle();
            }
            catch
            {
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe<T>(this Action<T> handle, T t)
        {
            try
            {
                handle(t);
            }
            catch
            {
            }
        }
    }
}
