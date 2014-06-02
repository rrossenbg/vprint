using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

namespace ScanTest
{
    [TestClass]
    public class ServerDataAccessTest
    {
        public ServerDataAccessTest()
        {
            ServerDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.WebService_ConnectionString].ToStringSf();            
        }

        [TestMethod]
        public void serverdataaccess_insertfile_test()
        {
        }

        [TestMethod]
        public void serverdataaccess_selectfiles_test()
        {
        }
    }
}
