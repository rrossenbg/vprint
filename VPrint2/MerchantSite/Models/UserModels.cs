/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MerchantSite.Models
{
    public class SetupUser_Model
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int HeadOffice { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Retailer { get; set; }

        public void Validate(ModelStateDictionary errorTable)
        {
            if (string.IsNullOrWhiteSpace(Name))
                errorTable.AddModelError("Name", "Name invalid");

            if (HeadOffice < 0)
                errorTable.AddModelError("HeadOffice", "HeadOffice invalid");

            if (Retailer < 0)
                errorTable.AddModelError("Retailer", "Retailer invalid");

            if (!errorTable.IsValid)
                errorTable.AddModelError("", "There are some errors. Please correct");
        }
    }
}