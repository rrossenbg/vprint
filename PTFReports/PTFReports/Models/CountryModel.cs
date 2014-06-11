using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PTF.Reports.PTFDB;

namespace PTF.Reports.Models
{
    public class CountryModel
    {
        public int CountryID { get; set; }

        [Required]
        [DisplayName("Country name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Iso2")]
        public string Iso2 { get; set; }

        public static explicit operator CountryModel(ISO_ptf p1)
        {
            return new CountryModel()
            {
                CountryID = p1.iso_number,
                Name = p1.iso_country,
                Iso2 = p1.iso_2
            };
        }
    }
}