using System.IO;
using System.Runtime;
using System.Xml.Linq;

namespace VPrinting
{
    public static class XElementEx
    {
        [TargetedPatchingOptOut("na")]
        public static byte[] ToArray(this XElement xml)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                xml.Save(memory, SaveOptions.DisableFormatting);
                return memory.ToArray();
            }
        }
    }
}
