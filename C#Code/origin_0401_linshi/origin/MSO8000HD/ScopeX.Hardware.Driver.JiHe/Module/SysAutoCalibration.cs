using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ScopeX.ComModel;
using System.IO;

namespace ScopeX.Hardware.Driver
{
    internal class SysAutoCalibration
    {
        public static SysAutoCalibration Default = new SysAutoCalibration();
        private List<Int32> _AcqProcBdLooptimeBySysClockCount = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };//最多8个AcqBoard,其序号是Acqboard Index
        public Int32 Trig_AcqProcBdLooptimBySysClockCount(int channelIndex)
        {
            if (channelIndex >= ChannelIdExt.AnaChnlNum)
                channelIndex = 0;
            ChannelBdAdcInputDefine? channelBdAdcInputDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.GetChannelAcqBdAdcInputCorresponding(channelIndex);
            if (channelBdAdcInputDefine != null)
                return _AcqProcBdLooptimeBySysClockCount[(int)channelBdAdcInputDefine.BdNo];
            else
                return _AcqProcBdLooptimeBySysClockCount[0];
        }
        string fileName_TrigScanAcqProcBdLoopTime = $@"\CaliData\TrigScanAcqProcBdLoopTimeData.txt";
        private bool LoadSettingTrigScanAcqProcBdLoopTime()
        {
            string txtFileName = $@"{AppDomain.CurrentDomain.BaseDirectory}{fileName_TrigScanAcqProcBdLoopTime}";
            if (File.Exists(txtFileName))
            {
                string[] fileContent = File.ReadAllLines(txtFileName);
                foreach (string line in fileContent)
                {
                    if (line.Trim() == "" || line[0] == '#' || line.Substring(0, 2) == "//")
                        continue;
                    string[] items=line.Split(',');
                    if (items.Length == 2)
                    {
                        AcqBdNo acqBdNo = items[0].Trim() switch
                        {
                            "B0"=> AcqBdNo.B0,
                            "B1" => AcqBdNo.B1,
                            "B2" => AcqBdNo.B2,
                            "B3" => AcqBdNo.B3,
                            "B4" => AcqBdNo.B4,
                            "B5" => AcqBdNo.B5,
                            "B6" => AcqBdNo.B6,
                            "B7" => AcqBdNo.B7,
                            _ => AcqBdNo.B0,
                        };
                        Int32 data = Int32.Parse(items[1]);
                        _AcqProcBdLooptimeBySysClockCount[(int)acqBdNo] = data;
                    }
                }
            }
            return false;
        }
        public void Trig_AcqProcBdLooptime_Cali()
        {

        }
    }
}
