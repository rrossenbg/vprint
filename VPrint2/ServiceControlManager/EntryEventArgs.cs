/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;

namespace FintraxServiceManager
{
    public class EntryEventArgs<T> : EventArgs
    {
        private readonly T m_value;
        public T Value
        {
            get
            {
                return this.m_value;
            }
        }
        public EntryEventArgs(T value)
        {
            this.m_value = value;
        }
    }
}
