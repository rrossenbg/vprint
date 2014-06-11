/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Text;
using Microsoft.Reporting.WebForms;

namespace PTF.Reports
{
    public static class ReportViewerEx
    {
        public static string toString(this ReportParameter para, string line = null)
        {
            StringBuilder b = new StringBuilder(line);
            b.AppendFormat("Name: {0}\r\n", para.Name);
            b.AppendFormat("Visible: {0}\r\n", para.Visible);
            foreach (string value in para.Values)
                b.AppendFormat("Value: {0}\r\n", value);
            return b.ToString();
        }
    }
}