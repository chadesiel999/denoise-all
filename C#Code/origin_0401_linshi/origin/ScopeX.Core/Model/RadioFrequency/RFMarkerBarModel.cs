using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Model.RadioFrequency;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class RFMarkerBarModel : INotifyPropertyChanged
    {

        private MarkerItemModel _Owner;

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged(nameof(Source));
                }
            }
        }

        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged(nameof(Active));
                }
            }
        }

        private Color _DrawColor = Color.White;
        public Color DrawColor
        {
            get => _DrawColor;
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
                    OnPropertyChanged(nameof(DrawColor));
                }
            }
        }

        //Maybe there are more than two cursors, for example frequency markers 
        private readonly Double?[] _PosIndexes;

        public Int32 Capacity => _PosIndexes.Length;

        public Int32 Count => _PosIndexes.Count(o => o.HasValue);

        public Double? this[Int32 index]
        {
            get => _PosIndexes[index];
            set
            {
                value = Validate(value);
                if (value != _PosIndexes[index])
                {
                    _PosIndexes[index] = value;
                    OnPropertyChanged("PosIndex");
                }
            }
        }

        public Double MaxPosIndex
        {
            get;
            private init;
        }

        public Double MinPosIndex
        {
            get;
            private init;
        }

        private Double? Validate(Double? pos)
        {
            if (pos > MaxPosIndex)
                pos = MaxPosIndex;
            else if (pos < MinPosIndex)
                pos = MinPosIndex;

            return pos;
        }

        private List<(Int32 PositionIndex, Double Amp/*, Double Frequency*/)>? _LastResult = null;
        public List<(Int32 PositionIndex, Double Amp/*, Double Frequency*/)>? LastResult
        {
            get { return _LastResult; }
        }

        #region RF

        private Double _RFThreshold = Constants.RF_THRESHOLD;//******************值的范围待确定
        /// <summary>
        /// 阈值
        /// </summary>
        public Double RFThreshold
        {
            get { return _RFThreshold; }
            set
            {
                value = ValidateRFThreshold(value);
                if (_RFThreshold != value)
                {
                    _RFThreshold = value;
                    OnPropertyChanged(nameof(RFThreshold));
                }
            }
        }

        /// <summary>
        /// 根据选择的单位限定可设置的阈值
        /// </summary>     
        private Double ValidateRFThreshold(Double value)
        {
            if (value > Constants.RF_THRESHOLD_MAX)
            {
                value = Constants.RF_THRESHOLD_MAX;
                WeakTip.Default.Write(nameof(RFThreshold), MsgTipId.GreatethanMax, false, "", 1);
            }
            else if (value < Constants.RF_THRESHOLD_MIN)
            {
                value = Constants.RF_THRESHOLD_MIN;
                WeakTip.Default.Write(nameof(RFThreshold), MsgTipId.LessthanMin, false, "", 1);
            }
            return value;
        }


        private Double _RFExcursion = Constants.RF_EXCURSION;//******************值的范围待确定
        /// <summary>
        /// 振幅
        /// </summary>
        public Double RFExcursion
        {
            get { return _RFExcursion; }
            set
            {
                value = ValidateRFExcursion(value);
                if (_RFExcursion != value)
                {
                    _RFExcursion = value;
                    OnPropertyChanged(nameof(RFExcursion));
                }
            }
        }

        private Double ValidateRFExcursion(Double value)
        {
            if (value < Constants.RF_EXCURSION_MIN)
            {
                value = Constants.RF_EXCURSION_MIN;
                WeakTip.Default.Write(nameof(RFExcursion), MsgTipId.LessthanMin, false, "", 1);
            }
            else if (value > Constants.RF_EXCURSION_MAX)
            {
                value = Constants.RF_EXCURSION_MAX;
                WeakTip.Default.Write(nameof(RFExcursion), MsgTipId.GreatethanMax, false, "", 1);
            };
            return value;
        }

        private Int32 _MaxMarkerCount = Constants.RF_MAX_MARKER_COUNT_MAX;

        public Int32 MaxMarkerCount
        {
            get { return _MaxMarkerCount; }
            set
            {
                value = ValidateMaxMarkerCount(value);
                if (_MaxMarkerCount != value)
                {
                    _MaxMarkerCount = value;
                    OnPropertyChanged(nameof(MaxMarkerCount));
                }
            }
        }

        private Int32 ValidateMaxMarkerCount(Int32 value)
        {
            if (value < Constants.RF_MAX_MARKER_COUNT_MIN)
            {
                value = Constants.RF_MAX_MARKER_COUNT_MIN;
                WeakTip.Default.Write(nameof(MaxMarkerCount), MsgTipId.LessthanMin, false, "", 1);
            }
            else if (value > Constants.RF_MAX_MARKER_COUNT_MAX)
            {
                value = Constants.RF_MAX_MARKER_COUNT_MAX;
                WeakTip.Default.Write(nameof(MaxMarkerCount), MsgTipId.GreatethanMax, false, "", 1);
            }
            return value;
        }



        #endregion

        public IEnumerable<(Int32 Index, Double Position)> PosIndexes => _PosIndexes.Where(o => o.HasValue).Select((o, i) => (i, o!.Value));

        public void MoveAll(Double dist)
        {
            lock (_PosIndexes.SyncRoot)
            {
                var maxpos = PosIndexes.Max(ivp => ivp.Position);
                var minpos = PosIndexes.Min(ivp => ivp.Position);

                var pos = maxpos + dist;
                if (pos > MaxPosIndex)
                    dist = MaxPosIndex - maxpos;
                else
                {
                    pos = minpos + dist;
                    if (pos < MinPosIndex)
                        dist = MinPosIndex - minpos;
                }

                foreach (var (index, value) in PosIndexes)
                    _PosIndexes[index] = value + dist;

                OnPropertyChanged("PosIndex");
            }
        }

        #region Position Index Converter  
        private CursorPosFormat _PosFormat = CursorPosFormat.Axis;

        public CursorPosFormat PosFormat
        {
            get => _PosFormat;
            set
            {
                if (_PosFormat != value)
                {
                    _PosFormat = value;
                    OnPropertyChanged(nameof(PosFormat));
                }
            }
        }

        protected virtual (Double Value, Prefix Pfx, String Unit) PosIndexToPercent(Double position)
        {
            var value = (position - InitialRefPos) * 100.0 / Math.Abs(InitialRefPos - FinalRefPos);
            return (value, Prefix.Empty, QuantityUnit.Percent.ToUnitString());
        }

        protected virtual (Double Value, Prefix Pfx, String Unit) PosIndexToAngle(Double position)
        {
            var value = (position - InitialRefPos) * 360.0 / Math.Abs(InitialRefPos - FinalRefPos);
            return (value, Prefix.Empty, QuantityUnit.Angle.ToUnitString());
        }

        protected virtual (Double Value, Prefix Pfx, String Unit) PosIndexToAxis(Double position)
        {
            var (scale, pos0, pfx, u) = GetPosAxisInfo(_Source);
            var value = (position - pos0) * scale;
            return (value, pfx, u);
        }

        protected virtual (Double Value, Prefix Pfx, String Unit) PosIndexToInvAxis(Double position)
        {
            var (value, pfx, u) = PosIndexToAxis(position);
            value = 1 / value;
            var hz = u is "s" ? "Hz" : "?";
            return (value, pfx, hz);
        }

        public Double InitialRefPos
        {
            get;
            set;
        }

        public Double FinalRefPos
        {
            get;
            set;
        }

        //Cursor position axis info
        public Func<ChannelId, (Double Scale, Double Pos0, Prefix Pfx, String Unit)> GetPosAxisInfo
        {
            get;
            set;
        } = (s) => (1, 0, Prefix.Empty, "?");

        public (Double Value, Prefix Pfx, String Unit) GetPosInfo(Double position, Func<Double, Object?, (Double Value, Prefix Pfx, String Unit)>? converter = null, Object? arg = null)
        {
            return PosFormat switch
            {
                CursorPosFormat.Axis => PosIndexToAxis(position),
                CursorPosFormat.InvAxis => PosIndexToInvAxis(position),
                CursorPosFormat.Degree => PosIndexToAngle(position),
                CursorPosFormat.Percent => PosIndexToPercent(position),
                _ => converter?.Invoke(position, arg) ?? (position, Prefix.Empty, "?"),
            };
        }

        public (Double Value, Prefix Pfx, String Unit) GetValueInfo(Int32 position)
        {
            var (buffer, _, pos, pfx, u, s, o) = GetValueAxisInfo(_Source);
            if (buffer != null && buffer.GetLength(1) > 0 && DsoModel.Default.TryGetChannel(_Source, out var chnl) && chnl is MathModel mm && mm != null && mm.Pack != null)
            {
                var chnlbias = mm.Pack.Properties.ChnlBias;
                var scale = mm.Conditioning.ScaleBymV;
                var posidxperdiv = mm.Conditioning.PosIdxPerDiv;
                var posdndex = mm.Conditioning.PosIndex;
                var posscale = posidxperdiv / scale;
                var value = (buffer[0, position] - posdndex) / posscale + chnlbias;

                return (value, pfx, u);
            }
            return (Double.NaN, pfx, u);
        }

        public Double MaxPosition => GetPosInfo(MaxPosIndex).Value;

        public Double MinPosition => GetPosInfo(MinPosIndex).Value;
        #endregion

        #region Sample Values Converter
        //Sampling value axis info, it is relative to the waveform.
        public Func<ChannelId, (Double[,]? Buffer, Double Scale, Double Pos0, Prefix Pfx, String Unit, Double SampleRate, Double Offset)> GetValueAxisInfo
        {
            get;
            set;
        } = (s) => (null, 1, 0, Prefix.Empty, "?", 1, 0);

        private Range? CalcSamplesIdxFromPos(Double position, Int32 length, Double SampleRate, Double Offset)
        {
            var ratio = length / (MaxPosIndex - MinPosIndex);
            var start = (Int32)((position - MinPosIndex) * ratio);
            start = (Int32)((position - Offset) * SampleRate);
            var end = length == 0 ? 0 : (Int32)Math.Ceiling(ratio) + start;
            if (end > length || start < 0)
            {
                return null;
            }
            return start..end;
        }

        public (List<Double>? Values, Prefix Pfx, String Unit) GetFirstSamples(Double position)
        {
            return GetFirstSamples(position, _Source);
        }

        public (List<Double>? Values, Prefix Pfx, String Unit) GetFirstSamples(Double position, ChannelId source)
        {
            var (buffer, scale, pos, pfx, u, s, o) = GetValueAxisInfo(source);
            if (buffer?.GetLength(1) > 0)
            {
                var range = CalcSamplesIdxFromPos(position, buffer.GetLength(1), s, o);
                if (range == null)
                {
                    return (null, pfx, u);
                }
                Int32 length = ((Range)range).End.Value - ((Range)range).Start.Value;
                if (((Range)range).End.Value + 1 < buffer.GetLength(1))
                {
                    length = length + 1;
                }
                var values = buffer.SubColumnMatrix(((Range)range).Start.Value, length).ToEnumerable().ToList();
                //return (values, pfx, u);
                //var result = values;//.Select(x =>(x - pos)/*/ Constants.IDX_PER_YDIV*/*scale).ToList();
                var result = values.Select(x => (x - pos)/*/ Constants.IDX_PER_YDIV*/* scale).ToList();
                return (result, pfx, u);
            }
            return (new(), pfx, u);
        }


        public (List<Double>? Values, Prefix Pfx, String Unit) GetSamples(Double position, Func<Double[,], Range, List<Double>> calculator)
        {
            var (buffer, _, _, pfx, u, s, o) = GetValueAxisInfo(_Source);
            if (buffer != null)
            {
                var range = CalcSamplesIdxFromPos(position, buffer.GetLength(1), s, o);
                if (range == null)
                {
                    return (null, pfx, u);
                }
                var values = calculator(buffer, (Range)range);
                return (values, pfx, u);
            }
            return (new(), pfx, u);
        }

        public (Double[,]? Buffer, Range? Index, Prefix Pfx, String Unit) GetSamples(Double position)
        {
            var (buffer, _, _, pfx, u, s, o) = GetValueAxisInfo(_Source);
            if (buffer != null)
            {
                var range = CalcSamplesIdxFromPos(position, buffer.GetLength(1), s, o);
                if (range == null)
                {
                    return (null, null, pfx, u);
                }
                return (buffer, (Range)range, pfx, u);
            }
            return (buffer, 0..0, pfx, u);
        }
        #endregion

        public Double GetFirstSamplesCoordinateY(Double position)
        {
            var (buffer, scale, pos, _, _, sampleRate, offset) = GetValueAxisInfo(_Source);
            if (buffer?.GetLength(1) > 0)
            {
                var range = CalcSamplesIdxFromPos(position, buffer.GetLength(1), scale, pos);
                if (range != null)
                {
                    if (((Range)range).End.Value == ((Range)range).Start.Value && ((Range)range).Start.Value == 0)
                    {
                        return 0;
                    }
                    if (((Range)range).Start.Value < buffer?.GetLength(1))
                    {
                        var values = buffer.SubColumnMatrix(((Range)range).Start.Value, 1).ToEnumerable().ToList();
                        return values[0];
                    }
                }

            }
            return 0;
        }

        public void AutoLocate(Int32 num, Func<Double[,], List<Double>> search)
        {
            var (buffer, _, _, pfx, u, s, o) = GetValueAxisInfo(_Source);
            if (buffer != null)
            {
                var res = search(buffer);
                if (num > res.Count)
                    num = res.Count;
                for (Int32 i = 0; i < num; i++)
                    _PosIndexes[i] = res[i];
            }
        }


        public List<(Int32 PositionIndex, Double Amp/*, Double Frequency*/)>? GetLargestValueIndexs()
        {
            //AtuoMarkerActive
            if (!_Owner.AtuoMarkerActive)
                return null;
            //var (buffer, _, _, pfx, u, s, o) = GetValueAxisInfo(_Source);
            var chn = DsoModel.Default.GetChannel(_Source);
            var cbuffer = chn.VuDatabase?.Current?.Buffer;
            if (cbuffer == null)
                return null;
            #region 在屏幕显示的数据里寻峰
            var range = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(chn.Id, true);
            Int32 length = range.End.Value - range.Start.Value;
            Double[,] sbuffer = new Double[1, length];
            if (length == 0 || length == cbuffer.GetLength(1))
                sbuffer = cbuffer;
            else
            {
                for (Int32 i = 0; i < length; i++)
                {
                    sbuffer[0, i] = cbuffer[0, range.Start.Value + i];
                }
            }
            #endregion

            var scale = chn.Conditioning.Scale;
            var chnlbias = chn is AnalogModel am ? am.Conditioning.BiasByuV * 1E-3 : 0;
            var posidxperdiv = chn.Conditioning.PosIdxPerDiv;
            var posdndex = chn.Conditioning.PosIndex;
            var posscale = posidxperdiv / scale;
            var threshold = _RFThreshold * 1E3;
            var excursion = _RFExcursion * 1E3;
            excursion = (excursion - chnlbias) * posscale;
            threshold = (threshold - chnlbias) * posscale + posdndex;
            if (chn is MathModel mathModel && mathModel.Args is MathFftArg args)
            {
                switch (args.ResultUnit)
                {
                    case FFTCoordUnit.Vrms:
                    default:

                        break;
                    case FFTCoordUnit.dBm:
                    case FFTCoordUnit.dBμW:
                    case FFTCoordUnit.dBmV:
                    case FFTCoordUnit.dBμV:
                    case FFTCoordUnit.dBmA:
                    case FFTCoordUnit.dBμA:

                        break;
                }
            }
            else if (chn is RadioFrequencyModel)
            {

            }
            else
            {
                return null;
            }

            if (sbuffer != null)
            {
                var startindex = (length == 0 || length == cbuffer.GetLength(1)) ? 0 : range.Start.Value;
                var rst = FindPeaks.FindMarkablePeaks(sbuffer, _MaxMarkerCount, excursion, threshold);
                List<(Int32 PositionIndex, Double Amp)> temp = new List<(Int32 PositionIndex, Double Amp)>();
                foreach (var item in rst)
                {
                    temp.Add((item.PositionIndex + startindex, item.Amp));
                }

                return temp;
            }
            return null;
        }
        public void Run()
        {
            _LastResult = GetLargestValueIndexs();
        }

        public RFMarkerBarModel(MarkerItemModel owner, Double maxPos, Double minPos, Int32 num = 11)
        {
            _PosIndexes = new Double?[num];

            MaxPosIndex = maxPos;
            MinPosIndex = minPos;

            _PosIndexes[0] = InitialRefPos = (maxPos - minPos) * 0.1 + minPos;
            _PosIndexes[1] = FinalRefPos = (maxPos - minPos) * 0.9 + minPos;
            _Owner = owner;
            Source = owner.Source;
        }


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
    }
}
