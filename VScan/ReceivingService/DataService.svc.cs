using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web.Configuration;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Messages;
using PremierTaxFree.PTFLib.Web;
using ReceivingService.VoucherEntryAndModificationProxy;

namespace ReceivingService
{
    [ServiceBehavior(   InstanceContextMode = InstanceContextMode.PerCall,
                        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DataService : IDataService
    {

#if DEBUG
        private const int AUDITIDS_COUNT = 10;
#else
        private const int AUDITIDS_COUNT = 100;
#endif

        public DataService()
        {
            ServerDataAccess.ConnectionString = WebConfigurationManager.AppSettings[Strings.WebService_ConnectionString].ToStringSf();
            ServerDataAccess.ConnectionStringPft = WebConfigurationManager.AppSettings[Strings.WebService_ConnectionStringPTF].ToStringSf();
            ServiceContext.DataCache[Strings.TransferringService_Key] = new DataPair<string, string>("@:LDkfif()_!xd23", "982*723!EFjhdu;<");
        }

        public void ValidateUser(UserData data)
        {
            if (data == null)
                throw new ArgumentException();

            Trace.Write("ReceivingService::ValidateUser(countryId,'user','pass')");

            if (!data.IsSuperUser)
            {
                string encriptedPassword = null;

                var key = (DataPair<string, string>)ServiceContext.DataCache[Strings.TransferringService_Key];

                if (!ServerDataAccess.ValidateUser(key, data.CountryID, data.Name, data.Pass, ref encriptedPassword))
                {
                    Trace.WriteLine("-Invalid");
                    throw new SecurityException();
                }
            }

            Trace.WriteLine("-Valid");
        }

        public int CreateClient(string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
                return -1;

            Trace.WriteLine(string.Format("ReceivingService::CreateClient('{0}')", machineName));

            int id;
            ServerDataAccess.CreateClient(machineName, ClientIP, out id);
            return id;
        }

        public List<CountryData> QueryContries()
        {
            Trace.WriteLine("ReceivingService::QueryContries()");
            List<CountryData> list = ServerDataAccess.SelectCountries().ConvertAll(c => 
                new CountryData { CountryId=c.CountryId, Name = c.Name, ShortName = c.ShortName });
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="countryID"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        /// <example>countryId = 300, P2FileNumber = 100018P2300D2</example>
        public List<string> QuerySiteCodes(int clientID, int countryID, string P2FileNumber, int count)
        {
            Trace.WriteLine("ReceivingService::QuerySiteCodes()");

            var siteCodes = new List<string>();

            var proxy = new VoucherEntryAndModificationSoapClient();

            for (int i = 0; i < count; i++)
            {
                var code = proxy.GenerateSiteCode(countryID, P2FileNumber);
                ServerDataAccess.InsertFile(clientID, countryID, code);
                siteCodes.Add(code);
            }
            return siteCodes;
        }

        public void SaveVoucher(VoucherData data, int clientID)
        {
            Trace.WriteLine("ReceivingService::SaveVoucher()");

            if (data == null)
                throw new ArgumentException();

            ServerDataAccess.UpdateFile(
                clientID,
                data.CountryID,
                data.RetailerID,
                data.VoucherID,
                data.SiteCode,
                data.VoucherImage,
                data.BarCodeImage,
                data.Comment,
                data.DateCreated);
        }

        public void SaveMessages(List<MessageData> data, int clientID)
        {
            Trace.WriteLine("ReceivingService::SaveMessages()");

            if (data == null)
                throw new ArgumentException();
        }

        public string Test()
        {
            Trace.WriteLine("ReceivingService::Test()");
            return DateTime.Now.ToString();
        }

        private string ClientIP
        {
            get
            {
                var props = OperationContext.Current.IncomingMessageProperties;
                var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpointProperty != null)
                    return endpointProperty.Address;
                return string.Empty;
            }
        }

        public object SendCmd(string name, params object[] values)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Trace.WriteLine(string.Format("ReceivingService::SendCommand('{0}', values)", name));

            switch (name)
            {
                case "reload":
                    ServiceContext.Reload(true);
                    break;
                case "enable_client":
                    Debug.Assert(values != null && values.Length == 2);
                    ServerDataAccess.UpdateClient(values[0].Cast<int>(), values[1].Cast<bool>());
                    break;
                case "client_list":
                    var clients = new Hashtable();
                    ServerDataAccess.SelectClients(clients, false);
                    var list = new ArrayList();
                    clients.ForEach<DictionaryEntry>((e) => { list.Add(e.Key); list.Add(e.Value); });
                    return list;
                case "delete_batch":
                    Debug.Assert(values != null && values.Length == 1);
                    ServerDataAccess.DeleteFilesByBatchID( 0, (Guid)values[0]);
                    break;
                default:
                    return null;
            }
            return null;
        }

        private static void OnError(object sender, ThreadExceptionEventArgs args)
        {
            Trace.WriteLine("ReceivingWebService::OnInsertErrorCallback send to msmq -> ".concat(Strings.All_SaveQueueName));
            using (SQLWorker.CommandInfo info = (SQLWorker.CommandInfo)sender)
            {
                Debug.Assert(info != null, "Info is null");
                Hashtable ht = SQL.CreateSerializationData(info.Command);
                MSMQ.SendToQueue(Strings.All_SaveQueueName, DateTime.Now.ToString(), ht);
            }
        }
    }
}