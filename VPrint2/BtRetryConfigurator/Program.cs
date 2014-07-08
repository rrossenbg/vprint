using System.IO;
using System;

// BtRetryConfigurator /filename "C:\PROJECTS\VPrint\BtRetryServiceDOC\EmailList.txt" /connstring "data source=192.168.57.16;initial catalog=TransferDb;Integrated Security=true;packet size=4096;" /update

namespace BtRetryConfigurator
{
    class Program
    {
        static string FileName { get; set; }
        static string ConnectionString { get; set; }
        static ConsoleColor @default { get; set; }

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            try
            {

                int index = 0;

                foreach (var str in args)
                {
                    switch (str)
                    {
                        case "/filename":
                            FileName = args[index + 1];
                            break;
                        case "/connstring":
                            DataAccess.ConnectionString = args[index + 1];
                            break;
                        case "/export":
                            break;
                        case "/update":
                            {
                                string[] lines = File.ReadAllLines(FileName);
                                foreach (var line in lines)
                                {
                                    if (string.IsNullOrWhiteSpace(line))
                                        continue;
                                    if (line.StartsWith("--"))
                                        continue;
                                    var objs = line.Split(';');
                                    var id = int.Parse(objs[0]);
                                    var emails = objs[1].Trim();

                                    Console.WriteLine(string.Format("update Id {0} value {1}", id, emails));

                                    DataAccess.UpdateEmailList(id, emails);
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                @default = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.BackgroundColor = @default;
            }
            finally
            {
                Console.WriteLine("Done!");
                Console.Read();
            }
        }
    }
}
