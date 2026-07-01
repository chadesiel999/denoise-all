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
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;
using System.IO;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageCaliData_Channel : UserControl, IMainFormTabPage
    {
        public TabPageCaliData_Channel()
        {
            InitializeComponent();
            Init();
        }
        private CaliDataType currCaliDataType = CaliDataType.PhyChannel;
        public CaliDataType CaliDataType { get => currCaliDataType; }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
            ProductType.JiHe_MSO7000X,
            ProductType.B21_HD4G,
            ProductType.ForTest,
            ProductType.B21_JinHui_PXI,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_HB8G,
            ProductType.B21_MS2G,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            AnalogChannelType analogChannelType = (AnalogChannelType)ServerDomainConstants.AnalogChannelType;
            if (analogChannelType != AnalogChannelType.BW8G)
            {
                if (comboBoxModelIndex.SelectedIndex != 0)
                {
                    comboBoxModelIndex.SelectedIndex = 0;
                    currCaliDataType = CaliDataType.PhyChannel;
                }
            }
            if (instrumentInteract != null)
            {
                string AnaloChannelgCaliMemo = "";

                string cmdStr = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData) + $"? AnalogChannelCaliMemo";
                currInstrument!.WriteString(cmdStr);
                AnaloChannelgCaliMemo = currInstrument!.ReadString();

                richTextBoxAnalogCaliMemo.Text = AnaloChannelgCaliMemo;
            }
            comboBoxModelIndex.Enabled = analogChannelType == AnalogChannelType.BW8G;
            buttonStartAutoCaliBaseline.Enabled = currInstrument != null;
            buttonCancelAutoCaliBaseline.Enabled = false;
            progressBarAutoCaliBaseline.Visible = false;

            RefreshData();
        }
        enum ChannelDataGridColumnIndex : int
        {
            CoarseCtrlWord = 1,
            OffsetPreceding = 2,
            OffsetPreceding_3Div = 3,
            OffsetPosterior = 4,
            OffsetPosterior_3Div = 5,
            Gain_FineByAdc = 6,
            Gain_FineByFpga = 7,
            DCTrigZero = 8,
            DCTrigZero_3Div = 9,
            Reserved1 = 10,
            Reserved2 = 11,
        }
        private bool bCaliData_Channel_InitFinished = false;

        private void Init()
        {
            dataGridViewCaliData_ChannelScale.RowCount = CaliConstants.Fixed_MaxPhyCoarseScaleCount;
            for (int scaleID = 0; scaleID < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleID++)
            {
                dataGridViewCaliData_ChannelScale.Rows[scaleID].Cells[0].ReadOnly = true;
                if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] < 1000)
                    dataGridViewCaliData_ChannelScale.Rows[scaleID].Cells[0].Value = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID].ToString() + "uV";
                else if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] < 1_000_000)
                    dataGridViewCaliData_ChannelScale.Rows[scaleID].Cells[0].Value = (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] / 1_000) + "mV";
                else
                    dataGridViewCaliData_ChannelScale.Rows[scaleID].Cells[0].Value = (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleID] / 1_000_000) + "V";
            }
            dataGridViewCaliData_ChannelScale.Columns[0].DefaultCellStyle.BackColor = Color.DarkGray;
            dataGridViewCaliData_ChannelScale.Columns[0].Frozen = true;
            comboBoxCaliDataChannelSelectedChannel.Items.Clear();
            for (int i = 0; i < CaliConstants.Fixed_MaxPhysicsChannelCount; i++)
            {
                comboBoxCaliDataChannelSelectedChannel.Items.Add($"CH{i + 1}");
            }
            comboBoxCaliDataChannelImpedance.SelectedIndex = 1;
            comboBoxCaliDataChannelSelectedChannel.SelectedIndex = 0;
            comboBoxModelIndex.SelectedIndex = 0;
            bCaliData_Channel_InitFinished = true;
            RefreshData();
        }
        public void RefreshData()
        {
            if (!bCaliData_Channel_InitFinished)
                return;

            int currSelectedChannel = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            int currImpedance = comboBoxCaliDataChannelImpedance.SelectedIndex;
            for (int i = 0; i < CaliConstants.Fixed_MaxPhyCoarseScaleCount; i++)
            {
                ChannelPerScaleItem channelPerScaleItem = currCaliDataType == CaliDataType.PhyChannel ? ChannelParams.Default[currSelectedChannel, currImpedance, i] : ChannelParamsModel2.Default[currSelectedChannel, currImpedance, i];
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.CoarseCtrlWord].Value = channelPerScaleItem.Gain_CoarseCtrlWord.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPreceding].Value = channelPerScaleItem.OffsetPreceding.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPreceding_3Div].Value = channelPerScaleItem.OffsetPreceding_3Div.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPosterior].Value = channelPerScaleItem.OffsetPosterior.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPosterior_3Div].Value = channelPerScaleItem.OffsetPosterior_3Div.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Gain_FineByAdc].Value = channelPerScaleItem.Gain_FineByAdc.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Gain_FineByFpga].Value = channelPerScaleItem.Gain_FineByFpgaThousand.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.DCTrigZero].Value = channelPerScaleItem.DCTrigZero.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.DCTrigZero_3Div].Value = channelPerScaleItem.DCTrigZero_3Div.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Reserved1].Value = channelPerScaleItem.Reserved1.ToString();
                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Reserved2].Value = channelPerScaleItem.Reserved2.ToString();
            }

            textBoxCaliData_Channel_ACZero.Text = (currCaliDataType == CaliDataType.PhyChannel ? ChannelParams.Default.Trig_ACZero : ChannelParamsModel2.Default.Trig_ACZero).ToString();
            textBoxCaliData_Channel_AC3Div.Text = (currCaliDataType == CaliDataType.PhyChannel ? ChannelParams.Default.Trig_ACZero3Div : ChannelParamsModel2.Default.Trig_ACZero3Div).ToString();

            textBoxTemperatureAtCaliGain.Text = $"{ChannelParams.Default.TemperatureAtCaliGain_mCelsius * 1.0 / 1000}℃";
            textBoxTemperatureAtCaliBaseline.Text = $"{ChannelParams.Default.TemperatureAtCaliBaseline_mCelsius * 1.0 / 1000}℃";
        }
        private void CaliData_Channel_SendGridData(int rowIndex, int columnIndex, bool bNeedSend)
        {
            int currSelectedChannel = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            int currImpedance = comboBoxCaliDataChannelImpedance.SelectedIndex;
            DataGridViewCell currCell = dataGridViewCaliData_ChannelScale.Rows[rowIndex].Cells[columnIndex];
            UInt32 data = 0;
            UInt32.TryParse(currCell.Value.ToString(), out data);
            ChannelDataGridColumnIndex enumColumn = (ChannelDataGridColumnIndex)columnIndex;
            //ChannelPerScaleItem 是结构，不是引用类型的，之后需要拷贝
            ChannelPerScaleItem perScaleItem = (currCaliDataType == CaliDataType.PhyChannel) ? ChannelParams.Default[currSelectedChannel, currImpedance, rowIndex] : ChannelParamsModel2.Default[currSelectedChannel, currImpedance, rowIndex];
            UInt32 oldData = enumColumn switch
            {
                ChannelDataGridColumnIndex.CoarseCtrlWord => perScaleItem.Gain_CoarseCtrlWord,
                ChannelDataGridColumnIndex.OffsetPreceding => perScaleItem.OffsetPreceding,
                ChannelDataGridColumnIndex.OffsetPreceding_3Div => perScaleItem.OffsetPreceding_3Div,
                ChannelDataGridColumnIndex.OffsetPosterior => perScaleItem.OffsetPosterior,
                ChannelDataGridColumnIndex.OffsetPosterior_3Div => perScaleItem.OffsetPosterior_3Div,
                ChannelDataGridColumnIndex.Gain_FineByAdc => perScaleItem.Gain_FineByAdc,
                ChannelDataGridColumnIndex.Gain_FineByFpga => perScaleItem.Gain_FineByFpgaThousand,
                ChannelDataGridColumnIndex.DCTrigZero => perScaleItem.DCTrigZero,
                ChannelDataGridColumnIndex.DCTrigZero_3Div => perScaleItem.DCTrigZero_3Div,
                ChannelDataGridColumnIndex.Reserved1 => perScaleItem.Reserved1,
                ChannelDataGridColumnIndex.Reserved2 => perScaleItem.Reserved2,
                _ => data

            };
            if (oldData == data) //判断新旧值，避免连续发送
                return;
            switch (enumColumn)
            {
                case ChannelDataGridColumnIndex.CoarseCtrlWord: perScaleItem.Gain_CoarseCtrlWord = data; currCell.Value = perScaleItem.Gain_CoarseCtrlWord.ToString(); break;
                case ChannelDataGridColumnIndex.OffsetPreceding: perScaleItem.OffsetPreceding = data; currCell.Value = perScaleItem.OffsetPreceding.ToString(); break;
                case ChannelDataGridColumnIndex.OffsetPreceding_3Div: perScaleItem.OffsetPreceding_3Div = data; currCell.Value = perScaleItem.OffsetPreceding_3Div.ToString(); break;
                case ChannelDataGridColumnIndex.OffsetPosterior: perScaleItem.OffsetPosterior = data; currCell.Value = perScaleItem.OffsetPosterior.ToString(); break;
                case ChannelDataGridColumnIndex.OffsetPosterior_3Div: perScaleItem.OffsetPosterior_3Div = data; currCell.Value = perScaleItem.OffsetPosterior_3Div.ToString(); break;
                case ChannelDataGridColumnIndex.Gain_FineByAdc: perScaleItem.Gain_FineByAdc = data; currCell.Value = perScaleItem.Gain_FineByAdc.ToString(); break;
                case ChannelDataGridColumnIndex.Gain_FineByFpga: perScaleItem.Gain_FineByFpgaThousand = data; currCell.Value = perScaleItem.Gain_FineByFpgaThousand.ToString(); break;
                case ChannelDataGridColumnIndex.DCTrigZero: perScaleItem.DCTrigZero = data; currCell.Value = perScaleItem.DCTrigZero.ToString(); break;
                case ChannelDataGridColumnIndex.DCTrigZero_3Div: perScaleItem.DCTrigZero_3Div = data; currCell.Value = perScaleItem.DCTrigZero_3Div.ToString(); break;
                case ChannelDataGridColumnIndex.Reserved1: perScaleItem.Reserved1 = data; currCell.Value = perScaleItem.Reserved1.ToString(); break;
                case ChannelDataGridColumnIndex.Reserved2: perScaleItem.Reserved2 = data; currCell.Value = perScaleItem.Reserved2.ToString(); break;
            };
            if (currCaliDataType == CaliDataType.PhyChannel)
                ChannelParams.Default[currSelectedChannel, currImpedance, rowIndex] = perScaleItem;
            else
                ChannelParamsModel2.Default[currSelectedChannel, currImpedance, rowIndex] = perScaleItem;

            if (bNeedSend && currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, currCaliDataType);
        }
        private void CaliData_Channel_AllSaveToMemory()
        {
            for (int rowIndex = 0; rowIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; rowIndex++)
            {
                for (int columnIndex = (int)ChannelDataGridColumnIndex.CoarseCtrlWord; columnIndex < (int)ChannelDataGridColumnIndex.DCTrigZero_3Div; columnIndex++)
                    CaliData_Channel_SendGridData(rowIndex, columnIndex, false);
            }
            UInt32 data = 0;
            UInt32.TryParse(textBoxCaliData_Channel_ACZero.Text, out data);
            textBoxCaliData_Channel_ACZero.Text = data.ToString();
            if (currCaliDataType == CaliDataType.PhyChannel)
                ChannelParams.Default.Trig_ACZero = data;
            else
                ChannelParamsModel2.Default.Trig_ACZero = data;
            UInt32.TryParse(textBoxCaliData_Channel_AC3Div.Text, out data);
            textBoxCaliData_Channel_AC3Div.Text = data.ToString();
            if (currCaliDataType == CaliDataType.PhyChannel)
                ChannelParams.Default.Trig_ACZero3Div = data;
            else
                ChannelParamsModel2.Default.Trig_ACZero3Div = data;
        }

        private void dataGridViewCaliData_ChannelScale_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 1)
                CaliData_Channel_SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_ChannelAutoSend.Checked);
        }
        private void buttonCaliData_Channel_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }

            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, currCaliDataType);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void buttonCaliData_Channel_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            CaliData_Channel_AllSaveToMemory();
            bool bResult = InstrumentInteract.CaliData_SaveData(currInstrument, currCaliDataType);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void buttonCaliData_Channel_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            CaliData_Channel_AllSaveToMemory();
            bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.PhyChannel);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void onCaliData_Channel_NeedRefreshGrid(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void buttonCaliData_LoadDefualtValue_Channel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("装载缺省值将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            if (currCaliDataType == CaliDataType.PhyChannel)
                ChannelParams.Default.LoadDefaultValue((AnalogChannelType)ServerDomainConstants.AnalogChannelType);
            else
                ChannelParamsModel2.Default.LoadDefaultValue((AnalogChannelType)ServerDomainConstants.AnalogChannelType);
            RefreshData();
            if (currInstrument != null)
                InstrumentInteract.CaliData_Send(this.currInstrument, currCaliDataType);
        }

        private void comboBoxModelIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            string source = comboBoxModelIndex.SelectedIndex == 0 ? "BNC" : "SMA";
            Cursor = Cursors.WaitCursor;

            for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount; channelID++)
            {
                if (!bCaliData_Channel_InitFinished)
                    continue;
                if (currInstrument == null)
                    continue;
                currInstrument.WriteString($":FACT:CHAN{channelID + 1}:INP {source}");
                Thread.Sleep(1000);
            }

            currCaliDataType = comboBoxModelIndex.SelectedIndex == 0 ? CaliDataType.PhyChannel : CaliDataType.PhyChannelModel2;
            RefreshData();
            Cursor = Cursors.Default;
        }
        #region 基线自动校准
        private
        enum AutoCaliOffsetStatus
        {
            Working = 1,
            Failure = 2,
            Ok = 3
        }
        class PerChannel
        {
            public bool bIs4GHzBWChannel = true;
            public int channelID;
            public int yLevelID;
            public int Impedance;
            public int StaticTimes = 0;
            //public long TotalStaticMeasureValue_uV = 0;
            public long TotalStaticBaseLine = 0;
            public long LastTotalStaticBaseLine = 0;
            public long LastMeasureValue_uV = 0;
            public bool bBaselineCtrlWordChanged = false;
            public bool bBaseline0Div = false;

            public uint Cali_ChannelOffset
            {
                get
                {
                    if (bIs4GHzBWChannel)
                        return ChannelParams.Default[channelID, Impedance, yLevelID].OffsetPosterior;
                    else
                        return ChannelParamsModel2.Default[channelID, Impedance, yLevelID].OffsetPosterior;
                }
                set
                {
                    if (bIs4GHzBWChannel)
                    {
                        ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                        perScaleItem.OffsetPosterior = value;
                        ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    }
                    else
                    {
                        ChannelPerScaleItem perScaleItem = ChannelParamsModel2.Default[channelID, Impedance, yLevelID];
                        perScaleItem.OffsetPosterior = value;
                        ChannelParamsModel2.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    }
                }
            }
            public uint Cali_Channel3Div
            {
                get
                {
                    if (bIs4GHzBWChannel)
                        return ChannelParams.Default[channelID, Impedance, yLevelID].OffsetPosterior_3Div;
                    else
                        return ChannelParamsModel2.Default[channelID, Impedance, yLevelID].OffsetPosterior_3Div;
                }
                set
                {
                    if (bIs4GHzBWChannel)
                    {
                        ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                        perScaleItem.OffsetPosterior_3Div = value;
                        ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    }
                    else
                    {
                        ChannelPerScaleItem perScaleItem = ChannelParamsModel2.Default[channelID, Impedance, yLevelID];
                        perScaleItem.OffsetPosterior_3Div = value;
                        ChannelParamsModel2.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    }
                }
            }

            public AutoCaliOffsetStatus Status = AutoCaliOffsetStatus.Working;
        }

        private (BatchTaskPartResult, int) CaliBaseLine(List<PerChannel> caliChannels, double baseErrorLimitByPercentDiv, int DesDiv, CancellationToken cancelToken, out string message)
        {
            Int32 PerYDivAdcSamples = (int)ServerDomainConstants.SAMPS_PER_YDIV;
            int errorCount = 0;
            const int NeedStaticTimes = 5;
            const int MaxOffsetCtrlWord = int.MaxValue;
            long maxWaitMilliseconds = 80 * 1000;
            StringBuilder stringBuilder = new StringBuilder();
            message = "";
            int WorkingFlag = 0;
            foreach (PerChannel perChannel in caliChannels)
                WorkingFlag |= 1 << perChannel.channelID;
            int AdcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits);


            int[] step = new int[] { 100, 100, 100, 100, 100, 100 };
            List<long> TotalStaticBaseLine = new List<long>() { 0, 0, 0, 0, 0, 0 };

            foreach (PerChannel channel in caliChannels)
            {
                currInstrument?.WriteString($":CHAN{(int)channel.channelID + 1}:OFFSet {DesDiv}");
                channel.Status = AutoCaliOffsetStatus.Working;
                if (channel.yLevelID <= (int)AnaChnlScaleIndex.Lv5m)
                {
                    baseErrorLimitByPercentDiv = 0.5;
                    step[channel.channelID] = 50;
                }
            }
            bool bfirst = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (WorkingFlag != 0)
            {

                //校准数据生效
                InstrumentInteract.CaliData_Send(currInstrument, currCaliDataType);
                Thread.Sleep(300);
                #region 统计
                foreach (PerChannel channel in caliChannels)
                {
                    channel.StaticTimes = 0;
                    TotalStaticBaseLine[channel.channelID] = 0;
                    channel.bBaselineCtrlWordChanged = false;
                }

                for (int staticTimes = 0; staticTimes < NeedStaticTimes; staticTimes++)
                {
                    //获取波形数据
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000);
                    cancelToken.ThrowIfCancellationRequested();
                    if (allChannelData == null)
                    {
                        message = "数据读取错误！";
                        return (BatchTaskPartResult.ErrorFatal, errorCount);
                    }
                    foreach (PerChannel channel in caliChannels)
                    {
                        if (channel.Status != AutoCaliOffsetStatus.Working)
                            continue;

                        channel.LastTotalStaticBaseLine = channel.TotalStaticBaseLine;
                        TotalStaticBaseLine[channel.channelID] += (long)allChannelData[channel.channelID].Average<ushort>(s => s); //uV

                        if (staticTimes == NeedStaticTimes - 1)
                            channel.TotalStaticBaseLine = TotalStaticBaseLine[channel.channelID];
                    }
                }
                #endregion

                #region 调整
                foreach (PerChannel channel in caliChannels)
                {
                    cancelToken.ThrowIfCancellationRequested();

                    if (channel.Status != AutoCaliOffsetStatus.Working)
                        continue;

                    long delta1 = DesDiv switch
                    {
                        0 => 8,
                        _ => 15
                    };
                    long delta2 = DesDiv switch
                    {
                        0 => 2,
                        _ => 3
                    };
                    if (channel.Impedance == 0)  //高阻调节步进更小，因此阈值较大
                    {
                        delta1 *= 2;
                        delta2 *= 2;
                    }

                    long average = channel.TotalStaticBaseLine / NeedStaticTimes;
                    long oldaverage = channel.LastTotalStaticBaseLine / NeedStaticTimes;
                    double OlderrorThousand = (double)(oldaverage - (AdcMaxValue / 2 + DesDiv * PerYDivAdcSamples)) / (AdcMaxValue / 2) * 100;
                    double errorThousand = (double)(average - (AdcMaxValue / 2 + DesDiv * PerYDivAdcSamples)) / (AdcMaxValue / 2) * 100;

                    if (Math.Abs(errorThousand) <= baseErrorLimitByPercentDiv) //,OK判据
                    {
                        WorkingFlag &= (~(1 << channel.channelID));
                        channel.Status = AutoCaliOffsetStatus.Ok;
                        stringBuilder.Append($"[√CH{channel.channelID + 1}]。");
                        continue;
                    }
                    if (Math.Abs(errorThousand) <= delta2)
                    {
                        //channel.Stage = AutoCaliOffset_Stage.Fine;
                        if (channel.yLevelID <= (int)AnaChnlScaleIndex.Lv10m)
                            step[channel.channelID] = 1;
                        else
                            step[channel.channelID] = 2;
                        if (channel.Impedance == 0)
                        {
                            step[channel.channelID] = step[channel.channelID] / 2;
                            if (step[channel.channelID] == 0)
                                step[channel.channelID] = 1;
                        }
                    }
                    else if (Math.Abs(errorThousand) <= delta1)
                    {
                        if (channel.yLevelID <= (int)AnaChnlScaleIndex.Lv10m)
                            step[channel.channelID] = 10;
                        else
                            step[channel.channelID] = 40;

                        if (channel.Impedance == 0)
                        {
                            step[channel.channelID] = step[channel.channelID] / 2;
                        }
                    }

                    if (Math.Abs(errorThousand) > Math.Abs(OlderrorThousand) && (bfirst == false))//保留上一次更小的结果
                    {
                        continue;
                    }
                    bfirst = false;
                    int newCtrlWord = DesDiv switch
                    {
                        0 => (int)channel.Cali_ChannelOffset,
                        _ => (int)channel.Cali_Channel3Div
                    };
                    if (channel.bIs4GHzBWChannel)
                        newCtrlWord += (errorThousand > 0) ? -step[channel.channelID] : step[channel.channelID];//控制字变大，偏置越大
                    else
                        newCtrlWord += (errorThousand > 0) ? step[channel.channelID] : -step[channel.channelID];
                    if (newCtrlWord < 0)
                    {
                        //失败情况之一：控制字已到最小值
                        WorkingFlag &= (~(1 << channel.channelID));
                        if (DesDiv == 0)
                            channel.Cali_ChannelOffset = 0;
                        else
                            channel.Cali_Channel3Div = 0;
                        channel.Status = AutoCaliOffsetStatus.Failure;
                        stringBuilder.Append($"[×CH{channel.channelID + 1}]，已到控制字最小值。");
                        errorCount++;
                    }
                    else if (newCtrlWord > MaxOffsetCtrlWord)
                    {
                        //失败情况之二：控制字已到最大值
                        WorkingFlag &= (~(1 << channel.channelID));
                        if (DesDiv == 0)
                            channel.Cali_ChannelOffset = MaxOffsetCtrlWord;
                        else
                            channel.Cali_Channel3Div = MaxOffsetCtrlWord;
                        channel.Status = AutoCaliOffsetStatus.Failure;
                        stringBuilder.Append($"[×CH{channel.channelID + 1}]，已到控制字最大值。");
                        errorCount++;
                    }
                    else
                    {
                        if (DesDiv == 0)
                            channel.Cali_ChannelOffset = (uint)newCtrlWord;
                        else
                            channel.Cali_Channel3Div = (uint)newCtrlWord;
                    }
                }
                #endregion

                if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds)
                {
                    message = "超时退出";
                    return (BatchTaskPartResult.ErrorOvertime, 1);
                }
            }
            foreach (PerChannel channel in caliChannels)
            {
                if (channel.Status == AutoCaliOffsetStatus.Working)
                    stringBuilder.Append($"[×CH{channel.channelID + 1}]，{DesDiv}Div超时没有校准好。");
            }
            message = stringBuilder.ToString();
            if (WorkingFlag != 0)
                return (BatchTaskPartResult.ErrorOvertime, errorCount);
            else if (errorCount != 0)
                return (BatchTaskPartResult.ErrorGeneral, errorCount);
            else
                return (BatchTaskPartResult.Succeed, errorCount);
        }
        private void buttonCancelAutoCaliBaseline_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
        private async void ProcessBarUpdateAsync(int step)
        {
            await Task.Run(() =>
            {
                if (InvokeRequired)
                {
                    progressBarAutoCaliBaseline.Invoke(new Action(() => this.progressBarAutoCaliBaseline.Value = step));

                }
                else
                {
                    this.progressBarAutoCaliBaseline.Value = step;
                }
            });
        }
        private CancellationTokenSource? cancellationTokenSource;
        private async Task<(bool, bool)> AutoCaliBaselineTaskAsync(int startLevelID)
        {
            return await Task.Run(() =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                var cancelToken = cancellationTokenSource.Token;
                Action<int> updateAction = ProcessBarUpdateAsync;
                int curStep = 0;
                bool bCanceled = false;
                for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount; channelID++)
                {
                    if (currCaliDataType == CaliDataType.PhyChannel)
                        currInstrument?.WriteString($":FACT:CHAN{channelID + 1}:INP BNC");
                    else
                        currInstrument?.WriteString($":FACT:CHAN{channelID + 1}:INP SMA");
                }
                updateAction(curStep++);

                double baseErrorLimitByPercentDiv = 0.1;
                int AdcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits);
                bool bHaveErrorHappen = false;
                int Errorcount = 0;
                int endLevelIDHighz = (int)AnaChnlScaleIndex.Lv10;
                int endLevelIDLowz = (int)AnaChnlScaleIndex.Lv1;
                bool bIncludeHighz = true;
                if (currCaliDataType == CaliDataType.PhyChannel)
                {
                    switch (ServerDomainConstants.ProductType)
                    {
                        case ProductType.B21_HB8G:
                            startLevelID = (int)AnaChnlScaleIndex.Lv2m;
                            break;
                        case ProductType.B21_MD8G:
                            startLevelID = (int)AnaChnlScaleIndex.Lv2m;
                            bIncludeHighz = false;
                            break;
                        case ProductType.B21_MS2G:
                            startLevelID = (int)AnaChnlScaleIndex.Lv500u;
                            break;
                        default:
                            startLevelID = (int)AnaChnlScaleIndex.Lv1m;
                            break;
                    }
                }
                else
                {
                    startLevelID = (int)AnaChnlScaleIndex.Lv10m;
                    if (ServerDomainConstants.ProductType == ProductType.B21_MD8G | ServerDomainConstants.ProductType == ProductType.B21_HB8G)
                        bIncludeHighz = false;
                }
                #region 高阻
                int Impedance = 0; //高阻
                if (bIncludeHighz)
                {
                    if (currCaliDataType == CaliDataType.PhyChannel)
                    {
                        for (int inputLevelID = startLevelID; inputLevelID <= endLevelIDLowz && !bCanceled; inputLevelID++)
                        {
                            List<PerChannel> caliChannels = new List<PerChannel>();
                            for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount && !bCanceled; channelID++)
                            //for (int channelID = 0; channelID < 2 && !bCanceled; channelID++)
                            {
                                PerChannel perChannel = new PerChannel();
                                perChannel.bIs4GHzBWChannel = (currCaliDataType == CaliDataType.PhyChannel);
                                perChannel.channelID = channelID;
                                perChannel.yLevelID = inputLevelID;
                                perChannel.Impedance = Impedance;
                                caliChannels.Add(perChannel);
                            }

                            int Imp = Impedance switch
                            {
                                0 => 0,
                                _ => 2
                            };
                            try
                            {
                                currInstrument?.WriteString($":FACTory:ALLSource:APPLy 1,{AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[inputLevelID] / 1000},0,{Imp},0,0,0,0,0,0");
                                (BatchTaskPartResult result0Div, int Errorcount0Div) = CaliBaseLine(caliChannels, baseErrorLimitByPercentDiv, 0, cancelToken, out string message1);
                                cancelToken.ThrowIfCancellationRequested();
                                currInstrument?.WriteString($":FACTory:ALLSource:APPLy 1,{AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[inputLevelID] / 1000},3000,{Imp},0,0,0,0,0,0");
                                (BatchTaskPartResult result3Div, int Errorcount3Div) = CaliBaseLine(caliChannels, baseErrorLimitByPercentDiv, 3, cancelToken, out string message2);
                                cancelToken.ThrowIfCancellationRequested();
                                BatchTaskPartResult result = result0Div | result3Div;
                                int Errorcount1 = Errorcount0Div + Errorcount3Div;
                                if (result != BatchTaskPartResult.Succeed)
                                    bHaveErrorHappen = true;
                            }
                            catch
                            {
                                bCanceled = true;
                                break; ;
                            }
                            updateAction(curStep++);
                        }
                    }
                    if (bCanceled)
                        return (bCanceled, bHaveErrorHappen);
                }
                #endregion
                #region 低阻
                Impedance = 1;//低阻
                for (int inputLevelID = startLevelID; inputLevelID <= endLevelIDLowz && !bCanceled; inputLevelID++)
                {
                    List<PerChannel> caliChannels = new List<PerChannel>();
                    for (int channelID = 0; channelID < ServerDomainConstants.AnalogChannelCount && !bCanceled; channelID++)
                    {
                        PerChannel perChannel = new PerChannel();
                        perChannel.bIs4GHzBWChannel = (currCaliDataType == CaliDataType.PhyChannel);
                        perChannel.channelID = channelID;
                        perChannel.yLevelID = inputLevelID;
                        perChannel.Impedance = Impedance;
                        caliChannels.Add(perChannel);
                    }

                    int Imp = Impedance switch
                    {
                        0 => 0,
                        _ => 2
                    };
                    try
                    {
                        currInstrument?.WriteString($":FACTory:ALLSource:APPLy 1,{AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[inputLevelID] / 1000},0,{Imp},0,0,0,0,0,0");
                        (BatchTaskPartResult result0Div, int Errorcount0Div) = CaliBaseLine(caliChannels, baseErrorLimitByPercentDiv, 0, cancelToken, out string message1);
                        cancelToken.ThrowIfCancellationRequested();
                        BatchTaskPartResult result = result0Div;
                        int Errorcount2 = Errorcount0Div;
                        currInstrument?.WriteString($":FACTory:ALLSource:APPLy 1,{AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[inputLevelID] / 1000},3000,{Imp},0,0,0,0,0,0");
                        if (currCaliDataType == CaliDataType.PhyChannel)
                        {
                            (BatchTaskPartResult result3Div, int Errorcount3Div) = CaliBaseLine(caliChannels, baseErrorLimitByPercentDiv, 3, cancelToken, out string message2);
                            cancelToken.ThrowIfCancellationRequested();
                            result = result0Div | result3Div;
                            Errorcount2 = Errorcount0Div + Errorcount3Div;
                        }

                        if (result != BatchTaskPartResult.Succeed)
                            bHaveErrorHappen = true;
                    }
                    catch
                    {
                        bCanceled = true;
                        break; ;
                    }
                    updateAction(curStep++);
                }
                #endregion
                return (bCanceled, bHaveErrorHappen);
            });
        }
        private async void buttonStartAutoCaliBaseline_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请确保各个通道输入端口没有信号输入！", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            buttonStartAutoCaliBaseline.Enabled = false;
            buttonCancelAutoCaliBaseline.Enabled = !buttonStartAutoCaliBaseline.Enabled;
            progressBarAutoCaliBaseline.Visible = true;
            bool bHaveErrorHappen = false;
            bool bCanceled = false;

            int startLevelID = (int)AnaChnlScaleIndex.Lv1m;
            int totalStepCount = 1/*设置输入端口*/;
            totalStepCount += ((int)AnaChnlScaleIndex.Lv10 - startLevelID + 1);/*高阻*/
            totalStepCount += ((int)AnaChnlScaleIndex.Lv1 - startLevelID + 1);/*低阻*/
            progressBarAutoCaliBaseline.Maximum = totalStepCount + 1;
            //currInstrument?.WriteString($":ACQuire:TYPe HIGHres");            
            (bCanceled, bHaveErrorHappen) = await AutoCaliBaselineTaskAsync(startLevelID);

            if (!bCanceled)
            {
                if (bHaveErrorHappen)
                {
                    if (MessageBox.Show("校准存在{Errorcount}个错误，需要保存本次数据吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        InstrumentInteract.CaliData_SaveData(currInstrument, currCaliDataType);
                }
                else
                    InstrumentInteract.CaliData_SaveData(currInstrument, currCaliDataType);
            }
            RefreshData();

            buttonCancelAutoCaliBaseline.Enabled = false;
            buttonStartAutoCaliBaseline.Enabled = !buttonCancelAutoCaliBaseline.Enabled;
            progressBarAutoCaliBaseline.Visible = false;
        }
        #endregion

        private void buttonCh1ToOtherChannel_Click(object sender, EventArgs e)
        {
            for (int channelIndex = 1; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedance = 0; impedance < 2; impedance++)
                {
                    for (int scaleID = 0; scaleID < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleID++)
                    {
                        ChannelPerScaleItem sourceScaleItem = (currCaliDataType == CaliDataType.PhyChannel) ? ChannelParams.Default[0, impedance, scaleID] : ChannelParamsModel2.Default[0, impedance, scaleID];
                        if (currCaliDataType == CaliDataType.PhyChannel)
                            ChannelParams.Default[channelIndex, impedance, scaleID] = sourceScaleItem;
                        else
                            ChannelParamsModel2.Default[channelIndex, impedance, scaleID] = sourceScaleItem;
                    }
                }
            }
            RefreshData();
            if (currInstrument != null)
                InstrumentInteract.CaliData_Send(this.currInstrument, currCaliDataType);
            MessageBox.Show("Finished!");
        }

        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            dataGridViewCaliData_ChannelScale.Enabled = false;
            comboBoxCaliDataChannelSelectedChannel.Enabled = false;
            comboBoxCaliDataChannelImpedance.Enabled = false;
            int chnlIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            int impIndex = comboBoxCaliDataChannelImpedance.SelectedIndex;

            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string filePath = Path.Combine(fbd.SelectedPath, $"ChannelCali_{DateTime.Now.ToString("yy_MM_dd_HH_mm_ss")}.csv");
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    //循环访问不同的通道(Ch1-Ch4)和阻抗(高,低)
                    for (int chnlId = 0; chnlId < 4; chnlId++)
                    {
                        var chnlName = comboBoxCaliDataChannelSelectedChannel.Items[chnlId];
                        for (int impId = 0; impId < comboBoxCaliDataChannelImpedance.Items.Count; impId++)
                        {
                            var impName = comboBoxCaliDataChannelImpedance.Items[impId];
                            for (int i = 0; i < CaliConstants.Fixed_MaxPhyCoarseScaleCount; i++)
                            {
                                ChannelPerScaleItem channelPerScaleItem = (currCaliDataType == CaliDataType.PhyChannel) ?
                                    ChannelParams.Default[chnlId, impId, i] : ChannelParamsModel2.Default[chnlId, impId, i];
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.CoarseCtrlWord].Value = channelPerScaleItem.Gain_CoarseCtrlWord.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPreceding].Value = channelPerScaleItem.OffsetPreceding.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPreceding_3Div].Value = channelPerScaleItem.OffsetPreceding_3Div.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPosterior].Value = channelPerScaleItem.OffsetPosterior.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.OffsetPosterior_3Div].Value = channelPerScaleItem.OffsetPosterior_3Div.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Gain_FineByAdc].Value = channelPerScaleItem.Gain_FineByAdc.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Gain_FineByFpga].Value = channelPerScaleItem.Gain_FineByFpgaThousand.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.DCTrigZero].Value = channelPerScaleItem.DCTrigZero.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.DCTrigZero_3Div].Value = channelPerScaleItem.DCTrigZero_3Div.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Reserved1].Value = channelPerScaleItem.Reserved1.ToString();
                                dataGridViewCaliData_ChannelScale.Rows[i].Cells[(int)ChannelDataGridColumnIndex.Reserved2].Value = channelPerScaleItem.Reserved2.ToString();
                            }
                            string content = $"{chnlName}_{impName} \r\n" + dataGridViewCaliData_ChannelScale.GetContentAsCsv();
                            byte[] contBytes = new UTF8Encoding(true).GetBytes(content);
                            fileStream.Write(contBytes, 0, contBytes.Length);
                            fileStream.Flush();
                        }
                    }

                }
            }

            comboBoxCaliDataChannelSelectedChannel.SelectedIndex = chnlIndex;
            comboBoxCaliDataChannelImpedance.SelectedIndex = impIndex;
            dataGridViewCaliData_ChannelScale.Enabled = true;
            comboBoxCaliDataChannelSelectedChannel.Enabled = true;
            comboBoxCaliDataChannelImpedance.Enabled = true;
        }

        private void buttonTemperatureAtCaliGain_Click(object sender, EventArgs e)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $"? GetPhyChannelTemperatures{1}";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readBack = currInstrument.ReadString();
            if (readBack != "")
            {
                ChannelParams.Default.TemperatureAtCaliGain_mCelsius = (Int32)(double.Parse(readBack) * 1000);
                textBoxTemperatureAtCaliGain.Text = $"{ChannelParams.Default.TemperatureAtCaliGain_mCelsius * 1.0 / 1000}℃";
            }
        }

        private void buttonTemperatureAtCaliBaseline_Click(object sender, EventArgs e)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $"? GetPhyChannelTemperatures{1}";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readBack = currInstrument.ReadString();
            if (readBack != "")
            {
                ChannelParams.Default.TemperatureAtCaliBaseline_mCelsius = (Int32)(double.Parse(readBack) * 1000);
                textBoxTemperatureAtCaliBaseline.Text = $"{ChannelParams.Default.TemperatureAtCaliBaseline_mCelsius * 1.0 / 1000}℃";
            }
        }
    }
}
