/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Runtime;
using System.Xml.Linq;
using System.Diagnostics;

namespace MerchantSite
{
    public static class XmlEx
    {
        [TargetedPatchingOptOut("na")]
        public static XElement ElementThrow(this XContainer container, string name)
        {
            var node = container.Element(name);
            if (node == null)
                throw new Exception("Can not find " + name);
            return node;
        }

        [TargetedPatchingOptOut("na")]
        public static string ElementValueOrDefault(this XContainer container, string name, string @default)
        {
            var node = container.Element(name);
            if (node == null)
                return @default;
            return node.Value;
        }

        [TargetedPatchingOptOut("na")]
        public static string ElementValueOrDefault(this XContainer container, string name, Func<string> funct)
        {
            Debug.Assert(container != null);
            Debug.Assert(name != null);
            Debug.Assert(funct != null);

            var node = container.Element(name);
            if (node == null)
                return funct();
            return node.Value;
        }
    }
}