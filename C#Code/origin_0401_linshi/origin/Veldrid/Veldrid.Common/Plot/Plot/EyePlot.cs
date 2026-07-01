using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.EyeRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class EyePlot : BasePlot
    {
        private EyeRender _EyeRender;
        private VeldridText veldridText;
        public EyePlot(IVeldridContent control, UInt32 width = 4096, UInt32 height = 512,bool pointSampler = true) : base(control)
        {
            _EyeRender = new EyeRender(control, width, height, pointSampler);
            _EyeRender.CreateResources();
            veldridText = new VeldridText(control);
            veldridText.CreateResources();
            veldridText.Color = Color.White;
            (this as IRender).Children.Add(veldridText);
            Visibily = false;
        }
        public override string Label { get => veldridText.Text; set => veldridText.Text = value; }
        public override string FontName { get => veldridText.FontName; set => veldridText.FontName = value; }
        public override float FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }
        public override string FontStyle { get => veldridText.FontStyle; set => veldridText.FontStyle = value; }
        public UInt32 Width => _EyeRender.Width;
        public UInt32 Height => _EyeRender.Height;

        public Color TextColor
        {
            get {return veldridText.Color; }
            set {
                if (value!= veldridText.Color)
                {
                    veldridText.Color = value;
                }
            }
        }
        

        public void SetData(Single[,] data)
        {
            _EyeRender.SetData(data);
        }
        public unsafe void SetData(Single[] data, uint width = 4096, uint height = 512)
        {
            if (data == null || data.Length == 0)
            {
                Visibily = false;
                return;
            }
            _EyeRender.Max = data.Max();
            _EyeRender.Min = data.Min();
            fixed (void* ptr = &data[0])
            {
                _EyeRender.SetData((IntPtr)ptr, (UInt32)(Unsafe.SizeOf<Single>() * data.Length), width, height);
            }
        }

        // 双线性插值函数
        private double BilinearInterpolation(double x, double y, Int32 x1, Int32 y1, Int32 x2, Int32 y2, double[] pixels, Int32 width)
        {
            double q11 = pixels[y1 * width + x1];
            double q12 = pixels[y2 * width + x1];
            double q21 = pixels[y1 * width + x2];
            double q22 = pixels[y2 * width + x2];

            double r1 = (x2 - x) / (x2 - x1) * q11 + (x - x1) / (x2 - x1) * q21;
            double r2 = (x2 - x) / (x2 - x1) * q12 + (x - x1) / (x2 - x1) * q22;

            double value = (y2 - y) / (y2 - y1) * r1 + (y - y1) / (y2 - y1) * r2;

            return value;
        }

        private protected override BaseVeldridRender Renderer => _EyeRender;
    }
}
