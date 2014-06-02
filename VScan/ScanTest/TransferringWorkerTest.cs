using System.Configuration;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Net;
using TransferringService;

namespace ScanTest
{
    [TestClass]
    public class TransferringWorkerTest
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ClientDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.Scan_ConnectionString].ToStringSf();
            SQLWorker.Default.Start(ThreadPriority.Lowest, "SQLWorker");

            UserAuth auth = new UserAuth(826, "rosen.rusev", "rosen123$", true) { ClientID = 1 };
            DBConfigValue.Save(Strings.Transferring_AuthObject, auth);
            SettingsObj settings = new SettingsObj()
            {
                CentralServerUrl = "http://localhost/ReceivingService/DataService.svc",
                ConnectionString = ClientDataAccess.ConnectionString,
                KeepHistoryDays = 1,
                MaximumFilesForExport = 2,
                MaximumMessagesForExport = 10,
                SendInterval = 1,
            };
            settings.SiteCodeTable[250] = "100018P2250D2";
            settings.SiteCodeTable[300] = "100018P2300D2";
            settings.SiteCodeTable[826] = "100018P2826D2";
            DBConfigValue.Save(Strings.Transferring_SettingsObject, settings);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();
        }

        [TestMethod]
        public void OK_TransferringWorker_ThreadFunction()
        {
            TransferringWorkerClass cls = new TransferringWorkerClass();
            cls.Test();
        }
    }

    public class TransferringWorkerClass : TransferringWorker
    {
        public void Test()
        {
            base.ThreadFunction();
        }
    }
}
