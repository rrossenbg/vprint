using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VPrinting;
using VCover.Properties;

namespace VCover
{
    public class AppContext : ApplicationContext
    {
        public static AppContext Default { get; set; }
        private Form m_form = new Form();
        private readonly MenuItem m_showMenuItem, m_closeMenuItem, m_lockMenuItem, m_exitMenuItem;
        private readonly NotifyIcon m_notifyIcon = new NotifyIcon();
 
        public event EventHandler<ValueEventArgs<string>> NewInFileEvent;
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event ThreadExceptionEventHandler Error;

        public AppContext()
        {
            m_showMenuItem = new MenuItem("Login", new EventHandler(ShowHideMainForm_Click));
            m_lockMenuItem = new MenuItem("Lock", new EventHandler(LockUnlockMenuItem_Click));
            m_closeMenuItem = new MenuItem("Close", new EventHandler(Close_Click));
            m_exitMenuItem = new MenuItem("Exit", new EventHandler(Exit_Click));

            Bitmap bitmap = new Bitmap(Resources.PTFLogo);
            Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            m_notifyIcon.Icon = icon;
            m_notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { 
                m_showMenuItem, m_lockMenuItem, m_closeMenuItem, m_exitMenuItem });
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            m_notifyIcon.DoubleClick += new EventHandler(ShowHideMainForm_Click);

            Default = this;
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        public bool IsStarted { get; set; }

        public bool IsLocked
        {
            get
            {
                return m_form.Enabled;
            }
        }

        public void Start()
        {
            IsStarted = true;
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        public void Stop()
        {
            IsStarted = false;
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        public void Exit()
        {
            m_notifyIcon.Visible = false;
            Application.Exit();
        }

        public void Reset()
        {
        }

        public void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }

        public void NewImage(string filePath)
        {
            if (NewInFileEvent != null)
                NewInFileEvent(this, new ValueEventArgs<string>(filePath));
        }

        private void CommandWatcher_NewInFileCreated(object sender, FileSystemEventArgs e)
        {
            if (NewInFileEvent != null)
                NewInFileEvent(this, new ValueEventArgs<string>(e.FullPath));
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

        private void InitMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void RunOnceMenuItem_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void StartStopMenuItem_Click(object sender, EventArgs e)
        {
            if (IsStarted)
                Stop();
            else
                Start();
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
            m_lockMenuItem.Enabled = (Program.currentUser != null);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
        }
    }
}
