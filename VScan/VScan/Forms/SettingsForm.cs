/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using PremierTaxFree.Controls;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;


namespace PremierTaxFree
{
    public partial class SettingsForm : RibbonForm, ISettingsControl
    {
        ToolTip toolTip1;
        RibbonButton[] m_RibbonButtons;

        protected override Ribbon Ribbon
        {
            get { return ribbon1; }
        }

        public SettingsForm()
        {
            InitializeComponent();

            var cnt1 = new SettingsPage1();
            btnGeneral.Tag = cnt1;

            var cnt2 = new SettingsPage2();
            btnGeneral2.Tag = cnt2;

            var cnt3 = new SettingsPage3();
            btnConnection.Tag = cnt3;

            var cnt4 = new SettingsPage4();
            btnAdvance.Tag = cnt4;

            var cnt5 = new SettingsPage5();
            btnSecurity.Tag = cnt5;

            this.Controls.AddRange(new Control[] { cnt1, cnt2, cnt3, cnt4, cnt5 });

            this.toolTip1 = new ToolTip(new Container());
            //this.toolTip1.SetToolTip(this.btnGeneral, "This is a tooltip.");
            //this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            //this.toolTip1.ToolTipTitle = "Titled ToolTip";

            m_RibbonButtons = new RibbonButton[] { btnGeneral, btnGeneral2, btnConnection, btnAdvance, btnSecurity };
        }

        public bool IsDirty
        {
            get
            {
                bool isDirty = false;

                foreach (var cnt in this.Controls)
                {
                    if (cnt is ISettingsControl && ((ISettingsControl)cnt).IsDirty)
                    {
                        isDirty = true;
                        break;
                    }
                }

                return isDirty;
            }
            set
            {
                //Nothing to be done here
            }
        }
          
        public void Read()
        {
            foreach (var cnt in this.Controls)
                if (cnt is ISettingsControl)
                    ((ISettingsControl)cnt).Read();
        }

        public bool Verify()
        {
            try
            {
                foreach (var cnt in this.Controls)
                    if (cnt is ISettingsControl && ((ISettingsControl)cnt).IsDirty)
                        ((ISettingsControl)cnt).Verify();
                return true;
            }
            catch (Exception ex)
            {
                this.ShowError(ex.Message);
                return false;
            }
        }

        public void Save()
        {
            foreach (var cnt in this.Controls)
                if (cnt is ISettingsControl && ((ISettingsControl)cnt).IsDirty)
                    ((ISettingsControl)cnt).Save();
        }

        public void UpdateEnvironment()
        {
            foreach (var cnt in this.Controls)
                if (cnt is ISettingsControl && ((ISettingsControl)cnt).IsDirty)
                    ((ISettingsControl)cnt).UpdateEnvironment();
        }

        protected override void OnLoad(EventArgs e)
        {
            Read();
            btnGeneral.PerformClick();
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
                return;

            if (IsDirty)
            {
                DialogResult result = MessageBox.Show(this,
                    "Settings are not saved yet.\r\nWould you like to save them before close?", Application.ProductName,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes && Verify())
                {
                    Save();
                    UpdateEnvironment();
                }
                else
                {
                    e.Cancel = result == DialogResult.Cancel;
                }
            }

            base.OnClosing(e);
        }

        private Control m_ActiveControl;

        private void TabButton_Click(object sender, EventArgs e)
        {
            m_RibbonButtons.ForEach((b) => b.Bordered = b == sender);

            m_ActiveControl = (Control)((RibbonButton)sender).Tag;
            m_ActiveControl.Dock = DockStyle.Fill;
            m_ActiveControl.BringToFront();
            this.Invalidate(true);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Verify();
            Save();
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ISettingsControl cnt = (ISettingsControl)m_ActiveControl;
            Debug.Assert(cnt != null);

            if (cnt.IsDirty)
            {
                try
                {
                    cnt.Verify();
                    cnt.Save();
                    cnt.UpdateEnvironment();
                }
                catch (Exception ex)
                {
                    this.ShowError(ex.Message);
                }
            }
            else
            {
                this.ShowExclamation("Nothing to save.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}