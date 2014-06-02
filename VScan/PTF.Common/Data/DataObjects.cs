/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;

namespace PremierTaxFree.PTFLib.Data
{
    [Serializable]
    public class DataPair<K,V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public DataPair()
        {
        }
        public DataPair(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class DataTriple<K, V1, V2>
    {
        public K Key { get; set; }
        public V1 Value1 { get; set; }
        public V2 Value2 { get; set; }
        public DataTriple(K key, V1 value1, V2 value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
        }
    }
}
