using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Styled
{
	public class GoogleTalkFormDesigner : DocumentDesigner
	{
		#region Class Variables

		private ArrayList controls;

		#endregion

		#region Class Methods

		#region Initialize

		public override void Initialize(IComponent component) 
		{ 
			base.Initialize(component); 
 
			ISelectionService service = (ISelectionService)this.GetService(typeof(ISelectionService)); 
 
			if (service != null) 
				service.SelectionChanged += new EventHandler(this.OnSelectionChanged);
		}

		#endregion

		#region OnControlLocationChanged

		private void OnControlLocationChanged(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			ControlState controlState = this.findControlState(control);

			if (isControlLocationValid(control) == false)
				control.Location = controlState.Location;
			else
				controlState.Location = control.Location;
		}

		#endregion

		#region OnPaintAdornments

		/// <summary>
		/// Called when the control that the designer is managing has painted its surface so the designer can paint any additional adornments on top of the control.
		/// </summary>
		/// <param name="pe">A <see cref="System.Windows.Forms.PaintEventArgs"/> that provides data for the event.</param>
		protected override void OnPaintAdornments(PaintEventArgs pe) 
		{ 		
			if (this.Control is GoogleTalkForm && this.DrawGrid == true)
			{
				GoogleTalkForm control = (GoogleTalkForm)this.Control;
				Rectangle rect = control.BodyRectangle;
				rect.X++;
				rect.Width++;
				rect.Height++;

				ControlPaint.DrawGrid(pe.Graphics, rect, this.GridSize, control.BackColor);
			}
			else
			{
				base.OnPaintAdornments(pe);
			}
		}

		#endregion

		#region OnSelectionChanged

		private void OnSelectionChanged(object sender, EventArgs e) 
		{
			ISelectionService service = (ISelectionService)sender; 
			ControlState controlState;

			if (service != null && service.SelectionCount != 0) 
			{
				foreach (Control control in service.GetSelectedComponents())
				{
					controlState = new ControlState();

					controlState.Control = control;
					controlState.Control.LocationChanged += new EventHandler(OnControlLocationChanged);
					controlState.Location = control.Location;

					if (this.controls == null)
						this.controls = new ArrayList();

					this.controls.Add(controlState);
				}

				return;
			}

			this.controls = null;
		}

		#endregion

		#region findControlState

		private ControlState findControlState(Control control)
		{
			if (this.controls == null)
				return null;

			foreach(ControlState controlState in this.controls)
			{
				if (controlState.Control == control)
					return controlState;
			}

			return null;
		}

		#endregion

		#region isControlLocationValid

		private bool isControlLocationValid(Control control)
		{
			Point pointTopLeft;
			Point pointBottomRight;

			if (this.Control is GoogleTalkForm)
			{
				GoogleTalkForm form = (GoogleTalkForm)this.Control;

				pointTopLeft = control.Location;

				pointBottomRight = control.Location;
				pointBottomRight.X += control.Width;
				pointBottomRight.Y += control.Height;

				if (form.BodyRectangle.Contains(pointTopLeft) == false)
					return false;

				if (form.BodyRectangle.Contains(pointBottomRight) == false)
					return false;

				if (form.IsResizable == true)
				{
					if (form.ResizableRectangle.Contains(pointTopLeft) == true)
						return false;

					if (form.ResizableRectangle.Contains(pointBottomRight) == true)
						return false;
				}

				return true;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#endregion

		#region Internal Classes

		private class ControlState
		{
			public Control Control;
			public Point Location;
		}

		#endregion
	}
}
