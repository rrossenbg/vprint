/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Threading;
using System.Windows.Forms;

namespace PremierTaxFree.PTFLib
{
    public static class PTFUtils
    {
        /// <summary>
        /// Gets the name of the machine
        /// </summary>
        /// <returns></returns>
        public static string GetMachine()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// Gets current user name
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return SystemInformation.UserName;
        }

        /// <summary>
        /// Empty event handler. Used for handler math.
        /// </summary>
        /// <returns></returns>
        public static EventHandler EmptyEventHandler()
        {
            return new EventHandler((_1, _2) => { });
        }

        /// <summary>
        /// Empty event handler. Used for handler math.
        /// </summary>
        /// <returns></returns>
        public static ThreadExceptionEventHandler EmptyThreadExceptionEventHandler()
        {
            return new ThreadExceptionEventHandler((_1, _2) => { });
        }
    }
}
