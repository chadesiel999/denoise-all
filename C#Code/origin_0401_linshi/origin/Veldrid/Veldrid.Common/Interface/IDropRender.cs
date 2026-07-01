using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common
{
    public interface IDropRender:IRender
    {
        public event EventHandler<PointF> MouseDown;
        public event EventHandler<PointF> RightMouseDown;
        public event EventHandler<PointF> DoubleClick;
        public event EventHandler<PointF> MouseUp;
        public event EventHandler<PointF> Dragged;
        public event EventHandler MouseLeave;
        public event EventHandler<IDropRender> SelectionChanged;
        internal Boolean Selected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handle">指示事件是否已被处理</param>
        internal void OnMouseMove(PointF point, ref bool handle);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handle">指示事件是否已被处理</param>
        internal void OnMouseDown(PointF point, ref bool handle);

        internal void OnRightMouseDown(PointF point, ref bool handle);

        internal void OnDoubleClick(PointF point, ref bool handle);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handle">指示事件是否已被处理</param>
        internal void OnMouseUp(PointF point, ref bool handle);
        internal void OnMouseLeave(ref bool handle);
        internal void OnDragged(PointF position);
    }
}
