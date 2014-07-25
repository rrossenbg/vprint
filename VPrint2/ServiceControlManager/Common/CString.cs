/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Text;

namespace FintraxServiceManager.Common
{
    public class CString
    {
        private readonly StringBuilder m_Builder;
        public CString(CString str)
        {
            this.m_Builder = new StringBuilder(str.m_Builder.ToString());
        }
        public CString(string str)
        {
            this.m_Builder = new StringBuilder(str);
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
        public void ReplaceAll(string oldString, string newString)
        {
            this.m_Builder.Replace(oldString, newString);
        }
        public void Format(string format, params object[] values)
        {
            this.m_Builder.AppendFormat(format, values);
        }
        public override string ToString()
        {
            return this.m_Builder.ToString();
        }
    }
}
