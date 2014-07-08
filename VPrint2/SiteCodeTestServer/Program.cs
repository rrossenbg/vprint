using System;
using System.Data.SqlClient;
using System.IO;
using System.ServiceModel;
using SiteCodeLib;

namespace SiteCodeTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            DataAccess.ConnectionString = "data source=192.168.58.57;initial catalog=ptf;persist security info=False;............;packet size=4096;";
            //DataAccess.ConnectionString = "data source=192.168.58.27;initial catalog=ptf;persist security info=False;user id=sa;pwd=Singapore123;packet size=4096;";

            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerData.xml");

            if (File.Exists(fileName))
            {
                DataAccess.SaveLocations(DataAccess.LoadLocationsFromFile(fileName));
                File.Delete(fileName);
            }

            SiteCodeObject server = new SiteCodeObject();
            server.LoadLocations(DataAccess.LoadLocationsFromLocations());
            server.LoadCountries(DataAccess.LoadCountries());
            var svcHost = new ServiceHost(server);
            svcHost.Open();
            Console.Write("Click to exit");
            Console.ReadLine();
            try
            {
                DataAccess.SaveLocations(server.SaveLocations());
            }
            catch (SqlException)
            {
                DataAccess.SaveLocationsToFile(server.SaveLocations(), fileName);
            }
        }
    }
}
