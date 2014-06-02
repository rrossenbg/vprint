using System;
using System.Text;
using System.Windows.Forms;
using VPrinting.Documents;

namespace VPrinting
{
    public partial class MapPrinterForm : Form
    {
        public MapPrinterForm()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            const string LINE = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int hLength = tbHorizontalLength.Text.Cast<int>();
            int vLength = tbVerticaLength.Text.Cast<int>();
            int spaces = tbSpaces.Text.Cast<int>(); //2
            int tabs = tbTabs.Text.Cast<int>(); //1
            int numberOflines = tbNumberOfLines.Text.Cast<int>();
            int barcodePos = tbBarcodePos.Text.Cast<int>();

            StringBuilder b = new StringBuilder();

            if (cbPrintBarcode.Checked)
            {
                b.Append(MTPL.SetAbsoluteHorizontalPosition(barcodePos));
                b.Append(MTPL.PrintI2Of5Barcode("123456789", 10, "000"));
                b.AppendLine();
            }

            for (int i = 0; i < vLength; i++)
            {
                b.Append(LINE.CharOfString(i));

                if (i % numberOflines == 0)
                {
                    for (int j = 0; j < hLength; j++)
                        b.Append(LINE.CharOfString(i + j + 1));
                    b.AppendLine();

                }
                else if (i % numberOflines == 1)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        b.Append(" ".Miltiply(j));
                        b.Append(LINE.CharOfString(i + j + 1));
                    }
                    b.AppendLine();
                }
                else if (i % numberOflines == 2)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        b.Append("\t".Miltiply(j));
                        b.Append(LINE.CharOfString(i + j + 1));
                    }
                    b.AppendLine();
                }
                else
                {
                    b.AppendLine();
                }
            }

            string text = b.toString();

            string printerName = tbPrinterName.Text;

            PrinterQueue.AddJob(printerName, this.Text, text);
        }
    }
}
