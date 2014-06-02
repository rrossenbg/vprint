/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Styled
{
    public class PromptedTextBox : MaskedTextBox
    {
        const int WM_SETFOCUS = 7;
        const int WM_KILLFOCUS = 8;
        const int WM_ERASEBKGND = 14;
        const int WM_PAINT = 15;

        private bool m_focusSelect = true;
        private string m_promptText = String.Empty;
        private Color m_promptColor = SystemColors.GrayText;
        private Font m_promptFont = null;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("The prompt text to display when there is nothing in the Text property.")]
        public string PromptText
        {
            get { return m_promptText; }
            set
            {
                m_promptText = value.Trim();
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("The ForeColor to use when displaying the PromptText.")]
        public Color PromptForeColor
        {
            get { return m_promptColor; }
            set
            {
                m_promptColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("The Font to use when displaying the PromptText.")]
        public Font PromptFont
        {
            get { return m_promptFont; }
            set
            {
                m_promptFont = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Behavior")]
        [Description("Automatically select the text when control receives the focus.")]
        public bool FocusSelect
        {
            get { return m_focusSelect; }
            set { m_focusSelect = value; }
        }

        public PromptedTextBox()
        {
            this.PromptFont = this.Font;
        }

        protected override void OnEnter(EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text) && m_focusSelect)
                this.SelectAll();

            base.OnEnter(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            this.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {

            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_KILLFOCUS:
                case WM_SETFOCUS:
                case WM_PAINT:
                    DrawTextPrompt();
                    break;
            }
        }

        protected virtual void DrawTextPrompt()
        {
            if (!string.IsNullOrEmpty(this.Text))
                return;

            using (Graphics g = this.CreateGraphics())
            {
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top | TextFormatFlags.EndEllipsis;
                Rectangle rect = this.ClientRectangle;

                switch (this.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags = flags | TextFormatFlags.HorizontalCenter;
                        rect.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags = flags | TextFormatFlags.Left;
                        rect.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags = flags | TextFormatFlags.Right;
                        rect.Offset(0, 1);
                        break;
                }

                TextRenderer.DrawText(g, m_promptText, m_promptFont, rect, m_promptColor, this.BackColor, flags);
            }
        }
    }
}