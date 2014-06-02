namespace XmlVisualizer.XmlEditor
{
    partial class ShowForm
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
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.xmlEditorControl1 = new XmlVisualizer.XmlEditorControl();
            this.SuspendLayout();
            // 
            // xmlEditorControl1
            // 
            this.xmlEditorControl1.AllowXmlFormatting = true;
            this.xmlEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.xmlEditorControl1.Name = "xmlEditorControl1";
            this.xmlEditorControl1.ReadOnly = false;
            this.xmlEditorControl1.Size = new System.Drawing.Size(470, 372);
            this.xmlEditorControl1.TabIndex = 0;
            // 
            // ShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 372);
            this.Controls.Add(this.xmlEditorControl1);
            this.Name = "ShowForm";
            this.Text = "ShowForm";
            this.ResumeLayout(false);

        }

        #endregion

        public XmlEditorControl xmlEditorControl1;

    }
}