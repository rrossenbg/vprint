using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPrintTest
{
    [TestClass]
    public class CertTests
    {
        [TestMethod]
        public void Test_Whole_Functionality()
        {
            const string IMAGE_FOLDER_IN = @"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (12)\";
            const string IMAGE_FOLDER_OUT = @"C:\Users\Rosen.rusev\Pictures\Presenter\";
            const string COMMAND_FOLDER = @"C:\COMMAND\";

            Random rnd = new Random();

            for (int i = 0; i < 30; i++)
            {
                int voucherId = rnd.Next(24567890, 34567890);

                File.WriteAllText(COMMAND_FOLDER + rnd.Next() + ".txt", "826;123456;" + voucherId + ";1;False");
                Thread.Sleep(400);
                File.Copy(GetRandomFileName(IMAGE_FOLDER_IN), IMAGE_FOLDER_OUT + rnd.Next() + ".jpg");

                Thread.Sleep(400);

                File.WriteAllText(COMMAND_FOLDER + rnd.Next() + ".txt", "826;123456;" + voucherId + ";1;False");
                Thread.Sleep(400);
                File.Copy(GetRandomFileName(IMAGE_FOLDER_IN), IMAGE_FOLDER_OUT + rnd.Next() + ".jpg");

                Thread.Sleep(400);

                File.WriteAllText(COMMAND_FOLDER + rnd.Next() + ".txt", "826;123456;" + voucherId + ";1;True");
                Thread.Sleep(400);
            }
        }

        private string GetRandomFileName(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            var rnd = new Random();
            return files[rnd.Next(0, files.Length - 1)];
        }
    }
}
