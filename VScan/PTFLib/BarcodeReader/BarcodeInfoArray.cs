using System;
using System.Collections;
using System.Runtime.Serialization;
using DTKBarReader;

namespace PremierTaxFree.PTFLib
{
    [Serializable]
    public class BarcodeInfoArray : ArrayList, ISerializable
    {
        public new BarcodeInfo this[int index]
        {
            get
            {
                return base[index] as BarcodeInfo;
            }
        }

        public BarcodeInfoArray()
        {
        }

        public BarcodeInfoArray(Barcode[] values)
        {
            foreach (var value in values)
                base.Add(new BarcodeInfo(value));
        }

        public override sealed int Add(object value)
        {
            throw new NotImplementedException("Do not call this method anymore");
        }

        public BarcodeInfoArray(SerializationInfo info, StreamingContext context)
        {
            int count = info.GetInt32("Count");
            for (int i = 0; i < count; i++)
                base.Add(info.GetValue(i.ToString(), typeof(BarcodeInfo)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Count", base.Count);
            for (int i = 0; i < base.Count; i++)
                info.AddValue(i.ToString(), base[i], typeof(BarcodeInfo));
        }
    }
}
