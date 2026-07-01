using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Veldrid.SDL2
{
    public struct DollarGestureEvent
    {
    }
    public struct MultiGestureEvent
    {
        public Int64 TouchID { get; }
        public float Theta { get; }
        public float Dist { get; }
        public Vector2 Center { get; }
        public UInt16 FingerNumber { get; }
        public MultiGestureEvent(Int64 touchID,float theta,float dist,float cx,float cy,UInt16 fingerNum)
        {
            TouchID= touchID;
            Theta= theta;
            Dist= dist;
            Center = new Vector2(cx, cy);
            FingerNumber = fingerNum;
        }
    }
}
