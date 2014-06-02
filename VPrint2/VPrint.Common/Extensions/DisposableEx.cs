/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Reflection;
using System.Runtime;

namespace VPrinting.Extentions
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class DisposableEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <example>this.BackgroundImage = this.BackgroundImage.DisposeSf();</example>
        [TargetedPatchingOptOut("na")]
        public static T DisposeSf<T>(this T obj) where T : class, IDisposable
        {
            using (obj) { }
            return (T)null;
        }

        [TargetedPatchingOptOut("na")]
        public static void Dispose<T>(this Func<T> act) where T : class, IDisposable
        {
            using (var t = act()) { }
        }
    }
}
