using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExtGPU;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using MathNet.Numerics;

namespace ScopeX.Core
{
    internal class DataSrcMath : IDataSource
    {
        internal sealed record MathContext(IEnumerable<Double> LastBuffer, MathModel Math, String Name, MathType ExpType, String Expression, Boolean Init, CancellationToken CancelToken)
        {
            public Int32 AverageTimes = 2;
        };

        public Object? Prepare(Boolean init, ChannelId mid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            var mch = (MathModel)DsoModel.Default.GetChannel(mid);
            var lastbuffer = mch.Pack?.Buffer?.ToEnumerable() ?? Enumerable.Empty<Double>();
            if (mch.InitFlag || /*init||*/mch.ClearFlag)
            {
                lastbuffer = Enumerable.Empty<Double>();
                mch.ClearFlag = false;
            }
            if (MathArgPrsnt.TryParse(mch.Formula, out var arg))
            {
                string expression = arg.Value.Exp;
                if (System.Text.RegularExpressions.Regex.IsMatch(expression, @"M4\d+"))
                {
                    expression = System.Text.RegularExpressions.Regex.Replace(expression, @"M4\d+", "M7");
                }
                return new MathContext(lastbuffer, mch, mch.Name, arg.Value.ExpType, expression, init, ct) { AverageTimes = mch.AverageTimes };
                //return new MathContext(lastbuffer, mch, mch.Name, arg.Value.ExpType, arg.Value.Exp, init, ct) { AverageTimes = mch.AverageTimes };
            }

            return new MathContext(lastbuffer, mch, mch.Name, MathType.Binary, "C1+C2", init, ct) { AverageTimes = mch.AverageTimes };
        }

        public FFTGPU _FFTGPU;

        public (Double[,], Object)? Read(Object? arg)
        {
            MathContext ctx = (MathContext)arg!;
            if (ctx == null || ctx.Math.Args!.RunState == RunStateType.Stop)
            {
                return (new Double[1, 0], Config(ctx, null));
            }

            Vector? res;
            WfmProperties prop;

            if (ctx.Math.Occupier != null)
            {
                if (ctx.Math.Occupier is AdvancedMathModel amm)
                {
                    if (amm.IsExternMethod)
                    {
                        var vec = amm.Take();
                        if (vec != null)
                        {
                            res = (Vector)vec;
                            prop = Config(ctx, res);
                            return (res.Elements.Multiply_(1E3), prop);
                        }
                    }
                }
            }
            String? error = null;
            //if ((ctx.Math.Args.Type == MathType.FFT) && ((MathFftArg)(ctx.Math.Args)).Number >= FFTNumber.Num32K) 
            if ((ctx.Math.Args.Type == MathType.FFT) && ((MathFftArg)(ctx.Math.Args)).Number > FFTNumber.Num512K)
            {
                res = GetFFTByGPU(ctx, out error);
            }
            //else if (ctx.Math.Args.Type == MathType.Filter && ctx.Math.Args is MathFilterArg filter && filter.IsQuickDesign)
            //{
            //    res = GetFilterByQuickDesign(ctx, out error);
            //}
            //else
            //{
            //    res = DynamicExecute.Run(ctx.Name, ctx.Expression, out error);
            //}
            else if (ctx.Math.Args.Occupier != null)
            {
                if (ctx.Name == "M41")
                {
                    MathVecBuffer.Default.TryGetVector("AbnormalData(C1)", out res);
                }
                else if (ctx.Math.Id >= ChannelIdExt.MinJMChId && ctx.Math.Id <= ChannelIdExt.MaxJMChId) 
                {
                    String expstring = ctx.Expression;
                    if (ctx.Expression.Contains("Track"))
                    {
                        var length = DsoModel.Default.Timebase.AnaChnlLengthSource.First(x => !x.Key.Equals("Auto")).Value.ToString();
                        expstring = expstring.Replace("1000000", length);
                    }
                    //res = DynamicExecute.Run(ctx.Name, ctx.Expression, out error);
                    res = DynamicExecute.Run(ctx.Name, expstring, out error);
                }
                else
                {
                    Boolean flag = OccupierBuffer.Default.TryGetVector(ctx.Math.Label, out Vector? tmpdata);
                    if (flag && tmpdata != null)
                    {
                        res = tmpdata;
                        error = null;
                    }
                    else
                    {
                        res = new Vector();
                        error = "No Data";
                    }
                }
            }
            else if (ctx.Math.Args.Type == MathType.Filter && ctx.Math.Args is MathFilterArg math)
            {
                res = math.IsQuickDesign ? GetFilterByQuickDesign(ctx, out error) : GetFilterByFilterDesigner(ctx, out error);
            }
            else if (ctx.Expression != String.Empty)
            {
                String expstring = ctx.Expression;
                if (ctx.Expression.Contains("Track"))
                {
                    var length = DsoModel.Default.Timebase.AnaChnlLengthSource.First(x=>!x.Key.Equals("Auto")).Value.ToString();
                    expstring = expstring.Replace("1000000", length);
                }
                //res = DynamicExecute.Run(ctx.Name, ctx.Expression, out error);
                res = DynamicExecute.Run(ctx.Name, expstring, out error);
            }
            else
            {
                res = null;
            }

            if (String.IsNullOrEmpty(error))
            {
                //!!!Notice: When source channel's parameters are changed, the math channel can not receive notification,
                //so math channel gather actively information from source channel.
                prop = Config(ctx, res);
                //!!!ZQC 11.08
                //if (ctx.Math.Args!.RunState == RunStateType.Single)
                //{
                //    ctx.Math.Args!.RunState = RunStateType.Stop;
                //}
                //if (ctx.Math.Args is MathEResArg eresmath && eresmath.Source.IsReference())
                //{
                //    return (res?.Elements.Multiply_(1) ?? new Double[1, 0], prop);
                //}
                Double[,] data;

                if(res?.Elements != null)
                {
                    if (res?.YUnit != null && res?.YUnit == QuantityUnit.Percent.ToString())
                    {
                        data = res.Elements.Multiply_(1E5);
                    }
                    else
                    {
                        var flag = true;
                        if (ctx.Expression.Contains(nameof(MathType.Trend)) || ctx.Expression.Contains(nameof(MathType.Track)))
                        {
                            if (ctx.Expression.Contains(nameof(ChannelId.DVM)))
                            {
                                flag = false;
                            }
                        }
                        if (ctx.Math.Id >= ChannelIdExt.MinIRMChId && ctx.Math.Id <= ChannelIdExt.MaxIRMChId)
                            flag = false;
                        if (ctx.Math.Id >= ChannelIdExt.MinMDChId && ctx.Math.Id <= ChannelIdExt.MaxMDChId)
                            flag = false;
                        if (flag && ctx.Math.Label != "Constellation()")
                        {
                            data = res.Elements.Multiply_(1E3);
                        }
                        else
                        {
                            data = res.Elements;
                        }
                    }

                    if(ctx.Math.MathType == MathType.Trend|| ctx.Math.MathType == MathType.Track)
                    {
                        List<Double> temp = new List<Double>();
                        for (Int32 i = 0; i < data.GetLength(1); i++)
                        {
                            if (data[0, i].IsFinite())
                            {
                                temp.Add(data[0, i]);
                            }
                        }
                        data = new double[1, temp.Count];
                        for (Int32 i = 0,l= temp.Count; i < l; i++)
                        {
                            data[0, i] = temp[i];
                        }
                    }
                }
                else
                {
                    data = new Double[1, 0];
                }

                return (data, prop);
            }

            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"The formula of '{ctx.Expression}' can not be compiled and executed.\n{error}", EventBus.LogLevel.Debug));

            return (res?.Elements ?? new Double[1, 0], Config(ctx, null));
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            var ctx = (MathContext)arg!;
            if (ctx.ExpType == MathType.Trend)
            {
                var lastlength = ctx.LastBuffer.Count();
                var newlength = pkg.Buffer.GetLength(1);
                //var length = (lastLength + newLength) > Constants.MAX_TREND_LENGTH ? Constants.MAX_TREND_LENGTH : lastLength + newLength;
                var length = (lastlength + newlength) > 10000 ? 10000 : lastlength + newlength;

                var buffer = ctx.LastBuffer.Concat(pkg.Buffer.ToEnumerable()).TakeLast(length);
                pkg.Buffer = buffer.ToMatrix(1, buffer.Count());
            }

            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.Length, (WfmProperties)pkg.Prop);
        }

        public DataSrcMath()
        {
            _FFTGPU = new FFTGPU();
        }

        private static WfmProperties Config(MathContext ctx, Vector? vec)
        {
            var mch = ctx.Math;

            switch (ctx.ExpType)
            {
                case MathType.Binary:
                    return MathBinaryArg.Config(mch, ctx.Expression, vec);
                case MathType.FFT:
                    return MathFftArg.Config(mch, ctx.Expression, vec);
                case MathType.Zoom:
                    return MathZoomArg.Config(mch, ctx.Expression, vec);
                case MathType.Filter:
                    return MathFilterArg.Config(mch, ctx.Expression, vec);
                case MathType.ERes:
                    return MathEResArg.Config(mch, ctx.Expression, vec);
                case MathType.Histgram:
                    return MathHistArg.Config(mch, ctx.Expression, vec);
                case MathType.Track:
                    return MathTrackArg.Config(mch, ctx.Expression, vec);
                case MathType.Trend:
                    return MathTrendArg.Config(mch, ctx.Expression, vec, ctx.LastBuffer.Count());
                case MathType.Custom:
                    return MathCustomArg.Config(mch, ctx.Expression, vec);
                case MathType.UserProgram:
                    return MathUserProgramArg.Config(mch, ctx.Expression, vec);
            }

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = mch.Conditioning.InitialScale,
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (mch.Sampling.PosDefIndex, mch.Sampling.GetPosition(mch.Sampling.PosDefIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };
            mch.Sampling.PosDefIndex = ComModel.Constants.DEF_XPOS_IDX;
            prop.SampInterval = vec?.SampInterval ?? 1;
            //prop.VuFactor = prop.TmbScale.Value * 1E-6 / mch.Sampling.PosIdxPerDiv / prop.SampInterval;

            return prop;
        }

        private Vector? GetFFTByGPU(Object arg, out String? error)
        {
            error = null;
            MathContext ctx = (MathContext)arg!;
            if (ctx.Math.Args == null)
            {
                error = "Math.Args == null";
                return null;
            }
            Vector res;
            var math = (MathFftArg)ctx.Math.Args;
            MathVecBuffer.Default.TryGetVector(math.Source.ToString(), out var vec);
            if (vec != null)
            {
                Single[] source = vec.Elements.ToEnumerable().Select(o => (Single)o).ToArray();
                var freqres = 1 / (vec.SampInterval * (Int32)(math.Number));
                Boolean success = _FFTGPU.Excute(source, math.Window, (Single)freqres, math.ResultType, math.ResultUnit, (Int32)(math.Number), out Single[] result); //此处采样率没有使用，后面删除该参数
                res = new Vector(
                    result.Select(o => (Double)o).ToMatrix(1, result.Count()),
                    QuantityUnitExt.ToUnitString(QuantityUnit.Hertz) /*QuantityUnit.Hertz.ToString()*/,
                    vec.YUnit.ConvertToSpectrumUnit(math.ResultType, math.ResultUnit, math.PhaseUnit),
                    freqres);
                return res;
            }
            else
            {
                error = "vec == null";
                return null;
            }
        }

        private Vector? GetFilterByQuickDesign(Object arg, out String? error)
        {
            error = null;
            MathContext ctx = (MathContext)arg!;
            if (ctx.Math.Args == null)
            {
                error = "Math.Args == null";
                return null;
            }
            Vector res;
            var math = (MathFilterArg)ctx.Math.Args;
            MathVecBuffer.Default.TryGetVector(math.Source.ToString(), out var vec);
            if (vec != null)
            {
                WindowType type = WindowType.Hamming;//缺省使用汉明窗
                Int32 order = 100;//暂定最小阶
                Double wc1 = (Double)math.Freq1 * vec.SampInterval / 1 * 2;
                Double wc2 = (Double)math.Freq2 * vec.SampInterval / 1 * 2;
                if (wc1 >= 1)
                {
                    wc1 = 0.9999999;
                }
                else if (wc1 == 0)
                {
                    wc1 = 0.000001;
                }
                if (wc2 >= 1)
                {
                    wc2 = 0.99999999;
                }
                else if (wc2 == 0)
                {
                    wc2 = 0.00001;
                }
                var num = MathToolAPI.ConvertIntPtrToDoubleArray(MathToolAPI.CreateFirByWindow(order, (Int32)math.RespType, (Int32)type, wc1, wc2), order + 1);
                var source = vec.Elements;
                var signal = Enumerable.Range(0, source.GetLength(1))
                                    .Select(x => source[0, x])
                                    .ToArray();
                //填充0在信号前后，预防边缘效应
                //Double[] paddedsignal = new Double[signal.Length +  order];    
                //Array.Copy(signal, 0, paddedsignal, order, signal.Length);
                //Array.Copy(signal, signal.Length-order, paddedsignal, 0, order);
                var temp = MathToolAPI.ConvertIntPtrToDoubleArray(MathToolAPI.FirFilterToSignal(signal, num, signal.Length, order + 1), signal.Length);
                //// 裁剪掉填充的部分
                //Double[] result = new Double[signal.Length];
                //Array.Copy(temp, order, result, 0, signal.Length);
                res = new Vector(temp, vec.XUnit, vec.YUnit, vec.SampInterval, vec.RefSampPos);
                return res;
            }
            else
            {
                error = "vec == null";
                return null;
            }
        }

        private Vector? GetFilterByFilterDesigner(Object arg, out String? error)
        {
            error = null;
            MathContext ctx = (MathContext)arg!;
            if (ctx.Math.Args == null)
            {
                error = "Math.Args == null";
                return null;
            }
            Vector res;
            var math = (MathFilterArg)ctx.Math.Args;
            MathVecBuffer.Default.TryGetVector(math.Source.ToString(), out var vec);
            if (vec != null)
            {
                if (math.Numerator == null)
                {
                    error = "Numerator == null";
                    return vec;
                }
                if (math.FilterType == FilterType.IIRFilter && math.Denominator == null)
                {
                    error = "Denominator == null";
                    return vec;
                }
                var source = vec.Elements;
                var signal = Enumerable.Range(0, source.GetLength(1))
                                    .Select(x => source[0, x])
                                    .ToArray();
                var temp = MathToolAPI.ConvertIntPtrToDoubleArray(MathToolAPI.Filter(signal, math.Numerator, math.Denominator, signal.Length, math.Numerator.Length), signal.Length);

                res = new Vector(temp, vec.XUnit, vec.YUnit, vec.SampInterval, vec.RefSampPos);
                return res;
            }
            else
            {
                error = "vec == null";
                return null;
            }
        }

        //private static Double ApplyFilterToSignal(Double[] signal, Double[] num, Int32 index, Int32 filterLength)
        //{
        //    Double sum = 0.0;
        //    for (Int32 j = 0; j < filterLength; j++)
        //    {
        //        if (index - j >= 0 && index - j < signal.Length)
        //        {
        //            sum += signal[index - j] * num[j];
        //        }
        //    }
        //    return sum;
        //}
        #region 谱线
        public WfmPack ProcessNormal((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (MathContext)context!;
            //var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;

            return new WfmPack(buffer, 0, buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }
        public WfmPack ProcessAverage((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (MathContext)context!;
            //var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            Int32 num = _WfmNums;
            if (_AverageBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _AverageBuffer[j] = buffer[i, j];
                num = 1;
                _RestartAverage = false;
            }
            else
            {
                lock (_AverageBuffer.SyncRoot)
                {
                    Int32 maxaveragenum = ctx.AverageTimes;
                    if (num == 0)
                        num++;
                    else if (num < maxaveragenum)
                    {
                        for (Int32 i = 0; i < buffer.GetLength(0); i++)
                            for (Int32 j = 0; j < buffer.GetLength(1); j++)
                                _AverageBuffer[j] += buffer[i, j];
                        num++;
                    }
                    else
                    {
                        for (Int32 i = 0; i < buffer.GetLength(0); i++)
                            for (Int32 j = 0; j < buffer.GetLength(1); j++)
                            {
                                _AverageBuffer[j] *= num - 1;
                                _AverageBuffer[j] /= num;
                                _AverageBuffer[j] += buffer[i, j];
                            }
                    }
                }
            }
            for (Int32 i = 0; i < buffer.GetLength(0); i++)
                for (Int32 j = 0; j < buffer.GetLength(1); j++)
                {
                    buffer[i, j] = _AverageBuffer[j] / num;
                    //buffer[i, j] = (buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                }

            if (_WfmNums < ctx.AverageTimes)
                _WfmNums++;

            return new WfmPack(buffer, 0, buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }
        public WfmPack ProcessMaxHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (MathContext)context!;
            //var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            if (_MaxHoldBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _MaxHoldBuffer[j] = buffer[i, j];
                _RestartMax = false;
            }
            if (!_Restart && _RestartMax)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MaxHoldBuffer[j] < buffer[i, j])
                                _MaxHoldBuffer[j] = buffer[i, j];
                        }
            }
            if (_RestartMax)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            buffer[i, j] = _MaxHoldBuffer[j];
                        }
            }
            else
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            buffer[i, j] = _MaxHoldBuffer[j];
                        }
            }


            return new WfmPack(buffer, 0, buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }
        public WfmPack ProcessMinHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (MathContext)context!;
            //var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            if (_MinHoldBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _MinHoldBuffer[j] = buffer[i, j];
                _RestartMin = false;
            }
            if (_RestartMin)
            {
                lock (_MinHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MinHoldBuffer[j] > buffer[i, j])
                                _MinHoldBuffer[j] = buffer[i, j];

                            buffer[i, j] = _MinHoldBuffer[j];
                        }
            }
            else
            {
                lock (_MinHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MinHoldBuffer[j] > buffer[i, j])
                                _MinHoldBuffer[j] = buffer[i, j];

                            buffer[i, j] = _MinHoldBuffer[j];
                        }
            }

            return new WfmPack(buffer, 0, buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        #region WaveformProcess

        private Double[] _AverageBuffer = new Double[10000];
        private Double[] _MaxHoldBuffer = new Double[10000];
        private Double[] _MinHoldBuffer = new Double[10000];
        private Boolean _RestartAverage = true;
        private Boolean _RestartMax = true;
        private Boolean _RestartMin = true;
        private Boolean _Restart = true;
        private Int32 _WfmNums;
        private readonly List<Double[]> _HistoryBuffer = new List<Double[]>();

        public void Init()
        {
            _Restart = true;
        }

        public void Init(Double[,] originalBuffer)
        {
            Double[] buffer = new Double[originalBuffer.GetLength(1)];
            for (Int32 i = 0; i < originalBuffer.GetLength(1); i++)
            {
                buffer[i] = originalBuffer[0, i];
            }
            _WfmNums = 0;

            _HistoryBuffer.Clear();
            lock (_AverageBuffer.SyncRoot)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                {
                    lock (_MinHoldBuffer.SyncRoot)
                    {
                        _MaxHoldBuffer = (Double[])buffer.Clone();
                        _MinHoldBuffer = (Double[])buffer.Clone();
                        _AverageBuffer = (Double[])buffer.Clone();
                    }
                }
            }

            for (Int32 i = 0; i < _HistoryBuffer.Count; i++)
                Array.Clear(_HistoryBuffer[i], 0, _HistoryBuffer[i].Length);

            if (!_RestartMax && !_RestartMax && !_RestartMin)
            {
                _RestartAverage = true;
                _RestartMax = true;
                _RestartMin = true;
                _Restart = false;
            }
        }



        #endregion

        #endregion
    }
}
