/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Native;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.Scan;

namespace PremierTaxFree
{
    public class ScanForm : Form, IMessageFilter
    {
        private Twain m_twain;
        private IntPtr m_MainFormHandler;
        private Voucher VoucherData { get; set; }
        private static readonly Hashtable ms_Aliases = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
        private static ScanForm ms_Current;

        static ScanForm()
        {
            ms_Aliases["[DATE]"] = new Func<string>(() => DateTime.Now.ToString());
            ms_Aliases["[OPERATOR]"] = new Func<string>(() => SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default).Name);
            ms_Aliases["[PCNAME]"] = new Func<string>(() => PTFUtils.GetMachine());
            ms_Aliases["[SITEID]"] = new Func<string>(() => { Debug.Assert(ms_Current != null && ms_Current.VoucherData != null); return ms_Current.VoucherData.SiteCode; });
            ms_Aliases["[NUMBER]"] = new Func<string>(() => Convert.ToString(SettingsTable.Get<int>(Strings.VScan_ScanCount, 0)));
        }

        public ScanForm(IntPtr mainForm)
        {
            this.components = new System.ComponentModel.Container();
            InitializeComponent();
            ms_Current = this;
            this.Text = Application.ProductName.concat("-Scanner");

            bool defaultScanner = SettingsTable.Get<bool>(Strings.VScan_TWAINUseDefaultScanner, false);
            bool defaultScannerSettings = SettingsTable.Get<bool>(Strings.VScan_TWAINUseDefaultScannerSettings, false);
            this.ShowIcon = true;
            this.ShowInTaskbar = true;

            m_MainFormHandler = mainForm;

            m_twain = new Twain();
            m_twain.MainForm = mainForm;
            m_twain.ScanForm = this.Handle;
            m_twain.Form = this;
            m_twain.Init(this.Handle);

            if (defaultScanner)
            {
                var name = SettingsTable.Get<string>(Strings.VScan_ScannerName, Strings.VScan_DefaultScannerName);
                m_twain.Select(name);
            }

            AcquireVoucher(defaultScannerSettings);
        }

        public List<string> SelectAllScanner()
        {
            var result = new List<string>(m_twain.Select("<NA>"));
            return result;
        }

        protected override void OnLoad(EventArgs e)
        {            
            this.DesktopLocation = new Point(-10000, -10000);
            this.Visible = false;
            Application.AddMessageFilter(this);
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            m_twain.CloseDS();
            Application.RemoveMessageFilter(this);
            base.OnClosed(e);
        }

        private void AcquireVoucher(bool defaultScannerSettings)
        {
            bool useImprinter = SettingsTable.Get<bool>(Strings.VScan_UseImPrinter, false);
            string voucherText = null;

            if (useImprinter)
            {
                VoucherData = ScanAppContext.FillFromScanContext(new Voucher());
                string voucherTemplate = SettingsTable.Get<string>(Strings.VScan_ImPrinterTemplate, Strings.VScan_ImPrinterTemplateDefault);
                voucherText = voucherTemplate.format(ms_Aliases);
            }
            m_twain.Acquire(defaultScannerSettings, voucherText);
        }

        public bool PreFilterMessage(ref Message m)
        {
            TwainCommand cmd = m_twain.PassMessage(ref m);
            if (cmd == TwainCommand.Not)
                return false;

            switch (cmd)
            {
                case TwainCommand.TransferReady:
                    {
                        bool status = m_twain.TransferPictures(VoucherData);
                        if (status)
                        {
                            AcquireVoucher(true);
                        }
                        else
                        {
                            m_twain.CloseDS();
                            m_twain.CloseDSM();
                            Close();
                        }
                        break;
                    }
                case TwainCommand.CloseRequest:
                    {
                        m_twain.CloseDS();
                        m_twain.CloseDSM();
                        Close();
                        break;
                    }
                case TwainCommand.CloseOk:
                    {
                        WinMsg.SendText(m_MainFormHandler, Strings.VScan_ScanIsDoneEvent);
                        Close();
                        break;
                    }
                case TwainCommand.DeviceEvent:
                    break;
                default:
                    break;
            }

            return true;
        }

        private static Thread ms_Thread;

        public static void Start(IntPtr hWnd)
        {
            if (ms_Thread != null)
                return;

            IntPtr handle = hWnd;
            ms_Thread = new Thread((h) =>
            {
                try
                {
                    ScanForm form = new ScanForm((IntPtr)h);                    
                    Application.Run(form);
                }
                catch (Exception ex)
                {
                    ex.ThrowAndForget();
                }
                finally
                {
                    ms_Thread = null;
                }
            }) { IsBackground = true };
            ms_Thread.Start(handle);
        }

        private System.ComponentModel.IContainer components;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case user32.WM_COPYDATA:
                    {
                        IntPtr sender;
                        string text = WinMsg.ReceiveText(m.LParam, out sender);
                        if (text.CompareNoCase(Strings.VScan_StopTheScanner))
                            Close();
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanForm));
            this.SuspendLayout();
            // 
            // ScanForm
            // 
            this.ClientSize = new System.Drawing.Size(193, 0);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScanForm";
            this.ResumeLayout(false);
        }
    }
}
