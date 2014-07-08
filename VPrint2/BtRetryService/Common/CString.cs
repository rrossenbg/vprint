/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Text;

namespace BtRetryService
{
    /// <summary>
    /// This class is base on the StringBuilder class
    /// and allows you to sum strings without 
    /// any affect over the system performance
    /// </summary>
    /// <example>
    /// CString str = string.Empty;
    /// str += "Test1" + "\r\n";
    /// str += "Test2" + "\r\n";
    /// str += "Test3" + "\r\n";
    /// Console.Write(str);
    /// </example>
    public class CString
    {
        private readonly StringBuilder m_Builder;

        public CString(CString str)
        {
            m_Builder = new StringBuilder(str.m_Builder.ToString());
        }

        public CString(string str)
        {
            m_Builder = new StringBuilder(str);
        }

        public static CString operator +(CString str1, string str2)
        {
            str1.m_Builder.Append(str2);
            return str1;
        }

        public static CString operator +(CString str1, CString str2)
        {
            str1.m_Builder.Append(str2.m_Builder);
            return str1;
        }

        public static implicit operator string(CString str)
        {
            return str.m_Builder.ToString();
        }

        public static implicit operator CString(string str)
        {
            return new CString(str);
        }

        public override string ToString()
        {
            return m_Builder.ToString();
        }
    }
}
