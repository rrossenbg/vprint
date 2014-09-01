/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Reflection;

namespace ReceivingServiceLib.Data
{
    /// <summary>
    /// WARNING: PREPARE SQL SERVER
    /// -----------------------------
    /// -- Reset the "allow updates" setting to the recommended 0
    /// sp_configure 'allow updates',0;
    /// reconfigure with override
    /// go
    /// sp_configure 'show advanced options',0;
    /// reconfigure
    /// go
    /// EXEC sp_configure filestream_access_level, 2
    /// RECONFIGURE
    /// go
    /// </summary>
    [Obfuscation(StripAfterObfuscation = true)]
    public class BaseDataAccess
    {
        protected static void CheckImagesConnectionStringThrow()
        {
            if (string.IsNullOrWhiteSpace(Global.Strings.ConnString))
                throw new ApplicationException("ConnectionString might not be empty");
        }

        protected static void CheckPTFConnectionStringThrow()
        {
            if (string.IsNullOrWhiteSpace(Global.Strings.PTFConnString))
                throw new ApplicationException("PTF ConnectionString might not be empty");
        }
    }
}
