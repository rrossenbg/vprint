/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using VPrinting.Common;

namespace VPrinting.ScaningProcessors
{
    public interface IScanProcessor
    {
        Action<TaskProcessOrganizer<string>.TaskItem> GetAction();
    }
}
