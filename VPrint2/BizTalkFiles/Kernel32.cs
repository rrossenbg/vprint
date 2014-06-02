using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace BizTalkFiles
{
    public class Kernel32
    {
        public const int MAX_PATH = 260;
        public const int MAX_ALTERNATE = 14;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh; //changed all to uint from int, otherwise you run into unexpected overflow
            public uint nFileSizeLow;  //| http://www.pinvoke.net/default.aspx/Structures/WIN32_FIND_DATA.html
            public uint dwReserved0;   //|
            public uint dwReserved1;   //v
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFindHandle FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FindClose(SafeHandle hFindFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FindNextFile(SafeHandle hFindFile, out WIN32_FIND_DATA lpFindFileData);

        public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            // Methods
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            private SafeFindHandle()
                : base(true)
            {
            }

            private SafeFindHandle(IntPtr preExistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                base.SetHandle(preExistingHandle);
            }

            protected override bool ReleaseHandle()
            {
                if (!(IsInvalid || IsClosed))
                {
                    return FindClose(this);
                }
                return (IsInvalid || IsClosed);
            }

            protected override void Dispose(bool disposing)
            {
                if (!(IsInvalid || IsClosed))
                {
                    FindClose(this);
                }
                base.Dispose(disposing);
            }
        }
    }
}
