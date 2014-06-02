using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizTalkFiles
{
    public class TimeWorker : CycleWorkerBase
    {
        public TimeWorker()
        {
            SleepTime = TimeSpan.FromHours(3);
        }

        protected override void ThreadFunction()
        {
            try
            {
                Globals.m_TimesUp = (Globals.EndDay < Globals.GetNistTime());
            }
            catch
            {
                //No errors
            }
        }
    }
}
