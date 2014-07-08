/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemoteTracerLib
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// static void Main(string[] args)
    /// {
    ///     Console.WriteLine("Client started.");
    ///     RemoteTracerClient client = new RemoteTracerClient(Console.Out);
    ///     Console.Read();
    /// }
    /// </example>
    public class RemoteTracerClient
    {
        public RemoteTracerClient(TextWriter writer)
        {
            TcpServerChannel channel = new TcpServerChannel(15456);
            ChannelServices.RegisterChannel(channel,false);
            RemoteListener resumeListener = new RemoteListener(writer);
            RemotingServices.Marshal(resumeListener, "RemoteListener");
        }
    }
}