using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace WCFClient
{
    public partial class Form1 : Form
    {
        string endPointAddr = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                endPointAddr = "net.tcp://" + textBox2.Text + ":8000/MyService";
                NetTcpBinding tcpBinding = new NetTcpBinding();
                tcpBinding.TransactionFlow = false;
                tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                tcpBinding.Security.Mode = SecurityMode.None;

                EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);

                Append("Attempt to connect to: " + endPointAddr);

                IService1 proxy = ChannelFactory<IService1>.CreateChannel(tcpBinding, endpointAddress);

                using (proxy as IDisposable)
                {
                    Append("Message from server: " + (proxy.HelloWorld(textBox1.Text) + " back to you :)"));
                }   
            }
        }

        private void Append(string str)
        {
            textBox3.AppendText("\r\n" + str);
        }
    }
}