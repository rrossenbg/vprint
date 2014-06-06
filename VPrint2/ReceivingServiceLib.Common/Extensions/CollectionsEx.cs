using System;
using System.Collections.Generic;

namespace ReceivingServiceLib
{
    public static class CollectionsEx
    {
        public static T Max<T>(this IEnumerable<T> items, Func<T, T, bool> func)
            where T : class
        {
            T tt = default(T);

            foreach (var t in items)
                if (tt == null || func(t, tt))
                    tt = t;

            return tt;
        }
    }
}
