
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: DebuggerVisualizer(
    typeof(ImageVisualizer.DebuggerSide), 
    typeof(VisualizerObjectSource), 
            Target = typeof(Image), 
            Description = "Image Visualizer")]

namespace ImageVisualizer
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/KB/trace/ImageVisualizer.aspx"/>
    public class DebuggerSide : DialogDebuggerVisualizer
    {
        /// <summary>
        /// In order for Visual Studio to use our debugger visualizer, we must drop the DLL in @Visual Studio Install Dir@\Common7\Packages\Debugger\Visualizers.
        /// </summary>
        /// <param name="windowService"></param>
        /// <param name="objectProvider"></param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            Image image = objectProvider.GetObject() as Image;

            if (image != null)
            {
                Form form = new Form();
                form.Text = string.Format("Width: {0}, Height: {1}", image.Width, image.Height);
                form.ClientSize = image.Size.Min(SystemInformation.PrimaryMonitorSize);
                form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                form.AutoScrollMinSize = image.Size;

                PictureBox pictureBox = new PictureBox();
                pictureBox.Dock = DockStyle.Fill;
                form.Controls.Add(pictureBox);

                pictureBox.Image = image;
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