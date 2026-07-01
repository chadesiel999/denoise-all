using ScopeX.ComModel;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ScopeX.Core.Decode
{
    internal sealed partial class ARINC429DecodeModel
    {
        private struct Arinc429PacketInfo
        {
            public Boolean HasGap;
            public Boolean Gap;
            public Int32 GapLength;
            public Int32 GapIndex;

            public Boolean HasSOF;
            public Int32 SOFIndex;

            public Boolean HasEOF;
            public Int32 EOFIndex;

            public Boolean HasExtraBit;
            public Int32 ExtraBitIndex;
            public Int32 ExtraBitLen;
            public Byte ExtraData;
            public Int32 ExtraDataBitCount;

            public Boolean HasParity;
            public Int32 ParityIndex;
            public Int32 ParityLength;
            public Boolean Parity;
            public Boolean SuccessParity;

            public Boolean HasSSM;
            public Int32 SSMIndex;
            public Int32 SSMLength;
            public Byte SSM;
            public Int32 SSMBitCount;
            public Int32 SSMSuccessBitCount;
            public Boolean SuccessSSM;

            public Boolean HasData;
            public Int32 DataIndex;
            public Int32 DataLength;
            public Int32 DataBitCount;
            public UInt32 TempData;
            public Int32 DataSuccessDataBitCount;
            public Byte[] Data;
            public Boolean SuccessData;

            public Boolean HasSDI;
            public Int32 SDIIndex;
            public Int32 SDILength;
            public Byte SDI;
            public Int32 SDIBitCount;
            public Int32 SDISuccessDataBitCount;
            public Boolean SuccessSDI;

            public Boolean HasLabel;
            public Int32 LabelIndex;
            public Int32 LabelLength;
            public Byte Label;
            public Int32 LabelBitCount;
            public Int32 LabelSuccessDataBitCount;
            public Boolean SuccessLabel;

            public override String ToString()
            {
                return $"Label:{Label:X2} SDI:{SDI:X2} Data:{String.Join(" ", Data.Select(x => $"{x:X2}"))} SSM:{SSM:X2} {nameof(Parity)}:{Parity}";
            }
        }
    }
}
