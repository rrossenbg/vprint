/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;

using PremierTaxFree.PTFLib.Threading;

namespace PremierTaxFree.PTFLib.Data
{
    /// <summary>
    /// Sql background queue
    /// </summary>
    /// <example>
    ///  SQLWorker.Default.Start(ThreadPriority.AboveNormal, "SQLWorkerThread");
    ///  SQLWorker.Default.Add(comm, new ThreadExceptionEventHandler((s, e) =>
    ///             {
    ///                 throw new ApplicationException("Cannot run this command.");
    ///             }));
    ///  SQLWorker.Default.Empty.WaitOne();
    ///  SQLWorker.Default.Stop();
    /// </example>
    public class SQLWorker : CycleWorkerBase
    {
        /// <summary>
        /// Time for thread spleep before check for new work
        /// </summary>
        public static TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Time for thread to sleep after it has finished once cycle
        /// </summary>
        public static TimeSpan DEFAULT_SLEEPTIME = TimeSpan.FromMilliseconds(100);

        public class CommandInfo : IDisposable
        {
            public IDbCommand Command { get; private set; }
            public event EventHandler Success;
            public event ThreadExceptionEventHandler Error;
            public bool HandlesError
            {
                get
                {
                    return Error != null;
                }
            }

            public CommandInfo(IDbCommand comm)
            {
                Command = comm;
            }

            public void FireSuccess()
            {
                if (Success != null)
                    Success(this, EventArgs.Empty);
            }

            public void FireError(Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }

            public void Dispose()
            {
                Success.RemoveAll();
                Error.RemoveAll();
            }
        }

        private static SQLWorker instance = new SQLWorker();
        public static SQLWorker Default
        {
            get { return instance; }
        }

        private readonly ArrayList m_Queue = ArrayList.Synchronized(new ArrayList());

        /// <summary>
        /// Indicates the command queue is empty.
        /// </summary>
        /// <example>SQLWorker.Default.Empty.WaitOne();</example>
        public readonly AutoResetEvent Empty = new AutoResetEvent(false);

        private SQLWorker()
        {
            base.SleepTime = DEFAULT_SLEEPTIME;
        }

        ~SQLWorker()
        {
            Empty.Close();
        }

        public void Add(IDbCommand command)
        {
            Add(command, null, null);
        }

        public void Add(IDbCommand command, ThreadExceptionEventHandler error)
        {
            Add(command, null, error);
        }

        public void Add(IDbCommand command, EventHandler success, ThreadExceptionEventHandler error)
        {
            Debug.Assert(m_Worker != null, "Worker is not started");
            Debug.Assert(command != null);
            Debug.Assert(command.Connection != null);

            lock (this)
            {
                CommandInfo info = new CommandInfo(command);
                if (success != null)
                    info.Success += success;
                if (error != null)
                    info.Error += error;
                m_Queue.Add(info);
                Monitor.Pulse(this);
            }
        }

        public void Clear()
        {
            m_Queue.Clear();
        }

        protected override void ThreadFunction()
        {
            CommandInfo cmd = null;
            try
            {
                Empty.Reset();

                lock (this)
                {
                    while (m_Queue.Count == 0)
                    {
                        Empty.Set();
                        Monitor.Wait(this, DEFAULT_TIMEOUT);
                    }
                }

                cmd = (CommandInfo)m_Queue[0];
                m_Queue.RemoveAt(0);

                if (cmd != null)
                    SQL.ExecuteNonQuery(cmd.Command);
                cmd.FireSuccess();
            }
            catch (ThreadAbortException)
            {
                Debug.Assert(m_Queue.Count == 0, "Queue is not empty. Call Empty.WaitOne() first.");
                //No problem 
                //exit signal
            }
            catch (ThreadInterruptedException)
            {
                Debug.Assert(m_Queue.Count == 0, "Queue is not empty. Call Empty.WaitOne() first.");
                //No problem 
                //join signal
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "SQLWorkerThread");

                if (cmd == null)
                    return;

                if (cmd.HandlesError)
                {
                    cmd.FireError(ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                cmd.DisposeSf();
            }
        }
    }
}
