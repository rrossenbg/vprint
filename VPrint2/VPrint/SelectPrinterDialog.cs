/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace VPrinting
{
    public partial class SelectPrinterDialog : Form
    {
        public string SelectedPrinter { get { return cbPrinters.Text; } }

        public SelectPrinterDialog()
        {
            InitializeComponent();
            PreparePrinterComboBox();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = DialogResult == DialogResult.OK && string.IsNullOrEmpty(SelectedPrinter);
            base.OnClosing(e);
        }

        private void PreparePrinterComboBox()
        {
            string defaultPrinterName = PrintManager.GetDefaultPrinterName();

            this.cbPrinters.Items.Clear();

            foreach (string printer in PrintManager.GetInstalledPrinters())
            {
                int index = cbPrinters.Items.Add(printer);

                if (string.Equals(printer, defaultPrinterName))
                {
                    cbPrinters.SelectedIndex = index;
                    cbPrinters.SetItemColor(index, Color.Red);
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            DialogResult = sender == btnOK ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        public static bool Show(IWin32Window owner, ref string printerName)
        {
            using (SelectPrinterDialog dlg = new SelectPrinterDialog())
            {
                if (dlg.ShowDialog(owner) == DialogResult.OK)
                {
                    printerName = dlg.SelectedPrinter;
                    return true;
                }
            }
            return false;
        }
    }
}
