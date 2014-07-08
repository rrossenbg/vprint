using System.Collections.Generic;
using HeaderNP = HobexCommonLib.VoucherNP.AuthenticationHeader;
using Server = HobexCommonLib.VoucherNP.VoucherNumberingAllocationPrintingSoapClient;
using Server2 = HobexCommonLib.VoucherEM.VoucherEntryAndModificationSoapClient;

namespace HobexCommonLib
{
    public class ServiceAccessClass
    {
        public static bool CheckTerminalValid(HeaderNP header, int countryId, int retailerId, string postId)
        {
            using (var service = new Server())
            {
                var valid = service.CheckTerminalValid(header, countryId: countryId, retailerId: retailerId, posId: postId);
                return valid;
            }
        }

        public static string CalculateCheckDigit(HeaderNP header, int Id)
        {
            using (var service = new Server())
            {
                var retailerId = service.CalculateCheckDigit(header, Id);
                return retailerId;
            }
        }

        public static string[] AllocateVouchers(HeaderNP header, int countryId, int retailerId, string postId, int volume,
            string originator, out int from, out int to)
        {
            using (var service = new Server())
            {
                var allocationId = service.AllocateRangePOS(
                    header,
                    countryId: countryId,
                    retailerId: retailerId,
                    posId: postId,
                    orderVolume: volume,
                    originator: originator);

                var allocation = service.RetrieveAllocation(header, allocationId);
                from = allocation.RangeFrom;
                to = allocation.RangeTo;

                List<string> vouchers = new List<string>();

                for (int voucher = allocation.RangeFrom / 10; voucher <= allocation.RangeTo / 10; voucher++)
                    vouchers.Add(voucher.ToString());

                return vouchers.ToArray();
            }
        }

        public static void RecordImportSafe(string fileName, int countryId, int retailerId, string terminalId, int count, string message, string xml)
        {
            try
            {
                using (var service = new Server2())
                    service.RecordPOSAllocation2(fileName, countryId, retailerId, terminalId, count, message, xml);
            }
            catch
            {
            }
        }
    }
}
