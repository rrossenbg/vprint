/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;

namespace VPrinting.Tools
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// 
    /// void Run(object o)
    /// {
    ///     var name = Convert.ToString(o);
    ///    
    ///     using (ScopeLocker locker = new ScopeLocker(name))
    ///     {
    ///         if (locker.TryGetin())
    ///         {
    ///             //Simulate long running process
    ///             Thread.Sleep(1000);
    ///             Console.WriteLine(name);
    ///         }
    ///     }
    /// }
    /// 
    /// </example>
    public class ScopeLocker : IDisposable
    {
        private readonly TimeSpan TIMEOUT = TimeSpan.FromMilliseconds(100);
        private readonly EventWaitHandle m_wait;

        public ScopeLocker(string name)
        {
            try
            {
                m_wait = EventWaitHandle.OpenExisting(name);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                m_wait = new EventWaitHandle(true, EventResetMode.ManualReset, name);
            }
        }

        public bool TryGetin()
        {
            if (m_wait.WaitOne(TIMEOUT))
            {
                m_wait.Reset();
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            m_wait.Dispose();
        }
    }
}
