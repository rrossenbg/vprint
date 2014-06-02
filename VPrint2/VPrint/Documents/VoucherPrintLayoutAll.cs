using System;
using System.Collections.Generic;
using System.Text;

namespace VPrinting.Documents
{
    /// <summary>
    /// an old code
    /// </summary>
    public class VoucherPrintLayoutAll : IVoucherLayout
    {
        public string DocumentInitialization {get;set;}

        public int FormLength {get;set;}
        
        public List<IPrintLine> PrintLines
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Init()
        {
        }

        public void Clear()
        {
            
        }

        public void DataBind(IDataProvider pr, string voucherNo, int voucher, bool printDemo)
        {
            
        }

        public void InitPrinter(string printDoc)
        {
            //No implementation
        }

        public void PrintVoucher(string printName, string printDocName, int length, string docInitialization, IList<IPrintLine> lines)
        {
            StringBuilder b = new StringBuilder();

            //b.Append(ESC_P2.SetPageLengthLines(layout.FormLength));
            //b.Append(layout.Context);
            //b.Clean();

            //b.Replace("<nbsp>", " ");

            //foreach (PrintLine line in lines)
            //    b.Replace(string.Format("[{0}]", line.Description), line.Text);

            //b.Replace("<ht>", ESC_P2.HT);
            //b.Replace("<vt>", ESC_P2.VT);
            //b.Replace("<br>", ESC_P2.LF);
            //b.Append(ESC_P2.FF);

            var docText = b.ToString();

            PrinterQueue.AddJob(printName, printDocName, docText);
        } 
    }
}
