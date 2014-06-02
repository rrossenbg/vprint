/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Data;

namespace PremierTaxFree.PTFLib
{
    public static class DataEx
    {
        /// <summary>
        /// Checks whether a row collection is empty
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static bool Empty(this DataRow[] rows)
        {
            return rows != null && rows.Length > 0;
        }
    }
}
