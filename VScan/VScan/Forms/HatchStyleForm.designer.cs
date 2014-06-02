namespace PremierTaxFree.Forms
{
    partial class HatchStyleForm
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
            this.backPanel = new PremierTaxFree.Forms.ColorPanel();
            this.forePanel = new PremierTaxFree.Forms.ColorPanel();
            this.hatchStyleComboBox = new PremierTaxFree.Forms.HatchStyleComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backPanel
            // 
            this.backPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.backPanel.Color = System.Drawing.SystemColors.Control;
            this.backPanel.Location = new System.Drawing.Point(87, 19);
            this.backPanel.Name = "backPanel";
            this.backPanel.Size = new System.Drawing.Size(29, 24);
            this.backPanel.TabIndex = 2;
            this.backPanel.ColorChanged += new System.EventHandler(this.ColorPanel_ColorChanged);
            // 
            // forePanel
            // 
            this.forePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.forePanel.Color = System.Drawing.SystemColors.Control;
            this.forePanel.Location = new System.Drawing.Point(87, 51);
            this.forePanel.Name = "forePanel";
            this.forePanel.Size = new System.Drawing.Size(29, 24);
            this.forePanel.TabIndex = 1;
            this.forePanel.ColorChanged += new System.EventHandler(this.ColorPanel_ColorChanged);
            // 
            // hatchStyleComboBox
            // 
            this.hatchStyleComboBox.Back_Color = System.Drawing.Color.White;
            this.hatchStyleComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.hatchStyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hatchStyleComboBox.DropDownWidth = this.hatchStyleComboBox.Width;
            this.hatchStyleComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hatchStyleComboBox.Fore_Color = System.Drawing.Color.Black;
            this.hatchStyleComboBox.FormattingEnabled = true;
            this.hatchStyleComboBox.Items.AddRange(new object[] {
            System.Drawing.Drawing2D.HatchStyle.Horizontal,
            System.Drawing.Drawing2D.HatchStyle.Horizontal,
            System.Drawing.Drawing2D.HatchStyle.Vertical,
            System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.DiagonalCross,
            System.Drawing.Drawing2D.HatchStyle.Percent05,
            System.Drawing.Drawing2D.HatchStyle.Percent10,
            System.Drawing.Drawing2D.HatchStyle.Percent20,
            System.Drawing.Drawing2D.HatchStyle.Percent25,
            System.Drawing.Drawing2D.HatchStyle.Percent30,
            System.Drawing.Drawing2D.HatchStyle.Percent40,
            System.Drawing.Drawing2D.HatchStyle.Percent50,
            System.Drawing.Drawing2D.HatchStyle.Percent60,
            System.Drawing.Drawing2D.HatchStyle.Percent70,
            System.Drawing.Drawing2D.HatchStyle.Percent75,
            System.Drawing.Drawing2D.HatchStyle.Percent80,
            System.Drawing.Drawing2D.HatchStyle.Percent90,
            System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LightVertical,
            System.Drawing.Drawing2D.HatchStyle.LightHorizontal,
            System.Drawing.Drawing2D.HatchStyle.NarrowVertical,
            System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DarkVertical,
            System.Drawing.Drawing2D.HatchStyle.DarkHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DashedHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DashedVertical,
            System.Drawing.Drawing2D.HatchStyle.SmallConfetti,
            System.Drawing.Drawing2D.HatchStyle.LargeConfetti,
            System.Drawing.Drawing2D.HatchStyle.ZigZag,
            System.Drawing.Drawing2D.HatchStyle.Wave,
            System.Drawing.Drawing2D.HatchStyle.DiagonalBrick,
            System.Drawing.Drawing2D.HatchStyle.HorizontalBrick,
            System.Drawing.Drawing2D.HatchStyle.Weave,
            System.Drawing.Drawing2D.HatchStyle.Plaid,
            System.Drawing.Drawing2D.HatchStyle.Divot,
            System.Drawing.Drawing2D.HatchStyle.DottedGrid,
            System.Drawing.Drawing2D.HatchStyle.DottedDiamond,
            System.Drawing.Drawing2D.HatchStyle.Shingle,
            System.Drawing.Drawing2D.HatchStyle.Trellis,
            System.Drawing.Drawing2D.HatchStyle.Sphere,
            System.Drawing.Drawing2D.HatchStyle.SmallGrid,
            System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard,
            System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard,
            System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond,
            System.Drawing.Drawing2D.HatchStyle.SolidDiamond});
            this.hatchStyleComboBox.Location = new System.Drawing.Point(87, 81);
            this.hatchStyleComboBox.Name = "hatchStyleComboBox";
            this.hatchStyleComboBox.Size = new System.Drawing.Size(63, 24);
            this.hatchStyleComboBox.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.backPanel);
            this.groupBox1.Controls.Add(this.hatchStyleComboBox);
            this.groupBox1.Controls.Add(this.forePanel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 124);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Style";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Fore Color";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Back Color";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(24, 142);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(125, 142);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // HatchStyleForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(224, 180);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(232, 214);
            this.MinimumSize = new System.Drawing.Size(232, 214);
            this.Name = "HatchStyleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HatchStyle";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HatchStyleComboBox hatchStyleComboBox;
        private ColorPanel forePanel;
        private ColorPanel backPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}

