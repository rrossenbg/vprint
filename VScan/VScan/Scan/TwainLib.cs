/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PremierTaxFree.Data;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.Utils;

namespace PremierTaxFree.Scan
{
    public enum TwainCommand
    {
        Not = -1,
        Null = 0,
        TransferReady = 1,
        CloseRequest = 2,
        CloseOk = 3,
        DeviceEvent = 4
    }

    /// <summary>
    /// TWAIN driver interface class
    /// </summary>
    public class Twain
    {
        private const short CountryUSA = 1;
        private const short LanguageEN_USA = 13;

        public IntPtr MainForm { get; set; }
        public IntPtr ScanForm { get; set; }
        public ScanForm Form { get; set; }
        private bool UseImprinter { get; set; }

        public Twain()
        {
            m_appId = new TwIdentity();
            m_appId.Id = IntPtr.Zero;
            m_appId.Version.MajorNum = 1;
            m_appId.Version.MinorNum = 1;
            m_appId.Version.Language = LanguageEN_USA;
            m_appId.Version.Country = CountryUSA;
            m_appId.Version.Info = Application.ProductVersion;
            m_appId.ProtocolMajor = TwProtocol.Major;
            m_appId.ProtocolMinor = TwProtocol.Minor;
            m_appId.SupportedGroups = (int)(TwDG.Image | TwDG.Control);
            m_appId.Manufacturer = Application.CompanyName;
            m_appId.ProductFamily = Application.ProductName;
            m_appId.ProductName = Application.ProductName;

            m_srcDs = new TwIdentity();
            m_srcDs.Id = IntPtr.Zero;

            m_evtMsg.EventPtr = Marshal.AllocHGlobal(Marshal.SizeOf(m_winMsg));
        }

        ~Twain()
        {
            Marshal.FreeHGlobal(m_evtMsg.EventPtr);
        }

        /// <summary>
        /// Init data source
        /// </summary>
        /// <param name="hwndp"></param>
        public void Init(IntPtr hwndp)
        {
            CloseDSM();
            TwRC rc = DSMparent(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.OpenDSM, ref hwndp);
            if (rc == TwRC.Success)
            {
                rc = DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.GetDefault, m_srcDs);
                if (rc == TwRC.Success)
                    m_hWnd = hwndp;
                else
                    rc = DSMparent(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.CloseDSM, ref hwndp);
            }
        }

        /// <summary>
        /// Selects a datasource.
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns></returns>
        public IEnumerable<string> Select(string scannerName)
        {
            CloseDS();
            if (m_appId.Id == IntPtr.Zero)
            {
                Init(m_hWnd);
            }
            if (string.IsNullOrEmpty(scannerName))
            {
                CheckAndThrow(DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.UserSelect, m_srcDs), "Cannot select scanner");
            }
            else
            {
                var twid = new TwIdentity();
                TwRC rc = DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.GetFirst, twid);
                while (rc == TwRC.Success)
                {
                    if (string.Equals(scannerName, twid.ProductName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        rc = DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.Set, twid);
                        if (rc == TwRC.Success)
                        {
                            m_srcDs = twid;
                            yield return twid.ProductName;
                        }
                    }
                    rc = DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.GetNext, twid);
                }
            }
        }

        /// <summary>
        /// Acquires image from scanner
        /// </summary>
        /// <param name="defaltSettings"></param>
        /// <param name="imprinterText">String to print. It's null or empty if no Imprinting</param>
        public void Acquire(bool defaltSettings, string imprinterText)
        {
            Debug.Assert(string.IsNullOrEmpty(imprinterText) || Encoding.ASCII.GetByteCount(imprinterText) < 256);

            UseImprinter = !string.IsNullOrEmpty(imprinterText);

            CloseDS();

            CloseDSM();

            Init(m_hWnd);

            CheckAndThrow(DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.OpenDS, m_srcDs), "Cannot open DS");

            var cap = new TwCapability(TwCap.CAP_XFERCOUNT, 1);
            CheckCloseAndThrow(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap), "Cannot set cap CAP_XFERCOUNT");

            cap = new TwCapability(TwCap.CAP_SUPPORTEDSIZES, 0);
            CheckCloseAndThrow(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap), "Cannot set cap CAP_SUPPORTEDSIZES");
            var layout = new TwImageLayout();
            CheckCloseAndThrow(DSilayout(m_appId, m_srcDs, TwDG.Image, TwDAT.ImageLayout, TwMSG.Get, layout), "Cannot get ImageLayout");

            //Use Imprinter
            cap = new TwCapability(TwCap.CAP_PRINTER, TWPR_IMPRINTERBOTTOMAFTER);
            if (Check(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap)))
            {
                cap = new TwCapability(TwCap.CAP_PRINTERENABLED, (imprinterText.IsNullOrEmpty() ? FALSE : TRUE));
                CheckCloseAndThrow(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap), "Cannot set cap CAP_PRINTERENABLED");

                if (!imprinterText.IsNullOrEmpty())
                {
                    cap = new TwCapability(TwCap.CAP_PRINTERMODE, TWPM_SINGLESTRING);
                    CheckCloseAndThrow(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap), "Cannot set cap CAP_PRINTERMODE");

                    cap = new TwCapability(TwCap.CAP_PRINTERSTRING, imprinterText, TwType.Str255);
                    CheckCloseAndThrow(DScap(m_appId, m_srcDs, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap), "Cannot set cap CAP_PRINTERSTRING");
                }
            }
            
            var guif = new TwUserInterface();
            guif.ShowUI = (short)(defaltSettings ? FALSE : TRUE);
            guif.ModalUI = 1;
            guif.ParentHand = m_hWnd;
            CheckCloseAndThrow(DSuserif(m_appId, m_srcDs, TwDG.Control, TwDAT.UserInterface, TwMSG.EnableDS, guif), "Cannot enable DS");
        }

        /// <summary>
        /// Transfers images from the scanner. Starts image processing by processing delegate chains.
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public bool TransferPictures(Voucher voucherData)
        {
            if (m_srcDs.Id == IntPtr.Zero)
                return false;

            TwRC rc;
            IntPtr hbitmap = IntPtr.Zero;
            TwPendingXfers pxfr = new TwPendingXfers();

            do
            {
                pxfr.Count = 0;

                try
                {
                    //Acquirs image from scanner
                    TwImageInfo iinf = new TwImageInfo();
                    rc = DSiinf(m_appId, m_srcDs, TwDG.Image, TwDAT.ImageInfo, TwMSG.Get, iinf);
                    if (rc != TwRC.Success)
                        return false;

                    rc = DSixfer(m_appId, m_srcDs, TwDG.Image, TwDAT.ImageNativeXfer, TwMSG.Get, ref hbitmap);
                    if (rc != TwRC.XferDone)
                        return false;

                    rc = DSpxfer(m_appId, m_srcDs, TwDG.Control, TwDAT.PendingXfers, TwMSG.EndXfer, pxfr);
                    if (rc != TwRC.Success)
                        return false;

                    //Reads voucher data from common context
                    if (voucherData == null)
                        voucherData = ScanAppContext.FillFromScanContext(new Voucher());

                    //Creates temporary state object
                    StateObj stateObj = new StateObj()
                    {
                        Main = MainForm,
                        Scan = ScanForm,
                        Dib = hbitmap,
                    };

                    //Creates processing delegate chain. Start processing the image.
                    var del = DelegateUtils.GetProcessDelegateChain();
                    del.FireAndForget(voucherData, stateObj);
                    voucherData = null;
                }
                finally
                {
                    DSpxfer(m_appId, m_srcDs, TwDG.Control, TwDAT.PendingXfers, TwMSG.Reset, pxfr);
                }
            }
            while (pxfr.Count != 0 && !UseImprinter);
            return true;
        }

        /// <summary>
        /// Message processing function
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public TwainCommand PassMessage(ref Message m)
        {
            if (m_srcDs.Id == IntPtr.Zero)
                return TwainCommand.Not;

            int pos = GetMessagePos();

            m_winMsg.hwnd = m.HWnd;
            m_winMsg.message = m.Msg;
            m_winMsg.wParam = m.WParam;
            m_winMsg.lParam = m.LParam;
            m_winMsg.time = GetMessageTime();
            m_winMsg.x = (short)pos;
            m_winMsg.y = (short)(pos >> 16);

            Marshal.StructureToPtr(m_winMsg, m_evtMsg.EventPtr, false);
            m_evtMsg.Message = 0;
            TwRC rc = DSevent(m_appId, m_srcDs, TwDG.Control, TwDAT.Event, TwMSG.ProcessEvent, ref m_evtMsg);
            if (rc == TwRC.NotDSEvent)
                return TwainCommand.Not;
            if (m_evtMsg.Message == (short)TwMSG.XFerReady)
                return TwainCommand.TransferReady;
            if (m_evtMsg.Message == (short)TwMSG.CloseDSReq)
                return TwainCommand.CloseRequest;
            if (m_evtMsg.Message == (short)TwMSG.CloseDSOK)
                return TwainCommand.CloseOk;
            if (m_evtMsg.Message == (short)TwMSG.DeviceEvent)
                return TwainCommand.DeviceEvent;

            return TwainCommand.Null;
        }

        /// <summary>
        /// Closes data source manager
        /// </summary>
        public void CloseDSM()
        {
            if (m_appId.Id != IntPtr.Zero)
                DSMparent(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.CloseDSM, ref m_hWnd);
            m_appId.Id = IntPtr.Zero;
        }

        /// <summary>
        /// Closes data source
        /// </summary>
        public void CloseDS()
        {
            if (m_srcDs.Id != IntPtr.Zero)
            {
                TwUserInterface guif = new TwUserInterface();
                DSuserif(m_appId, m_srcDs, TwDG.Control, TwDAT.UserInterface, TwMSG.DisableDS, guif);
                DSMident(m_appId, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.CloseDS, m_srcDs);
            }
            m_srcDs.Id = IntPtr.Zero;
        }

        private static void CheckAndThrow(TwRC rc, string message)
        {
            if (rc != TwRC.Success)
                throw new Exception(message);
        }

        private void CheckCloseAndThrow(TwRC rc, string message)
        {
            if (rc != TwRC.Success)
            {
                CloseDS();
                throw new Exception(message);
            }
        }

        private bool Check(TwRC rc)
        {
            return (rc == TwRC.Success);
        }
        
        #region PRIVATE MEMBERS

        private IntPtr m_hWnd;
        private TwIdentity m_appId;
        private TwIdentity m_srcDs;
        private TwEvent m_evtMsg;
        private WINMSG m_winMsg;

        //<prefix-string><number><suffix-string>
        const int TWPM_SINGLESTRING = 0;
        const int TWPM_MULTISTRING = 1;
        const int TWPM_COMPOUNDSTRING = 2;

        const int TWPR_IMPRINTERTOPBEFORE = 0;
        const int TWPR_IMPRINTERTOPAFTER = 1;
        const int TWPR_IMPRINTERBOTTOMBEFORE = 2;
        const int TWPR_IMPRINTERBOTTOMAFTER = 3;
        const int TWPR_ENDORSERTOPBEFORE = 4;
        const int TWPR_ENDORSERTOPAFTER = 5;
        const int TWPR_ENDORSERBOTTOMBEFORE = 6;
        const int TWPR_ENDORSERBOTTOMAFTER = 7;

        const short TRUE = 1;
        const short FALSE = 0;

        // ------ DSM entry point DAT_ variants:
        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSMparent([In, Out] TwIdentity origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, ref IntPtr refptr);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSMident([In, Out] TwIdentity origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwIdentity idds);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSMstatus([In, Out] TwIdentity origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwStatus dsmstat);

        // ------ DSM entry point DAT_ variants to DS:
        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSuserif([In, Out] TwIdentity origin, [In, Out] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, TwUserInterface guif);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSevent([In, Out] TwIdentity origin, [In, Out] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, ref TwEvent evt);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSstatus([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwStatus dsmstat);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DScap([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwCapability capa);

        [DllImport("twain_32.dll", EntryPoint ="#1")]
        private static extern TwRC DSilayout([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwImageLayout imglo);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSiinf([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwImageInfo imginf);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSixfer([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, ref IntPtr hbitmap);

        [DllImport("twain_32.dll", EntryPoint = "#1")]
        private static extern TwRC DSpxfer([In, Out] TwIdentity origin, [In] TwIdentity dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwPendingXfers pxfr);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GlobalAlloc(int flags, int size);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern bool GlobalUnlock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GlobalFree(IntPtr handle);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int GetMessagePos();

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int GetMessageTime();

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateDC(string szdriver, string szdevice, string szoutput, IntPtr devmode);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        #endregion//PRIVATE MEMBERS

        public static int ScreenBitDepth
        {
            get
            {
                IntPtr screenDC = CreateDC("DISPLAY", null, null, IntPtr.Zero);
                int bitDepth = GetDeviceCaps(screenDC, 12);
                bitDepth *= GetDeviceCaps(screenDC, 14);
                DeleteDC(screenDC);
                return bitDepth;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct WINMSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public int x;
            public int y;
        }

        public static double FromFix32(int value)
        {
            return (value >> 16) / 65536 + (value & 0xFFFF);
        }

        public static int ToFix32(double value)
        {
            return (Convert.ToInt32 (value) & 0xFFFF);//CHECK!  Return (valu And &HFFFF)
        }
    } // class Twain
}