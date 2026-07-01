using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using System.Runtime.InteropServices;

namespace ScopeX.Core
{
    public class WfmProperties
    {
        private const String _VER = "U2Core221103";
        private Double dpxCorrection = 1.0;

        public WfmProperties(String name)
        {
            Name = name;
            Stamp = DateTime.Now.Ticks;
            Version = _VER;
        }

        public String Name
        {
            get;
            set;
        }

        public Int64 Stamp
        {
            get;
        }

        public String Version
        {
            get;
        }

        public Boolean IsCompatible => Version == _VER;

        public Int32 ProcessFlag
        {
            get;
            set;
        }

        public (Int32 Index, Double Value) ChnlScale
        {
            get;
            set;
        }

        //Index: 1000/div
        public (Double Index, Double Value) ChnlPosition
        {
            get;
            set;
        }

        public (Prefix Prefix, String Name) ChnlUnit
        {
            get;
            set;
        }

        public Double ChnlBias
        {
            get;
            set;
        }

        public (Int32 Index, Double Value) TmbScale
        {
            get;
            init;
        }

        //Index: 1000/div
        public (Double Index, Double Value) TmbPosition
        {
            get;
            init;
        }

        public (Prefix Prefix, String Name) TmbUnit
        {
            get;
            set;
        }

        public (Double Gain, Double UnitRatio)? ProbeInfo
        {
            get;
            set;
        }

        //Horizontal drawing ratio
        //public Double VuFactor
        //{
        //    get;
        //    set;
        //}

        //Horizontal drawing offset
        public Double VuStartIndex
        {
            get;
            set;
        }

        public Double SampInterval
        {
            get;
            set;
        }

        public Double DpxCorrection
        {
            get => dpxCorrection;
            set

            {
                dpxCorrection = value;
            }
        }
        public Double TrigErrorTime
        {
            get;
            set;
        }

        public DrawMethod DrawMethod
        {
            get;
            set;
        } = DrawMethod.Plot;
        public Int64 WfmUpdateTime
        {
            get;
            set;
        }

        public readonly Int32 ClippingThreshold = 10;

        //波形超出屏幕
        public Clipping Clipping
        {
            get;
            set;
        } = Clipping.None;

        /// <summary>
        /// DDR帧号
        /// </summary>
        public UInt16 FrameNo
        {
            get;
            set;
        }
    }

    public sealed record WfmPack(Double[,] Buffer, Int32 Offset, Int32 Length, WfmProperties Properties);

    //public sealed record DigiWfmPack(UInt32[,] Buffer, Int32 Offset, Int32 Length, WfmProperties Properties);
}
