/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiteCodeLib
{
    public static class Tools
    {
        public static int? TryParseCountryID(string site)
        {
            var re = new Regex(@"(P1Half|P2Half|P1Copy|P1POS|P1|P2)(\d+)[A-Z]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var ma = re.Match(site);
            int result;
            if (ma.Success && ma.Groups[2].Success && int.TryParse(ma.Groups[2].Value, out result))
                return result;
            return null;
        }

        public static Guid ToGuid(int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(value1));
            bytes.AddRange(BitConverter.GetBytes(value2));
            bytes.AddRange(BitConverter.GetBytes(value3));
            bytes.AddRange(BitConverter.GetBytes(value4));
            return new Guid(bytes.ToArray());
        }

        public static void FromGuid(Guid guid, out int value1, out int value2, out int value3, out int value4)
        {
            var bytes = guid.ToByteArray();
            value1 = BitConverter.ToInt32(bytes, 0);
            value2 = BitConverter.ToInt32(bytes, 4);
            value3 = BitConverter.ToInt32(bytes, 8);
            value4 = BitConverter.ToInt32(bytes, 12);
        }
    }

    public static class Strings
    {
        /// <summary>
        /// SITECODE
        /// </summary>
        public const string SRVNAME = "SITECODE"; 
    }
}
