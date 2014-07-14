/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Runtime;
using System.Globalization;

namespace FintraxPTFImages
{
    public static class CoreEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool EqualsNoCase(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception">If name != null && value == null</exception>
        [TargetedPatchingOptOut("na")]
        public static U ConvertTo<T, U>(this T value, string name = null) where T : IConvertible
        {
            if (name != null && value == null)
                throw new Exception(name + " can not be null");
            try
            {
                return (U)Convert.ChangeType(value, typeof(U), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Join(" ", name, "can not be converted to", typeof(U)), ex);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static string Unique(this string value)
        {
            return Guid.NewGuid().ToString().Trim('-');
        }

        /// <summary>
        /// (T)Convert.ChangeType(value, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T cast<T>(this string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

    }
}