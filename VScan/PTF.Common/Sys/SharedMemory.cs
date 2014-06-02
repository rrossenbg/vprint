/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Runtime.InteropServices;

namespace Lirex.Common.Net
{
    /// <summary>
    /// Shared memory class
    /// </summary>
    public class SharedMemory : IDisposable
    {
        /// <summary>
        /// constants from winnt.h
        /// </summary>
        private enum FileProtection : uint
        {
            ReadOnly = 2,
            ReadWrite = 4
        }

        /// <summary>
        /// constants from WinBASE.h
        /// </summary>
        private enum FileRights : uint
        {
            Read = 4,
            Write = 2,
            ReadWrite = Read + Write,
        }

        private IntPtr fileHandle, fileMap;

        public IntPtr Root
        {
            get { return fileMap; }
        }

        public SharedMemory(string name, bool existing, uint sizeInBytes)
        {
            if (existing)
                fileHandle = OpenFileMapping(FileRights.ReadWrite, false, name);
            else
                fileHandle = CreateFileMapping(NoFileHandle, 0,
                                                FileProtection.ReadWrite,
                                                0, sizeInBytes, name);
            if (fileHandle == IntPtr.Zero)
                throw new Exception("Open/create error: " + Marshal.GetLastWin32Error());

            // Obtain a read/write map for the entire file
            fileMap = MapViewOfFile(fileHandle, FileRights.ReadWrite, 0, 0, 0);

            if (fileMap == IntPtr.Zero)
                throw new Exception("MapViewOfFile error: " + Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Frees shared memory
        /// </summary>
        public void Dispose()
        {
            if (fileMap != IntPtr.Zero)
                UnmapViewOfFile(fileMap);
            if (fileHandle != IntPtr.Zero)
                CloseHandle(fileHandle);
            fileMap = fileHandle = IntPtr.Zero;
        }

        private static readonly IntPtr NoFileHandle = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFileMapping(
                                            IntPtr hFile,
                                            int lpAttributes,
                                            FileProtection flProtect,
                                            uint dwMaximumSizeHigh,
                                            uint dwMaximumSizeLow,
                                            string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenFileMapping(
                                            FileRights dwDesiredAccess,
                                            bool bInheritHandle,
                                            string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(
                                            IntPtr hFileMappingObject,
                                            FileRights dwDesiredAccess,
                                            uint dwFileOffsetHigh,
                                            uint dwFileOffsetLow,
                                            uint dwNumberOfBytesToMap);
        [DllImport("Kernel32.dll")]
        private static extern bool UnmapViewOfFile(IntPtr map);

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(IntPtr hObject);
    }
}

