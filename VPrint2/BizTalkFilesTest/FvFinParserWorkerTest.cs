using System;
using System.Threading;
using BizTalkFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizTalkFilesTest
{
    [TestClass]
    public class FvFinParserWorkerTest
    {
        public static FvFinParserWorker worker = new FvFinParserWorker();

        public FvFinParserWorkerTest()
        {
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            worker.Stop();
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
        public void FvFinParserWorker_workerfunction_test()
        {
            worker.ArchiveFolder = @"C:\DiData\PTF\Locations\FvFIn\Archive";
            worker.ErrorFolder = @"C:\DiData\PTF\Locations\FvFin\Error";
            worker.FvFinInputFolder = @"C:\DiData\PTF\Locations\FvFin";
            worker.FvFinParsedFolder = @"C:\DiData\PTF\Locations\FvFin\Parsed";
            worker.MaxProcessFilesCount = 12;
            worker.ParsedErrFolder = @"C:\DiData\PTF\Locations\FvFin\Parsed_Error";
            worker.SleepTime = TimeSpan.FromMinutes(1);
            worker.Start(ThreadPriority.Lowest, "Test");
        }
    }
}
