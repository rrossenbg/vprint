/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using VPrinting.Extentions;
using VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting
{
    public partial class PrintObjectForm : Form
    {
        public string SelectedText
        {
            get
            {
                return txtText.Text;
            }
            set
            {
                txtText.Text = value;
            }
        }

        public Font SelectedFont
        {
            get
            {
                return txtText.Font;
            }
            set
            {
                txtText.Font = value;
            }
        }

        public string DataSourceColumn
        {
            get
            {
                return ddlDataColumn.Text;
            }
            set
            {
                ddlDataColumn.Text = value;
            }
        }
        
        public bool IsBarCode
        {
            get
            {
                return cbIsBarCode.Checked;
            }
            set
            {
                cbIsBarCode.Checked = value;
            }
        }

        public string Format
        {
            get
            {
                return txtFormat.Text;
            }
            set
            {
                txtFormat.Text = value;
            }
        }

        public PrintObjectForm()
        {
            InitializeComponent();
            fontDialog1.FontMustExist = true;
        }

        public void Bind(string[] values)
        {
            ddlDataColumn.Items.Clear();
            ddlDataColumn.Items.AddRange(values);
        }

        public void DisableTypeChange()
        {
            cbIsBarCode.Enabled = false;
        }

        private void Font_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = txtText.Font;

            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                SelectedFont = fontDialog1.Font;
                txtText.Font = fontDialog1.Font;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            DialogResult = (sender == btnOK) ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        public static IImageObject ShowCreate(Form owner)
        {
            IImageObject result = null;

            using (PrintObjectForm form = new PrintObjectForm())
            {
                Type type = typeof(VoucherAllocation);
                var properties = type.GetProperties();
                var propertiesstrList = properties.Convert<PropertyInfo, string>((i) => i.Name).ToArray();
                form.Bind(propertiesstrList);
                form.Location = Control.MousePosition;

                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    result = (form.IsBarCode) ? (IImageObject)new BarcodeInter2of5() : (IImageObject)new TextObject();
                    result.Text = form.SelectedText;
                    result.Font = form.SelectedFont;
                    result.BoundColumn = form.DataSourceColumn;
                    result.Format = form.Format;

                    using (Graphics g = owner.CreateGraphics())
                        result.Measure(g);

                }
                return result;
            }             
        }

        public static bool ShowEdit(Form owner, IImageObject obj)
        {
            Debug.Assert(obj != null);

            using (PrintObjectForm form = new PrintObjectForm())
            {
                Type type = typeof(VoucherAllocation);
                var properties = type.GetProperties();
                var propertiesstrList = properties.Convert<PropertyInfo, string>((i) => i.Name).ToArray();
                form.Bind(propertiesstrList);
                form.Location = Control.MousePosition;
                form.SelectedFont = obj.Font;
                form.SelectedText = obj.Text;
                form.Format = obj.Format;
                form.IsBarCode = obj is BarcodeInter2of5;
                form.DataSourceColumn = obj.BoundColumn;
                form.DisableTypeChange();

                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    obj.Font = form.SelectedFont;
                    obj.Text = form.SelectedText;
                    obj.Format = form.Format;
                    obj.BoundColumn = form.DataSourceColumn;

                    using (Graphics g = owner.CreateGraphics())
                        obj.Measure(g);

                    return true;
                }
                return false;
            }
        }
    }
}
