/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;
using System.Reflection;
using System;
using System.Diagnostics;

namespace VPrinting.Colections
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public class IgnoreList<T>
    {
        [Obfuscation]
        private readonly HashSet<T> m_State = new HashSet<T>();

        public bool Add(T value)
        {
            lock (this)
            {
                return m_State.Add(value);
            }
        }

        public bool Contains(T value)
        {
            lock (this)
            {
                return m_State.Contains(value);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                m_State.Clear();
            }
        }
    }
   
}
