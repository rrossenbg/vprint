﻿/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

namespace VPrinting
{
    public class CurrentUser
    {
        public int UserID { get; private set; }
        public string Username { get; private set; }
        public string Name { get; private set; }
        public int CountryID { get; private set; }
        public bool IsDomain { get; private set; }

        public CurrentUser(int userId, string username, int countryId, bool isDomain)
        {
            UserID = userId;
            Username = username;
            CountryID = countryId;
            IsDomain = isDomain;
        }

        public override string ToString()
        {
            return string.Concat("Country: ", CountryID, " User: ", UserID);
        }
    }
}
