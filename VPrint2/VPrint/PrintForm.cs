/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;

namespace VPrinting
{
    public partial class PrintForm : Form
    {
        public static ArrayList OpenForms = ArrayList.Synchronized(new ArrayList());

        public PrintState State { get; set; }

        private long m_ActiveJobs = 0, m_TotalJobs = 0;
        private long m_totalJobs;

        public PrintForm()
        {
            InitializeComponent();
            this.Location = new Point(-1000, -1000);
            this.ShowInTaskbar = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OpenForms.Add(this);

            const int MAX_PRINT_JOBS = 100;

            var voucherNumberAllocation = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
            var allocation = voucherNumberAllocation.RetrieveAllocation(State.AllocationID);
            m_totalJobs = allocation.RangeTo - allocation.RangeFrom + 1;

            for (int current = allocation.RangeFrom; current <= allocation.RangeTo; current++)
            {
                Interlocked.Increment(ref m_ActiveJobs);
                Interlocked.Increment(ref m_TotalJobs);
                //Maximum 
                while (Interlocked.Read(ref m_ActiveJobs) > MAX_PRINT_JOBS)
                {
                    Application.DoEvents();
                    Thread.Sleep(1000);
                }

                var eng = new VoucherPrintEngine(State.DocumentLayout);
                eng.DataObject = allocation;
                eng.EndPrint += new PrintEventHandler(Document_EndPrint);
                eng.Print();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            OpenForms.Remove(this);
        }

        private void Document_EndPrint(object sender, PrintEventArgs e)
        {
            PrintDocument eng = (PrintDocument)sender;
            //Remove binding and let the object die
            eng.EndPrint -= new PrintEventHandler(Document_EndPrint);

            var voucherNumberAllocation = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
            voucherNumberAllocation.LogVoucherAllocationPrinted(State.AllocationID,
                Program.currentUser.UserID, Program.currentUser.CountryID);
            voucherNumberAllocation.SetVoucherAllocationPrinted(State.AllocationID, true,
                Program.currentUser.UserID);//set the printed status to true

            State.DocumentDone(this, EventArgs.Empty);

            Interlocked.Decrement(ref m_ActiveJobs);

            if (Interlocked.Read(ref m_TotalJobs) == m_totalJobs)
                Close();
        }

        public static void Start(PrintState st)
        {
            var th = new Thread((o) =>
            {
                PrintState state = (PrintState)o;
                PrintForm form = new PrintForm();
                form.State = state;
                Application.Run(form);
            });
            th.IsBackground = true;
            th.Start(st);
        }
    }

    public class PrintState
    {
        public int AllocationID { get; set; }
        public int CountryID { get; set; }
        public bool IsDoubleSale { get; set; }
        public AllocationDocumentLayout DocumentLayout { get; set; }
        public EventHandler DocumentDone { get; set; }
    }
}
