/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Drawing;
using System.Runtime;
using System.Windows.Forms;

namespace CardCodeCover
{
    public static class FormsEx
    {
        [TargetedPatchingOptOut("na")]
        public static void ShowMessage(this IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void GetDpi(this Control control, out float dx, out float dy)
        {
            using (var g = control.CreateGraphics())
            {
                dx = g.DpiX;
                dy = g.DpiY;
            }
        }
    }
}
