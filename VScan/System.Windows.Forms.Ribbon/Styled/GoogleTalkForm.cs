using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms.Styled
{
	[ToolboxItem(false), 
    Designer(typeof(GoogleTalkFormDesigner), typeof(IRootDesigner)),
    DesignerCategory("Form"),
    DefaultEvent("Load"), 
    DesignTimeVisible(false)]
    public class GoogleTalkForm : Form
	{	
		#region Class Variables

		private const int edgeRadius = 10;
		private const int padding = 3;
		private const int snapPixels = 5;
		private Color transparentColor = Color.Pink;

		private bool mouseDown = false;
		private bool windowMoving    = false;
		private bool windowResizing  = false;
		private bool windowMaximized = true;
		private bool onCloseIcon = false;
		private bool onMinimizeIcon = false;
		private int windowHeight;

		private Form hiddenForm = null;
		private Bitmap bmpIcon = null;
		private Point mousePosition;
		private Rectangle rectTitleArea;
		private Rectangle rectBody;
		private Rectangle rectCloseIcon;
		private Rectangle rectMinimizeIcon;
		private Rectangle rectResizableArea;

		private bool isDisposing = false;

		/// <summary>Required designer variable.</summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Class Constructor

		public GoogleTalkForm()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.StandardClick, true);
			this.SetStyle(ControlStyles.StandardDoubleClick, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			this.UpdateStyles();

			hiddenForm = new Form();
			hiddenForm.Width = 1;
			hiddenForm.Height = 1;
			hiddenForm.StartPosition = FormStartPosition.Manual;
			hiddenForm.Top = 0;
			hiddenForm.Left = 0;
			hiddenForm.BackColor = transparentColor;
			hiddenForm.TransparencyKey = transparentColor;
			hiddenForm.FormBorderStyle = FormBorderStyle.None;
			hiddenForm.ShowInTaskbar = false;
		}

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// GoogleTalkForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 272);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "GoogleTalkForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Disposed +=new EventHandler(OnDisposed);

		}

		#endregion

		#region Class Properties

		#region IsWindowSnappable

		private bool isWindowSnappable = true;

		public bool IsWindowSnappable
		{
			get
			{
				return this.isWindowSnappable;
			}

			set
			{
				this.isWindowSnappable = value;
			}
		}

		#endregion

		#region IsResizable

		private bool isResizable = true;

		public bool IsResizable
		{
			get
			{
				return this.isResizable;
			}

			set
			{
				if (this.isResizable != value)
					this.Invalidate();

				this.isResizable = value;
			}
		}

		#endregion

		#region ResizableColor

		private Color resizableColor = Color.LightGray;

		public Color ResizableColor
		{
			get
			{	
				return this.resizableColor;
			}
			set
			{
				this.resizableColor = value;
			}
		}

		#endregion

		#region TitleColor

		private Color titleColor = Color.FromArgb(16, 48, 107);

		public Color TitleColor
		{
			get
			{	
				return this.titleColor;
			}
			set
			{
				this.titleColor = value;
			}
		}

		#endregion

		#region TitleFont

		private Font titleFont = new Font("Tahoma", 10F, FontStyle.Bold);

		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}

			set
			{
				this.titleFont = value;
			}
		}

		#endregion

		#region TitleBackColor

		private Color titleBackColor = Color.FromArgb(206, 211, 222);

		public Color TitleBackColor
		{
			get
			{
				return this.titleBackColor;
			}
			set
			{
				this.titleBackColor = value;
			}
		}

		#endregion

		#region TitleForeColor

		private Color titleForeColor = Color.White;

		public Color TitleForeColor
		{
			get
			{
				return this.titleForeColor;
			}
			set
			{
				this.titleForeColor = value;
			}
		}

		#endregion

		#region TitleStyle

		private ColorStyle titleStyle = ColorStyle.VerticalGradient;

		public ColorStyle TitleStyle
		{
			get
			{
				return this.titleStyle;
			}
			set
			{
				this.titleStyle = value;
			}
		}

		#endregion

		#region BodyBackColor

		private Color bodyBackColor = Color.FromArgb(239, 239, 239);

		public Color BodyBackColor
		{
			get
			{
				return this.bodyBackColor;
			}
			set
			{
				this.bodyBackColor = value;
			}
		}

		#endregion

		#region BodyForeColor

		private Color bodyForeColor = Color.White;

		public Color BodyForeColor
		{
			get
			{
				return this.bodyForeColor;
			}
			set
			{
				this.bodyForeColor = value;
			}
		}

		#endregion

		#region BodyStyle

		private ColorStyle bodyStyle = ColorStyle.BackwardDiagonalGradient;

		public ColorStyle BodyStyle
		{
			get
			{
				return this.bodyStyle;
			}
			set
			{
				this.bodyStyle = value;
			}
		}

		#endregion

		#region OutlineColor

		private Color outlineColor = Color.FromArgb(148, 150, 148);

		public Color OutlineColor
		{
			get
			{
				return this.outlineColor;
			}
			set
			{
				this.outlineColor = value;
			}
		}

		#endregion

		#region OutlineSize

		private int outlineSize = 1;

		public int OutlineSize
		{
			get
			{
				return this.outlineSize;
			}
			set
			{
				this.outlineSize = value;
			}
		}

		#endregion

		#region IconsNormalColor

		private Color iconsNormalColor = Color.FromArgb(148, 150, 148);

		public Color IconsNormalColor
		{
			get
			{
				return this.iconsNormalColor;
			}
			set
			{
				this.iconsNormalColor = value;
			}
		}

		#endregion

		#region IconsHiLiteColor

		private Color iconsHiLiteColor = Color.FromArgb(16, 48, 107);

		public Color IconsHiLiteColor
		{
			get
			{
				return this.iconsHiLiteColor;
			}
			set
			{
				this.iconsHiLiteColor = value;
			}
		}

		#endregion


		#region MinimumHeight

		private int minimumHeight = 200;

		public int MinimumHeight
		{
			get
			{
				return this.minimumHeight;
			}

			set
			{
				this.minimumHeight = value;
			}
		}

		#endregion

		#region MinimumWidth

		private int minimumWidth = 200;

		public int MinimumWidth
		{
			get
			{
				return this.minimumWidth;
			}

			set
			{
				this.minimumWidth = value;
			}
		}

		#endregion

	
		#region BodyRectangle

		public Rectangle BodyRectangle
		{
			get
			{
				return this.rectBody;
			}
		}

		#endregion

		#region ResizableRectangle

		public Rectangle ResizableRectangle
		{
			get
			{
				return this.rectResizableArea;
			}
		}

		#endregion

		#region mouseInTitleArea

		private bool mouseInTitleArea
		{
			get
			{
				return this.isMousePointerInArea(Control.MousePosition, this.rectTitleArea);
			}
		}

		#endregion

		#region mouseInResizeArea

		private bool mouseInResizeArea
		{
			get
			{
				return this.isMousePointerInArea(Control.MousePosition, this.rectResizableArea);
			}
		}

		#endregion

		#region mouseInCloseIconArea

		private bool mouseInCloseIconArea
		{
			get
			{
				return this.isMousePointerInArea(Control.MousePosition, this.rectCloseIcon);
			}
		}

		#endregion

		#region mouseInMinimizeIconArea

		private bool mouseInMinimizeIconArea
		{
			get
			{
				return this.isMousePointerInArea(Control.MousePosition, this.rectMinimizeIcon);
			}
		}

		#endregion

		#endregion

		#region Class Events

		#region OnPaint

		protected override void OnPaint(PaintEventArgs e)
		{
			#region Declare the internal variables

			SolidBrush bSolid = null;
			LinearGradientBrush bGradient = null;
			Pen p;
			Bitmap bmp;
			Graphics g;
			GraphicsPath gpTitlePath, gpRegion;
			Rectangle rectTopArea, rectBottomArea;
			Rectangle rectLeftCorner, rectRightCorner;
			Rectangle rectTitleBar, rectTitleIcon;
			StringFormat sf;
			SizeF sizeTitleText;
			Size sizeTitleBar, sizeTitleBody;

			#endregion

			#region Initialize the common variables

			// Create a new Bitmap object
			bmp = new Bitmap(this.Width, this.Height);

			// Initialize the Form Graphics instance
			g = Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = InterpolationMode.High;

			// Create a new Brush object
			bSolid = new SolidBrush(this.transparentColor);

			// Fill all the area with pink (will be used to create the transparent region)
			g.FillRectangle(bSolid, 0, 0, this.Width, this.Height);

			// Dispose the Brush object
			bSolid.Dispose();
			bSolid = null;

			// Create the rectangles that will be used to round the corners of the form
			rectLeftCorner  = new Rectangle(0, 0, (edgeRadius * 2), (edgeRadius * 2));
			rectRightCorner = new Rectangle(this.Width - (edgeRadius * 2) - 1, 0, (edgeRadius * 2), (edgeRadius * 2));

			// Create the remaining rectangles to complete the form
			rectTopArea    = new Rectangle(edgeRadius, 0, this.Width - (edgeRadius * 2), this.Height);
			rectBottomArea = new Rectangle(0, edgeRadius, this.Width, this.Height);

			#endregion

			#region Calculate the TitleBar and Body dimensions

			sizeTitleText = g.MeasureString(this.Text, this.TitleFont);
			sizeTitleBar = new Size(0, 0);
			sizeTitleBar.Width = this.Width;
			sizeTitleBar.Height = Convert.ToInt32(sizeTitleText.Height) + (padding * 2);

			if (this.Icon != null && sizeTitleBar.Height < 16 + (padding * 2))
			{
				sizeTitleBar.Height = 16 + (padding * 2);
			}

			sizeTitleBody = new Size(0, 0);
			sizeTitleBody.Width = this.Width;
			sizeTitleBody.Height = this.Height - sizeTitleBar.Height;
			
			rectTitleBar = new Rectangle(0, 0, sizeTitleBar.Width, sizeTitleBar.Height);
			rectBody     = new Rectangle(0, sizeTitleBar.Height, sizeTitleBody.Width, sizeTitleBody.Height);

			#endregion

			#region Draw the TitleBar background color

			// Create a GraphicsPath instance to map our form title shape
			gpTitlePath = new GraphicsPath();
			gpTitlePath.AddArc(rectLeftCorner, 180, 90);
			gpTitlePath.AddArc(rectRightCorner, 270, 90);

			GraphicsPath gTitlePath2 = new GraphicsPath();
			gTitlePath2.AddRectangle(new Rectangle(0, edgeRadius, rectTitleBar.Width - 1, rectTitleBar.Height));
			gTitlePath2.AddPath(gpTitlePath, true);
			gTitlePath2.Flatten();

			if (this.TitleStyle == ColorStyle.Solid)
			{
				// Create a new Brush object
				bSolid = new SolidBrush(this.TitleForeColor);

				// Fill the title area
				g.FillPath(bSolid, gTitlePath2);

				// Dispose the Brush object
				bSolid.Dispose();
				bSolid = null;
			}
			else
			{
				switch (this.TitleStyle)
				{
					case ColorStyle.BackwardDiagonalGradient :
						// Create a new Brush object
						bGradient = new LinearGradientBrush(rectTitleBar, this.TitleForeColor, this.TitleBackColor, LinearGradientMode.BackwardDiagonal);
						break;

					case ColorStyle.ForwardDiagonalGradient :
						// Create a new Brush object
						bGradient = new LinearGradientBrush(rectTitleBar, this.TitleForeColor, this.TitleBackColor, LinearGradientMode.ForwardDiagonal);
						break;
					
					case ColorStyle.HorizontalGradient :
						// Create a new Brush object
						bGradient = new LinearGradientBrush(rectTitleBar, this.TitleForeColor, this.TitleBackColor, LinearGradientMode.Horizontal);
						break;
					
					case ColorStyle.VerticalGradient :
						// Create a new Brush object
						bGradient = new LinearGradientBrush(rectTitleBar, this.TitleForeColor, this.TitleBackColor, LinearGradientMode.Vertical);
						break;
				}

				g.FillPath(bGradient, gTitlePath2);

				// Dispose the LinearGradientBrush object
				bGradient.Dispose();
				bGradient = null;
			}

			#endregion

			#region Draw the Title icons and text

			// Draw the Title icon and text
			bSolid = new SolidBrush(this.TitleColor);
			sf = new StringFormat();
			sf.Alignment =  StringAlignment.Near;
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Trimming = StringTrimming.None;

			rectTitleArea = new Rectangle(0, 0, 0, 0);
			rectTitleArea.X = edgeRadius / 2 + padding;
			rectTitleArea.Y = Convert.ToInt32((rectTitleBar.Height - sizeTitleText.Height) / 2);
			rectTitleArea.Width = this.Width - (edgeRadius * 2) - padding;
			rectTitleArea.Height = padding + Convert.ToInt32(sizeTitleText.Height);

			#region Draw the Application icon

			if (this.Icon != null)
			{
				if (this.bmpIcon == null)
				{
					bmpIcon = this.Icon.ToBitmap();
				}

				rectTitleIcon = new Rectangle(0, 0, 0, 0);
				rectTitleIcon.X = edgeRadius / 2 + padding;
				rectTitleIcon.Y = Convert.ToInt32((rectTitleBar.Height - sizeTitleText.Height) / 2);
				rectTitleIcon.Width = 16;
				rectTitleIcon.Height = 16;

				g.SmoothingMode = SmoothingMode.HighQuality;
				g.DrawImage(bmpIcon, rectTitleIcon, 0, 0, 32,32, GraphicsUnit.Pixel);
				g.SmoothingMode = SmoothingMode.None;

				// Compensate for the space taken by the icon image
				rectTitleArea.X = rectTitleArea.X  + 16 + padding;
			}

			#endregion

			#region Draw the Close icon

			rectCloseIcon = new Rectangle(0, 0, 0, 0);
			rectCloseIcon.X = this.Width - edgeRadius - padding - 16 + 2;
			rectCloseIcon.Y = Convert.ToInt32((rectTitleBar.Height - sizeTitleText.Height) / 2) + 2;
			rectCloseIcon.Width = 16 - 2;
			rectCloseIcon.Height = 16 - 2;

			// Create a new Pen object
			if (this.onCloseIcon == false)
				p = new Pen(this.IconsNormalColor, 1);
			else
				p = new Pen(this.IconsHiLiteColor, 1);

			g.DrawLine(p, rectCloseIcon.X + 1, rectCloseIcon.Y, rectCloseIcon.X + rectCloseIcon.Width - 1, rectCloseIcon.Y);
			g.DrawLine(p, rectCloseIcon.X, rectCloseIcon.Y + 1, rectCloseIcon.X, rectCloseIcon.Y + rectCloseIcon.Height - 1);
			g.DrawLine(p, rectCloseIcon.X + rectCloseIcon.Width, rectCloseIcon.Y + 1, rectCloseIcon.X + rectCloseIcon.Width, rectCloseIcon.Y + rectCloseIcon.Height - 1);
			g.DrawLine(p, rectCloseIcon.X + 1, rectCloseIcon.Y + rectCloseIcon.Height, rectCloseIcon.X + rectCloseIcon.Width - 1, rectCloseIcon.Y + rectCloseIcon.Height);

			// Dispose the Pen object
			p.Dispose();
			p = null;

			// Create a new Pen object
			if (this.onCloseIcon == false)
				p = new Pen(this.IconsNormalColor, 2);
			else
				p = new Pen(this.IconsHiLiteColor, 2);

			g.DrawLine(p, rectCloseIcon.X + 3, rectCloseIcon.Y + 3, rectCloseIcon.X + rectCloseIcon.Width - 3, rectCloseIcon.Y + rectCloseIcon.Height - 3);
			g.DrawLine(p, rectCloseIcon.X + rectCloseIcon.Width - 3, rectCloseIcon.Y + 3, rectCloseIcon.X + 3, rectCloseIcon.Y + rectCloseIcon.Height - 3);

			rectCloseIcon.Width += 1;
			rectCloseIcon.Height += 1;

			#endregion

			#region Draw the Minimize icon

			rectMinimizeIcon = new Rectangle(0, 0, 0, 0);
			rectMinimizeIcon.X = this.Width - edgeRadius - (padding * 2) - (16 * 2) + 2;
			rectMinimizeIcon.Y = Convert.ToInt32((rectTitleBar.Height - sizeTitleText.Height) / 2) + 2;
			rectMinimizeIcon.Width = 16 - 2;
			rectMinimizeIcon.Height = 16 - 2;

			// Create a new Pen object
			if (this.onMinimizeIcon == false)
				p = new Pen(this.IconsNormalColor, 1);
			else
				p = new Pen(this.IconsHiLiteColor, 1);

			g.DrawLine(p, rectMinimizeIcon.X + 1, rectMinimizeIcon.Y, rectMinimizeIcon.X + rectMinimizeIcon.Width - 1, rectMinimizeIcon.Y);
			g.DrawLine(p, rectMinimizeIcon.X, rectMinimizeIcon.Y + 1, rectMinimizeIcon.X, rectMinimizeIcon.Y + rectMinimizeIcon.Height - 1);
			g.DrawLine(p, rectMinimizeIcon.X + rectMinimizeIcon.Width, rectMinimizeIcon.Y + 1, rectMinimizeIcon.X + rectMinimizeIcon.Width, rectMinimizeIcon.Y + rectMinimizeIcon.Height - 1);
			g.DrawLine(p, rectMinimizeIcon.X + 1, rectMinimizeIcon.Y + rectMinimizeIcon.Height, rectMinimizeIcon.X + rectMinimizeIcon.Width - 1, rectMinimizeIcon.Y + rectMinimizeIcon.Height);

			// Dispose the Pen object
			p.Dispose();
			p = null;

			// Create a new Pen object
			if (this.onMinimizeIcon == false)
				p = new Pen(this.IconsNormalColor, 2);
			else
				p = new Pen(this.IconsHiLiteColor, 2);

			g.DrawLine(p, rectMinimizeIcon.X + 3, rectMinimizeIcon.Y + rectMinimizeIcon.Height - 3, rectMinimizeIcon.X + rectMinimizeIcon.Width - 2, rectMinimizeIcon.Y + rectMinimizeIcon.Height - 3);

			rectMinimizeIcon.Width += 1;
			rectMinimizeIcon.Height += 1;

			#endregion

			// Compensate for the space taken by the icon image
			rectTitleArea.Width = rectTitleArea.Width - 50;

			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawString(this.Text, this.TitleFont, bSolid, rectTitleArea, sf);
			g.SmoothingMode = SmoothingMode.None;

			// Make sure that TitleArea Rectangle values covers the whole title area
			rectTitleArea.X = 0;
			rectTitleArea.Y = 0;
			rectTitleArea.Width = this.Width;

			// Dispose the StringFormat object
			sf.Dispose();
			sf = null;

			// Dispose the Brush object
			bSolid.Dispose();
			bSolid = null;

			#endregion

			#region Draw the Body background color

			if (this.windowMaximized == true)
			{
				if (this.BodyStyle == ColorStyle.Solid)
				{
					// Create a new Brush object
					bSolid = new SolidBrush(this.BodyForeColor);

					// Fill the body area
					g.FillRectangle(bSolid, rectBody);

					// Dispose the Brush object
					bSolid.Dispose();
					bSolid = null;
				}
				else
				{
					switch (this.BodyStyle)
					{
						case ColorStyle.BackwardDiagonalGradient :
							// Create a new Brush object
							bGradient = new LinearGradientBrush(rectBody, this.BodyForeColor, this.BodyBackColor, LinearGradientMode.BackwardDiagonal);
							break;

						case ColorStyle.ForwardDiagonalGradient :
							// Create a new Brush object
							bGradient = new LinearGradientBrush(rectBody, this.BodyForeColor, this.BodyBackColor, LinearGradientMode.ForwardDiagonal);
							break;
					
						case ColorStyle.HorizontalGradient :
							// Create a new Brush object
							bGradient = new LinearGradientBrush(rectBody, this.BodyForeColor, this.BodyBackColor, LinearGradientMode.Horizontal);
							break;
					
						case ColorStyle.VerticalGradient :
							// Create a new Brush object
							bGradient = new LinearGradientBrush(rectBody, this.BodyForeColor, this.BodyBackColor, LinearGradientMode.Vertical);
							break;
					}

					// Fill the body area
					g.FillRectangle(bGradient, rectBody);

					// Dispose the LinearGradientBrush object
					bGradient.Dispose();
					bGradient = null;
				}
			}

			#endregion

			#region Draw the Resizable symbol

			if (this.IsResizable == true && this.windowMaximized == true)
			{
				rectResizableArea = new Rectangle(0, 0, 0, 0);
				rectResizableArea.X = this.Width - padding - 11;
				rectResizableArea.Y = this.Height - padding - 11;
				rectResizableArea.Height = padding + 11;
				rectResizableArea.Width = padding + 11;

				// Create a new Pen object
				p = new Pen(this.ResizableColor, 2);

				// Draw the resizable symbol
				g.DrawLine(p, this.Width - padding - 11, this.Height - padding, this.Width - padding, this.Height - padding - 11);
				g.DrawLine(p, this.Width - padding - 7, this.Height - padding, this.Width - padding, this.Height - padding - 7);
				g.DrawLine(p, this.Width - padding - 3, this.Height - padding, this.Width - padding, this.Height - padding - 3);

				// Dispose the Pen object
				p.Dispose();
				p = null;
			}

			#endregion

			#region Draw the Form Outline

			// Create a new Pen object
			p = new Pen(this.OutlineColor, this.OutlineSize);

			// Draw the form outline
			g.DrawArc(p, rectLeftCorner, 180, 90);
			g.DrawArc(p, rectRightCorner, 270, 90);
			g.DrawLine(p, edgeRadius, 0, this.Width - edgeRadius, 0);
			g.DrawLine(p, 0, edgeRadius, 0, this.Height);
			g.DrawLine(p, this.Width - 1, edgeRadius, this.Width - 1, this.Height);
			g.DrawLine(p, 0, this.Height - 1, this.Width, this.Height - 1);

			// Dispose the Pen object
			p.Dispose();
			p = null;

			#endregion

			#region Create/Apply a transparent region

			if (this.Region != null)
			{
				this.Region.Dispose();
				this.Region = null;
			}

			// Create GraphicsPath to be used to crop the region required
			gpRegion = new GraphicsPath();

			// Loop through every pixel in the top left corner.
			// Create a 1 x 1 rectangle regions of pixels that do not match the transparent color
			for (int x = rectLeftCorner.X; x < rectLeftCorner.Width; x++)
			{
				for (int y = rectLeftCorner.Y; y < rectLeftCorner.Height / 2; y++) 
				{
					if (isSameColor(bmp.GetPixel(x, y), this.transparentColor) == false) 
					{
						gpRegion.AddRectangle(new Rectangle(x, y, 1, 1));
					}
				}
			}

			// Loop through every pixel in the top right corner.
			// Create a 1 x 1 rectangle regions of pixels that do not match the transparent color
			for (int x = rectRightCorner.X + 1; x < rectRightCorner.X + rectRightCorner.Width + 1; x++)
			{
				for (int y = rectRightCorner.Y; y < rectRightCorner.Y + rectRightCorner.Height / 2; y++) 
				{
					if (isSameColor(bmp.GetPixel(x, y), this.transparentColor) == false) 
					{
						gpRegion.AddRectangle(new Rectangle(x, y, 1, 1));
					}
				}
			}

			// Create the remaining rectangular regions to complete cover all the windows form area
			gpRegion.AddRectangle(new Rectangle(rectLeftCorner.Width, 0, this.Width - (edgeRadius * 4), rectLeftCorner.Height / 2));
			gpRegion.AddRectangle(new Rectangle(0, rectLeftCorner.Height / 2, this.Width, this.Height));

			// Apply region
			this.Region = new Region(gpRegion);

			#endregion

			#region Draw the buffered image on the windows form Graphics object

			// Draw the buffered image on the windows form Graphics object
			e.Graphics.DrawImageUnscaled(bmp, 0, 0, this.Width, this.Height);

			// Dispose the Graphics object
			g.Dispose();
			g = null;

			// Dispose the Bitmap object
			bmp.Dispose();
			bmp = null;

			#endregion
		}

		#endregion

		#region OnDisposed

		private void OnDisposed(object sender, EventArgs e)
		{
			this.isDisposing = true;
		}

		#endregion

		#region OnMouseDown

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mouseDown = true;
				mousePosition = Control.MousePosition;
			}
		}

		#endregion

		#region OnMouseUp

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.mouseDown = false;
				this.windowMoving = false;
				this.windowResizing = false;

				if (this.mouseInCloseIconArea == true)
					this.Close();

				if (this.mouseInMinimizeIconArea == true)
				{
					this.onMinimizeIcon = false;
					this.Region = null;
					this.WindowState = FormWindowState.Minimized;
				}
			}
		}

		#endregion

		#region OnMouseMove

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int x, y, width, height;
			Point currentMousePosition = Control.MousePosition;

			if (mouseDown == true)
			{
				// Window moving
				if (this.mouseInTitleArea == true || this.windowMoving == true)
				{
					x = currentMousePosition.X - this.mousePosition.X + this.Location.X;
					y = currentMousePosition.Y - this.mousePosition.Y + this.Location.Y;

					#region Snap the form window to special locations

					if (this.IsWindowSnappable)
					{
						if (snapHitTest(x, 0) == true)
							x = 0;
						else if (snapHitTest(x + this.Width, this.ClientRectangle.Width) == true)
							x = this.ClientRectangle.Width - this.Width;
						else if (snapHitTest(x + this.Width, Screen.PrimaryScreen.WorkingArea.Width) == true)
							x = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
						else if (this.MdiParent != null && snapHitTest(x + this.Width, this.MdiParent.ClientRectangle.Width - 4) == true)
							x = this.MdiParent.ClientRectangle.Width - this.Width - 4;

						if (snapHitTest(y, 0) == true)
							y = 0;
						else if (snapHitTest(y + this.Height, this.ClientRectangle.Height) == true)
							y = this.ClientRectangle.Height - this.Height;
						else if (snapHitTest(y + this.Height, Screen.PrimaryScreen.WorkingArea.Height) == true)
							y = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
						else if (this.MdiParent != null && snapHitTest(y + this.Height, this.MdiParent.ClientRectangle.Height - 4) == true)
							y = this.MdiParent.ClientRectangle.Height - this.Height - 4;
					}

					#endregion

					this.Location = new Point(x, y);
					this.mousePosition = currentMousePosition;
					this.windowMoving = true;
				}

					// Window resizing
				else if ((this.mouseInResizeArea || this.windowResizing) && this.IsResizable)
				{
					width  = currentMousePosition.X - this.mousePosition.X + this.Size.Width;
					height = currentMousePosition.Y - this.mousePosition.Y + this.Size.Height;

					if (width < this.MinimumWidth)
					{
						width = this.MinimumWidth;
						this.windowResizing = false;
					}
					else if (height < this.MinimumHeight)
					{
						height = this.MinimumHeight;
						this.windowResizing = false;
					}
					else
					{
						this.windowResizing = true;
					}

					this.Size = new Size(width, height);
					this.mousePosition = currentMousePosition;
				}
			}
			else
			{
				if (this.mouseInResizeArea == true && this.IsResizable == true)
					this.Cursor = Cursors.SizeNWSE;
				else
					this.Cursor = Cursors.Arrow;

				if (this.mouseInCloseIconArea == true)
				{
					this.onCloseIcon = true;
					this.Invalidate(this.rectCloseIcon, false);
				}
				else
				{
					if (this.onCloseIcon == true)
						this.Invalidate(this.rectCloseIcon, false);

					this.onCloseIcon = false;
				}

				if (this.mouseInMinimizeIconArea == true)
				{
					this.onMinimizeIcon = true;
					this.Invalidate(this.rectMinimizeIcon, false);
				}
				else
				{
					if (this.onMinimizeIcon == true)
						this.Invalidate(this.rectMinimizeIcon, false);

					this.onMinimizeIcon = false;
				}
			}
		}

		#endregion

		#region OnDoubleClick

		protected override void OnDoubleClick(EventArgs e)
		{
			if (this.mouseInTitleArea == true)
			{
				if (this.mouseInCloseIconArea == true)
				{
					// Do Nothing
				}
				else if (this.mouseInMinimizeIconArea == true)
				{
					// Do Nothing
				}
				else if (this.windowMaximized == true)
				{
					this.windowMaximized = false;
					this.windowHeight = this.Height;
					this.Height = this.rectTitleArea.Height + padding;
				}
				else
				{
					this.windowMaximized = true;
					this.Height = this.windowHeight;
				}
			}
		}

		#endregion



		#region OnMenuMeasureItem Event

		private void OnMenuMeasureItem(object sender, MeasureItemEventArgs e)
		{
			MenuItem item = (MenuItem) sender;

			if (item.Text == "-")
			{
				e.ItemHeight = 4;
			}
			else
			{
				e.ItemHeight = 18;
				e.ItemWidth = Convert.ToInt32(e.Graphics.MeasureString(item.Text, this.Font).Width) + 30;
			}
		}

		#endregion

		#region OnMenuSeperatorDrawItem

		private void OnMenuSeperatorDrawItem(object sender, DrawItemEventArgs e)
		{
			drawMenuItem((MenuItem) sender, e);
		}

		#endregion

		#region OnMenuRestoreClick

		private void OnMenuRestoreClick(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Normal;
		}

		#endregion

		#region OnMenuRestoreDrawItem

		private void OnMenuRestoreDrawItem(object sender, DrawItemEventArgs e)
		{
			MenuItem item = (MenuItem) sender;

			drawMenuItem(item, e);
			drawMenuIcon("StyledForms.restore.png", e);
		}

		#endregion

		#region OnMenuMaximizeClick

		private void OnMenuMaximizeClick(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Maximized;
		}

		#endregion

		#region OnMenuMaximizeDrawItem

		private void OnMenuMaximizeDrawItem(object sender, DrawItemEventArgs e)
		{
			MenuItem item = (MenuItem) sender;

			drawMenuItem(item, e);
			drawMenuIcon("StyledForms.maximize.png", e);
		}
		#endregion

		#region OnMenuCloseClick

		private void OnMenuCloseClick(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region OnMenuCloseDrawItem

		private void OnMenuCloseDrawItem(object sender, DrawItemEventArgs e)
		{
			MenuItem item = (MenuItem) sender;

			drawMenuItem(item, e);
			drawMenuIcon("StyledForms.close.png", e);
		}

		#endregion

		#endregion

		#region Class Methods

		#region WndProc

		protected override void WndProc(ref Message m)
		{
			ContextMenu menu;
			int num1 = m.Msg;

			if (num1 == 0x313)
			{
				Point position = new Point(0);
				position.Y = Convert.ToInt32(m.LParam.ToInt64() >> 16);
				position.X = Convert.ToInt32(m.LParam.ToInt64() & 0x0000ffff);

				hiddenForm.Show();

				menu = createContextMenu();
				menu.Show(hiddenForm, position);

				hiddenForm.Hide();

				return;
			}

			base.WndProc (ref m);
		}

		#endregion

		#region isMousePointerInArea

		private bool isMousePointerInArea(Point mousePosition, Rectangle area)
		{
			if (!this.isDisposing)
				return area.Contains(this.PointToClient(mousePosition));
			else
				return false;
		}

		#endregion

		#region snapHitTest

		private bool snapHitTest(int value, int snapValue)
		{
			if (value > snapValue - snapPixels && value < snapValue + snapPixels)
			{
				return true;
			}

			return false;
		}

		#endregion

		#region isSameColor

		private static bool isSameColor(Color color1, Color color2)
		{
			if (color1.A != color2.A)
			{
				return false;
			}
			
			if (color1.B != color2.B)
			{
				return false;
			}

			if (color1.G != color2.G)
			{
				return false;
			}

			if (color1.R != color2.R)
			{
				return false;
			}

			return true;
		}

		#endregion

		#region createContextMenu

		private ContextMenu createContextMenu()
		{
			MenuItem item = null;
			ContextMenu menu = new ContextMenu();

			item = new MenuItem();
			item.Text = "Restore";
			item.Visible = false;
			item.OwnerDraw = true;
			item.DrawItem += new DrawItemEventHandler(OnMenuRestoreDrawItem);
			item.MeasureItem +=new MeasureItemEventHandler(OnMenuMeasureItem);
			item.Click += new EventHandler(OnMenuRestoreClick);
			menu.MenuItems.Add(item);

			if (this.WindowState == FormWindowState.Minimized)
				item.Visible = true;

			item = new MenuItem();
			item.Text = "Maximize";
			item.OwnerDraw = true;
			item.DrawItem += new DrawItemEventHandler(OnMenuMaximizeDrawItem);
			item.MeasureItem +=new MeasureItemEventHandler(OnMenuMeasureItem);
			item.Click += new EventHandler(OnMenuMaximizeClick);
			menu.MenuItems.Add(item);

			item = new MenuItem();
			item.Text = "-";
			item.OwnerDraw = true;
			item.DrawItem += new DrawItemEventHandler(OnMenuSeperatorDrawItem);
			item.MeasureItem +=new MeasureItemEventHandler(OnMenuMeasureItem);
			menu.MenuItems.Add(item);

			item = new MenuItem();
			item.Text = "Close";
			item.OwnerDraw = true;
			item.DrawItem += new DrawItemEventHandler(OnMenuCloseDrawItem);
			item.MeasureItem +=new MeasureItemEventHandler(OnMenuMeasureItem);
			item.Click += new EventHandler(OnMenuCloseClick);
			menu.MenuItems.Add(item);

			if (this.RightToLeft == RightToLeft.Yes)
				menu.RightToLeft = RightToLeft.Yes;
			else
				menu.RightToLeft = RightToLeft.No;

			return menu;
		}

		#endregion

		#region drawMenuItem

		private void drawMenuItem(MenuItem item, DrawItemEventArgs e)
		{
			#region Declare Variables

			SolidBrush b = null;
			Pen p = null;

			#endregion

			#region Draw the MenuItem background

			b = new SolidBrush(Color.FromArgb(214, 211, 206));

			e.Graphics.FillRectangle(b, e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height + 1);

			b.Dispose();
			b = null;

			#endregion

			#region Draw the MenuItem separator

			if (item.Text == "-")
			{
				p = new Pen(Color.FromArgb(132, 130, 132), 1F);

				e.Graphics.DrawLine(p, e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width + 1, e.Bounds.Y + 1);

				p.Dispose();
				p = null;

				p = new Pen(Color.White, 1F);

				e.Graphics.DrawLine(p, e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width + 1, e.Bounds.Y + 2);

				p.Dispose();
				p = null;
			}

			#endregion

			#region Draw the MenuItem text

			if (item.Text != "=")
			{
				if ((e.State & DrawItemState.Selected) != 0)
				{
					b = new SolidBrush(Color.FromArgb(8, 36, 107));

					e.Graphics.FillRectangle(b, e.Bounds);

					b.Dispose();
					b = null;

					b = new SolidBrush(Color.White);

					e.Graphics.DrawString(item.Text, e.Font, b, e.Bounds.X + 22, e.Bounds.Y + 2);

					b.Dispose();
					b = null;
				}
				else
				{
					b = new SolidBrush(Color.Black);

					e.Graphics.DrawString(item.Text, e.Font, b, e.Bounds.X + 22, e.Bounds.Y + 2);

					b.Dispose();
					b = null;
				}
			}

			#endregion
		}

		#endregion

		#region drawMenuIcon

		private void drawMenuIcon(string iconPath, DrawItemEventArgs e)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = null;
			Bitmap bmp = null;

			try
			{
				stream = assembly.GetManifestResourceStream(iconPath);
				stream.Position = 0;

				bmp = (Bitmap)Bitmap.FromStream(stream);

				if ((e.State & DrawItemState.Selected) != 0)
				{
					for (int x = 0; x < bmp.Width; x++)
					{
						for (int y = 0; y < bmp.Height; y++) 
						{
							if (isSameColor(bmp.GetPixel(x, y), Color.Black) == true) 
								bmp.SetPixel(x, y, Color.White);
						}
					}
				}

				e.Graphics.DrawImageUnscaled(bmp, e.Bounds.X, e.Bounds.Y + 2);
			}
			finally
			{
				#region Dispose objects

				if (bmp != null)
				{
					bmp.Dispose();
					bmp = null;
				}

				if (stream != null)
				{
					stream.Close();
					stream = null;
				}

				#endregion
			}
		}

		#endregion

		#region Dispose

		/// <summary>Clean up any resources being used.</summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}

			base.Dispose( disposing );
		}

		#endregion

		#endregion
	}
}
