/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VPrinting.Common;

namespace VPrinting.Controls
{
    public partial class SchedulerForm : Form
    {
        public SchedulerForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            cbActive.Checked = StateSaver.Default.Get<bool>(Strings.RUN_SCHEDULETIME, false);
            dateTimePicker1.Value = DateTime.Now.At(StateSaver.Default.Get<TimeSpan>(Strings.SCHEDULETIME, DateTime.Now.TimeOfDay));
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (cbActive.Checked)
                {
                    StateSaver.Default.Set(Strings.RUN_SCHEDULETIME, true);
                    StateSaver.Default.Set(Strings.SCHEDULETIME, dateTimePicker1.Value.TimeOfDay);
                }
                else
                {
                    StateSaver.Default.Set(Strings.RUN_SCHEDULETIME, false);
                    StateSaver.Default.Remove(Strings.SCHEDULETIME);
                }
            }
            base.OnClosing(e);
        }

        private void cbActive_CheckedChanged(object sender, EventArgs e)
        {
            cbActive.BackColor = cbActive.Checked ? Color.Green : SystemColors.Control;
        }
    }
}
