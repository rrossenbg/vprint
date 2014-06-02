/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Styled;
using PremierTaxFree.Controls;

namespace PremierTaxFree
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/KB/dotnet/iextenderprovider.aspx"/>
    [ProvideProperty("HookupToTextBoxBase", typeof(TextBoxBase))]
    [ProvideProperty("HookupToVistaCheckBox", typeof(VistaCheckBox))]
	[ProvideProperty("MyInt", typeof(ButtonBase))]
    [ProvideProperty("MyString", typeof(Panel))]
	public class DirtyControlExtender : Component, IExtenderProvider, ISupportInitialize
	{
		private Hashtable properties;
		private System.ComponentModel.Container components = null;

        public IDirtyControl Control { get; set; }

		public DirtyControlExtender()
		{
			properties = new Hashtable();
			InitializeComponent();
		}

        public DirtyControlExtender(System.ComponentModel.IContainer container)
            : this()
        {
            container.Add(this);
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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		bool IExtenderProvider.CanExtend(object o)
		{
			if( o is TextBoxBase || o is VistaCheckBox )
				return true;
			else
				return false;
		}

		private class Properties
		{
			public string MyString;
			public int MyInt;

			public Properties()
			{
				MyString = System.Environment.Version.ToString();
				MyInt = -1;
			}
		}

		private Properties EnsurePropertiesExists(object key)
		{
			Properties p = (Properties) properties[key];
			if( p == null )
			{
				p = new Properties();
				properties[key] = p;
			}

			return p;
		}

		[Category("Design")]
		[DefaultValue(false)]
        public bool GetHookupToTextBoxBase(TextBoxBase txt)
		{
			EnsurePropertiesExists(txt);

			// This shouldn't get called at run time,
			// but in case it does don't do anything
			if( DesignMode )
			{
                txt.TextChanged -= new EventHandler(Control_Changed);
                txt.TextChanged += new EventHandler(Control_Changed);
			}

			return true;
		}

        public void SetHookupToTextBoxBase(TextBoxBase txt, bool value)
        {
            EnsurePropertiesExists(txt);
        }

        [Category("Design")]
        [DefaultValue(false)]
        public bool GetHookupToVistaCheckBox(VistaCheckBox vcb)
        {
            EnsurePropertiesExists(vcb);

            // This shouldn't get called at run time,
            // but in case it does don't do anything
            if (DesignMode)
            {
                vcb.Click -= new EventHandler(Control_Changed);
                vcb.Click += new EventHandler(Control_Changed);
            }

            return true;
        }

        public void SetHookupToVistaCheckBox(VistaCheckBox vcb, bool value)
        {
            EnsurePropertiesExists(vcb);
        }

        private void Control_Changed(object sender, EventArgs e)
        {
            Debug.Assert(Control != null, "Set property of component");
            Control.IsDirty = true;
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            if (!DesignMode)
            {
                foreach (DictionaryEntry de in properties)
                {
                    TextBoxBase txt = de.Key as TextBoxBase;

                    if (txt != null)
                    {
                        txt.TextChanged += Control_Changed;
                    }
                }
            }
        }

        [Description("This string will be drawn in the center of the panel")]
        [Category("Appearance")]
        public string GetMyString(Panel p)
        {
            return EnsurePropertiesExists(p).MyString;
        }

        public void SetMyString(Panel p, string value)
        {
            EnsurePropertiesExists(p).MyString = value;

            p.Invalidate();
        }

        private bool ShouldSerializeMyString(Panel p)
        {
            if (GetMyString(p) != System.Environment.Version.ToString())
                return true;
            else
                return false;
        }

        private void ResetMyString(Panel p)
        {
            SetMyString(p, System.Environment.Version.ToString());
        }

		#region MyInt
		[DefaultValue(-1)]
		[Category("Something")]
		[Description("This is used by some code somewhere to do something")]
		public int GetMyInt(ButtonBase b)
		{
			return EnsurePropertiesExists(b).MyInt;
		}

		public void SetMyInt(ButtonBase b, int value)
		{
			if( value >= -1 )
				EnsurePropertiesExists(b).MyInt = value;
			else
				throw new ArgumentOutOfRangeException("MyInt", value, "MyInt must be greater than or equal to -1");
		}
		#endregion
	}
}
