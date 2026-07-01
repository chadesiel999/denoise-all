using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ScopeX.ComModel;
using System.Collections;
using System.Threading;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Data;
using static ScopeX.Hardware.Driver.AbstractAcquirer_LA;

namespace ScopeX.Hardware.Driver
{
    public class AbstractAcquirer_LA : AbstractAcquirer
    {
        internal override AcqDataType DataType { get => AcqDataType.LA; }
        protected static Action? ConfigFunc = null;
        internal static void Config() { ConfigFunc?.Invoke(); }
        #region 属性变量定义
        /// <summary>
        /// 正在采集的采集参数
        /// </summary>
        internal readonly AcquireAttribute AcquingParameters = new AcquireAttribute();
        /// <summary>
        /// 已经采集的数据的采集参数。在读回数据后赋值
        /// </summary>
        internal readonly AcquireAttribute AcquedParameters = new AcquireAttribute();
        /// <summary>
        /// 为true时表示深机箱
        /// </summary>
        internal static Boolean S6BoardIsAlone => Hd.CurrProduct?.HardwareConfig?.bS6BoardIsAlone ?? false;


        /// <summary>
        /// SPI时钟，单位MHz
        /// </summary>
        protected static Int32 SPIClock => 1;
        /// <summary>
        /// 5668上电的地址
        /// </summary>
        protected static UInt16 DAC5668StartAddress => (UInt16)(S6BoardIsAlone ? 0b111 : 0b001);
        /// <summary>
        /// DAC5668端口的比较电平范围为-<see cref="DAC5668Range"/>~+<see cref="DAC5668Range"/>，单位mv
        /// </summary>
        protected static UInt32 DAC5668Range => 65535;

        /// <summary>
        /// DAC5668输出电压范围为0~+<see cref="DAC5668OutRange"/>,单位为mv
        /// </summary>
        protected static UInt32 DAC5668OutRange => 4096 * 2;

        /// <summary>
        /// LA探头衰减系数
        /// </summary>
        protected static UInt16 AttenuationCoefficient => 1;

        #endregion 属性变量定义

        internal override void Init()
        {
        }
        internal virtual void PowerOn()
        {

        }
        internal virtual void PowerOff()
        {
        }
        internal override void CreateAcquireAttribute()
        {
            AcquingParameters.AcqStorageMode = Hd.UIMessage?.Timebase?.AcqLength ?? AnaChnlStorageMode.Normal;
        }
        /// <summary>
        /// 目前的方案缺省通过模拟通道一起读回来。
        /// </summary>
        /// <returns></returns>
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            bDataVaild = true;
            return true;
        }

        /// <summary>
        /// 提供16/48路的LA
        /// </summary>
        /// <param name="outputData">LA数据，一个ushort代表16路,如果只有16路的数据，一个给定的采样时钟为16路数据，48路的为3个ushort,依次表示48路</param>
        /// <param name="wfmSampleInfo">提供当前数据的采样率等信息</param>
        /// <returns>成功后返回true</returns>
        public virtual bool TryTakeWave([NotNullWhen(true)] out List<ushort>? outputData, [NotNullWhen(true)] out WfmSampleInfo? wfmSampleInfo)
        {
            outputData = new List<ushort>();
            wfmSampleInfo = new WfmSampleInfo()
            {
                HdMessage = Hd.CurrProduct.Acquirer_AnalogChannel!.AcquedParameters.HdMessage,
                StartTimeByus = AcqedDataPool.LAData.WfmSampleInfo.StartTimeByus,
                SampleIntervalByus = AcqedDataPool.LAData.WfmSampleInfo.SampleIntervalByus,
            };

            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.UIMessage == null)
            {
                //test code
                AcqedDataPool.LAData.Data.Clear();
                for (int i = 0; i < 10000; i++)
                    AcqedDataPool.LAData.Data.Add((byte)(i % 0xff));
                wfmSampleInfo.SampleIntervalByus = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters.PerDataByfs_AtDdr * 1.0 / 1000_000_000 ?? .05;
                bDataVaild = true;
                //end test code
            }


            if (bDataVaild)
                outputData.AddRange(AcqedDataPool.LAData.Data);

            return bDataVaild;
        }

        /// <summary>
        /// Cali Tool 调用函数
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public virtual bool TakeAllChannelWaveform(out List<ushort> waveData)
        {
            waveData = new List<ushort>();

            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.UIMessage == null)
            {
                //test code
                AcqedDataPool.LAData.Data.Clear();
                for (int i = 0; i < 10000; i++)
                    AcqedDataPool.LAData.Data.Add((byte)(i % 0xff));
                bDataVaild = true;
                //end test code
            }
            if (bDataVaild)
                waveData.AddRange(AcqedDataPool.LAData.Data);

            return bDataVaild;
        }

        /// <summary>
        /// 计算比较电平的寄存器值
        /// </summary>
        /// <param name="voltage">比较电平，单位为mv</param>
        /// <returns>ADC5668的寄存器值</returns>
        protected static UInt16 CalculateComparisonLevelRegister(Int32 comparatorIndex, Int32 voltage)
        {
            Int32 calibase = MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparatorIndex * 2];
            if (calibase == 0)
                calibase = (Int32)(DAC5668Range / 2);
            double radio = MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparatorIndex * 2 + 1] * 1.0 / 1000;
            if (radio == 0)
                radio = ((double)DAC5668Range / DAC5668OutRange);

            //LA探头10倍衰减
            double ret = Math.Max(Math.Min(calibase + voltage / 10D * radio, DAC5668Range), 0);
            return (UInt16)ret;
        }
        /// <summary>
        /// 设置ADC5668寄存器
        /// </summary>
        /// <param name="command">控制命令</param>
        /// <param name="address">ADC5668地址</param>
        /// <param name="data">设置的数据</param>
        /// <param name="dacindex">发送到哪个dac5668
        /// <para>
        /// 当<see cref="S6BoardIsAlone"/>为true时有3个5668，此参数有效
        /// </para>
        /// 当<see cref="S6BoardIsAlone"/>为false时有1个5668，此参数无效，固定为<see cref="DAC5668Index.First"/>
        /// </param>
        /// <param name="hight4bitDB">ADC5668控制字高4位数据，默认为0</param>
        /// <param name="low4bitDB">ADC5668控制字低4位数据，默认为0</param>
        protected static void SendDataTo5668(DAC5668Command command, UInt32 address, UInt16 data, DAC5668Index dacindex = DAC5668Index.First, Byte hight4bitDB = 0, Byte low4bitDB = 0)
        {
            if (Hd.CurrProduct == null || Hd.CurrProduct.Acquirer_LA == null || Hd.CurrProduct.HardwareConfig == null)
                return;
            UInt32 tempdata = 0x00;
            tempdata |= ((UInt32)hight4bitDB & 0x0F) << 28;
            tempdata |= ((UInt32)command) << 24;
            tempdata |= ((UInt32)address) << 20;
            tempdata |= ((UInt32)data) << 8;
            tempdata |= ((UInt32)low4bitDB & 0xFF);


            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataL, (UInt16)(tempdata & 0x00FFFF));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataH, (UInt16)(tempdata >> 16));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, (Byte)((Byte)dacindex & 0b0111));
            HdIO.WaitForSpiTransfer(SPIClock, sizeof(UInt32));
            HdIO.Sleep(5);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x00);
        }

        /// <summary>
        /// 设置ADC5675寄存器
        /// </summary>
        /// <param name="command">控制命令</param>
        /// <param name="address">ADC5675地址</param>
        /// <param name="data">设置的数据</param>
        /// <param name="dacindex">发送到哪个dac5675
        protected static void SendDataTo5675(DAC5675Command command, UInt32 address, UInt16 data, DAC5675Index dacindex = DAC5675Index.First)
        {
            if (Hd.CurrProduct == null || Hd.CurrProduct.Acquirer_LA == null || Hd.CurrProduct.HardwareConfig == null)
                return;
            UInt32 tempdata = 0x00000000;
            tempdata |= ((UInt32)command) << 20;
            tempdata |= ((UInt32)address) << 16;
            tempdata |= ((UInt32)data) & 0xFFFF;

            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x9000);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataL, (UInt16)((tempdata >> 16) & 0xFF));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataH, (UInt16)(tempdata & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, (UInt16)(0x9000 | ((Byte)dacindex & 0b0111)));
            //HdIO.WaitForSpiTransfer(SPIClock, sizeof(UInt32));
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x8000);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x9000);
            Thread.Sleep(10);
        }

        /// <summary>
        /// 设置FIFO满深度
        /// </summary>
        /// <param name="depth">FIFO深度</param>
        internal static void SetFIFODepth(UInt32 depth = 1024)
        {
            depth &= 0x1F_FFFF;//只取21bit的有效数据
            //HdIO.WriteReg(ProcBdReg.W.LA_FIFODepthH, depth >> 16);//高5位
            //HdIO.WriteReg(ProcBdReg.W.LA_FIFODepthL, depth & 0xFFFF);//低16位
        }
        /// <summary>
        /// 设置迟滞电平
        /// </summary>
        protected static void SetHysteresisLevel()
        {
            for (int comparatorIndex = 0; comparatorIndex < DAC5668HysteresisChannelsAddress.Count; comparatorIndex++)
            {
                //Int32 voltage = ((Int32?)Hd.UIMessage?.Digital?[comparatorIndex].HystBymV) ?? 0;
                //迟滞默认为0，对应0mv;
                SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675HysteresisChannelsAddress[comparatorIndex], 0, DAC5675Index.First);
            };
        }
        /// <summary>
        /// 设置比较电平
        /// </summary>
        protected static void SetComparisonLevel()
        {
            for (int comparatorIndex = 0; comparatorIndex < DAC5668ComparisonLevelAddress.Count; comparatorIndex++)
            {
                Int32 voltage = ((Int32?)Hd.UIMessage?.Digital?[comparatorIndex].UserThroldBymV) ?? 0;

                UInt16 controldata =  CalculateComparisonLevelRegister(comparatorIndex,voltage);//电平计算公式
                SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ComparisonLevelAddress[comparatorIndex], controldata, DAC5675Index.First);
            };
        }


        protected enum DAC5668Command : Byte
        {
            WriteRegister = 0b0000,
            UpdateRegister = 0b0001,
            WriteRegisterAndUpdateAll = 0b0010,
            WriteAndUpdateChannel = 0b0011,
            PowerDownOrUp = 0b0100,
            LoadClearCodeRegister = 0b0101,
            LoadLDACRegister = 0b0110,
            Reset = 0b0111,
            SetREFRegister = 0b1000,
        }

        protected enum DAC5675Command : Byte
        {
            WriteRegister = 0b0001,
            UpdateRegister = 0b0010,
            WriteAndUpdateChannel = 0b0011,
            PowerDownOrUp = 0b0100,
            HardwareLDACMaskRegister = 0b0101,
            SoftwareReset = 0b0110,
            GainSetupRegister = 0b0111,
            SetupReadbackRegister = 0b1001,
            UpdateAllChannelsInputRegister = 0b1010,
            UpdateAllChannelsDACRegisterAndInputRegister = 0b1011,
        }

        protected static readonly List<UInt32> DAC5668ComparisonLevelAddress = new List<uint>()
        {
            0b0111,//DACH
            0b0101,//DACF
            0b0000,//DACA
            0b0100,//DACE
        };
        private static readonly  List<UInt32> DAC5668HysteresisChannelsAddress = new List<uint>()
        {
            0b0011,//DACD
            0b0010,//DACC
            0b0110,//DACG
            0b0001,//DACB
        };
        protected static readonly List<UInt32> DAC5675ComparisonLevelAddress = new List<uint>()
        {
            0b0100,//DAC4
            0b0101,//DAC5
            0b0110,//DAC6
            0b0111,//DAC7
        };
        private static readonly List<UInt32> DAC5675HysteresisChannelsAddress = new List<uint>()
        {
            0b0011,//DAC3
            0b0010,//DAC2
            0b0001,//DAC1
            0b0000,//DAC0
        };

        internal protected enum DAC5668Index : Byte
        {
            First = 0b001,
        }

        internal protected enum DAC5675Index : Byte
        {
            First = 0b001,
        }

    }
}
