/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;

using PremierTaxFree.PTFLib.Native;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Security;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.PTFLib.Web;

namespace PremierTaxFree
{
    public partial class LockForm : Form
    {
        private static Thread ms_Thread = null;
        private static IntPtr ms_This = IntPtr.Zero;

        protected static Image ms_Image;
        protected static Image LockImage
        {
            get
            {
                lock (ms_Image)
                {
                    return ms_Image;
                }
            }
        }

        public static EventHandler Unlocked;

        public string Message { get; set; }

        static LockForm()
        {
            ms_Image = Resources.locked.SetSize(300, 300);
        }

        public static bool Locked
        {
            get
            {
                return ms_Thread != null;
            }
        }

        private LockForm()
        {
            InitializeComponent();
            this.DesktopLocation = Point.Empty;
            this.Size = SystemInformation.VirtualScreen.Size;
        }

        public static void Start(string message)
        {
            //Start once
            if (ms_Thread != null)
                return;

            ms_Thread = new Thread(() =>
            {
                LockForm form = new LockForm();
                form.Message = message;
                ms_This = form.Handle;
                Application.Run(form);
            }) { IsBackground = true };
            ms_Thread.Start();
        }

        public static void Stop()
        {
            if (ms_This != IntPtr.Zero)
                WinMsg.SendText(ms_This, Strings.VScan_DesktopUnlock);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.L)
                Unlock();
            base.OnKeyDown(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = (DialogResult != DialogResult.OK);
            base.OnClosing(e);
        }

        private Brush m_MessageBrush;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            e.Graphics.FillRectangleHatch(this.DisplayRectangle, Color.Black, Color.White);

            e.Graphics.DrawImage(LockImage, this.Size.Center(LockImage.Size));

            if (!string.IsNullOrEmpty(Message))
            {
                Size s = e.Graphics.MeasureString(Message, this.Font).ToSize();
                var point = this.ClientSize.Center(s);
                e.Graphics.DrawString(Message, this.Font, m_MessageBrush = (m_MessageBrush != Brushes.Red ? Brushes.Red : Brushes.Yellow), point);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case user32.WM_COPYDATA:
                    {
                        string text = WinMsg.ReceiveText(m.LParam);
                        if (text.CompareNoCase(Strings.VScan_DesktopUnlock))
                        {
                            DialogResult = DialogResult.OK;
                            if (Unlocked != null)
                                Unlocked(this, EventArgs.Empty);
                            ms_Thread = null;
                            ms_This = IntPtr.Zero;
                            Application.ExitThread();
                        }
                        else
                        {
                            Message = text;
                            Invalidate();
                        }
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void Unlock()
        {
            if (PasswordForm.Authenticate(this, false, false))
            {
                new MethodInvoker(() =>
                {
                    var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
                    var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
                    var countryId = SettingsTable.Get<int>(Strings.VScan_DefaultCountryCode, 826);
                    //authUser.PassHash = ReceivingWebService.ValidateUser(authUser, url);
#warning TODO
                    SettingsTable.Set(Strings.Transferring_AuthObject, authUser);
                    DBConfigValue.Save(Strings.Transferring_AuthObject, authUser);
                    Stop();
                }).FireAndForget();
            }
        }
    }
}
