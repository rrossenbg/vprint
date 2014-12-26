/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections.Generic;
using MerchantSite.Common;
using MerchantSite.ScanServiceRef;

namespace MerchantSite
{
    public static class Class1Ex
    {
        public static string GetOperatorId(this VoucherInfo i, Dictionary<int, CurrentUser> dict)
        {
            return dict.ContainsKey(i.OperatorId) ? dict[i.OperatorId].UserName : i.OperatorId.ToString();
        }
    }
}