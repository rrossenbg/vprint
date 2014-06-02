/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;


namespace PremierTaxFree.PTFLib.Data.Objects
{
    public class DbMessageInfo : IReadable
    {
        public int MessageID { get; private set; }
        public string Message { get; private set; }
        public int Type { get; private set; }
        public string Source { get; private set; }
        public string StackTrace { get; private set; }
        public DateTime DateInserted { get; private set; }

        public void Load(IDataReader reader)
        {
            MessageID = reader.GetValue<int>("MessageID");
            Message = reader.GetValue<string>("Message");
            Type = reader.GetValue<int>("Type");
            Source = reader.GetValue<string>("Source");
            StackTrace = reader.GetValue<string>("StackTrace");
            DateInserted = reader.GetValue<DateTime>("DateInserted");
        }
    }
}
