using System;
using System.Windows.Forms;
using System.Drawing;

namespace VPrinting.Controls
{
    public partial class ToggleButtonControl : UserControl
    {
        public Color ColorOn { get; set; }
        public Color ColorOff { get; set; }

        private readonly CheckBox[] m_Checks;

        public event EventHandler<ValueEventArgs<int>> CheckedChanged;

        public event EventHandler CheckedChanged1
        {
            add
            {
                this.checkBox1.CheckedChanged += value;
            }
            remove
            {
                this.checkBox1.CheckedChanged -= value;
            }
        }

        public event EventHandler CheckedChanged2
        {
            add
            {
                this.checkBox2.CheckedChanged += value;
            }
            remove
            {
                this.checkBox2.CheckedChanged -= value;
            }
        }

        public event EventHandler CheckedChanged3
        {
            add
            {
                this.checkBox3.CheckedChanged += value;
            }
            remove
            {
                this.checkBox3.CheckedChanged -= value;
            }
        }

        public string[] Text1 { get; set; }

        public string[] Text2 { get; set; }

        public string[] Text3 { get; set; }

        public ToggleButtonControl()
        {
            InitializeComponent();
            Text1 = new string[2];
            Text2 = new string[2];
            Text3 = new string[2];
            ColorOn = ColorOff = Color.LightGray;
            m_Checks = new CheckBox[] { checkBox1, checkBox2, checkBox3 };
        }

        public void PerformClick(int index)
        {
            if (index < 0 || index >= m_Checks.Length)
                throw new IndexOutOfRangeException("index should in [0, 3]");

            m_Checks[index].Checked = true;

            checkBox_Click(m_Checks[index], EventArgs.Empty);
        }

        public int GetClicked()
        {
            for (int i = 0; i < m_Checks.Length; i++)
                if (m_Checks[i].Checked)
                    return i;
            return -1;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            RefreshControl();
            base.OnParentChanged(e);
        }

        public void RefreshControl()
        {
            foreach (var cb in m_Checks)
            {
                cb.BackColor = ColorOff;
                ResetText(cb);
            }
        }

        private void checkBox_Click(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            foreach (var ch in m_Checks)
            {
                if (ch != sender)
                {
                    ch.Checked = !cb.Checked;
                    if (!cb.Checked)
                        break;
                }
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            cb.BackColor = cb.Checked ? ColorOn : ColorOff;
            ResetText(cb);
            FireCheckedChanged(cb);
        }

        private void FireCheckedChanged(CheckBox cb)
        {
            if (CheckedChanged != null)
            {
                int index = -1;

                if (cb == checkBox1)
                    index = 0;
                else if (cb == checkBox2)
                    index = 1;
                else if (cb == checkBox3)
                    index = 2;

                CheckedChanged(this, new ValueEventArgs<int>(index));
            }
        }

        private void ResetText(CheckBox cb)
        {
            if (cb == checkBox1 && Text1 != null)
                checkBox1.Text = Text1[Convert.ToInt32(cb.Checked)];
            else if (cb == checkBox2 && Text2 != null)
                checkBox2.Text = Text2[Convert.ToInt32(cb.Checked)];
            else if (cb == checkBox3 && Text3 != null)
                checkBox3.Text = Text3[Convert.ToInt32(cb.Checked)];
        }
    }
}
