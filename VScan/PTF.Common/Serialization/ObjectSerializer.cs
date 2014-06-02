/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace PremierTaxFree.PTFLib.Serialization
{
    public class ObjectSerializer
    {
        private IFormatter mFormatter;

        public ObjectSerializer(bool useBinary)
        {
            mFormatter = (useBinary) ?
                (IFormatter)new BinaryFormatter() :
                (IFormatter)new SoapFormatter();
        }

        /// <summary>
        /// Serializes an object to byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public byte[] Serialize<T>(T @object)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                mFormatter.Serialize(memory, @object);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Deserializes an object from byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] buffer)
        {
            using (MemoryStream memory = new MemoryStream(buffer))
            {
                T @object = (T)mFormatter.Deserialize(memory);
                return @object;
            }
        }

        /// <summary>
        /// Deserializes an object from part of byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] buffer, int length)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                memory.Write(buffer, 0, length);
                T @object = (T)mFormatter.Deserialize(memory);
                return @object;
            }
        }

        /// <summary>
        /// Deserializes an object from part of byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] buffer, int position, int length)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                memory.Write(buffer, position, length);
                T @object = (T)mFormatter.Deserialize(memory);
                return @object;
            }
        }
    }
}
