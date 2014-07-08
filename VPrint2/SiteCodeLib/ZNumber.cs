/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Text;
using System.Collections.Generic;

namespace SiteCodeLib
{
    public class ZNumber : IEqualityComparer<ZNumber>
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private List<int> m_Coefficients = new List<int>();

        public int Base
        {
            get
            {
                return CHARS.Length;
            }
        }

        public ZNumber(params int[] coeficients)
        {
            for (int i = coeficients.Length - 1; i >= 0; i--)
                m_Coefficients.Add(coeficients[i]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>This</returns>
        public ZNumber Increase()
        {
            if (m_Coefficients.Count == 0)
                m_Coefficients.Add(0);
            m_Coefficients[0] += 1;
            Addition();
            return this;
        }

        private void Addition()
        {
            List<int> coeff = new List<int>();
            int carry = 0;
            foreach (int c in m_Coefficients)
            {
                int value2 = (c + carry) % Base;
                coeff.Add(value2);
                carry = (c + carry) / Base;
            }
            if (carry > 0)
                coeff.Add(carry - 1);
            m_Coefficients = coeff;
        }

        public ZNumber Decrease()
        {
            throw new NotImplementedException();
        }

        private void Subtraction()
        {
            throw new NotImplementedException();
        }

        public static bool operator <(ZNumber n1, ZNumber n2)
        {
            if (n1.m_Coefficients.Count < n2.m_Coefficients.Count)
                return true;
            else if (n1.m_Coefficients.Count > n2.m_Coefficients.Count)
                return false;

            for (int i = n1.m_Coefficients.Count - 1; i >= 0; i--)
            {
                if (n1.m_Coefficients[i] < n2.m_Coefficients[i])
                    return true;
                else if (n1.m_Coefficients[i] > n2.m_Coefficients[i])
                    return false;
            }
            return false;
        }

        public static bool operator >(ZNumber n1, ZNumber n2)
        {
            if (n1.m_Coefficients.Count > n2.m_Coefficients.Count)
                return true;
            else if (n1.m_Coefficients.Count < n2.m_Coefficients.Count)
                return false;

            for (int i = n1.m_Coefficients.Count - 1; i >= 0; i--)
            {
                if (n1.m_Coefficients[i] > n2.m_Coefficients[i])
                    return true;
                else if (n1.m_Coefficients[i] < n2.m_Coefficients[i])
                    return false;
            }
            return false;
        }

        public static bool operator ==(ZNumber n1, ZNumber n2)
        {
            if (n1.m_Coefficients.Count == n2.m_Coefficients.Count)
            {
                for (int i = n1.m_Coefficients.Count - 1; i >= 0; i--)
                    if (n1.m_Coefficients[i] != n2.m_Coefficients[i])
                        return false;
                return true;
            }
            return false;
        }

        public static bool operator !=(ZNumber n1, ZNumber n2)
        {
            if (n1.m_Coefficients.Count != n2.m_Coefficients.Count)
                return true;

            for (int i = n1.m_Coefficients.Count - 1; i >= 0; i--)
                if (n1.m_Coefficients[i] == n2.m_Coefficients[i])
                    return false;
            return true;
        }

        public static ZNumber Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            List<int> coeff = new List<int>();
            foreach (char ch in value)
            {
                int index = CHARS.IndexOf(ch);
                if (index == -1)
                    throw new ArgumentOutOfRangeException("Unknown char in string");
                coeff.Add(index);
            }
            return new ZNumber(coeff.ToArray());
        }

        public bool Equals(ZNumber x, ZNumber y)
        {
            return x == y;
        }

        public int GetHashCode(ZNumber obj)
        {
            return obj.ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            for (int i = m_Coefficients.Count - 1; i >= 0; i--)
                b.Append(CHARS[m_Coefficients[i]]);
            return b.ToString();
        }
    }

    public static class ListEx
    {
        public static T Get<T>(this List<T> list, int index)
        {
            if (list.Count > index)
                return list[index];
            return default(T);
        }

        public static T[] ToReverseArray<T>(this IList<T> list)
        {
            List<T> result = new List<T>();
            for (int i = list.Count - 1; i >= 0; i--)
                result.Add(list[i]);
            return result.ToArray();
        }
    }
}