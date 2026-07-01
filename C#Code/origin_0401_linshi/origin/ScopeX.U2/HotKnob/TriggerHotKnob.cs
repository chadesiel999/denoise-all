using System;
using ScopeX.Core;

namespace ScopeX.U2
{
    public sealed class TriggerHotKnob : IHotKnob
    {
        private Int32 _FuncIndex = 0;

        private TriggerPrsnt Prsnt
        {
            get;
        }

        private void TurnHoldoff(Int32 keyCode, Int32 keyStep, Action OnPressKnob)
        {
            switch (keyCode)
            {
                case KeyCode.KNOB_UPMULTI_LEFT:
                    TriggerPrsnt.AdjHoldoff(-keyStep);
                    break;
                case KeyCode.KNOB_UPMULTI_RIGHT:
                    TriggerPrsnt.AdjHoldoff(keyStep);
                    break;
                case KeyCode.KNOB_UPMULTI_SELECT:
                    //TriggerPrsnt.HoldoffByps = TriggerPrsnt.MinHoldoff;
                    OnPressKnob();
                    break;
            }
        }

        private void TurnPulseWidth(TrigWidthPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjWidth(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjWidth(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;
                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnPatDuration(TrigPatPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjDuration(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjDuration(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnRuntWidth(TrigRuntPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjWidth(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjWidth(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;
                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnTimeout(TrigTimeOutPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjDuration(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjDuration(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnTransWidth(TrigTransPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjWidth(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjWidth(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnVideo(TrigVideoPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.Line -= (Int16)keyStep;
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.Line += (Int16)keyStep;
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else if (_FuncIndex == 1)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.Field -= (Int16)keyStep;
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.Field += (Int16)keyStep;
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 2;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnWindow(TrigWindowPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjWidth(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjWidth(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        private void TurnSetupHold(TrigSetupHoldPrsnt prsnt, Int32 keyCode, Int32 keyStep)
        {
            if (_FuncIndex == 0)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjThd(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjThd(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 1;
                        break;

                }
            }
            else if (_FuncIndex == 1)
            {
                switch (keyCode)
                {
                    case KeyCode.KNOB_UPMULTI_LEFT:
                        prsnt.AdjTsu(-keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_RIGHT:
                        prsnt.AdjTsu(keyStep);
                        break;
                    case KeyCode.KNOB_UPMULTI_SELECT:
                        _FuncIndex = 2;
                        break;

                }
            }
            else
            {
                TurnHoldoff(keyCode, keyStep, () => _FuncIndex = 0);
            }
        }

        public void Turn(Int32 keyCode, Int32 keyStep)
        {
            switch (Prsnt)
            {
                case TrigWidthPrsnt tpwp:
                    TurnPulseWidth(tpwp, keyCode, keyStep);
                    break;
                case TrigPatPrsnt tsp:
                    TurnPatDuration(tsp, keyCode, keyStep);
                    break;
                case TrigRuntPrsnt trp:
                    TurnRuntWidth(trp, keyCode, keyStep);
                    break;
                case TrigTimeOutPrsnt ttop:
                    TurnTimeout(ttop, keyCode, keyStep);
                    break;
                case TrigTransPrsnt ttp:
                    TurnTransWidth(ttp, keyCode, keyStep);
                    break;
                case TrigVideoPrsnt tvp:
                    TurnVideo(tvp, keyCode, keyStep);
                    break;
                case TrigWindowPrsnt twp:
                    TurnWindow(twp, keyCode, keyStep);
                    break;
                case TrigSetupHoldPrsnt tshp:
                    TurnSetupHold(tshp, keyCode, keyStep);
                    break;
                default:
                    TurnHoldoff(keyCode, keyStep, () =>
                    {
                        if (TriggerPrsnt.HoldoffType == ComModel.DelayOpt.Time)
                        {
                            TriggerPrsnt.HoldoffByps = TriggerPrsnt.MinHoldoffTime;
                        }
                        else
                        {
                            TriggerPrsnt.HoldoffByCnt = TriggerPrsnt.MinHoldoffCnt;
                        }
                    });

                    break;
            }
        }
    
        public TriggerHotKnob(TriggerPrsnt p)
        {
            Prsnt = p;
        }
    }
}
