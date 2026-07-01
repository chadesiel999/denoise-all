using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.Common.Plot
{
    public abstract class BaseCursor : BaseDropRender, ICursor
    {
        protected BaseCursor(IVeldridContent control) : base(control)
        {
        }
    }
}
