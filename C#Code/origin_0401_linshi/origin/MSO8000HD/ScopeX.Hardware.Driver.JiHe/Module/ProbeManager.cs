using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 探头校准数据-示波器侧数据
    /// </summary>
    public class ProbeCalibDsoData
    {
        public String ProbeSN { get; set; } = String.Empty;

        public ChannelId Id { get; set; }

        public Double GainCaliRatio { get; set; }

        public Double OffsetCaliBias { get; set; }
    }

    /// <summary>
    /// 驱动层维护的探头
    /// </summary>
    public class ProbeNowInfo
    {
        /// <summary>
        /// 序列号
        /// </summary>
        public String? SerailNumber { get; set; }

        /// <summary>
        /// 按键
        /// </summary>
        public Boolean IsPushed { get; set; }

        /// <summary>
        /// 倍率
        /// </summary>
        public Double X { get; set; }

        /// <summary>
        /// 增益
        /// </summary>
        public Double CaliRadio { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public Boolean IsConnected { get; set; }

        /// <summary>
        /// 校准日期 示波器侧暂时不用
        /// </summary>
        public String? CaliDate { get; set; } = null;

        /// <summary>
        /// 校准通道A的码值 示波器侧暂时不用
        /// </summary>
        public UInt16 CaliDnA { get; set; }

        /// <summary>
        /// 校准通道B的码值 示波器侧暂时不用
        /// </summary>
        public UInt16 CaliDnB { get; set; }

        /// <summary>
        /// 探头用户校准默认增益
        /// </summary>
        public float? CaliGain { get; set; } = 1;

        /// <summary>
        /// 探头用户校准默认偏置
        /// </summary>
        public float? CaliBias { get; set; } = 0;

        public float? DacSlope { get; set; } = 0;

        /// <summary>
        /// 探头频响数据
        /// </summary>
        public List<(float, float)> FreqData { get; set; } = new List<(float, float)>();

    /// <summary>
    /// 探头固件版本
    /// </summary>
    public String? ProbeVersion { get; set; }

        internal void Reset()
        {
            SerailNumber = "";
            ProbeVersion = "";
            IsPushed = false;
            X = 1.0;
            CaliRadio = 1.0;
            IsConnected = false;

            CaliDnA = 0;
            CaliDnB = 0;
            CaliGain = 1;
            CaliBias = 0;
            FreqData.Clear();
        }

    }

    public class ProbeManager
    {
        internal Queue<List<Byte>> ReceiveQueue = new();

        private ConcurrentDictionary<ChannelId, ProbeNowInfo> ExistsProbes = new ConcurrentDictionary<ChannelId, ProbeNowInfo>();
        internal Dictionary<ChannelId, Double> CurrChannelProbeRadioList = new Dictionary<ChannelId, Double>();
        internal static ProbeManager Default = new ProbeManager();
        private ChannelId _FocusedChId_ReadInfo = ChannelId.C1;
        private Int32 MinDAC = 0;
        private Int32 MaxDAC = 65535;
        private ProbeManager()
        {

            ProbeCaliData.Default.Load();
            for (Int32 channelID = (Int32)ChannelId.C1; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            {
                CurrChannelProbeRadioList.Add((ChannelId)channelID, 1.0);
                var probeInfo = new ProbeNowInfo();
                probeInfo.Reset();
                
                ExistsProbes.TryAdd((ChannelId)channelID, probeInfo);                 
            }
        }

        UInt32 oldProbeConnectAndPushStatus = 0;


        public String GetAllProbeFactoryInfo()
        {
            String result = "";
            foreach(var v in ExistsProbes)
            {
                if (result.Length > 0)
                    result=result+(System.Environment.NewLine);
                result= $"{result}{v.Key.ToString()} {(v.Value.IsConnected ? 1:0)} {(v.Value.SerailNumber.Length==0? "#": v.Value.SerailNumber)} {v.Value.CaliRadio.ToString()}";
            }
            return result;
        }

        public void SetOneProbeFactoryInfo(String factoryInfo)
        {
            //格式为：C1 厂家信息 增益校准值（浮点数）
            String[] values = factoryInfo.Split(' ');
            ChannelId channelId = Enum.Parse<ChannelId>(values[0]);
            if (ExistsProbes[channelId].IsConnected)
            {
                List<Byte> sendData = new List<Byte>();
                sendData.Add((Byte)(1 << (Byte)channelId));
                sendData.AddRange(Encoding.ASCII.GetBytes(values[1], 0, values[1].Length));
                CtrlAnalogChannel_JiHe2d5G.COMPort_WriteFactoryInfo(sendData);
                CalibrationSetting(channelId, Double.Parse(values[2]));
            }
        }

        public void SetOneProbeFactoryInfo(ChannelId channelId, String factoryInfo)
        {
            //暂未从7000X迁移
        }

            /// <summary>
            /// 解析版本信息
            /// </summary>
            /// <param name="bytes"></param>
            private void GetProbeVersion(Byte[] bytes)
        {

            ChannelId chl = CvtChlMaskToChannel(bytes[0]);
            if (ExistsProbes.ContainsKey(chl))
            {
                Int32 mainNo = bytes[1];
                Int32 subNo = bytes[2];
                Int32 modifyNo = bytes[3];
                ExistsProbes[chl].ProbeVersion = mainNo.ToString()+"."+ subNo.ToString()+"."+ modifyNo.ToString();
            }
        }
       
        /// <summary>
        /// 解析校准信息
        /// </summary>
        /// <param name="bytes"></param>
        private void GetProbeCaliInfo(Byte[] bytes)
        {
            if (bytes.Length != 0x23/*0x1B*/)
            {
                //校准信息返回长度26字节, 
                /* 00-03  A B通道DAC码值
                 * 04-07  A B通道DAC码值
                 * 08-11  Fpga透存数据
                 * 12-24  校准日期字符串yyyyMMDDHHMMSS
                 * | CH  |     REG           |    ROM            |    FPGA           |              Datetime                                               |    Bias           |
                 * +-----+-------------------+-------------------+-------------------+---------------------------------------------------------------------+-------------------+
                 * |  00 | 01   02   03  04  | 05   06   07  08  | 09   10   11  12  | 13   14   15   16   17   18   19   20   21   22   23   24   25  26  | 27   28   29  30  |
                 * +-----+---------+---------+---------+---------+-----------------------------------------------------------------------------------------+--------------------
                 * |     |0x00 0x00|0x00 0x00|0x00 0x00|0x00 0x00|0x00 0x00 0x00 0x00|0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x00|0x00 0x00 0x00 0x00|
                 */
                return;
            }
            float val;
            ChannelId chl = CvtChlMaskToChannel(bytes[0]);
            if (ExistsProbes.ContainsKey(chl))
            {
                List<Byte> srcbytes = new List<Byte>(bytes);
                ExistsProbes[chl].CaliDnA = (UInt16)((UInt16)(bytes[6]) << 8 | (UInt16)(bytes[5]));
                ExistsProbes[chl].CaliDnB = (UInt16)((UInt16)(bytes[8]) << 8 | (UInt16)(bytes[7]));
                if (srcbytes.Count >= 13 && BytesToFloat(srcbytes.GetRange(9,4).ToArray(),out float caligain ))
                {
                    ExistsProbes[chl].CaliGain = caligain;
                }
                else
                {
                    ExistsProbes[chl].CaliGain = 1;
                }
                if (srcbytes.Count >= 31 && BytesToFloat(srcbytes.GetRange(27, 4).ToArray(), out float calibias))
                {
                    ExistsProbes[chl].CaliBias = calibias;
                }
                else
                {
                    ExistsProbes[chl].CaliBias = 0;
                }
                if (srcbytes.Count >= 35 && BytesToFloat(srcbytes.GetRange(31, 4).ToArray(), out float dacslope))
                {
                    ExistsProbes[chl].DacSlope = dacslope;
                }
                else
                {
                    ExistsProbes[chl].DacSlope = 0;
                }
            }
        }

        /// <summary>
        /// 解析探头频响数据
        /// </summary>
        private void GetProbeFreqData(Byte[] bytes)
        {
            if (bytes.Length <5)
            {
                return;
            }

            List<Byte> srcbytes = new List<Byte>(bytes);
            ChannelId chl = CvtChlMaskToChannel(bytes[0]);
            if (!ExistsProbes.ContainsKey(chl))
            {
                return;
            }

            if(!BytesToFloatArray(srcbytes.Skip(1).ToArray(),out float[] floats))
            {
                return;
            }

            if (floats.Length %2 != 0)
            {
                return;
            }

            ExistsProbes[chl].FreqData = new List<(float, float)>();
            for (Int32 i = 0; i < floats.Length; i+=2)
            {
                ExistsProbes[chl].FreqData.Add(new(floats[i], floats[i + 1]));
            }

        }

        private Boolean BytesToFloat(Byte[] bytes, out float f)
        {
            f = 0.0f;
            Int32 valbytes = sizeof(float);
            if (bytes.Length < valbytes)
            {
                return false;
            }

            Byte[] oneval = new Byte[valbytes];
            for (Int32 i = 0; i < valbytes; i ++) oneval[i] = bytes[i];
            f = BitConverter.ToSingle(oneval, 0);
            return true;
        }

        public Boolean BytesToFloatArray(Byte[] bytes, out float[] floats)
        {
            Int32 valbytes = sizeof(float);

            List<float> temp = new List<float>();
            if (bytes.Length % valbytes != 0)
            {
                floats = temp.ToArray();
                return false;

            }

            Byte[] oneval = new Byte[valbytes];
            for (Int32 i = 0; i < bytes.Length; i += sizeof(float))
            {
                oneval[0] = bytes[i + 0];
                oneval[1] = bytes[i + 1];
                oneval[2] = bytes[i + 2];
                oneval[3] = bytes[i + 3];
                temp.Add(BitConverter.ToSingle(oneval, 0));
            }

            floats = temp.ToArray();
            return true;
        }


        /// <summary>
        /// 解析厂家信息
        /// </summary>
        private void GetProbeFactoryInfo(Byte[] bytes)
        {
            Byte[] newbytes;
            newbytes = bytes;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0xff)
                {
                    newbytes = new Byte[i];
                    newbytes = bytes.Skip(0).Take(i).ToArray();
                    break;
                }
            }
            newbytes = newbytes.Skip(1).ToArray();
            String serailNumber = Encoding.UTF8.GetString(newbytes).Trim();
            ////读回来的字符串经过解密后如下形式：UNIT-T,UT-PA1500,UTXXXXX1234567887,X1....
            ////String[] readbackFactoryInfo = Encoding.UTF8.GetString(bytes).Split(',');

            Double X = 1.0;// readbackFactoryInfo[3].Trim()=="X1"? 1.0:10.0;
            Double caliRadio = 1.0;
            if ((bytes[0] == 0 && bytes[1] == 0xff) || serailNumber.Length < 5 || String.IsNullOrWhiteSpace(serailNumber))
            {
                //serailNumber = "(NULL)";//test
                serailNumber = "";
            }
            else
            {
                //todo
                //X = ;
            }

            ChannelId chl = CvtChlMaskToChannel(bytes[0]);
            if (ExistsProbes.ContainsKey(chl))
            {
                serailNumber = serailNumber.Replace("�", "");
                ExistsProbes[chl].SerailNumber = serailNumber;
                ExistsProbes[chl].IsPushed = false;
                ExistsProbes[chl].X = X;
                ExistsProbes[chl].CaliRadio = caliRadio;
                ExistsProbes[chl].IsConnected = true;
            }
        }


        public ChannelId CvtChlMaskToChannel(Byte chlMask)
        {
            Int32 CH1_CTRL_CMD = (chlMask & 0X01);
            Int32 CH2_CTRL_CMD = (chlMask & 0X02);
            Int32 CH3_CTRL_CMD = (chlMask & 0X04);
            Int32 CH4_CTRL_CMD = (chlMask & 0X08);
            Int32 CH5_CTRL_CMD = (chlMask & 0X10);
            Int32 CH6_CTRL_CMD = (chlMask & 0X20);
            Int32 CH7_CTRL_CMD = (chlMask & 0X40);
            Int32 CH8_CTRL_CMD = (chlMask & 0X80);
            if (CH1_CTRL_CMD > 0) { return ChannelId.C1; }
            if (CH2_CTRL_CMD > 0) { return ChannelId.C2; }
            if (CH3_CTRL_CMD > 0) { return ChannelId.C3; }
            if (CH4_CTRL_CMD > 0) { return ChannelId.C4; }
            if (CH5_CTRL_CMD > 0) { return ChannelId.C5; }
            if (CH6_CTRL_CMD > 0) { return ChannelId.C6; }
            if (CH7_CTRL_CMD > 0) { return ChannelId.C7; }
            if (CH8_CTRL_CMD > 0) { return ChannelId.C8; }
            return ChannelId.C1;
        }

        /// <summary>
        /// 只查看探头状态
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<ChannelId, ProbeNowInfo>? ProbeStatus()
        {
            return ExistsProbes;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns> (String/*序列号,若为Empty表示没有探头插入*/, Boolean/*是否存在按键动作*/, Int32/*探头倍率，1/10*/,Double/*校准数据*/)</returns>
        public ConcurrentDictionary<ChannelId, ProbeNowInfo>? Read()
        {
            //SysMonitor.Default.ReadAnalogChannelTemperatures();
            if (ReceiveQueue.Count <= 0) return ExistsProbes;
            var readback = ReceiveQueue.Dequeue();
            if (readback == null)
            {
                return ExistsProbes;
            }

            Int32 dataLen = (UInt16)readback[1] + ((UInt16)readback[2] << 8);
            Byte cmd = readback[0];
            if (readback.Count - 4 - dataLen != 0)
            {
                return ExistsProbes;
            }
            switch (cmd)
            {
                case 0x31: //CMD0x31_ReadBack_ProbeStatus
                    #region CMD0x31_ReadBack_ProbeStatus

                    var probeConnected = readback[3];
                    var probeClicked = readback[4];
                    UInt32 probeConnectAndPushStatus = (UInt16)(probeConnected | ((UInt16)probeClicked << 8));
                    
                    //硬件检测过快适配
                    UInt32 bakProbeConnectAndPushStatus = oldProbeConnectAndPushStatus;
                    if (probeConnectAndPushStatus == oldProbeConnectAndPushStatus)
                    {
                        return null;
                    }
                    else
                    {
                        oldProbeConnectAndPushStatus = probeConnectAndPushStatus;
                    }

                    Boolean bProbeConnectChanged = false;
                    for (Int32 channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
                    {
                        Boolean bProbeConnected = false;
                        var chnlConnected = (probeConnected >> channelID) & 0x01;
                        if (((probeConnectAndPushStatus >> channelID) & 0x01) == 0)
                        {
                            //通道上无探头
                            ExistsProbes[(ChannelId)channelID].Reset();
                        }
                        else
                        {
                            //通道上有探头
                            if (ExistsProbes[(ChannelId)channelID].SerailNumber.Trim() == String.Empty)
                            {
                                //如果探头信息为空就发送读取指令
                                Thread.Sleep(30);
                                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeCaliInfo(channelID);
                                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeVersion(channelID);
                                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeFreqData(channelID);
                                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestFactoryInfo(channelID);
                                Debug.WriteLine($"Query Info,Probe-{(ChannelId)channelID}");
                            }
                            bProbeConnectChanged = true;
                            bProbeConnected = true;
                        }

                        //按键分析
                        var chnlClicked = (probeClicked >> channelID) & 0x01;
                        if (chnlClicked == 0)
                        {
                            //ExistsProbes[(ChannelId)channelID].IsPushed = false;
                            ExistsProbes[(ChannelId)channelID].IsConnected = bProbeConnected;
                        }
                        else
                        {
                            ExistsProbes[(ChannelId)channelID].IsPushed = true;
                            ExistsProbes[(ChannelId)channelID].IsConnected = bProbeConnected;
                        }
                    }

                    if (bProbeConnectChanged)
                        ApplyRadioByFpga();
                    #endregion
                    break;
                case 0x33://CMD0x33_Request_ReadProbeFactoryInfo
                    #region CMD0x33_Request_ReadProbeFactoryInfo
                    Byte[] bytes = readback.Skip(3).SkipLast(1).ToArray();
                    GetProbeFactoryInfo(bytes);

                    #endregion
                    break;
                case 0x90:
                    #region COMPort_RequestProbeCaliInfo
                    Byte[] cmd90Bytes = readback.Skip(3).SkipLast(1).ToArray();
                    GetProbeCaliInfo(cmd90Bytes);
                    #endregion
                    break;
                case 0x7D:
                    #region COMPort_RequestProbeVersion
                    Byte[] cmd7DBytes = readback.Skip(3).SkipLast(1).ToArray();
                    GetProbeVersion(cmd7DBytes);
                    #endregion
                    break;
                case 0x9E:
                    #region 请求探头的频响数据
                    Byte[] cmd9EBytes = readback.Skip(3).SkipLast(1).ToArray();
                    GetProbeFreqData(cmd9EBytes);
                    #endregion
                    break;
                default:
                    return ExistsProbes;
            }

            Boolean bHavePushed = false;
            //for (Int32 channelID = 8; channelID < 8 + ChannelIdExt.AnaChnlNum; channelID++)
            //{
            //    if (((readback >> channelID) & 0x01) != ((oldProbeConnectAndPushStatus >> channelID) & 0x01))
            //    {
            //        Boolean bPushed = (((readback >> channelID) & 0x01) == 1) && (((oldProbeConnectAndPushStatus >> channelID) & 0x01) == 0);
            //        bHavePushed |= bPushed;
            //        Debug.WriteLine($"pushed={bPushed}");
            //        ExistsProbes[(ChannelId)(channelID - 8)] = (ExistsProbes[(ChannelId)(channelID - 8)].SerailNumber, bPushed, ExistsProbes[(ChannelId)(channelID - 8)].X, ExistsProbes[(ChannelId)(channelID - 8)].CaliRadio);
            //    }
            //}
            //#region 更新校准数据
            //if (bProbeConnectChanged)
            //    ApplyRadioByFpga();
            //#endregion
            //oldProbeConnectAndPushStatus = readback;
            //if (bHavePushed)
            //    Thread.Sleep(600);
            return ExistsProbes;
        }
        private void ApplyRadioByFpga()
        {
            ////取得增益
            //foreach (var channelId in ExistsProbes.Keys)
            //{
            //    if (ProbeCaliData.Default.RadioData.ContainsKey(ExistsProbes[channelId].SerailNumber))
            //    {
            //        ExistsProbes[channelId].CaliRadio = ProbeCaliData.Default.RadioData[ExistsProbes[channelId].SerailNumber];
            //    }
            //}

            ////更新当前个通道增益
            //Boolean probeRadioChanged = false;
            //for (Int32 channelID = (Int32)ChannelId.C1; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            //{
            //    var v = ExistsProbes.FirstOrDefault(o => o.Key == (ChannelId)channelID, new KeyValuePair<ChannelId, ProbeNowInfo>((ChannelId)channelID, new ProbeNowInfo() { SerailNumber = "", IsPushed = false, X = 1.0, CaliRadio = 1.0, IsConnected = true }));
            //    if (CurrChannelProbeRadioList[(ChannelId)channelID] != v.Value.CaliRadio)
            //    {
            //        CurrChannelProbeRadioList[(ChannelId)channelID] = v.Value.CaliRadio;
            //        probeRadioChanged = true;
            //    }
            //}

            ////增益有改变
            //if (probeRadioChanged)
            //{
            //    AbstractController_AnalogChannel.CtrlGainByFpga();
            //}
        }
        private BitArray probeLedDifferenceOrSingle = new BitArray(new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        private BitArray probeLed = new BitArray(new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        public void ReversalHeadLight(ChannelId channel)
        {
            var OnOff = probeLed.Get(channel - ChannelId.C1);
            OnOff = !OnOff;
            probeLed.Set(channel - ChannelId.C1, OnOff);
            Debug.WriteLine($"LED: {channel},{OnOff}");

            UInt32 data = 0;
            for (Int32 i = 0; i < 8; i++)
            {
                if (probeLed.Get(i))
                    data |= (UInt32)(1 << i);
            }
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ProbeLed, data);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0b010);
            Thread.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0);
            CtrlAnalogChannel_JiHe2d5G.COMPort_CtrlHeadLight(data);
        }

        public void CtrlHeadLight(ChannelId channel, Boolean OnOff)
        {
            probeLed.Set(channel - ChannelId.C1, OnOff);
            Debug.WriteLine($"LED: {channel},{OnOff}");

            UInt32 data = 0;
            for (Int32 i = 0; i < 16; i++)
            {
                if (probeLed.Get(i))
                    data |= (UInt32)(1 << i);
            }
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ProbeLed, data);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0b010);
            Thread.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromMCU_Enable, 0);
            CtrlAnalogChannel_JiHe2d5G.COMPort_CtrlHeadLight(data);
        }
        private BitArray probeDifference = new BitArray(new Byte[] { 0, 0 });
        public void DifferenceOrSingleEnded(ChannelId channel, Boolean isDifference)
        {
            probeDifference.Set((Int32)channel + 8, isDifference);
            UInt32 data = 0;
            for (Int32 i = 8; i < 16; i++)
            {
                if (probeDifference.Get(i))
                    data |= (UInt32)(1 << i);
            }
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ProbeLed, data);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromFPGA_Enable, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromFPGA_Enable, 0b010);
            Thread.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_ReadProbeInfoFromFPGA_Enable, 0);
        }

        /// <summary>
        /// 设置探头DAC码值
        /// </summary>
        public Boolean SetProbeCodeValue(ChannelId channelID, Double biasBymV)
        { 
            Double k = (Double)ExistsProbes[channelID].DacSlope;
            if (k == 0)
                return false;
            Double dac = (biasBymV / 10000) / k; /*+ ExistsProbes[(ChannelId)channelID].CaliDnA*/;
            
            if (dac < MinDAC)
                dac = dac + MaxDAC;
            if (dac > MaxDAC)
                dac = dac - MaxDAC;
            return WriteProbeCodeValue((Int32)channelID, (UInt16)dac, 0);
        }
        private Boolean WriteProbeCodeValue(Int32 channelID, UInt16 dacA, UInt16 dacB, Int32 tryCount = 3)
        {
            Int32 tryReadMax = 2;

            for (Int32 syncIdx = 0; syncIdx < tryCount; syncIdx++)
            {
                //写码值-同步
                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRegSet(channelID, dacA, 0);

                //读码值-检查                
                for (Int32 readIdx = 0; readIdx < tryReadMax; readIdx++)
                {
                    Thread.Sleep(30);

                    var probeDnValue = CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRegGet(channelID);
                    if (probeDnValue.Length == 0 || probeDnValue.Length != 4)
                    {
                        continue;
                    }

                    UInt16 probeDacA = (UInt16)((UInt16)probeDnValue[0] | (UInt16)probeDnValue[1] << 8);
                    UInt16 probeDacB = (UInt16)((UInt16)probeDnValue[2] | (UInt16)probeDnValue[3] << 8);
                    if (probeDacA == dacA && probeDacB == dacB)
                    {
                        return true;
                    }
                }

                //再次通信前
                Thread.Sleep(30);
            }

            return false;
        }

        public void CalibrationSetting(ChannelId channel, Double radio)
        {
            //ExistsProbes[channel].CaliRadio = radio;
            //ExistsProbes[channel].IsConnected = true;
            //ApplyRadioByFpga();
            //ProbeCaliData.Default.Update(ExistsProbes[channel].SerailNumber, radio);
        }

        /*******************************************************************
         * 
         * 探头校准维护 校准数据来源分两类
         *  1、探头零偏和频响
         *  2、探头增益和偏置
         *  
         *  出厂前两类数据都会生成并保存到探头
         *  出厂后用户可以再次对探头增益和偏置校准，但校准数据保存到本地
         */

        public Boolean SaveProbeUserCalibDataToLocal(ProbeCalibDsoData data)
        {
            if (String.IsNullOrEmpty(data.ProbeSN))
            {
                return false;
            }

            ProbeCaliDataItem item = new ProbeCaliDataItem();
            item.Offset = data.OffsetCaliBias;
            item.GainRatio = data.GainCaliRatio;
            item.AtChannel = data.Id;
            item.ProbeId = data.ProbeSN;



            ProbeCaliData.Default[data.Id, item.ProbeId] = item;
            return ProbeCaliData.Default.Save();
        }

        public Boolean LoadProbeUserCalibDataToLocal(ChannelId ChlId, String probeSn, out ProbeCalibDsoData data)
        {
            var  item = ProbeCaliData.Default[ChlId, probeSn];
            if (null == item)
            {
                data = null;
                return false;
            }

            data = new ProbeCalibDsoData();
            data.ProbeSN = probeSn;
            data.Id = ChlId;
            data.GainCaliRatio = item.GainRatio;
            data.OffsetCaliBias = item.Offset;

            return true;
        }

        public Boolean SaveProbeUserCalibDataToProbe(ProbeCalibDsoData data)
        {
            return true;
        }
    }
}
