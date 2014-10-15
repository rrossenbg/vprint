//#define SPAIN
#define PORTUGAL

using System;
using System.Collections;
using System.IO;
using VPrint;
using VPrint_addon.Properties;
using VPrinting;
using VPrinting.Common;
using VPrinting.Interfaces;

namespace VPrint_addon
{
    public class Class2 : IRunnable
    {
        private FileInfo m_PfxFileInfo;
        private FileInfo m_LogoFileInfo;
        private static GetMetaDataDelegate ms_GetMetaDataDelegate = new GetMetaDataDelegate(GetMetaData);

#if SPAIN
        public void Run()
        {
            StateSaver.Default.Set(Strings.Certigicate_COUNTRY, "Spain");
            StateSaver.Default.Set(Strings.Certigicate_LOCATION, "Madrid, Spain");

            m_PfxFileInfo = ((FileInfo)null).App("FNMT_Premier.pfx");
            m_PfxFileInfo.DeleteSafe();

            byte[] certificate = Resources.FNMT_Premier;
            m_PfxFileInfo.WriteAllBytes(certificate);

            //////////////////////////////////////////////////////////////////////

            StateSaver.Default.Set(Strings.COUNTRY_CERTIFICATE_PATH, m_PfxFileInfo.FullName);

            StateSaver.Default.Set(Strings.COUNTRY_CERTIFICATE_PASS, Resources.COUNTRY_CERTIFICATE_PASS);

            m_LogoFileInfo = ((FileInfo)null).App("PTFLogo.jpg");
            m_LogoFileInfo.DeleteSafe();
            var logo = Resources.PTFLogo;
            m_LogoFileInfo.WriteAllBytes(logo.ToArray());

            //////////////////////////////////////////////////////////////////////

            StateSaver.Default.Set(Strings.PTFLogoFileFullPath, m_LogoFileInfo.FullName);
            StateSaver.Default.Set(Strings.CERTIFICATE_SIGNING_AVAILABLE, true);

            var prtGetMetaData = ms_GetMetaDataDelegate.GetFunctionPointer();
            StateSaver.Default.Set(Strings.Certigicate_METADATA_FUNC, prtGetMetaData);
        }

#endif

#if PORTUGAL
        public void Run()
        {
            StateSaver.Default.Set(Strings.CERTIFICATE_SIGNING_AVAILABLE, false);
        }
#endif

        public void Exec(object data)
        {
        }

        public void Exit()
        {
            m_PfxFileInfo.DeleteSafe();
            m_LogoFileInfo.DeleteSafe();
        }

#if SPAIN
        public static ArrayList GetMetaData(IServiceData servdata, int iso, int hoid, int reId)
        {
            if (servdata == null)
                throw new ArgumentException("IServiceData");

            var list = ArrayList.Synchronized(new ArrayList());
            var isodata = servdata.RetrieveTableData("iso_country", "ISO", string.Format("where iso_number={0}", iso));
            list.Add(new Tuple<string, string>("Country", isodata[0].Cast<string>()));

            //var rdata = servdata.RetrieveTableData("br_ho_id, br_name, br_trading_name, br_add_1, br_add_3, br_add_5, br_add_6", "Branch", string.Format("where br_iso_id={0} and br_id={1}", iso, reId));
            //if (rdata != null && rdata.Length == 7)
            //{
            //    var odata = servdata.RetrieveTableData("ho_name, ho_trading_name, ho_email_1", "HeadOffice", string.Format("where ho_iso_id={0} and ho_id={1}", iso, rdata[0].Cast<string>()));
            //    if (odata != null && odata.Length == 3)
            //    {

            //        //list.Add(new Tuple<string, string>("Head office name", odata[0].Cast<string>()));
            //        //list.Add(new Tuple<string, string>("Head office Id", rdata[0].Cast<int>().ToString()));
            //        //list.Add(new Tuple<string, string>("Email address", odata[2].Cast<string>()));

            //        //list.Add(new Tuple<string, string>("Retailer name", rdata[1].Cast<string>()));
            //        //list.Add(new Tuple<string, string>("Retailer Id", reId.ToString()));
            //        //list.Add(new Tuple<string, string>("Retailer trading name", rdata[2].Cast<string>()));
            //        //list.Add(new Tuple<string, string>("", ""));
            //        //list.Add(new Tuple<string, string>("Retailer address", rdata[3].Cast<string>()));
            //        //list.Add(new Tuple<string, string>("Town", rdata[5].Cast<string>()));
            //        //list.Add(new Tuple<string, string>("Country", rdata[6].Cast<string>()));
            //    }
            //}

            return list;
        }
#endif

#if PORTUGAL
        public static ArrayList GetMetaData(IServiceData servdata, int iso, int hoid, int reId)
        {
            var list = ArrayList.Synchronized(new ArrayList());
            return list;
        }
#endif
    }
}
