/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PremierTaxFree
{
    public static class FormsEx
    {
        /// <summary>
        /// Disposes a control in a safe manner
        /// </summary>
        /// <param name="cnt"></param>
        public static void DisposeSf(this Control cnt)
        {
            if (cnt == null)
                return;
            if (cnt.IsDisposed || !cnt.IsHandleCreated)
                return;
            cnt.Dispose();
        }

        //[Obsolete("Do not use this any more. Use InvokeSafe instead.")]
        /// <summary>
        /// Invokes a control safely
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="del"></param>
        public static void InvokeSf(this Control cnt, MethodInvoker del)
        {
            if (cnt == null)
                return;
            if (cnt.IsDisposed || !cnt.IsHandleCreated)
                return;
            if (cnt.InvokeRequired)
                cnt.Invoke(del);
            else
                del();
        }

        /// <summary>
        /// Invokes a control safely
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="del"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object InvokeSafe(this Control cnt, Delegate del, params object[] data)
        {
            if (cnt == null)
                return null;
            if (cnt.IsDisposed || !cnt.IsHandleCreated)
                return null;
            if (cnt.InvokeRequired)
                return cnt.Invoke(del, data);
            else
                return del.DynamicInvoke(data);
        }

        //[Obsolete("Do not use this any more. Use BeginInvokeSafe instead.")]
        /// <summary>
        /// Begins invoking control safely
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="del"></param>
        public static void BeginInvokeSf(this Control cnt, MethodInvoker del)
        {
            if (cnt == null)
                return;
            if (cnt.IsDisposed || !cnt.IsHandleCreated)
                return;
            if (cnt.InvokeRequired)
                cnt.BeginInvoke(del);
            else
                del();
        }

        /// <summary>
        /// Begins invoking contol safely
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="del"></param>
        /// <param name="data"></param>
        public static void BeginInvokeSafe(this Control cnt, Delegate del, params object[] data)
        {
            if (cnt == null)
                return;
            if (cnt.IsDisposed || !cnt.IsHandleCreated)
                return;
            if (cnt.InvokeRequired)
                cnt.BeginInvoke(del, data);
            else
                del.DynamicInvoke(data);
        }

        /// <summary>
        /// Closes a form asynchronously
        /// </summary>
        /// <param name="form"></param>
        public static void CloseAsync(this Form form)
        {
            form.InvokeSf(new MethodInvoker(() => { form.Close(); }));
        }

        /// <summary>
        /// Converts FormCollection to an IEnumerable of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static IEnumerable<T> Convert<T>(this FormCollection coll)
        {
            foreach (T t in coll)
                yield return t;
        }

        /// <summary>
        /// Finds a button and performs click on it
        /// </summary>
        /// <param name="coll"></param>
        /// <param name="funct"></param>
        public static void FindAndClick(this ICollection<RibbonItem> coll, Func<RibbonItem, bool> funct)
        {
            var btn = (RibbonButton)(coll.FirstOrDefault(funct));
            if (btn != null)
                btn.PerformClick();
        }

        /// <summary>
        /// Selects a row in DataGridViewRow component
        /// </summary>
        /// <param name="row"></param>
        public static void Select(this DataGridViewRow row)
        {
            DataGridView view = row.DataGridView;
            view.ClearSelection();
            view.CurrentCell = row.Cells[0];
            row.Selected = true;
        }

        /// <summary>
        /// Shows information pop-up
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        public static void ShowInformation(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows error pop-up
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        public static void ShowError(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows warning pop-up
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        public static void ShowWarning(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows question pop-up
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static DialogResult ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(owner, message, Application.ProductName, buttons, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows exclamation pop-up
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        public static void ShowExclamation(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
