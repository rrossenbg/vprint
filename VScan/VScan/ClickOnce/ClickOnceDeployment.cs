/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using TTimer = System.Timers.Timer;

namespace PremierTaxFree
{
    public class ClickOnceDeployment : IDisposable
    {
        public static void CheckForCertificate()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                X509Certificate2 cert = new X509Certificate2(Resources.VScan_Key);
                CertificateInstall.TryAddInStore(cert, StoreName.AddressBook, StoreLocation.CurrentUser, null);
            }
        }

        private TTimer ms_updateTimer = null;

        public void StartListeningForUpdates(TimeSpan period)
        {
            if (ApplicationDeployment.IsNetworkDeployed && ms_updateTimer == null)
            {
                ms_updateTimer = new TTimer();
                ms_updateTimer.Interval = period.TotalMilliseconds;                    
                ms_updateTimer.Elapsed += new ElapsedEventHandler(UpdateTimer_Elapsed);
                ms_updateTimer.Start();
            }
        }

        public void StopListeningForUpdates()
        {
            if (ms_updateTimer != null)
            {
                ms_updateTimer.Stop();
                ms_updateTimer.Elapsed -= new ElapsedEventHandler(UpdateTimer_Elapsed);
                ms_updateTimer.Dispose();
            }
        }

        public void Dispose()
        {
            StopListeningForUpdates();
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment current = ApplicationDeployment.CurrentDeployment;

                try
                {
                    if (current.CheckForUpdate())
                    {
                        Trace.WriteLine("VScan: Check for update");

                        current.Update();

                        DialogResult result = MessageBox.Show("Update downloaded, restart application?",
                        Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                            Application.Restart();
                    }
                }
                catch
                {
                    ms_updateTimer.Stop();
                    MessageBox.Show("ClickOnce connection failed.", Application.ProductName, 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}