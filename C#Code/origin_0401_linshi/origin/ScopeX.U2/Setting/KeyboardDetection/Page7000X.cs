// Copyright (c) ScopeX. All Rights Reserved
// <author>Zhang XuLin</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using ScopeX.ComModel;
    using ScopeX.Core;
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="Page7000X" />.
    /// </summary>
    public partial class Page7000X : UserControl, IkeyBoardDetetionView//, IStylize//ISystemCheckView
    {
        //[Browsable(false)]
        //public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        //[Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        //public Boolean StylizeFlag { get; set; } = false;

        private Color _DefaultColor = Color.DarkGray;
        private Color _KeyPressedColor = Color.LimeGreen;//Color.Lime;
        private Color _UnenableColor = Color.Gray;
        private Int32 _LeftAngle = 90;
        private Int32 _RightAngle = -90;
        private Int32 _MinValue = 0;
        private Int32 _MidValue = 50;
        private Int32 _MaxValue = 100;
        private ConcurrentDictionary<Int32, Boolean> _KeyState = new ConcurrentDictionary<Int32, Boolean>();
        private Boolean _ArgToCtrl = false;

        public Page7000X()
        {
            InitializeComponent();
            InitControl();
        }

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public SystemCheckPrsnt Presenter
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        public override void Refresh()
        {
            base.Refresh();
        }
        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.KeyCheckCode):
                    SetControlColor();
                    break;
                case nameof(Presenter.ExitCount):
                    //UpdateExitCount();
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;

        }

        private void InitControl()
        {
            BtnRunStop.BackColor = _DefaultColor;
            BtnAutoset.BackColor = _UnenableColor;
            BtnSingle.BackColor = _DefaultColor;
            BtnNormal.BackColor = _DefaultColor;
            BtnAuto.BackColor = _DefaultColor;

            BtnMenu.BackColor = _UnenableColor;
            BtnForce.BackColor = _UnenableColor;
            BtnCursors.BackColor = _DefaultColor;
            BtnUltraAcq.BackColor = _DefaultColor;
            BtnMeas.BackColor = _DefaultColor;
            BtnQuickMeas.BackColor = _DefaultColor;
            BtnTouchLock.BackColor = _DefaultColor;
            BtnUtility.BackColor = _UnenableColor;
            BtnDefault.BackColor = _UnenableColor;
            BtnAPP.BackColor = _UnenableColor;
            BtnScreenshot.BackColor = _UnenableColor;
            BtnClear.BackColor = _UnenableColor;
            BtnGen.BackColor = _DefaultColor;
            BtnDVM.BackColor = _DefaultColor;
            BtnMath.BackColor = _DefaultColor;
            BtnRef.BackColor = _DefaultColor;
            BtnDigital.BackColor = _DefaultColor;
            BtnBus.BackColor = _DefaultColor;
            BtnC1.BackColor = _DefaultColor;
            BtnC2.BackColor = _DefaultColor;
            BtnC3.BackColor = _DefaultColor;
            BtnC4.BackColor = _DefaultColor;
            CircLevel.GroundBackColor = _DefaultColor;
            CircFunction.GroundBackColor = _DefaultColor;
            CircHScale.GroundBackColor = _DefaultColor;
            CircHPosition.GroundBackColor = _DefaultColor;
            CircVScale.GroundBackColor = _DefaultColor;
            CircVPosition.GroundBackColor = _DefaultColor;
        }

        private void SetControlColor()
        {
            if (!_KeyState.ContainsKey(Presenter.KeyCheckCode))
            {
                _KeyState[Presenter.KeyCheckCode] = false;
            }
            _KeyState[Presenter.KeyCheckCode] = !_KeyState[Presenter.KeyCheckCode];
            switch (Presenter.KeyCheckCode)
            {
                case KeyCode.RUNSTOP:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedRunStop, _KeyState[KeyCode.RUNSTOP]);
                    if (_KeyState[KeyCode.RUNSTOP])
                    {
                        BtnRunStop.BackColor = _KeyPressedColor;
                        BtnRunStop.MouseinBackColor = _KeyPressedColor;
                        BtnRunStop.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnRunStop.BackColor = _DefaultColor;
                        BtnRunStop.MouseinBackColor = _DefaultColor;
                        BtnRunStop.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.AUTOSET:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedAutoset, _KeyState[KeyCode.AUTOSET]);
                    if (_KeyState[KeyCode.AUTOSET])
                    {
                        BtnAutoset.BackColor = _KeyPressedColor;
                        BtnAutoset.MouseinBackColor = _KeyPressedColor;
                        BtnAutoset.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnAutoset.BackColor = _UnenableColor;
                        BtnAutoset.MouseinBackColor = _UnenableColor;
                        BtnAutoset.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.SINGLE:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedSingle, _KeyState[KeyCode.SINGLE]);
                    if (_KeyState[KeyCode.SINGLE])
                    {
                        BtnSingle.BackColor = _KeyPressedColor;
                        BtnSingle.MouseinBackColor = _KeyPressedColor;
                        BtnSingle.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnSingle.BackColor = _DefaultColor;
                        BtnSingle.MouseinBackColor = _DefaultColor;
                        BtnSingle.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.NORMAL:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedNomal, _KeyState[KeyCode.NORMAL]);
                    if (_KeyState[KeyCode.NORMAL])
                    {
                        BtnNormal.BackColor = _KeyPressedColor;
                        BtnNormal.MouseinBackColor = _KeyPressedColor;
                        BtnNormal.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnNormal.BackColor = _DefaultColor;
                        BtnNormal.MouseinBackColor = _DefaultColor;
                        BtnNormal.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.AUTO:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedAuto, _KeyState[KeyCode.AUTO]);
                    if (_KeyState[KeyCode.AUTO])
                    {
                        BtnAuto.BackColor = _KeyPressedColor;
                        BtnAuto.MouseinBackColor = _KeyPressedColor;
                        BtnAuto.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnAuto.BackColor = _DefaultColor;
                        BtnAuto.MouseinBackColor = _DefaultColor;
                        BtnAuto.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.TRIGGER:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedMenu, _KeyState[KeyCode.TRIGGER]);
                    if (_KeyState[KeyCode.TRIGGER])
                    {
                        BtnMenu.BackColor = _KeyPressedColor;
                        BtnMenu.MouseinBackColor = _KeyPressedColor;
                        BtnMenu.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnMenu.BackColor = _UnenableColor;
                        BtnMenu.MouseinBackColor = _UnenableColor;
                        BtnMenu.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.TRIG_FORCE:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedForce, _KeyState[KeyCode.TRIG_FORCE]);
                    if (_KeyState[KeyCode.TRIG_FORCE])
                    {
                        BtnForce.BackColor = _KeyPressedColor;
                        BtnForce.MouseinBackColor = _KeyPressedColor;
                        BtnForce.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnForce.BackColor = _UnenableColor;
                        BtnForce.MouseinBackColor = _UnenableColor;
                        BtnForce.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_LEFT:
                    if (CircLevel.Progress == _MinValue)
                    {
                        CircLevel.Progress = _MidValue;
                        CircLevel.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircLevel.StartAngle == _LeftAngle)
                        {
                            CircLevel.Progress = _MinValue;
                        }
                        else
                        {
                            CircLevel.StartAngle = _LeftAngle;
                            CircLevel.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_RIGHT:
                    if (CircLevel.Progress == _MinValue)
                    {
                        CircLevel.Progress = _MidValue;
                        CircLevel.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircLevel.StartAngle == _RightAngle)
                        {
                            CircLevel.Progress = _MinValue;
                        }
                        else
                        {
                            CircLevel.StartAngle = _RightAngle;
                            CircLevel.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_SELECT:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedTriggerLevel, _KeyState[KeyCode.KNOB_TRIG_YPOS_SELECT]);
                    if (CircLevel.GroundBackColor == _DefaultColor)
                    {
                        CircLevel.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircLevel.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.CURSOR:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedCursor, _KeyState[KeyCode.CURSOR]);
                    if (_KeyState[KeyCode.CURSOR])
                    {
                        BtnCursors.BackColor = _KeyPressedColor;
                        BtnCursors.MouseinBackColor = _KeyPressedColor;
                        BtnCursors.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnCursors.BackColor = _DefaultColor;
                        BtnCursors.MouseinBackColor = _DefaultColor;
                        BtnCursors.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.MEASURE:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedMeasure, _KeyState[KeyCode.MEASURE]);
                    if (_KeyState[KeyCode.MEASURE])
                    {
                        BtnMeas.BackColor = _KeyPressedColor;
                        BtnMeas.MouseinBackColor = _KeyPressedColor;
                        BtnMeas.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnMeas.BackColor = _DefaultColor;
                        BtnMeas.MouseinBackColor = _DefaultColor;
                        BtnMeas.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.VK_SNAPSHOT:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedQuickMeasure, _KeyState[KeyCode.VK_SNAPSHOT]);
                    if (_KeyState[KeyCode.VK_SNAPSHOT])
                    {
                        BtnQuickMeas.BackColor = _KeyPressedColor;
                        BtnQuickMeas.MouseinBackColor = _KeyPressedColor;
                        BtnQuickMeas.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnQuickMeas.BackColor = _DefaultColor;
                        BtnQuickMeas.MouseinBackColor = _DefaultColor;
                        BtnQuickMeas.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.FASTACQ:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedUltraAcq, _KeyState[KeyCode.FASTACQ]);
                    if (_KeyState[KeyCode.FASTACQ])
                    {
                        BtnUltraAcq.BackColor = _KeyPressedColor;
                        BtnUltraAcq.MouseinBackColor = _KeyPressedColor;
                        BtnUltraAcq.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnUltraAcq.BackColor = _DefaultColor;
                        BtnUltraAcq.MouseinBackColor = _DefaultColor;
                        BtnUltraAcq.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.KNOB_MULTI_LEFT:
                    if (CircFunction.Progress == _MinValue)
                    {
                        CircFunction.Progress = _MidValue;
                        CircFunction.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircFunction.StartAngle == _LeftAngle)
                        {
                            CircFunction.Progress = _MinValue;
                        }
                        else
                        {
                            CircFunction.StartAngle = _LeftAngle;
                            CircFunction.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_MULTI_RIGHT:
                    if (CircFunction.Progress == _MinValue)
                    {
                        CircFunction.Progress = _MidValue;
                        CircFunction.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircFunction.StartAngle == _RightAngle)
                        {
                            CircFunction.Progress = _MinValue;
                        }
                        else
                        {
                            CircFunction.StartAngle = _RightAngle;
                            CircFunction.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_MULTI_SELECT:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedMultipupose, _KeyState[KeyCode.KNOB_MULTI_SELECT]);
                    if (CircFunction.GroundBackColor == _DefaultColor)
                    {
                        CircFunction.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircFunction.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.VK_APPS:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedApps, _KeyState[KeyCode.VK_APPS]);
                    if (_KeyState[KeyCode.VK_APPS])
                    {
                        BtnAPP.BackColor = _KeyPressedColor;
                        BtnAPP.MouseinBackColor = _KeyPressedColor;
                        BtnAPP.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnAPP.BackColor = _UnenableColor;
                        BtnAPP.MouseinBackColor = _UnenableColor;
                        BtnAPP.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.TOUCH:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedTouchLock, _KeyState[KeyCode.TOUCH]);
                    if (_KeyState[KeyCode.TOUCH])
                    {
                        BtnTouchLock.BackColor = _KeyPressedColor;
                        BtnTouchLock.MouseinBackColor = _KeyPressedColor;
                        BtnTouchLock.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnTouchLock.BackColor = _DefaultColor;
                        BtnTouchLock.MouseinBackColor = _DefaultColor;
                        BtnTouchLock.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.VK_SCREENSHOT:

                    if (_KeyState[KeyCode.VK_SCREENSHOT])
                    {
                        BtnScreenshot.BackColor = _KeyPressedColor;
                        BtnScreenshot.MouseinBackColor = _KeyPressedColor;
                        BtnScreenshot.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnScreenshot.BackColor = _UnenableColor;
                        BtnScreenshot.MouseinBackColor = _UnenableColor;
                        BtnScreenshot.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.SETTING:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedUtility, _KeyState[KeyCode.SETTING]);
                    if (_KeyState[KeyCode.SETTING])
                    {
                        BtnUtility.BackColor = _KeyPressedColor;
                        BtnUtility.MouseinBackColor = _KeyPressedColor;
                        BtnUtility.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnUtility.BackColor = _UnenableColor;
                        BtnUtility.MouseinBackColor = _UnenableColor;
                        BtnUtility.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.VK_VOLTMETER:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedDVM, _KeyState[KeyCode.VK_VOLTMETER]);
                    if (_KeyState[KeyCode.VK_VOLTMETER])
                    {
                        BtnDVM.BackColor = _KeyPressedColor;
                        BtnDVM.MouseinBackColor = _KeyPressedColor;
                        BtnDVM.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnDVM.BackColor = _DefaultColor;
                        BtnDVM.MouseinBackColor = _DefaultColor;
                        BtnDVM.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.VK_AWGALL:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedAWG, _KeyState[KeyCode.VK_AWGALL]);
                    if (_KeyState[KeyCode.VK_AWGALL])
                    {
                        BtnGen.BackColor = _KeyPressedColor;
                        BtnGen.MouseinBackColor = _KeyPressedColor;
                        BtnGen.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnGen.BackColor = _DefaultColor;
                        BtnGen.MouseinBackColor = _DefaultColor;
                        BtnGen.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.VK_CLEAR:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedClear, _KeyState[KeyCode.VK_CLEAR]);
                    if (_KeyState[KeyCode.VK_CLEAR])
                    {
                        BtnClear.BackColor = _KeyPressedColor;
                        BtnClear.MouseinBackColor = _KeyPressedColor;
                        BtnClear.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnClear.BackColor = _UnenableColor;
                        BtnClear.MouseinBackColor = _UnenableColor;
                        BtnClear.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.DEFAULT:
                    //KeyboardLed.Default.LedStateControl(LedEnum.LedDefault, _KeyState[KeyCode.DEFAULT]);
                    if (_KeyState[KeyCode.DEFAULT])
                    {
                        BtnDefault.BackColor = _KeyPressedColor;
                        BtnDefault.MouseinBackColor = _KeyPressedColor;
                        BtnDefault.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnDefault.BackColor = _UnenableColor;
                        BtnDefault.MouseinBackColor = _UnenableColor;
                        BtnDefault.PressedBackColor = _UnenableColor;
                    }
                    break;
                case KeyCode.KNOB_XLEVEL_LEFT:
                    if (CircHScale.Progress == _MinValue)
                    {
                        CircHScale.Progress = _MidValue;
                        CircHScale.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircHScale.StartAngle == _LeftAngle)
                        {
                            CircHScale.Progress = _MinValue;
                        }
                        else
                        {
                            CircHScale.StartAngle = _LeftAngle;
                            CircHScale.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_XLEVEL_RIGHT:
                    if (CircHScale.Progress == _MinValue)
                    {
                        CircHScale.Progress = _MidValue;
                        CircHScale.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircHScale.StartAngle == _RightAngle)
                        {
                            CircHScale.Progress = _MinValue;
                        }
                        else
                        {
                            CircHScale.StartAngle = _RightAngle;
                            CircHScale.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_XLEVEL_SELECT:
                    if (CircHScale.GroundBackColor == _DefaultColor)
                    {
                        CircHScale.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircHScale.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.KNOB_XPOS_LEFT:
                    if (CircHPosition.Progress == _MinValue)
                    {
                        CircHPosition.Progress = _MidValue;
                        CircHPosition.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircHPosition.StartAngle == _LeftAngle)
                        {
                            CircHPosition.Progress = _MinValue;
                        }
                        else
                        {
                            CircHPosition.StartAngle = _LeftAngle;
                            CircHPosition.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_XPOS_RIGHT:
                    if (CircHPosition.Progress == _MinValue)
                    {
                        CircHPosition.Progress = _MidValue;
                        CircHPosition.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircHPosition.StartAngle == _RightAngle)
                        {
                            CircHPosition.Progress = _MinValue;
                        }
                        else
                        {
                            CircHPosition.StartAngle = _RightAngle;
                            CircHPosition.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_XPOS_SELECT:
                    if (CircHPosition.GroundBackColor == _DefaultColor)
                    {
                        CircHPosition.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircHPosition.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.MATH:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedMath, _KeyState[KeyCode.MATH]);
                    if (_KeyState[KeyCode.MATH])
                    {
                        BtnMath.BackColor = _KeyPressedColor;
                        BtnMath.MouseinBackColor = _KeyPressedColor;
                        BtnMath.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnMath.BackColor = _DefaultColor;
                        BtnMath.MouseinBackColor = _DefaultColor;
                        BtnMath.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.REF:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedReference, _KeyState[KeyCode.REF]);
                    if (_KeyState[KeyCode.REF])
                    {
                        BtnRef.BackColor = _KeyPressedColor;
                        BtnRef.MouseinBackColor = _KeyPressedColor;
                        BtnRef.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnRef.BackColor = _DefaultColor;
                        BtnRef.MouseinBackColor = _DefaultColor;
                        BtnRef.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.LOGIC:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedDigital, _KeyState[KeyCode.LOGIC]);
                    if (_KeyState[KeyCode.LOGIC])
                    {
                        BtnDigital.BackColor = _KeyPressedColor;
                        BtnDigital.MouseinBackColor = _KeyPressedColor;
                        BtnDigital.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnDigital.BackColor = _DefaultColor;
                        BtnDigital.MouseinBackColor = _DefaultColor;
                        BtnDigital.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.DECODE:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedBus, _KeyState[KeyCode.DECODE]);
                    if (_KeyState[KeyCode.DECODE])
                    {
                        BtnBus.BackColor = _KeyPressedColor;
                        BtnBus.MouseinBackColor = _KeyPressedColor;
                        BtnBus.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnBus.BackColor = _DefaultColor;
                        BtnBus.MouseinBackColor = _DefaultColor;
                        BtnBus.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.KNOB_YLEVEL_LEFT:
                    if (CircVScale.Progress == _MinValue)
                    {
                        CircVScale.Progress = _MidValue;
                        CircVScale.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircVScale.StartAngle == _LeftAngle)
                        {
                            CircVScale.Progress = _MinValue;
                        }
                        else
                        {
                            CircVScale.StartAngle = _LeftAngle;
                            CircVScale.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_YLEVEL_RIGHT:
                    if (CircVScale.Progress == _MinValue)
                    {
                        CircVScale.Progress = _MidValue;
                        CircVScale.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircVScale.StartAngle == _RightAngle)
                        {
                            CircVScale.Progress = _MinValue;
                        }
                        else
                        {
                            CircVScale.StartAngle = _RightAngle;
                            CircVScale.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_YLEVEL_SELECT:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedVerticalScale, _KeyState[KeyCode.KNOB_YLEVEL_SELECT]);
                    if (CircVScale.GroundBackColor == _DefaultColor)
                    {
                        CircVScale.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircVScale.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.KNOB_YPOS_LEFT:
                    if (CircVPosition.Progress == _MinValue)
                    {
                        CircVPosition.Progress = _MidValue;
                        CircVPosition.StartAngle = _LeftAngle;
                    }
                    else
                    {
                        if (CircVPosition.StartAngle == _LeftAngle)
                        {
                            CircVPosition.Progress = _MinValue;
                        }
                        else
                        {
                            CircVPosition.StartAngle = _LeftAngle;
                            CircVPosition.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_YPOS_RIGHT:
                    if (CircVPosition.Progress == _MinValue)
                    {
                        CircVPosition.Progress = _MidValue;
                        CircVPosition.StartAngle = _RightAngle;
                    }
                    else
                    {
                        if (CircVPosition.StartAngle == _RightAngle)
                        {
                            CircVPosition.Progress = _MinValue;
                        }
                        else
                        {
                            CircVPosition.StartAngle = _RightAngle;
                            CircVPosition.Progress = _MidValue;
                        }
                    }
                    break;
                case KeyCode.KNOB_YPOS_SELECT:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedVerticalPosition, _KeyState[KeyCode.KNOB_YPOS_SELECT]);
                    if (CircVPosition.GroundBackColor == _DefaultColor)
                    {
                        CircVPosition.GroundBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        CircVPosition.GroundBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.CH1:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedCH1, _KeyState[KeyCode.CH1]);
                    if (_KeyState[KeyCode.CH1])
                    {
                        BtnC1.BackColor = _KeyPressedColor;
                        BtnC1.MouseinBackColor = _KeyPressedColor;
                        BtnC1.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnC1.BackColor = _DefaultColor;
                        BtnC1.MouseinBackColor = _DefaultColor;
                        BtnC1.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.CH2:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedCH2, _KeyState[KeyCode.CH2]);
                    if (_KeyState[KeyCode.CH2])
                    {
                        BtnC2.BackColor = _KeyPressedColor;
                        BtnC2.MouseinBackColor = _KeyPressedColor;
                        BtnC2.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnC2.BackColor = _DefaultColor;
                        BtnC2.MouseinBackColor = _DefaultColor;
                        BtnC2.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.CH3:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedCH3, _KeyState[KeyCode.CH3]);
                    if (_KeyState[KeyCode.CH3])
                    {
                        BtnC3.BackColor = _KeyPressedColor;
                        BtnC3.MouseinBackColor = _KeyPressedColor;
                        BtnC3.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnC3.BackColor = _DefaultColor;
                        BtnC3.MouseinBackColor = _DefaultColor;
                        BtnC3.PressedBackColor = _DefaultColor;
                    }
                    break;
                case KeyCode.CH4:
                    KeyboardLed.Default.LedStateControl(LedEnum.LedCH4, _KeyState[KeyCode.CH4]);
                    if (_KeyState[KeyCode.CH4])
                    {
                        BtnC4.BackColor = _KeyPressedColor;
                        BtnC4.MouseinBackColor = _KeyPressedColor;
                        BtnC4.PressedBackColor = _KeyPressedColor;
                    }
                    else
                    {
                        BtnC4.BackColor = _DefaultColor;
                        BtnC4.MouseinBackColor = _DefaultColor;
                        BtnC4.PressedBackColor = _DefaultColor;
                    }
                    break;
                default:
                    break;
            }
            this.Refresh();
        }

        private void BtnRunStop_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.RUNSTOP;
            }
        }

        private void BtnAutoset_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.AUTOSET;
            //}
        }

        private void BtnSingle_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.SINGLE;
            }
        }

        private void BtnNormal_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.NORMAL;
            }
        }

        private void BtnAuto_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.AUTO;
            }
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.TRIGGER;
            //}
        }

        private void BtnForce_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.TRIG_FORCE;
            //}
        }

        private void CircLevel_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.KNOB_TRIG_YPOS_SELECT;
            }
        }

        private void BtnCursors_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.CURSOR;
            }
        }

        private void BtnMeas_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.MEASURE;
            }
        }

        private void BtnQuickMeas_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.VK_SNAPSHOT;
            }
        }

        private void BtnUltraAcq_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.FASTACQ;
            }
        }

        private void BtnAPP_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.VK_APPS;
            //}
        }

        private void BtnTouchLock_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.TOUCH;
            }
        }

        private void BtnScreenshot_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.VK_SCREENSHOT;
            //}
        }

        private void BtnUtility_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.SETTING;
            //}
        }

        private void BtnDVM_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.VK_VOLTMETER;
            }
        }

        private void BtnGen_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.VK_AWGALL;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.VK_CLEAR;
            //}
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.KeyCheckCode = KeyCode.DEFAULT;
            //}
        }

        private void CircFunction_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.KNOB_MULTI_SELECT;
            }
        }

        private void CircHScale_Click(object sender, EventArgs e)
        {

        }

        private void CircHPosition_Click(object sender, EventArgs e)
        {

        }

        private void BtnMath_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.MATH;
            }
        }

        private void BtnRef_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.REF;
            }
        }

        private void BtnDigital_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.LOGIC;
            }
        }

        private void BtnBus_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.DECODE;
            }
        }

        private void CircVScale_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.KNOB_YLEVEL_SELECT;
            }
        }

        private void CircVPosition_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.KNOB_YPOS_SELECT;
            }
        }

        private void BtnC1_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.CH1;
            }
        }

        private void BtnC2_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.CH2;
            }
        }

        private void BtnC3_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.CH3;
            }
        }

        private void BtnC4_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.KeyCheckCode = KeyCode.CH4;
            }
        }
    }
}
