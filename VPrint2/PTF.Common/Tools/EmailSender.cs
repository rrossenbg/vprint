/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace VPrinting
{
    public class EmailSender
    {
        private static string EXCHANGESERVER = ConfigurationManager.AppSettings["EXCHANGESERVER"];
        private static string EXCHANGESERVER_DOMAIN = ConfigurationManager.AppSettings["EXCHANGESERVER_DOMAIN"];
        private static string EXCHANGESERVER_USER = ConfigurationManager.AppSettings["EXCHANGESERVER_USER"];
        private static string EXCHANGESERVER_PASS = ConfigurationManager.AppSettings["EXCHANGESERVER_PASS"];
        private static string EXCHANGESERVER_FROM = ConfigurationManager.AppSettings["EXCHANGESERVER_FROM"];

        public static void SendSafe(string email, string ccEmail, string subject, string message, bool html, params Attachment[] attachments)
        {
            try
            {
                using (var smtpClient = new SmtpClient(EXCHANGESERVER))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(EXCHANGESERVER_USER, EXCHANGESERVER_PASS, EXCHANGESERVER_DOMAIN);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    using (var msg = new MailMessage(EXCHANGESERVER_FROM, email, subject, message))
                    {
                        if (!string.IsNullOrWhiteSpace(ccEmail))
                            msg.CC.Add(ccEmail);
                        msg.IsBodyHtml = html;

                        foreach (var attach in attachments)
                            msg.Attachments.Add(attach);

                        smtpClient.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
