using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;

namespace VPrintTest
{
    [TestClass]
    public class OtherTest
    {
        [TestMethod]
        public void line_parse_test()
        {
            var value = "1, 2, 4-5, 7-8, 100";
            var numberList = new List<int>();

            value = value.Replace(" ", "");

            var ranges = value.Split(',', ';');

            foreach (var range in ranges)
            {
                var numbers = range.Split('-');
                switch (numbers.Length)
                {
                    case 1:
                        {
                            int from = int.Parse(numbers[0], CultureInfo.InvariantCulture);
                            numberList.Add(from);
                        }
                        break;
                    case 2:
                        {
                            int from = int.Parse(numbers[0], CultureInfo.InvariantCulture);
                            int to = int.Parse(numbers[1], CultureInfo.InvariantCulture);
                            if (from > to)
                                throw new Exception();

                            for (int i = from; i <= to; i++)
                                numberList.Add(i);
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }

            Debug.WriteLine(numberList);
        }

        [TestMethod]
        public void TestBizTalk()
        {
            string connString = "data source=192.168.58.57;initial catalog=ptf;persist security info=False;user id=sa;pwd=sa;packet size=4096;";

            Hashtable rtable = Hashtable.Synchronized(new Hashtable());

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("getIsoTypes", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    //read the refund amount
                    while (reader.Read())
                    {
                        var iso = reader.GetInt32(0);
                        var iso_type = reader.GetNullString(1);
                        rtable[iso] = iso_type;
                    }
                }
            }
        }

        [TestMethod]
        public void TryParseLong()
        {
            long l = long.Parse("4000000000000052");
            var dec = System.Convert.ToDecimal("0.5");
        }

        [TestMethod]
        public void SplitJoinSafe()
        {
            //12.34.56.78.9
            var result1 = "123456789".SplitJoinSafe(2, ".");

            //123-456-78
            var result2 = "12345678".SplitJoinSafe(3, "-");

            //123x456x780x0
            var result3 = "1234567800".SplitJoinSafe(3, "x");
        }
    }

    public static class ExE
    {
        public static string GetNullString(this SqlDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }
    }
}
