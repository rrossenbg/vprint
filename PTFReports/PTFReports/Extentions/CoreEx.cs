/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;

namespace PTF.Reports
{
    public static class CoreEx
    {
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        public static string GetName<T>(this Enum en, int index)
        {
            return Enum.GetName(typeof(T), index);
        }

        public static IDictionary<int, string> ToDict<T>(this Enum en)
        {
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));
            var table = new Dictionary<int, string>();
            for (int i = 0; i < values.Length; i++)
                table.Add(Convert.ToInt32(values.GetValue(i)), names[i]);
            return table;
        }

        public static string GenerateString(this Random random, int length)
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";

            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
                chars[i] = allowedChars[random.Next(0, allowedChars.Length)];

            return new string(chars);
        }        
    }
}