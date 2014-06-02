/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;

namespace PremierTaxFree.PTFLib
{
    public static class EquatableEx
    {
        /// <summary>
        /// Inverts a set of values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static T Invert2<T>(this T t, T v1, T v2) where T : IEquatable<T>
        {
            Debug.Assert(t != null);
            return t.Equals(v1) ? v2 : v1;
        }
    }
}
