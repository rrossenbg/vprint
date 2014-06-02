/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Styled;
using PremierTaxFree.Components;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Controls
{
    public class ManualInsertDataControl : ShapeControl
    {
        public const string MANUALINSERT_DATACONTROL_NAME = "MANUALINSERTDATACONTROL";
        private IContainer components = null;
        private static readonly InstanceCountingTimer ms_Timer = new InstanceCountingTimer();
        private Label lblMessage;
        private TTabControl tTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private MaskedTextBox mtbRetailerID;
        private MaskedTextBox mtbActivityCode;
        private MaskedTextBox mtbCountryID;
        private MaskedTextBox mtbVoucherID;
        private Label label2;
        private Label label1;
        private Label label4;
        private Label label3;
        private Label label5;
        private MaskedTextBox mtbBarCode;

        public string Message
        {
            get
            {
                return lblMessage.Text;
            }
            set
            {
                lblMessage.Text = value;
            }
        }

        public string BarCodeString
        {
            get
            {
                return mtbBarCode.Text;
            }
        }

        public string[] BarCodeNumberGroups
        {
            get
            {
                return new string[] { mtbCountryID.Text, mtbActivityCode.Text, mtbRetailerID.Text, mtbVoucherID.Text };
            }
        }

        static ManualInsertDataControl()
        {
            ms_Timer.Interval = 500;
        }

        public ManualInsertDataControl()
        {
            InitializeComponent();
            mtbCountryID.Text = SettingsTable.Get<int>(Strings.VScan_DefaultCountryCode, 826).ToString();
            ms_Timer.Tick += Timer_Tick;
        }

        ~ManualInsertDataControl()
        {
            ms_Timer.Tick -= Timer_Tick;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lblMessage.ForeColor = lblMessage.ForeColor != Color.Red ? Color.Red : Color.Black;
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.tTabControl1 = new PremierTaxFree.Controls.TTabControl();
            this.tabPage1 = new TabPage();
            this.label5 = new Label();
            this.mtbBarCode = new MaskedTextBox();
            this.tabPage2 = new TabPage();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.mtbVoucherID = new MaskedTextBox();
            this.mtbRetailerID = new MaskedTextBox();
            this.mtbActivityCode = new MaskedTextBox();
            this.mtbCountryID = new MaskedTextBox();
            this.tTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoEllipsis = true;
            this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMessage.Dock = DockStyle.Top;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(20, 20);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(302, 26);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "No barcode fount.";
            // 
            // tTabControl1
            // 
            this.tTabControl1.Controls.Add(this.tabPage1);
            this.tTabControl1.Controls.Add(this.tabPage2);
            this.tTabControl1.Dock = DockStyle.Fill;
            this.tTabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tTabControl1.Location = new System.Drawing.Point(20, 46);
            this.tTabControl1.Name = "tTabControl1";
            this.tTabControl1.SelectedIndex = 0;
            this.tTabControl1.Size = new System.Drawing.Size(302, 204);
            this.tTabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.mtbBarCode);
            this.tabPage1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(294, 172);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Barcode";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Voucher code";
            // 
            // mtbBarCode
            // 
            this.mtbBarCode.BeepOnError = true;
            this.mtbBarCode.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            this.mtbBarCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtbBarCode.Location = new System.Drawing.Point(20, 32);
            this.mtbBarCode.Mask = "000_00_000000_000000000";
            this.mtbBarCode.Name = "mtbBarCode";
            this.mtbBarCode.Size = new System.Drawing.Size(242, 26);
            this.mtbBarCode.TabIndex = 2;
            this.mtbBarCode.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.mtbVoucherID);
            this.tabPage2.Controls.Add(this.mtbRetailerID);
            this.tabPage2.Controls.Add(this.mtbActivityCode);
            this.tabPage2.Controls.Add(this.mtbCountryID);
            this.tabPage2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(294, 172);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Voucher";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(32, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Voucher ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Retailer ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(32, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Activity Code";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Country ID";
            // 
            // mtbVoucherID
            // 
            this.mtbVoucherID.BeepOnError = true;
            this.mtbVoucherID.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            this.mtbVoucherID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtbVoucherID.Location = new System.Drawing.Point(128, 116);
            this.mtbVoucherID.Mask = "000000000";
            this.mtbVoucherID.Name = "mtbVoucherID";
            this.mtbVoucherID.Size = new System.Drawing.Size(103, 26);
            this.mtbVoucherID.TabIndex = 6;
            this.mtbVoucherID.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // mtbRetailerID
            // 
            this.mtbRetailerID.BeepOnError = true;
            this.mtbRetailerID.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            this.mtbRetailerID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtbRetailerID.Location = new System.Drawing.Point(128, 84);
            this.mtbRetailerID.Mask = "000000";
            this.mtbRetailerID.Name = "mtbRetailerID";
            this.mtbRetailerID.Size = new System.Drawing.Size(82, 26);
            this.mtbRetailerID.TabIndex = 5;
            this.mtbRetailerID.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // mtbActivityCode
            // 
            this.mtbActivityCode.BeepOnError = true;
            this.mtbActivityCode.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            this.mtbActivityCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtbActivityCode.Location = new System.Drawing.Point(128, 52);
            this.mtbActivityCode.Mask = "00";
            this.mtbActivityCode.Name = "mtbActivityCode";
            this.mtbActivityCode.Size = new System.Drawing.Size(36, 26);
            this.mtbActivityCode.TabIndex = 4;
            this.mtbActivityCode.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // mtbCountryID
            // 
            this.mtbCountryID.BeepOnError = true;
            this.mtbCountryID.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            this.mtbCountryID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtbCountryID.Location = new System.Drawing.Point(128, 20);
            this.mtbCountryID.Mask = "000";
            this.mtbCountryID.Name = "mtbCountryID";
            this.mtbCountryID.Size = new System.Drawing.Size(50, 26);
            this.mtbCountryID.TabIndex = 3;
            this.mtbCountryID.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // ManualInsertDataControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.tTabControl1);
            this.Controls.Add(this.lblMessage);
            this.Name = "ManualInsertDataControl";
            this.Padding = new Padding(20);
            this.Shape = System.Windows.Forms.Styled.ShapeType.RoundedRectangle;
            this.Size = new System.Drawing.Size(342, 270);
            this.tTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
