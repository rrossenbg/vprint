/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace BtRetryService
{
    public class EmailSender
    {
        public static void SendSafe(string email, string subject, string message, bool html)
        {
            string EXCHANGESERVER = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER];
            string EXCHANGESERVER_DOMAIN = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_DOMAIN];
            string EXCHANGESERVER_USER = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_USER];
            string EXCHANGESERVER_PASS = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_PASS];
            string EXCHANGESERVER_FROM = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_FROM];

            try
            {
                var smtpClient = new SmtpClient(EXCHANGESERVER);
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(EXCHANGESERVER_USER, EXCHANGESERVER_PASS, EXCHANGESERVER_DOMAIN);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage msg = new MailMessage(EXCHANGESERVER_FROM, email, subject, message);
                msg.IsBodyHtml = html;
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "EML");
            }
        }
    }
}
