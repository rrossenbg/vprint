/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using VPrinting;

namespace ReceivingServiceLib.Data
{
    public class CoverDataAccess : BaseDataAccess
    {
        public static readonly CoverDataAccess Default = new CoverDataAccess();

        public List<int> GetIsos()
        {
            CheckImagesConnectionStringThrow();

            #region

            const string SQL = "select distinct iso_id from [Voucher] order by iso_id;";

            #endregion

            var list = new List<int>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read())
                        list.Add(reader.Get<int>("iso_id").GetValueOrDefault());
            }
            return list;
        }

        public class SelectNextNotCoveredVouchers_Data : IDbReadable
        {
            public int Id { get; set; }
            public int Size { get; set; }
            public string SessionId { get; set; }
            public bool IsProtected { get; set; }

            public IDbReadable Load(SqlDataReader reader)
            {
                Id = reader.Get<int>("id").GetValueOrDefault();
                Size = reader.Get<int>("scan_image_size").GetValueOrDefault();
                SessionId = reader.GetString("session_Id");
                IsProtected = reader.Get<bool>("v_protected").GetValueOrDefault();
                return this;
            }
        }

        public List<SelectNextNotCoveredVouchers_Data> SelectNextNotCoveredVouchers(int id, int iso)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT TOP(5) id, scan_image_size, session_Id, v_protected FROM Voucher 
                                    WHERE id > @id and iso_id = @iso and v_cover is NULL;";

            #endregion

            var list = new List<SelectNextNotCoveredVouchers_Data>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@iso", iso);

                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read())
                        list.Add((SelectNextNotCoveredVouchers_Data)new SelectNextNotCoveredVouchers_Data().Load(reader));
            }

            return list;
        }

        public class SelectTemplates_Data : IDbReadable, IDisposable
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int TemplateSize { get; set; }
            public Bitmap TemplateImage { get; set; }
            public string HiddenAreas { get; set; }

            public IDbReadable Load(SqlDataReader reader)
            {
                Id = reader.Get<int>("id").GetValueOrDefault();
                Name = reader.GetString("Name");
                TemplateSize = reader.Get<int>("TemplateSize").GetValueOrDefault();
                HiddenAreas = reader.GetString("HiddenAreas");
                return this;
            }

            public void Dispose()
            {
                using (TemplateImage) ;
            }
        }

        public List<SelectTemplates_Data> SelectAllTemplatesInfo(int iso)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT [id], [Name], [Size], [Image], [TemplateSize], [HiddenAreas], [CreatedAt] FROM [Template] WHERE IsoId=@iso;";

            #endregion

            var list = new List<SelectTemplates_Data>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@iso", iso);

                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read())
                        list.Add((SelectTemplates_Data)new SelectTemplates_Data().Load(reader));
            }

            return list;
        }

        public void SelectTemplate(SelectTemplates_Data data)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT @TMP = TemplateImage FROM [Template] WHERE Id=@Id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@Id", data.Id);

                var p1 = new SqlParameter("@TMP", SqlDbType.VarBinary, data.TemplateSize);
                p1.Direction = ParameterDirection.Output;
                comm.Parameters.Add(p1);

                comm.ExecuteNonQuery();

                data.TemplateImage = new Bitmap(new MemoryStream((byte[])p1.Value));
            }
        }

        public void SelectVoucher(int id, byte[] buffer, int size)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"SELECT scan_image FROM Voucher WHERE id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@id", id);

                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    if (reader.Read())
                        reader.GetBytes(0, 0, buffer, 0, size);
            }
        }

        public void UpdateVoucher(int id, string cover, int templId)
        {
            CheckImagesConnectionStringThrow();

            #region SQL

            const string SQL = @"UPDATE Voucher SET v_cover = @cover, v_templateid = @templId  WHERE id = @id;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@cover", cover);
                comm.Parameters.AddWithValue("@templId", templId);

                comm.ExecuteNonQuery();
            }
        }
    }
}
