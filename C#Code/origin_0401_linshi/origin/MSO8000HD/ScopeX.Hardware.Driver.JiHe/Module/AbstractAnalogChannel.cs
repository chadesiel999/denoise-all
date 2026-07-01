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
    internal abstract class AbstractAnalogChannel
    {
        public record FloatHighLowShortPair() { public ushort High; public ushort Low; }
        public static FloatHighLowShortPair Convert2HighLowShortPair(double data)
        {
            float new_data = (float)data;
            byte[] bytes = BitConverter.GetBytes(new_data);
            return new FloatHighLowShortPair() { Low = (ushort)(bytes[0] | (bytes[1] << 8)), High = (ushort)(bytes[2] | (bytes[3] << 8)) };
        }

        public virtual void InitRefDac() { }
        public virtual void CtrlOffset() { }
        public virtual void CtrlBias() { }
        public virtual void CtrlGain() { }
        public virtual void CtrlTrigVolt() { }
        public virtual void Ctrl4094() { }
        public virtual void ActiveChannged() { }

        public virtual string Get_McuVersion() { return ""; }
        public virtual void Update_Start() { }
        public virtual bool Update_Send(byte[] data,int bytes) { return false; }
        public virtual void Update_Finished(){}
        public virtual bool Update_IsInBoot() { return false; }
    }
}
