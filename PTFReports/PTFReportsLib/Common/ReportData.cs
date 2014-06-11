/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace PTF.Reports.Common
{
    [Serializable]
    public class ReportData
    {
        public string ReportName { get; set; }
        public string ReportPath { get; set; }
        public List<ReportParameter> List { get; set; }

        public ReportData()
        {
            List = new List<ReportParameter>();
        }
    }
}
