/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Windows.Forms;

namespace PremierTaxFree.Controls
{
    public partial class SettingsPage2 : UserControl, ISettingsControl
    {
        public bool IsDirty { get; set; }

        public SettingsPage2()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            extender.Control = this;
        }

        public void Read()
        {

        }

        public bool Verify()
        {
            return true;
        }

        public void Save()
        {

        }

        public void UpdateEnvironment()
        {

        }
    }
}
