/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using FintraxPTFImages.ScanServiceRef;

namespace FintraxPTFImages.Models
{
    public class VoucherInfoModel : VoucherInfo
    {
    }

    public class SearchModel
    {
        [Required]
        [DataType(DataType.Text)]
        public int Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int HeadOffice { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Retailer { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }

        public SearchModel()
        {
            Country = 0;
        }

        public void Validate(ModelStateDictionary errorTable)
        {
            if (Country <= 0)
                errorTable.AddModelError("Country", "Country invalid");

            if (HeadOffice <= 0)
                errorTable.AddModelError("HeadOffice", "HeadOffice invalid");

            if (Retailer <= 0)
                errorTable.AddModelError("Retailer", "Retailer invalid");

            if (FromDate == DateTime.MinValue)
                errorTable.AddModelError("FromDate", "FromDate invalid");

            if (ToDate == DateTime.MinValue)
                errorTable.AddModelError("ToDate", "ToDate invalid");

            if (FromDate >= ToDate)
            {
                errorTable.AddModelError("FromDate", "FromDate should be less than ToDate");
                errorTable.AddModelError("ToDate", "ToDate should be greater than FromDate");
            }

            if (FromDate >= DateTime.Today)
                errorTable.AddModelError("FromDate", "FromDate should not be into the future");

            if (ToDate >= DateTime.Today)
                errorTable.AddModelError("ToDate", "ToDate should not be into the future");

            if (!errorTable.IsValid)
                errorTable.AddModelError("", "There are some errors. Please correct");
        }
    }

    public class ShowModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Id { get; set; }

        public ShowModel(string name, string path)
        {
            Name = name;
            Path = path;
            Id = "".Unique();
        }
    }

    public class ShowXmlModel : ShowModel
    {
        public int JobID { get; set; }
        public int CountryID { get; set; }
        public int RetailerID { get; set; }
        public int VoucherID { get; set; }
        public string SiteCode { get; set; }
        public string BarCode { get; set; }
        public int OperatorID { get; set; }
        public int LocationID { get; set; }
        public string SessionID { get; set; }
        public DateTime CreateAt { get; set; }

        public ShowXmlModel(string name, string path)
            : base(name, path)
        {
        }

        public void Load()
        {
            XDocument xml = XDocument.Load(Path);
            JobID = xml.Root.ElementThrow("JobID").Value.ConvertTo<string, int>("JobID");
            CountryID = xml.Root.ElementThrow("CountryID").Value.ConvertTo<string, int>("CountryID");
            RetailerID = xml.Root.ElementThrow("RetailerID").Value.ConvertTo<string, int>("RetailerID");
            VoucherID = xml.Root.ElementThrow("VoucherID").Value.ConvertTo<string, int>("VoucherID");
            SiteCode = xml.Root.ElementThrow("SiteCode").Value;
            BarCode = xml.Root.ElementThrow("BarCode").Value;
            OperatorID = xml.Root.ElementThrow("OperatorID").Value.ConvertTo<string, int>("OperatorID");
            LocationID = xml.Root.ElementThrow("LocationID").Value.ConvertTo<string, int>("LocationID");
            SessionID = xml.Root.ElementValueOrDefault("SessionID", () => new List<string>(new FileInfo(Path).PathParts()).Last());
        }
    }
}