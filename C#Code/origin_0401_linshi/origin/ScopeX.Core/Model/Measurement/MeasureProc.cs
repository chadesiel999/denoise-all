using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Core
{
    internal class MeasureProc
    {
        public MeasureProc(MeasureModel m)
        {
            Options = m;
        }

        public MeasureModel Options
        {
            get;
        }

        #region Engine
        //测量数据源与参数计算引擎绑定，Key是数据源名+范围
        private readonly ConcurrentDictionary<String, Calculator> _CalcBond = new();

        public static readonly List<String> OperationDescriptions = Enum.GetValues(typeof(MeasureOperator))
                                      .Cast<MeasureOperator>()
                                      .Select(op => op.GetDescription())
                                      .ToList();
        public Calculator? GetOrAdd(ChannelId id, MeasureGate strobe, Boolean newwfm = true)
        {
            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var cm) || !cm.Active)
            {
                return null;
            }
            //if (Dispatcher.IsScan)
            //return null;
            var pkg = cm.Pack;
            if (pkg == null)
            {
                return null;
            }

            Int32 length;
            //if (strobe == MeasureGate.Screen) //在屏幕内
            //{

            //    _StartIndex = pkg.Offset;

            //    length = pkg.Length;
            //}
            //else
            //{
            //    var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(DsoModel.Default.Cursors.VCursor.Source, false);
            //    _StartIndex = datarange.Start.Value;
            //    _EndIndex = datarange.End.Value;
            //    var range = _EndIndex - _StartIndex;
            //    if (range < 0)
            //    {
            //        length = 0;
            //    }
            //    else
            //    {
            //        length = range;
            //    }
            //}

            var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(id, strobe == MeasureGate.Screen);
            var startindex = datarange.Start.Value;
            var endindex = datarange.End.Value;
            var range = endindex - startindex;
            if (range < 0)
            {
                length = 0;
            }
            else
            {
                length = range;
            }


            String key = pkg.Properties.Name + startindex.ToString() + length.ToString();

            //??? [,] =>[]
            Double[] buffer;
            //!!!Patch: Digital channel must be converted to 0 and 1 sequence.
            if (id.IsDigital())
            {
                var idx = Int32.Parse(id.ToString()[1..]) - 1;
                //buffer = pkg.Buffer.ToColumnEnumerable().ElementAt(idx / 16).Select(o => (((UInt16)o) & (0x8000 >> (idx % 16))) != 0 ? 1D : 0D).ToArray();
                buffer = pkg.Buffer.ToRowEnumerable().ElementAt(idx / 16).Select(o => (((UInt16)o) & (0x8000 >> (idx % 16))) != 0 ? 1D : 0D).ToArray();
            }
            else
            {
                buffer = new Double[pkg.Buffer.GetLength(1)];
                Buffer.BlockCopy(pkg.Buffer, 0, buffer, 0, buffer.Length * sizeof(Double));
            }

            //!!!降噪，Disable noise reduction when Double.NaN is here
            //!!!If Sample rate is too small, the move average will reduce the amplitude. 
            //if (!buffer.Any(o => Double.IsNaN(o)))
            //{
            //    buffer = Algorithm.MoveAverage(buffer, 5);
            //}

            if (newwfm && buffer.Length > 0 && length > 0)
            {
                if (_CalcBond.TryGetValue(key, out var val))
                {
                    //val?.Dispose();///释放Calc中缓存，Calc可能存在其他引用，需要手动释放下
                }
                _CalcBond[key] = new Calculator(buffer, startindex, length)
                {
                    MinPk2Pk = pkg.Properties.ChnlScale.Value / 2,// * 10E-3 / 3
                };

                return _CalcBond[key];
            }

            if (_CalcBond.TryGetValue(key, out var calc))
            {
                //_CalcBond.GetOrAdd(key, new Calculator(buffer, _StartIndex, length));
                return calc;
            }

            return null;
        }

        public Calculator? GetOrAddInRange(ChannelId id, Int32 startindex, Int32 endindex)
        {
            if (!DsoModel.Default.TryGetChannel(id, out var cm) || !cm.Active)
            {
                return null;
            }

            var pkg = cm.Pack;
            if (pkg == null)
            {
                return null;
            }

            if (startindex == -1 || endindex == -1 || startindex >= endindex || pkg.Buffer.GetLength(1) < endindex - startindex)
            {
                return null;
            }

            Int32 length = endindex - startindex;
            Double[] buffer = buffer = new Double[length];
            Buffer.BlockCopy(pkg.Buffer, 0, buffer, 0, buffer.Length * sizeof(Double));

            Calculator temp = new Calculator(buffer, startindex, length)
            {
                MinPk2Pk = 0,// * 10E-3 / 3
            };

            return temp;

        }

        //private Calculator GetOrAdd(ChannelId source, Boolean newwfm = true)
        //{
        //    return GetOrAdd(source, Measurement.Default.Strobe, newwfm);
        //}

        private Calculator? Get(ChannelId id, Boolean newwfm = false) => GetOrAdd(id, Options.Strobe, newwfm);

        public static String GetKey(String name, ChannelId source, ChannelId? destination = null) => name + source.ToString() + destination?.ToString();
        #endregion

        #region Result
        //测试结果，Key是测量原和参数名共同构成
        private readonly ConcurrentDictionary<String, Double> _Results = new();

        private void AddResult(MeasureItemModel mi, Double value) => _Results.AddOrUpdate(mi.Key, value, (k, v) => value);
        private void UpdateResult(MeasureItemModel mi, Double value)
        {
            if (_Results.Keys.Contains(mi.Key))
            {
                _Results[mi.Key] = value;
            }
        }
        //private void AddResult(ChannelId source, String name, Double value)
        //{
        //    Results.AddOrUpdate(MeasureItem.GetKey(name, source), value, (k, v) => value);
        //}

        public Double? GetResult(MeasureItemModel mi)
        {
            if (_Results.TryGetValue(mi.Key, out var value))
            {
                return value;
            }

            return null;
        }

        //public Double? GetResult(ChannelId source, String name)
        //{
        //    if (Results.TryGetValue(MeasureItem.GetKey(name, source), out var value))
        //    {
        //        return value;
        //    }

        //    return null;
        //}

        private void RemoveResult(MeasureItemModel mi) => _Results.TryRemove(mi.Key, out var _);

        //private void RemoveResult(ChannelId source, String name)
        //{
        //    Results.TryRemove(MeasureItem.GetKey(name, source), out var _);
        //}
        #endregion

        #region Statistic
        public StatisticBuffer[] StatBuffer
        {
            get;
            private set;
        } = new StatisticBuffer[]
            {
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000),
                new StatisticBuffer(1000)
            };

        public void ClearStat(Int32 index)
        {
            if ((TriggerModel.State == SysState.Stop && !DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure) || !(DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag || DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag))
            {
                return;
            }
            if (!(DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag || DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag))
            {
                return;
            }

            StatBuffer[index].Clear();
        }

        public void ClearAllStat()
        {
            if (!DsoPrsnt.DefaultDsoPrsnt.Measure.ClearStrongFlag)
            {
                if ((TriggerModel.State == SysState.Stop && !DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure) || !(DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag || DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag))
                {
                    return;
                }
                if (!(DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag || DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag))
                {
                    return;
                }
            }

            List<Int32> indices = new List<Int32>(); //当前直方图数据来源
            if (!DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag)
            {
                var mathsources = DsoModel.Default.MathChnls
                                                         .Where(x => x.Active && x.MathType == MathType.Histgram)
                                                         .Select(x => (x.Args as MathHistArg)?.Source)
                                                         .Where(x => x != null)
                                                         .ToHashSet();
                if (mathsources.Count > 0)
                {
                    var mears = DsoModel.Default.Meas.SelectedItems.Where(x => x.Active && mathsources.Contains(x.Id));
                    indices = mears.Select(item => Array.IndexOf(DsoModel.Default.Meas.SelectedItems, item)).ToList();
                }
            }

            for (Int32 i = 0; i < StatBuffer.Length; i++)
            {
                if (!indices.Contains(i))
                {
                    var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source - ChannelId.P1 == i).ToList();
                    maths?.ForEach(x => x.ClearFlag = true);
                    StatBuffer[i].Clear();
                }
            }

            foreach (var key in _Results.Keys)
            {
                _Results[key] = Double.NaN;
            }

            DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag = false;
            DsoPrsnt.DefaultDsoPrsnt.Measure.ClearStrongFlag = false;
        }


        #endregion

        #region Derivative Parameters
        //Single Channel
        private static Double MultiplySampInterval(ChannelId src, Double data)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            data *= pkg?.Properties.SampInterval ?? 1;

            return data;
        }

        private static Double GetAmp(ChannelId src, Double index1, Double index2, Double threshold, Boolean positive)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            Int32 min = (Int32)Math.Min(Math.Ceiling(index1), Math.Ceiling(index2));
            Int32 max = (Int32)Math.Max(Math.Ceiling(index1), Math.Ceiling(index2));
            if (max >= pkg!.Buffer.GetLength(1))
                max = pkg!.Buffer.GetLength(1);
            for (Int32 i = min; i < max + 1; i++)
            {
                if (positive)
                {
                    if (pkg!.Buffer[0, i] > threshold)
                    {
                        return pkg!.Buffer[0, i];
                    }
                }
                else
                {
                    if (pkg!.Buffer[0, i] < threshold)
                    {
                        return pkg!.Buffer[0, i];
                    }
                }

            }
            return Double.NaN;
        }

        private static (Double, Double) GetFristPoint(ChannelId src, Double index1, Double index2, Double threshold, Int32 type)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            Int32 min = (Int32)Math.Min(Math.Floor(index1), Math.Floor(index2));
            Int32 max = (Int32)Math.Max(Math.Ceiling(index1), Math.Ceiling(index2));
            if (max >= pkg!.Buffer.GetLength(1))
                max = pkg!.Buffer.GetLength(1);
            if (type > 0)
            {
                for (Int32 i = min; i < max + 1; i++)
                {
                    if (pkg!.Buffer[0, i] > threshold)
                    {
                        return (i, pkg!.Buffer[0, i]);
                    }
                }
            }
            else
            {
                for (Int32 i = max; i > min - 1; i--)
                {
                    if (pkg!.Buffer[0, i] > threshold)
                    {
                        return (i, pkg!.Buffer[0, i]);
                    }
                }
            }
            return (Double.NaN, Double.NaN);

        }

        private static Double? GetSkewFromTrigger(ChannelId src, Double data)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            if (pkg?.Properties != null)
            {
                return pkg!.Properties.SampInterval * (data - pkg.Properties.TmbPosition.Index);
            }

            return null;
        }

        private static Double GetIndex(ChannelId src, Double index1, Double index2, Double threshold, Boolean positive)
        {
            var pkg = DsoModel.Default.GetWfmPack(src);

            Int32 min = (Int32)Math.Min(Math.Ceiling(index1), Math.Ceiling(index2));
            Int32 max = (Int32)Math.Max(Math.Ceiling(index1), Math.Ceiling(index2));
            if (max >= pkg!.Buffer.GetLength(1))
                max = pkg!.Buffer.GetLength(1);
            for (Int32 i = min; i < max + 1; i++)
            {
                if (positive)
                {
                    if (pkg!.Buffer[0, i] > threshold)
                    {
                        return i;
                    }
                }
                else
                {
                    if (pkg!.Buffer[0, i] < threshold)
                    {
                        return i;
                    }
                }
            }
            return Double.NaN;
        }

        private static (Double Duty, List<Double> Values)? CalcDuty(Calculator calc, PulsePolarity polarity = PulsePolarity.Positive)
        {
            var pw = calc.Take(MeasParameter.PWidth);
            var nw = calc.Take(MeasParameter.NWidth);
            var pd = calc.Take(MeasParameter.Period);
            if (pw != null && pd != null)
            {
                var duty = (pw[1] - pw[0]) * 1.00 / (pd[1] - pd[0]);
                return polarity == PulsePolarity.Positive ? (duty, pw) : (1 - duty, nw);
            }

            return null;
        }

        private static Double? CalcPosOverShoot(Calculator calc)
        {
            var os = calc.Take(MeasParameter.POverShoot);
            var ap = calc.Take(MeasParameter.Amplitude);
            if (os != null && ap != null)
            {
                if (os[1] < os[0])
                    return 0;
                return (os[1] - os[0]) / Math.Abs(ap[1] - ap[0]);
            }

            return null;
        }

        private static Double? CalcNegOverShoot(Calculator calc)
        {
            var os = calc.Take(MeasParameter.NOverShoot);
            var ap = calc.Take(MeasParameter.Amplitude);
            if (os != null && ap != null)
            {
                if (os[0] < os[1])
                    return 0;
                return (os[0] - os[1]) / Math.Abs(ap[1] - ap[0]);
            }

            return null;
        }

        private static Double? CalcPosPreShoot(Calculator calc)
        {
            var ps = calc.Take(MeasParameter.PPreShoot);
            var ap = calc.Take(MeasParameter.Amplitude);
            if (ps != null && ap != null)
            {
                if (ps[1] < ps[0])
                    return 0;
                return (ps[1] - ps[0]) * 1.00 / Math.Abs(ap[1] - ap[0]);
            }

            return null;
        }

        private static Double? CalcNegPreShoot(Calculator calc)
        {
            var ps = calc.Take(MeasParameter.NPreShoot);
            var ap = calc.Take(MeasParameter.Amplitude);
            if (ps != null && ap != null)
            {
                if (ps[0] < ps[1])
                    return 0;
                return (ps[0] - ps[1]) * 1.00 / Math.Abs(ap[1] - ap[0]);
            }

            return null;
        }

        private Double CalcTAtMax(ChannelId src, Calculator calc)
        {
            var pkg = DsoModel.Default.GetChannel(src).Pack;

            if (pkg == null)
            {
                return Double.NaN;
            }

            var tmax = calc.Take(MeasParameter.TAtMax)[0];
            return tmax + calc.StartIndex - pkg.Properties.TmbPosition.Index /*pkg.XSysDepth * pkg.XSysSamples*/;
        }

        private Double CalcTAtMin(ChannelId src, Calculator calc)
        {
            var pkg = DsoModel.Default.GetChannel(src).Pack;

            if (pkg == null)
            {
                return Double.NaN;
            }

            var tmax = calc.Take(MeasParameter.TAtMin)[0];
            return tmax + calc.StartIndex - pkg.Properties.TmbPosition.Index/*pkg.XSysDepth * pkg.XSysSamples*/;
        }

        private static Double CalcOutsideTime(Calculator calc, Int32 type)
        {
            var orderidx = calc.TakeOutsideTime(type);
            if (orderidx != null)
            {
                return orderidx.Difference().Where((o, i) => i % 2 == 0).Sum();
            }

            return 0;
        }

        private static List<(Double value, Int32 type)>? GetOrderIndexes(MeasureItemModel mi, Calculator calc)
        {
            Double ytop, ybase;
            switch (mi.RefLevel.RefStandard)
            {
                case MeasureTopBaseRef.MinMax:
                    ytop = calc.Take(MeasParameter.Max)[0];
                    ybase = calc.Take(MeasParameter.Min)[0];
                    break;
                case MeasureTopBaseRef.ZeroMax:
                    ytop = calc.Take(MeasParameter.Max)[0];
                    ybase = 0;
                    break;
                case MeasureTopBaseRef.ZeroMin:
                    ytop = 0;
                    ybase = calc.Take(MeasParameter.Min)[0];
                    break;

                default:
                    ytop = calc.Take(MeasParameter.HistTop)[0];
                    ybase = calc.Take(MeasParameter.HistBase)[0];
                    break;
            }

            if (mi.RefLevel.RefUnit == MeasureTopBaseRefUnit.Percent)
            {
                Int32 y10 = mi.RefLevel.LowThrold;
                Int32 y50 = mi.RefLevel.MidThrold;
                Int32 y90 = mi.RefLevel.HighThrold;

                if (y10 >= y50 - 5)
                {
                    return null;
                }

                if (y10 >= y90 - 10)
                {
                    return null;
                }

                if (y50 >= y90 - 5)
                {
                    return null;
                }
                return calc.TakeOrderIndexesBy(ytop, ybase, y50, y10, y90);
            }
            else if (mi.RefLevel.RefUnit == MeasureTopBaseRefUnit.Absolute)
            {
                return calc.TakeOrderIndexesByAbsolute(ytop, ybase, mi.RefLevel.MidAbsoluteThrold, mi.RefLevel.LowAbsoluteThrold, mi.RefLevel.HighAbsoluteThrold);
            }
            else
            {
                return null;
            }
        }

        private static List<Double>? CalcEdgeAtLv(MeasureItemModel mi, Calculator calc, Int32 type, Boolean first = true)
        {
            var orderidx = GetOrderIndexes(mi, calc);
            if (orderidx != null)
            {
                return first ? Calculator.TakeIndexOfFirstEdge(orderidx, type) : Calculator.TakeIndexesOfEdge(orderidx, type);
            }

            return null;
        }


        private static List<Double>? CalcPeriodAtLv(MeasureItemModel mi, Calculator calc, Boolean first = true)
        {
            var orderidx = GetOrderIndexes(mi, calc);
            if (orderidx != null)
            {
                //return first ? TakeIndexOfFirstPeriod(orderidx) : TakeIndexesOfPeriod(orderidx);
                return first ? calc.TakeIndexOfFirstPeriod(orderidx) : Calculator.TakeIndexesOfPeriod(orderidx);
            }

            return null;
        }

        private static List<Double>? CalcWidthAtLv(MeasureItemModel mi, Calculator calc, Int32 type, Boolean first = true)
        {
            var orderidx = GetOrderIndexes(mi, calc);
            if (orderidx != null)
            {
                return first ? Calculator.TakeIndexOfFirstWidth(orderidx, type) : Calculator.TakeIndexesOfWidth(orderidx, type);
            }

            return null;
        }

        private List<Double>? CalcDelayAtLv(MeasureItemModel mi, Boolean fromRiseEdge = true, Boolean ToRiseEdge = true)
        {
            var stcalc = GetOrAdd(mi.Source, MeasureGate.Screen);
            var edcalc = GetOrAdd(mi.Source2nd, MeasureGate.Screen);

            if (stcalc != null && edcalc != null)
            {
                var storderidx = GetOrderIndexes(mi, stcalc);
                var edorderidx = GetOrderIndexes(mi, edcalc);

                if (storderidx != null && edorderidx != null)
                {
                    var startmid = Calculator.TakeIndexesOfWidth(storderidx, fromRiseEdge ? 1 : -1).Where((o, i) => i % 2 == 0).ToList();
                    var endmid = Calculator.TakeIndexesOfWidth(edorderidx, ToRiseEdge ? 1 : -1).Where((o, i) => i % 2 == 0).ToList();

                    if (startmid != null && endmid != null)
                    {
                        //Int32 j = 0;
                        //for (Int32 i = 1; i < startmid.Count; i += 2)
                        //{
                        //    j = endmid.FindIndex(j, o => o > startmid[i - 1]);
                        //    if (j >= 0)
                        //    {
                        //        startmid[i] = endmid[j];
                        //    }
                        //    else
                        //    {
                        //        startmid.RemoveRange(i - 1, startmid.Count - (i - 1));
                        //        break;
                        //    }
                        //}
                        List<Double> combinedSquence = new List<Double>();
                        for (Int32 i = 0; i < startmid.Count; i++)
                        {
                            if (i < endmid.Count)
                            {
                                combinedSquence.Add(startmid[i]);
                                combinedSquence.Add(endmid[i]);
                            }
                            else
                            {
                                break;
                            }

                        }
                        return combinedSquence;
                        //return startmid;
                    }
                }
            }
            return null;
        }

        private static Double? CalcFirstDutyAtLv(MeasureItemModel mi, Calculator calc)
        {
            var orderidx = GetOrderIndexes(mi, calc);
            if (orderidx != null)
            {
                var pw = Calculator.TakeIndexOfFirstWidth(orderidx, 1);
                var pd = calc.TakeIndexOfFirstPeriod(orderidx);
                //var pw = TakeIndexOfFirstWidth(orderidx, 1);
                //var pd = TakeIndexOfFirstPeriod(orderidx);
                if (pw != null && pd != null)
                {
                    return (pw[1] - pw[0]) /** 100*/ / (pd[1] - pd[0]);
                }
            }
            return null;
        }

        private List<Double>? CalcFirstDelayAtLv(MeasureItemModel mi, MeasureGate strobe, Boolean fromRiseEdge = true, Boolean ToRiseEdge = true)
        {
            //if (mi.Source2nd == null)
            //{
            //    return null;
            //}

            var stcalc = GetOrAdd(mi.Source, strobe);
            var edcalc = GetOrAdd(mi.Source2nd, strobe);

            if (stcalc != null && edcalc != null)
            {
                //if (mi.RefLevel.MidThrold != 50)
                //{
                //    var storderidx = GetOrderIndexes(mi, stcalc);
                //    var edorderidx = GetOrderIndexes(mi, edcalc);

                //    if (storderidx != null && edorderidx != null)
                //    {
                //        //var stmid = TakeIndexOfFirstMid(storderidx, fromRiseEdge ? 1 : -1);
                //        //var edmid = TakeIndexOfFirstMid(edorderidx, ToRiseEdge ? 1 : -1);
                //        var stmid = Calculator.TakeIndexOfFirstMid(storderidx, fromRiseEdge ? 1 : -1);
                //        var edmid = Calculator.TakeIndexOfFirstMid(edorderidx, ToRiseEdge ? 1 : -1);

                //        if (stmid != null && edmid != null)
                //        {
                //            return new List<Double> { stmid[0], edmid[0] };
                //        }
                //    }
                //}
                //else
                //{
                //    //List<Double> stmid = stcalc?.Take(fromRiseEdge ? MeasParameter.FirstRiseMid : MeasParameter.FirstFallMid);
                //    //List<Double> edmid = edcalc?.Take(ToRiseEdge ? MeasParameter.FirstRiseMid : MeasParameter.FirstFallMid);
                //    List<Double>? stmid = null;
                //    List<Double>? edmid = null;
                //    if (stcalc!=null)
                //    {
                //        //stmid=TakeIndexOfFirstMid(stcalc, 1);
                //        //stmid= stcalc.TakeIndexOfFirstMid(fromRiseEdge ? 1 : -1);
                //        //stmid = fromRiseEdge ? stcalc.Take(MeasParameter.FirstRiseMid) : edcalc.Take(MeasParameter.FirstFallMid);
                //        stmid = Calculator.TakeIndexOfFirstMid(stcalc.OrderIndexes, fromRiseEdge ? 1 : -1);
                //    }
                //    if (edcalc!=null)
                //    {
                //        //edmid=TakeIndexOfFirstMid(edcalc, 1);
                //        //edmid = ToRiseEdge ? edcalc.Take(MeasParameter.FirstRiseMid) : edcalc.Take(MeasParameter.FirstFallMid);
                //        //edmid = edcalc.TakeIndexOfFirstMid( ToRiseEdge ? 1 : -1);
                //        edmid = Calculator.TakeIndexOfFirstMid(edcalc.OrderIndexes, ToRiseEdge ? 1 : -1);
                //    }

                //    if (stmid != null && edmid != null)
                //    {
                //        return new List<Double> { stmid[0], edmid[0] };
                //    }
                //}

                var storderidx = GetOrderIndexes(mi, stcalc);
                var edorderidx = GetOrderIndexes(mi, edcalc);

                if (storderidx != null && edorderidx != null)
                {
                    //var stmid = TakeIndexOfFirstMid(storderidx, fromRiseEdge ? 1 : -1);
                    //var edmid = TakeIndexOfFirstMid(edorderidx, ToRiseEdge ? 1 : -1);
                    var stmid = Calculator.TakeIndexOfFirstMid(storderidx, fromRiseEdge ? 1 : -1);
                    if (stmid != null)
                    {
                        var edmid = Calculator.TakeIndexOfFirstMid(edorderidx, ToRiseEdge ? 1 : -1, (Int32)stmid[0]);
                        if (edmid != null)
                        {
                            return new List<Double> { stmid[0], edmid[0] };
                        }
                    }


                }

            }
            return null;
        }

        private List<Double>? CalcRatio(MeasureItemModel mi, MeasureGate strobe)
        {
            //if (mi.Source2nd == null)
            //{
            //    return null;
            //}

            var stcalc = GetOrAdd(mi.Source, strobe);
            var edcalc = GetOrAdd(mi.Source2nd, strobe);



            if (stcalc != null && edcalc != null)
            {
                var scrms = stcalc.Take(MeasParameter.RMS);
                var edrms = edcalc.Take(MeasParameter.RMS);
                return new List<Double> { scrms[0] / edrms[0] };
            }
            return null;
        }

        private Double? CalcFirstPhaseAtLv(MeasureItemModel mi, MeasureGate strobe)
        {
            var delay = CalcFirstDelayAtLv(mi, strobe);

            if (delay != null)
            {
                var stcalc = GetOrAdd(mi.Source, strobe);
                Double? result = null;
                if (stcalc != null)
                {
                    if (mi.RefLevel.MidThrold != 50)
                    {
                        var storderidx = GetOrderIndexes(mi, stcalc);

                        if (storderidx != null)
                        {
                            var pd = Calculator.TakeIndexesOfPeriod(storderidx);

                            if (pd != null)
                            {
                                result = (delay[1] - delay[0]) * 360 / (pd[1] - pd[0]);
                            }
                        }
                    }
                    else
                    {
                        var pd = stcalc?.Take(MeasParameter.Period);
                        //var pd = stcalc != null ? TakePeriod(stcalc) : null;
                        if (pd != null)
                        {
                            result = (delay[1] - delay[0]) * 360 / (pd[1] - pd[0]);
                        }
                    }
                }

                if (result != null)
                {
                    if (Math.Abs(result.Value) > 360)
                        result = result.Value % 360;

                    if (result.Value < 0)
                        result = 360 + result;

                    return result;
                }
            }

            return null;
        }

        private List<Double>? CalcFirstCrossing(MeasureItemModel mi, MeasureGate strobe)
        {
            var stcalc = GetOrAdd(mi.Source, strobe);
            var edcalc = GetOrAdd(mi.Source2nd, strobe);

            if (stcalc != null && edcalc != null)
            {
                var presign = stcalc.Range[0] - edcalc.Range[0] >= 1e-7;
                for (Int32 i = 1; i < stcalc.Range.Count; i++)
                {
                    var cursign = stcalc.Range[i] - edcalc.Range[i] >= 1e-7;
                    if (cursign ^ presign)
                    {
                        return new() { Algorithm.GetCrossPoint(stcalc.Range[i - 1], stcalc.Range[i], edcalc.Range[i - 1], edcalc.Range[i]) };
                    }
                    presign = cursign;
                }
            }
            return null;
        }
        #endregion

        #region Run
        private Double? Calc(MeasureItemModel mi, MeasureGate strobe)
        {
            var calc = GetOrAdd(mi.Source, strobe);
            if (calc == null)
            {
                return null;
            }

            List<Double>? res;
            switch (mi.Name)
            {
                case nameof(MeasParameter.Max):
                    res = calc.Take(MeasParameter.Max);
                    if (res != null)
                    {
                        return res[0];
                    }
                    break;
                case nameof(MeasParameter.Min):
                    res = calc.Take(MeasParameter.Min);
                    if (res != null)
                    {
                        return res[0];
                    }
                    break;
                case nameof(MeasParameter.Average):
                    return calc.Take(MeasParameter.Average)[0];
                case nameof(MeasParameter.Pk2Pk):
                    res = calc.Take(MeasParameter.Pk2Pk);
                    return res[0] - res[1];
                case nameof(MeasParameter.RMS):
                    return calc.Take(MeasParameter.RMS)[0];
                case nameof(MeasParameter.Stddev):
                    return calc.Take(MeasParameter.Stddev)[0];

                case nameof(MeasParameter.Top):
                    return calc.Take(MeasParameter.Top)[0];
                case nameof(MeasParameter.Base):
                    return calc.Take(MeasParameter.Base)[0];
                case nameof(MeasParameter.Amplitude):
                    res = calc.Take(MeasParameter.Amplitude);
                    return res[0] - res[1];
                case nameof(MeasParameter.Mid):
                    return calc.Take(MeasParameter.Mid)[0];
                case nameof(MeasParameter.Upper):
                    return calc.Take(MeasParameter.Upper)[0];
                case nameof(MeasParameter.Lower):
                    return calc.Take(MeasParameter.Lower)[0];

                case nameof(MeasParameter.POverShoot):
                    return CalcPosOverShoot(calc);
                case nameof(MeasParameter.NOverShoot):
                    return CalcNegOverShoot(calc);

                case nameof(MeasParameter.PPreShoot):
                    return CalcPosPreShoot(calc);
                case nameof(MeasParameter.NPreShoot):
                    return CalcNegPreShoot(calc);
                case nameof(MeasParameter.Period):
                    res = calc.Take(MeasParameter.Period);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.Freq):
                    res = calc.Take(MeasParameter.Freq);
                    if (res != null)
                    {
                        return 1 / MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.NPeriods):
                    res = calc.Take(MeasParameter.NPeriods);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.PWidth)://正脉宽
                    res = calc.Take(MeasParameter.PWidth);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.NWidth)://负脉宽
                    res = calc.Take(MeasParameter.NWidth);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case nameof(MeasParameter.Rise):
                    res = calc.Take(MeasParameter.Rise);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.Fall):
                    res = calc.Take(MeasParameter.Fall);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case "Duty":
                    return CalcDuty(calc, PulsePolarity.Positive)?.Duty;
                case "PDuty"://正占空比
                    return CalcDuty(calc, PulsePolarity.Positive)?.Duty;
                case "NDuty"://负占空比
                    return CalcDuty(calc, PulsePolarity.Negative)?.Duty;
                case "RSlewRate":
                    res = calc.Take(MeasParameter.Rise);
                    if (res != null)
                    {
                        return calc.Take(MeasParameter.Amplitude)[0] * 0.8 / MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "FSlewRate":
                    res = calc.Take(MeasParameter.Fall);
                    if (res != null)
                    {
                        return calc.Take(MeasParameter.Amplitude)[0] * 0.8 / MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case nameof(MeasParameter.BurstLen):
                    res = calc.Take(MeasParameter.BurstLen);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case nameof(MeasParameter.BurstInterval):
                    res = calc.Take(MeasParameter.BurstInterval);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[0]);
                    }
                    break;
                case nameof(MeasParameter.BurstWidth):
                    res = calc.Take(MeasParameter.BurstWidth);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[0]);
                    }
                    break;
                case "BurstPeriod":
                    var bw = calc.Take(MeasParameter.BurstWidth);
                    var bi = calc.Take(MeasParameter.BurstInterval);
                    if (bw != null && bi != null)
                    {
                        res = new List<Double>() { bw[0] + bi[0] };
                        return MultiplySampInterval(mi.Source, res[0]);
                    }
                    break;
                case nameof(MeasParameter.BurstCycle):
                    res = calc.Take(MeasParameter.BurstCycle);
                    if (res != null)
                    {
                        return res[0];
                    }
                    break;
                case nameof(MeasParameter.Area):
                    return MultiplySampInterval(mi.Source, -calc.Take(MeasParameter.Sum)[0]);
                case nameof(MeasParameter.AbsArea):
                    return MultiplySampInterval(mi.Source, calc.Take(MeasParameter.AbsoluteSum)[0]);
                case nameof(MeasParameter.RiseEdges):
                case nameof(MeasParameter.Cycles):
                    res = calc.Take(MeasParameter.Cycles);
                    if (res != null)
                    {
                        return res.Count;
                    }
                    break;
                
                    res = calc.Take(MeasParameter.RiseEdges);
                    if (res != null)
                    {
                        return res.Count / 2;
                    }
                    break;
                case nameof(MeasParameter.FallEdges):
                    res = calc.Take(MeasParameter.FallEdges);
                    if (res != null)
                    {
                        if (res.Count==0)
                        {
                            return null;
                        }
                        return res.Count / 2;
                    }
                    break;
                case nameof(MeasParameter.PosPulses):
                    res = calc.Take(MeasParameter.PosPulses);
                    if (res != null)
                    {
                        if (res.Count == 0)
                        {
                            return null;
                        }
                        return res.Count / 2;
                    }
                    break;
                case nameof(MeasParameter.NegPulses):
                    res = calc.Take(MeasParameter.NegPulses);
                    if (res != null)
                    {
                        if (res.Count == 0)
                        {
                            return null;
                        }
                        return res.Count / 2;
                    }
                    break;
                case nameof(MeasParameter.WfmLength):
                    return calc.Take(MeasParameter.WfmLength)[0];
                case "T@max":
                    return MultiplySampInterval(mi.Source, CalcTAtMax(mi.Source, calc));
                case "T@min":
                    return MultiplySampInterval(mi.Source, CalcTAtMin(mi.Source, calc));

                case "Rise@lv":
                    res = CalcEdgeAtLv(mi, calc, 1);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "Fall@lv":
                    res = CalcEdgeAtLv(mi, calc, -1);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case "Period@lv":
                    res = CalcPeriodAtLv(mi, calc);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "Freq@lv":
                    res = CalcPeriodAtLv(mi, calc);
                    if (res != null)
                    {
                        return 1 / MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case "Width@lv":
                    res = CalcWidthAtLv(mi, calc, 1);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "Duty@lv":
                    return CalcFirstDutyAtLv(mi, calc);
                case "Skew":
                case "RRDelay@lv":
                case "SetupTime":
                    res = CalcFirstDelayAtLv(mi, strobe);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "FFDelay@lv":
                    res = CalcFirstDelayAtLv(mi, strobe, false, false);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "RFDelay@lv":
                //case "Ratio":

                //        return CalcRatio(mi, strobe)[0];


                case "HoldTime":
                    res = CalcFirstDelayAtLv(mi, strobe, true, false);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;
                case "FRDelay@lv":
                    res = CalcFirstDelayAtLv(mi, strobe, false, true);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[1] - res[0]);
                    }
                    break;

                case "Phase@lv":
                    return CalcFirstPhaseAtLv(mi, strobe);
                case nameof(MeasParameter.Crossing):
                    return CalcFirstCrossing(mi, strobe)?[0];
                case nameof(MeasParameter.CycMax):
                    return calc.Take(MeasParameter.CycMax)[0];
                case nameof(MeasParameter.CycMin):
                    return calc.Take(MeasParameter.CycMin)[0];
                case nameof(MeasParameter.CycRMS):
                    return calc.Take(MeasParameter.CycRMS)[0];
                case nameof(MeasParameter.CycAverage):
                    return calc.Take(MeasParameter.CycAverage)[0];
                case nameof(MeasParameter.CycPeak):
                    //var cycp = TakeFirstCycPeak(calc);
                    var cycp = calc.Take(MeasParameter.CycPeak);
                    if (cycp != null)
                    {
                        return cycp[0];
                    }
                    break;
                case nameof(MeasParameter.CycMid):
                    return calc.Take(MeasParameter.CycMid)[0];
                case nameof(MeasParameter.CycArea):
                    return MultiplySampInterval(mi.Source, calc.Take(MeasParameter.CycArea)[0]);
                case nameof(MeasParameter.AbsCycArea):
                    return MultiplySampInterval(mi.Source, calc.Take(MeasParameter.AbsCycArea)[0]);
                case nameof(MeasParameter.OutsideTime):
                    return MultiplySampInterval(mi.Source, CalcOutsideTime(calc, 1) + CalcOutsideTime(calc, -1));
                case nameof(MeasParameter.HighTime):
                    return MultiplySampInterval(mi.Source, CalcOutsideTime(calc, 1));
                case nameof(MeasParameter.LowTime):
                    return MultiplySampInterval(mi.Source, CalcOutsideTime(calc, -1));
                case nameof(MeasParameter.HistMean):
                    return calc.Take(MeasParameter.HistMean)[0];
                case nameof(MeasParameter.HistMax):
                    return calc.Take(MeasParameter.HistMax)[0];
                case nameof(MeasParameter.HistMin):
                    return calc.Take(MeasParameter.HistMin)[0];
                case nameof(MeasParameter.HistMid):
                    return calc.Take(MeasParameter.HistMid)[0];
                case nameof(MeasParameter.HistRange):
                    return calc.Take(MeasParameter.HistRange)[0];
                case nameof(MeasParameter.HistMaxPop):
                    return calc.Take(MeasParameter.HistMaxPop)[0];
                case nameof(MeasParameter.HistMeanPop):
                    return calc.Take(MeasParameter.HistMeanPop)[0];
                case nameof(MeasParameter.HistMode):
                    return calc.Take(MeasParameter.HistMode)[0];
                case nameof(MeasParameter.HistTotalPop):
                    return calc.Take(MeasParameter.HistTotalPop)[0];
                case nameof(MeasParameter.HistTop):
                    return calc.Take(MeasParameter.HistTop)[0];
                case nameof(MeasParameter.HistBase):
                    return calc.Take(MeasParameter.HistBase)[0];
                case nameof(MeasParameter.HistAmp):
                    return calc.Take(MeasParameter.HistAmp)[0];
                case nameof(MeasParameter.HistArea):
                    return calc.Take(MeasParameter.HistArea)[0];
                case nameof(MeasParameter.HistMu1Sigma):
                    return calc.Take(MeasParameter.HistMu1Sigma)[0];
                case nameof(MeasParameter.HistMu2Sigma):
                    return calc.Take(MeasParameter.HistMu2Sigma)[0];
                case nameof(MeasParameter.HistMu3Sigma):
                    return calc.Take(MeasParameter.HistMu3Sigma)[0];
                case nameof(MeasParameter.HistSigma):
                    return calc.Take(MeasParameter.HistSigma)[0];
                case nameof(MeasParameter.HistWfmCnt):
                    return calc.Take(MeasParameter.HistWfmCnt)[0];

                case nameof(MeasParameter.PulseCount):
                    return calc.Take(MeasParameter.PulseCount)[0];
                case "DataRate":
                    res = calc.Take(MeasParameter.UI);
                    if (res != null)
                    {
                        return 1 / MultiplySampInterval(mi.Source, res[0]);
                    }
                    break;
                case nameof(MeasParameter.UI):
                    res = calc.Take(MeasParameter.UI);
                    if (res != null)
                    {
                        return MultiplySampInterval(mi.Source, res[0]);
                    }
                    break;

            }
            return null;
        }

        private List<(Double Start, Double End)>? CalcForPowerByScreen(String measureName, ChannelId soure, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90)
        {
            var calc = GetOrAdd(soure, MeasureGate.Screen);
            if (calc == null)
            {
                return null;
            }

            List<(Double Start, Double End)>? res = null;
            switch (measureName)
            {
                case nameof(MeasParameter.Freq):
                case nameof(MeasParameter.Period):
                    {
                        res = calc.TakeIndexOfAllPeriodsBy(0, mp, bp, tp);
                    }
                    break;
                case nameof(MeasParameter.FallEdges):
                    res = calc.TakeEdgesBy(-1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.RiseEdges):
                    res = calc.TakeEdgesBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.PosPulses):
                    res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.NegPulses):
                    res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    break;
                case "PDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    }
                    break;
                case "NDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    }
                    break;
            }

            return res;
        }

        private List<(Double Start, Double End)>? CalcForPowerAndGetCalcDataByScreen(String measureName, ChannelId soure, ref (Double[] data, Double fs)? data, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90)
        {
            var calc = PowerGetOrAddAndGetCalcData(soure, MeasureGate.Screen, ref data);
            if (calc == null)
            {
                return null;
            }

            List<(Double Start, Double End)>? res = null;
            switch (measureName)
            {
                case nameof(MeasParameter.Freq):
                case nameof(MeasParameter.Period):
                    {
                        res = calc.TakeIndexOfAllPeriodsBy(0, mp, bp, tp);
                    }
                    break;
                case nameof(MeasParameter.FallEdges):
                    res = calc.TakeEdgesBy(-1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.RiseEdges):
                    res = calc.TakeEdgesBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.PosPulses):
                    res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.NegPulses):
                    res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    break;
                case "PDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    }
                    break;
                case "NDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    }
                    break;
            }

            return res;
        }

        private Calculator? PowerGetOrAddAndGetCalcData(ChannelId id, MeasureGate strobe, ref (Double[] data, Double fs)? data)
        {
            if (!DsoModel.Default.TryGetChannel(id, out var cm) || !cm.Active)
            {
                return null;
            }

            var pkg = cm.Pack;
            if (pkg == null)
            {
                return null;
            }

            Int32 length;

            var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(id, strobe == MeasureGate.Screen);
            var startindex = datarange.Start.Value;
            var endindex = datarange.End.Value;
            var range = endindex - startindex;
            if (range < 0)
            {
                length = 0;
            }
            else
            {
                length = range;
            }

            Double[] buffer;
            String key = "Power" + pkg.Properties.Name + startindex.ToString() + length.ToString();

            if (id.IsDigital())
            {
                var idx = Int32.Parse(id.ToString()[1..]) - 1;
                buffer = pkg.Buffer.ToRowEnumerable().ElementAt(idx / 16).Select(o => (((UInt16)o) & (0x8000 >> (idx % 16))) != 0 ? 1D : 0D).ToArray();
            }
            else
            {
                buffer = new Double[pkg.Buffer.GetLength(1)];
                Buffer.BlockCopy(pkg.Buffer, 0, buffer, 0, buffer.Length * sizeof(Double));
            }

            data = (buffer.Skip(startindex).Take(length).ToArray(), 1 / pkg.Properties.SampInterval);

            if (buffer.Length > 0 && length > 0)
            {
                if (_CalcBond.TryGetValue(key, out var val))
                {
                }
                _CalcBond[key] = new Calculator(buffer, startindex, length)
                {
                    MinPk2Pk = pkg.Properties.ChnlScale.Value / 2,// * 10E-3 / 3
                };

                return _CalcBond[key];
            }

            return null;
        }


        private (List<(Double Start, Double End)>? Periods, List<(Double Start, Double End)>? PosPulses, List<(Double Start, Double End)>? NegPulses, List<(Double Start, Double End)>? Rises, List<(Double Start, Double End)>? Falls) CalcForPowerModulation(ChannelId soure, ref (Double[] data, Double fs)? data)
        {
            var calc = PowerGetOrAddAndGetCalcData(soure, MeasureGate.Screen, ref data);
            if (calc == null)
            {
                return (null,null,null,null,null);
            }

            List<(Double Start, Double End)>? periods = calc.TakeIndexOfAllPeriodsBy(0);
            List<(Double Start, Double End)>? pospulses = calc.TakeIndexesAllWidthsBy(1);
            List<(Double Start, Double End)>? negpulses = calc.TakeIndexesAllWidthsBy(-1);
            List<(Double Start, Double End)>? rises = calc.TakeEdgesBy(1);
            List<(Double Start, Double End)>? falls = calc.TakeEdgesBy(-1);
           
            return (periods,pospulses,negpulses,rises,falls);
        }

        private List<(Double Start, Double End)>? CalcForPowerByCustom(String measureName, ChannelId soure, Int32 startIndex, Int32 endIndex, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90)
        {
            var calc = GetOrAddInRange(soure, startIndex, endIndex);
            if (calc == null)
            {
                return null;
            }

            List<(Double Start, Double End)>? res = null;
            switch (measureName)
            {
                case nameof(MeasParameter.Freq):
                case nameof(MeasParameter.Period):
                    {
                        res = calc.TakeIndexOfAllPeriodsBy(0, mp, bp, tp);
                    }
                    break;
                case nameof(MeasParameter.FallEdges):
                    res = calc.TakeEdgesBy(-1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.RiseEdges):
                    res = calc.TakeEdgesBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.PosPulses):
                    res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    break;
                case nameof(MeasParameter.NegPulses):
                    res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    break;
                case "PDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(1, mp, bp, tp);
                    }
                    break;
                case "NDuty":
                    {
                        res = calc.TakeIndexesAllWidthsBy(-1, mp, bp, tp);
                    }
                    break;
            }

            return res;
        }


        public (List<(Double, Double)>, Double? @top, Double? @base) CalcPwrSwtichLossParas(ChannelId source)
        {
            var calc = GetOrAdd(source, MeasureGate.Screen);
            if (calc != null)
            {
                return (calc.TakeIndexOfAllPeriods(-1), calc.Take(MeasParameter.Top)?[0], calc.Take(MeasParameter.Base)?[0]);
            }
            return (null, null, null);
        }

        private Double? Calc(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen) => Calc(new MeasureItemModel(name, source) { Source2nd = destination }, strobe);

        public Double[]? CalcOnFrequencyDomain(String name, ChannelId source, ChannelType channelType)
        {
            Double? result = null;
            if (!FrequencyMeasurement.IsCorrectChannelState(source, out var mathModel, out var fftArg))
            {
                return null;
            }
            switch (name)
            {
                case "ChannelPower":
                    result = fftArg.CP.Run();
                    return new Double[1] { result == null ? Double.NaN : (Double)result };

                case "OccupiedBandwidth":
                    result = fftArg.OB.Run();
                    return new Double[1] { result == null ? Double.NaN : (Double)result };

                case "AdjacentChannelPowerRatio":
                    return fftArg.ACPR.Run();

                case "TotalHarmonicDistortion":
                    result = fftArg.THD.Run();
                    return new Double[1] { result == null ? Double.NaN : (Double)result };
                default:
                    return null;
            }
        }

        public void CalcFrequencyMeas(Boolean wfmTaken)
        {
            if (!wfmTaken)
                return;
            foreach (var id in ChannelIdExt.GetMaths())
            {
                if (!FrequencyMeasurement.IsCorrectChannelState(id, out var mathmodel, out var fftarg))
                {
                    return;
                }
                try
                {
                    fftarg.CP.Run();
                    fftarg.OB.Run();
                    fftarg.ACPR.Run();
                    fftarg.THD.Run();
                }
                catch (Exception)
                {
                }

            }
        }

        public Double? GetResultOrCalc(MeasureItemModel mi, MeasureGate strobe = MeasureGate.Screen)
        {
            if (_Results.TryGetValue(mi.Key, out var value))
            {
                return value;
            }

            return Calc(mi, strobe);
        }

        public Double? GetResultOrCalc(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen)
        {
            if (_Results.TryGetValue(GetKey(name, source, destination), out var value))
            {
                return value;
            }

            return Calc(name, source, destination, strobe);
        }
        public Double? ForceGetResultOrCalc(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen) => Calc(name, source, destination, strobe);
        public Double? ForceGetResultOrCalc(MeasureItemModel mi, MeasureGate strobe = MeasureGate.Screen) => Calc(mi, strobe);

        public List<(Double Start, Double End)>? ForceGetResultOrCalcForPowerByScreen(String name, ChannelId source, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90) => CalcForPowerByScreen(name, source, mp, bp, tp);
        public List<(Double Start, Double End)>? ForceGetResultOrCalcForPowerByScreen(String name, ChannelId source, ref (Double[] data, Double fs)? data, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90) => CalcForPowerAndGetCalcDataByScreen(name, source, ref data, mp, bp, tp);


        public (List<(Double Start, Double End)>? Periods, List<(Double Start, Double End)>? PosPulses, List<(Double Start, Double End)>? NegPulses, List<(Double Start, Double End)>? Rises, List<(Double Start, Double End)>? Falls) ForceGetResultOrCalcForModulation(ChannelId source, ref (Double[] data, Double fs)? data) => CalcForPowerModulation(source,ref data);

        public List<(Double Start, Double End)>? ForceGetResultOrCalcForPowerByCustom(String name, ChannelId source, Int32 startIndex, Int32 endIndex, Int32 mp = 50, Int32 bp = 10, Int32 tp = 90) => CalcForPowerByCustom(name, source, startIndex, endIndex, mp, bp, tp);

        public (Double Result, Prefix Prefix, String UnitTxt)? ForceGetResultOrCalcWithUnit(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen)
        {
            var val = Calc(name, source, destination, strobe);
            var dfx = GetPfxUnitString(name, source);
            if (val == null)
            {
                return null;
            }

            return (val.Value, dfx.Prefix, dfx.UnitString);
        }

        public (List<Double>? x, List<Double>? y) GetTrack(MeasureItemModel mi, MeasureGate strobe = MeasureGate.Screen)
        {
            if (mi.MeasureType == MeasureType.Composite)
            {
                var value = DsoModel.Default.Meas.Calc.StatBuffer[mi.Id - ChannelId.P1].Current;
                return (new List<Double>(), new List<Double>() { Double.IsFinite(value) ? value * 1E3 : value });
            }
            var calc = GetOrAdd(mi.Source, strobe);
            if (calc == null)
            {
                return (null, null);
            }

            var p = calc.Take(MeasParameter.PeriodSequence);
            if (p == null || p.Count == 0)
            {
                p = new List<Double>() { 0, 0 };
            }

            List<Double>? y;
            switch (mi.Name)
            {
                case "Period":
                    y = calc.Take(MeasParameter.PeriodSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Freq":
                    y = calc.Take(MeasParameter.PeriodSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => 1 / MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "PWidth":
                    y = calc.Take(MeasParameter.PWidthSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "NWidth":
                    y = calc.Take(MeasParameter.NWidthSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Rise":
                    y = calc.Take(MeasParameter.RiseSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Fall":
                    y = calc.Take(MeasParameter.FallSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Duty":
                    y = calc.Take(MeasParameter.PWidthSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Zip(p.Difference().Skip(1), (pw, pd) => (pw / pd) * 100)?.ToList());
                    }
                    break;
                case "NDuty":
                    y = calc.Take(MeasParameter.NWidthSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Zip(p.Difference().Skip(1), (pw, pd) => (pw / pd) * 100)?.ToList());
                    }
                    break;

                case "CycMax":
                    y = calc.Take(MeasParameter.MaxSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycMin":
                    y = calc.Take(MeasParameter.MinSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycRMS":
                    y = calc.Take(MeasParameter.RMSSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycAverage":
                    y = calc.Take(MeasParameter.AveSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycPeak":
                    y = calc.Take(MeasParameter.PeakSequence);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycArea":
                    y = calc.Take(MeasParameter.SumSequence);
                    if (y != null && y.Count > 0)
                    {

                        return (p?.ToList(), y.Select((o) => MultiplySampInterval(mi.Source, o)).ToList());
                    }
                    break;

                case "Rise@lv":
                    y = CalcEdgeAtLv(mi, calc, 1, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Fall@lv":
                    y = CalcEdgeAtLv(mi, calc, -1, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Period@lv":
                    y = CalcPeriodAtLv(mi, calc, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Freq@lv":
                    y = CalcPeriodAtLv(mi, calc, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => 1 / MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Width@lv":
                    y = CalcWidthAtLv(mi, calc, 1, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Duty@lv":
                    y = CalcWidthAtLv(mi, calc, 1, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Zip(p.Difference().Skip(1), (pw, pd) => (pw / pd) * 100)?.ToList());
                    }
                    break;

                case "Skew":
                case "SetupTime":
                case "RRDelay@lv":
                    y = CalcDelayAtLv(mi);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;

                case "FFDelay@lv":
                    y = CalcDelayAtLv(mi, false, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "HoldTime":
                case "RFDelay@lv":
                    y = CalcDelayAtLv(mi, true, false);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "FRDelay@lv":
                    y = CalcDelayAtLv(mi, false, true);
                    if (y != null && y.Count > 0)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Phase@lv":
                default:
                    var temp = Calc(mi, strobe);
                    if (temp.HasValue)
                    {
                        return (p?.Difference()?.ToList(), new List<Double>() { temp.Value });
                    }

                    break;
            }
            return (null, null);
        }
        public (List<Double>? x, List<Double>? y) GetSearch(MeasureItemModel mi, Double threshold, MeasureGate strobe = MeasureGate.Screen)
        {
            var calc = GetOrAdd(mi.Source, strobe);
            if (calc == null)
            {
                return (null, null);
            }

            var p = calc.Take(MeasParameter.PeriodSequence);
            //var p = TakeIndexesOfPeriod(calc);

            List<Double>? y;
            switch (mi.Name)
            {
                case "Period":
                    y = calc.Take(MeasParameter.PeriodSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Freq":
                    y = calc.Take(MeasParameter.PeriodSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => 1 / MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "PWidth":
                    //y = calc.Take(MeasParameter.PWidthSequence);
                    y = calc.SearchTakeIndexesOfWidth(threshold, 1);
                    if (y != null)
                    {
                        return (y?.Where((o, i) => i % 2 == 1).ToList(),
                            y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "NWidth":
                    //y = calc.Take(MeasParameter.NWidthSequence);
                    y = calc.SearchTakeIndexesOfWidth(threshold, -1);
                    if (y != null)
                    {
                        return (y?.Where((o, i) => i % 2 == 1).ToList(),
                            y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Rise":
                    y = calc.Take(MeasParameter.RiseSequence);
                    if (y != null)
                    {
                        return (y?.Where((o, i) => i % 2 == 1).ToList(),
                            y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Fall":
                    y = calc.Take(MeasParameter.FallSequence);
                    if (y != null)
                    {
                        return (y?.Where((o, i) => i % 2 == 1).ToList(),
                            y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;

                case "PEdge":
                    //y = calc.Take(MeasParameter.RiseSequence);
                    //if(y!=null)
                    //{
                    //    var x = calc.Take(MeasParameter.RiseSequence);
                    //    var r1 = y.Where((o, i) => i % 2 == 0).Zip(
                    //                y.Where((o, i) => i % 2 == 1),
                    //                (first, second) => GetIndex(mi.Source, first, second, threshold, true)).ToList();
                    //    var r2 = y.Where((o, i) => i % 2 == 0).Zip(
                    //                y.Where((o, i) => i % 2 == 1),
                    //                (first, second) => GetAmp(mi.Source, first, second, threshold, true)).ToList();

                    //    return (y.Where((o, i) => i % 2 == 0).Zip(
                    //                y.Where((o, i) => i % 2 == 1),
                    //                (first, second) => GetIndex(mi.Source, first, second, threshold, true)).ToList(),
                    //                y.Where((o, i) => i % 2 == 0).Zip(
                    //                y.Where((o, i) => i % 2 == 1),
                    //                (first, second) => GetAmp(mi.Source, first, second, threshold, true)).ToList());
                    //}
                    {
                        var riseindexs = calc.SearchTakeIndexesOfEdge(threshold, 1);
                        if (riseindexs != null)
                        {
                            var tmp = riseindexs.Select(x => GetFristPoint(mi.Source, x.Item1, x.Item2, threshold, 1)).ToList();
                            return (tmp.Select(t => t.Item1).ToList(), tmp.Select(t => t.Item2).ToList());
                        }
                    }
                    break;
                case "NEdge":
                    //y = calc.Take(MeasParameter.FallSequence);
                    //if (y != null)
                    //{
                    //    return (y.Where((o, i) => i % 2 == 0).Zip(
                    //                 y.Where((o, i) => i % 2 == 1),
                    //                 (first, second) => GetIndex(mi.Source, first, second, threshold, false)).ToList(),
                    //                 y.Where((o, i) => i % 2 == 0).Zip(
                    //                 y.Where((o, i) => i % 2 == 1),
                    //                 (first, second) => GetAmp(mi.Source, first, second, threshold, false)).ToList());
                    //}
                    {
                        List<(Double index, Double value)> rst = new List<(Double index, Double value)>();
                        var fallindexs = calc.SearchTakeIndexesOfEdge(threshold, -1);
                        if (fallindexs != null)
                        {
                            var tmp = fallindexs.Select(x => GetFristPoint(mi.Source, x.Item1, x.Item2, threshold, -1)).ToList();
                            return (tmp.Select(t => t.Item1).ToList(), tmp.Select(t => t.Item2).ToList());
                        }
                    }
                    break;
                case "Edge":
                    //y = calc.Take(MeasParameter.EdgeSequence);
                    //if (y != null)
                    //{
                    //    return (y?.Where((o, i) => i % 2 == 1).ToList(),
                    //        y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    //}
                    {
                        List<(Double index, Double value)> rst = new List<(Double index, Double value)>();
                        var riseindexs = calc.SearchTakeIndexesOfEdge(threshold, 1);
                        if (riseindexs != null)
                        {
                            var tmp = riseindexs.Select(x => GetFristPoint(mi.Source, x.Item1, x.Item2, threshold, 1)).ToList();
                            rst.AddRange(tmp.Select(x => (x.Item1, x.Item2)).ToList());
                        }
                        var fallindexs = calc.SearchTakeIndexesOfEdge(threshold, -1);
                        if (fallindexs != null)
                        {
                            var tmp = fallindexs.Select(x => GetFristPoint(mi.Source, x.Item1, x.Item2, threshold, -1)).ToList();
                            rst.AddRange(tmp.Select(x => (x.Item1, x.Item2)).ToList());
                        }
                        rst.OrderBy(x => x.index);
                        return (rst.Select(x => x.index).ToList(), rst.Select(x => x.value).ToList());
                    }
                case "Duty":
                    y = calc.Take(MeasParameter.PWidthSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Zip(p.Difference().Skip(1), (pw, pd) => pw / pd)?.ToList());
                    }
                    break;

                case "CycMax":
                    y = calc.Take(MeasParameter.MaxSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycMin":
                    y = calc.Take(MeasParameter.MinSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycRMS":
                    y = calc.Take(MeasParameter.RMSSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycAverage":
                    y = calc.Take(MeasParameter.AveSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case "CycPeak":
                    y = calc.Take(MeasParameter.PeakSequence);
                    if (y != null)
                    {
                        return (p?.ToList(), y);
                    }
                    break;
                case nameof(MeasParameter.CycArea):
                    y = calc.Take(MeasParameter.SumSequence);
                    if (y != null)
                    {

                        return (p?.ToList(), y.Select((o) => MultiplySampInterval(mi.Source, o)).ToList());
                    }
                    break;

                case "Rise@lv":
                    y = CalcEdgeAtLv(mi, calc, 1, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Fall@lv":
                    y = CalcEdgeAtLv(mi, calc, -1, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Period@lv":
                    y = CalcPeriodAtLv(mi, calc, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Freq@lv":
                    y = CalcPeriodAtLv(mi, calc, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Skip(1).Select((o) => 1 / MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Width@lv":
                    y = CalcWidthAtLv(mi, calc, 1, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;
                case "Duty@lv":
                    y = CalcWidthAtLv(mi, calc, 1, false);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Zip(p.Difference().Skip(1), (pw, pd) => pw / pd)?.ToList());
                    }
                    break;

                case "RRDelay@lv":
                    y = CalcDelayAtLv(mi);
                    if (y != null)
                    {
                        return (p?.ToList(), y.Difference().Where((o, i) => i % 2 == 1).Select((o) => MultiplySampInterval(mi.Source, o))?.ToList());
                    }
                    break;

                case "FFDelay@lv":
                case "RFDelay@lv":
                case "FRDelay@lv":
                case "Phase@lv":
                default:
                    var temp = Calc(mi, strobe);
                    if (temp.HasValue)
                    {
                        return (p.Difference()?.ToList(), new List<Double>() { temp.Value });
                    }

                    break;
            }
            return (null, null);
        }

        public (List<Double>? x, List<Double>? y) GetTrack(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen) => GetTrack(new MeasureItemModel(name, source) { Source2nd = destination }, strobe);


        public void Run(Boolean newWfm)
        {
            var frmechanged = CheckAndUpdateFrameNo();
            if ((newWfm && IsMeasure()) || frmechanged)
            {
                //单次触发可能拿不到数据需要延迟100ms计算
                if (!(frmechanged && IsMeasure()))
                {
                    Thread.Sleep(100);
                }

                if (Options.Active)
                {
                    //独立项计算
                    var singleitems = Options.SelectedItems.Where(x => x.MeasureType == MeasureType.Single && x.Active).ToList();
                    ItemsCalc(singleitems);
                    //复合项计算
                    var compositeitems = Options.SelectedItems.Where(x => x.MeasureType == MeasureType.Composite && x.Active).ToList();
                    ItemsCalc(compositeitems);
                }

                void ItemsCalc(List<MeasureItemModel> items)
                {
                    for (Int32 i = 0; i < items.Count; i++)
                    {
                        var mi = items[i];
                        Double? rst = null;
                        if (mi.MeasureType == MeasureType.Single)
                        {
                            rst = Calc(mi, Options.Strobe);

                            //!!!Measure appendix calculate
                            if (rst is null)
                            {
                                if (Options.ExtCalcName.TryGetValue(mi.Name, out var calc))
                                {
                                    if (mi.Source.IsMeasure() && mi.Source2nd.IsMeasure())
                                    {
                                        var s1 = GetResult(Options.SelectedItems[mi.Source - ChannelId.P1]);
                                        var s2 = GetResult(Options.SelectedItems[mi.Source2nd - ChannelId.P1]);

                                        rst = calc(s1, s2);
                                    }
                                }
                            }
                        }
                        else if (mi.MeasureType == MeasureType.Composite)
                        {
                            if (Options.SelectedItems[mi.Source - ChannelId.P1].MeasureType == MeasureType.Single && Options.SelectedItems[mi.Source - ChannelId.P1].Active && Options.SelectedItems[mi.Source2nd - ChannelId.P1].MeasureType == MeasureType.Single && Options.SelectedItems[mi.Source2nd - ChannelId.P1].Active)
                            {
                                var key = Options.SelectedItems[mi.Source - ChannelId.P1].Key;
                                var value = _Results.ContainsKey(key) ? _Results[key] : Double.NaN;
                                var key2nd = Options.SelectedItems[mi.Source2nd - ChannelId.P1].Key;
                                var value2nd = _Results.ContainsKey(key2nd) ? _Results[key2nd] : Double.NaN;
                                var (pfx, _) = GetPfxUnitString(mi.Source - ChannelId.P1);
                                var (pfx2nd, _) = GetPfxUnitString(mi.Source2nd - ChannelId.P1);
                                if (Double.IsFinite(value))
                                {
                                    value = Quantity.ConvertByPrefix(value, pfx, Prefix.Empty);
                                }
                                if (Double.IsFinite(value2nd))
                                {
                                    value2nd = Quantity.ConvertByPrefix(value2nd, pfx2nd, Prefix.Empty);
                                }
                                if (Double.IsFinite(value) && Double.IsFinite(value2nd))
                                {
                                    switch (mi.Operation)
                                    {
                                        case MeasureOperator.Add:
                                        default:
                                            rst = value + value2nd;
                                            break;
                                        case MeasureOperator.Subtract:
                                            rst = value - value2nd;
                                            break;
                                        case MeasureOperator.Multiply:
                                            rst = value * value2nd;
                                            break;
                                        case MeasureOperator.Division:
                                            if (value2nd != 0)
                                            {
                                                rst = value / value2nd;
                                            }
                                            else
                                            {
                                                rst = Double.NaN;
                                            }
                                            break;
                                    }
                                }

                                if (rst != null && Double.IsFinite(rst.Value))
                                {
                                    rst *= 1E3;
                                }
                            }
                        }


                        if (rst is not null && rst.HasValue && Double.IsFinite(rst.Value))
                        {
                            AddResult(mi, rst.Value);
                            StatBuffer[mi.Id - ChannelId.P1].Insert(rst.Value);
                        }
                        else
                        {
                            UpdateResult(mi, Double.NaN);
                        }
                    }
                }

            }
        }

        internal Boolean CalcSnapshotAllResult()
        {
            DsoModel.Default.Meas.SnapShotDataTable.Clear();
            if (DsoModel.Default.TryGetChannel(DsoModel.Default.Meas.SnapshotSource, out var chnl) && chnl.Active)
            {
                foreach (var item in Options.SnapShotTableNames)
                {
                    var datawithunit = ForceGetResultOrCalcWithUnit(item, Options.SnapshotSource, strobe: Options.Strobe);
                    if (datawithunit == null)
                    {
                        datawithunit = (Double.MaxValue, Prefix.Empty, "");
                    }
                    var data = Quantity.ConvertByPrefix(datawithunit.Value.Result, datawithunit.Value.Prefix, Prefix.Empty);

                    DsoModel.Default.Meas.SnapShotDataTable.Add(data.ToString("E5"));
                }
                return true;
            }
            return false;
        }

        private UInt16 _CalcFrameNo = 0;//记录当前计算的波形对应的FrameNo
        private Boolean IsMeasure()
        {
            if (DsoModel.Default.Meas.StopMeasure)
            {
                return true;
            }
            else if (TriggerModel.State == SysState.Stop)
            {
                return false;
            }

            return true;
        }

        private Boolean CheckAndUpdateFrameNo()
        {
            var anch = DsoModel.Default.Channels.Where(x => x.Id.IsAnalog() && x.Active);
            if (anch.Count() == 0)
                return false;

            if (anch.FirstOrDefault() is AnalogModel am && am.Pack != null && am.Pack?.Properties.FrameNo != _CalcFrameNo)
            {
                _CalcFrameNo = am.Pack.Properties.FrameNo;
                return true;
            }
            return false;

        }

        #endregion

        #region Unit        
        internal static String GetPfxUnitString(Double valuein, Prefix prefix, String unit)
        {
            return SolveString(valuein, prefix, unit);
        }

        internal static String GetPfxUnitString(String name, ChannelId src, Double? valuein)
        {
            if (valuein == null)
                return MeasureHelper.MeasureEmpty;
            var pu = GetPfxUnitString(name, src);
            return SolveString((Double)valuein, pu.Prefix, pu.UnitString);
        }

        internal static String SolveString(Double valuein, Prefix prefix, String unit)
        {
            switch (QuantityUnitExt.Parse(unit))
            {
                case QuantityUnit.Hour:
                    break;
                case QuantityUnit.Angle:
                    break;
                case QuantityUnit.Radian:
                    break;
                case QuantityUnit.Percent:
                    return SolveStringPercent(valuein, prefix, unit);
                default:
                    break;
            }
            return $"{(new Quantity(valuein, prefix, unit)).ValueChangeToSI(3)}";
        }

        internal static String SolveStringPercent(Double valuein, Prefix prefix, String unit)
        {
            if (Double.IsNaN(valuein))
            {
                return MeasureHelper.MeasureEmpty;
            }

            valuein = Quantity.ConvertByPrefix(valuein, prefix, Prefix.Empty);
            Double value = valuein * 100;
            //if (Math.Abs(valuein) < 0.1)
            //{
            //    value = value * 10;
            //    unit = "‰";
            //}
            return value.ToString("0.00") + unit;
        }

        internal static (Prefix Prefix, String UnitString) GetPfxUnitString(String name, ChannelId src)
        {
            return name switch
            {
                "Pk2Pk" or "Amplitude" or "Stddev" or "RMS" or "Min" or "Max" or "Top" or "Base" or "Mid" or "Average" or "Upper" or "Lower" or "CycMax" or "CycMin" or "CycRMS" or "CycAverage" or "CycPeak" or "CycMid" or "Crossing" or "HistMean" or "HistMax" or "HistMin" or "HistRange" or "HistMid" or "HistMode" or "HistTop" or "HistBase" or "HistSigma" => GetChnlUnit(src),
                "POverShoot" or "NOverShoot" or "PPreShoot" or "NPreShoot" or "Duty" or "NDuty" or "Duty@lv" or "HistMu1Sigma" or "HistMu2Sigma" or "HistMu3Sigma" => (Prefix.Empty, QuantityUnit.Percent.ToUnitString()),
                "Rise" or "Fall" or "PWidth" or "NWidth" or "Period" or "BurstLen" or "BurstWidth" or "BurstInterval" or "BurstPeriod" or "SetupTime" or "HoldTime" or "RRDelay@lv" or "RFDelay@lv" or "FRDelay@lv" or "FFDelay@lv" or "Rise@lv" or "Fall@lv" or "Period@lv" or "Width@lv" or "T@max" or "T@min" or nameof(MeasParameter.OutsideTime) or "HighTime" or "LowTime" or "NPeriods" or "Skew" or "UI" => (Prefix.Empty, QuantityUnit.Second.ToUnitString()),//return GetSampUnit(src);
                "Freq" or "Freq@lv" or "DataRate" => (Prefix.Empty, QuantityUnit.Hertz.ToUnitString()),
                "HistMaxPop" or "HistMeanPop" or "HistTotalPop" or "WfmLength" or "HistWfmCnt" or nameof(MeasParameter.RiseEdges) or nameof(MeasParameter.FallEdges) or nameof(MeasParameter.PosPulses) or nameof(MeasParameter.NegPulses) or nameof(MeasParameter.Cycles) or "BurstCycle" or "PulseCount" or "Ratio" => (Prefix.Empty, QuantityUnit.Constant.ToUnitString()),
                nameof(MeasParameter.Area) or nameof(MeasParameter.CycArea) or nameof(MeasParameter.AbsArea) or nameof(MeasParameter.AbsCycArea) or nameof(MeasParameter.HistArea) => GetAreaUnit(src),
                "Phase@lv" => (Prefix.Empty, QuantityUnit.Angle.ToUnitString()),
                "FSlewRate" or "RSlewRate" => GetSlewUnit(src),
                _ => (Prefix.Empty, "?"),
            };

            static (Prefix Prefix, String Name) GetChnlUnit(ChannelId src)
            {
                var pkg = DsoModel.Default.GetWfmPack(src);
                if (pkg is not null)
                {
                    return pkg.Properties.ChnlUnit;
                }
                return (Prefix.Empty, "?");
            }

            static (Prefix Prefix, String Name) GetAreaUnit(ChannelId src)
            {
                var pkg = DsoModel.Default.GetWfmPack(src);
                if (pkg is not null)
                {
                    return (pkg.Properties.ChnlUnit.Prefix, pkg.Properties.ChnlUnit.Name + pkg.Properties.TmbUnit.Name);
                }
                return (Prefix.Empty, "?");
            }

            static (Prefix Prefix, String Name) GetSlewUnit(ChannelId src)
            {
                var pkg = DsoModel.Default.GetWfmPack(src);
                if (pkg is not null)
                {
                    return (Prefix.Empty, pkg.Properties.ChnlUnit.Name + "/" + pkg.Properties.TmbUnit.Name);
                }
                return (Prefix.Empty, "?");
            }

#pragma warning disable CS8321 // 已声明本地函数，但从未使用过
            static (Prefix Prefix, String Name) GetSampUnit(ChannelId src)
            {
                var pkg = DsoModel.Default.GetWfmPack(src);
                if (pkg is not null)
                {
                    return pkg.Properties.TmbUnit;
                }
                return (Prefix.Empty, "?");
            }
#pragma warning restore CS8321 // 已声明本地函数，但从未使用过
        }

        public Boolean GetIndicatorStates(String name)
        {
            return name switch
            {
                nameof(MeasParameter.Cycles) or nameof(MeasParameter.RiseEdges) or
                nameof(MeasParameter.FallEdges) or nameof(MeasParameter.PosPulses) or
                nameof(MeasParameter.NegPulses) => false,
                _ => true
            };

        }
        public (Prefix Prefix, String Name) GetPfxUnitString(Int32 pindex) => Options.SelectedItems[pindex].MeasureType == MeasureType.Single ? GetPfxUnitString(Options.SelectedItems[pindex].Name, Options.SelectedItems[pindex].Source) : (Prefix.Milli, String.Empty);
        #endregion

        #region Indicator
        private static List<Double>? ToHorzIdx(ChannelId source, List<Double>? values)
        {
            var chnl = DsoModel.Default.GetChannel(source);
            var bias = 0.0;
            if (chnl is AnalogModel am)
            {
                bias = am.Conditioning.BiasByuV / 1_000D;
            }
            return (values?.Count < 10 ? values : values?.TakeLast(10).ToList())?.ConvertAll((o) => (o - bias) / chnl.Conditioning.ScaleBymV * chnl.Conditioning.PosIdxPerDiv + chnl.Conditioning.PosIndex);
        }

        private List<Double>? ToVertIdx(ChannelId source, List<Double>? values, Int32 startindex)
        {
            var chnl = DsoModel.Default.GetChannel(source);
            var pkg = chnl.Pack;
            if (pkg is null)
            {
                return null;
            }

            var ratio = (chnl.Sampling.Scale * 1E-6 / chnl.Sampling.PosIdxPerDiv) / pkg.Properties.SampInterval;
            //var start = chnl.Sampling.PosIndex - pkg.Properties.TmbPosition.Index / ratio;
            var start = chnl.Sampling.PosIndex - (pkg.Properties.TmbPosition.Index - pkg.Properties.VuStartIndex) * pkg.Properties.TmbScale.Value / chnl.Sampling.Scale;
            return (values?.Count < 10 ? values : values?.TakeLast(10).ToList())?.ConvertAll((o) =>
                (o + startindex - pkg.Offset) / ratio + start);
        }

        internal (List<Double>?, List<Double>?) GetIndicator(MeasureItemModel mim)
        {
            var calc = Get(mim.Source, true);
            if (calc == null)
            {
                return (null, null);
            }

            (List<Double>?, List<Double>?) result = mim.Name switch
            {
                "Max" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Max))),
                "Min" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Min))),
                "Average" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Average))),
                "Pk2Pk" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.Pk2Pk))),
                "RMS" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? GetHorzIdx(mim) ?? calc.Take(MeasParameter.RMS))),
                "Stddev" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Stddev))),
                "Top" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Top))),
                "Base" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Base))),
                "Amplitude" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.Amplitude))),
                "Mid" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Mid))),
                "Upper" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Upper))),
                "Lower" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.Lower))),
                "CycMax" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.CycMax))),
                "CycMin" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.CycMin))),
                "CycRMS" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.CycRMS))),
                "CycAverage" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.CycAverage))),
                //"CycPeak" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.CycPeak))),
                "CycPeak" => (null, ToHorzIdx(mim.Source, TakeFirstCycPeak(calc))),
                "CycMid" => (null, ToHorzIdx(mim.Source, GetHorzIdx(mim) ?? calc.Take(MeasParameter.CycMid))),
                "Crossing" => (null, ToHorzIdx(mim.Source, GetCrossingIndicator(mim))),


                "POverShoot" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.POverShoot))),
                "NOverShoot" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.NOverShoot))),
                "PPreShoot" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.PPreShoot))),
                "NPreShoot" => (null, ToHorzIdx(mim.Source, calc.Take(MeasParameter.NPreShoot))),



                "Period" or "Freq" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.Period), calc.StartIndex), null),
                //"Period" or "Freq" => (ToVertIdx(mim.Source, TakePeriod(calc)), null),
                "PWidth" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.PWidth), calc.StartIndex), null),
                //"PWidth" => (ToVertIdx(mim.Source, TakeIndexOfFirstMid(calc, 1)), null),
                //"NWidth" => (ToVertIdx(mim.Source, TakeIndexOfFirstWidth(calc,-1)), null),
                "NWidth" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.NWidth), calc.StartIndex), null),
                "Rise" or "RSlewRate" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.Rise), calc.StartIndex), null),
                //"Rise" or "RSlewRate" => (ToVertIdx(mim.Source, TakeIndexOfFirstEdge(calc,1)), null),
                "Fall" or "FSlewRate" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.Fall), calc.StartIndex), null),
                //"Fall" or "FSlewRate" => (ToVertIdx(mim.Source, TakeIndexOfFirstEdge(calc, -1)), null),
                "Duty" => (ToVertIdx(mim.Source, CalcDuty(calc, PulsePolarity.Positive)?.Values, calc.StartIndex), null),
                //"Duty" => (ToVertIdx(mim.Source, TakeIndexOfFirstWidth(calc, 1)), null),
                "NDuty" => (ToVertIdx(mim.Source, CalcDuty(calc, PulsePolarity.Negative)?.Values, calc.StartIndex), null),
                "BurstLen" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.BurstLen), calc.StartIndex), null),
                "NPeriods" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.NPeriods), calc.StartIndex), null),
                "T@max" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.TAtMax), calc.StartIndex), null),
                "T@min" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.TAtMin), calc.StartIndex), null),
                "Rise@lv" => (ToVertIdx(mim.Source, CalcEdgeAtLv(mim, calc, 1), calc.StartIndex), null),
                "Fall@lv" => (ToVertIdx(mim.Source, CalcEdgeAtLv(mim, calc, -1), calc.StartIndex), null),
                "Period@lv" or "Freq@lv" => (ToVertIdx(mim.Source, CalcPeriodAtLv(mim, calc), calc.StartIndex), null),
                "Width@lv" => (ToVertIdx(mim.Source, CalcWidthAtLv(mim, calc, 1), calc.StartIndex), null),
                "Duty@lv" => (ToVertIdx(mim.Source, CalcWidthAtLv(mim, calc, 1), calc.StartIndex), null),
                "Phase@lv" or "RRDelay@lv" or "SetupTime" or "Skew" => (ToVertIdx(mim.Source, CalcFirstDelayAtLv(mim, Options.Strobe), calc.StartIndex), null),
                "FFDelay@lv" => (ToVertIdx(mim.Source, CalcFirstDelayAtLv(mim, Options.Strobe, false, false), calc.StartIndex), null),
                "HoldTime" or "RFDelay@lv" => (ToVertIdx(mim.Source, CalcFirstDelayAtLv(mim, Options.Strobe, true, false), calc.StartIndex), null),
                "FRDelay@lv" => (ToVertIdx(mim.Source, CalcFirstDelayAtLv(mim, Options.Strobe, false, true), calc.StartIndex), null),
                //"Skew" => (ToVertIdx(mim.Source, calc.Take(MeasParameter.FirstMid)), null),
                _ => (null, null),
            };

            return result;
        }

        private List<Double>? GetHorzIdx(MeasureItemModel mim)
        {
            var ave = StatBuffer[mim.Id - ChannelId.P1].Current;
            if (Double.IsFinite(ave))
            {
                return new() { ave };
            }

            return null;
        }

        private List<Double>? GetCrossingIndicator(MeasureItemModel mim)
        {
            var ave = StatBuffer[mim.Id - ChannelId.P1].Average;
            if (ave.HasValue)
            {
                return new() { ave.Value };
            }

            return new() { CalcFirstCrossing(mim, Options.Strobe) };
        }
        #endregion

        private static List<Double>? TakeFirstCycPeak(Calculator calc)
        {
            Double? num = calc.Take(MeasParameter.CycMax).First();
            Double? num2 = calc.Take(MeasParameter.CycMin).First();
            if (num != null && num2 != null)
            {
                return new List<Double> { num.Value, num2.Value };
            }
            return null;
        }
    }
}
