/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DTKBarReader;
using PremierTaxFree.Controls;
using PremierTaxFree.Data.Objects;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.Tools;
using PremierTaxFree.Utils;

namespace PremierTaxFree
{
    public partial class VoucherForm : Form
    {
        public static readonly VoucherForm Empty = new VoucherForm();

        public VoucherForm()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        public void SetEmptyTool()
        {
            BaseTool.Reset(Canvas);
        }

        public void Open()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = false;
                dlg.Filter = Strings.VScan_ImageFilter;
                dlg.CheckFileExists = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        Image img = Image.FromFile(dlg.FileName);
                        Canvas.BackgroundImage.DisposeSf();
                        Canvas.BackgroundImage = img;
                        Canvas.FileName = Path.GetFileName(dlg.FileName);
                        Canvas.Invalidate();
                        m_FileName = dlg.FileName;
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private string m_FileName;

        public void Save()
        {
            if (Canvas.BackgroundImage == null)
            {
                 new AppExclamationException("No Image found").ThrowAndForget();
                 return;
            }

            if (string.IsNullOrEmpty(m_FileName))
            {
                SaveAs();
            }
            else
            {
                Canvas.BackgroundImage.Save(m_FileName);
            }
        }

        public void SaveAs()
        {
            if (Canvas.BackgroundImage == null)
            {
                new AppExclamationException("No Image found").ThrowAndForget();
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.FileName = m_FileName;
                dlg.Filter = Strings.VScan_ImageFilter;
                dlg.FilterIndex = 1;
                dlg.CheckPathExists = true;
                dlg.AddExtension = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        Canvas.BackgroundImage.Save(dlg.FileName);
                        m_FileName = dlg.FileName;
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        public void Print()
        {
            WaitForm.StartAsync(this);
            printComponent.Print(this);
            WaitForm.Stop(this);
        }

        public void PrintPreview()
        {
            WaitForm.StartAsync(this);
            printComponent.PrintPreview(this);
            WaitForm.Stop(this);
        }

        public void SetupAndPrint()
        {
            WaitForm.StartAsync(this);
            printComponent.Setup();
            printComponent.Print(this);
            WaitForm.Stop(this);
        }

        public void Cut()
        {
            IDataObject data = new DataObject();
            data.SetData(Canvas.m_SelectedImgList.GetType().FullName, Canvas.m_SelectedImgList);
            Clipboard.SetDataObject(data, true);

            foreach (var obj in Canvas.m_SelectedImgList)
            {
                Canvas.m_ImgList.Remove(obj);
                Canvas.Invalidate(obj.Rect.InflateEx());
            }
            Canvas.m_SelectedImgList.Clear();
        }

        public void Copy()
        {
            IDataObject data = new DataObject();
            data.SetData(Canvas.m_SelectedImgList.GetType().FullName, Canvas.m_SelectedImgList);
            Clipboard.SetDataObject(data, true);

            foreach (var obj in Canvas.m_SelectedImgList)
                Canvas.Invalidate(obj.Rect.InflateEx());
            Canvas.m_SelectedImgList.Clear();
        }

        public void Paste()
        {
            if (Clipboard.ContainsData(Canvas.m_SelectedImgList.GetType().FullName))
            {
                HashSet<ImageObj> hashSet = (HashSet<ImageObj>)
                    Clipboard.GetData(Canvas.m_SelectedImgList.GetType().FullName);

                foreach (var obj in hashSet)
                {
                    obj.MoveTo(0, 0);
                    Canvas.m_ImgList.Add(obj);
                    Canvas.Invalidate(obj.Rect.InflateEx());
                }
                Canvas.m_SelectedImgList.Clear();
            }
        }

        public void ReadBarCodeAsync()
        {
            new MethodInvoker(() =>
            {
                if (Canvas.BackgroundImage == null)
                    throw new AppWarningException("No image found");

                Bitmap bmp = new Bitmap(Canvas.BackgroundImage);

                lock (typeof(BarcodeReader))
                {
                    BarcodeReader reader = new BarcodeReader(Strings.VScan_BarcodeReaderSDKDeveloperLicenseKey);
                    reader.LicenseManager.AddLicenseKey(Strings.VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey);
                    reader.BarcodesToRead = 1;
                    reader.BarcodeTypes = BarcodeTypeEnum.BT_Inter2of5;

                    Bitmap barcode = null;
                    Barcode[] readBarCodes = reader.ReadFromBitmapRotateAll(ref bmp, Canvas.Data);
                    if (readBarCodes == null || readBarCodes.Length == 0)
                    {
                        var ex = new AppExclamationException("No barcode found");
                        ex.AddNext(new MethodInvoker(() =>
                        {
                            Canvas.InvokeSf(() =>
                            {
                                Canvas.ShowInputControl("No barcode found.");
                            });
                        }));
                        throw ex;
                    }
                    else if (readBarCodes.Length > 1)
                    {
                        var ex = new AppExclamationException("More than one barcode on image.");
                        ex.AddNext(new MethodInvoker(() =>
                        {
                            Canvas.InvokeSf(() =>
                            {
                                Canvas.ShowInputControl("More than one barcode on image.");
                            });
                        }));
                        throw ex;
                    }
                    Canvas.InvokeSf(() =>
                    {
                        Canvas.Data.Parse(readBarCodes[0].BarcodeString);
                        Canvas.Data.BarCodeImage = barcode;
                    });
                }

                bmp.DisposeSf();
            }).FireAndForget();
        }

        public void ExportAndCloseAsync()
        {
            if (this.Parent != null)
            {
                IntPtr main = this.Parent.Handle;

                Voucher bData = Canvas.Data;
                bData.VoucherImage = (Bitmap)Canvas.BackgroundImage;

                var cnts = Canvas.Controls.Find(ManualInsertDataControl.MANUALINSERT_DATACONTROL_NAME, true);
                if (cnts != null && cnts.Length != 0)
                {
                    ManualInsertDataControl cnt = (ManualInsertDataControl)cnts[0];
                    if (!string.IsNullOrEmpty(cnt.BarCodeString))
                    {
                        bData.Parse(cnt.BarCodeString);
                    }
                    else
                    {
                        string[] str = cnt.BarCodeNumberGroups;
                        bData.CountryID = int.Parse(str[0]);
                        //bData.Business = int.Parse(str[1]);
                        bData.RetailerID = int.Parse(str[2]);
                        bData.VoucherID = str[3];
                        bData.BarCodeString = string.Format("{0:000}{1:00}{2:000000}{3:000000000}",
                            bData.CountryID, "na", bData.RetailerID, bData.VoucherID);
                    }
                }

                new Action<Voucher>((data) =>
                {
                    Debug.Assert(data != null);
                    Debug.Assert(data.VoucherImage != null);

                    lock (data.VoucherImage)
                    {
                        try
                        {
                            DelegateUtils.CreateAuditIdReloadDelegate().FireAndForget();

                            data.SiteCode = AuditIDSTable.SelectRemoveFirstOrEmpty().ThrowIfDefault<string, AppExclamationException>();

                            data.Validate();

                            ClientDataAccess.UpdateFileAsync((DbClientVoucher)data,
                                new EventHandler((o, s) =>
                                {
                                    string fileName = Path.ChangeExtension(
                                        string.Format("{0}{1}", PTFUtils.GetMachine(),
                                        DateTime.Now).ReplaceAll(new char[] { '.', '/', '\\', '-' }, '_'), ".xml");

                                    //Data has been saved successfully.
                                    string text = string.Concat(Strings.VScan_ItemSaved, fileName);
                                    WinMsg.SendText(main, text);
                                    this.InvokeSf(() =>
                                    {
                                        //Set DialogResult OK
                                        //Or else the form will resign to close
                                        this.DialogResult = DialogResult.OK;
                                        this.Close();
                                    });
                                }),
                                //No error handling. 
                                //General processing handler.
                                null);
                        }
                        catch (Exception ee)
                        {
                            ee.AddNext(new MethodInvoker(() =>
                            {
                                Canvas.InvokeSf(() =>
                                {
                                    Canvas.ShowInputControl(ee.Message);
                                });
                            }));
                            throw;
                        }
                    }
                }).FireAndForget(bData);
            }
        }

        public void Lasso()
        {
            BaseTool.Install(new LassoTool(Canvas));
        }

        public void Pen()
        {
            BaseTool.Install(new PenTool(Canvas));
        }

        public void PolioPen()
        {
            BaseTool.Install(new PolyPenTool(Canvas));
        }

        public void Rubber()
        {
            BaseTool.Install(new EraserTool(Canvas));
        }

        public void DoText()
        {
            BaseTool.Install(new TextTool(Canvas));
        }

        public void Rotate(bool left)
        {
            if (Canvas.BackgroundImage == null)
            {
                new AppExclamationException("No Image found").ThrowAndForget();
                return;
            }

            if (left)
            {
                Canvas.BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else
            {
                Canvas.BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            Canvas.Invalidate();
        }

        public void SetFontStyle(FontStyle style)
        {
            Font font = this.Canvas.Font;
            try
            {
                this.Canvas.Font = new Font(
                    this.Canvas.Font.FontFamily,
                    this.Canvas.Font.Size,
                    style);
            }
            catch (Exception ex)
            {
                this.Canvas.Font = font;
                ex.ThrowAndForget();
            }
        }

        public float SetFontSize(bool setUpper)
        {
            Font font = this.Canvas.Font;
            try
            {
                if (setUpper)
                {
                    this.Canvas.Font = new Font(
                            this.Canvas.Font.FontFamily,
                            this.Canvas.Font.Size + 1,
                            this.Canvas.Font.Style);
                }
                else
                {
                    this.Canvas.Font = new Font(
                            this.Canvas.Font.FontFamily,
                            this.Canvas.Font.Size - 1,
                            this.Canvas.Font.Style);
                }
                return this.Canvas.Font.Size;
            }
            catch (Exception ex)
            {
                this.Canvas.Font = font;
                ex.ThrowAndForget();
                return 0;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            BaseTool.Reset(Canvas);
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                e.Cancel = MessageBox.Show(this, "Voucher is not saved yet.\nAre you sure want to close?",
                    Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
            }

            base.OnFormClosing(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            var form = (this.MdiParent as MainForm);
            if (form != null)
            {
                form.ActiveChild = this;
            }
            base.OnActivated(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
