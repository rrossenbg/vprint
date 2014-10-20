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
        static readonly DirectoryInfo st_appLocation;

        static FileInfoEx()
        {
            var asm = Assembly.GetEntryAssembly();
            st_appLocation = new FileInfo(asm.Location).Directory;
        }

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
        [Obfuscation]
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
        [Obfuscation]
        public static bool DeleteSafe(this FileInfo info)
        {
            try
            {
                if (info == null)
                    return false;

                info.Refresh();
                if (info.Exists)
                {
                    info.Delete();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
#if DEBUGGER
                Trace.WriteLine(ex, "ISRV");
#endif
                return false;
            }
        }
        
        [TargetedPatchingOptOut("na")]
        public static string[] PathParts(this FileInfo info)
        {
            Debug.Assert(info != null);
            var dirName = Path.GetDirectoryName(info.FullName);
            return dirName.Split(Path.DirectorySeparatorChar);
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
        public static FileInfo Rename(this FileInfo info, Func<FileInfo, string> func)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            if (func == null)
                throw new ArgumentNullException("func");

            var newname = func(info);
            return info.Directory.CombineFileName(string.Concat(newname, info.Extension));
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo App(this FileInfo info, string fileName)
        {
            return st_appLocation.CombineFileName(fileName);
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo Temp(this FileInfo info, string ext = ".jpg")
        {
            var file2 = new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), ext));
            return file2;
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo Temp2(this FileInfo info, string fileName)
        {
            var file2 = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));
            return file2;
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo IfDebug(this FileInfo info, string debugPath)
        {
#if DEBUG
            var info2 = new FileInfo(debugPath);
            return info2;
#else
            return info;
#endif
        }

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static DirectoryInfo Combine(this DirectoryInfo info, string subFolder)
        //{
        //    return new DirectoryInfo(Path.Combine(info.FullName, subFolder));
        //}

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static FileInfo CombineFileName(this DirectoryInfo info, string fileName)
        //{
        //    return new FileInfo(Path.Combine(info.FullName, fileName));
        //}

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void WriteAllBytes(this FileInfo file, byte[] bytes, int length = 0)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length == 0)
                throw new ArithmeticException("Buffer is empty");

            using (var stream = file.OpenWrite())
                stream.Write(bytes, 0, ((length > 0) ? length : bytes.Length));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ReadAllText(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            using (var reader = file.OpenText())
                return reader.ReadToEnd();
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetFileName(this FileInfo info)
        {
            Debug.Assert(info != null);
            return Path.GetFileNameWithoutExtension(info.Name);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetFileNameWithoutExtension(this FileInfo info)
        {
            Debug.Assert(info != null);
            return Path.GetFileNameWithoutExtension(info.FullName);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void Read(this FileInfo file, int from, int length, byte[] buffer)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (file.Length - from < 0)
                throw new ArgumentOutOfRangeException("from");

            using (var reader = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                reader.Seek(from, SeekOrigin.Begin);
                reader.Read(buffer, 0, length);
            }
        }
    }
}
