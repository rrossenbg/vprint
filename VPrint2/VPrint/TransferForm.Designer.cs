namespace VPrinting
{
    partial class TransferForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbCountry = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSiteCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.err = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpRange = new System.Windows.Forms.TabPage();
            this.tpSingle = new System.Windows.Forms.TabPage();
            this.cbCountry2 = new System.Windows.Forms.ComboBox();
            this.tbSiteCode2 = new System.Windows.Forms.TextBox();
            this.tbFrom2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.err)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpRange.SuspendLayout();
            this.tpSingle.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(56, 210);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(182, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbCountry
            // 
            this.cbCountry.FormattingEnabled = true;
            this.cbCountry.Location = new System.Drawing.Point(86, 20);
            this.cbCountry.Name = "cbCountry";
            this.cbCountry.Size = new System.Drawing.Size(100, 21);
            this.cbCountry.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Country";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "From";
            // 
            // tbFrom
            // 
            this.tbFrom.Location = new System.Drawing.Point(86, 91);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.Size = new System.Drawing.Size(100, 20);
            this.tbFrom.TabIndex = 3;
            // 
            // tbTo
            // 
            this.tbTo.Location = new System.Drawing.Point(86, 127);
            this.tbTo.Name = "tbTo";
            this.tbTo.Size = new System.Drawing.Size(100, 20);
            this.tbTo.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "To";
            // 
            // tbSiteCode
            // 
            this.tbSiteCode.Location = new System.Drawing.Point(86, 56);
            this.tbSiteCode.Name = "tbSiteCode";
            this.tbSiteCode.Size = new System.Drawing.Size(100, 20);
            this.tbSiteCode.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Sitecode";
            // 
            // err
            // 
            this.err.ContainerControl = this;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpRange);
            this.tabControl1.Controls.Add(this.tpSingle);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(284, 192);
            this.tabControl1.TabIndex = 10;
            // 
            // tpRange
            // 
            this.tpRange.Controls.Add(this.cbCountry);
            this.tpRange.Controls.Add(this.label4);
            this.tpRange.Controls.Add(this.tbSiteCode);
            this.tpRange.Controls.Add(this.label3);
            this.tpRange.Controls.Add(this.label1);
            this.tpRange.Controls.Add(this.tbTo);
            this.tpRange.Controls.Add(this.label2);
            this.tpRange.Controls.Add(this.tbFrom);
            this.tpRange.Location = new System.Drawing.Point(4, 22);
            this.tpRange.Name = "tpRange";
            this.tpRange.Padding = new System.Windows.Forms.Padding(3);
            this.tpRange.Size = new System.Drawing.Size(276, 166);
            this.tpRange.TabIndex = 0;
            this.tpRange.Text = "Add Range";
            this.tpRange.UseVisualStyleBackColor = true;
            // 
            // tpSingle
            // 
            this.tpSingle.Controls.Add(this.label5);
            this.tpSingle.Controls.Add(this.label6);
            this.tpSingle.Controls.Add(this.label7);
            this.tpSingle.Controls.Add(this.cbCountry2);
            this.tpSingle.Controls.Add(this.tbSiteCode2);
            this.tpSingle.Controls.Add(this.tbFrom2);
            this.tpSingle.Location = new System.Drawing.Point(4, 22);
            this.tpSingle.Name = "tpSingle";
            this.tpSingle.Padding = new System.Windows.Forms.Padding(3);
            this.tpSingle.Size = new System.Drawing.Size(276, 217);
            this.tpSingle.TabIndex = 1;
            this.tpSingle.Text = "Add Single";
            this.tpSingle.UseVisualStyleBackColor = true;
            // 
            // cbCountry2
            // 
            this.cbCountry2.FormattingEnabled = true;
            this.cbCountry2.Location = new System.Drawing.Point(88, 25);
            this.cbCountry2.Name = "cbCountry2";
            this.cbCountry2.Size = new System.Drawing.Size(100, 21);
            this.cbCountry2.TabIndex = 7;
            // 
            // tbSiteCode2
            // 
            this.tbSiteCode2.Location = new System.Drawing.Point(88, 61);
            this.tbSiteCode2.Name = "tbSiteCode2";
            this.tbSiteCode2.Size = new System.Drawing.Size(100, 20);
            this.tbSiteCode2.TabIndex = 8;
            // 
            // tbFrom2
            // 
            this.tbFrom2.Location = new System.Drawing.Point(88, 96);
            this.tbFrom2.Name = "tbFrom2";
            this.tbFrom2.Size = new System.Drawing.Size(100, 20);
            this.tbFrom2.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Sitecode";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(29, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Country";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(42, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "From";
            // 
            // TransferForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(311, 251);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.MaximumSize = new System.Drawing.Size(319, 278);
            this.MinimumSize = new System.Drawing.Size(319, 278);
            this.Name = "TransferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transfer Form";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.err)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpRange.ResumeLayout(false);
            this.tpRange.PerformLayout();
            this.tpSingle.ResumeLayout(false);
            this.tpSingle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbCountry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSiteCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ErrorProvider err;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpRange;
        private System.Windows.Forms.TabPage tpSingle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbCountry2;
        private System.Windows.Forms.TextBox tbSiteCode2;
        private System.Windows.Forms.TextBox tbFrom2;
    }
}