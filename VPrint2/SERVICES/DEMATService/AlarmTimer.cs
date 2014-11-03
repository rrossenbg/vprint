/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Threading;
using DEMATLib;
using System.Reflection;

namespace DEMATService
{
    /// <summary>
    /// AlarmTimer
    /// </summary>
    /// <example>
    /// var timer = new AlarmTimer();
    /// timer.Tick += new EventHandler(timer_Tick);
    /// timer.AutoRestart = true;
    /// timer.AlarmAt = new TimeSpan(15, 14, 0);
    /// timer.Start();
    /// </example>
    [Obfuscation(ApplyToMembers = true)]
    internal class AlarmTimer : IDisposable
    {
        private System.Threading.Timer m_Timer;

        public event EventHandler Tick;

        /// <summary>
        /// TimeSpan(15, 14, 0)
        /// Fire every day at 15:14 pm
        /// </summary>
        public TimeSpan AlarmAt { get; set; }
        public TimeSpan Period { get; set; }
        public bool AutoRestart { get; set; }

        public AlarmTimer()
        {
            Period = new TimeSpan(0, 0, 0, 0, -1);
        }

        public void Start()
        {
            Dispose();
            m_Timer = new System.Threading.Timer(TimerTick);
            m_Timer.Change(DateTime.Now.Minus(AlarmAt), Period);
        }

        public void Stop()
        {
            Dispose();
        }

        private void TimerTick(object data)
        {
            FireTick();

            if (AutoRestart)
            {
                Dispose();
                m_Timer = new System.Threading.Timer(TimerTick);
            }
        }

        private void FireTick()
        {
            if (Tick != null)
                Tick(this, new EventArgs());
        }

        public void Dispose()
        {
            if (m_Timer != null)
            {
                using (var h = new ManualResetEvent(false))
                {
                    m_Timer.Dispose(h);
                    h.WaitOne();
                    m_Timer = null;
                }
            }
        }
    }
}
