/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace VPrinting.Colections
{
    [Obfuscation(StripAfterObfuscation = true)]
    public class UniqueSyncStringList
    {
        [Obfuscation]
        private readonly HashSet<string> m_Set = new HashSet<string>();
        private DateTime m_LastAdd;

        [Obfuscation]
        public bool Add(string value)
        {
            lock (this)
            {
                if (m_LastAdd.Date != DateTime.Now.Date)
                    m_Set.Clear();

                m_LastAdd = DateTime.Now;

                return m_Set.Add(value);
            }
        }
    }
}
