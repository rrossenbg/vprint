using System;
using System.Collections.Generic;

namespace HobexCommonLib.Collections
{
    public class CustomDictionary : Dictionary<DateTime, List<Guid>>
    {
        public DateTime Current { get; set; }

        public bool Exists(Guid value)
        {
            lock (this)
            {
                if (!CheckCurrent())
                    return false;
                return this[Current].Contains(value);
            }
        }

        public void Add(Guid value)
        {
            lock (this)
            {
                CheckCurrent();
                this[Current].Add(value);
            }
        }

        public void DeleteButCurrent()
        {
            lock (this)
            {
                this.DeleteAll((i) => i != Current);
            }
        }

        private bool CheckCurrent()
        {
            if (!this.ContainsKey(Current))
            {
                this[Current] = new List<Guid>();
                return false;
            }
            return true;
        }
    }
}
