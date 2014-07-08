/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

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
    }

    public static class Strings
    {
        /// <summary>
        /// SITECODE
        /// </summary>
        public const string SRVNAME = "SITECODE"; 
    }
}
