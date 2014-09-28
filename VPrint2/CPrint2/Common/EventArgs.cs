/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;

namespace CPrint2.Common
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; set; }

        public ValueEventArgs()
        {
        }

        public ValueEventArgs(T current)
        {
            this.Value = current;
        }
    }

    public class ValueEventArgs<T1, T2> : EventArgs
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }

        public ValueEventArgs()
        {
        }

        public ValueEventArgs(T1 value1, T2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }
}
