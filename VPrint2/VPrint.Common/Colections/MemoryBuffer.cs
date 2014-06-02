/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace VPrinting.Colections
{
    /// <summary>
    /// Caches byte arrays for later use
    /// </summary>
    /// <example>
    /// using (var buffer = new MemoryBuffer(bufferSize))
    /// {
    ///     using (var file = info.OpenRead())
    ///     using (BinaryReader reader = new BinaryReader(file))
    ///     {
    ///         int read = 0;
    ///         while ((read = reader.Read(buffer.Buffer, 0, buffer.Size)) != 0)
    ///             copyFunct(buffer.Buffer, read);
    ///     }
    /// }
    /// </example>
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public class MemoryBuffer : IDisposable
    {
        [Obfuscation]
        private static readonly Hashtable ms_MemoryBufferTable = Hashtable.Synchronized(new Hashtable());

        public int Size { get; private set; }
        public byte[] Buffer { get; private set; }

        public MemoryBuffer(int size)
        {
            Size = size;

            Queue queue = (Queue)ms_MemoryBufferTable[size];
            if (queue == null)
                ms_MemoryBufferTable[size] = queue = Queue.Synchronized(new Queue());

            if (queue.Count == 0)
                Buffer = new byte[size];
            else
                Buffer = (byte[])queue.Dequeue();
        }

        public void Dispose()
        {
            Queue queue = (Queue)ms_MemoryBufferTable[Size];
            Debug.Assert(queue != null);
            Array.Clear(Buffer, 0, Buffer.Length);
            queue.Enqueue(Buffer);
        }
    }
}