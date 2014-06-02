/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;

namespace PremierTaxFree.PTFLib.Security
{
    public class TokenTable
    {
        public readonly Hashtable Data = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

        private static readonly TokenTable ms_default = new TokenTable();
        public static TokenTable Default
        {
            get { return ms_default; }
        }
    }
}
