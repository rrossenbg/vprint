/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects;
using PremierTaxFree.PTFLib.DataServiceProxy;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Threading;

namespace TransferringService
{
    public class TransferringWorker : CycleWorkerBase
    {
        private UserAuth Auth { get; set; }

        private SettingsObj Settings { get; set; }

        public event EventHandler Success;

        private DateTime m_CleanDbCheckRun = DateTime.MinValue;

        protected override void ThreadFunction()
        {
            bool fireSuccess = true;

            try
            {
                Auth = DBConfigValue.ReadSf<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default);

                if (Auth == null || !Auth.IsValid)
                    throw new ApplicationException("No authentication provided");

                Settings = DBConfigValue.ReadSf<SettingsObj>(Strings.Transferring_SettingsObject, SettingsObj.Default);

                if (m_CleanDbCheckRun.Day != DateTime.Now.Day)
                {
                    CleanDataBase();
                    m_CleanDbCheckRun = DateTime.Now;
                }

                using (var client = new DataServiceClient())
                {
                    QuerySiteCodes(client);

                    SendFiles(client, true);

                    if (DateTime.Now.Hour.IsBetween(Program.START_EXPORTMESSAGES_HOUR, Program.END_EXPORTMESSAGES_HOUR))
                        SendMessages(client);
                }
            }
            catch
            {
                fireSuccess = false;
                throw;
            }
            finally
            {
                if (fireSuccess && Success != null)
                    Success(this, EventArgs.Empty);
            }
        }

        private void CleanDataBase()
        {
            int historyDays = (Settings != null && Settings.KeepHistoryDays > 0) ?
                Settings.KeepHistoryDays :
                Program.PURGE_RECORDS_OLDER_THAN_DAYS;

            Trace.WriteLine("TransferringService: ClientDataAccess.DeleteOlderSentFilesAsync");
            ClientDataAccess.DeleteOlderSentFilesAsync(historyDays);
            Trace.WriteLine("TransferringService: ClientDataAccess.DeleteOlderSentMessagesAsync");
            ClientDataAccess.DeleteOlderSentMessagesAsync(historyDays);
        }

        private void QuerySiteCodes(DataServiceClient client)
        {
            //******************************************************************
            //  SELECT SITEIDS FROM THE SERVER
            //******************************************************************

            Trace.WriteLine("TransferringService: ClientDataAccess.SelectAvailableAuditIDsCount");
            int Ids = ClientDataAccess.SelectAvailableSiteCodeIDsCount();
            if (Ids < Program.MINIMUM_SITECODE_IDS_COUNT)
            {
                foreach (KeyValuePair<int, string> country in Settings.SiteCodeTable)
                {
                    Stopwatch w = Stopwatch.StartNew();

                    Trace.WriteLine("TransferringService: client.QuerySiteCodes");

                    string[] siteCodes = client.QuerySiteCodes(Auth.ClientID, country.Key, country.Value, Program.MINIMUM_SITECODE_IDS_COUNT);

                    Trace.WriteLine("TransferringService: ".concat(w.Elapsed));

                    Trace.WriteLine("TransferringService: ClientDataAccess.InsertAuditIdsAsync");

                    ClientDataAccess.InsertFileAsync(country.Key, siteCodes, Program.OnError);
                }
            }
            else
            {
                Trace.WriteLine("TransferringService: QUERYSITECODES (OK)");
            }
        }

        private void SendFiles(DataServiceClient client, bool querySiteIds)
        {
            //******************************************************************
            //  SEND FILES TO THE SERVER
            //******************************************************************
            Trace.WriteLine("TransferringService: ClientDataAccess.SelectFilesForExport");
            int filesCount = (Settings != null && Settings.MaximumFilesForExport > 0) ?
                Settings.MaximumFilesForExport :
                Program.MAXIMUM_FILES_FOR_EXPORT;

            List<DbClientFileInfo> files = ClientDataAccess.SelectFilesForExport(filesCount);
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Stopwatch w = Stopwatch.StartNew();
                    Trace.WriteLine("TransferringService: client.SaveDataXmlText");
                    var voucher = new VoucherData()
                    {
                        CountryID = file.CountryID,
                        RetailerID = file.RetailerID,
                        VoucherID = file.VoucherID,
                        SiteCode = file.SiteCode,
                        BarCodeImage = file.BarCodeImage,
                        VoucherImage = file.VoucherImage,
                        DateCreated = DateTime.Now,
                    };

                    client.SaveVoucher(voucher, Auth.ClientID);

                    Trace.WriteLine("TransferringService: ClientDataAccess.SetFileExportedAsync");
                    ClientDataAccess.SetFileExportedAsync(file.SiteCode, null, null);

                    if (querySiteIds)
                        QuerySiteCodes(client);

                    Thread.Sleep(0);
                }
            }
            else
            {
                Trace.WriteLine("TransferringService: FILES to Send (OK)");
            }
        }

        private void SendMessages(DataServiceClient client)
        {
            //******************************************************************
            //  SEND MESSAGES TO THE SERVER
            //******************************************************************
            Trace.WriteLine("TransferringService: ClientDataAccess.SelectMessagesForExport");

            int msgCount = (Settings != null && Settings.MaximumMessagesForExport > 0) ?
                Settings.MaximumMessagesForExport :
                Program.MAXIMUM_MESSAGES_FOR_EXPORT;

            List<DbMessageInfo> messages = ClientDataAccess.SelectMessagesForExport(msgCount);
            if (messages != null && messages.Count > 0)
            {
                foreach (var msg in messages)
                {
                    Stopwatch w = Stopwatch.StartNew();
                    Trace.WriteLine("TransferringService: client.SaveMessageXmlText");
                    Hashtable table = new Hashtable();
                    table.Add("Message", msg.Message);
                    //TODO: Hardcode
                    table.Add("Source", 0);
                    table.Add("StackTrace", msg.StackTrace);
                    table.Add("Type", msg.Type);
                    table.Add("DateInserted", msg.DateInserted);
#warning TODO
                    ////////using (ProtectedXml xml = new ProtectedXml(ProtectionOptions.Encript))
                    ////////{
                    ////////    xml.AddElement("MSG", table);
                    ////////    client.SaveMessageXmlText(xml.OuterXml);
                    ////////    Trace.WriteLine("TransferringService: ".concat(w.Elapsed));
                    ////////}

                    ClientDataAccess.SetMessageExportedAsync(msg.MessageID);

                    Thread.Sleep(0);
                }
            }
            else
            {
                Trace.WriteLine("TransferringService: MESSAGES to Send (OK)");
            }
        }

    }
}
