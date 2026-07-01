// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/24</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using ScopeX.ComModel;
    using ScopeX.MathExt;



    internal class LimitTest : INotifyPropertyChanged
    {
        private Region? _Topregion = null;
        private Region? _Btmregion = null;
        private GraphicsPath? _GraphicsPath = null;
        //垂直容限，单位mdiv，1div=1000mdiv
        private List<(Double x, Double y)> _TopSegmentRaw = new List<(Double x, Double y)>();
        private List<(Double x, Double y)> _BtmSegmentRaw = new List<(Double x, Double y)>();
        private Double _ChnlPostion;
        private Double _ChnlScale;
        private Double _TimePostion;
        private Double _TimeScale;
        private Object _UpdateMaskLocker = new object();

        private Int32 _VertTolerance = 200;

        public Int32 VertTolerance
        {
            get => _VertTolerance;
            set
            {
                value = Validate(value, Constants.MAX_VERT_TOLERANCE_IDX);
                if (_VertTolerance != value)
                {
                    _VertTolerance = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MaxVertTolerance = Constants.MAX_VERT_TOLERANCE_IDX;

        public readonly Int32 MinVertTolerance = 1;
        //水平容限，单位mdiv，1div=1000mdiv
        private Int32 _HorzTolerance = 100;
        public Int32 HorzTolerance
        {
            get => _HorzTolerance;
            set
            {
                value = Validate(value, Constants.MAX_HORZ_TOLERANCE_IDX);
                if (_HorzTolerance != value)
                {
                    _HorzTolerance = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MaxHorzTolerance = Constants.MAX_HORZ_TOLERANCE_IDX;

        public readonly Int32 MinHorzTolerance = 1;

        private static Int32 Validate(Int32 value, Int32 max)
        {
            if (value > max)
            {
                value = max;
            }
            else if (value < 1)
            {
                value = 1;
            }

            return value;
        }
        private List<(Double x, Double y)> _TopSegment = new List<(Double x, Double y)>();

        public List<(Double x, Double y)> TopSegment => _TopSegment.AsReadOnly().ToList();


        private List<(Double x, Double y)> _BtmSegment = new List<(Double x, Double y)>();

        public List<(Double x, Double y)> BtmSegment => _BtmSegment.AsReadOnly().ToList();

        private Double _MaskZoomRatio = 1;
        public Double MaskZoomRatio => _MaskZoomRatio;
        private Double _MaskStart = 0;
        public Double MaskStart => _MaskStart;

        //模板是否创建或读入
        public Boolean MaskCreated
        {
            get;
            set;
        } = false;

        public void MakeMask(ChannelId id)
        {
            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl?.Pack is null)
            {
                return;
            }
            if (chnl is not AnalogModel analog)
            {
                return;
            }

            var buffer = chnl.Pack.Buffer.ToJagged()[0];
            var yratio = analog.Conditioning.ScaleBymV;
            var offsety = chnl.Conditioning.PosIndex / chnl.Conditioning.PosIdxPerDiv * yratio;
            var topsegment = new List<(double x, double y)>();
            var btmsegment = new List<(double x, double y)>();

            _MaskZoomRatio = (chnl.Sampling.Scale * 1E-6 / chnl.Sampling.PosIdxPerDiv) / (chnl.Pack.Properties.SampInterval);
            _MaskStart = chnl.Sampling.PosIndex - (chnl.Pack.Properties.TmbPosition.Index - chnl.Pack.Properties.VuStartIndex) * chnl.Pack.Properties.TmbScale.Value / chnl.Sampling.Scale;

            List<PointF> toppointfs = new List<PointF>();
            List<PointF> btmpointfs = new List<PointF>();
            Int32 maxyposindex = Constants.VIS_YDIVS_NUM / 2 * Constants.IDX_PER_YDIV;
            Int32 minyposindex = -maxyposindex;

            topsegment.Add((_MaskStart, maxyposindex));
            btmsegment.Add((_MaskStart, minyposindex));

            toppointfs.Add(new PointF((Single)_MaskStart, maxyposindex));
            btmpointfs.Add(new PointF((Single)_MaskStart, minyposindex));

            _TopSegmentRaw.Clear();
            _BtmSegmentRaw.Clear();

            _TopSegmentRaw.Add(((_MaskStart - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, maxyposindex / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
            _BtmSegmentRaw.Add(((_MaskStart - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, minyposindex / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));

            Int32 lb, rb;
            Int32 bufferlength = buffer.Length;
            Int32 temp = -1;
            Double tempbtmy = 0;
            Double temptopy = 0;
            Double xvalue;
            Int32 topbufferlastuseindex = -1;
            Int32 btmbufferlastuseindex = -1;
            Int32 tempscale = bufferlength / ((Int32)Constants.MAX_XPOS_IDX) * 5;
            tempscale = tempscale < 1 ? 1 : tempscale;
            var horzindex = HorzTolerance * 1E-3 * Constants.IDX_PER_XDIV * bufferlength / ((Int32)Constants.MAX_XPOS_IDX);
            for (Int32 i = 0; i < bufferlength; i++)
            {
                if (i / tempscale == temp)
                {
                    continue;
                }
                temp = i / tempscale;
                //水平单元0-500毫格
                lb = (Int32)(i - horzindex);
                lb = lb < 0 ? 0 : lb;
                rb = (Int32)(i + horzindex);
                rb = rb >= bufferlength ? bufferlength - 1 : rb;
                var segment = buffer.Skip(lb).Take(rb - lb + 1);
                var max = segment.Max();
                var min = segment.Min();

                if (Double.IsFinite(max) && Double.IsFinite(min))
                {
                    xvalue = _MaskStart + i / _MaskZoomRatio;
                    //垂直单元0-1000毫格
                    var topy = (max + VertTolerance * 1E-3 * yratio + offsety) / analog.Conditioning.ScaleBymV * chnl.Conditioning.PosIdxPerDiv;
                    if (temptopy != topy)
                    {
                        Double masktempy = temptopy >= maxyposindex ? (maxyposindex-1) : temptopy;
                        if (topbufferlastuseindex != -1 && (i - topbufferlastuseindex) > 1)
                        {
                            var xvaluetemp = _MaskStart + (i - 1) / _MaskZoomRatio;
                            topsegment.Add((xvaluetemp, masktempy));
                            toppointfs.Add(new PointF((Single)(xvaluetemp), (Single)temptopy));
                            _TopSegmentRaw.Add(((xvaluetemp - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, temptopy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
                        }

                        temptopy = topy;
                        masktempy = temptopy >= maxyposindex ? (maxyposindex - 1) : temptopy;
                        topsegment.Add((xvalue, masktempy));
                        toppointfs.Add(new PointF((Single)(xvalue), (Single)temptopy));
                        _TopSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, temptopy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
                        topbufferlastuseindex = i;
                    }

                    var btmy = (min - VertTolerance * 1E-3 * yratio + offsety) / analog.Conditioning.ScaleBymV * chnl.Conditioning.PosIdxPerDiv;
                    if (tempbtmy != btmy)
                    {
                        Double masktempy = tempbtmy <= minyposindex ? (minyposindex+1) : tempbtmy;
                        if (btmbufferlastuseindex != -1 && (i - btmbufferlastuseindex) > 1)
                        {
                            var xvaluetemp = _MaskStart + (i - 1) / _MaskZoomRatio;
                            btmsegment.Add((xvaluetemp, masktempy));
                            btmpointfs.Add(new PointF((Single)(xvaluetemp), (Single)tempbtmy));
                            _BtmSegmentRaw.Add(((xvaluetemp - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, tempbtmy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
                        }

                        tempbtmy = btmy;
                        masktempy = tempbtmy <= minyposindex ? (minyposindex + 1) : tempbtmy;
                        btmsegment.Add((xvalue, masktempy));
                        btmpointfs.Add(new PointF((Single)(xvalue), (Single)tempbtmy));
                        _BtmSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, tempbtmy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
                        btmbufferlastuseindex = i;
                    }
                }
            }
            xvalue = _MaskStart + (bufferlength - 1) / _MaskZoomRatio;
            if (topbufferlastuseindex != bufferlength - 1)
            {
                Double masktempy = temptopy >= maxyposindex ? (maxyposindex-1) : temptopy;
                topsegment.Add((xvalue, masktempy));
                toppointfs.Add(new PointF((Single)(xvalue), (Single)temptopy));
                _TopSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, temptopy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
            }
            if (btmbufferlastuseindex != bufferlength - 1)
            {
                Double masktempy = tempbtmy <= minyposindex ? (minyposindex+1) : tempbtmy;
                btmsegment.Add((xvalue, masktempy));
                btmpointfs.Add(new PointF((Single)(xvalue), (Single)tempbtmy));
                _BtmSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, tempbtmy / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
            }
            topsegment.Add((xvalue, maxyposindex));
            btmsegment.Add((xvalue, minyposindex));

            toppointfs.Add(new PointF((Single)(xvalue), maxyposindex));
            btmpointfs.Add(new PointF((Single)(xvalue), minyposindex));

            _TopSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, maxyposindex / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));
            _BtmSegmentRaw.Add(((xvalue - chnl.Sampling.PosIndex) / chnl.Sampling.PosIdxPerDiv * chnl.Sampling.Scale, minyposindex / chnl.Conditioning.PosIdxPerDiv * yratio - offsety));

            _TopSegment = topsegment;
            _BtmSegment = btmsegment;

            CreatRegion(toppointfs, btmpointfs);

            _ChnlPostion = chnl.Conditioning.PosIndex;
            _ChnlScale = analog.Conditioning.ScaleBymV;
            _TimePostion = chnl.Sampling.PosIndex;
            _TimeScale = chnl.Sampling.Scale;

            MaskCreated = true;
        }

        private void CreatRegion(List<PointF> toppointfs, List<PointF> btmpointfs)
        {
            if (_GraphicsPath == null)
            {
                _GraphicsPath = new GraphicsPath();
            }
            if (toppointfs != null && toppointfs.Count > 0)
            {
                _GraphicsPath.Reset();
                _GraphicsPath.AddPolygon(toppointfs.ToArray());
                if (_Topregion == null)
                {
                    _Topregion = new Region();
                }
                _Topregion.MakeEmpty();
                _Topregion.Union(_GraphicsPath);
            }

            if (btmpointfs != null && btmpointfs.Count > 0)
            {
                _GraphicsPath.Reset();
                _GraphicsPath.AddPolygon(btmpointfs.ToArray());
                if (_Btmregion == null)
                {
                    _Btmregion = new Region();
                }
                _Btmregion.MakeEmpty();
                _Btmregion.Union(_GraphicsPath);
            }
        }

        public void UpdateMask(ChannelId id)
        {
            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl?.Pack is null)
            {
                return;
            }
            if (chnl is not AnalogModel analog)
            {
                return;
            }

            if (chnl.Conditioning.PosIndex == _ChnlPostion && _ChnlScale == analog.Conditioning.ScaleBymV
                && _TimePostion == chnl.Sampling.PosIndex && _TimeScale == chnl.Sampling.Scale)
            {
                return;
            }
            lock (_UpdateMaskLocker)
            {
                _ChnlPostion = chnl.Conditioning.PosIndex;
                _ChnlScale = analog.Conditioning.ScaleBymV;
                _TimePostion = chnl.Sampling.PosIndex;
                _TimeScale = chnl.Sampling.Scale;
                var topsegment = new List<(double x, double y)>();
                var btmsegment = new List<(double x, double y)>();

                List<PointF> toppointfs = new List<PointF>();
                List<PointF> btmpointfs = new List<PointF>();
                Double temptopy = 0;
                Double tempbtmy = 0;
                Double toptempx = 0;
                Double btmtempx = 0;
                //性能优化，Double btmx =item.x / chnl.Sampling.Scale * chnl.Sampling.PosIdxPerDiv + chnl.Sampling.PosIndex;
                Double tempx = chnl.Sampling.PosIdxPerDiv / chnl.Sampling.Scale;
                Double tempy = chnl.Conditioning.PosIdxPerDiv / analog.Conditioning.ScaleBymV;
                var offsety = chnl.Conditioning.PosIndex / chnl.Conditioning.PosIdxPerDiv * analog.Conditioning.ScaleBymV;
                Int32 index = 0;
                Int32 topsegmentrawcount = _TopSegmentRaw.Count;
                Int32 btmsegmentrawcount = _BtmSegmentRaw.Count;
                Int32 lasthandlecount = 3;
                Int32 topbufferlastuseindex = -1;
                Int32 btmbufferlastuseindex = -1;
                foreach (var item in _TopSegmentRaw)
                {
                    Double topx = item.x * tempx + chnl.Sampling.PosIndex;
                    Double topy = (item.y + offsety) * tempy;

                    if (temptopy != topy || index > topsegmentrawcount - lasthandlecount)
                    {
                        if (topbufferlastuseindex != -1 && (index - topbufferlastuseindex) > 1)
                        {
                            topsegment.Add((toptempx, temptopy));
                            toppointfs.Add(new PointF((Single)toptempx, (Single)temptopy));
                        }
                        temptopy = topy;
                        topsegment.Add((topx, topy));
                        toppointfs.Add(new PointF((Single)topx, (Single)topy));
                        topbufferlastuseindex = index;
                    }
                    toptempx = topx;
                    index++;
                }

                index = 0;
                foreach (var item in _BtmSegmentRaw)
                {
                    Double btmx = item.x * tempx + chnl.Sampling.PosIndex;
                    Double btmy = (item.y + offsety) * tempy;
                    if (tempbtmy != btmy || index > btmsegmentrawcount - lasthandlecount)
                    {
                        if (btmbufferlastuseindex != -1 && (index - btmbufferlastuseindex) > 1)
                        {
                            btmsegment.Add((btmtempx, tempbtmy));
                            btmpointfs.Add(new PointF((Single)btmtempx, (Single)tempbtmy));
                        }

                        tempbtmy = btmy;
                        btmsegment.Add((btmx, btmy));
                        btmpointfs.Add(new PointF((Single)btmx, (Single)btmy));
                        btmbufferlastuseindex = index;
                    }
                    btmtempx = btmx;
                    index++;
                }
                _TopSegment = topsegment;
                _BtmSegment = btmsegment;

                CreatRegion(toppointfs, btmpointfs);
            }
        }

        public Boolean Test(ChannelId id, PassFailInfo pfi,Boolean locked)
        {
            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl?.PackForVu is null)
            {
                return false;
            }
            if (chnl is not AnalogModel analog)
            {
                return false;
            }
            if (locked == false)
            {
                UpdateMask(id);
            }
            lock (_UpdateMaskLocker)
            {
                var source = chnl.PackForVu.Buffer.ToJagged()[0];
                var offsety = analog.Conditioning.Position * analog.Conditioning.ScaleBymV / analog.Conditioning.Scale;
                Double zoomratio = 1;
                Double xstart = 0;
                if (chnl.VuDatabase != null && chnl.VuDatabase.Current != null)
                {
                    zoomratio = chnl.VuDatabase.Current.ZoomRatio;
                    xstart = chnl.VuDatabase.Current.Start;
                }
                Boolean bpassed = true;

                Int32 hitstate = 0;//0:不相交，1：_TopSegment相交；2：_BtmSegment相交
                var hits = new List<List<(Double x, Double y)>>();
                var hit = new List<(Double x, Double y)>();
                var calc = chnl.Conditioning.PosIdxPerDiv / (analog.Conditioning.ScaleBymV);
                for (Int32 i = 0; i < source.Length; i++)
                {
                    if (Double.IsNaN(source[i]))
                    {
                        hitstate = 0;
                        if (hit.Count > 0)
                        {
                            hits.Add(hit);
                            hit = new List<(Double x, Double y)>();
                        }
                        continue;
                    }
                    Single x = (Single)(i / zoomratio + xstart);
                    Single y = (Single)((source[i] + offsety) * calc);
                    if (_Btmregion!.IsVisible(new PointF(x, y)))
                    {
                        if (hitstate == 1 && hit.Count > 0)
                        {
                            hits.Add(hit);
                            hit = new List<(Double x, Double y)>();
                        }

                        hitstate = 2;
                        hit.Add((x, y));

                        pfi.SegHits[1, 0]++;
                        pfi.TotalHits[0]++;
                        bpassed = false;
                    }
                    else if (_Topregion!.IsVisible(new PointF(x, y)))
                    {
                        if (hitstate == 2 && hit.Count > 0)
                        {
                            hits.Add(hit);
                            hit = new List<(Double x, Double y)>();
                        }
                        hitstate = 1;
                        hit.Add((x, y));

                        pfi.SegHits[0, 0]++;
                        pfi.TotalHits[0]++;
                        bpassed = false;
                    }
                    else
                    {
                        hitstate = 0;
                        if (hit.Count > 0)
                        {
                            hits.Add(hit);
                            hit = new List<(Double x, Double y)>();
                        }
                    }
                }
                if (hit.Count > 0)
                {
                    hits.Add(hit);
                }
                pfi.HitsBuffer = hits;
                return bpassed;
            }
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
