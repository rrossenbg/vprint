using System;

namespace WCFNamedPipesExample.Host
{
    /// <summary>
    /// Helper class for generic event payload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T t)
        {
            Data = t;
        }

        public T Data { get; private set; }
    }
}
