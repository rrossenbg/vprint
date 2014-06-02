using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BizTalkFiles
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string message = "Wrong Connection String";

            if (string.IsNullOrEmpty(txtConnStr.Text) || !new SqlConnection(txtConnStr.Text).Test(out message))
            {
                this.ShowError(message);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public static bool ShowDialog(IWin32Window owner, ref string connString)
        {
            using(InputForm form = new InputForm())
            {
                form.txtConnStr.Text = connString;

                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    connString = form.txtConnStr.Text;
                    return true;
                }
            }
            return false;
        }
    }
}
