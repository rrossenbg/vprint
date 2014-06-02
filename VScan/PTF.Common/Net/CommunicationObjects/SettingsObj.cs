/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using PremierTaxFree.PTFLib.Data;
using System.Collections.Generic;

namespace PremierTaxFree.PTFLib.Net
{
    [Serializable]
    public class SettingsObj
    {
        public static SettingsObj Default = new SettingsObj()
                    {
                        ConnectionString = ClientDataAccess.ConnectionString,
                        SendInterval = 1,
                        CentralServerUrl = Strings.All_CentralServerUrlPathDefault,
                        KeepHistoryDays = 7,
                    };

        public string ConnectionString { get; set; }

        public string CentralServerUrl { get; set; }

        /// <summary>
        /// Send interval in minutes
        /// </summary>
        public int SendInterval { get; set; }
        public int MaximumFilesForExport { get; set; }
        public int MaximumMessagesForExport { get; set; }
        public int KeepHistoryDays { get; set; }

        /// <summary>
        /// CountryID, SiteCode => Table
        /// </summary>
        public Dictionary<int, string> SiteCodeTable { get; set; }

        public SettingsObj()
        {
            SiteCodeTable = new Dictionary<int, string>();
        }
    }
}
