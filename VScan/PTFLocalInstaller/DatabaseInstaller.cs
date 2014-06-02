/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LocalDbInstaller.Properties;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Sys;

namespace LocalDbInstaller
{
    [RunInstaller(true)]
    public partial class DatabaseInstaller : Installer
    {
        /// <summary>
        /// "DATA"
        /// </summary>
        const string DBINSTALLPATH = "DATA";

        public DatabaseInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
#if ADD_DB_TO_SQL_SERVER_DATA
            //var targetdir = this.Context.Parameters["assemblypath"];
            //Debug.Assert(targetdir != null);
            //string path = Path.Combine(Path.GetPathRoot(targetdir), DBINSTALLPATH);
#else //INSTALL ON SYSTEM DRIVE
            
            string path = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), DBINSTALLPATH);
#endif
            try
            {
                if (ClientDataAccess.TestConnection() != null)
                {
                    string sql = string.Format(Resources.PTFLocalInstallScript, path);
                    ClientDataAccess.SetupDatabase(path, sql);
                }
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
            string path = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), DBINSTALLPATH);
            if (MessageBox.Show("Would you like to drop the database?\r\nKeep in mind if there are unsent data will be lost.", "Installer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string msg;
                if ((msg = ClientDataAccess.DropDatabaseSafe(path)) != null)
                    MessageBox.Show(msg, "Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            base.Uninstall(savedState);
        }
    }
}
