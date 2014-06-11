/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BackupRestore
{
    class Program
    {
        static void Main(string[] args)
        {
            bool completedOK = true;
            try
            {
                if (args.Contains("-help", "--help", "/help", "-?", "/?"))
                {
                    string help = Environment.NewLine +
                        @"  DbBackupRestore " + Environment.NewLine +
                        "================================================" + Environment.NewLine +
                        " Usage:" + Environment.NewLine +
                        " -nocopy, --nocopy, /nocopy - skip copying file" + Environment.NewLine +
                        " -norestore, --norestore, /norestore - skip restorying database" + Environment.NewLine +
                        " -noindex, --noindex, /noindex - skip indexing database" + Environment.NewLine +
                        " -help, --help, /help -? /? - show this help" + Environment.NewLine + Environment.NewLine +
                        "  Copyright (c) Premium Tax Free 2012" + Environment.NewLine;
                    Console.WriteLine(help);
                    return;
                }

                EventLogHelper.WriteInfo("Started at {0:g}", DateTime.Now);
                Console.WriteLine("Started at {0:g}", DateTime.Now);

                var connStr = ConfigurationManager.AppSettings["ConnectionString"];
                var conn = new SqlConnectionStringBuilder(connStr);

                var localFileName = ConfigurationManager.AppSettings["LocalFileName"];

                if (!args.Contains("-nocopy", "--nocopy", "/nocopy"))
                {
                    var remoteFileName = ConfigurationManager.AppSettings["RemoteFileName"];
                    var markRemoteFileName = ConfigurationManager.AppSettings["StartJobRemoteFileName"];

                    if (!File.Exists(markRemoteFileName))
                        throw new FileNotFoundException("Can not find start file.");

                    EventLogHelper.WriteInfo("Copy started at {0:g}", DateTime.Now);
                    Console.WriteLine("Copy started at {0:g}", DateTime.Now);

                    var started = Stopwatch.StartNew();
                    File.Copy(remoteFileName, localFileName, true);
                    File.Delete(markRemoteFileName);

                    EventLogHelper.WriteInfo("Copy succeed: {0:g}", started.Elapsed);
                    Console.WriteLine("Copy succeed: {0:g}", started.Elapsed);
                }

                if (!args.Contains("-norestore", "--norestore", "/norestore"))
                {
                    var timeoutHours = ConfigurationManager.AppSettings["ConnectionTimeoutHours"].Cast<int>();
                    var timeoutSeconds = TimeSpan.FromHours(timeoutHours).TotalSeconds.Cast<int>(); 

                    EventLogHelper.WriteInfo("Restore started at {0:g}", DateTime.Now);
                    Console.WriteLine("Restore started at {0:g}", DateTime.Now);

                    var started = Stopwatch.StartNew();
                    DatabaseHelper.RestoreDatabase(conn, localFileName, timeoutSeconds);

                    EventLogHelper.WriteInfo("Restore succeed: {0:g}", started.Elapsed);
                    Console.WriteLine("Restore succeed: {0:g}", started.Elapsed);
                }

                if (!args.Contains("-noindex", "--noindex", "/noindex"))
                {
                    var indexDatabaseName = ConfigurationManager.AppSettings["IndexDatabaseName"];
                    var indexTables = ConfigurationManager.AppSettings["IndexTables"];
                    var indexTableInfos = indexTables.Parse<TableInfo>(';', '-', ',');


                    EventLogHelper.WriteInfo("Index creating started at {0:g}", DateTime.Now);
                    Console.WriteLine("Index creating started at {0:g}", DateTime.Now);

                    var started = Stopwatch.StartNew();

                    DatabaseHelper.BuildIndexes(conn, indexDatabaseName, indexTableInfos.ToArray());

                    EventLogHelper.WriteInfo("Index creating succeed: {0:g}", started.Elapsed);
                    Console.WriteLine("Index creating succeed: {0:g}", started.Elapsed);
                }
            }
            catch (Exception ex)
            {
                completedOK = false;

                if (ex is FileNotFoundException)
                    EventLogHelper.WriteWarning(ex.Message);
                else
                    EventLogHelper.WriteError(ex);
                Console.WriteLine(ex);
                //Throw the exception to make the scheduler start this again
                throw;
            }
            finally
            {
                var status = completedOK ? "OK" : "Error";
                EventLogHelper.WriteInfo("Completed at {0:g}\t Status: {1}", DateTime.Now, status);
                Console.WriteLine("Completed at {0:g}\t Status: {1}", DateTime.Now, status);
            }
        }
    }
}
