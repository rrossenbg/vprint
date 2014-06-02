/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Cryptography.X509Certificates;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Extensions;
using PremierTaxFree.PTFLib.Security;
using SecurityInstaller.Properties;

namespace SecurityInstaller
{
    [RunInstaller(true)]
    public partial class CertificateInstaller : Installer
    {
        public CertificateInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                var cert = new X509Certificate2(Resources.PTF, "Stanley2011");
                if (CertificatesUtils.FindCertificateInStore(X509FindType.FindBySerialNumber, Strings.VScan_CertificateSerialNumber, false) == null)
                    cert.Install(StoreName.My);
            }
            catch (Exception ex)
            {
                ex.ShowDialog();
            }
            finally
            {
                base.Install(stateSaver);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                var cert = new X509Certificate2(Resources.PTF, "Stanley2011");
                cert.Uninstall(StoreName.My);
            }
            catch (Exception ex)
            {
                ex.ShowDialog();
            }
            finally
            {
                base.Uninstall(savedState);
            }
        }
    }
}
