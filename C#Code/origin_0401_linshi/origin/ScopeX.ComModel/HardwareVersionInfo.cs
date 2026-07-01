using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public enum HardwareVersionItem
    {
        FPGA_Pcie=0,
        FPGA_S6=1,
        FPGA_ProcBd=2,

        FPGA_AcqBd1=3,
        FPGA_AcqBd2 = 4,
        FPGA_AcqBd3 = 5,
        FPGA_AcqBd4 = 6,
        FPGA_AcqBd5 = 7,
        FPGA_AcqBd6 = 8,
        FPGA_AcqBd7 = 9,
        FPGA_AcqBd8 = 10,
        FPGA_AcqBd9 = 11,
        FPGA_AcqBd10 = 12,
        FPGA_AcqBd11 = 13,
        FPGA_AcqBd12 = 14,
        FPGA_AcqBd13 = 15,
        FPGA_AcqBd14 = 16,
        FPGA_AcqBd15 = 17,
        FPGA_AcqBd16 = 18,

        AnalogChannel_B1 =33,
        AnalogChannel_B2 = 34,
        AnalogChannel_B1_Boot = 40,
        AnalogChannel_B2_Boot = 41,
        Keyboard = 60,
        AWG = 80,
        HardWare=90
    }
    [Serializable]
    public record HardwareVersionInfo
    {
        public Int32 Major;
        public Int32 Minor;
        public Int32 Build;
        public Int32 Revision;
        public string ModelName="";
        public DateTime LastBuildDateTime;
        public string LastModifier="";
        public string LastComment="";
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision},ModelName:{ModelName},LastBuildDateTime:{LastBuildDateTime.ToString("yyyyMMddHHmmss")},LastModifier:{LastModifier},LastComment:{LastComment}";
        }
        public int CompareTo(HardwareVersionInfo? hardwareVersionInfo2)
        {
            Int64 this_merge = this.Revision & 0xffff;
            this_merge |= (Int64)((this.Build & 0xffff) << 16);
            this_merge |= (Int64)((this.Minor & 0xffff) << 32);
            this_merge |= ((Int64)(this.Minor & 0xffff) << 48);

            Int64 merge2 = hardwareVersionInfo2.Revision & 0xffff;
            merge2 |= (Int64)((hardwareVersionInfo2.Build & 0xffff) << 16);
            merge2 |= (Int64)((hardwareVersionInfo2.Minor & 0xffff) << 32);
            merge2 |= ((Int64)(hardwareVersionInfo2.Minor & 0xffff) << 48);
            if (this_merge == merge2)
                return 0;
            return (this_merge > merge2) ? 1 : -1;
        }
    }
}
