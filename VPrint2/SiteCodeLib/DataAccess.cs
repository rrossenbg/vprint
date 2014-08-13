/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;

namespace SiteCodeLib
{
    public class DataAccess
    {
        public static string ConnectionString { private get; set; }

        public static IEnumerable<Location> LoadLocationsFromVoucherPart()
        {
            #region S Q L

            const string SQL = @"
            DECLARE @iso_id int, @site_code varchar(10), @location_number int;
            DECLARE @T1 TABLE(iso_id int, site_code varchar(10), location_number int);

            DECLARE CUR2 CURSOR
            FOR 
            SELECT  vp_iso_id, vp_site_code
            FROM VoucherPart
            WHERE vp_site_code IS NOT NULL and vp_location_number IS NOT NULL
            GROUP BY vp_iso_id, vp_site_code;

            OPEN CUR2

            FETCH NEXT FROM CUR2 
            INTO @iso_id, @site_code;

            WHILE @@FETCH_STATUS = 0 
            BEGIN 
   
               SELECT @location_number = max(vp_location_number)
               FROM VoucherPart 
               WHERE vp_iso_id = @iso_id and vp_site_code = @site_code;
   
               INSERT INTO @T1(iso_id, site_code, location_number) 
               VALUES(@iso_id, RIGHT(@site_code, LEN(@site_code)-1), @location_number);
   
               FETCH NEXT FROM CUR2 
               INTO @iso_id, @site_code;

            END 
            CLOSE CUR2;
            DEALLOCATE CUR2;

            SELECT * FROM @T1";

            #endregion

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SQL, ConnectionString);
            da.SelectCommand.CommandTimeout = 0;
            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Location()
                    {
                        ISO = row["iso_id"].Get<int>(),
                        Code = row["site_code"].Get<string>(),
                        Number = row["location_number"].GetNull<int>().GetValueOrDefault(),
                    };
                }
            }
        }

        public static IEnumerable<Location> LoadLocationsFromLocations()
        {
            #region S Q L

            const string SQL = @"
            SELECT *
            FROM [SQ_Location] 
            INNER JOIN [LocationLookup] ON sq_site = ll_site 
            WHERE sq_sequence = 'Location'";
            
            #endregion

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SQL, ConnectionString);
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var site = row["sq_site"].Get<string>();
                if (string.IsNullOrWhiteSpace(site))
                    throw new ApplicationException("sq_site may not be empty");

                var sqisoid = row["sq_iso_id"].Get<int>();
                var iso = Tools.TryParseCountryID(site);
                if (!iso.HasValue && sqisoid == 0)
                    continue;

                yield return new Location()
                {
                    Id = row["sq_id"].Get<int>(),
                    Site = site,
                    Code = row["ll_code"].Get<string>(),
                    Number = row["sq_number"].Get<int>(),
                    ISO = iso.GetValueOrDefault(sqisoid),
                };
            }
        }

        public static IEnumerable<Location> LoadLocationsFromFile(string fileName)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Location>));
            using (var file = File.OpenRead(fileName))
            {
                List<Location> list = (List<Location>)formatter.Deserialize(file);
                foreach (var item in list)
                    yield return item;
            }
        }

        public static List<Location> JoinLocations(IEnumerable<Location> voucherPartLocations, IEnumerable<Location> lookupLocations)
        {
#if VERY_OLD_CODE
            var q1 = from vp in voucherPartLocations
                     join ll in lookupLocations on new { vp.ISO, vp.Code } equals new { ll.ISO, ll.Code }
                     select new Location() { Id = ll.Id, ISO = vp.ISO, Code = vp.Code, Number = vp.Number, Site = ll.Site };
            return q1.ToList();
#elif OLD_CODE
            var q1 = from ll in lookupLocations
                     join vp in voucherPartLocations on new { ll.ISO, ll.Code } equals new { vp.ISO, vp.Code } into j1
                     from j2 in j1.DefaultIfEmpty()
                     select new Location() { Id = ll.Id, ISO = ll.ISO, Code = ll.Code, Number = (j2 != null ? j2.Number : ll.Number), Site = ll.Site };
            return q1.ToList();
#else
            var dict = new Hashtable();
            var list = new List<Location>();

            foreach (var lloc in lookupLocations)
                dict[lloc.ISO + lloc.Code] = lloc;

            foreach (var ploc in voucherPartLocations)
            {
                var lloc = (Location)dict[ploc.ISO + ploc.Code];
                if (lloc != null)
                {
                    ploc.Site = lloc.Site;
                    list.Add(ploc);
                }
            }
            dict.Clear();
            return list;
#endif
        }

        public static void SaveLocations(IEnumerable<Location> items)
        {
            #region S Q L

            const string SQL0 = @"DELETE FROM LocationLookup; DELETE FROM SQ_Location;";

            const string SQL1 = @"INSERT INTO LocationLookup(ll_code, ll_site) VALUES (@ll_code, @ll_site);";

            const string SQL2 = @"INSERT INTO SQ_Location (sq_sequence, sq_number, sq_site, sq_iso_id) VALUES ('Location', @sq_number, @sq_site, @sq_iso_id);";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction("SAVELOCATIONS"))
                {
                    try
                    {
                        using (var comm = new SqlCommand(SQL0, conn, tran))
                            comm.ExecuteNonQuery();

                        foreach (var loc in items)
                        {
                            using (var comm = new SqlCommand(SQL1, conn, tran))
                            {
                                comm.Parameters.AddWithValue("@ll_code", loc.Code);
                                comm.Parameters.AddWithValue("@ll_site", loc.Site);
                                comm.ExecuteNonQuery();
                            }

                            using (var comm = new SqlCommand(SQL2, conn, tran))
                            {
                                comm.Parameters.AddWithValue("@sq_id", loc.Id);
                                comm.Parameters.AddWithValue("@sq_number", loc.Number);
                                comm.Parameters.AddWithValue("@sq_site", loc.Site);
                                comm.Parameters.AddWithValue("@sq_iso_id", loc.ISO != 0 ? loc.ISO : 826);
                                comm.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void SaveLocationsToFile(IEnumerable<Location> items, string fileName)
        {
            List<Location> list = new List<Location>(items);
            XmlSerializer formatter = new XmlSerializer(typeof(List<Location>));
            using (var file = File.OpenWrite(fileName))
                formatter.Serialize(file, list);
        }

        public static IEnumerable<KeyValuePair<string, string>> LoadCountries()
        {
            const string SQL = @"SELECT * FROM ISO";

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(SQL, ConnectionString);
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
                yield return new KeyValuePair<string, string>(
                    row["iso_number"].Get<string>(),
                    row["iso_shortcode"].Get<string>());
        }
    }
}
