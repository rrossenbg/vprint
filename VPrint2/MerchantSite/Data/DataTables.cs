/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Web;

namespace MerchantSite.Data
{
    public class DataTables
    {
        public static DataTables Default
        {
            get
            {
                return new DataTables();
            }
        }

        public T Get<T>(string tableName, Lazy<T> lazy)
        {
            var table = HttpContext.Current.Application.GetCached(tableName, lazy, TimeSpan.FromHours(1));
            return table;
        }
    }
}