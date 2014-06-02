/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;

namespace PremierTaxFree.PTFLib.Threading
{
    public static class DataSlot
    {
        private readonly static Hashtable ms_Table = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Gets value from slot by value name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static T Get<T>(string dataName)
        {
            return (T)ms_Table[dataName];
        }

        /// <summary>
        /// Sets data to slot by name
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="data"></param>
        public static void Set(string dataName, object data)
        {
            ms_Table[dataName] = data;
        }

        /// <summary>
        /// Frees the slot by name
        /// </summary>
        /// <param name="dataName"></param>
        public static void Free(string dataName)
        {
            ms_Table.Remove(dataName);
        }
    }
}
