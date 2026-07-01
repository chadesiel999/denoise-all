using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageCaliData_AcqSync : UserControl,IMainFormTabPage
    {
        /// <summary>
        /// readme:
        /// 1、此工具是手工校准单片ADC中的多个核之间的采集 亚稳态的问题。该亚稳态只与采样时钟和ADC的相位调整有关。而与模拟通道的信号无关。
        /// 2、由于DBI项目中存在子带的概念，该概念与通道的概念是不一样的，并存在非一一对应的情况。所以其序号在DBI项目中和非DBI项目中存在不一样的防疫。
        ///    在dbi项目中，其序号就是子带的含义，也就是采集板顺序的含义，从0开始的自然数。
        ///    在非dbi项目中，其序号是通道的含义，具体到哪个采集板的哪个ADC，需要看板卡的插入位置而定。
        /// </summary>
        public TabPageCaliData_AcqSync()
        {
            InitializeComponent();
            Init();
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_HD4G,
            ProductType.ForTest,
            ProductType.B21_JinHui_PXI,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_HB8G,
            ProductType.B21_DBI16G,
            ProductType.B21_DBI20G,
            ProductType.B21_MS2G,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        public CaliDataType CaliDataType { get => CaliDataType.TiAdc_SyncSampleClock; }
        private IInstrumentSession? currInstrument = null;
        private void Init()
        {
            dataGridViewCaliData_AcqSync.RowCount = CaliConstants.Fixed_PerChannelMergeAdcMaxCount;
            for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
            {
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[0].ReadOnly = true;
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[0].Value = $"Adc{adcIndex + 1}";
            }

            comboBoxCaliDataChannelSelectedChannel.SelectedIndex = 0;

            dataGridViewCaliData_AcqSync.Columns[1].Frozen = true;
            dataGridViewCaliData_AcqSync.Columns[1].Visible = true;
            dataGridViewCaliData_AcqSync.Columns[1].DefaultCellStyle.BackColor = Color.DarkGray;
            dataGridViewCaliData_AcqSync.Columns[2].Frozen = true;
            dataGridViewCaliData_AcqSync.Columns[2].Visible = true;
            dataGridViewCaliData_AcqSync.Columns[2].DefaultCellStyle.BackColor = Color.DarkGray;
            //目前暂时没有使用
            //dataGridViewCaliData_AcqSync.Columns[2].Visible = false;
            dataGridViewCaliData_AcqSync.Columns[3].Visible = false;
            dataGridViewCaliData_AcqSync.Columns[4].Visible = false;
            dataGridViewCaliData_AcqSync.Columns[5].Visible = false;
            dataGridViewCaliData_AcqSync.Columns[6].Visible = false;
            RefreshData();
        }
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            if (currInstrument != null)
            {
                //if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI16G || ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI20G)
                //{
                //    comboBoxCaliDataChannelSelectedChannel.Items.Clear();
                //    for (int i = 0; i < CaliConstants.Fixed_AcqBoardMaxCount; i++)
                //        comboBoxCaliDataChannelSelectedChannel.Items.Add($"子带{i + 1}");
                //    labelTitle.Text = "子带选择：";

                //}
                //else
                //{
                //    comboBoxCaliDataChannelSelectedChannel.Items.Clear();
                //    for (int i = 0; i < ServerDomainConstants.AnalogChannelCount; i++)
                //        comboBoxCaliDataChannelSelectedChannel.Items.Add($"通道{i + 1}");
                //    labelTitle.Text = "通道选择：";
                //}
                comboBoxCaliDataChannelSelectedChannel.SelectedIndex = 0;
            }
        }
        enum AcqSyncColumnIndex : int
        {
            Start = 1,
            SampleClock10G = 1,
            SampleClock20G = 2,
            SyncReset = 3,
            RM = 4,
            Serdes = 5,
            WriteEnable = 6,
            End = 7
        }
        public void RefreshData()
        {
            int currAcqBdIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;

            for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
            {
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.SampleClock10G].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].Sample10GClockDelay.ToString();
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.SampleClock20G].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].Sample20GClockDelay.ToString();
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.SyncReset].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].SyncResetDelay.ToString();
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.RM].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].RMDelay.ToString();
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.Serdes].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].SerdesDelay.ToString();
                dataGridViewCaliData_AcqSync.Rows[adcIndex].Cells[(int)AcqSyncColumnIndex.WriteEnable].Value = TiAdc_SyncSampleClock.Default[currAcqBdIndex, adcIndex].WriteEnableDelay.ToString();
            }
        }
        private void CaliData_AcqSync_SendGridData(int rowIndex, int columnIndex, bool bNeedSend)
        {
            DataGridViewCell currCell = dataGridViewCaliData_AcqSync.Rows[rowIndex].Cells[columnIndex];
            UInt32 data = 0;
            UInt32.TryParse(currCell.Value.ToString(), out data);
            AcqSyncColumnIndex enumColumn = (AcqSyncColumnIndex)columnIndex;
            //AcqSyncItem 是结构，不是引用类型的，之后需要拷贝
            int currAcqBdIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            AcqSyncItem currAcqSyncItem = TiAdc_SyncSampleClock.Default[currAcqBdIndex, rowIndex];

            UInt32 oldData = enumColumn switch
            {
                AcqSyncColumnIndex.SampleClock10G => currAcqSyncItem.Sample10GClockDelay,
                AcqSyncColumnIndex.SampleClock20G => currAcqSyncItem.Sample20GClockDelay,
                AcqSyncColumnIndex.SyncReset => currAcqSyncItem.SyncResetDelay,
                AcqSyncColumnIndex.RM => currAcqSyncItem.RMDelay,
                AcqSyncColumnIndex.Serdes => currAcqSyncItem.SerdesDelay,
                AcqSyncColumnIndex.WriteEnable => currAcqSyncItem.WriteEnableDelay,
                _ => data

            };
            if (oldData == data) //判断新旧值，避免连续发送
                return;
            switch (enumColumn)
            {
                case AcqSyncColumnIndex.SampleClock10G: currAcqSyncItem.Sample10GClockDelay = data; currCell.Value = currAcqSyncItem.Sample10GClockDelay.ToString(); break;
                case AcqSyncColumnIndex.SampleClock20G: currAcqSyncItem.Sample20GClockDelay = data; currCell.Value = currAcqSyncItem.Sample20GClockDelay.ToString(); break;
                case AcqSyncColumnIndex.SyncReset: currAcqSyncItem.SyncResetDelay = data; currCell.Value = currAcqSyncItem.SyncResetDelay.ToString(); break;
                case AcqSyncColumnIndex.RM: currAcqSyncItem.RMDelay = data; currCell.Value = currAcqSyncItem.RMDelay.ToString(); break;
                case AcqSyncColumnIndex.Serdes: currAcqSyncItem.SerdesDelay = data; currCell.Value = currAcqSyncItem.SerdesDelay.ToString(); break;
                case AcqSyncColumnIndex.WriteEnable: currAcqSyncItem.WriteEnableDelay = data; currCell.Value = currAcqSyncItem.WriteEnableDelay.ToString(); break;
            };
            TiAdc_SyncSampleClock.Default[currAcqBdIndex, rowIndex] = currAcqSyncItem;
            if (bNeedSend && currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.TiAdc_SyncSampleClock);
        }
        private void buttonCaliData_AcqSyncLoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.TiAdc_SyncSampleClock);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_AcqSyncSaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.TiAdc_SyncSampleClock);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void onCaliData_AcqSync_NeedRefreshGrid(object sender, DataGridViewCellEventArgs e)
        {
            CaliData_AcqSync_SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_AcqSync_AutoSend.Checked);
        }

        private void buttonCaliData_AcqSync_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            for (int rowIndex = 0; rowIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; rowIndex++)
            {
                for (int columnIndex = (int)AcqSyncColumnIndex.Start; columnIndex < (int)AcqSyncColumnIndex.End; columnIndex++)
                    CaliData_AcqSync_SendGridData(rowIndex, columnIndex, false);
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.TiAdc_SyncSampleClock);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private void buttonCaliData_LoadDefualtValue_AcqSync_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("装载缺省值将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            TiAdc_SyncSampleClock.Default.LoadDefaultValue();
            RefreshData();
            if (currInstrument!=null)
                InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.TiAdc_SyncSampleClock);
        }

        private void comboBoxCaliDataChannelSelectedChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void buttonRead5200AdcWindow_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "ADC5200SyncWindowRegValue";
            currInstrument.WriteString(scpiCmd);
            Thread.Sleep(1000);
            string readbackStr= currInstrument.ReadString();
            richTextBoxAdc5200SyncWindows.Text= readbackStr;
        }
    }
}
