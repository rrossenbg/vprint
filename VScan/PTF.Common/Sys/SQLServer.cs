/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using Microsoft.Win32;

namespace PremierTaxFree.PTFLib.Sys
{
    public static class SQLServer
    {
        /// <summary>
        /// Read from registry path to the MSSQLServer
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        /// <example>
        /// string sqlpath = SQLServer.GetInstallPath("SQLEXPRESS");
        /// </example>
        public static string GetInstallPath(string instanceName)
        {
            using (RegistryKey sqlServerKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server"))
            {
                foreach (string subKeyName in sqlServerKey.GetSubKeyNames())
                {
                    if (subKeyName.StartsWith("MSSQL") && subKeyName.EndsWith(instanceName))
                    {
                        using (RegistryKey instanceKey = sqlServerKey.OpenSubKey(subKeyName))
                        using (RegistryKey setupKey = instanceKey.OpenSubKey(@"Setup"))
                        {
                            string path = Convert.ToString(setupKey.GetValue("SQLDataRoot"));
                            return path;
                        }
                    }
                }
            }
            return null;
        }
    }
}
