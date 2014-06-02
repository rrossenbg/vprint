using System;
using System.Collections;
using DTKBarReader;

namespace BarReaderLib
{
    [Serializable]
    public class BarcodeInfoArray : ArrayList
    {
        public BarcodeInfoArray(Barcode[] values)
        {
            foreach (var value in values)
                base.Add(new BarcodeInfo(value));
        }

        public override sealed int Add(object value)
        {
            throw new NotImplementedException();
        }
    }
}
