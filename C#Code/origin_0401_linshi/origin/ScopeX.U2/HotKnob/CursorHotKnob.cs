using ScopeX.Core;
using System;
using System.Collections.Generic;

namespace ScopeX.U2
{
    public class CursorHotKnob : IHotKnob
    {
        private readonly CursorPrsnt _Prsnt;
        private CursorBarPrsnt _CurrentCursor;
        private KeySpeed<Int32> _KeySpeed;
        private readonly IReadOnlyList<Int32> _SupportSpeedUpKey = new List<Int32>()
        {
            KeyCode.KNOB_MULTI_LEFT,
            KeyCode.KNOB_MULTI_RIGHT,
            KeyCode.KNOB_UPMULTI_LEFT,
            KeyCode.KNOB_UPMULTI_RIGHT,
            KeyCode.KNOB_DNMULTI_LEFT,
            KeyCode.KNOB_DNMULTI_RIGHT,
        };

        private Int32 _Count = 0;
        public void Turn(Int32 keyCode, Int32 keyStep)
        {
            if (!_Prsnt.Active)//光标未选中
            {
                return;
            }

            if (_Prsnt.Type != ComModel.CursorType.HorizontalVertical)
            {
                _CurrentCursor = _Prsnt.Current;
            }

            if (_CurrentCursor.SelectedIndex == -1 && keyCode != KeyCode.KNOB_MULTI_SELECT)//光标未选中
            {
                return;
            }

            Int32 step = _Prsnt.MoveMode == ComModel.CursorMoveMode.Fast ? _KeySpeed.GetStep(keyCode, Math.Abs(keyStep)) : 1;
            switch (keyCode)
            {
                #region 单多功能旋钮

                case KeyCode.KNOB_MULTI_LEFT:

                    _CurrentCursor.Move(-step * CursorApp.Default.CurrentPiexlPerPosindex, _CurrentCursor.SelectedIndex, _CurrentCursor.SelectedIndex != 0 && _Prsnt.IsSyncMove);
                    if (_CurrentCursor.SelectedIndex == 0)//A 线始终更新
                    {
                        CursorApp.Default.IsUpdateHoriDistance = true;
                        CursorApp.Default.IsUpdateVertDistance = true;
                        CursorApp.Default.IsZoomUpdateVertDistance = true;
                        CursorApp.Default.IsZoomUpdateHoriDistance = true;
                    }
                    else if (_CurrentCursor.SelectedIndex == 1)//B 线非同步移动时更新
                    {
                        CursorApp.Default.IsUpdateHoriDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsUpdateVertDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsZoomUpdateVertDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsZoomUpdateHoriDistance = !_Prsnt.IsSyncMove;
                    }
                    break;
                case KeyCode.KNOB_MULTI_RIGHT:

                    _CurrentCursor.Move(step * CursorApp.Default.CurrentPiexlPerPosindex, _CurrentCursor.SelectedIndex, _CurrentCursor.SelectedIndex != 0 && _Prsnt.IsSyncMove);
                    if (_CurrentCursor.SelectedIndex == 0)//A 线始终更新
                    {
                        CursorApp.Default.IsUpdateHoriDistance = true;
                        CursorApp.Default.IsUpdateVertDistance = true;
                        CursorApp.Default.IsZoomUpdateVertDistance = true;
                        CursorApp.Default.IsZoomUpdateHoriDistance = true;
                    }
                    else if (_CurrentCursor.SelectedIndex == 1)//B 线非同步移动时更新
                    {
                        CursorApp.Default.IsUpdateHoriDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsUpdateVertDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsZoomUpdateVertDistance = !_Prsnt.IsSyncMove;
                        CursorApp.Default.IsZoomUpdateHoriDistance = !_Prsnt.IsSyncMove;
                    }
                    break;
                case KeyCode.KNOB_MULTI_SELECT:

                    if (_Prsnt.Type != ComModel.CursorType.HorizontalVertical)
                    {
                        _CurrentCursor.SelectedIndex = _CurrentCursor.SelectedIndex == -1 ? 0 : _CurrentCursor.SelectedIndex ^ 1;
                    }
                    else
                    {
                        _Count++;
                        _CurrentCursor = _Count >= 2 ? _Prsnt.HCursor : _Prsnt.VCursor;
                        if (_Count >= 2)
                        {
                            _Prsnt.VCursor.SelectedIndex = -1;
                        }
                        else
                        {
                            _Prsnt.HCursor.SelectedIndex = -1;
                        }
                        _CurrentCursor.SelectedIndex = _Count % 2;
                        _Count = _Count >= 3 ? -1 : _Count;
                    }
                    break;

                #endregion 单多功能旋钮

                #region 双多功能旋钮

                case KeyCode.KNOB_UPMULTI_LEFT:
                    _CurrentCursor[0] += -step;
                    break;
                case KeyCode.KNOB_UPMULTI_RIGHT:

                    _CurrentCursor[0] += step;
                    break;
                case KeyCode.KNOB_UPMULTI_SELECT:

                    break;
                case KeyCode.KNOB_DNMULTI_LEFT:
                    _CurrentCursor[1] += -step;
                    if (_Prsnt.IsSyncMove)
                    {
                        _CurrentCursor[0] += -step;
                    }
                    break;
                case KeyCode.KNOB_DNMULTI_RIGHT:
                    _CurrentCursor[1] += step;
                    if (_Prsnt.IsSyncMove)
                    {
                        _CurrentCursor[0] += step;
                    }
                    break;
                case KeyCode.KNOB_DNMULTI_SELECT:
                    break;

                    #endregion 双多功能旋钮
            }
            if (_Prsnt.Type != ComModel.CursorType.HorizontalVertical)
            {
                _CurrentCursor = _Prsnt.VCursor;
            }
        }

        public CursorHotKnob(CursorPrsnt p)
        {
            _Prsnt = p;
            _CurrentCursor = p.Type == ComModel.CursorType.HorizontalVertical ? p.VCursor : p.Current;
            _KeySpeed = new KeySpeed<Int32>(_SupportSpeedUpKey, 500, 1, 0.15);
        }

    }
}
