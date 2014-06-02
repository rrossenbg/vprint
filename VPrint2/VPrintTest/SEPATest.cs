using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrintTest.Properties;

namespace VPrintTest
{
    /// <summary>
    /// Summary description for SEPATest1
    /// </summary>
    [TestClass]
    public class SEPATest
    {
        public SEPATest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Testpain_TouristRefunds001_001_03()
        {

            string fileName = @"C:\Users\Rosen.rusev\Desktop\tr.xml";
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

            XmlTextReader xsdReader = new XmlTextReader(new StringReader(Resources.pain_001_001_03));
            xmlSchemaSet.Add(null, xsdReader);
            xmlSchemaSet.Compile();

            foreach (XmlSchema schema in xmlSchemaSet.Schemas())
            {
                Debug.Write("Schema with target namespace ", schema.TargetNamespace);
                Debug.WriteLine(" contains elements", schema.Elements.Count);
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(xmlSchemaSet);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(xmlReaderSettings_ValidationEventHandler);

            XmlReader xmlReader = XmlReader.Create(fileName, xmlReaderSettings);
            while (xmlReader.Read())
            {

            };

            Debug.WriteLine("Validation complete");
        }

        [TestMethod]
        public void Testpain_Rebates001_001_03()
        {
            string fileName = @"C:\Users\Rosen.rusev\Desktop\r.xml";
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

            XmlTextReader xsdReader = new XmlTextReader(new StringReader(Resources.pain_001_001_03));
            xmlSchemaSet.Add(null, xsdReader);
            xmlSchemaSet.Compile();

            foreach (XmlSchema schema in xmlSchemaSet.Schemas())
            {
                Debug.Write("Schema with target namespace ", schema.TargetNamespace);
                Debug.WriteLine(" contains elements", schema.Elements.Count);
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(xmlSchemaSet);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(xmlReaderSettings_ValidationEventHandler);

            XmlReader xmlReader = XmlReader.Create(fileName, xmlReaderSettings);
            while (xmlReader.Read())
            {

            };

            Debug.WriteLine("Validation complete");
        }

        [TestMethod]
        public void Testpain_DirectDebit008_001_02()
        {
            //string fileName = @"C:\Users\Rosen.rusev\Desktop\(AAA)aut_SEPA_DDPaymnt_E-4393_MM 008.001.02.XML";
            string fileName = @"C:\Users\Rosen.rusev\Desktop\dd3.xml";

            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

            XmlTextReader xsdReader = new XmlTextReader(new StringReader(Resources.pain_008_001_02));
            xmlSchemaSet.Add(null, xsdReader);
            xmlSchemaSet.Compile();

            foreach (XmlSchema schema in xmlSchemaSet.Schemas())
            {
                Debug.Write("Schema with target namespace ", schema.TargetNamespace);
                Debug.WriteLine(" contains elements", schema.Elements.Count);
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(xmlSchemaSet);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(xmlReaderSettings_ValidationEventHandler);

            XmlReader xmlReader = XmlReader.Create(fileName, xmlReaderSettings);
            while (xmlReader.Read())
            {

            };

            Debug.WriteLine("Validation complete");
        }

        static void xmlReaderSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Debug.Write("WARNING: ");
                Debug.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Debug.Write("ERROR: ");
                Debug.WriteLine(e.Message);
            }
        }
    }
}
