using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    //本板与MiscData.Default[((int)MiscDefine.SyncDataRxIDelayCE_Board1) + fpgaIndex] 校准数据有关。但与运行态状态的改变无关
    internal class ProcBd_Standard : AbstractProcBd
    {
        public override void Init()
        {
            ConfigRecvDelay();
            IniAverageModule();
            ReadFpgaVersion();
            HdIO.WriteReg(ProcBdReg.W.DataPath_ChDataCacheThresh, 28 * 1024);

            //采集系统传输回路复位成正常模式(非2级传输模式)
            HdIO.WriteReg(ProcBdReg.W.FlashOperator_ActionCode, 0);
        }
        public override void Test()
        {
        }

        public override void ConfigRecvDelay()
        {
        }
    }
}