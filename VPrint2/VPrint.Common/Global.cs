/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Reflection;
using System.Threading;
using VPrinting.Colections;
using VPrinting.Extentions;

namespace VPrinting
{
    [Obfuscation(ApplyToMembers = true)]
    public class Global : IDisposable
    {
        public static readonly IgnoreList<string> IgnoreList = new IgnoreList<string>();

        private static readonly Global ms_instance = new Global();
        public static Global Instance
        {
            get { return ms_instance; }
        }

        #region FIELDS

        private int? m_parentId;

        public static int? FolderID
        {
            get
            {
                Thread.MemoryBarrier();
                return ms_instance.m_parentId;
            }
            set
            {
                ms_instance.m_parentId = value;
                Thread.MemoryBarrier();
            }
        }

        private bool m_ExitEvent;
        /// <summary>
        /// Signal Exit
        /// </summary>
        public bool ExitSignal
        {
            get
            {
                Thread.MemoryBarrier();
                return m_ExitEvent;
            }
            set
            {
                m_ExitEvent = value;
                Thread.MemoryBarrier();
            }
        }

        #endregion

        public readonly ManualResetEventSlim LoadCompleted = new ManualResetEventSlim(false);         

        public void Dispose()
        {
            LoadCompleted.DisposeSf();
        }

        //public void VersionUpdate(string currentVersion, Action onOk, Action onErr)
        //{
        //    Task.Factory.StartNew((o) =>
        //        {
        //            var data = (Tuple<Action, Action, string>)o;
        //            try
        //            {
        //                var versionDir = Global.GetAppSubFolder("VERSION");
        //                versionDir.ClearSf();

        //                var srv = ServiceDataAccess.Instance;
        //                var files = srv.GetVersionInfo(data.Item3);

        //                foreach (var file in files)
        //                {
        //                    long from = 0;

        //                    byte[] buffer = srv.ReadVersionFile(file.Name, from);

        //                    while (buffer != null)
        //                    {
        //                        var fileInfo = versionDir.CombineFileName(file.Name);
        //                        fileInfo.Append(buffer);
        //                        buffer = srv.ReadVersionFile(file.Name, from += buffer.Length);
        //                    }
        //                }

        //                data.Item1();
        //            }
        //            catch (Exception ex)
        //            {
        //                Program.OnThreadException(this, new ThreadExceptionEventArgs(ex));
        //                data.Item2();
        //            }
        //        },
        //        new Tuple<Action, Action, string>(onOk, onErr, currentVersion), TaskCreationOptions.LongRunning);
        //}
    }
}
