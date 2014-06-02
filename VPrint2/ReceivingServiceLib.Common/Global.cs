/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections.Concurrent;
using System.Reflection;

namespace ReceivingServiceLib
{
    [Obfuscation]
    public static class Global
    {
        [Obfuscation]
        public static Strings Strings { get; set; }

        [Obfuscation]
        public static readonly ConcurrentDictionary<string, object> Data = new ConcurrentDictionary<string, object>();

        static Global()
        {
            Strings = Strings.Read();
        }
    }
}
