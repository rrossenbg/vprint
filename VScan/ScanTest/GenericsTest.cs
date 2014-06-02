using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Management;
using PremierTaxFree.PTFLib;

namespace ScanTest
{
    [TestClass]
    public class GenericsTest
    {
        [TestMethod]
        public void dictionary_test()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["A"] = new object();
            data["A"] = new object();
            Assert.IsNotNull(data["A"]);
        }

        [TestMethod]
        public void copy_test()
        {

        }
    }
}
