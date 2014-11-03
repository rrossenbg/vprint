/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DEMATLib.Data;
using System.Collections.Generic;
using DEMATLib.Dior;

namespace DEMATLib
{
    /// <summary>
    /// http://blogs.msdn.com/b/wriju/archive/2008/02/28/linq-to-xml-creating-complete-xml-document.aspx
    /// </summary>
    public class DiorXmlBuilder
    {
        /// <summary>
        /// storeId is blank, PremierStoreId is br_id, storeCountryCode is br_iso_id, vouchercount = no of vouchers, document Date = get date
        /// vouchers are group by retailer and country
        /// </summary>
        public class VoucherHeader
        {
            public int StoreId { get; set; }
            public int PremierStoreId { get; set; }
            public int StoreCountryCode { get; set; }
            public DateTime DocumentDate { get; set; }

            public static explicit operator VoucherHeader(Retailer r)
            {
                var vh = new VoucherHeader();
                vh.StoreId = 0;
                vh.PremierStoreId = r.BrId;
                vh.StoreCountryCode = r.IsoId;
                vh.DocumentDate = DateTime.Now;
                return vh;
            }
        }

        public class VoucherStatus
        {
            /// <summary>
            /// v_number from voucher table
            /// </summary>
            public int VoucherNumber { get; set; }

            /// <summary>
            /// v_br_id from voucher
            /// </summary>
            public int PremierStoreId { get; set; }

            /// <summary>
            /// v_iso_id
            /// </summary>
            public int CountryCode { get; set; }

            /// <summary>
            /// get date ()
            /// </summary>
            public DateTime TimeStamp { get; set; }

            /// <summary>
            /// v_date_voucher
            /// </summary>
            public DateTime? BdvDate { get; set; }

            /// <summary>
            /// v_voided flag
            /// </summary>
            public bool? IsVoided { get; set; }

            /// <summary>
            /// this date is on the void voucher table
            /// </summary>
            public DateTime? VoidedDate { get; set; }

            public bool? IsClaimed { get { return ClaimedDate.HasValue ? true : (bool?)null; } }
            public DateTime? ClaimedDate { get; set; }

            /// <summary>
            /// v_date_stamp
            /// </summary>
            public DateTime? StampedDate { get; set; }

            /// <summary>
            /// if voucher refunded (use v_payment status. 
            /// if payment status is refund paid,
            /// refund issued or refund successful then true else blank
            /// </summary>
            public bool? IsRefunded { get { return RefundedDate.HasValue ? true : (bool?)null; } }

            /// <summary>
            /// for cash (refund method 3,5,6,7) vouchers 
            /// use v_refund_Date from voucher table. 
            /// else use rh_date from refund history table
            /// </summary>
            public DateTime? RefundedDate { get; set; }

            /// <summary>
            /// v_date_debited
            /// </summary>
            public DateTime? DebitDate { get; set; }
            public DateTime? DebitRejectedDate { get; set; }

            /// <summary>
            /// send description of v_ic_id. join voucher and 
            /// inhibit code to get the description for the code
            /// </summary>
            public string ErrorCode { get; set; }

            /// <summary>
            /// site code of P2 voucher (p2_site_code + p2_location_number)
            /// </summary>
            public string SiteCodeRose { get; set; }

            /// <summary>
            /// if P1 exists true else blank
            /// </summary>
            public bool? HasFactureP1 { get { return FactureP1Date.HasValue ? true : (bool?)null; } }

            /// <summary>
            ///  P1 entry date v_date_p1
            /// </summary>
            public DateTime? FactureP1Date { get; set; }

            /// <summary>
            /// If P2 exists true else blank
            /// </summary>
            public bool? HasFactureP2 { get { return FactureP2Date.HasValue ? true : (bool?)null; } }

            /// <summary>
            /// P2 entry date v_date_p2
            /// </summary>
            public DateTime? FactureP2Date { get; set; }

            public static explicit operator VoucherStatus(Voucher v)
            {
                var vs = new VoucherStatus();
                vs.VoucherNumber = v.VId;
                vs.PremierStoreId = v.BrId;
                vs.CountryCode = v.IsoId;
                vs.TimeStamp = DateTime.Now;
                vs.BdvDate = v.v_date_purchase;

                vs.VoidedDate = v.VoidedDate;
                vs.ClaimedDate = v.v_date_p2;

                vs.StampedDate = v.v_date_stamp;
                vs.RefundedDate = v.v_date_refund;
                vs.DebitDate = v.v_date_debited;

                if (!v.v_ic_id.IsNullOrWhiteSpace() && DiorExportProcessor.InhibitCodes.ContainsKey(v.v_ic_id))
                    vs.ErrorCode = Convert.ToString(DiorExportProcessor.InhibitCodes[v.v_ic_id]);

                vs.SiteCodeRose = v.SiteCodeRose;

                //Ravi said I should leave those empty. 22-10-2014
                vs.FactureP1Date = null;// v.v_date_p1 ?? v.v_date_p0;
                vs.FactureP2Date = null;// v.v_date_p2;

                vs.IsVoided = v.v_voucher_void;
                vs.DebitRejectedDate = null;
                return vs;
            }
        }

        private readonly XDocument m_doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
        private readonly XElement m_vlistData, m_vlistHead, m_vstatusList;

        private int m_HeadersCount;
        private int m_VouchersCount;

        public bool IsEmpty
        {
            get
            {
                return m_VouchersCount == 0;
            }
        }

        public DiorXmlBuilder()
        {
            m_vlistHead = new XElement("VoucherListHeader");
            m_vstatusList = new XElement("VoucherStatusList");
            m_vlistData = new XElement("VoucherListData", m_vlistHead, m_vstatusList);
            m_doc.Add(m_vlistData);
        }

        public void AddHeader(VoucherHeader h)
        {
            m_HeadersCount++;
            m_vlistHead.Add(new XAttribute("StoreId", h.StoreId));
            m_vlistHead.Add(new XAttribute("PremierStoreId", h.PremierStoreId));
            m_vlistHead.Add(new XAttribute("StoreCountryCode", h.StoreCountryCode));
            m_vlistHead.Add(new XAttribute("DocumentDate", h.DocumentDate));
        }

        public void AddStatus(VoucherStatus s)
        {
            m_VouchersCount++;

            m_vstatusList.Add(new XElement("VoucherStatus",
                new XElement("VoucherNumber", s.VoucherNumber.ToString("000000000")),
                new XElement("PremierStoreId", s.PremierStoreId),
                new XElement("CountryCode", s.CountryCode),
                new XElement("TimeStamp", s.TimeStamp),
                new XElement("BdvDate", s.BdvDate),
                new XElement("IsVoided", s.IsVoided),
                new XElement("VoidedDate", s.VoidedDate),
                new XElement("IsClaimed", s.IsClaimed),
                new XElement("ClaimedDate", s.ClaimedDate),
                new XElement("StampedDate", s.StampedDate),
                new XElement("IsRefunded", s.IsRefunded),
                new XElement("RefundedDate", s.RefundedDate),
                new XElement("DebitDate", s.DebitDate),
                new XElement("ErrorCode", s.ErrorCode),
                new XElement("SiteCodeRose", s.SiteCodeRose),
                new XElement("HasFactureP1", s.HasFactureP1),
                new XElement("FactureP1Date", s.FactureP1Date),
                new XElement("HasFactureP2", s.HasFactureP2),
                new XElement("FactureP2Date", s.FactureP2Date)));
        }

        public void Close()
        {
            var vCount = new XAttribute("VoucherCount", m_VouchersCount);
            m_vlistHead.Add(vCount);
        }

        public override string ToString()
        {
            using (var mem = new MemoryStream())
            using (var writer = new XmlTextWriter(mem, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                m_doc.WriteTo(writer);
                writer.Flush();
                mem.Flush();
                mem.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(mem))
                {
                    var xml = reader.ReadToEnd();
                    return xml;
                }
            }
        }
    }
}
