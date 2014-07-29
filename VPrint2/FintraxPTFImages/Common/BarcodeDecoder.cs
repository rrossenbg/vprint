//#define SPAIN

using System;
using System.Collections.Concurrent;
using System.Web;
using VPrinting.Common;

namespace FintraxPTFImages.Common
{
    public class BarcodeDecoder
    {
        public BarcodeData Match(string barcode)
        {
            var bag = (ConcurrentBag<BarcodeConfig>)HttpContext.Current.Application[Strings.LIST_OF_BARCODECONFIGS];

            BarcodeData data = null;

            foreach (var item in bag)
                if (item.ParseBarcode(barcode, ref data))
                    return data;
            return data;
        }

        public static void Run()
        {
            HttpContext.Current.Application.Add(Strings.LIST_OF_BARCODECONFIGS,
                new ConcurrentBag<BarcodeConfig>()
            {
                new BarcodeConfig()
                {
                    Name = "Default",
                    Length = 19,
                    HasCheckDigit = true,
                    //iso, ty, br, voucher
                    Template = "{0:000}{1:00}{2:000000}{3:00000000}",
                    Sample = "826 20 188025 33359669 9",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 8),
                },
#if SPAIN
                new BarcodeConfig()
                {
                    Name = "Spanish",
                    Length = 18,
                    HasCheckDigit = false,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:000000000}",
                    Sample = "724 214310 148767199",
                    CountryID = new Tuple<int,int>(0, 3),
                    RetailerID = new Tuple<int,int>(3, 6),
                    VoucherID = new Tuple<int,int>(9, 9),
                },
#endif
#if IRELAND
                new BarcodeConfig()
                {
                    Name = "Gucci",
                    Length = 18,
                    HasCheckDigit = false,
                    //iso, ty, br, voucher
                    Template = "{3:00000000}{0:000}{2:000000}",
                    Sample = "001977684 056 100353",
                    VoucherID = new Tuple<int,int>(0, 9),
                    CountryID = new Tuple<int,int>(9, 3),
                    RetailerID = new Tuple<int,int>(12,6),
                    BuzType = new Tuple<int,int>(0, 1),
                },
#endif
            });
        }
    }
}