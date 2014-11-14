using System;
using System.Threading;

namespace VPrinting
{
    public class WaitObject : IDisposable
    {
        private readonly ManualResetEventSlim m_Done = new ManualResetEventSlim(false);

        public object Value { get; private set; }
        public Exception Err { get; set; }

        public WaitObject(object value)
        {
            Value = value;
        }

        public void Signal(bool set = true)
        {
            if (set)
                m_Done.Set();
            else
                m_Done.Reset();
        }

        public bool WaitOne(TimeSpan timeout)
        {
            return m_Done.Wait(timeout);
        }

        public void Dispose()
        {
            using (m_Done) ;
        }
    }
}
