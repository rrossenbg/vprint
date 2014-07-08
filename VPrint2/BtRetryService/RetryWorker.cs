/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PremierTaxFree.PTFLib.Threading;
using thread = System.Threading.Thread;

namespace BtRetryService
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://velocitydb.com/samples.aspx"/>
    public class RetryWorker : CycleWorkerBase
    {
        /// <summary>
        /// 3
        /// </summary>
        private const int MAX_RETRIES = 3;

        private readonly Queue m_dates = Queue.Synchronized(new Queue());

        public string BT_InParsed { get; set; }

        public event EventHandler Started, Completed;

        public void RunForDate(DateTime date)
        {
            m_dates.Enqueue(date);
        }

        public override void RunOnce()
        {
            if (m_dates.Count == 0)
                FireCompleted();

            DateTime date = m_dates.Count != 0 ? m_dates.Dequeue().cast<DateTime>() : DateTime.Now.Yesterday();

            var da = new TransferDBDataAccess();

            var rules = da.GetExportRules();
            var countries = da.GetAllCountries();

            foreach (int iso in countries)
            {
                Trace.WriteLine("check iso_id {0}  date {1}".format(iso, date), "RETRY");

                var vouchers = da.GetListOfVouchers(iso, date);

                foreach (var v in vouchers)
                {
                    if (da.CheckVoucherForStatusOK(v.Item1, v.Item2, v.Item3))
                    {
                        thread.Yield();
                        continue;
                    }

                    Trace.WriteLine("date {0:dd-MM-yy}  country id {1}  v_number {2}  v_type_id {3}".format(date, v.Item1, v.Item2, v.Item3), "RETRY");
                    var desc = da.GetVoucherTransDescriptiont(v.Item1, v.Item2, v.Item3);

                    string rule;

                    if (desc.Count >= MAX_RETRIES)
                    {
                        Trace.WriteLine("Too many retries. Skip it.", "RETRY");
                    }
                    else if (rules.Exists(desc, (r, i) => i != null && r != null && i.Contains(r), out rule))
                    {
                        var xml = da.GetVoucher(v.Item1, v.Item2, v.Item3).First();

                        try
                        {
                            string fileName = "DbExport_".concat(v.Item1, "_", v.Item2, '_', v.Item3, "_".Unique(), ".xml");
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
                            Trace.WriteLine(ex.Message, "RETRY");
                            Program.LogSafe(ex.ToString(), EventLogEntryType.Error);
                        }
                    }

                    thread.Yield();
                }
            }
        }

        protected override void FireStarted()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected void FireCompleted()
        {
            if (Completed != null)
                Completed(this, EventArgs.Empty);
        }
    }

    public class TranData
    {
        public int vt_v_number { get; set; }
        public int vt_v_type_id { get; set; }
        public int vt_v_country_iso_id { get; set; }
        public int vt_status_id { get; set; }
        public string vt_status_description { get; set; }
        public DateTime vt_creation_date { get; set; }
        public string vt_created_by { get; set; }
        public DateTime vt_last_modification_date { get; set; }
        public string vt_last_modified_by { get; set; }
        public int? vt_br_id { get; set; }
    }
}
