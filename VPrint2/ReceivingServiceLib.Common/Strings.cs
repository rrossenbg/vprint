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
        public string VOCUHERSFOLDER { get; set; }
        public string VOCUHERSEXPORTFOLDER { get; set; }
        public string UPLOADERRORS { get; set; }
        public string VERSIONFOLDER { get; set; }
        public string pfxFileFullPath { get; set; }
        public string PTFLogoFileFullPath { get; set; }

        public static Strings Read()
        {
            var xml = File.ReadAllText(FILEPATH);
            XmlSerializer formatter = new XmlSerializer(typeof(Strings));
            var value = formatter.ToObject<Strings>(xml);
            if (value == null)
                throw new ArgumentNullException("value", "Can not create Strings from xml");
            return value;
        }

        public void Save()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Strings));
            var value = formatter.FromObject<Strings>(this);
            if (File.Exists(FILEPATH))
                File.Delete(FILEPATH);
            File.WriteAllText(FILEPATH, value);
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
