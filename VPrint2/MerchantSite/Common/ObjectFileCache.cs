/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using VPrinting;

namespace MerchantSite.Common
{
    public class ObjectFileCache
    {
        private readonly DirectoryInfo m_Directory;

        public ObjectFileCache(string folder)
        {
            m_Directory = new DirectoryInfo(folder);
            m_Directory.EnsureDirectory();
        }

        public bool IsExist(string id)
        {
            var file = m_Directory.CombineFileName(id);
            return file.Exists;
        }

        public byte[] Get(string id, Func<byte[]> setFunct)
        {
            var file = m_Directory.CombineFileName(id.ToString());
            return (file.Exists) ? file.ReadAllBytes() : Set(id, setFunct());
        }

        public string Get(string id, Func<string> setFunct)
        {
            var file = m_Directory.CombineFileName(id.ToString());
            return (file.Exists) ? file.ReadAllText() : Set(id, setFunct());
        }

        public byte[] Set(string id, byte[] data)
        {
            var file = m_Directory.CombineFileName(id);
            file.WriteAllBytes(data);
            return data;
        }

        public string Set(string id, string text)
        {
            var file = m_Directory.CombineFileName(id);
            file.WriteAllText(text);
            return text;
        }

        public void Reset()
        {
            m_Directory.DeleteSafe(true);
        }
    }
}