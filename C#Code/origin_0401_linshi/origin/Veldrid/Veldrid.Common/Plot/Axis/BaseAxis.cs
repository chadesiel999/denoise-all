using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.Common.Plot
{
    public abstract class BaseAxis : BaseRender, IAxis
    {
        protected BaseAxis(IVeldridContent control) : base(control)
        {
        }
    }
}
