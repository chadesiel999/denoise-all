using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Decode.Model.FlexRay;
using ScopeX.Core.Decode.Model.SENT;
using static ScopeX.Core.Decode.ARINC429DecodeModelCPP;
using static ScopeX.Core.Decode.DecoderTypes;
using static ScopeX.Core.Decode.EthernetDecodeModel;
using static ScopeX.Core.Decode.ManchesterDecodeModel;
using static ScopeX.Core.Decode.MILDecodeModelCPP;
using static ScopeX.Core.Decode.I3CDecodeModel;
using static ScopeX.Core.Decode.D8B10BDecodeModel;
using static ScopeX.Core.Decode.Mlt3DecodeModel;
using static ScopeX.Core.Decode.CXPIDecodeModel;

namespace ScopeX.Core.Decode
{
    // 加载日志实例 dll初始化加载日志
    // static ;
    internal class DecoderImpl
    {
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeEthernet(EthernetOptions options, IntPtr edgePulseData, out EthernetResult results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeEthernet(EthernetResult results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeCAN(CANOptions options, IntPtr edgePulseData, out CANResult decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeCAN(CANResult results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeARINC429(ARINC429Options options, IntPtr edgePulseData, out ARINC429Result decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeARINC429(ARINC429Result results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeMIL1553(MILOptions options, IntPtr edgePulseData, out MILResult decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeMIL1553(MILResult results);
        [DllImport("Decode\\Decoder.dll")]
	    public static extern Boolean DecodeNrz(NRZOptions options, IntPtr edgePulseData, out NRZResultStruct decodeResult);
		[DllImport("Decode\\Decoder.dll")]
		public static extern Boolean FreeNrz(NRZResultStruct decodeResult);

		[DllImport("Decode\\Decoder.dll")]
		public static extern Boolean DecodeRs232(Rs232Options options, IntPtr edgePulseData, out Rs232ResultStruct decodeResult);
		[DllImport("Decode\\Decoder.dll")]
		public static extern Boolean FreeRs232(Rs232ResultStruct decodeResult);

		[DllImport("Decode\\Decoder.dll")]
		public static extern Boolean DecodeSpi(ref SpiOptions options, IntPtr edgePulseClk, IntPtr edgePulseMosi, IntPtr edgePulseCs, out SpiResultStruct decodeResult);
		[DllImport("Decode\\Decoder.dll")]
		public static extern Boolean FreeSpi(ref SpiResultStruct decodeResult);

        //[DllImport("Decode\\Decoder.dll")]
        //public static extern Boolean DecodeI2c(ref I2cOption option, IntPtr clkPulse, IntPtr dataPulse, out I2cResult result);

        //[DllImport("Decode\\Decoder.dll")]
        //public static extern Boolean FreeI2c(ref I2cResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeI2c(ref I2cOption option, IntPtr clkPulse, IntPtr dataPulse, out I2cResult result);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeI2c(ref I2cResult result);

  //      [DllImport("Decode\\Decoder.dll")]
		//public static extern Boolean DecodeUsb(UsbOptions options, IntPtr edgePulseData1, IntPtr edgePulseData2, out UsbResultStruct decodeResult);
		//[DllImport("Decode\\Decoder.dll")]
		//public static extern Boolean FreeUsb(UsbResultStruct decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeSENT(SentOptions options,IntPtr edgePulseData, out SentResult decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeSENT(SentResult results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeFlexRay(FlexRayOptions options, IntPtr edgePulseData, out FlexRayResult decodeResult);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeFlexRay(FlexRayResult results);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean AnalysisPAMData(WaveformInfoCPP options, ThresholdInfoCPP thresholdInfo, out IntPtr edgePulseSeq);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreePAMData(PAMType pamType, IntPtr edgePulseData);
		
		[DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeAudioBus(AudioBusOptions options, IntPtr clkPulseData, IntPtr wsPulseData, IntPtr sdPulseData, out AudioBusResultInfoCPP results);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeAudioBus(ref AudioBusResultInfoCPP result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeLIN(LinOption option,IntPtr edgePulse, out LinResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeLIN(ref LinResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodePCIE(ref PCIEOption option, IntPtr edgePulse, out PCIEResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreePCIE(ref PCIEResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeSPMI(ref SPMIOption option, IntPtr clkPulse, IntPtr dataPulse, out SPMIResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeSPMI(ref SPMIResult result);
		
		[DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeCPHY(CPHYOptions option, IntPtr abEdgePulse, IntPtr bcEdgePulse, IntPtr caEdgePulse, out CPHYResultInfoCPP result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeCPHY(ref CPHYResultInfoCPP result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeManchester(ref ManchesterOptions option, IntPtr edgePulse, out ManchesterResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeManchester(ref ManchesterResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeI3C(ref I3COption option, IntPtr clkPulse, IntPtr dataPulse, out I3CResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeI3C(ref I3CResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodePSI5(ref PSI5Options option, IntPtr edgePulse, out PSI5Result result);
        
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreePSI5(ref PSI5Result result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeSMBus(ref SMBusOption option, IntPtr clkPulse, IntPtr dataPulse, out SMBusResult result);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeSMBus(ref SMBusResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean Decode8B10B(ref D8B10BOptions option, IntPtr edgePulse, out D8B10BResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean Free8B10B(ref D8B10BResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeMLT3(ref Mlt3Options option, IntPtr edgePulse, out Mlt3Result result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeMLT3(ref Mlt3Result result);
        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeCXPI(ref CXPIOptions option, IntPtr edgePulse, out CXPIResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeCXPI(ref CXPIResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean DecodeUsb_New(ref UsbOptions option, IntPtr edgePulse1, IntPtr edgePulse2, out UsbResult result);

        [DllImport("Decode\\Decoder.dll")]
        public static extern Boolean FreeUsb_New(ref UsbResult result);
    }
}
