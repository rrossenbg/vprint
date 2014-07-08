using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using PremierTaxFree.PTFLib.Threading;

namespace BtRetryService
{
    public class RetryWorker2 : CycleWorkerBase
    {
        private const int MAX_RETRIES = 5;

        private readonly Queue m_dates = Queue.Synchronized(new Queue());

        private readonly TransferDBDataAccess m_da = new TransferDBDataAccess();

        private List<string> m_Rules;
        private List<int> m_Countries;

        public string BT_InParsed { get; set; }

        public RetryWorker2()
        {
        }

        public void Init()
        {
            Program.LogSafe("BtRetryService: RetryWorker started.", EventLogEntryType.Information);
            m_Rules = m_da.GetExportRules();
            m_Countries = m_da.GetAllCountries();
        }

        public void RunForDate(DateTime date)
        {
            m_dates.Enqueue(date);
        }

        public override void RunOnce()
        {
            if (m_Rules == null)
                throw new ArgumentNullException("m_Rules");

            if (m_Countries == null)
                throw new ArgumentNullException("m_Countries");

            var da = new TransferDBDataAccess();

            DateTime date = m_dates.Count != 0 ? m_dates.Dequeue().cast<DateTime>() : DateTime.Today;

            foreach (int iso in m_Countries)
            {
                for (int vttype = 0; vttype <= 3; vttype++)
                {
                    var list = da.GetListOfVouchersForDate(iso, date, vttype);
                    ProcessList(list, iso, vttype, date);
                }
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }

            Program.LogSafe(string.Format("Sleeping to {0:t}.", DateTime.Now.Add(SleepTime)), EventLogEntryType.Information);
        }

        protected override void FireStarted()
        {

        }

        public void ProcessList(List<TransferDBDataAccess.DbVoucherInfo> list, int iso, int vttype, DateTime date)
        {
            foreach (var v in list)
            {
                var vouchers = m_da.GetListOfVouchersByVid(v.vt_v_number, iso, vttype);

                var q = from i in vouchers
                        group i by i.vt_v_number into voucherGroup
                        select new { Key = voucherGroup.Key, Vouchers = voucherGroup };

                foreach (var g in q)
                {
                    Trace.WriteLine(string.Format("Processing date: {0:dd-MM-yyyy} iso: {1} vid: {2} type: {3}",
                        date, iso, g.Key, vttype), "RETRY");
                    //Voucher status is OK
                    if (g.Vouchers.FirstOrDefault(i1 => i1.vt_status_id.In(5, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 23, 24)) != null)
                        continue;

                    //Too many exports
                    if (g.Vouchers.Count() > MAX_RETRIES)
                        continue;

                    //voucher already exists 
                    if (g.Vouchers.FirstOrDefault(i1 => i1.vt_status_description != null && i1.vt_status_description.Contains("voucher already exists")) != null)
                        continue;

                    string rule = null;

                    //Voucher doesn't contain error from export rule error list
                    if (g.Vouchers.FirstOrDefault(
                        i1 => i1.vt_status_description != null && 
                        i1.vt_status_description.In(m_Rules, (i, j) => i != null && j != null && (i.Contains(j) || j.Contains(i)), out rule)) == null)
                        continue;

                    string fileName = "DbExport_".concat(iso, "_", g.Key, '_', vttype, "_".Unique(), ".xml");
                    var fullFileName = Path.Combine(BT_InParsed, fileName);

                    ExportFileSafe(g.Vouchers.Last().vt_id, fullFileName, rule);
                }
            }
        }

        private void ExportFileSafe(int vt_id, string fileName, string rule)
        {
            try
            {
                var xml = m_da.GetVoucher(vt_id);
                //File.AppendAllText("C:\\test.txt", string.Format("{0},{1},{2}\r\n", iso, g.Key, vttype));
                var fullFileName = Path.Combine(BT_InParsed, fileName);

                using (var file = File.CreateText(fullFileName))
                {
                    file.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    file.WriteLine("<VFPData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\");");
                    file.WriteLine(xml);
                    file.WriteLine("</VFPData>");
                }

                Trace.WriteLine("Export file: ".concat(fileName, " rule: ", rule), "RETRY");
                Program.LogSafe("Export file: ".concat(fullFileName, " rule: ", rule), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                FireError(ex);
            }
            finally
            {
                Thread.Yield();
            }
        }
    }
}
