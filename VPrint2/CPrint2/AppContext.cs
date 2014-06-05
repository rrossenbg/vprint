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

        private readonly MainForm m_form = new MainForm();
        private readonly MenuItem m_showMenuItem, m_closeMenuItem, m_startMenuItem,  m_exitMenuItem;
        private readonly NotifyIcon m_notifyIcon = new NotifyIcon();
        private readonly FileSystemWatcher m_commandWatcher = new FileSystemWatcher();
        private readonly FileSystemWatcher m_imageWatcher = new FileSystemWatcher();

        public event EventHandler<ValueEventArgs<string>> NewCommandFileEvent;
        public event EventHandler<ValueEventArgs<string>> NewImageFileEvent;
        public event ThreadExceptionEventHandler Error;

        public AppContext()
        {
            m_showMenuItem = new MenuItem("Login", new EventHandler(ShowHideMainForm_Click));
            m_startMenuItem = new MenuItem("Start", new EventHandler(StartStopMenuItem_Click));
            m_closeMenuItem = new MenuItem("Close", new EventHandler(Close_Click));
            m_exitMenuItem = new MenuItem("Exit", new EventHandler(Exit_Click));
            
            m_notifyIcon.Icon = Resources.camera_unmount2;
            m_notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { m_showMenuItem, m_startMenuItem, m_closeMenuItem, m_exitMenuItem });
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            m_notifyIcon.DoubleClick += new EventHandler(ShowHideMainForm_Click);

            m_commandWatcher.Path = Config.CommandInputPath;
            m_commandWatcher.Filter = Config.CommandFilter;
            m_commandWatcher.Created += new FileSystemEventHandler(CommandWatcher_Created);

            m_imageWatcher.Path = Config.ImageOutputPath;
            m_imageWatcher.Filter = Config.ImageFileFilter; ;
            m_imageWatcher.Created += new FileSystemEventHandler(ImageWatcher_Created);

            Default = this;
        }

        public bool IsStarted
        {
            get
            {
                return m_commandWatcher.EnableRaisingEvents;
            }
        }

        public void Start()
        {
            m_commandWatcher.EnableRaisingEvents = true;
            m_imageWatcher.EnableRaisingEvents = true;

            PresenterCameraShooter shooter = new PresenterCameraShooter();
            shooter.TryStartPresenter(Config.PresenterPath);
        }

        public void Stop()
        {
            m_commandWatcher.EnableRaisingEvents = false;
            m_imageWatcher.EnableRaisingEvents = false;

            PresenterCameraShooter shooter = new PresenterCameraShooter();
            shooter.TryStopPresenter();

            ImageProcessor.Clear();
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
        }

        private void CommandWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (NewCommandFileEvent != null)
                NewCommandFileEvent(this, new ValueEventArgs<string>(e.FullPath));
        }

        private void ImageWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (NewImageFileEvent != null)
                NewImageFileEvent(this, new ValueEventArgs<string>(e.FullPath));
        }
    }
}
