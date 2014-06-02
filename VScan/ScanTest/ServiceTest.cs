using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PremierTaxFree;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.DataServiceProxy;
using PremierTaxFree.PTFLib.Net;
using System.IO;
using System.Threading;

namespace ScanTest
{
    [TestClass]
    public class ServiceTest
    {
        public ServiceTest()
        {
            ClientDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.Scan_ConnectionString].ToStringSf();            
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            SQLWorker.Default.Start(System.Threading.ThreadPriority.Lowest, "SQLWorker");
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        UserAuth user;
        
        #region USERDATA
        const string USER = "rosen.rusev";
        const string PASS = "rosen123$";
        #endregion

        [TestMethod]
        public void OK_dataservice_create_client()
        {
            user = AuthenticationClient.CallAuthenticateUser(826, USER, PASS);
            user.ClientID = DataServiceClient.CallCreateClient(Environment.MachineName);
        }

        [TestMethod]
        public void OK_dataservice_getcountries()
        {
            user = AuthenticationClient.CallAuthenticateUser(826, USER, PASS);
            user.ClientID = DataServiceClient.CallCreateClient(Environment.MachineName);
            var data = DataServiceClient.CallQueryContries(user);
        }

        [TestMethod]
        public void OK_dataservice_querysiteCodes()
        {
            var siteCodes = DataServiceClient.CallQuerySiteCodes(1, 826, "100018P2826D2", 100);
        }

        [TestMethod]//OK
        public void OK_dataservice_querysiteCodes_insertdatabase()
        {
            var siteCodes = DataServiceClient.CallQuerySiteCodes(1, 826, "100018P2826D2", 100);
            ClientDataAccess.InsertFileAsync(826, siteCodes);
            Thread.Sleep(1000);
        }

        [TestMethod]//OK
        public void OK_dataservice_savevoucher()
        {
            var barcode = File.ReadAllBytes("C:\\test.bmp");
            for (int i = 0; i < 14; i++)
                DataServiceClient.CallSaveVoucher(1, 826, 1110, "voucr_id" + i, "AA" + (29656 + i), "na",
                    DateTime.Now, barcode, barcode);
        }
    }
}
