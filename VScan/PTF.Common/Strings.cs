/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

namespace PremierTaxFree.PTFLib
{
    /// <summary>
    /// String table
    /// </summary>
    public static class Strings
    {
        #region CONSTS

        public const string Empty = "";

        /// <summary>
        /// "&lt;@EMPTY@&gt;"
        /// </summary>
        public const string Empty2 = "<@EMPTY@>";
        /// <summary>
        /// rosen
        /// </summary>
        public const string Rosen = "rosen";

        /// <summary>
        /// "BMP|*.bmp|JPG|*.jpg|PNG|*.png|All|*.*"
        /// </summary>
        public const string VScan_ImageFilter = "BMP|*.bmp|JPG|*.jpg|PNG|*.png|All|*.*";

        #endregion

        #region DEFAULTS

        //Transferring service
        public const string Transferring_VersionFileFullFileName = "VersionFileFullFilePath";
        public const string Transferring_LocalDeploymentPath = "LocalDeploymentPath";
        public const string Transferring_VersionFileName = "VersionFileName";
        public const string Transferring_CurrentVersion = "CurrentVersion";
        /// <summary>
        /// Site Code: [SITEID] | [DATE]
        /// </summary>
        public const string VScan_ImPrinterTemplateDefault = "Site Code: [SITEID] | [DATE]";

        /// <summary>
        /// NET_PIPE_1
        /// </summary>
        public const string Transferring_PipeChannelName = "NET_PIPE_1";
        /// <summary>
        /// List of UserAuth
        /// </summary>
        public const string Transferring_AuthObjectList = "AurhObjectList";
        public const string Transferring_AuthObject = "AurhObject";
        public const string Transferring_SuperAdminAuthObject = "SuperAdminAurhObject";
        public const string Transferring_CurrentUICultureInfo = "CurrentUICultureInfo";
        public const string Transferring_SettingsObject = "SettingsObject";
        /// <summary>
        /// "tcp://localhost:12345/RemoteObj"
        /// </summary>
        public const string Transferring_RemoteObjectUrl = "tcp://localhost:12345/RemoteObj";

        /// <summary>
        /// SQL1
        /// </summary>
        public const string All_CentralSQLServerInstance = "SQL1";
        /// <summary>
        /// PTFVoucher
        /// </summary>
        public const string All_CentralDbName = "PTFVoucher";
#if DEBUG
        /// <summary>
        /// SQL2
        /// </summary>
        public const string All_LocalSQLServerInstance = "SQL2";
#else
        /// <summary>
        /// SQLEXPRESS
        /// </summary>
        public const string All_LocalSQLServerInstance = "SQLEXPRESS";
#endif
        /// <summary>
        /// PTFLocal
        /// </summary>
        public const string All_LocalDbName = "PTFLocal";

        public const string All_CentralServerUrl = "CentralServerUrl";
        /// <summary>
        /// http://localhost/ReceivingService/DataService.svc
        /// </summary>
        public const string All_CentralServerUrlPathDefault = "http://localhost/ReceivingService/DataService.svc";

        /// <summary>
        /// .\\Private$\\SaveQueue
        /// </summary>
        public const string All_SaveQueueName = ".\\Private$\\SaveQueue";

        public const string VScan_TRS_UrlAddress = "TRS_UrlAddress";
        /// <summary>
        /// http://192.168.58.57/Ptfwebsite/Default.aspx
        /// </summary>
        public const string VScan_TRS_UrlAddressDefault = "http://192.168.58.57/Ptfwebsite/Default.aspx";

        public const string VScan_WebTracerUrl = "WebTracerUrl";
        /// <summary>
        /// http://127.0.0.1:8080
        /// </summary>
        public const string VScan_WebTracerUrlDefault = "http://127.0.0.1:8080";

        /// <summary>
        /// QXCWIX4FUU8LIMDY
        /// </summary>
        public const string VScan_BarcodeReaderSDKDeveloperLicenseKey = "QXCWIX4FUU8LIMDY";

        /// <summary>
        /// 5AKAXDCVFJPSA3ZHEJ55
        /// </summary>
        public const string VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey = "5AKAXDCVFJPSA3ZHEJ55";

        #endregion//DEFAULTS

        #region NAMES

        /// <summary>
        /// PTFScanDB
        /// </summary>
        public const string WebService_ConnectionString = "PTFScanDB";

        /// <summary>
        /// PTFLocalDB
        /// </summary>
        public const string Scan_ConnectionString = "PTFLocalDB";

        /// <summary>
        /// PTFLocalDB
        /// </summary>
        public const string WinService_ConnectionString = "PTFLocalDB";

        /// <summary>
        /// PTFDB
        /// </summary>
        public const string WebService_ConnectionStringPTF = "PTFDB";

        /// <summary>
        /// NET_PIPE_2
        /// </summary>
        public const string VScan_PipeChannelName = "NET_PIPE_2";
        public const string VScan_ScanIsDoneEvent = "<@SCANISDONE@>";
        public const string VScan_SourceIsClosedEvent = "<@SOURCECLOSED@>";
        /// <summary>
        /// TransferringService
        /// </summary>
        public const string TransferringService = "TransferringService";
        /// <summary>
        /// ReceivingWebService
        /// </summary>
        public const string ReceivingWebService = "ReceivingWebService";
        /// <summary>
        /// VScan
        /// </summary>
        public const string VScan = "VScan";
        /// <summary>
        /// PTF Agent 2011
        /// </summary>
        public const string VScan_UserAgent = "PTF Agent 2011";

        /// <summary>
        /// Name of settings to save into database
        /// </summary>
        public const string VScan_SettingsTable = "SettingsTable";

        /// <summary>
        /// FUJITSU fi-5120Cdj
        /// </summary>
        public const string VScan_DefaultScannerName = "FUJITSU fi-5120Cdj";

        public const string TransferringService_Key = "TransferringService_Key";

        #endregion //NAMES

        #region COMMANDS

        public const string VScan_EditItem = "<@EDITITEM@>";
        public const string VScan_DesktopUnlock = "<@UNLOCKTHEDESKTOP@>";

        /// <summary>
        /// MainForm sends this message to ScanForm to stop scanning
        /// </summary>
        public const string VScan_StopTheScanner = "<@STOPTHESCANNER@>";

        /// <summary>
        /// This message has been send by 
        /// Transffering service to lock the VScan UI
        /// </summary>
        public const string Transferring_RemoteLock = "<@REMOTELOCK@>";

        #endregion //COMMANDS

        #region INFO

        /// <summary>
        /// ScanForm sends this message to MainForm to inform 
        /// an xml was inserted to the database
        /// </summary>
        public const string VScan_ItemSaved = "File saved|";

        #endregion //INFO

        #region OTHER
        public const string VScan_DefaultFontFamily = "VScan_DefaultFontFamily";
        public const string VScan_DefaultFontSize = "VScan_DefaultFontSize";
        public const string VScan_DefaultForeColor = "VScan_DefaultForeColor";
        public const string VScan_DefaultBackColor = "VScan_DefaultBackColor";
        public const string VScan_DefaultLineSize = "VScan_DefaultLineSize";
        
        public const string VScan_CertificateSerialNumber = "A1928ED088DB27844420CBF6A879E9F2";
        public const string VScan_SinglentonFormClose = "SinglentonFormClose";
        #endregion//OTHER

        #region SETTINGS

        /// <summary>
        /// Name of default settings
        /// </summary>
        public const string VScan_DefaultSettingsName = "DefaultSettingsName";

        public const string VScan_TWAINUseDefaultScanner = "TWAINUseDefaultScanner";
        public const string VScan_TWAINUseDefaultScannerSettings = "TWAINUseDefaultScannerSettings";
        public const string VScan_ScanFormAutoClose = "SettingsAutoCloseScanForm";
        //Connections
        public const string VScan_TranfSendInterval = "TranfSendTime";
        //General
        public const string VScan_AutoInstallService = "AutoInstallService";
        public const string VScan_AutoReadBarcodeAfterScan = "AutoReadBarcodeAfterScan";
        public const string VScan_AutoInsertDataAfterScan = "AutoInsertDataAfterScan";
        public const string VScan_MaximumOpenedScanForms = "MaximumOpenedForms";
        public const string VScan_MinSiteIDSAvailable = "MinSiteIDSAvailable";
        public const string VScan_KeepHistoryDays = "KeepHistoryDays";
        public const string VScan_ConnectionString = "PTFLocalConnectionString";
        public const string VScan_DefaultCountryCode = "DefaultCountryCode";
        /// <summary>
        /// Color of border to be removed from the image
        /// </summary>
        public const string VScan_ImageBorderColor = "ImageBorderColor";
        public const string VScan_ImageBorderColorDistance = "ImageBorderColorDistance";
        /// <summary>
        /// Minimum available free siteids in cache
        /// </summary>
        public const string VScan_MinimumAuditIDsInCache = "MinimumAuditIDsInCache";
        /// <summary>
        /// 
        /// </summary>
        public const string VScan_DistanceFromBarcodeBottomLeftToHiddenArea = "DistanceFromBarcodeBottomLeftToHiddenArea";
        /// <summary>
        /// hide area size
        /// </summary>
        public const string VScan_HiddenAreaSize = "HideAreaSize";
        /// <summary>
        /// Area to be use for printing SiteCodeID and other info
        /// </summary>
        public const string VScan_PrintOnImage = "PrintOnImage";
        /// <summary>
        /// Area to be use for printing SiteCodeID
        /// </summary>
        public const string VScan_PrintAreaLocation = "PrintAreaLocation";
        /// <summary>
        /// Clean after 15 secs
        /// </summary>
        public const string VScan_SleepBeforeCleanTime = "SleepBeforeCleanTime";

        public const string VScan_Email_To = "Email_To";
        public const string VScan_Email_Subject = "Email_Subject";
        public const string VScan_Email_Body = "Email_Body";

        public const string VScan_ApplicationPrinterName = "ApplicationPrinterName";
        public const string VScan_UseImPrinter = "UseImPrinter";
        public const string VScan_ImPrinterTemplate = "ImPrinterTemplate";

        public const string VScan_LastStarted = "LastStarted";
        public const string VScan_ScanCount = "ScanCount";

        /// <summary>
        /// use either barcode or mark to find hidden area
        /// </summary>
        public const string VScan_HideCardCodeDetailsBybarcode = "HideCardCodeDetailsBybarcode";

        /// <summary>
        /// Voucher Image
        /// </summary>
        public const string VScan_VoucherLayout = "VoucherLayout";

        /// <summary>
        /// Allows/denies crop tool
        /// </summary>
        public const string VScan_AllowCropTool = "AllowCropTool";

        /// <summary>
        /// Configuration of mark into the voucher that will be seek for
        /// </summary>
        public const string VScan_MarkAreaConfiguration = "MarkAreaConfiguration";

        /// <summary>
        /// Configurates drawing tools to draw over the hidden area 
        /// </summary>
        public const string VScan_HiddenAreaDrawingCfg = "HiddenAreaDrawingCfg";

        /// <summary>
        /// Level of compression to use for saveing images
        /// </summary>
        public const string VScan_CompressionLevel = "CompressionLevel";

        /// <summary>
        /// Last adjusted scanner name
        /// </summary>
        public const string VScan_ScannerName = "ScannerName";

        public const string VScan_NumericInterval = "NumericInterval";
        /// <summary>
        /// Describes current scanning operation
        /// </summary>
        public const string VScan_VoucherScanType = "VoucherScanType";

        /// <summary>
        /// All Available DbCountries
        /// </summary>
        public const string VScan_SelectDbCountries = "SelectDbCountries";

        #endregion //SETTINGS

        #region COMMON

        public const string NextDelegate = "AppExceptionContinueDelegate";
        public const string NextParam1 = "AppExceptionContinueDelegateP1";
        public const string NextParam2 = "AppExceptionContinueDelegateP2";

        #endregion //COMMON
    }

    public static class Consts
    {
        /// <summary>
        /// 50L
        /// </summary>
        public static long DEFAULTCOMPRESSIONLEVEL = 50L;
    }
}
