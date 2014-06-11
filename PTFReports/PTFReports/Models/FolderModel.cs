using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class FolderModel
    {
        public Guid FolderID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Folder")]
        public string ParentName { get; set; }
        public Guid? Parent { get; set; }

        public static explicit operator FolderModel(Folder p1)
        {
            var rm = new FolderModel()
            {
                FolderID = p1.FolderID,
                Name = p1.Name,
                Description = p1.Description,
                ParentName = Folder.GetNameById(p1.ParentID),
                Parent = p1.ParentID,
            };

            return rm;
        }
    }
}