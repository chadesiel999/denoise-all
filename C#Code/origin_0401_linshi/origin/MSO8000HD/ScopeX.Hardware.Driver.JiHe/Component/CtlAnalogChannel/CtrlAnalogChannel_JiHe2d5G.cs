#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Linq;
using static ScopeX.ComModel.HdMessage;
using System.Collections.Concurrent;
using static ScopeX.Hardware.Driver.AbstractAcquirer_LA;
using System.Text;
using ScopeX.Hardware.Driver.Module;

namespace ScopeX.Hardware.Driver
{
    class JiHeAnalogChannelDacPortInfo
    {
        public JiHeAnalogChannelDacPortInfo(short _dacIndex, short _portIndex, short dacIndexEx = 0)
        {
            dacIndex = _dacIndex;
            portIndex = _portIndex;
            this.dacIndexEx = dacIndexEx;
        }
        public short dacIndex;
        public short portIndex;
        public short dacIndexEx;
    }
    /// <summary>
    /// 模拟通道
    /// </summary>
    internal partial class CtrlAnalogChannel_JiHe2d5G : McuComPortUpdater
    {
        internal static string GetCaliMemo()
        {
            return $"文档待完善！对偏置电压(Bias)的校准:{System.Environment.NewLine}" +
                $" 高阻：1mV~50mV 档，应该使用2V的直流输入{System.Environment.NewLine}" +
                $"       100mV~200mV应该使用2V的直流输入{System.Environment.NewLine}" +
                $"       500mV~2V应该使用2V的直流输入{System.Environment.NewLine}" +
                $"       5V~10V应该使用2V的直流输入{System.Environment.NewLine}";
        }

        #region 5668：偏置的控制
        const UInt32 TableIndex_RefCtrl = 0;
        const UInt32 TableIndex_OffsetLowImpedance = 1;//Lz=低阻
        const UInt32 TableIndex_OffsetHighImpedance = 2;//Hz=高阻
        const UInt32 TableIndex_MoveLowImpedance = 3;
        const UInt32 TableIndex_MoveHighImpedance = 4;
        static JiHeAnalogChannelDacPortInfo[,] dacPortInfoTable =
        {
            // refCtrl   lz_offset   hz_offset  lz_move      hz_move
            {new (6, 5,8), new (6, 0,8), new (6, 4,8), new (6, 2,8), new (6, 7,8)}, // ch1
            {new (6, 5,8), new (6, 6, 8), new (6, 3, 8), new (6, 1, 8), new (7, 3,10)}, // ch2
            {new (7, 5,10), new (7, 0,10), new (7, 4,10), new (7, 2,10), new (7, 7,10)}, // ch3
            {new (7, 5,10), new (8, 0,12), new (8, 4,12), new (8, 2,12), new (8, 7,12)}, // ch4
        };
        /// <summary>
        /// Offset,Bias,Trigger Volt等DAC控制
        /// </summary>0x0377b480
        /// <param name="DAC_Select"></param>
        /// <param name="dacindexEx"></param>
        /// <param name="Port"></param>
        /// <param name="Data"></param>
        //internal static void SendDataTo5675(short DAC_Select, UInt32 Port, uint dacindexEx, UInt32 Data)
        //{
        //    //Data = 0;
        //    UInt32 ConfigWord = 0x1030_0000;
        //    ConfigWord |= dacindexEx << 24;
        //    ConfigWord |= Port << 16;
        //    ConfigWord |= Data;
        //    //return;

        //    HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)DAC_Select);
        //    HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, ConfigWord);
        //    HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
        //    System.Threading.Thread.Sleep(1);
        //    HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
        //    /*
        //     * 由于现在通道板单片机无法处理粘包的情况，所有通道控制命令完成后增加延迟，后期单片机程序修改后再删除延迟
        //     */
        //    //Thread.Sleep(10);
        //}
        protected static void SendDataTo5675(DAC5675Command command, DAC5675ChannelsAddress address, UInt16 data, DAC5675Index dacindex = DAC5675Index.First)
        {
            if (Hd.CurrProduct == null || Hd.CurrProduct.HardwareConfig == null)
                return;
            UInt32 tempdata = 0x00000000;
            tempdata |= ((UInt32)command) << 20;
            tempdata |= ((UInt32)address) << 16;
            tempdata |= ((UInt32)data) & 0xFFFF;

            //tempdata = (UInt32)(data & 0xFFFF)|(UInt32)(((UInt32)command << 20)) | (UInt32)(address << 16) ;

            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x9000);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataL, (UInt16)((tempdata >> 16) & 0xFF));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668CtrlDataH, (UInt16)(tempdata & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, (UInt16)(0x9000 | (((Byte)dacindex & 0b0111) << 1)));
            //HdIO.WaitForSpiTransfer(SPIClock, sizeof(UInt32));
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x8000);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.LA_AD5668TransStart, 0x9000);
            Thread.Sleep(10);
        }

        internal static void SendDataTo5668(short DAC_Select, UInt32 Port, UInt32 Data)
        {
            UInt32 ConfigWord = 0x00030000u | (Port << 12);
            ConfigWord <<= 8;
            ConfigWord |= Data << 4;

            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)DAC_Select);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, ConfigWord);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(2);//什么道理，要10ms
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);

            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);
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


        protected enum DAC5675ChannelsAddress : Byte
        {
            TRIG_CTRL_DECAY = 0b0100,//DAC4 TRIG_CTRL_DECAY，0为10倍衰减，1为2倍衰减,3.3V->DATA 0xCE3F
            TRIG_COUPLE_AC_EN = 0b0101,//DAC5 TRIG_COUPLE_AC_EN，AC耦合使能
            TRIG_COUPLE_DC_EN = 0b0110,//DAC6 TRIG_COUPLE_DC_EN，DC耦合使能
            TRIG_COUPLE_HFR = 0b0111,//DAC7 TRIG_COUPLE_HFR, 高频抑制

            TRIG_CTRL_IMP = 0b0011,//DAC3 TRIG_CTRL_IMP，0为高阻1为低阻路径
            //0b0010,//DAC2
            TRIG_HYS_VOLTAGE_CTRL = 0b0001,//DAC1 TRIG_HYS_VOLTAGE_CTRL, 触发灵敏度
            TRIG_LEVEL_VOLTAGE_CTRL = 0b0000,//DAC0 TRIG_LEVEL_VOLTAGE_CTRL, 触发电平控制
        };

        internal protected enum DAC5675Index : Byte
        {
            First = 0b001,
        }

        /// <summary>
        /// 根据通道id和dataType，发送5668的配置数据data；
        /// dataType与dacPortInfoTable的定义耦合：0-ref，1-offset_lz，2-offset_hz,3-move_lz,4-move_hz
        /// </summary>
        private static bool SendDataTo5675ByChannel(UInt32 channelId, UInt32 dataType, UInt32 data)
        {
            if (channelId >= dacPortInfoTable.GetLength(0) || dataType >= dacPortInfoTable.GetLength(1))
                return false;

            short dacIndex = dacPortInfoTable[channelId, dataType].dacIndex;
            short portIndex = dacPortInfoTable[channelId, dataType].portIndex;
            short dacindexex = dacPortInfoTable[channelId, dataType].dacIndexEx;
            //SendDataTo5675(dacIndex, (UInt32)portIndex, (UInt32)dacindexex, data);
            SendDataTo5668(dacIndex, (UInt32)portIndex, data);
            //COMPort_SendDataTo5675(dacIndex, (UInt32)portIndex, (UInt32)dacindexex, data);
            Thread.Sleep(10);
            return true;
        }
        #endregion

        #region 4094:带宽、阻抗、粗略的衰减倍数控制
        private const UInt32 bwCtrlBit0 = 0x1 << 0;
        private const UInt32 bwCtrlBit2 = 0x1 << 1;
        private const UInt32 bwCtrlBit1 = 0x1 << 2;
        private const UInt32 relayCtrlBit1 = 0x1 << 3;
        private const UInt32 relayCtrlBit0 = 0x1 << 4;
        private const UInt32 acDcCtrl = 0x1 << 5;
        private const UInt32 relayCtrlBit2 = 0x1 << 6;

        private static Dictionary<Int32, UInt32> bwCtrlWords = new Dictionary<Int32, UInt32>()
        {
            {0,     0},//Full
            {1,     bwCtrlBit1 | bwCtrlBit0},//Bw1GHz
            {2,     bwCtrlBit0},//Bw500MHz
            {3,     bwCtrlBit2 | bwCtrlBit0},//Bw20MHz
        };

        //LZ衰减表,24.6.17,wcj
        internal static Dictionary<AnaChnlScaleIndex, UInt32> newAttTableLz = new Dictionary<AnaChnlScaleIndex, UInt32>()
        {
            // 直通
            {AnaChnlScaleIndex.Lv1m,    0},
            {AnaChnlScaleIndex.Lv2m,    0},
            {AnaChnlScaleIndex.Lv5m,    0},
            {AnaChnlScaleIndex.Lv10m,   0},
            {AnaChnlScaleIndex.Lv20m,   0},
            {AnaChnlScaleIndex.Lv50m,   0},
            // 6dB
			{AnaChnlScaleIndex.Lv100m,  6},
            // 12dB
            {AnaChnlScaleIndex.Lv200m,  12},
            // 20dB
            {AnaChnlScaleIndex.Lv500m,  20},
            {AnaChnlScaleIndex.Lv1,     20},
        };
        //HZ衰减表,24.6.17,wcj
        internal static Dictionary<AnaChnlScaleIndex, UInt32> newAttTableHz = new Dictionary<AnaChnlScaleIndex, UInt32>()
        {
            // 直通
            {AnaChnlScaleIndex.Lv1m,    0},
            {AnaChnlScaleIndex.Lv2m,    0},
            {AnaChnlScaleIndex.Lv5m,    0},
            {AnaChnlScaleIndex.Lv10m,   0},
            {AnaChnlScaleIndex.Lv20m,   0},
            {AnaChnlScaleIndex.Lv50m,   0},
            {AnaChnlScaleIndex.Lv100m,  0},
            // 12dB
            {AnaChnlScaleIndex.Lv200m,  12},
            {AnaChnlScaleIndex.Lv500m,  12},
            // 26dB
            {AnaChnlScaleIndex.Lv1,     26},
            {AnaChnlScaleIndex.Lv2,     26},
            // 38dB
            {AnaChnlScaleIndex.Lv5,     38},
            {AnaChnlScaleIndex.Lv10,    38},
        };

        internal static UInt32 GetDACScaleMv(AnaChnlScaleIndex currentScale, Dictionary<AnaChnlScaleIndex, UInt32> attTable)
        {
            AnaChnlScaleIndex dacScale = attTable.Last().Key;
            var dsaTable = attTable.GroupBy(att => att.Value);
            foreach (var group in dsaTable)
            {
                if (currentScale <= group.Last().Key)
                {
                    dacScale = group.Last().Key;
                    break;
                }
            }
            var scaleMv = (UInt32)(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[(int)dacScale] / 1000);
            return scaleMv;
        }

        //低阻
        internal static Dictionary<AnaChnlScaleIndex, UInt32> yScalCtrlLz_4094 = new Dictionary<AnaChnlScaleIndex, UInt32>()
        {
            // 直通
            {AnaChnlScaleIndex.Lv1m,    0},
            {AnaChnlScaleIndex.Lv2m,    0},
            {AnaChnlScaleIndex.Lv5m,    0},
            {AnaChnlScaleIndex.Lv10m,   0},
            {AnaChnlScaleIndex.Lv20m,   0},
            // 14dB
            {AnaChnlScaleIndex.Lv50m,   relayCtrlBit1},
            {AnaChnlScaleIndex.Lv100m,  relayCtrlBit1},
            // 26dB
            {AnaChnlScaleIndex.Lv200m,  relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv500m,  relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv1,     relayCtrlBit1 | relayCtrlBit2},
        };
        //高阻
        internal static Dictionary<AnaChnlScaleIndex, UInt32> yScalCtrlHz_4094 = new Dictionary<AnaChnlScaleIndex, UInt32>()
        {
            // 直通
            {AnaChnlScaleIndex.Lv1m,    relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv2m,    relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv5m,    relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv10m,   relayCtrlBit1 | relayCtrlBit2},
            {AnaChnlScaleIndex.Lv20m,   relayCtrlBit1 | relayCtrlBit2},
            // 20dB
            {AnaChnlScaleIndex.Lv50m,   relayCtrlBit1},
            {AnaChnlScaleIndex.Lv100m,  relayCtrlBit1},
            {AnaChnlScaleIndex.Lv200m,  relayCtrlBit1},
            // 26dB
            {AnaChnlScaleIndex.Lv500m,  relayCtrlBit2},
            {AnaChnlScaleIndex.Lv1,     relayCtrlBit2},
            // 46dB
            {AnaChnlScaleIndex.Lv2,     0},
            {AnaChnlScaleIndex.Lv5,     0},
            {AnaChnlScaleIndex.Lv10,    0},
        };

        private static Dictionary<AnaChnlCoupling, UInt32> coupleCtrl_4094 = new Dictionary<AnaChnlCoupling, UInt32>()
        {
            {AnaChnlCoupling.DC50, relayCtrlBit0},
            {AnaChnlCoupling.DC1M,   acDcCtrl},
            {AnaChnlCoupling.AC1M,   0},
        };
        protected static (Int32 ScaleIndex, Int32 ScaleValueByuV, Int32 Gain_FineByFpgaThousand) GetCurrentScaleIndex(Int32 channelIndex)
        {
            HdMessage.AnalogOptions analogparameters = Hd.UIMessage!.Analog![channelIndex];
            Int32 impedance_h_is0 = analogparameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
            Int32 scalevaluebyuv = 0;
            Int32 maxscaleindex = (impedance_h_is0 == 0) ? (Int32)AnaChnlScaleIndex.Lv10 : (Int32)AnaChnlScaleIndex.Lv1;
            Int32 minscaleindex = (Int32)AnaChnlScaleIndex.Lv1m;
            Int32 scaleindex_firstgteq = minscaleindex;
            Int32 currscalebyuv = (Int32)(analogparameters.Scale * 1000);
            var autocaliparams = AutoCaliParams.Default![channelIndex, impedance_h_is0, scaleindex_firstgteq];
            if (!Hd.CurrProduct.HardwareConfig!.bExistsAnalogChannelGainFineAdjustMode)
            {
                scalevaluebyuv = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[analogparameters.ScaleIndex];
                AnalogChannelItem_Base chnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelIndex, impedance_h_is0 == 0, (UInt32)(scalevaluebyuv / 1000)))!.Value;
                autocaliparams = AutoCaliParams.Default![channelIndex, impedance_h_is0, analogparameters.ScaleIndex];
                return (analogparameters.ScaleIndex, scalevaluebyuv, autocaliparams.Gain_FineByFpgaThousand);
            }


            //从最接近的档位开始找：最接近的含义是，当前scale 第一次 >= 整数档的值
            Int32 scale = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.FirstOrDefault(o => o >= currscalebyuv);
            scaleindex_firstgteq = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.IndexOf(scale);
            if (Math.Abs(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleindex_firstgteq] - currscalebyuv) < AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleindex_firstgteq] * 1.0 / 1000)
            {
                scalevaluebyuv = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleindex_firstgteq];
                AnalogChannelItem_Base chnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelIndex, impedance_h_is0 == 0, (UInt32)(scalevaluebyuv / 1000)))!.Value;
                autocaliparams = AutoCaliParams.Default![channelIndex, impedance_h_is0, scaleindex_firstgteq];
                return (analogparameters.ScaleIndex, scalevaluebyuv, autocaliparams.Gain_FineByFpgaThousand); //约定，细调必须大于等于档位的千分之一。
            }

            Int32 upscaleindex = scaleindex_firstgteq;
            Int32 dnScaleindex = scaleindex_firstgteq;
            if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleindex_firstgteq] >= currscalebyuv)
            {
                if (scaleindex_firstgteq > minscaleindex)
                    dnScaleindex--;
            }
            else if (scaleindex_firstgteq < maxscaleindex)
                upscaleindex = scaleindex_firstgteq + 1;

            AnalogChannelItem_Base upchnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                new((ChannelId)channelIndex, impedance_h_is0 == 0, (UInt32)(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[upscaleindex] / 1000)))!.Value;
            autocaliparams = AutoCaliParams.Default![channelIndex, impedance_h_is0, upscaleindex];
            Int32 upfpgafine = (Int32)((Int32)(autocaliparams.Gain_FineByFpgaThousand * currscalebyuv / AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[upscaleindex]));
            AnalogChannelItem_Base dnchnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                new((ChannelId)channelIndex, impedance_h_is0 == 0, (UInt32)(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[upscaleindex] / 1000)))!.Value;
            autocaliparams = AutoCaliParams.Default![channelIndex, impedance_h_is0, upscaleindex];
            Int32 dnfpgafine = (Int32)((Int32)(autocaliparams.Gain_FineByFpgaThousand * currscalebyuv / AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[dnScaleindex]));

            (Int32 scaleIndex, Int32 FPGAFine)[] three = new (Int32 scaleIndex, Int32 FPGAFine)[2]
            {
                (upscaleindex,upfpgafine),
                (dnScaleindex,dnfpgafine),
            };
            var v = three.MinBy(o => Math.Abs(o.FPGAFine - 1000));
            return (v.scaleIndex, AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[v.scaleIndex], v.FPGAFine);
        }
        /// <summary>
        /// 根据带宽，阻抗和幅度档，获取8bit的控制字；返回值为False时需核查当前配置是否超出表格范围
        /// </summary>
        private static bool Get4094CtrlWord(int channelIndex, ref UInt32 ctrlWord)
        {
            (int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
            HdMessage.AnalogOptions phychannel = Hd.UIMessage!.Analog![channelIndex];
            bool retVal = true;
            ctrlWord = 0;
            // 带宽
            int curBandwidth = phychannel.Bandwidth;
            if (phychannel.Coupling != AnaChnlCoupling.DC50)
            {
                if (curBandwidth < 2)//Bw500MHz
                    curBandwidth = 2;//Bw500MHz
            }
            retVal &= bwCtrlWords.TryGetValue(curBandwidth, out UInt32 bwCtrlWord);
            ctrlWord |= bwCtrlWord;
            // 阻抗
            retVal &= coupleCtrl_4094.TryGetValue(phychannel.Coupling, out UInt32 coupleCtrlWord);
            ctrlWord |= coupleCtrlWord;

            // 默认使用高阻的幅度衰减表，如果是低阻，显式地进行更换
            Dictionary<AnaChnlScaleIndex, UInt32> scaleTable = yScalCtrlHz_4094;
            if (phychannel.Coupling == AnaChnlCoupling.DC50)
            {
                scaleTable = yScalCtrlLz_4094;
            }
            retVal &= scaleTable.TryGetValue((AnaChnlScaleIndex)bestScaleIndexFpgaFine.ScaleIndex, out UInt32 scalCtrlWord);
            ctrlWord |= scalCtrlWord;

            return retVal;
        }

        /// <summary>
        /// 根据带宽，阻抗和幅度档，获取外触发通道9bit的控制字；
        /// </summary>
        private static bool GetExt4094CtrlWord(ref UInt32 ctrlWord)
        {
            bool retVal = true;
            TriggerOptions tOption = Hd.UIMessage?.Trigger;
            TrigEdgeOptions teOption = tOption?.Edge;
            //TRIG_SOURCE_SELECT设置
            ctrlWord |= 0b0000_0010_0;
            //TRIG_COUPLE_CTRL
            ctrlWord |= teOption.Coupling switch
            {
                TriggerCoupling.DC => 0b0000_0000_1,
                TriggerCoupling.AC => 0b0000_1000_1,
                TriggerCoupling.LFR => 0b0000_1000_0,
                TriggerCoupling.HFR => 0b1000_0000_0,
            };
            //TRIG_DECAY_CTRL
            ctrlWord |= (UInt32)(tOption.EnableExtAtten ? 0b0010_0000_0 : 0b0000_0000_0);
            //TRIG_IMP_CTRL
            ctrlWord |= teOption.Impedance switch
            {
                TriggerImpedance.Low50 => 0b0100_0000_0,
                TriggerImpedance.High1M => 0b0000_0000_0,
            };

            return retVal;
        }
        #endregion

        /// <summary>
        /// 通道底板单片机上电
        /// </summary>
        /// <returns></returns>
        public static bool McuPowerOn()
        {
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_channelpower_ctrl, (UInt16)0x0001);
            HdIO.Sleep(200);
            return true;
        }
        /// <summary>
        /// 通道上电
        /// </summary>
        public static void ChlPowerOn()
        {
            //串口通路
            COMPort_PowerCtrl(true);
        }

        /// <summary>
        /// 通道下电
        /// </summary>
        public static void ChlPowerOff()
        {
            //串口通路
            COMPort_PowerCtrl(false);
        }

        /// <summary>
        /// 通道底板开始工作：1、单片机上电，2、打开串口，3、检测运行模式
        /// </summary>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        internal static Boolean WorkOn(bool isUpdate = false)
        {
            Init();
            //if (baseObj1.Connected)
            //{
            //    Hd.SysLogger?.Invoke("通道板串口已被打开!", "Warning");
            //    return false;
            //}
            //var result = baseObj1.Open($"COM{Constants.COMPORTNUM_ANALGCHANNEL1}", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            //if (!result)
            //{
            //    Hd.SysLogger?.Invoke("通道板串口打开失败!", "Warning");
            //    return false;
            //}
            //if (Constants.BOARD_ATTACHED)
            //{
            //    baseObj1.SendData(true, (byte)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);
            //    Thread.Sleep(500);
            //    var isBoot = false;
            //    var isApp = baseObj1.McuUpdate_IsRunAtApp(1500);
            //    if (!isApp)
            //    {
            //        isBoot = baseObj1.McuUpdate_IsRunAtBoot(150);
            //    }

            //    if (isUpdate)
            //    {
            //        result = isBoot || isApp;
            //    }
            //    else
            //    {
            //        result = isApp;
            //    }
            //    if (!result)
            //    {
            //        //上电失败
            //        Hd.SysLogger?.Invoke("通道板上电失败!", "Warning");
            //        return false;
            //    }
            //    baseObj1.RegisterAppRunStartTime();
            //    Hd.SysLogger?.Invoke($"ChannelMcuLastUpdateTime:{baseObj1.ReadUpdatetimeStamp()}", "Info");
            //    InitRefDac();
            //}
            return true;
        }
        /// <summary>
        /// 通道底板停止工作：1、通道下电，2、关闭串口，3、通道底板下电
        /// </summary>
        internal static void WorkOff()
        {
            PowerOff();
            ////串口通路
            //COMPort_PowerCtrl(false);

            ////串口关闭
            //baseObj1.Close();
            //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_channelpower_ctrl, (UInt16)0x0000);
        }
        private static void AfterPowerOn()
        {
            bNeedInitDacRef = true;
            SystemMonitor.Default.ResetReadTemperature();
            Hd.SysLogger?.Invoke($"Channel Mcu restart at{DateTime.Now}", "Info");
        }
       
        internal static void PowerOff()
        {
            PowerCtrl(false);
            HdIO.Sleep(10);
        }
        private static void PowerCtrl(Boolean enable)
        {
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x13);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, (enable ? 0x0u : 0x1u) << 20);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(2);//什么道理，要10ms
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);
        }

        private static void PowerOver(Boolean enable)
        {
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x13);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, (enable ? 0x0u : 0x8u) << 22);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(2);//什么道理，要10ms
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);

            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x13);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, (enable ? 0x0u : 0x8u) << 21);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(2);//什么道理，要10ms
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);

        }
        internal static void Init()
        {
            PowerCtrl(true);
            OverVoltageReset(true, 0);
        }
        public static void OverVoltageReset(bool allChnl, uint chnlId)
        {
            if (allChnl == true)
            {
                SendCtrlWordsTo8G(3, 1 << 28);
                SendCtrlWordsTo8G(3, 2 << 28);
                SendCtrlWordsTo8G(3, 4 << 28);
                SendCtrlWordsTo8G(3, (uint)8 << 28);
                //SendCtrlWordsTo8G(3, 0);
            }
            else
            {
                uint ctrlWords = (uint)((chnlId * 2) << 28);
                SendCtrlWordsTo8G(3, ctrlWords);
            }
        }
        private static void SendCtrlWordsTo8G(uint flag, uint ctrlWords)
        {
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x10 | flag);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData, ctrlWords);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(2);//什么道理，要10ms
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);
        }

        internal static void SendADCCalibrationData()
        {

        }
        private static bool bNeedInitDacRef = false;
        internal static void InitRefDac()
        {
            UInt32 defaultRefValue = 32000;
            for (UInt32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                SendDataTo5675ByChannel(channelIndex, TableIndex_RefCtrl, defaultRefValue);
            }
            if (baseObj1.AfterPowerOnAction == null)
                baseObj1.AfterPowerOnAction = AfterPowerOn;
        }
        internal static void SendCmdToDSA(UInt32 ctrlWords, UInt32 channelId)
        {
            //UInt32 EnableWord = 0x0001;
            //EnableWord = EnableWord << (int)channelIndex;
            ////HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_DATA, Data);
            //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_DATA, 0x32);
            //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, EnableWord);
            //Thread.Sleep(1);
            //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, 0x0);
            ///*
            // * 由于现在通道板单片机无法处理粘包的情况，所有通道控制命令完成后增加延迟，后期单片机程序修改后再删除延迟
            // */
            //Thread.Sleep(10);

            UInt32 EnableWord = 0x0008;
            EnableWord = EnableWord >> (int)channelId;
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0X10);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_reg_ch_dsa_data_8g, ctrlWords);   //CHAI
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, EnableWord);
            Thread.Sleep(5);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, 0x0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0);
        }
        private static AcqBdReg.W[] fpgaOffsetRegister = new AcqBdReg.W[]
        {
            AcqBdReg.W.Acq_Ch1AcqOffset,
            AcqBdReg.W.Acq_Ch2AcqOffset,
            AcqBdReg.W.Acq_Ch3AcqOffset,
            AcqBdReg.W.Acq_Ch4AcqOffset,
        };
        private static void CtrlOffset(bool bOnlyBias)
        {
            for (UInt32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                if (Hd.UIMessage?.Analog?[channelIndex] == null)
                    continue;
                if (Hd.UIMessage?.Analog?[channelIndex].InputSource != AnaChnlIpnutSource.BNC)
                    continue;
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage.Analog[channelIndex];
                int Impedance_H_is0 = analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                Dictionary<AnaChnlScaleIndex, UInt32> attTable = (analogParameters.Coupling == AnaChnlCoupling.DC50) ? newAttTableLz : newAttTableHz;

                (int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex((int)channelIndex);
                int yScaleIndex = bestScaleIndexFpgaFine.ScaleIndex;
                AnalogChannelItem_Base chnlParams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelIndex, Impedance_H_is0 == 0, (uint)(bestScaleIndexFpgaFine.ScaleValueByuV / 1000)))!.Value;

                #region Offset-Hardware
                if (!bOnlyBias)
                {
                    Int32 pos0Div = chnlParams.Offset;
                    Int32 pos3Div_P = chnlParams.Offset_Pos3Div;
                    Int32 pos3Div_N = chnlParams.Offset_Neg3Div;
                    Int32 posPosition = (Int32)(Constants.SAMPS_PER_YDIV * analogParameters.Position * 1000 / bestScaleIndexFpgaFine.ScaleValueByuV);
                    Int32 posPositionCtrl = 0;
                    if (posPosition >= 0)
                        posPositionCtrl = (Int32)(posPosition * pos3Div_P / (Constants.SAMPS_PER_YDIV * 3));
                    else
                        posPositionCtrl = (Int32)(posPosition * pos3Div_N / (Constants.SAMPS_PER_YDIV * 3));

                    posPositionCtrl = pos0Div + posPositionCtrl;
                    if (Impedance_H_is0 == 0)
                        SendDataTo5675ByChannel(channelIndex, TableIndex_MoveHighImpedance, (uint)posPositionCtrl);
                    else
                        SendDataTo5675ByChannel(channelIndex, TableIndex_MoveLowImpedance, (uint)posPositionCtrl);
                    #region AcqFPGA Offset
                    Int32 acqFpgaUsedOffset = (int)(analogParameters.PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV + 128);
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(fpgaOffsetRegister[channelIndex], (uint)acqFpgaUsedOffset);
                    #endregion
                }
                #endregion

                #region Bias-Hardware
                int Pre0Div = chnlParams.Bias;
                uint moveFactorScaleMv = GetDACScaleMv((AnaChnlScaleIndex)yScaleIndex, attTable);
                double MoveFactor = ProductDataTranslate_MSO8000X.GetChnlParamsItem(new((ChannelId)channelIndex, Impedance_H_is0 == 0, moveFactorScaleMv))!
                    .Value.Bias_Pos3Div / 100_000_000D;  //Bias_Pos3Div表示：斜率放大100_000_000倍的值。该值的大小可以理解为最大输入电平的值(以uV为单位)

                Int32 Bias = Pre0Div - (Int32)((double)analogParameters.Bias * MoveFactor);
                if (Impedance_H_is0 == 0)
                    SendDataTo5675ByChannel(channelIndex, TableIndex_OffsetHighImpedance, (uint)Bias);
                else
                    SendDataTo5675ByChannel(channelIndex, TableIndex_OffsetLowImpedance, (uint)Bias);
                #endregion
            }
        }
        private static Int32[] YscaleBymV = new Int32[4];
        public static void AnalogChannelMiscSet()
        {
            if (Hd.UIMessage?.Analog?.Any(ana => ana == null) ?? true)
                return;

            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog[channelindex];
                if (YscaleBymV[channelindex] == (Int32)analogparas.Scale)
                    continue;
                YscaleBymV[channelindex] = (Int32)analogparas.Scale;
                UInt32 schmidtgate;
                switch (YscaleBymV[channelindex])
                {
                    case 1:
                    case 2:
                    case 5:
                        schmidtgate = 200;
                        break;
                    default:// >= 5mv
                        schmidtgate = 200;
                        break;
                }
                
                switch (channelindex)
                {
                    case (Int32)ChannelId.C1:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch1, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x40000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x41000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x40000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x41000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C2:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch2, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x42000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x43000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x42000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x43000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C3:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch3, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x44000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x45000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x44000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x45000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C4:
                        //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch4, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x46000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x47000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x46000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x47000, schmidtgate);
                        break;
                }
            }
        }

        public static void AnalogChannelMiscSetAll()
        {
            
            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog[channelindex];
                YscaleBymV[channelindex] = (Int32)analogparas.Scale;
                UInt32 schmidtgate;
                switch (YscaleBymV[channelindex])
                {
                    case 1:
                    case 2:
                    case 5:
                        schmidtgate = 500;
                        break;
                    default:// >= 5mv
                        schmidtgate = 150;
                        break;
                }

                switch (channelindex)
                {
                    case (Int32)ChannelId.C1:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch1, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x40000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x41000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x40000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x41000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C2:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch2, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x42000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x43000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x42000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x43000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C3:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch3, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x44000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x45000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x44000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x45000, schmidtgate);
                        break;
                    case (Int32)ChannelId.C4:
                        //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch4, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x46000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch1 | 0x47000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x46000, schmidtgate);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.TrigCtrl_SchmittGate_Ch2 | 0x47000, schmidtgate);
                        break;
                }
            }
        }
        public static void CtrlAnalogChannelSet()
        {
            if (bNeedInitDacRef)
            {
                InitRefDac();
                bNeedInitDacRef = false;
            }
            var temprature = ReadTemperatures() * 4;
            var versionInfo = ReadMcuVersion();
            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
            {
                CtrlAnalogChannel_3U8G.COMPort_AnalogChannelSet();
            }
            else
            {
                COMPort_AnalogChannelSet();
            }
            AnalogChannelMiscSet();
            #region 调节点2：Adc_Fine
            //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
            #endregion
            #region 调节点3：FPGA_Fine
            CtrlGainByFpga();
            #endregion

            CtrlGainByFpgaUpo();

            //2023/07/24 HC
            Thread.Sleep(50);
            //Hd.PushWaitMilliseconds(500);

            //下发校准系数
            //CoefficientsTableSender_8000X.Send_IFCCoefficientsToAcqBoardByRegisterMode(true);
        }

        public static void Send_IFCCoefficientsToAcqBoardByRegisterMode()
        {
            CoefficientsTableSender_8000X.Send_IFCCoefficientsToAcqBoardByRegisterMode(true);
        }
        public static void CtrlOffset()
        {
            CtrlOffset(false);
        }
        public static void CtrlBias()
        {
            CtrlOffset(true);
        }
        public static void CtrlGain()
        {
            if (Hd.UIMessage?.Analog?[0] == null)
                return;
            List<FPGAAdcPair> waitForCaliFinishedAdcList = new List<FPGAAdcPair>();
            for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                //HdMessage.AnalogOptions analogParameters = Hd.UIMessage.Analog[channelIndex];
                //int Impedance_H_is0 = analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                //(int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
                //int yScaleIndex = bestScaleIndexFpgaFine.ScaleIndex;
                //#region 调节点1：粗调增益
                //int GainCtr = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                //    new((ChannelId)channelIndex, Impedance_H_is0 == 0, (uint)(bestScaleIndexFpgaFine.ScaleValueByuV / 1000)))!.Value.Gain;
                //if (Hd.bAdjustGainByTemperature)
                //{

                //}
                ////过界保护
                //if (GainCtr < 0)
                //    GainCtr = 0;
                //else if (GainCtr > 32)
                //    GainCtr = 32;

                //SendCmdToDSA((UInt32)GainCtr, (ChannelId)channelIndex);
                ////COMPort_SendCmdToDSA((UInt32)GainCtr, (ChannelId)channelIndex);
                //#endregion
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![channelIndex];

                #region 调节点1：粗调增益
                int Impedance_H_is0 = 1;//只有低阻 analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                int yScaleIndex = analogParameters.ScaleIndex;
                int GainCtrlow = (Int32)ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].Gain_CoarseCtrlWord;
                int GainCtrlhigh_14dB = (Int32)ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].Gain_CoarseCtrlWord << 8;
                int GainCtr = GainCtrlow | GainCtrlhigh_14dB;
                SendCmdToDSA((UInt32)GainCtr, (UInt32)channelIndex);
                #endregion
            }
            #region 调节点2：Adc_Fine
            //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
            #endregion
            #region 调节点3：FPGA_Fine
            CtrlGainByFpga();
            #endregion
            CtrlGainByFpgaUpo();
        }

        /// <summary>
        /// 获取5G通道的DSA1，及DSA2粗调值
        /// </summary>
        /// <param name="totalWord"></param>
        /// <returns></returns>
        public static (int Dsa1, int Dsa2) Get5GDsaCtrlWord(bool isHighImp, AnaChnlScaleIndex yScale, int totalWord)
        {
            //设置策略：两级DSA，Dsa2衰减值固定，调整Dsa1衰减值
            double stepDb = 0.25;
            double totalAttenuate = stepDb * totalWord;
            int Dsa1, Dsa2;

            (double Dsa1, double Dsa2) attenuateDbPair = (isHighImp, yScale) switch
            {
                //低阻
                (false, AnaChnlScaleIndex.Lv1m) => (0, 0),
                (false, AnaChnlScaleIndex.Lv2m) => (0, 0),
                (false, AnaChnlScaleIndex.Lv5m) => (5.5, 0),
                (false, AnaChnlScaleIndex.Lv10m) => (5.5, 6),
                (false, AnaChnlScaleIndex.Lv20m) => (5.5, 12),
                (false, AnaChnlScaleIndex.Lv50m) => (3.75, 0),
                (false, AnaChnlScaleIndex.Lv100m) => (4.5, 0),
                (false, AnaChnlScaleIndex.Lv200m) => (4, 0),
                (false, AnaChnlScaleIndex.Lv500m) => (4, 0),
                (false, AnaChnlScaleIndex.Lv1) => (7, 3),
                //高阻
                (true, AnaChnlScaleIndex.Lv1m) => (0, 0),
                (true, AnaChnlScaleIndex.Lv2m) => (0, 0),
                (true, AnaChnlScaleIndex.Lv5m) => (6.25, 0),
                (true, AnaChnlScaleIndex.Lv10m) => (6.25, 6),
                (true, AnaChnlScaleIndex.Lv20m) => (6.25, 12),
                (true, AnaChnlScaleIndex.Lv50m) => (4.5, 0),
                (true, AnaChnlScaleIndex.Lv100m) => (7.5, 3),
                (true, AnaChnlScaleIndex.Lv200m) => (4.5, 0),
                (true, AnaChnlScaleIndex.Lv500m) => (8.5, 4),
                (true, AnaChnlScaleIndex.Lv1) => (4.5, 0),
                (true, AnaChnlScaleIndex.Lv2) => (7.5, 3),
                (true, AnaChnlScaleIndex.Lv5) => (6.5, 0),
                (true, AnaChnlScaleIndex.Lv10) => (8.5, 4),
                _ => (0, 0)
            };

            if (totalAttenuate >= attenuateDbPair.Dsa2)
            {
                Dsa2 = (int)(attenuateDbPair.Dsa2 / stepDb);
                Dsa1 = (int)((totalAttenuate - attenuateDbPair.Dsa2) / stepDb);
            }
            else //注意此种异常情况
            {
                Dsa2 = (int)(totalAttenuate / stepDb);
                Dsa1 = 0;
            }
            return (Dsa1, Dsa2);
        }

        public static void CtrlExtTrig()
        {
            var compVoltBymV = Hd.UIMessage?.Trigger?.Edge?.Position ?? 0.0;
            var decayEnable = Hd.UIMessage?.Trigger?.EnableExtAtten ?? false;
            UInt32 trigsource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();

            if (trigsource != (UInt32)ChannelId.Ext && trigsource != (UInt32)ChannelId.Ext5)
                return;

            (Int32 Volt100VDeltaByuV, Int32 ZeroDelta) calidata = ((ChannelId)trigsource, Hd.UIMessage!.Trigger!.Edge!.Impedance, Hd.UIMessage!.Trigger!.Edge!.Coupling) switch
            {
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.DC) => (MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_DC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV]),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.AC) => (MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_AC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_AC_ZeroDelta_uV]),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.HFR) => (MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_HFR_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_HFR_ZeroDelta_uV]),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.LFR) => (MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_LFR_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_LFR_ZeroDelta_uV]),
                (ChannelId.Ext, TriggerImpedance.Low50, _) => (MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_DC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV]),

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.DC) => (MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_DC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV]),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.AC) => (MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_AC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_AC_ZeroDelta_uV]),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.HFR) => (MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_HFR_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_HFR_ZeroDelta_uV]),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.LFR) => (MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_LFR_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_LFR_ZeroDelta_uV]),

                (ChannelId.Ext5, TriggerImpedance.Low50, _) => (MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_DC_100VDelta_uV], MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV]),

                (_, _, _) => (0, 0),
            };

            //double dacRegMax = Math.Pow(2, 16) - 1;
            //double dacMaxOutputByMv = Math.Pow(2, 12) - 1;
            //int attenu = Hd.CurrHdMessage?.Trigger.Edge.Source == ChannelId.Ext ? 2 : 10;
            //double destVoltByMv = Hd.CurrHdMessage!.Trigger.Edge.Position / attenu;

            //double dacSendValue = destVoltByMv / (dacMaxOutputByMv / 2) * (dacRegMax / 2) + dacRegMax / 2;
            //double volt100VComp = (caliData.volt100VDelta / 1000 / attenu) / dacMaxOutputByMv * dacRegMax;
            //double zeroComp = (caliData.ZeroDelta / 1000 / attenu) / dacMaxOutputByMv * dacRegMax;
            //double compensation = destVoltByMv / 100_000 * (volt100VComp - zeroComp) + zeroComp;
            //dacSendValue = Math.Min(Math.Max(0, dacSendValue + compensation), dacRegMax);

            #region 前辈们的

            //double dacRegMax = Math.Pow(2, 16) - 1;
            //double dacMaxOutputBymV = Math.Pow(2, 12) - 1;
            //int attenu = Hd.UIMessage?.Trigger.Edge.Source == ChannelId.Ext ? 2 : 10;
            //double destVoltBymV = Hd.UIMessage!.Trigger.Edge.Position * (1e8 + caliData.volt100VDeltaByuV) / 1e8 / attenu;//1e8==100V by uV为单位

            //double dacSendValue = -1 * destVoltBymV / (dacMaxOutputBymV / 2) * (dacRegMax / 2);
            //double zeroCompensation = -1 * (caliData.ZeroDelta / 1e3 / attenu) / dacMaxOutputBymV * dacRegMax;//1e3,uV==>mV
            //dacSendValue += zeroCompensation + (dacRegMax / 2);
            //if (dacSendValue < 0)
            //    dacSendValue = 0;
            //else if (dacSendValue > dacRegMax)
            //    dacSendValue = dacRegMax;

            #endregion

            #region Hc 2024.12.20 新加

            var triggeredge = Hd.UIMessage!.Trigger!.Edge!;

            var offsetbyuv = ((ChannelId)trigsource, triggeredge.Impedance, triggeredge.Coupling) switch
            {
                //Ext
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.DC) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.DC) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.AC) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_AC_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.AC) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_AC_ZeroDelta_uV],


                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.LFR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_LFR_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.LFR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_LFR_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.HFR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_HFR_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.HFR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_HFR_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.NR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV] - 88000, //chenyan:噪声抑制未校准，特殊处理，用实测值，下同
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.NR) => MiscData.Default[(Int32)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV] - 880000,


                //Ext5  使用Ext校准值
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.DC) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.DC) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.AC) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_AC_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.AC) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_AC_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.LFR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_LFR_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.LFR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_LFR_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.HFR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_HFR_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.HFR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_HFR_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.NR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV] - 90000 * 5,
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.NR) => MiscData.Default[(Int32)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV] - 90000 * 5,

                (_, _, _) => 0,
            };

            var offset = offsetbyuv / 1e3; //uV to mV
            var ratio = (trigsource == (UInt32)ChannelId.Ext) ? 1D : 5D;
            var smtgate_hfrorother = (triggeredge.Coupling == TriggerCoupling.HFR) ? 70D : 55D;
            var smtgatebymv = (triggeredge.Coupling == TriggerCoupling.NR) ? 280 * ratio : smtgate_hfrorother * ratio;
            if (!Hd.Calibration.IsCaliExttrigger)
            {
                if (triggeredge.Slope == EdgeSlope.Rise)
                {
                    offset -= smtgatebymv / 4;
                }
                else if (triggeredge.Slope == EdgeSlope.Fall)
                {
                    offset += smtgatebymv / 4;
                }
                else
                {
                    //offset += smtGateBymV * 0.375;
                }
            }
            else
            {
                offset = 0;
            }

            var triggerPosbymv = offset + triggeredge.Position * 0.6D;
            //归一化到0xFFFF
            Int32 dacsendvalue = (Int32)((Constants.EXT_TRIGGER_MAX_MV * ratio - triggerPosbymv) / ((Constants.EXT_TRIGGER_MAX_MV - Constants.EXT_TRIGGER_MIN_MV) * ratio) * 0xFFFF);
            dacsendvalue = Math.Clamp(dacsendvalue, 0, 0xFFFF); //limit（v, min,max）

            #endregion Hc 2024.12.20 新加
            //SendDataTo5675(8, 3, 0xc, (uint)dacSendValue); //0x8600对应0v触发电平
            //衰减
            Int32 param = 0xCE40;
            UInt16 trigctrldecay = (UInt16)((ChannelId)trigsource == ChannelId.Ext ? param : 0);
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_CTRL_DECAY, trigctrldecay, DAC5675Index.First);

            //触发耦合方式
            UInt16 trigcoupleacen = (UInt16)(Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.AC ? param : 0);
            UInt16 trigcoupledcen = (UInt16)(Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.DC ? param : 0);
            UInt16 trigcouplehfr = (UInt16)(Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.HFR ? param : 0);
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_COUPLE_AC_EN, trigcoupleacen, DAC5675Index.First);
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_COUPLE_DC_EN, trigcoupledcen, DAC5675Index.First);
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_COUPLE_HFR, trigcouplehfr, DAC5675Index.First);

            //噪声抑制走的是DC参数
            //触发灵敏度
            //UInt16 positiondac = Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.NR ? Constants.EXT_TRIGGER_NR_POSITION_DEFAULT : Constants.EXT_TRIGGER_ACDC_POSITION_DEFAULT;
            var positiondac = Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.NR ? 0x7500/*Constants.EXT_TRIGGER_NR_POSITION_DEFAULT*/ : 0x6530/*Constants.EXT_TRIGGER_ACDC_POSITION_DEFAULT*/;
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_HYS_VOLTAGE_CTRL, (UInt16)positiondac, DAC5675Index.First);

            //阻抗
            UInt16 triggerimpedance = (ushort)(Hd.UIMessage!.Trigger!.Edge!.Impedance == TriggerImpedance.High1M ? 0 : param);
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_CTRL_IMP, triggerimpedance, DAC5675Index.First);

            //触发电平控制
            SendDataTo5675(DAC5675Command.WriteAndUpdateChannel, DAC5675ChannelsAddress.TRIG_LEVEL_VOLTAGE_CTRL, (ushort)dacsendvalue, DAC5675Index.First);

            //COMPort_ExitTrigChannelSet();
        }
        public static void CtrlTrigVolt()
        {
        }

        private static void GeneratedAndSend4094CtrlWord(bool bChannelActive)
        {
            UInt32 ctrlWord_4094 = 0;
            UInt32 Low32HC595 = 0;
            UInt32 High8HC595 = 0;
            for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                //if (Hd.CurrHdMessage?.Analog?[(int)channelIndex] == null)
                //    return;
                HdMessage.AnalogOptions phychannel = Hd.UIMessage!.Analog![(int)channelIndex];
                Get4094CtrlWord((int)channelIndex, ref ctrlWord_4094);
                Low32HC595 |= ctrlWord_4094 << 8 * channelIndex;
            }

            //通道选择控制
            if (bChannelActive)
            {

                High8HC595 = 0;
                Low32HC595 = 0;
                //High8HC595 |= ((3 << 4) | (channelSelectFlag43 << 2 | channelSelectFlag21)) << 4;
            }
            else
                High8HC595 |= 0xA << 8;

            //外触发相关配置
            if (Hd.UIMessage?.Trigger?.Edge?.Source != null &&
               Hd.UIMessage?.Trigger?.Edge?.Source == ChannelId.Ext)
            {
                UInt32 High9Bit = 0;
                GetExt4094CtrlWord(ref High9Bit);
                //把High9Bit的第1-8位赋值给High8HC595
                High8HC595 |= (High9Bit >> 1);
                //把High9Bit的第1位赋值给Low32HC595的第31位
                Low32HC595 |= (High9Bit & 0x1) << 31;
            }

            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x0);
            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(Low32HC595, High8HC595);
            COMPort_Ctrl4094(Low32HC595, High8HC595);
        }
        public static void Ctrl4094()
        {
            GeneratedAndSend4094CtrlWord(false);
        }
        public static void ActiveChannged()
        {
            //GeneratedAndSend4094CtrlWord(true);
        }

        #region 幅度细调

        /// <summary>
        /// 幅度细调发送参数
        /// </summary>
        /// <param name="bUseOldData"></param>
        internal static void CtrlGainByFpgaUpo()
        {
            //相关寄存器定义
            (AcqBdReg.W SecondLevelGainFine_L, AcqBdReg.W SecondLevelGainFine_H, AcqBdReg.W Offset, AcqBdReg.W OffsetDelta, AcqBdReg.W Invert)[] channelparamsregs = new (AcqBdReg.W SecondLevelGainFine_L, AcqBdReg.W SecondLevelGainFine_H, AcqBdReg.W Offset, AcqBdReg.W OffsetDelta, AcqBdReg.W Invert)[]
            {
                (AcqBdReg.W.Upo_Ch1Gain_L,AcqBdReg.W.Upo_Ch1Gain_H,AcqBdReg.W.Upo_Ch1Offset,AcqBdReg.W.Upo_Ch1DeltaOffset , AcqBdReg.W.Upo_Ch1Invert),
                (AcqBdReg.W.Upo_Ch2Gain_L,AcqBdReg.W.Upo_Ch2Gain_H,AcqBdReg.W.Upo_Ch2Offset,AcqBdReg.W.Upo_Ch2DeltaOffset , AcqBdReg.W.Upo_Ch2Invert),
                (AcqBdReg.W.Upo_Ch1Gain_L,AcqBdReg.W.Upo_Ch1Gain_H,AcqBdReg.W.Upo_Ch1Offset,AcqBdReg.W.Upo_Ch1DeltaOffset , AcqBdReg.W.Upo_Ch1Invert),
                (AcqBdReg.W.Upo_Ch2Gain_L,AcqBdReg.W.Upo_Ch2Gain_H,AcqBdReg.W.Upo_Ch2Offset,AcqBdReg.W.Upo_Ch2DeltaOffset , AcqBdReg.W.Upo_Ch2Invert),
            };

            //LA_ChannelDelayEn：gain_en
            //LA_ChannelDelay0：channel_select 
            //LA_ChannelDelay1:gain->Gain
            //LA_ChannelDelay2：camp->Offset
            //LA_ChannelDelay3:Deltaoffset
            HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelayEn, (UInt32)(Hd.CurrDebugVarints.BEnable_DsoGainByFpga ? 0x01 : 0x0));
            //Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
            AcqModeAndInterleaveDefine interleavedefine = Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            for (Int32 channelindex = 0; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                //判断当前通道是否存在
                if (!interleavedefine.Details.Keys.Contains((ChannelId)channelindex))
                    continue;
                AcqBdNo acqbd = interleavedefine.Details[(ChannelId)channelindex][0].AcqBdNo;
                //foreach (var item in interleavedefine.Details[(ChannelId)channelindex])
                {
                    //acqbd = item.AcqBdNo;
                    HdMessage channelinfousemessage = Hd.UIMessage!.bAcquireStopped ? Acquisition.AcqedDataMsg! : Hd.UIMessage!;
                    //参数计算
                    var posgainfine = channelinfousemessage.Analog![channelindex].Scale / Hd.UIMessage!.Analog![channelindex].ScaleBymV;
                    UInt32 gainFine = (UInt32)(posgainfine * 65535); //Gain = (gain) * 65536

                    Boolean bneedinvertprocessoffsetdelta = (Hd.UIMessage!.Analog![channelindex].IsInverted, channelinfousemessage.Analog[channelindex].IsInverted) switch
                    {
                        (false, false) => false,
                        (true, true) => false,
                        (_, _) => true,
                    };
                    Int32 invert = (Hd.UIMessage!.bAcquireStopped && bneedinvertprocessoffsetdelta) ? -1 : 1;
                    UInt32 posoffset = (UInt32)(Constants.MAX_ADC_RES / 2 + channelinfousemessage.Analog![channelindex].PositionIndex * invert * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV);

                    Int32 posoffsetdelta = (short)((Hd.UIMessage!.Analog![channelindex].PositionIndex - channelinfousemessage.Analog![channelindex].PositionIndex) * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV);
                    if (Hd.UIMessage!.bAcquireStopped && bneedinvertprocessoffsetdelta)
                        posoffsetdelta += (short)(2 * channelinfousemessage.Analog![channelindex].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV);
                    Boolean posinvert = Hd.UIMessage!.Analog![channelindex].IsInverted;//反相目前是由Core层操作
                    if (!Hd.CurrDebugVarints.BEnable_DsoGainByFpga)
                    {
                        gainFine = 65535;
                        posoffset = 2048;
                        posoffsetdelta = 0;
                        posinvert = false;
                    }
                    //cij_pro_0807                                              
                    //Hd.CurrProduct?.AcqBd?.WriteReg(channelparamsregs[channelindex].SecondLevelGainFine_L, acqbd, gainFine & 0xffff);
                    //Hd.CurrProduct?.AcqBd?.WriteReg(channelparamsregs[channelindex].SecondLevelGainFine_H, acqbd, (gainFine >> 16) & 0xffff);
                    //Hd.CurrProduct?.AcqBd?.WriteReg(channelparamsregs[channelindex].Offset, acqbd, posoffset & 0xffff);
                    //Hd.CurrProduct?.AcqBd?.WriteReg(channelparamsregs[channelindex].OffsetDelta, acqbd, (UInt32)posoffsetdelta & 0xffff);
                    //Hd.CurrProduct?.AcqBd?.WriteReg(channelparamsregs[channelindex].Invert, acqbd, (UInt32)(posinvert ? 1U : 0));

                    uint channeid = (UInt32)(1 << channelindex);
                    HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelay0, channeid & 0xffff);

                    HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelay1, gainFine & 0xffff);
                    HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelay4, (gainFine >> 16) & 0xffff);

                    HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelay2, posoffset & 0xffff);
                    HdIO.WriteReg(ProcBdReg.W.LA_ChannelDelay3, (UInt32)posoffsetdelta & 0xffff); ;
                }
               
            }
        }

        #endregion


        internal static void CtrlGainByFpga()
        {
            if (Hd.UIMessage?.Analog?[0] == null)
                return;
            for (Int32 channelIndex = (Int32)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                Int32 decimalplaces = 11;
                (Int32 ScaleIndex, Int32 ScaleValueByuV, Int32 Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
                Double gain = Hd.CurrDebugVarints.bEnable_OpenCrystal ? 1000 : bestScaleIndexFpgaFine.Gain_FineByFpgaThousand;
                Double gain_finebyfpgathousand = (Double)(gain / 1000D * (1 << decimalplaces));
                //if (Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate)
                //{
                //    Hd.CurrProduct!.Acquirer_AnalogChannel!.GetPhyAnalogChAmplitudeTemperaturesCompensationCoefficient(out List<double> amplitudeTemperaturesCompensationCoefficient);
                //    gain_FineByFpgaThousand *= amplitudeTemperaturesCompensationCoefficient[(int)channelIndex];
                //}
                gain_finebyfpgathousand *= ProbeManager.Default.CurrChannelProbeRadioList[(ChannelId)channelIndex];
                if (!Hd.CurrDebugVarints.bEnable_CtrlGainByFpga)
                {
                    gain_finebyfpgathousand = (1 << decimalplaces);
                }
                (UInt32 lowReg, UInt32 highReg) setRegPair = (ChannelId)channelIndex switch
                {
                    ChannelId.C2 => ((UInt32)AcqBdReg.W.DigZoom_Gainch2_L, (UInt32)AcqBdReg.W.DigZoom_Gainch2_H),
                    ChannelId.C3 => ((UInt32)AcqBdReg.W.DigZoom_Gainch3_L, (UInt32)AcqBdReg.W.DigZoom_Gainch3_H),
                    ChannelId.C4 => ((UInt32)AcqBdReg.W.DigZoom_Gainch4_L, (UInt32)AcqBdReg.W.DigZoom_Gainch4_H),
                    _ => ((UInt32)AcqBdReg.W.DigZoom_Gainch1_L, (UInt32)AcqBdReg.W.DigZoom_Gainch1_H),
                };

                UInt32 gain_FineByFpgaThousand_UInt = (UInt32)gain_finebyfpgathousand;

                //var FloatHighLowPair = AbstractAnalogChannel.Convert2HighLowShortPair((float)gain_FineByFpgaThousand);
                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(setRegPair.lowReg, FloatHighLowPair.Low);
                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(setRegPair.highReg, FloatHighLowPair.High);

                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(setRegPair.lowReg, gain_FineByFpgaThousand_UInt & 0xffff);
                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(setRegPair.highReg, (UInt32)(gain_FineByFpgaThousand_UInt >> 16) & 0xffff);
                var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
                AcqModeAndInterleaveDefine interDefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
                interDefine.Details.TryGetValue((ChannelId)channelIndex, out AdcUsedInfo[] dtl);
                if (dtl != null)
                {
                    var adcInfo = dtl[0];
                    foreach (var item in dtl)
                    {
                        adcInfo = item;
                        var usedAdcs = analogAcquireModel.GetUsedAdcs(adcInfo);
                        foreach (var adcId in usedAdcs)
                        {
                            if (adcId == 0)
                            {
                                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, adcInfo.AcqBdNo, gain_FineByFpgaThousand_UInt);
                                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, adcInfo.AcqBdNo, gain_FineByFpgaThousand_UInt);
                            }
                            else
                            {
                                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, adcInfo.AcqBdNo, gain_FineByFpgaThousand_UInt);
                                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, adcInfo.AcqBdNo, gain_FineByFpgaThousand_UInt);
                            }
                        }
                    }
               
                }
            }
        }
        internal static ConcurrentDictionary<ChannelId, Double> AllChannelTemperatures = new ConcurrentDictionary<ChannelId, double>();

        /// <summary>
        /// 返回平均温度
        /// </summary>
        /// <returns></returns>
        internal static double ReadTemperatures()
        {
            var temperatures = new List<double>();
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature, null);
            if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature, 50, true, out var readbackResult))
            {
                if (readbackResult.Value.Data.Count >= 2 * 4)
                {
                    for (int ch = 0; ch < 4; ch++)
                    {
                        short tmp = (short)readbackResult.Value.Data[2 * ch];
                        tmp <<= 8;
                        tmp |= (short)(short)readbackResult.Value.Data[2 * ch + 1];
                        //return tmp * 1.0 / 10;
                        var currentreadtemperature = tmp * 5.0 / 80;//8.9Mcu修改温度读取后的算法

                        //添加温度到字典
                        if (AllChannelTemperatures.ContainsKey((ChannelId)ch))
                        {
                            AllChannelTemperatures[(ChannelId)ch] = currentreadtemperature;
                        }
                        else
                            AllChannelTemperatures.TryAdd((ChannelId)ch, currentreadtemperature);

                        //记录有效通道温度
                        if (currentreadtemperature <= 75 || currentreadtemperature > 0)
                        {
                            temperatures.Add(currentreadtemperature);
                        }
                        else
                        {
                            Hd.SysLogger?.Invoke($"{(ChannelId)ch}温度读取异常：{currentreadtemperature}℃", "warning");
                        }
                    }
                }
            }

            //如果有效通道温度 ＜ 1，则返回默认温度 65℃*（必须高于目标温度 Constants.ANALOGCHANNEL_WORKING_TEMPERATURE）
            var temperature = temperatures.Count > 0 ? temperatures.Average() : SystemMonitor.InvalidTemperature + 10D;

            return temperature;
        }

        internal static HardwareVersionInfo ReadMcuVersion()
        {
            baseObj1.SendData(true, (byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion, null);
            if (baseObj1.ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion, 1000, false, out var readbackResult))
            {
                if (readbackResult.Value.Data == null)
                {
                    return null;
                }
                if (readbackResult.Value.Data.Count == McuVersionInfoBytes)//mcu版本信息默认为129字节
                {
                    List<byte> item = readbackResult.Value.Data;
                    int byteIndex = 0;
                    int majorVersion = item[byteIndex++];
                    int minorVersion = item[byteIndex++];
                    int build = item[byteIndex++];
                    string lastModifiedDatetimeStr = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifiedDatetime_TotalBytes);
                    DateTime lastModifiedDatetime;
                    try
                    {
                        lastModifiedDatetime = new DateTime
                            (int.Parse(lastModifiedDatetimeStr.Substring(0, 4)),
                            int.Parse(lastModifiedDatetimeStr.Substring(4, 2)),
                            int.Parse(lastModifiedDatetimeStr.Substring(6, 2)),
                            int.Parse(lastModifiedDatetimeStr.Substring(8, 2)),
                            int.Parse(lastModifiedDatetimeStr.Substring(10, 2)),
                            int.Parse(lastModifiedDatetimeStr.Substring(12, 2)))
                            ;
                    }
                    catch
                    {
                        return null;
                    }
                    byteIndex += versionStructLastModifiedDatetime_TotalBytes;
                    string LastModifierName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifierName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructLastModifierName_TotalBytes;
                    string ModelName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructModelName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructModelName_TotalBytes;
                    string Comment = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructComment_TotalBytes).Replace('\0', ' ').Trim();
                    return new HardwareVersionInfo() { Major = majorVersion, Minor = minorVersion, Build = build, LastBuildDateTime = lastModifiedDatetime, LastModifier = LastModifierName, ModelName = ModelName, LastComment = Comment };
                }
            }
            return null;
        }

        internal static Double[] ReadAllChannelTemperatures()
        {
            var temps = new Double[4];
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature, null);
            if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature, 50, true, out var readbackResult))
            {
                if (readbackResult.Value.Data.Count >= 2 * temps.Length)
                {
                    for (int ch = 0; ch < temps.Length; ch++)
                    {
                        short tmp = (short)readbackResult.Value.Data[2 * ch];
                        tmp <<= 8;
                        tmp |= (short)readbackResult.Value.Data[2 * ch + 1];
                        var res = tmp * 5.0 / 80;
                        temps[ch] = res;
                    }
                }
            }
            return temps;
        }

        internal static String ourReadAppRegistedRunStartTime()
        {
            return baseObj1.ReadAppRegistedRunStartTime();
        }

        /// <summary>
        /// 通道延迟配置
        /// </summary>
        internal static void CtrlChannelDelay()
        {
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interDefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            Double samplingrate = interDefine.InterleaveMode == AdcInterleaveMode.Mode2To1 ? 2e10 : 1e10;
            foreach (var dtl in interDefine.Details)
            {
                if (Hd.UIMessage?.Analog?[(int)dtl.Key] == null)
                    continue;
                Int32 firstStageDelay = Hd.CurrDebugVarints.bEnable_ChannelDelay ? Hd.UIMessage.Analog[(int)dtl.Key].FirstStageDelay : 0;
                var adcInfo = dtl.Value[0];
                var usedAdcs = analogAcquireModel.GetUsedAdcs(adcInfo);
                int temprdelaytime = 0;
                foreach (var adcId in usedAdcs)
                {
                    var tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new(interDefine.Name, dtl.Key, adcId))!.Value;
                    temprdelaytime = firstStageDelay + tiadcItem.AdcDelay_FPGA;
                    //目前FPGA是反着丢点，暂时先反向丢点
                    //cij_new_0517
                            AcqBdReg.W reg = adcId%2 == 1 ? AcqBdReg.W.Calibration_Adc1Delay : AcqBdReg.W.Calibration_Adc2Delay;
                             Hd.CurrProduct.AcqBd!.WriteReg(reg, adcInfo.AcqBdNo, (uint)temprdelaytime);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Calibration_Adc1Delay, 0x0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Calibration_Adc2Delay, 0x0);

                }

                //// 一级延时
                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch4Delay1, 0);
#if JiHe_MSO7000X
                if (channelIndex == (int)ChannelId.C1)
                {
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch1Delay1, (uint)firstStageDelay);
                }
                else if (channelIndex == (int)ChannelId.C2)
                {
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch2Delay1, (uint)firstStageDelay);
                }
                else if (channelIndex == (int)ChannelId.C3)
                {
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch3Delay1, (uint)firstStageDelay);
                }
                else if (channelIndex == (int)ChannelId.C4)
                {
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch4Delay1, (uint)firstStageDelay);
                }
#endif
                //// 二级延时
                //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ChannelDelay_Ch1Delay2, 100);
            }

        }
    }
}
#endif