/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Runtime;
using System.Windows.Forms;
using System.IO;

namespace VPrinting.Extentions
{
    public static class ExceptionEx
    {
        [TargetedPatchingOptOut("na")]
        public static void ShowDialog(this Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowDialog(this Exception ex, IWin32Window owner)
        {
            MessageBox.Show(owner, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
