using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class LineAxis : BaseAxis
    {
        class StringInfo
        {
            public string Str = string.Empty;
            public float Value;
            public Vector2 Position;
            public Color Color;
            public VerticalAlignment Vertical;
            public HorizontalAlignment Horizontal;
        }
        float lableoffet = 10;
        private Boolean needupdate = true;
        VeldridRender.TextRender.MutiText veldridText;
        private Veldrid.Common.VeldridRender.LineRender.DataRender dataRender;
        private Veldrid.Common.VeldridRender.LineRender.DataRender dataRender_backGroud;
        List<Vector2> backCrossDatas = new List<Vector2>();
        List<Vector2> backPointDatas = new List<Vector2>();

        List<Vector2> xAxisMajorDatas = new List<Vector2>();
        List<Vector2> xAxisMinorDatas = new List<Vector2>();

        List<Vector2> yAxisMajorDatas = new List<Vector2>();
        List<Vector2> yAxisMinorDatas = new List<Vector2>();


        List<StringInfo> xAxisLables = new List<StringInfo>();

        List<StringInfo> yAxisLables = new List<StringInfo>();
        public LineAxis(IVeldridContent control) : base(control)
        {
            dataRender = new VeldridRender.LineRender.DataRender(control, 2000);
            dataRender_backGroud = new VeldridRender.LineRender.DataRender(control, 2000);
            dataRender.BlendState = new BlendStateDescription()
            {
                AlphaToCoverageEnabled = false,
                BlendFactor = RgbaFloat.Clear,
                AttachmentStates = new BlendAttachmentDescription[]
                {
                    new BlendAttachmentDescription()
                    {
                        BlendEnabled = true,
                        SourceColorFactor = BlendFactor.SourceAlpha,
                        DestinationColorFactor = BlendFactor.Zero,
                        ColorFunction = BlendFunction.Add,
                        SourceAlphaFactor = BlendFactor.SourceAlpha,
                        DestinationAlphaFactor = BlendFactor.Zero,
                        AlphaFunction = BlendFunction.Add,
                    }
                }
            };
            dataRender.Range = this.LineRange;
            dataRender.Margin = this.Margin;
            dataRender.WindowSizeState = true;
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs =new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Brightness= BackInfo.CrossBrighness,
                            Color = BackInfo.CrossColor,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//背景十字
                        new PointConfig()
                        {
                            Brightness = XAxis.Brightness,
                            Color = XAxis.Color,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//XMajor
                        new PointConfig()
                        {
                            Color = XAxis.Color,
                            Brightness= XAxis.Brightness,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//XMinor
                        new PointConfig()
                        {
                            Color = YAxis.Color,
                            Brightness= YAxis.Brightness,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//YMajor
                        new PointConfig()
                        {
                            Color = YAxis.Color,
                            Brightness= YAxis.Brightness,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//YMinor

                    },
                    Primitive =  PrimitiveTopology.LineList,
                }
            };
            dataRender.CreateResources();

            dataRender_backGroud.BlendState = BlendStateDescription.SingleAlphaBlend;
            dataRender_backGroud.Range = this.LineRange;
            dataRender_backGroud.Margin = this.Margin;
            dataRender_backGroud.WindowSizeState = true;
            dataRender_backGroud.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()///背景点
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Color = BackInfo.PointColor,
                            Brightness= BackInfo.PointBrightness,
                            PointCounts = new PointVisibily[]
                            {
                                new PointVisibily(0),
                            }
                        },
                    },
                    Primitive = PrimitiveTopology.PointList,
                },
                new DataRenderConfig()
                {
                    PointConfigs =new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Brightness= BackInfo.CrossBrighness,
                            Color = BackInfo.CrossColor,
                            PointCounts = new PointVisibily[]{new PointVisibily(0) }
                        },//背景十字
                    },
                    Primitive =  PrimitiveTopology.LineList,
                }
            };
            dataRender_backGroud.CreateResources();
            (this as IRender).Children.Add(dataRender_backGroud);

            veldridText = new VeldridRender.TextRender.MutiText(control);
            veldridText.CreateResources();
            BackInfo.PropertyChanged += BackInfo_PropertyChanged;
            XAxis.PositionVerify = (po) =>
            {
                if (po == Position.Top || po == Position.Bottom) return po;
                else return Position.Bottom;
            };
            YAxis.PositionVerify = (po) =>
            {
                if (po == Position.Left || po == Position.Right) return po;
                else return Position.Left;
            };
            XAxis.PropertyChanged += XAxis_PropertyChanged;
            YAxis.PropertyChanged += YAxis_PropertyChanged;
            RefreshData();
            (this as IRender).Children.Add(veldridText);
        }
        private void RefreshData()
        {
            RefreshBackData();
            RefreshXAxisData();
            RefreshYAxisData();
        }
        private void YAxis_PropertyChanged(object sender, string e)
        {
            switch (e)
            {
                case nameof(LineAxisConfig.Visibily):
                    dataRender.DataRenderConfigs[0].PointConfigs[2].PointCounts[0].Visibily = YAxis.Visibily;
                    dataRender.DataRenderConfigs[0].PointConfigs[3].PointCounts[0].Visibily = YAxis.Visibily;
                    break;
                case nameof(LineAxisConfig.Step):
                    RefreshData();
                    break;
                case nameof(LineAxisConfig.OffsetValue):
                case nameof(LineAxisConfig.ShowGrid):
                    RefreshYMajorData();
                    break;
                case nameof(LineAxisConfig.LableStep):
                case nameof(LineAxisConfig.Unit):
                case nameof(LineAxisConfig.Decimal):
                    yAxisLables.ForEach(x =>
                    {
                        x.Str = YAxis.LabelFormatter != null ? YAxis.LabelFormatter(x.Value) : Tools.SiHelper.ValueChangeToSI(x.Value * YAxis.LableStep, out _, out _, YAxis.Decimal, YAxis.Unit);
                    });
                    break;
                case nameof(LineAxisConfig.MajorTick):
                    dataRender.DataRenderConfigs[0].PointConfigs[2].PointCounts[0].Visibily = YAxis.MinjorTick > 0;
                    RefreshCrossData();
                    RefreshYMajorData();
                    break;
                case nameof(LineAxisConfig.MinjorTick):
                case nameof(LineAxisConfig.MinjorTickCount):
                    RefeshYMinorData();
                    RefreshCrossData();
                    dataRender.DataRenderConfigs[0].PointConfigs[3].PointCounts[0].Visibily = YAxis.MinjorTick > 0;
                    break;
                case nameof(LineAxisConfig.Brightness):
                    dataRender.DataRenderConfigs[0].PointConfigs[2].Brightness = YAxis.Brightness;
                    dataRender.DataRenderConfigs[0].PointConfigs[3].Brightness = YAxis.Brightness;
                    break;
                case nameof(LineAxisConfig.Color):
                    dataRender.DataRenderConfigs[0].PointConfigs[2].Color = YAxis.Color;
                    dataRender.DataRenderConfigs[0].PointConfigs[3].Color = YAxis.Color;
                    break;
                case nameof(LineAxisConfig.PointOffset):
                case nameof(LineAxisConfig.Position):
                    RefreshYAxisData();
                    break;
                case nameof(LineAxisConfig.LabelFormatter):
                    yAxisLables.ForEach(x =>
                    {
                        x.Str = YAxis.LabelFormatter != null ? YAxis.LabelFormatter(x.Value) : x.Value.ToString();
                    });
                    break;
            }
            needupdate = true;
        }
        private void XAxis_PropertyChanged(object sender, string e)
        {
            switch (e)
            {
                case nameof(LineAxisConfig.Visibily):
                    dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].Visibily = XAxis.Visibily;
                    dataRender.DataRenderConfigs[0].PointConfigs[1].PointCounts[0].Visibily = XAxis.Visibily;
                    break;
                case nameof(LineAxisConfig.Step):
                    RefreshData();
                    break;
                case nameof(LineAxisConfig.LableStep):
                case nameof(LineAxisConfig.Unit):
                case nameof(LineAxisConfig.Decimal):
                    xAxisLables.ForEach(x =>
                    {
                        x.Str = XAxis.LabelFormatter != null ? XAxis.LabelFormatter(x.Value) : Tools.SiHelper.ValueChangeToSI(x.Value * XAxis.LableStep, out _, out _, XAxis.Decimal, XAxis.Unit);
                    });
                    break;
                case nameof(LineAxisConfig.MajorTick):
                    RefreshCrossData();
                    RefreshXMajorData();
                    dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].Visibily = XAxis.MajorTick > 0;
                    break;
                case nameof(LineAxisConfig.OffsetValue):
                case nameof(LineAxisConfig.ShowGrid):
                    RefreshXMajorData();
                    break;
                case nameof(LineAxisConfig.MinjorTick):
                case nameof(LineAxisConfig.MinjorTickCount):
                    RefeshXMinorData();
                    RefreshCrossData();
                    dataRender.DataRenderConfigs[0].PointConfigs[1].PointCounts[0].Visibily = XAxis.MinjorTick > 0;
                    break;
                case nameof(LineAxisConfig.Brightness):
                    dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = XAxis.Brightness;
                    dataRender.DataRenderConfigs[0].PointConfigs[1].Brightness = XAxis.Brightness;
                    break;
                case nameof(LineAxisConfig.Color):
                    dataRender.DataRenderConfigs[0].PointConfigs[0].Color = XAxis.Color;
                    dataRender.DataRenderConfigs[0].PointConfigs[1].Color = XAxis.Color;
                    break;
                case nameof(LineAxisConfig.PointOffset):
                case nameof(LineAxisConfig.Position):
                    RefreshXAxisData();
                    break;
                case nameof(LineAxisConfig.LabelFormatter):
                    xAxisLables.ForEach(x =>
                    {
                        x.Str = XAxis.LabelFormatter != null ? XAxis.LabelFormatter(x.Value) : x.Value.ToString();
                    });
                    break;
            }
            needupdate = true;
        }

        private void BackInfo_PropertyChanged(Object sender, String e)
        {
            switch (e)
            {
                case nameof(LineAxisBackInfo.CrossBrighness):
                    dataRender_backGroud.DataRenderConfigs[1].PointConfigs[0].Brightness = BackInfo.CrossBrighness;
                    break;
                case nameof(LineAxisBackInfo.CrossColor):
                    dataRender_backGroud.DataRenderConfigs[1].PointConfigs[0].Color = BackInfo.CrossColor;
                    break;
                case nameof(LineAxisBackInfo.PointBrightness):
                    dataRender_backGroud.DataRenderConfigs[0].PointConfigs[0].Brightness = BackInfo.PointBrightness;
                    break;
                case nameof(LineAxisBackInfo.PointColor):
                    dataRender_backGroud.DataRenderConfigs[0].PointConfigs[0].Color = BackInfo.PointColor;
                    break;
            }
            needupdate = true;
        }

        public override void Draw()
        {
            if (!Visibily) return;
            if (dataRender.WindowSizeState)
            {
                RefreshData();
                needupdate = true;
            }
            // dataRender_backGroud.Draw();// 先绘制背景点
            if (needupdate)
            {
                List<Vector2> tempdatas = new List<Vector2>();
                //tempdatas.AddRange(backPointDatas);
                //tempdatas.AddRange(backCrossDatas);
                tempdatas.AddRange(xAxisMajorDatas);
                tempdatas.AddRange(xAxisMinorDatas);
                tempdatas.AddRange(yAxisMajorDatas);
                tempdatas.AddRange(yAxisMinorDatas);
                dataRender.DataRenderConfigs[0].DataLenght = (uint)(tempdatas.Count);
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)(tempdatas.Count);
                dataRender.WriteData(0, tempdatas.ToArray());

                var tempdata_Background = new List<Vector2>();
                tempdata_Background.AddRange(backPointDatas);
                tempdata_Background.AddRange(backCrossDatas);

                dataRender_backGroud.DataRenderConfigs[0].DataLenght = (uint)backPointDatas.Count;
                dataRender_backGroud.DataRenderConfigs[0].FixedDataLenght = (uint)backPointDatas.Count;
                dataRender_backGroud.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].Visibily = BackInfo.PointVisibily;
                dataRender_backGroud.DataRenderConfigs[1].DataLenght = (uint)backCrossDatas.Count;
                dataRender_backGroud.DataRenderConfigs[1].FixedDataLenght = (uint)backCrossDatas.Count;
                dataRender_backGroud.DataRenderConfigs[1].PointConfigs[0].PointCounts[0].Visibily = BackInfo.CrossVisibily;
                dataRender_backGroud.WriteData(0, tempdata_Background.ToArray());
                needupdate = false;
            }
            List<TextInfo> temp = new List<TextInfo>();
            if (YAxis.LableVisibily && YAxis.Visibily && YAxis.Brightness > 0)
            {
                temp.AddRange(yAxisLables.Select(x =>
                {
                    TextInfo textInfo = new TextInfo();
                    textInfo.Text = x.Str;
                    textInfo.Rotation = 0;
                    textInfo.Color = YAxis.Color;
                    textInfo.HorizontalAlignment = x.Horizontal;
                    textInfo.VerticalAlignment = x.Vertical;
                    textInfo.BackColor = Color.Empty;
                    textInfo.Local = Unsafe.As<Vector2, PointF>(ref x.Position);
                    return textInfo;
                }));
            }
            if (XAxis.LableVisibily && XAxis.Visibily && XAxis.Brightness > 0)
            {
                temp.AddRange(xAxisLables.Select(x =>
                {
                    TextInfo textInfo = new TextInfo();
                    textInfo.Text = x.Str;
                    textInfo.Rotation = 0;
                    textInfo.Color = XAxis.Color;
                    textInfo.HorizontalAlignment = x.Horizontal;
                    textInfo.VerticalAlignment = x.Vertical;
                    textInfo.BackColor = Color.Empty;
                    textInfo.Local = Unsafe.As<Vector2, PointF>(ref x.Position);
                    return textInfo;
                }));
            }
            veldridText.TextInfos = temp.ToArray();
            base.Draw();
        }

        private void RefreshXAxisData()
        {
            RefreshXMajorData();
            RefeshXMinorData();
        }
        private void RefreshXMajorData()
        {
            xAxisMajorDatas.Clear();
            xAxisLables.Clear();
            float templableoffset = lableoffet * (this as IRender).Camera.AspectRatio * -1;
            float tick = XAxis.MajorTick * (this as IRender).Camera.AspectRatio;
            List<Vector2> points = new List<Vector2>();
            float ystartposition = LineRange.MinY;
            float yendposition = LineRange.MinY + tick;
            if (XAxis.Position == Position.Top)
            {
                templableoffset *= -1;
                ystartposition = LineRange.MaxY;
                yendposition = LineRange.MaxY - tick;
            }
            if (XAxis.ShowGrid)
            {
                points.Add(new Vector2(LineRange.MinX, ystartposition));
                points.Add(new Vector2(LineRange.MaxX, ystartposition));
            }
            float xoffset = XAxis.PointOffset;
            float minvalue = LineRange.MinX + xoffset % XAxis.Step;
            int majorcount = (int)(LineRange.XLenght / XAxis.Step) + 1;
            for (int i = 0; i < majorcount; i++)
            {
                float val = XAxis.Step * i + minvalue;
                if (val < LineRange.MinX || val > LineRange.MaxX) continue;
                points.Add(new Vector2(val, ystartposition));
                points.Add(new Vector2(val, yendposition));
                val = (val - xoffset) / XAxis.Step + XAxis.OffsetValue;
                xAxisLables.Add(new StringInfo()
                {
                    Str = XAxis.LabelFormatter != null ? XAxis.LabelFormatter(val) : Tools.SiHelper.ValueChangeToSI(val * XAxis.LableStep, out _, out _, XAxis.Decimal, XAxis.Unit),
                    Position = new Vector2(points[^1].X, points[^1].Y - templableoffset),
                    Value = val,
                    Vertical = XAxis.Position == Position.Top ? VerticalAlignment.Top : VerticalAlignment.Bottom,
                    Horizontal = HorizontalAlignment.Center,
                });
            }
            RemoveOverlapXLableStr(ref xAxisLables);
            xAxisMajorDatas.AddRange(points);
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].PointCount = (uint)points.Count;
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].FixedPointCount = (uint)points.Count;
        }

        /// <summary>
        /// 清除重叠X坐标标签
        /// </summary>
        /// <param name="lables"></param>
        private void RemoveOverlapXLableStr(ref List<StringInfo> lables)
        {
            List<float> xsizes = lables.Select(x =>
            {
                return veldridText.GetVirtualSize(x.Str).X;
            }).ToList();

            int num = lables.Where(x => !string.IsNullOrEmpty(x.Str)).Count();
            float Expansion = LineRange.XLenght / xsizes.Sum();
            if (Expansion > 1.2 || xsizes.Sum() == 0)
                return;
            //1.2以上为完整展示系数
            //1.2以下，0.6以上剔除一半标签

            if (Expansion < 1.2 && Expansion >= 0.6)
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 2 != 0)
                    {
                        lables[i].Str = string.Empty;
                    }
                }
                return;
            }

            if (Expansion < 0.6 && Expansion >= 0.3)
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 4 != 0)
                    {
                        lables[i].Str = string.Empty;
                    }
                }
                return;
            }

            if (Expansion < 0.3 && Expansion >= 0.1)
            {
                if ((xsizes[0] + xsizes[xsizes.Count - 1]) * 2 < LineRange.XLenght)
                {
                    for (int i = 1; i < num - 1; i++)
                    {
                        lables[i].Str = string.Empty;
                    }
                    return;
                }
                else if (xsizes[xsizes.Count / 2] * 1.2 < LineRange.XLenght)
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (i != xsizes.Count / 2)
                        {
                            lables[i].Str = string.Empty;
                        }
                    }
                    return;
                }

            }

            for (int i = 0; i < num; i++)
            {
                lables[i].Str = string.Empty;
            }
            return;
        }

        private void RemoveOverlapYLableStr(ref List<StringInfo> lables)
        {
            List<float> xsizes = lables.Select(x =>
            {
                return veldridText.GetVirtualSize(x.Str).X;
            }).ToList();

            List<float> ysizes = lables.Select(x =>
            {
                return veldridText.GetVirtualSize(x.Str).Y;
            }).ToList();

            if (xsizes.Any(x => x > 0.9 * LineRange.XLenght))
            {
                lables.ForEach(x => x.Str = string.Empty);
                return;
            }


            int num = lables.Where(x => !string.IsNullOrEmpty(x.Str)).Count();
            float Expansion = LineRange.YLenght / ysizes.Sum();
            if (Expansion > 1.2 || xsizes.Sum() == 0)
                return;
            //1.2以上为完整展示系数
            //1.2以下，0.6以上剔除一半标签

            if (Expansion < 1.2 && Expansion >= 0.6)
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 2 != 0)
                    {
                        lables[i].Str = string.Empty;
                    }
                }
                return;
            }

            if (Expansion < 0.6 && Expansion >= 0.3)
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 4 != 0)
                    {
                        lables[i].Str = string.Empty;
                    }
                }
                return;
            }

            if (Expansion < 0.3 && Expansion >= 0.1)
            {
                if ((xsizes[0] + xsizes[xsizes.Count - 1]) * 2 < LineRange.XLenght)
                {
                    for (int i = 1; i < num - 1; i++)
                    {
                        lables[i].Str = string.Empty;
                    }
                    return;
                }
                else if (xsizes[xsizes.Count / 2] * 1.2 < LineRange.XLenght)
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (i != xsizes.Count / 2)
                        {
                            lables[i].Str = string.Empty;
                        }
                    }
                    return;
                }

            }

            for (int i = 0; i < num; i++)
            {
                lables[i].Str = string.Empty;
            }
            return;
        }
        private void RefeshXMinorData()
        {
            xAxisMinorDatas.Clear();
            float tick = XAxis.MinjorTick * (this as IRender).Camera.AspectRatio;
            List<Vector2> points = new List<Vector2>();
            float ystartposition = LineRange.MaxY;
            float yendposition = LineRange.MaxY - tick;
            if (XAxis.Position == Position.Bottom)
            {
                ystartposition = LineRange.MinY;
                yendposition = LineRange.MinY + tick;
            }

            float xoffset = XAxis.PointOffset;
            float minvalue = LineRange.MinX + (xoffset % XAxis.Step - XAxis.Step);
            Int32 majorcount = (Int32)(LineRange.XLenght / XAxis.Step) + 2;

            if (XAxis.Type == LineAxisType.Linear)
            {
                for (Int32 i = 0; i < majorcount; i++)
                {
                    float val = XAxis.Step * i + minvalue;
                    for (Int32 j = 0; j < XAxis.MinjorTickCount; j++)
                    {
                        float temp = val + j * XAxis.Step / XAxis.MinjorTickCount;
                        if (temp < LineRange.MinX || temp > LineRange.MaxX) continue;
                        points.Add(new Vector2(temp, ystartposition));
                        points.Add(new Vector2(temp, yendposition));
                    }
                }
            }
            else
            {
                for (Int32 i = 0; i < majorcount; i++)
                {
                    float val = XAxis.Step * i + minvalue;
                    for (Int32 j = 0; j < XAxis.MinjorTickCount; j++)
                    {
                        float temp = (float)(val + Math.Log10(j + 1) * XAxis.Step);
                        if (temp < LineRange.MinX || temp > LineRange.MaxX) continue;
                        points.Add(new Vector2(temp, ystartposition));
                        points.Add(new Vector2(temp, yendposition));
                    }
                }
            }

            xAxisMinorDatas.AddRange(points);
            dataRender.DataRenderConfigs[0].PointConfigs[1].PointCounts[0].PointCount = (uint)points.Count;
            dataRender.DataRenderConfigs[0].PointConfigs[1].PointCounts[0].FixedPointCount = (uint)points.Count;
        }

        private void RefreshYAxisData()
        {
            RefreshYMajorData();
            RefeshYMinorData();
        }
        private void RefreshYMajorData()
        {
            float tick = YAxis.MajorTick;
            yAxisMajorDatas.Clear();
            yAxisLables.Clear();
            float templabeloffset = lableoffet;
            List<Vector2> points = new List<Vector2>();
            float xstartposition = LineRange.MinX;
            float xendposition = LineRange.MinX + tick;
            if (YAxis.Position == Position.Right)
            {
                templabeloffset *= -1;
                xstartposition = LineRange.MaxX;
                xendposition = LineRange.MaxX - tick;
            }
            if (YAxis.ShowGrid)
            {
                points.Add(new Vector2(xstartposition, LineRange.MinY));
                points.Add(new Vector2(xstartposition, LineRange.MaxY));
            }
            float yoffset = YAxis.PointOffset;
            float minvalue = LineRange.MinY + yoffset % YAxis.Step;
            int majorcount = (int)(LineRange.YLenght / YAxis.Step) + 1;
            for (int i = 0; i < majorcount; i++)
            {
                float val = YAxis.Step * i + minvalue;
                if (val < LineRange.MinY || val > LineRange.MaxY) continue;
                points.Add(new Vector2(xstartposition, val));
                points.Add(new Vector2(xendposition, val));
                if (YAxis.Type == LineAxisType.Linear)
                {
                    val = MathF.Round((val - yoffset) / YAxis.Step, 0) + YAxis.OffsetValue;
                    yAxisLables.Add(new StringInfo()
                    {
                        Position = new Vector2(points[^1].X + templabeloffset, points[^1].Y),
                        Value = val,
                        Vertical = VerticalAlignment.Center,
                        Horizontal = YAxis.Position == Position.Left ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                        Str = YAxis.LabelFormatter != null ? YAxis.LabelFormatter(val) : Tools.SiHelper.ValueChangeToSI(val * YAxis.LableStep, out _, out _, YAxis.Decimal, YAxis.Unit),
                    });
                }
                else
                {
                    val = MathF.Round((val - yoffset) / YAxis.Step, 0) + YAxis.OffsetValue;
                    yAxisLables.Add(new StringInfo()
                    {
                        Position = new Vector2(points[^1].X + templabeloffset, points[^1].Y),
                        Value = val,
                        Vertical = VerticalAlignment.Center,
                        Horizontal = YAxis.Position == Position.Left ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                        Str = YAxis.LabelFormatter != null ? YAxis.LabelFormatter(i) : Tools.SiHelper.ValueChangeToSI(val * YAxis.LableStep, out _, out _, YAxis.Decimal, YAxis.Unit),
                    });

                }
            }
            RemoveOverlapYLableStr(ref yAxisLables);
            yAxisMajorDatas.AddRange(points);
            dataRender.DataRenderConfigs[0].PointConfigs[2].PointCounts[0].PointCount = (uint)points.Count;
            dataRender.DataRenderConfigs[0].PointConfigs[2].PointCounts[0].FixedPointCount = (uint)points.Count;
        }
        private void RefeshYMinorData()
        {
            yAxisMinorDatas.Clear();
            float tick = YAxis.MinjorTick;
            List<Vector2> points = new List<Vector2>();
            float xstartposition = LineRange.MinX;
            float xendposition = LineRange.MinX + tick;
            if (YAxis.Position == Position.Right)
            {
                xstartposition = LineRange.MaxX;
                xendposition = LineRange.MaxX - tick;
            }
            float yoffset = YAxis.PointOffset;
            float minvalue = LineRange.MinY + yoffset % YAxis.Step - YAxis.Step;
            int majorcount = (int)(LineRange.YLenght / YAxis.Step) + 2;
            if (YAxis.Type == LineAxisType.Linear)
            {
                for (int i = 0; i < majorcount; i++)
                {
                    float val = YAxis.Step * i + minvalue;
                    for (int j = 0; j < YAxis.MinjorTickCount; j++)
                    {
                        float tempvalue = val + j * YAxis.Step / YAxis.MinjorTickCount;
                        if (tempvalue < LineRange.MinY || tempvalue > LineRange.MaxY) continue;
                        points.Add(new Vector2(xstartposition, tempvalue));
                        points.Add(new Vector2(xendposition, tempvalue));
                    }
                }
            }
            else
            {
                for (int i = 0; i < majorcount; i++)
                {
                    float val = YAxis.Step * i + minvalue;
                    for (int j = 0; j < YAxis.MinjorTickCount; j++)
                    {
                        float tempvalue = (float)(val + Math.Log10((j + 1) * YAxis.LableStep) * YAxis.Step);
                        if (tempvalue < LineRange.MinY || tempvalue > LineRange.MaxY) continue;
                        points.Add(new Vector2(xstartposition, tempvalue));
                        points.Add(new Vector2(xendposition, tempvalue));
                    }
                }
            }
            yAxisMinorDatas.AddRange(points);
            dataRender.DataRenderConfigs[0].PointConfigs[3].PointCounts[0].PointCount = (uint)points.Count;
            dataRender.DataRenderConfigs[0].PointConfigs[3].PointCounts[0].FixedPointCount = (uint)points.Count;
        }
        private void RefreshBackData()
        {
            RefreshCrossData();
            RefreshBackPointData();
        }
        /// <summary>
        /// 绘制背景虚线（点）
        /// </summary>
        private void RefreshBackPointData()
        {
            backPointDatas.Clear();
            List<Vector2> points = new List<Vector2>();
            int majorcount = (int)(LineRange.XLenght / XAxis.Step);
            float start = 0;
            float position = 0;
            int majorcount1 = (int)(LineRange.YLenght / YAxis.Step);
            if (XAxis.Type == LineAxisType.Linear)
            {
                for (int y = 1; y < majorcount1; y++)
                {
                    position = YAxis.Step * y + LineRange.MinY;
                    for (int i = 0; i < majorcount; i++)
                    {
                        start = i * XAxis.Step + LineRange.MinX;

                        for (int j = 0; j < XAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2(start + j * (XAxis.Step / XAxis.MinjorTickCount), position));
                        }
                    }
                }

                for (int y = 1; y < majorcount; y++)
                {
                    position = XAxis.Step * y + LineRange.MinX;
                    for (int i = 0; i < majorcount1; i++)
                    {
                        start = i * YAxis.Step + LineRange.MinY;

                        for (int j = 0; j < YAxis.MinjorTickCount; j++)
                        {
                            float ypos = 0.0f;
                            if (YAxis.Type == LineAxisType.Linear)
                            {
                                ypos = start + j * (YAxis.Step / YAxis.MinjorTickCount);
                            }
                            else if (YAxis.Type == LineAxisType.Log)
                            {
                                ypos = start + (float)Math.Log10((j + 1) * YAxis.LableStep) * YAxis.Step;
                            }
                            points.Add(new Vector2(position, ypos));
                        }
                    }
                }
            }
            else
            {
                for (int y = 1; y < majorcount1; y++)
                {
                    position = YAxis.Step * y + LineRange.MinY;
                    for (int i = 0; i < majorcount; i++)
                    {
                        start = i * XAxis.Step + LineRange.MinX;

                        for (int j = 0; j < XAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2((float)(start + Math.Log10(j + 1) * XAxis.Step), position));
                        }
                    }
                }

                for (int y = 1; y < majorcount; y++)
                {
                    position = XAxis.Step * y + LineRange.MinX;
                    for (int i = 0; i < majorcount1; i++)
                    {
                        start = i * YAxis.Step + LineRange.MinY;

                        for (int j = 0; j < YAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2(position, (float)(start + Math.Log10(j + 1) * YAxis.Step)));
                        }
                    }
                }
            }
            backPointDatas.AddRange(points);
            dataRender_backGroud.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].PointCount = (uint)points.Count;
            dataRender_backGroud.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].FixedPointCount = (uint)points.Count;
        }
        /// <summary>
        /// 绘制正十字交叉线
        /// </summary>
        private void RefreshCrossData()
        {
            backCrossDatas.Clear();
            List<Vector2> points = new List<Vector2>();
            if (XAxis.Type == LineAxisType.Linear)
            {
                points.Add(new Vector2(LineRange.MinX, (LineRange.MinY + LineRange.MaxY) / 2));
                points.Add(new Vector2(LineRange.MaxX, (LineRange.MinY + LineRange.MaxY) / 2));
                int majorcount = (int)(LineRange.XLenght / XAxis.Step);
                float start = 0;
                float end = 0;
                for (int i = 0; i < majorcount; i++)
                {
                    start = i * XAxis.Step + LineRange.MinX;
                    end = start + XAxis.Step;
                    if (XAxis.MinjorTick > 0)
                    {
                        for (int j = 0; j < XAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2(start + j * (XAxis.Step / XAxis.MinjorTickCount), (LineRange.MinY + LineRange.MaxY) / 2 - XAxis.MinjorTick * (this as IRender).Camera.AspectRatio));
                            points.Add(new Vector2(start + j * (XAxis.Step / XAxis.MinjorTickCount), (LineRange.MinY + LineRange.MaxY) / 2 + XAxis.MinjorTick * (this as IRender).Camera.AspectRatio));
                        }
                    }
                    points.Add(new Vector2(end, (LineRange.MinY + LineRange.MaxY) / 2 - XAxis.MajorTick * (this as IRender).Camera.AspectRatio));
                    points.Add(new Vector2(end, (LineRange.MinY + LineRange.MaxY) / 2 + XAxis.MajorTick * (this as IRender).Camera.AspectRatio));
                }

                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2, LineRange.MinY));
                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2, LineRange.MaxY));


                majorcount = (int)(LineRange.YLenght / YAxis.Step);
                start = 0;
                end = 0;
                for (int i = 0; i < majorcount; i++)
                {
                    start = i * YAxis.Step + LineRange.MinY;
                    end = start + YAxis.Step;
                    if (YAxis.MinjorTick > 0)
                    {
                        for (int j = 0; j < YAxis.MinjorTickCount; j++)
                        {
                            if (YAxis.Type == LineAxisType.Linear)
                            {
                                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 - YAxis.MinjorTick, start + j * (YAxis.Step / YAxis.MinjorTickCount)));
                                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 + YAxis.MinjorTick, start + j * (YAxis.Step / YAxis.MinjorTickCount)));
                            }
                            else if (YAxis.Type == LineAxisType.Log)
                            {
                                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 - YAxis.MinjorTick, start + (float)Math.Log10((YAxis.MinjorTickCount - j) * YAxis.LableStep) * YAxis.Step));
                                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 + YAxis.MinjorTick, start + (float)Math.Log10((YAxis.MinjorTickCount - j) * YAxis.LableStep) * YAxis.Step));
                            }
                        }
                    }

                    points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 - YAxis.MajorTick, end));
                    points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 + YAxis.MajorTick, end));
                }
            }
            else
            {
                points.Add(new Vector2(LineRange.MinX, (LineRange.MinY + LineRange.MaxY) / 2));
                points.Add(new Vector2(LineRange.MaxX, (LineRange.MinY + LineRange.MaxY) / 2));
                int majorcount = (int)(LineRange.XLenght / XAxis.Step);
                float start = 0;
                float end = 0;
                for (int i = 0; i <= majorcount; i++)
                {
                    start = i * XAxis.Step + LineRange.MinX;
                    end = start + XAxis.Step;
                    if (XAxis.MinjorTick > 0)
                    {
                        for (int j = 0; j < XAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2((float)(start + Math.Log10(j + 1) * XAxis.Step), (LineRange.MinY + LineRange.MaxY) / 2 - XAxis.MinjorTick * (this as IRender).Camera.AspectRatio));
                            points.Add(new Vector2((float)(start + Math.Log10(j + 1) * XAxis.Step), (LineRange.MinY + LineRange.MaxY) / 2 + XAxis.MinjorTick * (this as IRender).Camera.AspectRatio));
                        }
                    }
                    points.Add(new Vector2(end, (LineRange.MinY + LineRange.MaxY) / 2 - XAxis.MajorTick * (this as IRender).Camera.AspectRatio));
                    points.Add(new Vector2(end, (LineRange.MinY + LineRange.MaxY) / 2 + XAxis.MajorTick * (this as IRender).Camera.AspectRatio));
                }

                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2, LineRange.MinY));
                points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2, LineRange.MaxY));


                majorcount = (int)(LineRange.YLenght / YAxis.Step);
                start = 0;
                end = 0;
                for (int i = 0; i <= majorcount; i++)
                {
                    start = i * YAxis.Step + LineRange.MinY;
                    end = start + YAxis.Step;
                    if (YAxis.MinjorTick > 0)
                    {
                        for (int j = 0; j < YAxis.MinjorTickCount; j++)
                        {
                            points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 - YAxis.MinjorTick, (float)(start + Math.Log10(j + 1) * YAxis.Step)));
                            points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 + YAxis.MinjorTick, (float)(start + Math.Log10(j + 1) * YAxis.Step)));
                        }
                    }

                    points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 - YAxis.MajorTick, end));
                    points.Add(new Vector2((LineRange.MinX + LineRange.MaxX) / 2 + YAxis.MajorTick, end));
                }
            }

            backCrossDatas.AddRange(points);
            dataRender_backGroud.DataRenderConfigs[1].PointConfigs[0].PointCounts[0].PointCount = (uint)points.Count;
            dataRender_backGroud.DataRenderConfigs[1].PointConfigs[0].PointCounts[0].FixedPointCount = (uint)points.Count;
        }

        public LineAxisBackInfo BackInfo { get; } = new LineAxisBackInfo();
        public LineAxisConfig XAxis { get; } = new LineAxisConfig()
        {
            Position = Position.Bottom,
        };
        public LineAxisConfig YAxis { get; } = new LineAxisConfig()
        {
            Position = Position.Left,
        };
        private protected override BaseVeldridRender Renderer => dataRender;

        protected override void Dispose(bool disposing)
        {
            dataRender_backGroud.DisposeResources();
            backCrossDatas.Clear();
            backPointDatas.Clear();
            xAxisLables.Clear();
            yAxisLables.Clear();
            xAxisMajorDatas.Clear();
            xAxisMinorDatas.Clear();
            yAxisMajorDatas.Clear();
            yAxisMinorDatas.Clear();
            XAxis?.Dispose();
            YAxis?.Dispose();
            BackInfo?.Dispose();
            base.Dispose(disposing);
        }

    }
}
