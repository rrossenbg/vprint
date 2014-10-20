/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class DirectoryInfoEx
    {
        [TargetedPatchingOptOut("na")]
        public static void DeleteSubFolders(this DirectoryInfo info, bool recursive = true)
        {
            Debug.Assert(info != null);
            foreach (var dir in info.GetDirectories())
                dir.Delete(recursive);
        }

        [TargetedPatchingOptOut("na")]
        public static void DeleteSubFoldersSafe(this DirectoryInfo info, bool recursive = true)
        {
            Debug.Assert(info != null);
            try
            {
                foreach (var dir in info.GetDirectories())
                {
                    try
                    {
                        dir.Delete(recursive);
                    }
                    catch (Exception ex1)
                    {
                        Debug.WriteLine(ex1);
                    }
                }
            }
            catch (Exception ex2)
            {
                Debug.WriteLine(ex2);
            }
        }

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
        public static void ClearSafe(this DirectoryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (info.Exists)
            {
                try
                {
                    foreach (var file in info.GetFiles())
                        file.DeleteSafe();
                    foreach (var dir in info.GetDirectories())
                        dir.DeleteSafe();
                }
                catch
                {
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ClearSafe(this DirectoryInfo info, DateTime date)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            foreach (var f in info.GetFiles())
                if (f.CreationTime < date)
                    f.DeleteSafe();

            foreach (var f in info.GetDirectories())
                if (f.CreationTime < date)
                    f.DeleteSafe();
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
        public static string[] PathParts(this DirectoryInfo info)
        {
            Debug.Assert(info != null);
            return info.FullName.Split(Path.DirectorySeparatorChar);
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
    }
}
