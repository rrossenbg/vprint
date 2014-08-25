/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CardCodeCover
{
    public static class CommonEx
    {
        [TargetedPatchingOptOut("na")]
        public static IDisposable DisposeSf(this IDisposable obj)
        {
            using (obj)
                return obj;
        }

        [TargetedPatchingOptOut("na")]
        public static string FromObject<T>(this T value)
        {
            Debug.Assert(value != null);
            var serializer = new XmlSerializer(typeof(T));
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(new StringWriter(builder)))
            {
                Debug.Assert(writer != null);
                serializer.Serialize(writer, value);
                return builder.ToString();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T ToObject<T>(this string text)
        {
            Debug.Assert(!string.IsNullOrEmpty(text));

            var formatter = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(reader != null);
                return (T)formatter.Deserialize(reader);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
