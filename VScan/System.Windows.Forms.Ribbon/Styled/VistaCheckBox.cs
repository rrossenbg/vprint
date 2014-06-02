using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms.Styled
{
    public partial class VistaCheckBox : VistaButton
    {
        private bool m_Checked = false;
        public bool Checked
        {
            get
            {
                return m_Checked;
            }
            set
            {
                m_Checked = value;
                ShowState();
            }
        }

        public VistaCheckBox()
        {
            InitializeComponent();
            this.ButtonStyle = Style.Flat;
        }

        private void ShowState()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(VistaCheckBox));

            this.Image = m_Checked ?
                ((Image)(resources.GetObject("ButtonYes"))) :
                ((Image)(resources.GetObject("ButtonNo")));

            this.ButtonColor = m_Checked ?
                Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(211)))), ((int)(((byte)(40))))) :
                Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))); ;
        }

        private void VistaCheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
        }
    }
}
