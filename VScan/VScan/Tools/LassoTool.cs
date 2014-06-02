/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Tools
{
    /// <summary>
    /// Lasso tool class. Creates selecting lasso on image
    /// </summary>
    public class LassoTool : BaseTool
    {
        private enum EditorMode
        {
            None,
            Lasso, //Cut, Select
            DragDrop,
            Edit, //Resize, Rotate,
        };

        private TestType m_LastTest;

        private Point m_StartPoint, m_CurrentPoint;
        private float m_Angle = float.NaN;

        public LassoTool(CanvasControl canvas)
            : base(canvas)
        {
        }

        private EditorMode m_EditorMode;

        protected override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_StartPoint = m_CurrentPoint = Control.MousePosition;

                if (Control.ModifierKeys != Keys.Control)
                    m_Canvas.m_SelectedImgList.Clear();

                m_EditorMode = EditorMode.Lasso;

                foreach (var obj in m_Canvas.m_ImgList)
                {
                    m_LastTest = obj.Test(e.Location);
                    if (m_LastTest != TestType.None)
                    {
                        m_Canvas.m_SelectedImgList.Add(obj);
                        m_EditorMode = EditorMode.Edit;
                        return;
                    }
                    else if (Control.ModifierKeys != Keys.Control)
                    {
                        obj.Selection = SelectionType.None;
                    }
                }

                foreach (var obj in m_Canvas.m_ImgList)
                {
                    if (obj.Rect.Contains(e.Location))
                    {
                        m_Canvas.m_SelectedImgList.Add(obj);
                        m_EditorMode = EditorMode.DragDrop;
                        return;
                    }
                }
            }
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            if (m_EditorMode == EditorMode.Lasso)
            {
                ControlPaint.DrawReversibleFrame(
                    Rectangle.FromLTRB(m_StartPoint.X, m_StartPoint.Y, m_CurrentPoint.X, m_CurrentPoint.Y),
                    SystemColors.ActiveBorder,
                    FrameStyle.Dashed);

                m_CurrentPoint = Control.MousePosition;

                ControlPaint.DrawReversibleFrame(
                    Rectangle.FromLTRB(m_StartPoint.X, m_StartPoint.Y, m_CurrentPoint.X, m_CurrentPoint.Y),
                    SystemColors.ActiveBorder,
                    FrameStyle.Dashed);
            }
            else if (m_EditorMode == EditorMode.DragDrop && !m_Canvas.m_SelectedImgList.IsEmpty())
            {
                foreach (var obj in m_Canvas.m_SelectedImgList)
                {
                    Point point = m_Canvas.PointToClient(m_StartPoint);
                    Size size = new Size(obj.Rect.Location);
                    obj.Offset = Point.Subtract(point, size);
                    obj.Resize(new Rectangle(Point.Empty, obj.Rect.Size));
                    obj.Selection = SelectionType.Resize;
                }

                m_Canvas.Cursor.SetOffset(m_Canvas.m_SelectedImgList.First().Offset.Invert());

                IDataObject data = new DataObject();
                data.SetData(m_Canvas.m_SelectedImgList);

                m_Canvas.DoDragDrop(data, DragDropEffects.All);
                m_Canvas.DeleteSelectedImages();
                m_Canvas.Invalidate();
            }
            else if (m_EditorMode == EditorMode.Edit)
            {
                foreach (var obj in m_Canvas.m_SelectedImgList)
                {
                    switch (obj.Selection)
                    {
                        case SelectionType.Resize:
                            {
                                Point sub = Point.Subtract(Control.MousePosition, new Size(m_CurrentPoint));
                                Rectangle rect = obj.Rect;
                                switch (m_LastTest)
                                {
                                    case TestType.LeftTop:
                                        rect.Inflate(-sub.X, -sub.Y);
                                        break;
                                    case TestType.RightTop:
                                        rect.Inflate(sub.X, -sub.Y);
                                        break;
                                    case TestType.RightBottom:
                                        rect.Inflate(sub.X, sub.Y);
                                        break;
                                    case TestType.LeftBottom:
                                        rect.Inflate(-sub.X, sub.Y);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }

                                obj.Resize(rect);
                            }
                            break;
                        case SelectionType.Rotate:
                            {
                                PointF center = obj.Rect.Center();
                                Point p = m_Canvas.PointToClient(Control.MousePosition);
                                float angle = center.ToAngle(p.Cast());

                                if (float.IsNaN(m_Angle))
                                {
                                    m_Angle = angle;
                                }
                                else
                                {
                                    Debug.WriteLine(angle, " Angle ");
                                    obj.Rotate(angle - m_Angle);
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    m_Canvas.Invalidate(obj.Rect.InflateEx());
                }

                m_CurrentPoint = Control.MousePosition;
            }
            else
            {
                bool inscope = false;
                Point point = m_Canvas.PointToClient(Control.MousePosition);

                foreach (var obj in m_Canvas.m_ImgList)
                {
                    m_LastTest = obj.Test(point);

                    if (m_LastTest != TestType.None)
                    {
                        inscope = true;
                        Cursor.Current = Cursors.Cross;
                        break;
                    }
                }
                if (!inscope)
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        protected override void MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (m_EditorMode == EditorMode.Lasso)
                {
                    if (m_Canvas.BackgroundImage != null)
                    {
                        Rectangle rec = m_Canvas.RectangleToClient(
                            Rectangle.FromLTRB(m_StartPoint.X , //- m_Canvas.HorizontalScroll.Value,
                                                m_StartPoint.Y , //- m_Canvas.VerticalScroll.Value,
                               m_CurrentPoint.X, m_CurrentPoint.Y));

                        if (rec.Width > 0 && rec.Height > 0)
                        {
                            ImageObj obj = new ImageObj(m_Canvas.BackgroundImage as Bitmap, rec);
                            m_Canvas.m_ImgList.Add(obj);
                            return;
                        }
                    }
                }
                else if (m_EditorMode == EditorMode.Edit)
                {
                    foreach (var obj in m_Canvas.m_SelectedImgList)
                    {
                        switch (obj.Selection)
                        {
                            case SelectionType.Resize:
                                m_Canvas.Invalidate(obj.Rect.InflateEx());
                                return;
                            case SelectionType.Rotate:
                                m_Angle = float.NaN;
                                break;
                        }
                    }
                }
                m_Canvas.Invalidate();
            }
            finally
            {
                m_EditorMode = EditorMode.None;
            }
        }

        protected override void MouseLeave(object sender, EventArgs e)
        {
            if (m_EditorMode == EditorMode.DragDrop)
            {
                //m_UndoRedoManager.Execute(new RemoveCommand());
            }
            else if (m_EditorMode == EditorMode.Edit)
            {
                foreach (var obj in m_Canvas.m_SelectedImgList)
                {
                    switch (obj.Selection)
                    {
                        case SelectionType.Resize:
                            m_Canvas.Invalidate(obj.Rect);
                            break;
                        case SelectionType.Rotate:
                            m_Angle = float.NaN;
                            break;
                    }
                }
            }

            m_EditorMode = EditorMode.None;
        }

        protected override void Click(object sender, EventArgs e)
        {
            if (m_EditorMode != EditorMode.Edit)
            {
                if (Control.ModifierKeys != Keys.Control)
                    m_Canvas.m_SelectedImgList.Clear();

                Point point = m_Canvas.PointToClient(Control.MousePosition);

                foreach (var obj in m_Canvas.m_ImgList)
                {
                    if (obj.Click(point).ToBool())
                    {
                        m_Canvas.m_SelectedImgList.Add(obj);
                        m_Canvas.Invalidate(obj.Rect.InflateEx());
                    }
                    else if (Control.ModifierKeys != Keys.Control)
                    {
                        obj.Selection = SelectionType.None;
                        m_Canvas.Invalidate(obj.Rect.InflateEx());
                    }
                }
            }
        }
    }    
}
