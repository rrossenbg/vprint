using System;
using System.Net.Security;
using System.ServiceModel;

namespace FintraxPTFImages.ScanServiceRef
{
    partial class ScanServiceClient : IDisposable
    {
        void IDisposable.Dispose()
        {
            if (this.State == CommunicationState.Faulted)
            {
                this.Abort();
            }
            else
            {
                this.Close();
            }
        }

        public static ScanServiceClient Instance { get { return new ScanServiceClient(); } }

        /// <example>
        ////    IScanService client = null;
        ////    try
        ////    {
        ////        client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
        ////        var keys = Security.CreateInstance().GenerateSecurityKeys();
        ////        var list = client.ReadHistory(Program.currentUser.CountryID, Program.currentUser.UserID,
        ////            data, fromTime, toTime, keys.Item1, keys.Item2);
        ////        return list;
        ////    }
        ////    finally
        ////    {
        ////        ((IDisposable)client).DisposeSf();
        ////    }
        /// </example>
        public static IScanService CreateProxy()
        {
            ///net.tcp://localhost:8080/ReceivingServiceLib.ScanService/mex
            var tcpBinding = GetBinding();
            var endpointAddress = GetEnpoint();
            IScanService proxy = ChannelFactory<IScanService>.CreateChannel(tcpBinding, endpointAddress);
            return proxy;
        }

        public static NetTcpBinding GetBinding()
        {
            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.MaxBufferSize = 2147483647;
            tcpBinding.MaxBufferPoolSize = 2147483647;
            tcpBinding.MaxReceivedMessageSize = 2147483647;
            tcpBinding.ReaderQuotas.MaxDepth = 2147483647;
            tcpBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            tcpBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            tcpBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            tcpBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            tcpBinding.TransactionFlow = false;
            tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            tcpBinding.Security.Mode = SecurityMode.None;
            return tcpBinding;
        }

        public static EndpointAddress GetEnpoint()
        {
            const string endPointAddr = "net.tcp://192.168.53.143:8080/ReceivingServiceLib.ScanService";
            EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);
            return endpointAddress;
        }
    }
}