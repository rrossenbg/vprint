/***************************************************
//  Copyright (c) Premium Tax Free 2012-2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.Extentions;
using VPrinting.PartyManagement;
using mng = VPrinting.PartyManagement;

namespace VPrinting.Forms
{
    public partial class RetailerForm : Form, IAsyncFormManagerTarget<RetailerForm>
    {
        public AsyncFormManager<RetailerForm> Target { get; set; }

        protected StateManager.VoucherItem Data
        {
            get
            {
                Debug.Assert(Target != null);
                return (StateManager.VoucherItem)Target.Result;
            }
        }

        public RetailerForm()
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
                cbCountryID.Items.Add(country);

            int countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);

            cbCountryID.SetSelected<CountryDetail>((c) => c.Number == countryId);

            timer1.Start();

            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ResetErr();

            try
            {
                if (this.DialogResult == DialogResult.OK)
                {
                    int retailerId = 0, voucherId = 0;

                    switch (tabControl1.SelectedIndex)
                    {
                        #region COUNTRY - RETAILER - VOUCHER
                        case 0:
                            {
                                if (cbCountryID.SelectedItem == null)
                                {
                                    err.SetError(cbCountryID, "Country invalid");
                                    e.Cancel = true;
                                }
                                else if (!int.TryParse(tbRetailerId.Text, out retailerId) || retailerId < 0)
                                {
                                    err.SetError(tbRetailerId, "Retailer invalid");
                                    e.Cancel = true;
                                }
                                else if (!int.TryParse(tbVoucherId.Text, out voucherId) || voucherId < 0)
                                {
                                    err.SetError(tbVoucherId, "Voucher invalid");
                                    e.Cancel = true;
                                }
                                else
                                {
                                    int countryId = ((CountryDetail)cbCountryID.SelectedItem).Number;

                                    if (!cbIgnoreServerValidation.Checked)
                                        ServiceDataAccess.Instance.ValidateVoucherThrow(countryId, (cbSSDS.Text == "10"), retailerId, voucherId,
                                            StateManager.Default.VoucherMustExist);

                                    Data.CountryID = countryId;
                                    Data.RetailerID = retailerId;
                                    Data.VoucherID = voucherId;
                                    Data.Barcode = string.Format("{0}{1}{2}{3}", countryId, cbSSDS.Text, retailerId, voucherId);
                                    timer1.Stop();
                                }
                                break;
                            }
                        #endregion
                        #region BARCODE
                        case 1:
                            {
                                if (tbBarcode.Text.IsNullOrWhiteSpace())
                                {
                                    err.SetError(tbBarcode, "Invalid or missing barcode");
                                    e.Cancel = true;
                                }
                                else
                                {
                                    BarcodeData data = null;

                                    List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

                                    foreach (var cfg in barcodeLayouts)
                                        if (cfg.ParseBarcode(tbBarcode.Text, ref data))
                                            break;

                                    if (data == null)
                                    {
                                        err.SetError(tbBarcode, "Invalid barcode");
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        if (!cbIgnoreServerValidation.Checked)
                                            ServiceDataAccess.Instance.ValidateVoucherThrow(data.CountryID, (cbSSDS.Text == "10"), data.RetailerID, data.VoucherID,
                                                StateManager.Default.VoucherMustExist);

                                        Data.CountryID = data.CountryID;
                                        Data.RetailerID = data.RetailerID;
                                        Data.VoucherID = data.VoucherID;
                                        Data.Barcode = data.Barcode;

                                        timer1.Stop();
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region SITECODE
                        case 2:
                            {
                                if (txtSitecode.Text.IsNullOrWhiteSpace())
                                {
                                    err.SetError(tbBarcode, "Invalid or missing sitecode");
                                    e.Cancel = true;
                                }
                                else
                                {
                                    string site;
                                    int location;
                                    if (!VPrinting.Common.CommonTools.ParseSiteCode(txtSitecode.Text, out site, out location))
                                        throw new Exception("Wrong sitecode");

                                    var voucher = ServiceDataAccess.Instance.FindVoucherTRSBySiteCode(site, location);

                                    if (voucher == null || !voucher.IsValid)
                                    {
                                        err.SetError(tbBarcode, "Invalid sitecode");
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        Data.CountryID = voucher.IsoId;
                                        Data.RetailerID = voucher.RetailerId;
                                        Data.VoucherID = voucher.VoucherId;

                                        timer1.Stop();
                                    }
                                }
                                break;
                            }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ResetErr("SRV ERR: " + ex.Message);
                e.Cancel = true;
            }
            finally
            {
                base.OnClosing(e);
            }
        }

        private void ResetErr(string message = null)
        {
            err.SetError(cbCountryID, message);
            err.SetError(tbRetailerId, message);
            err.SetError(tbVoucherId, message);
            err.SetError(tbBarcode, message);
            err.SetError(txtSitecode, message);
        }

        private void Show_Click(object sender, EventArgs e)
        {
            SelectFilesForm.ShowFiles(this, Data.FileInfoList);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.SetTopmost();
        }
    }
}
