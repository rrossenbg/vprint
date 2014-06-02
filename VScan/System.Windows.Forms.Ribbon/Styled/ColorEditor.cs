using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Styled
{
	internal class ColorEditorControl : UserControl
	{
		private TrackBar trackBarAlpha;
		private TrackBar trackBarRed;
		private TrackBar trackBarGreen;
		private TrackBar trackBarBlue;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;

		public Color color,old_color;
		private Label labelColor;
		private Panel panelColor;
		private Bitmap bm;

		public ColorEditorControl(Color initial_color)
		{
			this.color  = initial_color; 
            this.old_color =initial_color;    
	        InitializeComponent();
    
		}

		private void InitializeComponent()
		{
			this.trackBarAlpha = new TrackBar();
			this.trackBarRed = new TrackBar();
			this.trackBarGreen = new TrackBar();
			this.trackBarBlue = new TrackBar();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.panelColor = new Panel();
			this.labelColor = new Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).BeginInit();
			this.panelColor.SuspendLayout();
			this.SuspendLayout();
			// 
			// trackBarAlpha
			// 
			this.trackBarAlpha.Location = new System.Drawing.Point(40, 3);
			this.trackBarAlpha.Maximum = 255;
			this.trackBarAlpha.Name = "trackBarAlpha";
			this.trackBarAlpha.Size = new System.Drawing.Size(94, 45);
			this.trackBarAlpha.TabIndex = 0;
			this.trackBarAlpha.TickFrequency = 20;
			this.trackBarAlpha.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// trackBarRed
			// 
			this.trackBarRed.Location = new System.Drawing.Point(40, 33);
			this.trackBarRed.Maximum = 255;
			this.trackBarRed.Name = "trackBarRed";
			this.trackBarRed.Size = new System.Drawing.Size(94, 45);
			this.trackBarRed.TabIndex = 1;
			this.trackBarRed.TickFrequency = 20;
			this.trackBarRed.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// trackBarGreen
			// 
			this.trackBarGreen.Location = new System.Drawing.Point(40, 65);
			this.trackBarGreen.Maximum = 255;
			this.trackBarGreen.Name = "trackBarGreen";
			this.trackBarGreen.Size = new System.Drawing.Size(94, 45);
			this.trackBarGreen.TabIndex = 2;
			this.trackBarGreen.TickFrequency = 20;
			this.trackBarGreen.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// trackBarBlue
			// 
			this.trackBarBlue.Location = new System.Drawing.Point(40, 97);
			this.trackBarBlue.Maximum = 255;
			this.trackBarBlue.Name = "trackBarBlue";
			this.trackBarBlue.Size = new System.Drawing.Size(94, 45);
			this.trackBarBlue.TabIndex = 3;
			this.trackBarBlue.TickFrequency = 20;
			this.trackBarBlue.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 24);
			this.label1.TabIndex = 5;
			this.label1.Text = "Alpha";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 24);
			this.label2.TabIndex = 6;
			this.label2.Text = "Red";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 24);
			this.label3.TabIndex = 7;
			this.label3.Text = "Green";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 24);
			this.label4.TabIndex = 8;
			this.label4.Text = "Blue";
			// 
			// panelColor
			// 
			this.panelColor.BorderStyle = BorderStyle.FixedSingle;
			this.panelColor.Controls.Add(this.labelColor);
			this.panelColor.Location = new System.Drawing.Point(136, 11);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(31, 106);
			this.panelColor.TabIndex = 9;
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(-10, -3);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(56, 160);
			this.labelColor.TabIndex = 10;
			// 
			// ColorEditorControl
			// 
			this.BackColor = System.Drawing.Color.LightGray;
			this.Controls.Add(this.trackBarBlue);
			this.Controls.Add(this.trackBarGreen);
			this.Controls.Add(this.trackBarRed);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.trackBarAlpha);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "ColorEditorControl";
			this.Size = new System.Drawing.Size(171, 135);
			this.KeyPress += new KeyPressEventHandler(this.ColorEditorControl_KeyPress);
			this.Load += new System.EventHandler(this.ColorEditorControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).EndInit();
			this.panelColor.ResumeLayout(false);
			this.ResumeLayout(false);

		}

	
		private void ColorEditorControl_Load(object sender, System.EventArgs e)
		{
		  int argb=this.color.ToArgb();
		  // the colors are store in the argb value as 4 byte : AARRGGBB
		  byte alpha=(byte)(argb>>24);
		  byte red=(byte)(argb>>16);
		  byte green=(byte)(argb>>8);
		  byte blue=(byte)argb;
		  trackBarAlpha.Value =alpha;
		  trackBarRed.Value =red;
		  trackBarGreen.Value =green;
		  trackBarBlue.Value =blue;

          //Foreground Label on the Color Panel
		  labelColor.BackColor =Color.FromArgb(alpha,red,green,blue);

			//Create the Background Image for Color Panel 
			//The Color Panel is to allow the user to check on Alpha transparency
			Bitmap bm=new Bitmap(this.panelColor.Width,this.panelColor.Height);
			this.panelColor.BackgroundImage =bm;
			Graphics g=Graphics.FromImage(this.panelColor.BackgroundImage);
			g.FillRectangle(Brushes.White ,0,0,this.panelColor.Width ,this.panelColor.Height );
			
			//For formating the string
			StringFormat sf=new StringFormat();
			sf.Alignment=StringAlignment.Center;
			sf.LineAlignment=StringAlignment.Center;
			
			//For rotating the string 
			//If you want the text to be rotated uncomment the 5 lines below and comment off the last line

			//Matrix m=new Matrix ();
			//m.Rotate(90);
			//m.Translate(this.panelColor.Width ,0 ,MatrixOrder.Append);
			//g.Transform=m;
			//g.DrawString("TEST",new Font("Arial",16,FontStyle.Bold),Brushes.Black,new Rectangle(0,0,panelColor.Height ,panelColor.Width  ),sf);

			g.DrawString("TEST",new Font("Arial",16,FontStyle.Bold),Brushes.Black,new Rectangle(0,0,panelColor.Width ,panelColor.Height  ),sf);

		}

		private void UpdateColor(int a, int r, int g, int b)
		{
			this.color=Color.FromArgb(a,r,g,b);
			labelColor.BackColor =this.color;
		}

		private void trackBar_ValueChanged(object sender, System.EventArgs e)
		{
		  UpdateColor(trackBarAlpha.Value,trackBarRed.Value,trackBarGreen.Value,trackBarBlue.Value);
		}

		private void ColorEditorControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			//If the user hit the escape key we restore back the old color
			if(e.KeyChar.Equals(Keys.Escape))
			{
               this.color =this.old_color;
			}
		}
	}

	public class ColorEditor : UITypeEditor
	{        
		public ColorEditor()
		{
		}

		// Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
		// drop down dialog, or no UI outside of the properties window.
		[PermissionSet(SecurityAction.Demand, Name="FullTrust")] 
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{            

			if( value.GetType() != typeof(Color))
				return value;

			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if( edSvc != null )
			{
				ColorEditorControl editor = new ColorEditorControl((Color)value);
				edSvc.DropDownControl( editor );

				// Return the value in the appropraite data format.
				if( value.GetType() == typeof(Color) )
					return editor.color;
			}
			return value;
		}

		// Draws a representation of the property's value.
		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]   
		public override void PaintValue(PaintValueEventArgs e)
		{
		    e.Graphics.FillRectangle(new SolidBrush((Color)e.Value),1,1,e.Bounds.Width ,e.Bounds.Height ); 
		}
        
		// Indicates whether the UITypeEditor supports painting a 
		// representation of a property's value.
		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}



}

