/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;

using PremierTaxFree.PTFLib.Printing;
using PremierTaxFree.PTFLib.Security;
using PremierTaxFree.Sys;

namespace PremierTaxFree.Controls
{
    public partial class SettingsPage3 : UserControl, ISettingsControl
    {
        public SettingsPage3()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            extender.Control = this;
        }

        public bool IsDirty { get; set; }

        public void Read()
        {
            ptbScanner.Text = SettingsTable.Get<string>(Strings.VScan_ScannerName, Strings.VScan_DefaultScannerName);
            ptbWebTracerUrl.Text = SettingsTable.Get<string>(Strings.VScan_WebTracerUrl,
                Strings.VScan_WebTracerUrlDefault);

            string url = SettingsTable.Get<string>(Strings.VScan_TRS_UrlAddress, Strings.VScan_TRS_UrlAddressDefault);
            ptbTRSURL.Text = url;

            PreparePrinterComboBox();
            PrepareScannerComboBox();
        }

        public bool Verify()
        {
            var uri1 = new Uri(ptbWebTracerUrl.Text, UriKind.Absolute);
            var uri2 = new Uri(ptbTRSURL.Text, UriKind.Absolute);
            return true;
        }

        public void Save()
        {
            SettingsTable.Set(Strings.VScan_ScannerName, ptbScanner.Text);
            SettingsTable.Set(Strings.VScan_WebTracerUrl, ptbWebTracerUrl.Text);
            SettingsTable.Set(Strings.VScan_TRS_UrlAddress, ptbTRSURL.Text);
        }

        public void UpdateEnvironment()
        {
        }

        private void InstallTracer_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, 
                "Please be informed.\r\n" +
                "To uninstall the tracer you'll need\r\n" + 
                "to restart the application\r\n" + 
                "Would you like to continue?", 
                Application.ProductName, 
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
            }
        }

        private void TestConnection_Click(object sender, EventArgs e)
        {
            new NotImplementedException().ThrowAndForget();
        }

        private void Printers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string printerName = Convert.ToString(cbPrinters.SelectedItem);

            if (!string.IsNullOrEmpty(printerName))
            {
                DialogResult res = this.ShowQuestion("You selected application printer. \r\n " + 
                                                         "Would you like this printer\r\n" + 
                                                         "to be set as system default?", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    if (PrintManager.SetDefaultPrinter(printerName))
                    {
                        this.ShowInformation("Default printer was changed successfully!");
                    }
                }

                SettingsTable.Set(Strings.VScan_ApplicationPrinterName, printerName);

                PreparePrinterComboBox();
            }
        }

        private void Scanners_SelectedIndexChanged(object sender, EventArgs e)
        {
            var scanner = cbPrinters.SelectedItem.ToStringSf();
            if (!string.IsNullOrEmpty(scanner))
                SettingsTable.Set(Strings.VScan_ScannerName, scanner);
        }

        private void PrepareScannerComboBox()
        {
            var scannerName = SettingsTable.Get<string>(Strings.VScan_ScannerName, Strings.VScan_DefaultScannerName);

            //this.cbScanners.SelectedIndexChanged -= new System.EventHandler(this.Scanners_SelectedIndexChanged);

            this.cbScanners.Clear();

            foreach (var scanner in ScannerInfo.SelectInstalled())
            {
                int index = cbScanners.Items.Add(scanner);

            }

            this.cbScanners.SelectedItem = scannerName;
            //this.cbScanners.SelectedIndexChanged += new System.EventHandler(this.Scanners_SelectedIndexChanged);
        }

        private void PreparePrinterComboBox()
        {
            string defaultPrinterName = PrintManager.GetDefaultPrinterName();
            string applicationPrinter = SettingsTable.Get(Strings.VScan_ApplicationPrinterName, defaultPrinterName);

            this.cbPrinters.SelectedIndexChanged -= new System.EventHandler(this.Printers_SelectedIndexChanged);

            this.cbPrinters.Clear();

            foreach (string printer in PrintManager.GetInstalledPrinters())
            {
                int index = cbPrinters.Items.Add(printer);

                if (string.Equals(printer, applicationPrinter) && string.Equals(printer, defaultPrinterName))
                {
                    cbPrinters.SelectedIndex = index;
                    cbPrinters.SetItemColor(index, Color.Green);
                }
                else if (string.Equals(printer, applicationPrinter))
                {
                    //Application Printer
                    cbPrinters.SelectedIndex = index;
                    cbPrinters.SetItemColor(index, Color.Blue);
                }
                else if (string.Equals(printer, defaultPrinterName))
                {
                    //Default printer
                    cbPrinters.SetItemColor(index, Color.Red);
                }
            }

            this.cbPrinters.SelectedIndexChanged += new System.EventHandler(this.Printers_SelectedIndexChanged);
        }
    }
}
