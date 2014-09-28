namespace CPrint2.Controls
{
    partial class CameraControl
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
            using (m_Cap) ;

            if (disposing && (components != null))
            {
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
            this.captureBox = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.captureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // captureBox
            // 
            this.captureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captureBox.Location = new System.Drawing.Point(5, 5);
            this.captureBox.Name = "captureBox";
            this.captureBox.Size = new System.Drawing.Size(302, 238);
            this.captureBox.TabIndex = 2;
            this.captureBox.TabStop = false;
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.captureBox);
            this.Name = "CameraControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(312, 248);
            ((System.ComponentModel.ISupportInitialize)(this.captureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox captureBox;
    }
}
