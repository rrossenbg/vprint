using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace EnumBuilder
{
    public partial class MainForm : Form
    {
        public string BuildText { get; set; }

        public MainForm()
        {
            InitializeComponent();
            dgvData.AutoGenerateColumns = true;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {

        }

        private void Load_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet data = new DataSet();

                using (var conn = new SqlConnection(txtConnectionString.Text))
                {
                    conn.Open();

                    using (var comm = new SqlCommand(txtSQL.Text, conn))
                    {
                        var reader = comm.ExecuteReader();
                        data.Load(reader, LoadOption.OverwriteChanges, "Voucher");
                    }
                    dgvData.DataSource = data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
