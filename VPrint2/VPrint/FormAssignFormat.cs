using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VPrinting.PartyManagement;
using mng = VPrinting.PartyManagement;
using prn = VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting
{
    public partial class FormAssignFormat : Form
    {
        private readonly SynchronizationContext m_Context;

        public FormAssignFormat()
        {
            m_Context = SynchronizationContext.Current;

            InitializeComponent();

            cbCountryID.SelectedIndexChanged += new EventHandler(cbCountryID_SelectedIndexChanged);
            cbHeadOffice.SelectedIndexChanged += new EventHandler(cbHeadOffice_SelectedIndexChanged);
            cbRetailer.SelectedIndexChanged += new EventHandler(cbRetailer_SelectedIndexChanged);
            cbFormat.SelectedIndexChanged += new EventHandler(cbFormat_SelectedIndexChanged);
        }

        protected override void OnLoad(EventArgs e)
        {
            var service = new mng.PartyManagement();
            service.GetPtfCountryListCompleted += new GetPtfCountryListCompletedEventHandler(OnGetPtfCountryListCompleted);
            service.GetPtfCountryListAsync();

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            cbCountryID.SelectedIndexChanged -= new EventHandler(cbCountryID_SelectedIndexChanged);
            cbHeadOffice.SelectedIndexChanged -= new EventHandler(cbHeadOffice_SelectedIndexChanged);
            cbRetailer.SelectedIndexChanged -= new EventHandler(cbRetailer_SelectedIndexChanged);
            cbFormat.SelectedIndexChanged -= new EventHandler(cbFormat_SelectedIndexChanged);

            base.OnClosed(e);
        }

        private void OnGetPtfCountryListCompleted(object sender, GetPtfCountryListCompletedEventArgs e)
        {
            var service = (mng.PartyManagement)sender;
            service.GetPtfCountryListCompleted -= new GetPtfCountryListCompletedEventHandler(OnGetPtfCountryListCompleted);

            cbCountryID.ClearDontFireIndexChanged(cbCountryID_SelectedIndexChanged, (b, d) =>
            {
                foreach (var i in d.OrderBy(c => c.Nationality))
                    b.Items.Add(i);
            }, e.Result);

            int countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);
            cbCountryID.SetSelected<CountryDetail>((c) => c.Number == countryId);
        }

        private void cbCountryID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;

            var service = new mng.PartyManagement();
            service.RetrieveHeadOfficeListCompleted += new RetrieveHeadOfficeListCompletedEventHandler(OnRetrieveHeadOfficeListCompleted); 
            service.RetrieveHeadOfficeListAsync(countryId);
        }

        private void OnRetrieveHeadOfficeListCompleted(object sender, RetrieveHeadOfficeListCompletedEventArgs e)
        {
            var service = (mng.PartyManagement)sender;
            service.RetrieveHeadOfficeListCompleted -= new RetrieveHeadOfficeListCompletedEventHandler(OnRetrieveHeadOfficeListCompleted);

            int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;

            cbHeadOffice.ClearDontFireIndexChanged(cbHeadOffice_SelectedIndexChanged, (b, d) =>
            {
                foreach (var i in d.OrderBy(c => c.Name))
                    b.Items.Add(i);
            }, e.Result);

            var service2 = new prn.VoucherNumberingAllocationPrinting();
            service2.GetSavedVoucherFormatsCompleted += new prn.GetSavedVoucherFormatsCompletedEventHandler(OnGetSavedVoucherFormatsCompleted);
            service2.GetSavedVoucherFormatsAsync(countryId);
        }

        private void OnGetSavedVoucherFormatsCompleted(object sender, prn.GetSavedVoucherFormatsCompletedEventArgs e)
        {
            var service2 = (prn.VoucherNumberingAllocationPrinting)sender;
            service2.GetSavedVoucherFormatsCompleted -= new prn.GetSavedVoucherFormatsCompletedEventHandler(OnGetSavedVoucherFormatsCompleted);

            cbFormat.ClearDontFireIndexChanged(cbFormat_SelectedIndexChanged, (b, d) =>
            {
                foreach (var i in d.OrderBy(r => r.Name))
                    b.Items.Add(i);
            }, e.Result);
        }

        private void cbHeadOffice_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;
            int headofficeId = cbHeadOffice.SelectedItem.cast<HeadOffice>().Id;

            var service = new mng.PartyManagement();
            service.RetrieveRetailerListCompleted += new RetrieveRetailerListCompletedEventHandler(OnRetrieveRetailerListCompleted);
            service.RetrieveRetailerListAsync(countryId, headofficeId);
        }

        private void OnRetrieveRetailerListCompleted(object sender, RetrieveRetailerListCompletedEventArgs e)
        {
            var service = (mng.PartyManagement)sender;
            service.RetrieveRetailerListCompleted -= new RetrieveRetailerListCompletedEventHandler(OnRetrieveRetailerListCompleted);

            cbRetailer.ClearDontFireIndexChanged(cbRetailer_SelectedIndexChanged, (b, d) =>
            {
                foreach (var i in d.OrderBy(r => r.Name))
                    b.Items.Add(i);
            }, e.Result);
        }

        private void cbRetailer_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;
            int retailerId = cbRetailer.SelectedItem.cast<Retailer>().Id;

            var service = new mng.PartyManagement();
            service.GetPrinterInfoCompleted += new GetPrinterInfoCompletedEventHandler(OnGetPrinterInfoCompleted);
            service.GetPrinterInfoAsync(countryId, retailerId);
        }

        private void OnGetPrinterInfoCompleted(object sender, GetPrinterInfoCompletedEventArgs e)
        {
            int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;
            int retailerId = cbRetailer.SelectedItem.cast<Retailer>().Id;

            var service = (mng.PartyManagement)sender;
            service.GetPrinterInfoCompleted -= new GetPrinterInfoCompletedEventHandler(OnGetPrinterInfoCompleted);

            cbFormat.SetSelected<PrinterDetails>((c) => c.IsoID == countryId && c.RetailerID == retailerId);
        }

        private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ShowQuestion("Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int countryId = cbCountryID.SelectedItem.cast<CountryDetail>().Number;
                int retailerId = cbRetailer.SelectedItem.cast<Retailer>().Id;
                //TODO:
                int printerId = cbFormat.SelectedItem.cast<PrinterDetails>().IsoID;

                var service = new mng.PartyManagement();
                service.SetPrinterInfoCompleted += new SetPrinterInfoCompletedEventHandler(OnSetPrinterInfoCompleted);
                service.SetPrinterInfoAsync(countryId, retailerId, printerId);
            }
        }

        private void OnSetPrinterInfoCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var service = (mng.PartyManagement)sender;
            service.SetPrinterInfoCompleted -= new SetPrinterInfoCompletedEventHandler(OnSetPrinterInfoCompleted);
            lblMessage.ForeColor = e.Error != null ? Color.Red : Color.Black;
            lblMessage.Text = e.Error != null ? e.Error.Message : "Done";
        }
    }
}
