/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Web.Configuration;

namespace PTF.Reports
{
    public static class Config
    {
        public static T Get<T>(string name)
        {
            return WebConfigurationManager.AppSettings[name].Cast<T>();
        }
    }
}