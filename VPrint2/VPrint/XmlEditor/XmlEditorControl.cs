///---------------------------------------------------------------------------
/// File Name:      XmlEditor.cs
/// Description:    Editor that does Xml formatting and syntax highlighting.
/// 
/// Author:         Ali Badereddin
/// Created:        26/12/2009
///---------------------------------------------------------------------------

#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

#endregion

namespace VPrinting.XmlEditor
{
    /// <summary>
    /// Editor that does Xml formatting and syntax highlighting.
    /// </summary>
    public partial class XmlEditorControl : UserControl
    {
        #region Instance Variables

        private bool allowXmlFormatting = true;               //  Whether to do Xml formatting when text changes

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public XmlEditorControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set or get the text of the xml editor.
        /// </summary>
        public override string Text
        {
            set
            {
                xmlTextBox.Text = value;
            }
            get
            {
                return xmlTextBox.Text;
            }
        }

        /// <summary>
        /// Tells whether to format the editor's Xml or not.
        /// </summary>
        public bool AllowXmlFormatting
        {
            set
            {
                allowXmlFormatting = value;
            }
            get
            {
                return allowXmlFormatting;
            }
        }

        /// <summary>
        /// Whether to allow the user to change text.
        /// </summary>
        public bool ReadOnly
        {
            set
            {
                this.xmlTextBox.ReadOnly = value;
            }
            get
            {
                return this.xmlTextBox.ReadOnly;
            }
        }

        #endregion

        public void refresh()
        {
            this.xmlTextBox.FormatXml();
        }

        #region UI Events

        /// <summary>
        /// Format Xml when text changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xmlTextBox_TextChanged(object sender, EventArgs e)
        {
            if (allowXmlFormatting)
                this.xmlTextBox.FormatXml();
        }

        #endregion
    }

    #region Helper Class

    /// <summary>
    /// Helper class to change colors on a RichTextBox without flickering.
    /// </summary>
    public class RichTextDrawing
    {
        private static int lastSelection;

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void StopRedraw(RichTextBox richTextBox)
        {
            LockWindowUpdate(richTextBox.Handle);

            //  Save the last location 
            lastSelection = richTextBox.SelectionStart;

            // Refresh colors
            richTextBox.SelectAll();
            richTextBox.SelectionColor = richTextBox.ForeColor;
        }

        public static void RestoreRedraw(RichTextBox richTextBox)
        {
            LockWindowUpdate(IntPtr.Zero);

            //  Restore selection and color state
            richTextBox.SelectionStart = lastSelection;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionColor = richTextBox.ForeColor;
        }
    }

    #endregion
}