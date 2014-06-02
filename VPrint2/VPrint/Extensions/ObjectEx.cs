/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Runtime;
using System.Text;

namespace VPrinting
{
    public static class ObjectEx
    {
        /// <summary>
        /// Esc,X,10,70 => LPRINT CHR$(27); CHR$(88); CHR$(10); CHR$(70);
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static string ToChr(this object[] values)
        {
            StringBuilder b = new StringBuilder();

            foreach (object value in values)
            {
                if (value is string)
                {
                    string si;
                    if (ASCII.TryParse((string)value, out si))
                        b.Append(si);
                    else
                        b.Append((string)value);
                }
                else if (value is int)
                {
                    b.Append((char)(int)value);
                }
                else
                    throw new NotImplementedException();
            }
            return b.ToString();
        }
    }
}
