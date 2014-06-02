using System;
using System.Windows.Forms;

namespace VPrinting.Controls
{
    public partial class DateTimePicker2 : UserControl
    {
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

        public DateTime? Value
        {
            get
            {
                return cbEnabled.Checked ? dtPicker.Value : (DateTime?)null;
            }
            set
            {
                dtPicker.Value = (value.HasValue) ? value.Value : DateTime.Now;
            }
        }

        public DateTimePicker2()
        {
            InitializeComponent();
            dtPicker.Enabled = cbEnabled.Checked;
        }

        private void Enable_CheckedChanged(object sender, EventArgs e)
        {
            dtPicker.Enabled = cbEnabled.Checked;
        }
    }
}
