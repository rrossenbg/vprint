﻿using System.ServiceProcess;

namespace RestartService
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
				new RestartService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
