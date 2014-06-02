/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Data;

namespace PremierTaxFree.PTFLib.Data
{
    public delegate void SqlRefreshDelegate(SqlDataReader reader);

    /// <summary>
    /// Listen in MSSQL database for data changes
    /// The database should be prepared!
    /// (See EnableServiceBroker.sql in DB project)
    /// </summary>
    /// <example>
    /// SqlDataListener listener = new SqlDataListener(connectionString, "SELECT FileID FROM dbo.FILES;", 5);
    /// listener.Refresh += (r) => { Debug.WriteLine("Refresh!"); };
    /// listener.Start();
    /// .....
    /// listener.Dispose();
    /// </example>
    public class SqlDataListener : IDisposable
    {
        /// <summary>
        /// SqlDataReader with data changes.
        /// Do not do anything with the reader 
        /// if you wish no data.
        /// </summary>
        public event SqlRefreshDelegate Refresh;
        public event ThreadExceptionEventHandler Error;
        private Thread m_Thread;

        private string m_ConnectionString, m_Sql;
        private int m_Timeout = 5;
        private SqlDependency m_Dependency;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql">"select FileID from dbo.FILES"</param>
        /// <param name="timeOutSec">5</param>
        public SqlDataListener(string connectionString, string sql, int timeOutSec)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
            Debug.Assert(!string.IsNullOrEmpty(sql));

            m_ConnectionString = connectionString;
            m_Sql = sql;
            m_Timeout = timeOutSec;
        }

        public void Start()
        {
            m_Thread = new Thread(RefreshDataWithSqlDependency);
            m_Thread.IsBackground = true;
            m_Thread.Start();
            SqlDependency.Start(m_ConnectionString);
        }

        private void RefreshDataWithSqlDependency()
        {

            try
            {
                //Remove existing dependency, if necessary
                if (m_Dependency != null)
                {
                    m_Dependency.OnChange -= OnDependencyChange;
                    m_Dependency = null;
                }

                using (SqlConnection connection = new SqlConnection(m_ConnectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(m_Sql, connection);

                    // Create a dependency (class member) and associate it with the command.
                    m_Dependency = new SqlDependency(command, null, m_Timeout);

                    // Subscribe to the SqlDependency event.
                    m_Dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

                    // start dependency listener
                    SqlDependency.Start(m_ConnectionString);

                    // execute command and refresh data
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (Refresh != null)
                            Refresh(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
        }

        private void OnDependencyChange(object sender, SqlNotificationEventArgs args)
        {
            if ((args.Source == SqlNotificationSource.Data) || (args.Source == SqlNotificationSource.Timeout))
            {
                RefreshDataWithSqlDependency();
            }
            else
            {
                //Data not refreshed due to unexpected SqlNotificationEventArgs
            }
        }

        public void Dispose()
        {
            SqlDependency.Stop(m_ConnectionString);
        }
    }
}
