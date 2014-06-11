using System;
using System.Collections.Generic;
using System.Web;
/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

namespace PTF.Reports.Common
{
    public static class CommonTools
    {
        public static Guid ToGuid(int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(value1));
            bytes.AddRange(BitConverter.GetBytes(value2));
            bytes.AddRange(BitConverter.GetBytes(value3));
            bytes.AddRange(BitConverter.GetBytes(value4));
            return new Guid(bytes.ToArray());
        }

        public static void FromGuid(Guid guid, out int value1, out int value2, out int value3, out int value4)
        {
            var bytes = guid.ToByteArray();
            value1 = BitConverter.ToInt32(bytes, 0);
            value2 = BitConverter.ToInt32(bytes, 4);
            value3 = BitConverter.ToInt32(bytes, 8);
            value4 = BitConverter.ToInt32(bytes, 12);
        }

        /// <summary>
        /// Get cached object or creating if there is no any
        /// </summary>
        /// <typeparam name="T">ObjectType</typeparam>
        /// <param name="name">Instance Name</param>
        /// <param name="createFunct">Create function</param>
        /// <returns></returns>
        public static T Cached<T>(string name, Func<T> createFunct)
        {
            if (HttpContext.Current.Cache[name] == null)
                HttpContext.Current.Cache[name] = createFunct();
            return (T)HttpContext.Current.Cache[name];
        }
    }
}