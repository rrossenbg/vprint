/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using PremierTaxFree.Extensions;
using PremierTaxFree.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Native;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.PTFLib.Threading;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Web;
using PremierTaxFree.Sys;
using PremierTaxFree.PTFLib.DataServiceProxy;

namespace PremierTaxFree
{
    public partial class MainForm : RibbonForm
    {
        protected override Ribbon Ribbon
        {
            get { return ribbon1; }
        }

        public bool UnLocked
        {
            get { return ribbon1.Enabled; }
            set { ribbon1.Enabled = value; }
        }

        private VoucherForm m_ActiveChild;
        public VoucherForm ActiveChild
        {
            get
            {
                return m_ActiveChild ?? VoucherForm.Empty;
            }
            set
            {
                m_ActiveChild = value;
                SetupTools(value);
            }
        }

        public IEnumerable<VoucherForm> Children
        {
            get
            {
                foreach (VoucherForm form in this.MdiChildren)
                    yield return (VoucherForm)form;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            InitColorList();
            InitTextSizeList();
            InitFontsList();
            InitLineSizeList();

            this.StartPosition = FormStartPosition.WindowsDefaultBounds;

            LockForm.Unlocked += (s, e) => { this.BeginInvokeSf(() => this.Unlock()); };
        }

        #region INITIALIZATION

        private void InitFontsList()
        {
            cbFonts.AllowTextEdit = false;
            cbFonts.DropDownResizable = false;

            var FONTSTYLES = new FontStyle[] { 
                FontStyle.Regular, 
                FontStyle.Italic, 
                FontStyle.Bold, 
                FontStyle.Underline, 
                FontStyle.Strikeout };

            InstalledFontCollection fonts = new InstalledFontCollection();

            using (Bitmap tmpBmp = new Bitmap(100, 100))
            using (Graphics gr = Graphics.FromImage(tmpBmp))
            {
                foreach (FontFamily family in fonts.Families)
                {
                    foreach (FontStyle style in FONTSTYLES)
                    {
                        if (!family.IsStyleAvailable(style))
                            continue;

                        using (Font font = new Font(family, 10, style))
                        {
                            SizeF size = gr.MeasureString(family.Name, font);

                            Bitmap bmp = new Bitmap(Convert.ToInt32(size.Width + 1), Convert.ToInt32(size.Height + 1));

                            using (Graphics gb = Graphics.FromImage(bmp))
                            {
                                gb.DrawString(family.Name, font, Brushes.Black, 0, 0);
                            }

                            RibbonButton btn = new RibbonButton();
                            btn.Tag = new KeyValuePair<FontFamily, FontStyle>(family, style);
                            btn.Click += FontFamily_Click;
                            btn.SmallImage = bmp;
                            btn.MaxSizeMode = RibbonElementSizeMode.Compact;

                            cbFonts.DropDownItems.Add(btn);
                        }
                        break;
                    }
                }
            }
        }

        private void InitColorList()
        {
            List<Color> colorList = Color.Wheat.GetPalette();

            RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();

            this.BackColor = rend.ColorTable.RibbonBackground;

            #region Color Squares

            using (GraphicsPath path = RibbonProfessionalRenderer.RoundRectangle(new Rectangle(2, 2, 12, 12), 3),
                                outer = RibbonProfessionalRenderer.RoundRectangle(new Rectangle(0, 0, 16, 16), 3))
            {
                for (int i = 0; i < colorList.Count; i++)
                {
                    Bitmap bmp = new Bitmap(16, 16);

                    Color color = colorList[i];

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        using (Brush br = new SolidBrush(color))
                            g.FillPath(br, path);

                        using (Pen p = new Pen(Color.White, 2))
                            g.DrawPath(p, path);

                        g.DrawPath(Pens.Wheat, path);
                    }


                    RibbonButton btn = new RibbonButton();
                    btn.SmallImage = bmp;
                    btn.Tag = color;
                    btn.MaxSizeMode = RibbonElementSizeMode.Compact;
                    btn.Click += Colors_Click;
                    lstColors.Buttons.Add(btn);
                }
            }

            #endregion
        }

        private void InitTextSizeList()
        {
            int[] FOND_SIZES = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            cbFondSize.AllowTextEdit = false;
            cbFondSize.DropDownResizable = false;

            foreach (int i in FOND_SIZES)
            {
                RibbonButton btn = new RibbonButton();
                btn.Image = null;
                btn.Tag = i;
                btn.Text = i.ToString();
                btn.Click += SizeButton_Click;
                cbFondSize.DropDownItems.Add(btn);
            }
        }

        private void InitLineSizeList()
        {
            btnLineSize.MinSizeMode = RibbonElementSizeMode.Medium;

            var LINE_SIZES = new int[] { 1, 2, 3, 5, 10, 15, 20 };

            foreach (var size in LINE_SIZES)
            {
                RibbonButton btn = new RibbonButton();
                Bitmap bmp = new Bitmap(32, 32);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRectangle(Brushes.Black, Rectangle.FromLTRB(0, 2, bmp.Width, 2 + size));
                }
                btn.SmallImage = bmp;
                btn.MinSizeMode = RibbonElementSizeMode.Medium;
                btn.Tag = size;
                btn.Click += LineSizeButton_Click;
                btnLineSize.DropDownItems.Add(btn);
            }
        }

        #endregion

        #region SET TOOLS PRIVATE INTERFACE

        private void SetupTools(VoucherForm form)
        {
            var item0 = lstColors.Buttons.FirstOrDefault((i) => ((Color)i.Tag == form.Canvas.BackColor));
            if (item0 != null) ShowBackColor(((RibbonButton)item0).SmallImage);
            //
            var item1 = lstColors.Buttons.FirstOrDefault((i) => ((Color)i.Tag == form.Canvas.ForeColor));
            if (item1 != null) ShowForeColor(((RibbonButton)item1).SmallImage);
            //
            ShowFontSize(m_ActiveChild.Canvas.Font.Size.ToString());
            ShowFontName(m_ActiveChild.Canvas.Font.Name);
            //
            var item2 = btnLineSize.DropDownItems.FirstOrDefault((i) => i.Tag.Cast<int>() == form.Canvas.LineSize);
            if (item2 != null) ShowLineSize(((RibbonButton)item2).SmallImage);
        }

        private void ShowBackColor(Image image)
        {
            btnBackColor.SmallImage = image;
        }

        private void ShowForeColor(Image image)
        {
            btnForeColor.SmallImage = image;
        }

        private void ShowFontSize(string text)
        {
            cbFondSize.TextBoxText = text;
        }

        private void ShowLineSize(Image image)
        {
            btnLineSize.Image = image;
        }

        private void ShowFontName(string name)
        {
            cbFonts.TextBoxText = name;
        }

        #endregion

        #region PRIVATE METHODS

        private void New()
        {
            MainForm form = new MainForm();
            form.Show();
        }

        private void Scan()
        {
            WaitForm.StartAsync(this);
            ScanForm.Start(this.Handle);
        }

        public void Lock()
        {
            LockForm.Start("Use Ctrl-L to unlock the window");
            UnLocked = false;
            Invalidate(true);
        }

        public void Unlock()
        {
            LockForm.Stop();
            UnLocked = true;
            Invalidate(true);
        }

        public VoucherForm AddNewChild()
        {
            VoucherForm form = new VoucherForm();
            form.MdiParent = this;
            form.Show();
            return form;
        }

        #endregion

        #region EVENT HANDLERS

        private void FileButton_Click(object sender, EventArgs e)
        {
            if (sender == btnOpen)
            {
                ActiveChild.Open();
            }
            else if (sender == btnSave)
            {
                ActiveChild.Save();
            }
            else if (sender == btnSaveAs)
            {
                ActiveChild.SaveAs();
            }
            else if (sender == btnPrint)
            {
                ActiveChild.Print();
            }
            else if (sender == btnPrintPreview)
            {
                ActiveChild.PrintPreview();
            }
            else
                throw new NotImplementedException();
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            if (sender == btnScan)
            {
                if (ScanJobForm2.show(this))
                   Scan();
            }
            else if (sender == btnBarCode)
            {
                ActiveChild.ReadBarCodeAsync();
            }
            else
                throw new NotImplementedException();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (ActiveChild != VoucherForm.Empty)
                ActiveChild.Close();
        }

        private void ToolsButton_Click(object sender, EventArgs e)
        {
            if (sender == btnNew)
            {
                AddNewChild();
            }
            else
            {
                if(ActiveChild.Canvas.BackgroundImage == null)
                {
                    new NoImageFoundException().ThrowAndForget();
                    return;
                }

                if (sender == btnMouse)
                {
                    ActiveChild.SetEmptyTool();
                }
                else if (sender == btnSelect)
                {
                    ActiveChild.Lasso();
                }
                else if (sender == btnPen)
                {
                    ActiveChild.Pen();
                }
                else if (sender == btnPolioPen)
                {
                    ActiveChild.PolioPen();
                }
                else if (sender == btnCut)
                {
                    var allowCrop = SettingsTable.Get<bool>(Strings.VScan_AllowCropTool, false);
                    if (allowCrop)
                    {
                        ActiveChild.Cut();
                    }
                    else
                    {
                        new NotAllowedToolException().ThrowAndForget();
                    }
                }
                else if (sender == btnRubber)
                {
                    ActiveChild.Rubber();
                }
                else if (sender == btnRect)
                {
                    //TODO:
                }
                else if (sender == btnEllipse)
                {
                    //TODO:
                }
                else if (sender == btnText)
                {
                    ActiveChild.DoText();
                }
                else
                    throw new NotImplementedException();
            }
        }

        private void Translate_Click(object sender, EventArgs e)
        {
            TranslateForm form = new TranslateForm();
            form.MdiParent = this;
            form.Show();
        }

        private void Rotate_Click(object sender, EventArgs e)
        {
            if (sender == btnRotateLeft)
            {
                ActiveChild.Rotate(true);
            }
            else if (sender == btnRotateRight)
            {
                ActiveChild.Rotate(false);
            }
            else
                throw new NotImplementedException();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnExport)
                {
                    ActiveChild.ExportAndCloseAsync();
                }
                else if (sender == btnExportAll)
                {
                    foreach (var form in Children)
                    {
                        form.ExportAndCloseAsync();
                    }
                }
            }
            //Do not remove the catch scope
            catch (Exception ex)
            {
                ex.ThrowAndForget();
            }
        }

        private void OtherButton_Click(object sender, EventArgs e)
        {
            if (sender == btnOpen2)
            {
                ActiveChild.Open();
            }
            else if (sender == btnSave2)
            {
                ActiveChild.Save();
            }
            else if (sender == btnPrint2)
            {
                ActiveChild.Print();
            }
            else if (sender == btnUndo2)
            {
                new NotSupportedException().ThrowAndForget();
            }
            else
                throw new NotImplementedException();
        }

        private void TRS_Click(object sender, EventArgs e)
        {
            WebForm form = new WebForm();
            form.MdiParent = this;
            form.Show();
        }

        private void Group3_Click(object sender, EventArgs e)
        {
            if (sender == btnNew3)
            {
                New();
            }
            else if (sender == btnOpen3)
            {
                ActiveChild.Open();
            }
            else if (sender == btnSave3)
            {
                ActiveChild.Save();
            }
            else if (sender == btnSaveAs3)
            {
                ActiveChild.SaveAs();
            }
            else if (sender == btnPrepare3)
            {
                new AppInfoException("Prepare not completed yet").ThrowAndForget();
            }
            else if (sender == btnPublish3)
            {
                new AppInfoException("Publish not completed yet").ThrowAndForget();
            }
            else if (sender == btnSend3)
            {
                string to = SettingsTable.Get<string>(Strings.VScan_Email_To, "rosen.rusev@fintrax.com");
                string subject = SettingsTable.Get<string>(Strings.VScan_Email_Subject, "VScan issue");
                string body = SettingsTable.Get<string>(Strings.VScan_Email_Body, "Dear Support");
                Process.Start(new ProcessStartInfo(string.Format( "mailto:{0}?subject={1}&body={2}", to, subject, body)));
            }
            else if (sender == btnClose3)
            {
                Close();
            }
            else if (sender == btnQuickPrint3)//print
            {
                ActiveChild.Print();
            }
            else if (sender == btnAdvancePrint3)// set printer
            {
                ActiveChild.SetupAndPrint();
            }
            else if (sender == btnPrintPreview3)//preview
            {
                ActiveChild.PrintPreview();
            }
            else
                throw new NotImplementedException();
        }

        private void Options_Click(object sender, EventArgs e)
        {
            new AppInfoException("Options not completed yet").ThrowAndForget();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            if (sender == btnHelp || sender == btnAbout)
            {
                //TODO:
                new AppInfoException("Help & About is not completed yet").ThrowAndForget();
            }
            else
                throw new NotImplementedException();
        }

        private void Colors_Click(object sender, EventArgs e)
        {
            if ((sender == btnForeColor) || (sender == btnBackColor))
            {
                //Sets forecolor or backcolor button as active
                btnForeColor.Tag = (sender == btnForeColor);
                btnBackColor.Tag = (sender == btnBackColor);
            }
            else
            {
                Image image = ((RibbonButton)sender).SmallImage;
                Color color = (Color)(((RibbonButton)sender).Tag);

                //Gets the active button
                if (btnBackColor.Tag.Cast<bool>())
                {
                    this.ActiveChild.Canvas.BackColor = color;
                    ShowBackColor(image);
                }
                else
                {
                    this.ActiveChild.Canvas.ForeColor = color;
                    ShowForeColor(image);
                }
            }
        }

        private void SizeButton_Click(object sender, EventArgs e)
        {
            int size = Convert.ToInt32(((RibbonButton)sender).Tag);
            ShowFontSize(size.ToString());
            this.ActiveChild.Canvas.Font = new Font(this.ActiveChild.Canvas.Font.FontFamily, size, this.ActiveChild.Canvas.Font.Style);
        }

        private void LineSizeButton_Click(object sender, EventArgs e)
        {
            RibbonButton button = (RibbonButton)sender;
            ShowLineSize(button.SmallImage);
            this.ActiveChild.Canvas.LineSize = button.Tag.Cast<int>();
        }

        private void FontFamily_Click(object sender, EventArgs e)
        {
            var font = (KeyValuePair<FontFamily, FontStyle>)(((RibbonButton)sender).Tag);
            ShowFontName(font.Key.Name);
            this.ActiveChild.Canvas.Font = new Font(font.Key, this.ActiveChild.Canvas.Font.Size, font.Value);
        }

        private void FontProp_Click(object sender, EventArgs e)
        {
            if (sender == btnBold)
            {
                ActiveChild.SetFontStyle(FontStyle.Bold);
            }
            else if (sender == btnItalic)
            {
                ActiveChild.SetFontStyle(FontStyle.Italic);
            }
            else if (sender == btnUnderLined)
            {
                ActiveChild.SetFontStyle(FontStyle.Underline);
            }
            else if (sender == btnStrikeLined)
            {
                ActiveChild.SetFontStyle(FontStyle.Strikeout);
            }
            else if (sender == btnFondSizeIncrease)
            {
                ShowFontSize(ActiveChild.SetFontSize(true).ToString());
            }
            else if (sender == btnFindSizeDecrease)
            {
                ShowFontSize(ActiveChild.SetFontSize(false).ToString());
            }
            else
                throw new NotImplementedException();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            if (PasswordForm.Authenticate(this, true, false))
                using (SettingsForm form = new SettingsForm())
                    form.ShowDialog(this);
        }

        private void Windows_MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (var btn in btnWindows.DropDownItems)
                btn.Click -= new EventHandler(WindowButton_Click);

            btnWindows.DropDownItems.Clear();

            foreach (var form in ScanAppContext.Default.OpenFormsOf<MainForm>())
            {
                RibbonButton btn = new RibbonButton(form.Text)
                {
                    Image = Resources.window_32,
                    SmallImage = Resources.window16,
                    Tag = form,
                };
                btn.Click += new EventHandler(WindowButton_Click);
                btnWindows.DropDownItems.Add(btn);
            }
        }

        private void WindowButton_Click(object sender, EventArgs e)
        {
            Form frm = (Form)((RibbonButton)sender).Tag;
            Debug.Assert(frm != null);
            frm.Activate();
        }

        private void MdiButton_Click(object sender, EventArgs e)
        {
            if (sender == btnCascade)
                this.LayoutMdi(MdiLayout.Cascade);
            else if (sender == btnTileH)
                this.LayoutMdi(MdiLayout.TileVertical);
            else if (sender == btnTileV)
                this.LayoutMdi(MdiLayout.TileHorizontal);
            else if (sender == btnArrangeIconics)
                this.LayoutMdi(MdiLayout.ArrangeIcons);
            else
                throw new NotImplementedException();
        }

        private void SpyButton_Click(object sender, EventArgs e)
        {
            if (!VoucherMonitorForm.IsStarted)
            {
                VoucherMonitorForm.Start();
            }
            else
            {
                VoucherMonitorForm.Stop();
            }
        }

        private void Debug_Click(object sender, EventArgs e)
        {
            DebugForm form = new DebugForm();
            form.Show();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region PROTECTED OVERRIDES

        private const int POINTS_TO_STEP = 5;

        protected override void OnLoad(EventArgs e)
        {
            //Setting defaults
            cbFonts.DropDownItems.FindAndClick((ri) =>
            {
                return string.CompareOrdinal((((KeyValuePair<FontFamily, FontStyle>)ri.Tag).Key).Name,
                    SettingsTable.Get(Strings.VScan_DefaultFontFamily, "Arial")) == 0;
            });

            btnForeColor.PerformClick();

            lstColors.Buttons.FindAndClick((ri) =>
            {
                return ((Color)ri.Tag) == SettingsTable.Get(Strings.VScan_DefaultForeColor, Color.Black);
            });

            btnBackColor.PerformClick();

            lstColors.Buttons.FindAndClick((ri) =>
            {
                return ((Color)ri.Tag) == SettingsTable.Get(Strings.VScan_DefaultBackColor, Color.Black);
            });

            cbFondSize.DropDownItems.FindAndClick((ri) =>
            {
                return ri.Tag.Cast<int>() == SettingsTable.Get(Strings.VScan_DefaultFontSize, 10);
            });

            btnLineSize.DropDownItems.FindAndClick((ri) =>
            {
                return ri.Tag.Cast<int>() == SettingsTable.Get(Strings.VScan_DefaultLineSize, 3);
            });

            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            if (PasswordForm.ChangePassword(this, false))
            {
                new MethodInvoker(() =>
                {
                    var url = SettingsTable.Get<string>(Strings.All_CentralServerUrl, Strings.All_CentralServerUrlPathDefault);
                    var authUser = SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject);
                    DBConfigValue.Save(Strings.Transferring_AuthObject, authUser);
                    var settingsObj = SettingsTable.Get<SettingsObj>(Strings.Transferring_SettingsObject, SettingsObj.Default);
                    //Send to win service                    
                    DBConfigValue.Save(Strings.Transferring_SettingsObject, settingsObj);

                }).FireAndForget();
            }

            base.OnShown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            CanvasControl canvas = ActiveChild.Canvas;
            if (e.KeyCode == Keys.Escape)
            {
                canvas.UnselectSelectedImages();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                canvas.DeleteSelectedImages();
            }
            else if (e.Control && e.KeyCode == Keys.L)
            {
                if (UnLocked)
                    Lock();
                else
                    Unlock();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                ActiveChild.Copy();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                ActiveChild.Paste();
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                ActiveChild.Cut();
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                //m_UndoRedoManager.Redo();
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                //m_UndoRedoManager.Undo();
            }
            else if (!canvas.m_SelectedImgList.IsEmpty())
            {
                foreach (var obj in canvas.m_SelectedImgList)
                {
                    if (e.KeyCode == Keys.Left)
                    {
                        obj.Move(-POINTS_TO_STEP, 0);
                    }
                    else if (e.KeyCode == Keys.Right)
                    {
                        obj.Move(POINTS_TO_STEP, 0);
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        obj.Move(0, -POINTS_TO_STEP);
                    }
                    else if (e.KeyCode == Keys.Down)
                    {
                        obj.Move(0, POINTS_TO_STEP);
                    }

                    Invalidate(obj.Rect.InflateEx());
                }
            }

            base.OnKeyDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case user32.WM_COPYDATA:
                    {
                        //The scaning Form sends ready status
                        //Get image from appslot
                        IntPtr sender;
                        string text = WinMsg.ReceiveText(m.LParam, out sender);
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (text.StartsWith(Strings.VScan_EditItem))
                            {
                                new MethodInvoker(() =>
                                {
                                    const int MAX_OPENED_FORMS = 5;
                                    int openedForms = ScanAppContext.Default.OpenFormsOf<VoucherForm>().Count();
                                    int settings = SettingsTable.Get<int>(Strings.VScan_MaximumOpenedScanForms, MAX_OPENED_FORMS);
                                    if (openedForms > settings)
                                    {
                                        WinMsg.SendText(sender, Strings.VScan_StopTheScanner);
                                        throw new AppExclamationException("Too many opened windows in editor.\nStop scanning.");
                                    }

                                }).FireAndForget();

                                var data = DataSlot.Get<Voucher>(text);
                                Debug.Assert(data != null && data.VoucherImage != null && !string.IsNullOrEmpty(data.Message),
                                    "MainForm.WndProc");
                                try
                                {
                                    VoucherForm form = AddNewChild();
                                    form.Canvas.Update(data);
                                }
                                finally
                                {
                                    if (VoucherMonitorForm.IsStarted)
                                        VoucherMonitorForm.ShowImage(text);
                                    DataSlot.Free(text);
                                }
                            }
                            else if (text.StartsWith(Strings.VScan_ItemSaved))
                            {
                                string[] msgs = text.Split('|');
                                if (msgs.Length > 1)
                                {
                                    tssLabel1.Text = msgs[0];
                                    tssLabel2.Text = msgs[1];
                                }

                                if (VoucherMonitorForm.IsStarted)
                                    VoucherMonitorForm.ShowImage(text);
                                DataSlot.Free(text);
                            }
                            else if (text.CompareNoCase(Strings.VScan_ScanIsDoneEvent))
                            {
                                WaitForm.Stop(this);
                            }
                            else if (string.Compare(text, Strings.Transferring_RemoteLock) == 0)
                            {
                                Lock();
                            }
                        }
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion
    }
}