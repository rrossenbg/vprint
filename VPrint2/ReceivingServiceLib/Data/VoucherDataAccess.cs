/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

#define USE_INSERT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using VPrinting;

namespace ReceivingServiceLib.Data
{
    /// <summary>
    /// WARNING: PREPARE SQL SERVER
    /// -----------------------------
    /// -- Reset the "allow updates" setting to the recommended 0
    /// sp_configure 'allow updates',0;
    /// reconfigure with override
    /// go
    /// sp_configure 'show advanced options',0;
    /// reconfigure
    /// go
    /// EXEC sp_configure filestream_access_level, 2
    /// RECONFIGURE
    /// go
    /// </summary>
    public class VoucherDataAccess : BaseDataAccess 
    {
        public static VoucherDataAccess Instance
        {
            get
            {
                return new VoucherDataAccess();
            }
        }

        #region SCAN

        public void AddVoucher(
            int jobId, int isoId, int branchId, int voucherId, int? folderId, string siteCode, string barCode,
            int locationId, int operatorId, byte[] buffer, int length, string sessionId, bool isProtected)
        {
            CheckImagesConnectionStringThrow();

            #region SQL
#if INSERT           

            const string SQL1 = @"INSERT Voucher( job_id, iso_id,   branch_id,  v_number,  v_fl_id,  sitecode,  barcode, scandate,   location,  operator_id,  scan_image_size,  scan_image, session_id, v_protected)
	                                    VALUES (@job_id, @iso_id, @branch_id, @v_number, @v_fl_id, @sitecode, @barcode, getdate(), @location, @operator_id, @scan_image_size, @scan_image, @session_id, @protected);
                                  SELECT SCOPE_IDENTITY();";
#else

            const string SQL2 = @"
            MERGE Voucher AS t
            USING (SELECT @job_id, @iso_id, @branch_id, @v_number, @v_fl_id, @sitecode, @barcode, @location, @operator_id, @scan_image_size, @scan_image, @session_Id, @protected) AS s 
			              (job_id, iso_id,  branch_id,  v_number, v_fl_id, sitecode,  barcode,  location, operator_id,   scan_image_size, scan_image, session_id, v_protected)
            ON (t.iso_id = s.iso_id and t.branch_id = s.branch_id and t.v_number = s.v_number and t.sitecode = s.sitecode)
            WHEN MATCHED THEN 
                UPDATE SET  job_id = s.job_id,
					        sitecode = s.sitecode,
					        barcode = s.barcode,
					        scandate = getdate(),
					        location = s.location,
					        operator_id = s.operator_id,
					        scan_image_size = s.scan_image_size,
					        scan_image = s.scan_image,
					        session_id = s.session_id,
                            v_protected = s.v_protected,
                            v_fl_id = s.v_fl_id
	        WHEN NOT MATCHED THEN	
	            INSERT (job_id,   iso_id,   branch_id,   v_number,   v_fl_id,   sitecode,   barcode,  scandate,   location,   operator_id,   scan_image_size,  scan_image, session_id, v_protected)
	            VALUES (s.job_id, s.iso_id, s.branch_id, s.v_number, s.v_fl_id, s.sitecode, s.barcode, getdate(), s.location, s.operator_id, s.scan_image_size, s.scan_image, s.session_id, s.v_protected);
	            --OUTPUT deleted.*, $action, inserted.* INTO #MyTempTable;
                SELECT SCOPE_IDENTITY();";
#endif

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                int v_id;

                using (var comm = new SqlCommand(SQL2, conn))
                {
                    comm.CommandTimeout = 0;
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@job_id", jobId);
                    comm.Parameters.AddWithValue("@iso_id", isoId);
                    comm.Parameters.AddWithValue("@branch_id", branchId);
                    comm.Parameters.AddWithValue("@v_number", voucherId);
                    comm.Parameters.AddWithValue("@sitecode", siteCode);
                    comm.Parameters.AddWithValue("@protected", isProtected);
                    comm.Parameters.AddWithValue("@barcode", barCode);
                    comm.Parameters.AddWithValue("@location", locationId);
                    comm.Parameters.AddWithValue("@operator_id", operatorId);
                    comm.Parameters.AddWithValue("@v_fl_id", folderId.GetValue());
                    comm.Parameters.AddWithValue("@scan_image_size", length);
                    var p1 = comm.Parameters.Add("@scan_image", SqlDbType.Binary, length);
                    p1.Value = buffer;
                    comm.Parameters.AddWithValue("@session_Id", sessionId);
                    v_id = comm.ExecuteScalar().Cast<int>();
                }
            }
        }

        public void AddCoversheet(int? folderId, int locationId, int operatorId, byte[] buffer, int length, string sessionId, bool isProtected)
        {
            #region SQL

            const string SQL = @"INSERT INTO [File] ([f_fl_id], [f_location], [f_operator_id], [f_session_id], [f_image_size], [f_image], [f_createdAt], [f_protected])
                                 VALUES (@f_fl_id, @f_location, @f_operator_id, @f_session_id, @f_image_size, @f_image, @f_createdAt, @protected)";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandTimeout = 0;
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@f_fl_id", folderId.GetValue());
                    comm.Parameters.AddWithValue("@f_location", locationId);
                    comm.Parameters.AddWithValue("@f_operator_id", operatorId);
                    comm.Parameters.AddWithValue("@f_session_id", sessionId);
                    comm.Parameters.AddWithValue("@f_image_size", length);
                    comm.Parameters.AddWithValue("@f_createdAt", DateTime.Now);
                    var p1 = comm.Parameters.Add("@f_image", SqlDbType.Binary, length);
                    p1.Value = buffer;
                    comm.Parameters.AddWithValue("@session_Id", sessionId);
                    comm.Parameters.AddWithValue("@protected", isProtected);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public class SelectVouchersData
        {
            public int id { get; set; }
            public int v_number { get; set; }
            public string sitecode { get; set; }
            public string barcode { get; set; }
            public DateTime scandate { get; set; }
            public int location { get; set; }
            public int operator_id { get; set; }
            public string session_id { get; set; }
            public string v_name { get; set; }
            public bool v_protected { get; set; }

            public SelectVouchersData()
            {
            }

            public SelectVouchersData(int id, int v_number, string sitecode, string barcode, DateTime scandate, int location, int operator_id, string session_id, string v_name, bool isProtected)
            {
                this.id = id;
                this.v_number = v_number;
                this.sitecode = sitecode;
                this.barcode = barcode;
                this.scandate = scandate;
                this.location = location;
                this.operator_id = operator_id;
                this.session_id = session_id;
                this.v_name = v_name;
                this.v_protected = isProtected;
            }
        }

        public List<SelectVouchersData> SelectVouchers(int isoId, int branchId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"select id, job_id,  v_number, sitecode, barcode, scandate, location, operator_id, session_Id, v_name, v_protected from Voucher
                                where iso_id = @iso_id and branch_id = @branch_id;";

            #endregion

            var result = new List<SelectVouchersData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandTimeout = 0;
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", isoId);
                    comm.Parameters.AddWithValue("@branch_id", branchId);
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(
                            reader.ReadRange((r) =>
                            {
                                var id = r.Get<int>("id").Value;
                                var v_number = r.Get<int>("v_number").Value;
                                var sitecode = r.GetString("sitecode");
                                var barcode = r.GetString("barcode");
                                var scandate = r.Get<DateTime>("scandate").GetValueOrDefault();
                                var location = r.Get<int>("location").GetValueOrDefault();
                                var operator_id = r.Get<int>("operator_id").GetValueOrDefault();
                                var session_id = r.GetString("session_Id");
                                var v_name = r.GetString("v_name");
                                var v_protected = r.Get<bool>("v_protected").GetValueOrDefault();
                                var t = new SelectVouchersData(id, v_number, sitecode, barcode, scandate, location, operator_id, session_id, v_name, v_protected);
                                return t;
                            }));
                    }
                }
            }

            return result;
        }

        public class SelectVoucherInfoData
        {
            public int vid { get; set; }
            public int isoId { get; set; }
            public int branch_id { get; set; }
            public int v_number { get; set; }
            public string session_Id { get; set; }

            public SelectVoucherInfoData()
            {
            }

            public SelectVoucherInfoData(int vid, int isoId, int branch_id, int v_number, string session_Id)
            {
                this.vid = vid;
                this.isoId = isoId;
                this.branch_id = branch_id;
                this.v_number = v_number;
                this.session_Id = session_Id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_id"></param>
        /// <returns>Id, IsoId, RetailerId, VoucherId, SessionId</returns>
        public SelectVoucherInfoData SelectVoucherInfo(int v_id)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT [id]
                                  ,[iso_id]
                                  ,[branch_id]
                                  ,[v_number]
                                  ,[session_Id]
                                FROM [Voucher]
                               WHERE Id = @id";

            #endregion //SQL

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@id", v_id);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            var vid = reader.Get<int>("id").Value;
                            var isoId = reader.Get<int>("iso_id").Value;
                            var branch_id = reader.Get<int>("branch_id").Value;
                            var v_number = reader.Get<int>("v_number").Value;
                            var session_Id = reader.GetString("session_Id");

                            var result = new SelectVoucherInfoData(vid, isoId, branch_id, v_number, session_Id);
                            return result;
                        }
                        else
                        {
                            return new SelectVoucherInfoData();
                        }
                    }
                }
            }
        }

        public byte[] SelectImageById(int id, bool isVoucher)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = isVoucher ?
                "SELECT scan_image, scan_image_size FROM [Voucher] WHERE id = @id" :
                "SELECT f_image, f_image_size FROM [File] WHERE f_id = @id";

            string LengthField = isVoucher ?
                "scan_image_size" :
                "f_image_size";

            #endregion //SQL

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@id", id);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            var size = reader.Get<int>(LengthField).Value;
                            byte[] buffer = new byte[size];
                            reader.GetBytes(0, 0, buffer, 0, size);
                            return buffer;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public void ValidateVoucherThrow(int countryId, int retailerId, int voucherId, bool voucherExists)
        {
            CheckPTFConnectionStringThrow();

            const string SQL0 = "SELECT iso_number FROM ISO WHERE iso_number = @iso;";

            var db = MSSQL.Instance;

            int iso_number = db.ExecuteScalar<int>(Global.Strings.PTFConnString, SQL0, CommandType.Text,
                new SqlParameter("@iso", countryId));

            if (iso_number == 0)
                throw new DataException("ISO not found");

            const string SQL1 = "SELECT br_id FROM Branch WHERE br_iso_id = @iso and br_id = @br_id;";

            int br_id = db.ExecuteScalar<int>(Global.Strings.PTFConnString, SQL1, CommandType.Text,
                new SqlParameter("@iso", countryId),
                new SqlParameter("@br_id", retailerId));

            if (br_id == 0)
                throw new DataException("Branch not found");

            if (voucherExists)
            {
                const string SQL2 = "SELECT v_number FROM Voucher WHERE v_iso_id = @iso and v_br_id = @br_id  and v_number = @v_id;";

                int v_number = db.ExecuteScalar<int>(Global.Strings.PTFConnString, SQL2, CommandType.Text,
                    new SqlParameter("@iso", countryId),
                    new SqlParameter("@br_id", retailerId),
                    new SqlParameter("@v_id", voucherId));

                if (v_number == 0)
                    throw new DataException("Voucher not found");
            }
            else
            {
                const string SQL3 = "SELECT vn_id FROM VoucherNumber WHERE vn_iso_id = @iso and vn_br_id = @br_id and vn_first <= @v_id and @v_id <=vn_last;";

                int vn_id = db.ExecuteScalar<int>(Global.Strings.PTFConnString, SQL3, CommandType.Text,
                    new SqlParameter("@iso", countryId),
                    new SqlParameter("@br_id", retailerId),
                    new SqlParameter("@v_id", voucherId));

                if (vn_id == 0)
                    throw new DataException("Voucher allocation not found");
            }
        }

        public string GetConfigValue(string key)
        {
            CheckPTFConnectionStringThrow();

            const string SQL = "SELECT config_value FROM Config WHERE config_key = @key;";

            var db = MSSQL.Instance;
            string value = db.ExecuteScalar<string>(Global.Strings.PTFConnString, SQL, CommandType.Text,
               new SqlParameter("@key", key));
            return value;
        }

        #endregion

        #region CCC Cover

        public string SelectCoverInfo(int id)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = "SELECT v_cover FROM Voucher WHERE Id=@Id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();
                comm.Parameters.Add("@Id", id);
                return comm.ExecuteScalar().Cast<string>("");
            }
        }

        #endregion

        public class RetailerPrinterData
        {
            public int CountryId { get; set; }
            public int HeadOfficeId { get; set; }
            public int RetailerId { get; set; }

            public int FormatId { get; set; }

            public string FormatName { get; set; }
            public string FormatType { get; set; }
            public string PrinterPath { get; set; }
        }

        public List<RetailerPrinterData> SelectRetailerPrinterData(int countryId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"select  br_iso_id, br_id, br_ho_id, vpd_id, vpd_printer_path, vpf_name, vpf_type2 from Branch br
                                inner join dbo.VoucherPrinterDetails vpd on vpd_id = br_vpd_id 
                                inner join dbo.VoucherPrintingFormat vpf on vpf_id = vpd_vpf_id
                                where br_iso_id = @iso_id";

            #endregion

            var list = new List<RetailerPrinterData>();

            using (SqlConnection conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", countryId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new RetailerPrinterData()
                            {
                                CountryId = reader.Get<int>("br_iso_id").GetValueOrDefault(),
                                HeadOfficeId = reader.Get<int>("br_ho_id").GetValueOrDefault(),
                                RetailerId = reader.Get<int>("br_id").GetValueOrDefault(),
                                FormatId = reader.Get<int>("vpd_id").GetValueOrDefault(),

                                FormatName = reader.GetString("vpf_name"),
                                PrinterPath = reader.GetString("vpd_printer_path"),
                                FormatType = reader.GetString("vpf_type2"),
                            };

                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        #region FOLDERS

        public void AddFolder(int? parentId, string name, int countryId, int userId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"INSERT INTO [Folder] ([fl_parent_id],[fl_name],[fl_createdby_isoid],[fl_createdby_userId])
                                        VALUES (@parent_id, @name, @createdby_isoid, @createdby_userId)";
            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@parent_id", parentId.GetValue());
                    comm.Parameters.AddWithValue("@name", name);
                    comm.Parameters.AddWithValue("@createdby_isoid", countryId);
                    comm.Parameters.AddWithValue("@createdby_userId", userId);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFolder(int f_Id)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"delete from Folder where fl_id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", f_Id);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteVoucherOrFile(int id, bool isVoucher)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = isVoucher ?
                "update Voucher set v_fl_id = null, deletedAt=getdate() where id = @id;" :
                "update [File] set f_fl_id = null, f_deletedAt=getdate() where f_id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", id);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllFilesInFolder(int folderId, bool isVoucher)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = isVoucher ?
                "update [Voucher] set v_fl_id = null where v_fl_id = @id;" :
                "update [File] set f_fl_id = null where f_fl_id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", folderId);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void RenameFolder(int f_id, string name)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"update Folder 
                                 set fl_name = @name
                                 where fl_id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", f_id);
                    comm.Parameters.AddWithValue("@name", name);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void UpdateFolder(int f_id, string name, int? parentId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"update Folder 
                                 set fl_parent_id = @parent_id,
                                     fl_name = @name
                                 where fl_id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", f_id);
                    comm.Parameters.AddWithValue("@parent_id", parentId.GetValue());
                    comm.Parameters.AddWithValue("@name", name);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public class FlattenFolderData
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
        }

        public List<FlattenFolderData> FlattenFolderTable()
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"WITH myCTE (Item_Id, Parent_Id, Depth)
                                 AS
                                 (
                                    Select fl_id, fl_parent_id, 0 as Depth From Folder 
                                    where fl_parent_id is null
                                    Union ALL
                                    Select Folder.fl_id, Folder.fl_parent_id, Depth + 1 
                                    From Folder 
                                    inner join myCte on Folder.fl_parent_id = myCte.Item_Id
                                 )
 
                                 Select Item_Id, Parent_Id, Depth From myCTE;";

            #endregion

            var list = new List<FlattenFolderData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new FlattenFolderData()
                            {
                                Id = reader.Get<int>("Item_id").GetValueOrDefault(),
                                ParentId = reader.Get<int>("Parent_id"),
                            };
                            list.Add(data);
                        }
                    }
                }
            }

            return list;
        }

        public class FolderData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int? ParentId { get; set; }
        }

        public List<FolderData> SelectAllByParent(int? fl_parentId, int fl_createdby_isoid)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = fl_parentId.HasValue ?
                @"SELECT [fl_id], [fl_name], [fl_parent_id] FROM [Folder] WHERE [fl_parent_id] = @parent_id;" :
                @"SELECT [fl_id], [fl_name], [fl_parent_id] FROM [Folder] WHERE [fl_parent_id] IS NULL and fl_createdby_isoid = @createdby_isoid";

            #endregion

            var list = new List<FolderData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@parent_id", fl_parentId.GetValue());
                    comm.Parameters.AddWithValue("@createdby_isoid", fl_createdby_isoid);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new FolderData()
                            {
                                Id = reader.Get<int>("fl_id").GetValueOrDefault(),
                                Name = reader.GetString("fl_name"),
                                ParentId = reader.Get<int>("fl_parent_id"),
                            };
                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        public class fileData
        {
            public int Id { get; set; }
            public int FolderId { get; set; }
            public string SessionId { get; set; }
            public int CountryId { get; set; }
            public int RetailerId { get; set; }
            public int VoucherId { get; set; }
            public string SiteCode { get; set; }
            public string Name { get; set; }
        }

        public List<fileData> SelectVouchersByFolder(int folderId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT [id], [iso_id], [v_fl_id], [branch_id], [v_number], [session_Id], [sitecode], [v_name] FROM [Voucher] WHERE [v_fl_id] = @flId and deletedAt is NULL;";

            #endregion

            var list = new List<fileData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@flId", folderId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new fileData()
                            {
                                Id = reader.Get<int>("id").GetValueOrDefault(),
                                FolderId = reader.Get<int>("v_fl_id").GetValueOrDefault(),
                                SessionId = reader.GetString("session_Id"),
                                CountryId = reader.Get<int>("iso_id").GetValueOrDefault(),
                                RetailerId = reader.Get<int>("branch_id").GetValueOrDefault(),
                                VoucherId = reader.Get<int>("v_number").GetValueOrDefault(),
                                SiteCode = reader.GetString("sitecode"),
                                Name = reader.GetString("v_name"),
                            };
                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        public class file2Data
        {
            public int Id { get; set; }
            public int FolderId { get; set; }
            public int CountryId { get; set; }
            public int Location { get; set; }
            public int Operator { get; set; }
            public string SessionId { get; set; }
            public string Name { get; set; }
        }

        public List<file2Data> SelectFilesByFolder(int folderId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT [f_id], [f_fl_id], [f_location], [f_operator_id], [f_country_id], [f_session_Id] , [f_name] FROM [File] WHERE [f_fl_id] = @flId and f_deletedAt is NULL;";

            #endregion

            var list = new List<file2Data>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@flId", folderId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new file2Data()
                            {
                                Id = reader.Get<int>("f_id").GetValueOrDefault(),
                                FolderId = reader.Get<int>("f_fl_id").GetValueOrDefault(),
                                Location = reader.Get<int>("f_location").GetValueOrDefault(),
                                CountryId = reader.Get<int>("f_country_id").GetValueOrDefault(),
                                Operator = reader.Get<int>("f_operator_id").GetValueOrDefault(),
                                SessionId = reader.GetString("f_session_Id"),
                                Name = reader.GetString("f_name"),
                            };
                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        public List<fileData> SelectVouchersBySql(string whereClause)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = @"SELECT TOP 1000 [id], [iso_id], [v_fl_id], [branch_id], [v_number], [session_Id], [sitecode] FROM [Voucher] WHERE ".concat(whereClause, ';');

            #endregion

            var list = new List<fileData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new fileData()
                            {
                                Id = reader.Get<int>("id").GetValueOrDefault(),
                                FolderId = reader.Get<int>("v_fl_id").GetValueOrDefault(),
                                SessionId = reader.GetString("session_Id"),
                                CountryId = reader.Get<int>("iso_id").GetValueOrDefault(),
                                RetailerId = reader.Get<int>("branch_id").GetValueOrDefault(),
                                VoucherId = reader.Get<int>("v_number").GetValueOrDefault(),
                                SiteCode = reader.GetString("sitecode"),
                            };
                            list.Add(data);
                        }
                    }
                }
            }
            return list;
        }

        public void UpdateVouchersBySql(string setsql, string whereClause)
        {
            CheckImagesConnectionStringThrow();

            string SQL = string.Format(@"UPDATE [Voucher] SET {0} WHERE {1};", setsql, whereClause);

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                    comm.ExecuteNonQuery();
            }
        }

        public void UpdateFilesBySql(string setsql, string whereClause)
        {
            CheckImagesConnectionStringThrow();

            string SQL = string.Format(@"UPDATE [File] SET {0} WHERE {1};", setsql, whereClause);

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                    comm.ExecuteNonQuery();
            }
        }

        public byte[] SelectVoucherById(int id, bool isVoucher, out bool isProtected)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string LengthField = isVoucher ?
                "scan_image_size" :
                "f_image_size";

            string ProtectedField = isVoucher ?
                "v_protected" :
                "f_protected";

            string SQL = isVoucher ?
                 "SELECT [scan_image], [scan_image_size], [v_protected] FROM [Voucher] WHERE [id] = @id;" :
                 "SELECT [f_image], [f_image_size], [f_protected] FROM [File] WHERE f_id = @id";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", id);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            int size = reader.Get<int>(LengthField).GetValueOrDefault();
                            isProtected = reader.Get<bool>(ProtectedField).GetValueOrDefault();

                            var buffer = new byte[size];
                            reader.GetBytes(0, 0, buffer, 0, size);
                            return buffer;
                        }
                        isProtected = false;
                        return null;
                    }
                }
            }
        }

        public List<int> FindVoucherImage(int countryId, int voucherId, int voucherIdCD)
        {
            CheckImagesConnectionStringThrow();

            //, branch_id, v_fl_id, sitecode, barcode, scandate, location, operator_id, scan_image_size
            const string SQL = "select id from voucher where iso_id = @iso_id and (v_number = @v_number or v_number = @v_numberCD)";

            List<int> ids = new List<int>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("iso_id", countryId);
                    comm.Parameters.AddWithValue("v_number", voucherId);
                    comm.Parameters.AddWithValue("v_numberCD", voucherIdCD);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            int id = reader.Get<int>("id").GetValueOrDefault();
                            ids.Add(id);
                        }
                    }
                }
            }

            return ids;
        }

        #region TRS

        public string FindVoucher(int countryId, int voucherId, int voucherIdCD)
        {
            CheckPTFConnectionStringThrow();

            const string SQL = "select vp_site_code, vp_location_number from  VoucherPart where (vp_v_number = @v_number or vp_v_number = @v_numberCD) and vp_iso_id = @iso_id and vp_type_id = 3";

            using (var conn = new SqlConnection(Global.Strings.PTFConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("iso_id", countryId);
                    comm.Parameters.AddWithValue("v_number", voucherId);
                    comm.Parameters.AddWithValue("v_numberCD", voucherIdCD);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            string a = reader.GetString("vp_site_code");
                            int b = reader.Get<int>("vp_location_number").GetValueOrDefault();
                            return string.Concat(a, b);
                        }
                    }
                    return null;
                }
            }
        }

        #endregion

        #endregion

        #region TRANSFER FILE

        public class TransferFileData
        {
            public string InvNo { get; set; }
            public int BranchId { get; set; }
            public string SiteLocationNo { get; set; }
            public int VoucherNumber { get; set; }
        }

        public List<TransferFileData> GetTransferFileReport(int countryId, int beginNumber, int endNumber, string siteCode)
        {
            CheckPTFConnectionStringThrow();

            #region SQL
            /// <summary>
            /// The report is called voucherScanningExport and it runs this SQL
            /// </summary>
            const string SQL = @"
            DECLARE @invno int EXEC @invno = getSequence2 'ScanningExports' 

            SELECT        @invno [invno], v_br_id, vp_site_code + CAST(vp_location_number AS varchar) AS SiteLocationNo,  vp_v_number
            FROM            VoucherPart(nolock) JOIN
            Voucher(nolock) ON v_iso_id = vp_iso_id AND v_number = vp_v_number AND vp_site_code = @SiteCode
            WHERE        vp_iso_id = @Country AND vp_site_code = @SiteCode AND vp_location_number >= @StartNumber AND vp_location_number <= @EndNumber
            ORDER BY vp_location_number";
            #endregion

            var list = new List<TransferFileData>();

            using (SqlConnection conn = new SqlConnection(Global.Strings.PTFConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@Country", countryId);
                    comm.Parameters.AddWithValue("@StartNumber", beginNumber);
                    comm.Parameters.AddWithValue("@EndNumber", endNumber);
                    comm.Parameters.AddWithValue("@SiteCode", siteCode);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var tr = new TransferFileData()
                            {
                                InvNo = reader.GetString("invno"),
                                BranchId = reader.Get<int>("v_br_id").GetValueOrDefault(),
                                SiteLocationNo = reader.GetString("SiteLocationNo"),
                                VoucherNumber = reader.Get<int>("vp_v_number").GetValueOrDefault()
                            };
                            list.Add(tr);
                        }
                    }
                }
            }
            return list;
        }

        #endregion

        #region TIMES

        public static object SelectT(bool one)
        {
            CheckImagesConnectionStringThrow();
            CheckPTFConnectionStringThrow();

            #region SQL

            const string SQL = @"select top 1 getdate() from voucher;";

            #endregion

            using (SqlConnection conn = new SqlConnection(one ? Global.Strings.PTFConnString : Global.Strings.ConnString))
            {
                conn.Open();
                using (var comm = new SqlCommand(SQL, conn))
                    return comm.ExecuteScalar();
            }
        }

        #endregion

        #region HISTORY

        public void SaveHistory(int operCountryId, int operUserId, int operType, Guid oper_uniq_id, int br_iso_id, int br_id, int v_id, int v2_id, int count, string details)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"INSERT INTO [History] ([h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details])
                                 VALUES (@iso_id, @user_id, getdate(), @operation, @br_iso_id, @br_id, @v_id, @v2_id, @count, @uniq_id, @details)";
            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", operCountryId);
                    comm.Parameters.AddWithValue("@user_id", operUserId);
                    comm.Parameters.AddWithValue("@operation", operType);
                    comm.Parameters.AddWithValue("@br_iso_id", br_iso_id);
                    comm.Parameters.AddWithValue("@br_id", br_id);
                    comm.Parameters.AddWithValue("@v_id", v_id);
                    comm.Parameters.AddWithValue("@v2_id", v2_id);
                    comm.Parameters.AddWithValue("@count", count);
                    comm.Parameters.AddWithValue("@uniq_id", oper_uniq_id);
                    comm.Parameters.AddWithValue("@details", details);

                    comm.ExecuteNonQuery();
                }
            }
        }

        public class HistoryByCountryData
        {
            public int Index { get; set; }
            public int h_iso_id { get; set; }
            public int h_user_id { get; set; }
            public DateTime h_datetime { get; set; }
            public OperationHistory h_operation { get; set; }
            public int h_br_iso_id { get; set; }
            public int h_br_id { get; set; }
            public int h_v_id { get; set; }
            public int h_v2_id { get; set; }
            public int h_count { get; set; }
            public Guid h_uniq_id { get; set; }
            public string h_details { get; set; }
        }

        public List<HistoryByCountryData> SelectHistoryByCountryAndOperator(int operCountryId, int? operUserId, int operation, DateTime from, DateTime to)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            string SQL = operUserId.HasValue ?
                @"SELECT TOP 1000 ROW_NUMBER ( ) OVER(ORDER BY h_id ASC)  AS 'Index', [h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details] 
                  FROM History WHERE h_iso_id = @iso_id and h_user_id = @user_id and h_operation = @operation and h_datetime between @from and @to;" :

                @"SELECT TOP 1000 ROW_NUMBER ( ) OVER(ORDER BY h_id ASC)  AS 'Index', [h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details] 
                  FROM History WHERE h_iso_id = @iso_id and h_operation = @operation and h_datetime between @from and @to;";
            #endregion

            var list = new List<HistoryByCountryData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", operCountryId);
                    comm.Parameters.AddWithValue("@user_id", operUserId.GetValueOrDefault());
                    comm.Parameters.AddWithValue("@operation", operation);
                    comm.Parameters.AddWithValue("@from", from);
                    comm.Parameters.AddWithValue("@to", to);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new HistoryByCountryData()
                            {
                                Index = reader.Get<int>("Index").GetValueOrDefault(),
                                h_iso_id = reader.Get<int>("h_iso_id").GetValueOrDefault(),
                                h_user_id = reader.Get<int>("h_user_id").GetValueOrDefault(),
                                h_datetime = reader.Get<DateTime>("h_datetime").GetValueOrDefault(),
                                h_operation = (OperationHistory)reader.Get<int>("h_operation").GetValueOrDefault(),

                                h_br_iso_id = reader.Get<int>("h_br_iso_id").GetValueOrDefault(),
                                h_br_id = reader.Get<int>("h_br_id").GetValueOrDefault(),
                                h_v_id = reader.Get<int>("h_v_id").GetValueOrDefault(),
                                h_v2_id = reader.Get<int>("h_v2_id").GetValueOrDefault(),
                                h_count = reader.Get<int>("h_count").GetValueOrDefault(),
                                h_uniq_id = (Guid)reader.GetRaw("h_uniq_id"),
                                h_details = reader.GetString("h_details"),
                            };

                            list.Add(data);
                        }
                    }
                }
            }

            return list;
        }

        #endregion

        #region GENERAL

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public ArrayList RetrieveTableData(string fieldList, string tableName, string where)
        {
            if (tableName.IsNullOrEmpty())
                throw new ArgumentNullException("tableName");

            if (tableName.Contains((c) => !Char.IsLetter(c)))
                throw new SecurityException("Wrong data");

            if (fieldList.IsNullOrEmpty())
                throw new ArgumentNullException("fieldList");

            if (fieldList.Contains((c) => c == ';'))
                throw new SecurityException("Wrong data");

            if (!where.IsNullOrEmpty() && where.Contains((c) => c == ';'))
                throw new SecurityException("Wrong data");

            string SQL = string.Format("SELECT {0} FROM {1} {2};", fieldList, tableName, where);

            ArrayList result = new ArrayList();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (SqlCommand cmd = new SqlCommand(SQL, conn))
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                while (reader.Read())
                    for (int i = 0; i < reader.FieldCount; i++)
                        result.Add(reader[i]);
            return result;
        }

        public int UpdateTableData(Hashtable table)
        {
            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = CreateCommand(conn, table))
            {
                conn.Open();
                return comm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates MSSQL Command object
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private SqlCommand CreateCommand(SqlConnection conn, Hashtable table)
        {
            if (!table.ContainsKey("<key>"))
                throw new Exception("Not authorized");

            DateTime date = DateTime.MinValue;
            if (!DateTime.TryParse(Convert.ToString(table["<key>"]), out date) || date.Date != DateTime.Now.Date)
                throw new Exception("Not authorized");

            string sql = Convert.ToString(table["<sql>"]);
            CommandType type = (CommandType)table["<type>"];
            int timeout = Convert.ToInt32(table["<timeout>"]);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = type;
            comm.CommandTimeout = timeout;
            foreach (DictionaryEntry en in table)
            {
                string name = Convert.ToString(en.Key);
                if (string.Equals(name, "<sql>") || string.Equals(name, "<type>") || string.Equals(name, "<timeout>") || string.Equals(name, "<key>"))
                    continue;
                comm.Parameters.AddWithValue(name, en.Value);
            }
            table.Clear();
            return comm;
        }

        #endregion
    }
}
