using System;
using System.Net.Security;
using System.ServiceModel;

namespace CPrint2.ScanServiceRef
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

        public static IScanService CreateProxy(string ip)
        {
            ///net.tcp://localhost:8080/ReceivingServiceLib.ScanService/mex
            var endPointAddr = string.Concat("net.tcp://", ip, ":8080/ReceivingServiceLib.ScanService");
            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.TransactionFlow = false;
            tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            tcpBinding.Security.Mode = SecurityMode.None;
            EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);
            IScanService proxy = ChannelFactory<IScanService>.CreateChannel(tcpBinding, endpointAddress);
            return proxy;
        }
    }
}
