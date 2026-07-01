using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class ThreeDimensionsPlot : BasePlot
    {
        Common.VeldridRender.LineRender.ThressDimensionsRender render;
        VeldridRender.LineRender.ThreeDimLineRender lineRender;
        VeldridRender.LineRender.AxisInstructionsRender axisrender;
        ColorPickerRender colorPicker;
        ThreeDimMutiText mutiText;
        private Color minColor;
        private Color middleColor;
        private Color lastColor;

        public ThreeDimensionsPlot(IVeldridContent control, int chDataLenght = 10000, int maxframeCount = 30) : base(control)
        {
            render = new VeldridRender.LineRender.ThressDimensionsRender(control, chDataLenght, maxframeCount);
            render.CreateResources();
            lineRender = new VeldridRender.LineRender.ThreeDimLineRender(control);
            lineRender.DataRenderConfigs = new VeldridRender.LineRender.DataRenderConfig[]
            {
                new VeldridRender.LineRender.DataRenderConfig()
                {
                    PointConfigs = new VeldridRender.LineRender.PointConfig[]
                    {
                        new VeldridRender.LineRender.PointConfig()
                        {
                            Color= Color.White,
                            PointCounts = new VeldridRender.LineRender.PointVisibily[]{0 }
                        },
                        new VeldridRender.LineRender.PointConfig()
                        {
                            Color =Color.FromArgb(10,Color.White),
                            PointCounts = new VeldridRender.LineRender.PointVisibily[]{ 0}
                        },
                    },
                    Primitive = PrimitiveTopology.LineList,
                }
            };
            lineRender.CreateResources();
            mutiText = new ThreeDimMutiText(control);
            mutiText.TextInfos = Enumerable.Range(0, 20).Select(x => new ThreeDimMutiText.TextInfo()
            {
                Color= RgbaFloat.Red,
                LayerDepth =-1f,
            }).ToArray();
            mutiText.CreateResources();
            InitAxisData();
            axisrender = new VeldridRender.LineRender.AxisInstructionsRender(control);
            axisrender.CreateResources();
            colorPicker = new ColorPickerRender(control);
            colorPicker.CreateResources();
            (this as IRender).Children.Add(lineRender);
            (this as IRender).Children.Add(mutiText);
            (this as IRender).Children.Add(axisrender);
            (this as IRender).Children.Add(colorPicker);
        }

        private void InitAxisData()
        {
            List<Vector3> vector3s = new List<Vector3>();
            vector3s.Add(new Vector3(-1, -1, 1));
            vector3s.Add(new Vector3(1, -1, 1));


            vector3s.Add(new Vector3(-1, -1, 1));
            vector3s.Add(new Vector3(-1, 1, 1));


            vector3s.Add(new Vector3(-1, -1, 1));
            vector3s.Add(new Vector3(-1, -1, -1));
            lineRender.WriteData(0, vector3s.ToArray());
            lineRender.DataRenderConfigs[0].DataLenght = (uint)vector3s.Count;
            lineRender.DataRenderConfigs[0].FixedDataLenght = (uint)vector3s.Count;
            lineRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0] = (uint)vector3s.Count;

            vector3s.Clear();


            for(int i =0;i<11;i++)
            {
                vector3s.Add(new Vector3(-1+i*0.2f, -1, 1));
                vector3s.Add(new Vector3(-1+i*0.2f, -1, -1));


                vector3s.Add(new Vector3(-1, -1 + i * 0.2f, 1));
                vector3s.Add(new Vector3(-1, -1 + i * 0.2f, -1));


                vector3s.Add(new Vector3(-1, -1,-1+  i*0.2f));
                vector3s.Add(new Vector3(1, -1, -1+ i*0.2f));


                vector3s.Add(new Vector3(-1, -1, -1+i * 0.2f));
                vector3s.Add(new Vector3(-1, 1, -1+i * 0.2f));



                vector3s.Add(new Vector3(-1, -1+i*0.2f, -1));
                vector3s.Add(new Vector3(1, -1+0.2f*i, -1));


                vector3s.Add(new Vector3(-1+0.2f*i, -1, -1));
                vector3s.Add(new Vector3(-1+0.2f*i, 1, -1));

            }


            lineRender.WriteData(6, vector3s.ToArray());
            lineRender.DataRenderConfigs[0].DataLenght = (uint)vector3s.Count+6;
            lineRender.DataRenderConfigs[0].FixedDataLenght = (uint)vector3s.Count+6;
            lineRender.DataRenderConfigs[0].PointConfigs[1].PointCounts[0] = (uint)vector3s.Count;


            for(int i =0;i<11;i++)
            {
                mutiText.TextInfos[i].Text = i.ToString();
                mutiText.TextInfos[i].Position = new Vector2(0 + 0.1f * i, 0);
            }


        }
        public int MaxFrameCount => render.MaxFrameCount;
        private protected override BaseVeldridRender Renderer => render;
        public void SetData(float[] data)
        {
            if (data == null || data.Length == 0) return;
            render.SetData(data);
        }
        public float Max { get => render.Max; set => render.Max = value; }
        public float Min { get => render.Min; set => render.Min = value; }
        public int FrameLenght { get => render.FrameLenght; }
        public override float Brightness { get => render.Brightness; set => render.Brightness = value; }
        public int TotalFrameCount => render.TotalFrameCount;
        public Color MinColor 
        { 
            get => minColor;
            set
            {
                if (minColor != value)
                {
                    minColor = value;
                    render.MinColor = value.ColorConverToRGBA();
                }
            }
        }
        public Color MiddleColor 
        {
            get => middleColor;
            set
            {
                if (middleColor != value)
                {
                    middleColor = value;
                    render.MiddleColor= value.ColorConverToRGBA();
                }
            }
        }
        public Color LastColor 
        { 
            get => lastColor;
            set
            {
                if (lastColor != value)
                {
                    lastColor = value;
                    render.LastColor= value.ColorConverToRGBA();
                }
            }
        }
        public float MinValue { get => render.MinValue; set => render.MinValue = value; }
        public float MiddleValue { get => render.MiddleValue; set => render.MiddleValue = value; }
        public float LastValue { get => render.LastValue; set => render.LastValue = value; }
    }
}
