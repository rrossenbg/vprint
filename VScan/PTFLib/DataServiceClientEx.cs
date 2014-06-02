/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using PremierTaxFree.PTFLib.Collections;
using PremierTaxFree.PTFLib.Net;

namespace PremierTaxFree.PTFLib.DataServiceProxy
{
    public partial class DataServiceClient
    {
        public static void CallValidateUser(int countryId, string name, string pass)
        {
            try
            {
                var data = new UserData { CountryID = countryId, Name = name, Pass = pass };

                using (var client = new DataServiceClient())
                    client.ValidateUser(data);
            }
            catch
            {
                throw new SecurityException("Wrong user or password");
            }
        }

        public static string[] CallQuerySiteCodes(int clientId, int countryId, string P2FileNumber, int count)
        {
            using (var client = new DataServiceClient())
                return client.QuerySiteCodes(clientId, countryId, P2FileNumber, count);
        }

        /// <summary>
        /// Call DataServer to create ClientID
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static int CallCreateClient(string machineName)
        {
            using (var client = new DataServiceClient())
                return client.CreateClient(machineName);
        }

        public static CountryData[] CallQueryContries(UserAuth user)
        {
            using (var client = new DataServiceClient())
                return client.QueryContries();
        }

        public static void CallSaveVoucher(
            int clientId,
            int countryId, 
            int retailerId, 
            string voucherId, 
            string siteCode,
            string comment, 
            DateTime date, 
            byte[] voucher, 
            byte[] barcode)
        {
            var data = new VoucherData()
            {
                CountryID = countryId,
                RetailerID = retailerId,
                VoucherID = voucherId,
                SiteCode = siteCode,
                Comment = comment,
                DateCreated = DateTime.Now,
                VoucherImage = voucher,
                BarCodeImage = barcode
            };

            using (var client = new DataServiceClient())
                client.SaveVoucher(data, clientId);
        }

        public static void CallSaveMessages(int clientId, LinkedDictionary<DateTime, string> messages)
        {
            var data = new List<MessageData>(messages.ConvertAll<KeyValuePair<DateTime, string>, MessageData>(i => new MessageData { Date = i.Key, Message = i.Value }));

            using (var client = new DataServiceClient())
                client.SaveMessages(data.ToArray(), clientId);
        }

        public static object CallSendCmd(string name, params object[] values)
        {
            using (var client = new DataServiceClient())
                return client.SendCmd(name, values);
        }

        public static string CallTestConnection()
        {
            string message = null;
            try
            {
                using (var client = new DataServiceClient())
                    client.Open();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }
    }
}
