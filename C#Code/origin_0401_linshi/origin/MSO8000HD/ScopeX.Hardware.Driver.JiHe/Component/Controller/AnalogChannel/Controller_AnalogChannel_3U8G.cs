using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道
    /// </summary>
    internal class Controller_AnalogChannel_3U8G : AbstractController_AnalogChannel
    {
        public Controller_AnalogChannel_3U8G()
        {
            _CtrlADCOffset = CtrlAnalogChannel_3U8G.CtrlADCOffset;
            _CtrlAnalogChannelSet = CtrlAnalogChannel_3U8G.FPGAReg_AnalogChannelSet;
            _CtrlAnalogChannelCoefficientsParams = CtrlAnalogChannel_3U8G.Send_IFCCoefficientsSelect;
            _CtrlExtTrig = CtrlAnalogChannel_3U8G.CtrlTrigVolt;
            _ChannelModel = AnalogChannelType.BW8G;

            //_PowerOff = CtrlAnalogChannel_JiHe2d5G.WorkOff;
            //_PowerOn = CtrlAnalogChannel_JiHe2d5G.WorkOn;
        }
    }
}