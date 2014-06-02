/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Windows.Forms;

namespace PremierTaxFree.PTFLib
{
    public static class ExceptionEx
    {
        /// <summary>
        /// Throws the exception in another thread.
        /// Does not affect current thread execution.
        /// </summary>
        /// <param name="ex"></param>
        public static void ThrowAndForget(this Exception ex)
        {
            new MethodInvoker(() => { throw ex; }).FireAndForget();
        }

        /// <summary>
        /// Adds a MethodInvoker delegate to an exception object
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        public static void AddNext(this Exception ex, MethodInvoker data)
        {
            ex.AddInfo(Strings.NextDelegate, data);
        }

        /// <summary>
        /// Adds a EventHandler delegate to an exception object
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        public static void AddNext(this Exception ex, EventHandler data)
        {
            ex.AddInfo(Strings.NextDelegate, data);
        }

        /// <summary>
        /// Gets delegate from exception object
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Delegate GetNext(this Exception ex)
        {
            return ex.GetInfo<Delegate>(Strings.NextDelegate);
        }

        /// <summary>
        /// Gets EventHandler from exception object
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static EventHandler GetNextHandler(this Exception ex)
        {
            return ex.GetInfo<EventHandler>(Strings.NextDelegate);
        }

        /// <summary>
        /// Adds object to exception object by name
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public static void AddInfo(this Exception ex, string name, object data)
        {
            ex.Data.Add(name, data);
        }

        /// <summary>
        /// Gets object of type T from an exception object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetInfo<T>(this Exception ex, string name)
        {
            if (!ex.Data.Contains(name))
                return default(T);
            return (T)ex.Data[name];
        }

        /// <summary>
        /// Shows exception message as error dialog
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowDialog(this Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows exception message as error dialog by owner
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="owner"></param>
        public static void ShowDialog(this Exception ex, IWin32Window owner)
        {
            MessageBox.Show(owner, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
