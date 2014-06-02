/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Data;

namespace PremierTaxFree.PTFLib.Data.Objects
{
    public class DbConfigInfo : IReadable
    {
        public string Key { get; private set; }
        public object Value { get; private set; }

        public void Load(IDataReader reader)
        {
            Key = reader.GetValue<string>("Key");
            Value = reader.GetValue<object>("Value");
        }
    }
}
