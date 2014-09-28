
namespace ReceivingServiceLib.Services
{
    public class RefundServiceDataAccess
    {
        public class PR_Sitecode
        {
            public int? IsoId { get; set; }
            public int? RetailerId { get; set; }
            public int? VoucherId { get; set; }
            public string SiteCode { get; set; }
        }

        public PR_Sitecode CallRefundService(string siteLocationCode)
        {
            var client = new PremierRefundVoucherServiceClient();
            try
            {
                var data = client.GetVoucherWithLocationCode(siteLocationCode);
                var result = new PR_Sitecode();
                if (data.Match)
                {
                    result.IsoId = data.CountryId;
                    result.RetailerId = data.RetailerId;
                    result.VoucherId = data.VoucherNumber;
                    result.SiteCode = siteLocationCode;
                }
                return result;
            }
            finally
            {
                client.Close();
            }
        }
    }
}
