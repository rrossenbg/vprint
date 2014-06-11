using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PTF.Reports;
using PTF.Reports.Data;
using PTF.Reports.PTFReportsDB;
using rpt = PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class ReportModel
    {
        public Guid ReportID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Url")]
        public string Page { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Folder")]
        public string Folder { get; set; }

        public static explicit operator ReportModel(rpt.Report p1)
        {
            var rm = new ReportModel()
            {
                ReportID = p1.ReportID,
                Name = p1.Name,
                Page = p1.Page,
                Description = p1.Description,
                Folder = p1.Folder.Name,
            };
            return rm;
        }

        //public static explicit operator List<Parameter>(ReportModel p1)
        //{
        //    var pr1 = new Parameter { Name = p1.Name1 ?? "", Text = p1.Text1 ?? "", Description = p1.Description1, ParamType = p1.ParamType1.GetValueOrDefault() };
        //    var pr2 = new Parameter { Name = p1.Name2 ?? "", Text = p1.Text2 ?? "", Description = p1.Description2, ParamType = p1.ParamType2.GetValueOrDefault() };
        //    var pr3 = new Parameter { Name = p1.Name3 ?? "", Text = p1.Text3 ?? "", Description = p1.Description3, ParamType = p1.ParamType3.GetValueOrDefault() };
        //    var pr4 = new Parameter { Name = p1.Name4 ?? "", Text = p1.Text4 ?? "", Description = p1.Description4, ParamType = p1.ParamType4.GetValueOrDefault() };
        //    var pr5 = new Parameter { Name = p1.Name5 ?? "", Text = p1.Text5 ?? "", Description = p1.Description5, ParamType = p1.ParamType5.GetValueOrDefault() };
        //    var pr6 = new Parameter { Name = p1.Name6 ?? "", Text = p1.Text6 ?? "", Description = p1.Description6, ParamType = p1.ParamType6.GetValueOrDefault() };
        //    return new List<Parameter>() { pr1, pr2, pr3, pr4, pr5, pr6 }.Where(p => p.IsValid).ToList();
        //}
    }
}