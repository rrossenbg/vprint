/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;

namespace PTF.Reports.Common
{
    public class ObjectWrapper<T>
    {
        private readonly WeakReference _object;

        public bool IsAlive
        {
            get { return _object.IsAlive; }
        }

        public T Object
        {
            get { return (T)_object.Target; }
        }

        public ObjectWrapper(T obj)
        {
            _object = new WeakReference(obj);
        }
    } 
}
