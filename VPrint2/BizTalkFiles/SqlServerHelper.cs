
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace BizTalkFiles
{
    public class SqlServerHelper
    {
        /// <summary>
        /// 100
        /// </summary>
        const int MAX_ACTIVE_FILES = 100;

        public static string ConnectionString { get; set; }
        public static string OrechestrationName { get; set; }

        static SqlServerHelper()
        {
            ConnectionString = "Data Source=FIN-IE-PA017;Initial Catalog=BizTalkMgmtDb;Integrated Security=True;";
            OrechestrationName = "CommonOrchestration";
        }

        public static bool IsBizTalkReady(int dehydratedFiles, int MaxBizTalkActiveFilesCount = MAX_ACTIVE_FILES)
        {
            const string sql = @"SET TRANSACTION ISOLATION LEVEL READ COMMITTED
                                SET DEADLOCK_PRIORITY LOW
                                SELECT  o.nvcName AS Orchestration, COUNT(*) as Count,
                                      CASE i.nState
                                            WHEN 1 THEN 'Ready To Run'
                                            WHEN 2 THEN 'Active'
                                            WHEN 4 THEN 'Suspended Resumable'
                                            WHEN 8 THEN 'Dehydrated'
                                            WHEN 16 THEN 'Completed With Discarded Messages'            
                                            WHEN 32 THEN 'Suspended Non-Resumable'
                                      END as State
                                FROM [BizTalkMsgboxDb]..[Instances] AS i WITH (NOLOCK) 
                                JOIN [BizTalkMgmtDb]..[bts_Orchestration] AS o WITH (NOLOCK) ON i.uidServiceID = o.uidGUID
                                --WHERE dtCreated > '2004-08-24 00:00:00' AND dtCreated < '2004-08-24 13:30:00'  
                                GROUP BY o.nvcName, i.nState";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand com = new SqlCommand(sql, conn))
                    {
                        var reader = com.ExecuteReader(CommandBehavior.CloseConnection);

                        while (reader.Read())
                        {
                            string colName = Convert.ToString(reader[0]);

                            string colStatus = Convert.ToString(reader[2]);

                            if (colName.CompareNoCase(OrechestrationName))
                            {
                                if (colStatus.CompareNoCase("Ready To Run"))
                                    return false;

                                if (colStatus.CompareNoCase("Dehydrated") && Convert.ToInt32(reader[1]) > dehydratedFiles)
                                    return false;

                                if (colStatus.CompareNoCase("Active") && Convert.ToInt32(reader[1]) > MaxBizTalkActiveFilesCount)
                                    return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
    }
}

