/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace PremierTaxFree.PTFLib.Threading
{
    /// <summary>
    /// Runs methods by method pointer
    /// </summary>
    /// <remarks>
    /// Delegates may not be generics types
    /// </remarks>
    /// <example>
    /// public delegate void WriteLineDelegate(string text);
    /// public delegate void WriteLineDelegateFormat(string format, object[] @params);
    /// static void Main(string[] args)
    /// {
    ///     MethodRunnerThread th = new MethodRunnerThread();
    ///     th.Start();
    ///     WriteLineDelegate act1 = new WriteLineDelegate(WriteLine);
    ///     string typeName1 = act1.GetType().FullName;
    ///     IntPtr ptr1 = Marshal.GetFunctionPointerForDelegate(act1);
    ///     WriteLineDelegateFormat act2 = new WriteLineDelegateFormat(WriteLine);
    ///     string typeName2 = act2.GetType().FullName;
    ///     IntPtr ptr2 = Marshal.GetFunctionPointerForDelegate(act2);
    ///     for (int i = 0; i < 100; i++)
    ///     {
    ///         th.Run(typeName1, (int)ptr1, "Test " + i);
    ///         th.Run(typeName2, (int)ptr2, "{0} {1} {2}.", new object[] { i, i + i, i * i });
    ///     }
    ///     Console.Read();
    /// }
    /// public static void WriteLine(string text)
    /// {
    ///     Console.WriteLine(text);
    /// }
    /// public static void WriteLine(string format, object[] data )
    /// {
    ///     Console.WriteLine(format, data);
    /// }
    /// </example>
    public class MethodRunnerThread : CycleWorkerBase
    {
        private class MethodItem
        {
            public string Type { get; set; }
            public int Pointer { get; set; }
            public object[] Params { get; set; }
        }

        private readonly object m_Lock = new object();
        private readonly Queue m_Stack = Queue.Synchronized(new Queue());

        /// <summary>
        /// Run Method By Pointer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pointer"></param>
        /// <param name="params"></param>
        public void Run(string type, int pointer, params object[] @params)
        {
            lock (m_Lock)
            {
                m_Stack.Enqueue(new MethodItem() { Type = type, Pointer = pointer, Params = @params });
                Monitor.Pulse(m_Lock);
            }
        }

        protected override void ThreadFunction()
        {
            lock (m_Lock)
            {
                while (m_Stack.Count == 0)
                    Monitor.Wait(m_Lock);
            }

            MethodItem item = (MethodItem)m_Stack.Dequeue();
            Type type = Type.GetType(item.Type);
            IntPtr pointer = new IntPtr(item.Pointer);

            Delegate @delegate = Marshal.GetDelegateForFunctionPointer(pointer, type);
            @delegate.DynamicInvoke(item.Params);
        }
    }
}
