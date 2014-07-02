/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using VPrinting.Razor.RazorTemplating;

namespace VPrinting.Documents
{
    [Serializable]
    public abstract class VoucherPrintLayoutRaz_Base : VoucherPrinterSettings, IVoucherLayout
    {
        public string DocumentInitialization { get; set; }
        public int FormLength { get; set; }
        public string Context { get; set; }
        public string TemplateName { get; set; }
        public Point MoveAll { get; set; }

        [XmlIgnore]
        public abstract List<IPrintLine> PrintLines { get; }

        protected IRazorTemplateGenerator m_Generator;
        protected string m_Output;

        public void InitPrinter(string printDoc)
        {
            //Do nothing here
        }

        public virtual void Clear()
        {
            m_Output = string.Empty;
        }

        public abstract void Init();

        public virtual void DataBind(IDataProvider pr, string voucherNo, int voucher, bool printDemo)
        {
            if (m_Generator == null)
                throw new ArgumentNullException("m_Generator");

            m_Output = m_Generator.GenerateOutput(pr);
        }

        public abstract void PrintVoucher(string printerName, string printDocName, int length, string docInitialization, IList<IPrintLine> lines);
    }
}
