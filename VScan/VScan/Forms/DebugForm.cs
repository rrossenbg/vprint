/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.DataServiceProxy;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.Sys;

namespace PremierTaxFree.Forms
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// reload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadServer_Click(object sender, EventArgs e)
        {
            var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
            var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
            DataServiceClient.CallSendCmd("reload");
        }

        /// <summary>
        /// client_list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectClients_Click(object sender, EventArgs e)
        {
            var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
            var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
            var list = new ArrayList((ICollection)DataServiceClient.CallSendCmd("client_list"));
        }

        /// <summary>
        /// enable_client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableClient_Click(object sender, EventArgs e)
        {
            var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
            var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
            DataServiceClient.CallSendCmd("enable_client", new object[] { 1, false });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.sConfirm_VouchersAreNotInserted);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(SettingsTable.Default.ActiveTable.Count);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                Windows windows = new Windows(false, false);
                foreach (Window win in windows)
                {
                    if (win.Title.StartsWith("DTK Barcode Reader"))
                    {
                        var children = win.GetChildWindows();
                        Debug.Assert(children != null);
                        var close = children.Find((w) => w.Title == "Close");
                        Debug.Assert(close != null);
                        close.Click();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
