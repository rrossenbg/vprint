/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace PremierTaxFree.PTFLib.Data.Objects
{
    public class AuditIDSTable
    {
        private readonly static ArrayList ms_Items = ArrayList.Synchronized(new ArrayList());

        public static void AddRange(IEnumerable<string> ids, int maximum)
        {
            lock (ms_Items.SyncRoot)
            {
                if (ms_Items.Count < maximum)
                {
                    foreach (var obj in ids)
                    {
                        ms_Items.Add(obj);
                    }
                }
            }
        }

        public static string SelectRemoveFirstOrEmpty()
        {
            lock (ms_Items.SyncRoot)
            {
                if (ms_Items.Count > 0)
                {
                    string id = Convert.ToString(ms_Items[0]);
                    ms_Items.RemoveAt(0);
                    return id;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
