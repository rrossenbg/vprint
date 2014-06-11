/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;

namespace PTF.Reports.Common
{
    [Serializable]
    public class ValueList
    {
        public string Value { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(Value);
            }
        }

        public IEnumerable<T> Split<T>(char ch)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                var str = Value.Split(ch);
                foreach (var v in str)
                    if (!string.IsNullOrWhiteSpace(v))
                        yield return (v.Cast<T>());
            }
        }
    }
}