using System.Collections.Generic;
using FintraxPTFImages.Common;
using FintraxPTFImages.ScanServiceRef;

namespace FintraxPTFImages
{
    public static class Class1Ex
    {
        public static string GetOperatorId(this VoucherInfo i, Dictionary<int, CurrentUser> dict)
        {
            return dict.ContainsKey(i.OperatorId) ? dict[i.OperatorId].UserName : i.OperatorId.ToString();
        }
    }
}