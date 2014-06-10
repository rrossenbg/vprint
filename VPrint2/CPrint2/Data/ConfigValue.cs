using System;
using System.Collections;

namespace CPrint2.Data
{
    public class ConfigValue<T>
    {
        private static readonly Hashtable ms_Table = Hashtable.Synchronized(new Hashtable());

        public ConfigValue(string value)
        {
            var t = Convert.ChangeType(value, typeof(T));
            ms_Table[this] = t;
        }

        public T GetValue()
        {
            return (T)ms_Table[this];
        }
    }
}
