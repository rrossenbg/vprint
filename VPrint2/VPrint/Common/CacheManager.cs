/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;

namespace VPrinting.Common
{
    public class CacheManager
    {
        public readonly Hashtable Table = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

        protected CacheManager()
        {
        }

        public static CacheManager Instance = new CacheManager();

        public void Get<T>(Guid id, out T value, Func<T> getFunct) where T: class, IKeyable
        {
            Debug.Assert(getFunct != null);

            if (!Table.ContainsKey(id))
            {
                value = getFunct();
                if (value == null)
                    throw new InvalidOperationException("Get function failed");

                value.SetKey(id);
                Table[id] = value;
            }
            else
            {
                value = (T)Table[id];
            }
        }

        public void Clear()
        {
            Table.Clear();
        }
    }
}
