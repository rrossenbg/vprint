using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEMATLib.Data
{
    [Serializable]
    public class HeadOffice
    {
        public int IsoId { get; set; }
        public int HoId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 826,1;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HeadOffice Parse(string text)
        {
            string[] txts = text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var ho = new HeadOffice();
            ho.IsoId = int.Parse(txts[0]);
            ho.HoId = int.Parse(txts[1]);
            return ho;
        }

        /// <summary>
        /// 826,1; 826,2; 826,3; 826,4;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<HeadOffice> ParseList(string text)
        {
            var list = new List<HeadOffice>();
            string[] txts = text.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in txts)
            {
                var ho = HeadOffice.Parse(s);
                list.Add(ho);
            }
            return list;
        }
    }

    [Serializable]
    public class Retailer
    {
        public int IsoId { get; set; }
        public int BrId { get; set; }
        public int HoId { get; set; }

        public override string ToString()
        {
            return string.Concat("Retailer: IsoId=", IsoId, " HoId=", HoId, " BrId=", BrId);
        }
    }

    [Serializable]
    public class Voucher
    {
        public int VId { get; set; }
        public int IsoId { get; set; }
        public int BrId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? v_date_purchase { get; set; }
        public DateTime? VoidedDate { get; set; }
        public DateTime? ClaimedDate { get; set; }
        public DateTime? v_date_stamp { get; set; }
        public DateTime? v_date_debited { get; set; }
        public DateTime? v_date_refund { get; set; }
        public string v_ic_id { get; set; }
        public string SiteCodeRose { get; set; }
        public DateTime? v_date_p0 { get; set; }
        public DateTime? v_date_p1 { get; set; }
        public DateTime? v_date_p2 { get; set; }
        public bool v_voucher_void { get; set; }

        public override string ToString()
        {
            return string.Concat("Voucher: IsoId=", IsoId, " BrId=", BrId, " VId=", VId);
        }

        public bool Equals(Voucher other)
        {
            if (other == null)
                return false;

            return
                VId == other.VId &&
                BrId == other.BrId &&
                IsoId == other.IsoId &&
                //TimeStamp == other.TimeStamp &&
                v_date_purchase == other.v_date_purchase &&
                VoidedDate == other.VoidedDate &&
                ClaimedDate == other.ClaimedDate &&

                v_voucher_void == other.v_voucher_void &&
                v_date_stamp == other.v_date_stamp &&
                v_date_refund == other.v_date_refund &&
                v_date_debited == other.v_date_debited &&
                v_ic_id == other.v_ic_id &&
                SiteCodeRose == other.SiteCodeRose &&
                v_date_p0 == other.v_date_p0 &&
                v_date_p1 == other.v_date_p1 &&
                v_date_p2 == other.v_date_p2;
        }
    }
}
