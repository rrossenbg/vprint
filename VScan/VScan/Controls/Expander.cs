/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PremierTaxFree.Controls
{
    public partial class Expander : UserControl
    {
        public Expander()
        {
            InitializeComponent();
            this.Expanded = true;
        }

        #region Events

        public event EventHandler StateChanged;
        public event CancelEventHandler StateChanging;

        #endregion

        #region Properties

        public bool Expanded { get; private set; }

        public Control Header
        {
            get { return this.header; }

            set
            {
                if (this.header != null)
                    this.Controls.Remove(this.header);

                this.header = value;
                this.header.Dock = DockStyle.Top;
                this.Controls.Add(this.header);
                this.Controls.SetChildIndex(this.header, this.Controls.Count > 1 ? 1 : 0);
            }
        }

        public Control Content
        {
            get { return this.content; }

            set
            {
                if (this.content != null)
                    this.Controls.Remove(this.content);

                this.content = value;
                this.Size = new Size(this.Width, this.header.Height + this.content.Height);
                this.content.Top = this.header.Height;

                this.Controls.Add(this.content);
                this.Controls.SetChildIndex(this.content, 0);
            }
        }

        #endregion

        #region Public methods

 

        public void Expand()
        {
            if (this.Expanded)
                return;

            if (StateChanging != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                StateChanging(this, args);
                if (args.Cancel)
                    return;
            }

            this.Expanded = true;
            ArrangeLayout();

            if (StateChanged != null)
                StateChanged(this, null);
        }

        public void Collapse()
        {
            if (!this.Expanded)
                return;

            if (StateChanging != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                StateChanging(this, args);
                if (args.Cancel)
                    return;
            }

            if (this.Content != null)
                this.contentHeight = this.Content.Height;
            this.Expanded = false;
            ArrangeLayout();

            if (StateChanged != null)
                StateChanged(this, null);
        }

        public void Toggle()
        {
            if (this.Expanded)
                Collapse();
            else
                Expand();
        }

        #endregion

        #region Private methods
        
        private void ArrangeLayout()
        {
            int h = 0;
            if (this.header != null)
                h += this.header.Height;
            if (this.Expanded && this.content != null)
                h += this.content.Height;
            this.Size = new Size(this.Width, h);
        }

        #endregion
        
        #region Priate fields

        private Control header;
        private Control content;
        private int contentHeight = 0;

        #endregion

        public void CreateContentLabel(string text, int height)
        {
            Label labelContent = new Label();
            labelContent.Text = text;
            labelContent.Size = new Size(this.Width, height);
            this.Content = labelContent;
        }
    }

    public static class ExpanderHelper
    {
        public static Label CreateLabelHeader(Expander expander, string text, Color backColor)
        {
            return CreateLabelHeader(expander, text, backColor, null, null, 25, null);
        }
        public static Label CreateLabelHeader(Expander expander, string text, Color backColor, Image collapsedImage)
        {
            return CreateLabelHeader(expander, text, backColor, collapsedImage, null, 25, null); 
        }
        public static Label CreateLabelHeader(Expander expander, string text, Color backColor, Image collapsedImage, Image expandedImage)
        {
            return CreateLabelHeader(expander, text, backColor, collapsedImage, expandedImage, 25, null);
        }
        public static Label CreateLabelHeader(Expander expander, string text, Color backColor, Image collapsedImage, Image expandedImage, int height)
        {
            return CreateLabelHeader(expander, text, backColor, collapsedImage, expandedImage, height, null);
        }
        public static Label CreateLabelHeader(Expander expander, string text, Color backColor, Image collapsedImage, Image expandedImage, int height, Font font)
        {
            Label headerLabel = new Label();
            headerLabel.Text = text;
            headerLabel.AutoSize = false;
            headerLabel.Height = height;
            if (font != null)
                headerLabel.Font = font;
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            if (collapsedImage != null && expandedImage != null)
            {
                headerLabel.Image = collapsedImage;
                headerLabel.ImageAlign = ContentAlignment.MiddleRight;
            }
            headerLabel.BackColor = backColor;

            if (collapsedImage != null && expandedImage != null)
            {
                expander.StateChanged += delegate { headerLabel.Image = expander.Expanded ? collapsedImage : expandedImage; };
            }

            headerLabel.Click += delegate { expander.Toggle(); };
            headerLabel.Cursor = Cursors.Hand;

            expander.Header = headerLabel;

            return headerLabel;
        }
    }
}
