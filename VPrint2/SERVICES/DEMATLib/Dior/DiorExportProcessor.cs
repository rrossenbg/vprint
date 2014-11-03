/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using DEMATLib.Data;

namespace DEMATLib.Dior
{
    public class DiorExportProcessor
    {
        public static event ThreadExceptionEventHandler Error;

        public static string ExportDirectory { get; set; }

        private readonly IList<HeadOffice> m_HeadOffices;

        public static Hashtable InhibitCodes { get; set; }

        public DiorExportProcessor(IList<HeadOffice> hoList)
        {
            m_HeadOffices = hoList;
        }

        public void Run()
        {
            try
            {
                if (m_HeadOffices == null)
                    throw new ArgumentNullException("headOffices");

                if (string.IsNullOrWhiteSpace(ExportDirectory))
                    throw new ArgumentException("ExportDirectory");

                if (!Directory.Exists(ExportDirectory))
                    Directory.CreateDirectory(ExportDirectory);

                if (InhibitCodes == null)
                    InhibitCodes = Hashtable.Synchronized(DiorDataAccess.SelectInhibitCodes());

                foreach (var ho in m_HeadOffices)
                {
                    string name = DiorDataAccess.SelectTradingName(ho.IsoId, ho.HoId);

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        FireError(new Exception(string.Format("Cannot find trading name for iso: {0} ho: {1}", ho.IsoId, ho.HoId)));
                        ho.Name = "NA";
                    }
                    else if (name.IndexOf("dior", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "DIOR";
                    else if (name.IndexOf("chanel", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "CHANEL";
                    else if (name.IndexOf("printemps", StringComparison.InvariantCultureIgnoreCase) != -1)
                        ho.Name = "PRINTEMPS";
                    else
                        ho.Name = name.Replace(' ', '_');
                }

                foreach (var ho in m_HeadOffices)
                {
                    var retailers = DiorDataAccess.SelectAllDiorRetailes(ho.IsoId, ho.HoId);

                    foreach (var br in retailers)
                    {
                        try
                        {
                            Trace.WriteLine(br.ToString(), "DIOR");

                            var b = new DiorXmlBuilder();
                            b.AddHeader((DiorXmlBuilder.VoucherHeader)br);

                            var vouchersInVoucherTable = DiorDataAccess.SelectVouchersPerRetailer(br.IsoId, br.BrId);
                            var vouchersInCacheTable = DiorObjDataAccess.SelectVouchersPerRetailer(br.IsoId, br.BrId);

                            foreach (var voucherFromVTable in vouchersInVoucherTable)
                            {
                                Debug.Assert(voucherFromVTable != null);

                                Trace.WriteLine("Voucher From VTable " + voucherFromVTable.ToString(), "DIOR");

                                var voucherFromCacheTable = vouchersInCacheTable.FirstOrDefault(_v => _v.IsoId == voucherFromVTable.IsoId && _v.VId == voucherFromVTable.VId);

                                if (voucherFromCacheTable == null || !voucherFromVTable.Equals(voucherFromCacheTable))
                                {
                                    //Not exist or changed
                                    try
                                    {
                                        Trace.WriteLine("Voucher Not exist or changed ", "DIOR");

                                        using (var tran = new TransactionScope(TransactionScopeOption.RequiresNew))
                                        {
                                            DiorObjDataAccess.DeleteVoucher(voucherFromVTable);
                                            DiorObjDataAccess.InsertVoucher(voucherFromVTable);
                                            tran.Complete();
                                            b.AddStatus((DiorXmlBuilder.VoucherStatus)voucherFromVTable);
                                            Trace.WriteLine("Save voucher state ", "DIOR");
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        FireError(ex);
                                    }
                                }
                            }

                            if (!b.IsEmpty)
                            {
                                b.Close();

                                Trace.WriteLine("Export voucher ", "DIOR");

                                var xml = b.ToString();
                                string fileName = string.Format("{0}_Export_{1}_{2}_{3:yyyy-MM-dd}.xml", ho.Name, br.BrId, br.IsoId, DateTime.Today);
                                string path = Path.Combine(ExportDirectory, fileName);
                                File.WriteAllText(path, xml, Encoding.UTF8);
                            }
                        }
                        finally
                        {
                            Thread.Yield();
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                FireError(ex2);
            }
        }

        private void FireError(Exception ex2)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex2));
        }
    }
}
