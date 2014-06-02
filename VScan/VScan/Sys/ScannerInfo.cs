/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Collections.Generic;
using System.Management;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Sys
{
    public class UsbScanner
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public bool Working { get; set; }

        public override string ToString()
        {
            return "{0}\t{1}".format(Name, Status);
        }
    }

    /// <summary>
    /// Reads scanner infomation. 
    /// WARNING: WMI should be installed and accessable for the user.
    /// </summary>
    public static class ScannerInfo
    {
        // More details on the valid properties:
        // http://msdn.microsoft.com/en-us/library/aa394353(VS.85).aspx
        public static List<UsbScanner> SelectInstalled()
        {
            List<UsbScanner> devices = new List<UsbScanner>();

            using (var deviceList = new ManagementObjectSearcher("Select * from Win32_PnPEntity"))
            {
                foreach (var device in deviceList.Get())
                {
                    string name = device.GetPropertyValue("Name").ToStringSf();
                    string service = device.GetPropertyValue("Service").ToStringSf();
                    if (service == "usbscan")
                    {
                        string status = device.GetPropertyValue("Status").ToStringSf();
                        bool working = ((status == "OK") || (status == "Degraded") || (status == "Pred Fail"));
                        devices.Add(new UsbScanner() { Name = name, Status = status, Working = working });
                    }
                }
            }

            return devices;
        }
    }
}
