/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using VPrinting;

namespace MerchantService
{
    public class UserDataAccess
    {
        public static string TRSConnectionString { get; set; }
        public static string ImagesConnectionString { get; set; }

        static UserDataAccess()
        {
            TRSConnectionString = ConfigurationManager.ConnectionStrings["TRSConnectionString"].ConnectionString;
            ImagesConnectionString = ConfigurationManager.ConnectionStrings["ImagesConnectionString"].ConnectionString;
        }

        ////   CREATE TABLE [dbo].[Users](
        ////   [us_id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
        ////   [us_iso_id] [int] NOT NULL,
        ////   [us_first_name] [nvarchar](50) NOT NULL,
        ////   [us_last_name] [nvarchar](50) NOT NULL,
        ////   [us_login] [nvarchar](50) NOT NULL,
        ////   [us_password] [nvarchar](255) NULL,
        ////   [us_email] [nvarchar](255) NULL,
        ////   [us_gp_id] [int] NULL,
        ////   [us_active] [char](1) NOT NULL,
        ////   [us_salt] [nvarchar](50) NULL,
        ////   [us_br_id] [int] NOT NULL,
        ////CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
        public class User_Data
        {
            public int us_id { get; set; }
            public int us_iso_id { get; set; }
            public string us_first_name { get; set; }
            public string us_last_name { get; set; }
            public string us_login { get; set; }
            public string us_password { get; set; }
            public string us_email { get; set; }
            public int us_gp_id { get; set; }
            public bool us_active { get; set; }
            public string us_salt { get; set; }
            public int us_br_id { get; set; }

            public User_Data()
            {
            }

            public User_Data(DbDataReader reader)
            {
                us_id = reader.Get<int>("us_id").GetValueOrDefault();
                us_iso_id = reader.Get<int>("us_iso_id").GetValueOrDefault();
                us_first_name = reader.GetString("us_first_name");
                us_last_name = reader.GetString("us_last_name");
                us_login = reader.GetString("us_login");
                us_password = reader.GetString("us_password");
                us_email = reader.GetString("us_email");
                us_gp_id = reader.Get<int>("us_gp_id").GetValueOrDefault();
                us_active = reader.GetString("us_active") == "Y";
                us_salt = reader.GetString("us_salt");
                us_br_id = reader.Get<int>("us_br_id").GetValueOrDefault();
            }
        }

        public List<User_Data> SelectAllUsers()
        {
            const string SQL = "SELECT * FROM Users";

            var list = new List<User_Data>();

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        while (reader.Read())
                            list.Add(new User_Data(reader));
                }
            }
            return list;
        }

        public List<User_Data> SelectAllUsersByCountry(int isoId)
        {
            const string SQL = "SELECT * FROM Users WHERE us_iso_id = @us_iso_id";

            var list = new List<User_Data>();

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@us_iso_id", isoId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        while (reader.Read())
                            list.Add(new User_Data(reader));
                }
            }
            return list;
        }

        public List<User_Data> SelectAllUsersByBranches(int isoId, params int[] branchIds)
        {
            StringBuilder b = new StringBuilder();
            foreach (var branch in branchIds)
                b.AppendFormat("{0},", branch);

            string NumberLine = b.ToString().TrimEnd(',');

            string SQL = string.Format("SELECT * FROM Users WHERE us_iso_id = @us_iso_id and [us_br_id] in ( {0} ) ", NumberLine);

            var list = new List<User_Data>();

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@us_iso_id", isoId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        while (reader.Read())
                            list.Add(new User_Data(reader));
                }
            }
            return list;
        }

        public User_Data SelectUserById(int userId)
        {
            const string SQL = "SELECT * FROM Users WHERE us_id = @us_id";

            var list = new List<User_Data>();

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@us_id", userId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        if (reader.Read())
                            return new User_Data(reader);
                }
            }
            return new User_Data();
        }

        public void UpdateUser(User_Data data)
        {
            #region SQL

            const string SQL = @"
            MERGE [Users] AS T
            USING (SELECT @us_id, @us_iso_id, @us_first_name, @us_last_name, @us_login, @us_password, @us_email, 
	            @us_gp_id, @us_active, @us_salt, @us_br_id) AS 
            S ([us_id], [us_iso_id], [us_first_name], [us_last_name], [us_login], [us_password], [us_email], 
            [us_gp_id], [us_active], [us_salt], [us_br_id])
            ON (T.us_id = S.us_id) 
            WHEN NOT MATCHED BY TARGET
                THEN 
	            INSERT([us_iso_id], [us_first_name], [us_last_name], [us_login], [us_password], [us_email], 
            [us_gp_id], [us_active], [us_salt], [us_br_id]) 
	            VALUES(S.us_iso_id, S.us_first_name, S.us_last_name, S.us_login, S.us_password, S.us_email, 
            S.us_gp_id, S.us_active, S.us_salt, S.us_br_id)
            WHEN MATCHED 
                THEN UPDATE SET 
	            us_iso_id = S.us_iso_id, 
	            us_first_name = S.us_first_name, 
	            us_last_name = S.us_last_name, 
	            us_login = S.us_login, 
	            us_password = S.us_password, 
	            us_email = S.us_email, 
	            us_gp_id = S.us_gp_id, 
	            us_active = S.us_active, 
	            us_salt = S.us_salt, 
	            us_br_id= S.us_br_id
            --WHEN NOT MATCHED BY SOURCE 
            --    THEN DELETE 
            OUTPUT $action, inserted.*, deleted.*;";

            #endregion

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@us_id", data.us_id);
                    comm.Parameters.AddWithValue("@us_iso_id", data.us_iso_id);
                    comm.Parameters.AddWithValue("@us_first_name", data.us_first_name);
                    comm.Parameters.AddWithValue("@us_last_name", data.us_last_name);
                    comm.Parameters.AddWithValue("@us_login", data.us_login);
                    comm.Parameters.AddWithValue("@us_password", data.us_password);
                    comm.Parameters.AddWithValue("@us_email", data.us_email);
                    comm.Parameters.AddWithValue("@us_gp_id", data.us_gp_id);
                    comm.Parameters.AddWithValue("@us_active", data.us_active);
                    comm.Parameters.AddWithValue("@us_salt", data.us_salt);
                    comm.Parameters.AddWithValue("@us_br_id", data.us_br_id);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void RemoveUser(int us_id)
        {
            #region SQL

            const string SQL = "DELETE [Users] WHERE us_id = @us_id;";

            #endregion

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();
                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@us_id", us_id);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public void SetUserActive(int us_id, bool active)
        {
            const string SQL = @"UPDATE [Users]
                            SET [us_active] = @active
                            WHERE [us_id] = @id";
            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();
                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@id", us_id);
                    comm.Parameters.AddWithValue("@active", active.ToYesNo('Y', 'N'));
                    comm.ExecuteNonQuery();
                }
            }
        }

        ////CREATE TABLE [dbo].[Rights](
        ////    [r_id] [int] NOT NULL,
        ////    [r_us_id] [int] NOT NULL,
        ////    [r_br_iso_id] [int] NOT NULL,
        ////    [r_br_id] [int] NOT NULL,
        ////    [r_active] [char](1) NOT NULL,
        ////    [r_granted_by] [int] NOT NULL,
        ////    [r_granthed_at] [date] NOT NULL,
        //// CONSTRAINT [PK_Rights] PRIMARY KEY CLUSTERED 
        ////(
        ////    [r_id] ASC
        public class Right_Data
        {
            public int r_id { get; set; }
            public int r_us_id { get; set; }
            public int r_ho_iso_id { get; set; }
            public int r_ho_id { get; set; }
            public int r_br_id { get; set; }
            public bool r_active { get; set; }
            public int r_granted_by { get; set; }
            public DateTime r_granthed_at { get; set; }

            public Right_Data()
            {
            }

            public Right_Data(DbDataReader reader)
            {
                r_id = reader.Get<int>("r_id").GetValueOrDefault();
                r_us_id = reader.Get<int>("r_us_id").GetValueOrDefault();
                r_ho_iso_id = reader.Get<int>("r_ho_iso_id").GetValueOrDefault();
                r_ho_id = reader.Get<int>("r_ho_id").GetValueOrDefault();
                r_br_id = reader.Get<int>("r_br_id").GetValueOrDefault();
                r_active = reader.GetString("r_active") == "Y";
                r_granted_by = reader.Get<int>("r_granted_by").GetValueOrDefault();
                r_granthed_at = reader.Get<DateTime>("r_granthed_at").GetValueOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList"></param>
        /// <see cref="http://stackoverflow.com/questions/5595353/how-to-pass-table-value-parameters-to-stored-procedure-from-net-code"/>
        public void UpdateUserRight(IList<Right_Data> datalist)
        {
            const string SQL = @"
               MERGE [Rights] AS T
                USING @Table AS S
                ON (T.r_id = S.r_id) 
                WHEN NOT MATCHED BY TARGET
                    THEN 
	                INSERT([r_us_id], [r_ho_iso_id], [r_ho_id], [r_br_id], [r_active], [r_granted_by], [r_granthed_at]) 
	                VALUES(S.r_us_id, S.r_ho_iso_id, S.r_ho_id, S.r_br_id, S.r_active, S.r_granted_by, S.r_granthed_at)
                WHEN MATCHED 
                    THEN UPDATE SET 
	                r_us_id = S.r_us_id,
	                r_br_ho_id = S.r_br_ho_id, 
                    r_br_ho = S.r_ho_id, 
                    r_br_id = S.r_br_id, 
	                r_active = S.r_active, 
	                r_granted_by = S.r_granted_by, 
	                r_granthed_at = S.r_granthed_at
                WHEN NOT MATCHED BY SOURCE 
                    THEN DELETE 
                OUTPUT $action, inserted.*, deleted.*;";

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@Table", datalist.ToDataTable());
                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}