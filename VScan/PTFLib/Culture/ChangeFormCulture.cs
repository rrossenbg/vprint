// File           : ChangeFormCulture.cs
// Original Author: Guy Smith-Ferrier
// Date Created   : September 2005
// Notes          : This file is part of the downloadable source code for .NET Internationalization, by
//                  Guy Smith-Ferrier, published by Addison-Wesley. See http://www.dotneti18n.com for details.
// Disclaimer     : No warranty is provided. Use at your own risk.
// Modification   : You are free to modify this code providing that this block of comments is not altered
//                  and is placed before any code.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace PremierTaxFree.PTFLib.Culture
{
    /// <summary>
    /// ChangeFormCulture provides methods to change the culture 
    /// used by a single or all forms in an application
    /// </summary>
    public class ChangeFormCulture
    {
        /// <summary>
        /// ChangeAllForms changes the culture of all existing forms in the application
        /// </summary>
        /// <param name="culture">The culture name to change the forms to</param>
        /// <example>ChangeFormCulture.ChangeAllForms("es");</example>
        /// <example>ChangeFormCulture.ChangeAllForms("fr");</example>
        /// <example>ChangeFormCulture.ChangeAllForms("en");</example>
        public static void ChangeAllForms(CultureInfo culture)
        {
            FormCollection forms = Application.OpenForms;
            foreach (Form form in forms)
            {
                ChangeForm(form, culture);
            }
        }
        /// <summary>
        /// ChangeForm changes the culture of an existing 
        /// form by forcing a reload of its resources
        /// </summary>
        /// <param name="form">The form for which the culture should be changed</param>
        /// <param name="culture">The culture name to change the form to</param>
        /// <remarks>This method changes the CurrentUICulture to the given culture</remarks>
        public static void ChangeForm(Form form, CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            ComponentResourceManager resourceManager = new ComponentResourceManager(form.GetType());
            // apply resources to each control
            foreach (Control control in form.Controls)
            {
                resourceManager.ApplyResources(control, control.Name);
            }
            // apply resources to the form
            int X = form.Location.X;
            int Y = form.Location.Y;
            resourceManager.ApplyResources(form, "$this");
            form.Location = new Point(X, Y);

            ApplyMenuResources(resourceManager, form);
        }

        private static void ApplyMenuResources(ComponentResourceManager resourceManager, Form form)
        {
            if (form.Menu != null)
            {
                FieldInfo[] fieldInfos = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.FieldType == typeof(System.Windows.Forms.MenuItem))
                    {
                        MenuItem menuItem = (MenuItem)fieldInfo.GetValue(form);
                        resourceManager.ApplyResources(menuItem, fieldInfo.Name);
                    }
                }
            }
        }

        /// <summary>
        /// ChangeFormUsingInitializeComponent changes the 
        /// culture of an existing form by removing all of its
        /// controls and adding them back by calling InitializeComponent
        /// </summary>
        /// <param name="form">The form for which the culture should be changed</param>
        /// <param name="culture">The culture name to change the form to</param>
        /// <remarks>This method changes the CurrentUICulture to the given culture</remarks>
        public static void ChangeFormUsingInitializeComponent(Form form, string culture)
        {
            // get the form's private InitializeComponent method
            MethodInfo initializeComponentMethodInfo = form.GetType().GetMethod(
                "InitializeComponent", BindingFlags.Instance | BindingFlags.NonPublic);

            if (initializeComponentMethodInfo != null)
            {
                // the form has an InitializeComponent method that we can invoke

                // save all controls
                List<Control> controls = new List<Control>();
                foreach (Control control in form.Controls)
                {
                    controls.Add(control);
                }
                // remove all controls
                foreach (Control control in controls)
                {
                    form.Controls.Remove(control);
                }

                int X = form.Location.X;
                int Y = form.Location.Y;

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

                // call the InitializeComponent method to add back controls
                initializeComponentMethodInfo.Invoke(form, new object[] { });

                form.Location = new Point(X, Y);
            }
        }
    }
}