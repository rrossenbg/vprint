/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;


namespace PremierTaxFree.PTFLib.Data.Objects
{
    [Serializable]
    public class DbClientInfo : IReadable
    {
        public int ClientID { get; set; }
        public string IP { get; set; }

        public void Load(IDataReader reader)
        {
            ClientID = reader.GetValue<int>("ClientID");
            IP = reader.GetValue<string>("IP");
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ClientID, IP);
        }
    }
}
