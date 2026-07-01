using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Schema;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public sealed class DecodeLineConfig
    {
        private DecodeLineRender _Render;
        internal DecodeLineConfig(DecodeLineRender render)
        {
            _Render = render;
        }
        public Boolean Visibily { get => _Render.Visibily; set => _Render.Visibily = value; }
        public UInt32 ChannelCount { get => _Render.ChannelCount; set => _Render.ChannelCount = value; }
        public UInt32 PerChannelDataLength { get => _Render.PerChannelDataLength; set => _Render.PerChannelDataLength = value; }
        public Int32 InterwovenBitCount { get => _Render.InterwovenBitCount; set => _Render.InterwovenBitCount = value; }
        public float DataInterval { get => _Render.DataInterval; set => _Render.DataInterval = value; }
        public Int32 TriggerIndex { get => _Render.TriggerIndex; set => _Render.TriggerIndex = value; }
        public float Position { get => _Render.Position; set => _Render.Position = value; }
        public Int32 Chindex { get => _Render.Chindex; set => _Render.Chindex = value; }
        public void SetData<T>(ref T value, UInt32 sizeInBytes) where T : unmanaged => _Render.SetData(ref value, sizeInBytes);
    }
    public class DecodePlot :BasePlot
    {
        struct StringInfo
        {
            public String Text;
            public Color Color;
            public Vector2 Local;
            public StringInfo(String str, Color color, Vector2 local)
            {
                Text = str;
                Color = color;
                Local = local;
            }
        }
        private Boolean needupdate = true;
        private StringInfo _ZoomStr;
        private float horoffet = 5;
        private CursorPlot cursorPlot;
        private DecodeRender decodeRender;
        private DataRender dataRender;
        private MutiText veldridText;
        private MutiText labelText;
        private NoDataPlot noDataPlot;
        List<List<StringInfo>> _TextConfig = new List<List<StringInfo>>();
        private Single _MaxWidth = 0.8f;
        private DecodeLineRender decodeLineRender;
        private DataRender decodedataRender;


        private List<DecodeRender.DecodeInfo> _Decodeinfos = new List<DecodeRender.DecodeInfo>();
        private List<(String Name, HexagonDecodeData[] Data)> data = new List<(String Name, HexagonDecodeData[] Data)>();
        private float _Height = 200;
        private float _ZoomX = 1.0f;
        private float _ZoomY = 1.0f;
        private float _OffsetX = 0.0f;
        private float _OffsetY = 0.0f;

        public DecodePlot(IVeldridContent control) : base(control)
        {
            decodeRender = new DecodeRender(control, 5000);
            decodeRender.CreateResources();
            veldridText = new MutiText(control, true, false);
            veldridText.CreateResources();
            veldridText.FontStyle = "Semibold";
            labelText = new MutiText(control, true);
            labelText.Visibily = true;
            labelText.Local = new PointF(StartX + 168, VerticalOffset + 40);
            labelText.CreateResources();
            cursorPlot = new CursorPlot(control);
            cursorPlot.VerticalAlignment = VerticalAlignment.Center;
            cursorPlot.HorizontalAlignment = HorizontalAlignment.Left;
            cursorPlot.Local = VerticalOffset;
            cursors.Add(cursorPlot);
            dataRender = new DataRender(control, 17);
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    DataLenght = 9,
                    FixedDataLenght =9,
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Brightness= Brightness,
                            PointCounts=new PointVisibily[]{9}
                        }
                    },
                    Primitive = PrimitiveTopology.TriangleList,
                },
                new DataRenderConfig()
                {
                    DataLenght = 8,
                    FixedDataLenght =8,
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Brightness = Brightness,
                            PointCounts=new PointVisibily[]{ 8},
                        }
                    },
                    Primitive = PrimitiveTopology.LineStrip,
                },
            };
            dataRender.Visibily = false;
            dataRender.CreateResources();
            (this as IRender).Children.Add(veldridText);
            (this as IRender).Children.Add(labelText);
            (this as IRender).Children.Add(dataRender);
            Brightness = 100;
            StartX = LineRange.MinX;
            StopX = LineRange.MaxX;
            noDataPlot = new NoDataPlot(control);
            noDataPlot.Text = "No Decode";
            noDataPlot.Visibily = false;
            //decodeLineRender = new DecodeLineRender(control);
            //decodeLineRender.CreateResources();
            //(this as IRender).Children.Add(decodeLineRender);
            //DecodeLine = new DecodeLineConfig(decodeLineRender);
            decodedataRender = new DataRender(control);
            decodedataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            Color = Color.Green,
                            PointCounts = new PointVisibily[1]{ 0},
                        }
                    },
                    Primitive = PrimitiveTopology.LineStrip,
                }
            };
            decodedataRender.CreateResources();
            decodedataRender.Visibily = true;
            (this as IRender).Children.Add(decodedataRender);
        }
        public void SetDecodeData(Vector2[] data)
        {
            if (data.Length > 5000)
                return;
            decodedataRender.WriteData(0, data);
            decodedataRender.DataRenderConfigs[0].FixedDataLenght = (UInt16)data.Length;
            decodedataRender.DataRenderConfigs[0].DataLenght = (UInt16)data.Length;
            decodedataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].PointCount = (UInt16)data.Length;
            decodedataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].FixedPointCount = (UInt16)data.Length;
        }
        public DecodeLineConfig DecodeLine { get; set; }

        public Single StartX { get; set; }
        public Single StopX { get; set; }

        public Color LineColor
        {
            get => cursorPlot.Color;
            set
            {
                if (cursorPlot.Color != value)
                {
                    noDataPlot.Color = value;
                    cursorPlot.Color = value;
                    for (Int32 i = 0; i < labelText.TextInfos.Length; i++)
                    {
                        labelText.TextInfos[i].Color = value;
                    }
                    needupdate = true;
                }
            }
        }
        public override float Brightness
        {
            get => cursorPlot.Brightness;
            set
            {
                if (decodeRender.Brightness != value)
                {
                    cursorPlot.Brightness = value;
                    decodeRender.Brightness = value;
                    dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value * 0.2f;
                    dataRender.DataRenderConfigs[1].PointConfigs[0].Brightness = value;
                }
            }
        }
        private protected override BaseVeldridRender Renderer => decodeRender;
        public override String FontName { get => veldridText.FontName; set => veldridText.FontName = value; }
        public override String FontStyle { get => veldridText.FontStyle; set => veldridText.FontStyle = value; }
        public override float FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }
        public CursorImageCollection CursorImages => cursorPlot.CursorImages;
        public CursorLineStyle CursorLineStyle { get => cursorPlot.LineStyle; set => cursorPlot.LineStyle = value; }
        public int CursorIndex { get => cursorPlot.CursorIndex; set => cursorPlot.CursorIndex = value; }
        /// <summary>
        /// 当字符过长时显示的字符
        /// </summary>
        public String DefaultString { get; set; } = "...";
        private String _Label = "B1";
        /// <summary>
        /// 当显示为<see cref="DefaultString"/>仍然过长时显示的字符
        /// </summary>
        /// <summary>
        /// 当<see cref="DecodeDatas"/>的长度为0时显示的字符
        /// </summary>
        public String NoDataString { get => noDataPlot.Text; set => noDataPlot.Text = value; }
        public override String Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (Data.Count <= 1 && labelText.TextInfos.Length != 1)
                {
                    labelText.TextInfos = new TextInfo[]
                    {
                        new TextInfo()
                        {
                            BackColor = Color.Transparent,
                            Color = LineColor,
                            Text = value,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment= HorizontalAlignment.Left,
                            Local = new PointF(LineRange.MinX + LineRange.XLenght*(this.CursorImages[0].Width/this.Rectangle.Width), VerticalOffset + 40),
                        }
                    };
                }
                if (Data.Count > 1 && labelText.TextInfos.Length != Data.Count)
                {
                    labelText.TextInfos = Enumerable.Range(0, Data.Count).Select(x => new TextInfo()).ToArray();
                }
                if (Data.Count > 1)
                {
                    for (int i = 0; i < Data.Count; i++)
                    {
                        labelText.TextInfos[i].Color = LineColor;
                        labelText.TextInfos[i].Text = $"{value}-{Data[i].Name}";
                        labelText.TextInfos[i].VerticalAlignment = VerticalAlignment.Bottom;
                        labelText.TextInfos[i].HorizontalAlignment = HorizontalAlignment.Left;
                        labelText.TextInfos[i].Local = new PointF(LineRange.MinX + LineRange.XLenght * (this.CursorImages[0].Width / this.Rectangle.Width), VerticalOffset - i * 1000 + 40);
                    }
                }
                if (labelText.TextInfos.Length == 1)
                {
                    labelText.TextInfos[0].Text = value;
                    labelText.TextInfos[0].Local = new PointF(LineRange.MinX + LineRange.XLenght * (this.CursorImages[0].Width / this.Rectangle.Width), VerticalOffset + 40);
                }
            }
        }
        public String LabelFontName { get => labelText.FontName; set => labelText.FontName = value; }
        public String LabelFontStyle { get => labelText.FontStyle; set => labelText.FontName = value; }
        public float LabelFontSize { get => labelText.FontSize; set => labelText.FontSize = value; }
        public override Boolean LabelVisibility { get => labelText.Visibily; set => labelText.Visibily = value; }
        public float Height
        {
            get => _Height;
            set
            {
                if (_Height != value)
                {
                    decodeRender.Slop = this.VirtualSizeToLocalSize(value * 0.8f, 0).X;
                    _Height = value;
                    needupdate = true;
                }
            }
        }

        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            base.OnMouseMove(point, ref handle);
            if (handle)
                return;
            List<HexagonDecodeData> temp = new List<HexagonDecodeData>();
            lock (data)
            {
                temp = data.SelectMany(x => x.Data).ToList();
            }
            if (temp.Count == 0)
                return;
            Int32 index = temp.FindIndex(x =>
            {
                RectangleF rec = new RectangleF(x.Rectangle.X, x.Rectangle.Y + VerticalOffset, x.Rectangle.Width, x.Rectangle.Height);
                return rec.Contains(point);
            });
            if (index == -1)
                return;
            else
            {
                handle = true;
                SetCursor(Sdl2.SDL_SystemCursor.Hand);
            }
        }
        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            base.OnMouseDown(point, ref handle);
            if (handle)
                return;
            List<HexagonDecodeData> temp = new List<HexagonDecodeData>();
            lock (data)
            {
                temp = data.SelectMany(x => x.Data).ToList();
            }
            if (temp.Count == 0)
                return;
            Int32 index = temp.FindIndex(x =>
            {
                RectangleF rec = new RectangleF(x.Rectangle.X, x.Rectangle.Y + VerticalOffset, x.Rectangle.Width, x.Rectangle.Height);
                return rec.Contains(point);
            });
            if (index == -1)
            {
                lock (data)
                {
                    temp.ForEach(x => x.IsSelected = false);
                }
                needupdate = true;
                return;
            }
            else
            {
                lock (data)
                {
                    UpdateZoomData(temp[index]);
                    temp.ForEach(x => x.IsSelected = temp[index].Index == x.Index);
                }
                handle = true;
                SetCursor(Sdl2.SDL_SystemCursor.Hand);
            }
            needupdate = true;
        }

        private void UpdateZoomData(HexagonDecodeData hexagon)
        {
            Int32 lineNum = 1;
            _ZoomStr.Text = String.Empty;
            Single relativelyheight = GetRelativelyHeight(Height);
            String s = String.Empty;
            if (hexagon.IsInfoPacket)
            {
                if (!String.IsNullOrEmpty(hexagon.ShowStr))
                {
                    s = $"{hexagon.ShowStr}:{Encoding.Default.GetString(hexagon.Data)}";
                }
                else
                {
                    s = Encoding.Default.GetString(hexagon.Data);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(hexagon.Title))
                {
                    s = GetHexagonDecodeDataString(hexagon);
                }
                else
                {
                    s = hexagon.Title + ":" + GetHexagonDecodeDataString(hexagon);
                }
            }
            if (!String.IsNullOrEmpty(hexagon.ErrorInfo))
            {
                if(hexagon.ErrorInfoData!=null&&hexagon.ErrorInfoData.Length>0)
                {
                    s += "\r\n" + String.Format(hexagon.ErrorInfo, GetHexagonDecodeErrorInfoDataString(hexagon));
                }
                else
                {
                    s += "\r\n" + hexagon.ErrorInfo;
                }
                
                s = s.Trim();
            }
            if (String.IsNullOrEmpty(s))
                return;
            int linestrcount = 100;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < Math.Ceiling(s.Length / (Single)linestrcount); i++)
            {
                stringBuilder.AppendLine(s.Substring(i * linestrcount, Math.Min(linestrcount, s.Length - i * linestrcount)));
            }
            _ZoomStr.Text = stringBuilder.ToString().TrimEnd('\n', '\r');
            _ZoomStr.Color = hexagon.Color;
            float height = MathF.Ceiling(s.Length / (Single)linestrcount) * relativelyheight;
            var yoffset = GetRelativelyHeight(4);
            var size = this.LocalSizeToVirtualSize(veldridText.MeasureSize(_ZoomStr.Text));
            lineNum = _ZoomStr.Text.Split('\n').Length;
            if (size.Y < height)
                size.Y = height;
            size.X += 40;
            size.Y += 40 + yoffset;
            Boolean top = false;
            PointF point = new PointF();
            point.X = hexagon.Rectangle.Left + hexagon.Rectangle.Width / 2 - size.X / 2;
            point.Y = hexagon.Rectangle.Y - yoffset - size.Y + VerticalOffset;
            if (point.Y <= LineRange.MinY + 50)
            {
                point.Y = hexagon.Rectangle.Y + height + yoffset + VerticalOffset;
                top = true;
            }
            if (point.Y + size.Y >= LineRange.MaxY - 50)
            {
                point.Y = LineRange.MaxY - 50 - size.Y;
            }
            if (point.X <= StartX + 50)
            {
                point.X = StartX + 50;
            }
            if (point.X + size.X >= StopX - 50)
            {
                point.X = StopX - 50 - size.X;
            }
            Vector2[] points = new Vector2[17];
            if (top)
            {
                if (lineNum <= 0)
                {
                    _ZoomStr.Local = new Vector2(point.X + size.X / 2, point.Y + (size.Y + yoffset) / 2);
                }
                else
                {
                    _ZoomStr.Local = new Vector2(point.X + size.X / 2, point.Y + (size.Y / lineNum + yoffset * (lineNum - 1)) / 2);
                }
                points[0] = new Vector2(hexagon.Rectangle.X + hexagon.Rectangle.Width / 2, hexagon.Rectangle.Y + hexagon.Rectangle.Height + VerticalOffset);
                points[1] = new Vector2(point.X + size.X / 2 - 40, point.Y + yoffset);
                points[2] = new Vector2(point.X + size.X / 2 + 40, point.Y + yoffset);

                points[3] = new Vector2(point.X, point.Y + yoffset);
                points[4] = new Vector2(point.X, point.Y + size.Y);
                points[5] = new Vector2(point.X + size.X, point.Y + yoffset);

                points[6] = points[5];
                points[7] = points[4];
                points[8] = new Vector2(point.X + size.X, point.Y + size.Y);

                points[9] = points[0];
                points[10] = points[2];
                points[11] = points[5];
                points[12] = points[8];
                points[13] = points[7];
                points[14] = points[3];
                points[15] = points[1];
                points[16] = points[0];
            }
            else
            {
                if (lineNum <= 0)
                {
                    _ZoomStr.Local = new Vector2(point.X + size.X / 2, point.Y + (size.Y - yoffset) / 2);
                }
                else
                {
                    _ZoomStr.Local = new Vector2(point.X + size.X / 2, point.Y + (size.Y / lineNum - yoffset * (lineNum - 1)) / 2);
                }
                points[0] = Unsafe.As<PointF, Vector2>(ref point);
                points[2] = new Vector2(point.X + size.X, point.Y);
                points[1] = new Vector2(point.X, point.Y + size.Y - yoffset);

                points[3] = new Vector2(point.X + size.X, point.Y);
                points[5] = new Vector2(point.X + size.X, point.Y + size.Y - yoffset);
                points[4] = new Vector2(point.X, point.Y + size.Y - yoffset);

                points[6] = new Vector2(point.X + size.X / 2 - 40, point.Y + size.Y - yoffset);
                points[8] = new Vector2(point.X + size.X / 2 + 40, point.Y + size.Y - yoffset);
                points[7] = new Vector2(hexagon.Rectangle.X + hexagon.Rectangle.Width / 2, hexagon.Rectangle.Y + VerticalOffset);

                points[9] = points[0];
                points[10] = points[2];
                points[11] = points[5];
                points[12] = points[8];
                points[13] = points[7];
                points[14] = points[6];
                points[15] = points[4];
                points[16] = points[0];
            }
            dataRender.WriteData(0, points);
            dataRender.DataRenderConfigs[0].PointConfigs[0].Color = hexagon.BorderColor;
            dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = Brightness * 0.2f;
            dataRender.DataRenderConfigs[1].PointConfigs[0].Color = hexagon.BorderColor;
            dataRender.DataRenderConfigs[1].PointConfigs[0].Brightness = Brightness;
        }

        protected override void Dispose(bool disposing)
        {
            noDataPlot.Dispose();
            _TextConfig.Clear();
            base.Dispose(disposing);
        }
        public override void Draw()
        {
            if (!Visibily)
                return;
            if (dataRender.WindowSizeState)
            {
                needupdate = true;
            }
            //labelText.Local = new PointF(cursorPlot.GetVirtualSize().X + 4, (LineRange.MinY + LineRange.MaxY) / 2 + VerticalOffset + 4);
            if (needupdate)
            {
                lock (data)
                {
                    RefreshRenderData();
                    var res = data.SelectMany(x => x.Data).Where(x => x.IsSelected && x.Visiliby).ToList();
                    if (res.Count > 0)
                    {
                        UpdateZoomData(res[0]);
                    }
                    dataRender.Visibily = res.Count > 0;
                    needupdate = false;
                }
            }
            else
            {
                lock (data)
                {
                    dataRender.Visibily = data.SelectMany(x => x.Data).Any(x => x.IsSelected && x.Visiliby);
                }
            }
            //noDataPlot.Visibily = !dataRender.Visibily;
            if (noDataPlot.Visibily)
            {
                noDataPlot.Draw();
            }
            if (dataRender.Visibily && !String.IsNullOrEmpty(_ZoomStr.Text))
            {
                List<StringInfo> stringInfos = new List<StringInfo>();
                StringInfo stringInfo = _ZoomStr;
                stringInfos.AddRange(Enumerable.Range(0, _TextConfig.Count).SelectMany(x =>
                {
                    return _TextConfig[x].Select(y =>
                    {
                        return new StringInfo
                        {
                            Color = y.Color,
                            Text = y.Text,
                            Local = new Vector2(y.Local.X, VerticalOffset - x * 1000),
                        };
                    });
                }));
                stringInfos.Add(stringInfo);
                veldridText.TextInfos = stringInfos.Select(x => new TextInfo()
                {
                    Color = x.Color,
                    Text = x.Text,
                    Local = Unsafe.As<Vector2, PointF>(ref x.Local),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Visibily = true,
                }).ToArray();
            }
            else
            {
                veldridText.TextInfos = _TextConfig.SelectMany(x => x).Select(x => new TextInfo()
                {
                    Color = x.Color,
                    Text = x.Text,
                    Local = new PointF(x.Local.X, x.Local.Y),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Visibily = true,
                }).ToArray();
            }
            base.Draw();
            if (dataRender.WindowSizeState)
            {
                dataRender.WindowSizeState = false;
            }
        }
        private void RefreshRenderData()
        {
            _Decodeinfos.Clear();
            _TextConfig.Clear();
            noDataPlot.Visibily = Data.Count == 0;
            for (Int32 index = 0; index < Data.Count; index++)
            {
                List<DecodeRender.DecodeInfo> decodeInfos = new List<DecodeRender.DecodeInfo>();
                List<StringInfo> stringInfos = new List<StringInfo>();
                RefreshRenderData(Data[index].Data, ref decodeInfos, ref stringInfos, (Single)(((decimal)LineRange.MaxY + (decimal)LineRange.MinY) / (decimal)2 - (decimal)1000 * (decimal)index));

                _Decodeinfos.AddRange(decodeInfos.ToArray());
                _TextConfig.Add(stringInfos);
            }
            decodeRender.UpdateDecodeinfos(_Decodeinfos.OrderBy(x => x.Polygon).ToArray());
        }
        private void RefreshRenderData(HexagonDecodeData[] datas,
            ref List<DecodeRender.DecodeInfo> decodeInfos,
            ref List<StringInfo> stringInfos,
            Single position = 0)
        {
            decodeInfos = new List<DecodeRender.DecodeInfo>();
            Single relativelyheight = GetRelativelyHeight(Height);
            var minsize = decodeRender.PerPixelSize;

            System.Numerics.Vector2 start = new System.Numerics.Vector2(StartX, position);
            System.Numerics.Vector2 end = start;

            noDataPlot.Visibily = datas.Length == 0;
            if (datas.Length == 0)
            {
            }
            else
            {
                var longsize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(DefaultString));
                var strsize = Vector2.Zero;
                Vector2 infoLocal = new Vector2(0, position + VerticalOffset);
                for (Int32 index = 0; index < datas.Length; index++)
                {
                    if (datas[index].XStart > StopX || datas[index].XStart + datas[index].XLenght < StartX)
                        continue;
                    end.X = datas[index].XStart;
                    if (end.X - start.X > 0)
                    {
                        decodeInfos.Add(new DecodeRender.DecodeInfo()
                        {
                            Color = LineColor.ColorConverToRGBA(),
                            Polygon = (Single)DecodeRender.Polygon.Line,
                            Position = start,
                            Size = new Vector2(end.X - start.X, 0),
                        });
                    }
                    HexagonDecodeData hexdata = datas[index];
                    if (hexdata.XLenght + hexdata.XStart < StartX || hexdata.XStart > StopX)
                    {
                        hexdata.Visiliby = false;
                        continue;
                    }
                    hexdata.Visiliby = true;
                    String str = String.Empty;
                    double reclength = hexdata.XLenght;//可显示区域长度
                    if (hexdata.XStart < StartX && 200 < hexdata.XLenght)
                    {
                        reclength = hexdata.XLenght + hexdata.XStart - 200;
                    }
                    else if (hexdata.XStart + hexdata.XLenght > StopX)
                    {
                        reclength = StopX - hexdata.XStart;
                    }
                    if (hexdata.IsInfoPacket)
                    {
                        if(!String.IsNullOrEmpty(hexdata.ShowStr))
                        {
                            str = hexdata.ShowStr;
                        }
                        else
                        {
                            str = Encoding.Default.GetString(hexdata.Data);
                        }
                        strsize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(str));
                        if (strsize.X >= reclength * _MaxWidth)
                        {
                            str = GetShortString(str);
                        }

                    }
                    else
                    {

                        String hexstr = GetHexagonDecodeDataString(hexdata);
                        str = hexstr;
                        if (String.IsNullOrEmpty(hexdata.Title))
                        {
                        }
                        else
                        {
                            str = hexdata.Title + ":" + hexstr;
                        }
                        strsize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(str));
                        if (strsize.X >= reclength * _MaxWidth)
                        {
                            if (String.IsNullOrEmpty(hexdata.Title))
                            {
                                str = hexstr;
                            }
                            else
                            {
                                str = GetShortString(hexdata.Title) + ":" + hexstr;
                            }
                        }
                        strsize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(str));
                        if (strsize.X >= reclength * _MaxWidth)
                        {
                            str = hexstr;
                        }
                    }
                    strsize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(str));
                    Boolean ishex = reclength >= Height * 2.2f;
                    if (strsize.X >= reclength * _MaxWidth)
                    {
                        //ishex = false;
                        if (longsize.X < reclength * _MaxWidth)
                        {
                            str = DefaultString;
                            strsize = longsize;
                        }
                        else
                        {
                            str = String.Empty;
                        }
                    }
                    if (String.IsNullOrEmpty(str))
                    {
                        if (hexdata.BorderColor.A > 0)
                        {
                            if (ishex)
                            {
                                if (hexdata.IsSelected)
                                {
                                    decodeInfos.Add(new DecodeRender.DecodeInfo()
                                    {
                                        Color = Color.FromArgb(60, hexdata.BorderColor).ColorConverToRGBA(),
                                        Polygon = (Single)(DecodeRender.Polygon.HexagonFill),
                                        Size = new Vector2(Math.Max(minsize.X, (Single)hexdata.XLenght), relativelyheight),
                                        Position = new Vector2((Single)hexdata.XStart, start.Y),
                                    });
                                }
                                decodeInfos.Add(new DecodeRender.DecodeInfo()
                                {
                                    Color = hexdata.BorderColor.ColorConverToRGBA(),
                                    Polygon = (Single)(DecodeRender.Polygon.Hexagon),
                                    Size = new Vector2((Single)hexdata.XLenght, relativelyheight),
                                    Position = new Vector2((Single)hexdata.XStart, start.Y),
                                });
                            }
                            else
                            {
                                decodeInfos.Add(new DecodeRender.DecodeInfo()
                                {
                                    Color = hexdata.BorderColor.ColorConverToRGBA(),
                                    Polygon = (Single)(DecodeRender.Polygon.RectangleFill),
                                    Size = new Vector2(Math.Max(minsize.X, (Single)hexdata.XLenght), relativelyheight),
                                    Position = new Vector2((Single)hexdata.XStart, start.Y),
                                });
                            }
                        }
                    }
                    else
                    {
                        if (hexdata.IsSelected)
                        {
                            if (hexdata.BorderColor.A > 0)
                            {
                                decodeInfos.Add(new DecodeRender.DecodeInfo()
                                {
                                    Color = Color.FromArgb(60, hexdata.BorderColor).ColorConverToRGBA(),
                                    Polygon = (Single)(ishex ? DecodeRender.Polygon.HexagonFill : DecodeRender.Polygon.RectangleFill),
                                    Size = new Vector2((Single)hexdata.XLenght, relativelyheight),
                                    Position = new Vector2((Single)hexdata.XStart, start.Y),
                                });
                            }
                            if (hexdata.BorderColor.A > 0)
                            {
                                decodeInfos.Add(new DecodeRender.DecodeInfo()
                                {
                                    Color = hexdata.BorderColor.ColorConverToRGBA(),
                                    Polygon = (Single)(ishex ? DecodeRender.Polygon.Hexagon : DecodeRender.Polygon.Rectangle),
                                    Size = new Vector2((Single)hexdata.XLenght, relativelyheight),
                                    Position = new Vector2((Single)hexdata.XStart, start.Y),
                                });

                            }
                        }
                        else
                        {
                            if (hexdata.BorderColor.A > 0)
                            {
                                decodeInfos.Add(new DecodeRender.DecodeInfo()
                                {
                                    Color = hexdata.BorderColor.ColorConverToRGBA(),
                                    Polygon = (Single)(ishex ? DecodeRender.Polygon.Hexagon : DecodeRender.Polygon.Rectangle),
                                    Size = new Vector2((Single)hexdata.XLenght, relativelyheight),
                                    Position = new Vector2((Single)hexdata.XStart, start.Y),
                                });

                            }
                        }

                        infoLocal.X = hexdata.XStart + ((Single)hexdata.XLenght) / 2f;
                        if (!String.IsNullOrEmpty(str))
                        {
                            if (infoLocal.X - strsize.X / 2.0 - 200 < StartX)//如果起始点在屏幕外显示，需要显示在屏幕内
                            {
                                //如果屏幕内长度足够，则显示全部
                                if (strsize.X <= reclength && 200 < hexdata.XLenght)
                                {
                                    infoLocal.X = (float)(StartX + strsize.X / 2.0 + 200);
                                }
                                //如果屏幕外长度不够，则显示shortstring
                                else if (longsize.X <= reclength + 200 && 200 < hexdata.XLenght)
                                {
                                    str = DefaultString;
                                    infoLocal.X = (float)(StartX + longsize.X / 2.0 + 200);
                                }
                            }
                            if ((infoLocal.X + strsize.X / 2.0) > StopX)
                            {
                                if (reclength >= strsize.X)
                                {
                                    infoLocal.X = (float)(StopX - strsize.X / 2.0);
                                }
                                //如果屏幕外长度不够，则显示shortstring
                                else if (reclength >= longsize.X)
                                {
                                    str = DefaultString;
                                    infoLocal.X = (float)(float)(StopX - longsize.X / 2.0);
                                }
                            }
                        }

                        stringInfos.Add(new StringInfo(str, hexdata.Color, infoLocal));
                    }

                    hexdata.rec = new RectangleF(new PointF(hexdata.XStart, start.Y - relativelyheight / 2), new SizeF((Single)hexdata.XLenght, relativelyheight));
                    start.X = (Single)(hexdata.XStart + hexdata.XLenght);
                }
                if (end.X < StopX)
                {
                    end.X = StopX;
                    decodeInfos.Add(new DecodeRender.DecodeInfo()
                    {
                        Color = LineColor.ColorConverToRGBA(),
                        Polygon = (Single)DecodeRender.Polygon.Line,
                        Position = start,
                        Size = new Vector2(end.X - start.X, 0),
                    });
                }
            }
        }
        private String GetShortString(String str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            return new String(str.Split(' ').Where(x => !String.IsNullOrEmpty(x.Trim())).Select(x => x[0]).ToArray()).ToUpper();
        }
        private String GetHexagonDecodeDataString(HexagonDecodeData hexagon)
        {
            if (hexagon.DataFormat != null)
                return hexagon.DataFormat.Invoke(hexagon.Data, hexagon.BitCount);
            String tempstr = "";
            switch (hexagon.DisPlayFormat)
            {
                case DisPlayFormat.ASCII:
                    tempstr = GetASCIIString(hexagon.Data);
                    break;
                case DisPlayFormat.Bin:
                    tempstr = GetBinString(hexagon.Data, hexagon.BitCount);
                    break;
                case DisPlayFormat.Dec:
                    tempstr = GetDecString(hexagon.Data, hexagon.BitCount);
                    break;
                case DisPlayFormat.Hex:
                    tempstr = GetHexString(hexagon.Data, hexagon.BitCount);
                    break;
            }

            return tempstr;
        }

        private String GetHexagonDecodeErrorInfoDataString(HexagonDecodeData hexagon)
        {
            if (hexagon.DataFormat != null)
                return hexagon.DataFormat.Invoke(hexagon.ErrorInfoData, hexagon.ErrorInfoBitCount);
            String tempstr = "";
            switch (hexagon.DisPlayFormat)
            {
                case DisPlayFormat.ASCII:
                    tempstr = GetASCIIString(hexagon.ErrorInfoData);
                    break;
                case DisPlayFormat.Bin:
                    tempstr = GetBinString(hexagon.ErrorInfoData, hexagon.ErrorInfoBitCount);
                    break;
                case DisPlayFormat.Dec:
                    tempstr = GetDecString(hexagon.ErrorInfoData, hexagon.ErrorInfoBitCount);
                    break;
                case DisPlayFormat.Hex:
                    tempstr = GetHexString(hexagon.ErrorInfoData, hexagon.ErrorInfoBitCount);
                    break;
            }

            return tempstr;
        }

        private String GetBinString(Byte[] data, UInt32 bitcount)
        {
            String temp = "";
            Int32 bytecount = (Int32)Math.Ceiling(bitcount / 8f);
            List<Byte> tempbytes = new List<Byte>();
            if (data.Length < bytecount)
            {
                tempbytes.AddRange(Enumerable.Repeat<Byte>(0, bytecount - data.Length));
            }
            else
            {

            }
            tempbytes.AddRange(data);

            for (Int32 i = 0; i < bytecount; i++)
            {
                Byte lenght = 0;
                if (i == 0)
                {
                    lenght = (Byte)(bitcount % 8);
                }
                else
                {
                    lenght = 8;
                }
                if (lenght == 0)
                {
                    lenght = 8;
                }
                Byte tempvalue = (Byte)(tempbytes[i] & (Byte)(Math.Pow(2, lenght) - 1));
                temp += Convert.ToString(tempvalue, 2).PadLeft(lenght, '0');
            }
            return temp + "b";
        }

        private float _OriginVerticalOffset = 0.0f;
        public override float VerticalOffset
        {
            get
            {
                var temp = _OriginVerticalOffset;
                if (_ZoomY != 0.0)
                {
                    temp = (temp - _OffsetY) / _ZoomY;
                    if (decodeRender != null)
                    {
                        decodeRender.VerticalOffset = temp;
                    }
                    if (cursorPlot != null)
                    {
                        cursorPlot.Local = temp;
                    }
                    if(noDataPlot!=null)
                    {
                        noDataPlot.ZoomY = _ZoomY;
                        noDataPlot.Local = temp;
                    }
                }
                return temp;
            }
            set
            {
                if (_OriginVerticalOffset!= value)
                {
                    noDataPlot.Local = value;
                    if (decodeRender.VerticalOffset != value)
                    {
                        Single offset = decodeRender.VerticalOffset - value;
                        _TextConfig.ForEach(x =>
                        {
                            x.ForEach(y =>
                            {
                                y.Local = new Vector2(y.Local.X, y.Local.Y + value);
                            });
                        });
                        decodeRender.VerticalOffset = value;
                        _OriginVerticalOffset = value;
                        cursorPlot.Local = value;
                        for (int i = 0; i < labelText.TextInfos.Length; i++)
                        {
                            labelText.TextInfos[i].Local = new PointF(LineRange.MinX + 168, value - i * 1000 + 40);
                        }
                        lock (data)
                        {
                            var res = data.SelectMany(x => x.Data).Where(x => x.IsSelected).ToArray();
                            if (res.Length > 0)
                            {
                                UpdateZoomData(res[0]);
                            }
                        }
                        needupdate = true; 
                    }
                }
            }
        }

        public float ZoomX
        {
            get => _ZoomX;
            set
            {
                if (_ZoomX != value)
                {
                    _ZoomX = value;
                }
            }
        }
        public float ZoomY
        {
            get => _ZoomY;
            set
            {
                if (_ZoomY != value)
                {
                    _ZoomY = value;
                    needupdate = true;
                }
            }
        }
        public float OffsetX
        {
            get => _OffsetX;
            set
            {
                if (_OffsetX != value)
                {
                    _OffsetX = value;
                }
            }
        }
        public float OffsetY
        {
            get => _OffsetY;
            set
            {
                if (_OffsetY != value)
                {
                    _OffsetY = value;
                    needupdate = true;
                }
            }
        }
        private Single GetRelativelyHeight(Single val)
        {
            return val / Rectangle.Height * LineRange.YLenght;
        }
        private String GetDecString(Byte[] data, UInt32 bitcount)
        {
            StringBuilder decimalStringBuilder = new StringBuilder();
            foreach (Byte b in data)
            {
                decimalStringBuilder.Append(b.ToString());
                decimalStringBuilder.Append(" "); // 添加空格分隔每个字节
            }
            return decimalStringBuilder.ToString().Trim();
        }
        private String GetHexString(Byte[] data, UInt32 bitcount)
        {
            String temp = "";
            Int32 bytecount = (Int32)Math.Ceiling(bitcount / 8f);
            List<Byte> tempbytes = new List<Byte>();
            if (data.Length < bytecount)
            {
                tempbytes.AddRange(Enumerable.Repeat<Byte>(0, bytecount - data.Length));
            }
            else
            {

            }
            tempbytes.AddRange(data);
            for (Int32 i = 0; i < bytecount; i++)
            {
                Byte lenght = 0;
                if (i == 0)
                {
                    lenght = (Byte)(bitcount % 8);
                }
                else
                {
                    lenght = 7;
                }
                if (lenght == 0)
                {
                    lenght = 7;
                }
                Byte tempvalue = (Byte)(tempbytes[i] & (Byte)(Math.Pow(2, lenght + 1) - 1));
                temp += Convert.ToString(tempvalue, 16).PadLeft((Int32)Math.Ceiling(lenght / 4d), '0');
            }
            return temp.ToUpper() + "h";
        }
        private String GetASCIIString(Byte[] data)
        {
            return string.Join(' ', data.Select(x => (Int32)x switch
            {
                0 => "NUL",
                1 => "SOH",
                2 => "STX",
                3 => "ETX",
                4 => "EOT",
                5 => "ENQ",
                6 => "ACK",
                7 => "BEL",
                8 => "BS",
                9 => "HT",
                10 => "LF",
                11 => "VT",
                12 => "FF",
                13 => "CR",
                14 => "SO",
                15 => "SI",
                16 => "DLE",
                17 => "DC1",
                18 => "DC2",
                19 => "DC3",
                20 => "DC4",
                21 => "NAK",
                22 => "SYN",
                23 => "ETB",
                24 => "CAN",
                25 => "EM",
                26 => "SUB",
                27 => "ESC",
                28 => "FS",
                29 => "GS",
                30 => "RS",
                31 => "US",
                32 => "SP",
                int ch when (ch >= 33 && ch <= 126) => System.Text.Encoding.ASCII.GetString(new byte[1] { (byte)ch }),
                127 => "DEL",
                _ => Encoding.ASCII.GetString(new byte[1] { x }),
            }));
        }
        public List<(String Name, HexagonDecodeData[] Data)> Data
        {
            get => data;
            set
            {
                lock (data)
                {
                    if (value == null || value.Count == 0)
                    {
                        data = new List<(String Name, HexagonDecodeData[] Data)>();
                        Label = _Label;
                        needupdate = true;
                        return;
                    }

                    if (value.Count != data.Count)
                    {
                        data = value;
                        Label = _Label;
                        needupdate = true;
                        return;

                    }
                    if (value == null && data.Count != 0)
                    {
                        data = new List<(String Name, HexagonDecodeData[] Data)>();
                        needupdate = true;
                        Label = _Label;
                        return;
                    }
                    if (value.Count != data.Count)
                    {
                        data = value;
                        Label = _Label;
                        needupdate = true;
                    }
                    else
                    {
                        Boolean result = false;
                        for (Int32 index = 0; index < data.Count; index++)
                        {
                            if (!data[index].Data.Equals<HexagonDecodeData>(value[index].Data))
                            {
                                needupdate = true;
                                result = true;
                                break;
                            }
                        }
                        if (result)
                        {
                            Int32 selectedindex = data.SelectMany(x => x.Data).ToList().FindIndex(x => x.IsSelected);

                            data = value;
                            if (selectedindex >= 0)
                            {
                                Int32 totalcount = 0;
                                for (Int32 index = 0; index < data.Count; index++)
                                {
                                    totalcount += data[index].Data.Length;
                                    if (totalcount > selectedindex)
                                    {
                                        data[index].Data[selectedindex - (totalcount - data[index].Data.Length)].IsSelected = true;
                                        break;
                                    }
                                }
                            }
                        }
                        Label = _Label;
                    }

                }

            }
        }

        /// <summary>
        /// 显示格式
        /// </summary>
        public enum DisPlayFormat
        {
            /// <summary>
            /// 16进制
            /// </summary>
            Hex,
            /// <summary>
            /// 十进制
            /// </summary>
            Dec,
            /// <summary>
            /// 2进制
            /// </summary>
            Bin,
            /// <summary>
            /// ASCII码
            /// </summary>
            ASCII,
        }
        public sealed class HexagonDecodeData
        {
            internal Boolean Visiliby { get; set; } = false;
            public Int32 Index { get; set; } = 0;
            internal RectangleF rec;

            public HexagonDecodeData()
            {
            }

            /// <summary>
            /// 颜色
            /// </summary>
            public Color Color { get; set; } = Color.White;
            /// <summary>
            /// 起始位置
            /// </summary>
            public float XStart { get; set; } = 0;
            /// <summary>
            /// 图形在波形图中的位置
            /// </summary>
            public RectangleF Rectangle => rec;
            internal Boolean IsSelected { get; set; } = false;
            /// <summary>
            /// 数据
            /// </summary>
            public Byte[] Data { get; set; } = new Byte[0];
            /// <summary>
            /// 数据格式化回调函数
            /// </summary>
            [AllowNull]
            public Func<Byte[], UInt32, String> DataFormat { get; set; }
            /// <summary>
            /// 图形长度
            /// </summary>
            public Double XLenght { get; set; } = 1f;
            /// <summary>
            /// 数据显示格式
            /// </summary>
            public DisPlayFormat DisPlayFormat { get; set; } = DisPlayFormat.Hex;

            /// <summary>
            /// 是否为信息包，当为信息包时，<see cref="DisPlayFormat"/>参数将无效,
            /// 控件会将<see cref="Data"/>中数据按照gb2312编码的方式直接显示为字符
            /// 在此模式下时<see cref="BitCount"/>、<see cref="DisPlayFormat"/>、<see cref="Title"/>参数无效
            /// </summary>
            public Boolean IsInfoPacket { get; set; } = false;
            public UInt32 BitCount { get; set; } = 8;
            public String Title { get; set; } = String.Empty;
            public String ShowStr { get; set; } = String.Empty;

            public Color BorderColor { get; set; } = Color.Transparent;
            public String ErrorInfo { get; set; } = String.Empty;

            public Byte[] ErrorInfoData { get; set; } = new Byte[0];
            public UInt32 ErrorInfoBitCount { get; set; } = 8;

            private Boolean EqualsBytes(Byte[] first, Byte[] second)
            {
                if (first == null && second == null)
                {
                    return true;
                }
                if (first == null || second == null)
                {
                    return false;
                }
                if (first.Length != second.Length)
                {
                    return false;
                }
                for (Int32 i = 0; i < first.Length; i++)
                {
                    if (first[i] != second[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            public override Boolean Equals(Object obj)
            {
                if (obj is HexagonDecodeData hexagon)
                {
                    return hexagon.XStart == XStart
                        && hexagon.Color == Color
                        && hexagon.Index == Index
                        && hexagon.ErrorInfo == ErrorInfo
                        && hexagon.XLenght == XLenght
                        && hexagon.DisPlayFormat == DisPlayFormat
                        && hexagon.Title == Title
                        && hexagon.BitCount == BitCount
                        && EqualsBytes(hexagon.Data, Data)
                        && hexagon.IsInfoPacket == IsInfoPacket
                        && hexagon.BorderColor == BorderColor;
                }
                else
                {
                    return false;
                }
            }
            internal Boolean Equals(Object obj, out Boolean dataEquals)
            {
                if (obj is HexagonDecodeData hexagon)
                {
                    Boolean result = this.Equals(obj);
                    if (result)
                    {
                        dataEquals = true;
                    }
                    else
                    {
                        dataEquals = EqualsBytes(hexagon.Data, Data);
                    }
                    return result;
                }
                else
                {
                    dataEquals = false;
                    return false;
                }

            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }
    }
}
