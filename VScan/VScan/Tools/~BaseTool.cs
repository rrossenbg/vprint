/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;


namespace PremierTaxFree.Tools
{
    public class InfoEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Drawing tool abstraction
    /// </summary>
    public abstract class BaseTool : IDisposable
    {
        protected CanvasControl m_Canvas;

        protected static event EventHandler<EventArgs> Execute;        
        public static event EventHandler<InfoEventArgs> Info;

        public object Tag { get; set; }

        public Point Offset
        {
            get
            {
                return new Point(m_Canvas.HorizontalScroll.Value, m_Canvas.VerticalScroll.Value);
            }
        }

        public BaseTool(CanvasControl canvas)
        {
            m_Canvas = canvas;

            m_Canvas.MouseDown += MouseDown;
            m_Canvas.MouseMove += MouseMove;
            m_Canvas.MouseUp += MouseUp;
            m_Canvas.MouseLeave += MouseLeave;
            m_Canvas.MouseClick += MouseClick;
            m_Canvas.Click += Click;
            m_Canvas.DoubleClick += DoubleClick;
            m_Canvas.Paint += Paint;
            Form frm = m_Canvas.FindForm();
            if (frm != null)
                frm.KeyPress += KeyPress;
            Execute += this.ExecuteEvent;
        }

        ~BaseTool()
        {
            DisposeInternal();
        }

        public void Dispose()
        {
            DisposeInternal();
            GC.SuppressFinalize(this);
        }

        protected void DisposeInternal()
        {
            m_Canvas.MouseDown -= MouseDown;
            m_Canvas.MouseMove -= MouseMove;
            m_Canvas.MouseUp -= MouseUp;
            m_Canvas.MouseLeave -= MouseLeave;
            m_Canvas.MouseClick -= MouseClick;
            m_Canvas.Click -= Click;
            m_Canvas.DoubleClick -= DoubleClick;
            m_Canvas.Paint -= Paint;
            Form frm = m_Canvas.FindForm();
            if (frm != null)
                frm.KeyPress -= KeyPress;
            Execute -= this.ExecuteEvent;
        }

        protected void FireInfo(string name, string value)
        {
            if (Info != null)
                Info(this, new InfoEventArgs { Name = name, Value = value });
        }

        protected virtual void ExecuteEvent(object sender, EventArgs e)
        {
        }

        protected virtual void MouseDown(object sender, MouseEventArgs e)
        {
        }

        protected virtual void MouseMove(object sender, MouseEventArgs e)
        {
        }

        protected virtual void MouseUp(object sender, MouseEventArgs e)
        {
        }

        protected virtual void MouseClick(object sender, MouseEventArgs e)
        {
        }

        protected virtual void MouseLeave(object sender, EventArgs e)
        {
        }

        protected virtual void Click(object sender, EventArgs e)
        {
        }

        protected virtual void DoubleClick(object sender, EventArgs e)
        {
        }

        protected virtual void KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        protected virtual void Paint(object sender, PaintEventArgs e)
        {

        }

        public static void Install(BaseTool tool)
        {
            if (tool == tool.m_Canvas.Tool)
                return;

            if (tool.m_Canvas.Tool != null)
                tool.m_Canvas.Tool.Dispose();
            tool.m_Canvas.Tool = tool;
        }

        public static void Reset(CanvasControl canvas)
        {
            Install(new EmptyTool(canvas));
        }

        public static BaseTool Current(CanvasControl cnt)
        {
            return cnt.Tool;
        }

        public static void FireExecute(CanvasControl canvas, object sender, EventArgs e)
        {
            foreach (Delegate del in Execute.GetInvocationList())
            {
                //Fire the event only in case the tool is attached to the same canvas
                if (object.ReferenceEquals(((BaseTool)del.Target).m_Canvas, canvas))
                    del.DynamicInvoke(sender, e);
            }
        }

        public static void SetTag(CanvasControl canvas, object value)
        {
            if (canvas.Tool != null)
                canvas.Tool.Tag = value;
        }
    }
}
