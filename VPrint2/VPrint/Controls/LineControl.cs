using System;
using System.Windows.Forms;
using VPrinting.Documents;
using VPrinting.Extentions;

namespace VPrinting.Controls
{
    public partial class LineControl : UserControl
    {
        public PrintLine PrintLine { get; private set; }

        public LineControl()
        {
            InitializeComponent();
        }

        public void Bind(PrintLine line)
        {
            PrintLine = line;
            
            tbX.Minimum = line.MinX;
            tbX.Maximum = line.MaxX;
            tbX.Value = line.X.SetValueInRange((int)line.MinX, (int)line.MaxX);
            
            tbY.Minimum = line.MinY;
            tbY.Maximum = line.MaxY;
            tbY.Value = line.Y.SetValueInRange((int)line.MinY, (int)line.MaxY);

            cbSize.Text = line.Size.ToString();
            txtText.Text = line.Text;

            this.cbSize.SelectedIndexChanged += new System.EventHandler(this.Size_SelectedIndexChanged);
        }

        private void tbX_ValueChanged(object sender, EventArgs e)
        {
            if (tbX.Value.IsValueInRange(PrintLine.MinX, PrintLine.MaxX))
            {
                PrintLine.X = tbX.Value;
                lblX.Text = Convert.ToString(tbX.Value);
            }
        }

        private void tbY_ValueChanged(object sender, EventArgs e)
        {
            if (tbY.Value.IsValueInRange(PrintLine.MinY, PrintLine.MaxY))
            {
                PrintLine.Y = tbY.Value;
                lblY.Text = Convert.ToString(tbY.Value);
            }
        }

        private void Size_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = cbSize.SelectedItem.Cast<int>();
            PrintLine.Size = value;
        }
    }
}
