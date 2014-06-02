namespace PremierTaxFree
{
    partial class VoucherForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoucherForm));
            this.Canvas = new PremierTaxFree.CanvasControl();
            this.printComponent = new PremierTaxFree.PrintComponent(this.components);
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.AllowDrop = true;
            this.Canvas.BrushNum = 2;
            this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Canvas.EraserSize = new System.Drawing.Size(10, 10);
            this.Canvas.FileName = null;
            this.Canvas.LineSize = 3;
            this.Canvas.Location = new System.Drawing.Point(0, 0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(485, 454);
            this.Canvas.TabIndex = 5;
            this.Canvas.Tool = null;
            // 
            // VoucherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 454);
            this.Controls.Add(this.Canvas);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VoucherForm";
            this.ResumeLayout(false);

        }

        #endregion

        public CanvasControl Canvas;
        private PrintComponent printComponent;

    }
}