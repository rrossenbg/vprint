/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Net;
using System.Web.Services.Protocols;
using PTF.Reports.TRSReportingService;

namespace PTF.Reports.Common
{
    
    // Class used to implement and extend the Reporting Services proxy
    // class. This extension enables cookie management.
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// //RSClient rs = new RSClient();
    //// Pass credentials from the user
    //NetworkCredential creds = rs.GetCredentials();
    //rs.LogonUser(creds.UserName, creds.Password, null);
    //try
    //{
    //if (rs.CheckAuthorized())
    //{
    //    ItemTypeEnum type = rs.GetItemType("/");
    //    Console.WriteLine(type);
    //}
    //}
    //catch (Exception)
    //{
    //Console.WriteLine("Exception on call to GetItemType.");
    //}
    //rs.SessionHeaderValue = new SessionHeader();
    //// Render arguments
    //byte[] result = null;
    //string reportPath = "/SampleReports/Sales Order Detail";
    //string format = "IMAGE";
    //string historyID = null;
    //string devInfo = @"<DeviceInfo><OutputFormat>GIF</OutputFormat></DeviceInfo>";
    //// Use default parameter
    //ParameterValue[] parameters = null;
    //DataSourceCredentials[] credentials = null;
    //string showHideToggle = null;
    //string encoding;
    //string mimeType;
    //Warning[] warnings = null;
    //ParameterValue[] reportHistoryParameters = null;
    //string[] streamIDs = null;
    //try
    //{
    //result = rs.Render(reportPath, format, historyID, devInfo, parameters, credentials, 
    //    showHideToggle, out encoding, out mimeType, out reportHistoryParameters, out warnings,
    //    out streamIDs);
    //}
    //catch (SoapException e)
    //{
    //Console.WriteLine(e.Detail.OuterXml);
    //}
    //// Write the contents of the report to file.
    //try
    //{
    //FileStream stream = File.Create( @"C:\report.gif", result.Length );
    //Console.WriteLine( "File created." );
    //stream.Write( result, 0, result.Length );
    //Console.WriteLine( "Result written to the file." );
    //stream.Close();
    //}
    //catch ( Exception e )
    //{
    //Console.WriteLine( e.Message );
    //}
    /// </example>
    public class RSClient : TRSReportingService.ReportingService2005
    {
        public bool m_needLogon = false;
        private string m_authCookieName;
        private Cookie m_authCookie;

        /// <summary>
        /// "http://localhost/reportserver/reportservice.asmx"
        /// </summary>
        /// <param name="url"></param>
        public RSClient(string url)
        {
            // Set the server URL
            base.Url = url;
            // Set default credentials to integrated.
            Credentials = System.Net.CredentialCache.DefaultCredentials;
        }

        /// <summary>
        /// Gets the type of the item on the report server. Use the 
        /// new modifier to hide the base implementation.
        /// </summary>
        public new ItemTypeEnum GetItemType(string item)
        {
            ItemTypeEnum type = ItemTypeEnum.Unknown;
            try
            {
                type = base.GetItemType(item);
            }
            catch (SoapException)
            {
                return ItemTypeEnum.Unknown;
            }

            return type;
        }

        /// <summary>
        /// Get whether the given credentials can connect to the report server.
        /// Returns false if not authorized. Other errors throw an exception.
        /// </summary>
        public bool CheckAuthorized()
        {
            try
            {
                GetItemType("/");
            }
            catch (WebException e)
            {
                if (!(e.Response is HttpWebResponse) ||
                   ((HttpWebResponse)e.Response).StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw;
                }
                return false;
            }
            catch (InvalidOperationException)
            {
                // This condition could be caused by a redirect to a forms logon page
                if (m_needLogon)
                {
                    NetworkCredential creds = Credentials as NetworkCredential;
                    if (creds != null && creds.UserName != null)
                    {
                        try
                        {
                            base.CookieContainer = new CookieContainer();
                            base.LogonUser(creds.UserName, creds.Password, null);
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
            return true;
        }
        /// <summary>
        /// Enables users to enter credentials from the command prompt.
        /// </summary>
        public void SetCredentials(string userName, string password, string domain)
        {
            Credentials = new NetworkCredential(userName, password, domain);
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest request;
            request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Credentials = base.Credentials;
            request.CookieContainer = new CookieContainer();
            if (m_authCookie != null)
                request.CookieContainer.Add(m_authCookie);
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            string cookieName = response.Headers["RSAuthenticationHeader"];
            if (cookieName != null)
            {
                m_authCookieName = cookieName;
                HttpWebResponse webResponse = (HttpWebResponse)response;
                Cookie authCookie = webResponse.Cookies[cookieName];
                // save it away 
                m_authCookie = authCookie;
            }
            // need to call logon
            if (response.Headers["RSNotAuthenticated"] != null)
            {
                m_needLogon = true;
            }
            return response;
        }
    }
}
