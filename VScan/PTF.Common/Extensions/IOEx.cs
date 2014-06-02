/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;

namespace PremierTaxFree.PTFLib
{
    public static class IOEx
    {
        /// <summary>
        /// Compresses a directory to file
        /// </summary>
        /// <param name="dir"></param>
        public static void Compress(this DirectoryInfo dir)
        {
            string name = dir.Name + ".gz";

            using (Stream outFile = File.Create(Path.Combine(dir.FullName, name)))
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (string.Compare(name, file.Name) == 0)
                        continue;

                    byte[] buffer = null;

                    using (FileStream inFile = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        buffer = new byte[inFile.Length];
                        int count = inFile.Read(buffer, 0, buffer.Length);
                        if (count != buffer.Length)
                            throw new IOException("Unable to read data from file");
                        inFile.Close();
                    }

                    using (GZipStream gZip = new GZipStream(outFile, CompressionMode.Compress, true))
                    {
                        gZip.Write(buffer, 0, buffer.Length);
                        gZip.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Reads registry key by sub-key name and value name. If the key is not pressent returns a default value.
        /// </summary>
        /// <param name="root">Registry.LocalMachine</param>
        /// <param name="key">"SOFTWARE\\Test\\Preferences"</param>
        /// <returns></returns>
        public static T ReadKey<T>(this RegistryKey root, string subKey, string valueName, T @default)
        {
            RegistryKey key1 = root.OpenSubKey(subKey);
            try
            {
                return (T)Convert.ChangeType(key1.GetValue(valueName, @default), typeof(T));
            }
            finally
            {
                if (key1 != null)
                    key1.Close();
            }
        }


        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
