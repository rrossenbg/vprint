/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MerchantSite.Models
{
    public class NotaDebito_Model
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

        //[Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public NotaDebito_Model()
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

            if (FromDate > ToDate)
            {
                errorTable.AddModelError("FromDate", "FromDate should be less than ToDate");
                errorTable.AddModelError("ToDate", "ToDate should be greater than FromDate");
            }

            if (FromDate != DateTime.MinValue && FromDate >= DateTime.Today)
                errorTable.AddModelError("FromDate", "FromDate should not be into the future");

            if (ToDate >= DateTime.Today)
                errorTable.AddModelError("ToDate", "ToDate should not be into the future");

            if (!errorTable.IsValid)
                errorTable.AddModelError("", "There are some errors. Please correct");
        }
    }

    public class NotaDebitoEmail_Model
    {
        [Required]
        [DataType(DataType.Text)]
        public int Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int HeadOffice { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string CC { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string Subject { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        public string Body { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public NotaDebitoEmail_Model()
        {
            Country = 0;
        }

        public void Validate(ModelStateDictionary errorTable)
        {
            if (Country <= 0)
                errorTable.AddModelError("Country", "Country invalid");

            if (HeadOffice <= 0)
                errorTable.AddModelError("HeadOffice", "HeadOffice invalid");

            if (FromDate == DateTime.MinValue)
                errorTable.AddModelError("FromDate", "FromDate invalid");

            if (ToDate == DateTime.MinValue)
                errorTable.AddModelError("ToDate", "ToDate invalid");

            if (FromDate > ToDate)
            {
                errorTable.AddModelError("FromDate", "FromDate should be less than ToDate");
                errorTable.AddModelError("ToDate", "ToDate should be greater than FromDate");
            }

            if (FromDate != DateTime.MinValue && FromDate >= DateTime.Today)
                errorTable.AddModelError("FromDate", "FromDate should not be into the future");

            if (ToDate >= DateTime.Today)
                errorTable.AddModelError("ToDate", "ToDate should not be into the future");

            if (!errorTable.IsValid)
                errorTable.AddModelError("", "There are some errors. Please correct");
        }
    }

    public class InvoiceSearch_Model
    {
        [Required]
        [DataType(DataType.Text)]
        public int Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Number { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int HeadOffice { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Retailer { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }

        public bool UseNumber
        {
            get
            {
                return Number > 0;
            }
        }

        public void Validate(ModelStateDictionary errorTable)
        {
            if (Country < 0)
                errorTable.AddModelError("Country", "Country invalid");

            if (Number < 0)
                errorTable.AddModelError("Number", "Number invalid");

            if (HeadOffice < 0)
                errorTable.AddModelError("HeadOffice", "HeadOffice invalid");

            if (Retailer < 0)
                errorTable.AddModelError("Retailer", "Retailer invalid");

            if (FromDate > ToDate)
            {
                errorTable.AddModelError("FromDate", "FromDate should be less than ToDate");
                errorTable.AddModelError("ToDate", "ToDate should be greater than FromDate");
            }

            if (Number != 0)
            {
                // Use number
                if (HeadOffice > 0)
                    errorTable.AddModelError("HeadOffice", "Number is provided HeadOffice should empty");

                if (Retailer > 0)
                    errorTable.AddModelError("Retailer", "Number is provided Retailer should empty");

                if (FromDate != DateTime.MinValue)
                    errorTable.AddModelError("FromDate", "Number is provided FromDate should empty");

                if (ToDate != DateTime.MinValue)
                    errorTable.AddModelError("ToDate", "Number is provided ToDate should empty");
            }
            else
            {
                // Use HOID, RID & dates
                if (HeadOffice == 0)
                    errorTable.AddModelError("HeadOffice", "HeadOffice invalid");

                if (Retailer == 0)
                    errorTable.AddModelError("Retailer", "Retailer invalid");

                if (FromDate != DateTime.MinValue && FromDate >= DateTime.Today)
                    errorTable.AddModelError("FromDate", "FromDate should not be into the future");

                if (!errorTable.IsValid)
                    errorTable.AddModelError("", "There are some errors. Please correct");
            }

            if (!errorTable.IsValid)
                errorTable.AddModelError("", "There are some errors. Please correct");
        }
    }
}