/*
*	Copyright © 2011 Premier Tax Free
*	DbUserInfo
*/

using System.Data;


namespace PremierTaxFree.PTFLib.Data.Objects
{
    public class DbUserInfo : IReadable
    {
        public int UserID { get; set; }
        /// <summary>
        /// Login
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Password hash
        /// </summary>
        public string Password { get; set; }

        public void Load(IDataReader reader)
        {
            UserID = reader.GetValue<int>("us_id");
            Name = reader.GetValue<string>("us_login");
            Password = reader.GetValue<string>("us_password");
        }
    }

    /// <summary>
    /// GetUserLoginDetail
    /// </summary>
    public class DbPasswordInfo : IReadable
    {
        public string EncryptedPassword { get; set; }
        public string Salt { get; set; }

        public void Load(IDataReader reader)
        {
            EncryptedPassword = reader.GetValue<string>("us_password");
            Salt = reader.GetValue<string>("us_salt");
        }
    }
}
