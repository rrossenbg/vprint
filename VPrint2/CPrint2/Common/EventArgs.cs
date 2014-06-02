/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;

namespace CPrint2.Common
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public ValueEventArgs(T current)
        {
            this.Value = current;
        }
    }
}
