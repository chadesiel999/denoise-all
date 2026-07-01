using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common
{
    public interface ISeries:IDropRender
    {
        public event EventHandler<float> VerticalOffsetChanged;
        public event EventHandler<float> HorizontalOffsetChanged;
        public float HorizontalOffset { get; set; }
        public Boolean IsDragged { get; set; }
        public float VerticalOffset { get; set; }
        public IReadOnlyList<ICursor> Cursors { get; }
    }
}
