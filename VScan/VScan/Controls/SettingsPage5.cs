/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.PTFLib.Web;
using PremierTaxFree.PTFLib.DataServiceProxy;

namespace PremierTaxFree.Controls
{
    public partial class SettingsPage5 : UserControl, ISettingsControl
    {
        private SqlConnectionStringBuilder builder;

        public SettingsPage5()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            extenderProvider.Control = this;
        }

        public bool IsDirty { get; set; }

        public void Read()
        {
            string connString = SettingsTable.Get<string>(Strings.VScan_ConnectionString, ClientDataAccess.ConnectionString);
            builder = new SqlConnectionStringBuilder(connString);
            ptbDataSource.Text = builder.DataSource;
            ptbDbUserName.Text = builder.UserID;
            ptbDbPassword.Text = builder.Password;
            cbIntegrated.Checked = builder.IntegratedSecurity;
            ptbCatalog.Text = builder.InitialCatalog;

            ptbSendInterval.Text = SettingsTable.Get<int>(Strings.VScan_TranfSendInterval, 1).ToString();
            ptbRemoteSrvURL.Text = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);

            UserAuth auth = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default);

            ptbPTFUserName.Text = auth.Name;
            ptbPTFPassword.Text = auth.Password;
            ptbPTFPasswordTwice.Text = auth.Password;

            vcbAutoInstallService.Checked = SettingsTable.Get<bool>(Strings.VScan_AutoInstallService, false);
        }

        public bool Verify()
        {
            builder = new SqlConnectionStringBuilder();
            builder.DataSource = ptbDataSource.Text;
            builder.UserID = ptbDbUserName.Text;
            builder.Password = ptbDbPassword.Text;
            builder.IntegratedSecurity = cbIntegrated.Checked;
            builder.InitialCatalog = ptbCatalog.Text;

            if (new SqlConnection(builder.ConnectionString).TestSf() != null)
                throw new AppExclamationException("Wrong connection string.");
            if (string.IsNullOrEmpty(ptbSendInterval.Text))
                throw new AppExclamationException("Send interval cannot be empty.");
            int i;
            if (!int.TryParse(ptbSendInterval.Text, out i))
                throw new AppExclamationException("Send interval should be integer.");
            if (string.IsNullOrEmpty(ptbRemoteSrvURL.Text))
                throw new AppExclamationException("Remote service url cannot be empty.");
            if (string.IsNullOrEmpty(ptbPTFUserName.Text))
                throw new AppExclamationException("TRS user name cannot be empty.");
            if (string.IsNullOrEmpty(ptbPTFPassword.Text))
                throw new AppExclamationException("TRS user password cannot be empty.");
            if (string.IsNullOrEmpty(ptbPTFPasswordTwice.Text))
                throw new AppExclamationException("TRS user password (second) cannot be empty.");
            if (!string.Equals(ptbPTFPassword.Text, ptbPTFPasswordTwice.Text))
                throw new AppExclamationException("Both TRS user passwords should match.");
            return true;
        }

        public void Save()
        {
            SettingsTable.Set(Strings.VScan_ConnectionString, builder.ConnectionString);
            SettingsTable.Set(Strings.VScan_TranfSendInterval, int.Parse(ptbSendInterval.Text));
            SettingsTable.Set(Strings.All_CentralServerUrl, ptbRemoteSrvURL.Text);
            SettingsTable.Set(Strings.VScan_AutoInstallService, vcbAutoInstallService.Checked);
        }

        public void UpdateEnvironment()
        {
            var settingsObj = new SettingsObj()
                {
                    ConnectionString = builder.ConnectionString,
                    CentralServerUrl = ptbRemoteSrvURL.Text,
                    SendInterval = int.Parse(ptbSendInterval.Text),
                    KeepHistoryDays = SettingsTable.Get<int>(Strings.VScan_KeepHistoryDays, 1),
                };
            SettingsTable.Set(Strings.Transferring_SettingsObject, settingsObj);
            DBConfigValue.Save(Strings.Transferring_SettingsObject, settingsObj);

            SettingsTable.Default.Save();
        }

        private void InstallTransfferingService_Click(object sender, EventArgs e)
        {
            UIUtils.TryStartTransferringServiceAsync();
        }

        private void CreateRemoteClient_Click(object sender, System.EventArgs e)
        {
            new MethodInvoker(() =>
            {
                this.BeginInvokeSafe(new MethodInvoker(() => errorProvider.SetError(btnCreateClient, null)));
                try
                {
                    string url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
                    string machineName = PTFUtils.GetMachine();
                    var clientId = DataServiceClient.CallCreateClient(machineName);
                }
                catch (Exception ex)
                {
                    this.BeginInvokeSafe(new MethodInvoker(() => errorProvider.SetError(btnCreateClient, ex.Message)));
                    throw;
                }
            }).FireAndForget();
        }

        private void TestConnection_Click(object sender, EventArgs e)
        {
            if (sender == btnDbTestConnection)
            {
                errorProvider.SetError(btnDbTestConnection, ClientDataAccess.TestConnection());
            }
            else if (sender == btnRcvTestConnection)
            {
                new MethodInvoker(() =>
                {
                    string url = SettingsTable.Get<string>(Strings.All_CentralServerUrl);
                    Debug.Assert(url != null);
                    UserAuth auth = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default);
                    this.InvokeSafe(new MethodInvoker(() => errorProvider.SetError(btnRcvTestConnection, DataServiceClient.CallTestConnection())));
                }).FireAndForgetSafe();
            }
            else if (sender == btnTrfTestConnection)
            {
                errorProvider.SetError(btnTrfTestConnection, OS.TestService(Strings.TransferringService, TimeSpan.FromSeconds(10)));
            }
        }
    }
}
