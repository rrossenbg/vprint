/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
namespace FintraxPTFImages.Common
{
    [Serializable]
    public class CurrentUser
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int CountryID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName) && CountryID != 0;
            }
        }

        public static CurrentUser Empty = new CurrentUser(0, "", 0);

        public CurrentUser()
        {
        }

        public CurrentUser(int userId, string userName, int countryId)
        {
            UserID = userId;
            UserName = userName;
            CountryID = countryId;
            IsAdmin = userName.Contains("rosen");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userData">UserID, CountryID</param>
        public CurrentUser(string userName, string[] userData)
        {
            UserID = int.Parse(userData[0]);
            UserName = userName;
            CountryID = int.Parse(userData[1]);
            IsAdmin = userName.Contains("rosen");
        }
    }
}