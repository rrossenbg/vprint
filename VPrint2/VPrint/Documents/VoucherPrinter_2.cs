/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using VPrinting.Common;
using VPrinting.PartyManagement;
using VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting.Documents
{
    partial class VoucherPrinter
    {
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

                    var guid = CommTools.ToGuid(Allocation.CountryId, Allocation.RetailerId);

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

                    CacheManager.Instance.Table[Strings.IVoucherLayout] = layout;

                    CacheManager.Instance.Table[Strings.SubRangeFrom] = RangeFrom;

                    var multyLines = new Queue<IList<IPrintLine>>();

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

                            for (int count = 0; count < Repeat[countryId]; count++)
                            {
#if DEBUGGER
                            Debug.WriteLine(string.Format("{0}\t{1}\t{2}", countryId, Retailer.Name, voucher), Strings.VRPINT);
#endif

                                //Single document
                                multyLines.Enqueue(new List<IPrintLine>(layout.PrintLines));
                                if (!SimulatePrint && !MultyPagePrint)
                                    layout.PrintVouchers(printer.Path, StrVoucherNo, layout.FormLength, layout.DocumentInitialization, multyLines);

                                if (Test != null)
                                    Test(this, EventArgs.Empty);

                                if (PrintOnce)
                                    break;
                            }
                        }
                        if (PrintOnce)
                            break;
                    }

                    if (!SimulatePrint && MultyPagePrint && multyLines.Count > 0)
                    {
                        layout.PrintVouchers(printer.Path, Strings.VRPINT, layout.FormLength, layout.DocumentInitialization, multyLines);
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
    }
}
