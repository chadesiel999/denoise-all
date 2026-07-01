using System;

namespace ScopeX.Core
{
    public interface IHotKnob
    {
        void Turn(Int32 keyCode, Int32 keyStep);
    }
}
