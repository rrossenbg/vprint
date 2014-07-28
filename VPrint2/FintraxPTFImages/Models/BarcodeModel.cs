using System.ComponentModel.DataAnnotations;

namespace FintraxPTFImages.Models
{
    public class BarcodeModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Barcode { get; set; }
    }
}