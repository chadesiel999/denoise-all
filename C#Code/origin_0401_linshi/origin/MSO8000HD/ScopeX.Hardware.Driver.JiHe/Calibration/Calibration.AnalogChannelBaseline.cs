using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;
/******************************基线自校正流程************************************/
//1、前提是在ToolChain端（出厂）已经进行过一次基线校正，才能保证UI端（用户）基线自校正成功
//2、基于V3版本，50mV档为衰减档
//3、选取50mV档为基准 手动校正0div、3div 得到初始斜率K0，50mV档位的DVGA值为 D0 = 18，由于V4版本50mV为直通档所以这里D0固定为18（V3版本）
//4、其他档位根据对应的DVGA值D与50mV进行比较得到Scale，此时斜率 K = Scale*K0
//5、Scale计算公式为：
//                 scale = Math.Pow(10, (D - D0) / 20D);
//6、从2mV开始校准，1mV数据使用2mV数据
/******************************基线自校正流程************************************/
namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        private String BaselineCaliLogFileName => $"{CaliDataPath}BaselineCaliLog{DateTime.Now.ToString("yyyy-MM-dd-HH")}.txt";

        private volatile Int32 _FinishedItemCount = 0;

        private const Int32 MaxIterationCount = 20;

        private const Double CaliError = 0.5D;//偏差

        private const Double CaliRMS = 0.015D;//误差

        public Boolean EnableAnalogTemperatureCompensate => Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate;

        internal Boolean IsCalibration { get; private set; } = false;

        private Boolean AverageAcqData(ushort[] source, out Double average)
        {
            if (source == null || source.Length <= 0)
            {
                average = 0D;
                return false;
            }
            var buffer = source.Select(x => (double)x).ToList();
            average = buffer.Average();
            return true;
        }

        public Int32 GetTotalItemCount()
        {
            var count = 0;

            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)//只有低阻
            {
                //幅度自校正 ±3Div
                count += (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv1m + 1);

                //计算基线自校正 0Div、±3Div
                count += 3 * (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv10m + 1);
            }

            else//高低阻
            {
                //幅度自校正 ±3Div
                count += (AnaChnlScaleIndex.Lv10 - AnaChnlScaleIndex.Lv1m + 1) + (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv1m + 1);

                //计算基线自校正 0Div、±3Div
                count += 3 * (AnaChnlScaleIndex.Lv10 - AnaChnlScaleIndex.Lv10m + 1) + 3 * (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv10m + 1);
            }
            //计算外触发校正 {AC,DC,LFR,HFR}{Ext,Ext5}{1MΩ,50Ω}
            count += 4 * 2 * 2;
            return count;
        }

        public Int32 GetFinishedCount() => _FinishedItemCount;

        public void ClearFinishedCount() => _FinishedItemCount = 0;

        //通道基线自动校准
        public Int32 AutoCaliAnalogChannelBaseline_Exec(List<ChannelId> needCaliChannels, CancellationToken? cancelToken, out string message)
        {
            Hd.CurrDebugVarints.BEnable_DsoGainByFpga = false;
            Hd.CurrDebugVarints.bEnable_Dsp = false;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;
            var res = 0;
            message = "";
            IsCalibration = true;
            StringBuilder sb = new StringBuilder();
            AppendCaliLog(sb, "开始基线校准");
            var backupmsg = Hd.UIMessage! with { };

            var couplinglist = new List<Coupling>()
             {
                Coupling.HighImpedance,
                Coupling.LowImpedance,
             };

            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
            {
                couplinglist = new List<Coupling>()
                { Coupling.LowImpedance};
            }

            var posdivlist = new List<PosDiv>()
            {
                PosDiv.Pos0Div,
                PosDiv.Pos3Div_P,
                PosDiv.Pos3Div_N,
            };

            AppendCaliLog(sb, "阻抗循环-->挡位循环-->Div循环");
            AppendCaliLog(sb, $"校准前温度：{SystemMonitor.Default.Read()}");
            //阻抗循环
            foreach (var currentcouping in couplinglist)
            {
                var YScaleStart = (Int32)AnaChnlScaleIndex.Lv10m;
                var YScaleEnd = currentcouping == Coupling.HighImpedance ? (Int32)AnaChnlScaleIndex.Lv10 : (Int32)AnaChnlScaleIndex.Lv1;
                //挡位循环
                for (var currentyscale = YScaleStart; currentyscale <= YScaleEnd; currentyscale++)
                {
                    // 0Div 3Div循环
                    foreach (var currentposdiv in posdivlist)
                    {
                        var allchnlfinish = false;
                        var iterationcount = 1;
                        //************************** 获取每个通道的offset信息*************************//
                        List<KeyValuePair<ChannelId, ChannelInfo>> calichnl = new();
                        foreach (var chnlid in needCaliChannels)
                        {
                            HdMessage.AnalogOptions analogParas = Hd.UIMessage.Analog![(Int32)chnlid];
                            ChannelInfo chnlinfo = new();
                            chnlinfo.ChannelId = (Int32)chnlid;
                            chnlinfo.IsFinish = false;
                            chnlinfo.YScaleCurrent = currentyscale;
                            //chnlinfo.IsPos0Div = currentposdiv == PosDiv.Pos0Div;
                            calichnl.Add(new KeyValuePair<ChannelId, ChannelInfo>(chnlid, chnlinfo));
                        }
                        //************************** 获取每个通道的offset信息*************************//
                        while (!allchnlfinish && iterationcount <= MaxIterationCount)
                        {
                            #region 构造每个通道信息 新的HDMessage 并配置下去

                            var scaleValueBymV = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[currentyscale] / 1e3;
                            var positionindex = (Int32)currentposdiv * 1000;
                            var newAnalogOptions = new List<HdMessage.AnalogOptions>();
                            for (var chnl = 0; chnl < needCaliChannels.Count; chnl++)
                            {
                                HdMessage.AnalogOptions ch = Hd.UIMessage.Analog![chnl] with
                                {
                                    Bias = 0,
                                    Bandwidth = 3,
                                    IsInverted = false,
                                    ProbeIndex = AnaChnlProbe.x1,
                                    InputSource = AnaChnlIpnutSource.BNC,
                                    Coupling = (AnaChnlCoupling)currentcouping,
                                    Scale = scaleValueBymV,
                                    ScaleBymV = scaleValueBymV,
                                    ScaleIndex = currentyscale,
                                    PositionIndex = positionindex,
                                    Position = positionindex / 1e3 * scaleValueBymV,
                                };
                                newAnalogOptions.Add(ch);
                                calichnl[chnl].Value.Impedance_H_Is0 = ch.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                                calichnl[chnl].Value.CalcRadio();
                            }
                            Hd.UIMessage = Hd.UIMessage! with { Analog = newAnalogOptions.ToArray() };
                            ConfigHardware(Hd.UIMessage, 400);

                            #endregion

                            #region 一次性获取所有通道数据

                            List<ushort[]>? channeldata = new();
                            var bok = AcqWaveData(out channeldata, 5 * 1000, Hd.UIMessage);
                            if (!bok)
                            {
                                _FinishedItemCount = GetTotalItemCount();
                                Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
                                Hd.CurrDebugVarints.bEnable_Dsp = true;
                                Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                                AppendCaliLog(sb, "数据采集错误，结束基线校准");
                                PrintCaliLog(sb, BaselineCaliLogFileName);
                                return 0;
                            }

                            #endregion

                            //********************************** 通道循环********************************//

                            var posdivByAdc = (Int32)currentposdiv * Constants.SAMPS_PER_YDIV;
                            var theoryvalue = posdivByAdc + (Math.Pow(2, Constants.ADC_BITS) / 2);
                            foreach (var chnl in calichnl)
                            {
                                chnl.Value.IsFinish = false;
                                var ans = AverageAcqData(channeldata![(Int32)chnl.Key], out var avg);
                                if (!ans)
                                {
                                    _FinishedItemCount = GetTotalItemCount();
                                    Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
                                    Hd.CurrDebugVarints.bEnable_Dsp = true;
                                    Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                                    AppendCaliLog(sb, "采集到的数据异常，可能是数据长度为0，结束基线校准");
                                    PrintCaliLog(sb, BaselineCaliLogFileName);
                                    return 0;
                                }
                                var error = 0.0;
                                var newCtrlWordDelta = 0D;
                                switch (currentposdiv)
                                {
                                    case PosDiv.Pos0Div:
                                        error = avg - theoryvalue;//偏差
                                        newCtrlWordDelta = Math.Round(-error * chnl.Value.RadioOfY1Dot_CtrlWord);
                                        break;
                                    default:
                                        error = -((avg - theoryvalue) / posdivByAdc);//比例
                                        newCtrlWordDelta = error;
                                        break;
                                }
                                var bOK = currentposdiv == PosDiv.Pos0Div ? Math.Abs(error) < CaliError : Math.Abs(newCtrlWordDelta) < CaliRMS;
                                var rms = currentposdiv == PosDiv.Pos0Div ? (avg - theoryvalue) / theoryvalue : newCtrlWordDelta;
                                if (bOK)
                                {
                                    chnl.Value.IsFinish = true;
                                    //#if DEBUG
                                    //                                    Debug.WriteLine($"tpye = Info    [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} 第{iterationcount}次校准成功 RMS={rms}");
                                    //#endif
                                }
                                else
                                {
                                    if (iterationcount >= MaxIterationCount)
                                    {
                                        chnl.Value.IsFinish = true;
                                    }
                                }

                                //Int32 newCtrlWordDelta = 0;//= (Int32)(-error * chnl.Value.RadioOfY1Dot_CtrlWord);
                                //newCtrlWordDelta = (Int32)(Math.Round(-error * chnl.Value.RadioOfY1Dot_CtrlWord));
                                if (newCtrlWordDelta == 0 && currentposdiv == PosDiv.Pos0Div)
                                {
                                    chnl.Value.IsFinish = true;
                                }
#if DEBUG
                                if (chnl.Value.IsFinish)
                                {
#if DEBUG
                                    if (iterationcount >= MaxIterationCount)
                                    {
                                        Debug.WriteLine($"tpye = Error [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} {iterationcount}次校准失败 RMS={rms}");
                                        AppendCaliLog(sb, $"{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} {iterationcount}次校准失败 delta={avg - theoryvalue} RMS={rms}");
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"tpye = Info    [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} 第{iterationcount}次校准成功 RMS={rms}");
                                        AppendCaliLog(sb, $"{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} 第{iterationcount}次校准成功 delta={avg - theoryvalue} RMS={rms}");
                                    }
#endif
                                }
                                else
                                {
                                    Debug.WriteLine($"tpye = warning [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} 第{iterationcount}次迭代  error={error} RMS={rms} Radio = {chnl.Value.RadioOfY1Dot_CtrlWord}");
                                    AppendCaliLog(sb, $"{chnl.Key} {currentcouping} {scaleValueBymV}mv档 {currentposdiv} 第{iterationcount}次迭代 delta={avg - theoryvalue} RMS={rms}");
                                }
#endif
                                chnl.Value.SetCtrlWord(newCtrlWordDelta, currentposdiv);
                            }
                            //********************************** 通道循环********************************//

                            iterationcount++;

                            var notfinish = calichnl.Where(chnl => chnl.Value.IsFinish == false);
                            allchnlfinish = notfinish == null || notfinish.Count() <= 0;

                        }//While
                         //所有通道完成
                        _FinishedItemCount++;

                    }//0div 3div -3div循环
                }//挡位循环
            }//阻抗循环
            AppendCaliLog(sb, $"基线校准完成,退出基线校准流程");
            AppendCaliLog(sb, $"校准后温度：{SystemMonitor.Default.Read()}");
            PrintCaliLog(sb, BaselineCaliLogFileName);
            CopyData(AnaChnlScaleIndex.Lv10m, AnaChnlScaleIndex.Lv1m, couplinglist, needCaliChannels, 10);
            CopyData(AnaChnlScaleIndex.Lv10m, AnaChnlScaleIndex.Lv2m, couplinglist, needCaliChannels, 5);
            CopyData(AnaChnlScaleIndex.Lv10m, AnaChnlScaleIndex.Lv5m, couplinglist, needCaliChannels, 2);

            #region 读取温度并保存

            AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius = (Int32)(double.Parse(SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0)) * 1000D);

            #endregion
            AutoCaliParams.Default?.SaveCaliDataToFile();
            Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
            Hd.CurrDebugVarints.bEnable_Dsp = true;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
            //Helper.GetICaliData(CaliDataType.AutoCalibration)?.SaveToFile();
            Hd.Execute(backupmsg);

            #region 外触发校正

            AutoCaliExeTrigger_Exec();

            #endregion

            _FinishedItemCount = GetTotalItemCount();

            return res;
        }

        private void CopyData(AnaChnlScaleIndex Source, AnaChnlScaleIndex Target, List<Coupling> couplinglist, List<ChannelId> needCopyChannels, Int32 ratio)
        {
            foreach (var coupling in couplinglist)
            {
                foreach (var channel in needCopyChannels)
                {
                    var impedance_H_Is0 = (AnaChnlCoupling)coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                    var perchannelsource = AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Source];
                    var perchanneltarget = AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Target];
                    perchanneltarget.OffsetPosterior = perchannelsource.OffsetPosterior;
                    perchanneltarget.OffsetPosterior_3Div = perchannelsource.OffsetPosterior_3Div / ratio;
                    perchanneltarget.OffsetPosterior_N3Div = perchannelsource.OffsetPosterior_N3Div / ratio;
                    //perchanneltarget.DCTrigZero = perchannelsource.DCTrigZero;
                    //perchanneltarget.DCTrigZero_3Div = perchannelsource.DCTrigZero_3Div;
                    //perchanneltarget.Gain_CoarseCtrlWord = perchannelsource.Gain_CoarseCtrlWord;
                    //perchanneltarget.Gain_FineByAdc = perchannelsource.Gain_FineByAdc;
                    //perchanneltarget.Gain_FineByFpgaThousand = perchannelsource.Gain_FineByFpgaThousand;
                    //perchanneltarget.OffsetPreceding = perchannelsource.OffsetPreceding;
                    //perchanneltarget.OffsetPreceding_3Div = perchannelsource.OffsetPreceding_3Div;
                    AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Target] = perchanneltarget;
                }
            }
        }

        #region Fan Test

        public Double GetCurrentTemperature() => Double.Parse(SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0)) * 1000D;

        //private Int32 _FanSpeed = 2000;
        //public Int32 FanSpeed
        //{
        //    get => _FanSpeed;
        //    set
        //    {
        //        if (_FanSpeed != value)
        //        {
        //            _FanSpeed = value;
        //            SysMonitor.Default.CtrlFanSpeed(_FanSpeed);
        //        }
        //    }
        //}

        #endregion
    }

    internal class ChannelInfo
    {
        internal Int32 ChannelId { get; set; }

        internal AnaChnlIpnutSource IpnutSource { get; set; } = AnaChnlIpnutSource.BNC;

        internal Int32 Impedance_H_Is0 { get; set; } = 0;

        internal Int32 YScaleCurrent { get; set; } = (Int32)AnaChnlScaleIndex.Lv1m;

        internal ChnlParamsKeyMap ChnlParamsKey
        {
            get
            {
                return new ChnlParamsKeyMap((ChannelId)ChannelId,
                                            Impedance_H_Is0 == 0,
                                            (UInt32)AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[YScaleCurrent] / 1000);
            }
        }

        internal Boolean IsFinish { get; set; } = false;

        //internal Boolean IsPos0Div { get; set; } = true;

        internal void SetCtrlWord(Double ctrlworddelta, PosDiv posDiv)
        {
            switch (posDiv)
            {
                case PosDiv.Pos0Div:
                    var pos0div = Pos0Div + ctrlworddelta;
                    Pos0Div = pos0div < 0 ? 0 : (UInt32)pos0div;
                    break;
                case PosDiv.Pos3Div_N:
                    Pos3Div_N += Pos3Div_N * ctrlworddelta;
                    break;
                case PosDiv.Pos3Div_P:
                    Pos3Div_P += Pos3Div_P * ctrlworddelta;
                    break;
                default:
                    break;
            }
        }

        private UInt32 Pos0Div
        {
            get
            {
                if (IpnutSource == AnaChnlIpnutSource.BNC)
                {
                    return (UInt32)ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value.Offset;
                }
                else
                {
                    return ChannelParamsModel2.Default[ChannelId, Impedance_H_Is0, YScaleCurrent].OffsetPosterior;
                }
            }
            set
            {
                if (IpnutSource == AnaChnlIpnutSource.BNC)
                {
                    var perScaleItem = ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value;
                    perScaleItem.Offset = (Int32)value;
                    ProductDataTranslate_MSO8000X.SetChnlParamsItem(ChnlParamsKey, perScaleItem);

                    var item = AutoCaliParams.Default![ChannelId, Impedance_H_Is0, YScaleCurrent];
                    item.OffsetPosterior = (Int32)value;
                    AutoCaliParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = item;
                }
                else
                {
                    ChannelPerScaleItem perScaleItem = ChannelParamsModel2.Default[ChannelId, Impedance_H_Is0, YScaleCurrent];
                    perScaleItem.OffsetPosterior = value;
                    ChannelParamsModel2.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = perScaleItem;
                }
            }
        }

        private Double Pos3Div_P
        {
            get => ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value.Offset_Pos3Div;

            set
            {
                var perScaleItem = ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value;
                perScaleItem.Offset_Pos3Div = (Int32)value;
                ProductDataTranslate_MSO8000X.SetChnlParamsItem(ChnlParamsKey, perScaleItem);

                var item = AutoCaliParams.Default![ChannelId, Impedance_H_Is0, YScaleCurrent];
                item.OffsetPosterior_3Div = (Int32)value;
                AutoCaliParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = item;
            }
        }

        private Double Pos3Div_N
        {
            get => AutoCaliParams.Default![ChannelId, Impedance_H_Is0, YScaleCurrent].OffsetPosterior_N3Div;
            set
            {
                var item = AutoCaliParams.Default![ChannelId, Impedance_H_Is0, YScaleCurrent];
                item.OffsetPosterior_N3Div = (Int32)value;
                AutoCaliParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = item;
            }
        }

        internal Double RadioOfY1Dot_CtrlWord { get; private set; } = 1.0D;//Y方向每个点对用多少个控制字。

        internal void CalcRadio()
        {
            //var standardradio = Impedance_H_Is0 == 0 ? 2.6793333333333333333333333333333D : 2.62D; //ChannelParams.Default[ChannelId, Impedance_H_Is0, (Int32)AnaChnlScaleIndex.Lv50m].OffsetPosterior_3Div * 1.0 / (3 * Constants.SAMPS_PER_YDIV);
            //var gainctrlwordBy_50mV = 14;// ChannelParams.Default[ChannelId, Impedance_H_Is0, (Int32)AnaChnlScaleIndex.Lv50m].Gain_CoarseCtrlWord;
            //var gainctrlwordBy_yscale = ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value.Gain;
            //var scale = Math.Pow(10, ((Double)gainctrlwordBy_yscale - gainctrlwordBy_50mV) / 20D);
            //RadioOfY1Dot_CtrlWord = scale * standardradio;

            var ctrlwordBy_yscale = ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value;
            RadioOfY1Dot_CtrlWord = ctrlwordBy_yscale.Offset_Pos3Div / (3 * Constants.SAMPS_PER_YDIV);
        }
    }
}
