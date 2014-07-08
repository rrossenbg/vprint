/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Collections.Generic;

namespace SiteCodeLib
{
    [ServiceContract]
    public interface ISiteCode
    {
        [OperationContract]
        Location GetLocation(string code);

        [OperationContract]
        void LoadLocation(Location location);

        [OperationContract]
        void LoadCountry(string CountryID, string iso2);

        [OperationContract]
        string GetShortCode(string countryCode);

        [OperationContract]
        string GetInfo();

        [OperationContract]
        void SaveCommand();
    }

    [DataContract]
    [Serializable]
    public class Location : IEqualityComparer<Location>
    {
        /// <summary>
        /// sq_id
        /// </summary>
        public int Id { get; set; }

        string m_code;
        /// <summary>
        /// ll_code - A, AA
        /// </summary>
        [DataMember]
        public string Code
        {
            get
            {
                Thread.MemoryBarrier();
                return m_code;
            }
            set
            {
                m_code = value;
                Thread.MemoryBarrier();
            }
        }

        int m_Number;

        /// <summary>
        /// sq_number - 18857
        /// </summary>
        [DataMember]
        public int Number
        {
            get
            {
                Thread.MemoryBarrier();
                return m_Number;
            }
            set
            {
                m_Number = value;
                Thread.MemoryBarrier();
            }
        }

        /// <summary>
        /// sq_site - 100018P1348D2
        /// </summary>
        [DataMember]
        public string Site { get; set; }

        /// <summary>
        /// sq_iso_id
        /// </summary>
        [DataMember]
        public int ISO { get; set; }

        public Location()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void NextNumber()
        {
            Number++;
        }

        public override string ToString()
        {
            return string.Format("Site {0} - ISO {1} - Code {2} - Number {3}", Site, ISO, Code, Number);
        }

        public bool Equals(Location x, Location y)
        {
            return x.ISO == y.ISO && x.Site == y.Site;
        }

        public int GetHashCode(Location obj)
        {
            return obj.Site.GetHashCode();
        }
    }
}