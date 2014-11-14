/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Reflection;
using System.Threading;
using VPrinting.Colections;
using VPrinting;

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

        public static event ThreadExceptionEventHandler Error;

        public static void FireError(Exception ex)
        {
            if (Error != null)
                Error(typeof(Global), new ThreadExceptionEventArgs(ex));
        }

        public void Dispose()
        {
            using(LoadCompleted);
        }
    }
}
