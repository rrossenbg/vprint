using System.Windows.Forms;

namespace VCover
{
    public partial class MainForm : Form
    {
        public static MainForm Default { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Default = this;
        }
    }
}
