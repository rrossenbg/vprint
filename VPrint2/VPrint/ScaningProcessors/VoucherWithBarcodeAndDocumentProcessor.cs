/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using VPrinting.Common;

namespace VPrinting.ScaningProcessors
{
    public class VoucherWithBarcodeAndDocumentProcessor : IScanProcessor
    {
        public Action<TaskProcessOrganizer<string>.TaskItem> GetAction()
        {
            throw new NotImplementedException();
        }
    }
}
