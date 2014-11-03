using System.Diagnostics;
using DEMATLib.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DEMATLib;
using System.Data;
using System.Data.SqlClient;
using System;

namespace DEMATTest
{
    [TestClass]
    public class Test_DataAccess
    {
        public Test_DataAccess()
        {
            DEMARDataAccess.ConnectionString = "data source=192.168.58.97;initial catalog=PTF;persist security info=True;user id=sa;password=In-ert56uat;";
        }

        private TestContext testContextInstance;

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
        public void dataaccess_logging()
        {
            DEMARDataAccess.LogVoucherExported("FintraxDEMATService", 250, 123456, "testTESTtest");
        }

        [TestMethod]
        public void dataaccess_select_allvalid_vouchers()
        {
            //var result = DataAccess.SelectAllVouchersToExport(250, DateTime.Today, false);
            //foreach (var r in result)
            //    Trace.WriteLine(r.v_br_id + "  " + r.v_number);
        }

        [TestMethod]
        public void dataaccess_select_allvoided_vouchers()
        {
            //var result = DataAccess.SelectAllVouchersToExport(250, DateTime.Today, true);
            //foreach (var r in result)
            //    Trace.WriteLine(r.v_br_id + "  " + r.v_number);
        }

        [TestMethod]
        public void dataaccess_select_voucher()
        {
            var v = DEMARDataAccess.SelectVoucher(250, 21627943);

            Debug.WriteLine("=========================================");
            Debug.WriteLine(v.v_number, "v_number");
            Debug.WriteLine(v.v_date_voucher, "v_date_voucher");
            Debug.WriteLine(v.ho_vat_number, "ho_vat_number");
            Debug.WriteLine(v.br_id, "vl_pp_vat");
            Debug.WriteLine(v.br_name, "br_name");
            Debug.WriteLine(v.br_add_1, "br_add_1");
            Debug.WriteLine(v.br_add_2, "br_add_2");
            Debug.WriteLine(v.br_add_3, "br_add_3");
            Debug.WriteLine(v.br_add_4, "br_add_4");
            Debug.WriteLine(v.br_add_5, "br_add_5");
        }

        [TestMethod]
        public void dataaccess_select_voucher_lines()
        {
            var list = DEMARDataAccess.SelectVoucherLines(250, 21627943);
            foreach (var l in list)
            {
                Debug.WriteLine("=========================================");
                Debug.WriteLine(l.vl_line_number, "vl_line_number");
                Debug.WriteLine(l.vl_pp_excl_vat, "vl_pp_excl_vat");
                Debug.WriteLine(l.vl_pp_incl_vat, "vl_pp_incl_vat");
                Debug.WriteLine(l.vl_pp_vat, "vl_pp_vat");
                Debug.WriteLine(l.vl_quantity, "vl_quantity");
                Debug.WriteLine(l.vl_unit_price, "vl_unit_price");
            }
        }

        [TestMethod]
        public void dataaccess_set_voucher_sent()
        {
            DEMARDataAccess.SetVoucherSentToDemat(250, 21627943);
        }

        [TestMethod]
        public void dataaccess_test_string_formating()
        {
            Assert.AreEqual("0000000001", 1.ToString(10));
            Assert.AreEqual("1", 1.ToString(1));
            Assert.AreEqual("1", 1.ToString(0));
            Assert.AreEqual("1", 1.ToString(-3));
        }

        [TestMethod]
        public void dataaccess_reset_database()
        {
            const string SQL = "update Voucher set v_sent_to_demat = null  where v_iso_id = 250";

            using (SqlConnection conn = new SqlConnection(DEMARDataAccess.ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
