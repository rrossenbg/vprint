using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Threading;

namespace VPrinting.Communication
{
    public delegate string ReceivedDataDelegate(string data);

    public class NamedPipes
    {
        public static event ThreadExceptionEventHandler Error;
        public static event ReceivedDataDelegate ReceivedData;

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
