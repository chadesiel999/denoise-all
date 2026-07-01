using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.PowerAnalysis
{
    internal class PwrSwitchingLossModel : INotifyPropertyChanged
    {
        public class SwitchingLossItem
        {
            private Double _Current;

            public Double Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (!Double.IsNaN(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public SwitchingLossItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrSwitchingLossModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _SwitchLoss = new ConcurrentDictionary<String, SwitchingLossItem>();
            foreach (var item in _Items)
            {
                _SwitchLoss.TryAdd(item,new SwitchingLossItem());
            }
            Count = _Items.Count;
        }

        private readonly List<String> _Items = new List<String>()
        {
            "PowerTOn",//0
            "PowerTConduct",//1
            "PowerTOff",//2
            "PowerTNonConduct",//3
            "PowerTotal",//4
            "EnergyTOn",//5
            "EnergyTConduct",//6
            "EnergyTOff",//7
            "EnergyTNonConduct",//8
            "EnergyTotal",//9
            "TotalTime",//10
            "CycleCount"//11
        };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;
        public String Titles = "Value,Average,Maximum,Minimum";
        private readonly ConcurrentDictionary<String, SwitchingLossItem> _SwitchLoss;

        public SwitchingLossItem this[String key] => _SwitchLoss[key];

        public readonly Int32 Count;


        public Double RdsOn
        {
            get
            {
                return _RdsOn;
            }
            set
            {
                if (value != _RdsOn)
                {
                    _RdsOn = value;
                    OnPropertyChanged("RdsOn");
                }
            }
        }

        public static Double MinRdsOn = 0;
        public static Double MaxRdsOn = 100;



        private readonly List<Int32> _CycleIdx = new();//周期“IIVVI”的起始index
        private List<(Double value, String type)> src = new();//上升沿信息集合


        #region 提取边沿信息
        //提取信号边沿信息
        private static List<(Double value, Int32 type)>? OrderIndexesOf(ChannelId id, Double y10, Double y50, Double y90)
        {
            DsoModel.Default.TryGetChannel(id, out var cm);
            if (cm is not null)
            {
                var pkg = cm.Pack;
                var buffer = pkg?.Buffer.ToEnumerable().Take(pkg.Buffer.GetLength(1)).ToArray();
                var _Range = buffer?.Skip(0)?.Take(10000).ToList();
                IEnumerable<(Double, Int32)> first = from o in _Range.Solve(y10)
                                                   select (o, -1);
                IEnumerable<(Double, Int32)> second = from o in _Range.Solve(y50)
                                                    select (o, 0);
                IEnumerable<(Double, Int32)> second2 = from o in _Range.Solve(y90)
                                                     select (o, 1);
                List<(Double, Int32)> list = (from o in first.Concat(second).Concat(second2)
                                            orderby o.Item1
                                            select o).ToList();
                List<(Double, Int32)> list2 = new();
                if (list.Count <= 0)
                {
                    return list2;
                }

                (Double, Int32) tuple = list[0];
                Double num = tuple.Item1;
                Int32 num2 = tuple.Item2;
                Int32 num3 = 1;
                Double num4 = num;
                for (Int32 i = 1; i < list.Count; i++)
                {
                    (Double, Int32) tuple2 = list[i];
                    Double item = tuple2.Item1;
                    Int32 item2 = tuple2.Item2;
                    if (item2 == num2 && item - num < 25.0)
                    {
                        num4 += item;
                        num3++;
                    }
                    else
                    {
                        num4 /= num3;
                        list2.Add((num4, num2));
                        num2 = item2;
                        num4 = item;
                        num3 = 1;
                    }

                    num = item;
                }

                list2.Add((num4 / num3, num2));
                return list2;
            }
            else
            {
                return null;
            }

        }

        //将上升沿信息(10%&90%位置)从所有的信号边沿信息中提取出来
        private List<(Double value, String type)>? SignalRiseEdge(ChannelId id, String srctype)
        {
            List<(Double value, String type)> RiseEdge = new();
            Double TopValue = _Meas.Calc.ForceGetResultOrCalc("Top", id) ?? Double.NaN;
            Double BtmValue = _Meas.Calc.ForceGetResultOrCalc("Base", id) ?? Double.NaN;
            if (Double.IsNaN(TopValue) || Double.IsNaN(BtmValue) || TopValue - BtmValue <= 1E-12)
            {
                return new List<(Double, String)>
                {
                    (0.0, ""),
                    (0.0, ""),
                    (0.0, "")
                };
            }
            Double y10 = (TopValue - BtmValue) * (10 / 100f) + BtmValue;
            Double y50 = (TopValue - BtmValue) * (50 / 100f) + BtmValue;
            Double y90 = (TopValue - BtmValue) * (90 / 100f) + BtmValue;
            var tmp = OrderIndexesOf(id, y10, y50, y90);
            if (tmp is null)
            {
                return null;
            }

            for (Int32 i = 0; i < tmp.Count - 1; i++)
            {
                (Double, Int32) cur = tmp[i];
                (Double, Int32) next = tmp[i + 1];
                if ((cur.Item2 == -1 && next.Item2 == 0) || (cur.Item2 == 1 && next.Item2 == 1))
                {
                    RiseEdge.Add((cur.Item1, srctype));
                }
            }
            return RiseEdge;
        }

        /*将电压源信号和电流源信号的上升沿信息按从小到大顺序进行排列
        开关损耗的计算按照5个点一周期来计算[(I10%,"I"),(I90%,"I"),(V10%,"V"),(V90%,"V"),(I10%,"I")]来计算Ton、Tcond、Toff和Tnoncond时间段的损耗*/
        private List<(Double value, String type)> RiseEdgeCollection()
        {
            List<(Double value, String type)> VoltageRiseEdge = SignalRiseEdge(Analysis.VoltageSrc1, "V");
            List<(Double value, String type)> CurrentRiseEdge = SignalRiseEdge(Analysis.CurrentSrc1, "I");
            List<(Double value, String type)> AllRiseEdge = (from o in VoltageRiseEdge.Concat(CurrentRiseEdge) orderby o.Item1 select o).ToList();
            return AllRiseEdge;
        }
        #endregion

        #region 获取特定时间点

        //KMP算法
        private Int32 KMP(List<String> waveString, String[] pattern, Int32 start)
        {
            Int32[] next = new Int32[pattern.Length];
            next = kmpNext(pattern);
            for (Int32 i = start, j = 0; i < waveString.Count; i++)
            {
                while (j > 0 && waveString[i] != pattern[j])
                {
                    j = next[j - 1];
                }
                if (waveString[i] == pattern[j])
                {
                    j++;
                }
                if (j == pattern.Length)
                {
                    return i;
                }
            }
            return -1;
        }

        //构造部分匹配表
        private Int32[] kmpNext(String[] pattern)
        {
            Int32[] next = new Int32[pattern.Length];
            next[0] = 0;
            for (Int32 j = 1, k = 0; j < pattern.Length; j++)
            {
                while (k > 0 && pattern[k] != pattern[j])
                {
                    k = next[k - 1];
                }
                if (pattern[j] == pattern[k])
                {
                    k++;
                }
                next[j] = k;

            }
            return next;
        }

        //记录符合开关周期的“IIVVI”起始索引,剔除无关信息
        private List<Int32> CycleStartIndexes()
        {
            _CycleIdx.Clear();
            src = RiseEdgeCollection();
            String[] pattern = new String[] { "I", "I", "V", "V", "I" };
            List<String> TypeCollection = new();
            Int32 index = 0;


            for (Int32 i = 0; i < src.Count; i++)
            {
                TypeCollection.Add(src[i].type);
            }

            while (index < src.Count)
            {
                Int32 pos = KMP(TypeCollection, pattern, index);
                index = pos != -1 ? pos : index + 1;
                if (pos != -1)
                {
                    _CycleIdx.Add(index - 5 + 1);
                }
            }

            return _CycleIdx;
        }

        #endregion

        #region 计算损耗的功率、能量
        private void CalcCycleCount()
        {
            if (_CycleIdx.Count != 0)
            {
                _SwitchLoss[_Items[11]].Current = Quantity.ConvertByPrefix(_CycleIdx.Count, Prefix.Empty);
            }
        }

        private Double CalcSegEnergy(Int32 startpos, Int32 endpos)
        {
            Double energy = 0;
            DsoModel.Default.TryGetChannel(Analysis.CurrentSrc1, out var im);
            DsoModel.Default.TryGetChannel(Analysis.VoltageSrc1, out var vm);
            var impkg = im?.Pack;
            var curbuffer = impkg?.Buffer.ToEnumerable().Take(impkg.Buffer.GetLength(1)).ToArray();
            var vmpkg = vm?.Pack;
            var volbuffer = vmpkg?.Buffer.ToEnumerable().Take(vmpkg.Buffer.GetLength(1)).ToArray();
            if (curbuffer is not null && volbuffer is not null)
            {
                for (Int32 i = startpos; i < endpos; i++)
                {
                    energy += volbuffer[i] * curbuffer[i] * impkg!.Properties.SampInterval;
                }
            }
            return energy;
        }

        /// <summary>
        /// 周期记录
        /// </summary>
        private List<(Int32 start, Int32 end, Double time)> _Cycles = new List<(Int32 start, Int32 end, Double time)>();

        /// <summary>
        /// 开通位置记录
        /// </summary>
        private List<(Int32 start, Int32 end)> _TurnOn = new List<(Int32 start, Int32 end)>();

        /// <summary>
        /// 关断位置记录
        /// </summary>
        private List<(Int32 start, Int32 end)> _TurnOff = new List<(Int32 start, Int32 end)>();

        /// <summary>
        /// 导通位置记录
        /// </summary>
        private List<(Int32 start, Int32 end)> _Conduction = new List<(Int32 start, Int32 end)>();




        private Double _RdsOn = 0.02;

        private void SwitchingLossAnalysis()
        {
            _RunCompleted = false;
            #region 注释的代码
            //Int32 count = _CycleIdx.Count;
            //Double totalTime = CalcTotalTime();

            //_SwitchLoss["TotalTime"].Current = Quantity.ConvertByPrefix(totalTime, Prefix.Empty);

            //_SwitchLoss["EnergyTOn"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(0, count), Prefix.Empty);
            //_SwitchLoss["PowerTOn"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(0, count) / totalTime, Prefix.Empty);

            //_SwitchLoss["EnergyTConduct"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(1, count), Prefix.Empty);
            //_SwitchLoss["PowerTConduct"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(1, count) / totalTime, Prefix.Empty);

            //_SwitchLoss["EnergyTOff"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(2, count), Prefix.Empty);
            //_SwitchLoss["PowerTOff"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(2, count) / totalTime, Prefix.Empty);

            //_SwitchLoss["EnergyTNonConduct"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(3, count), Prefix.Empty);
            //_SwitchLoss["PowerTNonConduct"].Current = Quantity.ConvertByPrefix(CalcRangeEnergy(3, count) / totalTime, Prefix.Empty);//非导通状态下的损耗基本为0

            //_SwitchLoss["EnergyTotal"].Current = _SwitchLoss["EnergyTOn"].Current + _SwitchLoss["EnergyTConduct"].Current + _SwitchLoss["EnergyTOff"].Current + _SwitchLoss["EnergyTNonConduct"].Current;
            //_SwitchLoss["PowerTotal"].Current = _SwitchLoss["EnergyTotal"].Current / _SwitchLoss["TotalTime"].Current;
            #endregion


            _Cycles.Clear();

            _TurnOn.Clear();

            _TurnOff.Clear();

            _Conduction.Clear();
            try
            {

                var currentPkg = DsoModel.Default.GetWfmPack(Analysis.CurrentSrc1);
                var voltagePkg = DsoModel.Default.GetWfmPack(Analysis.VoltageSrc1);
                if (currentPkg == null || voltagePkg == null)
                {
                    return;
                }
                Double[] currentData = currentPkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                Double[] voltageData = voltagePkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                Double[] powerData = voltageData.Zip(currentData, (v, i) => v * i).ToArray();
                var rdson = _RdsOn;
                Double[] conductionData = currentData.Select(x => x * x * rdson).ToArray();


                var paras = DsoPrsnt.DefaultDsoPrsnt.Measure.CalcPwrSwtichLossParas(Analysis.VoltageSrc1);
                var sampinterval = voltagePkg.Properties.SampInterval;
                if (paras.Item1 == null || paras.Item1.Count == 0 || paras.top == null || paras.@base == null)
                {
                    return;
                }

                Double @top = paras.top.Value * 1E-3;
                Double @base = paras.@base.Value * 1E-3;

                FindRealCycles(voltageData, sampinterval, paras.Item1, @top);

                FindTurnOnAndTurnOffs(voltageData, _Cycles, top, @base);

                #region Power
                List<Double> _TurnOnPowers = new List<Double>();
                List<Double> _TurnOffPowers = new List<Double>();
                List<Double> _ConductionPowers = new List<Double>();

                for (Int32 i = 0; i < _TurnOn.Count; i++)
                {
                    var power = CalcPowerIntegral(powerData, _TurnOn[i], sampinterval);
                    _TurnOnPowers.Add(power);
                }

                for (Int32 i = 0; i < _TurnOff.Count; i++)
                {
                    var power = CalcPowerIntegral(powerData, _TurnOff[i], sampinterval);
                    _TurnOffPowers.Add(power);
                }

                for (Int32 i = 0; i < _Conduction.Count; i++)
                {
                    var power = CalcPowerIntegral(conductionData, _Conduction[i], sampinterval);
                    _ConductionPowers.Add(power);
                }

                #endregion


                Double energy_TOn = _TurnOnPowers.Average();
                _SwitchLoss[_Items[5]].Current = Quantity.ConvertByPrefix(energy_TOn, Prefix.Empty);
                Double power_TOn = _TurnOnPowers.Zip(_Cycles.Select(x => x.time), (p, t) => p / t).Average();
                _SwitchLoss[_Items[0]].Current = Quantity.ConvertByPrefix(power_TOn, Prefix.Empty);

                Double energy_TConduct = _ConductionPowers.Average();
                _SwitchLoss[_Items[6]].Current = Quantity.ConvertByPrefix(energy_TConduct, Prefix.Empty);
                Double power_TConduct = _ConductionPowers.Zip(_Cycles.Select(x => x.time), (p, t) => p / t).Average();
                _SwitchLoss[_Items[1]].Current = Quantity.ConvertByPrefix(power_TConduct, Prefix.Empty);

                Double energy_TOff = _TurnOffPowers.Average();
                _SwitchLoss[_Items[7]].Current = Quantity.ConvertByPrefix(energy_TOff, Prefix.Empty);
                Double power_TOff = _TurnOffPowers.Zip(_Cycles.Select(x => x.time), (p, t) => p / t).Average();
                _SwitchLoss[_Items[2]].Current = Quantity.ConvertByPrefix(power_TOff, Prefix.Empty);

                _SwitchLoss[_Items[8]].Current = Quantity.ConvertByPrefix(0, Prefix.Empty);
                _SwitchLoss[_Items[3]].Current = Quantity.ConvertByPrefix(0, Prefix.Empty);//非导通状态下的损耗基本为0

                _SwitchLoss[_Items[9]].Current = _SwitchLoss[_Items[5]].Current + _SwitchLoss[_Items[6]].Current + _SwitchLoss[_Items[7]].Current + _SwitchLoss[_Items[8]].Current;
                _SwitchLoss[_Items[4]].Current = _SwitchLoss[_Items[0]].Current + _SwitchLoss[_Items[1]].Current + _SwitchLoss[_Items[2]].Current + _SwitchLoss[_Items[3]].Current;
                Double time_total = _Cycles.Sum(x => x.time);
                _SwitchLoss[_Items[10]].Current = Quantity.ConvertByPrefix(time_total, Prefix.Empty);
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"{ex.Message }\n{ex.StackTrace}", EventBus.LogLevel.Error));
            }
            finally
            {
                Thread.Sleep(10);
                _RunCompleted = true;
            }
            //旧代码

            //Int32 startIndex = 0;
            //Int32 endIndex = 0;
            //Double maxValue = currentData.Max();
            //endIndex = System.Array.IndexOf(currentData, maxValue);

            //Int32 sumIndex = 0;
            //Double sumValue = 0;
            //Double baseValue = 0;
            //for (Int32 i = (endIndex + 100); i < (endIndex + 300); i++)
            //{
            //    if (i < currentData.Length && currentData[i] > 0)
            //    {
            //        sumValue += currentData[i];
            //        sumIndex++;
            //    }
            //}
            //baseValue = sumValue / sumIndex;
            //baseValue = baseValue + 0.1 * (maxValue - baseValue);
            //for (Int32 i = 1; i < endIndex; i++)
            //{
            //    if ((currentData[i] > baseValue) && (currentData[i + 1] > baseValue)
            //        && (currentData[i + 3] > baseValue) && (currentData[i + 5] > baseValue))
            //    {
            //        startIndex = i;
            //        break;
            //    }
            //}

            //Double energyTOn = 0;
            //Double powerTOn = 0;

            //Double energyTConduct = 0;
            //Double powerTConduct = 0;

            //Double energyTOff = 0;
            //Double powerTOff = 0;
            //Double time = 0;
            //if (startIndex == 0)
            //{
            //    energyTOn = Double.NaN;
            //    powerTOn = Double.NaN;
            //    energyTOff = Double.NaN;
            //    powerTOff = Double.NaN;
            //    energyTConduct = Double.NaN;
            //    powerTConduct = Double.NaN;
            //}
            //else
            //{
            //    powerTOn = 0.1 * (Quantity.ConvertByPrefix(currentData[startIndex], currentPkg!.Properties.ChnlUnit.Prefix, Prefix.Empty) * Quantity.ConvertByPrefix(Math.Abs(voltageData[startIndex]), voltagePkg!.Properties.ChnlUnit.Prefix, Prefix.Empty));
            //    energyTOn = powerTOn * currentPkg!.Properties.SampInterval * 1000;
            //    //powerTOff = 0.5 * (currentData[endIndex] * voltageData[endIndex]);
            //    powerTOff = 0.01 * (Quantity.ConvertByPrefix(currentData[endIndex], currentPkg!.Properties.ChnlUnit.Prefix, Prefix.Empty) * Quantity.ConvertByPrefix(Math.Abs(voltageData[endIndex]), voltagePkg!.Properties.ChnlUnit.Prefix, Prefix.Empty));
            //    energyTOff = powerTOff * currentPkg!.Properties.SampInterval * 10000;
            //    for (Int32 i = startIndex; i < endIndex; i++)
            //    {
            //        powerTConduct += Quantity.ConvertByPrefix(currentData[i], currentPkg!.Properties.ChnlUnit.Prefix, Prefix.Empty) * Quantity.ConvertByPrefix(Math.Abs(voltageData[i]), voltagePkg!.Properties.ChnlUnit.Prefix, Prefix.Empty);//currentData[i] * voltageData[i];
            //        energyTConduct += (Quantity.ConvertByPrefix(currentData[i], currentPkg!.Properties.ChnlUnit.Prefix, Prefix.Empty) * Quantity.ConvertByPrefix(Math.Abs(voltageData[i]), voltagePkg!.Properties.ChnlUnit.Prefix, Prefix.Empty)) * currentPkg!.Properties.SampInterval;
            //    }
            //    powerTConduct = powerTConduct / 1000;
            //    time = (endIndex - startIndex) * currentPkg!.Properties.SampInterval;
            //}

            //_SwitchLoss["EnergyTOn"].Current = Quantity.ConvertByPrefix(energyTOn, Prefix.Empty);
            //_SwitchLoss["PowerTOn"].Current = Quantity.ConvertByPrefix(powerTOn, Prefix.Empty);

            //_SwitchLoss["EnergyTConduct"].Current = Quantity.ConvertByPrefix(energyTConduct, Prefix.Empty);
            //_SwitchLoss["PowerTConduct"].Current = Quantity.ConvertByPrefix(powerTConduct, Prefix.Empty);

            //_SwitchLoss["EnergyTOff"].Current = Quantity.ConvertByPrefix(energyTOff, Prefix.Empty);
            //_SwitchLoss["PowerTOff"].Current = Quantity.ConvertByPrefix(powerTOff, Prefix.Empty);

            //_SwitchLoss["EnergyTNonConduct"].Current = Quantity.ConvertByPrefix(0, Prefix.Empty);
            //_SwitchLoss["PowerTNonConduct"].Current = Quantity.ConvertByPrefix(0, Prefix.Empty);//非导通状态下的损耗基本为0

            //_SwitchLoss["EnergyTotal"].Current = _SwitchLoss["EnergyTOn"].Current + _SwitchLoss["EnergyTConduct"].Current + _SwitchLoss["EnergyTOff"].Current + _SwitchLoss["EnergyTNonConduct"].Current;
            //_SwitchLoss["PowerTotal"].Current = _SwitchLoss["PowerTOn"].Current + _SwitchLoss["PowerTConduct"].Current + _SwitchLoss["PowerTOff"].Current + _SwitchLoss["PowerTNonConduct"].Current;
            //_SwitchLoss["TotalTime"].Current = Quantity.ConvertByPrefix(time, Prefix.Empty);
        }

        private void FindRealCycles(Double[] data, Double sampinterval, List<(Double start, Double end)> cycles, Double @top)
        {
            Int32 lastcycleend = -1;//上一个开关周期的结束位置
            foreach ((Double start, Double end) item in cycles)
            {
                Int32 start = -1, end = -1;
                if (item.start != 0)
                {
                    //向前寻找顶值位置
                    var min = end != -1 ? end : 0;
                    for (Int32 i = (Int32)Math.Round(item.start) + 1; i > min; i--)
                    {
                        if (data[i] > @top)
                        {
                            start = i - 1;
                            break;
                        }
                    }

                    //向前寻找顶值位置
                    for (Int32 i = (Int32)Math.Round(item.end) + 1; i >= item.start; i--)
                    {
                        if (data[i] > @top)
                        {
                            end = i - 1;
                            break;
                        }
                    }


                    if (start != -1 && end != -1 && end > start)
                    {
                        var time = (end - start) * sampinterval;
                        _Cycles.Add((start, end, time));
                        lastcycleend = end;
                    }

                }
            }

        }

        private void FindTurnOnAndTurnOffs(Double[] data, List<(Int32 start, Int32 end, Double time)> cycles, Double @top, Double @base)
        {
            foreach ((Int32 start, Int32 end, Double time) item in cycles)
            {
                Double max = data.Skip(item.start).Take(item.end - item.start + 1).Max();
                Int32 maxindex = data.Skip(item.start).Take(item.end - item.start + 1).FirstIndex(x => x == max)!.Value + item.start;
                Int32 turnonend = -1;
                //turn on
                for (Int32 i = item.start + 1; i < item.end; i++)
                {
                    if (data[i] <= @base)
                    {
                        turnonend = i - 1;
                        break;
                    }
                }

                if (turnonend != -1)
                {
                    _TurnOn.Add((item.start + 1, turnonend));
                }

                Int32 turnoffstart = -1;
                Int32 turnoffend = -1;
                //turn off
                for (Int32 i = maxindex - 1; i > turnonend; i--)
                {
                    if (data[i] <= @base)
                    {
                        turnoffstart = i + 1;
                        break;
                    }
                }

                for (Int32 i = maxindex + 1; i < item.end; i++)
                {
                    if (data[i] < @top)
                    {
                        turnoffend = i - 1;
                        break;
                    }
                }

                if (turnoffstart != -1)
                {
                    _TurnOff.Add((turnoffstart, turnoffend));
                    _Conduction.Add((turnonend + 1, turnoffstart - 1));
                }
            }


        }


        private Double CalcPowerIntegral(Double[] power, (Int32 start, Int32 end) indexs, Double sampinterval)
        {
            Double result = 0.0;

            for (Int32 i = indexs.start; i < indexs.end; i++)
            {
                result += ((power[i] + power[i + 1]) * sampinterval / 2.0);
            }


            return result;
        }

        private Double CalcRangeEnergy(Int32 interval, Int32 count)
        {
            Double energyton = 0;
            for (Int32 i = 0; i < count; i++)
            {
                energyton += CalcSegEnergy((Int32)src[_CycleIdx[i] + interval].value, (Int32)src[_CycleIdx[i] + interval + 1].value);
            }
            return energyton / count;
        }

        private Double CalcTotalTime()
        {
            Int32 count = _CycleIdx.Count;
            Double totaltime = 0;

            for (Int32 i = 0; i < count; i++)
            {
                totaltime += CalcSegTime(Analysis.CurrentSrc1, src[_CycleIdx[i] + 4].value - src[_CycleIdx[i]].value);
            }

            return totaltime / count;
        }

        private Double CalcSegTime(ChannelId src, Double data)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            data *= pkg!.Properties.SampInterval;

            return data;
        }
        #endregion

        private VIType _Source = VIType.V;
        public VIType Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Statistics = true;
        public Boolean Statistics
        {
            get => _Statistics;
            set
            {
                if (_Statistics != value)
                {
                    _Statistics = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _RunCompleted = true;
        public Boolean RunCompleted
        {
            get { return _RunCompleted; }
            set
            {
                if (_RunCompleted != value)
                {
                    _RunCompleted = value;
                }
            }
        }
        public void Run()
        {
            if (_RunCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => SwitchingLossAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }



        public void Reset()
        {
            foreach (var p in _SwitchLoss)
            {
                p.Value.Current = Double.NaN;
                p.Value.StaBuffer.Clear();
            }
        }

        //public void TryShowPowerWfm(ChannelId id)
        //{
        //    _MathId = id;

        //    var pwrch = (MathModel)DsoModel.Default.GetChannel(_MathId);
        //    var pwrexp = $"{MathType.Custom}:{Analysis.VoltageSrc}*{Analysis.CurrentSrc}";
        //    if (pwrch.Formula != pwrexp)
        //    {
        //        pwrch.Formula = pwrexp;
        //        pwrch.Args = new MathCustomArg(_MathId, pwrexp, Analysis);

        //        pwrch.InitFlag = true;
        //    }
        //    pwrch.Label = "VI";
        //    pwrch.Active = true;
        //}
        public ChannelId BoundMathId
        {
            get;
            set;
        } = ChannelId.M1;

        public String Formula => $"{MathType.Custom}:{Analysis.VoltageSrc1}*{Analysis.CurrentSrc1}";

        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);

            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class SwitchingCycle
        {
            public SwitchingCycle(Double[] data, Int32 startindex, Int32 endindex, Int32 minindex, Int32 maxindex)
            {







            }

            /// <summary>
            /// 开关周期开始索引
            /// </summary>
            public Int32 CycleStartIndex { get; set; }

            /// <summary>
            /// 开关周期结束索引
            /// </summary>
            public Int32 CycleEndIndex { get; set; }

            /// <summary>
            /// 开通区域索引
            /// </summary>
            public (Int32 start, Int32 end) TurnOn { get; set; }

            /// <summary>
            /// 关断区域索引
            /// </summary>
            public (Int32 start, Int32 end) TurnOff { get; set; }

            /// <summary>
            /// 导通区域索引
            /// </summary>
            public (Int32 start, Int32 end) Conduction { get; set; }
        }
    }
}
