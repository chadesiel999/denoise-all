using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageCaliData_TiAdc : UserControl, IMainFormTabPage
    {
        public TabPageCaliData_TiAdc()
        {
            InitializeComponent();
            Init();
        }
        public CaliDataType CaliDataType { get => CaliDataType.TiadcPhaseOffsetGainParams; }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        enum TiAdcDataGridColumnIndex : int
        {
            Start = 1,
            Gain = 1,
            Offset = 2,
            Phase = 3,
            End = 3
        }
        private bool bCaliData_TiAdc_InitFinished = false;
        private void Init()
        {
            dataGridViewCaliData_TiAdc.RowCount = CaliConstants.Fixed_PerChannelMergeAdcMaxCount * CaliConstants.Fixed_PerAdcCoreMaxCount;
            for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
            {
                for (int coreIndex = 0; coreIndex < CaliConstants.Fixed_PerAdcCoreMaxCount; coreIndex++)
                {
                    int rowIndex = adcIndex * CaliConstants.Fixed_PerAdcCoreMaxCount + coreIndex;
                    dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[0].ReadOnly = true;
                    if (coreIndex == 0)
                        dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[0].Style.ForeColor = Color.Blue;
                    dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[0].Value = $"Adc{adcIndex + 1}-Core{coreIndex + 1}";
                }
            }
            comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex = 0;
            dataGridViewCaliData_TiAdc.Columns[0].Frozen = true;
            dataGridViewCaliData_TiAdc.Columns[0].DefaultCellStyle.BackColor = Color.DarkGray;

            bCaliData_TiAdc_InitFinished = true;
            RefreshData();
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            if (currInstrument != null)
            {
                //if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI16G || ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI20G)
                //{
                //    comboBoxCaliDataChannelSelectedTiAdc.Items.Clear();
                //    for (int i = 0; i < 10; i++)
                //        comboBoxCaliDataChannelSelectedTiAdc.Items.Add($"子带{i + 1}");
                //    labelTitle.Text = "子带选择：";

                //}
                //else
                //{
                //    comboBoxCaliDataChannelSelectedTiAdc.Items.Clear();
                //    for (int i = 0; i < ServerDomainConstants.AnalogChannelCount; i++)
                //        comboBoxCaliDataChannelSelectedTiAdc.Items.Add($"通道{i + 1}");
                //    labelTitle.Text = "通道选择：";
                //}
                comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex = 0;
            }
        }
        private string GetInputSource()
        {
            int currAcqBdIndex = comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex;
            string InputSourceStr = "";
            if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI16G)
                InputSourceStr = $"输入信号到通道1的子带{ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex] + 1}";
            else if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI20G)
            {
                if (currAcqBdIndex >= 4)
                    InputSourceStr = $"输入信号到通道3的子带{ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex] + 1}";
                else
                    InputSourceStr = $"输入信号到通道1的子带{ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex] + 1}";
            }
            else
                InputSourceStr = $"输入信号到通道{ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex] + 1}";
            return InputSourceStr;
        }
        public void RefreshData()
        {
            //if (!bCaliData_TiAdc_InitFinished)
            //    return;
            //int currAcqBdIndex = comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex;
            //for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
            //{
            //    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
            //    {
            //        int rowIndex = adcIndex * CaliConstants.Fixed_PerAdcCoreMaxCount + coreIndex;
            //        dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[(int)TiAdcDataGridColumnIndex.Gain].Value = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, adcIndex, coreIndex].GainErr;
            //        dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[(int)TiAdcDataGridColumnIndex.Offset].Value = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, adcIndex, coreIndex].OffsetErr;
            //        dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[(int)TiAdcDataGridColumnIndex.Phase].Value = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, adcIndex, coreIndex].PhaseErr;
            //    }
            //}
        }
        private void CaliData_TiAdc_SendGridData(int rowIndex, int columnIndex, bool bNeedSend)
        {
            DataGridViewCell currCell = dataGridViewCaliData_TiAdc.Rows[rowIndex].Cells[columnIndex];
            Int32 data = 0;
            Int32.TryParse(currCell.Value.ToString(), out data);
            //TiAdcDataGridColumnIndex enumColumn = (TiAdcDataGridColumnIndex)columnIndex;
            //int currAcqBdIndex = comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex;
            //int currAdcIndex = rowIndex / CaliConstants.Fixed_PerAdcCoreMaxCount;
            //int currCoreIndex = rowIndex % ServerDomainConstants.PerAdcCoreCount;
            ////TiAdcItem 是结构，不是引用类型的，之后需要拷贝
            //AdcPhaseOffsetGainItem currItem = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, currAdcIndex, currCoreIndex];
            //Int32 oldData = enumColumn switch
            //{
            //    TiAdcDataGridColumnIndex.Gain => currItem.GainErr,
            //    TiAdcDataGridColumnIndex.Offset => currItem.OffsetErr,
            //    TiAdcDataGridColumnIndex.Phase => currItem.PhaseErr,
            //    _ => data

            //};
            //if (oldData == data) //判断新旧值，避免连续发送
            //    return;
            //switch (enumColumn)
            //{
            //    case TiAdcDataGridColumnIndex.Gain: currItem.GainErr = data; currCell.Value = currItem.GainErr.ToString(); break;
            //    case TiAdcDataGridColumnIndex.Offset: currItem.OffsetErr = data; currCell.Value = currItem.OffsetErr.ToString(); break;
            //    case TiAdcDataGridColumnIndex.Phase: currItem.PhaseErr = data; currCell.Value = currItem.PhaseErr.ToString(); break;
            //};
            //TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, currAdcIndex, currCoreIndex] = currItem;
            //if (bNeedSend && currInstrument != null)
            //    InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_PhaseOffsetGain);
        }
        private void CaliData_TiAdc_AllSaveToMemory()
        {
            for (int rowIndex = 0; rowIndex < ServerDomainConstants.PerAnaChannelAdcCount * ServerDomainConstants.PerAdcCoreCount; rowIndex++)
            {
                for (int columnIndex = (int)TiAdcDataGridColumnIndex.Start; columnIndex <= (int)TiAdcDataGridColumnIndex.End; columnIndex++)
                    CaliData_TiAdc_SendGridData(rowIndex, columnIndex, false);
            }
        }

        private void comboBoxCaliDataChannelSelectedTiAdc_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBoxInputSource.Text = GetInputSource();
            RefreshData();
        }

        private void dataGridViewCaliData_TiAdc_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CaliData_TiAdc_SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_TiAdc_AutoSend.Checked);
        }

        private void buttonCaliData_TiAdc_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_CaliLogicValue);
            scpiCmd += " " + "bForceReFind5200AdcSyncWindow," + (checkBoxAutoCaliAdc5200Window.Checked ? "on" : "off");
            currInstrument.WriteString(scpiCmd);

            bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_TiAdc_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("从文件中装载将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_TiAdc_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void buttonCaliData_LoadDefualtValue_TiAdc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("装载缺省值将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            //TiAdc_PhaseOffsetGain.Default.LoadDefaultValue();
            RefreshData();
            if (currInstrument != null)
                InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
        }
        double inputsignalFreqByMHz = 100d;
        double spsByM = 10000d;

        private void buttonAutoCali_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            //int currAcqBdIndex = comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex;

            //if (MessageBox.Show($"请确保输入{textBoxSignalFreqByMHz.Text}MHz的信号到采集板[{currAcqBdIndex + 1}]对应的物理通道,并保证幅度大致合适，时基档在最高采样率挡位下。\r\n你确认吗？", "重要提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
            //    return;
            //inputsignalFreqByMHz = double.Parse(textBoxSignalFreqByMHz.Text);
            //int getDataFromChannelIndex = ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex];
            //spsByM = double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text) * 1000d;//1000 =G sps ==>M sps
            //Stopwatch stopwatch = new Stopwatch();
            //long maxWaitTime = (long)numericUpDownOverTimeOfMin.Value;
            //maxWaitTime *= 60 * 1000;
            //double gainLimit = (double)numericUpDownGainLimit.Value;
            //gainLimit /= 10_000;
            //double phaseLimitByfs = (double)numericUpDownPhaseLimit.Value;

            //stopwatch.Start();
            //bool bOk = false;

            //int NeedStaticTimes = 5;
            //bool bfirst = true;
            //Cursor = Cursors.WaitCursor;
            //#region Step1:Adjust Window,if Phase Delta>100ps
            //var Statics = SineFitStatis(NeedStaticTimes, currInstrument, getDataFromChannelIndex, inputsignalFreqByMHz, spsByM);
            //while (Math.Abs(Statics.Phase_fs) >= 100 * 1000)
            //{
            //    SynsWinChange(currInstrument, currAcqBdIndex, Statics.Phase_fs, bfirst);

            //    bfirst = false;
            //    Statics = SineFitStatis(NeedStaticTimes, currInstrument, getDataFromChannelIndex, inputsignalFreqByMHz, spsByM);
            //}
            //#endregion

            //#region step2:auto Cali Gain and Phase
            //bool PhasOk = false;
            //bool GainOk = false;
            //string errorMessage = "";
            //AdcPhaseOffsetGainItem currItem1 = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 0, 0];
            //currItem1.GainErr = 25000;
            //TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 0, 0] = currItem1;
            //InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_PhaseOffsetGain);
            //RefreshData();
            //Application.DoEvents();
            //Thread.Sleep(3_000);

            //while (true && stopwatch.ElapsedMilliseconds < maxWaitTime)
            //{
            //    Statics = SineFitStatis(NeedStaticTimes, currInstrument, getDataFromChannelIndex, inputsignalFreqByMHz, spsByM);
            //    AdcPhaseOffsetGainItem currItem = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 1, 0];
            //    currItem1 = TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 0, 0];
            //    if (Statics.Phase_fs > 200000)
            //        Statics.Phase_fs = Statics.Phase_fs - 200000;
            //    if (!PhasOk)
            //    {
            //        if (Math.Abs(Statics.Phase_fs) <= phaseLimitByfs)
            //            PhasOk = true;
            //        else
            //        {
            //            if (Statics.Phase_fs > 0)
            //                currItem1.PhaseErr = currItem1.PhaseErr + (int)(Math.Abs(Statics.Phase_fs) / 1000 * 256);
            //            else
            //                currItem.PhaseErr = currItem.PhaseErr + (int)(Math.Abs(Statics.Phase_fs) / 1000 * 256);

            //            if (currItem1.PhaseErr > 0xffff)
            //            {
            //                currItem1.PhaseErr = 0xffff;
            //                errorMessage = "校准失败,已超过相位控制字最大范围！";
            //                bOk = false;
            //                break;
            //            }
            //            if (currItem.PhaseErr < 0)
            //            {
            //                currItem.PhaseErr = 0;
            //                errorMessage = "校准失败,已超过相位控制字最小范围！";
            //                bOk = false;
            //                break;
            //            }
            //        }

            //    }
            //    if (!GainOk)
            //    {
            //        if (Math.Abs(Statics.Gain) <= gainLimit)
            //            GainOk = true;
            //        else
            //        {
            //            currItem.GainErr = currItem.GainErr + (int)(Statics.Gain / 0.02 * 400);
            //            if (currItem.GainErr > 0xffff)
            //            {
            //                currItem.GainErr = 0xffff;
            //                errorMessage = "校准失败,已超过两片ADC增益控制字可调范围！";
            //                bOk = false;
            //                break;
            //            }
            //            if (currItem.GainErr < 0x2000)
            //            {
            //                currItem.GainErr = 0x2000;
            //                currItem.GainErr = 0x2000;
            //                errorMessage = "校准失败,已超过两片ADC增益控制字可调范围！";
            //                bOk = false;
            //                break;
            //            }
            //        }
            //    }

            //    TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 1, 0] = currItem;
            //    TiAdc_PhaseOffsetGain.Default[currAcqBdIndex, 0, 0] = currItem1;
            //    bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_PhaseOffsetGain);
            //    RefreshData();
            //    Application.DoEvents();
            //    if (GainOk)
            //        Thread.Sleep(3_000);
            //    else
            //        Thread.Sleep(3_000);
            //    if (PhasOk && GainOk)
            //    {
            //        bOk = true;
            //        break;
            //    }
            //}
            //#endregion

            //#region step3:scan window again
            //if (bOk)
            //{
            //    if (Math.Abs(Statics.Phase_fs) >= 100 * 1000)
            //    {
            //        Statics = SineFitStatis(NeedStaticTimes, currInstrument, getDataFromChannelIndex, inputsignalFreqByMHz, spsByM);
            //        SynsWinChange(currInstrument, currAcqBdIndex, Statics.Phase_fs, bfirst);
            //    }
            //}
            //#endregion

            //RefreshData();
            //Application.DoEvents();
            //Cursor = Cursors.Default;
            //if (bOk)
            //{
            //    MessageBox.Show("校准成功完成！");
            //    ((ICaliData)TiAdc_SyncSampleClock.Default).SaveToFile();
            //    //((ICaliData)TiAdc_PhaseOffsetGain.Default).SaveToFile();
            //}
            //else
            //{
            //    if (errorMessage != "")
            //        MessageBox.Show($"校准失败!{errorMessage}");
            //    else
            //        MessageBox.Show($"校准失败!超时退出!");
            //}
        }
        #region Auto Cali TiAdc
        private (bool bOK, double Phase_fs, double Gain) SineFitStatis(int NeedStaticTimes, IInstrumentSession currInstrument, int currChannel, double inputsignalFreqByMHz, double spsByM)
        {
            double TotalStaticOfGain = 0;
            double TotalStaticOfPhase = 0;
            List<double> Statics = new List<double> { 0, 0 };
            for (int staticTimes = 0; staticTimes < NeedStaticTimes; staticTimes++)
            {
                List<ushort[]>? allAdcData = InstrumentInteract.Factory_WaveData_Adc(currInstrument, 6_000);
                if (allAdcData == null)
                {
                    return (false, 0, 0);
                }
                int dataLength = allAdcData[0].Length;
                if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI16G || ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI20G)
                    currChannel = currChannel % 4;//4=DBI项目只有子带，按目前一个通道最多4个子带（80G）来计算
                int getDataFromChannelIndex = currChannel;

                #region 数据拆分
                List<List<short>> sourceDataList = new List<List<short>>();
                int perChannelAdcCount = ServerDomainConstants.PerAnaChannelAdcCount;

                int perAdcCoreCount = ServerDomainConstants.PerAdcCoreCount;
                for (int adcIndex = 0; adcIndex < perChannelAdcCount; adcIndex++)
                {
                    if (checkBoxOnlyAdc.Checked)
                    {
                        sourceDataList.Add(new List<short>());
                        for (int i = 0; i < dataLength; i++)
                        {
                            for (int coreIndex = 0; coreIndex < perAdcCoreCount; coreIndex++)
                                sourceDataList[sourceDataList.Count - 1].Add((short)allAdcData[getDataFromChannelIndex * perChannelAdcCount * perAdcCoreCount + adcIndex * perAdcCoreCount + coreIndex][i]);
                        }
                    }
                    else
                    {
                        for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                        {
                            sourceDataList.Add(new List<short>());
                            for (int i = 0; i < dataLength; i++)
                                sourceDataList[sourceDataList.Count - 1].Add((short)allAdcData[getDataFromChannelIndex * perChannelAdcCount * perAdcCoreCount + adcIndex * perAdcCoreCount + coreIndex][i]);
                        }
                    }

                }

                #endregion
                //WaveOffsetGainPhase waveOffsetGainPhaseAdc1 = SineFitFunc.SineFit(adc1Data, spsByM, inputsignalFreqByMHz);
                //WaveOffsetGainPhase waveOffsetGainPhaseAdc2 = SineFitFunc.SineFit(adc2Data, spsByM, inputsignalFreqByMHz);
                WaveOffsetGainPhase waveOffsetGainPhaseAdc1 = SineFitFunc.SineFit(sourceDataList[0].ToArray(), spsByM / ServerDomainConstants.PerAnaChannelAdcCount, inputsignalFreqByMHz);
                WaveOffsetGainPhase waveOffsetGainPhaseAdc2 = SineFitFunc.SineFit(sourceDataList[1].ToArray(), spsByM / ServerDomainConstants.PerAnaChannelAdcCount, inputsignalFreqByMHz);
                waveOffsetGainPhaseAdc1.Phase += Math.PI * 2;
                waveOffsetGainPhaseAdc2.Phase += Math.PI * 2;
                waveOffsetGainPhaseAdc1.Phase = waveOffsetGainPhaseAdc1.Phase % (Math.PI);
                waveOffsetGainPhaseAdc2.Phase = waveOffsetGainPhaseAdc2.Phase % (Math.PI);

                TotalStaticOfGain += (waveOffsetGainPhaseAdc2.Gain - waveOffsetGainPhaseAdc1.Gain) / waveOffsetGainPhaseAdc1.Gain;
                TotalStaticOfPhase += (waveOffsetGainPhaseAdc2.Phase - waveOffsetGainPhaseAdc1.Phase);
            }
            TotalStaticOfGain /= NeedStaticTimes;
            TotalStaticOfPhase /= NeedStaticTimes;
            double theoryDelta_fS = 1_000_000_000d / spsByM;

            double TotalStaticOfPhase_fS = TotalStaticOfPhase * 1000_000_000 / inputsignalFreqByMHz / (2 * Math.PI) - theoryDelta_fS;
            return (true, TotalStaticOfPhase_fS, TotalStaticOfGain);
        }
        private void SynsWinChange(IInstrumentSession currInstrument, int currAcqBdIndex, double TotalStaticOfPhase_fS, bool bfirst)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "ADC5200SyncWindowRegValue";
            currInstrument.WriteString(scpiCmd);
            Thread.Sleep(1000);
            string data = currInstrument.ReadString();
            //返回数据样式
            //B1.Adc1=>0000_0000_0000_0000_0000_0000
            //B1.Adc2=>0000_0000_0000_0000_0000_0000
            //B3.Adc1=>0000_0000_0000_0000_0000_0000
            //B3.Adc2=>0000_0000_0000_0000_0000_0000
            //B5.Adc1=>0000_0000_0000_0000_0000_0000
            //B5.Adc2=>0000_0000_0000_0000_0000_0000
            //B7.Adc1=>0000_0000_0000_0000_0000_0000
            //B7.Adc2=>0000_0000_0000_0000_0000_0000
            string[] lines = data.Split(System.Environment.NewLine);
            Dictionary<Int32, List<string>> map = new Dictionary<Int32, List<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() != "")
                {
                    string tmpStr = lines[i].Substring(lines[i].IndexOf("=>") + "=>".Length).Replace("_", "");
                    int acqBdIndex = Int32.Parse(lines[i].Substring(1, 1)) - 1;
                    if (!map.ContainsKey(acqBdIndex))
                        map.Add(acqBdIndex, new List<string>() { tmpStr });
                    else
                        map[acqBdIndex].Add(tmpStr);
                }
            }
            var Adc1Window = GetMaxLengthZeroMid(map[currAcqBdIndex][0], 24);
            var Adc2Window = GetMaxLengthZeroMid(map[currAcqBdIndex][1], 24);
            if (bfirst)
            {
                TiAdc_SyncSampleClock.Default[currAcqBdIndex][0].Sample20GClockDelay = (uint)Adc1Window.MaxWidth;
                TiAdc_SyncSampleClock.Default[currAcqBdIndex][1].Sample20GClockDelay = (uint)Adc2Window.MaxWidth;
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_SyncSampleClock);
            }
            else
            {
                if (TotalStaticOfPhase_fS > 0)
                {
                    TiAdc_SyncSampleClock.Default[currAcqBdIndex][0].Sample20GClockDelay = (uint)Adc1Window.MaxWidth;
                    TiAdc_SyncSampleClock.Default[currAcqBdIndex][1].Sample20GClockDelay = (uint)Adc2Window.NextWidth;
                    InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_SyncSampleClock);
                }
                else
                {
                    TiAdc_SyncSampleClock.Default[currAcqBdIndex][0].Sample20GClockDelay = (uint)Adc1Window.NextWidth;
                    TiAdc_SyncSampleClock.Default[currAcqBdIndex][1].Sample20GClockDelay = (uint)Adc2Window.MaxWidth;
                    InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_SyncSampleClock);
                }
            }
            Thread.Sleep(2_000);
        }
        private (int MaxWidth, int NextWidth) GetMaxLengthZeroMid(int source, uint Length)
        {
            int data = 0;
            int j = 0;
            List<List<Int32>> zeroLists = new List<List<Int32>>();
            List<Int32> index = new List<Int32>();
            for (int i = 0; i < Length; i++)
            {
                i = j + 1;
                if (i == Length)
                    break;
                List<Int32> zeros = new List<Int32>();
                for (j = i; j < Length; j++)
                {
                    data = source & (0x0001 << j);
                    if (data == 0)
                    {
                        zeros.Add(j + 1);
                    }
                    else
                    {
                        zeroLists.Add(zeros);
                        break;
                    }
                }
            }

            Int32 Maxlength1 = 0;
            Int32 Maxlength2 = 0;
            Int32 Maxindex1 = -1;
            Int32 Maxindex2 = -1;
            for (int i = 0; i < zeroLists.Count; i++)
            {
                if (zeroLists[i].Count > Maxlength1)
                {
                    Maxlength2 = Maxlength1;
                    Maxindex2 = Maxindex1;
                    Maxlength1 = zeroLists[i].Count;
                    Maxindex1 = i;
                }
            }
            if (Maxindex1 != -1)
            {
                Int32 index1 = zeroLists[Maxindex1][0] + (Int32)Math.Ceiling((Maxlength1 / 2.0)) - 2;
                index.Add(index1);
                Int32 index2 = zeroLists[Maxindex2][0] + (Int32)Math.Ceiling((Maxlength2 / 2.0)) - 2;
                index.Add(index2);
            }
            else
            {
                index.Add(-1);         //no zero
            }
            return (index[0], index[1]);
        }
        private (int MaxWidth, int NextWidth) GetMaxLengthZeroMid(string source, uint Length)
        {
            char data;
            int j = 0;
            List<List<string>> zeroLists = new List<List<string>>();
            List<Int32> index = new List<Int32>();
            for (int i = 0; i < Length; i++)
            {
                i = j + 1;
                if (i == Length)
                    break;
                List<string> zeros = new List<string>();
                for (j = i; j < Length; j++)
                {
                    data = source[j];
                    if (data == '0')
                    {
                        zeros.Add((j + 1).ToString());
                    }
                    else
                    {
                        zeroLists.Add(zeros);
                        break;
                    }
                }
            }

            Int32 Maxlength1 = 0;
            Int32 Maxlength2 = 0;
            Int32 Maxindex1 = -1;
            Int32 Maxindex2 = -1;
            for (int i = 0; i < zeroLists.Count; i++)
            {

                if (zeroLists[i].Count >= Maxlength1)
                {
                    Maxlength2 = Maxlength1;
                    Maxindex2 = Maxindex1;
                    Maxlength1 = zeroLists[i].Count;
                    Maxindex1 = i;
                }
                if ((zeroLists[i].Count == Maxlength1 - 1) && (Maxlength2 != Maxlength1))
                {
                    Maxlength2 = Maxlength1 - 1;
                    Maxindex2 = i;
                }
            }
            if (Maxindex1 != -1)
            {
                Int32 index1 = BaseHelper.TryConvertToInt(zeroLists[Maxindex1][0]) + (Int32)Math.Ceiling((Maxlength1 / 2.0)) - 2;
                index.Add(index1);
                if (Maxindex2 != -1)
                {
                    Int32 index2 = BaseHelper.TryConvertToInt(zeroLists[Maxindex2][0]) + (Int32)Math.Ceiling((Maxlength2 / 2.0)) - 2;
                    if (Maxlength2 == Maxlength1)
                    {
                        index.Clear();
                        index.Add(index2);
                        index.Add(index1);
                    }
                    else
                        index.Add(index2);
                }
            }
            else
            {
                index.Add(-1);         //no zero
            }
            return (index[0], index[1]);
        }
        #endregion

        private void buttonCalcError_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            int currAcqBdIndex = comboBoxCaliDataChannelSelectedTiAdc.SelectedIndex;
            string inputSorceStr = GetInputSource();
            if (MessageBox.Show($"请确保输入{textBoxSignalFreqByMHz.Text}MHz的信号到{inputSorceStr},并保证幅度大致合适，时基档在最高采样率挡位下。\r\n你确认吗？", "重要提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            inputsignalFreqByMHz = double.Parse(textBoxSignalFreqByMHz.Text);
            List<ushort[]>? allAdcData = InstrumentInteract.Factory_WaveData_Adc(currInstrument, 6_000);
            if (allAdcData == null)
            {
                MessageBox.Show("数据读取错误！");
                return;
            }
            int dataLength = allAdcData[0].Length;
            int getDataFromChannelIndex = ServerDomainConstants.AcqBdNoChannelCorrespondence![currAcqBdIndex];
            #region 数据拆分
            List<List<short>> sourceDataList = new List<List<short>>();
            int perChannelAdcCount = ServerDomainConstants.PerAnaChannelAdcCount;
            int perAdcCoreCount = ServerDomainConstants.PerAdcCoreCount;
            for (int adcIndex = 0; adcIndex < perChannelAdcCount; adcIndex++)
            {
                if (checkBoxOnlyAdc.Checked)
                {
                    sourceDataList.Add(new List<short>());
                    for (int i = 0; i < dataLength; i++)
                    {
                        for (int coreIndex = 0; coreIndex < perAdcCoreCount; coreIndex++)
                            sourceDataList[sourceDataList.Count - 1].Add((short)allAdcData[getDataFromChannelIndex * perChannelAdcCount * perAdcCoreCount + adcIndex * perAdcCoreCount + coreIndex][i]);
                    }
                }
                else
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                    {
                        sourceDataList.Add(new List<short>());
                        for (int i = 0; i < dataLength; i++)
                            sourceDataList[sourceDataList.Count - 1].Add((short)allAdcData[getDataFromChannelIndex * perChannelAdcCount * perAdcCoreCount + adcIndex * perAdcCoreCount + coreIndex][i]);
                    }
                }

            }

            #endregion
            double perLoadSpsByM = 1000d * double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text) / sourceDataList.Count;
            WaveOffsetGainPhase[] waveOffsetGainPhasesList = new WaveOffsetGainPhase[sourceDataList.Count];
            for (int i = 0; i < waveOffsetGainPhasesList.Length; i++)
            {
                waveOffsetGainPhasesList[i] = SineFitFunc.SineFit(sourceDataList[i].ToArray(), perLoadSpsByM, inputsignalFreqByMHz);
                waveOffsetGainPhasesList[i].Phase += Math.PI * 2;
                waveOffsetGainPhasesList[i].Phase = waveOffsetGainPhasesList[i].Phase % (Math.PI);
            }
            double theoryDelta_pS = /*sourceDataList.Count * */1000d / double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text);
            dataGridView1.RowCount = sourceDataList.Count;
            int rowIndex = 0;
            for (int adcIndex = 0; adcIndex < perChannelAdcCount; adcIndex++)
            {
                if (checkBoxOnlyAdc.Checked)
                {
                    dataGridView1.Rows[rowIndex].Cells[0].Value = $"Adc{adcIndex + 1}";
                    if (rowIndex != 0)
                    {
                        double PhaseError_pS = (waveOffsetGainPhasesList[rowIndex].Phase - waveOffsetGainPhasesList[rowIndex - 1].Phase) * 1000_000 / inputsignalFreqByMHz / (2 * Math.PI) - theoryDelta_pS;
                        double GainError = 100 * (waveOffsetGainPhasesList[rowIndex].Gain - waveOffsetGainPhasesList[rowIndex - 1].Gain) / waveOffsetGainPhasesList[rowIndex].Gain;
                        double OffsetError = (waveOffsetGainPhasesList[rowIndex].Offset - waveOffsetGainPhasesList[rowIndex - 1].Offset);

                        dataGridView1.Rows[rowIndex].Cells[1].Value = OffsetError.ToString();
                        dataGridView1.Rows[rowIndex].Cells[2].Value = GainError.ToString();
                        dataGridView1.Rows[rowIndex].Cells[3].Value = PhaseError_pS.ToString();

                    }
                    else
                    {
                        dataGridView1.Rows[rowIndex].Cells[1].Value = "";
                        dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                        dataGridView1.Rows[rowIndex].Cells[3].Value = "";
                    }
                    rowIndex++;
                }
                else
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                    {
                        dataGridView1.Rows[rowIndex].Cells[0].Value = $"Adc{adcIndex + 1}_Core{coreIndex + 1}";
                        if (rowIndex != 0)
                        {
                            double PhaseError_pS = (waveOffsetGainPhasesList[rowIndex].Phase - waveOffsetGainPhasesList[rowIndex - 1].Phase) * 1000_000 / inputsignalFreqByMHz / (2 * Math.PI) - theoryDelta_pS;
                            double GainError = 100 * (waveOffsetGainPhasesList[rowIndex].Gain - waveOffsetGainPhasesList[rowIndex - 1].Gain) / waveOffsetGainPhasesList[rowIndex].Gain;
                            double OffsetError = (waveOffsetGainPhasesList[rowIndex].Offset - waveOffsetGainPhasesList[rowIndex - 1].Offset);

                            dataGridView1.Rows[rowIndex].Cells[1].Value = OffsetError.ToString();
                            dataGridView1.Rows[rowIndex].Cells[2].Value = GainError.ToString();
                            dataGridView1.Rows[rowIndex].Cells[3].Value = PhaseError_pS.ToString();

                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[1].Value = "";
                            dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                            dataGridView1.Rows[rowIndex].Cells[3].Value = "";
                        }
                        rowIndex++;
                    }
                }
            }
            MessageBox.Show("计算完成！");
        }

        private void buttonReadBackAdcRegValue_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            List<AdcRegisterDataFormat>? readBackAdcRegs = InstrumentInteract.Factory_Cali_Specail_ReadbackAdcRegisterValue(currInstrument);
            if (readBackAdcRegs == null)
                return;
            StringBuilder sb = new StringBuilder();
            if (readBackAdcRegs != null)
            {
                foreach (AdcRegisterDataFormat adcRegisterDataFormat in readBackAdcRegs)
                {
                    sb.AppendLine($"Board[{adcRegisterDataFormat.BoardIndex + 1}]:");
                    foreach (Adc adc in adcRegisterDataFormat.AllAdc!)
                    {
                        sb.AppendLine($"  Adc[{adc.AdcIndex + 1}]");
                        foreach (RegisterAddrValuePair rp in adc.RegisterValuePair!)
                        {
                            sb.AppendLine("       Address=0x" + rp.RegAddress.ToString("X") + ",Value=0x" + rp.RegValue.ToString("X"));
                        }
                    }
                }
            }
            richTextBox1.Text = sb.ToString();
        }

        private void checkBoxOnlyAdc_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
