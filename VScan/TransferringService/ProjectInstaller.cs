/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.ComponentModel;
using System.Configuration.Install;
using PremierTaxFree.PTFLib.Security;

namespace TransferringService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
