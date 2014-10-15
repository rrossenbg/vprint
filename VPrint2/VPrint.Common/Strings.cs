/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/
using System.Reflection;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class Strings
    {
        /// <summary>
        /// BMP|*.bmp|JPG|*.jpg|PNG|*.png|All|*.*
        /// </summary>
        public const string ImageFilter = "BMP|*.bmp|JPG|*.jpg|PNG|*.png|All|*.*";
        /// <summary>
        /// DAT|*.dat|All|*.*
        /// </summary>
        public const string LayoutFilter = "DAT|*.dat|All|*.*";

        /// <summary>
        /// .dat
        /// </summary>
        public const string LayoutDefaultExt = ".dat";

        /// <summary>
        /// SubRangeFrom
        /// </summary>
        public const string SubRangeFrom = "SubRangeFrom";

        public const string IVoucherLayout = "IVoucherLayout";
        public const string CurrentPrintedPage = "CurrentPrintedPage";

        /// <summary>
        /// Index
        /// </summary>
        public const string Index = "Index";

        ////Purchase Information:
        ////Barcode Reader SDK (1D) - Single Developer License (#300444310) 1 Unit(s)
        ////-   	License key for unlocking the product
        ////The license key for "Barcode Reader SDK (1D) - Single Developer License" is listed below. 
        ////  You will need this license key to complete the process and activate the product.
        ////This product is licensed to the name "Premier Tax Free UK limited".
        ////
        ////X772D630820YPCFQOS7B
        ////
        ////Runtime licenses:
        ////RES2LPVANKXX5XPDOS7B
        ////PQ19VDCVD40X2XLEUQ0E
        ////A3IEHHPRP34L9W9NCWL5
        ////SQ58YULN8EJWVGSCUQ0E
        ////NHU0MGS1I3372XLEOS7B 
        /// <summary>
        /// QXCWIX4FUU8LIMDY
        /// </summary>
        public const string VScan_BarcodeReaderSDKDeveloperLicenseKey = "X772D630820YPCFQOS7B";

        /// <summary>
        /// 5AKAXDCVFJPSA3ZHEJ55
        /// </summary>
        public const string VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey = "5AKAXDCVFJPSA3ZHEJ55";

        /// <summary>
        /// VOUCHERCOVERREGION
        /// </summary>
        public const string VOUCHERCOVERREGION = "VOUCHERCOVERREGION";

        public const string PIXELSIZE = "PIXELSIZE";

        /// <summary>
        /// VPRNT
        /// </summary>
        public const string VRPINT = "VPRNT";

        /// <summary>
        /// No document. No order.
        /// Everything going gets in.
        /// Must be a barcode.
        /// </summary>
        public const string SingleScanNoDocument = @"No document. No order.
Everything going gets in.
Must be a barcode.";

        /// <summary>
        /// There must be document. Single scan.
        /// Next voucher coming should be expected one.
        /// </summary>
        public const string SingleScanDocument = @"There must be document. Single scan.
Next voucher coming should be expected one.";


        /// <summary>
        /// No document. Multi scan.
        /// Click F2 to complete current voucher.
        /// </summary>
        public const string MultiScanNoDocument = @"No document. Multi scan.
Click F2 to complete current voucher";

        /// <summary>
        /// There must be document. Multi scan. Scanner scans TIFF document.
        /// If there is a barcode the barcode has been validated against document.
        /// Otherwise it has been inserted by using expected barcode details.
        /// </summary>
        public const string MultiScanDocument = @"There must be document. Multi scan. Scanner scans TIFF document.
If there is a barcode the barcode has been validated against document.
Otherwise it has been inserted by using expected barcode details.";

        /// <summary>
        /// This mode is not possible
        /// </summary>
        public const string ThisModeIsNotPossible = "This mode is not possible";
        public const string DOWNLOAD = "DOWNLOAD";
        public const string VERSION = "VERSION";

        public const string CERTIFICATE_SIGNING_AVAILABLE = "CERTIFICATE_SIGNING_AVAILABLE";
        public const string LIST_OF_BARCODECONFIGS = "List of BarcodeConfigs";
        public const string COUNTRY_CERTIFICATE_PATH = "COUNTRY_CERTIFICATE_PATH";
        public const string COUNTRY_CERTIFICATE_PASS = "COUNTRY_CERTIFICATE_PASS";
        public const string PTFLogoFileFullPath = "PTFLogoFileFullPath";
        public const string Certigicate_COUNTRY = "Certigicate_COUNTRY";
        public const string Certigicate_LOCATION = "Certigicate_LOCATION";
        public const string Certigicate_METADATA_FUNC = "Certigicate_METADATA_FUNC";

        /// <summary>
        /// {
        /// [Subject]
        ///   CN=localhost
        /// [Issuer]
        ///   CN=localhost
        /// [Serial Number]
        ///   3C35B35DAD31648C43DEFBA51B021336
        /// [Not Before]
        ///   30/05/2012 14:51:43
        /// [Not After]
        ///   30/05/2022 01:00:00
        /// [Thumbprint]
        ///   7C2D0635209FD29EBE3526591CC45881283F94E6
        /// }
        /// Archived: false
        /// Extensions: {System.Security.Cryptography.X509Certificates.X509ExtensionCollection}
        /// FriendlyName: "IIS Express Development Certificate"
        /// HasPrivateKey: true
        /// IssuerName: {System.Security.Cryptography.X509Certificates.X500DistinguishedName}
        /// NotAfter: {30/05/2022 01:00:00}
        /// NotBefore: {30/05/2012 14:51:43}
        /// PrivateKey: {System.Security.Cryptography.RSACryptoServiceProvider}
        /// PublicKey: {System.Security.Cryptography.X509Certificates.PublicKey}
        /// RawData: {byte[469]}
        /// SerialNumber: "3C35B35DAD31648C43DEFBA51B021336"
        /// SignatureAlgorithm: {System.Security.Cryptography.Oid}
        /// SubjectName: {System.Security.Cryptography.X509Certificates.X500DistinguishedName}
        /// Thumbprint: "7C2D0635209FD29EBE3526591CC45881283F94E6"
        /// Version: 3
        /// </summary>
        public const string CERTNUMBER = "3C35B35DAD31648C43DEFBA51B021336";

        public const string ClearScanDirectory = "ClearScanDirectory";
        public const string tbScanDirectory = "tbScanDirectory";
        public const string ScanCopyWait = "ScanCopyWait";
        public const string ScanCopyTimeout = "ScanCopyTimeout";

        public const string CROPIMAGE = "CROPIMAGE";
    }

    public static class Messages
    {
        public const string UnprocessedItemsCloseAnyway = "There are unprocessed items.\r\nClose anyway?";
        public const string UnsentItemsCloseAnyway = "There are unsent items.\r\nClose anyway?";
        public const string ScanningNotRun = "Scanning process is not run.\nClick on Run first.";
        public const string ScanDirMayNotbeEmpty = "Scan directory may not be empty";
        public const string CanNotFindDir = "Cannot find directory ";
        public const string NoDocumenLoadedLoadIt = "Nothing to process.\r\nPlease load transfer first.";
        public const string DontScanToRootFolder = "Please create a folder and open it.\r\nDon't scan to image root directly.";
        public const string InvalidCountryId = "CountryId is not valid.\r\nPlease re-enter.";
        public const string LoadVoucherFromCache = "Load Vouchers From Cache";

        public const string PleaseWaitOutput = "Please wait generating output...";

        public const string VoucherStatus_2 = "Session : {0} Status : {1}";
        public const string ItemsProcessed_1 = "Items processed : {0}";
        public const string ItemsSent_1 = "Items sent : {0}";
    }

    public static class Consts
    {
        /// <summary>
        /// 999
        /// </summary>
        public const int MAX_COUNTRYID = 999;

        /// <summary>
        /// 999999
        /// </summary>
        public const int MAX_RETAILERID = 999999;
        /// <summary>
        /// 9999999
        /// </summary>
        public const int MAX_RETAILERID_CD = 9999999;

        /// <summary>
        /// 99999999
        /// </summary>
        public const int MAX_VOUCHERID = 99999999;

        /// <summary>
        /// 999999999
        /// </summary>
        public const int MAX_VOUCHERID_CD = 999999999;
    }
}
