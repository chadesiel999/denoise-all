// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/24</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using ScopeX.MathExt;


    internal class StdMaskTest : INotifyPropertyChanged
    {
        private PFStdMaskType _MaskType = PFStdMaskType.ANSI_T1_102;
        public PFStdMaskType StdMaskType
        {
            get => _MaskType;
            set
            {
                if (_MaskType != value)
                {
                    _MaskIndex = 0;
                    OnPropertyChanged("StdMaskIndex");
                    _MaskType = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _MaskFullName = "";
        public String MaskFullName
        {
            get => _MaskFullName;
            set
            {
                if (_MaskFullName != value)
                {
                    _MaskFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _MaskIndex = 0;
        public Int32 MaskIndex
        {
            get => _MaskIndex;
            set
            {
                if (_MaskIndex != value)
                {
                    _MaskIndex = value;
                    OnPropertyChanged("StdMaskIndex");
                }
            }
        }

        //模板是否创建或读入
        public Boolean MaskCreated
        {
            get;
            set;
        } = false;

        public struct StdMaskPkg
        {
            //垂直余量偏移范围
            public Double Amplitude;
            //垂直刻度，单位V
            public Double VScale;
            //垂直位移，单位div
            public Double VPos;
            //垂直偏移电压，单位V
            public Double VOffset;
            //水平刻度，单位s
            public Double HScale;
            //触发水平位移，单位%
            public Double HPos;
            //
            public Double HWidth;
            //触发位置到屏幕中心的大小，单位s
            public Double TrigToSamp;
            //存储深度
            public Double RecordLength;
            //此模板的垂直余量
            public Double Percent;
            //各分段的数据区域
            public List<PointF>[] Segments;

            public Boolean IsVaild;

            //显示坐标下的分段绘制路径
            public List<(Double x, Double y)>[] SegPaths;
        }

        public StdMaskPkg StdMask = new()
        {
            Amplitude = 0,
            VScale = 0,
            VPos = 0,
            VOffset = 0,
            HScale = 0,
            HPos = 0,
            HWidth = 0,
            TrigToSamp = 0,
            Percent = 0,
            RecordLength = 0,
            Segments = new List<PointF>[]
            {
                new List<PointF>(), new List<PointF>(), new List<PointF>(), new List<PointF>(),
                new List<PointF>(), new List<PointF>(), new List<PointF>(), new List<PointF>()
            },

            IsVaild = false,

            SegPaths = new List<(Double x, Double y)>[]
            {
                new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(),
                new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>()
            },
        };

        //读取标准模板数据
        public Boolean ReadMask(String name, ChannelId id)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            //ResourceManager rm = new(this.GetType().Namespace + ".Properties.Resources", asm);

            //using StreamReader mr = new(new MemoryStream((Byte[])rm.GetObject(path)!));
            var obj = Properties.Resources.ResourceManager.GetObject(name);
            if (obj is null)
            {
                return false;
            }

            using StreamReader sr = new(new MemoryStream((Byte[])obj));
            String maskstr = sr.ReadToEnd();
            String key = "AMPLITUDE";
            Int32 firstidx = maskstr.IndexOf(key, 0);
            Int32 lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.Amplitude = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "VSCALE";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.VScale = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);
            //_StdMasks.VScale *= 1.25;

            key = "VPOS";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.VPos = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);
            //_StdMasks.VPos *= 0.8;

            key = "VOFFSET";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.VOffset = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "HSCALE";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.HScale = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "HTRIGPOS";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.HPos = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "WIDTH";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.HWidth = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "RECORDLENGTH";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.RecordLength = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "TRIGTOSAMP";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.TrigToSamp = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            key = "PERCENT";
            firstidx = maskstr.IndexOf(key, lastidx);
            lastidx = maskstr.IndexOf(":", firstidx);
            StdMask.Percent = Double.Parse(maskstr[(firstidx + key.Length + 1)..lastidx]);

            String[] segstrs =
            {
                    "SEG1:POINTS",
                    "SEG2:POINTS",
                    "SEG3:POINTS",
                    "SEG4:POINTS",
                    "SEG5:POINTS",
                    "SEG6:POINTS",
                    "SEG7:POINTS",
                    "SEG8:POINTS",
            };

            for (Int32 j = 0; j < segstrs.Length; j++)
            {
                key = segstrs[j];
                firstidx = maskstr.IndexOf(key, lastidx);
                lastidx = maskstr.IndexOf("\r\n", firstidx + key.Length);
                var points = maskstr[(firstidx + key.Length + 1)..lastidx];
                String[] subpoints = points.Split(char.Parse(","));
                StdMask.Segments[j].Clear();
                for (Int32 i = 0; i < subpoints.Length / 2; i++)
                {
                    StdMask.Segments[j].Add(new PointF(Single.Parse(subpoints[i * 2]), Single.Parse(subpoints[i * 2 + 1])));
                }
            }

            StdMask.IsVaild = true;

            SetPosScalePairs(id);

            return true;
        }

        private void SetPosScalePairs(ChannelId id)
        {

            if (id.IsAnalog() && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out IChnlPrsnt channel))
            {
                if (channel is AnalogPrsnt analog && DsoModel.Default.TryGetChannel(id, out var channelmodel))
                {
                    analog.Ylevel_SelectStatus = true;
                    StdMask.VScale = StdMask.VScale * 1_000;
                    analog.ScaleBymV = StdMask.VScale;
                    StdMask.VPos = StdMask.VPos * channelmodel.Conditioning.PosIdxPerDiv + channelmodel.Conditioning.PosDefIndex;
                    analog.PosIndexBymDiv = StdMask.VPos /** channelmodel.Conditioning.PosIdxPerDiv + channelmodel.Conditioning.PosDefIndex*/;

                    StdMask.HScale = StdMask.HScale * 1_000_000;
                    analog.Sampling.ScaleByus = StdMask.HScale;
                    StdMask.HPos = StdMask.HPos * Constants.VIS_XDIVS_NUM * channelmodel.Sampling.PosIdxPerDiv;
                    analog.Sampling.PosIndexBymDiv = StdMask.HPos /** Constants.VIS_XDIVS_NUM * channelmodel.Sampling.PosIdxPerDiv*/;
                }
            }

            //if (DsoModel.Default.TryGetChannel(id, out var chnl))
            //{
            //    //根据模板垂直档位设置最接近的物理垂直档位，有可能失败
            //    chnl.Conditioning.Scale = StdMask.VScale * 1_000;
            //    //根据模板垂直位移设置最接近的物理位移
            //    chnl.Conditioning.PosIndex = StdMask.VPos * chnl.Conditioning.PosIdxPerDiv + chnl.Conditioning.PosDefIndex;

            //    //根据模板时基设置最接近的物理时基，有可能失败
            //    chnl.Sampling.Scale = StdMask.HScale * 1_000_000;
            //    //根据模板触发深度百分比设置物理触发深度百分比，无论物理时基和模板时基是否同，触发深度百分比不变
            //    chnl.Sampling.PosIndex = StdMask.HPos * Constants.VIS_XDIVS_NUM * chnl.Sampling.PosIdxPerDiv;

            //}
        }

        public Boolean LockMask(ChannelId id)
        {
            if (DsoModel.Default.TryGetChannel(id, out var chnl))
            {
                StdMask.HScale = chnl.Sampling.Scale;
                StdMask.HPos = chnl.Sampling.PosIndex;

                StdMask.VScale = chnl.Conditioning.ScaleBymV;
                StdMask.VPos = chnl.Conditioning.PosIndex;
                return true;
            }
            return false;

        }

        public Boolean MakeMask(ChannelId id, Boolean locked)
        {
            Double xscale;
            Double xpos0;
            Double yscale;
            Double ypos0;

            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl is null)
            {
                return false;
            }
            if (chnl is not AnalogModel analog)
            {
                return false;
            }

            if (!locked)
            {
                xscale = chnl.Sampling.Scale;
                xpos0 = chnl.Sampling.PosIndex;

                yscale = analog.Conditioning.ScaleBymV;
                ypos0 = chnl.Conditioning.PosIndex;
            }
            else
            {
                xscale = StdMask.HScale /** 1e6*/;
                xpos0 = StdMask.HPos/* * chnl.Sampling.PosIdxPerDiv * Constants.VIS_XDIVS_NUM*/;

                yscale = StdMask.VScale /** 1e3*/;
                ypos0 = StdMask.VPos /** chnl.Conditioning.PosIdxPerDiv + chnl.Conditioning.PosDefIndex*/;
            }

            lock (StdMask.SegPaths.SyncRoot)
            {
                for (Int32 i = 0; i < 8; i++)
                {
                    StdMask.SegPaths[i].Clear();
                }

                //分别处理每一个封闭区域
                for (Int32 i = 0; i < StdMask.Segments.Length; i++)
                {
                    //平面上至少3个点才能定义一个封闭区域
                    if (StdMask.Segments[i].Count >= 3)
                    {
                        var points = new (Double, Double)[StdMask.Segments[i].Count];

                        Double x, y;
                        for (Int32 j = 0; j < StdMask.Segments[i].Count; j++)
                        {
                            //直接将物理时间坐标转换到图形显示坐标
                            //x = (Int32)(GCtrlParameter.DEF_X_POS + (_StdMasks.Segments[i][j].X + xpos0) / xscale * GCtrlParameter.XDOTS_PER_DIV);
                            x = xpos0 + StdMask.Segments[i][j].X * 1e6 / xscale * chnl.Sampling.PosIdxPerDiv;

                            //先将物理电压坐标转换到AD坐标
                            y = ypos0 + StdMask.Segments[i][j].Y * 1e3 / yscale * chnl.Conditioning.PosIdxPerDiv;

                            points[j] = (x, y);
                        }

                        StdMask.SegPaths[i].AddRange(points);
                    }
                }
            }

            MaskCreated = true;

            return true;
        }

        public Boolean Test(Double[] pbuffer, PassFailInfo pfi)
        {
            Boolean bpassed = true;

            var hits = new List<List<(Double x, Double y)>>();
            Int32 hitstate = -1;//-1:不相交
            var hit = new List<(Double x, Double y)>();
            lock (StdMask.SegPaths.SyncRoot)
            {
                //lock (pfi.HitsLocker)
                //{
                for (Int32 i = 0; i < pbuffer.Length; i++)
                {
                    Boolean pointpassed = true;

                    for (Int32 j = 0; j < StdMask.SegPaths.Length; j++)
                    {
                        if (StdMask.SegPaths[j].Count < 3)
                        {
                            continue;
                        }

                        var pt = (i, Convert.ToSingle(pbuffer[i]));

                        if (StdMask.SegPaths[j].InPolygon(pt))
                        {
                            if (hitstate != j && hit.Count > 0)
                            {
                                hits.Add(hit);
                                hit = new List<(Double x, Double y)>();
                            }
                            hitstate = j;
                            hit.Add(pt);

                            pfi.SegHits[j, 0]++;
                            pfi.TotalHits[0]++;
                            bpassed = false;
                            pointpassed = false;
                        }
                    }
                    if (pointpassed)
                    {
                        hitstate = -1;
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
                //}
                return bpassed;
            }
        }

        public Boolean Test(ChannelId id, PassFailInfo pfi)
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

            Double zoomratio = 1;
            Double xstart = 0;
            var pbuffer = chnl.PackForVu.Buffer.ToJagged()[0];
            var offsety = chnl.Conditioning.Position * analog.Conditioning.ScaleBymV / analog.Conditioning.Scale;
            if (chnl.VuDatabase != null && chnl.VuDatabase.Current != null)
            {
                zoomratio = chnl.VuDatabase.Current.ZoomRatio;
                xstart = chnl.VuDatabase.Current.Start;
            }

            Boolean bpassed = true;

            var hits = new List<List<(Double x, Double y)>>();
            var hit = new List<(Double x, Double y)>();
            Int32 hitstate = -1;//-1:不相交

            lock (StdMask.SegPaths.SyncRoot)
            {
                for (Int32 i = 0; i < pbuffer.Length; i++)
                {
                    Boolean pointpassed = true;
                    for (Int32 j = 0; j < StdMask.SegPaths.Length; j++)
                    {
                        if (StdMask.SegPaths[j].Count < 3)
                        {
                            continue;
                        }

                        var pt = (i / zoomratio + xstart, (Convert.ToSingle(pbuffer[i] + offsety)) / analog.Conditioning.ScaleBymV * chnl.Conditioning.PosIdxPerDiv);

                        if (StdMask.SegPaths[j].InPolygon((pt.Item1, pt.Item2)))
                        {
                            if (hitstate != j && hit.Count > 0)
                            {
                                hits.Add(hit);
                                hit = new List<(Double x, Double y)>();
                            }
                            hitstate = j;
                            hit.Add(pt);

                            pfi.SegHits[j, 0]++;
                            pfi.TotalHits[0]++;
                            bpassed = false;
                            pointpassed = false;
                        }
                    }
                    if (pointpassed)
                    {
                        hitstate = -1;
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
