/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Data;


namespace PremierTaxFree.PTFLib.Data.Objects
{
    public struct DbId : IReadable
    {
        public string ID { get; set; }

        public void Load(IDataReader reader)
        {
            ID = reader.GetValue<string>("ID");
        }
    }

    /// <summary>
    /// No data id found.
    /// </summary>
    public class NoDataIdException : AppExclamationException
    {
        public NoDataIdException()
            : base("No data id found.")
        {
        }
    }
}
