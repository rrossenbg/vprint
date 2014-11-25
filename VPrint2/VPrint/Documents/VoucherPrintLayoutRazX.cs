/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml.Serialization;
using VPrinting.Common;
using VPrinting.Razor.RazorTemplating;
using VPrinting.Tools;

namespace VPrinting.Documents
{
    /// <summary>
    /// Graphics printing. Razor implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class VoucherPrintLayoutRazX : VoucherPrintLayoutRaz_Base
    {
        private readonly List<IPrintLine> m_PrintLines = new List<IPrintLine>();

        public Size PageSize { get; set; }
        public bool Landscape { get; set; }

        [DefaultValue(1)]
        public int PageCount { get; set; }

        [XmlIgnore]
        public override List<IPrintLine> PrintLines
        {
            get { return m_PrintLines; }
        }

        public VoucherPrintLayoutRazX()
        {
            PageCount = 1;
        }

        public override void Init()
        {
            if (string.IsNullOrWhiteSpace(Context))
                throw new ArgumentNullException("Context");

            if (string.IsNullOrWhiteSpace(TemplateName))
                throw new ArgumentNullException("TemplateName");

            CacheManager.Instance.Table[Strings.CurrentPrintedPage] = 0;

            if (!CacheManager.Instance.Table.ContainsKey(TemplateName))
            {
                try
                {
                    m_Generator = new RazorTemplateGenerator();
                    m_Generator.RegisterTemplate<IDataProvider>(Context);
                    m_Generator.CompileTemplates();

                    CacheManager.Instance.Table[TemplateName] = m_Generator;
                }
                catch (TemplateCompileException ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            else
            {
                m_Generator = (RazorTemplateGenerator)CacheManager.Instance.Table[TemplateName];
            }
        }

        public override void Clear()
        {
            m_PrintLines.Clear();
            base.Clear();
        }

        public override void DataBind(IDataProvider pr, string voucherNo, int voucher, bool printDemo)
        {
            base.DataBind(pr, voucherNo, voucher, printDemo);

            if (string.IsNullOrWhiteSpace(m_Output))
                throw new ArgumentNullException("m_Output");

            XmlSerializer formatter = new XmlSerializer(typeof(VoucherPrintRazX));
            var voucherPrintObj = formatter.ToObject<VoucherPrintRazX>(m_Output.TrimStart());
            if (voucherPrintObj == null)
                throw new ArgumentNullException("voucherPrintObj", "Can not create voucherPrintObj from xml");

            if (voucherPrintObj.Lines.IsNullOrEmpty())
                throw new ArgumentNullException("voucherPrintObj.Lines", "voucherPrintObj.Lines can not be null or empty");

            m_PrintLines.AddRange(voucherPrintObj.Lines);
            m_PrintLines.AddRange(voucherPrintObj.Barcodes);
        }

        //public override void PrintVoucher(string printerName, string printDocName, 
        //    int length, string docInitialization, IList<IPrintLine> lines)
        //{
        //    for (int page = 0; page < PageCount; page++)
        //    {
        //        using (var doc = new EscapePrintDocument())
        //        {
        //            doc.DocumentName = printDocName;
        //            doc.PrintController = new StandardPrintController();
        //            doc.PrinterSettings.PrinterName = printerName;
        //            doc.DefaultPageSettings.PaperSize = 
        //                new PaperSize("CustomPaper", PageSize.Width, PageSize.Height);
        //            doc.DefaultPageSettings.Landscape = Landscape;

        //            this.Tag = lines;
        //            doc.PrintPage += DelegateHelper.CreatePrintPageEventHandler;
        //            doc.Print();
        //            doc.PrintPage -= DelegateHelper.CreatePrintPageEventHandler;
        //        }
        //    }
        //}

        public override void PrintVouchers(string printerName, string printDocName, 
            int length, string docInitialization, Queue<IList<IPrintLine>> multilines)
        {
            using (var doc = new PrintDocument())
            {
                doc.DocumentName = printDocName;
                doc.PrintController = new StandardPrintController();
                doc.PrinterSettings.PrinterName = printerName;
                doc.DefaultPageSettings.PaperSize =
                    new PaperSize("CustomPaper", PageSize.Width, PageSize.Height);
                doc.DefaultPageSettings.Landscape = Landscape;

                this.Tag = multilines;
                doc.PrintPage += DelegateHelper.CreatePrintPageMultyEventHandler;
                doc.Print();
                doc.PrintPage -= DelegateHelper.CreatePrintPageMultyEventHandler;
            }
        }

        public override string ToString()
        {
            return m_Output;
        }
    }

    /// <summary>
    /// Serialized sub-document
    /// </summary>
    [Serializable]
    public class VoucherPrintRazX
    {
        public List<GPrintLine> Lines { get; set; }
        public List<BarPrintLine> Barcodes { get; set; }
    }
}
