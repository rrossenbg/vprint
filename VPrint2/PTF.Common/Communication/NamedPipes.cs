/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace VPrinting.Communication
{
    public delegate string ReceivedDataDelegate(string data);

    public class NamedPipes
    {
        public static event ThreadExceptionEventHandler Error;
        public static event ReceivedDataDelegate ReceivedData;

        /// <summary>
        /// Server code
        /// </summary>
        /// <param name="pipename"></param>
        /// <example>
        /// NamedPipes.ReceivedData += new ReceivedDataDelegate(NamedPipes_ReceivedData);
        /// NamedPipes.Error += new ThreadExceptionEventHandler(NamedPipes_Error);
        /// NamedPipes.StartServer("VPRINT");
        /// 
        /// private string NamedPipes_ReceivedData(string data){}
        /// </example>
        public static void StartServer(string pipename)
        {
            Task.Factory.StartNew((o) =>
            {
                while (!Global.Instance.ExitSignal)
                {
                    try
                    {
                        var server = new NamedPipeServerStream(Convert.ToString(o));
                        server.WaitForConnection();
                        using (StreamReader reader = new StreamReader(server))
                        using (StreamWriter writer = new StreamWriter(server))
                        {
                            var line = reader.ReadLine();
                            if (ReceivedData != null)
                            {
                                var responce = ReceivedData(line);
                                writer.WriteLine(responce);
                                writer.Flush();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Error != null)
                            Error(typeof(NamedPipes), new ThreadExceptionEventArgs(ex));
                    }
                }
            }, pipename);
        }

        /// <summary>
        /// Client code
        /// </summary>
        /// <param name="pipename"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <example>
        /// NamedPipes.SendMessage("VCOVER", b.ToString());
        /// </example>
        public static string SendMessage(string pipename, string message)
        {
            var client = new NamedPipeClientStream(pipename);
            client.Connect();
            using (StreamReader reader = new StreamReader(client))
            using (StreamWriter writer = new StreamWriter(client))
            {
                writer.WriteLine(message);
                writer.Flush();
                return reader.ReadLine();
            }
        }
    }
}
