/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Reflection;

namespace PTF.Reports
{
    public static class ReflectionEx
    {
        public static U SafeGetValue<T, U>(this T obj, string propertyName, U defaultValue)
                where T : class
        {
            if (obj == null)
                return defaultValue;

            Type t = typeof(T);
            PropertyInfo info = t.GetProperty(propertyName);
            return (U)info.GetValue(obj, null);
        }
    }
}
