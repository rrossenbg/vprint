using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PTF.Reports.Models
{
    public class ReportParametersModel
    {
        public string CurrentStep { get; set; }
        public string Page { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [DisplayName("Country")]
        public string Country { get; set; }

        [Required]
        [DisplayName("Company")]
        public string Company { get; set; }

        [Required]
        [DisplayName("Retailer")]
        public string Retailer { get; set; }

        public Guid ReportID { get; set; }

        public int CountryID { get; set; }
        public bool SkipCountry { get; set; }
        public int CompanyID { get; set; }
        public bool SkipCompany { get; set; }
        public List<int> RetailerID { get; set; }
        public int LanguageID { get; set; }

        /// <summary>
        /// Name of Country ReportParam (ISO_ID)
        /// </summary>
        public string CountryParamID { get; set; }
        /// <summary>
        /// Name of Company ReportParam (ho_id)
        /// </summary>
        public string CompanyParamID { get; set; }
        /// <summary>
        /// Name of Retailer Report Param (br_id)
        /// </summary>
        public string RetailerParamID { get; set; }
        /// <summary>
        /// Name of Language Report Param
        /// </summary>
        public string LanguageParam { get; set; }

        public ReportParametersModel()
        {
            Page = Name = Description = string.Empty;
        }
    }

    public class DropDownInfo
    {
        public string Name { get; set; }
        public ParamMapping ControlID { get; set; }
        public string ID { get; set; }
        public SelectList Values { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValue()
        {
        }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public static KeyValue GetNew(string key, string value)
        {
            return new KeyValue(key, value);
        }
    }
}