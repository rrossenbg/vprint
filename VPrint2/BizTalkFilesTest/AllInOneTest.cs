using System;
using System.IO;
using BizTalkFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizTalkFilesTest
{
    [TestClass]
    public class AllInOneTest
    {
        [TestMethod]
        public void logging_test()
        {
            try
            {
                throw new OutOfMemoryException();
            }
            catch (Exception ex)
            {
                string fileName = "{0}{1:_dd_MM_yyyy}.log".format("c:\\app.exe", DateTime.Now);
                using (var file = File.AppendText(fileName))
                {
                    file.WriteLine("ERR");
                    file.WriteLine(DateTime.Now);
                    file.WriteLine();
                    file.WriteLine(ex);
                    file.WriteLine();
                    file.WriteLine("====================================================");
                }
            }
        }

        [TestMethod]
        public void string_test()
        {
            var str = "".Limit(4);
            var str2 = "strstr".Limit(6);
            var str3 = "strstr".Limit(116);
        }
    }
}
