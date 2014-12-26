/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using MerchantSite.AuthenticationRef;
using MerchantSite.Common;
using MerchantSite.PartyManagementRef;
using MerchantSite.ScanServiceRef;
using VPrinting;
using AAuthenticationHeader = MerchantSite.AuthenticationRef.AuthenticationHeader;
using PAuthenticationHeader = MerchantSite.PartyManagementRef.AuthenticationHeader;

namespace MerchantSite.Data
{
    public class ScanServiceAccess
    {
        public static ScanServiceAccess Instance
        {
            get
            {
                return new ScanServiceAccess();
            }
        }

        public List<VoucherInfo> SelectVouchersByRetailer(int countryId, int retailerId)
        {
            IScanService client = null;
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client = ScanServiceClient.CreateProxy();
                return new List<VoucherInfo>(client.ReadData(countryId, retailerId, keys.Item1, keys.Item2));
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public fileInfo[] SelectVouchersByNumber(int countryId, int voucherId)
        {
            IScanService client = null;
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client = ScanServiceClient.CreateProxy();
                string where = string.Format("[iso_id] = {0} and [v_number] = {1}", countryId, voucherId);
                return client.SelectFilesBySql(where, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public CurrentUser TryLogin(int countryId, string userName, string password)
        {
            var proxy = new AuthenticationSoapClient();
            string result = proxy.AuthenticateUser(countryId, userName, password);
            if (string.IsNullOrWhiteSpace(result))
                return CurrentUser.Empty;
            var userId = proxy.RetrieveUser(new AAuthenticationHeader(), countryId, userName);
            return new CurrentUser(userId, userName, countryId);
        }

        public HeadOffice[] RetrieveHeadOfficeList(PAuthenticationHeader header, int countryId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveHeadOfficeList(header, countryId);
            return results;
        }

        public Retailer[] RetrieveRetailerList(PAuthenticationHeader header, int countryId, int headOfficeId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveRetailerList(header, countryId, headOfficeId);
            return results;
        }

        public Dictionary<int, CurrentUser> RetrieveUsers()
        {
            var proxy = new PartyManagementSoapClient();
            var data = proxy.RetrieveTableData(new PAuthenticationHeader(), "us_id, us_first_name, us_last_name, us_iso_id", "Users", "");
            return data.ToDict(4, (o, i) => { return Convert.ToInt32(o[i]); }, (o, i) => { return new CurrentUser(Convert.ToInt32(o[i]), o[i + 1] + " " + o[i + 2], Convert.ToInt32(o[i + 3])); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="officeId"></param>
        /// <param name="date"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="format">ATOM, HTML4.0, MHTML, IMAGE, EXCEL, WORD, CSV, PDF, XML</param>
        /// <param name="outputFormat">&OutputFormat=JPEG&</param>
        /// <returns></returns>
        /// <remarks>http://msdn.microsoft.com/en-us/library/ms152835.aspx</remarks>
        /// <remarks>http://msdn.microsoft.com/en-us/library/hh231593.aspx</remarks>
        public byte[] DownloadNotaDebitoReport(int countryId, int officeId, DateTime date, int invoiceNumber, string format = "PDF", string moreOptions = "")
        {
            //02/12/2013
            string uri = string.Format("http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format={4}{5}&iso_id={0}&Office={1}&in_date={2:dd/MM/yyyy}&invoicenumber={3}",
                countryId, officeId, date, invoiceNumber, format, moreOptions);

            IScanService client = null;
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client = ScanServiceClient.CreateProxy();
                byte[] data = client.DownloadReport(uri, keys.Item1, keys.Item2);
                return data;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void EmailNotaDebito(List<EmailInfo> emailInfo)
        {
            IScanService client = null;
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client = ScanServiceClient.CreateProxy();
                client.EmailNotaDebito(emailInfo.ToArray(), keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }
    }
}