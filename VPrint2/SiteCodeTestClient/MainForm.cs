using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace SiteCodeTestClient
{
    public partial class MainForm : Form
    {
        private readonly SynchronizationContext m_Context;
        private readonly List<Task> m_Tasks = new List<Task>();

        public MainForm()
        {
            m_Context = SynchronizationContext.Current;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string TMP = "{0}P2300D2";

            if (m_Tasks.Count > 0)
                Task.WaitAll(m_Tasks.ToArray());

            m_Tasks.Clear();

            for (int i = 0; i < 10; i++)
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Random rnd = new Random();

                        SiteCodeServerProxy.SiteCodeClient client = new SiteCodeServerProxy.SiteCodeClient();
                        client.Open();

                        for (int j = 0; j < 100; j++)
                        {
                            foreach (string value in new string[] { "100018P2300D2", 
                                                                    string.Format(TMP, rnd.Next(90000, 90009))  ,
                                                                    "100018P1Half32D2" , 
                                                                    "100018P2858D1", 
                                                                    "100948P2372D1", 
                                                                    "102349P156D2", 
                                                                    "105636P2756D2", 
                                                                    "158999P1702D1" })
                            {
                                var loc = client.GetLocation(value);
                                if (loc == null)
                                    m_Context.Post(UpdateUI, new Exception("NA"));
                                else
                                    m_Context.Post(UpdateUI, string.Format("{0:H:mm:ss.ffff} -       {1} - {2} - {3}",
                                        DateTime.Now, loc.Code, loc.Site, loc.Number));
                            }
                            Thread.Sleep(100);
                        }
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        m_Context.Post(UpdateUI, ex);
                    }
                });

                m_Tasks.Add(t1);

                var t2 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        SiteCodeServerProxy.SiteCodeClient client = new SiteCodeServerProxy.SiteCodeClient();
                        client.Open();
                        foreach (var code in new string[] { "276", "826", "724", "40", "300" })
                        {
                            var result = client.GetShortCode(code);
                            if (result == null)
                                m_Context.Post(UpdateUI, new Exception("NA"));
                            else
                                m_Context.Post(UpdateUI, string.Format("{0:H:mm:ss.ffff} -       {1} - {2}", DateTime.Now, code, result));
                        }
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        m_Context.Post(UpdateUI, ex);
                    }
                });

                m_Tasks.Add(t2);
            }
        }

        private void UpdateUI(object state)
        {
            if (!txtDisplay.IsDisposed)
            {
                string text = state as string;
                if (text != null)
                {
                    if (txtDisplay.Lines.Length > 1000)
                        txtDisplay.Clear();
                    txtDisplay.AppendText(text);
                    txtDisplay.AppendText(Environment.NewLine);
                }
                else
                {
                    Exception ex = state as Exception;
                    if (ex != null)
                    {
                        txtDisplay.SelectionColor = Color.Red;
                        txtDisplay.AppendText(ex.ToString());
                        txtDisplay.SelectionColor = Color.Black;
                        txtDisplay.AppendText(Environment.NewLine);
                    }
                }
            }
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SiteCodeServerProxy.SiteCodeClient client = new SiteCodeServerProxy.SiteCodeClient();
                    client.Open();
                    m_Context.Post(UpdateUI, client.GetInfo());
                    client.Close();
                }
                catch (Exception ex)
                {
                    m_Context.Post(UpdateUI, ex);
                }
            });
        }

        private void btnSaveCommand_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SiteCodeServerProxy.SiteCodeClient client = new SiteCodeServerProxy.SiteCodeClient();
                    client.Open();
                    client.SaveCommand();
                    client.Close();
                }
                catch (Exception ex)
                {
                    m_Context.Post(UpdateUI, ex);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServiceReference1.AuthenticationHeader header = new ServiceReference1.AuthenticationHeader();
            ServiceReference1.PartyManagementSoapClient client = new ServiceReference1.PartyManagementSoapClient();
            var buffer = client.RetrieveTableData(header, "br_name, br_position_1, br_position_2, br_position_3", "Branch", "where br_id=1 and br_iso_id=826");
            ArrayList rlist = new ArrayList(buffer);
            Debug.WriteLine(rlist);
        }
    }
}
