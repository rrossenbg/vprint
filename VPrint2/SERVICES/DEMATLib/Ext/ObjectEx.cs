/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DEMATLib
{
    public static class ObjectEx
    {
        public static string Serialize<T>(this T obj)
        {
            Debug.Assert(obj != null);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream mem = new MemoryStream())
            {
                ser.Serialize(mem, obj);
                return Encoding.UTF8.GetString(mem.ToArray());
            }
        }

        public static T Deserialize<T>(this string xml)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(xml));
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                T obj = (T)ser.Deserialize(mem);
                return obj;
            }
        }
    }
}
