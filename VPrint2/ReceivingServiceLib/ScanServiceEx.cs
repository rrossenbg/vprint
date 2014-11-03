/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.ServiceModel;
using ReceivingServiceLib.Data;
using VPrinting;
using VPrinting.Colections;

namespace ReceivingServiceLib
{
    partial class ScanService
    {
        public const int MAX_BUFF_SIZE_50MB = 50 * 1024 * 1024; //33262605;

        public static NetworkCredential ReportServerCredentials { get; set; }

        public static event EventHandler<ValueEventArgs<EmailInfo>> EmailNotaDebitoEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <returns></returns>
        /// <example>
        /// http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format=PDF&iso_id=724&Office=167150&in_date=02/12/2013&invoicenumber=42538
        /// </example>
        public byte[] DownloadReport(string serverUrl, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DownloadReport");

                if (ReportServerCredentials == null)
                    throw new Exception("ReportServerCredentials may not be null");

                WebDataAccess access = new WebDataAccess();
                return access.DownloadReport(serverUrl, ReportServerCredentials);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public byte[] DownloadVouchers(int countryId, int[] voucherIds, string s1, string s2)
        {
            DirectoryInfo session = null;
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DownloadReport");

                Guid guid = CommTools.ToGuid(countryId, voucherIds.Get(0), voucherIds.Get(1), voucherIds.Get(2));
                var dir = new DirectoryInfo(Global.Strings.DOWNLOADROOT);
                dir.EnsureDirectory();
                session = dir.Combine(guid.ToString());
                session.EnsureDirectory();
                var result = dir.CombineFileName(string.Concat(guid.ToString(), ".zip"));

                var dac = VoucherDataAccess.Instance;
                var zac = ZipFileAccess.Instance;

                CString str = "Archive created at";
                str += DateTime.Now;
                str.NewLine();
                str += "Voucher numbers :";
                str.NewLine();

                foreach (int voucherId in voucherIds)
                {
                    bool isProtected;
                    string sessionId;
                    int size;

                    FileInfo binFile = null;
                    FileInfo zipFile = null;
                    try
                    {
                        using (var buffer = new MemoryBuffer(MAX_BUFF_SIZE_50MB))
                        {
                            if (dac.SelectVoucherByNumber(countryId, voucherId, out isProtected, out sessionId, out size, buffer.Buffer))
                            {
                                binFile = session.CombineFileName(string.Concat(sessionId, ".bin"));
                                zipFile = session.CombineFileName(string.Concat(sessionId, ".zip"));

                                if (isProtected)
                                {
                                    binFile.WriteAllBytes(buffer.Buffer, size);
                                    binFile.DecriptFile(zipFile);
                                }
                                else
                                {
                                    zipFile.WriteAllBytes(buffer.Buffer, size);
                                }

                                str += voucherId;
                            }
                            else
                            {
                                str += string.Format("Voucher {0} not found", voucherId);
                            }
                        }

                        var files = zac.ExtractFileZip(zipFile.FullName, session.FullName);
                        foreach (var f in files)
                            if (!f.Extension.EqualNoCase(".pdf") && !f.Name.Contains("_signed"))
                                f.DeleteSafe();
                    }
                    catch (Exception ex)
                    {
                        str += ex.Message;
                    }
                    finally
                    {
                        str.NewLine();
                        binFile.DeleteSafe();
                        zipFile.DeleteSafe();
                    }
                }

                zac.CreateZip(result.FullName, session.FullName, str.ToString());

                return null;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
            finally
            {
                session.DeleteSafe();
            }
        }

        public void EmailNotaDebito(EmailInfo[] emails, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("EmailNotaDebito");

                foreach (EmailInfo info in emails)
                    if (EmailNotaDebitoEvent != null)
                        EmailNotaDebitoEvent(this, new ValueEventArgs<EmailInfo>(info));
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }
    }
}
