/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPrint2.Common;
using CPrint2.Data;
using CPrint2.Properties;

namespace CPrint2
{
    public class AppContext : ApplicationContext 
    {
        public static AppContext Default { get; set; }

        private readonly MultyCamForm m_form = new MultyCamForm();
        private readonly MenuItem m_showMenuItem, m_closeMenuItem, m_startMenuItem, m_lockMenuItem, m_exitMenuItem;
        private readonly NotifyIcon m_notifyIcon = new NotifyIcon();
        private readonly SyncServiceDataAccess m_CommandWatcher = new SyncServiceDataAccess();
        public event ThreadExceptionEventHandler Error;

        public AppContext()
        {
            m_showMenuItem = new MenuItem("Login", new EventHandler(ShowHideMainForm_Click));
            m_startMenuItem = new MenuItem("Start", new EventHandler(StartStopMenuItem_Click));
            m_lockMenuItem = new MenuItem("Lock", new EventHandler(LockUnlockMenuItem_Click));
            m_closeMenuItem = new MenuItem("Close", new EventHandler(Close_Click));
            m_exitMenuItem = new MenuItem("Exit", new EventHandler(Exit_Click));
            
            m_notifyIcon.Icon = Resources.camera_unmount2;
            m_notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { m_showMenuItem, m_startMenuItem, m_lockMenuItem, m_closeMenuItem, m_exitMenuItem });
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            m_notifyIcon.DoubleClick += new EventHandler(ShowHideMainForm_Click);

            m_CommandWatcher.Submit += new EventHandler<ValueEventArgs<DataObj2>>(m_CommandWatcher_Submit);

            Default = this; 
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            MultyCamForm.Stop();
        }

        public bool IsStarted
        {
            get
            {
                return m_CommandWatcher.Running;
            }
        }

        public bool IsLocked
        {
            get
            {
                return m_form.Enabled;
            }
        }

        public void Start()
        {
            m_CommandWatcher.Start();
        }

        public void Stop()
        {
            m_CommandWatcher.Stop();
        }

        public void Exit()
        {
            m_notifyIcon.Visible = false;
            
            Application.Exit();
        }

        public void Reset()
        {
            m_form.ResetState();
        }

        public void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }

        private void StartStopMenuItem_Click(object sender, EventArgs e)
        {
            if (IsStarted)
            {
                m_startMenuItem.Text = "Start";
                Stop();
            }
            else
            {
                m_startMenuItem.Text = "Stop";
                Start();
            }
        }

        private void LockUnlockMenuItem_Click(object sender, EventArgs e)
        {
            if (IsLocked)
            {
                m_lockMenuItem.Text = "Lock";
                m_form.Enabled = true;
            }
            else
            {
                m_lockMenuItem.Text = "Unlock";
                m_form.Enabled = false;
            }
        }

        private void ShowHideMainForm_Click(object sender, EventArgs e)
        {
            if (m_form.Visible)
            {
                m_showMenuItem.Text = "Show";
                m_form.Hide();
            }
            else
            {
                if (Program.currentUser == null)
                {
                    using (var form = new FormLogin())
                        form.ShowDialog();
                }
                else
                {
                    m_showMenuItem.Text = "Hide";
                    m_form.ShowDialog();
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Program.currentUser = null;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            m_showMenuItem.Text = (Program.currentUser == null) ? "Login" : "Show";
            m_startMenuItem.Enabled = (Program.currentUser != null);
            m_lockMenuItem.Enabled = (Program.currentUser != null);
        }

        private void m_CommandWatcher_Submit(object sender, ValueEventArgs<DataObj2> e)
        {
            //TODO
            MultyCamForm.Default.ProcessCommand(e.Value);
        }
    }
}
