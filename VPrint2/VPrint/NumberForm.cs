using System;
using System.Windows.Forms;

namespace VPrinting
{
    public partial class NumberForm : Form
    {
        public NumberForm()
        {
            InitializeComponent();
            numericUpDown1.Minimum = 1;
        }

        public static int ShowDlg(string message, int maximum)
        {
            using (NumberForm form = new NumberForm())
            {
                form.txtMessage.Text = message;
                form.numericUpDown1.Maximum = maximum;
                form.ShowDialog();
                return Convert.ToInt32(form.numericUpDown1.Value);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
