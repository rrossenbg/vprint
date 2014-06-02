/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Windows.Forms;

namespace VPrinting
{
    public static class WinFormsEx
    {
        [TargetedPatchingOptOut("na")]
        public static void ClearDontFireIndexChanged(this ComboBox box, EventHandler @delegate)
        {
            box.SelectedIndexChanged -= @delegate;
            box.Items.Clear();
            box.SelectedIndexChanged += @delegate;
        }

        [TargetedPatchingOptOut("na")]
        public static void ClearDontFireIndexChanged<T>(this ComboBox box, EventHandler @delegate, Action<ComboBox, T> action, T data)
        {
            box.SelectedIndexChanged -= @delegate;
            box.Items.Clear();
            action(box, data);
            box.SelectedIndexChanged += @delegate;
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<DataGridViewRow> Find<T>(this DataGridViewRowCollection rows, string columnName, Predicate<T> funct)
        {
            lock (((ICollection)rows).SyncRoot)
            {
                foreach (DataGridViewRow row in rows)
                    if (funct((T)row.Cells[columnName].Value))
                        yield return row;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowInfo(this IWin32Window owner, string message)
        {
            ShowInfo(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowInfo(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [TargetedPatchingOptOut("na")]
        public static DialogResult ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons btns)
        {
            return MessageBox.Show(owner, message, Application.ProductName, btns, MessageBoxIcon.Question);
        }

        [TargetedPatchingOptOut("na")]
        public static bool ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons btns, DialogResult yes)
        {
            return MessageBox.Show(owner, message, Application.ProductName, btns, MessageBoxIcon.Question) == yes;
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowExclamation(this IWin32Window owner, string message)
        {
            ShowExclamation(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowExclamation(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowError(this IWin32Window owner, string message)
        {
            ShowError(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowError(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [TargetedPatchingOptOut("na")]
        public static void SetSelected<T>(this ComboBox combo, Func<T, bool> funct)
        {
            Debug.Assert(combo != null);
            Debug.Assert(funct != null);

            foreach (T item in combo.Items)
            {
                if (funct(item))
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
