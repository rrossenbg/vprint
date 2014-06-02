using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;

/// <summary>
/// 
/// </summary>
/// <see cref="http://www.codeproject.com/KB/trace/ImageVisualizer.aspx"/>
[assembly: DebuggerVisualizer(typeof(CollectionVisualizer.DebuggerSide), typeof(VisualizerObjectSource),
            Target = typeof(Array), Description = "ArrayToString Visualizer")]
namespace CollectionVisualizer
{
    public class DebuggerSide : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            IList array = objectProvider.GetObject() as IList;

            if (array != null)
            {
                Form form = new Form();

                form.ClientSize = new Size(400, 400).Min(SystemInformation.PrimaryMonitorSize);
                form.FormBorderStyle = FormBorderStyle.FixedToolWindow;

                TextBox txtBox = new TextBox();
                txtBox.Dock = DockStyle.Fill;
                form.Controls.Add(txtBox);

                form.Text = string.Format("Count: {0}", array.Count);

                foreach (var i in array)
                    txtBox.AppendText(i != null ? i.ToString() : "<NULL>");

                windowService.ShowDialog(form);
            }
        }
    }

    public static class ClassEx
    {
        public static Size Min(this Size size1, Size size2)
        {
            if (size1.Height > size2.Height || size1.Width > size2.Width)
                return size2;
            return size1;
        }
    }
}
