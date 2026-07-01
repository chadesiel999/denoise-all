using System;
using System.Collections.Generic;
using System.Linq;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class KeyLed
    {
        private readonly EventBus.IAnonymousEventData _KeyBoardEvent;

        private KeyLed()
        {
            _KeyBoardEvent = EventBus.EventBroker.Instance.GetEvent(KeyboardConstants.EventName);
        }

        public static KeyLed Default { get; } = new KeyLed();

        public void SetLed(LedEnum led, Boolean state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.LedStateControl, led, state);
        }

        public void SetLeds(IEnumerable<Boolean> states)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.LedStatesControl, states.ToArray());
        }

        public void SetLedColor(LedEnum led, System.Drawing.Color color)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.LedColorControl, led, color);
        }

        public void SetTriggerSrc(ChannelId id)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.TriggerChannel, (Int32)id);
        }

        public void SetFocusChannel(ChannelId id)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.SelectedChannel, (Int32)id);
        }

        public void SetTriggerModel(TriggerMode mode)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.TriggerModel, (Int32)mode);
        }

        /// <summary>
        /// added lihj 设置触发Slpoe 
        /// </summary>
        /// <param name="mode">0:上升沿 1:下降沿 2:任意沿 255:关闭</param>
        public void SetTriggerSlope(Int32 mode)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.TriggerLedControl, (Int32)mode);
        }

        public void SetRunStopState(SysState state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.RunStop, (Int32)state);
        }

        public void SetOtherChannelState(ControlChannelType type, IEnumerable<Boolean> states)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeOtherControl, type, states.ToArray());
        }

        public void SetAnalogChannelConfig(ChannelId channel, Boolean state, AnaChnlCoupling coupling, Int32 bw)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeAnalogControl, channel, state, coupling, bw);
        }
        public void SetAnalogChannelConfig(ChannelId channel, Boolean state, AnaChnlCoupling coupling, Int32 bw, Boolean ScaleAdd)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeAnalogControl, channel, state, coupling, bw, ScaleAdd);
        }

        public void SetPrint(Boolean state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.Print, Convert.ToInt32(state));
        }

        public void SetFastAcq(AnaChnlStorageMode mode)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.UltraAcq, (Int32)mode);
        }

        public void SetTouchSceen(Boolean state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.TouchSceen, Convert.ToInt32(state));
        }

        public void SetCursor(Boolean state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.Cursor, Convert.ToInt32(state));
        }

        public void SetClose(Boolean state)
        {
            _KeyBoardEvent.Publish(this, null, LedControlCommand.ScopeStateControl, ScopeStateType.Close, Convert.ToInt32(state));
        }
    }
}
