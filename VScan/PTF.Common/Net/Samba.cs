/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Management;

namespace PremierTaxFree.PTFLib.Net
{
    public static class Samba
    {
        public static string MapDrive(string deviceName, string url, string user = null, string pwd = null)
        {
            string cmdString = "net use ".concat(deviceName, ": ", url, ((user != null) ? " /user:" + user + " " + pwd : ""));
            ManagementClass processClass = new ManagementClass("Win32_Process");
            object[] methodArgs = { cmdString, null, null, 0 };
            object result = processClass.InvokeMethod("Create", methodArgs);
            return result.ToStringSf();
        }

        public static string UnMapDrive(string deviceName, string user = null, string pwd = null)
        {
            string cmdString = "net use ".concat(deviceName, ": /delete ", ((user != null) ? " /user:" + user + " " + pwd : ""));
            ManagementClass processClass = new ManagementClass("Win32_Process");
            object[] methodArgs = { cmdString, null, null, 0 };
            object result = processClass.InvokeMethod("Create", methodArgs);
            return result.ToStringSf();
        }
    }
}
