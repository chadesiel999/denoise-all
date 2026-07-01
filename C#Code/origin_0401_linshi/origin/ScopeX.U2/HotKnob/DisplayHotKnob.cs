using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core;

namespace ScopeX.U2
{
    public class DisplayHotKnob : IHotKnob
    {
        private readonly DisplayPrsnt _Prsnt;

        public void Turn(Int32 keyCode, Int32 keyStep)
        {
            switch (keyCode)
            {
                case KeyCode.KNOB_UPMULTI_LEFT:
                    _Prsnt.WfmIntensity -= keyStep;
                    break;
                case KeyCode.KNOB_UPMULTI_RIGHT:
                    _Prsnt.WfmIntensity += keyStep;
                    break;
                case KeyCode.KNOB_UPMULTI_SELECT:
                    _Prsnt.ResetWfmIntensity();
                    break;
            }
        }

        public DisplayHotKnob(DisplayPrsnt p)
        {
            _Prsnt = p;
        }
    }
}
