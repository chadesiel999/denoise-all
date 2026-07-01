using System;
using System.Collections.Generic;
using System.Text;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.Plot
{
    public class Test : BaseRender
    {
        Common.VeldridRender.ImageRender.TestImageRender imageRender;
        public Test(IVeldridContent control) : base(control)
        {
            imageRender = new TestImageRender(control);
            imageRender.CreateResources();
        }

        //public override void Draw()
        //{
        //    for (int i = 0; i < 100; i++)
        //    {
        //        imageRender.Column = i;
        //        imageRender.Local = new System.Drawing.PointF(LineRange.XLenght / 10 * (i%10) + LineRange.MinX, LineRange.MinY+LineRange.YLenght/10*(i/10));
        //        imageRender.Draw();
        //    }
        //}
        private protected override BaseVeldridRender Renderer => imageRender;
        public BitmapData BitmapData { get => imageRender.Bitmap; set => imageRender.Bitmap = value; }
        public int Row { get => imageRender.Row; set => imageRender.Row = value; }
        public int Colnum { get => imageRender.Column; set => imageRender.Column = value; }
    }
}
