using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime;

namespace VPrinting
{
    public static class SqlEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        /// using (SqlCommand cmd = new SqlCommand("update voucher set v_vsr_number=@a where v_iso_id=@iso and v_number=@n"))
        /// {
        ///     cmd.Parameters.AddWithValue("@a", "123");
        ///     cmd.Parameters.AddWithValue("@iso", 752);
        ///     cmd.Parameters.AddWithValue("@n", 24);
        ///     var cl = new ServiceReference1.PartyManagementSoapClient();
        ///     cl.UpdateTableData(new ServiceReference1.AuthenticationHeader(), cmd.CreateSerializationData().ToList().ToArray());
        /// }
        [TargetedPatchingOptOut("na")]
        public static Hashtable CreateSerializationData(this IDbCommand comm)
        {
            Debug.Assert(comm != null);

            Hashtable table = new Hashtable();
            table.Add("<sql>", comm.CommandText);
            table.Add("<type>", comm.CommandType);
            table.Add("<timeout>", comm.CommandTimeout);
            table.Add("<key>", DateTime.Now);

            foreach (IDbDataParameter p in comm.Parameters)
                table.Add(p.ParameterName, p.Value);

            return table;
        }
    }
}
