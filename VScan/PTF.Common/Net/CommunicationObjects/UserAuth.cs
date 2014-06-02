/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace PremierTaxFree.PTFLib.Net
{
    [Serializable]
    public class UserAuth : IComparable<UserAuth>
    {
        /// <summary>
        /// "User", "User123"
        /// </summary>
        public static UserAuth Default = new UserAuth(0, "User", "User123", false);
        /// <summary>
        /// "Admin", "Admin123
        /// </summary>
        public static UserAuth DefaultAdmin = new UserAuth(0, "Admin", "Admin123", true);
        /// <summary>
        /// "rosen", "rosen"
        /// </summary>
        public static UserAuth Rosen = new UserAuth(826, "rosen", "rosen", true);

        public int CountryID { get; set; }
        public int ClientID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        private readonly Dictionary<string, object> Data = new Dictionary<string, object>();

        public object this[string name]
        {
            get
            {
                return Data[name];
            }
            set
            {
                Data[name] = value;
            }
        }

        [XmlIgnore]
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password); }
        }

        public UserAuth()
            : this(826, "NA", "NA")
        {
        }

        public UserAuth(int countryId, string user, string pass, bool isAdmin)
            : this(countryId, user, pass)
        {
            IsAdmin = isAdmin;
        }

        public UserAuth(int countryId, string user, string pass)
        {
            Name = user;
            Password = pass;
            CountryID = countryId;
            IsAdmin = false;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(Name);
            b.AppendLine(Password);
            b.AppendLine(CountryID.ToString());
            b.AppendLine(IsAdmin.ToString());
            b.AppendLine(ClientID.ToString());
            return b.ToString();
        }

        public int CompareTo(UserAuth other)
        {
            if (other == null)
                return -1;

            if (string.Compare(Name, other.Name) == 0 &&
                string.Compare(Password, other.Password) == 0 &&
                IsAdmin == other.IsAdmin)
                return 0;

            return 1;
        }
    }

    [Serializable]
    public class UserAuthComparer : IEqualityComparer<UserAuth>
    {
        public bool Equals(UserAuth x, UserAuth y)
        {
            Debug.Assert(x != null);
            Debug.Assert(y != null);

            return x.CompareTo(y) == 0;
        }

        public int GetHashCode(UserAuth obj)
        {
            Debug.Assert(obj != null);
            return obj.Name != null ? obj.Name.GetHashCode() : 0;
        }
    }
}
