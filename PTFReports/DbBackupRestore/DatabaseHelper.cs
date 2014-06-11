/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace BackupRestore
{
    public class DatabaseHelper
    {
        public static void BackupDatabase(SqlConnectionStringBuilder csb, string destinationPath, int connectionTimeoutSecond)
        {
            ServerConnection connection = new ServerConnection(csb.DataSource, csb.UserID, csb.Password);
            connection.ConnectTimeout = connectionTimeoutSecond;
            connection.StatementTimeout = connectionTimeoutSecond;
            Server server = new Server(connection);
            Backup database = new Backup();
            database.Action = BackupActionType.Database;
            database.Database = csb.InitialCatalog;
            BackupDeviceItem device = new BackupDeviceItem(destinationPath, DeviceType.File);
            database.Devices.Add(device);
            database.SqlBackup(server);
            connection.Disconnect();
        }

        public static void RestoreDatabase(SqlConnectionStringBuilder csb, string sourcePath, int connectionTimeoutSecond)
        {
            ServerConnection connection = new ServerConnection(csb.DataSource, csb.UserID, csb.Password);
            connection.ConnectTimeout = connectionTimeoutSecond;
            connection.StatementTimeout = connectionTimeoutSecond;
            Server server = new Server(connection);
            Restore database = new Restore();
            database.Action = RestoreActionType.Database;
            database.Database = csb.InitialCatalog;
            BackupDeviceItem bkpDevice = new BackupDeviceItem(sourcePath, DeviceType.File);
            database.Devices.Add(bkpDevice);
            database.ReplaceDatabase = true;
            database.SqlRestore(server);
            //connection.Disconnect();
        }

        public static void BuildIndexes(SqlConnectionStringBuilder csb, string dbName, params TableInfo[] tables)
        {
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidArgumentException("dbName is missing");

            ServerConnection connection = new ServerConnection(csb.DataSource, csb.UserID, csb.Password);

            Server server = new Server(connection);
            // Reference the AdventureWorks2012 database. 
            Database database = server.Databases[dbName];

            foreach (var tab in tables)
            {
                Table table = database.Tables[tab.Name];

                // Define an Index object variable by providing the parent table and index name in the constructor. 
                Index idx = new Index(table, tab.Name + "_PrimaryKey");

                foreach (string name in tab.Values)
                {
                    // Add indexed columns to the index. 
                    IndexedColumn icol = new IndexedColumn(idx, name, true);
                    idx.IndexedColumns.Add(icol);
                }

                // Set the index properties. 
                idx.IndexKeyType = IndexKeyType.DriPrimaryKey;
                idx.IsClustered = false;
                idx.FillFactor = 90;
                // Create the index on the instance of SQL Server. 
                idx.Create();
                // Modify the page locks property. 
                idx.DisallowPageLocks = true;
                // Run the Alter method to make the change on the instance of SQL Server. 
                idx.Alter();
            }
        }
    }

    public class TableInfo : INamedList
    {
        public string Name { get; set; }
        public IList<string> Values { get; set; }
    }
}
