using System;
using System.Runtime;
using System.Windows.Forms;

namespace CPrint2
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
