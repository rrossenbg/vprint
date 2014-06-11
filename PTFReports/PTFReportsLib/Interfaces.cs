/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;

namespace PTF.Reports
{
    public interface INamedObject
    {
        string GetName();
        Guid GetID();
    }
}
