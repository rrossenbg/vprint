/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Management;

namespace CPrint2.Common
{
    public class Tools
    {
        public static int GetNumberOfCameras(string caption)
        {
            int result = 0;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SoundDevice");

            foreach (ManagementObject queryObj in searcher.Get())
                if (string.CompareOrdinal(Convert.ToString(queryObj["Caption"]), caption) == 0)
                    result++;
            return result;
        }
    }
}
