/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.ServiceProcess;

namespace ReceivingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FintraxReceivingService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
