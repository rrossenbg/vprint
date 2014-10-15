using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VCover.Data;
using VCover.Properties;
using VPrinting;

namespace VCover
{
    public class AppContext : ApplicationContext
    {
        public static AppContext Default { get; set; }
        private Form m_form = new Form();
        private readonly MenuItem m_showMenuItem, m_closeMenuItem, m_runMenuItem, m_startMenuItem, 
            m_initMenuItem, m_lockMenuItem, m_exitMenuItem;
        private readonly NotifyIcon m_notifyIcon = new NotifyIcon();
        private readonly FileSystemWatcher m_imageFileWatcher = new FileSystemWatcher();
        public event EventHandler<ValueEventArgs<string>> NewInFileEvent;
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event ThreadExceptionEventHandler Error;

        public string InFolder
        {
            set
            {
                m_imageFileWatcher.Path = value;
            }
        }

        public string InFilter
        {
            set
            {
                m_imageFileWatcher.Filter = value;
            }
        }

        public AppContext()
        {
            m_showMenuItem = new MenuItem("Login", new EventHandler(ShowHideMainForm_Click));
            m_initMenuItem = new MenuItem("Init", new EventHandler(InitMenuItem_Click));
            m_runMenuItem = new MenuItem("Run", new EventHandler(RunOnceMenuItem_Click));
            m_startMenuItem = new MenuItem("Start", new EventHandler(StartStopMenuItem_Click));
            m_lockMenuItem = new MenuItem("Lock", new EventHandler(LockUnlockMenuItem_Click));
            m_closeMenuItem = new MenuItem("Close", new EventHandler(Close_Click));
            m_exitMenuItem = new MenuItem("Exit", new EventHandler(Exit_Click));

            Bitmap bitmap = new Bitmap(Resources.PTFLogo);
            Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            m_notifyIcon.Icon = icon;
            m_notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { m_showMenuItem, m_initMenuItem, 
                #if DEBUG
                m_runMenuItem ,
#endif
                m_startMenuItem, m_lockMenuItem, m_closeMenuItem, m_exitMenuItem });
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            m_notifyIcon.DoubleClick += new EventHandler(ShowHideMainForm_Click);

            m_imageFileWatcher.Path = Config.IN_FOLDER;
            m_imageFileWatcher.Filter = Config.FAILURE_FOLDER;
            m_imageFileWatcher.Created += new FileSystemEventHandler(CommandWatcher_NewInFileCreated);

            Default = this;
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        public bool IsStarted
        {
            get
            {
                return m_imageFileWatcher.EnableRaisingEvents;
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
            m_imageFileWatcher.EnableRaisingEvents = true;
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        public void Stop()
        {
            m_imageFileWatcher.EnableRaisingEvents = false;
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
            m_startMenuItem.Enabled = (Program.currentUser != null);
            m_initMenuItem.Enabled = (Program.currentUser != null);
            m_lockMenuItem.Enabled = (Program.currentUser != null);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
        }
    }
}
