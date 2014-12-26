﻿/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web;
using System.Web.Mvc;
using MerchantSite.Attributes;

namespace MerchantSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrAttribute());
            filters.Add(new HandleErrAsyncAttribute());
        }
    }
}