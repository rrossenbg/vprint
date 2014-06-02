/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects.Server;

using PremierTaxFree.PTFLib.Messages;

namespace WinDbLst
{
    public partial class MainForm : Form
    {
        const string strSQL = "SELECT FileID FROM dbo.FILES;";

        private readonly SqlDataListener m_Listener = new SqlDataListener(Program.strCONNSTR, strSQL, 15);

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            m_Listener.Refresh += new SqlRefreshDelegate(m_Listener_Refresh);
            m_Listener.Start();
            base.OnLoad(e);
        }

        void m_Listener_Refresh(SqlDataReader reader)
        {
            new MethodInvoker(() =>
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    DbVoucher result = ServerDataAccess.SelectLastInserted();
                    if (result != null)
                    {
                        tbVoucherText.Text = result.ToString();
                        pbVoucherImage.Image = result.VoucherImage.ToImage();
                        pbBarCode.Image = result.BarCodeImage.ToImage();
                    }
                }));
            }).FireAndForget();
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Listener.Refresh -= new SqlRefreshDelegate(m_Listener_Refresh);
            m_Listener.Dispose();
            base.OnClosed(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Listener_Refresh(null);
        }

        private void processMessageQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = MSMQ.GetMessageCount(Strings.All_SaveQueueName);
            foreach (var data in MSMQ.ReceiveAllFromQueue<Hashtable>(Strings.All_SaveQueueName, TimeSpan.FromSeconds(15), count))
                ServerDataAccess.InsertFileAsync(data, OnInsertErrorCallback);
        }

        public static void OnInsertErrorCallback(object sender, ThreadExceptionEventArgs args)
        {
            Trace.WriteLine("ReceivingWebService::OnInsertErrorCallback send to msmq -> ".concat(Strings.All_SaveQueueName));

            using (SQLWorker.CommandInfo info = (SQLWorker.CommandInfo)sender)
            {
                Debug.Assert(info != null, "Info is null");

                Hashtable table = SQL.CreateSerializationData(info.Command);                
                MSMQ.SendToQueue(Strings.All_SaveQueueName, DateTime.Now.ToString(), table);
            }
        }
    }
}
