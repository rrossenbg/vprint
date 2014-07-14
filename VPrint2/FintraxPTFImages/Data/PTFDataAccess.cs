using System.Linq;
using FintraxPTFImages.Data.PTF;

namespace FintraxPTFImages.Data
{
    public class PTFDataAccess
    {
        public static string ConnectionString { get; set; }

        public IQueryable<Branch> GetBranchesByCountryId(int countryId)
        {
            PTFDataEntities2 db = new PTFDataEntities2();
            var branches = db.Branches.Where(br => br.br_iso_id == countryId);
            return branches;
        }
    }
}