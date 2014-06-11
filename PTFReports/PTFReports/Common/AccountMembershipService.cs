/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Linq;
using System.Web.Security;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Common
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }
        bool ValidateUser(string userName, string password, out UserDetail user);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword, out UserDetail user);
    }

    public class AccountMembershipService : IMembershipService
    {
        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
        }

        public int MinPasswordLength
        {
            get
            {
                return 6;
            }
        }

        public bool ValidateUser(string userName, string password, out UserDetail user)
        {
            if (String.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or empty.", "password");

            return PTFReportsContext.Current.ValidateUser(userName, password, out user);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword, out UserDetail user)
        {
            if (String.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrWhiteSpace(oldPassword))
                throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            user = PTFReportsContext.Current.FindUser(userName, oldPassword);
            if (user != null)
            {
                user.Ud_password = newPassword.Encript();
                PTFReportsContext.Current.SaveChanges();
                return true;
            }
            return false;
        }
    }
}