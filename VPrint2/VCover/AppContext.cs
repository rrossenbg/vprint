using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VCover.Properties;
using VPrinting;
using VPrinting.Communication;
using VPrinting.Forms.Explorer;
using System.IO;
using VCover.Data;

namespace VCover
{
    public class AppContext : ApplicationContext
    {
        public static AppContext Default { get; set; }
        private Form m_form = new Form();
        private readonly MenuItem m_showMenuItem, m_closeMenuItem, m_trainMenuItem, m_lockMenuItem, m_exitMenuItem;
        private readonly NotifyIcon m_notifyIcon = new NotifyIcon();
 
        public event EventHandler<ValueEventArgs<Guid, string>> NewInFileEvent;

        public event ThreadExceptionEventHandler Error;
        private readonly FileSystemWatcher m_trainWatcher = new FileSystemWatcher();

        public AppContext()
        {
            m_showMenuItem = new MenuItem("Login", new EventHandler(ShowHideMainForm_Click));
            m_trainMenuItem = new MenuItem("Train", new EventHandler(TrainMenuItem_Click));
            m_lockMenuItem = new MenuItem("Lock", new EventHandler(LockUnlockMenuItem_Click));
            m_closeMenuItem = new MenuItem("Close", new EventHandler(Close_Click));
            m_exitMenuItem = new MenuItem("Exit", new EventHandler(Exit_Click));

            Bitmap bitmap = new Bitmap(Resources.PTFLogo);
            Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            m_notifyIcon.Icon = icon;
            m_notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { m_showMenuItem, m_lockMenuItem, m_trainMenuItem, m_closeMenuItem, m_exitMenuItem });
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            m_notifyIcon.DoubleClick += new EventHandler(ShowHideMainForm_Click);

            m_trainWatcher.Created += new FileSystemEventHandler(TrainWatcher_Created);

            Default = this;
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        public bool IsLocked
        {
            get
            {
                return m_form.Enabled;
            }
            set
            {
                m_form.Enabled = value;
            }
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

        public void NewImage(Guid id, string filePath)
        {
            if (NewInFileEvent != null)
                NewInFileEvent(this, new ValueEventArgs<Guid, string>(id, filePath));
        }

        public void SaveResult(Guid id, params string[] fileList)
        {
            StringBuilder b = new StringBuilder();
            b.Append(id.ToString());
            b.Append(";");

            foreach (var st in fileList)
            {
                b.Append(st);
                b.Append(";");
            }
            NamedPipes.SendMessage("VPRINT", b.ToString());
        }

        private void TrainMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Explorer();
            form.Show();
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

        private void LockUnlockMenuItem_Click(object sender, EventArgs e)
        {
            if (IsLocked)
            {
                m_form.Enabled = true;
                m_trainWatcher.Path = Config.TRAINImageDirectory;
                m_trainWatcher.EnableRaisingEvents = true;
            }
            else
            {
                m_form.Enabled = false;
                m_trainWatcher.EnableRaisingEvents = false;
            }

            IsLocked = !IsLocked;
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
            m_lockMenuItem.Text = IsLocked ? "Unlock" : "Lock";
            m_trainMenuItem.Enabled = (Program.currentUser != null);
        }

        private void TrainWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (NewInFileEvent != null)
                NewInFileEvent(this, new ValueEventArgs<Guid, string>(Guid.NewGuid(), e.FullPath));
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
        }
    }
}
