using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
namespace ScopeX.Hardware.Calibration.Tool
{
    public class FpgaProjectLA
    {
        /// <summary>
        /// Fpga 工程文件的名称，以此来区分不同的FPGA工程
        /// </summary>
        public string ProjectFileName
        {
            get;
            set;
        } = "";
        public string? TopModuleName
        {
            get;
            set;
        } = "";
        public List<FpgaModuleAttribute> AllModules
        {
            get;
            set;
        } = new List<FpgaModuleAttribute>();

        public List<DefinedLAModule> DefinedLAModules
        {
            get;
            set;
        } = new List<DefinedLAModule>();

    }
    public class FpgaModuleAttribute
    {
        public string Name
        {
            get;
            set;
        } = "";
        public string ParentModule
        {
            get;
            set;
        } = "";
        public string DotVFileName
        {
            get;
            set;
        } = "";
        public List<Signal> AllSignals
        {
            get;
            set;
        } = new List<Signal>();
    }
    public class DefinedLAModule
    {
        public string ModuleName
        {
            get;
            set;
        } = "";
        public List<GroupDefine> GroupDefines
        {
            get;
            set;
        } = new List<GroupDefine>();
        public List<Signal> Probes
        {
            get;
            set;
        } = new List<Signal>();
    }
    public class GroupDefine
    {
        public string Name
        {
            get;
            set;
        } = "";
        public List<Signal> SelectedSignals
        {
            get;
            set;
        } = new List<Signal>();
    }
    public class Signal
    {
        [JsonPropertyName("SignalName")]
        public string SignalName { get; set; } = "";
        [JsonPropertyName("BitWidth")]
        public int BitWidth { get; set; }
        [JsonPropertyName("bIsTrig")]
        public bool bIsTrig { get; set; }
    }
    public class PackageToFPGA
    {
        public string FPGAMark
        {
            get;
            set;
        } = "";
        public string ModuleName
        {
            get;
            set;
        } = "";
        public int ModuleIndex
        {
            get;
            set;
        }
        public string GroupName
        {
            get;
            set;
        } = "";
        public int GroupIndex
        {
            get;
            set;
        }
        public UInt64 TrigConditionData
        {
            get;
            set;
        }
        public UInt16 OneSignalTrigCondition
        {
            get;
            set;
        }
        public Byte MultiTrigSignalLogicCondition
        {
            get;
            set;
        }
        public Int16 PreTrigOfClkCount
        {
            get;
            set;
        }
    }
    public class CaptureDataDecoder
    {
        public CaptureDataDecoder(byte[] data, List<Signal> SplitDefine)
        {
            StringBuilder sb = new StringBuilder(data.Length * 8);
            for (int i = 0; i < data.Length; i++)
                sb.Append(ByteToBinString(data[i]));
            string binString = sb.ToString();

            int totalSignalBits = 0;
            foreach (Signal signal in SplitDefine)
                totalSignalBits += signal.BitWidth;
            int perClockTotalBits = 8;
            if (totalSignalBits <= 8)
                perClockTotalBits = 8;
            else if (totalSignalBits<=16)
                perClockTotalBits = 16;
            else if (totalSignalBits <= 32)
                perClockTotalBits = 32;
            else if (totalSignalBits <= 64)
                perClockTotalBits = 64;
            else 
                perClockTotalBits = 128;

            int totalClockCount = data.Length * 8 / perClockTotalBits;
            int clockPos = 0;
            for (int clockIndex = 0; clockIndex < totalClockCount; clockIndex++)
            {
                List<UInt64> thisClock = new List<UInt64>();
                int startPos = 0;
                foreach (Signal signal in SplitDefine)
                {
                    thisClock.Add(Split(binString,clockPos + startPos, signal.BitWidth));
                    startPos += signal.BitWidth;
                }
                splitResult.Add(thisClock);

                clockPos += perClockTotalBits;
            }
        }
        private UInt64 Split(string binString,int start, int width)
        {
            UInt64 result = 0;
            for (int i = 0; i < width; i++)
                result |= (binString[start + i] == '0' ? 0 : (UInt64)(1 << i));
            return result;
        }
        private char[] ByteToBinString(byte b)
        {
            var result = new char[8];
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) == 0 ? '0' : '1';
            return result;
        }
        private List<List<UInt64>> splitResult = new List<List<ulong>>();
        public List<List<UInt64>> SplitResult
        {
            get => splitResult;
        }
    }
}
