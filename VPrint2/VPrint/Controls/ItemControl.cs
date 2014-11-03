/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Properties;

namespace VPrinting.Controls
{
    public partial class ItemControl : Control
    {
        private static readonly ConcurrentQueue<ItemControl> ms_ControlStore = new ConcurrentQueue<ItemControl>();

        private bool m_ShowBorder = false;
        protected bool m_ShowThumbnail = false;
        private StateManager.Item m_Item;

        public event EventHandler Updated;

        public StateManager.Item Item
        {
            get
            {
                Debug.Assert(m_Item != null);
                return m_Item;
            }
            set
            {
                Debug.Assert(value != null);
                if (m_Item != null)
                    m_Item.Updated -= new EventHandler(m_Item_Updated);
                m_Item = value;
                m_Item.Updated += new EventHandler(m_Item_Updated);
            }
        }

        public static ItemControl GetInstance()
        {
            ItemControl control;
            if (!ms_ControlStore.TryDequeue(out control))
                control = new ItemControl();
            return control;
        }

        public static void SetInstance(ItemControl control)
        {
            ms_ControlStore.Enqueue(control);
        }

        private void m_Item_Updated(object sender, EventArgs e)
        {
            this.Invalidate();

            if (Updated != null)
                Updated(this, EventArgs.Empty);
        }

        public ItemControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.Cursor = Cursors.Hand;
        }

        public void PlayEffect()
        {
            m_ShowBorder = true;
            this.Invalidate();
            m_ActionTimer.Start();
        }

        public void Lock()
        {
            this.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            this.Invalidate();
        }

        public void UnLock()
        {
            this.Enabled = true;
            this.Cursor = Cursors.Hand;
            this.Invalidate();
        }

        public void PerformClick()
        {
            if (base.CanSelect && this.Enabled)
                this.OnClick(EventArgs.Empty);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            //Revert view if item is completed
            if (m_Item.State != StateManager.eState.NA)
                m_ShowThumbnail = !m_ShowThumbnail;
            this.Invalidate();
            base.OnMouseHover(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_Item != null)
            {
                e.Graphics.Clear(m_Item.State != StateManager.eState.Err ? ((m_Item.Selected) ? Color.GreenYellow : Color.White) : Color.Tomato);

                #region STATUSES

                var img = GetImageFromStatus();

                if (img.Width < 100)
                {
                    e.Graphics.DrawImage(img, new PointF((Width - img.Width - 5) / 2, (Height - img.Height - 5) / 2 + 3));
                }
                else
                {
                    var r = this.ClientRectangle;
                    r.Inflate(5, 5);
                    e.Graphics.DrawImage(img, r);
                }

                if (m_Item.Forsed)
                    e.Graphics.DrawImage(Resources.lightning, 40, 40, 24, 24);
                else if (m_Item.Ignored)
                    e.Graphics.DrawImage(Resources.no, 40, 40, 24, 24);

                #endregion

                var vi = m_Item as StateManager.VoucherItem;

                using (var font = new Font("Microsoft Sans Serif", 7f, FontStyle.Bold))
                {
                    if (vi != null)
                    {
                        //if voucher
                        if (vi.RetailerID != 0)
                            e.Graphics.DrawString(vi.RetailerID.ToString(), font, Brushes.Black, new Point(2, 0));

                        if (vi.VoucherID != 0)
                            e.Graphics.DrawString(vi.VoucherID.ToString(), font, Brushes.Black, new Point(2, 10));

                        if (!string.IsNullOrWhiteSpace(vi.SiteCode))
                            e.Graphics.DrawString(vi.SiteCode, font, Brushes.Black, new Point(2, 30));

                        if (!string.IsNullOrWhiteSpace(vi.Name))
                            e.Graphics.DrawString(vi.Name, font, Brushes.DarkBlue, new Point(2, 40));

                        if (vi.IsSignatureValid.HasValue)
                        {
                            if (vi.IsSignatureValid.Value)
                                e.Graphics.DrawImageUnscaled(Resources.application_certificate__1_, new Point(10, 35));
                            else
                                e.Graphics.DrawImageUnscaled(Resources.no, new Point(10, 35));
                        }
                    }
                    else
                    {
                        //if cover sheet
                        if (!string.IsNullOrWhiteSpace(m_Item.Name))
                            e.Graphics.DrawString(m_Item.Name, font, Brushes.DarkBlue, new Point(2, 40));

                        if (m_Item.JobID != 0)
                            e.Graphics.DrawString(m_Item.JobID.ToString(), font, Brushes.Black, new Point(2, 0));
                    }
                }

                #region BORDERS

                if (m_ShowBorder || m_Item.Selected)
                {
                    var rec = this.ClientRectangle;
                    rec.Inflate(-4, -4);
                    using (var p = new Pen(Color.Black, 2.3f))
                        e.Graphics.DrawRectangle(p, rec);
                }
                else
                {
                    var rec = this.ClientRectangle;
                    rec.Inflate(-1, -1);
                    e.Graphics.DrawRectangle(Pens.Gray, rec);
                }

                #endregion
            }
            else
            {
                e.Graphics.Clear(Color.Black);
            }
            base.OnPaint(e);
        }

        protected Image GetImageFromStatus()
        {
            if (m_Item.Thumbnail == null)
            {
                switch (m_Item.State)
                {
                    case StateManager.eState.Err:
                        return Resources.Err;

                    case StateManager.eState.Sent:
                        return Resources.Sent;

                    case StateManager.eState.OK:
                        return Resources.OK;

                    case StateManager.eState.WAIT:
                        return Resources.Wait;

                    case StateManager.eState.VOUCHER:
                        return Resources.document;

                    case StateManager.eState.COVER:
                        return Resources.spreadsheet;

                    default:
                    case StateManager.eState.NA:
                        return Resources.NA;
                }
            }
            else
            {
                if (m_ShowThumbnail)
                    return m_Item.Thumbnail;

                switch (m_Item.State)
                {
                    case StateManager.eState.Err:
                        return Resources.Err;

                    case StateManager.eState.OK:
                        return m_Item.Thumbnail;

                    case StateManager.eState.WAIT:
                        return Resources.Wait;

                    case StateManager.eState.Sent:
                        return Resources.Sent;

                    case StateManager.eState.VOUCHER:
                        return Resources.document;

                    case StateManager.eState.COVER:
                        return Resources.spreadsheet;

                    default:
                    case StateManager.eState.NA:
                        return Resources.NA;
                }
            }
        }

        private void ActionTimer_Tick(object sender, EventArgs e)
        {
            m_ShowBorder = !m_ShowBorder;
            this.Invalidate();
            m_ActionTimer.Stop();
        }
    }
}
