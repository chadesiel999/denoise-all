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
        private void BoardInteractionDelay_Start_PcieBd()
        {
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Tap_Gap, (UInt32)ScanTap_Step);

            HdIO.WriteReg(PcieBdReg.W.BoardSync_Tap_Start, ScanTap_Start);
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Tap_Stop, ScanTap_Start + (ScanTap_Count - 1) * ScanTap_Step);
            // 开始扫窗控制
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Scan_Start, 0);
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Scan_Start, 1);
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Scan_Start, 0);
        }

        private void BoardInteractionDelay_Send_PcieBd(int ScanTap_Step, List<TapScanSteadyResult>? LineBestTapValue)
        {
            HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Sel, 0);


            if (LineBestTapValue != null)
            {
                for (int dataLineIndex = 0; dataLineIndex < LineBestTapValue.Count; dataLineIndex++)
                {
                    //Tap Value
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Io_Delay_Addr, (uint)dataLineIndex);              //先发地址
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Io_Delay_Value, (uint)((LineBestTapValue[dataLineIndex].Start + LineBestTapValue[dataLineIndex].Width / 2) * ScanTap_Step + ScanTap_Start));     //再发数据
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Io_Delay_Ld_En, 0x1);
                    HdIO.Sleep(1);
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                    //ShiftTimes
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Shift_Value, (uint)LineBestTapValue[dataLineIndex].ShiftTimes);     //再发数据
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex );         //通过控制地址的第7位拉高来生效数据
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex | 0x80);         //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex);
                }
            }
        }

        private void BoardInteractionDelay_DoCali_PcieBd()
        {
            string caliResultSavedFileName = $@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\ProcPcieBdLoopScanResult_Pcie.txt";
            int DataLineCount = 13;
            if (!Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime)
            {
                List<TapScanSteadyResult> CtrlLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                if (BoardInteractionDelayTap_ReadFromFile(caliResultSavedFileName, null, CtrlLineBestTapValue_FromFile))
                    BoardInteractionDelay_Send_PcieBd((int)ScanTap_Step, CtrlLineBestTapValue_FromFile);
                return;
            }
            if (!HdIO.CheckRegisterValue((uint)PcieBdReg.R.BoardSync_Scan_Finish, 0x01, 0x01, 1000))
                return;
            #region  Step2:Read Ram
            UInt32[/*LineIndex*/,/*TapValue*/] DataLineScanResult = new UInt32[DataLineCount, ScanTap_Count];//FPGA 实际多
            for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++)
            {
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(dataLineIndex * ScanTap_Count + tapIndex));                     //reg_ram_rd_addr_pro 先读数据线地址
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Ram_Rd_En, 0);                                        //reg_ram_rd_en_pro
                    HdIO.WriteReg(PcieBdReg.W.BoardSync_Ram_Rd_En, 1);                                    //reg_ram_rd_en_pro
                    DataLineScanResult[dataLineIndex, tapIndex] = HdIO.ReadReg(PcieBdReg.R.BoardSync_Scan_Result);                   //reg_scan_result_pro                   

                }
            }
            #endregion

            #region Step3:手动观察
            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("DataLine Scan Result:");
            //string perLineResult = "";
            //for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
            //    perLineResult += tapIndex.ToString().PadLeft(4, '=') + ',';
            //stringBuilder.AppendLine($"   CtrlLine    Result:{perLineResult}");
            //for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++)
            //{
            //    perLineResult = "";
            //    for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
            //    {
            //        perLineResult += DataLineScanResult[dataLineIndex, tapIndex].ToString().PadLeft(4, ' ') + ",";
            //    }
            //    stringBuilder.AppendLine($"   DataLine {dataLineIndex.ToString().PadLeft(2, '_')}_Result:{perLineResult}");
            //}
            //stringBuilder.AppendLine("CtrlLine Scan Result:");
            //perLineResult = "";
            //for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
            //    perLineResult += tapIndex.ToString().PadLeft(4, '=') + ',';
            //stringBuilder.AppendLine($"   CtrlLine    Result:{perLineResult}");
            //for (int ctrlLineIndex = 0; ctrlLineIndex < CtrlLineCount; ctrlLineIndex++)
            //{
            //    perLineResult = "";
            //    for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
            //    {
            //        perLineResult += CtrlLineScanResult[ctrlLineIndex, tapIndex].ToString().PadLeft(4, ' ') + ",";
            //    }
            //    stringBuilder.AppendLine($"   CtrlLine {ctrlLineIndex.ToString().PadLeft(2, '_')}_Result:{perLineResult}");
            //}
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}AcqProcBorad_ScanResult_ProcBoard_{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss_ffff")}.txt", stringBuilder.ToString());
            #endregion

            #region Step4:计算Tap Value
            #region DataLine
            List<List<TapScanSteadyResult>> CtrlLineScanSteadyResult = new List<List<TapScanSteadyResult>>();
            List<TapScanSteadyResult> tmp_CtrlLineBestTapValue = new List<TapScanSteadyResult>();

            for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++) //目前16根数据线
            {
                CtrlLineScanSteadyResult.Add(new List<TapScanSteadyResult>());
                int StartTapIndex = 0;
                uint lastShiftTimes = DataLineScanResult[dataLineIndex, 0];
                int maxWdith = 0;
                int bestTapIndex = 0;
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    if (DataLineScanResult[dataLineIndex, tapIndex] != lastShiftTimes)
                    {
                        CtrlLineScanSteadyResult[dataLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = tapIndex, ShiftTimes = (int)lastShiftTimes });
                        if (maxWdith < (tapIndex - StartTapIndex))
                        {
                            maxWdith = tapIndex - StartTapIndex;
                            bestTapIndex = StartTapIndex;
                        }
                        StartTapIndex = tapIndex;
                        lastShiftTimes = DataLineScanResult[dataLineIndex, tapIndex];
                    }
                }
                TapScanSteadyResult? lastTapScanSteadyResult = CtrlLineScanSteadyResult[dataLineIndex].LastOrDefault();
                if (lastTapScanSteadyResult == null || lastTapScanSteadyResult.Start != StartTapIndex)
                {
                    CtrlLineScanSteadyResult[dataLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = (int)ScanTap_Count, ShiftTimes = (int)DataLineScanResult[dataLineIndex, ScanTap_Count - 1] });
                    if (maxWdith < (ScanTap_Count - StartTapIndex))
                    {
                        maxWdith = (int)(ScanTap_Count - StartTapIndex);
                        bestTapIndex = StartTapIndex;
                    }
                }
                tmp_CtrlLineBestTapValue.Add(new TapScanSteadyResult() { Start = bestTapIndex, End = bestTapIndex + maxWdith, ShiftTimes = (int)DataLineScanResult[dataLineIndex, bestTapIndex] });
            }
            //修正
            List<TapScanSteadyResult> CtrlLineBestTapValue = AdjustTapScanSteadyResult(tmp_CtrlLineBestTapValue,8);
            #endregion

            #endregion

            #region Step5:SaveToFile
            Dictionary<string, List<TapScanSteadyResult>> saveContent = new Dictionary<string, List<TapScanSteadyResult>>();
            saveContent.Add("ctrlline", CtrlLineBestTapValue);
            BoardInteractionDelayTap_SaveToFile(caliResultSavedFileName, saveContent);
            #endregion

            #region Step6:Send Cali Data

            BoardInteractionDelay_Send_PcieBd((int)ScanTap_Step, CtrlLineBestTapValue);
            #endregion
        }

    }
}
