/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using PremierTaxFree.Data;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects;
using PremierTaxFree.PTFLib.Data.Objects.Server;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.DataServiceProxy;

namespace PremierTaxFree
{
    public class ScanAppContext : ApplicationContext
    {
        private static ScanAppContext ms_Instance;
        public static ScanAppContext Default
        {
            get
            {
                if (ms_Instance == null)
                    ms_Instance = new ScanAppContext();
                return ms_Instance;
            }
            set
            {
                ms_Instance = value;
            }
        }
        
        /// <summary>
        /// Current Scan Information
        /// </summary>
        public CurrentScanObj CurrentScan { get; set; }

        public ScanAppContext()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            CurrentScan = new CurrentScanObj(eVouchersScanType.BeforeInsertion, null, -1);
        }

        public ScanAppContext(Form main)
            : base(main)
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            CurrentScan = new CurrentScanObj(eVouchersScanType.BeforeInsertion, null, -1);
        }

        ~ScanAppContext()
        {
            Application.ApplicationExit -= new EventHandler(OnApplicationExit);
        }

        public IEnumerable<T> OpenFormsOf<T>() where T : Form
        {
            FormCollection allforms = Application.OpenForms;
            foreach (var frm in allforms)
                if (frm != null && frm is T)
                    yield return (T)frm;
        }

        public static void QueryCountryesAsync()
        {
            new MethodInvoker(() =>
            {
                UserAuth auth = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default);
                var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
                var countryes = DataServiceClient.CallQueryContries(auth).
                    ConvertAll<CountryData, DbCountry>(cd => new DbCountry() { CountryId = cd.CountryId, Name = cd.Name, ShortName = cd.ShortName }).ToArray();
                SettingsTable.Set(Strings.VScan_SelectDbCountries, countryes);
            }).FireAndForget();
        }

        /// <summary>
        /// Queries all site codes for user's country or for selected by Id country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public static string[] QueryCountryCodes(int? countryId)
        {
            UserAuth auth = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default);
            var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
            var codes = DataServiceClient.CallQueryContries(auth).ConvertAll<CountryData, string>(c => c.ShortName).ToArray();
            return codes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="QueueIsEmptyException"></exception>
        public static Voucher FillFromScanContext(Voucher data)
        {
            lock (data)
            {
                var currentScan = ScanAppContext.Default.CurrentScan;
                data.SiteCode = AuditIDSTable.SelectRemoveFirstOrEmpty().ThrowIfDefault<string, NoDataIdException>();
                data.Comment = currentScan.Comment;
                data.CompressionLevel = SettingsTable.Get<long>(Strings.VScan_CompressionLevel, Consts.DEFAULTCOMPRESSIONLEVEL);
                return data;
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }
    }
}
