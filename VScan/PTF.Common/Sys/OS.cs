/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;


namespace PremierTaxFree.PTFLib.Sys
{
    public static class OS
    {
        /// <summary>
        /// Installs windows service. NET 2 only. 
        /// </summary>
        /// <param name="servicePath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool InstallService(string servicePath, TimeSpan timeout)
        {
            try
            {
                string path = Registry.LocalMachine.ReadKey<string>(@"SOFTWARE\Microsoft\.NETFramework", "InstallRoot", null);

                if (string.IsNullOrEmpty(path))
                    throw new IOException("Can't find regisrty key");

                string installUtil = Path.Combine(path, @"v2.0.50727\InstallUtil.exe");

                int result = ExecuteCommand(string.Format("{0} \"{1}\"", installUtil, servicePath), Convert.ToInt32(timeout.TotalMilliseconds));
                return result == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Uninstall windows service
        /// </summary>
        /// <param name="servicePath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool UnInstallService(string servicePath, TimeSpan timeout)
        {
            try
            {
                string path = Registry.LocalMachine.ReadKey<string>(@"SOFTWARE\Microsoft\.NETFramework", "InstallRoot", null);

                if (string.IsNullOrEmpty(path))
                    throw new IOException("Can't find regisrty key");

                string installUtil = Path.Combine(path, @"v2.0.50727\InstallUtil.exe");

                int result = ExecuteCommand(string.Format("{0} -u \"{1}\"", installUtil, servicePath), Convert.ToInt32(timeout.TotalMilliseconds));
                return result == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Starts a windows service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool StartService(string serviceName, TimeSpan timeout)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Starts stops service
        /// </summary>
        /// <param name="serviceName">MSSQLSERVER</param>
        public static void StartService(string serviceName, bool start)
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName.Equals(serviceName))
                {
                    if (start && service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                    }
                    else if (!start && service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Tests service is installed and running. Safe
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string TestService(string serviceName, TimeSpan timeout)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Refresh();
                return service.Status == ServiceControllerStatus.Running ? null : "Service is not running"; 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Sends command to a windows service
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="SystemException"></exception>
        /// <example>ExecuteCommand(string.Format("{0} \"{1}\"", installUtil, servicePath), Convert.ToInt32(timeout.TotalMilliseconds));</example>
        public static int ExecuteCommand(string Command, int timeoutMs)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/C " + Command);
#if !HIDE
            info.CreateNoWindow = true;
#endif
            info.UseShellExecute = false;
            Process shellProcess = Process.Start(info);
            shellProcess.WaitForExit(timeoutMs);
            int exitCode = shellProcess.ExitCode;
            shellProcess.Close();
            return exitCode;
        }

        /// <summary>
        /// Deletes windows folder
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
                dir.Delete(true);
        }
    }
}
