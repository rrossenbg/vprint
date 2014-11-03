using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DEMATLib;

namespace DEMATTest
{
    [TestClass]
    public class BDV_RetailerInfoTest
    {
        [TestMethod]
        public void test_BDV_RetailerInfoTest_serialization()
        {
            BDV_RetailerInfoBuilder b = new BDV_RetailerInfoBuilder();
            b.AddRetailer(123, DateTime.Now, "rrossenbg1@yahoo.com", true);
            b.AddRetailer(124, DateTime.Now, "rrossenbg2@yahoo.com", true);
            b.AddRetailer(125, DateTime.Now, "rrossenbg3@yahoo.com", true);
            b.AddRetailer(126, DateTime.Now, "rrossenbg4@yahoo.com", true);
            b.AddRetailer(127, DateTime.Now, "rrossenbg5@yahoo.com", true);
            string xml = b.CreateXML();
        }
    }
}
