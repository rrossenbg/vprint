/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;

namespace VPrinting.Documents
{
    public class RepeaterCounter
    {
        private readonly int m_DEFAULT = 1;

        private readonly Hashtable m_Table = Hashtable.Synchronized(new Hashtable());

        public int this[int key]
        {
            get
            {
                return Convert.ToInt32(m_Table[key] ?? m_DEFAULT);
            }
        }

        public RepeaterCounter(int @default)
        {
            m_DEFAULT = @default;
        }

        /// <summary>
        /// Load from string
        /// </summary>
        /// <param name="strings">826,1;250,3;56,3;</param>
        public void Load(string strings)
        {
            if (!string.IsNullOrWhiteSpace(strings))
            {
                string[] values1 = strings.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var value in values1)
                {
                    string[] values2 = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (values2.Length == 2)
                    {
                        int key = int.Parse(values2[0]);
                        int value2 = int.Parse(values2[1]);
                        m_Table[key] = value2;
                    }
                }
            }
        }
    }
}
