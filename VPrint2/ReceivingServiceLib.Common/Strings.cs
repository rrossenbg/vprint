/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace ReceivingServiceLib
{
    [Serializable]
    [Obfuscation(StripAfterObfuscation = true)]
    public class Strings
    {
        private const string FILEPATH = "C:\\strings.xml";
        public const string APPNAME = "RSV";
        public const string APPNAMEWEB = "RSVWEB";
        public const string SingleSaleCountries = "SingleSaleCountries";

        public string ConnString { get; set; }
        public string PTFConnString { get; set; }
        public string UPLOADROOT { get; set; }
        public string DOWNLOADROOT { get; set; }

        public string VOCUHERSFOLDER { get; set; }
        public string VOCUHERSEXPORTFOLDER { get; set; }
        public string UPLOADERRORS { get; set; }
        public string VERSIONFOLDER { get; set; }
        public string pfxFileFullPath { get; set; }
        public string PTFLogoFileFullPath { get; set; }

        public static Strings Read()
        {
#if DEBUG
#warning !!! TEST CONNECTION STRINGS !!!
            var sc = new Strings();
            sc.LoadForTest();
            return sc;
#else
            var xml = File.ReadAllText(FILEPATH);
            XmlSerializer formatter = new XmlSerializer(typeof(Strings));
            var value = formatter.ToObject<Strings>(xml);
            if (value == null)
                throw new ArgumentNullException("value", "Can not create Strings from xml");
            return value;
#endif
        }

        public void Save()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Strings));
            var value = formatter.FromObject<Strings>(this);
            if (File.Exists(FILEPATH))
                File.Delete(FILEPATH);
            File.WriteAllText(FILEPATH, value);
        }

        public void LoadForTest()
        {
            ConnString = "data source=192.168.58.97;initial catalog=ptf_images;persist security info=False;user id=sa;pwd=In-ert56uat;packet size=4096;";
            PTFConnString = "data source=192.168.58.97;initial catalog=ptf;persist security info=False;user id=sa;pwd=In-ert56uat;packet size=4096;";
            UPLOADROOT = "C:\\";
            DOWNLOADROOT = "C:\\";
            VOCUHERSFOLDER = "C:\\";
            VOCUHERSEXPORTFOLDER = "C:\\";
            UPLOADERRORS = "C:\\";
            pfxFileFullPath = "C:\\test.pfx";
            PTFLogoFileFullPath = "C:\\test.png";
        }

        public bool IsValid()
        {
            return
                !ConnString.IsNullOrWhiteSpace() &&
                !PTFConnString.IsNullOrWhiteSpace() &&
                !UPLOADROOT.IsNullOrWhiteSpace() &&
                !VOCUHERSFOLDER.IsNullOrWhiteSpace() &&
                !VOCUHERSEXPORTFOLDER.IsNullOrWhiteSpace() &&
                !UPLOADERRORS.IsNullOrWhiteSpace() &&
                !pfxFileFullPath.IsNullOrWhiteSpace() &&
                !PTFLogoFileFullPath.IsNullOrWhiteSpace();
        }
    }
}
