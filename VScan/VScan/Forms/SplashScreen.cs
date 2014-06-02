/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree
{
    public partial class SplashScreen : Form
    {
        private readonly Timer m_Timer = new Timer();
        private bool fadeIn = true;
        private bool fadeOut = false;

        string text;

        public SplashScreen()
        {
            InitializeComponent();
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            text = string.Format("{0}\r\n{1}\r\nVersion: {2}\r\nLanguage: {3}",
                                    Application.ProductName,
                                    Application.CompanyName,
                                    version.ToString(),
                                    Application.CurrentCulture);

            StartTimer();
        }

        protected override void OnLoad(EventArgs e)
        {
            var _1st = true.Random();
            var img = _1st ? Resources.DELL2155cn : Resources.Fi_5120C;

            Color c = img.GetPixel(0, 0);
            img.MakeTransparent(c);

            this.panel1.BackgroundImage = img;
            this.panel1.Paint += (s, ev) =>
            {
                SizeF size = ev.Graphics.MeasureString(text, Font);
                ev.Graphics.DrawString(text, this.panel1.Font, _1st ? Brushes.Snow : Brushes.Black, this.ClientSize.Center(size.ToSize()));
            };
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 1;
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Timer.Dispose();
            base.OnClosed(e);
        }

        private void StartTimer()
        {
            m_Timer.Interval = 100;
            m_Timer.Tick += new EventHandler(Timer_Tick);
            m_Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (fadeIn)
            {
                if (this.Opacity < 1.0)
                {
                    this.Opacity += 0.02;
                }
                else
                {
                    fadeIn = false;
                    fadeOut = true;
                }
            }
            else if (fadeOut) 
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.02;
                }
                else
                {
                    fadeOut = false;
                }
            }

            if (!(fadeIn || fadeOut))
            {
                m_Timer.Stop();
                this.Close();
            }
        }
    }
}
