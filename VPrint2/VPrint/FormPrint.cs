/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using VPrinting.Extentions;
using System.Globalization;

namespace VPrinting
{
    public partial class FormPrint : Form
    {
        private int m_allocationId;
        private int m_voucherCount;
        private int m_rangeFrom;
        private int m_rangeTo;

        public List<int> ReprintVouchers { get; set; }

        public FormPrint(int allocationId)
        {
            InitializeComponent();
            m_allocationId = allocationId;
        }

        protected override void OnLoad(EventArgs e)
        {
            var vocherAllocationPrinting = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
            var va = vocherAllocationPrinting.RetrieveAllocation(m_allocationId);

            m_voucherCount = va.OrderVolume;
            m_rangeFrom = va.RangeFrom;
            m_rangeTo = va.RangeTo;

            base.OnLoad(e);
        }

        private List<int> ParseRange(string value)
        {
            var numberList = new List<int>();

            if (string.IsNullOrWhiteSpace(value))
                return numberList;

            try
            {

                value = value.Replace(" ", "");

                var ranges = value.Split(',', ';');

                foreach (var range in ranges)
                {
                    var numbers = range.Split('-');
                    switch (numbers.Length)
                    {
                        case 1:
                            {
                                int from = int.Parse(numbers[0], CultureInfo.InvariantCulture);
                                numberList.Add(from);
                            }
                            break;
                        case 2:
                            {
                                int from = int.Parse(numbers[0], CultureInfo.InvariantCulture);
                                int to = int.Parse(numbers[1], CultureInfo.InvariantCulture);
                                if (from > to)
                                    throw new Exception();

                                for (int i = from; i <= to; i++)
                                    numberList.Add(i);
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                numberList.Clear();
                Trace.WriteLine(ex);
                this.ShowExclamation("Wrong format. PLease correct.");
            }

            return numberList;
        }

        private void RadioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked)
            {
                textBoxRange.Enabled = false;
                textBoxRange.Text = "";
                buttonPrint.Enabled = true;
            }
        }

        private void ButtonPrint_Click(object sender, EventArgs e)
        {
            ReprintVouchers = ParseRange(textBoxRange.Text);

            if (ReprintVouchers.Count > 0)
            {
                if (this.ShowQuestion("Are you sure you want to reprint " + ReprintVouchers.Count + " voucher(s) ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                    LogRePrintRange();
                    Close();
                }
            }
            else
            {
                if (radioButtonAll.Checked)
                {
                    if (this.ShowQuestion("Are you sure you want to reprint ALL vouchers ? ", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ReprintVouchers = new List<int>();
                        this.DialogResult = DialogResult.OK;
                        LogRePrintRange();
                        Close();
                    }
                }
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RadioButtonVouchers_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonVouchers.Checked)
            {
                textBoxRange.Enabled = true;
                buttonPrint.Enabled = !string.IsNullOrEmpty(textBoxRange.Text);
            }
        }

        private void TextBoxRange_TextChanged(object sender, EventArgs e)
        {
            buttonPrint.Enabled = !string.IsNullOrEmpty(textBoxRange.Text);
        }

        private void LogRePrintRange()
        {
            //Set the wait cursor
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var vocherAllocationPrinting = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
                vocherAllocationPrinting.LogVoucherAllocationReprinted(m_allocationId, textBoxRange.Text, Program.currentUser.UserID, Program.currentUser.CountryID);
            }
            catch (Exception ex)
            {
                ex.ShowDialog(this);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
