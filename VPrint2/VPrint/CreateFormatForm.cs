/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VPrinting.Data;
using VPrinting.Documents;
using VPrinting.ScanServiceRef;
using mng = VPrinting.PartyManagement;
using vp = VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting
{
    public partial class CreateFormatForm : Form
    {
        public string XmlText
        {
            set
            {
                txtXmlText.Text = value;
            }
        }

        public CreateFormatForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            ReloadLayouts();

            base.OnLoad(e);
        }

        private void ReloadLayouts()
        {
            cbCountryID.SelectedValueChanged -= new EventHandler(cbCountryID_SelectedValueChanged);
            cbCountryID.Items.Clear();

            var service = new mng.PartyManagement();
            var countries = service.GetPtfCountryList().OrderBy(c => c.Nationality);
            foreach (var country in countries)
                cbCountryID.Items.Add(country);

            cbCountryID.SelectedValueChanged += new EventHandler(cbCountryID_SelectedValueChanged);
        }

        private void cbCountryID_SelectedValueChanged(object sender, EventArgs e)
        {
            cbDocName.SelectedValueChanged -= new EventHandler(cbDocName_SelectedValueChanged);
            cbDocName.Items.Clear();
            cbDocName.Text = null;
            txtType.Text = null;
            txtXmlText.Text = null;

            mng.CountryDetail detail = (mng.CountryDetail)cbCountryID.SelectedItem;
            if (detail != null)
            {
                var service2 = new vp.VoucherNumberingAllocationPrinting();
                var formats = service2.GetSavedVoucherFormats(detail.Number);
                foreach (var format in formats)
                    cbDocName.Items.Add(format);
            }

            cbDocName.SelectedValueChanged += new EventHandler(cbDocName_SelectedValueChanged);
        }

        private void cbDocName_SelectedValueChanged(object sender, EventArgs e)
        {
            vp.PrinterFormat format = (vp.PrinterFormat)cbDocName.SelectedItem;
            if (format != null)
            {
                txtType.Text = format.Type2;
                txtXmlText.Text = format.Xml;
                txtXmlText.refresh();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbDocName.Text))
            {
                this.ShowExclamation("Doc name may not be empty.");
                return;
            }

            if (string.IsNullOrEmpty(txtType.Text))
            {
                this.ShowExclamation("Voucher type should be a valid C# class name.");
                return;
            }

            if (Type.GetType(txtType.Text) == null)
            {
                this.ShowExclamation("Can not create type of: " + txtType.Text);
                return;
            }

            mng.CountryDetail detail = (mng.CountryDetail)cbCountryID.SelectedItem;
            if (detail == null)
            {
                this.ShowExclamation("No country selected");
                return;
            }

            txtXmlText.refresh();

            var service = new vp.VoucherNumberingAllocationPrinting();
            service.SaveVoucherFormat(cbDocName.Text, detail.Number, txtXmlText.Text, 0, txtType.Text);

            ServiceDataAccess.Instance.LogOperation(OperationHistory.ScanLayoutUpdate, Program.SessionId, detail.Number, 0, 0, 0, 0, cbDocName.Text.join("-", txtType.Text));

            this.ShowInfo("Format '" + cbDocName.Text + "' saved.");

            ReloadLayouts();
        }

        private void btnTryPrint_Click(object sender, EventArgs e)
        {
            int allocationId = 0;
            if (int.TryParse(tbAllocationId.Text, out allocationId))
            {
                string filePath = Path.GetTempFileName();
                File.WriteAllText(filePath, txtXmlText.Text);

                VoucherPrinter printer = new VoucherPrinter();
                printer.m_PrinterName = ConfigurationManager.AppSettings["PrinterName"];
                printer.m_ReportType2 = txtType.Text; //"VPrinting.Documents.VoucherPrintLayoutUnitRazX";
                printer.m_PrinterXmlFilePath = filePath;
                printer.PrintOnce = true;
                printer.UseLocalFormat = true;
                printer.UseLocalPrinter = true;
                printer.SimulatePrint = false;
                printer.PrintAllocation(allocationId, false);
            }
            else
            {
                this.ShowExclamation("Can't parse allocation id.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
