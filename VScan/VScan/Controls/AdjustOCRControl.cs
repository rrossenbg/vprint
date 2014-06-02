/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.ComponentModel;
using System.Windows.Forms.Styled;

namespace PremierTaxFree.Controls
{
    public partial class AdjustOCRControl : ShapeControl
    {
        public VistaButton vbtnAreaToOCR;
        public VistaButton vbtnAreaToHide;
        public VistaButton vbtnSave;
        private IContainer components = null;

        public AdjustOCRControl()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.vbtnAreaToOCR = new System.Windows.Forms.Styled.VistaButton();
            this.vbtnAreaToHide = new System.Windows.Forms.Styled.VistaButton();
            this.vbtnSave = new System.Windows.Forms.Styled.VistaButton();
            this.SuspendLayout();
            // 
            // vbtnAreaToOCR
            // 
            this.vbtnAreaToOCR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.vbtnAreaToOCR.BackColor = System.Drawing.Color.Transparent;
            this.vbtnAreaToOCR.ButtonText = "Barcode Area";
            this.vbtnAreaToOCR.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.vbtnAreaToOCR.Location = new System.Drawing.Point(32, 18);
            this.vbtnAreaToOCR.Name = "vbtnAreaToOCR";
            this.vbtnAreaToOCR.Size = new System.Drawing.Size(159, 32);
            this.vbtnAreaToOCR.TabIndex = 0;
            // 
            // vbtnAreaToHide
            // 
            this.vbtnAreaToHide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.vbtnAreaToHide.BackColor = System.Drawing.Color.Transparent;
            this.vbtnAreaToHide.ButtonText = "Area to hide";
            this.vbtnAreaToHide.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.vbtnAreaToHide.Location = new System.Drawing.Point(31, 60);
            this.vbtnAreaToHide.Name = "vbtnAreaToHide";
            this.vbtnAreaToHide.Size = new System.Drawing.Size(159, 32);
            this.vbtnAreaToHide.TabIndex = 1;
            // 
            // vbtnSave
            // 
            this.vbtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.vbtnSave.BackColor = System.Drawing.Color.Transparent;
            this.vbtnSave.ButtonText = "Save";
            this.vbtnSave.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.vbtnSave.Location = new System.Drawing.Point(32, 103);
            this.vbtnSave.Name = "vbtnSave";
            this.vbtnSave.Size = new System.Drawing.Size(159, 32);
            this.vbtnSave.TabIndex = 2;
            // 
            // AdjustOCRControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vbtnSave);
            this.Controls.Add(this.vbtnAreaToHide);
            this.Controls.Add(this.vbtnAreaToOCR);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimumSize = new System.Drawing.Size(192, 100);
            this.Name = "AdjustOCRControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Shape = System.Windows.Forms.Styled.ShapeType.RoundedRectangle;
            this.Size = new System.Drawing.Size(220, 150);
            this.ResumeLayout(false);

        }
    }
}
