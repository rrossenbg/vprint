/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;

namespace CPrint2.Common
{
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
    }
}
