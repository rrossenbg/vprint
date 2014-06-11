/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Collections.Generic;

namespace BackupRestore
{
    public interface INamedList
    {
        string Name { get; set; }
        IList<string> Values { get; set; }
    }
}
