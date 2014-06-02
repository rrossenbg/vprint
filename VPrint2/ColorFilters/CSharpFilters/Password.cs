using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CSharpFilters
{
	/// <summary>
	/// Summary description for Password.
	/// </summary>
	public class Password : System.Windows.Forms.Form
	{
		public string ThePassword = "";

		private System.Windows.Forms.TextBox EnterPassword;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Password()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.EnterPassword = new System.Windows.Forms.TextBox();
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// EnterPassword
			// 
			this.EnterPassword.Location = new System.Drawing.Point(24, 16);
			this.EnterPassword.Name = "EnterPassword";
			this.EnterPassword.Size = new System.Drawing.Size(232, 20);
			this.EnterPassword.TabIndex = 0;
			this.EnterPassword.Text = "";
			// 
			// OK
			// 
			this.OK.Location = new System.Drawing.Point(48, 64);
			this.OK.Name = "OK";
			this.OK.TabIndex = 1;
			this.OK.Text = "OK";
			this.OK.DialogResult = DialogResult.OK;
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// Cancel
			// 
			this.Cancel.Location = new System.Drawing.Point(144, 64);
			this.Cancel.Name = "Cancel";
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "Cancel";
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// Password
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(280, 94);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.Cancel,
																		  this.OK,
																		  this.EnterPassword});
			this.Name = "Password";
			this.Text = "Enter Password";
			this.ResumeLayout(false);

		}
		#endregion

		private void OK_Click(object sender, System.EventArgs e)
		{
			ThePassword = EnterPassword.Text;
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
