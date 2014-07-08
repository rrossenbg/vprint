/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.ServiceProcess;

namespace SiteCodeSrvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new FintraxSiteCodeService() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
