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

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 温漂自校正流程
    /// 1、读取基线自校正时的温度 Tc
    /// 2、读取上一次温度自校正的温漂系数和温差 ，以及当前的温度 Ts
    /// 3、如果 Maht.Abs(Tc-Ts)>5 则进行温漂自校正，否则不校正
    /// 4、读取波形数据，如果有信号则不进行校正
    /// 5、满足上述条件进行自校正得到当前这一次的温漂系数和与基线自校正的温度差
    /// 6、如果当前这次的温度差与上一次温度自校正时的温度差相差在 ±10°内则不保存当前这次校正得到的温漂系数，否则替换上次温漂系数温漂系数和温差并保存
    /// </summary>
    public partial class Cali
    {
        private String AnalogChannelTemperaturesCoefficientFile => $@"AnalogChannelTemperaturesCoefficientFile.txt";
        private String CaliDataPath => $@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\";

        private String TemperatureCaliLog => $"TemperatureCaliLog{DateTime.Now.ToString("yyyyMMdd")}.txt";

        internal Boolean IsCaliTemperatureOffset { get; private set; } = false;
        //温度偏移系数
        internal Dictionary<ChannelId, Dictionary<AnaChnlCoupling, (Double Coeffcient, Double TemperatureOffset)>> TemperatureCoeffcientTable = new();
        private record ChnlInfo(Boolean HaveSignal, Double Max, Double Min, Double Avg, Double Vpp, Double Mid);

        private const Double DefaultTemperatureCoeffcient = 0D;
        private const Double TemperatureDiff = 10D;
        private const Double CoeffcientError = 0.05D;
        private Double MaxppByADC = Constants.SAMPS_PER_YDIV * 2;
        private Double MaxOffsetByADC = Constants.SAMPS_PER_YDIV * 3;

        internal Boolean TemperatureOffsetCalibration()
        {
            var isAvailable = false;
#if DEBUG
            StringBuilder sb = new StringBuilder();
            AppendCaliLog(sb, $"=============================================");
            AppendCaliLog(sb, $"开始温漂校准");
#endif
            IsCaliTemperatureOffset = true;

            //需要自校正的通道
            List<ChannelId> needCaliChannels = new()
            {
                ChannelId.C1, ChannelId.C2, ChannelId.C3,  ChannelId.C4
            };

            //阻抗
            var couplinglist = new List<Coupling>()
            {
                Coupling.HighImpedance,
                Coupling.LowImpedance,
            };

            #region 温漂系数读取
#if DEBUG
            AppendCaliLog(sb, $"初始化温漂系数表");
            //缓存一份用作后面保存用
            var TemperatureCoeffcientTable_FormFile = new Dictionary<ChannelId, Dictionary<AnaChnlCoupling, (Double Coeffcient, Double TemperatureOffset)>>(TemperatureCoeffcientTable);
#endif
            needCaliChannels.ForEach(chnl =>
            {
                Dictionary<AnaChnlCoupling, (double, double)> dic = new();
                couplinglist.ForEach(coupling =>
                {
                    dic.Add((AnaChnlCoupling)coupling, (DefaultTemperatureCoeffcient, 0));
                });
                TemperatureCoeffcientTable[chnl] = dic;
                TemperatureCoeffcientTable_FormFile[chnl] = dic;
            });

#if DEBUG
            AppendCaliLog(sb, $"初始化完成");
            AppendCaliLog(sb, $"从文件中加载上一次温漂系数到内存中");
#endif

            ReadTemperatureCoefficientFromFile(TemperatureCoeffcientTable_FormFile);

#if DEBUG
            AppendCaliLog(sb, $"加载完成");
#endif

            #endregion

            #region 溫度读取
#if DEBUG
            AppendCaliLog(sb, $"开始读取上一次基线校准时通道平均温度");
#endif
            var oldtemperatureByCelsius = AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius / 1000D;//自校正时溫度
#if DEBUG
            AppendCaliLog(sb, $"读取完成，上一次基线校准时通道平均温度：{oldtemperatureByCelsius}℃");
            AppendCaliLog(sb, $"开始读取当前通道平均温度");
#endif
            var currenttemperatureByCelsius = 0D;
            var list = new List<Double>();
            var runtimes = 0;
            for (int count = 0; count < 8;)
            {
                currenttemperatureByCelsius = Double.Parse(SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0));
                if (currenttemperatureByCelsius != 0)
                {
                    count++;
                    list.Add(currenttemperatureByCelsius);
                }
                runtimes++;
#if DEBUG
                Debug.WriteLine($"[Temperature Calibration][{DateTime.Now.ToString("G")}] 第{runtimes}次  temperature = {currenttemperatureByCelsius}°");
#endif
                if (runtimes > 20)
                {
                    Hd.SysLogger?.Invoke($"[Temperature Calibration][{DateTime.Now.ToString("G")}] reading temperature error", "Debug");
                    IsCaliTemperatureOffset = false;
                    return false;
                }
                Thread.Sleep(5);
            }
            list.Sort();
            list.RemoveAt(0);
            list.RemoveAt(1);
            list.RemoveAt(list.Count - 2);
            list.RemoveAt(list.Count - 1);
            currenttemperatureByCelsius = list.Average();
            currenttemperatureByCelsius = currenttemperatureByCelsius < SystemMonitor.InvalidTemperature ? currenttemperatureByCelsius : oldtemperatureByCelsius;
#if DEBUG
            AppendCaliLog(sb, $"读取完成，当前通道平均温度：{currenttemperatureByCelsius}℃");
#endif
            var tempdiff = Math.Abs(currenttemperatureByCelsius - oldtemperatureByCelsius);
            isAvailable = tempdiff >= TemperatureDiff;
            if (!isAvailable)//温差小于10°不校正 使用上一次温补比例，及文件中保存的系数
            {
                //IsCaliTemperatureOffset = false;
#if DEBUG
                AppendCaliLog(sb, $"当前温差（{tempdiff}℃）小于{TemperatureDiff}℃，使用内存中的校准系数，本次校准无效");
                //AppendCaliLog(sb, $"退出温漂校准流程");
                //PrintCaliLog(sb);
#endif
                //return true;
            }

            #endregion

            var result = false;

            HdMessage backupmsg = Hd.UIMessage! with { };

            /***********************************初始化通道信息*******************************************/
#if DEBUG
            AppendCaliLog(sb, $"通道初始化，统一配置带宽限制、时基档、幅度档、存储深度");
#endif
            var scaleValueBymV = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[(Int32)AnaChnlScaleIndex.Lv1m] / 1e3;
            var positionindex = 0;
            var analog0ptions = new List<HdMessage.AnalogOptions>();
            for (var chnl = 0; chnl < needCaliChannels.Count; chnl++)
            {
                HdMessage.AnalogOptions ch = Hd.UIMessage.Analog![chnl] with
                {
                    Active = true,
                    Bandwidth = 3,
                    IsInverted = false,
                    Scale = scaleValueBymV,
                    ScaleIndex = (Int32)AnaChnlScaleIndex.Lv1m,
                    PositionIndex = positionindex,
                    Position = positionindex / 1e3 * scaleValueBymV,
                };
                analog0ptions.Add(ch);
            }

            var mode = Hd.UIMessage!.Timebase! with
            {
                TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv5m].Scale,
                TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv5m,
                StorageWaveDotsCnt = 25 * 1000,
                NeedWaveDotsCnt = 25 * 1000,
                InterleaveMode = AdcInterleaveMode.Mode1To1
            };

            var display = Hd.UIMessage!.Display! with { IsFast = false };

            Hd.UIMessage = Hd.UIMessage! with { Analog = analog0ptions.ToArray(), Timebase = mode, Display = display };
#if DEBUG
            AppendCaliLog(sb, $"通道初始化完成，时基档：{(AnaChnlTimebaseIndex)mode.TmbScaleIndex}，垂直挡位：{(AnaChnlScaleIndex)analog0ptions[0].ScaleIndex}，存储深度：{mode.StorageWaveDotsCnt}");
            AppendCaliLog(sb, $"开始采集数据进行校准");
#endif
            //阻抗循环
            foreach (var coupling in couplinglist)
            {
                /***********************************峰值采样 判断有无信号*******************************************/
#if DEBUG
                AppendCaliLog(sb, $"-------------{(AnaChnlCoupling)coupling}-------------");
                AppendCaliLog(sb, $"设置峰值采样，判断通道有无信号");
#endif
                analog0ptions.Clear();
                for (var chnl = 0; chnl < needCaliChannels.Count; chnl++)
                {
                    HdMessage.AnalogOptions ch = Hd.UIMessage.Analog![chnl] with
                    {
                        Coupling = (AnaChnlCoupling)coupling,
                    };
                    analog0ptions.Add(ch);
                }

                mode = Hd.UIMessage!.Timebase! with
                {
                    AcqMode = AnaChnlAcqMode.Peak,
                };
                Hd.UIMessage = Hd.UIMessage! with { Analog = analog0ptions.ToArray(), Timebase = mode };
                ConfigHardware(Hd.UIMessage, 100);
#if DEBUG
                AppendCaliLog(sb, $"采样模式：{mode.AcqMode}");
                AppendCaliLog(sb, $"开始数据采集");
#endif
                List<ushort[]> channeldata = new();
                var bok = AcqWaveData(out channeldata);
                Thread.Sleep(50);
                bok = AcqWaveData(out channeldata);
                if (!bok)
                {
                    IsCaliTemperatureOffset = false;
#if DEBUG
                    AppendCaliLog(sb, $"数据采集失败");
                    AppendCaliLog(sb, $"退出温漂校准流程");
                    PrintCaliLog(sb);
#endif
                    return false;
                }
#if DEBUG
                AppendCaliLog(sb, $"数据采集完成");
#endif
                var chnlinfo = GetChnlInfo(channeldata);
                if (chnlinfo == null)
                {
                    IsCaliTemperatureOffset = false;
#if DEBUG
                    AppendCaliLog(sb, $"数据读取错误");
                    AppendCaliLog(sb, $"退出温漂校准流程");
                    PrintCaliLog(sb);
#endif
                    return false;
                }
#if DEBUG
                AppendCaliLog(sb, $"数据读取完成");
#endif
                /***********************************读取数据 高分辨率*******************************************/
#if DEBUG
                AppendCaliLog(sb, $"设置高分辨率采样，开始校准系数");
#endif
                mode = Hd.UIMessage!.Timebase! with
                {
                    AcqMode = AnaChnlAcqMode.HighRes,
                };
                Hd.UIMessage = Hd.UIMessage! with { Timebase = mode };
                ConfigHardware(Hd.UIMessage, 100);
#if DEBUG
                AppendCaliLog(sb, $"采样模式：{mode.AcqMode}");
                AppendCaliLog(sb, $"开始数据采集");
#endif

                //一次性读取所有通道数据
                channeldata.Clear();
                bok = AcqWaveData(out channeldata);
                if (!bok)
                {
                    IsCaliTemperatureOffset = false;
#if DEBUG
                    AppendCaliLog(sb, $"数据采集失败");
                    AppendCaliLog(sb, $"退出温漂校准流程");
                    PrintCaliLog(sb);
#endif
                    return false;
                }
#if DEBUG
                AppendCaliLog(sb, $"数据采集完成");
#endif
                var currentchnlinfo = GetChnlInfo(channeldata);
                if (currentchnlinfo == null)
                {
                    IsCaliTemperatureOffset = false;
#if DEBUG
                    AppendCaliLog(sb, $"数据读取错误");
                    AppendCaliLog(sb, $"退出温漂校准流程");
                    PrintCaliLog(sb);
#endif
                    return false;
                }
#if DEBUG
                AppendCaliLog(sb, $"数据读取完成");
#endif
                foreach (var info in chnlinfo)
                {
#if DEBUG
                    AppendCaliLog(sb, $"开始校准{info.Key}");
#endif
                    if (info.Value.HaveSignal)//如果有信号 则本次校准无效
                    {
#if DEBUG
                        AppendCaliLog(sb, $"{info.Key}判定有信号，本次校准系数无效，使用内存中的系数，Max:{info.Value.Max}，Min:{info.Value.Min}，Avg:{info.Value.Avg}，Vpp：{info.Value.Vpp}，Mid：{info.Value.Mid}");
#endif
                        //continue;
                    }

                    var offset = currentchnlinfo[info.Key].Avg - Math.Pow(2, Constants.ADC_BITS) / 2;
                    var offsetBymV = offset * Hd.UIMessage!.Analog![(Int32)info.Key].Scale / Constants.SAMPS_PER_YDIV;
                    var currenttemperatureoffset = currenttemperatureByCelsius - oldtemperatureByCelsius;
                    var coeffcient = 0D;
                    var lasttemperatureoffset = 0D;

                    coeffcient = offsetBymV / currenttemperatureoffset;
                    lasttemperatureoffset = TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].TemperatureOffset;

                    var error = Math.Abs(TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].Coeffcient / coeffcient - 1);
                    if (!isAvailable)//无效
                    {
#if DEBUG
                        AppendCaliLog(sb, $"{info.Key}温差{tempdiff}小于{TemperatureDiff}℃校准系数无效，使用内存中的数据，当前校准系数：{coeffcient}，上一次校准系数：{TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].Coeffcient}");
#endif
                    }
                    else
                    {
                        if (info.Value.HaveSignal)
                        {
#if DEBUG
                            AppendCaliLog(sb, $"{info.Key}判定有信号，本次校准系数无效，使用内存中的数据，当前校准系数：{coeffcient}，上一次校准系数：{TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].Coeffcient}");
#endif
                        }
                        else
                        {
                            //保存当前这场温漂系数到温漂系数表  只要有效就更新至内存中
                            if (coeffcient != 0)
                            {
                                TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling] = (coeffcient, currenttemperatureoffset);
#if DEBUG
                                AppendCaliLog(sb, $"保存{info.Key}温漂系数至内存中，当前校准系数：{coeffcient}，" +
                                                                                $"当前内存中系数：{TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].Coeffcient}，" +
                                                                                $"上一次温漂系数：{TemperatureCoeffcientTable_FormFile[info.Key][(AnaChnlCoupling)coupling].Coeffcient}");
#endif
                            }
                            else
                            {
#if DEBUG
                                AppendCaliLog(sb, $"温漂系数计算异常：温差{currenttemperatureoffset}℃，基线偏移{offsetBymV}mV");
#endif
                            }

                            //                            if (coeffcient != 0 && error > CoeffcientError)
                            //                            {
                            //                                TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling] = ((coeffcient + TemperatureCoeffcientTable[info.Key][(AnaChnlCoupling)coupling].Coeffcient) / 2, currenttemperatureoffset);
                            //#if DEBUG
                            //                                AppendCaliLog(sb, $"保存{info.Key}温漂系数至内存中，温漂系数：{coeffcient}");
                            //#endif
                            //                            }
                            //                            else
                            //                            {
                            //#if DEBUG
                            //                                AppendCaliLog(sb, $"{info.Key}温漂系数与上一次校准系数相差比例（{error}）小于{CoeffcientError}，不保存至内存中");
                            //#endif
                            //                            }
                            var curtemperror = Math.Abs(lasttemperatureoffset - currenttemperatureoffset);

                            if (curtemperror >= TemperatureDiff)//相差超过 ±10°替换上次温漂系数温漂系数和温差并保存在文件下
                            {
                                #region 温漂系数保存

                                SaveTemperatureCoefficientToFile(TemperatureCoeffcientTable_FormFile);
#if DEBUG
                                AppendCaliLog(sb, $"保存{info.Key}温漂系数到文件中，温漂系数：{error}");
#endif
                                #endregion
                            }
                            else
                            {
#if DEBUG
                                AppendCaliLog(sb, $"{info.Key}与上一次校准温度差({curtemperror}℃)小于{TemperatureDiff}℃，不保存到文件");
#endif
                            }
                        }
                    }

                }
            }

#if DEBUG
            AppendCaliLog(sb, $"温漂校准完成");
            AppendCaliLog(sb, $"=============================================");
            PrintCaliLog(sb);
#endif
            IsCaliTemperatureOffset = false;
            return result;
        }

        private Dictionary<ChannelId, ChnlInfo>? GetChnlInfo(List<ushort[]> channeldata)
        {
            if (channeldata == null)
            {
                Hd.SysLogger?.Invoke($"[Temperature Calibration] reading channel data error", "Debug");
                return null;
            }
            var chnlInfos = new Dictionary<ChannelId, ChnlInfo>();
            for (var chnlid = 0; chnlid < channeldata.Count; chnlid++)
            {
                if (channeldata[chnlid].Count() > 0)
                {
                    var pbuffer = channeldata[chnlid].Select(x => (double)x).ToArray();
                    if (!pbuffer.Any())
                        continue;
                    var max = pbuffer.Max();
                    var min = pbuffer.Min();
                    var avg = pbuffer.Average();
                    var vpp = Math.Abs(max - min);
                    var mid = (max + min) / 2;
                    var havesignal = vpp > MaxppByADC || Math.Abs(mid - Math.Pow(2, Constants.ADC_BITS) / 2) > MaxOffsetByADC;
                    chnlInfos.Add((ChannelId)chnlid, new ChnlInfo(havesignal, max, min, avg, vpp, mid));
                }
                else
                {
                    Hd.SysLogger?.Invoke($"[Temperature Calibration][{chnlid}] reading channel data error", "Debug");
                    return null;
                }
            }
            return chnlInfos;
        }

        private void AppendCaliLog(StringBuilder? sb, String info)
        {
            if (sb == null)
            {
                return;
            }
            sb.AppendLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")}：{info}");
        }
        private void PrintCaliLog(StringBuilder sb, String filename)
        {
            if (!Directory.Exists(CaliDataPath))
            {
                Directory.CreateDirectory(CaliDataPath);
            }
            File.AppendAllText(filename, sb.ToString());
        }

        private void PrintCaliLog(StringBuilder sb)
        {
            if (!Directory.Exists(CaliDataPath))
            {
                Directory.CreateDirectory(CaliDataPath);
            }
            var filename = $"{CaliDataPath}{TemperatureCaliLog}";
            File.AppendAllText(filename, sb.ToString());
        }

        private void SaveTemperatureCoefficientToFile(Dictionary<ChannelId, Dictionary<AnaChnlCoupling, (Double Coeffcient, Double TemperatureOffset)>> old)
        {
            if (!Directory.Exists(CaliDataPath))
            {
                Directory.CreateDirectory(CaliDataPath);
            }
            var filename = $"{CaliDataPath}{AnalogChannelTemperaturesCoefficientFile}";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StringBuilder sb = new StringBuilder();
            TemperatureCoeffcientTable.ToList().ForEach(analog =>
            {
                analog.Value.ToList().ForEach(info =>
                {
                    sb.AppendLine($"{analog.Key},{info.Key},{info.Value.Coeffcient * 0.25 + old[analog.Key][info.Key].Coeffcient * 0.75},{info.Value.TemperatureOffset}");
                });
            });
            File.WriteAllText(filename, sb.ToString());
            //保存完之后设置为只读模式
            //File.SetAttributes(filename, FileAttributes.ReadOnly);
        }

        private Boolean ReadTemperatureCoefficientFromFile(Dictionary<ChannelId, Dictionary<AnaChnlCoupling, (Double Coeffcient, Double TemperatureOffset)>> old)
        {
            var fileame = $"{CaliDataPath}{AnalogChannelTemperaturesCoefficientFile}";
            if (!File.Exists(fileame))
            {
                Hd.SysLogger?.Invoke($"[{DateTime.Now.ToString("G")}][Temperature Calibration]: reading temperature coefficient file not exist", "Debug");
                return false;
            }
            string[] allLines = File.ReadAllLines(fileame);
            try
            {
                foreach (var line in allLines)
                {
                    var content = line.Split(",");
                    var chnl = (ChannelId)System.Enum.Parse(typeof(ChannelId), content[0]);
                    var coupling = (AnaChnlCoupling)System.Enum.Parse(typeof(AnaChnlCoupling), content[1]);
                    var cofe = Double.Parse(content[2]);
                    var offset = Double.Parse(content[3]);
                    TemperatureCoeffcientTable[chnl][coupling] = (cofe, offset);
                    old[chnl][coupling] = (cofe, offset);
                }
                return true;
            }
            catch
            {
                Hd.SysLogger?.Invoke($"[{DateTime.Now.ToString("G")}][Temperature Calibration]: reading temperature coefficient data error", "Debug");
                return false;
            }
        }
    }
}
