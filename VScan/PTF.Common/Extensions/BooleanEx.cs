/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;

namespace PremierTaxFree.PTFLib
{
    public static class BooleanEx
    {
        /// <summary>
        /// Returns random boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Random(this bool value)
        {
            Random rnd = new Random();
            return rnd.Next() % 2 == 0;
        }
    }
}
