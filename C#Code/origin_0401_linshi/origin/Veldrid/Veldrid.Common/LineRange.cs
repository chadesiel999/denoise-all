using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common
{

    public struct LineRange
    {
        public static implicit operator Vector4(LineRange range)
        {
            return new Vector4(range.MinX, range.MaxX, range.MinY, range.MaxY);
        }
        public static LineRange Zero { get; } = new LineRange(0,0,0,0);

        public UInt32 Size { get; } = (uint)Unsafe.SizeOf<Vector4>();
        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        internal LineRange(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }
        public void SetLineRange(float minX, float maxX, float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public float XLenght => MaxX - MinX;
        public float YLenght => MaxY - MinY;
        public Boolean IsEmpty => MinX >= MaxX || MinY >= MaxY;

        public float MinX
        {
            get => minX;
            set
            {
                if (minX != value)
                {
                    minX = value;
                }
            }
        }
        public float MaxX
        {
            get => maxX;
            set
            {
                if (maxX != value)
                {
                    maxX = value;
                }
            }
        }
        public float MinY
        {
            get => minY;
            set
            {
                if (minY != value)
                {
                    minY = value;
                }
            }
        }
        public float MaxY
        {
            get => maxY;
            set
            {
                if (maxY != value)
                {
                    maxY = value;
                }
            }
        }
    }
}
