using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using ScopeX.Hardware.Driver;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        private void BoardInteractionDelay_Reset_ProcBd()
        {
            HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 0);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 0);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Sel, 1);
        }

        private void BoardInteractionDelay_Start_ProcBd()
        {
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Tap_Gap, (UInt32)ScanTap_Step);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Tap_Start, ScanTap_Start);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Tap_Stop, ScanTap_Start + (ScanTap_Count - 1) * ScanTap_Step);
            // 开始扫窗控制
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Scan_Start, 0);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Scan_Start, 1);
            HdIO.WriteReg(ProcBdReg.W.BoardSync_Scan_Start, 0);
        }

        private String BoardInteractionDelay_Send_ProcBd_Acq(AcqBdNo acqBdNo, int ScanTap_Step, List<TapScanSteadyResult>? DataLineBestTapValue, List<TapScanSteadyResult>? CtrlLineBestTapValue, UInt32 Index, Boolean IsHigh)
        {
            ProcBdReg.W BoardSync_Io_Delay_Value_Acq;
            ProcBdReg.W BoardSync_Pattern_Shift_Value_Acq;
            if (GetAcqBdDescription(acqBdNo, out String result) && result.Split(',').Length == 2)
            {
                try
                {
                    BoardSync_Io_Delay_Value_Acq = (ProcBdReg.W)Enum.Parse(typeof(ProcBdReg.W), result.Split(',')[0]);
                    BoardSync_Pattern_Shift_Value_Acq = (ProcBdReg.W)Enum.Parse(typeof(ProcBdReg.W), result.Split(',')[1]);
                }
                catch (Exception e)
                {
                    return String.Format(SendError, acqBdNo, $"{result}\n{e.Message}\n{e.StackTrace}");
                }
            }
            else
            {
                return String.Format(SendError, acqBdNo, $"{result}");
            }

            HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Sel, 0);
            Index = IsHigh ? Index + (UInt32)0x01 : Index;
            //软件发送每条lane的最终TAP值,先发数据线
            if (DataLineBestTapValue != null)
            {
                for (int dataLineIndex = 0; dataLineIndex < DataLineBestTapValue.Count; dataLineIndex++)
                {
                    //Tap Value
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Addr, (uint)dataLineIndex);              //先发地址
                    HdIO.WriteReg(BoardSync_Io_Delay_Value_Acq, (uint)((DataLineBestTapValue[dataLineIndex].Start + DataLineBestTapValue[dataLineIndex].Width / 2) * ScanTap_Step + ScanTap_Start));     //再发数据
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Ld_En, (UInt32)0x01 << (Int32)Index);
                    HdIO.Sleep(1);
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                    //ShiftTimes
                    HdIO.WriteReg(BoardSync_Pattern_Shift_Value_Acq, (uint)DataLineBestTapValue[dataLineIndex].ShiftTimes);     //再发数据
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex | (0x800 * Index));
                    //HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex | 0x80);         //通过控制地址的第7位拉高来生效数据
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex | (0x80 + 0x800 * Index));         //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)dataLineIndex | (0x800 * Index));
                }
            }

            //再发控制线

            if (CtrlLineBestTapValue != null)
            {
                for (int ctrlLineIndex = 0; ctrlLineIndex < CtrlLineBestTapValue.Count; ctrlLineIndex++)
                {
                    //Tap Value
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Addr, ((uint)ctrlLineIndex) | 0x80);            //先发地址，0x80=代表是控制线
                    HdIO.WriteReg(BoardSync_Io_Delay_Value_Acq, (uint)((CtrlLineBestTapValue[ctrlLineIndex].Start + CtrlLineBestTapValue[ctrlLineIndex].Width / 2) * ScanTap_Step + ScanTap_Start));
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Ld_En, (UInt32)0x01 << (Int32)Index);
                    HdIO.Sleep(1);
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                    //ShiftTimes
                    HdIO.WriteReg(BoardSync_Pattern_Shift_Value_Acq, ((uint)CtrlLineBestTapValue[ctrlLineIndex].ShiftTimes));     //再发数据
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)ctrlLineIndex |0/* (0x800 * Index)*/);
                    //HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)ctrlLineIndex | 0x80);         //通过控制地址的第7位拉高来生效数据
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)ctrlLineIndex | (0x80 /*+ 0x800 * Index*/));         //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Pattern_Shift_Addr, (uint)ctrlLineIndex |0 /*(0x800 * Index)*/);
                }
            }

            return " Send Completed!";
        }

        private void BoardInteractionDelay_DoCali_ProcBd_Acq(AcqBdNo acqBdNo, UInt32 DataLineCount, UInt32 CtrlLineCount, String Path, UInt32 Index, Boolean ResultHigh)
        {
            if (!Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime)
            {
                List<TapScanSteadyResult> DataLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                List<TapScanSteadyResult> CtrlLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                if (BoardInteractionDelayTap_ReadFromFile(Path, DataLineBestTapValue_FromFile, CtrlLineBestTapValue_FromFile))
                    BoardInteractionDelay_Send_ProcBd_Acq(acqBdNo, (int)ScanTap_Step, DataLineBestTapValue_FromFile, CtrlLineBestTapValue_FromFile, Index, ResultHigh);
                return;
            }
            //if (!HdIO.CheckRegisterValue((uint)ProcBdReg.R.BoardSync_Scan_Finish, 0x01, 0x01, 1000))
            //    return;
            #region  Step2:Read Ram
            UInt32[/*LineIndex*/,/*TapValue*/] DataLineScanResult = new UInt32[DataLineCount, ScanTap_Count];//FPGA 实际多
            UInt32[/*LineIndex*/,/*TapValue*/] CtrlLineScanResult = new UInt32[CtrlLineCount, ScanTap_Count];
            for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++)
            {
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    if (!ResultHigh)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(dataLineIndex * ScanTap_Count + tapIndex) | Index * 0x800);                     //reg_ram_rd_addr_pro 先读数据线地址
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);                                        //reg_ram_rd_en_pro
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);                                    //reg_ram_rd_en_pro
                        DataLineScanResult[dataLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) & 0xff;
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(dataLineIndex * ScanTap_Count + tapIndex) | (Index * 0x800));                     //reg_ram_rd_addr_pro 先读数据线地址
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);                                        //reg_ram_rd_en_pro
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        //高位不需要重新读取，直接用上次读取的移位
                        DataLineScanResult[dataLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) >> BoardInteractionDelay_Data;
                    }




                }
            }
            for (int ctrlLineIndex = 0; ctrlLineIndex < CtrlLineCount; ctrlLineIndex++)
            {
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    //if (!ResultHigh)
                    //{
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index + 1) * 0x800));
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);

                    //    CtrlLineScanResult[ctrlLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) & 0x3f;  //acq1,2,3,4                 //reg_scan_result_pro
                    //    CtrlLineScanResult[ctrlLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.LowPower_AcqBoardPowerCtrlRead) & 0x3f; //acq5,6,7,8         //reg_scan_result_pro
                    //                                                                                                                                                                                                                                            //
                    //}
                    //else
                    //{
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index + 1) * 0x800));
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                    //    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                    //    //高位不需要重新读取，直接用上次读取的移位
                    //    CtrlLineScanResult[ctrlLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) >> BoardInteractionDelay_Ctrl;                   //reg_scan_result_pro        
                    //}
                    //cij_调试代码_需要软件同学优化
                    UInt32 Index2 = 0;
                    if (acqBdNo == AcqBdNo.B0)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) & 0xf;  //acq1                 //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B1)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) >> 4) & 0xf;  //acq2                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B2)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) >> 8) & 0xf;  //acq3                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B3)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.BoardSync_Scan_Result) >> 12) & 0xf;  //acq4                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B4)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = HdIO.ReadReg(ProcBdReg.R.LowPower_AcqBoardPowerCtrlRead) & 0xf;  //acq4                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B5)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.LowPower_AcqBoardPowerCtrlRead) >> 4) & 0xf;  //acq4                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B6)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.LowPower_AcqBoardPowerCtrlRead) >> 8) & 0xf;  //acq4                //reg_scan_result_pro
                    }
                    else if (acqBdNo == AcqBdNo.B7)
                    {
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_Addr, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | ((Index2 + 1) * 0x800));
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 0);
                        HdIO.WriteReg(ProcBdReg.W.BoardSync_Ram_Rd_En, 1);
                        CtrlLineScanResult[ctrlLineIndex, tapIndex] = (HdIO.ReadReg(ProcBdReg.R.LowPower_AcqBoardPowerCtrlRead) >> 12) & 0xf;  //acq4                //reg_scan_result_pro
                    }
                    else
                    { 
                    
                    }
                }
            }

            #endregion

            #region Step3:手动观察
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DataLine Scan Result:");
            string perLineResult = "";
            for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                perLineResult += tapIndex.ToString().PadLeft(4, '=') + ',';
            stringBuilder.AppendLine($"   CtrlLine    Result:{perLineResult}");
            for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++)
            {
                perLineResult = "";
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    perLineResult += DataLineScanResult[dataLineIndex, tapIndex].ToString().PadLeft(4, ' ') + ",";
                }
                stringBuilder.AppendLine($"   DataLine {dataLineIndex.ToString().PadLeft(2, '_')}_Result:{perLineResult}");
            }
            stringBuilder.AppendLine("CtrlLine Scan Result:");
            perLineResult = "";
            for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                perLineResult += tapIndex.ToString().PadLeft(4, '=') + ',';
            stringBuilder.AppendLine($"   CtrlLine    Result:{perLineResult}");
            for (int ctrlLineIndex = 0; ctrlLineIndex < CtrlLineCount; ctrlLineIndex++)
            {
                perLineResult = "";
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    perLineResult += CtrlLineScanResult[ctrlLineIndex, tapIndex].ToString().PadLeft(4, ' ') + ",";
                }
                stringBuilder.AppendLine($"   CtrlLine {ctrlLineIndex.ToString().PadLeft(2, '_')}_Result:{perLineResult}");
            }
            FileWrite($"AcqProcBorad_ScanResult_AcqBoard_Proc_{ acqBdNo}", stringBuilder.ToString());//后面稳定后需要将日期删除不然文件越来越大越来越多
            //FileWrite($"AcqProcBorad_ScanResult_AcqBoard_{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss_ffff")}_Proc_{ acqBdNo}", stringBuilder.ToString());//后面稳定后需要将日期删除不然文件越来越大越来越多
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}AcqProcBorad_ScanResult_ProcBoard_{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss_ffff")}_Proc_{acqBdNo}.txt", stringBuilder.ToString());
            #endregion

            #region Step4:计算Tap Value
            #region DataLine
            List<List<TapScanSteadyResult>> DataLineScanSteadyResult = new List<List<TapScanSteadyResult>>();
            List<TapScanSteadyResult> tmp_DataLineBestTapValue = new List<TapScanSteadyResult>();

            for (int dataLineIndex = 0; dataLineIndex < DataLineCount; dataLineIndex++) //目前16根数据线
            {
                DataLineScanSteadyResult.Add(new List<TapScanSteadyResult>());
                int StartTapIndex = 0;
                uint lastShiftTimes = DataLineScanResult[dataLineIndex, 0];
                int maxWidth = 0;
                int bestTapIndex = 0;
                //把结果值不变的连续区域分段
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    if (DataLineScanResult[dataLineIndex, tapIndex] != lastShiftTimes)
                    {
                        DataLineScanSteadyResult[dataLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = tapIndex, ShiftTimes = (int)lastShiftTimes });
                        StartTapIndex = tapIndex;
                        lastShiftTimes = DataLineScanResult[dataLineIndex, tapIndex];
                    }
                }
                TapScanSteadyResult? lastTapScanSteadyResult = DataLineScanSteadyResult[dataLineIndex].LastOrDefault();
                //补齐最后一段
                if (lastTapScanSteadyResult == null || lastTapScanSteadyResult.Start != StartTapIndex)
                {
                    DataLineScanSteadyResult[dataLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = (int)ScanTap_Count, ShiftTimes = (int)DataLineScanResult[dataLineIndex, ScanTap_Count - 1] });

                }

                //剔除31
                List<TapScanSteadyResult> removedata = new List<TapScanSteadyResult>();
                foreach (var data in DataLineScanSteadyResult[dataLineIndex])
                {
                    if (data.ShiftTimes == 31)
                    {
                        removedata.Add(data);
                    }
                }
                removedata.ForEach(x => DataLineScanSteadyResult[dataLineIndex].Remove(x));

                //找到最大区间
                foreach (var data in DataLineScanSteadyResult[dataLineIndex])
                {
                    if (maxWidth < (data.End - data.Start))
                    {
                        maxWidth = data.End - data.Start;
                        bestTapIndex = data.Start;
                    }
                }

                tmp_DataLineBestTapValue.Add(new TapScanSteadyResult() { Start = bestTapIndex, End = bestTapIndex + maxWidth, ShiftTimes = (int)DataLineScanResult[dataLineIndex, bestTapIndex] });
            }

            //修正
            UInt32 DataLineOverRangeThresh = 8;
            List<TapScanSteadyResult> DataLineBestTapValue = AdjustTapScanSteadyResult(tmp_DataLineBestTapValue, (int)DataLineOverRangeThresh);
            #endregion
            #region CtrlLine
            //计算控制线对应的tap值和pattern值
            List<List<TapScanSteadyResult>> CtrlLineScanSteadyResult = new List<List<TapScanSteadyResult>>();
            List<TapScanSteadyResult> tmp_CtrlLineBestTapValue = new List<TapScanSteadyResult>();
            for (int ctrlLineIndex = 0; ctrlLineIndex < CtrlLineCount; ctrlLineIndex++) //目前16根数据线
            {
                CtrlLineScanSteadyResult.Add(new List<TapScanSteadyResult>());
                int StartTapIndex = 0;
                uint lastShiftTimes = CtrlLineScanResult[ctrlLineIndex, 0];
                int maxWidth = 0;
                int bestTapIndex = 0;
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    if (CtrlLineScanResult[ctrlLineIndex, tapIndex] != lastShiftTimes)
                    {
                        CtrlLineScanSteadyResult[ctrlLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = tapIndex, ShiftTimes = (int)lastShiftTimes });
                        StartTapIndex = tapIndex;
                        lastShiftTimes = CtrlLineScanResult[ctrlLineIndex, tapIndex];
                    }
                }
                TapScanSteadyResult? lastTapScanSteadyResult = CtrlLineScanSteadyResult[ctrlLineIndex].LastOrDefault();
                if (lastTapScanSteadyResult == null || lastTapScanSteadyResult.Start != StartTapIndex)
                {
                    CtrlLineScanSteadyResult[ctrlLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = (int)ScanTap_Count, ShiftTimes = (int)CtrlLineScanResult[ctrlLineIndex, ScanTap_Count - 1] });

                }

                //剔除15
                List<TapScanSteadyResult> removedata = new List<TapScanSteadyResult>();
                foreach (var data in CtrlLineScanSteadyResult[ctrlLineIndex])
                {
                    if (data.ShiftTimes == 15)
                    {
                        removedata.Add(data);
                    }
                }
                removedata.ForEach(x => CtrlLineScanSteadyResult[ctrlLineIndex].Remove(x));

                //找到最大区间
                foreach (var data in CtrlLineScanSteadyResult[ctrlLineIndex])
                {
                    if (maxWidth < (data.End - data.Start))
                    {
                        maxWidth = data.End - data.Start;
                        bestTapIndex = data.Start;
                    }
                }

                tmp_CtrlLineBestTapValue.Add(new TapScanSteadyResult() { Start = bestTapIndex, End = bestTapIndex + maxWidth, ShiftTimes = (int)CtrlLineScanResult[ctrlLineIndex, bestTapIndex] });
            }
            //修正
            UInt32 CtrlLineOverRangeThresh = 4;
            List<TapScanSteadyResult> CtrlLineBestTapValue = AdjustTapScanSteadyResult(tmp_CtrlLineBestTapValue, (int)CtrlLineOverRangeThresh);
            #endregion

            #endregion

            #region Step5:SaveToFile
            Dictionary<string, List<TapScanSteadyResult>> saveContent = new Dictionary<string, List<TapScanSteadyResult>>();
            saveContent.Add("dataline", DataLineBestTapValue);
            saveContent.Add("ctrlline", CtrlLineBestTapValue);
            BoardInteractionDelayTap_SaveToFile(Path, saveContent);
            #endregion

            #region Step6:Send Cali Data

            var rst = BoardInteractionDelay_Send_ProcBd_Acq(acqBdNo, (int)ScanTap_Step, DataLineBestTapValue, CtrlLineBestTapValue, Index, ResultHigh);
            RecordSendStr(Path, rst);
            #endregion
        }


        private void BoardCtrlLineDelay_RstCali()
        {
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);
           // HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve25, 0x1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_6, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_7, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_7, 0);
            UInt32 ctrlineScanStateAcq1 = 0;
            UInt32 ctrlineScanStateAcq3 = 0;
            UInt32 errTime = 0;
            int caliMaxTimeMs = 10000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < caliMaxTimeMs)
            {
                HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_Line_Scan_Start, 1);
                ctrlineScanStateAcq1 = HdIO.ReadReg((UInt32)AcqBdReg.R.reverse_acq_reverse_rd_reg_0 | 0x40000);
                ctrlineScanStateAcq3 = HdIO.ReadReg((UInt32)AcqBdReg.R.reverse_acq_reverse_rd_reg_0 | 0x42000);

                if (ctrlineScanStateAcq1 == 2 & ctrlineScanStateAcq3 == 2)
                {
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_Line_Scan_Start, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_6, 0);
                    break;
                }
                else if (ctrlineScanStateAcq1 == 3 & ctrlineScanStateAcq3 == 3)
                {
                    HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_Line_Scan_Start, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_6, 0);
                }
                else
                {
                    errTime = errTime + 1;
                }
            }
        }
    }
}

