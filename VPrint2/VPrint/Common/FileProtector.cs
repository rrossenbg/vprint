/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VPrinting.Common
{
    public class FileProtector
    {
        public static ThreadExceptionEventHandler Error;

        public void Protect(FileInfo info)
        {
            var task = Task.Factory.StartNew((o) =>
            {
                FileInfo inf = (FileInfo)o;
                try
                {
                    if (inf != null && inf.Exists())
                    {
                        using (FileStream file = info.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var p = Process.Start(new ProcessStartInfo(info.FullName));
                            if (p != null)
                                p.WaitForExit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(this, new ThreadExceptionEventArgs(ex));
                }
            }, info);
        }
    }
}
