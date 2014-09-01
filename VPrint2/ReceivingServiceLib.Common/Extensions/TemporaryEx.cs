using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ReceivingServiceLib
{
    public static class TemporaryEx
    {
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string FromObject<T>(this XmlSerializer serializer, T value)
        {
            var builder = new StringBuilder();
            using (var str = XmlWriter.Create(new StringWriter(builder)))
            {
                Debug.Assert(str != null);
                serializer.Serialize(str, value);
                return builder.ToString();
            }
        }
    }
}
