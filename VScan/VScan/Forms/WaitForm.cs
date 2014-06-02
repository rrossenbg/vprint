/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.Extensions;

namespace PremierTaxFree
{
    public partial class WaitForm : Form
    {
        private static Hashtable ms_forms = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int MaxDelay
        {
            get { return timer1.Interval; }
            set { timer1.Interval = value; }
        }

        public WaitForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            timer1.Start();
            base.OnLoad(e);
        }

        public static void Start(Form owner)
        {
            Start(owner, Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds));
        }

        public static void Start(Form owner, int delay)
        {
            owner.FormClosed += new FormClosedEventHandler(Owner_FormClosed);
            WaitForm form = new WaitForm();
            form.MaxDelay = delay;
            ms_forms[owner] = form;
            form.ShowDialog(owner);
        }

        public static void StartAsync(Form owner)
        {
            Point point = owner.DesktopLocation;
            Size size = owner.Size;

            new Thread(() =>
            {
                WaitForm form = new WaitForm();
                point.Offset(size.Width, size.Height);
                form.DesktopLocation = point;

                ms_forms[owner] = form;
                owner.FormClosed += new FormClosedEventHandler(form.CloseAsyncForm);
                Application.Run(form);
                owner.FormClosed -= new FormClosedEventHandler(form.CloseAsyncForm);
            }) { IsBackground = true }.Start();
        }

        public static void Stop(Form owner)
        {
            if (owner == null)
                return;

            Form form = ms_forms[owner] as Form;
            if (form != null)
            {
                owner.FormClosed -= new FormClosedEventHandler(Owner_FormClosed);
                form.InvokeSf(new MethodInvoker(() => { form.Close(); }));
            }
            ms_forms.Remove(owner);
        }

        protected void CloseAsyncForm(object sender, EventArgs e)
        {
            this.InvokeSf(new MethodInvoker(() => { this.Close(); }));
        }

        private static void Owner_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop((Form)sender);
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
