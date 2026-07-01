using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Text;
using Veldrid.Common.Plot.Plot;

namespace Veldrid.Common.Plot
{
    public abstract class BaseSeries : BaseDropRender, ISeries
    {
        private protected List<ICursor> cursors = new List<ICursor>();
        public event EventHandler<float> VerticalOffsetChanged;
        public event EventHandler<float> HorizontalOffsetChanged;
        private int selectindex = -1;
        public LabelPlot LabelPlot { get; set; }

        public BaseSeries(IVeldridContent control) : base(control)
        {
        }

        public virtual float HorizontalOffset { get; set; }
        public virtual float VerticalOffset { get; set; }
        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            base.OnMouseDown(point, ref handle);
            if (!handle)
            {
                for (int i = 0; i < cursors.Count; i++)
                {
                    cursors[i]?.OnMouseDown(point, ref handle);
                    if (handle)
                    {
                        selectindex = i;
                        break;
                    }
                }
                
              
            }
            if (!handle)
            {
                selectindex = -1;
                LabelPlot?.OnMouseDown(point, ref handle);
            }
        }
        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            base.OnMouseMove(point, ref handle);
            if (!handle)
            {
                foreach (var cursor in cursors)
                {
                    cursor?.OnMouseMove(point, ref handle);
                    if (handle) return;
                }

                LabelPlot?.OnMouseMove(point, ref handle);
            }
        }
        internal override void OnMouseUp(PointF point, ref bool handle)
        {
            base.OnMouseUp(point, ref handle);
            selectindex = -1;
            selected = false;
            IsDragged = false;
            if (!handle)
            {
                foreach (var cursor in cursors)
                {
                    cursor?.OnMouseUp(point, ref handle);
                    if (handle) return;
                }
                LabelPlot?.OnMouseUp(point, ref handle);
            }
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            LabelPlot?.OnDragged(sender, point);
            if (selectindex >= 0 && selectindex < cursors.Count)
            {
                cursors[selectindex].OnDragged(point);
            }
            else base.ActiveDragged(sender, point);
        }

        public IReadOnlyList<ICursor> Cursors => cursors.AsReadOnly();

        public bool IsDragged { get; set; }

        protected void OnVerticalOffsetChanged(float e) => VerticalOffsetChanged?.Invoke(this, e);
        protected void OnHorizontalOffsetChanged(float e) => HorizontalOffsetChanged?.Invoke(this, e);
        public override void Draw()
        {
            base.Draw();
            // cursors.ForEach(x => x?.Draw());
            LabelPlot?.Draw();
        }
        protected override void SetWindowSizeState(bool state)
        {
            base.SetWindowSizeState(state);
            cursors.ForEach(x => x.WindowSizeState = state);
        }
        protected override void Dispose(bool disposing)
        {
            cursors?.ForEach(x => x?.Dispose());
            cursors?.Clear();
            LabelPlot?.Dispose();
            base.Dispose(disposing);
        }
    }
}
