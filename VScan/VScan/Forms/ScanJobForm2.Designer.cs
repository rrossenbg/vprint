namespace PremierTaxFree.Forms
{
    partial class ScanJobForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanJobForm2));
            this.panel1 = new System.Windows.Forms.Panel();
            this.ptbComment = new System.Windows.Forms.Styled.PromptedTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlScanAfter = new System.Windows.Forms.GroupBox();
            this.accordion2 = new PremierTaxFree.Controls.Accordion();
            this.pnlScanBefore = new System.Windows.Forms.GroupBox();
            this.cbConfirmBefore = new System.Windows.Forms.CheckBox();
            this.pnlLocalVouchers = new System.Windows.Forms.GroupBox();
            this.mtxtFileID = new System.Windows.Forms.MaskedTextBox();
            this.cbSiteCodes = new PremierTaxFree.Controls.MultyColorComboBox();
            this.pnlForeignVouchers = new System.Windows.Forms.GroupBox();
            this.mtxtAuditID = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.err = new System.Windows.Forms.ErrorProvider(this.components);
            this.accordion1 = new PremierTaxFree.Controls.Accordion();
            this.panel1.SuspendLayout();
            this.pnlScanAfter.SuspendLayout();
            this.pnlScanBefore.SuspendLayout();
            this.pnlLocalVouchers.SuspendLayout();
            this.pnlForeignVouchers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.err)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.ptbComment);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.err.SetError(this.panel1, resources.GetString("panel1.Error"));
            this.panel1.Font = null;
            this.err.SetIconAlignment(this.panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel1.IconAlignment"))));
            this.err.SetIconPadding(this.panel1, ((int)(resources.GetObject("panel1.IconPadding"))));
            this.panel1.Name = "panel1";
            // 
            // ptbComment
            // 
            this.ptbComment.AccessibleDescription = null;
            this.ptbComment.AccessibleName = null;
            resources.ApplyResources(this.ptbComment, "ptbComment");
            this.ptbComment.BackgroundImage = null;
            this.err.SetError(this.ptbComment, resources.GetString("ptbComment.Error"));
            this.ptbComment.FocusSelect = true;
            this.ptbComment.Font = null;
            this.err.SetIconAlignment(this.ptbComment, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ptbComment.IconAlignment"))));
            this.err.SetIconPadding(this.ptbComment, ((int)(resources.GetObject("ptbComment.IconPadding"))));
            this.ptbComment.Name = "ptbComment";
            this.ptbComment.PromptFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptbComment.PromptForeColor = System.Drawing.SystemColors.GrayText;
            this.ptbComment.PromptText = "Please, type come comment here";
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.err.SetError(this.btnCancel, resources.GetString("btnCancel.Error"));
            this.btnCancel.Font = null;
            this.err.SetIconAlignment(this.btnCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnCancel.IconAlignment"))));
            this.err.SetIconPadding(this.btnCancel, ((int)(resources.GetObject("btnCancel.IconPadding"))));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.AccessibleDescription = null;
            this.btnOk.AccessibleName = null;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.BackgroundImage = null;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.err.SetError(this.btnOk, resources.GetString("btnOk.Error"));
            this.btnOk.Font = null;
            this.err.SetIconAlignment(this.btnOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnOk.IconAlignment"))));
            this.err.SetIconPadding(this.btnOk, ((int)(resources.GetObject("btnOk.IconPadding"))));
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.err.SetError(this.label1, resources.GetString("label1.Error"));
            this.label1.Font = null;
            this.err.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.err.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // pnlScanAfter
            // 
            this.pnlScanAfter.AccessibleDescription = null;
            this.pnlScanAfter.AccessibleName = null;
            resources.ApplyResources(this.pnlScanAfter, "pnlScanAfter");
            this.pnlScanAfter.BackgroundImage = null;
            this.pnlScanAfter.Controls.Add(this.accordion2);
            this.err.SetError(this.pnlScanAfter, resources.GetString("pnlScanAfter.Error"));
            this.pnlScanAfter.Font = null;
            this.err.SetIconAlignment(this.pnlScanAfter, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pnlScanAfter.IconAlignment"))));
            this.err.SetIconPadding(this.pnlScanAfter, ((int)(resources.GetObject("pnlScanAfter.IconPadding"))));
            this.pnlScanAfter.Name = "pnlScanAfter";
            this.pnlScanAfter.TabStop = false;
            // 
            // accordion2
            // 
            this.accordion2.AccessibleDescription = null;
            this.accordion2.AccessibleName = null;
            resources.ApplyResources(this.accordion2, "accordion2");
            this.accordion2.BackgroundImage = null;
            this.err.SetError(this.accordion2, resources.GetString("accordion2.Error"));
            this.accordion2.Font = null;
            this.err.SetIconAlignment(this.accordion2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("accordion2.IconAlignment"))));
            this.err.SetIconPadding(this.accordion2, ((int)(resources.GetObject("accordion2.IconPadding"))));
            this.accordion2.Name = "accordion2";
            // 
            // pnlScanBefore
            // 
            this.pnlScanBefore.AccessibleDescription = null;
            this.pnlScanBefore.AccessibleName = null;
            resources.ApplyResources(this.pnlScanBefore, "pnlScanBefore");
            this.pnlScanBefore.BackgroundImage = null;
            this.pnlScanBefore.Controls.Add(this.cbConfirmBefore);
            this.err.SetError(this.pnlScanBefore, resources.GetString("pnlScanBefore.Error"));
            this.pnlScanBefore.Font = null;
            this.err.SetIconAlignment(this.pnlScanBefore, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pnlScanBefore.IconAlignment"))));
            this.err.SetIconPadding(this.pnlScanBefore, ((int)(resources.GetObject("pnlScanBefore.IconPadding"))));
            this.pnlScanBefore.Name = "pnlScanBefore";
            this.pnlScanBefore.TabStop = false;
            // 
            // cbConfirmBefore
            // 
            this.cbConfirmBefore.AccessibleDescription = null;
            this.cbConfirmBefore.AccessibleName = null;
            resources.ApplyResources(this.cbConfirmBefore, "cbConfirmBefore");
            this.cbConfirmBefore.BackgroundImage = null;
            this.err.SetError(this.cbConfirmBefore, resources.GetString("cbConfirmBefore.Error"));
            this.cbConfirmBefore.Font = null;
            this.err.SetIconAlignment(this.cbConfirmBefore, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cbConfirmBefore.IconAlignment"))));
            this.err.SetIconPadding(this.cbConfirmBefore, ((int)(resources.GetObject("cbConfirmBefore.IconPadding"))));
            this.cbConfirmBefore.Name = "cbConfirmBefore";
            this.cbConfirmBefore.UseVisualStyleBackColor = true;
            // 
            // pnlLocalVouchers
            // 
            this.pnlLocalVouchers.AccessibleDescription = null;
            this.pnlLocalVouchers.AccessibleName = null;
            resources.ApplyResources(this.pnlLocalVouchers, "pnlLocalVouchers");
            this.pnlLocalVouchers.BackgroundImage = null;
            this.pnlLocalVouchers.Controls.Add(this.mtxtFileID);
            this.pnlLocalVouchers.Controls.Add(this.label1);
            this.pnlLocalVouchers.Controls.Add(this.cbSiteCodes);
            this.err.SetError(this.pnlLocalVouchers, resources.GetString("pnlLocalVouchers.Error"));
            this.pnlLocalVouchers.Font = null;
            this.err.SetIconAlignment(this.pnlLocalVouchers, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pnlLocalVouchers.IconAlignment"))));
            this.err.SetIconPadding(this.pnlLocalVouchers, ((int)(resources.GetObject("pnlLocalVouchers.IconPadding"))));
            this.pnlLocalVouchers.Name = "pnlLocalVouchers";
            this.pnlLocalVouchers.TabStop = false;
            // 
            // mtxtFileID
            // 
            this.mtxtFileID.AccessibleDescription = null;
            this.mtxtFileID.AccessibleName = null;
            resources.ApplyResources(this.mtxtFileID, "mtxtFileID");
            this.mtxtFileID.BackgroundImage = null;
            this.err.SetError(this.mtxtFileID, resources.GetString("mtxtFileID.Error"));
            this.mtxtFileID.Font = null;
            this.err.SetIconAlignment(this.mtxtFileID, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mtxtFileID.IconAlignment"))));
            this.err.SetIconPadding(this.mtxtFileID, ((int)(resources.GetObject("mtxtFileID.IconPadding"))));
            this.mtxtFileID.Name = "mtxtFileID";
            this.mtxtFileID.ValidatingType = typeof(int);
            // 
            // cbSiteCodes
            // 
            this.cbSiteCodes.AccessibleDescription = null;
            this.cbSiteCodes.AccessibleName = null;
            resources.ApplyResources(this.cbSiteCodes, "cbSiteCodes");
            this.cbSiteCodes.BackgroundImage = null;
            this.cbSiteCodes.DefaultItemForeColor = System.Drawing.Color.Black;
            this.cbSiteCodes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbSiteCodes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.err.SetError(this.cbSiteCodes, resources.GetString("cbSiteCodes.Error"));
            this.cbSiteCodes.Font = null;
            this.cbSiteCodes.FormattingEnabled = true;
            this.err.SetIconAlignment(this.cbSiteCodes, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cbSiteCodes.IconAlignment"))));
            this.err.SetIconPadding(this.cbSiteCodes, ((int)(resources.GetObject("cbSiteCodes.IconPadding"))));
            this.cbSiteCodes.Name = "cbSiteCodes";
            // 
            // pnlForeignVouchers
            // 
            this.pnlForeignVouchers.AccessibleDescription = null;
            this.pnlForeignVouchers.AccessibleName = null;
            resources.ApplyResources(this.pnlForeignVouchers, "pnlForeignVouchers");
            this.pnlForeignVouchers.BackgroundImage = null;
            this.pnlForeignVouchers.Controls.Add(this.mtxtAuditID);
            this.pnlForeignVouchers.Controls.Add(this.label2);
            this.err.SetError(this.pnlForeignVouchers, resources.GetString("pnlForeignVouchers.Error"));
            this.pnlForeignVouchers.Font = null;
            this.err.SetIconAlignment(this.pnlForeignVouchers, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pnlForeignVouchers.IconAlignment"))));
            this.err.SetIconPadding(this.pnlForeignVouchers, ((int)(resources.GetObject("pnlForeignVouchers.IconPadding"))));
            this.pnlForeignVouchers.Name = "pnlForeignVouchers";
            this.pnlForeignVouchers.TabStop = false;
            // 
            // mtxtAuditID
            // 
            this.mtxtAuditID.AccessibleDescription = null;
            this.mtxtAuditID.AccessibleName = null;
            resources.ApplyResources(this.mtxtAuditID, "mtxtAuditID");
            this.mtxtAuditID.BackgroundImage = null;
            this.err.SetError(this.mtxtAuditID, resources.GetString("mtxtAuditID.Error"));
            this.err.SetIconAlignment(this.mtxtAuditID, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mtxtAuditID.IconAlignment"))));
            this.err.SetIconPadding(this.mtxtAuditID, ((int)(resources.GetObject("mtxtAuditID.IconPadding"))));
            this.mtxtAuditID.Name = "mtxtAuditID";
            this.mtxtAuditID.ValidatingType = typeof(int);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.err.SetError(this.label2, resources.GetString("label2.Error"));
            this.label2.Font = null;
            this.err.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.err.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // err
            // 
            this.err.ContainerControl = this;
            resources.ApplyResources(this.err, "err");
            // 
            // accordion1
            // 
            this.accordion1.AccessibleDescription = null;
            this.accordion1.AccessibleName = null;
            resources.ApplyResources(this.accordion1, "accordion1");
            this.accordion1.BackgroundImage = null;
            this.accordion1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.err.SetError(this.accordion1, resources.GetString("accordion1.Error"));
            this.accordion1.Font = null;
            this.err.SetIconAlignment(this.accordion1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("accordion1.IconAlignment"))));
            this.err.SetIconPadding(this.accordion1, ((int)(resources.GetObject("accordion1.IconPadding"))));
            this.accordion1.Name = "accordion1";
            // 
            // ScanJobForm2
            // 
            this.AcceptButton = this.btnOk;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.pnlScanAfter);
            this.Controls.Add(this.pnlLocalVouchers);
            this.Controls.Add(this.pnlForeignVouchers);
            this.Controls.Add(this.accordion1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlScanBefore);
            this.Font = null;
            this.Icon = null;
            this.MaximizeBox = false;
            this.Name = "ScanJobForm2";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlScanAfter.ResumeLayout(false);
            this.pnlScanBefore.ResumeLayout(false);
            this.pnlScanBefore.PerformLayout();
            this.pnlLocalVouchers.ResumeLayout(false);
            this.pnlLocalVouchers.PerformLayout();
            this.pnlForeignVouchers.ResumeLayout(false);
            this.pnlForeignVouchers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.err)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private PremierTaxFree.Controls.MultyColorComboBox cbSiteCodes;
        private PremierTaxFree.Controls.Accordion accordion1;
        private System.Windows.Forms.GroupBox pnlScanAfter;
        private System.Windows.Forms.GroupBox pnlScanBefore;
        private System.Windows.Forms.GroupBox pnlLocalVouchers;
        private System.Windows.Forms.GroupBox pnlForeignVouchers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox mtxtAuditID;
        private PremierTaxFree.Controls.Accordion accordion2;
        private System.Windows.Forms.ErrorProvider err;
        private System.Windows.Forms.CheckBox cbConfirmBefore;
        private System.Windows.Forms.MaskedTextBox mtxtFileID;
        private System.Windows.Forms.Styled.PromptedTextBox ptbComment;
    }
}