using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceivingServiceLib.FileWorkers;
using System;

namespace ReceivingServiceTest
{
    [TestClass]
    public class CovertWorkerTest
    {
        [TestMethod]
        public void coverworker_test()
        {
            CoverWorker worker = new CoverWorker();
            worker.Test();
        }

        [TestMethod]
        public void A()
        {
            string serverUrl0 = "http://192.168.53.144/";
            string serverUrl1 = "http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format=PDF&iso_id=724&Office=167150&in_date=02/12/2013&invoicenumber=42538";
            Uri u = new Uri(serverUrl1);
            string s = string.Concat("http://", u.Host, "/");
        }
    }
}
