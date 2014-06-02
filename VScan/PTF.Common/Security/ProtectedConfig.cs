/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Configuration;

namespace PremierTaxFree.PTFLib.Security
{
    public class ProtectedConfig
    {
        /// <summary>
        /// Decript section in application configuration file
        /// </summary>
        /// <param name="sectionName"></param>
        public static void Descript(string sectionName)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.GetSection(sectionName).SectionInformation.UnprotectSection();
            configuration.Save();
        }

        /// <summary>
        /// Encrypt section in application configuration file
        /// </summary>
        /// <param name="sectionName"></param>
        public static void Encrypt(string sectionName)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.GetSection(sectionName).SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            configuration.Save();
        }
    }
}

 
