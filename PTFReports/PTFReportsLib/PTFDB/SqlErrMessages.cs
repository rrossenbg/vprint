/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;

namespace PTF.Reports.Data
{
    public static class SqlErrMessages
    {
        private static Hashtable sm_table = Hashtable.Synchronized(new Hashtable());

        static SqlErrMessages()
        {
            sm_table["IX_SecurityObjects_Unique"] = "Name should be unique";
            sm_table["UK_principal_name"] = "Name should be unique";
            sm_table["IX_UserDetails_Email_Unique"] = "Email should be unique";
            sm_table["IX_UserDetails_LoginName_Unique"] = "Login name should be unique";
        }

        public static string Get(string key, string defaultValue)
        {
            return sm_table.ContainsKey(key) ? Convert.ToString(sm_table[key]) : defaultValue;
        }
    }
}
