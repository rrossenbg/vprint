/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;

namespace PremierTaxFree.PTFLib.Security
{
    public class SSPIHelper
    {
        /// <summary>
        /// Logs in to domain
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        /// <example>
        /// WindowsIdentity wi = SSPIHelper.LogonUser(token.Username, token.Password, "fintrax");
        /// if (wi == null)
        ///     throw new SecurityException();
        /// </example>
        public static WindowsIdentity LogonUser(string userName, string password, string domain)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);

            tcpListener.Start();

            WindowsIdentity id = null;

            tcpListener.BeginAcceptTcpClient(delegate(IAsyncResult asyncResult)
            {
                using (NegotiateStream serverSide = new NegotiateStream(tcpListener.EndAcceptTcpClient(asyncResult).GetStream()))
                {
                    serverSide.AuthenticateAsServer(
                        CredentialCache.DefaultNetworkCredentials, 
                        ProtectionLevel.None, 
                        TokenImpersonationLevel.Impersonation);
                    id = (WindowsIdentity)serverSide.RemoteIdentity;
                }
            }, null);

            using (NegotiateStream clientSide = new NegotiateStream(new TcpClient(IPAddress.Loopback.ToString(), ((IPEndPoint)tcpListener.LocalEndpoint).Port).GetStream()))
            {
                clientSide.AuthenticateAsClient(new NetworkCredential(userName, password, domain), "", ProtectionLevel.None, TokenImpersonationLevel.Impersonation);
            }
            return id;
        }
    }
}
