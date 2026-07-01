using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal record FpgaFlash_Registers
    {
        public UInt32 BoardID { init; get; }
        public UInt32 SS { init; get; }
        public UInt32 WriteData { init; get; }
        public UInt32 ReadData { init; get; }
        public UInt32 ReadStart { init; get; }
        public UInt32 WriteStart { init; get; }
        public UInt32 SpiClock { init; get; }
        public Int16 SpiClockSetValue { init; get; }
        public UInt32 FlashMarkID { init; get; }

        public String IDCodeVerify { init; get; } = "";

        public UInt32 FlashImageGoldenStartAddr { init; get; }
        public UInt32 FlashImageGoldenTotalBytes { init; get; }

        public UInt32 FpgaInfoZoneStartAddr { init; get; }
        public UInt32 FpgaInfoZoneTotalBytes { init; get; }
        public UInt32 FlashImageAppStartAddr { init; get; }
        public UInt32 FlashImageAppTotalBytes { init; get; }
        public UInt32 FlashDataStartAddr { init; get; }
        public UInt32 FlashDataTotalBytes { init; get; }

        public UInt32 FlashUpDateGoldenFlag { init; get; }
        public UInt32 FlashMinInfoSizeByte { init; get; }
        public UInt32 ProductInfoStartAtBytes { init; get; }
        public UInt32 ProductInfoTotalBytes { init; get; }

        public UInt32 OptionsInfoStartAtBytes { init; get; }
        public UInt32 OptionsInfoTotalBytes { init; get; }

        public UInt32 CaliDataStartAtBytes { init; get; }
        public UInt32 CaliDataTotalBytes { init; get; }
        public UInt32 CaliDataUsedSectorCount { init; get; }

        public UInt32 CaliDataStartAtBytes_AWG { init; get; }
        public UInt32 CaliDataTotalAtBytes_AWG { init; get; }

        public UInt32 CaliDataStartAtBytes_DSP { init; get; }
        public UInt32 CaliDataTotalAtBytes_DSP { init; get; }

        public UInt32 CaliDataStartAtBytes_Channel { init; get; }
        public UInt32 CaliDataTotalAtBytes_Channel { init; get; }

        public UInt32 CaliDataStartAtBytes_TiAdc { init; get; }
        public UInt32 CaliDataTotalAtBytes_TiAdc { init; get; }

        public UInt32 CaliDataStartAtBytes_MISC { init; get; }
        public UInt32 CaliDataTotalAtBytes_MISC { init; get; }
        public Boolean bExistsCaliData { init; get; } = false;
        public Boolean bExistsProductInfo { init; get; } = false;

        #region 2024 new Fast Mode Schema 
        public UInt32 FastMode_ActionReset { init; get; }
        public UInt32 FastMode_ActionCode { init; get; }
        public UInt32 FastMode_ActionStart { init; get; }
        public UInt32 FastMode_ActionStatus { init; get; }
        public UInt32 FastMode_FlashStartAddressL16 { init; get; }
        public UInt32 FastMode_FlashStartAddressH16 { init; get; }
        public UInt32 FastMode_FlashEndAddressL16 { init; get; }
        public UInt32 FastMode_FlashEndAddressH16 { init; get; }
        public UInt32 FastMode_WhichFlash { init; get; }
        public UInt32 FastMode_FlashID_L16 { init; get; }
        public UInt32 FastMode_FlashID_H16 { init; get; }

        #endregion
    }
}
