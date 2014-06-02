using System.Windows.Forms.Styled;
namespace PremierTaxFree.Controls
{
    partial class SettingsPage3
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPage3));
            this.ptbWebTracerUrl = new System.Windows.Forms.Styled.PromptedTextBox();
            this.gbTransfferingService = new System.Windows.Forms.GroupBox();
            this.btnInstallWeb = new System.Windows.Forms.Styled.VistaButton();
            this.btnInstallLocal = new System.Windows.Forms.Styled.VistaButton();
            this.doaTransparentLabel9 = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.doaTransparentLabel10 = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.btnTrfTestConnection = new System.Windows.Forms.Styled.VistaButton();
            this.doaWebUrl = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.gbPrinters = new System.Windows.Forms.GroupBox();
            this.lblPrintersNote = new System.Windows.Forms.Label();
            this.doaTransparentLabel1 = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.cbPrinters = new PremierTaxFree.Controls.MultyColorComboBox();
            this.ptbTRSURL = new System.Windows.Forms.Styled.PromptedTextBox();
            this.ptbScanner = new System.Windows.Forms.Styled.PromptedTextBox();
            this.gbTRS = new System.Windows.Forms.GroupBox();
            this.doaTransparentLabel2 = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            this.cbScanners = new PremierTaxFree.Controls.MultyColorComboBox();
            this.gbScanner = new System.Windows.Forms.GroupBox();
            this.doaTransparentLabel3 = new System.Windows.Forms.Styled.DOATransparentLabel();
            this.extender = new PremierTaxFree.DirtyControlExtender(this.components);
            this.gbTransfferingService.SuspendLayout();
            this.gbPrinters.SuspendLayout();
            this.gbTRS.SuspendLayout();
            this.gbScanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.extender)).BeginInit();
            this.SuspendLayout();
            // 
            // ptbWebTracerUrl
            // 
            this.ptbWebTracerUrl.AccessibleDescription = null;
            this.ptbWebTracerUrl.AccessibleName = null;
            resources.ApplyResources(this.ptbWebTracerUrl, "ptbWebTracerUrl");
            this.ptbWebTracerUrl.BackgroundImage = null;
            this.ptbWebTracerUrl.FocusSelect = true;
            this.ptbWebTracerUrl.Font = null;
            this.extender.SetHookupToTextBoxBase(this.ptbWebTracerUrl, true);
            this.ptbWebTracerUrl.Name = "ptbWebTracerUrl";
            this.toolTipProvider.SetToolTip(this.ptbWebTracerUrl, resources.GetString("ptbWebTracerUrl.ToolTip"));
            // 
            // gbTransfferingService
            // 
            this.gbTransfferingService.AccessibleDescription = null;
            this.gbTransfferingService.AccessibleName = null;
            resources.ApplyResources(this.gbTransfferingService, "gbTransfferingService");
            this.gbTransfferingService.BackgroundImage = null;
            this.gbTransfferingService.Controls.Add(this.btnInstallWeb);
            this.gbTransfferingService.Controls.Add(this.btnInstallLocal);
            this.gbTransfferingService.Controls.Add(this.doaTransparentLabel9);
            this.gbTransfferingService.Controls.Add(this.doaTransparentLabel10);
            this.gbTransfferingService.Controls.Add(this.btnTrfTestConnection);
            this.gbTransfferingService.Controls.Add(this.ptbWebTracerUrl);
            this.gbTransfferingService.Controls.Add(this.doaWebUrl);
            this.gbTransfferingService.Font = null;
            this.gbTransfferingService.Name = "gbTransfferingService";
            this.gbTransfferingService.TabStop = false;
            this.toolTipProvider.SetToolTip(this.gbTransfferingService, resources.GetString("gbTransfferingService.ToolTip"));
            // 
            // btnInstallWeb
            // 
            this.btnInstallWeb.AccessibleDescription = null;
            this.btnInstallWeb.AccessibleName = null;
            resources.ApplyResources(this.btnInstallWeb, "btnInstallWeb");
            this.btnInstallWeb.BackColor = System.Drawing.Color.Transparent;
            this.btnInstallWeb.BackgroundImage = null;
            this.btnInstallWeb.ButtonStyle = System.Windows.Forms.Styled.VistaButton.Style.Flat;
            this.btnInstallWeb.ButtonText = null;
            this.btnInstallWeb.CornerRadius = 25;
            this.btnInstallWeb.Font = null;
            this.btnInstallWeb.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnInstallWeb.Image = ((System.Drawing.Image)(resources.GetObject("btnInstallWeb.Image")));
            this.btnInstallWeb.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnInstallWeb.Name = "btnInstallWeb";
            this.toolTipProvider.SetToolTip(this.btnInstallWeb, resources.GetString("btnInstallWeb.ToolTip"));
            this.btnInstallWeb.Click += new System.EventHandler(this.InstallTracer_Click);
            // 
            // btnInstallLocal
            // 
            this.btnInstallLocal.AccessibleDescription = null;
            this.btnInstallLocal.AccessibleName = null;
            resources.ApplyResources(this.btnInstallLocal, "btnInstallLocal");
            this.btnInstallLocal.BackColor = System.Drawing.Color.Transparent;
            this.btnInstallLocal.BackgroundImage = null;
            this.btnInstallLocal.ButtonStyle = System.Windows.Forms.Styled.VistaButton.Style.Flat;
            this.btnInstallLocal.ButtonText = null;
            this.btnInstallLocal.CornerRadius = 25;
            this.btnInstallLocal.Font = null;
            this.btnInstallLocal.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnInstallLocal.Image = ((System.Drawing.Image)(resources.GetObject("btnInstallLocal.Image")));
            this.btnInstallLocal.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnInstallLocal.Name = "btnInstallLocal";
            this.toolTipProvider.SetToolTip(this.btnInstallLocal, resources.GetString("btnInstallLocal.ToolTip"));
            this.btnInstallLocal.Click += new System.EventHandler(this.InstallTracer_Click);
            // 
            // doaTransparentLabel9
            // 
            this.doaTransparentLabel9.AccessibleDescription = null;
            this.doaTransparentLabel9.AccessibleName = null;
            resources.ApplyResources(this.doaTransparentLabel9, "doaTransparentLabel9");
            this.doaTransparentLabel9.BackColor = System.Drawing.Color.Transparent;
            this.doaTransparentLabel9.BackgroundImage = null;
            this.doaTransparentLabel9.DimmedColor = System.Drawing.Color.White;
            this.doaTransparentLabel9.Font = null;
            this.doaTransparentLabel9.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaTransparentLabel9.Name = "doaTransparentLabel9";
            this.toolTipProvider.SetToolTip(this.doaTransparentLabel9, resources.GetString("doaTransparentLabel9.ToolTip"));
            // 
            // doaTransparentLabel10
            // 
            this.doaTransparentLabel10.AccessibleDescription = null;
            this.doaTransparentLabel10.AccessibleName = null;
            resources.ApplyResources(this.doaTransparentLabel10, "doaTransparentLabel10");
            this.doaTransparentLabel10.BackColor = System.Drawing.Color.Transparent;
            this.doaTransparentLabel10.BackgroundImage = null;
            this.doaTransparentLabel10.DimmedColor = System.Drawing.Color.White;
            this.doaTransparentLabel10.Font = null;
            this.doaTransparentLabel10.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaTransparentLabel10.Name = "doaTransparentLabel10";
            this.toolTipProvider.SetToolTip(this.doaTransparentLabel10, resources.GetString("doaTransparentLabel10.ToolTip"));
            // 
            // btnTrfTestConnection
            // 
            this.btnTrfTestConnection.AccessibleDescription = null;
            this.btnTrfTestConnection.AccessibleName = null;
            resources.ApplyResources(this.btnTrfTestConnection, "btnTrfTestConnection");
            this.btnTrfTestConnection.BackColor = System.Drawing.Color.Transparent;
            this.btnTrfTestConnection.BackgroundImage = null;
            this.btnTrfTestConnection.ButtonStyle = System.Windows.Forms.Styled.VistaButton.Style.Flat;
            this.btnTrfTestConnection.ButtonText = null;
            this.btnTrfTestConnection.CornerRadius = 25;
            this.btnTrfTestConnection.Font = null;
            this.btnTrfTestConnection.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnTrfTestConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnTrfTestConnection.Image")));
            this.btnTrfTestConnection.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnTrfTestConnection.Name = "btnTrfTestConnection";
            this.toolTipProvider.SetToolTip(this.btnTrfTestConnection, resources.GetString("btnTrfTestConnection.ToolTip"));
            this.btnTrfTestConnection.Click += new System.EventHandler(this.TestConnection_Click);
            // 
            // doaWebUrl
            // 
            this.doaWebUrl.AccessibleDescription = null;
            this.doaWebUrl.AccessibleName = null;
            resources.ApplyResources(this.doaWebUrl, "doaWebUrl");
            this.doaWebUrl.BackColor = System.Drawing.Color.Transparent;
            this.doaWebUrl.BackgroundImage = null;
            this.doaWebUrl.DimmedColor = System.Drawing.Color.White;
            this.doaWebUrl.Font = null;
            this.doaWebUrl.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaWebUrl.Name = "doaWebUrl";
            this.toolTipProvider.SetToolTip(this.doaWebUrl, resources.GetString("doaWebUrl.ToolTip"));
            // 
            // gbPrinters
            // 
            this.gbPrinters.AccessibleDescription = null;
            this.gbPrinters.AccessibleName = null;
            resources.ApplyResources(this.gbPrinters, "gbPrinters");
            this.gbPrinters.BackgroundImage = null;
            this.gbPrinters.Controls.Add(this.lblPrintersNote);
            this.gbPrinters.Controls.Add(this.doaTransparentLabel1);
            this.gbPrinters.Controls.Add(this.cbPrinters);
            this.gbPrinters.Font = null;
            this.gbPrinters.Name = "gbPrinters";
            this.gbPrinters.TabStop = false;
            this.toolTipProvider.SetToolTip(this.gbPrinters, resources.GetString("gbPrinters.ToolTip"));
            // 
            // lblPrintersNote
            // 
            this.lblPrintersNote.AccessibleDescription = null;
            this.lblPrintersNote.AccessibleName = null;
            resources.ApplyResources(this.lblPrintersNote, "lblPrintersNote");
            this.lblPrintersNote.Font = null;
            this.lblPrintersNote.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPrintersNote.Name = "lblPrintersNote";
            this.toolTipProvider.SetToolTip(this.lblPrintersNote, resources.GetString("lblPrintersNote.ToolTip"));
            // 
            // doaTransparentLabel1
            // 
            this.doaTransparentLabel1.AccessibleDescription = null;
            this.doaTransparentLabel1.AccessibleName = null;
            resources.ApplyResources(this.doaTransparentLabel1, "doaTransparentLabel1");
            this.doaTransparentLabel1.BackColor = System.Drawing.Color.Transparent;
            this.doaTransparentLabel1.BackgroundImage = null;
            this.doaTransparentLabel1.DimmedColor = System.Drawing.Color.White;
            this.doaTransparentLabel1.Font = null;
            this.doaTransparentLabel1.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaTransparentLabel1.Name = "doaTransparentLabel1";
            this.toolTipProvider.SetToolTip(this.doaTransparentLabel1, resources.GetString("doaTransparentLabel1.ToolTip"));
            // 
            // cbPrinters
            // 
            this.cbPrinters.AccessibleDescription = null;
            this.cbPrinters.AccessibleName = null;
            resources.ApplyResources(this.cbPrinters, "cbPrinters");
            this.cbPrinters.BackgroundImage = null;
            this.cbPrinters.DefaultItemForeColor = System.Drawing.Color.Black;
            this.cbPrinters.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrinters.Font = null;
            this.cbPrinters.FormattingEnabled = true;
            this.cbPrinters.Name = "cbPrinters";
            this.toolTipProvider.SetToolTip(this.cbPrinters, resources.GetString("cbPrinters.ToolTip"));
            // 
            // ptbTRSURL
            // 
            this.ptbTRSURL.AccessibleDescription = null;
            this.ptbTRSURL.AccessibleName = null;
            resources.ApplyResources(this.ptbTRSURL, "ptbTRSURL");
            this.ptbTRSURL.BackgroundImage = null;
            this.ptbTRSURL.FocusSelect = true;
            this.ptbTRSURL.Font = null;
            this.extender.SetHookupToTextBoxBase(this.ptbTRSURL, true);
            this.ptbTRSURL.Name = "ptbTRSURL";
            this.toolTipProvider.SetToolTip(this.ptbTRSURL, resources.GetString("ptbTRSURL.ToolTip"));
            // 
            // ptbScanner
            // 
            this.ptbScanner.AccessibleDescription = null;
            this.ptbScanner.AccessibleName = null;
            resources.ApplyResources(this.ptbScanner, "ptbScanner");
            this.ptbScanner.BackgroundImage = null;
            this.ptbScanner.FocusSelect = true;
            this.ptbScanner.Font = null;
            this.extender.SetHookupToTextBoxBase(this.ptbScanner, true);
            this.ptbScanner.Name = "ptbScanner";
            this.toolTipProvider.SetToolTip(this.ptbScanner, resources.GetString("ptbScanner.ToolTip"));
            // 
            // gbTRS
            // 
            this.gbTRS.AccessibleDescription = null;
            this.gbTRS.AccessibleName = null;
            resources.ApplyResources(this.gbTRS, "gbTRS");
            this.gbTRS.BackgroundImage = null;
            this.gbTRS.Controls.Add(this.doaTransparentLabel2);
            this.gbTRS.Controls.Add(this.ptbTRSURL);
            this.gbTRS.Font = null;
            this.gbTRS.Name = "gbTRS";
            this.gbTRS.TabStop = false;
            this.toolTipProvider.SetToolTip(this.gbTRS, resources.GetString("gbTRS.ToolTip"));
            // 
            // doaTransparentLabel2
            // 
            this.doaTransparentLabel2.AccessibleDescription = null;
            this.doaTransparentLabel2.AccessibleName = null;
            resources.ApplyResources(this.doaTransparentLabel2, "doaTransparentLabel2");
            this.doaTransparentLabel2.BackColor = System.Drawing.Color.Transparent;
            this.doaTransparentLabel2.BackgroundImage = null;
            this.doaTransparentLabel2.DimmedColor = System.Drawing.Color.White;
            this.doaTransparentLabel2.Font = null;
            this.doaTransparentLabel2.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaTransparentLabel2.Name = "doaTransparentLabel2";
            this.toolTipProvider.SetToolTip(this.doaTransparentLabel2, resources.GetString("doaTransparentLabel2.ToolTip"));
            // 
            // cbScanners
            // 
            this.cbScanners.AccessibleDescription = null;
            this.cbScanners.AccessibleName = null;
            resources.ApplyResources(this.cbScanners, "cbScanners");
            this.cbScanners.BackgroundImage = null;
            this.cbScanners.DefaultItemForeColor = System.Drawing.Color.Black;
            this.cbScanners.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbScanners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScanners.Font = null;
            this.cbScanners.FormattingEnabled = true;
            this.cbScanners.Name = "cbScanners";
            this.toolTipProvider.SetToolTip(this.cbScanners, resources.GetString("cbScanners.ToolTip"));
            // 
            // gbScanner
            // 
            this.gbScanner.AccessibleDescription = null;
            this.gbScanner.AccessibleName = null;
            resources.ApplyResources(this.gbScanner, "gbScanner");
            this.gbScanner.BackgroundImage = null;
            this.gbScanner.Controls.Add(this.cbScanners);
            this.gbScanner.Controls.Add(this.doaTransparentLabel3);
            this.gbScanner.Controls.Add(this.ptbScanner);
            this.gbScanner.Font = null;
            this.gbScanner.Name = "gbScanner";
            this.gbScanner.TabStop = false;
            this.toolTipProvider.SetToolTip(this.gbScanner, resources.GetString("gbScanner.ToolTip"));
            // 
            // doaTransparentLabel3
            // 
            this.doaTransparentLabel3.AccessibleDescription = null;
            this.doaTransparentLabel3.AccessibleName = null;
            resources.ApplyResources(this.doaTransparentLabel3, "doaTransparentLabel3");
            this.doaTransparentLabel3.BackColor = System.Drawing.Color.Transparent;
            this.doaTransparentLabel3.BackgroundImage = null;
            this.doaTransparentLabel3.DimmedColor = System.Drawing.Color.White;
            this.doaTransparentLabel3.Font = null;
            this.doaTransparentLabel3.ForeColor = System.Drawing.Color.SteelBlue;
            this.doaTransparentLabel3.Name = "doaTransparentLabel3";
            this.toolTipProvider.SetToolTip(this.doaTransparentLabel3, resources.GetString("doaTransparentLabel3.ToolTip"));
            // 
            // extender
            // 
            this.extender.Control = null;
            // 
            // SettingsPage3
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = null;
            this.Controls.Add(this.gbTRS);
            this.Controls.Add(this.gbTransfferingService);
            this.Controls.Add(this.gbPrinters);
            this.Controls.Add(this.gbScanner);
            this.Font = null;
            this.ForeColor = System.Drawing.Color.Turquoise;
            this.Name = "SettingsPage3";
            this.toolTipProvider.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.gbTransfferingService.ResumeLayout(false);
            this.gbTransfferingService.PerformLayout();
            this.gbPrinters.ResumeLayout(false);
            this.gbPrinters.PerformLayout();
            this.gbTRS.ResumeLayout(false);
            this.gbTRS.PerformLayout();
            this.gbScanner.ResumeLayout(false);
            this.gbScanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.extender)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PremierTaxFree.DirtyControlExtender extender;
        private System.Windows.Forms.GroupBox gbTransfferingService;
        private VistaButton btnInstallWeb;
        private VistaButton btnInstallLocal;
        private DOATransparentLabel doaTransparentLabel9;
        private DOATransparentLabel doaTransparentLabel10;
        private VistaButton btnTrfTestConnection;
        private PromptedTextBox ptbWebTracerUrl;
        private DOATransparentLabel doaWebUrl;
        private System.Windows.Forms.GroupBox gbPrinters;
        private DOATransparentLabel doaTransparentLabel1;
        private MultyColorComboBox cbPrinters;
        private System.Windows.Forms.Label lblPrintersNote;
        private System.Windows.Forms.GroupBox gbTRS;
        private DOATransparentLabel doaTransparentLabel2;
        private PromptedTextBox ptbTRSURL;
        private System.Windows.Forms.ToolTip toolTipProvider;
        private System.Windows.Forms.GroupBox gbScanner;
        private DOATransparentLabel doaTransparentLabel3;
        private PromptedTextBox ptbScanner;
        private MultyColorComboBox cbScanners;

    }
}
