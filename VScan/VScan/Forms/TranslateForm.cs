/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PremierTaxFree.Extensions;
using PremierTaxFree.Microsoft.Bing.TranslateService;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Forms
{
    public partial class TranslateForm : Form
    {
        private const string APP_ID = "D75686E2E43C3DB37BD3845F5F92BC594FF27974";

        public TranslateForm()
        {
            InitializeComponent();
            BindCombo(cbFromLang, CultureInfo.CurrentCulture);
            BindCombo(cbToLang, CultureInfo.GetCultureInfo("en-GB"));
        }

        private void BindCombo(ToolStripComboBox cb, CultureInfo select)
        {
            cb.Items.AddRange(CultureInfo.GetCultures(CultureTypes.FrameworkCultures));
            cb.SelectedItem = select;
        }

        private void Translate_Click(object sender, EventArgs e)
        {
            var fromLng = (CultureInfo)cbFromLang.SelectedItem;
            var toLng = (CultureInfo)cbToLang.SelectedItem;
            
            new Func<string, string>((s) =>
            {
                try
                {
                    var client = new LanguageServiceClient("BasicHttpBinding_LanguageService");
                    return client.Translate(APP_ID, s, fromLng.TwoLetterISOLanguageName, toLng.TwoLetterISOLanguageName);
                }
                catch (Exception ex)
                {
                    ex.ShowDialog();
                    return null;
                }
            }).RunAsync(new Action<string>((s) => this.InvokeSf(new MethodInvoker(() => tbTo.Text = s))), tbFrom.Text);
        }

        private Point m_MenuTarget;

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            m_MenuTarget = this.PointToClient(Control.MousePosition);
        }

        private void CutCopyPaste_Click(object sender, EventArgs e)
        {
            var pnl = splitContainer1.GetChildAtPoint(m_MenuTarget) as Panel;

            if (pnl != null)
            {
                var txt = pnl.Controls[0] as TextBox;

                Debug.Assert(txt != null, "The first child is always expected to be a TextBox");

                if (sender == tsmiCut)
                {
                    if (!string.IsNullOrEmpty(txt.SelectedText))
                    {
                        Clipboard.SetText(txt.SelectedText);
                        txt.Text = txt.Text.Replace(txt.SelectedText, "");
                    }
                }
                else if (sender == tsmiCopy)
                {
                    Clipboard.SetText(txt.SelectedText ?? txt.Text);
                }
                else if (sender == tsmiPaste)
                {
                    if (string.IsNullOrEmpty(txt.SelectedText))
                    {
                        txt.AppendText(Clipboard.GetText());
                    }
                    else
                    {
                        txt.Text = txt.Text.Replace(txt.SelectedText, Clipboard.GetText());
                    }
                }
                else
                    throw new NotImplementedException();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}