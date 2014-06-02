/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Text;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class CoreEx
    {
        [TargetedPatchingOptOut("na")]
        public static unsafe bool AreEqual(this byte[] a, byte[] b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            int len = a.Length / 8;
            if (len > 0)
            {
                fixed (byte* ap = &a[0])
                fixed (byte* bp = &b[0])
                {
                    long* apl = (long*)ap;
                    long* bpl = (long*)bp;

                    for (int i = 0; i < len; i++)
                    {
                        if (apl[i] != bpl[i])
                            return false;
                    }
                }
            }
            int rem = a.Length % 8;
            if (rem > 0)
            {
                for (int i = a.Length - rem; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                        return false;
                }
            }
            return true;
        }

        [TargetedPatchingOptOut("na")]
        public static IList<string> GetNames<T>(this Enum e)
        {
            return Enum.GetNames(typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static Array GetValues<T>(this Enum e)
        {
            return Enum.GetValues(typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static bool In<T>(this T t, params T[] arr) where T : IComparable
        {
            Debug.Assert(t != null);

            foreach (var a in arr)
                if (t.CompareTo(a) == 0)
                    return true;
            return false;
        }

        [TargetedPatchingOptOut("na")]
        public static bool CanConvertTo<T, U>(this T t)
            where T : IComparable
            where U : IComparable
        {
            var conv = TypeDescriptor.GetConverter(typeof(U));
            var result = conv.IsValid(t);
            return result;
        }

        /// <summary>
        /// return t.CompareTo(t1) == 0 ? t2 : t;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T Replace<T>(this T t, T t1, T t2) where T : IComparable<T>
        {
            return t.CompareTo(t1) == 0 ? t2 : t;
        }

        [TargetedPatchingOptOut("na")]
        public static void RemoveAll(this EventHandler source, EventHandler method)
        {
            Delegate.RemoveAll(source, method);
        }

        /// <summary>
        /// return t.CompareTo(t1) == 0 ? t2 : t1;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T Trigger<T>(this T t, T t1, T t2) where T : IComparable
        {
            return t.CompareTo(t1) == 0 ? t2 : t1;
        }

        [TargetedPatchingOptOut("na")]
        public static string ToString2(this byte[] arr)
        {
            Debug.Assert(arr != null);
            var b = new StringBuilder(arr.Length);
            for (int i = 0; i < arr.Length; i++)
                b.Append(arr[i].ToString("X2"));
            return b.ToString();
        }

        /// <summary>
        /// Returns newValue if not default otherwise returns value.
        /// Sample usage:
        /// var a = a.SetIfNotDefault(b);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T SetIfNotDefault<T>(this T value, T newValue)
        {
            if (!object.ReferenceEquals(newValue, default(T)) && !newValue.Equals(default(T)))
                return newValue;
            return value;
        }

        #region ENUMS

        /// [Flags]
        /// public enum Names
        /// {
        ///     None = 0,
        ///     Susan = 1,
        ///     Bob = 2,
        ///     Karen = 4,
        ///     John = 8,
        ///     All = Susan | Bob | Karen | John
        /// }
            
        /// [TestMethod]
        /// public void TestEnums()
        /// {
        ///     bool value1 = Names.All.IsSet(Names.Bob);
        ///     Debug.Assert(value1);
            
        ///     Names friends = Names.Bob | Names.Karen;
        ///     bool value2 = friends.IsSet(Names.Bob);
        ///     Debug.Assert(value2);
            
        ///     friends = friends.UnSet(Names.Karen);
        ///     Debug.Assert(friends == Names.Bob);
        /// }
        [TargetedPatchingOptOut("na")]
        public static bool IsSet<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        [TargetedPatchingOptOut("na")]
        public static T Set<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (T)(object)(flagsValue | flagValue);
        }

        [TargetedPatchingOptOut("na")]
        public static T UnSet<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (T)(object)(flagsValue & (~flagValue));
        }

        #endregion
    }
}
