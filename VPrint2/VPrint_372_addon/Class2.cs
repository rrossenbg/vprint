#define SPAIN

using System.IO;
using VPrint;
using VPrint_addon.Properties;
using VPrinting;
using VPrinting.Common;

namespace VPrint_addon
{
    public class Class2 : IRunnable
    {
        private FileInfo m_PfxFileInfo;
        private FileInfo m_LogoFileInfo;

        public void Run()
        {
#if SPAIN
            StateSaver.Default.Set(Strings.Certigicate_COUNTRY, "Spain");
            StateSaver.Default.Set(Strings.Certigicate_LOCATION, "Madrid, Spain");

            m_PfxFileInfo = ((FileInfo)null).Temp(".pfx");
            byte[] certificate = Resources.FNMT_Premier;
            m_PfxFileInfo.WriteAllBytes(certificate);
            
            StateSaver.Default.Set(Strings.COUNTRY_CERTIFICATE_PATH, m_PfxFileInfo.FullName);
            
            StateSaver.Default.Set(Strings.COUNTRY_CERTIFICATE_PASS, Resources.COUNTRY_CERTIFICATE_PASS);

            m_LogoFileInfo = ((FileInfo)null).Temp();
            var logo = Resources.PTFLogo;
            m_LogoFileInfo.WriteAllBytes(logo.ToArray());
            StateSaver.Default.Set(Strings.PTFLogoFileFullPath, m_LogoFileInfo.FullName);
            StateSaver.Default.Set(Strings.CERTIFICATE_SIGNING_AVAILABLE, true);
#endif
        }

        public void Exec(object data)
        {
        }

        public void Exit()
        {
            m_PfxFileInfo.DeleteSafe();
            m_LogoFileInfo.DeleteSafe();
        }
    }
}
