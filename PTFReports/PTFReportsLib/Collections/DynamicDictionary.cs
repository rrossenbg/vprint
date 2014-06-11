/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Collections.Generic;
using System.Dynamic;

namespace PTF.Reports.Collections
{
    public class DynamicDictionary : DynamicObject
    {
        private readonly Dictionary<string, object> m_dictionary = new Dictionary<string, object>();

        public int Count
        {
            get
            {
                return m_dictionary.Count;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return m_dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            m_dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }
}
