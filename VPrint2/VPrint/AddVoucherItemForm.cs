/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Extentions;
using VPrinting.PartyManagement;
using mng = VPrinting.PartyManagement;
using System.Collections.Generic;

namespace VPrinting
{
    public partial class AddVoucherItemForm : Form
    {
        private int CountryId { get; set; }
        private int RetailerId { get; set; }
        private int VoucherId { get; set; }
        private string Sitecode { get; set; }

        public static bool show(IWin32Window owner, ref int countryId, ref int retailerId, ref int voucherId, ref string siteCode)
        {
            using (var form = new AddVoucherItemForm())
            {
                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    countryId = form.CountryId;
                    retailerId = form.RetailerId;
                    voucherId = form.VoucherId;
                    siteCode = form.Sitecode;
                    return true;
                }
                return false;
            }
        }

        public AddVoucherItemForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            var service = new mng.PartyManagement();
            var countries = new Func<mng.PartyManagement, CountryDetail[]>((s) => s.GetPtfCountryList()).ReTry(service);
            if (countries == null)
                throw new ApplicationException("Can not connect to the server.");

            foreach (var country in countries.OrderBy(c => c.Nationality))
                cbCountryId.Items.Add(country);

            txtBarcode.Text = StateSaver.Default.Get<string>("AddVoucherItemForm.txtBarcode", null);
            txtSiteCode2.Text = StateSaver.Default.Get<string>("AddVoucherItemForm.txtSiteCode2", null);

            int countryId = StateSaver.Default.Get<int>("AddVoucherItemForm.cbCountryId", MainForm.CurrentCountryId);
            cbCountryId.SetSelected<CountryDetail>((c) => c.Number == countryId);

            txtRetailerId.Text = StateSaver.Default.Get<string>("AddVoucherItemForm.txtRetailerId", null);
            txtVoucherId.Text = StateSaver.Default.Get<string>("AddVoucherItemForm.txtVoucherId", null);
            txtSiteCode.Text = StateSaver.Default.Get<string>("AddVoucherItemForm.txtSiteCode", null);
            
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                ResetError();

                #region VALIDATE 

                switch (tabControl1.SelectedIndex)
                {
                    default:
                    case 0:
                        {
                            int value;

                            if (cbCountryId.SelectedItem != null)
                            {
                                CountryId = ((CountryDetail)cbCountryId.SelectedItem).Number;
                            }
                            else
                            {
                                err.SetError(cbCountryId, "Country is invalid");
                                e.Cancel = true;
                            }

                            if (int.TryParse(txtRetailerId.Text, out value) && value > 0 && value < Consts.MAX_RETAILERID_CD)
                            {
                                RetailerId = value;
                            }
                            else
                            {
                                err.SetError(txtRetailerId, "RetailerId is invalid");
                                e.Cancel = true;
                            }

                            if (int.TryParse(txtVoucherId.Text, out value) && value > 0 && value < Consts.MAX_VOUCHERID_CD)
                            {
                                VoucherId = value;
                            }
                            else
                            {
                                err.SetError(txtVoucherId, "VoucherId is invalid");
                                e.Cancel = true;
                            }

                            if (!txtSiteCode.Text.IsNullOrEmpty())
                            {
                                Sitecode = txtSiteCode.Text;
                            }
                            else
                            {
                                err.SetError(txtSiteCode, "SiteCode is invalid");
                                e.Cancel = true;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (!txtBarcode.Text.IsNullOrEmpty())
                            {
                                BarcodeData data = null;

                                List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

                                lock (barcodeLayouts)
                                {
                                    foreach (var cfg in barcodeLayouts)
                                        if (cfg.ParseBarcode(txtBarcode.Text, ref data))
                                            break;
                                }

                                if (data == null)
                                {
                                    err.SetError(txtBarcode, "Barcode is invalid");
                                    e.Cancel = true;
                                }
                                else
                                {
                                    CountryId = data.CountryID;
                                    RetailerId = data.RetailerID;
                                    VoucherId = data.VoucherID;
                                }
                            }
                            else
                            {
                                err.SetError(txtBarcode, "Barcode is invalid");
                                e.Cancel = true;
                            }

                            if (!txtSiteCode2.Text.IsNullOrEmpty())
                            {
                                Sitecode = txtSiteCode2.Text;
                            }
                            else
                            {
                                err.SetError(txtSiteCode2, "SiteCode is invalid");
                                e.Cancel = true;
                            }
                            break;
                        }
                }

                #endregion //VALIDATE
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            StateSaver.Default.Set("AddVoucherItemForm.txtBarcode", txtBarcode.Text);
            StateSaver.Default.Set("AddVoucherItemForm.txtSiteCode2", txtSiteCode2.Text);

            if (cbCountryId.SelectedItem != null)
                StateSaver.Default.Set("AddVoucherItemForm.cbCountryId", ((CountryDetail)cbCountryId.SelectedItem).Number);
            StateSaver.Default.Set("AddVoucherItemForm.txtRetailerId", txtRetailerId.Text);
            StateSaver.Default.Set("AddVoucherItemForm.txtVoucherId", txtVoucherId.Text);
            StateSaver.Default.Set("AddVoucherItemForm.txtSiteCode", txtSiteCode.Text);

            base.OnClosed(e);
        }

        private void ResetError()
        {
            err.SetError(cbCountryId, null);
            err.SetError(txtRetailerId, null);
            err.SetError(txtVoucherId, null);
            err.SetError(txtSiteCode, null);

            err.SetError(txtBarcode, null);
            err.SetError(txtSiteCode2, null);
        }
    }
}
