/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Threading;

namespace VPrinting.Communication
{
    /// <summary>
    /// 
    /// </summary>
    ///<example>
    /// public class Test
    /// {
    ///     static void Main()
    ///      {
    ///          Thread th1 = new Thread(new ThreadStart(TryTestLocker));
    ///          th1.IsBackground = true;
    ///          th1.Start();
    ///          Thread th2 = new Thread(new ThreadStart(TryTestLocky));
    ///          th2.IsBackground = true;
    ///          th2.Start();
    ///          ConsoleKeyInfo key;
    ///          do
    ///          {
    ///              key = Console.ReadKey();
    ///              Console.Clear();
    ///          }
    ///          while (key.Key != ConsoleKey.Escape);
    ///      }
    ///      public static void TryTestLocker()
    ///      {
    ///          while (true)
    ///          {
    ///              using (var global = new GlobalNamedEvent("127.0.0.1"))
    ///                  global.Signal();
    ///              Console.WriteLine("<-- SIGNAL -->");
    ///              Thread.Sleep(TimeSpan.FromSeconds(40));
    ///          }
    ///      }
    ///      public static void TryTestLocky()
    ///      {
    ///          while (true)
    ///          {
    ///              try
    ///              {
    ///                  using (var global = new GlobalNamedEvent("127.0.0.1"))
    ///                      if (!global.WaitOne(TimeSpan.FromSeconds(35)))
    ///                          throw new TimeoutException("Operation timeout");
    ///                  Console.WriteLine("<-- OK -->");
    ///              }
    ///              catch(Exception ex)
    ///              {
    ///                  Console.WriteLine(ex.Message);
    ///              }
    ///          }
    ///      }
    /// }
    /// </example>
    public class GlobalNamedEvent : IDisposable
    {
        private readonly EventWaitHandle m_globalEvent;

        public GlobalNamedEvent(string name, EventResetMode resetMode = EventResetMode.AutoReset)
        {
            try
            {
                m_globalEvent = EventWaitHandle.OpenExisting(name);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                m_globalEvent = new EventWaitHandle(false, resetMode, name);
            }
        }

        public void Signal(bool set = true)
        {
            if (set)
            {
                m_globalEvent.Set();
            }
            else
            {
                m_globalEvent.Reset();
            }
        }

        public bool WaitOne(TimeSpan timeout)
        {
            return m_globalEvent.WaitOne(timeout);
        }

        public void Dispose()
        {
            m_globalEvent.Dispose();
        }
    }
}