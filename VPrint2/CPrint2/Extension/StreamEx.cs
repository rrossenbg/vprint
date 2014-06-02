/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using CPrint2.Colections;

namespace CPrint2
{
    public static class StreamEx
    {
        [TargetedPatchingOptOut("na")]
        public static T[] Copy<T>(this T[] arr, int length)
        {
            Debug.Assert(arr != null);
            Debug.Assert(arr.Length >= length);

            if (arr.Length == length)
                return arr;

            var newarr = new T[length];
            Array.Copy(arr, newarr, length);
            return newarr;
        }

        [TargetedPatchingOptOut("na")]
        public static void SlimCopy(this Stream stream, Action<byte[], int> copyFunct, int bufferSize = 16384)
        {
            Debug.Assert(stream != null);
            Debug.Assert(copyFunct != null);

            using (var buffer = new MemoryBuffer(bufferSize))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int read = 0;
                    while ((read = reader.Read(buffer.Buffer, 0, buffer.Size)) != 0)
                        copyFunct(buffer.Buffer, read);
                }
            }
        }
    }
}
