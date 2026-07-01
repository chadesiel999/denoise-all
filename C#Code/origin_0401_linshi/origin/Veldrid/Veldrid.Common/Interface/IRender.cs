using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common
{
    public interface IRender:IDisposable
    {
        internal Boolean Skip { get; }
        internal void Draw();
        internal List<BaseVeldridRender> Children { get; }
        public Padding Margin { get; set; }
        public int ZIndex { get; set; }
        public LineRange LineRange { get; internal set; }
        internal Boolean WindowSizeState { get; set; }
        public RectangleF Rectangle { get; }
        internal Vector2 WindowSize { get; }
        internal Camera Camera { get;}
        public Boolean Visibily { get; set; }
        public Object Tag { get; set; }
    }
}
