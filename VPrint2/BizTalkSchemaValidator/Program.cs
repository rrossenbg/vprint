using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using BizTalkSchemaValidator.Properties;

namespace BzTConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("expect xml file name");
                return;
            }
            string fileName = args[0];
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

            XmlTextReader xsdReader = new XmlTextReader(new StringReader(Resources.CommonDocument));
            xmlSchemaSet.Add(null, xsdReader);
            xmlSchemaSet.Compile();
            foreach (XmlSchema schema in xmlSchemaSet.Schemas())
            {
                Console.Write("Schema with target namespace {0}", schema.TargetNamespace);
                Console.WriteLine(" contains {0} elements", schema.Elements.Count);
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(xmlSchemaSet);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(xmlReaderSettings_ValidationEventHandler);

            XmlReader xmlReader = XmlReader.Create(fileName, xmlReaderSettings);
            while (xmlReader.Read()) 
            { 

            };

            Console.WriteLine("Validation complete");
            Console.ReadLine();
        }

        static void xmlReaderSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
