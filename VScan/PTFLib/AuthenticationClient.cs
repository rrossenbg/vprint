/***************************************************
//  Copyright (c) Premium Tax Free 2013
***************************************************/

using System.Security;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.AuthenticationProxy;
using PremierTaxFree.PTFLib.Net;

namespace PremierTaxFree
{
    public partial class AuthenticationClient
    {
        /// <summary>
        /// Call TRS server to authenticate
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserAuth CallAuthenticateUser(int countryId, string userName, string password)
        {
            using (var client = new AuthenticationSoapClient())
            {
                var result = client.AuthenticateUser(countryId, userName, password);
                if (result.IsNullOrEmpty())
                    throw new SecurityException();

                AuthenticationHeader header = new AuthenticationHeader();
                var userId = client.RetrieveUser(header, countryId, userName);
                var auth = new UserAuth() { CountryID = countryId, Name = userName, Password = password, ClientID = 0  };
                auth["UserID"] = userId; //PTF UserID
                return auth;
            }
        }
    }
}
