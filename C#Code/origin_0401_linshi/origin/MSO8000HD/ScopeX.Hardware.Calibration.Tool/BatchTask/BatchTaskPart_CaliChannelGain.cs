using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.IO;
using CSScripting;
using System.Threading.Channels;
using System.Xml;
using ScopeX.Hardware.Calibration.Tool.Utilities;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_CaliChannelGain : BatchTaskPartBase
    {
        enum AutoCaliAmpStatus
        {
            Working = 1,
            Failure = 2,
            Ok = 3
        }
        enum AutoCaliAmpStage
        {
            DSA = 0,
            ADC = 1,
            FPGA = 2,
        }

        enum AmpMeasureType
        {
            AMPL,
            STDD,
            CACL,
        }
        
        /// <summary>
        /// 校准通道的信息 
        /// </summary>
        class CaliChnlInfo
        {
            public int channelID;
            public int YLevelID;
            public int Impedance;
            public uint LastUpperOffsetCtrlWord = 0;
            public uint LastLowerOffsetCtrlWord = 0;
            public double AdcPerStepChangedPercent = 0.1;
            public (uint reg, double err)? DsaLastSetting = null;
            public (uint reg, double err)? AdcLastSetting = null;
            public (uint reg, double err)? FpgaLastSetting = null;
            public (uint Dsa, uint Adc, uint Fpga) CaliTimes = (0, 0, 0);

            public List<long> MeasureValueUvs = new List<long>();

            public AutoCaliAmpStage Stage = AutoCaliAmpStage.DSA;
            public AutoCaliAmpStatus Status = AutoCaliAmpStatus.Working;

            public AnalogChannelItem_Base analogChannelItem_Base;
            public ChnlParamsKeyMap chnlParams;
            public string paramName;

            public uint Cali_CoarseControlWord
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Gain;
                    //return ChannelParams.Default[channelID, Impedance, YLevelID].Gain_CoarseCtrlWord;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Gain", (uint)value, paramName);
                    //由于 ChannelPerScaleItem 是struct ,必须用值赋值的方式
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, YLevelID];
                    //perScaleItem.Gain_CoarseCtrlWord = value;
                    //ChannelParams.Default[channelID, Impedance, YLevelID] = perScaleItem;
                }
            }
            public uint Cali_ChannelOffset
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Offset;
                    //return ChannelParams.Default[channelID, Impedance, YLevelID].OffsetPosterior;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Offset", (uint)value, paramName);
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, YLevelID];
                    //perScaleItem.OffsetPosterior = value;
                    //ChannelParams.Default[channelID, Impedance, YLevelID] = perScaleItem;
                }
            }
            public uint Cali_ADCFineControlWord
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Gain_FineByTenThousandByAdc1;
                    //return ChannelParams.Default[channelID, Impedance, YLevelID].Gain_FineByAdc;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Gain_FineByTenThousandByAdc1", (uint)value, paramName);
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, YLevelID];
                    //perScaleItem.Gain_FineByAdc = value;
                    //ChannelParams.Default[channelID, Impedance, YLevelID] = perScaleItem;
                }
            }
            public int Cali_FPGAFinegainWord
            {
                get
                {
                    return ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Gain_FineByFpgaThousand;
                    //return ChannelParams.Default[channelID, Impedance, YLevelID].Gain_FineByFpgaThousand;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Gain_FineByFpgaThousand", (uint)value, paramName);
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, YLevelID];
                    //perScaleItem.Gain_FineByFpgaThousand = value;
                    //ChannelParams.Default[channelID, Impedance, YLevelID] = perScaleItem;
                }
            }

            /// <summary>
            /// 固定粗调增益值
            /// </summary>
            public void FixCoarse()
            {
                Cali_CoarseControlWord = GetDefaultCoarse();
            }

            /// <summary>
            /// 获取通道在不同阻抗、垂直档位下的默认粗调增益值
            /// </summary>
            /// <returns></returns>
            public uint GetDefaultCoarse()
            {
                if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X)
                {
                    return (Impedance, YLevelID) switch
                    {
                        (1, 1) => 0,    //(ZLOW,1Mv)
                        (1, 2) => 4,    //(ZLOW,2Mv)
                        (1, 3) => 12,   //(ZLOW,5Mv)
                        (1, 4) => 18,   //(ZLOW,10Mv)00
                        (1, 5) => 24,   //(ZLOW,20Mv)
                        (1, 6) => 18,   //(ZLOW,50Mv)
                        (1, 7) => 24,   //(ZLOW,100Mv)
                        (1, 8) => 15,   //(ZLOW,200Mv)
                        (1, 9) => 23,   //(ZLOW,500Mv)
                        (1, 10) => 29,  //(ZLOW,1v)

                        (0, 1) => 0,    //(ZHIGH,1Mv)
                        (0, 2) => 3,    //(ZHIGH,2Mv)
                        (0, 3) => 11,   //(ZHIGH,5Mv)
                        (0, 4) => 17,   //(ZHIGH,10Mv)
                        (0, 5) => 23,   //(ZHIGH,20Mv)
                        (0, 6) => 31,   //(ZHIGH,50Mv)
                        (0, 7) => 18,   //(ZHIGH,100Mv)
                        (0, 8) => 24,   //(ZHIGH,200Mv)
                        (0, 9) => 31,   //(ZHIGH,500Mv)
                        (0, 10) => 32,   //(ZHIGH,1v)
                        (0, 11) => 18,   //(ZHIGH,2v)
                        (0, 12) => 26,   //(ZHIGH,5v)
                        (0, 13) => 32,   //(ZHIGH,10v)
                        _ => 32,
                    };
                }
                else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X)
                {
                    if (ServerDomainConstants.AnaChnlType == AnaChnlType.ANA_8G)
                    {
                        return (Impedance, YLevelID) switch
                        {
                            (_, 1) => 1,    //(ZLOW,1Mv)
                            (_, 2) => 1,    //(ZLOW,2Mv)
                            (_, 3) => 5,   //(ZLOW,5Mv)   
                            (_, 4) => 14,   //(ZLOW,10Mv)  
                            (_, 5) => 24,   //(ZLOW,20Mv)  
                            (_, 6) => 1,   //(ZLOW,50Mv)  
                            (_, 7) => 15,   //(ZLOW,100Mv) 
                            (_, 8) => 1,   //(ZLOW,200Mv) 
                            (_, 9) => 10,   //(ZLOW,500Mv) 
                            (_, 10) => 22,  //(ZLOW,1v)    
                        };
                    }
                    else
                    {
                        //5GV3
                        return (Impedance, YLevelID) switch
                        {
                            (1, 1) => 68,    //(ZLOW,1Mv)   
                            (1, 2) => 68,    //(ZLOW,2Mv)   
                            (1, 3) => 60,   //(ZLOW,5Mv)   
                            (1, 4) => 48,   //(ZLOW,10Mv)  
                            (1, 5) => 37,   //(ZLOW,20Mv)  
                            (1, 6) => 21,   //(ZLOW,50Mv)  
                            (1, 7) => 21,   //(ZLOW,100Mv) 
                            (1, 8) => 22,   //(ZLOW,200Mv) 
                            (1, 9) => 30,   //(ZLOW,500Mv) 
                            (1, 10) => 18,  //(ZLOW,1v)    

                            (0, 1) => 68,    //(ZHIGH,1Mv)   
                            (0, 2) => 68,    //(ZHIGH,2Mv)   
                            (0, 3) => 60,   //(ZHIGH,5Mv)  
                            (0, 4) => 49,   //(ZHIGH,10Mv) 
                            (0, 5) => 37,   //(ZHIGH,20Mv) 
                            (0, 6) => 21,   //(ZHIGH,50Mv) 
                            (0, 7) => 45,   //(ZHIGH,100Mv)
                            (0, 8) => 33,   //(ZHIGH,200Mv)
                            (0, 9) => 33,   //(ZHIGH,500Mv)
                            (0, 10) => 21,   //(ZHIGH,1v)  
                            (0, 11) => 44,   //(ZHIGH,2v)  
                            (0, 12) => 30,   //(ZHIGH,5v)  
                            (0, 13) => 20,   //(ZHIGH,10v) 
                            _ => 32,
                        };
                        //5GV2
                        //return (Impedance, YLevelID) switch
                        //{
                        //    (1, 1) => Cali_CoarseControlWord,    //(ZLOW,1Mv)    (0,0)
                        //    (1, 2) => Cali_CoarseControlWord,    //(ZLOW,2Mv)    (0,0)
                        //    (1, 3) => 22,   //(ZLOW,5Mv)    (5.5，0）
                        //    (1, 4) => 46,   //(ZLOW,10Mv)   (5.5，6）
                        //    (1, 5) => 72,   //(ZLOW,20Mv)   (5.5，12）
                        //    (1, 6) => 15,   //(ZLOW,50Mv)   (3.75，0）
                        //    (1, 7) => 18,   //(ZLOW,100Mv)  (4.5, 0)
                        //    (1, 8) => 16,   //(ZLOW,200Mv)  (4,0)
                        //    (1, 9) => 16,   //(ZLOW,500Mv)  (4,0)
                        //    (1, 10) => 40,  //(ZLOW,1v)     (7,3)

                        //    (0, 1) => Cali_CoarseControlWord,    //(ZHIGH,1Mv)   (0,0)
                        //    (0, 2) => Cali_CoarseControlWord,    //(ZHIGH,2Mv)   (0,0)
                        //    (0, 3) => 25,   //(ZHIGH,5Mv)   (6.25,0)
                        //    (0, 4) => 49,   //(ZHIGH,10Mv)  (6.25,6)
                        //    (0, 5) => 73,   //(ZHIGH,20Mv)  (6.25,12)
                        //    (0, 6) => 22,   //(ZHIGH,50Mv) (4.5,0)
                        //    (0, 7) => 42,   //(ZHIGH,100Mv) (7.5,3)
                        //    (0, 8) => 22,   //(ZHIGH,200Mv) (4.5,0)
                        //    (0, 9) => 50,   //(ZHIGH,500Mv) (8.5,4)
                        //    (0, 10) => 22,   //(ZHIGH,1v)  (4.5,0)
                        //    (0, 11) => 42,   //(ZHIGH,2v)   (7.5,3)
                        //    (0, 12) => 26,   //(ZHIGH,5v)   (6.5,0)
                        //    (0, 13) => 50,   //(ZHIGH,10v) (8.5,4)
                        //    _ => 32,
                        //};
                    }
                }

                return 0;
            }
        }

        private const Int32 EXTRASTATICTIMES = 0;
        private string _ChnlsText = "";
        private List<CaliChnlInfo> _CaliChannels = new List<CaliChnlInfo>();
        private double _SourceAmpVoltByuV = 0;
        private double _InputLevelBymV = 10000;
        private (bool Dsa, bool Adc, bool Fpga) _IsIncludeStage = (false, false, false);
        private double _DSAStepPercent = 1;
        private Boolean IsDSAPositive = false;//false:表示衰减；true:表示放大
        private double _ADCStepPercent = 0.1;
        private int _FPGAYLevelID = 1;
        private int _HardwareValidMs = 300;
        private double _TotalErrorByPercent = 1.0;
        private (uint Min, uint Max) _DSACtrlWord = (0, 32);
        private (uint Min, uint Max) _ADCCtrlWord = (0, 255);
        private (int Min, int Max) _FpgaCtrlWord = (200, 5000);
        private int _NeedStaticTimes = 3;
        private AmpMeasureType _AmpMeasureType = AmpMeasureType.AMPL;//默认使用参数测量"幅度"
        long _UniqueId = 0;

        public override string FuncionDescription
        {
            get => $"通道增益校准";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数: 表示要校准的通道，编号从1开始，可多个通道以'|'分隔；{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示高阻还是低阻，LOW表示低阻，HIGH表示高阻；{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示垂直档位，以mV为单位，浮点数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示输入信号的幅度值，以V为单位，浮点数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示包含的校准阶段，包括 ：Dsa_Adc_Fpga , Dsa_Adc , Fpga等{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示校准值生效时间，用ms表示{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 统计次数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示总体误差判据百分比，浮点数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示DSA每个控制字幅度改变百分比，浮点数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示ADC每个控制字幅度改变百分比，浮点数{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示数字调节档位{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示DSA控制字的最小值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示DSA控制字的最大值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示ADC控制字最小值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示ADC控制字最大值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示FPGA细调的最小值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示FPGA细调的最大值{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数: 表示幅度值的获取方式,AMPL / STDD / CACL{System.Environment.NewLine}";
            }
        }
        public override string Example
        {
            get => "BatchTaskPart_CaliChannelGain 1|2, LOW, 10, 0.06, Dsa_Adc_Fpga, 200, 2, 0.5, 12.2, 0.06, 1, 0, 32, 0, 255, 200, 5000, STDD";
        }

        private void GotoNextStage(CaliChnlInfo chnl, double currErrorByPercent)
        {
            switch (chnl.Stage, _IsIncludeStage.Adc, _IsIncludeStage.Fpga)
            {
                case (AutoCaliAmpStage.DSA, true, _)://!第三个参数是否适配true/false
                    chnl.Stage = AutoCaliAmpStage.ADC;
                    break;
                case (AutoCaliAmpStage.DSA, false, true):
                case (AutoCaliAmpStage.ADC, _, true):
                    chnl.Stage = AutoCaliAmpStage.FPGA;
                    break;
                default: //没有下一个Stage
                    chnl.Status = (Math.Abs(currErrorByPercent) < _TotalErrorByPercent) ? AutoCaliAmpStatus.Ok : AutoCaliAmpStatus.Failure;
                    break;
            };
        }

        private bool AnalyParameter(string parameter)
        {
            int argIndex = 0;
            int neccessaryArgCount = 17;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < neccessaryArgCount)
                return false;

            #region 解析参数列表

            //[0]:channel list
            _ChnlsText = paramList[argIndex++];
            string[] channelIDList = _ChnlsText.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelIDList.Length == 0)
                return false;

            //[1]:高低阻
            int impedance = paramList[argIndex++].ToUpper() switch
            {
                "HIGH" => 0,
                _ => 1
            };

            //[2]:幅度档
            _InputLevelBymV = BaseHelper.TryConvertToDouble(paramList[argIndex++]);
            int inputLevelID = Array.IndexOf<Int32>(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.ToArray(), (int)(_InputLevelBymV * 1000));
            if (inputLevelID < 0)
                return false;

            //[3]:输入信号的幅度，用V为单位
            _SourceAmpVoltByuV = BaseHelper.TryConvertToDouble(paramList[argIndex++]) * 1000000;
            if (_SourceAmpVoltByuV < 0)
                return false;

            //[4]:校准阶段
            string caliStage = paramList[argIndex++].ToUpper();
            _IsIncludeStage = (caliStage.Contains("DSA"), caliStage.Contains("ADC"), caliStage.Contains("FPGA"));

            //[5]:校准值生效时间
            _HardwareValidMs = int.Parse(paramList[argIndex++]);

            //[6]:统计次数，添加额外次数
            _NeedStaticTimes = int.Parse(paramList[argIndex++]) + EXTRASTATICTIMES;

            //[7]:总体误差判据百分比
            _TotalErrorByPercent = double.Parse(paramList[argIndex++]);

            //[8]:表示DSA每个控制字幅度改变百分比，浮点数
            _DSAStepPercent = double.Parse(paramList[argIndex++]);
            IsDSAPositive = _DSAStepPercent >= 0;
            _DSAStepPercent = Math.Abs(_DSAStepPercent);
            //[9]:表示ADC每个控制字幅度改变百分比，浮点数
            _ADCStepPercent = double.Parse(paramList[argIndex++]);

            //[10]:表示数字调节档位
            _FPGAYLevelID = int.Parse(paramList[argIndex++]);

            //[11]:表示DSA控制字的最小值;
            //[12]:表示DSA控制字的最大值;
            _DSACtrlWord = (uint.Parse(paramList[argIndex++]), uint.Parse(paramList[argIndex++]));

            //[13]:表示ADC控制字的最小值;
            //[14]:表示ADC控制字的最大值;
            _ADCCtrlWord = (uint.Parse(paramList[argIndex++]), uint.Parse(paramList[argIndex++]));

            //[15]:表示FPGA细调的最小值
            //[16]:表示FPGA细调的最大值
            _FpgaCtrlWord = (int.Parse(paramList[argIndex++]), int.Parse(paramList[argIndex++]));

            //[17]:表示幅度值的获取方式
            if (paramList.Length > neccessaryArgCount)
            {
                string tempType = paramList[argIndex++].ToUpper().Trim();
                switch (tempType)
                {
                    case "AMPL":
                        _AmpMeasureType = AmpMeasureType.AMPL;
                        break;
                    case "STDD":
                        _AmpMeasureType = AmpMeasureType.STDD;
                        break;
                    case "CACL":
                        _AmpMeasureType = AmpMeasureType.CACL;
                        break;
                    default:
                        return false;
                }
            }


            #endregion 解析参数列表

            //初始化校准通道集合
            foreach (string channelId in channelIDList)
            {
                int chnlId = BaseHelper.TryConvertToInt(channelId);
                if (chnlId <= 0 || chnlId > ServerDomainConstants.AnalogChannelCount)
                    return false;

                CaliChnlInfo perChannel = new CaliChnlInfo();
                perChannel.LastUpperOffsetCtrlWord = (uint)_DacOffset.max;
                perChannel.LastLowerOffsetCtrlWord = (uint)_DacOffset.min;
                perChannel.channelID = chnlId - 1;
                perChannel.YLevelID = inputLevelID;
                perChannel.Impedance = impedance;
                perChannel.chnlParams = new ChnlParamsKeyMap((ChannelId)perChannel.channelID, impedance == 0 ? true : false, (uint)_InputLevelBymV);
                perChannel.paramName = ProductDataTranslate_MSO8000X.GenerateChnlParamsKey(perChannel.chnlParams);
                _CaliChannels.Add(perChannel);
            }

            return (_CaliChannels.Count != 0);
        }

        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        /// <summary>
        /// 计算方波幅度的算法
        /// </summary>
        /// <param name="chnlData"></param>
        /// <returns></returns>
        private double? ClacSquareAmpByUv(ushort[] chnlData)
        {
            int period = chnlData.Length / 2;   //默认通道数据包含两个周期
            int discard = period / 10;          //丢点数

            // 丢点过小,周期点过小
            if (discard <= 10)
                return null;
            // 周期数过小
            if (chnlData.Length <= 1.5 * period)
                return null;

            int avrg = 0;
            int start = discard;
            int end = chnlData.Length - discard;

            ushort min = chnlData[start];
            ushort max = chnlData[start];
            for (int i = start; i < end; i++)
            {
                ushort v = chnlData[i];
                avrg += v;
                if (min > v)
                    min = v;
                if (max < v)
                    max = v;
            }
            // 信号过小
            if ((max - min) < ServerDomainConstants.SAMPS_PER_YDIV / 2)
                return null;

            avrg /= (chnlData.Length - discard);

            int sum_max = 0;
            int sum_min = 0;
            int num_max = 0;
            int num_min = 0;

            for (int i = start; i < end; i++)
            {
                ushort v = chnlData[i];
                if ((chnlData[i - discard] - avrg) * (chnlData[i + discard] - avrg) <= 0)
                    continue;

                if (v > avrg)
                {
                    sum_max += v;
                    num_max++;
                }
                else
                {
                    sum_min += v;
                    num_min++;
                }
            }

            double retMin = (num_min > 0) ? ((double)sum_min / (double)num_min) : 0;
            double retMax = (num_max > 0) ? ((double)sum_max / (double)num_max) : 0;

            return (retMax - retMin) / ServerDomainConstants.SAMPS_PER_YDIV * _InputLevelBymV * 1000;
        }

        /// <summary>
        /// 设置参数测量的幅度
        /// </summary>
        /// <param name="chnlId"></param>
        private void SetAmpByMeasure(int chnlId)
        {
            int itemId = chnlId;

            currInstrumentSession?.WriteString($":MEASure:ITEM{itemId}:SOURce C{chnlId}");
            currInstrumentSession?.WriteString($":MEASure:ITEM{itemId}:TYPe {_AmpMeasureType.ToString()}");
            currInstrumentSession?.WriteString($":MEAS:ITEM{itemId}:DISP ON");
        }

        /// <summary>
        /// 获取参数测量的幅度
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>结果值单位为Uv</returns>
        private double GetAmpByMeasure(int itemId)
        {
            //获取测量数据
            currInstrumentSession!.WriteString($":MEASure:ITEM{itemId}:VAL?");
            string measureresultstr = currInstrumentSession!.ReadShortString();
            while (measureresultstr.Trim() == "")
            {
                currInstrumentSession!.WriteString($":MEASure:ITEM{itemId}:VAL?");
                measureresultstr = currInstrumentSession!.ReadShortString();
            }
            Double measureresult = 0;
            if (!Double.TryParse(measureresultstr, out measureresult))
            {
                LogInfo($"获取参数测量值异常，result:{measureresult}");
            }
            return measureresult * (_AmpMeasureType == AmpMeasureType.STDD ? 2 : 1) * 1000_000;
        }

        private void LogInfo(string info)
        {
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliChannelGain", info);
        }
        public override void SetDebugVariantStatus(bool status)
        {
            CommonMethod.Set_CorrectTiAdc(currInstrumentSession, status);
            CommonMethod.SetChannelDelay(currInstrumentSession, status);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            //初始化
            long maxWaitMilliseconds = (overtimeOfSecond > 0) ? (long)(1000 * overtimeOfSecond) : (60 * 1000);
            message = string.Empty;
            StringBuilder messageSB = new StringBuilder();
            Stopwatch stopwatch = new Stopwatch();

            //!test record 
            List<ushort[]> recordData = new List<ushort[]>();


            //初始化通道设置
            foreach (CaliChnlInfo chnl in _CaliChannels)
            {
                if (_AmpMeasureType != AmpMeasureType.CACL)
                    SetAmpByMeasure(chnl.channelID + 1);

                chnl.Stage = AutoCaliAmpStage.DSA;
                if (!_IsIncludeStage.Dsa)
                    GotoNextStage(chnl, 100);

                //初始化粗调，adc细调，fpga细调增益值
                chnl.AdcPerStepChangedPercent = _ADCStepPercent;
                chnl.FixCoarse();
                chnl.Cali_ADCFineControlWord = 128;

                if (chnl.YLevelID > _FPGAYLevelID)
                    chnl.Cali_FPGAFinegainWord = _FpgaCtrlWord.Max > 0 ? 1000 : -1000;
            }

            _UniqueId = DateTime.Now.Ticks;
            LogInfo($"BatchTaskPart_CaliChannelGain_Start:Impedance_{_CaliChannels[0].Impedance};" +
                $"inputLevelBymV_{_InputLevelBymV};Chnls_{_ChnlsText}");

            Thread.Sleep(1000); //等待信号源输出稳定

            ///开始校准
            stopwatch.Start();
            while (_CaliChannels.Any(ch => ch.Status == AutoCaliAmpStatus.Working))
            {
                //取消处理
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    message = $"(ExecuteId={_UniqueId};)任务被取消！";
                    LogInfo(message);
                    return BatchTaskPartResult.Cancel;
                }

                if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds)
                {
                    message = $"(ExecuteId={_UniqueId};)超时退出";
                    LogInfo(message);
                    return BatchTaskPartResult.ErrorOvertime;
                }

                //校准数据生效
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                Thread.Sleep(_HardwareValidMs * 4);


                #region 获取通道波形幅度
                for (int staticIndex = 0; staticIndex < _NeedStaticTimes; staticIndex++)
                {
                    Thread.Sleep(1500);

                    List<ushort[]>? allChannelData = new List<ushort[]>();
                    if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X)
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession);
                    else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X)
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession, ServerDomainConstants.PerAdcCoreDataCount);


                    Int32 rereadtimes = 0;

                    while ((allChannelData == null || allChannelData.Count == 0) && rereadtimes < 10)
                    {
                        //InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                        Thread.Sleep(600);
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession, ServerDomainConstants.PerAdcCoreDataCount);
                        rereadtimes++;
                    }
                    if (allChannelData == null || allChannelData.Count == 0)
                    {
                        message = $"(ExecuteId={_UniqueId};)数据读取错误！";
                        LogInfo(message);
                        return BatchTaskPartResult.ErrorFatal;
                    }
                    recordData = allChannelData;

                    foreach (CaliChnlInfo channel in _CaliChannels)
                    {
                        if (channel.Status != AutoCaliAmpStatus.Working)
                            continue;

                        if (staticIndex == 0)
                            channel.MeasureValueUvs.Clear();

                        if (IsOverStep(allChannelData[channel.channelID], Math.Abs(channel.Cali_FPGAFinegainWord) / 1000D))
                        {
                            LogInfo($"[CH{channel.channelID + 1}]:第{staticIndex + 1}次采集Failure,0Div位置不对或通道增益异常！");
                            if (channel.YLevelID > _FPGAYLevelID)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            else
                            {
                                channel.Cali_FPGAFinegainWord = _FpgaCtrlWord.Max > 0 ? 1000 : -1000;
                            }
                        }
                        else
                        {
                            ushort[] data = allChannelData[0].ToArray();
                            double centerValue = allChannelData[channel.channelID].Average(d => d) - Math.Pow(2, ServerDomainConstants.AdcBits) / 2;
                            if (Math.Abs(centerValue) < ServerDomainConstants.SAMPS_PER_YDIV * 1)
                            {
                                double? measureResultByUv = null;
                                if (_AmpMeasureType == AmpMeasureType.CACL)
                                    measureResultByUv = ClacSquareAmpByUv(allChannelData[channel.channelID]);
                                else
                                    measureResultByUv = GetAmpByMeasure(channel.channelID + 1);

                                if (measureResultByUv == null)
                                {
                                    LogInfo($"[CH{channel.channelID + 1}]:第{staticIndex + 1}次采集Failure,获取幅度值出错！");
                                    Thread.Sleep(500);
                                    continue;
                                }
                                else
                                {
                                    double errorByPercent = (measureResultByUv.Value - _SourceAmpVoltByuV) / _SourceAmpVoltByuV;
                                    //if (Math.Abs(errorByPercent) > 0.25)
                                    if (false)
                                    {
                                        //此时认为当前测试值是无效的
                                        LogInfo($"[CH{channel.channelID + 1}]:第{staticIndex + 1}次采集Failure,幅度值不在正常范围内");

                                        double? refValue = ClacSquareAmpByUv(allChannelData[channel.channelID]);
                                        if (refValue != null)
                                        {
                                            LogInfo($"[CH{channel.channelID + 1}]:第{staticIndex + 1}次采集," +
                                                $"measureResultByUv_{measureResultByUv.Value},refValue_{refValue.Value}");
                                        }
                                        Thread.Sleep(500);
                                        continue;
                                    }
                                }

                                channel.MeasureValueUvs.Add((long)measureResultByUv.Value);
                            }
                            else
                            {
                                LogInfo($"[CH{channel.channelID + 1}]:第{staticIndex + 1}次采集Failure,0Div位置不对！");
                                Thread.Sleep(500);
                                continue;
                            }
                        }
                    }
                }
                #endregion

                ///调整增益设置 
                foreach (CaliChnlInfo channel in _CaliChannels)
                {
                    if (channel.Status != AutoCaliAmpStatus.Working)
                        continue;

                    if (channel.MeasureValueUvs.Count <= EXTRASTATICTIMES)
                    {
                        channel.Status = AutoCaliAmpStatus.Failure;
                        LogInfo($"[CH{channel.channelID + 1}]:{channel.Status},幅度有效统计次数为{channel.MeasureValueUvs.Count - EXTRASTATICTIMES}!");
                        continue;
                    }
                     
                    Int64 amp = (Int64)CommonMethod.MiddleDataFilter(channel.MeasureValueUvs, channel.MeasureValueUvs.Count() - EXTRASTATICTIMES).Average();
                    Int64 errorDelta = amp - (Int64)_SourceAmpVoltByuV;
                    Double errorByPercent = errorDelta * 1.0 * 100 / _SourceAmpVoltByuV;

                    //注意判断OK的条件//todo AutoCaliAmpStage.ADC
                    if (Math.Abs(errorByPercent) < _TotalErrorByPercent && channel.Stage != AutoCaliAmpStage.ADC)
                    {
                        channel.Status = AutoCaliAmpStatus.Ok;
                        LogInfo($"[CH{channel.channelID + 1}] OK: " +
                            $" errorByPercent_{errorByPercent}%;" +
                            $" Cali_CoarseControlWord_{channel.Cali_CoarseControlWord};" +
                            $" Cali_ADCFineControlWord_{channel.Cali_ADCFineControlWord};" +
                            $" Cali_FPGAFinegainWord_{channel.Cali_FPGAFinegainWord};");
                        continue;
                    }

                    #region 不同阶段进行校准调整
                    if (channel.Stage == AutoCaliAmpStage.DSA)
                    {
                        LogInfo($"[CH{channel.channelID + 1}] DsaStage: errorDeltaUv_{errorDelta};errorByPercent_{errorByPercent};" +
                            $"Cali_CoarseControlWord_{channel.Cali_CoarseControlWord}");

                        if (Math.Abs(errorByPercent) > _DSAStepPercent / 2)
                        {
                            if (channel.DsaLastSetting != null)
                            {
                                //要考虑震荡的情况
                                if (Math.Abs(errorByPercent) < _DSAStepPercent && Math.Abs(channel.DsaLastSetting.Value.err) < _DSAStepPercent)
                                {
                                    if (Math.Abs(errorByPercent) > Math.Abs(channel.DsaLastSetting.Value.err))
                                    {
                                        channel.Cali_CoarseControlWord = channel.DsaLastSetting.Value.reg;
                                    }
                                    LogInfo($"[CH{channel.channelID + 1}]，Dsa调整道震荡状态。");
                                    GotoNextStage(channel, channel.DsaLastSetting.Value.err);
                                }
                            }
                            channel.DsaLastSetting = (channel.Cali_CoarseControlWord, errorByPercent);

                            Int32 dsapositive = IsDSAPositive ? 1 : -1;
                            //set new ctrl word
                            int ctrlWordDelta = dsapositive * (int)Math.Round((Math.Log10(_SourceAmpVoltByuV / amp) * 20) / (Math.Log10(1 + _DSAStepPercent / 100) * 20));
                            int newCtrlWord = (int)channel.Cali_CoarseControlWord + ctrlWordDelta;

                            if (newCtrlWord < _DSACtrlWord.Min)
                            {
                                channel.Cali_CoarseControlWord = _DSACtrlWord.Min;
                                LogInfo($"[CH{channel.channelID + 1}]，粗调已到最小值。");
                                GotoNextStage(channel, channel.DsaLastSetting.Value.err);
                            }
                            else if (newCtrlWord > _DSACtrlWord.Max)
                            {
                                channel.Cali_CoarseControlWord = _DSACtrlWord.Max;
                                LogInfo($"[CH{channel.channelID + 1}]，粗调已到最大值。");
                                GotoNextStage(channel, channel.DsaLastSetting.Value.err);
                            }
                            else
                            {
                                channel.Cali_CoarseControlWord = (uint)newCtrlWord;
                                LogInfo($"[CH{channel.channelID + 1}]，DSA调整为{channel.Cali_CoarseControlWord}。");
                            }
                            channel.CaliTimes.Dsa++;
                            continue;
                        }
                        else
                            GotoNextStage(channel, errorByPercent);
                    }
                    else if (channel.Stage == AutoCaliAmpStage.ADC)
                    {
                        LogInfo($"[CH{channel.channelID + 1}] AdcStage: errorDeltaUv_{errorDelta};errorByPercent_{errorByPercent};" +
                            $"Cali_ADCFineControlWord_{channel.Cali_ADCFineControlWord}");

                        if (Math.Abs(errorByPercent) > _DSAStepPercent / 2 * 1.2)
                        {
                            channel.Status = AutoCaliAmpStatus.Failure;
                            LogInfo($"[CH{channel.channelID + 1}]:{channel.Status},误差超过adc调整范围!");

                            //!test
                            var otherCalc = ClacSquareAmpByUv(recordData[channel.channelID]);
                            if (otherCalc == null)
                                LogInfo($"otherClac:{otherCalc}");
                            else
                                LogInfo($"otherClac:{otherCalc.Value}");

                            using (FileStream fStream = new FileStream(@$".\Log_Cali\{DateTime.Now.ToString("MMdd_HH_mm_ss")}_record.csv", FileMode.Create, FileAccess.Write))
                            {
                                recordData[channel.channelID].ForEach(point =>
                                {
                                    byte[] content = new UTF8Encoding(true).GetBytes(point.ToString() + ",\r");
                                    fStream.Write(content, 0, content.Length);
                                });
                            }

                            continue;
                        }
                        else if (Math.Abs(errorByPercent) < _TotalErrorByPercent)
                        {
                            (int min, int max) modeCoreGain = ((int)_ADCCtrlWord.Min, (int)_ADCCtrlWord.Max);
                            #region 判断该档位是否有5G，10G模式的Core超过范围
                            foreach (var modeItem in ServerSpecailData.JiHe_MSO7000X_AcqModeInterleaveDefines!)
                            {
                                if (modeItem.Channels != 0xf && (modeItem.Channels & 1 << channel.channelID) != 0)
                                {
                                    List<Int32> chnls = new List<Int32>();
                                    for (int chnlId = 0; chnlId < 4; chnlId++)
                                    {
                                        if ((modeItem.Channels & (1 << chnlId)) != 0)
                                            chnls.Add(chnlId);
                                    }

                                    var interDetail = modeItem.Details[chnls.IndexOf(channel.channelID)];
                                    int refCoreGain2_5G_adcCali = 512;// TiAdc_PhaseOffsetGain_JiHe_MSO7000X.Default[0xf, (int)interDetail.FixedCore].GainErr;

                                    for (int coreIndex = 0; coreIndex < 4; coreIndex++)
                                    {
                                        if (interDetail.UsedCoreList!.Contains(coreIndex))
                                        {
                                            int currCoreGain_adcCali = 512;// TiAdc_PhaseOffsetGain_JiHe_MSO7000X.Default[modeItem.Channels, coreIndex].GainErr;
                                            //按照变化量来换算
                                            int currModeCoreGain = (int)channel.Cali_ADCFineControlWord + (currCoreGain_adcCali - refCoreGain2_5G_adcCali);
                                            if (currModeCoreGain > modeCoreGain.max)
                                                modeCoreGain.max = currModeCoreGain;
                                            else if (currModeCoreGain < modeCoreGain.min)
                                                modeCoreGain.min = currModeCoreGain;

                                            LogInfo($"[CH{channel.channelID + 1}] AdcStage: chnls_{modeItem.Channels},currCore_{coreIndex}," +
                                                $"currCoreGain_adcCali_{currCoreGain_adcCali}, refCoreGain2_5G_adcCali_{refCoreGain2_5G_adcCali}," +
                                                $"Cali_ADCFineControlWord_{channel.Cali_ADCFineControlWord},currModeCoreGain_{currModeCoreGain}");
                                        }
                                    }

                                }
                            }
                            if (modeCoreGain.max > _ADCCtrlWord.Max && modeCoreGain.min < _ADCCtrlWord.Min)
                            {
                                channel.Status = AutoCaliAmpStatus.Failure;
                                LogInfo($"[CH{channel.channelID + 1}]:" +
                                    $"{channel.Status},adc的core校准值gain之间的值离散太大!");
                                continue;
                            }
                            else if (modeCoreGain.max > _ADCCtrlWord.Max || modeCoreGain.min < _ADCCtrlWord.Min)
                            {
                                int deltaWord = 0;
                                if (modeCoreGain.max > _ADCCtrlWord.Max)
                                    deltaWord = (int)((_ADCCtrlWord.Max - 10) - modeCoreGain.max);
                                else if (modeCoreGain.min < _ADCCtrlWord.Min)
                                    deltaWord = (int)((_ADCCtrlWord.Min + 10) - modeCoreGain.min);


                                channel.Cali_ADCFineControlWord = (uint)(channel.Cali_ADCFineControlWord + deltaWord);
                                channel.CaliTimes.Adc++;

                                LogInfo($"[CH{channel.channelID + 1}] AdcStage: 该档位5G/10G模式有TiAdc的core的gain控制字变化量为:{deltaWord}，" +
                                    $"新的gain为:{channel.Cali_ADCFineControlWord}");

                                GotoNextStage(channel, errorByPercent);
                                continue;
                            }
                            #endregion
                            channel.Status = AutoCaliAmpStatus.Ok;
                            continue;
                        }

                        int newWord = 0;
                        //当发生震荡的时候，优化PerStepChangedPercent的值
                        if (channel.AdcLastSetting != null)
                        {
                            if (channel.AdcLastSetting.Value.err * errorByPercent < 0)
                            {
                                channel.AdcPerStepChangedPercent = (channel.AdcLastSetting.Value.err - errorByPercent)
                                    / ((int)channel.AdcLastSetting.Value.reg - (int)channel.Cali_ADCFineControlWord);
                                LogInfo($"[CH{channel.channelID + 1}] AdcPerStepChangedPercent:{channel.AdcPerStepChangedPercent}");
                            }
                        }
                        channel.AdcLastSetting = (channel.Cali_ADCFineControlWord, errorByPercent);

                        //计算新的adc控制字值
                        if (Math.Abs(errorByPercent) < 4 * channel.AdcPerStepChangedPercent)
                        {
                            newWord = (int)channel.Cali_ADCFineControlWord + (errorByPercent < 0 ? 1 : -1);
                        }
                        else
                        {
                            newWord = (int)((int)channel.Cali_ADCFineControlWord - errorByPercent / channel.AdcPerStepChangedPercent);
                        }

                        if (newWord < _ADCCtrlWord.Min || newWord > _ADCCtrlWord.Max)
                        {
                            channel.Status = AutoCaliAmpStatus.Failure;
                            LogInfo($"[CH{channel.channelID + 1}]:{channel.Status},ADC细调越界!");
                            continue;
                        }

                        channel.Cali_ADCFineControlWord = (uint)newWord;
                        channel.CaliTimes.Adc++;
                        continue;
                    }
                    else if (channel.Stage == AutoCaliAmpStage.FPGA)
                    {
                        double errorRatio = (double)(errorDelta) / amp;
                        int newFpgaCtrlWord = channel.Cali_FPGAFinegainWord - (int)(errorRatio * channel.Cali_FPGAFinegainWord);

                        LogInfo($"[CH{channel.channelID + 1}] FPGAStage: errorDeltaUv_{errorDelta};errorRatio_{errorRatio};" +
                            $"Cali_FPGAFinegainWord_{channel.Cali_FPGAFinegainWord}");

                        if (Math.Abs(errorByPercent) < _TotalErrorByPercent)
                        {
                            channel.Status = AutoCaliAmpStatus.Ok;
                            continue;
                        }
                        else if (newFpgaCtrlWord > _FpgaCtrlWord.Max)
                        {
                            channel.Cali_FPGAFinegainWord = _FpgaCtrlWord.Max;
                            messageSB.Append($"[CH{channel.channelID + 1}]，FPGA细调已到最大值。");
                            GotoNextStage(channel, errorByPercent);
                        }
                        else if (newFpgaCtrlWord < _FpgaCtrlWord.Min)
                        {
                            channel.Cali_FPGAFinegainWord = _FpgaCtrlWord.Min;
                            messageSB.Append($"[CH{channel.channelID + 1}]，FPGA细调已到最小值。");
                            GotoNextStage(channel, errorByPercent);
                        }
                        else
                        {
                            channel.Cali_FPGAFinegainWord = newFpgaCtrlWord;
                        }
                        channel.CaliTimes.Fpga++;
                        continue;
                    }
                    #endregion 不同阶段进行校准调整
                }
            }
            stopwatch.Stop();
            messageSB.Append($"(ExecuteId={_UniqueId};)");
            foreach (CaliChnlInfo channel in _CaliChannels)
            {
                int channelIndex = _CaliChannels.IndexOf(channel);
                messageSB.Append($"[Ch{channel.channelID + 1}]:{channel.Status};" +
                    $"DsaTimes={channel.CaliTimes.Dsa}, AdcTimes={channel.CaliTimes.Adc}, FpfaTimes={channel.CaliTimes.Fpga}。");

                //把2mv档位的系数设置给1mv档
                if (channel.YLevelID == 2 && channel.Status == AutoCaliAmpStatus.Ok)
                    Set1MvCaliData(channel);
            }

            messageSB.Append($"总共用时{stopwatch.ElapsedMilliseconds}ms.");
            message = messageSB.ToString();
            LogInfo("校准结果：" + message);
            Boolean isAllOk = _CaliChannels.All(ch => ch.Status == AutoCaliAmpStatus.Ok);
            return isAllOk ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorGeneral;
        }

        #region WCJ_校准处理

        private Boolean _FirstAdjustOverStepFlag = false;
        private (int min, int max) _DacOffset = (5000, 15000);
        private (int min, int max) _AdcRange { get => (0, (int)Math.Pow(2, ServerDomainConstants.AdcBits) - 1); }

        private Boolean IsOverStep(ushort[] chnlData, double ratio)
        {
            if (IsOverStepUp(chnlData, ratio) || IsOverStepDown(chnlData, ratio))
                return true;
            return false;
        }

        private Boolean IsOverStepUp(ushort[] chnlData, double ratio)
        {
            int upValue = _AdcRange.max;
            if ((ratio >= _FpgaCtrlWord.Min / 1000D) && (ratio < 1D))
                upValue -= (int)((_AdcRange.max - _AdcRange.min) * (1 - ratio) / 2 + 1);

            if (chnlData.Count(d => d >= upValue) >= chnlData.Length / 4)
                return true;
            return false;
        }

        private Boolean IsOverStepDown(ushort[] chnlData, double ratio)
        {
            int downValue = _AdcRange.min;
            if ((ratio >= _FpgaCtrlWord.Min / 1000D) && (ratio < 1D))
                downValue += (int)((_AdcRange.max - _AdcRange.min) * (1 - ratio) / 2 + 1);

            if (chnlData.Count(d => d <= downValue) >= chnlData.Length / 4)
                return true;
            return false;
        }

        #endregion WCJ_校准处理

        /// <summary>
        /// 给1mv档位设置参数；
        /// </summary>
        /// <param name="info"></param>
        private void Set1MvCaliData(CaliChnlInfo info)
        {
            ChannelPerScaleItem scaleItem = ChannelParams.Default[info.channelID, info.Impedance, 1];
            scaleItem.OffsetPosterior = info.Cali_ChannelOffset;
            scaleItem.Gain_CoarseCtrlWord = info.Cali_CoarseControlWord;
            scaleItem.Gain_FineByAdc = info.Cali_ADCFineControlWord;
            //scaleItem.Gain_FineByFpgaThousand = info.Cali_FPGAFinegainWord * 2;
            ChannelParams.Default[info.channelID, info.Impedance, 1] = scaleItem;
        }
    }
}
