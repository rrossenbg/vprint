using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PTF.Reports.PTFDB;

namespace PTF.Reports.Models
{
    public class HeadOfficeModel
    {
        public int HeadOfficeID { get; set; }
        public int CountryID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Country")]
        public string Country { get; set; }
        [Required]
        [DisplayName("Region")]
        public string Region { get; set; }
        [Required]
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [Required]
        [DisplayName("Fax")]
        public string Fax { get; set; }
        [Required]
        [DisplayName("Contact")]
        public string Contact1 { get; set; }
        [Required]
        [DisplayName("Position")]
        public string Position1 { get; set; }
        [Required]
        [DisplayName("Contact")]
        public string Contact2 { get; set; }
        [Required]
        [DisplayName("Position")]
        public string Position2 { get; set; }


        public static explicit operator HeadOfficeModel(HeadOffice p1)
        {
            var rm = new HeadOfficeModel()
            {
                HeadOfficeID = p1.ho_id,
                CountryID = p1.ho_iso_id,
                Name = p1.ho_name,
                Country = PTFContext.Current.ISO_ptf.First(i=>i.iso_number == p1.ho_iso_id).iso_country,
                Phone = p1.ho_phone,
                Fax = p1.ho_fax,
                Region = p1.ho_region,
                Contact1 = p1.ho_contact_1,
                Position1 = p1.ho_position_1,
                Contact2 = p1.ho_contact_2,
                Position2 = p1.ho_position_2,
            };
            return rm;
        }
    }
}