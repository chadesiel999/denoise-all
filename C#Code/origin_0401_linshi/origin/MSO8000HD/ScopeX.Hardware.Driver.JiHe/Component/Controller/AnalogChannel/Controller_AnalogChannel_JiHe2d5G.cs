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
    internal class Controller_AnalogChannel_JiHe2d5G : AbstractController_AnalogChannel
    {
        public Controller_AnalogChannel_JiHe2d5G()
        {
            //假设DBI的通道控制与众不同
             _ChannelModel = AnalogChannelType.JiHe2d5G;
            _CtrlOffset = CtrlAnalogChannel_JiHe2d5G.CtrlOffset;
            _CtrlBias = CtrlAnalogChannel_JiHe2d5G.CtrlBias;
            _CtrlGain = CtrlAnalogChannel_JiHe2d5G.CtrlGain;
            _Ctrl4094 = CtrlAnalogChannel_JiHe2d5G.Ctrl4094;
            _CtrlExtTrig = CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig;
            _ActiveChannged = CtrlAnalogChannel_JiHe2d5G.ActiveChannged;
            _CtrlAnalogChannelSet = CtrlAnalogChannel_JiHe2d5G.CtrlAnalogChannelSet;
            _CtrlAnalogChannelCoefficientsParams = CtrlAnalogChannel_JiHe2d5G.Send_IFCCoefficientsToAcqBoardByRegisterMode;
            _CtrlChannelDelay = CtrlAnalogChannel_JiHe2d5G.CtrlChannelDelay;

            //_Init= CtrlAnalogChannel_JiHe2d5G.Init;
            _PowerOff = CtrlAnalogChannel_JiHe2d5G.WorkOff;
            _PowerOn = CtrlAnalogChannel_JiHe2d5G.WorkOn;
            _CtrlGainByFpga = CtrlAnalogChannel_JiHe2d5G.CtrlGainByFpga;
            _GetCaliMemo = CtrlAnalogChannel_JiHe2d5G.GetCaliMemo;
            _SoftwareBandwidthProcess=null;
            _ReadTemperatures = CtrlAnalogChannel_JiHe2d5G.ReadTemperatures;
        }
    }
}