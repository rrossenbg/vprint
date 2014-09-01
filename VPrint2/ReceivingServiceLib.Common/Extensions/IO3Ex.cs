/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Reflection;

namespace VPrinting
{
    public static class IOEx
    {
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void EnsureDirectory(this DirectoryInfo info)
        {
            if (!info.Exists)
                info.Create();
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static DirectoryInfo ThrowIfNotExist(this DirectoryInfo info, string message)
        {
            Debug.Assert(info != null);

            info.Refresh();
            if (!info.Exists)
                throw new Exception(message);
            return info;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static DirectoryInfo CreateIfNotExist(this DirectoryInfo info)
        {
            Debug.Assert(info != null);

            info.Refresh();
            if (!info.Exists)
                info.Create();
            return info;
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsEmpty(this DirectoryInfo info, string filter = "*")
        {
            Debug.Assert(info != null);
            info.Refresh();
            Debug.Assert(info.Exists);
            var infos = info.GetFiles(filter);
            return infos.Length == 0;
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
        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length == 0)
                throw new ArithmeticException("Buffer is empty");

            using (var stream = file.OpenWrite())
                stream.Write(bytes, 0, bytes.Length);
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
        public static byte[] ReadAllBytes(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            return File.ReadAllBytes(file.FullName);
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
        public static void MoveTo(this DirectoryInfo fromInfo, DirectoryInfo toInfo)
        {
            Debug.Assert(fromInfo != null);
            Debug.Assert(fromInfo.Exists);

            Debug.Assert(toInfo != null);
            Debug.Assert(toInfo.Exists);

            foreach (var file in fromInfo.GetFiles())
            {
                var newFile = toInfo.CombineFileName(file.Name);
                file.MoveTo(newFile.FullName);
            }

            fromInfo.Delete(true);
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

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool EqualNoCase(this string str, string str2)
        {
            return string.Compare(str, str2, true) == 0;
        }
    }
}
