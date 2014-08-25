/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CardCodeCover
{
    public static class DataAccess
    {
        public static string ConnectionString { get; set; }

        public class TemplateInfoDb : ILoadable
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ImageSize { get; set; }
            public byte[] Image { get; set; }
            public int TemplateSize { get; set; }
            public byte[] Template { get; set; }
            public string HiddenAreas { get; set; }
            public DateTime CreatedAt { get; set; }

            public ILoadable Load(SqlDataReader reader)
            {
                Id = reader.Get<int>("Id").GetValueOrDefault();
                Name = reader.GetString("Name");
                ImageSize = reader.Get<int>("Size").GetValueOrDefault();
                TemplateSize = reader.Get<int>("TemplateSize").GetValueOrDefault();                
                CreatedAt = reader.Get<DateTime>("CreatedAt").GetValueOrDefault(DateTime.MinValue);
                return this;
            }
        }

        public static void DeleteTemplateById(int id)
        {
            const string SQL = @"DELETE FROM Template WHERE Id=@id;";
            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();
                comm.Parameters.AddWithValue("@Id", id);
                comm.ExecuteNonQuery();
            }
        }

        public static List<TemplateInfoDb> SelectAllTemplateInfos()
        {
            #region SQL

            const string SQL1 = @"SELECT [id]
                                  ,[Name]
                                  ,[Size]
                                  ,[TemplateSize]
                                  ,[CreatedAt]
                              FROM [Template]";
            #endregion

            var list = new List<TemplateInfoDb>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL1, conn))
            {
                conn.Open();

                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read())
                        list.Add((TemplateInfoDb)new TemplateInfoDb().Load(reader));
            }
            return list;
        }

        public static void SelectTemplate(TemplateInfoDb info)
        {
            #region SQL

            const string SQL1 = @"SELECT @AR=HiddenAreas, @IMG=Image, @TMP=TemplateImage FROM [Template] WHERE [id]=@id";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL1, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@id", info.Id);//HiddenAreas = reader.GetString("HiddenAreas");

                var img = new SqlParameter("@IMG", SqlDbType.VarBinary, info.ImageSize);
                img.Direction = ParameterDirection.Output;
                comm.Parameters.Add(img);

                var tmp = new SqlParameter("@TMP", SqlDbType.VarBinary, info.TemplateSize);
                tmp.Direction = ParameterDirection.Output;
                comm.Parameters.Add(tmp);

                var areas = new SqlParameter("@AR", SqlDbType.NVarChar, 1000);
                areas.Direction = ParameterDirection.Output;
                comm.Parameters.Add(areas);

                comm.ExecuteNonQuery();

                info.Image = (byte[])img.Value;
                info.Template = (byte[])tmp.Value;
                info.HiddenAreas = (string)areas.Value;
            }
        }

        public static void UpdateTemplate(TemplateInfoDb info)
        {
            #region SQL

            const string SQL = 
@" MERGE [Template] AS target
    USING (SELECT @Id, @Name, @Size, @Image, @TemplateSize, @TemplateImage, @HiddenAreas) AS 
			source (Id, Name, Size, [Image], [TemplateSize], [TemplateImage], [HiddenAreas])
    ON (target.Id = source.Id)
    WHEN MATCHED THEN 
        UPDATE SET Name = source.Name,
					Size = source.Size,
					[Image] = source.[Image],
					TemplateSize = source.TemplateSize,
					TemplateImage = source.TemplateImage,
					HiddenAreas = source.HiddenAreas
WHEN NOT MATCHED THEN
    INSERT (Name, Size, [Image], [TemplateSize], [TemplateImage], [HiddenAreas], [CreatedAt])
    VALUES (source.Name, source.Size, source.[Image], source.TemplateSize, source.TemplateImage,
				 source.HiddenAreas, getdate());";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                conn.Open();

                comm.Parameters.AddWithValue("@Id", info.Id);
                comm.Parameters.AddWithValue("@Name", info.Name);
                comm.Parameters.AddWithValue("@Size", info.ImageSize);
                comm.Parameters.AddWithValue("@Image", info.Image);
                comm.Parameters.AddWithValue("@TemplateSize", info.TemplateSize);
                comm.Parameters.AddWithValue("@TemplateImage", info.Template);
                comm.Parameters.AddWithValue("@HiddenAreas", info.HiddenAreas);

                comm.ExecuteNonQuery();
            }
        }
    }
}