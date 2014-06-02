using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace BarReaderLib
{
    public static class CommonTools
    {
        public static byte[] Serialize(object value)
        {
            Debug.Assert(value != null);
            using (MemoryStream memory = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memory, value);
                return memory.ToArray();
            }
        }
    }
}
