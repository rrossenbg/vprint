/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.ComponentModel.DataAnnotations;

namespace MerchantSite.Models
{
    public class BarcodeModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Barcode { get; set; }
    }
}