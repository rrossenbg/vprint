using System;
using System.Windows.Forms;
using DEMATLib;
using DEMATLib.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace DEMATTest
{
    [TestClass]
    public class Test_Main
    {
        public Test_Main()
        {
            DEMARDataAccess.ConnectionString = "data source=192.168.58.97;initial catalog=PTF;persist security info=True;user id=sa;password=In-ert56uat;";
            InvoiceProcessor.Error += (o, e) => { MessageBox.Show(e.Exception.Message); };
            RetailerProcessor.Error += (o, e) => { MessageBox.Show(e.Exception.Message); };
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

        [TestMethod]
        public void demat_test_all_first_run()
        {
            var processor = new InvoiceProcessor(250, "C:\\test");
            processor.ZeroExportNumbers();
            processor.ProcessAll();
        }

        [TestMethod]
        public void demat_test_all_next_run()
        {
            var processor = new InvoiceProcessor(250, "C:\\test");
            processor.ProcessAll();
        }

        [TestMethod]
        public void demat_test_ho()
        {
            var processor = new InvoiceProcessor(250, "C:\\test");
            processor.ProcessOne(120327); //141902, 137480, 158173, 200116, 200980 
        }

        [TestMethod]
        public void demat_test_retailer_export()
        {
            var processor = new RetailerProcessor("C:\\test");
            processor.Process(new DateTime(2013, 8, 19)); //141902, 137480, 158173, 200116, 200980 
        }

        [TestMethod]
        public void ExportManager_test()
        {
            DIMATExportManager.ExportDirectory = "C:\\test";
            DIMATExportManager.Iso = 250;

            DIMATExportManager man = new DIMATExportManager();
            man.Start();
        }


        [TestMethod]
        public void test_enum_to_enum_convertion()
        {
            var value = (filetype)(TabAppearance)(SystemParameter)2;
            Debug.Assert(value == filetype.ERREUR);
        }

        [TestMethod]
        public void test_remove_last()
        {
            var str = "123456".RemoveLast(2);
        }
    }
}
