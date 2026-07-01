using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Veldrid.Common.Plot
{
    public abstract class BaseDropRender : BaseRender, IDropRender
    {
        public event EventHandler<PointF> MouseDown;
        public event EventHandler<PointF> MouseUp;
        public event EventHandler<PointF> Dragged;
        public event EventHandler<IDropRender> SelectionChanged;
        public event EventHandler MouseLeave;
        public event EventHandler<PointF> RightMouseDown;
        public event EventHandler<PointF> DoubleClick;

        private protected bool selected;
        protected BaseDropRender(IVeldridContent control) : base(control)
        {
        }
        protected virtual void ActiveRightMouseDown(object sender, PointF point) => RightMouseDown?.Invoke(sender, point);
        protected virtual void ActiveDoubleClick(object sender, PointF point) => DoubleClick?.Invoke(sender, point);
        protected virtual void ActiveMouseDown(object sender, PointF point) => MouseDown?.Invoke(sender, point);
        protected virtual void ActiveMouseUp(object sender, PointF point) => MouseUp?.Invoke(sender, point);
        protected virtual void ActiveDragged(object sender, PointF point) => Dragged?.Invoke(sender, point);
        protected virtual void ActiveSelectionChanged(object sender, IDropRender render) => SelectionChanged?.Invoke(sender, render);
        internal virtual void OnMouseMove(PointF point, ref bool handle)
        {

        }

        internal virtual void OnRightMouseDown(PointF point, ref bool handle)
        {
        }

        internal virtual void OnDoubleClick(PointF point, ref bool handle)
        {
        }

        internal virtual void OnMouseDown(PointF point, ref bool handle)
        {

        }
        internal virtual void OnMouseUp(PointF point, ref bool handle)
        {

        }
        internal virtual void OnMouseLeave(ref bool handle)
        {

        }
        bool IDropRender.Selected { get => selected; set => selected =value; }
        void IDropRender.OnDragged(PointF position)=>this.ActiveDragged(this,position);


        void IDropRender.OnMouseDown(PointF point, ref bool handle)=>this.OnMouseDown(point, ref handle);
        void IDropRender.OnRightMouseDown(PointF point, ref bool handle) => this.OnRightMouseDown(point, ref handle);
        void IDropRender.OnDoubleClick(PointF point, ref bool handle) => this.OnDoubleClick(point, ref handle);

        void IDropRender.OnMouseMove(PointF point, ref bool handle)=>this.OnMouseMove(point, ref handle);
        void IDropRender.OnMouseUp(PointF point, ref bool handle)=>this.OnMouseUp(point, ref handle);
        void IDropRender.OnMouseLeave(ref bool handle)=>this.OnMouseLeave(ref handle);
    }
}
