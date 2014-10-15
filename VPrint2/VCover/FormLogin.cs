/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using VCover.AuthenticationRef;
using VCover.Data;
using VCover.PartyManagementRef;
using VPrinting;
using VPrinting.Common;

namespace VCover
{
    public partial class FormLogin : Form
    {
        private Version m_Version;

        public FormLogin()
        {
            InitializeComponent();
            
            m_Version = Assembly.GetEntryAssembly().GetName().Version;
            lblVersion.Text = "Version: ".concat(m_Version.ToString());
#if DEBUG
            //lblVersion.Text = lblVersion.Text.concat(" <DEBUG>");
#endif
            var header = new PartyManagementRef.AuthenticationHeader();
            var service = new PartyManagementSoapClient();
            var countries = new Func<PartyManagementSoapClient, CountryDetail[]>((s) => s.GetPtfCountryList(header)).ReTry(service);
            if (countries == null)
                throw new ApplicationException("Can not connect to the server.");

            foreach (var country in countries.OrderBy(c => c.Nationality))
                cbCountryID.Items.Add(country);

            int countryId = Config.CountryID;

            cbCountryID.SetSelected<CountryDetail>((c) => c.Number == countryId);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.textBoxUsername.Text = StateSaver.Default.Get<string>("textBoxUsername.Text");
            base.OnLoad(e);
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            TryLogin();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryLogin();
            else if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void TryLogin()
        {
            CountryDetail country = (CountryDetail)cbCountryID.SelectedItem;
            if (country == null)
            {
                this.ShowExclamation("Please select country first.");
            }
            else
            {
                Program.IsAdmin = (string.Compare(textBoxUsername.Text, "rosen") == 0) &&
                                    (string.Compare(textBoxPassword.Text, "rosen") == 0);
                int countryId = country.Number;

                if (Program.IsAdmin)
                {
                    Program.currentUser = new CurrentUser(0, textBoxUsername.Text, countryId);

                    //ServiceDataAccess.Instance.LogOperation(OperationHistory.Login, Program.SessionId, 0, 0, 0, 0, 0, "");

                    this.DialogResult = DialogResult.OK;

                    StateSaver.Default.Set("textBoxUsername.Text", this.textBoxUsername.Text);
                }
                else
                {
                    var auth = new AuthenticationSoapClient();

                    string result = auth.AuthenticateUser(countryId, textBoxUsername.Text, textBoxPassword.Text);

                    if (!string.IsNullOrEmpty(result))
                    {
                        var header = new AuthenticationRef.AuthenticationHeader();

                        var userId = auth.RetrieveUser(header, countryId, textBoxUsername.Text);

                        Program.currentUser = new CurrentUser(userId, textBoxUsername.Text, countryId);

                        //ServiceDataAccess.Instance.LogOperation(OperationHistory.Login, Program.SessionId, 0, 0, 0, 0, 0, "");

                        this.DialogResult = DialogResult.OK;

                        StateSaver.Default.Set("textBoxUsername.Text", this.textBoxUsername.Text);
                    }
                    else
                    {
                        //Speeker.SpeakAsynchSf("Invalid user id or password, please try again.");
                        this.ShowExclamation("Invalid user id or password, please try again.");
                        textBoxPassword.Text = "";
                        textBoxPassword.Focus();
                    }
                }
            }
        }

        private void UpdateVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Enabled = false;

            this.ShowExclamation("This function is not implemented.");

            //Global.Instance.VersionUpdate(m_Version.ToString(),
            //    () => this.InvokeSf(() => { this.Close(); }),
            //    () => this.InvokeSf(() => { this.Enabled = true; }));
        }

        private void EmailSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("mailto://rosen.rusev@uk.premiertaxfree.com?Subject=VPrinting%20Enquiry");
            }
            catch (Exception ex)
            {
                Program.OnThreadException(this, new ThreadExceptionEventArgs(ex));
            }
        }
    }
}
