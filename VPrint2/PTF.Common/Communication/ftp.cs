using System;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace VPrinting.Communication
{
    /*
        ftp ftpClient = new ftp(@"ftp://10.10.10.10/", "user", "password");

        ftpClient.upload("etc/test.txt", @"C:\Users\metastruct\Desktop\test.txt");

        ftpClient.download("etc/test.txt", @"C:\Users\metastruct\Desktop\test.txt");

        ftpClient.delete("etc/test.txt");

        ftpClient.rename("etc/test.txt", "test2.txt");

        ftpClient.createDirectory("etc/test");

        string fileDateTime = ftpClient.getFileCreatedDateTime("etc/test.txt");
        Console.WriteLine(fileDateTime);

        string fileSize = ftpClient.getFileSize("etc/test.txt");
        Console.WriteLine(fileSize);

        string[] simpleDirectoryListing = ftpClient.directoryListDetailed("/etc");
        for (int i = 0; i < simpleDirectoryListing.Count(); i++) { Console.WriteLine(simpleDirectoryListing[i]); }

        string[] detailDirectoryListing = ftpClient.directoryListDetailed("/etc");
        for (int i = 0; i < detailDirectoryListing.Count(); i++) { Console.WriteLine(detailDirectoryListing[i]); }
     */
    public class FtpClient
    {
        private string m_host = null;
        private string m_user = null;
        private string m_pass = null;
        private FtpWebRequest m_ftpRequest = null;
        private FtpWebResponse m_ftpResponse = null;
        private Stream m_ftpStream = null;
        private const int BUFFERSIZE = 2048;

        /* Construct Object */
        public FtpClient(string hostIP, string userName, string password)
        {
            m_host = hostIP; 
            m_user = userName; 
            m_pass = password;
        }

        /* Download File */
        public void download(string remoteFile, string localFile)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Get the FTP Server's Response Stream */
            m_ftpStream = m_ftpResponse.GetResponseStream();
            /* Open a File Stream to Write the Downloaded File */
            FileStream localFileStream = new FileStream(localFile, FileMode.Create);
            /* Buffer for the Downloaded Data */
            byte[] byteBuffer = new byte[BUFFERSIZE];
            int bytesRead = m_ftpStream.Read(byteBuffer, 0, BUFFERSIZE);
            /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
            try
            {
                while (bytesRead > 0)
                {
                    localFileStream.Write(byteBuffer, 0, bytesRead);
                    bytesRead = m_ftpStream.Read(byteBuffer, 0, BUFFERSIZE);
                }
            }
            finally
            {
                /* Resource Cleanup */
                localFileStream.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
            }
        }

        /* Upload File */
        public void upload(string remoteFile, string localFile)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + remoteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            /* Establish Return Communication with the FTP Server */
            m_ftpStream = m_ftpRequest.GetRequestStream();
            /* Open a File Stream to Read the File for Upload */
            FileStream localFileStream = new FileStream(localFile, FileMode.Create);
            /* Buffer for the Downloaded Data */
            byte[] byteBuffer = new byte[BUFFERSIZE];
            int bytesSent = localFileStream.Read(byteBuffer, 0, BUFFERSIZE);
            /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
            try
            {
                while (bytesSent != 0)
                {
                    m_ftpStream.Write(byteBuffer, 0, bytesSent);
                    bytesSent = localFileStream.Read(byteBuffer, 0, BUFFERSIZE);
                }
            }
            finally
            {
                /* Resource Cleanup */
                localFileStream.Close();
                m_ftpStream.Close();
            }
        }

        /* Delete File */
        public void delete(string deleteFile)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)WebRequest.Create(m_host + "/" + deleteFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Resource Cleanup */
            m_ftpResponse.Close();
        }

        /* Rename File */
        public void rename(string currentFileNameAndPath, string newFileName)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)WebRequest.Create(m_host + "/" + currentFileNameAndPath);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.Rename;
            /* Rename the File */
            m_ftpRequest.RenameTo = newFileName;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Resource Cleanup */
            m_ftpResponse.Close();
        }

        /* Create a New Directory on the FTP Server */
        public void createDirectory(string newDirectory)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)WebRequest.Create(m_host + "/" + newDirectory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Resource Cleanup */
            m_ftpResponse.Close();
        }

        /* Get the Date/Time a File was Created */
        public string getFileCreatedDateTime(string fileName)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + fileName);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            m_ftpStream = m_ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(m_ftpStream);
            /* Store the Raw Response */
            string fileInfo = null;
            /* Read the Full Response Stream */
            try
            {
                fileInfo = ftpReader.ReadToEnd();
            }
            finally
            {
                /* Resource Cleanup */
                ftpReader.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
                /* Return File Created Date Time */
            }
            return fileInfo;
        }

        /* Get the Size of a File */
        public string getFileSize(string fileName)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + fileName);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            m_ftpStream = m_ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(m_ftpStream);
            /* Store the Raw Response */
            string fileInfo = null;
            /* Read the Full Response Stream */
            try
            {
                while (ftpReader.Peek() != -1)
                    fileInfo = ftpReader.ReadToEnd();
            }
            finally
            {
                /* Resource Cleanup */
                ftpReader.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
            }
            /* Return File Size */
            return fileInfo;
        }

        /* List Directory Contents File/Folder Name Only */
        public string[] directoryListSimple(string directory)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + directory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            m_ftpStream = m_ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(m_ftpStream);
            /* Store the Raw Response */
            string directoryRaw = null;
            /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
            try
            {
                while (ftpReader.Peek() != -1)
                    directoryRaw += ftpReader.ReadLine() + "|";
            }
            finally
            {
                /* Resource Cleanup */
                ftpReader.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
            }
            /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */

            string[] directoryList = directoryRaw.Split("|".ToCharArray());
            return directoryList;
        }

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */
        public string[] directoryListDetailed(string directory)
        {
            /* Create an FTP Request */
            m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + directory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
            /* When in doubt, use these options */
            m_ftpRequest.UseBinary = true;
            m_ftpRequest.UsePassive = true;
            m_ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            m_ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            /* Establish Return Communication with the FTP Server */
            m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
            /* Establish Return Communication with the FTP Server */
            m_ftpStream = m_ftpResponse.GetResponseStream();
            /* Get the FTP Server's Response Stream */
            StreamReader ftpReader = new StreamReader(m_ftpStream);
            /* Store the Raw Response */
            string directoryRaw = null;
            /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
            try
            {
                while (ftpReader.Peek() != -1)
                    directoryRaw += ftpReader.ReadLine() + "|";
            }
            finally
            {
                /* Resource Cleanup */
                ftpReader.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
            }
            string[] directoryList = directoryRaw.Split("|".ToCharArray()); 
            return directoryList;
        }
    }
}
