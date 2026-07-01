using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class ControlsHotKnob
    {
        private static IKnob _Control;
        //private static IKnob _LastControl;
        public static ControlsHotKnob Default = new();
        public void Turn(Int32 keyCode, Int32 keyStep)
        {
            HotKnobManager.Default.Turn(keyCode, keyStep);
            if (PlatformUIManager.Default.Platform.Attribute.SupportKnobFine && (keyCode == KeyCode.TRIGGER || keyCode == KeyCode.KNOB_MULTI_SELECT))
            {
                _Control.FineEnable = HotKnobManager.Default.Fineable;
            }
        }

        public void InitHotKnob(params IKnob[] controls)
        {
            if (!PlatformUIManager.Default.Platform.Attribute.SupportKnodAdjust)
            {
                if (controls.Count() > 0)
                {
                    controls.ToList().ForEach(c => c.DoubleClickEnable = false);
                }
            }
            if (controls.Count() > 0)
            {
                controls.ToList().ForEach(c => c.Adjustable = false);
            }
        }

        public void SetHotKnob(Object obj, IKnob control, String propertyname = "", Double ministep = 1)
        {
            if (obj == null || control == null)
            {
                return;
            }

            //_LastControl=_Control;
            _Control = control;
            HotKnobManager.Default.LostFocus = false;
            HotKnobManager.Default.LedControl(true);

            //取消注册事件
            _Control.OnLostKnobEvent -= OnLostKnobEvent;
            _Control.OnLostKnobEvent += OnLostKnobEvent;

            if (control is TouchNeb neb)
            {
                HotKnobManager.Default.Adjustable = true;
                HotKnobManager.Default.TurnRight = (_, e) =>
                {
                    ScopeXNumericEditBox.NumericButtonEventData data = false;
                    data.Step = (Int32)e;
                    neb.AddClicked?.Invoke("", data);
                };
                HotKnobManager.Default.TurnLeft = (_, e) =>
                {
                    ScopeXNumericEditBox.NumericButtonEventData data = false;
                    data.Step = (Int32)e;
                    neb.SubClicked?.Invoke("", data);
                };
            }
            else
            {
                HotKnobManager.Default.Adjustable = false;
            }
            //因为在双击时要禁用掉旋钮的功能，所以在创建时需要打开
            control.Adjustable = PlatformUIManager.Default.Platform.Attribute.SupportKnodAdjust;
            //KeyboardLed.Default.LedStateControl(LedEnum.LedMultipupose, true);//打开多功能旋钮键盘灯
            HotKnobManager.Default.Presenter = obj;
            //ControlsHotKnobPrsnt.Default.Control = control;
            HotKnobManager.Default.PropertyName = propertyname;
            HotKnobManager.Default.MinStep = ministep;

            //得到当前细调状态，在创建时刷新
            control.FineEnable = HotKnobManager.Default.Fineable;
        }

        private void OnLostKnobEvent()
        {
            HotKnobManager.Default.LostFocus = !_Control.Adjustable;
            HotKnobManager.Default.LedControl(false);
        }
    }
}
