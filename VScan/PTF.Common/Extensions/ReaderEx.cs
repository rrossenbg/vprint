/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;

namespace PremierTaxFree.PTFLib
{
    public static class ReaderEx
    {
        /// <summary>
        /// Gets value from reader by value name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            if (reader.IsDBNull(index))
                return default(T);
            return (T)Convert.ChangeType(reader.GetValue(index), typeof(T));
        }

        /// <summary>
        /// Gets byte array from reader by name
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this IDataReader reader, string name)
        {
            return GetBytes(reader, name, int.MaxValue);
        }

        /// <summary>
        /// Gets byte array from reader by name having maximum length
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this IDataReader reader, string name, int maxLength)
        {
            int index = reader.GetOrdinal(name);
            if (reader.IsDBNull(index))
                return null;
            int length = Convert.ToInt32(reader.GetBytes(index, 0, null, 0, 0));
            byte[] buffer = new byte[length];
            reader.GetBytes(index, 0, buffer, 0, Math.Min(length, maxLength));
            return buffer;
        }
    }
}
