using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;
using XmlVisualizer.XmlEditor;

namespace XmlVisualizer
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
            XmlDocument doc = objectProvider.GetObject() as XmlDocument;
            if (doc != null)
            {
                ShowForm form = new ShowForm();
                form.Text = "XmlDocument";
                form.xmlEditorControl1.Text = doc.toString();
                windowService.ShowDialog(form);
            }
            else
            {
                XDocument xdoc = objectProvider.GetObject() as XDocument;
                if (xdoc != null)
                {
                    ShowForm form = new ShowForm();
                    form.Text = "XDocument";
                    form.xmlEditorControl1.Text = string.Concat(xdoc.Declaration.ToString(), xdoc.ToString());
                    windowService.ShowDialog(form);
                }
            }
        }
    }

    public static class ClassEx
    {
        public static string toString(this XmlDocument xmlDoc)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
