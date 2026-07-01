using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common
{
    [StructLayout(LayoutKind.Sequential,Pack =1)]
    public struct Padding
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public Padding(float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public Padding(float all = 0) : this(all, all, all, all)
        {

        }
        public static Padding Zero { get; } = new Padding(); 
        public static UInt32 Size { get; } = (uint)Unsafe.SizeOf<Padding>();
    }
}
