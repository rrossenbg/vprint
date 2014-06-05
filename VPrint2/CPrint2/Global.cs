/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Threading;

namespace CPrint2
{
    public class Global : IDisposable
    {
        private static readonly Global ms_instance = new Global();
        public static Global Instance
        {
            get { return ms_instance; }
        }

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

        public readonly ManualResetEventSlim LoadCompleted = new ManualResetEventSlim(false);

        public void Dispose()
        {
            LoadCompleted.DisposeSf();
        }
    }
}
