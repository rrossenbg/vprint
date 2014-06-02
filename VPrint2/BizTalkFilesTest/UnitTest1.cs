using System.IO;
using System.Linq;
using System.Xml.Linq;
using BizTalkFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizTalkFilesTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void processor_Process_test()
        {
            AppInfoHolder h = new AppInfoHolder();
            h.ArchiveFolder = "C:\\TEST";
            h.ErrorFolder = "C:\\TEST";
            h.FvFinInputFolder = "C:\\TEST";
            h.FvFinParsedFolder = "C:\\TEST";
            h.ParsedErrFolder = "C:\\TEST";

            var file = new FileInfo("c:\\test1.xml");

            RootElementProcessor processor = new RootElementProcessor(h);

            var elements = XDocument.Load(file.FullName).Descendants("root").ToList<XElement>();

            foreach (XElement element in elements)
                processor.Process(element, file, null);
        }
    }
}
