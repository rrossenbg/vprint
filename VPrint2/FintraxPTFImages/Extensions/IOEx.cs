/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.IO;
using System.Runtime;
using System.Diagnostics;
using System;

namespace FintraxPTFImages
{
    public static class IOEx
    {
        [TargetedPatchingOptOut("na")]
        public static DirectoryInfo Combine(this DirectoryInfo info, string subFolder)
        {
            Debug.Assert(info != null);
            return new DirectoryInfo(Path.Combine(info.FullName, subFolder));
        }

        [TargetedPatchingOptOut("na")]
        public static FileInfo CombineFileName(this DirectoryInfo info, string fileName)
        {
            Debug.Assert(info != null);
            return new FileInfo(Path.Combine(info.FullName, fileName));
        }

        [TargetedPatchingOptOut("na")]
        public static void DeleteSubFolders(this DirectoryInfo info, bool recursive = true)
        {
            Debug.Assert(info != null);
            foreach (var dir in info.GetDirectories())
                dir.Delete(recursive);
        }

        [TargetedPatchingOptOut("na")]
        public static bool Exists(this DirectoryInfo info)
        {
            Debug.Assert(info != null);
            info.Refresh();
            return info.Exists;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Empty(this DirectoryInfo info)
        {
            Debug.Assert(info != null);
            info.Refresh();
            return info.GetFiles().Length == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static void DeleteSafe(this DirectoryInfo info, bool subfolders = true)
        {
            if (info == null)
                return;
            try
            {
                info.Refresh();
                info.Delete(subfolders);
            }
            catch
            {
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void DeleteSafe(this FileInfo info)
        {
            if (info == null)
                return;
            try
            {
                info.Refresh();
                info.Delete();
            }
            catch
            {
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
                    f.DeleteSafe(true);
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
        public static string[] PathParts(this FileInfo info)
        {
            Debug.Assert(info != null);
            var dirName = Path.GetDirectoryName(info.FullName);
            return dirName.Split(Path.DirectorySeparatorChar);
        }

        [TargetedPatchingOptOut("na")]
        public static string[] PathParts(this DirectoryInfo info)
        {
            Debug.Assert(info != null);
            return info.FullName.Split(Path.DirectorySeparatorChar);
        }
    }
}