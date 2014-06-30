/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Net.Security;
using System.ServiceModel;

namespace VPrinting.ScanServiceRef
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
            EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);
            IScanService proxy = ChannelFactory<IScanService>.CreateChannel(tcpBinding, endpointAddress);
            return proxy;
        }
    }
}
