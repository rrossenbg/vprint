/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Runtime;
using System.Text;

namespace VPrinting
{
    public static class ObjectEx2
    {
        [TargetedPatchingOptOut("na")]
        public static T GetValueAdd<T>(this Hashtable table, T key, Func<T> valueFunct) where T : IConvertible
        {
            if (!table.ContainsKey(key))
                table[key] = valueFunct();
            return (T)Convert.ChangeType(table[key], typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static string Stringify(this object obj)
        {
            return obj != null ? obj.ToString() : string.Empty;
        }       

        [TargetedPatchingOptOut("na")]
        public static string toString(this object obj)
        {
            return Convert.ToString(obj);
        }
    }
}
