using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VPrinting.XmlEditor
{
    public static class RichTextBoxEx
    {
        private static Color SPECIALCHARCOLOR = Color.Blue;   //  Color for special characters
        private static Color ESCAPECOLOR = Color.Orchid;      //  Color for escape sequences
        private static Color ELEMENTCOLOR = Color.DarkRed;    //  Color for Xml elements
        private static Color ATTRIBUTECOLOR = Color.Red;      //  Color for Xml attributes
        private static Color VALUECOLOR = Color.DarkBlue;     //  Color for attribute values
        private static Color COMMENTCOLOR = Color.DarkGreen;  //  Color for Xml comments

        /// <summary>
        /// Format Xml in the passed rich text box.
        /// </summary>
        /// <param name="xmlEditor"></param>
        public static void FormatXml(this RichTextBox xmlEditor)
        {
            //  Stop redrawing
            RichTextDrawing.StopRedraw(xmlEditor);

            //  Tokenize the Xml string
            List<XmlToken> tokens = XmlTokenizer.Tokenize(xmlEditor.Text);
            foreach (XmlToken token in tokens)
            {
                xmlEditor.Select(token.Index, token.Text.Length);
                switch (token.Type)
                {
                    case XmlTokenType.None:
                        xmlEditor.SelectionColor = xmlEditor.ForeColor;
                        break;
                    case XmlTokenType.SpecialChar:
                        xmlEditor.SelectionColor = SPECIALCHARCOLOR;
                        break;
                    case XmlTokenType.Escape:
                        xmlEditor.SelectionColor = ESCAPECOLOR;
                        break;
                    case XmlTokenType.Element:
                        xmlEditor.SelectionColor = ELEMENTCOLOR;
                        break;
                    case XmlTokenType.Attribute:
                        xmlEditor.SelectionColor = ATTRIBUTECOLOR;
                        break;
                    case XmlTokenType.Value:
                        xmlEditor.SelectionColor = VALUECOLOR;
                        break;
                    case XmlTokenType.Comment:
                        xmlEditor.SelectionColor = COMMENTCOLOR;
                        break;
                }
            }

            //  Sample code to show that the perf problem is a RichTexBox problem
            //string content = xmlEditor.Text;
            //Random gen = new Random();
            //for (int i = 0; i < content.Length; i++)
            //{
            //    xmlEditor.Select(i, 1);
            //    Color c = Color.FromArgb(gen.Next(256), gen.Next(256), gen.Next(256));
            //    xmlEditor.SelectionColor = c;
            //}

            //  Resume redraw
            RichTextDrawing.RestoreRedraw(xmlEditor);
        }
    }
}
