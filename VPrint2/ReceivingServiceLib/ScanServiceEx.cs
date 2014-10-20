/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
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

        public static NetworkCredential ReportingServerCredentials { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverUrl">"http://myReportServer?MyReport&rs%3aCommand=Render&rs:Format=pdf"</param>
        /// <returns></returns>
        /// <example>
        /// "http://SqlServer/ReportServer?/MyReportFolder/Report1&rs:Command=Render&rs:format=PDF&ReportParam=" + ParamValue;
        /// </example>
        public byte[] DownloadReport(string serverUrl, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DownloadReport");

#if USE_TEMP_FILE
                var serverUri = new Uri(serverUrl);
                var wcli = new WebClient();
                if (Credentials != null)
                    wcli.Credentials = Credentials;
                wcli.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");
                wcli.DownloadFile(serverUri, file.FullName);
                return file.ReadAllBytes();
#else //MEMORY
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);
                if (ReportingServerCredentials != null)
                {
                    request.PreAuthenticate = true;
                    request.Credentials = ReportingServerCredentials;
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    response.Close();

                    //Now turn around and send this as the response..
                    byte[] file = stream.ReadAllBytes();
                    return file;
                }
#endif
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
    }
}
