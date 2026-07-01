using Newtonsoft.Json;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using static ScopeX.Hardware.Driver.Gain_FineByFpgaResult;
/******************************幅度细调自校正流程************************************/
//1、前提是在ToolChain端（出厂）已经进行过一次完整校正，才能保证UI端（用户）幅度细调自校正成功
//2、基于V3版本，50mV档为衰减档
//3、选取50mV档为基准 手动校正0div、3div 得到初始斜率K0，50mV档位的DVGA值为 D0 = 18，由于V4版本50mV为直通档所以这里D0固定为18（V3版本）
//4、其他档位根据对应的DVGA值D与50mV进行比较得到Scale，此时斜率 K = Scale*K0
//5、Scale计算公式为：
//                 scale = Math.Pow(10, (D - D0) / 20D);
//6、从2mV开始校准，1mV数据使用2mV数据
/******************************幅度细调自校正流程************************************/
namespace ScopeX.Hardware.Driver
{

    public partial class Cali
    {
        public Int32 AutoCalibration(List<ChannelId> needCaliChannels, CancellationToken? cancelToken, out String message)
        {
            var res = 0;
            message = "";

            IsCalibration = true;
            var tempenable = Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate;
            var dspenable = Hd.CurrDebugVarints.bEnable_Dsp;
            var dspenablepro = Hd.CurrDebugVarints.bEnable_Dsp_Pro;
            var dsogainenable = Hd.CurrDebugVarints.BEnable_DsoGainByFpga;
            Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_Dsp = false;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;
            Hd.CurrDebugVarints.BEnable_DsoGainByFpga = false;

            _FinishedItemCount = 0;

            try
            {
                //TiAdc
                if (Constants.BOARD_ATTACHED)
                {
                    //TiAdcAutoCali_Exec();
                    Hd.CurrProduct?.Acquirer_AnalogChannel?.ChnlSyncDiscardDotsEx();
                }

                //Gain
                //res = AutoCaliAnalogChannelGain_Exec(needCaliChannels, cancelToken, out message);

                ////Baselint
                //res = AutoCaliAnalogChannelBaseline_Exec(needCaliChannels, cancelToken, out message);

                ////Ext
                //AutoCaliExeTrigger_Exec();
            }
            catch (Exception ex)
            {
                Hd.SysLogger?.Invoke($"AutoCalibraion：{ex.Message}", "Error");
            }
            finally
            {
                _FinishedItemCount = GetTotalItemCount();
                IsCalibration = false;
                Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = tempenable;
                Hd.CurrDebugVarints.bEnable_Dsp = dspenable;
                Hd.CurrDebugVarints.bEnable_Dsp_Pro = dspenablepro;
                Hd.CurrDebugVarints.BEnable_DsoGainByFpga = dsogainenable;
                CleanUpOldLogs("BaselineCaliLog", BaselineCaliLogFileName);
            }
            return res;

        }

        /// <summary>
        /// 通道幅度细调自动校准
        /// </summary>
        /// <param name="needCaliChannels">需要校准的通道</param>
        /// <param name="cancelToken"></param>
        /// <param name="message">校准信息</param>
        /// <returns></returns>
        public Int32 AutoCaliAnalogChannelGain_Exec(List<ChannelId> needCaliChannels, CancellationToken? cancelToken, out String message)
        {
            var res = 0;
            _FinishedItemCount = 0;
            IsCalibration = true;
            message = "";
            ((ICaliData)ChannelParams.Default!).LoadFromFile();//重新装载
            AutoCaliParams.Default?.SaveToFile();
            var backupmsg = Hd.UIMessage! with { };
            Hd.CurrDebugVarints.BEnable_DsoGainByFpga = false;
            Hd.CurrDebugVarints.bEnable_Dsp = false;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;
            if (File.Exists("gain_FinVale.txt"))
            {
                File.Delete("gain_FinVale.txt");
            }
            //AutoCaliAnalogChannelBaseline_Exec(needCaliChannels, cancelToken, out message, true);
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
                PosDiv.Pos3Div_P,
                PosDiv.Pos3Div_N,
            };
            var sb = new StringBuilder();
            AppendCaliLog(sb, "开始幅度校准");
            AppendCaliLog(sb, "阻抗循环-->挡位循环-->Div循环");

            //阻抗循环
            foreach (var currentcouping in couplinglist)
            {
                var yscalestart = (Int32)AnaChnlScaleIndex.Lv1m;
                var yscaleend = currentcouping == Coupling.HighImpedance ? (Int32)AnaChnlScaleIndex.Lv10 : (Int32)AnaChnlScaleIndex.Lv1;
                //挡位循环
                for (var currentyscale = yscalestart; currentyscale <= yscaleend; currentyscale++)
                {
                    var scalevaluebymv = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[currentyscale] / 1e3;
                    AppendCaliLog(sb, $"{currentcouping} {scalevaluebymv}mv挡 校准基线0Div");
                    AutoCaliAnalogChannelBaseGainline_Exec(needCaliChannels, currentcouping, currentyscale, out message);
                    AppendCaliLog(sb, $"{currentcouping} {scalevaluebymv}mv挡 校准基线完成");
                    //通道集合
                    List<KeyValuePair<ChannelId, ChannelGainInfo>> calichnl = new();
                    for (var chnl = 0; chnl < needCaliChannels.Count; chnl++)
                    {
                        HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog![chnl];
                        ChannelGainInfo chnlinfo = new();
                        chnlinfo.ChannelId = chnl;
                        chnlinfo.IsFinish = false;
                        chnlinfo.YScaleCurrent = currentyscale;
                        chnlinfo.Impedance_H_Is0 = currentcouping == Coupling.LowImpedance ? 1 : 0;
                        calichnl.Add(new KeyValuePair<ChannelId, ChannelGainInfo>((ChannelId)chnl, chnlinfo));
                    }
                    var allchnlfinish = false;//所有通道是否校准完成
                    var iterationcount = 1;//校准次数

                    //校准循环
                    while (!allchnlfinish && iterationcount <= MaxIterationCount)
                    {
                        List<ushort[]>? pos3divp = new();//正3div的通道数据
                        List<ushort[]>? pos3divn = new();//负3div的通道数据

                        #region 获取±3div数据

                        // 3Div -3Div循环
                        foreach (var currentposdiv in posdivlist)
                        {
                            #region 构造每个通道信息 新的HDMessage 并配置下去
                            var positionindex = (Int32)currentposdiv * 1000;
                            var newanalogoptions = new List<HdMessage.AnalogOptions>();
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
                                    Scale = scalevaluebymv,
                                    ScaleIndex = currentyscale,
                                    PositionIndex = positionindex,
                                    Position = positionindex / 1e3 * scalevaluebymv,
                                };
                                newanalogoptions.Add(ch);
                            }
                            Hd.UIMessage = Hd.UIMessage! with { Analog = newanalogoptions.ToArray() };
                            ConfigHardware(Hd.UIMessage, 400);

                            #endregion

                            #region 一次性获取所有通道数据

                            List<UInt16[]>? channeldata = new();
                            var bok = AcqWaveData(out channeldata, 5 * 1000, Hd.UIMessage);
                            if (!bok)
                            {
                                IsCalibration = false;
                                _FinishedItemCount = GetTotalItemCount();
                                Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
                                Hd.CurrDebugVarints.bEnable_Dsp = true;
                                Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                                AppendCaliLog(sb, $"数据采集错误，退出幅度校准流程");
                                PrintCaliLog(sb, BaselineCaliLogFileName);
                                return 0;
                            }
                            switch (currentposdiv)
                            {
                                case PosDiv.Pos3Div_N:
                                    pos3divn = channeldata;
                                    break;
                                case PosDiv.Pos3Div_P:
                                    pos3divp = channeldata;
                                    break;
                                default:
                                    break;
                            }
                            #endregion

                        }//3div -3div循环

                        #endregion

                        #region 计算幅度差值

                        foreach (var chnl in calichnl)
                        {
                            //3div
                            var ans = AverageAcqData(pos3divp![(Int32)chnl.Key], out var avgp);
                            if (!ans)
                            {
                                IsCalibration = false;
                                _FinishedItemCount = GetTotalItemCount();
                                Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
                                Hd.CurrDebugVarints.bEnable_Dsp = true;
                                Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                                AppendCaliLog(sb, "采集到的数据异常，可能是数据长度为0，结束基线校准");
                                PrintCaliLog(sb, BaselineCaliLogFileName);
                                return 0;
                            }
                            //-3div
                            ans = AverageAcqData(pos3divn![(Int32)chnl.Key], out var avgn);
                            if (!ans)
                            {
                                IsCalibration = false;
                                _FinishedItemCount = GetTotalItemCount();
                                Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
                                Hd.CurrDebugVarints.bEnable_Dsp = true;
                                Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                                AppendCaliLog(sb, "采集到的数据异常，可能是数据长度为0，结束基线校准");
                                PrintCaliLog(sb, BaselineCaliLogFileName);
                                return 0;
                            }

                            //判断是否越界
                            if (avgp == 4096 || avgp == 0 || avgn == 4096 || avgn == 0)
                            {
                                //chnl.Value.IsFinish = true;
                                AutoCaliAnalogChannelBaseGainline_Exec(needCaliChannels, currentcouping, currentyscale, out message);
                                AppendCaliLog(sb, $"采集到的数据异常，校准0Div,avgp:{avgp} avgn:{avgn}");
                            }
                            else
                            {
                                var gain_finvale = GetGain_FineByFpgaVaule(chnl.Value.Gain_FineValue, avgp, avgn);
                                AppendCaliLog(sb, $"{chnl.Key} calibrationstatus:{gain_finvale.isCalibration} {currentcouping} {scalevaluebymv}mv挡 第{iterationcount}次 校准状态：{chnl.Value.IsFinish} 3Div平均值{avgp} -3Div平均值{avgn} rms = {gain_finvale.percentageVaule} lastfine:{chnl.Value.Gain_FineValue} nowfine:{gain_finvale.result}");
                                //是否进行细调
                                if (gain_finvale.isCalibration)
                                {
                                    //设置参数
                                    chnl.Value.SetGain_FineByFpgaThousand((Int32)gain_finvale.result);
                                }
                                else
                                {
                                    chnl.Value.IsFinish = true;
                                }
                            }
                        }

                        #endregion

                        //循环次数
                        iterationcount++;

                        //判断所有通道是否完成校准
                        var notfinish = calichnl.Where(chnl => chnl.Value.IsFinish == false);
                        allchnlfinish = notfinish == null || notfinish.Count() <= 0;

                    }//While
                    AppendCaliLog(sb, $"{currentcouping} {scalevaluebymv}mv挡 幅度校准完成");
                    _FinishedItemCount++;
                }//挡位循环
            }//阻抗循环
            AppendCaliLog(sb, "幅度校准完成，退出流程");
            PrintCaliLog(sb, BaselineCaliLogFileName);
            #region 读取温度并保存

            AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius = (Int32)(double.Parse(SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0)) * 1000D);

            #endregion

            AutoCaliParams.Default?.SaveCaliDataToFile();
            Hd.CurrDebugVarints.BEnable_DsoGainByFpga = true;
            Hd.CurrDebugVarints.bEnable_Dsp = true;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
            Hd.Execute(backupmsg);
            IsCalibration = false;

            return res;
        }

        internal Int32 AutoCaliAnalogChannelBaseGainline_Exec(List<ChannelId> needCaliChannels, Coupling coupling, Int32 yScale, out string message)
        {
            var res = 0;
            message = "";
            var backupmsg = Hd.UIMessage! with { };

            PosDiv posdiv = PosDiv.Pos0Div;

            //阻抗循环
            //挡位循环
            // 0Div 3Div循环
            var allchnlfinish = false;
            var iterationcount = 1;
            //************************** 获取每个通道的offset信息*************************//
            List<KeyValuePair<ChannelId, ChannelInfo>> calichnl = new();
            foreach (var chnlid in needCaliChannels)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog![(Int32)chnlid];
                ChannelInfo chnlinfo = new();
                chnlinfo.ChannelId = (Int32)chnlid;
                chnlinfo.IsFinish = false;
                chnlinfo.YScaleCurrent = yScale;
                //chnlinfo.IsPos0Div = currentposdiv == PosDiv.Pos0Div;
                calichnl.Add(new KeyValuePair<ChannelId, ChannelInfo>(chnlid, chnlinfo));
            }
            //************************** 获取每个通道的offset信息*************************//
            while (!allchnlfinish && iterationcount <= MaxIterationCount)
            {
                #region 构造每个通道信息 新的HDMessage 并配置下去

                var scalevaluebymv = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[yScale] / 1e3;
                var positionindex = (Int32)posdiv * 1000;
                var newanalogoptions = new List<HdMessage.AnalogOptions>();
                for (var chnl = 0; chnl < needCaliChannels.Count; chnl++)
                {
                    HdMessage.AnalogOptions ch = Hd.UIMessage.Analog![chnl] with
                    {
                        Bias = 0,
                        Bandwidth = 3,
                        IsInverted = false,
                        ProbeIndex = AnaChnlProbe.x1,
                        InputSource = AnaChnlIpnutSource.BNC,
                        Coupling = (AnaChnlCoupling)coupling,
                        Scale = scalevaluebymv,
                        ScaleIndex = yScale,
                        ScaleBymV = scalevaluebymv,
                        PositionIndex = positionindex,
                        Position = positionindex / 1e3 * scalevaluebymv,
                    };
                    newanalogoptions.Add(ch);
                    calichnl[chnl].Value.Impedance_H_Is0 = ch.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                    calichnl[chnl].Value.CalcRadio();
                }
                Hd.UIMessage = Hd.UIMessage! with { Analog = newanalogoptions.ToArray() };
                ConfigHardware(Hd.UIMessage, 400);

                #endregion

                #region 一次性获取所有通道数据

                List<ushort[]>? channeldata = new();
                var bok = AcqWaveData(out channeldata, 5 * 1000, Hd.UIMessage);
                if (!bok)
                {
                    return 0;
                }

                #endregion

                //********************************** 通道循环********************************//

                var posdivbyadc = (Int32)posdiv * Constants.SAMPS_PER_YDIV;
                var theoryvalue = posdivbyadc + (Math.Pow(2, Constants.ADC_BITS) / 2);
                foreach (var chnl in calichnl)
                {
                    chnl.Value.IsFinish = false;
                    var ans = AverageAcqData(channeldata![(Int32)chnl.Key], out var avg);
                    if (!ans)
                    {
                        return 0;
                    }
                    var error = 0.0;
                    var newctrlworddelta = 0D;
                    switch (posdiv)
                    {
                        case PosDiv.Pos0Div:
                            error = avg - theoryvalue;//偏差
                            newctrlworddelta = Math.Round(-error * chnl.Value.RadioOfY1Dot_CtrlWord);
                            break;
                        default:
                            error = -((avg - theoryvalue) / posdivbyadc);//比例
                            newctrlworddelta = error;
                            break;
                    }
                    bok = posdiv == PosDiv.Pos0Div ? Math.Abs(error) < CaliError : Math.Abs(newctrlworddelta) < CaliRMS;
                    var rms = posdiv == PosDiv.Pos0Div ? (avg - theoryvalue) / theoryvalue : newctrlworddelta;
                    if (bok)
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
                    if (newctrlworddelta == 0 && posdiv == PosDiv.Pos0Div)
                    {
                        chnl.Value.IsFinish = true;
                    }
                    string debuglog = string.Empty;
#if DEBUG
                    if (chnl.Value.IsFinish)
                    {
#if DEBUG
                        if (iterationcount >= MaxIterationCount)
                        {
                            debuglog = ($"tpye = Error [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {posdiv} {scalevaluebymv}mv档 {posdiv} {iterationcount}次校准失败 error={error}  RMS={rms}");
                        }
                        else
                        {
                            debuglog = ($"tpye = Info    [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {posdiv} {scalevaluebymv}mv档 {posdiv} 第{iterationcount}次校准成功 error={error} RMS={rms}");
                        }
#endif
                    }
                    else
                    {
                        debuglog = ($"tpye = warning [AutoCalibration] [{DateTime.Now.ToString("G")}]:{chnl.Key} {posdiv} {scalevaluebymv}mv档 {posdiv} 第{iterationcount}次迭代 RMS={rms} error={error} Radio = {chnl.Value.RadioOfY1Dot_CtrlWord}");
                    }
                    Debug.WriteLine(debuglog);
#endif
                    chnl.Value.SetCtrlWord(newctrlworddelta, posdiv);
                }
                //********************************** 通道循环********************************//

                iterationcount++;

                var notfinish = calichnl.Where(chnl => chnl.Value.IsFinish == false);
                allchnlfinish = notfinish == null || notfinish.Count() <= 0;

            }//While

            AutoCaliParams.Default?.SaveCaliDataToFile();
            Hd.Execute(backupmsg);
            return res;
        }


        /// <summary>
        /// 把2mV数据拷贝到1mV
        /// </summary>
        /// <param name="Source">源数据</param>
        /// <param name="Target">目标数据</param>
        /// <param name="couplinglist">阻抗</param>
        /// <param name="needCopyChannels">通道</param>
        private void CopyGainData(AnaChnlScaleIndex Source, AnaChnlScaleIndex Target, List<Coupling> couplinglist, List<ChannelId> needCopyChannels)
        {
            foreach (var coupling in couplinglist)
            {
                foreach (var channel in needCopyChannels)
                {
                    var impedance_H_Is0 = (AnaChnlCoupling)coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                    var perchannelsource = AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Source];
                    var perchanneltarget = AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Target];
                    perchanneltarget.Gain_FineByFpgaThousand = perchannelsource.Gain_FineByFpgaThousand * 2;
                    AutoCaliParams.Default![(Int32)channel, impedance_H_Is0, (Int32)Target] = perchanneltarget;


                    ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparamskeysource = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap(channel, impedance_H_Is0 == 0, (UInt32)AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[(int)Target] / 1000);
                    ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparamskeytarget = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap(channel, impedance_H_Is0 == 0, (UInt32)AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[(int)Target] / 1000);
                    var perscaleitemsource = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparamskeysource)!.Value;
                    var perscaleitemtarget = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparamskeytarget)!.Value;
                    perscaleitemtarget.Gain_FineByFpgaThousand = perscaleitemsource.Gain_FineByFpgaThousand * 2;
                    ProductDataTranslate_MSO8000X.SetChnlParamsItem(chnlparamskeytarget, perscaleitemtarget);
                }
            }
        }

        /// <summary>
        /// 判断偏差
        /// </summary>
        /// <param name="nowGainVauee">现在幅度细调值</param>
        /// <param name="pos3DivP">正3div的Adc值</param>
        /// <param name="pos3DivN">负3div的Adc值</param>
        /// <returns>是否在偏差范围内</returns>
        internal Gain_FineByFpgaResult GetGain_FineByFpgaVaule(Double nowGainVauee, Double pos3DivP, Double pos3DivN)
        {
            Gain_FineByFpgaResult gain_FineByFpgaResult = new Gain_FineByFpgaResult()
            {
                result = nowGainVauee,
                isCalibration = false,
                gainFineType = GainFineType.CalibrationVaule
            };
            var posdivByAdc = (Int32)PosDiv.Pos3Div_P * 2 * Constants.SAMPS_PER_YDIV;
            Double dValue = Math.Abs(pos3DivP - pos3DivN);
            gain_FineByFpgaResult.percentageVaule = Math.Round(1 - dValue / posdivByAdc, 4);
            if (Math.Abs(gain_FineByFpgaResult.percentageVaule) <= AppConfig.GetIntance().MaxGainCaliValue && Math.Abs(gain_FineByFpgaResult.percentageVaule) >= AppConfig.GetIntance().MinGainCaliValue)
            {
                gain_FineByFpgaResult.isCalibration = true;
                gain_FineByFpgaResult.result = nowGainVauee + gain_FineByFpgaResult.percentageVaule * 0.8 * 1000;
            }
            else
            {
                gain_FineByFpgaResult.gainFineType = Math.Abs(gain_FineByFpgaResult.percentageVaule) < AppConfig.GetIntance().MinGainCaliValue && Math.Abs(gain_FineByFpgaResult.percentageVaule) > 0
                    ? GainFineType.StandardVaule
                    : GainFineType.OutofVaule;
            }
            return gain_FineByFpgaResult;
        }
    }

    /// <summary>
    /// 幅度细调通道信息
    /// </summary>
    internal class ChannelGainInfo : ChannelInfo
    {
        /// <summary>
        /// 幅度细调值
        /// </summary>
        internal Int32 Gain_FineValue
        {
            get
            {
                return AutoCaliParams.Default != null
                    ? AutoCaliParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent].Gain_FineByFpgaThousand
                    : 1000;
            }
        }

        /// <summary>
        /// 设置幅度细调
        /// </summary>
        /// <param name="gain_FineVaule"></param>
        internal void SetGain_FineByFpgaThousand(Int32 gain_FineVaule)
        {

            //ChannelPerScaleItem perScaleItem = ChannelParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent];
            //perScaleItem.Gain_FineByFpgaThousand = (UInt32)gain_FineVaule;
            //ChannelParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = perScaleItem;

            var perScaleItem = ProductDataTranslate_MSO8000X.GetChnlParamsItem(ChnlParamsKey)!.Value;
            perScaleItem.Gain_FineByFpgaThousand = (Int32)gain_FineVaule;
            ProductDataTranslate_MSO8000X.SetChnlParamsItem(ChnlParamsKey, perScaleItem);

            var item = AutoCaliParams.Default![ChannelId, Impedance_H_Is0, YScaleCurrent];
            item.Gain_FineByFpgaThousand = gain_FineVaule;
            AutoCaliParams.Default[ChannelId, Impedance_H_Is0, YScaleCurrent] = item;
        }
    }

    /// <summary>
    /// 3div、-3div幅度细调偏差结果
    /// </summary>
    internal class Gain_FineByFpgaResult
    {
        internal enum GainFineType
        {
            /// <summary>
            /// 超出校准范围 >5%
            /// </summary>
            OutofVaule,
            /// <summary>
            /// 已在标准范围 <2% 
            /// </summary>
            StandardVaule,
            /// <summary>
            /// 可校准范围 >2%   <5%
            /// </summary>
            CalibrationVaule,
        }

        /// <summary>
        /// 幅度细调值
        /// </summary>
        public Double result { get; set; }

        /// <summary>
        /// 是否进行校准
        /// </summary>
        public Boolean isCalibration { get; set; }

        /// <summary>
        /// 细调类型
        /// </summary>
        public GainFineType gainFineType { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        public Double percentageVaule { get; set; }

    }
}
