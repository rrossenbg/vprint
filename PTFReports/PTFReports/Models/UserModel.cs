using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PTF.Reports.PTFDB;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public int CountryID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserTypeID { get; set; }
        public int LanguageID { get; set; }

        public string CurrentStep { get; set; }
        public string Salt { get; set; }     

        [Required]
        [DisplayName("Login")]
        public string Login { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Surname")]
        public string Surname { get; set; }
        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [Required]
        [DisplayName("Company")]
        public string Company { get; set; }
        [Required]
        [DisplayName("Branch")]
        public string Branch { get; set; }
        [Required]
        [DisplayName("Language")]
        public string Language { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool Active { get; set; }

        [DisplayName("UserType")]
        public string UserType { get; set; }

        [Required]
        [DisplayName("DefaultPass")]
        public string DefaultPass { get; set; }

        public DateTime? BlockedAt { get; set; }

        public static explicit operator UserModel(UserDetail p1)
        {
            int iso_id = p1.Ud_iso_id;
            int ho_id = p1.Ud_ho_id;
            int br_id = p1.Ud_br_id;

            var ctx1 = PTFContext.Current;

            var str1 = ctx1.HeadOffices.FirstOrDefault(ho => ho.ho_iso_id == iso_id && ho.ho_id == ho_id).SafeGetValue("ho_name", "(na)");
            var str2 = ctx1.ISO_ptf.FirstOrDefault(i => i.iso_number == iso_id).SafeGetValue("iso_country", "(na)");
            var str3 = ctx1.Branches.FirstOrDefault(br => br.br_iso_id == iso_id && br.br_ho_id == ho_id && br.br_id == br_id).SafeGetValue("br_name", "(na)");

            var user = new UserModel()
            {
                UserID = p1.Ud_id,
                CountryID = p1.Ud_iso_id,
                CompanyID = p1.Ud_ho_id,
                BranchID = p1.Ud_br_id,
                UserTypeID = p1.Ud_userType,
                Login = p1.Ud_loginName,
                Name = p1.Ud_firstName,
                Surname = p1.Ud_lastName,
                Email = p1.Ud_email,
                Company = str1,
                Country = str2,
                Branch = str3,
                UserType = p1.Ud_userType == 1 ? "Admin" : "Normal",
                Active = !p1.Ud_firstLogin,
                BlockedAt = p1.BlockedAt,
                LanguageID = (int)(p1.Ud_languageId ?? (int)eLanguage.English),
                Language = ((int)(p1.Ud_languageId ?? (int)eLanguage.English)).GetEnumName<eLanguage>(),
            };

            user.DefaultPass = p1.Ud_password.Decrypt();
            return user;
        }

        public static explicit operator UserDetail(UserModel model)
        {
            var pass = model.DefaultPass.Encript();
            var user = new UserDetail()
            {
                Ud_loginName = model.Login,
                Ud_firstName = model.Name,
                Ud_lastName = model.Surname,
                Ud_email = model.Email,
                Ud_userType = model.UserTypeID,
                Ud_password = pass,
                Ud_salt = model.Salt ?? "",
                Ud_iso_id = model.CountryID,
                Ud_ho_id = model.CompanyID,
                Ud_br_id = model.BranchID,
                Ud_languageId = (int)model.LanguageID,
            };
            return user;
        }
    }
}