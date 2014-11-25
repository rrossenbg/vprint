/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using PM = VPrinting.PartyManagement;
using VNAP = VPrinting.VoucherNumberingAllocationPrinting;
using System.Drawing;

namespace VPrinting
{
    /// <summary>
    /// Provides data for printing process
    /// Every class that provide data to 
    /// the printing should implement this.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Web service
        /// </summary>
        VNAP.VoucherNumberingAllocationPrinting Printing { get; set; }
        /// <summary>
        /// Web service
        /// </summary>
        PM.PartyManagement Manager { get; set; }
        /// <summary>
        /// Current retailer
        /// </summary>
        PM.Retailer Retailer { get; set; }

        VNAP.VoucherAllocation Allocation { get; set; }

        PM.PtfOffice Office { get; set; }

        int VoucherNo { get; set; }

        /// <summary>
        /// Number of the voucher as string
        /// </summary>
        string StrVoucherNo { get; set; }
    }    
    
    /// <summary>
    /// Print line is set of data. Position and text.
    /// Some Print lines may self-print
    /// </summary>
    public interface IPrintLine
    {
        float X { get; set; }
        float Y { get; set; }
        string Text { get; set; }
        void Print(StringBuilder b);
    }

    /// <summary>
    /// Prints and configures printing process.
    /// Every printing and configuration class implements this.
    /// </summary>
    public interface IVoucherLayout
    {
        string DocumentInitialization { get; set; }
        int FormLength { get; set; }
        Point MoveAll { get; set; }
        List<IPrintLine> PrintLines { get; }
        void Init();
        void Clear();
        void DataBind(IDataProvider pr, string voucherNo, int voucher, bool printDemo);
        void InitPrinter(string printDoc);
        void PrintVouchers(string printerName, string printDocName, int length, string docInitialization, Queue<IList<IPrintLine>> multilines);
        object Tag { get; set; }
    }

    /// <summary>
    /// Object that has key attached on it
    /// It may be called get and set key on it
    /// </summary>
    public interface IKeyable
    {
        Guid GetKey();
        void SetKey(Guid key);
    }
}
