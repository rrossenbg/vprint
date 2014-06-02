/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace PremierTaxFree.PTFLib.Collections
{
    [Serializable]
    public class LinkedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private LinkedList<KeyValuePair<TKey, TValue>> m_list = new LinkedList<KeyValuePair<TKey, TValue>>();
        private IEqualityComparer<TKey> m_comp = EqualityComparer<TKey>.Default;

        public void Add(TKey key, TValue value)
        {
            m_list.AddLast(new KeyValuePair<TKey, TValue>(key, value));
        }

        public TValue Get(TKey key)
        {
            return m_list.Where(x => m_comp.Equals(x.Key, key)).First().Value;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_list.GetEnumerator() as IEnumerator<KeyValuePair<TKey, TValue>>;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}
