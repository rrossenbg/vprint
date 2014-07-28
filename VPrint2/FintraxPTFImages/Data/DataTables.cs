/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Web;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FintraxPTFImages.Data
{
    public class DataTables
    {
        public const string USERS = "USERS";

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