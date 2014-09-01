/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using VPrinting.Colections;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class FileInfoEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsReadOnly(this FileInfo info, int tries)
        {
            for (int i = 0; i < tries; i++)
            {
                try
                {
                    using (info.OpenRead()) ;
                    return false;
                }
                catch
                {
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
            return true;
        }

        [TargetedPatchingOptOut("na")]
        public static void Append(this FileInfo info, byte[] buffer)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            using (var file = info.OpenWrite())
            {
                file.Seek(0, SeekOrigin.End);
                file.Write(buffer, 0, buffer.Length);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsLocked(this FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    stream.Close();

                //file is not locked
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// SlimCopy function
        /// </summary>
        /// <param name="info"></param>
        /// <param name="copyFunct"></param>
        /// <param name="bufferSize"></param>
        /// <example>
        /// string FROMFILENAME = "C:\\LABEL3.rar";
        /// string FILENAME = "LABEL3.rar";
        /// int countryId = 826;
        /// int retailerId = 123;
        /// int voucherId = 12345567;
        ///
        /// ScanServiceClient client = new ScanServiceClient();
        /// client.Delete(FILENAME, countryId, retailerId, voucherId);
        ///
        /// var info = new FileInfo(FROMFILENAME);
        /// info.SlimCopy((data, read) => client.SaveData(FILENAME, countryId, retailerId, voucherId, data.Copy(read)));
        /// </example>
        [TargetedPatchingOptOut("na")]
        public static void SlimCopy(this FileInfo info, Action<byte[], int> copyFunct, int bufferSize = 16384)
        {
            Debug.Assert(info != null);
            Debug.Assert(copyFunct != null);

            using (var buffer = new MemoryBuffer(bufferSize))
            {
                using (var file = info.OpenRead())
                using (BinaryReader reader = new BinaryReader(file))
                {
                    int read = 0;
                    while ((read = reader.Read(buffer.Buffer, 0, buffer.Size)) != 0)
                        copyFunct(buffer.Buffer, read);
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void SlimCopyAsync(this FileInfo info, Action<byte[], long, int> copyFunct, int bufferSize = 16384)
        {
            Debug.Assert(info != null);
            Debug.Assert(copyFunct != null);

            using (var buffer = new MemoryBuffer(bufferSize))
            {
                using (var file = info.OpenRead())
                using (BinaryReader reader = new BinaryReader(file))
                {
                    var tasks = new List<Task>();
                    int read = 0;
                    while ((read = reader.Read(buffer.Buffer, 0, buffer.Size)) != 0)
                    {
                        long pos = reader.BaseStream.Position;
                        var t = Task.Factory.StartNew((o) =>
                        {
                            Tuple<byte[], long, int> da = (Tuple<byte[], long, int>)o;
                            copyFunct(da.Item1, da.Item2, da.Item3);
                        },
                        new Tuple<byte[], long, int>(buffer.Buffer, pos, read), TaskCreationOptions.AttachedToParent);
                        tasks.Add(t);
                    }
                    Task.WaitAll(tasks.ToArray());
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static bool Exists(this FileSystemInfo info, bool refresh = true)
        {
            Debug.Assert(info != null);
            if (refresh)
                info.Refresh();
            return info.Exists;
        }

        [TargetedPatchingOptOut("na")]
        public static bool DeleteSafe(this FileSystemInfo info)
        {
            Debug.Assert(info != null);
            try
            {
                info.Refresh();
                if (info.Exists)
                {
                    info.Delete();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ReadAllBytes(this FileInfo info)
        {
            Debug.Assert(info != null);
            return File.ReadAllBytes(info.FullName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ReadAllBytes(this FileInfo info, byte[] buffer, int count)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            using (var file = info.OpenRead())
                file.Read(buffer, 0, count);
        }

        [TargetedPatchingOptOut("na")]
        public static IList<T> Add<T>(this IList<T> list1, IList<T> list2)
        {
            List<T> list = new List<T>(list1);
            list.AddRange(list2);
            return list;
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo Rename(this FileInfo info, Func<FileInfo, string> func)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            if (func == null)
                throw new ArgumentNullException("func");

            var newname = func(info);
            return info.Directory.CombineFileName(string.Concat(newname, info.Extension));
        }
    }

    public static class DirectoryInfoEx
    {
        [TargetedPatchingOptOut("na")]
        public static FileInfo GetUnique(this DirectoryInfo info, string fileExt)
        {
            return new FileInfo(Path.Combine(info.FullName, Guid.NewGuid().ToString(), fileExt));
        }

        [TargetedPatchingOptOut("na")]
        public static void EnsureDirectory(this DirectoryInfo info)
        {
            if (!info.Exists)
                info.Create();
        }

        [TargetedPatchingOptOut("na")]
        public static void ClearSf(this DirectoryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (info.Exists)
            {
                try
                {
                    foreach (var file in info.GetFiles())
                        file.Delete();
                    foreach (var dir in info.GetDirectories())
                        dir.Delete(true);
                }
                catch
                {
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static DirectoryInfo Combine(this DirectoryInfo info, string subFolder)
        {
            return new DirectoryInfo(Path.Combine(info.FullName, subFolder));
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo CombineFileName(this DirectoryInfo info, string fileName)
        {
            return new FileInfo(Path.Combine(info.FullName, fileName));
        }
    }
}
