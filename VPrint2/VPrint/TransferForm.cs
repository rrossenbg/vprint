/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using VPrinting.Extentions;
using VPrinting.PartyManagement;
using mng = VPrinting.PartyManagement;

namespace VPrinting
{
    public partial class TransferForm : Form
    {
        public int CountryId { get; set; }
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public string Sitecode { get; set; }

        public static bool show(IWin32Window parent, ref int countryId, ref int from, ref int to, ref string sitecode)
        {
            using (var form = new TransferForm())
            {
                if (form.ShowDialog(parent) == DialogResult.OK)
                {
                    countryId = form.CountryId;
                    from = form.FromNumber;
                    to = form.ToNumber;
                    sitecode = form.Sitecode;
                    return true;
                }
                return false;
            }
        }

        public TransferForm()
        {
            InitializeComponent();
        }

        private void ResetErr()
        {
            err.SetError(cbCountry, null);
            err.SetError(tbFrom, null);
            err.SetError(tbTo, null);
            err.SetError(tbSiteCode, null);
        }

        protected override void OnLoad(EventArgs e)
        {
            var service = new mng.PartyManagement();
            var countries = new Func<mng.PartyManagement, CountryDetail[]>((s) => s.GetPtfCountryList()).ReTry(service);
            if (countries == null)
                throw new ApplicationException("Can not connect to the server.");

            foreach (var country in countries.OrderBy(c => c.Nationality))
            {
                cbCountry.Items.Add(country);
                cbCountry2.Items.Add(country);
            }

            int countryId = MainForm.CurrentCountryId;

            cbCountry.SetSelected<CountryDetail>((c) => c.Number == countryId);
            cbCountry2.SetSelected<CountryDetail>((c) => c.Number == countryId);
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ResetErr();

            if (DialogResult == DialogResult.OK)
            {
                e.Cancel = true;

                if (tabControl1.SelectedTab == tpRange)
                {

                    int from, to;
                    if (cbCountry.SelectedItem == null)
                    {
                        err.SetError(cbCountry, "Country not selected");
                    }
                    else if (!int.TryParse(tbFrom.Text, out from))
                    {
                        err.SetError(tbFrom, "From value invalid");
                    }
                    else if (!int.TryParse(tbTo.Text, out to))
                    {
                        err.SetError(tbTo, "To value invalid");
                    }
                    else if (tbSiteCode.Text.IsNullOrEmpty())
                    {
                        err.SetError(tbSiteCode, "Site code not found");
                    }
                    else if (from > to)
                    {
                        err.SetError(tbFrom, "From should be greater than to");
                        err.SetError(tbTo, "From should be greater than to");
                    }
                    else
                    {
                        FromNumber = from;
                        ToNumber = to;
                        CountryId = ((CountryDetail)cbCountry.SelectedItem).Number;
                        Sitecode = tbSiteCode.Text.Trim();
                        e.Cancel = false;
                    }
                }
                else
                {
                    int from;
                    if (cbCountry2.SelectedItem == null)
                    {
                        err.SetError(cbCountry2, "Country not selected");
                    }
                    else if (!int.TryParse(tbFrom2.Text, out from))
                    {
                        err.SetError(tbFrom2, "From value invalid");
                    }
                    else if (tbSiteCode2.Text.IsNullOrEmpty())
                    {
                        err.SetError(tbSiteCode2, "Site code not found");
                    }
                    else
                    {
                        FromNumber = from;
                        ToNumber = from;
                        CountryId = ((CountryDetail)cbCountry2.SelectedItem).Number;
                        Sitecode = tbSiteCode2.Text.Trim();
                        e.Cancel = false;
                    }
                }
            }

            base.OnClosing(e);
        }
    }
}
