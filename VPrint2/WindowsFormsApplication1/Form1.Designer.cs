namespace VPrinting.Controls
{
    partial class Form1
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
            this.toggleButtonControl1 = new ToggleButtonControl();
            this.SuspendLayout();
            // 
            // toggleButtonControl1
            // 
            this.toggleButtonControl1.ColorOff = System.Drawing.Color.Red;
            this.toggleButtonControl1.ColorOn = System.Drawing.Color.Lime;
            this.toggleButtonControl1.Location = new System.Drawing.Point(51, 67);
            this.toggleButtonControl1.Name = "toggleButtonControl1";
            this.toggleButtonControl1.Padding = new System.Windows.Forms.Padding(5);
            this.toggleButtonControl1.Size = new System.Drawing.Size(218, 99);
            this.toggleButtonControl1.TabIndex = 0;
            this.toggleButtonControl1.Text1 = new string[] {
        "On1",
        "Off1"};
            this.toggleButtonControl1.Text2 = new string[] {
        "On2",
        "Off2"};
            this.toggleButtonControl1.Text3 = new string[] {
        "On3",
        "Off3"};
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 342);
            this.Controls.Add(this.toggleButtonControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ToggleButtonControl toggleButtonControl1;
    }
}

