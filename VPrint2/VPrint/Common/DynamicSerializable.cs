/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace VPrinting.Common
{
    /// <summary>
    /// Use dynamics with WCF
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/7501846/xml-serialize-dynamic-object"/>
    [Serializable]
    public class DynamicSerializable : DynamicObject, ISerializable
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!dictionary.ContainsKey(binder.Name))
            {
                dictionary.Add(binder.Name, value);
            }
            else
            {
                dictionary[binder.Name] = value;
            }

            return true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kvp in dictionary)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }
    }

    [KnownType(typeof(DynamicSerializable))]
    [DataContract]
    public class Root
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public dynamic DynamicValues { get; set; }
    }
}
