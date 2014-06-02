/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.ComponentModel;
using System.Windows.Forms;
using VPrinting.Forms;
using WT = System.Windows.Forms.Timer;

namespace VPrinting
{
    public partial class InputForm : Form, IAsyncFormManagerTarget<InputForm>
    {
        protected readonly WT m_Timer = new WT();

        public AsyncFormManager<InputForm> Target { get; set; }

        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        public static bool show(IWin32Window parent, ref string inputText, string caption = "InputForm", string message = "Text")
        {
            using (var form = new InputForm())
            {
                form.Text = caption;
                form.Message = message;

                if (form.ShowDialog(parent) == DialogResult.OK)
                {
                    inputText = form.txtInputText.Text;
                    return true;
                }
                return false;
            }
        }

        public InputForm()
        {
            InitializeComponent();

            this.components = new Container();

            m_Timer = new WT(this.components);

            m_Timer.Enabled = false;

            if (Target != null)
            {
                m_Timer.Interval = 1000;
                m_Timer.Tick += (_, __) =>
                {
                    this.Focus();
                    m_Timer.Enabled = false;
                };
                m_Timer.Enabled = false;
            }
        }

        public InputForm(AsyncFormManager<InputForm> manager)
        {
            InitializeComponent();
            Target = manager;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Target != null)
                m_Timer.Enabled = true;
            base.OnLoad(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            if (Target != null)
                m_Timer.Enabled = true;
            base.OnDeactivate(e);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (DialogResult == DialogResult.OK && Target != null)
                Target.Result = txtInputText.Text;
            Close();
        }
    }
}
