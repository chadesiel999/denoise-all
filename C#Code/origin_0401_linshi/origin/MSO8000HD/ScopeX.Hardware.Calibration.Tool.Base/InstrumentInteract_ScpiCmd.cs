using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public partial class InstrumentInteract
    {
        /// <summary>
        /// 该表的右部，摘抄自服务提供工程中的SCPI_AllCMD.cs 中的AllElements 的第一列[缩写命令]
        /// </summary>
        private static Dictionary<ScpiCmd, string> ScpiCmdTable = new Dictionary<ScpiCmd, string>()
        {
            [ScpiCmd.Factory_CaliData_GetSet] = ":FACT:CDAT:DATA",
            [ScpiCmd.Factory_CaliData_LoadFromFile] = ":FACT:CDAT:LOAD",
            [ScpiCmd.Factory_CaliData_SaveToFile] = ":FACT:CDAT:SAV",
            [ScpiCmd.Factory_CaliData_ByteSize] = ":FACT:CDAT:BYTES",


            [ScpiCmd.Factory_WaveData_Adc] = ":FACT:WDAT:ADC",
            [ScpiCmd.Factory_WaveData_Channel] = ":FACT:WDAT:CHAN",
            [ScpiCmd.Factory_WaveData_LA] = ":FACT:WDAT:LA",
            [ScpiCmd.Factory_WaveData_ByteSize] = ":FACT:WDAT:BYTES",
            [ScpiCmd.Factory_CaliChannelGain_AllPhyLevelImp] = ":FACT:CHCG:APLI",
            [ScpiCmd.Factory_Cali_Special_ReadbackAdcRegisterValue] = ":FACT:CALI:SPEC:RADC",
            [ScpiCmd.Factory_Cali_SourceApply] = ":FACT:SOUR:APPL",
            [ScpiCmd.Factory_FPGAVersionInfo] = ":FACT:FPGA:VER",
            [ScpiCmd.Factory_FPGAWriteVersionInfo] = ":FACT:FPGA:WVER",
            [ScpiCmd.Factory_FPGAAllWriteRegisterValueReadback] = ":FACT:FPGA:REGV",
            [ScpiCmd.Factory_WriteFPGA_Register] = ":FACT:FPGA:WREG",
            [ScpiCmd.Factory_SpecailData] = ":FACT:CALI:SPEC:DATA",
            [ScpiCmd.Factory_CaliLogicValue] = ":FACT:CALI:LOGV",
            [ScpiCmd.Factory_TakeSpecialBinData] = ":FACT:TAK:BIND",
            [ScpiCmd.Factory_FlashCaliData_Save] = ":FACT:FLASh:SAV",
            [ScpiCmd.Factory_CaliData_LoadFromFlash]= ":FACT:FLASh:READ",
        };
    }
    /// <summary>
    /// 该定义是自由的，唯一要求是必须唯一
    /// </summary>
    public enum ScpiCmd : int
    {
        Factory_CaliData_GetSet,
        Factory_CaliData_LoadFromFile,
        Factory_CaliData_SaveToFile,

        Factory_WaveData_Adc,
        Factory_WaveData_Channel,
        Factory_WaveData_LA,

        Factory_CaliChannelGain_AllPhyLevelImp,
        Factory_Cali_Special_ReadbackAdcRegisterValue,
        Factory_Cali_SourceApply,
        Factory_CaliLogicValue,
        Factory_FPGAVersionInfo,
        Factory_FPGAWriteVersionInfo,
        Factory_FPGAAllWriteRegisterValueReadback,
        Factory_WriteFPGA_Register,
        Factory_SpecailData,
        Factory_TakeSpecialBinData,

        Factory_CaliData_ByteSize,
        Factory_WaveData_ByteSize,

        Factory_FlashCaliData_Save,
        Factory_CaliData_LoadFromFlash,
    }
}
