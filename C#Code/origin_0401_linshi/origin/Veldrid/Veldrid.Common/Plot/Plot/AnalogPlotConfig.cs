using System;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.Plot
{
    public class AnalogPlotConfig : BaseProperty
    {
        private float[] points = new float[0];
        private int cursorIndex = -1;
        private float local;
        private bool visibily = true;

        internal AnalogPlotConfig(uint index)
        {
            Index = index;
            CursorImages.ValueChangedEvent += CursorImages_ValueChangedEvent;
            CursorImages.RemoveEvent += CursorImages_RemoveEvent;
        }

        private void CursorImages_RemoveEvent(object sender, int e)
        {
            if (CursorIndex >= CursorImages.Count)
            {
                CursorIndex = -1;
                CurrentBitMap = BitmapData.Empty;
            }
            else
            {
                CurrentBitMap = CursorImages[CursorIndex];
            }
        }

        private void CursorImages_ValueChangedEvent(object sender, int e)
        {
            if (e == CursorIndex)
            {
                CurrentBitMap = CursorImages[CursorIndex];
            }
        }

        public uint Index { get; }

        public CursorImageCollection CursorImages { get; } = new CursorImageCollection();
        public int CursorIndex
        {
            get => cursorIndex;
            set
            {
                if (value >= CursorImages.Count)
                {
                    cursorIndex = -1;
                    CurrentBitMap = BitmapData.Empty;
                }
                else
                {
                    cursorIndex = value;
                    CurrentBitMap = CursorImages[cursorIndex];
                }
            }
        }
        internal BitmapData CurrentBitMap { get; private set; } = BitmapData.Empty;
        public Position Position { get; set; } = Position.Left;
        public float Local { get => local; set => Set(ref local, value); }
        public float[] Points
        {
            get => points;
            set
            {
                Set(ref points, value);
            }
        }
        public Boolean Visibily { get => visibily; set =>Set(ref visibily,value); }
    }
}
