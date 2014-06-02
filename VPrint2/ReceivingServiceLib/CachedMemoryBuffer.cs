/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;

namespace ReceivingServiceLib
{
    /// <summary>
    /// Caches byte arrays for later use
    /// </summary>
    /// <example>
    /// </example>
    public class CachedMemoryBuffer<T> : IDisposable
    {
        private static readonly Hashtable ms_MemoryBufferTable = Hashtable.Synchronized(new Hashtable());

        private int MaxSize { get; set; }

        public T Id { get; private set; }

        public byte[] Buffer
        {
            get
            {
                return (byte[])ms_MemoryBufferTable[Id];
            }
            set
            {
                ms_MemoryBufferTable[Id] = value;
            }
        }

        public bool IsFirstRun
        {
            get
            {
                return !ms_MemoryBufferTable.ContainsKey(Id);
            }
        }

        public CachedMemoryBuffer(T id)
        {
            Id = id;
        }

        public byte[] Get(int startFrom, int length)
        {
            var orgBuffer = Buffer;
            var len = Math.Min(orgBuffer.Length - startFrom, length);

            byte[] resultBuffer = new byte[len];
            Array.Copy(orgBuffer, startFrom, resultBuffer, 0, len);
            return resultBuffer;
        }

        public void Remove()
        {
            ms_MemoryBufferTable.Remove(Id);
        }

        public static void Clear()
        {
            ms_MemoryBufferTable.Clear();
        }

        public void Dispose()
        {
        }
    }
}