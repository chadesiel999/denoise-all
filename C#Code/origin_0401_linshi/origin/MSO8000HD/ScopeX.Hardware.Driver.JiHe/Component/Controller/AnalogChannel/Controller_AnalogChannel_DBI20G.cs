using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class Controller_AnalogChannel_DBI20G : AbstractController_AnalogChannel
    {
        internal Controller_AnalogChannel_DBI20G()
        {
            _Init = CtrlAnalogChannel_DBI20G.Init;
            _PowerOn = CtrlAnalogChannel_DBI20G.PowerOn;
            _PowerOff = CtrlAnalogChannel_DBI20G.PowerOff;

            _CtrlOffset = CtrlAnalogChannel_DBI20G.CtrlOffset;
            _CtrlBias = CtrlAnalogChannel_DBI20G.CtrlBias;
            _CtrlGain = CtrlAnalogChannel_DBI20G.CtrlGain;


            //_CtrlAnalogChannelSet = CtrlAnalogChannel_DBI20G.Default.FPGAReg_AnalogChannelSet;        //HTF tmp_1105
              _CtrlAnalogChannelSet = CtrlAnalogChannel_DBI20G.COMPort_AnalogChannelSet;        //HTF tmp_1105
            //_CtrlBandwidth = CtrlAnalogChannel_DBI20G.Default.CtrlBandwidth;  //有需要再添加这些
            //_CtrlAdvancedParams = CtrlAnalogChannel_DBI20G.Default.CtrlAdvancedChannel; 
            _CtrlAnalogChannelCoefficientsParams = CtrlAnalogChannel_DBI20G.Send_IFCCoefficientsSelect;
        }


    }
}
