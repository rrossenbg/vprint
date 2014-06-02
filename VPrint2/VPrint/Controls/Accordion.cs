using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExpanderApp
{
    public partial class Accordion : Panel
    {
        public Accordion()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        public void Add(Expander expander)
        {
            if (this.Controls.Count > 0)
                expander.Collapse();

            expander.Width = this.Width - this.Margin.Horizontal - expander.Margin.Horizontal;
            this.Controls.Add(expander);

            expander.StateChanging += new CancelEventHandler(expander_StateChanging);
            expander.StateChanged += new EventHandler(expander_StateChanged);

            ArrangeLayout();
        }

        void expander_StateChanging(object sender, CancelEventArgs e)
        {
            if (this.processing)
                return;

            Expander expander = sender as Expander;
            if (expander.Expanded)
                e.Cancel = true;
        }

        void expander_StateChanged(object sender, EventArgs e)
        {
            if (this.processing)
                return;

            processing = true;
            Expander expander = sender as Expander;
            foreach (Expander ex in Controls)
            {
                if (ex == expander)
                    continue;

                if (ex.Expanded)
                    ex.Collapse();
            }

            ArrangeLayout();

            processing = false;
        }

        private void ArrangeLayout()
        {
            int h = 0;
            foreach (Expander ex in this.Controls)
                h += ex.Expanded ? 0 : ex.Header.Height;
            int remainingHeight = this.Height - this.Padding.Vertical - h;

            int y = this.Padding.Top;
            foreach (Expander ex in this.Controls)
            {
                ex.Width = this.Width;
                ex.Top = y;
                if (ex.Expanded)
                    ex.Height = remainingHeight;

                y += ex.Height;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ArrangeLayout();
        }

        private bool processing = false;
    }
}
