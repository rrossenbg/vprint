namespace VPrinting
{
    partial class CreateFormatForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbCountryID = new System.Windows.Forms.ComboBox();
            this.cbDocName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbAllocationId = new System.Windows.Forms.TextBox();
            this.btnTryPrint = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtType = new System.Windows.Forms.TextBox();
            this.txtXmlText = new VPrinting.XmlEditor.XmlEditorControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 79);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(96, 79);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbCountryID
            // 
            this.cbCountryID.FormattingEnabled = true;
            this.cbCountryID.Location = new System.Drawing.Point(83, 28);
            this.cbCountryID.Name = "cbCountryID";
            this.cbCountryID.Size = new System.Drawing.Size(101, 21);
            this.cbCountryID.TabIndex = 4;
            // 
            // cbDocName
            // 
            this.cbDocName.FormattingEnabled = true;
            this.cbDocName.Location = new System.Drawing.Point(231, 28);
            this.cbDocName.Name = "cbDocName";
            this.cbDocName.Size = new System.Drawing.Size(183, 21);
            this.cbDocName.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtType);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.cbDocName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbCountryID);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(618, 108);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbAllocationId);
            this.groupBox2.Controls.Add(this.btnTryPrint);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(420, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 94);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Test Print";
            // 
            // tbAllocationId
            // 
            this.tbAllocationId.Location = new System.Drawing.Point(76, 23);
            this.tbAllocationId.Name = "tbAllocationId";
            this.tbAllocationId.Size = new System.Drawing.Size(100, 20);
            this.tbAllocationId.TabIndex = 2;
            // 
            // btnTryPrint
            // 
            this.btnTryPrint.Location = new System.Drawing.Point(76, 50);
            this.btnTryPrint.Name = "btnTryPrint";
            this.btnTryPrint.Size = new System.Drawing.Size(75, 21);
            this.btnTryPrint.TabIndex = 1;
            this.btnTryPrint.Text = "Print";
            this.btnTryPrint.UseVisualStyleBackColor = true;
            this.btnTryPrint.Click += new System.EventHandler(this.btnTryPrint_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Allocation id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Country:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Layout class:";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(83, 55);
            this.txtType.Name = "txtType";
            this.txtType.Size = new System.Drawing.Size(331, 20);
            this.txtType.TabIndex = 6;
            // 
            // txtXmlText
            // 
            this.txtXmlText.AllowXmlFormatting = false;
            this.txtXmlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXmlText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtXmlText.Location = new System.Drawing.Point(6, 114);
            this.txtXmlText.Name = "txtXmlText";
            this.txtXmlText.ReadOnly = false;
            this.txtXmlText.Size = new System.Drawing.Size(618, 415);
            this.txtXmlText.TabIndex = 7;
            // 
            // CreateFormatForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 535);
            this.Controls.Add(this.txtXmlText);
            this.Controls.Add(this.groupBox1);
            this.Name = "CreateFormatForm";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Format";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbCountryID;
        private System.Windows.Forms.ComboBox cbDocName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.Label label3;
        private XmlEditor.XmlEditorControl txtXmlText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbAllocationId;
        private System.Windows.Forms.Button btnTryPrint;
        private System.Windows.Forms.Label label4;
    }
}