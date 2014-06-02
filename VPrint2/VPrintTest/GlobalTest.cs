using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace VPrintTest
{
    [TestClass]
    public class GlobalTest
    {
        [TestMethod]
        public void SubstringSafe_Test()
        {
            string test = "A".SubstringSafe(1, 100);
            Assert.AreEqual("1234567890asdfgh".SubstringSafe(10, 6), "1234567890asdfgh".Substring(10, 6));
            Assert.AreEqual("1234567890asdfgh".SubstringSafe(10, 8), "");
            Assert.AreEqual("1234567890asdfgh".SubstringSafe(6, 3), "789");
            Assert.AreEqual("1234567890asdfgh".SubstringSafe("1234567890asdfgh".Length - 1, 1), "h");

            IEnumerable<string> s = null;
            Assert.IsTrue(s.Count() == 0);
        }

        [TestMethod]
        public void CharOfString_Test()
        {
            string A = "0123456789ABCDEF";
            Assert.AreEqual(A.CharOfString(0), '0');
            Assert.AreEqual(A.CharOfString(4), '4');
            Assert.AreEqual(A.CharOfString(10), 'A');
            Assert.AreEqual(A.CharOfString(16), '0');
            Assert.AreEqual(A.CharOfString(34), '2');
        }

        [TestMethod]
        public void BuildString_Test()
        {
            const string LINE = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder b = new StringBuilder();
            int hLength = 10;
            int vLength = 30;

            int numberOflines = 5;

            for (int i = 0; i < vLength; i++)
            {
                b.Append(LINE.CharOfString(i));

                if (i % numberOflines == 0)
                {
                    for (int j = 0; j < hLength; j++)
                        b.Append(LINE.CharOfString(i + j + 1));
                    b.AppendLine();

                }
                else if (i % numberOflines == 1)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        b.Append(" ".Miltiply(j));
                        b.Append(LINE.CharOfString(i + j + 1));
                    }
                    b.AppendLine();
                }
                else if (i % numberOflines == 2)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        b.Append("\t".Miltiply(j));
                        b.Append(LINE.CharOfString(i + j + 1));
                    }
                    b.AppendLine();
                }
                else
                {
                    b.AppendLine();
                }
            }

            string str = b.toString();
        }

        [TestMethod]
        public void TestXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(@"C:\Users\Rosen.rusev\Desktop\Chanel_(Spain)_ES_2013-10-16-114513.xml"));
        }

        public enum eMode
        {
            NA,
            SingleScanNoDocument,
            SingleScanDocumentOrder,
            SingleScanDocumentMixed,

            MultiScanNoDocument,
            MultiScanDocumentOrder,
            MultiScanDocumentMixed,

            NoDocument = SingleScanNoDocument | MultiScanNoDocument,
            DocumentOrder = SingleScanDocumentOrder | SingleScanDocumentMixed | MultiScanDocumentOrder | MultiScanDocumentMixed,
            DocumentMixed = SingleScanDocumentMixed | MultiScanDocumentMixed,
        }

        [TestMethod]
        public void EnumTest()
        {
            Debug.Assert((eMode.DocumentMixed | eMode.MultiScanDocumentMixed) == eMode.MultiScanDocumentMixed);
            System.Data.SqlTypes.SqlMoney m = new System.Data.SqlTypes.SqlMoney(1);
                
        }
    }

    public static class Ex2
    {
        public static string SubstringSafe(this string value, int start, int count)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start");

            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length < start + count)
                return string.Empty;

            return value.Substring(start, count);
        }
    }
}
