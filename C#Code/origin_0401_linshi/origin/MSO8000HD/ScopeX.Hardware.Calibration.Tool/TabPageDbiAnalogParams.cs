using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageDbiAnalogParams : UserControl, IMainFormTabPage
    {
        public TabPageDbiAnalogParams()
        {
            InitializeComponent();
            dataGridView1.RowCount = 4;
            comboBoxYScale.Items.Clear();
            for (int subBandIndex = 0; subBandIndex < 4; subBandIndex++)
            {
                dataGridView1.Rows[subBandIndex].Cells[0].Value = $"子带{subBandIndex + 1}";
            }
            for (int scaleID = 0; scaleID < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleID++)
            {
                if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] < 1000)
                    comboBoxYScale.Items.Add(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID].ToString() + "uV");
                else if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] < 1_000_000)
                    comboBoxYScale.Items.Add((AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] / 1_000) + "mV");
                else
                    comboBoxYScale.Items.Add((AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] / 1_000_000) + "V");
            }
            comboBoxCaliDataChannelImpedance.SelectedIndex = 1;
            comboBoxBandMode.SelectedIndex = 0;
            comboBoxYScale.SelectedIndex = 0;
        }
        public CaliDataType CaliDataType { get => CaliDataType.DbiAnalogParams; }
        private int currBandMode = 0;
        private int yScaleIndex = 0;
        private int currChannelIndex = 0;
        public void RefreshData()
        {
            for (int subBandIndex = 0; subBandIndex < 4; subBandIndex++)
            {
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.AnalogChannelGain].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].AnalogChannelGain.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.IntDiscardDots].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].IntDiscardDots.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.SubbandGain].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].SubbandGain.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.BiasPreceding].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].BiasPreceding.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.BiasPreceding_3Div].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].BiasPreceding_3Div.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.OffsetPosterior].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].OffsetPosterior.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.OffsetPosterior_3Div].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].OffsetPosterior_3Div.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.Gain_FineByAdc1ByTenThousand].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].Gain_FineByAdc1ByTenThousand.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.Gain_FineByAdc2ByTenThousand].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].Gain_FineByAdc2ByTenThousand.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.Gain_FineByFpgaThousand].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].Gain_FineByFpgaThousand.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.Reserved1].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].Reserved1.ToString();
                dataGridView1.Rows[subBandIndex].Cells[(int)ColumnTypes.Reserved2].Value = DbiAnalogParams.Default[currBandMode, currChannelIndex, yScaleIndex, subBandIndex].Reserved2.ToString();
            }
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            if (currInstrument != null)
            {
                comboBoxCaliDataChannelSelectedChannel.Items.Clear();
                comboBoxBaselineCaliChannelSelected.Items.Clear();
                for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount; channelID++)
                {
                    comboBoxBaselineCaliChannelSelected.Items.Add($"CH{channelID + 1}");
                    comboBoxCaliDataChannelSelectedChannel.Items.Add($"CH{channelID + 1}");
                }
                comboBoxBaselineCaliChannelSelected.Items.Add("All");
                comboBoxCaliDataChannelSelectedChannel.Items.Add("All");
                comboBoxBaselineCaliChannelSelected.SelectedIndex = 0;
                comboBoxCaliDataChannelSelectedChannel.SelectedIndex = 0;

                if (instrumentInteract != null)
                {
                    string AnaloChannelgCaliMemo = "";

                    string cmdStr = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData) + $"? AnalogChannelCaliMemo";
                    currInstrument!.WriteString(cmdStr);
                    AnaloChannelgCaliMemo = currInstrument!.ReadString();

                    richTextBoxAnalogCaliMemo.Text = AnaloChannelgCaliMemo;
                }
            }
            buttonAutoCaliBaselineAnd3DIV.Enabled = (currInstrument != null);
            buttonCancelAutoCaliBaseline3Div.Visible = false;
            RefreshData();
        }

        private void comboBoxBandMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            currBandMode = comboBoxBandMode.SelectedIndex;
            RefreshData();
        }

        private void comboBoxCaliDataChannelSelectedChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            currChannelIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            RefreshData();
        }

        private void buttonCaliData_LoadDefualtValue_Click(object sender, EventArgs e)
        {

        }

        private void buttonCaliData_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.DbiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.DbiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void SendGridData(int RowIndex, int ColumnIndex, bool bNeedSend)
        {
            DataGridViewCell currCell = dataGridView1.Rows[RowIndex].Cells[ColumnIndex];
            UInt32 data = 0;
            UInt32.TryParse(currCell.Value.ToString(), out data);
            ColumnTypes enumColumn = (ColumnTypes)ColumnIndex;
            //AcqSyncItem 是结构，不是引用类型的，之后需要拷贝
            int currChannelID = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            DbiAnalogChannelSubbandItem currItem = DbiAnalogParams.Default[currBandMode, currChannelID, yScaleIndex, RowIndex];

            UInt32 oldData = enumColumn switch
            {
                ColumnTypes.AnalogChannelGain => currItem.AnalogChannelGain,
                ColumnTypes.IntDiscardDots => currItem.IntDiscardDots,
                ColumnTypes.SubbandGain => currItem.SubbandGain,
                ColumnTypes.BiasPreceding => currItem.BiasPreceding,
                ColumnTypes.BiasPreceding_3Div => currItem.BiasPreceding_3Div,
                ColumnTypes.OffsetPosterior => currItem.OffsetPosterior,
                ColumnTypes.OffsetPosterior_3Div => currItem.OffsetPosterior_3Div,
                ColumnTypes.Gain_FineByAdc1ByTenThousand => currItem.Gain_FineByAdc1ByTenThousand,
                ColumnTypes.Gain_FineByAdc2ByTenThousand => currItem.Gain_FineByAdc2ByTenThousand,
                ColumnTypes.Gain_FineByFpgaThousand => currItem.Gain_FineByFpgaThousand,
                ColumnTypes.Reserved1 => currItem.Reserved1,
                ColumnTypes.Reserved2 => currItem.Reserved2,
                _ => data

            };
            if (oldData == data) //判断新旧值，避免连续发送
                return;
            switch (enumColumn)
            {
                case ColumnTypes.AnalogChannelGain: currItem.AnalogChannelGain = data; currCell.Value = currItem.AnalogChannelGain.ToString(); break;
                case ColumnTypes.IntDiscardDots: currItem.IntDiscardDots = data; currCell.Value = currItem.IntDiscardDots.ToString(); break;
                case ColumnTypes.SubbandGain: currItem.SubbandGain = data; currCell.Value = currItem.SubbandGain.ToString(); break;
                case ColumnTypes.BiasPreceding: currItem.BiasPreceding = data; currCell.Value = currItem.BiasPreceding.ToString(); break;
                case ColumnTypes.BiasPreceding_3Div: currItem.BiasPreceding_3Div = data; currCell.Value = currItem.BiasPreceding_3Div.ToString(); break;
                case ColumnTypes.OffsetPosterior: currItem.OffsetPosterior = data; currCell.Value = currItem.OffsetPosterior.ToString(); break;
                case ColumnTypes.OffsetPosterior_3Div: currItem.OffsetPosterior_3Div = data; currCell.Value = currItem.OffsetPosterior_3Div.ToString(); break;
                case ColumnTypes.Gain_FineByAdc1ByTenThousand: currItem.Gain_FineByAdc1ByTenThousand = data; currCell.Value = currItem.Gain_FineByAdc1ByTenThousand.ToString(); break;
                case ColumnTypes.Gain_FineByAdc2ByTenThousand: currItem.Gain_FineByAdc2ByTenThousand = data; currCell.Value = currItem.Gain_FineByAdc2ByTenThousand.ToString(); break;
                case ColumnTypes.Gain_FineByFpgaThousand: currItem.Gain_FineByFpgaThousand = data; currCell.Value = currItem.Gain_FineByFpgaThousand.ToString(); break;
                case ColumnTypes.Reserved1: currItem.Reserved1 = data; currCell.Value = currItem.Reserved1.ToString(); break;
                case ColumnTypes.Reserved2: currItem.Reserved2 = data; currCell.Value = currItem.Reserved2.ToString(); break;
            };
            DbiAnalogParams.Default[currBandMode, currChannelID, yScaleIndex, RowIndex] = currItem;
            if (bNeedSend && currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.DbiAnalogParams);

        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_AcqSync_AutoSend.Checked);
        }
        private enum ColumnTypes
        {
            AnalogChannelGain = 1,//金老师的那个通道
            IntDiscardDots = 2,//整数丢点
            SubbandGain = 3,//邱老师的模拟通道
            BiasPreceding = 4,//Bias
            BiasPreceding_3Div = 5,//Bias 3Div
            OffsetPosterior = 6,//Offset 
            OffsetPosterior_3Div = 7,//Offset 3Div
            Gain_FineByAdc1ByTenThousand = 8,
            Gain_FineByAdc2ByTenThousand = 9,
            Gain_FineByFpgaThousand = 10,
            Reserved1 = 11,
            Reserved2 = 12,
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= (int)ColumnTypes.IntDiscardDots; columnIndex++)
                    SendGridData(rowIndex, columnIndex, false);
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.DbiAnalogParams);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void SaveToMemory()
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= (int)ColumnTypes.IntDiscardDots; columnIndex++)
                    SendGridData(rowIndex, columnIndex, false);
            }
        }
        private void comboBoxYScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            yScaleIndex = comboBoxYScale.SelectedIndex;
            RefreshData();
        }
        private bool CheckCancelClicked(ref bool bCanceled)
        {
            Application.DoEvents();
            if (cancelTokenSrc != null)
            {
                try
                {
                    cancelTokenSrc.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    bCanceled = true;
                }
            }
            return bCanceled;
        }
        protected CancellationTokenSource? cancelTokenSrc = null;
        private void buttonAutoCaliBaselineAnd3DIV_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请确保通道无输入信号，并确信硬件连接好，你确认吗？", "提示", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;
            cancelTokenSrc = new CancellationTokenSource();
            buttonAutoCaliBaselineAnd3DIV.Enabled = false;
            buttonCancelAutoCaliBaseline3Div.Visible = !buttonAutoCaliBaselineAnd3DIV.Enabled;
            Application.DoEvents();
            Dictionary<string, string> oldDebugVariantSetting = TabPageValueSetting.ReadbackDebugVariants(currInstrument!);
            List<int> CaliChannelList = new List<int>();
            if (comboBoxBaselineCaliChannelSelected.SelectedIndex == comboBoxBaselineCaliChannelSelected.Items.Count - 1)
            {
                for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount; channelID++)
                    CaliChannelList.Add(channelID);
            }
            else
                CaliChannelList.Add(comboBoxBaselineCaliChannelSelected.SelectedIndex);
            string scpiCmdPrimitive = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmdPrimitive += " DebugVariant,";
            scpiCmdPrimitive = "bEnable_AdcDataDebugMode:true,bEnable_DigitTrigger:false,bEnable_AcqbdInterpolation:false,bEnable_AcqBd_Afc:false,bEnable_AcqBd_Pfc:false,bEnable_CorrectTiAdc:false,bEnable_ProcBd_Average:false";
            scpiCmdPrimitive += $",bEnable_Dbi_IntDelay:false,bEnable_Dbi_AmpFreqCoef:false,bEnable_Dbi_AntiImageCoef:false,bEnable_Dbi_FractionaryDelayCoef:false,bEnable_Dbi_LocalOscillatorCoef:false,bEnable_Dbi_MultiRadioInterpolation:false,bEnable_Dbi_OverlapPhaseFreqDelayCoef:false,bEnable_Dbi_PhaseFreqCoef:false,bEnable_Dbi_IsSubbandMergeMode:false,iDbi_DebugChannelID:[CURR_iDbi_DebugChannelID]";
            int AdcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits);
            int BWMode_FullIs0 = 0;
            bool bCanceled = false;
            int overtimeByMs = 5000;
            foreach (int currChannelID in CaliChannelList)
            {
                if (bCanceled)
                    break;
                for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount; channelID++)
                    currInstrument!.WriteString($":CHAN{channelID + 1}:DISP OFF");

                currInstrument!.WriteString($":CHAN{currChannelID + 1}:DISP ON");

                string debugVarintSetting = scpiCmdPrimitive.Replace("[CURR_iDbi_DebugChannelID]", currChannelID.ToString());
                currInstrument!.WriteString(debugVarintSetting);
                for (AnaChnlScaleIndex yScaleID = AnaChnlScaleIndex.Lv2m; yScaleID <= AnaChnlScaleIndex.Lv1; yScaleID++)
                {
                    double yScalueByV = ServerDomainConstants.AnalyChannelYScaleTable[yScaleID] * 1.0 / 1e6;//uV==>V
                    currInstrument!.WriteString($":CHAN{currChannelID + 1}:SCAL {yScalueByV}");
                    currInstrument!.WriteString($":CHAN{currChannelID + 1}:BIAS 0");
                    currInstrument!.WriteString($":CHAN{currChannelID + 1}:OFFS 0");
                    Thread.Sleep(1500);
                    Stopwatch stopwatch = new Stopwatch();
                    ushort theoryOkValue = (ushort)(AdcMaxValue / 2);
                    #region Baseline
                    stopwatch.Start();
                    bool[] baseline4SubBandIsOK = { false, false, false, false };
                    theoryOkValue = (ushort)(AdcMaxValue / 2);
                    while (baseline4SubBandIsOK.Select(s => s = false).Count() > 0 && stopwatch.ElapsedMilliseconds < overtimeByMs)
                    {
                        if (CheckCancelClicked(ref bCanceled))
                            break;
                        List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000, 4);//4,4个子带
                        if (allChannelData == null)
                            continue;
                        for (int subBandIndex = 0; subBandIndex < 4; subBandIndex++)
                        {
                            if (CheckCancelClicked(ref bCanceled))
                                break;
                            if (baseline4SubBandIsOK[subBandIndex])
                                continue;
                            ushort average = (ushort)allChannelData[subBandIndex]!.Average<ushort>(s => s);
                            ushort max = (ushort)allChannelData[subBandIndex]!.Max<ushort>(s => s);
                            ushort min = (ushort)allChannelData[subBandIndex]!.Min<ushort>(s => s);
                            DbiAnalogChannelSubbandItem currItem = DbiAnalogParams.Default[BWMode_FullIs0, currChannelID, yScaleIndex, subBandIndex];

                            if (min == 0)
                            {
                                currItem.OffsetPosterior = currItem.OffsetPosterior + 1000;
                            }
                            else if (max == AdcMaxValue - 1)
                            {
                                currItem.OffsetPosterior = currItem.OffsetPosterior - 1000;
                            }
                            else
                            {
                                if (average > (theoryOkValue))
                                {
                                    currItem.OffsetPosterior = currItem.OffsetPosterior - 100;

                                }
                                else if (average < (theoryOkValue))
                                {
                                    currItem.OffsetPosterior = currItem.OffsetPosterior + 100;
                                }
                                else
                                {
                                    baseline4SubBandIsOK[subBandIndex] = true;

                                }
                            }
                            DbiAnalogParams.Default[BWMode_FullIs0, currChannelID, yScaleIndex, subBandIndex] = currItem;
                        }
                        if (bCanceled)
                            break;
                        bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.DbiAnalogParams);
                    }
                    stopwatch.Stop();
                    if (bCanceled)
                        break;
                    #endregion

                    #region 3Div
                    currInstrument!.WriteString($":CHAN{currChannelID + 1}:OFFS 3");
                    Thread.Sleep(500);
                    bool[] threeDiv4SubBandIsOK = { false, false, false, false };
                    stopwatch.Restart();
                    theoryOkValue = (ushort)(AdcMaxValue / 2 + ServerDomainConstants.SAMPS_PER_YDIV * 3);
                    while (threeDiv4SubBandIsOK.Select(s => s = false).Count() > 0 && stopwatch.ElapsedMilliseconds < overtimeByMs)
                    {
                        if (CheckCancelClicked(ref bCanceled))
                            break;
                        List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000, 4);//4,4个子带
                        if (allChannelData == null)
                            continue;
                        for (int subBandIndex = 0; subBandIndex < 4; subBandIndex++)
                        {
                            if (CheckCancelClicked(ref bCanceled))
                                break;
                            if (!baseline4SubBandIsOK[subBandIndex])
                                continue;
                            if (threeDiv4SubBandIsOK[subBandIndex])
                                continue;
                            ushort average = (ushort)allChannelData[subBandIndex]!.Average<ushort>(s => s);
                            ushort max = (ushort)allChannelData[subBandIndex]!.Max<ushort>(s => s);
                            ushort min = (ushort)allChannelData[subBandIndex]!.Min<ushort>(s => s);
                            DbiAnalogChannelSubbandItem currItem = DbiAnalogParams.Default[BWMode_FullIs0, currChannelID, yScaleIndex, subBandIndex];

                            if (min == 0)
                            {
                                currItem.OffsetPosterior_3Div = currItem.OffsetPosterior_3Div + 1000;
                            }
                            else if (max == AdcMaxValue - 1)
                            {
                                currItem.OffsetPosterior_3Div = currItem.OffsetPosterior_3Div - 1000;
                            }
                            else
                            {
                                if (average > (theoryOkValue))
                                {
                                    currItem.OffsetPosterior_3Div = currItem.OffsetPosterior_3Div - 100;

                                }
                                else if (average < (theoryOkValue))
                                {
                                    currItem.OffsetPosterior_3Div = currItem.OffsetPosterior_3Div + 100;
                                }
                                else
                                {
                                    threeDiv4SubBandIsOK[subBandIndex] = true;

                                }
                            }
                            DbiAnalogParams.Default[BWMode_FullIs0, currChannelID, yScaleIndex, subBandIndex] = currItem;
                        }
                        if (bCanceled)
                            break;
                        bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.DbiAnalogParams);
                    }
                    stopwatch.Stop();
                    #endregion
                }
            }
            buttonAutoCaliBaselineAnd3DIV.Enabled = true;
            buttonCancelAutoCaliBaseline3Div.Visible = !buttonAutoCaliBaselineAnd3DIV.Enabled;
            InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.DbiAnalogParams);
            RefreshData();
            if (!bCanceled)
                MessageBox.Show("校准结束！");
            else
                MessageBox.Show("校准被取消！");
        }

        private void buttonCancelAutoCaliBaseline3Div_Click(object sender, EventArgs e)
        {
            cancelTokenSrc?.Cancel();
        }

        private void buttonCopyCh1ToOtherChannel_Click(object sender, EventArgs e)
        {
            for(int currBandMode = 0; currBandMode < 2; currBandMode++)
            {
                for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
                {
                    for (int yScaleIndex = 0; yScaleIndex < 2; yScaleIndex++)
                    {
                        DbiAnalogChannelSubbandItem ch1Item = DbiAnalogParams.Default[currBandMode, 0, yScaleIndex, subbandIndex];
                        for (int channelID = 1; channelID < 4; channelID++)
                        {
                            DbiAnalogChannelSubbandItem newItem = new DbiAnalogChannelSubbandItem()
                            {
                                AnalogChannelGain = ch1Item.AnalogChannelGain,
                                IntDiscardDots = ch1Item.IntDiscardDots,
                                SubbandGain = ch1Item.SubbandGain,
                                BiasPreceding = ch1Item.BiasPreceding,
                                BiasPreceding_3Div = ch1Item.BiasPreceding_3Div,
                                OffsetPosterior = ch1Item.OffsetPosterior,
                                OffsetPosterior_3Div = ch1Item.OffsetPosterior_3Div,
                                Gain_FineByAdc1ByTenThousand = ch1Item.Gain_FineByAdc1ByTenThousand,
                                Gain_FineByAdc2ByTenThousand = ch1Item.Gain_FineByAdc2ByTenThousand,
                                Gain_FineByFpgaThousand = ch1Item.Gain_FineByFpgaThousand,
                                Reserved1 = ch1Item.Reserved1,
                                Reserved2 = ch1Item.Reserved2
                            };
                            DbiAnalogParams.Default[currBandMode, channelID, yScaleIndex, subbandIndex] = newItem;
                        }
                    }
                }
            }
            if (currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.DbiAnalogParams);
            RefreshData();
        }
    }
}
