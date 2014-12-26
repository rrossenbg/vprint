/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.DirectoryServices.AccountManagement;
using System.Reflection;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public class Security
    {
        public static Security CreateInstance()
        {
            return new Security();
        }

        public Tuple<string, string> GenerateSecurityKeys()
        {
            var e1 = DateTime.Now.ToString();
            var e2 = e1.Reverse();
            var s1 = e1.EncryptString();
            var s2 = e2.EncryptString();
            return new Tuple<string, string>(s1, s2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">mydomain</param>
        /// <param name="username">serviceAcct</param>
        /// <param name="password">serviceAcctPass</param>
        /// <returns></returns>
        public bool DomainValidate(string domain, string username, string password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, domain, string.Concat(domain, '\\', username), password))
                return context.ValidateCredentials(username, password);
        }

        [Obfuscation]
        private static DateTime ms_d = new DateTime(2015, 1, 1);

        [Obfuscation]
        public static void Check(bool smart = false)
        {
            if (smart)
            {
                //var now = (DateTime)BaseDataAccess.SelectT(true.Random());
                //if (now > ms_d)
                //    throw new ProjectTimedOutException();
            }
            else
            {
                if (DateTime.Now > ms_d)
                    throw new ProjectTimedOutException();
            }
        }
    }

    public class ProjectTimedOutException : Exception
    {
        public ProjectTimedOutException()
            : base("Project is cancelled. I'm sorry about inconvenience it might cause.")
        {
        }
    }
}
