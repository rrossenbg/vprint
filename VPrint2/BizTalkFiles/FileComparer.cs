using System.Collections.Generic;
using System.IO;

namespace BizTalkFiles
{
    public class FileComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return x.LastWriteTime.CompareTo(y.LastWriteTime);
        }
    }
}
