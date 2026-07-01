using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common.Plot.Plot
{
    public class UPOPlot : BaseSeries
    {
        private UPORender _UPORender;
        public UPOPlot(IVeldridContent control, int width =1000,int height = 200,int chcount =4) : base(control)
        {
            _UPORender = new UPORender(control,width,height,chcount);
            _UPORender.CreateResources();
        }
        public int Brightness
        {
            get=>_UPORender.Brightness;
            set => _UPORender.Brightness = value;
        }
        private protected override BaseVeldridRender Renderer => _UPORender;
        public Int32 ChIndex1
        {
            get=>_UPORender.ChIndex1;
            set => _UPORender.ChIndex1 = value;
        }
        public Int32 ChIndex2
        {
            get=> _UPORender.ChIndex2;
            set => _UPORender.ChIndex2 = value;
        }

        public Int32 ChOnCount
        {
            get => _UPORender.ChOnCount; set => _UPORender.ChOnCount = value;
        }
        public Color[] Colors { get => _UPORender.Colors; set => _UPORender.Colors = value;}
        public Int32 MaxValue { get => _UPORender.MaxValue; set => _UPORender.MaxValue = value; }
        public Int32 MinValue { get => _UPORender.MinValue; set => _UPORender.MinValue = value; }
        public Int32 DrawWidth { get => _UPORender.DrawWidth; set => _UPORender.DrawWidth = value; }
        public Int32 DrawHeigth { get => _UPORender.DrawHeigth; set => _UPORender.DrawHeigth = value; }
        public void SetData(ref byte data, UInt32 datalen) => _UPORender.SetData(ref data, datalen);
    }
}
