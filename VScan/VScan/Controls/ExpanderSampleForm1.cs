using System.Drawing;
using System.Windows.Forms;
using VPrinting.Properties;

namespace ExpanderApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            CreateFloatingExpander1();
            CreateFloatingExpander2();
            CreateFloatingExpander3();

            CreateAccordion();
        }

        private void CreateAccordion()
        {
            Accordion accordion = new Accordion();
            accordion.Size = new Size(250, 100);
            accordion.Dock = DockStyle.Left;

            Expander expander1 = new Expander();
            expander1.BorderStyle = BorderStyle.FixedSingle;
            ExpanderHelper.CreateLabelHeader(expander1, "Mail", SystemColors.ActiveBorder);
            CreateContentLabel(expander1, "Test1.\r\nLine2\r\nLine3\r\nLine4",80);
            accordion.Add(expander1);

            Expander expander2 = new Expander();
            expander2.BorderStyle = BorderStyle.FixedSingle;
            ExpanderHelper.CreateLabelHeader(expander2, "Calendar", SystemColors.ActiveBorder);
            CreateContentLabel(expander2, "Test2",120);
            accordion.Add(expander2);

            Expander expander3 = new Expander();
            expander3.BorderStyle = BorderStyle.FixedSingle;
            ExpanderHelper.CreateLabelHeader(expander3, "Contacts", SystemColors.ActiveBorder);
            CreateContentLabel(expander3, "Test3", 60);
            accordion.Add(expander3);

            this.Controls.Add(accordion);
        }

        private void CreateContentLabel(Expander expander, string text, int height)
        {
            Label labelContent = new Label();
            labelContent.Text = text;
            labelContent.Size = new System.Drawing.Size(expander.Width, height);
            expander.Content = labelContent;
        }

        private void CreateFloatingExpander1()
        {
            Expander expander = new Expander();
            expander.Size = new Size(250, 400);
            expander.Left = 350;
            expander.Top = 10;
            expander.BorderStyle = BorderStyle.FixedSingle;

            ExpanderHelper.CreateLabelHeader(expander, "Header", SystemColors.ActiveBorder, Resources.Collapse, Resources.Expand);

            Label labelContent = new Label();
            labelContent.Text = "This is the content part.\r\n\r\nYou can put any Controls here. You can use a Panel, a CustomControl, basically, anything you want.";
            labelContent.Size = new System.Drawing.Size(expander.Width, 80);
            expander.Content = labelContent;

            this.Controls.Add(expander);
        }

        private void CreateFloatingExpander2()
        {
            Expander expander = new Expander();
            expander.Size = new Size(250, 100);
            expander.Left = 350;
            expander.Top = 130;
            expander.BorderStyle = BorderStyle.FixedSingle;

            ExpanderHelper.CreateLabelHeader(expander, "Header", SystemColors.ActiveCaption, null, null, 35, new Font("Arial", 22, FontStyle.Bold));

            Label labelContent = new Label();
            labelContent.Text = "This expander doesn't have image and the font has been changed. Aslo, the header background color is different.";
            labelContent.Size = new System.Drawing.Size(expander.Width, 50);
            expander.Content = labelContent;

            this.Controls.Add(expander);
        }

        private void CreateFloatingExpander3()
        {
            Expander expander = new Expander();
            expander.Size = new Size(250, 120);
            expander.Left = 350;
            expander.Top = 230;
            expander.BorderStyle = BorderStyle.FixedSingle;

            Label headerLabel = new Label();
            headerLabel.Text = "Click me";
            headerLabel.AutoSize = false;
            headerLabel.Font = new Font(headerLabel.Font, FontStyle.Bold);
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            headerLabel.BackColor = SystemColors.ActiveBorder;

            headerLabel.Click += delegate
            {
                expander.Toggle();
                if (expander.Expanded)
                    headerLabel.BackColor = SystemColors.ActiveBorder;
                else
                    headerLabel.BackColor = SystemColors.ActiveCaption;
            };

            expander.Header = headerLabel;

            Label labelContent = new Label();
            labelContent.Text = "You are not limited to use the ExpanderHelper to create your header. Here is a example with a custom code and custom click event handler that change the header backcolor when the expander state change.";
            labelContent.Size = new System.Drawing.Size(expander.Width, 75);
            expander.Content = labelContent;

            this.Controls.Add(expander);
        }
    }
}
