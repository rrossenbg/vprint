using System;
using System.Net;

namespace ReceivingServiceLib.Data
{
    public class WebDataAccess
    {
        public byte[] DownloadReport(string serverUrl, NetworkCredential reportServerCredentials)
        {
            Uri uri = new Uri(serverUrl);
            string serverUrl0 = string.Concat("http://", uri.Host, "/");

            var client = new WebClient();
            client.UseDefaultCredentials = false;
            client.Headers.Add(HttpRequestHeader.Accept, Resource1.ReportServer_HttpRequestHeaderAccept);
            client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB");
            var cache = new CredentialCache();
            cache.Add(new Uri(serverUrl0), "NTLM", reportServerCredentials);
            cache.Add(new Uri(serverUrl0), "Digest", reportServerCredentials);
            client.Credentials = cache;
            var buffer = client.DownloadData(uri);
            return buffer;
        }
    }
}
