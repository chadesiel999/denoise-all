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
        private void BoardInteractionDelay_Reset_AcqBd()
        {
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_IO_Reset, 0);
            HdIO.Sleep(10);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_IO_Reset, 1);
            HdIO.Sleep(10);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_IO_Reset, 0);
            HdIO.Sleep(10);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Pattern_Sel, 1);
        }

        private void BoardInteractionDelay_Start_AcqBd()
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Gap, (UInt32)ScanTap_Step);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Start, ScanTap_Start);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Stop, ScanTap_Start + (ScanTap_Count - 1) * ScanTap_Step);

            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Gap   | 0x41000, (UInt32)1);
            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Start | 0x41000, (UInt32)100);
            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Stop  | 0x41000, (UInt32)450);
            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Gap   | 0x45000, (UInt32)1);
            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Start | 0x45000, (UInt32)100);
            //HdIO.WriteReg((UInt32)AcqBdReg.W.BoardSync_Tap_Stop | 0x45000, (UInt32)450);

            // Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Gap, (UInt32)1);
            // Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Start, 400);
            // Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Tap_Stop, 500);
            // 开始扫窗控制

            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Scan_Start, 0);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Scan_Start, 1);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Scan_Start, 0);
        }
        private void BoardTransferDelay_Send_AcqBd(AcqBdNo acqBd, Int32 scanTapStep, List<TapScanSteadyResult>? ctrlLineBestTapValue)
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Pattern_Sel, 0);      //reg_pattern_sel_acq

            if (ctrlLineBestTapValue != null)
            {
                //发控制线
                for (Int32 ctrllineindex = 0; ctrllineindex < ctrlLineBestTapValue.Count; ctrllineindex++)
                {
                    //Tap Value
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Addr, acqBd, ((UInt32)ctrllineindex) | 0x80);            //先发地址，0x80=代表是控制线
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, acqBd, (UInt32)((ctrlLineBestTapValue[ctrllineindex].Start + ctrlLineBestTapValue[ctrllineindex].Width / 2) * scanTapStep + ScanTap_Start));
                    // Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, acqBd, 500);
                    //UInt32 aaa = (UInt32)((ctrlLineBestTapValue[ctrllineindex].Start + ctrlLineBestTapValue[ctrllineindex].Width / 2) * scanTapStep + ScanTap_Start);
                    //if (acqBd == AcqBdNo.B0)
                    //{
                    //    aaa = 200;
                    //}
                    //if (acqBd == AcqBdNo.B7)
                    //{
                    //    aaa = 200;
                    //}
                    //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, acqBd, aaa);
                    //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Value, 200);
                    HdIO.Sleep(5);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x1);
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                    HdIO.Sleep(1);

                    //ShiftTimes
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Value, acqBd, (UInt32)ctrlLineBestTapValue[ctrllineindex].ShiftTimes);
                    HdIO.Sleep(1);
                    //神仙都难看懂的代码和逻辑
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex));     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex) | 0x80);     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex));     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                }
                if (acqBd == AcqBdNo.B0)
                {
                    HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_5, (UInt32)ctrlLineBestTapValue[5].ShiftTimes);
                }
            }
        }

        private void BoardInteractionDelay_DoCali_AcqBd(AcqBdNo acqBd, UInt32 LineCount, String Path)
        {
            if (!Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime)
            {
                List<TapScanSteadyResult> CtrlLineBestTapValue_FromFile = new List<TapScanSteadyResult>();
                if (BoardInteractionDelayTap_ReadFromFile(Path, null, CtrlLineBestTapValue_FromFile))
                    BoardTransferDelay_Send_AcqBd(acqBd, (int)ScanTap_Step, CtrlLineBestTapValue_FromFile);
                return;
            }
            //if (!HdIO.CheckRegisterValue(AcqBdReg.R.BoardSync_Scan_Finish, 0x01, 0x01, 1000))
            //{
            //    return;
            //}
            #region Step2:Read Ram
            UInt32[/*LineIndex*/,/*TapValue*/] CtrlLineScanResult = new UInt32[LineCount, ScanTap_Count];
            for (int ctrlLineIndex = 0; ctrlLineIndex < LineCount; ctrlLineIndex++)
            {
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Ram_Rd_Addr, acqBd, (uint)(ctrlLineIndex * ScanTap_Count + tapIndex) | 0x800);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Ram_Rd_En, acqBd, 0);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Ram_Rd_En, acqBd, 1);
                    CtrlLineScanResult[ctrlLineIndex, tapIndex] = Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R.BoardSync_Scan_Result, acqBd);
                    //CtrlLineScanResult[ctrlLineIndex, tapIndex] = Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R., acqBd);
                }
            }
            #endregion

            #region Step3:手动观察
            StringBuilder stringBuilder = new StringBuilder();
            string perLineResult = "";
            for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                perLineResult += tapIndex.ToString().PadLeft(4, '=') + ',';
            stringBuilder.AppendLine($"   CtrlLine    Result:{perLineResult}");
            for (int ctrlLineIndex = 0; ctrlLineIndex < LineCount; ctrlLineIndex++)
            {
                perLineResult = "";
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    perLineResult += CtrlLineScanResult[ctrlLineIndex, tapIndex].ToString().PadLeft(4, ' ') + ",";
                }
                stringBuilder.AppendLine($"   CtrlLine {ctrlLineIndex.ToString().PadLeft(2, '_')}_Result:{perLineResult}");
            }
            FileWrite($"AcqProcBorad_ScanResult_AcqBoard_Acq_{acqBd}", stringBuilder.ToString());//后面稳定后需要将日期删除不然文件越来越大越来越多
            //FileWrite($"AcqProcBorad_ScanResult_AcqBoard_{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss_ffff")}_Acq_{acqBd}", stringBuilder.ToString());//后面稳定后需要将日期删除不然文件越来越大越来越多
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}AcqProcBorad_ScanResult_AcqBoard_{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss_ffff")}_Acq_{acqBd}.txt", stringBuilder.ToString());
            #endregion

            #region Step4:计算Tap Value
            #region CtrlLine
            //计算控制线对应的tap值和pattern值
            List<List<TapScanSteadyResult>> CtrlLineScanSteadyResult = new List<List<TapScanSteadyResult>>();
            List<TapScanSteadyResult> tmp_CtrlLineBestTapValue = new List<TapScanSteadyResult>();
            for (int ctrlLineIndex = 0; ctrlLineIndex < LineCount; ctrlLineIndex++)
            {
                CtrlLineScanSteadyResult.Add(new List<TapScanSteadyResult>());
                int StartTapIndex = 0;
                uint lastShiftTimes = CtrlLineScanResult[ctrlLineIndex, 0];
                int maxWdith = 0;
                int bestTapIndex = 0;
                for (int tapIndex = 0; tapIndex < ScanTap_Count; tapIndex++)
                {
                    if (CtrlLineScanResult[ctrlLineIndex, tapIndex] != lastShiftTimes)
                    {
                        CtrlLineScanSteadyResult[ctrlLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = tapIndex, ShiftTimes = (int)lastShiftTimes });
                        if (maxWdith < (tapIndex - StartTapIndex))
                        {
                            maxWdith = tapIndex - StartTapIndex;
                            bestTapIndex = StartTapIndex;
                        }
                        lastShiftTimes = CtrlLineScanResult[ctrlLineIndex, tapIndex];
                        StartTapIndex = tapIndex;
                    }
                }
                TapScanSteadyResult? lastTapScanSteadyResult = CtrlLineScanSteadyResult[ctrlLineIndex].LastOrDefault();
                if (lastTapScanSteadyResult == null || lastTapScanSteadyResult.Start != StartTapIndex)
                {
                    CtrlLineScanSteadyResult[ctrlLineIndex].Add(new TapScanSteadyResult() { Start = StartTapIndex, End = (int)ScanTap_Count, ShiftTimes = (int)CtrlLineScanResult[ctrlLineIndex, ScanTap_Count - 1] });
                    if (maxWdith < (ScanTap_Count - StartTapIndex))
                    {
                        maxWdith = (int)(ScanTap_Count - StartTapIndex);
                        bestTapIndex = StartTapIndex;
                    }
                }
                tmp_CtrlLineBestTapValue.Add(new TapScanSteadyResult() { Start = bestTapIndex, End = bestTapIndex + maxWdith, ShiftTimes = (int)CtrlLineScanResult[ctrlLineIndex, bestTapIndex] });
            }
            //修正
            UInt32 CtrlLineOverRangeThresh = 4;
            List<TapScanSteadyResult> CtrlLineBestTapValue = AdjustTapScanSteadyResult(tmp_CtrlLineBestTapValue, (int)CtrlLineOverRangeThresh);
            //( 最后一根线）是板间（触发）同步专用线，不同采集板的第六根线ShiftTimes必须相同，可以是任意值，这里HL更喜欢0，所以选择0
            if (CtrlLineBestTapValue.Count > 0)
            {
                CtrlLineBestTapValue[CtrlLineBestTapValue.Count - 1].ShiftTimes = 0;
            }
            #endregion
            #endregion

            #region Step5:Save To File
            Dictionary<string, List<TapScanSteadyResult>> saveContent = new Dictionary<string, List<TapScanSteadyResult>>();
            saveContent.Add("ctrlline", CtrlLineBestTapValue);
            BoardInteractionDelayTap_SaveToFile(Path, saveContent);
            #endregion

            #region Step6:Send Cali Data
            BoardTransferDelay_Send_AcqBd(acqBd, (int)ScanTap_Step, CtrlLineBestTapValue);
            #endregion
        }

       
    }
}
