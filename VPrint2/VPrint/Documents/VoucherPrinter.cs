/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using VPrinting.Common;
using VPrinting.Extentions;
using VPrinting.PartyManagement;
using VPrinting.VoucherNumberingAllocationPrinting;
using VNAP = VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting.Documents
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(3680)); // 13 cm
    /// b.Append(PtfTally.PrintI2Of5Barcode(barcodeNo));
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(3700));
    /// b.Append(PtfTally.SetAbsoluteVerticalPosition(2800));
    /// b.Append(addressLines.Index(0) + Environment.NewLine);//"Address line 1"
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(3700));
    /// b.Append(addressLines.Index(1));//"Address line 2"
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(5240));
    /// b.Append(voucherId + Environment.NewLine);
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(3700));
    /// b.Append(addressLines.Index(2) + Environment.NewLine);//"Address line 3"
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(3700));
    /// b.Append(addressLines.Index(3));//"Address line 4"
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(5240));//26
    /// b.Append(retailer);
    /// b.Append(PtfTally.SetAbsoluteHorizontalPosition(8240));
    /// </example>
    public class VoucherPrinter : VoucherPrinterSettings, IDataProvider, IDisposable
    {
        private static readonly ArrayList ms_VPItems = ArrayList.Synchronized(new ArrayList());

        public VNAP.VoucherAllocation Allocation { get; set; }

        public VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting Printing { get; set; }

        public PartyManagement.PartyManagement Manager { get; set; }

        public PartyManagement.Retailer Retailer { get; set; }

        public PtfOffice Office { get; set; }

        /// <summary>
        /// Number of the voucher
        /// </summary>
        public int VoucherNo { get; set; }

        /// <summary>
        /// Number of the voucher as string
        /// </summary>
        public string StrVoucherNo { get; set; }

        public Guid SessionId { get; set; }

        public volatile int RangeFrom;
        public volatile int RangeTo;

        public event EventHandler Test;
        public event EventHandler Done;

        public static event ThreadExceptionEventHandler Error;

        public volatile int AllocationId;

        public bool UseLocalFormat { get; set; }
        public bool UseLocalPrinter { get; set; }
        public bool PrintOnce { get; set; }
        public bool SimulatePrint { get; set; }

        public VoucherPrinter()
        {
            SessionId = Guid.NewGuid();
            ms_VPItems.Add(this);
            UseLocalFormat = UseLocalPrinter = false;
        }

        public void Dispose()
        {
            ms_VPItems.Remove(this);
        }

        public void PrintAllocation(int allocationId, bool demo)
        {
            PrintAllocationInternal(allocationId, null, demo);
        }

        /// <summary>
        /// Print voucher index 110
        /// printer.PrintAllocation(440972, new List<int>() { 110 });
        /// </summary>
        /// <param name="allocationId"></param>
        /// <param name="voucherIndexes"></param>
        public void PrintAllocation(int allocationId, List<int> voucherIndexes)
        {
            PrintAllocationInternal(allocationId, voucherIndexes, false);
        }

        public virtual void PrintVouchers(int allocationId, List<int> voucherNumbers)
        {
            try
            {
                if (string.IsNullOrEmpty(m_PrinterName))
                    throw new ArgumentNullException("PrinterName", "Value can not be null or empty");

                if (string.IsNullOrEmpty(m_ReportType2))
                    throw new ArgumentNullException("ReportType2", "Value can not be null or empty");

                if (string.IsNullOrEmpty(m_PrinterXmlFilePath))
                    throw new ArgumentNullException("PrinterXmlFilePath", "Value can not be null or empty");

                if (!File.Exists(m_PrinterXmlFilePath))
                    throw new IOException("Can not find file");

                AllocationId = allocationId;

                lock (this)
                {
                    while (ms_VPItems.IndexOf(this) != 0)
                        Monitor.Wait(this, 3000);
                }

                Printing = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
                Manager = new PartyManagement.PartyManagement();

                Allocation = new Func<int, VoucherAllocation>((x) => Printing.RetrieveAllocation(x)).ReTry(allocationId);

                if (Allocation == null)
                    throw new ArgumentNullException("Can not get allocation:".concat(allocationId));

                Retailer = new Func<int, int, Retailer>((x, y) => Manager.RetrieveRetailerDetail(x, y)).ReTry(Allocation.CountryId, Allocation.RetailerId);

                if (Retailer == null)
                    throw new ArgumentNullException("Can not find retailer:".concat(" CountryId: ", Allocation.CountryId, " RetailerId: ", Allocation.RetailerId));

                Office = new Func<int, int, PtfOffice>((x, y) => Manager.RetrievePtfOfficeDetail(x, y)).ReTry(Retailer.CountryId, Retailer.PrinterBranchId);

                if (Office == null)
                    throw new ArgumentNullException("Can not get office:".concat(" CountryId: ", Allocation.CountryId, " PrinterBranchId:", Retailer.PrinterBranchId));

                var guid = CommonTools.ToGuid(Allocation.CountryId, Allocation.RetailerId);

                PrinterDetails printer = null;
                CacheManager.Instance.Get<PrinterDetails>(guid, out printer,
                    () => new Func<int, int, PrinterDetails>((x, y) => Manager.GetPrinterInfo(x, y)).ReTry(Retailer.CountryId, Allocation.RetailerId));

                if (printer == null || printer.IsEmpty || UseLocalFormat)
                {
                    printer = new PrinterDetails
                    {
                        Name = "Default",
                        Path = m_PrinterName,
                        Type2 = m_ReportType2,
                        Xml = File.ReadAllText(m_PrinterXmlFilePath),
                        IsoID = Retailer.CountryId,
                        RetailerID = Allocation.RetailerId,
                    };
                }

                if (UseLocalPrinter)
                {
                    printer.Path = m_PrinterName;
                }

                #region LOCAL PRINTER
                //if (!PrintManager.GetInstalledPrinters().Contains(printer.Path, StringComparer.InvariantCultureIgnoreCase))
                //    throw new ApplicationException("Can not find printer ".concat(printer.Path));
                #endregion

                int countryId = Allocation.CountryId;

                InitPrinter("Initialization_".concat(Retailer.Name, DateTime.Now.Ticks));

                RangeFrom = Allocation.RangeFrom / 10;
                RangeTo = Allocation.RangeTo / 10;

                //  VPrinting.Documents.VoucherPrintLayout250
                Type documentType = Type.GetType(printer.Type2);
                if (documentType == null)
                    throw new ArgumentNullException("documentType", "Can not find type of: ".concat(printer.Type2));

                XmlSerializer formatter = new XmlSerializer(documentType);
                var layout = formatter.ToObject<IVoucherLayout>(printer.Xml);
                if (layout == null)
                    throw new ArgumentNullException("layout", "Can not create layout from xml");
                layout.Init();

                CacheManager.Instance.Table[Strings.SubRangeFrom] = RangeFrom;

                int index = 0;

                foreach (int voucher in voucherNumbers)
                {
                    CacheManager.Instance.Table[Strings.Index] = index++;

                    VoucherNo = voucher;

                    //Counting starts from 1 for us.
                    StrVoucherNo = new Func<int, int, bool, string>((x, y, z) => Printing.CreateVoucherNumber(x, y, z)).ReTry(countryId, voucher, false);

                    layout.Clear();
                    layout.DataBind(this, StrVoucherNo, voucher, false);

                    Debug.WriteLine(string.Format("{0}\t{1}\t{2}", countryId, Retailer.Name, voucher), Strings.VRPINT);

                    if (!SimulatePrint)
                        layout.PrintVoucher(printer.Path, StrVoucherNo, layout.FormLength, layout.DocumentInitialization, layout.PrintLines);

                    if (PrintOnce)
                        break;
                }

                if (!SimulatePrint)
                {
                    new Action<int, int, int>((x, y, z) => Printing.LogVoucherAllocationPrinted(x, y, z)).ReTry(allocationId, Program.currentUser.UserID, Program.currentUser.CountryID);
                    new Action<int, bool, int>((x, y, z) => Printing.SetVoucherAllocationPrinted(x, y, z)).ReTry(allocationId, true, Program.currentUser.UserID);
                }
                //set the printed status to true
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                if (Done != null)
                    Done(this, EventArgs.Empty);
            }
        }

        protected virtual void PrintAllocationInternal(int allocationId, List<int> voucherIndexes, bool demo)
        {
            try
            {
                if (string.IsNullOrEmpty(m_PrinterName))
                    throw new ArgumentNullException("PrinterName", "Value can not be null or empty");

                if (string.IsNullOrEmpty(m_ReportType2))
                    throw new ArgumentNullException("ReportType2", "Value can not be null or empty");

                if (string.IsNullOrEmpty(m_PrinterXmlFilePath))
                    throw new ArgumentNullException("PrinterXmlFilePath", "Value can not be null or empty");

                if (!File.Exists(m_PrinterXmlFilePath))
                    throw new IOException("Can not find file");

                AllocationId = allocationId;

                lock (this)
                {
                    while (ms_VPItems.IndexOf(this) != 0)
                        Monitor.Wait(this, 3000);
                }

                if (!demo)
                {
                    Printing = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
                    Manager = new PartyManagement.PartyManagement();

                    Allocation = new Func<int, VoucherAllocation>((x) => Printing.RetrieveAllocation(x)).ReTry(allocationId);

                    if (Allocation == null)
                        throw new ArgumentNullException("Can not get allocation:".concat(allocationId));

                    Retailer = new Func<int, int, Retailer>((x, y) => Manager.RetrieveRetailerDetail(x, y)).ReTry(Allocation.CountryId, Allocation.RetailerId);

                    if (Retailer == null)
                        throw new ArgumentNullException("Can not find retailer:".concat(" CountryId: ", Allocation.CountryId, " RetailerId: ", Allocation.RetailerId));

                    Office = new Func<int, int, PtfOffice>((x, y) => Manager.RetrievePtfOfficeDetail(x, y)).ReTry(Retailer.CountryId, Retailer.PrinterBranchId);

                    if (Office == null)
                        throw new ArgumentNullException("Can not get office:".concat(" CountryId: ", Allocation.CountryId, " PrinterBranchId:", Retailer.PrinterBranchId));

                    var guid = CommonTools.ToGuid(Allocation.CountryId, Allocation.RetailerId);

                    PrinterDetails printer = null;
                    CacheManager.Instance.Get<PrinterDetails>(guid, out printer,
                        () => new Func<int, int, PrinterDetails>((x, y) => Manager.GetPrinterInfo(x, y)).ReTry(Retailer.CountryId, Allocation.RetailerId));

                    if (printer == null || printer.IsEmpty || UseLocalFormat)
                    {
                        printer = new PrinterDetails
                        {
                            Name = "Default",
                            Path = m_PrinterName,
                            Type2 = m_ReportType2,
                            Xml = File.ReadAllText(m_PrinterXmlFilePath),
                            IsoID = Retailer.CountryId,
                            RetailerID = Allocation.RetailerId,
                        };
                    }

                    if (UseLocalPrinter)
                    {
                        printer.Path = m_PrinterName;
                    }

                    #region LOCAL PRINTER
                    //if (!PrintManager.GetInstalledPrinters().Contains(printer.Path, StringComparer.InvariantCultureIgnoreCase))
                    //    throw new ApplicationException("Can not find printer ".concat(printer.Path));
                    #endregion

                    int countryId = Allocation.CountryId;

                    InitPrinter("Initialization_".concat(Retailer.Name, DateTime.Now.Ticks));

                    RangeFrom = Allocation.RangeFrom / 10;
                    RangeTo = Allocation.RangeTo / 10;

                    //  VPrinting.Documents.VoucherPrintLayout250
                    Type documentType = Type.GetType(printer.Type2);
                    if (documentType == null)
                        throw new ArgumentNullException("documentType", "Can not find type of: ".concat(printer.Type2));

                    XmlSerializer formatter = new XmlSerializer(documentType);
                    var layout = formatter.ToObject<IVoucherLayout>(printer.Xml);
                    if (layout == null)
                        throw new ArgumentNullException("layout", "Can not create layout from xml");
                    layout.Init();

                    CacheManager.Instance.Table[Strings.SubRangeFrom] = RangeFrom;

                    for (int voucher = RangeFrom, index = 0; voucher < RangeTo + 1; voucher++, index++)
                    {
                        CacheManager.Instance.Table[Strings.Index] = index;

                        VoucherNo = voucher;

                        //Counting starts from 1 for us.
                        if (voucherIndexes == null || voucherIndexes.Empty() || voucherIndexes.Exists((i) => voucher + 1 - i == RangeFrom))
                        {
                            StrVoucherNo = new Func<int, int, bool, string>((x, y, z) => Printing.CreateVoucherNumber(x, y, z)).ReTry(countryId, voucher, false);

                            layout.Clear();
                            layout.DataBind(this, StrVoucherNo, voucher, false);

                            Debug.WriteLine(string.Format("{0}\t{1}\t{2}", countryId, Retailer.Name, voucher), Strings.VRPINT);

                            if (!SimulatePrint)
                                layout.PrintVoucher(printer.Path, StrVoucherNo, layout.FormLength, layout.DocumentInitialization, layout.PrintLines);

                            if (Test != null)
                                Test(this, EventArgs.Empty);

                            if (PrintOnce)
                                break;
                        }
                    }

                    if (!SimulatePrint)
                    {
                        new Action<int, int, int>((x, y, z) => Printing.LogVoucherAllocationPrinted(x, y, z)).ReTry(allocationId, Program.currentUser.UserID, Program.currentUser.CountryID);
                        new Action<int, bool, int>((x, y, z) => Printing.SetVoucherAllocationPrinted(x, y, z)).ReTry(allocationId, true, Program.currentUser.UserID);
                    }
                    //set the printed status to true
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                if (Done != null)
                    Done(this, EventArgs.Empty);
            }
        }

        public virtual void InitPrinter(string printDoc)
        {
#if INIT_PRINTER_PER_RETAILER
            var b = new StringBuilder();
            b.Append(PtfTally.PUMOn(true));
            b.Append(PtfTally.SetPositionalUnitMode(SSU.Millimeters));
            b.Append(PtfTally.SetFormLength(Settings.FormLength));//6119
            b.Append(PtfTally.HQMode);
            PrinterQueue.AddJob(printDoc, b.ToString());
#endif
        }
        /// <summary>
        /// PrintVoucher("Tally T2365", "DGB8262015294824022390881", 6119, "", null); 
        /// </summary>
        /// <param name="printerName">Tally T2365</param>
        /// <param name="printDocumentName">DGB8262015294824022390881</param>
        /// <param name="length">6119</param>
        /// <param name="documentInitialization"></param>
        /// <param name="lines"></param>
    }
}

//[4032`[1d[?11~!C1;000:82620152948233631823;[?10~
//[4304`[221dDGB82620152948233631823
//[4002`[2901d1 Et 1 Font 3
//[5845`[2950d233631823
//[4002`[3001d54 Ledbury Road
//[4002`[3103dWestbourne Grove
//[4002`[3204dLondon
//[5845`[3207d152948
//[4002`[3306dW11 2AG
//

//[4032`[1d[?11~!C1;000:82620164197049335212;[?10~
//[272a[220eDGB82620164197049335212
//[-302a[2680e1 Stop Photo
//[1843a[49e49335212
//[-1843a[51e205 High Street
//[102eEdinburgh
//[101eEH1 2LD
//[1843a164197
//[-1843a[99e
//
