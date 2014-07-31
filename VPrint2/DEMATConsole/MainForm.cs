using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace DEMATConsole
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
                return;

            try
            {
                int command = int.Parse(tbCommand.Text);
                ServiceController service = new ServiceController(comboBox1.Text);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.ExecuteCommand(command);
                    lblMessage.Text = "Sent";
                }
                else
                {
                    lblMessage.Text = "Not running";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}
