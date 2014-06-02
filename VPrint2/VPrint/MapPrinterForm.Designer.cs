namespace VPrinting
{
    partial class MapPrinterForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbHorizontalLength = new System.Windows.Forms.TextBox();
            this.tbSpaces = new System.Windows.Forms.TextBox();
            this.tbTabs = new System.Windows.Forms.TextBox();
            this.tbVerticaLength = new System.Windows.Forms.TextBox();
            this.tbNumberOfLines = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbBarcodePos = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbPrintBarcode = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbPrinterName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(370, 174);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.85714F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.14286F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbHorizontalLength, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbSpaces, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbTabs, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbVerticaLength, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbNumberOfLines, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tbBarcodePos, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.cbPrintBarcode, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 141);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Horizontal length";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Spaces";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tabs";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Vertical length";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Number of lines";
            // 
            // tbHorizontalLength
            // 
            this.tbHorizontalLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHorizontalLength.Location = new System.Drawing.Point(132, 6);
            this.tbHorizontalLength.Name = "tbHorizontalLength";
            this.tbHorizontalLength.Size = new System.Drawing.Size(212, 20);
            this.tbHorizontalLength.TabIndex = 5;
            this.tbHorizontalLength.Text = "100";
            // 
            // tbSpaces
            // 
            this.tbSpaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSpaces.Location = new System.Drawing.Point(132, 26);
            this.tbSpaces.Name = "tbSpaces";
            this.tbSpaces.Size = new System.Drawing.Size(212, 20);
            this.tbSpaces.TabIndex = 6;
            this.tbSpaces.Text = "0";
            // 
            // tbTabs
            // 
            this.tbTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTabs.Location = new System.Drawing.Point(132, 46);
            this.tbTabs.Name = "tbTabs";
            this.tbTabs.Size = new System.Drawing.Size(212, 20);
            this.tbTabs.TabIndex = 7;
            this.tbTabs.Text = "0";
            // 
            // tbVerticaLength
            // 
            this.tbVerticaLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVerticaLength.Location = new System.Drawing.Point(132, 66);
            this.tbVerticaLength.Name = "tbVerticaLength";
            this.tbVerticaLength.Size = new System.Drawing.Size(212, 20);
            this.tbVerticaLength.TabIndex = 8;
            this.tbVerticaLength.Text = "80";
            // 
            // tbNumberOfLines
            // 
            this.tbNumberOfLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNumberOfLines.Location = new System.Drawing.Point(132, 86);
            this.tbNumberOfLines.Name = "tbNumberOfLines";
            this.tbNumberOfLines.Size = new System.Drawing.Size(212, 20);
            this.tbNumberOfLines.TabIndex = 9;
            this.tbNumberOfLines.Text = "5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Barcode pos";
            // 
            // tbBarcodePos
            // 
            this.tbBarcodePos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbBarcodePos.Location = new System.Drawing.Point(132, 126);
            this.tbBarcodePos.Name = "tbBarcodePos";
            this.tbBarcodePos.Size = new System.Drawing.Size(212, 20);
            this.tbBarcodePos.TabIndex = 11;
            this.tbBarcodePos.Text = "1000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(132, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(212, 20);
            this.label8.TabIndex = 12;
            this.label8.Text = "Print barcode";
            // 
            // cbPrintBarcode
            // 
            this.cbPrintBarcode.AutoSize = true;
            this.cbPrintBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPrintBarcode.Location = new System.Drawing.Point(6, 106);
            this.cbPrintBarcode.Name = "cbPrintBarcode";
            this.cbPrintBarcode.Size = new System.Drawing.Size(120, 14);
            this.cbPrintBarcode.TabIndex = 13;
            this.cbPrintBarcode.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbPrinterName);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 174);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 85);
            this.panel1.TabIndex = 1;
            // 
            // tbPrinterName
            // 
            this.tbPrinterName.Location = new System.Drawing.Point(53, 9);
            this.tbPrinterName.Name = "tbPrinterName";
            this.tbPrinterName.Size = new System.Drawing.Size(301, 20);
            this.tbPrinterName.TabIndex = 3;
            this.tbPrinterName.Text = "\\\\192.168.44.158\\della52fe3-p";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Printer";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(244, 50);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPrint.Location = new System.Drawing.Point(53, 50);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.Print_Click);
            // 
            // MapPrinterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 259);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(378, 286);
            this.MinimumSize = new System.Drawing.Size(378, 286);
            this.Name = "MapPrinterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map Printer";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbHorizontalLength;
        private System.Windows.Forms.TextBox tbSpaces;
        private System.Windows.Forms.TextBox tbTabs;
        private System.Windows.Forms.TextBox tbVerticaLength;
        private System.Windows.Forms.TextBox tbNumberOfLines;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbBarcodePos;
        private System.Windows.Forms.TextBox tbPrinterName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbPrintBarcode;
    }
}