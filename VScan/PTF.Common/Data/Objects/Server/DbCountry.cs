/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;


namespace PremierTaxFree.PTFLib.Data.Objects.Server
{
    /// <summary>
    /// 
    /// </summary>
    /// <value>exec getPtfCountries</value>
    /// <remarks>Use this object on the Server-Side Only</remarks>
    [Serializable]
    public class DbCountry : IReadable
    {
        /// <summary>
        /// 826
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// GB
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// UNITED KINGDOM
        /// </summary>
        public string Name { get; set; }

        public void Load(IDataReader reader)
        {
            CountryId = reader.GetValue<int>("iso_number");
            ShortName = reader.GetValue<string>("iso_2");
            Name = reader.GetValue<string>("iso_country");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

////    create procedure getPtfCountries
////    as
////    begin
////    SELECT iso_number, iso_2, iso_country
////      FROM [ISO]
////    WHERE iso_ptf='Y';
////    end;
