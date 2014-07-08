/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using BtRetryService.Razor.RazorTemplating;
using PremierTaxFree.PTFLib.Threading;
using thread = System.Threading.Thread;

namespace BtRetryService
{
    ////        Email Notify Service
    ////=============================================
    ////•	This is a separated process in FintraxReTryService
    ////•	Runs at certain period of time
    ////•	It selects as datasource all messages of transfers from yesterday 
    ////•	Select all active emails or group of emails that need to be notified
    ////•	Each email or group of emails have their own conditions that need to be meet
    ////•	Each email or group of emails have different Razor Template assigned 
    ////•	For each email of group of emails it applies that filter over the datasource and narrow it.
    ////•	It binds the narrowed datasource to the Razor template and generates html output for each email or group of emails.
    ////Means the view of report for each email or group of emails depends on the RazorTemplate  and can be easily and dynamically changed
    ////•	It sends the html to email or group or emails.
    public class EmailWorker : CycleWorkerBase
    {
        public event EventHandler Started;
        public volatile bool EmailMe = false;
        public volatile bool Force = false;
        private DateTime m_ForceDate;

        public DateTime ForceDate
        {
            get
            {
                Thread.MemoryBarrier();
                return m_ForceDate;
            }
            set
            {
                m_ForceDate = value;
                Thread.MemoryBarrier();
            }
        }

        public TimeSpan StartAt { get; set; }

        public EmailWorker()
        {
            ForceDate = DateTime.Now.Yesterday();
        }

        public override void RunOnce()
        {
            Trace.WriteLine("Mailer started.", "EML");

            Program.LogSafe("BtRetryService: Mailer started.", EventLogEntryType.Information);

            List<Task> emailTasks = new List<Task>();

            try
            {
                if (!Force)
                    Thread.CurrentThread.WaitUntil(StartAt);

                TransferDbEntities transferDb = new TransferDbEntities();
                var emails = from em in transferDb.EmailLists
                             where em.el_active == true
                             select em;

                if (emails.Count() == 0)
                {
                    Trace.WriteLine("No email addresses found", "EML");
                    Program.LogSafe("BtRetryService: No email addresses found.", EventLogEntryType.Warning);
                    return;
                }

                var generator = new RazorTemplateGenerator();

                //em.el_body_template -> "
                //<table>
                //      <tr><th>iso</th><th>date</th><th>description</th><th>created by</th></tr>
                //@foreach (var i in Model.Results)      
                // {<tr>
                //      <td>@i.vt_v_country_iso_id</td><td>@i.vt_last_modification_date</td><td>@i.vt_status_description</td><td>@i.vt_created_by</td>
                //  </tr>}  
                //</table>"
                foreach (var em in emails)
                    generator.RegisterTemplate<Data>("__" + em.el_id, em.el_body_template);

                generator.CompileTemplates();

                var date = Force ? ForceDate : DateTime.Now.Yesterday();

                foreach (var em in emails)
                {
                    if (!em.IsValid())
                    {
                        Trace.WriteLine("Invalid emailList entry: ".concat(em.ToString()), "EML");
                        Program.LogSafe("BtRetryService: Invalid emailList entry: ".concat(em.ToString()), EventLogEntryType.Warning);
                        continue;
                    }

                    int iso = em.el_iso_id;
                    int em_id = em.el_id;

                    Data data = new Data();
                    data.Db = transferDb;

                    foreach (var filter in em.el_filter.Split(';'))
                    {
                        var voucherTransfers = from vt in transferDb.VoucherTransfers
                                               where vt.vt_v_country_iso_id == iso &&
                                                   //select yesterday's transfers only
                                                    vt.vt_last_modification_date >= date
                                               select vt;
                        CString str = "vt_status_description != null && (" + filter + ")";
                        var voucherTransfers2 = voucherTransfers.Where(str);
                        data.Results.AddRange(voucherTransfers2);
                    }

                    if (!data.IsEmpty)
                    {
                        //em.el_list - > rrossenbg@yahoo.com,rrossenbg@gmail.com,rosen.rusev@uk.premiertaxfree.com
                        //em.el_subject -> Error in voucher import
                        var emd = new EmailData { AddrList = (EmailMe ? Strings.ROSENRUSEV : em.el_list), Subject = em.el_subject };
                        emd.Body = generator.GenerateOutput(data, "__" + em.el_id);

                        Trace.WriteLine("emailing:".concat(em.el_list), "EML");

                        var task = Task.Factory.StartNew<bool>((o) =>
                        {
                            var ed = (EmailData)o;
                            Program.LogSafe("Email : ".concat(ed.AddrList.FirstOf(35), "..."), EventLogEntryType.Information);
                            EmailSender.SendSafe(ed.AddrList, ed.Subject, ed.Body, true);
                            return true;
                        }, emd);
                        emailTasks.Add(task);
                    }

                    thread.Yield();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "EML");
                Program.LogSafe(ex.ToString(), EventLogEntryType.Error);
            }
            finally
            {
                Task.WaitAll(emailTasks.ToArray());
                Trace.WriteLine("Mailer stopped.", "EML");
                Program.LogSafe("Mailer stopped.", EventLogEntryType.Information);
            }
        }

        protected override void FireStarted()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Email data class
    /// </summary>
    public class EmailData
    {
        /// <summary>
        /// Comma separated list of emails
        /// </summary>
        public string AddrList { get; set; }
        /// <summary>
        /// Email subject. May not be null
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Email body text
        /// </summary>
        public string Body { get; set; }
    }

    /// <summary>
    /// Razor template data class
    /// </summary>
    public class Data
    {
        /// <summary>
        /// VoucherTransfer quierible
        /// </summary>
        public List<VoucherTransfer> Results { get; set; }

        public TransferDbEntities Db { get; set; }

        public bool IsEmpty { get { return Results.Count == 0; } }

        public Data()
        {
            Results = new List<VoucherTransfer>();
        }
    }
}
