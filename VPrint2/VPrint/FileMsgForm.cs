/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VPrinting.Properties;

namespace VPrinting
{
    public partial class FileMsgForm : Form
    {
        const float HEIGHT = 61.2f;

        private string Caption
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        private string Message
        {
            get
            {
                return txtMessage.Text;
            }
            set
            {
                txtMessage.Text = value;
            }
        }

        private FileInfo Info { get; set; }

        protected float m_IconY = 0;

        private MessageBoxIcon IconType
        {
            set
            {
                switch (value)
                {
                    //2
                    case MessageBoxIcon.Information:
                        m_IconY = 2 * HEIGHT;
                        break;
                    //1
                    case MessageBoxIcon.Exclamation:
                        m_IconY = 1 * HEIGHT;
                        break;
                    //0
                    case MessageBoxIcon.Error:
                        m_IconY = 0;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public FileMsgForm()
        {
            InitializeComponent();
        }

        public static DialogResult show(IWin32Window owner, string caption, string message,
            FileInfo info = null, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            using (var dlg = new FileMsgForm())
            {
                dlg.Caption = caption;
                dlg.Message = message;
                dlg.Info = info;
                dlg.IconType = icon;
                return dlg.ShowDialog(owner);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(Resources.MessageBoxIcons, new Rectangle(12, 12, 63, 63), 0, m_IconY, 63, HEIGHT, GraphicsUnit.Pixel);
            base.OnPaint(e);
        }

        private void Retry_Click(object sender, EventArgs e)
        {
            try
            {
                if (Info != null && Info.Exists())
                {
                    var dest = Path.Combine(Info.Directory.FullName, string.Concat(Info.Name, '1'), Info.Extension);
                    Info.CopyTo(dest);
                }
            }
            catch (Exception ex)
            {
                Program.OnThreadException(this, new ThreadExceptionEventArgs(ex));
            }
        }

        private void Show_Click(object sender, EventArgs e)
        {
            try
            {
                if (Info != null && Info.Exists())
                    Process.Start(Info.FullName);
            }
            catch (Exception ex)
            {
                Program.OnThreadException(this, new ThreadExceptionEventArgs(ex));
            }
        }
    }
}