/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Runtime.InteropServices;

namespace PremierTaxFree.Scan
{
    /// <summary>
    /// TWAIN driver definition file
    /// </summary>
    
    [Flags]
    internal enum TwDG : short
    {
        // DG_.....
        Control = 0x0001,
        Image = 0x0002,
        Audio = 0x0004
    }

    internal enum TwDAT : short
    {
        // DAT_....
        Null = 0x0000,
        Capability = 0x0001,
        Event = 0x0002,
        Identity = 0x0003,
        Parent = 0x0004,
        /// <summary>
        /// DAT_PENDINGXFERS
        /// </summary>
        PendingXfers = 0x0005,
        SetupMemXfer = 0x0006,
        SetupFileXfer = 0x0007,
        Status = 0x0008,
        UserInterface = 0x0009,
        XferGroup = 0x000a,
        TwunkIdentity = 0x000b,
        CustomDSData = 0x000c,
        DeviceEvent = 0x000d,
        FileSystem = 0x000e,
        PassThru = 0x000f,

        /// <summary>
        /// DAT_IMAGEINFO
        /// </summary>
        ImageInfo = 0x0101,
        /// <summary>
        /// DAT_IMAGELAYOUT
        /// </summary>
        ImageLayout = 0x0102,
        /// <summary>
        /// DAT_IMAGEMEMXFER
        /// </summary>
        ImageMemXfer = 0x0103,//
        /// <summary>
        /// DAT_IMAGENATIVEXFER
        /// </summary>
        ImageNativeXfer = 0x0104,
        ImageFileXfer = 0x0105,
        CieColor = 0x0106,
        GrayResponse = 0x0107,
        RGBResponse = 0x0108,
        JpegCompression = 0x0109,
        Palette8 = 0x010a,
        ExtImageInfo = 0x010b,

        SetupFileXfer2 = 0x0301
    }

    internal enum TwMSG : short
    {
        // MSG_.....
        Null = 0x0000,
        Get = 0x0001,
        GetCurrent = 0x0002,
        GetDefault = 0x0003,
        GetFirst = 0x0004,
        GetNext = 0x0005,
        Set = 0x0006,
        Reset = 0x0007,
        QuerySupport = 0x0008,

        XFerReady = 0x0101,
        CloseDSReq = 0x0102,
        CloseDSOK = 0x0103,
        DeviceEvent = 0x0104,

        CheckStatus = 0x0201,

        OpenDSM = 0x0301,
        CloseDSM = 0x0302,

        OpenDS = 0x0401,
        CloseDS = 0x0402,
        UserSelect = 0x0403,

        /// <summary>
        /// MSG_DISABLEDS
        /// </summary>
        DisableDS = 0x0501,
        EnableDS = 0x0502,
        EnableDSUIOnly = 0x0503,

        ProcessEvent = 0x0601,

        /// <summary>
        /// MSG_ENDXFER
        /// </summary>
        EndXfer = 0x0701,
        /// <summary>
        /// MSG_STOPFEEDER
        /// </summary>
        StopFeeder = 0x0702,

        ChangeDirectory = 0x0801,
        CreateDirectory = 0x0802,
        Delete = 0x0803,
        FormatMedia = 0x0804,
        GetClose = 0x0805,
        GetFirstFile = 0x0806,
        GetInfo = 0x0807,
        GetNextFile = 0x0808,
        Rename = 0x0809,
        Copy = 0x080A,
        AutoCaptureDir = 0x080B,

        PassThru = 0x0901
    }

    internal enum TwRC : short
    {
        // TWRC_....
        Success = 0x0000,
        Failure = 0x0001,
        CheckStatus = 0x0002,
        Cancel = 0x0003,
        DSEvent = 0x0004,
        NotDSEvent = 0x0005,
        XferDone = 0x0006,
        EndOfList = 0x0007,
        InfoNotSupported = 0x0008,
        DataNotAvailable = 0x0009
    }

    internal enum TwCC : short
    {
        // TWCC_....
        Success = 0x0000,
        Bummer = 0x0001,
        LowMemory = 0x0002,
        NoDS = 0x0003,
        MaxConnections = 0x0004,
        OperationError = 0x0005,
        BadCap = 0x0006,
        BadProtocol = 0x0009,
        BadValue = 0x000a,
        SeqError = 0x000b,
        BadDest = 0x000c,
        CapUnsupported = 0x000d,
        CapBadOperation = 0x000e,
        CapSeqError = 0x000f,
        Denied = 0x0010,
        FileExists = 0x0011,
        FileNotFound = 0x0012,
        NotEmpty = 0x0013,
        PaperJam = 0x0014,
        PaperDoubleFeed = 0x0015,
        FileWriteError = 0x0016,
        CheckDeviceOnline = 0x0017
    }

    internal enum TwOn : short
    {
        // TWON_....
        Array = 0x0003,
        Enum = 0x0004,
        One = 0x0005,
        Range = 0x0006,
        DontCare = -1
    }

    internal enum TwType : short
    {
        // TWTY_....
        Int8 = 0x0000,
        Int16 = 0x0001,
        Int32 = 0x0002,
        UInt8 = 0x0003,
        UInt16 = 0x0004,
        UInt32 = 0x0005,
        Bool = 0x0006,
        Fix32 = 0x0007,
        Frame = 0x0008,
        Str32 = 0x0009,
        Str64 = 0x000a,
        Str128 = 0x000b,
        Str255 = 0x000c,
        Str1024 = 0x000d,
        Str512 = 0x000e
    }

    internal enum TwCap : short
    {
        CAP_XFERCOUNT = 0x0001,
        CAP_SUPPORTEDCAPS = 0x1005,
        CAP_SUPPORTEDSIZES = 0x1122,
        CAP_PHYSICALHEIGHT = 0x1112,
        CAP_CUSTOMDSDATA = 0x1015,

        ICAP_COMPRESSION = 0x0100,
        ICAP_PIXELTYPE = 0x0101,
        ICAP_UNITS = 0x0102,
        ICAP_XFERMECH = 0x0103,
        ICAP_XRESOLUTION = 0x1118,
        ICAP_YRESOLUTION = 0x1119,
        ICAP_UNDEFINEDIMAGESIZE = 0X112D,

        CAP_PRINTER = 0x1026,
        CAP_PRINTERENABLED = 0x1027,
        CAP_PRINTERINDEX = 0x1028,
        CAP_PRINTERMODE = 0x1029,
        CAP_PRINTERSTRING = 0x102A,
        CAP_PRINTERSUFFIX = 0x102B,
    }

    //#define TWPR_IMPRINTERTOPBEFORE     0
    //#define TWPR_IMPRINTERTOPAFTER      1
    //#define TWPR_IMPRINTERBOTTOMBEFORE  2
    //#define TWPR_IMPRINTERBOTTOMAFTER   3
    //#define TWPR_ENDORSERTOPBEFORE      4
    //#define TWPR_ENDORSERTOPAFTER       5
    //#define TWPR_ENDORSERBOTTOMBEFORE   6
    //#define TWPR_ENDORSERBOTTOMAFTER    7

    public class TwProtocol
    {
        // TWON_PROTOCOL...
        public const short Major = 1;
        public const short Minor = 9;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    internal class TwIdentity
    {
        // TW_IDENTITY
        public IntPtr Id;
        public TwVersion Version;
        public short ProtocolMajor;
        public short ProtocolMinor;
        public int SupportedGroups;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string Manufacturer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string ProductFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string ProductName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    internal struct TwVersion
    {
        // TW_VERSION
        public short MajorNum;
        public short MinorNum;
        public short Language;
        public short Country;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string Info;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwUserInterface
    {
        // TW_USERINTERFACE
        public short ShowUI;				// bool is strictly 32 bit, so use short
        public short ModalUI;
        public IntPtr ParentHand;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwStatus
    {									// TW_STATUS
        public short ConditionCode;		// TwCC
        public short Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct TwEvent
    {
        // TW_EVENT
        public IntPtr EventPtr;
        public short Message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwImageInfo
    {
        // TW_IMAGEINFO
        public int XResolution;
        public int YResolution;
        public int ImageWidth;
        public int ImageLength;
        public short SamplesPerPixel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public short[] BitsPerSample;
        public short BitsPerPixel;
        public short Planar;
        public short PixelType;
        public short Compression;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwPendingXfers
    {
        // TW_PENDINGXFERS
        public short Count;
        public int EOJ;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwOn_OneValue
    {
        // TWON_ONEVALUE
        public short ItemType;
        public int Item;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct TwFix32
    {
        // TW_FIX32
        public short Whole;
        public ushort Frac;

        public float ToFloat()
        {
            return (float)Whole + ((float)Frac / 65536.0f);
        }

        public void FromFloat(float f)
        {
            int i = (int)((f * 65536.0f) + 0.5f);
            Whole = (short)(i >> 16);
            Frac = (ushort)(i & 0x0000ffff);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwCapability : IDisposable
    {
        public short Cap;
        public short ConType;
        public IntPtr Handle;
        // TW_CAPABILITY
        public TwCapability(TwCap cap)
        {
            Cap = (short)cap;
            ConType = -1;
        }
        public TwCapability(TwCap cap, short sval)
        {
            Cap = (short)cap;
            ConType = (short)TwOn.One;
            Handle = Twain.GlobalAlloc(0x42, 6);
            IntPtr pv = Twain.GlobalLock(Handle);
            Marshal.WriteInt16(pv, 0, (short)TwType.Int16);
            Marshal.WriteInt32(pv, 2, (int)sval);
            Twain.GlobalUnlock(Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cap"></param>
        /// <param name="str">Maximum 255 ASCII chars</param>
        /// <param name="typ"></param>
        public TwCapability(TwCap cap, string str, TwType typ)
        {
            Cap = (short)cap;
            ConType = (short)TwOn.One;
            Handle = Twain.GlobalAlloc(0x42, 258);
            IntPtr pv = Twain.GlobalLock(Handle);
            Marshal.WriteInt16(pv, 0, (short)typ);

            int j;
            for (j = 0; j < Math.Min(str.Length - 1, 255); j++)
                Marshal.WriteByte(pv, 2 + j, (byte)(int)str[j]);
            Marshal.WriteByte(pv, 2 + j, (byte)0);
            Twain.GlobalUnlock(Handle);
        }

        ~TwCapability()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
                Twain.GlobalFree(Handle);

            Handle = IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwFrame
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwImageLayout
    {
        public TwFrame Frame;
        public int DocumentNumber;
        public int PageNumber;
        public int FrameNumber;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class TwCustomData
    {
        public int DataSize;
        public byte[] Space = new byte[100000];
    }
}
