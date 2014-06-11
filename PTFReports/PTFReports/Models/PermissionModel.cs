using PTF.Reports.PTFDB;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class PermissionModel
    {
        public int UserID { get; set; }
        public int ISOID { get; set; }
        public int CompanyID { get; set; }
        public UserDetail User { get; set; }
        public HeadOffice HeadOffice { get; set; }
        public Branch Branch { get; set; }
    }
}