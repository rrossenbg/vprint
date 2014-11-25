using System.ComponentModel;
namespace VPrinting.Forms
{
    partial class RetailerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RetailerForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbRetailerId = new System.Windows.Forms.TextBox();
            this.tbVoucherId = new System.Windows.Forms.TextBox();
            this.cbSSDS = new System.Windows.Forms.ComboBox();
            this.cbCountryID = new System.Windows.Forms.ComboBox();
            this.cbIgnoreServerValidation = new System.Windows.Forms.CheckBox();
            this.err = new System.Windows.Forms.ErrorProvider(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbBarcode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtSitecode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.err)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Country Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Retailer Id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Voucher Id";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(9, 157);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(90, 157);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 7;
            this.btnShow.Text = "&Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.Show_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(171, 157);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btn_Click);
            // 
            // tbRetailerId
            // 
            this.tbRetailerId.Location = new System.Drawing.Point(95, 34);
            this.tbRetailerId.Name = "tbRetailerId";
            this.tbRetailerId.Size = new System.Drawing.Size(98, 20);
            this.tbRetailerId.TabIndex = 10;
            // 
            // tbVoucherId
            // 
            this.tbVoucherId.Location = new System.Drawing.Point(95, 65);
            this.tbVoucherId.Name = "tbVoucherId";
            this.tbVoucherId.Size = new System.Drawing.Size(98, 20);
            this.tbVoucherId.TabIndex = 11;
            // 
            // cbSSDS
            // 
            this.cbSSDS.FormattingEnabled = true;
            this.cbSSDS.Items.AddRange(new object[] {
            "10",
            "20"});
            this.cbSSDS.Location = new System.Drawing.Point(175, 5);
            this.cbSSDS.Name = "cbSSDS";
            this.cbSSDS.Size = new System.Drawing.Size(47, 21);
            this.cbSSDS.TabIndex = 12;
            this.cbSSDS.Text = "10";
            // 
            // cbCountryID
            // 
            this.cbCountryID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCountryID.FormattingEnabled = true;
            this.cbCountryID.Location = new System.Drawing.Point(94, 5);
            this.cbCountryID.Name = "cbCountryID";
            this.cbCountryID.Size = new System.Drawing.Size(75, 21);
            this.cbCountryID.TabIndex = 13;
            // 
            // cbIgnoreServerValidation
            // 
            this.cbIgnoreServerValidation.AutoSize = true;
            this.cbIgnoreServerValidation.Location = new System.Drawing.Point(53, 130);
            this.cbIgnoreServerValidation.Name = "cbIgnoreServerValidation";
            this.cbIgnoreServerValidation.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbIgnoreServerValidation.Size = new System.Drawing.Size(136, 17);
            this.cbIgnoreServerValidation.TabIndex = 14;
            this.cbIgnoreServerValidation.Text = "Ignore server validation";
            this.cbIgnoreServerValidation.UseVisualStyleBackColor = true;
            // 
            // err
            // 
            this.err.ContainerControl = this;
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(9, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(245, 117);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbRetailerId);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cbCountryID);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.cbSSDS);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tbVoucherId);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(237, 91);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Voucher details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbBarcode);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(237, 91);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Barcode";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbBarcode
            // 
            this.tbBarcode.Location = new System.Drawing.Point(10, 32);
            this.tbBarcode.Name = "tbBarcode";
            this.tbBarcode.Size = new System.Drawing.Size(213, 20);
            this.tbBarcode.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Barcode";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtSitecode);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(237, 91);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Sitecode";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtSitecode
            // 
            this.txtSitecode.Location = new System.Drawing.Point(7, 34);
            this.txtSitecode.Name = "txtSitecode";
            this.txtSitecode.Size = new System.Drawing.Size(213, 20);
            this.txtSitecode.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Sitecode";
            // 
            // RetailerForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(269, 190);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cbIgnoreServerValidation);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(277, 217);
            this.MinimumSize = new System.Drawing.Size(277, 217);
            this.Name = "RetailerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Voucher details";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.err)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbRetailerId;
        private System.Windows.Forms.TextBox tbVoucherId;
        private System.Windows.Forms.ComboBox cbSSDS;
        private System.Windows.Forms.ComboBox cbCountryID;
        private System.Windows.Forms.CheckBox cbIgnoreServerValidation;
        private System.Windows.Forms.ErrorProvider err;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tbBarcode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtSitecode;
        private System.Windows.Forms.Label label5;
    }
}