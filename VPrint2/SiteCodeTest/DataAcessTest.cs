using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SiteCodeLib;
using System.Diagnostics;
using System;

namespace SiteCodeTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DataAcessTest
    {
        public DataAcessTest()
        {
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void sitecode_merge_locations_test()
        {
            DataAccess.ConnectionString = "data source=192.168.58.57;initial catalog=ptf;persist security info=False;user id=sa;pwd=sa;packet size=4096;";
            var lookupLocations = DataAccess.LoadLocationsFromLocations();

            var voucherPartLocations = DataAccess.LoadLocationsFromVoucherPart();
            
            var locations = DataAccess.JoinLocations(voucherPartLocations, lookupLocations);
            DataAccess.SaveLocationsToFile(locations, "C:\\test\\locations.xml");

            SiteCodeObject server = new SiteCodeObject();
            server.LoadLocations(locations);

            for (int i = 0; i < 100; i++)
            {
                foreach (var site in new string[] {
                    "100018P1348D2",
                    "100018P1504D2",
                    "100018P1752D2",
                    "100018P1858D2",
                    "100018P1Half300D2",
                    "100018P1Half901D2",
                    "100018P2300D2",
                    "100018P232D2",
                    "100018P2484D2",
                    "100018P2504D2",
                    "100018P2858D1",
                    "100018P2858D2",
                    "100018P2901D2"
                })
                {
                    var result = server.GetLocation(site);
                    Trace.WriteLine(site + "\t" + result);
                }
            }

            DataAccess.SaveLocations(server.SaveLocations());
        }

        [TestMethod]
        public void test_live_sitecode_service()
        {
            var t = TimeSpan.Parse("10:00");
            Debug.WriteLine(t);
            using (var p = new Proxy.SiteCodeClient())
            {
                p.SaveCommand();
            }
        }
    }
}
