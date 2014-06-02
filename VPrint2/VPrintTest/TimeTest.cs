using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;

namespace VPrintTest
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void getTimeFromTimeServer()
        {
            var time = GetNistTime();
        }

        public static DateTime GetNistTime()
        {
            DateTime dateTime = DateTime.MinValue;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nist.time.gov/timezone.cgi?UTC/s/0");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string html = stream.ReadToEnd().ToUpper();
                string time = Regex.Match(html, @">\d+:\d+:\d+<").Value; //HH:mm:ss format
                string date = Regex.Match(html, @">\w+,\s\w+\s\d+,\s\d+<").Value; //dddd, MMMM dd, yyyy
                dateTime = DateTime.Parse((date + " " + time).Replace(">", "").Replace("<", ""));
            }

            return dateTime;
        }
    }
}
