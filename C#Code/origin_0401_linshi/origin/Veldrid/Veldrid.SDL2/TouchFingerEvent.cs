using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Veldrid.SDL2
{
    public struct TouchFingerEvent
    {
        /// <summary>
        /// 触摸设备ID
        /// </summary>
        public Int64 TouchID { get; }
        /// <summary>
        /// 手指ID
        /// </summary>
        public Int64 FingerID { get; }
        /// <summary>
        /// 触摸事件的位置,范围0~1
        /// </summary>
        public Vector2 Position { get; }
        /// <summary>
        /// 手指移动距离，范围-1~1
        /// </summary>
        public Vector2 Moved { get; }
        /// <summary>
        /// 施加的压力，范围0~1
        /// </summary>
        public float Pressure { get; }
        public TouchFingerEvent(Int64 touchID,Int64 fingerID,float x,float y,float dx,float dy,float pressure)
        {
            TouchID = touchID;
            FingerID = fingerID;
            Position = new Vector2(x, y);
            Moved = new Vector2(dx, dy);
            Pressure = pressure;
        }
    }
}
