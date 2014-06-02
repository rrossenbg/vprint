using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.RibbonHelpers;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/KB/toolbars/WinFormsRibbon.aspx"/>
    /// <permission cref="http://www.opensource.org/licenses/ms-pl.html">Microsoft Public License (MS-PL)</permission>
    public class RibbonForm : Form, IRibbonForm
    {
        protected virtual Ribbon Ribbon { get; set; }

        #region Fields

        private RibbonFormHelper _helper;

        #endregion

        #region Ctor

        public RibbonForm()
        {
            if (WinApi.IsWindows && !WinApi.IsGlassEnabled)
            {
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.Opaque, WinApi.IsGlassEnabled);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                DoubleBuffered = true;
            }
            //SetStyle(ControlStyles.EnableNotifyMessage, true);


            _helper = new RibbonFormHelper(this);
        }

        #endregion

        #region Overrides

        protected override void OnLoad(EventArgs e)
        {
            if (Ribbon != null)
                Ribbon.MouseEnter += new EventHandler(Ribbon_MouseEnter);
            base.OnLoad(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            if (Ribbon != null)
                Ribbon.Invalidate();
            base.OnActivated(e);
        }

        private void Ribbon_MouseEnter(object sender, EventArgs e)
        {
            Ribbon.Invalidate();
        }

        /// <summary>
        /// Just for debugging messages
        /// </summary>
        /// <param name="m"></param>
        protected override void OnNotifyMessage(Message m)
        {
            base.OnNotifyMessage(m);
            Console.WriteLine(m.ToString());
        }

        /// <summary>
        /// Overrides the WndProc funciton
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (!Helper.WndProc(ref m))
            {
                base.WndProc(ref m);
            }
        }

        #endregion

        #region IRibbonForm Members

        /// <summary>
        /// Gets the helper for making the form a ribbon form
        /// </summary>
        public RibbonFormHelper Helper
        {
            get { return _helper; }
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RibbonForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RibbonForm";
            this.ResumeLayout(false);
        }
    }
}
