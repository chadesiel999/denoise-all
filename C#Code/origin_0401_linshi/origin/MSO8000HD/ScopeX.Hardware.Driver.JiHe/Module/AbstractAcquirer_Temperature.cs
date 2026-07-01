using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public class AbstractAcquirer_Temperaturer : AbstractAcquirer
    {
        internal override void Init()
        {
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
        }
        internal override void InitAcq()
        {
        }
        private Int32 readbackData = 0;
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            UInt32 data = (HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature) >> 8) & 0xffff;
            readbackData = (Int32)data;
            return true;

        }

        /// <summary>
        ///  读取以摄氏度为单位的温度传感器数据。
        /// </summary>
        /// <param name="whichSensor">第几个传感器，目前只有一个。</param>
        /// <returns></returns>
        public virtual double ReadByCentigrade(int whichSensor = 0)
        {
            return readbackData * 1.0 / 10;
        }
    }
}
