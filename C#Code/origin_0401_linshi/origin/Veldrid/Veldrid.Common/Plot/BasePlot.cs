using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.Common.Plot
{
    public abstract class BasePlot : BaseSeries
    {
        private bool isFocused;
        protected BasePlot(IVeldridContent control) : base(control)
        {
        }
        public virtual System.String Label { get; set; } = String.Empty;
        public virtual float Brightness { get; set; }
        public virtual bool LabelVisibility { get; set; } = true;
        public virtual System.Boolean IsFocused { get; set; }
        public virtual LineStyle LineStyle { get; set; }
        public virtual String FontName { get; set; } = "Arila";
        public virtual String FontStyle { get; set; } = "Regular";
        public virtual float FontSize { get; set; } = 12;
    }
}
