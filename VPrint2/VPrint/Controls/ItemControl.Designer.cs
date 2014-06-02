using System;
namespace VPrinting.Controls
{
    partial class ItemControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (m_Item != null)
                    m_Item.Updated -= new EventHandler(m_Item_Updated);

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_ActionTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // m_ActionTimer
            // 
            this.m_ActionTimer.Interval = 2000;
            this.m_ActionTimer.Tick += new System.EventHandler(this.ActionTimer_Tick);
            // 
            // ItemControl
            // 
            this.Size = new System.Drawing.Size(64, 64);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer m_ActionTimer;
    }
}
