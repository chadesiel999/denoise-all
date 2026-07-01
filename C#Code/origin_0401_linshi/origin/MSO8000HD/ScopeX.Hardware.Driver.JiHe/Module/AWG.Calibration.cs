using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static ScopeX.ComModel.HdMessage;
using static ScopeX.Hardware.Calibration.Data.Base.AWGCaliData;
using static ScopeX.Hardware.Calibration.Data.Base.AWGCaliData.ArbWfmDcCailAmpTable;

namespace ScopeX.Hardware.Driver;

public static class AWGCailbration
{
    public static void InitCalibSN(string CalibSN)
    {
        AWG.CalibSN = CalibSN;
    }
}
internal partial class AWG
{
    #region 校准枚举

    public enum WfmGenCalStatus
    {
        OFF = 0,
        Cal_DC,
        Cal_AC,
        Cal_DC_Verify,
        Cal_AC_Verify
    }
    public enum WfmGenCalDataType
    {
        DC_CAL = 0,
        AC1K_CAL,
        AC_CAL
    }

    #endregion 校准枚举

    #region 校准变量

    static ushort _CalATT;
    static WfmGenCalStatus _CalStatus = WfmGenCalStatus.OFF;
    static AWGCaliChannel _CalChData = new();
    static ChannelId _WfmGenCalFocusCh = ChannelId.AWG1;
    //private static bool _CalibDataValid = false;
    public static string CalibSN { set => _CalibSN = value; get => _CalibSN; }
    static string _CalibSN = "SN12345678Test"; //后期放硬件里去
    static string _Cail_IDN = @"UTG5000_CH12%MSO7000CS#SN6455db7c7131"; //后期放硬件里去
    static string _Cail_SYS_INFO = "Model;EN;Bandwidth;Sofrware;Hardware"; //后期组合整理
    static ushort[] _CalValues = new ushort[AWG_AC_CAL_SIZE]; //  new ushort[2048];
    static double _CalOffset;
    static long _CalFreq;

    #endregion 校准变量

    #region 校准 cal

    static internal string GetCaliInfos(string param)
    {
        //Regex regex = new Regex(@"^(?:CalibAWG)([1-4])");
        int commasIndex = param.IndexOf(',');
        string cmd = "";
        if (commasIndex < 0)
        {
            return "";
        }
        else
        {
            cmd = param.Substring(commasIndex + 1).Trim();
        }
        if (cmd == "")
            return "";

        switch (cmd)
        {
            default:
                return "";
            case "CailIDN":
                return CalibGetIDN();
            case "CailSYSINFO":
                return CalibGetSysInfo();
            case "CailSN":
                return CalibGetSN();
        }
    }
    static internal void CalibATT(ushort att)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        if (channel == ChannelId.AWG1)
        {
            WriteAwgBySpi(AWG1_CNT_ATTEN, att); //幅度挡位选择
            Debug.WriteLine($"AWG1_CNT_ATTEN： {att}");
        }
        else if (channel == ChannelId.AWG2)
        {
            WriteAwgBySpi(AWG2_CNT_ATTEN, att); //幅度挡位选择
            Debug.WriteLine($"AWG2_CNT_ATTEN： {att}");
        }
        _CalATT = att;
    }
    static internal void CalibSetAMP(double ampSet)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        ushort regL = AWG1_VOL_ADJ_QUOT_L;
        ushort regH = AWG1_VOL_ADJ_QUOT_H;

        double amp = (AMP_DEFAULT / 6.0) * ampSet;

        //var amp = AWGCaliData.AMP_DEFAULT / 5.0 * ampSet;
        //var amp = AMP_DEFAULT * rate;
        //var amp = 20479 * rate;
        if (channel != ChannelId.AWG1)
        {
            regL = AWG2_VOL_ADJ_QUOT_L;
            regH = AWG2_VOL_ADJ_QUOT_H;
        }
        //if (_CalStatus == )
        //{

        // }
        //WriteAwgBySpi(regL, (UInt16)(((UInt64)awgCaliOptions.Amp) & 0xff)); //幅度细调
        //WriteAwgBySpi(regH, (UInt16)(((UInt64)awgCaliOptions.Amp) >> 8 & 0xff));
        WriteAwgBySpi(regL, (ushort)((ulong)amp & 0xff)); //幅度细调
        WriteAwgBySpi(regH, (ushort)(((ulong)amp >> 8) & 0xff));
        Debug.WriteLine($"AWG_CNT_AMP：{ampSet} Reg:{amp}");
    }

    static internal void CalibDCDAC(ushort DCDAC_Coarse, ushort DCDAC_Fine)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        //var channel = _WfmGenCalFocusCh;
        WriteAwgBySpi(DAC124_CONFIG_DATA_L, (ushort)((ulong)DCDAC_Coarse & 0xff)); //偏置 
        WriteAwgBySpi(DAC124_CONFIG_DATA_H, (ushort)((ushort)DCDAC_Coarse >> 8));
        //Thread.Sleep(5);
        WriteAwgBySpi(DAC124_CONFIG_DATA_L, (ushort)((ulong)DCDAC_Fine & 0xff)); //偏置 
        WriteAwgBySpi(DAC124_CONFIG_DATA_H, (ushort)((ushort)DCDAC_Fine >> 8));

        //WriteAwgBySpi(DAC124_CONFIG_DATA_L, (UInt16)(((UInt64)awgCaliOptions.DCDAC_Coarse) & 0xff), false);//偏置粗调
        //WriteAwgBySpi(DAC124_CONFIG_DATA_H, (UInt16)(((UInt16)awgCaliOptions.DCDAC_Coarse) >> 8 | 0x60), false);

        //WriteAwgBySpi(DAC124_CONFIG_DATA_L, (UInt16)(((UInt64)awgCaliOptions.DCDAC_Fine) & 0xff), false);//偏置粗调
        //WriteAwgBySpi(DAC124_CONFIG_DATA_H, (UInt16)(((UInt16)awgCaliOptions.DCDAC_Fine) >> 8 | 0x60), false);

        Debug.WriteLine($"DAC124_CONFIG_REG：Coarse：{DCDAC_Coarse} (0x{DCDAC_Coarse:X})");
        Debug.WriteLine($"DAC124_CONFIG_REG：Fine：{DCDAC_Fine}(0x{DCDAC_Fine:X})");
    }

    static internal void CalibFreq(long frequency)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        _CalFreq = frequency;
        AWG_Freq_CTRL(channel, frequency, false, out _);

    }

    static internal void CalibActive(bool active)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        if (!active)
        {
            Thread.Sleep(20);
            if (channel == ChannelId.AWG1)
            {
                WriteAwgBySpi(AWG1_CNT_ATTEN, 0); //幅度挡位选择
                Debug.WriteLine($"AWG1_CNT_ATTEN： {0}");
            }
            else if (channel == ChannelId.AWG2)
            {
                WriteAwgBySpi(AWG2_CNT_ATTEN, 0); //幅度挡位选择
                Debug.WriteLine($"AWG2_CNT_ATTEN： {0}");
            }
        }
        if (channel == ChannelId.AWG1)
        {
            WriteAwgBySpi(AWG1_CH_SW, (ushort)(active ? 1 : 0));
            Debug.WriteLine($"AWG1_CAL_ACTIVE：{active}");
            HdIO.Sleep(10);
            WriteAwgBySpi(AWG1_WaveEnb, (ushort)(active ? 1 : 0));//通道开
        }
        else if (channel == ChannelId.AWG2)
        {
            WriteAwgBySpi(AWG2_CH_SW, (ushort)(active ? 1 : 0));
            Debug.WriteLine($"AWG2_CAL_ACTIVE：{active}");
            HdIO.Sleep(10);
            WriteAwgBySpi(AWG2_WaveEnb, (ushort)(active ? 1 : 0));//通道开
        }
    }

    static internal void CalibWorkMode(WfmGenMode workMode)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        ushort reg = AWG1_WORK_MODE;
        if (channel != ChannelId.AWG1)
        {
            reg = AWG2_WORK_MODE;
        }

        WriteAwgBySpi(reg, (ushort)workMode); //工作模式
        Debug.WriteLine($"AWG_CAL_MODE：{workMode}");
    }

    static internal void CalibWriteFile(ushort[] calValues)
    {
        //tst 11.3
        Debug.WriteLine("测试点192 11.3");
        for (int i = 0; i < 197; i++)
        {
            calValues[i] = (ushort)(ushort.MaxValue * 0.8);

        }
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ushort dataSetReg = AWG1_DATA_SET_CAL;
        ChannelId channel = _WfmGenCalFocusCh;
        if (channel != ChannelId.AWG1)
        {
            dataSetReg = AWG2_DATA_SET_CAL;
        }
        WriteAwgBySpi(WRITE_DATA_SEL, dataSetReg);
        WriteAwgBySpi(WRITE_DATA_WE, 0x01);
        ArbitryWaveSend(calValues);
        WriteAwgBySpi(WRITE_DATA_WE, 0x00);
        Debug.WriteLine($"WriteCAL Len:{calValues.Length}");
        //Debug.WriteLine("==================================================");
        for (int i = 0; i < 10; i++)
        {
            Debug.Write($"0x{calValues[i]:X2} ");
        }
        Debug.WriteLine("\n==================================================");
    }
    static ArbWfmCailAmpData? GetATTtoAmpMap(ushort att)
    {
        ChannelId channel = _WfmGenCalFocusCh;
        return CailAmpDatas[channel - ChannelId.AWG1].FirstOrDefault(x => x.AttLV == att);
    }

    /// <summary>
    /// 设置通道-幅度  amp\offset单位V 1MΩ
    /// </summary>
    public static void WfmGenSetAmp(ChannelId channel, double amp, double offset = 0, WfmGenImpedance impedance = WfmGenImpedance.Low50)
    {
        // <Remark>作者：彭博 创建日期：2024/3/25 13:41:00 创建原因：切换挡位需要关闭通道 </Remark>
        #region 切换挡位，弃用

        //if (_CalStatus == WfmGenCalStatus.OFF)
        //{
        //    ushort att = 0;
        //    int acLvIndex = 0;
        //    int dcLvIndex = 0;
        //    //amp /= 2;
        //    for (int i = 0; i < AWG_CAL_ATT_LEVEL_NUM; i++)
        //    {
        //        double maxVpp = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxVpp;
        //        //根据电压找到对应挡位 
        //        bool condition0 = maxVpp >= (amp + Math.Abs(offset));
        //        if (condition0)
        //        {
        //            att = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].AttLV;
        //            acLvIndex = i;
        //            //att = 1;
        //            break;
        //        }
        //    }
        //    for (int i = 0; i < AWG_CAL_ATT_LEVEL_NUM; i++)
        //    {
        //        double maxVpp = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxDC;
        //        //根据电压找到对应挡位 
        //        // < Remark > 更改人：彭博 创建日期：2024 / 3 / 21 13:49:00 原因：偏置加幅度经衰减后，会超过当前档位，应切换到下一档位 </ Remark >
        //        int param = 1;
        //        switch (CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].AttLV)
        //        {
        //            case 0: param = 1; break;
        //            case 1: param = 25; break;
        //            case 2: param = 5; break;
        //            case 3: param = 125; break;
        //            default:
        //                break;
        //        }
        //        bool condition0 = maxVpp >= (amp / 2 + Math.Abs(offset)) * param;
        //        if (condition0)
        //        {

        //            dcLvIndex = i;
        //            if (dcLvIndex >= acLvIndex && CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxVpp >= amp)
        //            {
        //                att = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].AttLV;
        //                break;
        //            }
        //            //att = 1;

        //        }
        //    }
        //    //old 23.11.3
        //    //for (int i = AWG_CAL_ATT_LEVEL_NUM - 1; i >= 0; i--)
        //    //{
        //    //    var maxVpp = CailAmpDatas[channel - ChannelId.AWG1][i].MaxVpp;

        //    //    //根据电压找到对应挡位 
        //    //    var condition0 = maxVpp >= amp * 2 + Math.Abs(offset);
        //    //    if (condition0)
        //    //    {
        //    //        att = CailAmpDatas[channel - ChannelId.AWG1][i].AttLV;
        //    //        //att = 1;
        //    //        break;
        //    //    }
        //    //}
        //    _CalATT = att;
        //    //防止意外过幅 ljw 23.2
        //    CalibSetAMP(0);
        //    CalibATT(att);
        //}

        #endregion

        if (_CalStatus == WfmGenCalStatus.Cal_AC_Verify)
        {
            impedance = WfmGenImpedance.HighZ;
            // CalibActive(true);
        }
        double MapAmp = GetMapAmpATT(out bool isSmallLv, impedance);
        double rate = amp / MapAmp;
        rate = rate > 1 ? 1 : rate;
        if (isSmallLv)
        {
            rate *= 2.025;
        }
        // 设置校准值
        if (_CalStatus == WfmGenCalStatus.Cal_AC) // || CalStatus == WfmGenCalStatus.Cal_DC)
        {
            SetMaxCalValue();

            CalibSetAMP((Constants.AWG_AMP_1M_MAX / 1000.0) * rate);
        }
        else
        {
            SetCalValue(amp);
            if (_CalStatus == WfmGenCalStatus.OFF)
            {
                CalibSetAMP((Constants.AWG_AMP_1M_MAX / 1000.0 / 2) * rate);
            }
            else
            {
                CalibSetAMP((Constants.AWG_AMP_1M_MAX / 1000.0) * rate);
            }
        }

        //test
        //SetMaxCalValue();
        //CalibSetAMP(Constants.AWG_AMP_1M_MAX / 1000.0 * 1.0);
    }

    /// <summary>
    /// 根据幅度、偏置获取挡位
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="amp">幅度</param>
    /// <param name="offset">偏置</param>
    /// <returns>设否切换挡位</returns>
    private static bool WfmGenGetATT(ChannelId channel, double amp, double offset)
    {
        ushort arbWfmGenAtt = ArbWfmGenAttDic[channel];
        if (_CalStatus == WfmGenCalStatus.OFF)
        {
            ushort att = 0;
            int acLvIndex = 0;
            int dcLvIndex = 0;
            for (int i = 0; i < AWG_CAL_ATT_LEVEL_NUM; i++)
            {
                double maxVpp = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxVpp;
                //根据电压找到对应挡位 
                bool condition0 = maxVpp >= (amp + Math.Abs(offset) * 2) || ((amp + Math.Abs(offset) * 2) > maxVpp && maxVpp == 6);//当前电压超过最大值6V，设置为最大档位
                if (condition0)
                {
                    att = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].AttLV;
                    acLvIndex = i;
                    //att = 1;
                    break;
                }
            }
            for (int i = 0; i < AWG_CAL_ATT_LEVEL_NUM; i++)
            {
                double maxVpp = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxDC;
                //根据电压找到对应挡位 
                bool condition0 = maxVpp >= Math.Abs(offset);
                if (condition0)
                {

                    dcLvIndex = i;
                    if (dcLvIndex >= acLvIndex && CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].MaxVpp >= amp)
                    {
                        att = CailAmpDatas[channel - ChannelId.AWG1][CAIL_LV_SHORT[i]].AttLV;
                        break;
                    }
                    //att = 1;

                }
            }
            _CalATT = att;
            if (arbWfmGenAtt != att || optionsChanged || _CalStatus != WfmGenCalStatus.OFF)
            {
                ArbWfmGenAttDic[channel] = att;
                AWG_CH_CTRL(channel, false);
                CalibSetAMP(0);
                CalibATT(att);
                return true;
            }
        }

        return false;
    }

    static double GetMapAmpATT(out bool inSmallmV, WfmGenImpedance impedance = WfmGenImpedance.Low50)
    {
        double amp;
        ushort toUseAtt = _CalATT;
        inSmallmV = toUseAtt == 3 && _CalStatus == WfmGenCalStatus.OFF;
        //0.2Vpp 下使用0.2Vpp档数据
        if (inSmallmV)
        {
            toUseAtt = 1;
        }
        ArbWfmCailAmpData? pAmpSWMap = GetATTtoAmpMap(toUseAtt);

        if (null == pAmpSWMap)
            amp = Constants.AWG_AMP_1M_MAX / 1000;
        else
            amp = pAmpSWMap.MaxVpp;

        //return impedance == WfmGenImpedance.Low50 ? amp / 2 : amp;
        if (inSmallmV)
        {
            //amp /= 2.5;
            amp /= 2.5;
        }
        return amp;
    }
    static double GetDcCalValue(double vDAC)
    {
        double DACValue;

        ArbWfmCailAmpData? ampSWMap = GetATTtoAmpMap(_CalATT);

        if (null != ampSWMap)
        {

            if (ampSWMap.DC != null && ampSWMap.DC.FreqOrDcVol != 0)
            {
                double vol = ampSWMap.DC.FreqOrDcVol;
                double rate = ampSWMap.DC.Rate;
                DACValue = (vol * vDAC) - (rate * 2);
            }
            else
            {
                DACValue = vDAC;
            }
        }
        else
        {
            DACValue = vDAC;
        }
        //return DACValue /= GetLoadGain(GetLoad());
        return DACValue;
    }
    static bool VerifyListIsNullOrEmpty(ICollection list)
    {
        return !(list != null && list.Count != 0);
    }
    static bool VerifyCaliData(ChannelId channelId = ChannelId.AWG1)
    {
        AWGCaliChannel VerifyData = _CalChData;
        if (_CalStatus != WfmGenCalStatus.OFF)
        {
            //校准状态
            VerifyData = _CalChData;
        }
        else
        {
            //正常状态
            VerifyData = Default.ChannelData[channelId - ChannelId.AWG1];
        }
        if (VerifyData == null || VerifyListIsNullOrEmpty(VerifyData.AC_CAL)
                               || VerifyListIsNullOrEmpty(VerifyData.AC1K_CAL) || VerifyListIsNullOrEmpty(VerifyData.DC_CAL)
           )
        {
            return false;
        }

        if (GetAcCalDataSize(VerifyData.AC1K_CAL) > 0 || GetAcCalDataSize(VerifyData.DC_CAL) > 0)
        {
            return true;
        }
        for (int attLv = 0; attLv < AWG_CAL_ATT_LEVEL_NUM; attLv++)
        {
            if (!VerifyListIsNullOrEmpty(VerifyData.AC_CAL) && GetAcCalDataSize(VerifyData.AC_CAL[attLv].AcCal) > 0)
            {
                return true;
            }
        }
        return false;
    }
    static int GetAcCalDataSize(List<AdjustValue> AVT)
    {
        return GetAcCalDataSize(AVT.ToArray());
    }
    static int GetAcCalDataSize(AdjustValue[]? AVT)
    {
        int size = 0;
        if (AVT != null)
        {
            while (size < AVT.Count() && AVT[size].FreqOrDcVol > 0) size++;
        }
        Debug.WriteLine($"GetAcCalDataSize:{size}\n");
        return size;
    }
    static void ClacCalValue(double step, double Vpp)
    {
        ushort toUseAtt = _CalATT;
        bool inSmallmV = toUseAtt == 3 && _CalStatus == WfmGenCalStatus.OFF;
        //0.2Vpp 下使用0.2Vpp档数据
        if (inSmallmV)
        {
            toUseAtt = 1;
        }
        ArbWfmCailAmpData? ampSWMap = GetATTtoAmpMap(toUseAtt);
        ArbWfmCailAmpData? refAmpSWMap;
        ACAdjustValue AmpCal;
        //#if USE_REF_CAL
        //	double Ref_rate = 1;
        //#endif
        double refB;
        if (null == ampSWMap)
        {
            for (int i = 0; i < _CalValues.Length; i++)
            {
                _CalValues[i] = AMP_DEFAULT;
            }

            return;
        }

        Vpp = Vpp / 50 / 2; //   Impedance == WfmGenImpedance.Low50 ? 50 : 25;
        if (false == ampSWMap.RefAttEnable)
        {
            refAmpSWMap = ampSWMap;
        }
        else
        {
            refAmpSWMap = GetATTtoAmpMap(ampSWMap.AttLV);
            if (null == refAmpSWMap)
                refAmpSWMap = ampSWMap;
            Vpp = (Vpp * refAmpSWMap.MaxVpp) / ampSWMap.MaxVpp;
        }

        AmpCal = new ACAdjustValue();

        //if (refAmpSWMap.AVT == null || refAmpSWMap.AVT.GetLength(0) <= ampSWMap.AttLV)
        //if (refAmpSWMap.AVT == null || refAmpSWMap.AVT.Count <= ampSWMap.AttLV)
        if (refAmpSWMap.AVT == null)
        {
            Debug.WriteLine("refAmpSWMap.AVT 校准数据长度有误，或者没有校准数据");
            return;
        }

        AmpCal = refAmpSWMap.AVT; // AmpCal[i] = refAmpSWMap.AVT[0, i];
        AmpCal.AcCal = refAmpSWMap.AVT.AcCal;


        refB = AmpCal.AcCal[0].Rate;

        LinearInterpolation(AmpCal.AcCal.ToArray(), AWG_AC_CAL_SIZE, step);

        //SetCalDataAC1K();



        if (refAmpSWMap != ampSWMap)
        {
            double k;
            AmpCal = ampSWMap.AVT;
            if (0.0 == AmpCal.AcCal[0].Rate) //???
                return;

            k = AmpCal.AcCal[0].Rate / refB; //???
            int i = 0;

            for (; i < AWG_AC_CAL_SIZE; i++)
            {
                _CalValues[i] = (ushort)(_CalValues[i] * k);
            }

        }
    }
    static void SetCalValue(double amp)
    {
        //设置理论校准最大输出为100M,校准深度为1024,所以校准查询频率步进为: 100M/1024=97.65625kHzkHz
        ClacCalValue(AWG_AC_CAL_STEP, amp);
        //WriteFPGABulkData(ch, (uint8_t*)_CalValue, 2);
        CalibWriteFile();
    }

    static void SetMaxCalValue()
    {
        int i;
        for (i = 0; i < AWG_AC_CAL_SIZE; i++)
            _CalValues[i] = AMP_DEFAULT;
        //WriteFPGABulkData(ch, (uint8_t*)_CalValue, 2);
        CalibWriteFile();
    }
    static void CalibWriteFile()
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        //if (_CalStatus == WfmGenCalStatus.Cal_DC_Verify || _CalStatus == WfmGenCalStatus.Cal_AC_Verify)
        //{
        //    if (_CalValues[0] == 0)
        //    {
        //        _CalValues[0] = _CalValues[1];
        //    }
        //}
        //test 11.3
        //Debug.WriteLine("test11.3 l482");
        //for (int i = 0; i < 197; i++)
        //{
        //    _CalValues[i] = (ushort)(ushort.MaxValue * 0.73);
        //}
        //for (int i = 197; i < _CalValues.Length; i++)
        //{
        //    _CalValues[i] = 0;
        //}
        //for (int i = 196; i < _CalValues.Length; i++)
        //{
        //    if (_CalValues[i] == 0)
        //    {
        //        _CalValues[i] = _CalValues[195];
        //    }
        //}
        ushort dataSetReg = AWG1_DATA_SET_CAL;

        if (_WfmGenCalFocusCh != ChannelId.AWG1)
        {
            dataSetReg = AWG2_DATA_SET_CAL;
        }
        WriteAwgBySpi(WRITE_DATA_SEL, dataSetReg);
        WriteAwgBySpi(WRITE_DATA_WE, 0x01);
        ArbitryWaveSend(_CalValues);
        WriteAwgBySpi(WRITE_DATA_WE, 0x00);
        Debug.WriteLine($"WriteCAL Len:{_CalValues.Length}");
        //Debug.WriteLine("==================================================");
        for (int i = 0; i < 10; i++)
        {
            Debug.Write($"0x{_CalValues[i]:X2} ");
        }
        Debug.WriteLine("\n==================================================");
    }
    /// <summary>
    /// 插入设置1K校准值
    /// </summary>
    static void SetCalDataAC1K(ChannelId channelId = ChannelId.AWG1)
    {
        if (_CalStatus == WfmGenCalStatus.OFF && VerifyCaliData(channelId))
        {
            AWGCaliChannel channelData = Default.ChannelData[channelId - ChannelId.AWG1];
            double rate = channelData.AC1K_CAL[_CalATT].Rate;
            if (rate == 0)
            {
                //校准数据无效 1K未校
                rate = 1;
            }
            _CalValues[0] = (ushort)(rate * AMP_DEFAULT);
            return;
        }
        else
        {

            _CalValues[0] = (ushort)(_CalChData.AC1K_CAL[_CalATT].Rate * AMP_DEFAULT);
        }
    }
    static void LinearInterpolation(AdjustValue[] Points, int len, double step)
    {
        int i;
        ushort ampDACV;
        //var outdata = _CalValues;
        int size = GetAcCalDataSize(Points);
        if (size <= 0)
        {
            for (i = 0; i < len; i++)
            {
                _CalValues[i] = AMP_DEFAULT; //ampDACV;
            }
            return;
        }

        else if (size == 1)
        {
            ampDACV = (ushort)(AMP_DEFAULT * Points[0].Rate);
            for (i = 0; i < len; i++)
            {
                _CalValues[i] = ampDACV;
            }
            return;
        }
        else
        {
            double temp;
            int freIndex1 = 0, freIndex2 = 0;
            int IndexCal = 0;
            int firstpart = (int)Math.Ceiling(AWG_CAL_FREQ_MIN_RANGE / AWG_AC_CAL_STEP); // 向上取整

            for (i = 0; i < len; i++)
            {
                double curFre = i * step;
                while (curFre < Points[freIndex1].FreqOrDcVol || curFre > Points[freIndex2].FreqOrDcVol)
                {
                    if (0 == IndexCal && curFre < Points[freIndex1].FreqOrDcVol)
                        break;

                    if ((size - 1) <= IndexCal)
                        break;

                    IndexCal++;
                    freIndex1 = freIndex2;
                    freIndex2 = IndexCal;
                }

                //if(0==i && fabs((Points[freIndex1].A - 1e3)<0.0001))
                if ((0 == i) && i < firstpart && Math.Abs(Points[freIndex1].FreqOrDcVol - 1e3) < 0.0001)
                {
                    temp = Points[freIndex1].Rate;
                }
                else if (curFre <= Points[freIndex1].FreqOrDcVol || Points[freIndex2].FreqOrDcVol == Points[freIndex1].FreqOrDcVol)
                {
                    temp = Points[freIndex1].Rate;
                }
                else if (curFre < Points[freIndex2].FreqOrDcVol)
                {
                    double delta = (curFre - Points[freIndex1].FreqOrDcVol) / (Points[freIndex2].FreqOrDcVol - Points[freIndex1].FreqOrDcVol);
                    temp = (Points[freIndex1].Rate * (1 - delta)) + (Points[freIndex2].Rate * delta);
                }
                else
                {
                    temp = Points[freIndex2].Rate;
                }

                temp = AMP_CAL_MAX * temp /**rate*/;
                //#if USE_REF_CAL
                //			if(refrate)
                //			temp *=refrate;
                //#endif
                _CalValues[i] = (ushort)(temp + 0.5);
                //outdata[i] = AWG_AC_CAL_SIZE;
            }
        }
    }
    static internal void SaveDataToFile()
    {
        Helper.GetICaliData(CaliDataType.AWG)?.SaveToFile();
    }
    static internal void SetCHAmpData()
    {
        ArbWfmDcCailAmpTable.SetCHAmpData(_WfmGenCalFocusCh, _CalChData);
        //_CalibDataValid = VerifyCaliData();
        Debug.WriteLine($"[ CalibDataEnabled:{VerifyCaliData()} ]");
    }
    static internal string CalibGetSysInfo()
    {
        return _Cail_SYS_INFO;
    }
    static internal string CalibGetIDN()
    {
        return _Cail_IDN;
    }
    static internal string CalibGetSN()
    {
        return _CalibSN;
    }
    static internal void CalibSetSN(string snStr)
    {
        if (string.IsNullOrWhiteSpace(snStr))
        {
            return;
        }
        _CalibSN = snStr;
    }
    static internal void CalibWfmCalSetData(WfmGenCalDataType dataType, List<string> dataStrs)
    {
        _CalChData = Default.ChannelData[_WfmGenCalFocusCh - ChannelId.AWG1];
        int length = dataStrs.Count;
        if (length < 3 || !int.TryParse(dataStrs[0], out int attLevel)
                       || attLevel > AWG_CAL_ATT_LEVEL_NUM || attLevel < 0)
        {
            return;
        }

        switch (dataType)
        {
            default:
            case WfmGenCalDataType.DC_CAL:
                if (!uint.TryParse(dataStrs[1], out uint index) || index < 0 || index > 1)
                {
                    return;
                }
                AdjustValue buffer = _CalChData.DC_CAL[attLevel];

                if (double.TryParse(dataStrs[2], out double data))
                {
                    if (index == 0)
                    {
                        buffer.FreqOrDcVol = data;
                    }
                    else
                    {
                        buffer.Rate = data;
                    }

                    _CalChData.DC_CAL[attLevel] = buffer;
                }
                break;
            case WfmGenCalDataType.AC1K_CAL:
                if (!double.TryParse(dataStrs[1], out double freqReal) || freqReal < 0)
                {
                    return;
                }
                if (!double.TryParse(dataStrs[2], out double rate) || rate < 0 || rate > 1)
                {
                    return;
                }
                else
                {
                    AdjustValue calDataBuf = new();
                    calDataBuf.FreqOrDcVol = freqReal;
                    calDataBuf.Rate = rate;
                    _CalChData.AC1K_CAL[attLevel] = calDataBuf;
                    _CalChData.AC_CAL[attLevel].AcCal[0] = calDataBuf;

                }
                break;
            case WfmGenCalDataType.AC_CAL:
                if (length < 4)
                {
                    return;
                }
                if (!double.TryParse(dataStrs[1], out freqReal) || freqReal < 0)
                {
                    return;
                }
                if (!double.TryParse(dataStrs[2], out rate) || rate < 0 || rate > 1)
                {
                    return;
                }
                if (!double.TryParse(dataStrs[3], out double amplitude) || amplitude < 0)
                {
                    return;
                }
                else
                {
                    ACAdjustValue? dataBuf = _CalChData.AC_CAL[attLevel];
                    if (dataBuf == null)
                    {
                        dataBuf = new ACAdjustValue();
                    }
                    AdjustValue acCalAcCal = new();
                    dataBuf.Amp = amplitude;
                    acCalAcCal.FreqOrDcVol = freqReal;
                    acCalAcCal.Rate = rate;
                    if (dataBuf.AcCal.Length == 0)
                    {
                        dataBuf = new ACAdjustValue();
                    }

                    int needUpdateIndex = dataBuf.AcCal.ToList().FindIndex(cal => cal != null && cal.FreqOrDcVol == freqReal);
                    if (needUpdateIndex >= 0)
                    { //update old dara
                        dataBuf.AcCal[needUpdateIndex] = acCalAcCal;
                        _CalChData.AC_CAL[attLevel] = dataBuf;
                        Debug.WriteLine($"  [update]    AC Cal Data: LV: {attLevel},RealFreq:{freqReal} Hz,FreqPoint:{needUpdateIndex},Rate:{rate}");
                    }
                    else
                    { //new data
                        int emptyIndex = dataBuf.AcCal.ToList().FindIndex(cal => cal.FreqOrDcVol == 0);
                        if (emptyIndex >= 0)
                        {
                            dataBuf.AcCal[emptyIndex] = acCalAcCal;
                            _CalChData.AC_CAL[attLevel] = dataBuf;
                            Debug.WriteLine($"  [ new ]     AC Cal Data: LV: {attLevel},RealFreq:{freqReal} Hz,FreqPoint:{emptyIndex},Rate:{rate}");
                        }
                    }
                }
                break;
        }
    }
    static internal void CalibWfmCalClearTempData()
    {
        _CalChData = new AWGCaliChannel();
        SetCHAmpData();
    }
    static internal void CalibWfmCalStatus(ChannelId channel, WfmGenCalStatus status)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        _WfmGenCalFocusCh = channel;
        _CalStatus = status;
        if (_CalStatus != WfmGenCalStatus.OFF)
        {
            ClearAWGProtect(channel);
        }
        switch (_CalStatus)
        {
            default:
            case WfmGenCalStatus.OFF:

                break;
            case WfmGenCalStatus.Cal_DC:
                CalibWorkMode(WfmGenMode.Continuous);
                CalibWfmType(ArbWfmType.DC);
                break;
            case WfmGenCalStatus.Cal_DC_Verify:
                CalibWorkMode(WfmGenMode.Continuous);
                CalibWfmType(ArbWfmType.DC);
                _CalChData = Default.ChannelData[channel - ChannelId.AWG1];
                break;
            case WfmGenCalStatus.Cal_AC:
                CalibWorkMode(WfmGenMode.Continuous);
                CalibWfmType(ArbWfmType.Sinusoid);
                break;
            case WfmGenCalStatus.Cal_AC_Verify:
                CalibWorkMode(WfmGenMode.Continuous);
                CalibWfmType(ArbWfmType.Sinusoid);
                _CalChData = Default.ChannelData[channel - ChannelId.AWG1];
                break;
        }
    }
    static internal void CalibWfmOffset(double offset)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        //if (_CalStatus == WfmGenCalStatus.OFF)
        //{
        //    offset /= 2;
        //}

        Debug.WriteLine($"CalibWfmOffset:offset:{offset},att:{_CalATT}");
        ChannelId channel = _WfmGenCalFocusCh;
        ushort attTmp = _CalATT;

        _CalOffset = offset;
        //offset = -2.5;
        //double vDAC = 2048 - offset * 4096 / AWG_AMP_1M_MAX;
        double vDAC = 0;
        ushort maxDcDAC = 4095;
        //qDebug() <<"ch:"<<ch<< " input offset :"<<offset<<"isDcCal:"<<isDcCal;
        //  vDAC /= AWG_CAL_DC_LOAD_GAIN;

        vDAC = -offset / 0.5;
        if (_CalStatus != WfmGenCalStatus.Cal_DC)
        {
            //offset = pChc.GetDcCalValue(ATT, offset);
            vDAC = GetDcCalValue(vDAC);
        }
        double gain = 2.44;

        if ((attTmp & 2) == 2) // ATT 1/5
        {
            gain /= 5;
        }
        // * Vout = (2*Vin - 4.096) * 2.2
        // pChc.GetDCGain(ATT);//normal att to -12V ~ +12V
        vDAC = vDAC / gain; //AWG_CAL_DC_LOAD_GAIN;
        vDAC = (vDAC + 4.096) / 2;

        if (vDAC < 0) vDAC = 0;
        if (vDAC > 4.095) vDAC = 4.095;
        //get dac value grate to 10mV
        ushort _DAC_Coarse = (ushort)((int)((vDAC * 1000) / AWG_CAL_DAC_COARSE_GAIN - 400 * AWG_CAL_DAC_FINE_GAIN / AWG_CAL_DAC_COARSE_GAIN));
        //int vVdac = ((int)(Vdac * 10))*100/DACCoarseGain;
        if (_DAC_Coarse > maxDcDAC)
        {
            _DAC_Coarse = maxDcDAC;
        }

        double fvVdac = (_DAC_Coarse * AWG_CAL_DAC_COARSE_GAIN) / 1000;
        ushort _DAC_Fine = (ushort)Math.Round(((vDAC - fvVdac) * 1000) / AWG_CAL_DAC_FINE_GAIN);

        if (_DAC_Coarse > maxDcDAC)
            _DAC_Coarse = maxDcDAC;
        if (_DAC_Fine > maxDcDAC)
            _DAC_Fine = maxDcDAC;

        int chAddr = 0;
        if (channel == ChannelId.AWG1)
            chAddr = 1;
        else
            chAddr = 3;
        Debug.WriteLine($"============== DCDAC =====================");
        Debug.WriteLine($"Data_DCDAC_Coarse:{_DAC_Coarse}");
        Debug.WriteLine($"Data_DCDAC_Fine:{_DAC_Fine}");
        ushort _DCDAC_Coarse = (ushort)((chAddr << 14) | (0x01 << 12) | (_DAC_Coarse & 0x0FFF));

        chAddr--;
        //chAddr++;
        ushort _DCDAC_Fine = (ushort)((chAddr << 14) | (0x01 << 12) | (_DAC_Fine & 0x0FFF));

        //Thread.Sleep(100);
        CalibDCDAC(_DCDAC_Coarse, _DCDAC_Fine);
    }
    static internal void CalibWfmType(ArbWfmType arbWfmType)
    {
        if (!Hd.CurrProduct!.HardwareConfig!.bExistAWGModule)
            return;
        ChannelId channel = _WfmGenCalFocusCh;
        ushort reg_signal = AWG1_SIGNAL_SEL;
        ushort reg_mode = AWG1_WORK_MODE;
        var dataSetReg = AWG1_DATA_SET_BASE;
        if (channel != ChannelId.AWG1)
        {
            reg_signal = AWG2_SIGNAL_SEL;
            reg_mode = AWG2_WORK_MODE;
            dataSetReg = AWG2_DATA_SET_BASE;
        }
        switch (arbWfmType)
        {
            default:
            case ArbWfmType.Sinusoid: //正弦
                WriteAwgBySpi(reg_mode, 0x00);
                WriteAwgBySpi(reg_signal, 0x00);
                WriteAwgBySpi(WRITE_DATA_SEL, dataSetReg);
                WriteAwgBySpi(WRITE_DATA_WE, 0x01);
                SineWave();
                WriteAwgBySpi(WRITE_DATA_WE, 0x00);
                break;
            case ArbWfmType.DC:
                WriteAwgBySpi(reg_signal, 0x04);
                WriteAwgBySpi(WRITE_DATA_SEL, dataSetReg);
                WriteAwgBySpi(WRITE_DATA_WE, 0x01);
                break;
        }

        Debug.WriteLine($"AWG_CAL_WFM_TYPE：{arbWfmType}");
    }

    #endregion 校准 cal

    #region scpi接口

    static void FactoryCaliAWGCMDs(ChannelId channelId, string cmd, string[] paramLst)
    {
        if (string.IsNullOrWhiteSpace(cmd) || paramLst.Length < 1)
        {
            return;
        }
        _WfmGenCalFocusCh = channelId;
        switch (cmd)
        {
            case "ATT":
                if (ushort.TryParse(paramLst[0], out ushort att))
                {
                    CalibATT(att);
                }
                break;
            case "AMP":
                if (double.TryParse(paramLst[0], out double amp))
                {
                    //if (_CalStatus == WfmGenCalStatus.Cal_DC_Verify || _CalStatus == WfmGenCalStatus.Cal_AC_Verify)
                    WfmGenSetAmp(channelId, amp, _CalOffset);
                    //else
                    //    CalibSetAMP(amp);
                }
                break;
            case "DCDAC":
                if (ushort.TryParse(paramLst[0], out ushort coarse) && ushort.TryParse(paramLst[1], out ushort fine))
                {
                    CalibDCDAC(coarse, fine);
                }
                break;
            case "FREQ":
                if (long.TryParse(paramLst[0], out long freq))
                {
                    CalibFreq(freq);
                }
                break;
            case "ACT":
                if (ushort.TryParse(paramLst[0], out ushort active))
                {
                    if (active == 0 && (_CalStatus == WfmGenCalStatus.Cal_DC_Verify || _CalStatus == WfmGenCalStatus.Cal_AC_Verify))
                    {
                        active = 1;
                    }
                    CalibActive(active == 1);
                }
                break;
            case "MODE":
                if (ushort.TryParse(paramLst[0], out ushort mode))
                {
                    CalibWorkMode((WfmGenMode)mode);
                }
                break;

            case "FILE":
                int length = paramLst.Length;
                ushort[] datas = new ushort[length];
                for (int i = 0; i < length; i++)
                {
                    if (!ushort.TryParse(paramLst[i], out ushort data))
                    {
                        break;
                    }
                    datas[i] = data;
                }
                CalibWriteFile(datas);
                break;
            case "TYPE":
                if (ushort.TryParse(paramLst[0], out ushort type))
                {
                    CalibWfmType((ArbWfmType)type);
                }
                break;
            case "OFFSET":
                if (double.TryParse(paramLst[0], out double offset))
                {
                    //Debug.WriteLine($"Test P00: offset:{offset}");
                    CalibWfmOffset(offset);
                }
                break;
            case "STATUS":
                if (int.TryParse(paramLst[0], out int status) && Enum.IsDefined(typeof(WfmGenCalStatus), status))
                {
                    CalibWfmCalStatus(channelId, (WfmGenCalStatus)status);
                    Debug.WriteLine($"Test P947: status:{status}");
                }
                break;
            case "CLEAR_CAL_TEMP_DATA":
                CalibWfmCalClearTempData();
                break;

            case "DC_CAL":
                CalibWfmCalSetData(WfmGenCalDataType.DC_CAL, paramLst.ToList());
                break;
            case "AC1K_CAL":
                CalibWfmCalSetData(WfmGenCalDataType.AC1K_CAL, paramLst.ToList());
                break;
            case "AC_CAL":
                CalibWfmCalSetData(WfmGenCalDataType.AC_CAL, paramLst.ToList());
                break;
            case "CAIL_SN_SET":
                CalibSetSN(paramLst[0]);
                break;
            case "CAIL_SET_AMP_DATA":
                SetCHAmpData();
                Default.UpdateDatas(_WfmGenCalFocusCh, _CalChData);
                //SaveDataToFile();
                break;
            case "CAIL_SAVE_DATA":

                _CalStatus = WfmGenCalStatus.OFF;
                SaveDataToFile();
                break;
        }
    }
    static internal HdCmd ExecSetCommand(int commasIndex, string key, string value)
    {
        string cmd = "";
        string[] patams;
        commasIndex = value.IndexOf(',');
        if (commasIndex < 0)
        {
            return HdCmd.None;
        }
        else
        {
            cmd = value.Substring(0, commasIndex).Trim();
        }
        patams = value.Substring(commasIndex + 1).Split(',');
        Debug.WriteLine($"========== Test:{cmd}");
        switch (key)
        {
            case "CalibAWG1":

                FactoryCaliAWGCMDs(ChannelId.AWG1, cmd, patams);
                return HdCmd.None;
            case "CalibAWG2":
                FactoryCaliAWGCMDs(ChannelId.AWG2, cmd, patams);
                return HdCmd.None;
            case "CalibAWG3":
                FactoryCaliAWGCMDs(ChannelId.AWG3, cmd, patams);
                return HdCmd.None;
            case "CalibAWG4":
                FactoryCaliAWGCMDs(ChannelId.AWG4, cmd, patams);
                return HdCmd.None;
            case "CalibAWG255":
                FactoryCaliAWGCMDs(ChannelId.AWG1, cmd, patams);
                FactoryCaliAWGCMDs(ChannelId.AWG2, cmd, patams);
                return HdCmd.None;
        }
        return HdCmd.None;
    }

    #endregion scpi接口

    #region 初始化

    #endregion 初始化
}
