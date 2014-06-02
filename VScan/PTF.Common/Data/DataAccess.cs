/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using PremierTaxFree.Data.Objects;
using PremierTaxFree.PTFLib.Data.Objects;
using PremierTaxFree.PTFLib.Data.Objects.Server;
using PremierTaxFree.PTFLib.Security;
using PremierTaxFree.PTFLib.Sys;

namespace PremierTaxFree.PTFLib.Data
{
    /// <summary>
    /// Central Server Data Access
    /// </summary>
    public static class ServerDataAccess
    {
        /// <summary>
        /// Server connection string
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// PTF database connection string
        /// </summary>
        public static string ConnectionStringPft { get; set; }

        /// <summary>
        /// Gets all database names
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDatabases()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                DataTable databases = conn.GetSchema("Databases");
                return databases;
            }
        }

        /// <summary>
        /// Tests the server connection. Returns null or error message.
        /// </summary>
        /// <returns></returns>
        public static string TestConnection()
        {
            return new SqlConnection(ConnectionString).TestSf();
        }

        #region COUNTRIES

        /// <summary>
        /// PTF Command. Calls PTF database to read all ptf countries
        /// </summary>
        /// <returns></returns>
        public static List<DbCountry> SelectCountries()
        {
            string sql = "SELECT iso_number, iso_2, iso_country FROM [ISO] WHERE iso_ptf='Y';";
            List<DbCountry> list = MSSQL.ExecuteReader<DbCountry>(ConnectionStringPft, CommandType.Text, sql);
            return list;
        }

        #endregion

        #region FILES

        public static void InsertFile(int clientId, int countryId, string siteCode)
        {
            SQL.ExecuteNonQuery(
                CreateInsertFileCommand(clientId, countryId, siteCode, DateTime.Now));
        }

        /// <summary>
        /// Inserts file synchronously.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        /// <param name="countryCode"></param>
        /// <param name="fileID"></param>
        /// <param name="relatedID"></param>
        /// <param name="siteCode"></param>
        /// <param name="comment"></param>
        /// <param name="retailerID"></param>
        /// <param name="voucherID"></param>
        /// <param name="voucherImage"></param>
        /// <param name="barCodeImage"></param>
        /// <param name="voucherData"></param>
        /// <param name="dateScanned"></param>
        /// <returns></returns>
        public static int UpdateFile(
            int clientID,
            int countryID,
            int retailerID,
            string voucherID,
            string siteCode,
            byte[] voucherImage,
            byte[] barCodeImage,
            string comment,
            DateTime dateScanned)
        {
            return SQL.ExecuteNonQuery(
                CreateUpdateFileCommand(
                clientID,
                countryID,
                retailerID,
                voucherID,
                siteCode,
                voucherImage,
                barCodeImage,
                comment,
                dateScanned));
        }

        /// <summary>
        /// Inserts file asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        /// <param name="countryCode"></param>
        /// <param name="fileID"></param>
        /// <param name="relatedID"></param>
        /// <param name="siteCode"></param>
        /// <param name="comment"></param>
        /// <param name="retailerID"></param>
        /// <param name="voucherID"></param>
        /// <param name="voucherImage"></param>
        /// <param name="barCodeImage"></param>
        /// <param name="voucherData"></param>
        /// <param name="dateScanned"></param>
        /// <param name="error"></param>
        public static void UpdateFileAsync(
            int clientID,
            int countryID,
            int retailerID,
            string voucherID,
            string siteCode,
            string comment,
            byte[] voucherImage,
            byte[] barCodeImage,
            DateTime dateScanned,
            ThreadExceptionEventHandler error)
        {
            SQLWorker.Default.Add(
            CreateUpdateFileCommand(
                    clientID,
                    countryID,
                    retailerID,
                    voucherID,
                    siteCode,
                    voucherImage,
                    barCodeImage,
                    comment,
                    dateScanned),
            error);
        }

        private static SqlCommand CreateInsertFileCommand(
                int clientID,
                int countryID,
                string siteCode,
                DateTime dateAllocated)
        {
            Debug.Assert(!string.IsNullOrEmpty(ConnectionString));

            const string SQL = @"INSERT FILES( ClientID, CountryID, SiteCode, DateAllocated )
                                VALUES ( @ClientID, @CountryID, @SiteCode, @DateAllocated );";

            SqlCommand command = MSSQL.CreateCommand(ConnectionString,
                        SQL,
                        CommandType.Text,
                        new SqlParameter("@ClientID", clientID),
                        new SqlParameter("@CountryID", countryID),
                        new SqlParameter("@SiteCode", siteCode),
                        new SqlParameter("@DateAllocated", dateAllocated));

            return command;
        }

        /// <summary>
        /// Creates command object that insert file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        /// <param name="countryCode"></param>
        /// <param name="fileID"></param>
        /// <param name="relatedId"></param>
        /// <param name="siteCode"></param>
        /// <param name="comment"></param>
        /// <param name="retailerID"></param>
        /// <param name="voucherID"></param>
        /// <param name="voucherImage"></param>
        /// <param name="barCodeImage"></param>
        /// <param name="voucherData"></param>
        /// <param name="dateScanned"></param>
        /// <returns></returns>
        private static SqlCommand CreateUpdateFileCommand(
            int clientID,
            int countryID,
            int retailerID,
            string voucherID,
            string siteCode,
            byte[] voucherImage, 
            byte[] barCodeImage, 
            string comment,
            DateTime dateScanned)
        {
            Debug.Assert(!string.IsNullOrEmpty(ConnectionString));

            const string SQL = @"UPDATE FILES 
                                SET ClientID = @ClientID,
                                    RetailerID = @RetailerID,
                                    VoucherID = @VoucherID, 
                                    VoucherImage = @VoucherImage, 
                                    BarCodeImage = @BarCodeImage, 
                                    DateScanned = @DateScanned,
                                    Comment = @Comment
                                WHERE SiteCode = @SiteCode AND CountryID = @CountryID";

            SqlCommand command = MSSQL.CreateCommand(ConnectionString,
                        SQL,
                        CommandType.Text,
                        new SqlParameter("@ClientID", clientID),
                        new SqlParameter("@CountryID", countryID),
                        new SqlParameter("@RetailerID", retailerID),
                        new SqlParameter("@VoucherID", voucherID),
                        new SqlParameter("@SiteCode", siteCode),
                        new SqlParameter("@VoucherImage", voucherImage),
                        new SqlParameter("@BarCodeImage", barCodeImage),
                        new SqlParameter("@DateScanned", dateScanned),
                        new SqlParameter("@Comment", comment));
            
            return command;
        }

        /// <summary>
        /// Inserts file by serializetion data saved in a hashtable
        /// </summary>
        /// <param name="serializationData"></param>
        /// <param name="error"></param>
        public static void InsertFileAsync(Hashtable serializationData, ThreadExceptionEventHandler error)
        {
            SqlCommand command = MSSQL.CreateCommand(ConnectionString, serializationData);
            SQLWorker.Default.Add(command, error);
        }

        /// <summary>
        /// Deletes files by batch id
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        public static void DeleteFilesByBatchID(int clientID, Guid batchID)
        {
            SQL.ExecuteNonQuery(CreateDeleteFilesByBatchIDCommand(clientID, batchID));
        }

        /// <summary>
        /// Deletes files by batch id asynchronously
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        public static void DeleteFilesByBatchIDAsync(int clientID, Guid batchID)
        {
            SQLWorker.Default.Add(CreateDeleteFilesByBatchIDCommand(clientID, batchID));
        }

        /// <summary>
        /// Creates a command for deleting files by batch id
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="batchID"></param>
        /// <returns></returns>
        private static SqlCommand CreateDeleteFilesByBatchIDCommand(int clientID, Guid batchID)
        {
            const string sql = "DELETE FILES WHERE ClientID = @ClientID AND BatchID = @BatchID;";
            var cmd = MSSQL.CreateCommand(ConnectionString, sql, CommandType.Text,
                new SqlParameter("@ClientID", clientID),
                new SqlParameter("@BatchID", batchID));
            return cmd;
        }

        #endregion

        #region MESSAGES

        /// <summary>
        /// Insert message to the database. Do not fire errors.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="stacktrase"></param>
        /// <param name="onServer"></param>
        public static void InsertMessage(string message, int clientID, eSources source, eMessageTypes type, string stackTrase, DateTime dateGenerated)
        {
            // Do not refire errors
            SQL.ExecuteNonQuery(CreateInsertMessage(message, clientID, source, type, stackTrase, dateGenerated));
        }

        /// <summary>
        /// Insert message to the database. Do not fire errors.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="stacktrase"></param>
        /// <param name="onServer"></param>
        public static void InsertMessageAsync(string message, int clientID, eSources source, eMessageTypes type, string stackTrase, DateTime dateGenerated)
        {
            // Do not refire errors
            SQLWorker.Default.Add(CreateInsertMessage(message, clientID, source, type, stackTrase, dateGenerated), 
                //No Errors!
                PTFUtils.EmptyThreadExceptionEventHandler());
        }

        /// <summary>
        /// Creates a command for inserting message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="stacktrase"></param>
        /// <param name="onServer"></param>
        /// <returns></returns>
        private static SqlCommand CreateInsertMessage(string message, int clientID,
            eSources source, eMessageTypes type,
            string stackTrace, DateTime dateGenerated)
        {
            const string sql = "spInsertMessage";
            return MSSQL.CreateCommand(ConnectionString,
                sql,
                CommandType.StoredProcedure,
                new SqlParameter("@ClientID", clientID),
                new SqlParameter("@Message", message),
                new SqlParameter("@Type", Convert.ToInt32(type)),
                new SqlParameter("@SourceID", Convert.ToInt32(source)),
                new SqlParameter("@StackTrace", stackTrace),
                new SqlParameter("@DateGenerated", dateGenerated));
        }

        #endregion

        #region CONFIG

        /// <summary>
        /// Select config value
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<DbConfigInfo> SelectConfig(IDictionary dict, eSources source)
        {
            const string sql = "SELECT Key, Value FROM CONFIG WHERE SourceID = @SourceID;";
            IList<DbConfigInfo> list = MSSQL.ExecuteReader<DbConfigInfo>(ConnectionString, sql,
                new SqlParameter("@SourceID", Convert.ToInt32(source)));
            return list;
        }

        /// <summary>
        /// Update config value
        /// </summary>
        /// <param name="dict"></param>
        //public static void UpdateConfig(IDictionary dict)
        //{
        //    Thread.BeginCriticalRegion();
        //    try
        //    {
        //        DataSchemaServer.CONFIGDataTable config = new DataSchemaServer.CONFIGDataTable();
        //        foreach (var key in dict.Keys)
        //        {
        //            config.AddCONFIGRow(Convert.ToString(key), dict[key]);
        //        }

        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            MSSQL.ExecuteNonQuery(ConnectionString, "TRUNCATE TABLE CONFIG;");
        //            MSSQL.ExecuteTable(ConnectionString, "CONFIG", config);
        //            scope.Complete();
        //        }
        //    }
        //    finally
        //    {
        //        Thread.EndCriticalRegion();
        //    }
        //}

        #endregion

        #region CLIENTS

        /// <summary>
        /// Creates a client in server database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        public static void CreateClient(string name, string ip, out int id)
        {
            var comm = CreateCreateClientCommand(name.TrimSafe().UpperSafe(), ip.TrimSafe().UpperSafe());
            SQL.ExecuteNonQuery(comm);
            id = comm.Parameters["@ID"].Value.Cast<int>();
        }

        /// <summary>
        /// Creates a client asynchronously
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        public static void CreateClientAsync(string name, string ip)
        {
            SQLWorker.Default.Add(CreateCreateClientCommand(name, ip));
        }

        /// <summary>
        /// Creates a command for creating client in server database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static SqlCommand CreateCreateClientCommand(string name, string ip)
        {
            return MSSQL.CreateCommand(ConnectionString, "spCreateClient", CommandType.StoredProcedure,
                new SqlParameter("@Name", name),
                new SqlParameter("@IP", ip),
                new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output });
        }

        /// <summary>
        /// Select enabled clients from Client table
        /// </summary>
        /// <param name="clients"></param>
        public static void SelectClients(IDictionary clients, bool active)
        {
            lock (clients.SyncRoot)
            {
                clients.Clear();

                string sql = "SELECT ClientID, IP FROM CLIENTS".concat((active ? " WHERE Enabled = 1;" : ";"));
                List<DbClientInfo> clientList = MSSQL.ExecuteReader<DbClientInfo>(ConnectionString, sql);

                clients[Strings.Empty2] = Strings.Empty2;

                foreach (var info in clientList)
                {
                    Trace.WriteLine(info, "ServerDataAccess.SelectClients");
                    clients[info.IP] = info.ClientID;
                }
            }
        }

        /// <summary>
        /// Updates client on the server (Enable/Disable)
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="active"></param>
        public static void UpdateClient(int clientId, bool active)
        {
            SQL.ExecuteNonQuery(CreateUpdateClientCommand(clientId, active));
        }

        /// <summary>
        /// Updates client on the server (Enable/Disable) asynchronously
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="active"></param>
        public static void UpdateClientAsync(int clientId, bool active)
        {
            SQLWorker.Default.Add(CreateUpdateClientCommand(clientId, active));
        }

        /// <summary>
        /// Creates command for updating client on the server (Enable/Disable)
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public static SqlCommand CreateUpdateClientCommand(int clientId, bool active)
        {
            string sql = "UPDATE CLIENTS SET Enabled = {1} WHERE ClientID = {0};".format(clientId, Convert.ToInt32(active));
            var cmd = MSSQL.CreateCommand(ConnectionString, sql);
            return cmd;
        }

        /// <summary>
        /// Selects the last inserted voucher on the server
        /// </summary>
        /// <returns></returns>
        public static DbVoucher SelectLastInserted()
        {
            const string sql =
            "SELECT FileID, ClientID, CountryID, RetailerID, VoucherID, SiteCode, Comment, " +
            " VoucherImage, BarCodeImage, DateAllocated, DateScanned, DateInserted FROM FILES " +
            " WHERE FileID = (SELECT MAX(FileID) FROM FILES WHERE VoucherImage IS NOT NULL);";
            var reader = MSSQL.ExecuteReader<DbVoucher>(ConnectionString, sql);
            return reader.FirstOrDefault();
        }

        #endregion

        #region USERS

        /// <summary>
        /// Validates client agains PTF database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="countryId"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="encryptedPassword"></param>
        /// <returns></returns>
        public static bool ValidateUser(DataPair<string,string> key, int countryId, string userName, string password, 
            ref string encryptedPassword)
        {
            List<DbPasswordInfo> psw = MSSQL.ExecuteReader<DbPasswordInfo>(ConnectionStringPft,
                    CommandType.StoredProcedure, "GetUserLoginDetail",
                    new SqlParameter("@CountryId", countryId),
                    new SqlParameter("@UserName", userName)
                );

            if (psw.Count == 0)
                return false;

            if (string.IsNullOrEmpty(psw[0].Salt))
            {
                return (psw[0].EncryptedPassword == password);
            }
            else
            {
                RijndaelCryptography rc = new RijndaelCryptography();
                rc.Key = Encoding.ASCII.GetBytes(key.Key);
                rc.IV = Encoding.ASCII.GetBytes(key.Value);
                rc.Encrypt(password + psw[0].Salt);
                encryptedPassword = HttpUtility.UrlEncode(Convert.ToBase64String(rc.Encrypted));
                return (psw[0].EncryptedPassword == encryptedPassword);
            }
        }

        /// <summary>
        /// Select users from PTF Users table
        /// PTF database table
        /// </summary>
        /// <param name="userTable"></param>
        public static void SelectUsers(IDictionary userTable)
        {
            Debug.Assert(userTable != null);

            lock (userTable.SyncRoot)
            {
                userTable.Clear();

                const string sql = "SELECT us_id, us_login, us_password FROM Users WHERE us_active = 'Y';";
                List<DbUserInfo> users = MSSQL.ExecuteReader<DbUserInfo>(ConnectionStringPft, sql);
                userTable[Strings.Rosen] = Strings.Rosen;

                foreach (var user in users)
                    userTable[user.Name] = user.Password;
            }
        }

        #endregion

        #region SITEIDS

        /// <summary>
        /// Selects (allocates) image file ids into server database.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="count"></param>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        public static void SelectAuditIDS(int clientId, int count, out int fromId, out int toId)
        {
            const string sql = "spGenerateAuditIDs";
            SqlParameter
                pFrom = new SqlParameter("@AuditIDFrom", SqlDbType.Int) { Direction = ParameterDirection.Output },
                pTo = new SqlParameter("@AuditIDTo", SqlDbType.Int) { Direction = ParameterDirection.Output };

            SQL.ExecuteNonQuery(MSSQL.CreateCommand(ConnectionString, 
                sql, CommandType.StoredProcedure, 
                new SqlParameter("@ClientID", clientId),
                new SqlParameter("@Count", count),
                pFrom, pTo));

            fromId = Convert.ToInt32(pFrom.Value);
            toId = Convert.ToInt32(pTo.Value);
        }

        #endregion
    }

    /// <summary>
    /// Remote Client Data Access
    /// </summary>
    public static class ClientDataAccess
    {
        /// <summary>
        /// Use master database only with installer as local administrator
        /// </summary>
        public static string MasterConnectionString;

        /// <summary>
        /// Use local database as restricted user
        /// </summary>
        public static string ConnectionString;

        /// <summary>
        /// Gets all databses on the client
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDatabases()
        {
            using (SqlConnection conn = new SqlConnection(MasterConnectionString))
            {
                conn.Open();
                DataTable databases = conn.GetSchema("Databases");
                return databases;
            }
        }

        /// <summary>
        /// Setup database by osql tools
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="datasource"></param>
        /// <param name="sqlBatchText"></param>
        public static void SetupDatabase(string directoryPath, string datasource, string sqlBatchText)
        {
            if (!Directory.Exists(directoryPath))
                 Directory.CreateDirectory(directoryPath);
            string path = Path.GetTempFileName();
            File.WriteAllText(path, sqlBatchText);
            Trace.WriteLine(sqlBatchText);
            MSSQL.ExecuteSqlFile(datasource, path, TimeSpan.FromMinutes(3).TotalMilliseconds.Cast<int>());
            File.Delete(path);
        }

        /// <summary>
        /// Setup database by C# code
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="sqlBatchText"></param>
        public static void SetupDatabase(string directoryPath, string sqlBatchText)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string[] sqlArray = sqlBatchText.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            string connString = null;

            foreach (string sql in sqlArray)
            {
                try
                {
                    string tsql = sql.Trim();

                    if (string.IsNullOrEmpty(tsql))
                        continue;

                    if (tsql.IndexOf("USE [master]", StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        connString = MasterConnectionString;
                        continue;
                    }
                    else if (tsql.IndexOf("USE [PTFLocal]", StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        connString = ConnectionString;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(connString))
                    {
                        Trace.WriteLine(tsql, "ClientDataAccess.SetupDatabase");
                        Trace.WriteLine("----------->>");
                        MSSQL.ExecuteNonQuery(connString, tsql);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Drops client database safely. Deletes database file as well.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string DropDatabaseSafe(string path)
        {
            try
            {
                MSSQL.ExecuteNonQuery(MasterConnectionString, "drop database PTFLocal;");
                if (!string.IsNullOrEmpty(path))
                    OS.DeleteFolder(path);
                return null;
            }
            catch (Exception ex)
            {
                //Save operation
                //Do nothing.
                return ex.Message;
            }
        }

        /// <summary>
        /// Save test sql connection. Returns null in case of success.
        /// </summary>
        /// <returns>
        /// Returns null on success or 
        /// exception message on failure.
        /// </returns>
        public static string TestConnection()
        {
            return new SqlConnection(ConnectionString).TestSf();
        }

        #region INSERT/UPDATE FILES

        /// <summary>
        /// Inserts empty records into client client database, files table
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void InsertFileAsync(int countryId, string[] siteCodes, ThreadExceptionEventHandler error = null)
        {
            foreach (var siteCode in siteCodes)
                SQLWorker.Default.Add(CreateInsertFileCommand(countryId, siteCode), null, error);
        }

        /// <summary>
        /// Creates an insert fileid command
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public static SqlCommand CreateInsertFileCommand(int countryId, string siteCode)
        {
            if (string.IsNullOrWhiteSpace(siteCode))
                throw new ArgumentNullException("siteCode");

            const string sql = "INSERT FILES( SiteCode, CountryID, DateAllocated) VALUES (@SiteCode, @CountryID, @DateAllocated);";
            SqlCommand command = MSSQL.CreateCommand(ConnectionString, sql,
                        new SqlParameter("@CountryID", countryId),
                        new SqlParameter("@SiteCode", siteCode),
                        new SqlParameter("@DateAllocated", DateTime.Now));
            return command;
        }

        /// <summary>
        /// Updates files into database table by concrete values
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="barcode"></param>
        /// <param name="fileId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int UpdateFile(DbClientVoucher voucher)
        {
            return SQL.ExecuteNonQuery(CreateUpdateFileCommand(voucher));
        }

        /// <summary>
        /// Updates files int database table by concrete values asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="barcode"></param>
        /// <param name="fileId"></param>
        /// <param name="content"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        public static void UpdateFileAsync(DbClientVoucher voucher, EventHandler success, ThreadExceptionEventHandler error)
        {
            SQLWorker.Default.Add(CreateUpdateFileCommand(voucher), success, error);
        }

        /// <summary>
        /// Creates command for updating files into database table by concrete values
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="barcode"></param>
        /// <param name="fileId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static SqlCommand CreateUpdateFileCommand(DbClientVoucher voucher)
        {
            const string sql = @"UPDATE FILES " +
                               "SET  " +
                               "RetailerID = @RetailerID, " +
                               "VoucherID = @VoucherID, " +
                               "BarCode = @BarCode, " +
                               "VoucherImage = @VoucherImage, " +
                               "BarCodeImage = @BarCodeImage, " + 
                               "DateInserted = @DateInserted " +
                               "WHERE SiteCode = @SiteCode AND CountryID = @CountryID";
            SqlCommand command = MSSQL.CreateCommand(ConnectionString,
                        sql,
                        new SqlParameter("@CountryID", voucher.CountryID),
                        new SqlParameter("@RetailerID", voucher.RetailerID),
                        new SqlParameter("@VoucherID", voucher.VoucherID),
                        new SqlParameter("@BarCode", voucher.BarCode),
                        new SqlParameter("@VoucherImage", voucher.VoucherImage),
                        new SqlParameter("@BarCodeImage", voucher.BarCodeImage),
                        new SqlParameter("@DateInserted", DateTime.Now),
                        new SqlParameter("@SiteCode", voucher.SiteCode));
            return command;
        }

        #endregion

        #region INSERT/SELECT BARCODES

        public static void InsertBarcodeInfo(Guid id, byte[] data)
        {
            const string sql = "INSERT INTO TMP(ID, DATA, LENGTH) VALUES (@ID, @DATA, @LENGTH)";
            MSSQL.ExecuteNonQuery(ConnectionString, sql,
                new SqlParameter("@ID", id),
                new SqlParameter("@LENGTH", data.Length),
                new SqlParameter("@DATA", SqlDbType.Binary, data.Length) { Value = data });
        }

        public static void UpdateBarcodeInfo(Guid id, byte[] data)
        {
            const string sql = "UPDATE TMP SET DATA=@DATA, LENGTH=@LENGTH WHERE ID=@ID";
            MSSQL.ExecuteNonQuery(ConnectionString, sql,
                new SqlParameter("@ID", id),
                new SqlParameter("@LENGTH", data.Length), 
                new SqlParameter("@DATA", SqlDbType.Binary, data.Length) { Value = data });
        }

        public static byte[] SelectBarcodeInfoData(Guid id)
        {
            const string sql = "SELECT @DATA = DATA, @LENGTH = LENGTH FROM TMP WHERE ID=@ID";

            var length = new SqlParameter("@LENGTH", SqlDbType.Int);
            length.Direction = ParameterDirection.Output;

            var data = new SqlParameter("@DATA", SqlDbType.Binary, int.MaxValue);
            data.Direction = ParameterDirection.Output;

            MSSQL.ExecuteNonQuery(ConnectionString, sql, new SqlParameter("@ID", id), length, data);

            int count = Convert.ToInt32(length.Value);
            byte[] readArr = (byte[]) data.Value;
            byte[] result = new byte[count];
            Array.Copy(readArr, result, result.Length);
            return result;
        }

        public static void DeleteBarcodeInfo(Guid id)
        {
            const string sql = "DELETE TMP WHERE ID=@ID";
            MSSQL.ExecuteNonQuery(ConnectionString, sql, new SqlParameter("@ID", id));
        }

        #endregion

        #region UPDATE FILE, SET EXPORTED

        /// <summary>
        /// Sets file exported to the client database
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public static int SetFileExported(string siteCode)
        {
            return SQL.ExecuteNonQuery(CreateSetFileExportedCommand(siteCode));
        }

        /// <summary>
        /// Sets file exported to the client database asynchronously 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="success"></param>
        /// <param name="error"></param>
        public static void SetFileExportedAsync(string siteCode, EventHandler success, ThreadExceptionEventHandler error)
        {
            SQLWorker.Default.Add(CreateSetFileExportedCommand(siteCode), success, error);
        }

        /// <summary>
        /// Creates command for setting file as exported to the client database 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public static SqlCommand CreateSetFileExportedCommand(string siteCode)
        {
            const string sql = "UPDATE FILES SET DateExported = @DateExported WHERE FileID = @FileID;";
            SqlCommand command = MSSQL.CreateCommand(ConnectionString, sql,
                        new SqlParameter("@FileID", siteCode),
                        new SqlParameter("@DateExported", DateTime.Now));
            return command;
        }

        #endregion

        #region UPDATE MESSAGE, SET EXPORTED

        /// <summary>
        /// Sets messages as exported to the client database
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public static int SetMessageExported(int messageId)
        {
            return SQL.ExecuteNonQuery(CreateSetMessageExportedCommand(messageId));
        }

        /// <summary>
        /// Sets messages as exported to the client database asynchronously
        /// </summary>
        /// <param name="messageId"></param>
        public static void SetMessageExportedAsync(int messageId)
        {
            SQLWorker.Default.Add(CreateSetMessageExportedCommand(messageId), 
                null,
                // Skip error messages
                PTFUtils.EmptyThreadExceptionEventHandler());
        }

        /// <summary>
        /// Creates command for setting messages as exported to the client database
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public static SqlCommand CreateSetMessageExportedCommand(int messageId)
        {
            const string sql = "UPDATE MESSAGES SET DateExported = @DateExported WHERE MessageID = @MessageID;";
            SqlCommand command = MSSQL.CreateCommand(ConnectionString, sql,
                        new SqlParameter("@MessageID", messageId),
                        new SqlParameter("@DateExported", DateTime.Now));
            return command;
        }

        #endregion

        #region BATCHES

        /// <summary>
        /// Selects BatchID
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static int SelectBatchID(int fileId)
        {
            const string sql = "SelectBatchID";
            var p0 = new SqlParameter("@FileID", fileId);
            var p1 = new SqlParameter("@BatchID", 0) { Direction = ParameterDirection.Output };
            SqlCommand com = MSSQL.CreateCommand(ConnectionString, sql, CommandType.StoredProcedure, p0, p1);
            SQL.ExecuteNonQuery(com);
            return p1.Value.Cast<int>();
        }

        #endregion

        #region SELECTS

        /// <summary>
        /// Selects all messages for export
        /// </summary>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static List<DbMessageInfo> SelectMessagesForExport(int maximum)
        {
            string sql = "SELECT TOP {0} MessageID, Message, Source, Type, StackTrace, DateInserted FROM MESSAGES WHERE DateExported IS NULL;".format(maximum);
            List<DbMessageInfo> list = MSSQL.ExecuteReader<DbMessageInfo>(ConnectionString, sql);
            return list;
        }

        /// <summary>
        /// Selects file by file id
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public static DbClientFileInfo SelectFile(string siteCode)
        {
            const string sql = "SELECT * FROM FILES WHERE SiteCode = @SiteCode;";
            List<DbClientFileInfo> results = MSSQL.ExecuteReader<DbClientFileInfo>(ConnectionString, sql, new SqlParameter("@SiteCode", siteCode));
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Selects max file id
        /// </summary>
        /// <returns></returns>
        public static DbClientFileInfo SelectFileMax()
        {
            const string sql = "SELECT * FROM FILES WHERE SiteCode = (SELECT MAX(FileID) FROM FILES WHERE VoucherImage IS NOT NULL);";
            List<DbClientFileInfo> results = MSSQL.ExecuteReader<DbClientFileInfo>(ConnectionString, sql);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Selects top N files for export
        /// </summary>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static List<DbClientFileInfo> SelectFilesForExport(int maximum)
        {
            string sql = "SELECT TOP {0} * FROM FILES WHERE VoucherImage IS NOT NULL AND DateExported IS NULL;".format(maximum);
            var list = MSSQL.ExecuteReader<DbClientFileInfo>(ConnectionString, sql);
            return list;
        }

        /// <summary>
        /// Selects all available files from client database
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DbClientFileInfo> SelectAllFiles()
        {
            const string sql = "SELECT * FROM FILES WHERE VoucherImage IS NOT NULL;";
            var comm = MSSQL.CreateCommand(ConnectionString, sql);
            var list = SQL.ExecuteYieldReader<DbClientFileInfo>(comm);
            return list;
        }

        #endregion

        #region DELETE OLDER SENT FILES

        /// <summary>
        /// Deletes older than N days files
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static int DeleteOlderSentFiles(int days)
        {
            return SQL.ExecuteNonQuery(CreateDeleteOlderSentFiles(days));
        }

        /// <summary>
        /// Deletes older than N days files asynchronously
        /// </summary>
        /// <param name="days"></param>
        public static void DeleteOlderSentFilesAsync(int days)
        {
            SQLWorker.Default.Add(CreateDeleteOlderSentFiles(days));
        }

        /// <summary>
        /// Creates command for deleting older than N days files
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        private static SqlCommand CreateDeleteOlderSentFiles(int days)
        {
            const string sql = "DELETE FROM FILES WHERE DateExported < @Date;";
            return MSSQL.CreateCommand(ConnectionString,
                    sql,
                    new SqlParameter("@Date", DateTime.Now.AddDays(-days)));
        }

        #endregion

        #region DELETE OLDER SENT MESSAGES

        /// <summary>
        /// Deletes older than N days messages
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static int DeleteOlderSentMessages(int days)
        {
            return SQL.ExecuteNonQuery(CreateDeleteOlderSentMessages(days));
        }

        /// <summary>
        /// Deletes older than N days messages asynchronously
        /// </summary>
        /// <param name="days"></param>
        public static void DeleteOlderSentMessagesAsync(int days)
        {
            SQLWorker.Default.Add(CreateDeleteOlderSentMessages(days));
        }

        /// <summary>
        /// Creates command for deleting older than N days messages
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        private static SqlCommand CreateDeleteOlderSentMessages(int days)
        {
            const string sql = "DELETE FROM MESSAGES WHERE DateExported < @Date;";
            return MSSQL.CreateCommand(ConnectionString,
                    sql,
                    new SqlParameter("@Date", DateTime.Now.AddDays(-days)));
        }

        #endregion

        #region INSERT MESSAGE

        /// <summary>
        /// Inserts message to the client database
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="stacktrase"></param>
        public static void InsertMessage(string message, eSources source, eMessageTypes type, string stacktrase)
        {
            SQL.ExecuteNonQuery(CreateInsertMessage(message, source, type, stacktrase));
        }

        /// <summary>
        /// Inserts message to the client database asynchronously
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="stacktrase"></param>
        public static void InsertMessageAsync(string message, eSources source, eMessageTypes type, string stacktrase)
        {
            // Do not refire errors
            SQLWorker.Default.Add(CreateInsertMessage(message, source, type, stacktrase));
        }

        /// <summary>
        /// Creates command for inserting message to the client database
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="stacktrase"></param>
        /// <param name="onServer"></param>
        /// <returns></returns>
        private static SqlCommand CreateInsertMessage(string message, eSources source, eMessageTypes type, string stackTrase)
        {
            const string sql = "INSERT INTO MESSAGES(Message, Source, Type, StackTrace, DateInserted) VALUES " +
                                        "(@Message, @Source, @Type, @StackTrace, @DateInserted);";
            return MSSQL.CreateCommand(ConnectionString,
                sql,
                new SqlParameter("@Message", message.Top(1024)),
                new SqlParameter("@Source", Convert.ToInt32(source)),
                new SqlParameter("@Type", Convert.ToInt32(type)),
                new SqlParameter("@StackTrace", stackTrase.Top(2028)),
                new SqlParameter("@DateInserted", DateTime.Now));
        }

        #endregion

        #region CONFIG

        /// <summary>
        /// Inserts configuration value to the client database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void InsertConfigValue(string name, byte[] value)
        {
            SQL.ExecuteNonQuery(CreateInsertConfigurationCommand(name, value));
        }

        /// <summary>
        /// Inserts configuration value to the client database asynchronously
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void InsertConfigurationAsync(string name, byte[] value)
        {
            if (value == null)
                return;
            SQLWorker.Default.Add(CreateInsertConfigurationCommand(name, value));
        }

        /// <summary>
        /// Creates command for inserting configuration value to the client database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlCommand CreateInsertConfigurationCommand(string key, byte[] value)
        {
            const string sql = @"IF(EXISTS(SELECT NULL FROM [CONFIG] WHERE [Key]=@Key))
                                    UPDATE [CONFIG] 
                                    Set [Value] = @Value 
                                    WHERE [Key]= @Key
                                ELSE
                                    INSERT INTO CONFIG([Key],[Value])VALUES(@Key,@Value)";
            return MSSQL.CreateCommand(ConnectionString, sql,
                    new SqlParameter("@Key", key.Top(100)),
                    new SqlParameter("@Value", value));
        }

        /// <summary>
        /// Selects value from config datatable, client database
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] SelectConfigValue(string key)
        {
            const string sql = "SELECT @Result = [Value] FROM CONFIG WHERE [Key] = @Key;";
            var result = new SqlParameter("@Result", SqlDbType.Binary, int.MaxValue) { Direction = ParameterDirection.Output };
            MSSQL.ExecuteNonQuery(ConnectionString, sql, 
                new SqlParameter("@Key", key.Top(100)), result);
            return result.Value as byte[];
        }

        #endregion

        #region SITEIDS

        /// <summary>
        /// Selects available files count from client database
        /// </summary>
        /// <returns></returns>
        public static int SelectAvailableSiteCodeIDsCount()
        {
            const string sql = "SELECT Count(*) FROM FILES WHERE VoucherImage IS NULL;";
            int availableIds = MSSQL.ExecuteScalar<int>(ConnectionString, sql);
            return availableIds;
        }

        /// <summary>
        /// Selects TOP N file ids from client database
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<string> SelectAvailableSiteCodeIDs(int count)
        {
            string sql = string.Format("SELECT TOP({0}) FileID as ID FROM FILES WHERE VoucherImage IS NULL;", count);
            List<DbId> siteIds = MSSQL.ExecuteReader<DbId>(ConnectionString, sql);
            return siteIds.ConvertAll((id) => id.ID);
        }

        #endregion
    }
}
