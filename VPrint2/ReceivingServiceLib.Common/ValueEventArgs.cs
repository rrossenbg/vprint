using System;

namespace ReceivingService
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public ValueEventArgs(T t)
        {
            Value = t;
        }
    }
}
