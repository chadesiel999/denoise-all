using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    struct MILPacketInfo
    {
        public Boolean HasSOF;
        public Int32 SOFIndex;

        public MILPacketType PacketType;

        public Boolean HasSync;
        public Int32 SyncIndex;
        public Int32 SyncLength;
        /*
         * Command
         */

        public Boolean HasRTA;
        public Int32 RTAIndex;
        public Int32 RTALength;
        public UInt32 RTABitCount;
        public Byte RTA;

        public Boolean HasTR;
        public Int32 TRIndex;
        public Int32 TRLength;
        public Boolean TR;

        public Boolean HasSubAddress;
        public Byte SubAddress;
        public UInt32 SubAddressBitCount;
        public Int32 SubAddressIndex;
        public Int32 SubAddressLength;

        public Boolean HasModelCode;
        public Byte ModelCode;
        public UInt32 ModelCodeBitCount;
        public Int32 ModelCodeIndex;
        public Int32 ModelCodeLength;

        /*
         * Data
         */

        public Boolean HasData;
        public Byte[] Data;
        public UInt16 TempData;
        public UInt32 DataBitCount;
        public Int32 DataIndex;
        public Int32 DataLength;

        /*
         * Status
         */

        public Boolean HasMessageError;
        public Boolean MessageError;
        public Int32 MessageErrorIndex;
        public Int32 MessageErrorLength;

        public Boolean HasInstrumentation;
        public Boolean Instrumentation;
        public Int32 InstrumentationIndex;
        public Int32 InstrumentationLength;

        public Boolean HasServiceRequest;
        public Int32 ServiceRequestIndex;
        public Int32 ServiceRequestLength;
        public Boolean ServiceResquest;

        public Boolean HasReserved;
        public Int32 ReservedIndex;
        public Int32 ReservedLength;
        public Byte Reserved;
        public UInt32 ReservedBitCount;

        public Boolean HasBroadcastCommandReceived;
        public Boolean BroadcastCommandReceived;
        public Int32 BroadcastCommandReceivedIndex;
        public Int32 BroadcastCommandReceivedLength;

        public Boolean HasBusy;
        public Boolean Busy;
        public Int32 BusyIndex;
        public Int32 BusyLength;

        public Boolean HasSubsystemFlag;
        public Int32 SubsystemFlagIndex;
        public Int32 SubsystemFlagLength;
        public Boolean SubsystemFlag;

        public Boolean HasDynamicBusControlAcceptance;
        public Boolean DynamicBusControlAcceptance;
        public Int32 DynamicBusControlAcceptanceIndex;
        public Int32 DynamicBusControlAcceptanceLength;

        public Boolean HasTerminalFlag;
        public Int32 TerminalFlagIndex;
        public Int32 TerminalFlagLength;
        public Boolean TerminalFlag;

        public Boolean HasParity;
        public Boolean Parity;
        public Boolean SuccessParity;
        public Int32 ParityIndex;
        public Int32 ParityLength;

        public Boolean HasEOF;
        public Int32 EOFIndex;
        public override String ToString()
        {
            return PacketType switch
            {
                MILPacketType.Data => $"{PacketType} {TempData:X4}",
                MILPacketType.Command => $"{PacketType} {RTA:X2} {(TR ? "T" : "R")} {SubAddress:X2} {ModelCode:X2}",
                MILPacketType.Status =>
                    $"{PacketType} {RTA:X2} {Convert.ToByte(MessageError)}{Convert.ToByte(Instrumentation)}{Convert.ToByte(ServiceResquest)} {Reserved:X2} {Convert.ToByte(BroadcastCommandReceived)}{Convert.ToByte(Busy)}{Convert.ToByte(SubsystemFlag)}{Convert.ToByte(DynamicBusControlAcceptance)}{Convert.ToByte(TerminalFlag)}",
                _ => "",
            };
        }
    }
}
