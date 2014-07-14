/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

namespace FintraxPTFImages.Common
{
    public class CurrentUser
    {
        public int UserID { get; private set; }
        public string UserName { get; private set; }
        public int CountryID { get; private set; }

        public static CurrentUser Empty = new CurrentUser(0, "", 0);

        public bool IsAdmin { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName) && CountryID != 0;
            }
        }

        public CurrentUser(int userId, string userName, int countryId)
        {
            UserID = userId;
            UserName = userName;
            CountryID = countryId;
            IsAdmin = userName.Contains("rosen");
        }
    }
}