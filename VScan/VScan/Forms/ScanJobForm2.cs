/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PremierTaxFree.Controls;
using PremierTaxFree.Data;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data.Objects.Server;

namespace PremierTaxFree.Forms
{
    public partial class ScanJobForm2 : Form
    {
        public eVouchersScanType ScanType { get; private set; }
        public string SiteCodeString { get; private set; }
        public int VoucherStartNumber { get; private set; }
        public string Comment { get { return ptbComment.Text; } }

        public ScanJobForm2()
        {
            InitializeComponent();
            ScanType = eVouchersScanType.AfterInsertionDomestic;
        }

        protected override void OnLoad(EventArgs e)
        {
            new Func<int?, string[]>((s) => ScanAppContext.QueryCountryCodes(s)).RunAsync(
                new Action<string[]>((sc) =>
            this.InvokeSf(new MethodInvoker(() => { cbSiteCodes.DataSource = sc; }))), null);

            CreateAccordion1();
            CreateAccordion2();
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = (this.DialogResult == DialogResult.OK && !ValidateChildren());
            base.OnClosing(e);
        }

        public override bool ValidateChildren()
        {
            err.SetError(cbSiteCodes, null);
            err.SetError(mtxtAuditID, null);
            err.SetError(cbConfirmBefore, null);

            int number;

            switch (ScanType)
            {
                case eVouchersScanType.NotSet:
                    return false;
                case eVouchersScanType.AfterInsertionDomestic:
                    {
                        if (cbSiteCodes.SelectedItem == null)
                        {
                            err.SetError(cbSiteCodes, Resources.sProvide_SiteCode);
                            return false;
                        }
                        else if (!int.TryParse(mtxtFileID.Text, out number))
                        {
                            err.SetError(mtxtFileID, Resources.sProvide_FileID);
                            return false;
                        }

                        SiteCodeString = ((int)cbSiteCodes.SelectedItem).ToStringSf();
                        VoucherStartNumber = number;
                    }
                    break;
                case eVouchersScanType.AfterInsertionForeign:
                    {
                        if (!int.TryParse(mtxtAuditID.Text, out number))
                        {
                            err.SetError(mtxtAuditID, Resources.sProvide_AuditID);
                            return false;
                        }

                        VoucherStartNumber = number;
                    }
                    break;
                case eVouchersScanType.BeforeInsertion:
                    {
                        if (!cbConfirmBefore.Checked)
                        {
                            err.SetError(cbConfirmBefore, Resources.sConfirm_VouchersAreNotInserted);
                            return false;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        private void CreateAccordion1()
        {
            accordion1.Dock = DockStyle.Fill;

            foreach (var obj in new Control[] { pnlScanAfter, pnlScanBefore })
                accordion1.Add(CreateContent(obj));
        }

        private void CreateAccordion2()
        {
            accordion2.Dock = DockStyle.Fill;

            foreach (var obj in new Control[] { pnlLocalVouchers, pnlForeignVouchers })
                accordion2.Add(CreateContent(obj));
        }

        private Expander CreateContent(Control cnt)
        {
            Expander expander = new Expander();
            expander.BorderStyle = BorderStyle.FixedSingle;
            var lbl = ExpanderHelper.CreateLabelHeader(expander, cnt.Text ?? Convert.ToString(cnt.Tag), SystemColors.ActiveBorder);
            lbl.Tag = cnt;
            lbl.Click += new EventHandler(Header_Click);

            cnt.Parent.Controls.Remove(cnt);
            cnt.Dock = DockStyle.Fill;
            cnt.Text = null;
            expander.Content = cnt;
            return expander;
        }

        private void Header_Click(object sender, EventArgs e)
        {
            Control cnt = (Control)sender;
            Control tagCnt = (Control)cnt.Tag;

            if (tagCnt == pnlLocalVouchers)
            {
                ScanType = eVouchersScanType.AfterInsertionDomestic;
            }
            else if (tagCnt == pnlForeignVouchers)
            {
                ScanType = eVouchersScanType.AfterInsertionForeign;
            }
            else if (tagCnt == pnlScanBefore)
            {
                ScanType = eVouchersScanType.BeforeInsertion;
            }
            else
            {
                ScanType = eVouchersScanType.NotSet;
            }
        }

        public static bool show(IWin32Window owner)
        {
            Debug.Assert(owner != null);

            using (ScanJobForm2 form = new ScanJobForm2())
            {
                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    ScanAppContext.Default.CurrentScan.SetNew(form.ScanType, form.SiteCodeString, form.VoucherStartNumber - 1, form.Comment);
                    return true;
                }
                return false;
            }
        }
    }
}
