/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;

namespace ReceivingServiceLib
{
    public interface IExceptionToFaultConverter
    {
        object ConvertExceptionToFaultDetail(Exception error);
    }
}
