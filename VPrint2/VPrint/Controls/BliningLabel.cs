using System;
using System.Windows.Forms;

namespace VPrinting.Controls
{
    public partial class BliningLabel : Label
    {
        protected bool m_Started = false;
        private readonly Timer m_TimerLow = new Timer();
        private readonly Timer m_TimerUp = new Timer();

        public TimeSpan IntervalLow
        {
            get
            {
                return TimeSpan.FromMilliseconds(m_TimerLow.Interval);
            }
            set
            {
                m_TimerLow.Interval = Convert.ToInt32(value.TotalMilliseconds);
            }
        }

        public TimeSpan IntervalUp
        {
            get
            {
                return TimeSpan.FromMilliseconds(m_TimerUp.Interval);
            }
            set
            {
                m_TimerUp.Interval = Convert.ToInt32(value.TotalMilliseconds);
            }
        }

        public BliningLabel()
        {
            m_TimerUp.Tick += new EventHandler(TimerUp_Tick);
            m_TimerLow.Tick += new EventHandler(LowTimer_Tick);
        }

        private void LowTimer_Tick(object sender, EventArgs e)
        {
            Visible = true;
            m_TimerLow.Stop();
            m_TimerUp.Start();
        }

        private void TimerUp_Tick(object sender, EventArgs e)
        {
            Visible = false;
            m_TimerUp.Stop();

            if (m_Started)
                m_TimerLow.Start();
        }

        public void Start()
        {
            m_Started = true;
            m_TimerLow.Start();
        }

        public void Stop()
        {
            Visible = false;
            m_Started = false;
            m_TimerLow.Stop();
        }
    }
}
