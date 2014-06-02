/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using PremierTaxFree.PTFLib.Data;


namespace PremierTaxFree.PTFLib.Web
{
    public static class ServiceContext
    {
        /// <summary>
        /// Contains TRS users and Encripted passwords with no salts
        /// </summary>
        public static readonly Hashtable UserTable = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
        /// <summary>
        /// Contains ClientID and mashine IPs
        /// </summary>
        public static readonly Hashtable ClientTable = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
        /// <summary>
        /// Contains any common data that may be shared among clients
        /// </summary>
        public static readonly Hashtable DataTable = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
        /// <summary>
        /// Contains internal data. The data can not be shared with any one
        /// </summary>
        public static readonly Hashtable DataCache = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

        private static DateTime ms_LastLoad;
        private static readonly TimeSpan ONE_HOUR = TimeSpan.FromHours(1);

        public static void Reload(bool force)
        {
            if (force || ms_LastLoad.IsExpired(ONE_HOUR))
            {
                ServerDataAccess.SelectUsers(UserTable);

                ServerDataAccess.SelectClients(ClientTable, true);

                ms_LastLoad = DateTime.Now;
            }
        }
    }
}
