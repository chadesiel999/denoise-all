using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        private List<Byte> TAPList { get; } = new List<Byte>()
        {
            16,
            17,
            17,
            17,
            17,
            17,
            17,
            17,//
            16,
            16,
            17,
            17,
            17,
            17,
            18,
            17,
            16,
            17,
            16,
            16,
            16,
            17,
            17,
            17,
            15,
            16,
            17,
            17,
            17,
            18,
            17,
            17,
            17,
            17,
            18,
            17,
            16,
            17,
            17,
            17,
            17,
            17,
            17,
            17,
            18,
            17,
            17,
            17,
            17,
            17,
            18,
            16,
            16,
            16,
            17,
            17,
            18,
            17,
            16,
            16,
            16,
            16,
            17,
            17,
            15,
            15,
            16,
            16,
        };

        private void SendToTAP(Byte index, Int32 tap)
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_No_CH, index);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_tap, (uint)tap);

            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_ld, 0);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_ld, 1);
            System.Threading.Thread.Sleep(millisecondsTimeout: 1);
        }

        private void StartSync()
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_start_sync, 1);
        }

        private void StopSync()
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_start_sync, 0);
        }

        private void SetShifer(byte shifter)
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_shifter, shifter);
        }

        private void SetNoCH(byte No_ch)
        {
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.ADC_Rx_adc_serdes_No_CH, No_ch);
        }

        private UInt32 GetSyncState(out bool bFinished)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            UInt32 finishedMark = Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R.ADC_Rx_adc_serdes_synced, AcqBdNo.B1) & 0x02;
            while (finishedMark == 0 && stopwatch.ElapsedMilliseconds < 100)
            {
                finishedMark = Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R.ADC_Rx_adc_serdes_synced, AcqBdNo.B1) & 0x02;
            }
            bFinished = finishedMark != 0;
            UInt32 state = Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R.ADC_Rx_adc_serdes_synced, AcqBdNo.B1);
            return state & 0x01;
        }

        private Int32 GetTap(UInt32 window, byte NoCh, byte firstch)
        {
            Int32 tap_min = 0;
            Int32 tap_max = 0;
            Int32 bitIndex = 0;

            if (NoCh == firstch)
            {
                for (bitIndex = 4; bitIndex < 32; bitIndex++)
                {
                    if (((0x01 << bitIndex) & (int)window) == 0)
                    {
                        break;
                    }
                }
            }

            for (; bitIndex < 32; bitIndex++)
            {
                if (((0x01 << bitIndex) & (int)window) != 0)
                {
                    tap_min = bitIndex;
                    break;
                }
            }

            for (; bitIndex < 32; bitIndex++)
            {
                if (((0x01 << bitIndex) & (int)window) == 0)
                {
                    tap_max = bitIndex - 1;
                    break;
                }
            }
            return (tap_min + tap_max) / 2;
        }

        private Int32 SearchShifter(byte firstline)
        {
            Int32 i;
            bool bFinished;
            for (i = 0; i < 32; i++)
            {
                StopSync();
                SetShifer((byte)i);
                SetNoCH(firstline);
                StartSync();
                //    System.Threading.Thread.Sleep(3);
                if (GetSyncState(out bFinished) == 1)
                {
                    StopSync();
                    break;
                }
                //此时代表FPGA回读功能异常，不需要再进行后续操作了
                if (!bFinished)
                    return 0;
            }
            return i;
        }

        private Int32 ScanOtherCh(byte Ch_no)
        {
            bool bFinished;
            Int32 window = 0;
            for (Int32 i = 0; i < 32; i++)
            {
                SendToTAP(Ch_no, i);
                StopSync();
                SetNoCH(Ch_no);
                StartSync();
                // System.Threading.Thread.Sleep(2);
                if (GetSyncState(out bFinished) == 1)
                {
                    StopSync();
                    window |= (int)(0x01 << i);
                }
                //此时代表FPGA回读功能异常，不需要再进行后续操作了
                if (!bFinished)
                    return 0;
            }
            return window;
        }
        private void SendTo8Q2500(Int32 adcIndex, UInt32 Address_7bit, UInt32 Commmand_16bit)
        {
            UInt32 tmp = ((0x001 << 23) | (Address_7bit << 16) | Commmand_16bit);//(0x000 << 23)Instruction W(1b'1)/R(1b'0)
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Adc_DataCmdL8, tmp & 0xff);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Adc_DataCmdH16, (tmp >> 8) & 0xffff);

            Hd.CurrProduct.AcqBd.WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 1);
            HdIO.WaitForSpiTransfer(1, 24);
            Hd.CurrProduct.AcqBd.WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 0);
        }
        internal void AdcReceiveStability()
        {
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.UIMessage == null)
                return;
            String fileName = AppDomain.CurrentDomain.BaseDirectory + @"CaliData\AdcRxCaliResult.txt";
            List<int> CaliResult = new List<int>();
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Adc_Reset, 1);
            System.Threading.Thread.Sleep(5);
            Hd.CurrProduct.AcqBd.WriteToAllFpga(AcqBdReg.W.Adc_Reset, 0);

            Hd.CurrProduct.AcqBd.WriteToAllFpga(AcqBdReg.W.ADC_Rx_serdes_reset, 1);
            System.Threading.Thread.Sleep(5);
            Hd.CurrProduct.AcqBd.WriteToAllFpga(AcqBdReg.W.ADC_Rx_serdes_reset, 0);

            var LastTime = DateTime.Now;
            bool bFinished;

            #region 自动扫描并保存
            if (Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows && Hd.UIMessage != null)
            {
                /*-------------------------耿亚通(GYT)添加代码开始-------------------------------------------------*/
                SendTo8Q2500(0, 0x01, 0x0012);         //将ADC配置为码流产生模式,在最后配置ADC时，调整为非流模式
                byte firstLine = 0;
                UInt32[] window_ch = new UInt32[68];
                byte[] tap = new byte[68];


                //初始化window及tap值
                for (int i = 0; i < 68; i++)
                {
                    window_ch[i] = (byte)0;
                    tap[i] = (byte)0;
                    SendToTAP((byte)i, 0);
                }
                //第一路数据扫窗
                for (Int32 i = 0; i < 32; i++)
                {
                    SendToTAP(firstLine, i);      //设置首先进行扫窗的链路序号，并将初始tap值设置为0
                    StopSync();
                    for (Int32 j = 0; j < 32; j++)
                    {
                        StopSync();
                        SetShifer((byte)j);           //设置bitslip数值
                        SetNoCH(firstLine);     //设置通道号
                        StartSync();            //开始扫窗

                        //     System.Threading.Thread.Sleep(3);

                        if (GetSyncState(out bFinished) == 1)
                        {
                            StopSync();
                            window_ch[firstLine] |= (uint)(0x01 << i);
                            break;
                        }
                        //此时代表FPGA回读功能异常，不需要再进行后续操作了
                        if (!bFinished)
                        {
                            ComModel.ErrorCode.ErrorType = ComModel.ErrorType.S_ADC_0007;
                            throw new Exception("FPGA 关于ADC扫窗回读失败，可能是寄存器回读异常！");
                        }
                    }
                }
                tap[firstLine] = (byte)GetTap(window_ch[firstLine], firstLine, firstLine);
                SendToTAP(firstLine, tap[firstLine]);

                Int32 shifter = SearchShifter(firstLine);
                SetShifer((byte)shifter);

                UInt32 firstLinewindow_ch = window_ch[firstLine];

                for (int i = 0; i < 64; i++)
                {
                    window_ch[i] = (UInt32)ScanOtherCh((byte)i);
                    tap[i] = (byte)GetTap(window_ch[i], (byte)i, firstLine);
                    CaliResult.Add(tap[i]);
                }

                var timeLast = (DateTime.Now - LastTime).TotalSeconds;

                #region 保存
                StringBuilder stringBuilder = SaveScanAdcRxWindowsResult(fileName, window_ch, shifter, firstLinewindow_ch);


                String fileNameResult = AppDomain.CurrentDomain.BaseDirectory + @"CaliData\AdcRxCaliResult_Tap.txt";

                if (!Directory.Exists(Path.GetDirectoryName(fileNameResult)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileNameResult));
                if (File.Exists(fileNameResult))
                    File.Delete(fileNameResult);
                StringBuilder stringBuilderResult = new StringBuilder();
                foreach (var v in CaliResult)
                    stringBuilderResult.AppendLine(v.ToString());
                File.WriteAllText(fileNameResult, stringBuilderResult.ToString());

                if ((CaliResult.Max() - CaliResult.Min()) > 6)
                {
                    ComModel.ErrorCode.ErrorType = ComModel.ErrorType.S_ADC_0007;
                    throw new Exception("FPGA 关于ADC扫窗TAP值验证失败，最大最小值差异过大！");
                }

                if (CaliResult.Max() == 0)
                {
                    ComModel.ErrorCode.ErrorType = ComModel.ErrorType.S_ADC_0007;
                    throw new Exception("FPGA 关于ADC扫窗TAP值验证失败，TAP值全为0！");
                }

                #region 调试用
                if (Constants.ENABLE_DEBUG)
                {
                    String debugFileName = AppDomain.CurrentDomain.BaseDirectory + $@"CaliData\AdcRxCaliResult_{DateTime.Now.ToString("yyyy_M_d h_m_s")}.txt";

                    if (!Directory.Exists(Path.GetDirectoryName(debugFileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(debugFileName));
                    if (File.Exists(debugFileName))
                        File.Delete(debugFileName);

                    double fpgaTemperature = SystemMonitor.Default.GetAcqFpgaTemperatureBymCelsius(AcqBdNo.B1).FirstOrDefault().Temperature / 1000D;
                    stringBuilder.Insert(0, $"Cali At FpgaTemperature:{fpgaTemperature}℃{System.Environment.NewLine}");
                    File.WriteAllText(debugFileName, stringBuilder.ToString());
                }
                #endregion

                #endregion
            }
            else
            {
                #region 从文件中读取校准后的数据
                CaliResult.Clear();
                if (File.Exists(fileName))
                {
                    String[] content = File.ReadAllLines(fileName);
                    foreach (string lineStr in content)
                    {
                        if (lineStr.Trim() == "" || lineStr.Trim().StartsWith("//"))
                            continue;
                        CaliResult.Add(int.Parse(lineStr.Trim()));
                    }
                }
                #endregion
            }
            #endregion

            #region 发送
            for (int i = 0; i < CaliResult.Count; i++)
            {
                SendToTAP((byte)i, CaliResult[i]);
                System.Threading.Thread.Sleep(millisecondsTimeout: 1);

            }
            #endregion
        }

        private static StringBuilder SaveScanAdcRxWindowsResult(string fileName, uint[] window_ch, int shifter, uint firstLinewindow_ch)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            if (File.Exists(fileName))
                File.Delete(fileName);

            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                List<string> firstlise = new List<string>();
                Boolean isOk = false;
                for (Int32 i = 0; i < 32; i++)
                {
                    if (((firstLinewindow_ch >> i) & 0x01) == 0)
                    {
                        firstlise.Add(" x");
                        if (isOk)
                        {
                            shifter++;
                            isOk = false;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            shifter--;
                        }
                        isOk = true;
                        firstlise.Add((shifter < 10 ? " " : "") + shifter.ToString());
                    }
                }
                String s_first = "  1   ";
                String sl = "      ";
                for (int i = firstlise.Count - 1; i >= 0; i--)
                {
                    s_first = s_first + firstlise[i] + "   ";
                    sl = sl + (i > 9 ? "" : " ") + i + "   ";
                }
                stringBuilder.AppendLine(sl);
                stringBuilder.AppendLine(s_first);

                for (int i = 1; i < 64; i++)
                {
                    string temp = " " + ((i + 1) > 9 ? "" : " ") + (i + 1).ToString() + "   ";
                    for (Int32 j = 31; j >= 0; j--)
                    {
                        if (((window_ch[i] >> j) & 0x01) == 0)
                        {
                            temp = temp + " x   ";
                        }
                        else
                        {
                            temp = temp + " 1   ";
                        }
                    }
                    stringBuilder.AppendLine(temp);
                }

                //foreach (var v in CaliResult)
                //    stringBuilder.AppendLine(v.ToString());
                File.WriteAllText(fileName, stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                Hd.SysLogger?.Invoke($"ADC扫窗保存文件 异常[ {ex.Message},{ex.StackTrace}]", "Info");
            }
            return stringBuilder;
        }
    }
}
