using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace HobexCommonLib
{
    public static class Common
    {
        /// <summary>
        /// Gets datetime to 17 o'clock
        /// </summary>
        /// <param name="oclock"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeIntervalTo(TimeSpan oclock)
        {
            var value = DateTime.Today.Add(oclock).Subtract(DateTime.Now);
            if (value < TimeSpan.Zero)
                value = DateTime.Today.AddDays(1).Add(oclock).Subtract(DateTime.Now);
            return value;
        }

        public static void Send(string server, int port, string pass,
            string from, string to, string subject, string body, bool isHtml)
        {
            if (server.IsNullOrEmpty() || port == 0 || pass.IsNullOrEmpty())
                return;

            MailMessage email = new MailMessage(from, to, subject, body);
            email.IsBodyHtml = isHtml;

            try
            {
                var smtp = new SmtpClient
                {
                    Host = server,
                    Port = port,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(email.From.Address, pass)
                };
                smtp.Send(email);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex, "HBX");
            }
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
}
