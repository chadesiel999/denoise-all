using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        private uint ScanTap_Start = 0;
        private uint ScanTap_Step = 10;
        private uint ScanTap_Count = 50;

        class TapScanSteadyResult
        {
            public int Start { get; set; }
            public int End { get; set; }
            public int Width { get => End - Start; }
            public int ShiftTimes { get; set; }
        }

        private bool BoardInteractionDelayTap_ReadFromFile(string fileName, List<TapScanSteadyResult>? dataLineBestTapValue, List<TapScanSteadyResult>? ctrlLineBestTapValue)
        {
            if (File.Exists(fileName))
            {
                List<TapScanSteadyResult> DataLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                List<TapScanSteadyResult> CtrlLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                string[] fileContent = File.ReadAllLines(fileName);
                foreach (string line in fileContent)
                {
                    if (line.Trim() == "" || line.Trim().StartsWith("//"))
                        continue;
                    if (line.Trim().ToLower().StartsWith("ctrlline"))
                    {
                        string[] items = line.Trim().Split(',');
                        if (items.Length < 5)
                            continue;
                        if (ctrlLineBestTapValue != null)
                            ctrlLineBestTapValue.Add(new TapScanSteadyResult() { Start = int.Parse(items[2]), End = (int.Parse(items[3])), ShiftTimes = int.Parse(items[4]) });
                    }
                    else if (line.Trim().ToLower().StartsWith("dataline"))
                    {
                        string[] items = line.Trim().Split(',');
                        if (items.Length < 5)
                            continue;
                        if (dataLineBestTapValue != null)
                            dataLineBestTapValue.Add(new TapScanSteadyResult() { Start = int.Parse(items[2]), End = (int.Parse(items[3])), ShiftTimes = int.Parse(items[4]) });
                    }
                }
                return true;
            }
            else
                return false;
        }
        
        private void BoardInteractionDelayTap_SaveToFile(string fileName, Dictionary<string, List<TapScanSteadyResult>> content)
        {
            var dir = Path.GetDirectoryName(fileName);
            if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(fileName))
                File.Delete(fileName);
            StringBuilder fileContent = new StringBuilder();
            fileContent.AppendLine($"//格式为：");
            fileContent.AppendLine($"//第一列，标识是什么线型，值取值包括dataline或ctrlline；");
            fileContent.AppendLine($"//第二列，线号，如0,1，2,3；");
            fileContent.AppendLine($"//第三列，StartTapIndex，第几个以10位步进的TapIndex;");
            fileContent.AppendLine($"//第四列，End结束， 标识经过几个以10位宽度还是连续稳定的；");
            fileContent.AppendLine($"//第五列，移位次数");
            foreach (KeyValuePair<string, List<TapScanSteadyResult>> entry in content)
            {
                for (int lineIndex = 0; lineIndex < entry.Value.Count; lineIndex++)
                    fileContent.AppendLine($"{entry.Key},{lineIndex},{entry.Value[lineIndex].Start},{entry.Value[lineIndex].End},{entry.Value[lineIndex].ShiftTimes}");
            }
            File.WriteAllText(fileName, fileContent.ToString());
        }

        private void RecordSendStr(string fileName, string sendstr)
        {
            if (File.Exists(fileName))
            {
                File.AppendAllText(fileName, sendstr);
            }
        }

        private List<TapScanSteadyResult> AdjustTapScanSteadyResult(List<TapScanSteadyResult> source, Int32 thresh)
        {
            List<TapScanSteadyResult> result = new List<TapScanSteadyResult>();
            int maxShiftTimes = 0;
            int minShiftTimes = 255;
            foreach (TapScanSteadyResult s in source)
            {
                maxShiftTimes = Math.Max(maxShiftTimes, s.ShiftTimes);
                minShiftTimes = Math.Min(minShiftTimes, s.ShiftTimes);
            }
            int adjust = 0;
            if ((maxShiftTimes - minShiftTimes) > thresh)
                adjust = thresh * 2;
            foreach (TapScanSteadyResult s in source)
            {
                result.Add(new TapScanSteadyResult() { Start = s.Start, End = s.End, ShiftTimes = s.ShiftTimes + (s.ShiftTimes > thresh ? 0 : adjust) });
            }
            return result;
        }




        internal void BoardInteractionDelay_DoAllCali(List<(String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)> config)
        {
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.UIMessage == null || config == null || config.Count == 0)
                return;
            var docali = config.Where(x => x.ISENABLE).ToList();
            #region Step1:Scan TrandferDelay Reset
            BoardInteractionDelay_Reset_AcqBd();
            BoardInteractionDelay_Reset_ProcBd();

            //PcieBoard
            //HdIO.WriteReg(PcieBdReg.W.BoardSync_IO_Reset, 0);
            //HdIO.WriteReg(PcieBdReg.W.BoardSync_IO_Reset, 1);
            //HdIO.Sleep(10);
            //HdIO.WriteReg(PcieBdReg.W.BoardSync_IO_Reset, 0);

            //HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Sel, 1);
            #endregion
            //cij_0606
            #region Step2:Start
            BoardInteractionDelay_Start_AcqBd();
            BoardInteractionDelay_Start_ProcBd();

            //BoardInteractionDelay_Start_PcieBd();
            #endregion

            #region Step3:Do Cali

            #region AcqBd

            #endregion

            String Path = String.Empty;
            AcqBdNo no;

            foreach (var item in docali)
            {
                Path = String.Format(CaliResultSavedFileName, BasePath, $"Acq_{item.Name}");
                no = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), item.Name);
                BoardInteractionDelay_DoCali_AcqBd(no, item.NUM_PROC, Path);
            }

            UInt32 index = 0;
            for (Int32 i = 0, L = docali.Count; i < L; i++)
            {
                Path = String.Format(CaliResultSavedFileName, BasePath, $"Proc_{docali[i].Name}");
                no = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), docali[i].Name);
                BoardInteractionDelay_DoCali_ProcBd_Acq(no, docali[i].NUM_ACQ_DATA, docali[i].NUM_ACQ_CTRL, Path, index, false);
                i++;

                Path = String.Format(CaliResultSavedFileName, BasePath, $"Proc_{docali[i].Name}");
                no = (AcqBdNo)Enum.Parse(typeof(AcqBdNo), docali[i].Name);
                BoardInteractionDelay_DoCali_ProcBd_Acq(no, docali[i].NUM_ACQ_DATA, docali[i].NUM_ACQ_CTRL, Path, index, true);
                index += 2;
            }

            //？
            BoardCtrlLineDelay_RstCali();
            //BoardInteractionDelay_DoCali_PcieBd();
            #endregion
        }

    }
}
