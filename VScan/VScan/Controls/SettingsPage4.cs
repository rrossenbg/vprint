/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.Extensions;
using PremierTaxFree.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Culture;

using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Web;
using PremierTaxFree.PTFLib.Data.Objects.Server;
using PremierTaxFree.PTFLib.DataServiceProxy;

namespace PremierTaxFree.Controls
{
    public partial class SettingsPage4 : UserControl, ISettingsControl
    {
        public bool IsDirty { get; set; }

        public SettingsPage4()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            extender.Control = this;
        }

        public void Read()
        {
            BindSettingsComboBox();
            PopulateCountiesComboBox();
            PopulateCultureConmboBox();
        }

        public bool Verify()
        {
            return true;
        }

        public void Save()
        {
            
        }

        public void UpdateEnvironment()
        {
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (sender == vbtnAddSettings)
            {
                string text = ptbAddSettingsName.Text.TrimSafe();
                if (string.IsNullOrEmpty(text))
                {
                    this.ShowWarning("Settings name could not be empty space.");
                }
                else if (SettingsTable.Default.ActiveTable.ContainsKey(text))
                {
                    this.ShowWarning("The key already exists");
                }
                else
                {
                    SettingsTable.Default.CopyTable(text, SettingsTable.SettingsKeys);
                    var settings = SettingsTable.Default.GetSettingNames();
                    settings.Add(text);
                    BindSettingsComboBox();
                }
            }
            else if (sender == vbtnRemoveSettings)
            {
                if (cbAvailableSettings.SelectedItem == null)
                {
                    this.ShowWarning("No settings is selected.");
                }
                else
                {
                    string text = Convert.ToString(cbAvailableSettings.SelectedItem);
                    SettingsTable.Default.DataTable.Remove(text);
                    BindSettingsComboBox();
                }
            }
            else if (sender == vbtnSetDefault)
            {
                if (cbAvailableSettings.SelectedItem == null)
                {
                    this.ShowWarning("No settings is selected.");
                }
                else
                {
                    string text = Convert.ToString(cbAvailableSettings.SelectedItem);
                    SettingsTable.Default.LoadTable(text);
                    SettingsTable.Set(Strings.VScan_DefaultSettingsName, text);
                    this.FindForm().Close();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void BindSettingsComboBox()
        {
            var settings = SettingsTable.Default.GetSettingNames();
            cbAvailableSettings.Items.Clear();
            cbAvailableSettings.Items.AddRange(settings.ToArray());
            cbAvailableSettings.SelectedItem = SettingsTable.Get<string>(Strings.VScan_DefaultSettingsName);
            int index = cbAvailableSettings.SelectedIndex;
            cbAvailableSettings.SetItemColor(index, Color.Green);
        }

        private void PopulateCountiesComboBox()
        {
            var countries = SettingsTable.Get<DbCountry[]>(Strings.VScan_SelectDbCountries, null);
            cbCountries.DataSource = countries;
            cbCountries.SelectedIndexChanged += new EventHandler(Countries_SelectedIndexChanged);
            if (cbCountries.Items.Count > 0)
                cbCountries.SelectedIndex = 0;
        }

        private void PopulateCultureConmboBox()
        {
            var list = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).ToList();
            list.Sort(new Comparison<CultureInfo>((c1, c2)=>c1.Name.CompareTo(c2.Name)));
            cbCultures.DataSource = list;
            cbCultures.SelectedItem = Thread.CurrentThread.CurrentUICulture;
            cbCultures.SelectedIndexChanged += new EventHandler(Cultures_SelectedIndexChanged);
        }

        private void Countries_SelectedIndexChanged(object sender, EventArgs e)
        {
            DbCountry country = (DbCountry)cbCountries.SelectedItem;

            new Func<DbCountry, string[]>((s) => ScanAppContext.QueryCountryCodes(s.CountryId)).RunAsync(
                  new Action<string[]>((sc) =>
                      this.InvokeSf(new MethodInvoker(() => { cbSiteCodes.DataSource = sc; }))), country);
        }

        private void ChangePassword_Click(object sender, EventArgs e)
        {
            bool isAdmin = sender == vbtnChangeAdmin;
            //No authentication is needed here
            if (PasswordForm.ChangePassword(this.FindForm(), isAdmin))
            {
                if (!isAdmin)
                {
                    new MethodInvoker(() =>
                    {
                        var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
                        var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
                        //Validate on TRS server
                        DataServiceClient.CallValidateUser(authUser.CountryID, authUser.Name, authUser.Password);

                        //Send to win service
                        var settingsObj = SettingsTable.Get<SettingsObj>(Strings.Transferring_SettingsObject, SettingsObj.Default);
                        DBConfigValue.Save(Strings.Transferring_AuthObject, authUser);
                        DBConfigValue.Save(Strings.Transferring_SettingsObject, settingsObj);
                    }).FireAndForget();
                }
            }
        }

        private void ShowDatabase_Click(object sender, EventArgs e)
        {
            DbObserverForm.Start();
        }

        private void Cultures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ShowQuestion("You are about to change current culture.\r\n Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CultureInfo ci = (CultureInfo)cbCultures.SelectedItem;
                ChangeFormCulture.ChangeAllForms(ci);
                SettingsTable.Set(Strings.Transferring_CurrentUICultureInfo, ci);
            }
        }

        private void StartConsole_Click(object sender, EventArgs e)
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, "RemoteTracer.exe");
                ProcessStartInfo info = new ProcessStartInfo(path);
                Process shellProcess = Process.Start(info);
                shellProcess.Close();
            }
            catch (Exception ex)
            {
                ex.ThrowAndForget();
            }
        }
    }
}
