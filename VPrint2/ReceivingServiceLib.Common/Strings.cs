/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using VPrinting;

namespace ReceivingServiceLib
{
    [Serializable]
    [Obfuscation(StripAfterObfuscation = true)]
    public class Strings
    {
        private const string FILEPATH = "C:\\strings.xml";
        /// <summary>
        /// RSV
        /// </summary>
        public const string APPNAME = "RSV";
        public const string APPNAMEWEB = "RSVWEB";
        public const string SingleSaleCountries = "SingleSaleCountries";
        public const string COVER = "COVER";

        /// <summary>
        /// Images connection string
        /// </summary>
        public string ConnString { get; set; }

        /// <summary>
        /// PTF connection string
        /// </summary>
        public string PTFConnString { get; set; }

        /// <summary>
        /// Upload folder
        /// </summary>
        public string UPLOADROOT { get; set; }

        /// <summary>
        /// Download folder
        /// </summary>
        public string DOWNLOADROOT { get; set; }

        /// <summary>
        /// Country/ Retailer/ Voucher hierarchy
        /// </summary>
        public string VOCUHERSFOLDER { get; set; }

        /// <summary>
        /// Export service folder
        /// </summary>
        public string VOCUHERSEXPORTFOLDER { get; set; }

        /// <summary>
        /// Cover service folder
        /// </summary>
        public string COVERWORKFOLDER { get; set; }

        /// <summary>
        /// Content service folder
        /// </summary>
        public string CONTENTWORKFOLDER { get; set; }

        /// <summary>
        /// General work folder
        /// </summary>
        public string SERVICESWORKFOLDER { get; set; }

        /// <summary>
        /// General error folder
        /// </summary>
        public string UPLOADERRORS { get; set; }

        /// <summary>
        /// Version folder
        /// </summary>
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
            COVERWORKFOLDER = "C:\\";
            UPLOADERRORS = "C:\\";
            pfxFileFullPath = "C:\\test.pfx";
            PTFLogoFileFullPath = "C:\\test.png";
        }

        public bool IsValid()
        {
            return
                !string.IsNullOrWhiteSpace(ConnString) &&
                !string.IsNullOrWhiteSpace(PTFConnString) &&
                !string.IsNullOrWhiteSpace(UPLOADROOT) &&
                !string.IsNullOrWhiteSpace(VOCUHERSFOLDER) &&
                !string.IsNullOrWhiteSpace(VOCUHERSEXPORTFOLDER) &&
                !string.IsNullOrWhiteSpace(COVERWORKFOLDER) &&
                !string.IsNullOrWhiteSpace(UPLOADERRORS) &&
                !string.IsNullOrWhiteSpace(pfxFileFullPath) &&
                !string.IsNullOrWhiteSpace(PTFLogoFileFullPath);
        }
    }
}
