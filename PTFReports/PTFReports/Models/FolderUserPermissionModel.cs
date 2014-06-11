using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class FolderUserPermissionModel
    {
        public int UserID { get; set; }
        public Guid FolderID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("FolderName")]
        public string FolderName { get; set; }

        [Required]
        [DisplayName("Enable")]
        public bool Enabled { get; set; }

        public static explicit operator FolderUserPermissionModel(UserDetail p1)
        {
            var rm = new FolderUserPermissionModel()
            {
                UserID = p1.Ud_id,
                Name = p1.FullName,
            };
            return rm;
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(Name);
            }
        }
    }
}