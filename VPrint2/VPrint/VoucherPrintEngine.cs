/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace VPrinting
{
    public class VoucherPrintEngine : PrintDocument
    {
        public object DataObject { get; set; }

        public AllocationDocumentLayout DocumentLayout { get; set; }

        public VoucherPrintEngine(AllocationDocumentLayout layout)
        {
            DocumentLayout = layout;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            Debug.Assert(DataObject != null);
            Debug.Assert(DocumentLayout != null);
            Debug.Assert(DocumentLayout.MetaObjectsList != null);

            RectangleF rect = e.Graphics.VisibleClipBounds;

            Type type = DataObject.GetType();
            var properties = type.GetProperties();

            lock (((ICollection)DocumentLayout.MetaObjectsList).SyncRoot)
            {
                foreach (var metaObj in DocumentLayout.MetaObjectsList)
                {
                    try
                    {
                        Debug.Assert(metaObj != null);

                        if (!string.IsNullOrEmpty(metaObj.BoundColumn))
                        {
                            var prop = properties.SingleOrDefault((p) => string.Equals(p.Name, metaObj.BoundColumn, 
                                StringComparison.InvariantCultureIgnoreCase));
                            if (prop == null)
                                throw new ApplicationException("Data property not found.");

                            object value = prop.GetValue(DataObject, null);
                            metaObj.Text = Convert.ToString(value);
                            metaObj.Draw(e.Graphics, Point.Empty, DrawingSurface.Printer);
                        }
                        else
                        {
                            //No databinding
                            //Just label
                            metaObj.Draw(e.Graphics, Point.Empty, DrawingSurface.Printer);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Print stopped", ex);
                    }
                }
            }
            base.OnPrintPage(e);
        }
    }
}
