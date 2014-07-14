/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.IO;
using System.Runtime;
using System.Diagnostics;

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