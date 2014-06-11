using System.ComponentModel;
using PTF.Reports.PTFDB;

namespace PTF.Reports.Models
{
    public class BranchModel
    {
        public int CompanyID;
        public int ISO_ID;
        public int BR_ID;

        [DisplayName("Company name")]
        public string Name { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Email")]
        public string Email1 { get; set; }
        [DisplayName("Email")]
        public string Email2 { get; set; }
        [DisplayName("Line1")]
        public string Line1 { get; set; }
        [DisplayName("Line2")]
        public string Line2 { get; set; }
        [DisplayName("Line3")]
        public string Line3 { get; set; }
        [DisplayName("Line4")]
        public string Line4 { get; set; }
        [DisplayName("County")]
        public string County { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("Phone1")]
        public string Phone1 { get; set; }
        [DisplayName("Contact")]
        public string Contact { get; set; }
        [DisplayName("Contact")]
        public string Contact1 { get; set; }
        [DisplayName("Contact")]
        public string Contact2 { get; set; }

        public BranchModel()
        {
        }

        public static explicit operator BranchModel(Branch p1)
        {
            return new BranchModel()
            {
                ISO_ID = p1.br_iso_id,
                CompanyID = p1.br_ho_id,
                BR_ID = p1.br_id,
                Name = p1.br_name,
                Line1 = p1.br_add_1,
                Line2 = p1.br_add_2,
                Line3 = p1.br_add_city,
                Line4 = p1.br_add_county,
                Country = PTFContext.Current.ISO_ptf.First(i => i.iso_number == p1.br_iso_id).iso_country,
                Email = p1.br_email_1,
                Email1 = p1.br_email_2,
                Email2 = p1.br_email_3,
                Phone1 = p1.br_phone,
                Contact = p1.br_contact_1,
                Contact1 = p1.br_contact_2,
                Contact2 = p1.br_contact_3,
            };
        }

        public static explicit operator Branch(BranchModel p1)
        {
            return new Branch();
        }
    }
}