/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Net;
using System.Net.Mail;

namespace PTF.Reports.Tools
{
    public static class EmailSender
    {
        public static void Send(string email, string subject, string message, bool html)
        {
            string EXCHANGESERVER = Config.Get<string>(Strings.EXCHANGESERVER);
            string EXCHANGESERVER_DOMAIN = Config.Get<string>(Strings.EXCHANGESERVER_DOMAIN);
            string EXCHANGESERVER_USER = Config.Get<string>(Strings.EXCHANGESERVER_USER);
            string EXCHANGESERVER_PASS = Config.Get<string>(Strings.EXCHANGESERVER_PASS);
            string EXCHANGESERVER_FROM = Config.Get<string>(Strings.EXCHANGESERVER_FROM);

            var smtpClient = new SmtpClient(EXCHANGESERVER);
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(EXCHANGESERVER_USER, EXCHANGESERVER_PASS, EXCHANGESERVER_DOMAIN);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage msg = new MailMessage(EXCHANGESERVER_FROM, email, subject, message);
            msg.IsBodyHtml = html;
            smtpClient.Send(msg);
        }
    }
}