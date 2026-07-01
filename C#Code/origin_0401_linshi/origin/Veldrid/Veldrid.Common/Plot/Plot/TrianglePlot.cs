using System;
using System.Data;
using System.Drawing;
using System.Linq;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class TrianglePlot : BasePlot
    {
        private TriangleRender _TriangleRender;
        public TrianglePlot(IVeldridContent control) : base(control)
        {
            _TriangleRender = new TriangleRender(control);
            _TriangleRender.CreateResources();
            _TriangleRender.Brightness = 100;
            _TriangleRender.Width = this.LocalSizeToVirtualSize(halfSideLength, 0).Width * 2 / LineRange.YLenght;
            veldridText = new MutiText(control, true);
            veldridText.TextInfos = new TextInfo[11] {
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo()
            };
            Text = new MutiCursorTextConfig[11] {
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig()
            };

            veldridText.CreateResources();
            (this as IDropRender).Children.Add(veldridText);
        }
        private MutiText veldridText;
        public MutiCursorTextConfig[] Text { get; } = new MutiCursorTextConfig[11];

        private Int32 _MaxCount = 11;

        private float halfSideLength = 1;
        public float Width
        {
            get { return _TriangleRender.Width; }
            set
            {
                _TriangleRender.Width = this.LocalSizeToVirtualSize(value, 0).Width * 2 / LineRange.YLenght;
            }
        }
        private protected override BaseVeldridRender Renderer => _TriangleRender;
        public Color Color
        {
            get => _TriangleRender.Color;
            set
            {
                if (_TriangleRender.Color == value) return;
                _TriangleRender.Color = value;
            }
        }

        public override float Brightness
        {
            get => _TriangleRender.Brightness;
            set
            {
                if (_TriangleRender.Brightness == value) return;
                _TriangleRender.Brightness = value;
            }
        }
        public override float HorizontalOffset { get => _TriangleRender.HorizontalOffset; set => _TriangleRender.HorizontalOffset = value; }
        public (Double[,] Data,Boolean[] IsKey) Points
        {
            set
            {
                _TriangleRender.SetData(value);
            }
        }

        private float[] _Position = new float[11];
        public float[] Position
        {
            set
            {
                _Position = value;
                _TriangleRender.SetData(value);
            }
        }

        public (float position, String infos)[] MarkerInfo
        {
            set
            {
                _Position = value.Select(o => o.position).ToArray();
                _TriangleRender.SetData(value.Select(o => o.position).ToArray());
                for (int i = 0; i < value.Length; i++)
                {
                    veldridText.TextInfos[i].Text = value[i].infos;
                }
                for (int i = 0; i < veldridText.TextInfos.Length; i++)
                {
                    if (i < value.Length)
                    {
                        veldridText.TextInfos[i].Text = value[i].infos;
                    }
                    else
                    {
                        veldridText.TextInfos[i].Text = "";
                    }

                }

                CalcTextPosition();
            }
        }
        public Color TextBackColor
        {
            set
            {
                for (int i = 0; i < veldridText.TextInfos.Length; i++)
                {
                    veldridText.TextInfos[i].BackColor = value;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                Text[i].Dispose();
            }
            base.Dispose(disposing);
        }
        private float _Offset = 1000; // 不能使用固定值，在窗口缩放时会出现位置偏移，暂时使用mtextsize.Y*1.3f替代；
        private void CalcTextPosition()
        {
            var point = new PointF();
            for (int i = 0; i < Text.Length; i++)
            {
                if (i < _TriangleRender.DataLengh)
                {
                    var mtextsize = veldridText.GetVirtualSize(veldridText.TextInfos[i].Text);
                    var xOffset = _Position[i] + mtextsize.X / 2 + 50;
                    point.X = xOffset;
                    point.Y = LineRange.MaxY - mtextsize.Y*1.3f;
                    if (Renderer.Range.MaxX - xOffset < mtextsize.X && _Position[i] < Renderer.Range.MaxX)
                    {
                        point.X = Renderer.Range.MaxX - mtextsize.X;
                    }
                    
                    veldridText.TextInfos[i].Local = point;
                }

            }

        }


    }
}
