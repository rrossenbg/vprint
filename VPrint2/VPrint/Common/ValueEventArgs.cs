﻿using System;

namespace VPrinting
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
