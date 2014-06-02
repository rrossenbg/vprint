/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Collections;
using PremierTaxFree.PTFLib.Data.Objects.Server;
using PremierTaxFree.PTFLib.DataServiceProxy;
using PremierTaxFree.PTFLib.Net;

namespace PremierTaxFree
{
    public partial class PasswordForm : Form
    {
        private class tmpCountry
        {
            public string Name { get; private set; }
            public int Id { get; private set; }

            public tmpCountry(string name, int id)
            {
                Name = name;
                Id = id;
            }
            public override string ToString()
            {
                return Name;
            }
        }

        public bool IsInAdminContext { get; set; }

        private bool ChangePasswordAction
        {
            get
            {
                return !lblChange.Visible;
            }
            set
            {
                Text = value ? "Change password" : "Authenticate user";
                lblChange.Visible = !value;
            }
        }

        private string UserName { get { return tbUserName.Text; } }
        private string Password { get { return tbPassword.Text; } }
        private int CountryID { get; set; }

        public bool ShowCountries
        {
            get { return cbCountries.Visible; }
            set { cbCountries.Visible = value; }
        }

        private PasswordForm()
        {
            InitializeComponent();
            ShowCountries = false;
            CountryID = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default).CountryID;
            tbUserName.TextChanged += new EventHandler(TextBox_TextChanged);
            tbPassword.TextChanged += new EventHandler(TextBox_TextChanged);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (cbCountries.Visible)
            {
                var countries = SettingsTable.Get<DbCountry[]>(Strings.VScan_SelectDbCountries,
                   new DbCountry[] { new DbCountry() { CountryId = 826, ShortName = "UK" } }).ToList().
                   ConvertAll<tmpCountry>((c) => { return new tmpCountry(c.ShortName, c.CountryId); });
                cbCountries.DataSource = countries;
                cbCountries.SelectedItem = countries.FirstOrDefault((c) => c.Id == CountryID);
                cbCountries.SelectedIndexChanged += new EventHandler(Countries_SelectedIndexChanged);
            }
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                e.Cancel = true;

                if (string.IsNullOrEmpty(tbUserName.Text))
                {
                    errorProvider.SetError(tbUserName, "Field may not be empty");
                }
                else if (string.IsNullOrEmpty(tbPassword.Text))
                {
                    errorProvider.SetError(tbPassword, "Field may not be empty");
                }
                else
                {
                    e.Cancel = false;
                }
            }
            base.OnFormClosing(e);
        }

        private bool AuthenticateInternal()
        {
            var authUser = new UserAuth(CountryID, UserName, Password, IsInAdminContext);
            DataServiceClient.CallValidateUser(authUser.CountryID, authUser.Name, authUser.Password);
            return true;
        }

        private bool ChangePasswordInternal(Form owner)
        {
            var authUser = new UserAuth(CountryID, UserName, Password, IsInAdminContext);
            //Validate on TRS server
            DataServiceClient.CallValidateUser(authUser.CountryID, authUser.Name, authUser.Password);
            SettingsTable.Set(Strings.Transferring_AuthObject, authUser);
            SettingsTable.Get<UniqueList<UserAuth>>(Strings.Transferring_AuthObjectList).Add(authUser);
            return true;
        }

        public static bool Authenticate(Form owner, bool asAdmin, bool change)
        {
            try
            {
                using (PasswordForm form = new PasswordForm())
                {
                    form.IsInAdminContext = asAdmin;

                    if (form.ShowDialog(owner) == DialogResult.OK)
                    {
                        form.AuthenticateInternal();
                        if (change)
                            ChangePassword(form, asAdmin);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.ShowDialog(owner);
                return false;
            }
        }

        public static bool ChangePassword(Form owner, bool asAdmin)
        {
            try
            {
                using (PasswordForm form = new PasswordForm())
                {
                    form.IsInAdminContext = asAdmin;
                    form.ChangePasswordAction = true;
                    form.ShowCountries = true;

                    if (form.ShowDialog(owner) == DialogResult.OK)
                    {
                        form.ChangePasswordInternal(owner);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.ShowDialog(owner);
                return false;
            }
        }

        private void Change_Click(object sender, EventArgs e)
        {
            if (AuthenticateInternal() && ChangePassword(this, IsInAdminContext))
                this.ShowInformation("Your password has been successfully changed.");
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            errorProvider.SetError((Control)sender, null);
        }

        private void Countries_SelectedIndexChanged(object sender, EventArgs e)
        {
            tmpCountry country = (tmpCountry)cbCountries.SelectedItem;
            CountryID = country.Id;
        }
    }
}
