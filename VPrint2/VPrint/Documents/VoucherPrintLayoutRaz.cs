/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using VPrinting.Common;
using VPrinting.Razor.RazorTemplating;

namespace VPrinting.Documents
{
    /// <summary>
    /// Direct simple printing. Razor implementation
    /// </summary>
    /// <remarks>
    /// spaces may be a problem for this format. 
    /// try to avoid those at begging of lines
    /// </remarks>
    [Serializable]
    public class VoucherPrintLayoutRaz : VoucherPrintLayoutRaz_Base
    {
        private readonly Regex m_BodyRegEx;

        [XmlIgnore]
        public override List<IPrintLine> PrintLines
        {
            get { return null; }
        }

        public VoucherPrintLayoutRaz()
        {
            m_BodyRegEx = new Regex(@"<body>(.*)</body>",
                        RegexOptions.Compiled |
                        RegexOptions.IgnoreCase |
                        RegexOptions.CultureInvariant |
                        RegexOptions.Singleline);
        }

        public override void Init()
        {
            if (string.IsNullOrWhiteSpace(Context))
                throw new ArgumentNullException("Context");

            if (string.IsNullOrWhiteSpace(TemplateName))
                throw new ArgumentNullException("TemplateName");

            if (!CacheManager.Instance.Table.ContainsKey(TemplateName))
            {
                var ma = m_BodyRegEx.Match(Context);

                if (!ma.Groups[1].Success)
                    throw new Exception("Can not find body tag");

                string compressedContext = m_BodyRegEx.Replace(Context, new MatchEvaluator((m) =>
                {
                    string str = m.Value.Clean();
                    return str;
                }));

                try
                {
                    m_Generator = new RazorTemplateGenerator();
                    m_Generator.RegisterTemplate<IDataProvider>(compressedContext);
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

        public override void PrintVoucher(string printerName, string printDocName, int length, string docInitialization, IList<IPrintLine> lines)
        {
            var ma = m_BodyRegEx.Match(m_Output);

            if (!ma.Groups[1].Success)
                throw new Exception("Can't find body tag");

            if (string.IsNullOrWhiteSpace(ma.Groups[1].Value))
                throw new Exception("Body tag is empty");

            StringBuilder builder = new StringBuilder();

            builder.Append(ma.Groups[1].Value);
            //Replace spaces
            builder.Replace("<nbsp>", " ");
            //Replace htabs
            builder.Replace("<ht>", ASCII.HT);
            //Replace vtabs
            builder.Replace("<vt>", ASCII.VT);
            //Replace lf
            builder.Replace("<lf>", ASCII.LF);
            //Replace line breaks
            builder.Replace("<br>", ASCII.LF + ASCII.CR);
            //End of form
            builder.Append(ASCII.FF);

            var docText = builder.ToString();
            PrinterQueue.AddJob(printerName, printDocName, docText);
        }

        public override string ToString()
        {
            return m_Output;
        }
    }
}
