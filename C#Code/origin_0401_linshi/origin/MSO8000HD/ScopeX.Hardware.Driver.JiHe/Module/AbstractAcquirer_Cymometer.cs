using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 频率计
    /// </summary>
    public class AbstractAcquirer_Cymometer : AbstractAcquirer
    {
        internal override AcqDataType DataType { get => AcqDataType.Cymometer; }

        internal override void Init()
        {
            GateTime = 400;
        }

        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            return true;
        }

        internal void DoConfig(UInt32 gateTimeByms)
        {
            double gate = 312500000.0 * gateTimeByms / 1000 / (1 << 16);

            //处理板上：外触发及市电触发频率计配置
            HdIO.WriteReg(ProcBdReg.W.Cymometer_Pro_GateTimeReset, (UInt32)gate);
            HdIO.WriteReg(ProcBdReg.W.Cymometer_Pro_CymometerReset, 1);
            HdIO.WriteReg(ProcBdReg.W.Cymometer_Pro_CymometerReset, 0);

            //采集板上：模拟通道C1-C4触发频率计配置
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Cymometer_GateTimeReset, (UInt32)gate);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Cymometer_CymometerReset, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Cymometer_CymometerReset, 0);
        }
        internal double readbackData = 0;
        private bool ReadData(int whichChannel = 0)
        {
            //频率计测试结果
            uint n_l = 0;
            uint n_h = 0;
            uint t_l = 0;
            uint t_h = 0;

            //获取当前触发通道
            ChannelId trigChannel = (ChannelId)Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource();

            if(trigChannel == ChannelId.AC ||
                trigChannel == ChannelId.Ext ||
                trigChannel == ChannelId.Ext5)
            {
                t_l = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1StandardFrequenceCountL);
                t_h = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1StandardFrequenceCountH);
                n_l = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1FrequenceCountL);
                n_h = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1FrequenceCountH);
            }
            else // 模拟通道
            {
                var channelAcqBdAdcInputCorresponding = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetChannelAcqBdAdcInputCorresponding((int)trigChannel);
                if(channelAcqBdAdcInputCorresponding != null)
                {
                    AcqBdNo acqBdNo = channelAcqBdAdcInputCorresponding.BdNo;
                    (AcqBdReg.R FrequenceCountL, AcqBdReg.R FrequenceCountH, AcqBdReg.R StandardFrequenceCountL, AcqBdReg.R StandardFrequenceCountH)[] registers =
                    {
                    (AcqBdReg.R.Cymometer_Ch1FrequenceCountL, AcqBdReg.R.Cymometer_Ch1FrequenceCountH, AcqBdReg.R.Cymometer_Ch1StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch1StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch2FrequenceCountL, AcqBdReg.R.Cymometer_Ch2FrequenceCountH, AcqBdReg.R.Cymometer_Ch2StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch2StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch3FrequenceCountL, AcqBdReg.R.Cymometer_Ch3FrequenceCountH, AcqBdReg.R.Cymometer_Ch3StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch3StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch4FrequenceCountL, AcqBdReg.R.Cymometer_Ch4FrequenceCountH, AcqBdReg.R.Cymometer_Ch4StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch4StandardFrequenceCountH),
                    };

                    n_l = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].FrequenceCountL, acqBdNo));
                    n_h = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].FrequenceCountH, acqBdNo));
                    t_l = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].StandardFrequenceCountL, acqBdNo));
                    t_h = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].StandardFrequenceCountH, acqBdNo));
                }
            }
                
            uint n = (n_h << 16) | n_l;
            uint t = (t_h << 16) | t_l;
            if (t!= 0 && n!=0xffffffff)
            {
                readbackData = n * 312.5 * 1e6 / t;
                return true;
            }
            else
            {
                readbackData = 0.0;
                return false;
            }
        }
        /// <summary>
        ///  读取以Hz为单位的频率数。
        /// </summary>
        /// <param name="whichChannel">如果存在多个频率计，则读取指定通道的频率计。-1表示触发通道</param>
        /// <returns>如果读取失败，返回Double.NaN。以Hz为单位</returns>
        public double GetFrequencyByHz(int whichChannel = 0)
        {
            ReadData(whichChannel);
            return readbackData;
        }
        UInt32 _GateTimeByms = 1000;
        public UInt32 GateTime
        {
            get { return _GateTimeByms; }
            set
            {
                _GateTimeByms = value;
                DoConfig(_GateTimeByms);
            }
        }
        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source=value;
            }
        }
    }
}
