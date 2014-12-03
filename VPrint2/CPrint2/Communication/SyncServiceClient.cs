
using System;
using System.Threading;
using CPrint2.Common;
using CPrint2.Data;
using CPrint2.SyncServiceRef;
using VPrinting.Threading;

namespace CPrint2
{
    public class SyncServiceDataAccess : CycleWorkerBase
    {
        public event EventHandler<ValueEventArgs<DataObj2>> Submit;

        public override void RunOnce()
        {
            try
            {
                using (var client = new SyncServiceClient())
                {
                    var result = client.Sync();
                    var obj = DataObj2.Parse(result);
                    if (Submit != null && obj != null)
                        Submit(this, new ValueEventArgs<DataObj2>(obj));
                }
            }
            catch (TimeoutException)
            {
                //No problems
                //Timeout every 10 minute
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        protected override void FireStarted()
        {
        }
    }
}
