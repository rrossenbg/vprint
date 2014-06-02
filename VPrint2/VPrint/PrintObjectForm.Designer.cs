namespace VPrinting
{
    partial class PrintObjectForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtText = new System.Windows.Forms.TextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.cbIsBarCode = new System.Windows.Forms.CheckBox();
            this.ddlDataColumn = new System.Windows.Forms.ComboBox();
            this.txtFormat = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.lblFont = new System.Windows.Forms.LinkLabel();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(71, 145);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(58, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(209, 145);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Text";
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(61, 27);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(196, 29);
            this.txtText.TabIndex = 3;
            this.txtText.Text = "NA";
            // 
            // cbIsBarCode
            // 
            this.cbIsBarCode.AutoSize = true;
            this.cbIsBarCode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbIsBarCode.Location = new System.Drawing.Point(223, 104);
            this.cbIsBarCode.Name = "cbIsBarCode";
            this.cbIsBarCode.Size = new System.Drawing.Size(76, 17);
            this.cbIsBarCode.TabIndex = 5;
            this.cbIsBarCode.Text = "Is barcode";
            this.cbIsBarCode.UseVisualStyleBackColor = true;
            // 
            // ddlDataColumn
            // 
            this.ddlDataColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlDataColumn.FormattingEnabled = true;
            this.ddlDataColumn.Location = new System.Drawing.Point(61, 74);
            this.ddlDataColumn.Name = "ddlDataColumn";
            this.ddlDataColumn.Size = new System.Drawing.Size(196, 21);
            this.ddlDataColumn.TabIndex = 6;
            // 
            // txtFormat
            // 
            this.txtFormat.Location = new System.Drawing.Point(59, 101);
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.Size = new System.Drawing.Size(153, 20);
            this.txtFormat.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Databind";
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.lblFont);
            this.gbProperties.Controls.Add(this.cbIsBarCode);
            this.gbProperties.Controls.Add(this.label3);
            this.gbProperties.Controls.Add(this.btnOK);
            this.gbProperties.Controls.Add(this.txtFormat);
            this.gbProperties.Controls.Add(this.btnCancel);
            this.gbProperties.Controls.Add(this.label2);
            this.gbProperties.Controls.Add(this.label1);
            this.gbProperties.Controls.Add(this.ddlDataColumn);
            this.gbProperties.Controls.Add(this.txtText);
            this.gbProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProperties.Location = new System.Drawing.Point(10, 10);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(325, 184);
            this.gbProperties.TabIndex = 10;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Properties";
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(271, 27);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(28, 13);
            this.lblFont.TabIndex = 10;
            this.lblFont.TabStop = true;
            this.lblFont.Text = "Font";
            this.lblFont.Click += new System.EventHandler(this.Font_Click);
            // 
            // PrintObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 204);
            this.Controls.Add(this.gbProperties);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(361, 242);
            this.MinimumSize = new System.Drawing.Size(361, 242);
            this.Name = "PrintObjectForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Print Object";
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.CheckBox cbIsBarCode;
        private System.Windows.Forms.ComboBox ddlDataColumn;
        private System.Windows.Forms.TextBox txtFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.LinkLabel lblFont;
    }
}