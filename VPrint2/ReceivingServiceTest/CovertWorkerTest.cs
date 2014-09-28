using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceivingServiceLib.FileWorkers;

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
    }
}
