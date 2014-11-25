/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Text;
using System.Diagnostics;

namespace FintraxServiceManager.Common
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
            Debug.Assert(str != null);
            m_Builder = new StringBuilder(str.m_Builder.ToString());
        }

        public CString(string str)
        {
            Debug.Assert(str != null);
            m_Builder = new StringBuilder(str);
        }

        public static CString operator +(CString str1, string str2)
        {
            Debug.Assert(str1 != null);
            str1.m_Builder.Append(str2);
            return str1;
        }

        public static CString operator +(CString str1, CString str2)
        {
            Debug.Assert(str1 != null);
            Debug.Assert(str2 != null);

            str1.m_Builder.Append(str2.m_Builder);
            return str1;
        }

        public static implicit operator string(CString str)
        {
            Debug.Assert(str != null);
            return str.m_Builder.ToString();
        }

        public static implicit operator CString(string str)
        {
            return new CString(str);
        }

        public void ReplaceAll(string oldString, string newString)
        {
            Debug.Assert(oldString != null);
            Debug.Assert(newString != null);

            m_Builder.Replace(oldString, newString);
        }

        public void Format(string format, params object[] values)
        {
            Debug.Assert(format != null);

            m_Builder.AppendFormat(format, values);
        }

        public override string ToString()
        {
            return m_Builder.ToString();
        }
    }
}
