/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace VPrinting.Colections
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public class UniqueList<T> : List<T>
    {
        [Obfuscation]
        private readonly HashSet<string> m_State = new HashSet<string>();
        private readonly Func<T, string> m_Funct;

        public UniqueList(Func<T, string> funct)
        {
            Debug.Assert(funct != null);
            m_Funct = funct;
        }

        public new bool Add(T value)
        {
            if (m_State.Add(m_Funct(value)))
            {
                base.Add(value);
                return true;
            }
            return false;
        }

        public new void AddRange(IEnumerable<T> values)
        {
            foreach (var t in values)
                if (m_State.Add(m_Funct(t)))
                    base.Add(t);
        }

        public new bool Contains(T value)
        {
            return m_State.Contains(m_Funct(value));
        }

        public new void Clear()
        {
            m_State.Clear();
            base.Clear();
        }
    }
}
