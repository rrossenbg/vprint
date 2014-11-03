/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

namespace DEMATLib
{
    public interface IXMLBuilder
    {
        string CreateFileName(long voucherSequenceNumber);
        string CreateXML();
    }
}
