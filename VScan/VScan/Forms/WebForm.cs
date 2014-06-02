/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;


namespace PremierTaxFree.Forms
{
    public partial class WebForm : Form
    {
        public WebForm()
        {
            InitializeComponent();
            this.Text = "Web browser";
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
        }

        protected const int MIN = 50, MAX = 200;
        private int m_ZoomFactor = 88;

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Zoom(m_ZoomFactor);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if ((m_ZoomFactor - (e.Delta / 20)).IsBetween(MIN, MAX))
            {
                m_ZoomFactor -= (e.Delta / 20);
                webBrowser1.Zoom(m_ZoomFactor);
            }
            base.OnMouseWheel(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            string url = SettingsTable.Get<string>(Strings.VScan_TRS_UrlAddress, Strings.VScan_TRS_UrlAddressDefault);
            this.webBrowser1.Url = new System.Uri(url, System.UriKind.Absolute);
            base.OnLoad(e);
        }
    }
}
