/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using PremierTaxFree.Data;
using PremierTaxFree.Extensions;
using PremierTaxFree.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Net;

namespace PremierTaxFree.Controls
{
    public partial class SettingsPage1 : UserControl, ISettingsControl
    {
        public SettingsPage1()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            extender.Control = this;
            ptbDelayBeforeClean.ValidatingType = typeof(TimeSpan);
            ptbDelayBeforeClean.Mask = "00:00:00";
        }

        public bool IsDirty { get; set; }

        public void Read()
        {
            vcbUseDefaultScanner.Checked = SettingsTable.Get<bool>(Strings.VScan_TWAINUseDefaultScanner);
            vcbUseDefaultScannerSettings.Checked = SettingsTable.Get<bool>(Strings.VScan_TWAINUseDefaultScannerSettings);
            vcbAutoReadBarcode.Checked = SettingsTable.Get<bool>(Strings.VScan_AutoReadBarcodeAfterScan);
            vcbAutoInsertData.Checked = SettingsTable.Get<bool>(Strings.VScan_AutoInsertDataAfterScan);
            ptbMaxOpenForms.Text = SettingsTable.Get<int>(Strings.VScan_MaximumOpenedScanForms).ToString();
            ptbMinSiteIDS.Text = SettingsTable.Get<int>(Strings.VScan_MinSiteIDSAvailable).ToString();
            ptbHistoryDays.Text = SettingsTable.Get<int>(Strings.VScan_KeepHistoryDays).ToString();
            ptbDefaultCountryCode.Text = SettingsTable.Get<int>(Strings.VScan_DefaultCountryCode, 826).ToString();
            ptbDelayBeforeClean.Text = SettingsTable.Get<TimeSpan>(Strings.VScan_SleepBeforeCleanTime, TimeSpan.FromSeconds(10)).ToString();
            ptbDelayBeforeAutoClose.Text = SettingsTable.Get<TimeSpan>(Strings.VScan_ScanFormAutoClose, TimeSpan.FromSeconds(35)).ToString();

            vcbUseImprinter.Checked = SettingsTable.Get<bool>(Strings.VScan_UseImPrinter, false);
            m_BorderColor = SettingsTable.Get<Color>(Strings.VScan_ImageBorderColor, Color.White);
            ptbBorderColorDistance.Text = SettingsTable.Get<int>(Strings.VScan_ImageBorderColorDistance, 40).ToString();
            ptbImprinterTemplate.Text = SettingsTable.Get<string>(Strings.VScan_ImPrinterTemplate, Strings.VScan_ImPrinterTemplateDefault);
            vcbPrintOnImage.Checked = SettingsTable.Get<bool>(Strings.VScan_PrintOnImage, false);
            vbtnAllowCrop.Checked = SettingsTable.Get<bool>(Strings.VScan_AllowCropTool, false);
            vcbCardCodeByBarcode.Checked = SettingsTable.Get<bool>(Strings.VScan_HideCardCodeDetailsBybarcode, true);

            m_BorderColor = SettingsTable.Get<Color>(Strings.VScan_ImageBorderColor, Color.White);
            m_HiddenArea = SettingsTable.Get<HiddenAreaDrawingCfg>(Strings.VScan_HiddenAreaDrawingCfg, HiddenAreaDrawingCfg.Default);
        }

        public bool Verify()
        {
            int i;
            if (string.IsNullOrEmpty(ptbMaxOpenForms.Text))
                throw new AppExclamationException("Max opened windows cannot be empty.");
            if (!int.TryParse(ptbMaxOpenForms.Text, out i))
                throw new AppExclamationException("Max opened windows should be integer.");

            if (string.IsNullOrEmpty(ptbMinSiteIDS.Text))
                throw new AppExclamationException("Min site ids cannot be empty.");
            if (!int.TryParse(ptbMinSiteIDS.Text, out i))
                throw new AppExclamationException("Min site ids should be integer.");

            if (string.IsNullOrEmpty(ptbHistoryDays.Text))
                throw new AppExclamationException("History days cannot be empty.");
            if (!int.TryParse(ptbHistoryDays.Text, out i))
                throw new AppExclamationException("History days should be integer.");
            TimeSpan time;
            if (!TimeSpan.TryParse(ptbDelayBeforeClean.Text, out time))
                throw new AppExclamationException("Delay before cleaning should be a valid time variable.");
            if (!TimeSpan.TryParse(ptbDelayBeforeAutoClose.Text, out time) || time < TimeSpan.FromSeconds(20))
                throw new AppExclamationException("Delay before autoclose should be a valid time variable and more than 20s.");
            if (!int.TryParse(ptbBorderColorDistance.Text, out i) || i < 0 || i > 100)
                throw new AppExclamationException("Color distance should be integer value between 0 and 100");
            return true;
        }

        public void Save()
        {
            SettingsTable.Set(Strings.VScan_TWAINUseDefaultScanner, vcbUseDefaultScanner.Checked);
            SettingsTable.Set(Strings.VScan_TWAINUseDefaultScannerSettings, vcbUseDefaultScannerSettings.Checked);
            SettingsTable.Set(Strings.VScan_AutoReadBarcodeAfterScan, vcbAutoReadBarcode.Checked);
            SettingsTable.Set(Strings.VScan_AutoInsertDataAfterScan, vcbAutoInsertData.Checked);
            SettingsTable.Set(Strings.VScan_MaximumOpenedScanForms, int.Parse(ptbMaxOpenForms.Text));
            SettingsTable.Set(Strings.VScan_MinSiteIDSAvailable, int.Parse(ptbMinSiteIDS.Text));
            SettingsTable.Set(Strings.VScan_KeepHistoryDays, int.Parse(ptbHistoryDays.Text));
            SettingsTable.Set(Strings.VScan_DefaultCountryCode, int.Parse(ptbDefaultCountryCode.Text));
            SettingsTable.Set(Strings.VScan_SleepBeforeCleanTime, TimeSpan.Parse(ptbDelayBeforeClean.Text));
            SettingsTable.Set(Strings.VScan_ScanFormAutoClose, TimeSpan.Parse(ptbDelayBeforeAutoClose.Text));

            SettingsTable.Set(Strings.VScan_UseImPrinter, vcbUseImprinter.Checked);
            SettingsTable.Set(Strings.VScan_ImageBorderColor, m_BorderColor);
            SettingsTable.Set(Strings.VScan_ImageBorderColorDistance, int.Parse(ptbBorderColorDistance.Text));
            SettingsTable.Set(Strings.VScan_ImPrinterTemplate, ptbImprinterTemplate.Text);
            SettingsTable.Set(Strings.VScan_PrintOnImage, vcbPrintOnImage.Checked);
            SettingsTable.Set(Strings.VScan_AllowCropTool, vbtnAllowCrop.Checked);
            SettingsTable.Set(Strings.VScan_HideCardCodeDetailsBybarcode, vcbCardCodeByBarcode.Checked);

            SettingsTable.Set(Strings.VScan_ImageBorderColor, m_BorderColor);
            SettingsTable.Set(Strings.VScan_HiddenAreaDrawingCfg, m_HiddenArea);
        }

        public void UpdateEnvironment()
        {
            DBConfigValue.Save(Strings.Transferring_SettingsObject,
                new SettingsObj()
                {
                    ConnectionString = ClientDataAccess.ConnectionString,
                    CentralServerUrl = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault),
                    SendInterval = SettingsTable.Get<int>(Strings.VScan_TranfSendInterval, 1),
                    KeepHistoryDays = SettingsTable.Get<int>(Strings.VScan_KeepHistoryDays, 1),
                });

            this.ShowWarning("When change scanner settings\r\n" +
                    "you may need to restart scanner as well");
        }

        private void CardCodeRegions_Click(object sender, System.EventArgs e)
        {
            CardCodeRegionsForm form = new CardCodeRegionsForm();
            form.ShowDialog(this);
        }

        private Color m_BorderColor;
        private HiddenAreaDrawingCfg m_HiddenArea;

        private void ColorStyleButton_Click(object sender, EventArgs e)
        {
            if (sender == vbtnBorderColor)
            {
                using (var form = new ColorDialog())
                {
                    form.Color = m_BorderColor;
                    if (form.ShowDialog(this) == DialogResult.OK)
                        m_BorderColor = form.Color;
                }
            }
            else if (sender == vbtnHiddenAreaStyle)
            {
                using (var form = new HatchStyleForm())
                {
                    form.Back_Color = m_HiddenArea.BackColor;
                    form.Fore_Color = m_HiddenArea.ForeColor;
                    form.Style = m_HiddenArea.Style;

                    if (form.ShowDialog(this) == DialogResult.OK)
                        m_HiddenArea = new HiddenAreaDrawingCfg()
                        {
                            BackColor = form.Back_Color,
                            ForeColor = form.Fore_Color,
                            Style = form.Style,
                        };
                }
            }
        }
    }
}
