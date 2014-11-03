/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;
using DEMATLib.Data;

namespace DEMATLib
{
    public class InvoiceProcessor : Processor
    {
        private readonly int m_Iso;

        public InvoiceProcessor(int iso, string exportDirectory)
            : base(exportDirectory)
        {
            m_Iso = iso;
        }

        public void ZeroExportNumbers()
        {
            DEMARDataAccess.ZeroInvoiceNumbers(m_Iso);
        }

        /// <summary>
        /// Process all (Safe)
        /// </summary>
        /// <remarks>Safe</remarks>
        public void ProcessAll()
        {
            try
            {
                var list = DEMARDataAccess.SelectAllDistinctHOs(m_Iso);
                foreach (var hoId in list)
                    ProcessOne(hoId);
            }
            catch (Exception ex2)
            {
                FireError(ex2);
            }
        }

        /// <summary>
        /// Process one (Safe)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="valid"></param>
        /// <remarks>Safe</remarks>
        public void ProcessOne(int hoId)
        {
            Trace.WriteLine(string.Format("Processing ISOID: {0}\t HOID: {1}", m_Iso, hoId), Strings.DEMAT);
            try
            {
                var list = DEMARDataAccess.SelectAllVouchersToExport(m_Iso, hoId);

                foreach (var g in list.GroupBy(i => i.v_br_id))
                {
                    //Disqualified vouchers would probably have a v_status_id 10, 12, 17, 18, 26, 27, 35
                    foreach (var item in g)
                    {
                        Trace.WriteLine(string.Format("Exporting HOID: {0} BRID: {1} VID: {2} Type: {3}", hoId, item.v_br_id, item.v_number, item.isValid ? "VALID" : "VOID"), Strings.DEMAT);
                        try
                        {
                            var v = DEMARDataAccess.SelectVoucher(m_Iso, item.v_number);

                            var b = new BDV_InvoiceBuilder(item.isValid);

                            b.SetBuyer("Premier Tax Free", "", "33-35 Rue Rennequin", "", "", "Paris", "75017", "FR50377627641");
                            b.SetError("");

                            b.SetRetailer(v.br_id, v.br_name, "", 
                                v.br_add_1 ?? " ", 
                                v.br_add_2 ?? " ", 
                                v.br_add_4 ?? " ", 
                                v.br_add_3 ?? " ", 
                                v.br_add_5 ?? " ", 
                                //TRS_DEMAT_024	Due to that the value of “Retailer_VATNumber “ is the VATNumber of the HO into the TRS, All HO must have a VATNumber.
                                v.ho_vat_number);
                            b.SetVoucherDetails(v.v_number, v.v_date_voucher);

                            string name = string.Concat(v.v_title, " ", v.v_firstname, " ", v.v_lastname).Trim();

                            var refund_currency = DEMARDataAccess.GetRefundCurrency(m_Iso, v.v_number);

                             b.SetTouristDetails(name, 
                                v.v_final_country, 
                                v.v_passport_no,
                                refund_currency.GetValueOrDefault(),
                                refund_currency.HasValue, 
                                v.v_refund_str);

                            var lines = DEMARDataAccess.SelectVoucherLines(m_Iso, item.v_number);

                            #region SAMPLE_LINES

                            /// vl_line_number	vl_unit_price	vl_quantity	vl_pp_excl_vat	vl_pp_vat	vl_pp_incl_vat
                            /// 1	            7.50	            1	        6.17	        1.33	    7.50
                            /// 2	            7.50	            1	        6.17	        1.33	    7.50
                            /// 3	            7.50	            1	        6.17	        1.33	    7.50
                            /// 4	            7.50	            1	        6.17	        1.33	    7.50
                            /// 5	            7.50	            6	        37.04	        7.96	    45.00

                            #endregion //SAMPLE_LINES

                            //NOTE: Make sure the InvoiceTotal adds up to the line items total purchase price and also individual VAT rates totalpurchase price

                            var vatrates = DEMARDataAccess.GetVATRatesByCountryAndDate(m_Iso, v.v_date_voucher);
                            if (vatrates.Count == 0)
                                throw new ApplicationException("No vat rates. Country: {0} Date: {1}".format(m_Iso, v.v_date_voucher));

                            foreach (var vl in lines)
                            {
                                var vlvat = vatrates[(DEMARDataAccess.VatRates)vl.vl_code_ttc];

                                b.SetVoucherLine(
                                    vl.vl_line_number.ToString(),
                                    vl.vl_product,
                                    vl.vl_unit_price,
                                    vl.vl_quantity.ToString(),
                                    vlvat,//<--Rate
                                    vl.vl_pp_excl_vat,
                                    vl.vl_pp_incl_vat,
                                    vl.vl_pp_vat);
                            }

                            //Rate - It should be in numerical value and not N, L, H, O
                            foreach (var gvat in lines.GroupBy(vl => vl.vl_code_ttc))
                            {
                                var vat = gvat.Key;
                                var vlvat = vatrates[(DEMARDataAccess.VatRates)vat];
                                var sum_vl_pp_vat = gvat.Sum(vl => vl.vl_pp_vat);
                                var sum_vl_pp_excl_vat = gvat.Sum(vl => vl.vl_pp_excl_vat);
                                var sum_vl_pp_incl_vat = gvat.Sum(vl => vl.vl_pp_incl_vat);
                                b.SetTotalPerVAT(vlvat, sum_vl_pp_vat, sum_vl_pp_excl_vat, sum_vl_pp_incl_vat);
                            }
                            b.SetInvoiceTotal(lines.Sum(vl => vl.vl_pp_incl_vat));

                            var xml = b.CreateXML();
                            var fileName = b.CreateFileName(++v.br_DEMAT_export_number);//++count//
                            var fullFileName = Path.Combine(ExportDirectory, fileName);

                            using (TransactionScope tran = new TransactionScope())
                            {
                                if (File.Exists(fullFileName))
                                    File.Delete(fullFileName);

                                File.WriteAllText(fullFileName, xml);

                                DEMARDataAccess.SetVoucherSentToDemat(m_Iso, item.v_number);

                                DEMARDataAccess.SaveDEMATExportID(m_Iso, item.v_br_id, v.br_DEMAT_export_number);

                                Trace.WriteLine(fileName, Strings.DEMAT);
                                var message = string.Concat("P1 Voucher exported successfully. Export filename ", fileName);
                                DEMARDataAccess.LogVoucherExported("FintraxDEMATService", m_Iso, item.v_number, message);

                                tran.Complete();
                            }
                        }
                        catch (Exception ex1)
                        {
                            FireError(ex1);
                        }
                        finally
                        {
                            Trace.WriteLine("", Strings.DEMAT);
                            Thread.Yield();
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                FireError(ex2);
            }
            finally
            {
                Trace.WriteLine("", Strings.DEMAT);
                Thread.Yield();
            }
        }
    }
}
