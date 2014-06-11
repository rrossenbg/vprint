/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PTF.Reports
{
    public static class ReflectionEx
    {
        /// <summary>
        /// Works for simple types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toObj"></param>
        /// <param name="values"></param>
        public static void SetValues<T>(this T toObj, IDictionary<string, object> values) where T : class
        {
            if (toObj == null || values == null)
                return;
            var type = toObj.GetType();
            var fields = type.GetFields();
            foreach (KeyValuePair<string, object> item in values)
            {
                var field = fields.First(fi => fi.Name == item.Key);
                field.SetValue(toObj, item.Value);
            }
        }

        /// <summary>
        /// Works for simple types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toObj"></param>
        /// <param name="values"></param>
        public static void SetValues<T>(this T toObj, NameValueCollection values) where T : class
        {
            if (toObj == null || values == null)
                return;
            var type = toObj.GetType();
            var properties = type.GetProperties();
            foreach (string key in values.AllKeys)
            {
                var prop = properties.First(fi => string.Equals(fi.Name, key, StringComparison.InvariantCultureIgnoreCase));
                prop.SetValue(toObj, Convert.ChangeType(values[key], prop.PropertyType), null);
            }
        }

        /// <summary>
        /// Copies all fields of an object to the other
        /// </summary>
        /// <typeparam name="T">Type object</typeparam>
        /// <param name="toObj">To object</param>
        /// <param name="fromObj">From object</param>
        /// <remarks>Ref type fields are copied by reference</remarks>
        /// <remarks>Ref type fields should be ICloneable</remarks>
        public static void MergeCopy<T>(this T toObj, T fromObj) where T : class
        {
            if (toObj == null || fromObj == null)
                return;
            var type = toObj.GetType();
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) || field.FieldType.IsValueType)
                {
                    //Structures and string
                    object fromValue = field.GetValue(fromObj);
                    object defaultValue = null;
                    if (field.FieldType.IsValueType)
                        defaultValue = Activator.CreateInstance(field.FieldType);
                    if (fromValue != defaultValue)
                        field.SetValue(toObj, fromValue);
                }
                else
                {
                    var fromValue = field.GetValue(fromObj);
                    var toValue = field.GetValue(toObj);
                    if (fromValue == null)
                        continue;
                    if (toValue == null)
                        toValue = ((ICloneable)fromValue).Clone();
                    else
                        toValue.MergeCopy(fromValue);
                    field.SetValue(toObj, toValue);
                }
            }
        }
    }

    public class Book : Dictionary<string, object>
    {
    }
}
