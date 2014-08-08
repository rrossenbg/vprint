/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace SiteCodeLib
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.Single,
                        ConcurrencyMode = ConcurrencyMode.Single)]
    public class SiteCodeObject : ISiteCode
    {
        public readonly ConcurrentDictionary<string, string> m_CountryIDToShortCountryCode = new ConcurrentDictionary<string, string>();

        public readonly ConcurrentDictionary<string, Location> m_SiteToLocation = new ConcurrentDictionary<string, Location>();

        public readonly ConcurrentDictionary<int, ZNumber> m_CountryIDToMaxChar = new ConcurrentDictionary<int, ZNumber>();

        public event EventHandler Save;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site">100018P1348D2</param>
        /// <returns></returns>
        public Location GetLocation(string site)
        {
            Location location = null;
            if (m_SiteToLocation.TryGetValue(site, out location))
            {
                lock (location)
                {
                    location.NextNumber();
                    return location;
                }
            }
            else
            {
                var iso = Tools.TryParseCountryID(site);
                if (!iso.HasValue)
                    throw new FaultException("Cannot find CountryID");

                ZNumber zNumber = null;

                lock (zNumber = m_CountryIDToMaxChar.GetOrAdd(iso.Value, new ZNumber()))
                {
                    zNumber.Increase();

                    location = new Location() { ISO = iso.Value, Code = zNumber.ToString(), Site = site };
                    location.NextNumber();

                    m_SiteToLocation[site] = location;
                    return location;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iso">826</param>
        /// <returns></returns>
        public string GetShortCode(string iso)
        {
            string countryCode = null;
            if (m_CountryIDToShortCountryCode.TryGetValue(iso, out countryCode))
                return countryCode;
            return null;
        }

        public void SetLocations(IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                if (string.IsNullOrWhiteSpace(location.Code))
                    continue;

                m_SiteToLocation[location.Site] = location;

                var znum = ZNumber.Parse(location.Code);

                if (!m_CountryIDToMaxChar.ContainsKey(location.ISO) || m_CountryIDToMaxChar[location.ISO] < znum)
                    m_CountryIDToMaxChar[location.ISO] = znum;
            }
        }

        public void SetCountries(IEnumerable<KeyValuePair<string, string>> countryCodes)
        {
            foreach (var pair in countryCodes)
                m_CountryIDToShortCountryCode[pair.Key] = pair.Value;
        }

        public IEnumerable<Location> GetLocations()
        {
            foreach (var item in new ConcurrentDictionary<string, Location>(m_SiteToLocation))
                yield return item.Value;
        }

        public void Clear()
        {
            m_CountryIDToShortCountryCode.Clear();
            m_SiteToLocation.Clear();
        }

        public string GetInfo()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("=== In memory Site Codes ==== ");
            foreach (var item in m_SiteToLocation)
                b.AppendLine(item.Value.ToString());

            b.AppendLine();

            b.AppendLine("=== In memory Country Codes ==== ");
            foreach (var item in m_CountryIDToMaxChar)
                b.AppendFormat("{0} - {1} \r\n", item.Key, item.Value);

            b.AppendLine();
            return b.ToString();
        }

        public void SaveCommand()
        {
            if (Save != null)
                Save(this, EventArgs.Empty);
        }

        public void LoadLocation(Location location)
        {
            SetLocations(new Location[] { location });
        }

        public void LoadCountry(string CountryID, string iso2)
        {
            m_CountryIDToShortCountryCode[CountryID] = iso2;
        }
    }
}
