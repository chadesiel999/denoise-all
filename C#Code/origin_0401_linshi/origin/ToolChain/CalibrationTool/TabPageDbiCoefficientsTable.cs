using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageDbiCoefficientsTable : UserControl, IMainFormTabPage
    {
        public TabPageDbiCoefficientsTable()
        {
            InitializeComponent();
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        public void RefreshData()
        {
        }
        private void Init()
        {
            comboBoxChannel.Items.Clear();
            for (int id = (int)ChannelId.C1; id < ServerDomainConstants.AnalogChannelCount; id++)
                comboBoxChannel.Items.Add((ChannelId)id);
            comboBoxBandMode.SelectedIndex = 0;
            comboBoxCaliType.SelectedIndex = 0;
            comboBoxChannel.SelectedIndex = 0;
            comboBoxFilterbandMode.SelectedIndex = 0;
            comboBoxSubband.SelectedIndex = 0;

            currBandMode = 0;
            currChannelIndex = 0;
            currDbiCoefficientsTableType = (DbiCoefficientsTablesType)comboBoxCaliType.SelectedIndex;
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            buttonReadFile.Enabled = (currInstrument != null);
            buttonReSendAll.Enabled = (currInstrument != null);
            if (currInstrument != null)
                Init();
        }
        private int currBandMode = 0;
        private int currChannelIndex = 0;
        private int currSubBandIndex = 0;
        private int currFilterBandMode = 0;
        private DbiCoefficientsTablesType currDbiCoefficientsTableType = DbiCoefficientsTablesType.AmpFreqCoefficients;
        private void validSetting()
        {
            currBandMode = comboBoxBandMode.SelectedIndex;
            currDbiCoefficientsTableType = (DbiCoefficientsTablesType)comboBoxCaliType.SelectedIndex;
            ChannelId channelID = (ChannelId)comboBoxChannel.SelectedItem;
            currChannelIndex = (int)channelID;
            currFilterBandMode = comboBoxFilterbandMode.SelectedIndex;
            currSubBandIndex = comboBoxSubband.SelectedIndex;

            currChannelIndex = panelChannelSelect.Visible ? currChannelIndex : 0;
            currFilterBandMode = panelFilterbandModeSelect.Visible ? currFilterBandMode : 0;
            currSubBandIndex = panelSubBandSelect.Visible ? currSubBandIndex : 0;
        }
        private void comboBoxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedItemChanged(sender, e);
        }

        private void comboBoxCaliType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DbiCoefficientsTablesType type = (DbiCoefficientsTablesType)comboBoxCaliType.SelectedIndex;
            panelChannelSelect.Visible = (type != DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients) && (type != DbiCoefficientsTablesType.InterpolationCoefficients);
            panelFilterbandModeSelect.Visible = (type == DbiCoefficientsTablesType.InterpolationCoefficients);
            panelSubBandSelect.Visible = (type != DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients) && (type != DbiCoefficientsTablesType.InterpolationCoefficients);
            OnSelectedItemChanged(sender, e);
        }
        private void comboBoxSubband_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedItemChanged(sender, e);
        }
        private void comboBoxFilterbandMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedItemChanged(sender, e);
        }
        private void comboBoxBandMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedItemChanged(sender, e);
        }
        private void OnSelectedItemChanged(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
                return;
            if (comboBoxBandMode.SelectedIndex == -1) return;
            if (comboBoxCaliType.SelectedIndex == -1) return;
            if (comboBoxChannel.SelectedIndex == -1) return;
            if (comboBoxFilterbandMode.SelectedIndex == -1) return;
            if (comboBoxSubband.SelectedIndex == -1) return;
            //共5个参数
            currBandMode = comboBoxBandMode.SelectedIndex;
            currDbiCoefficientsTableType = (DbiCoefficientsTablesType)comboBoxCaliType.SelectedIndex;
            ChannelId channelID = (ChannelId)comboBoxChannel.SelectedItem;
            currChannelIndex = (int)channelID;
            currFilterBandMode = comboBoxFilterbandMode.SelectedIndex;
            currSubBandIndex = comboBoxSubband.SelectedIndex;

            InstrumentInteract.DbiCoefficientsTable_GetFromServer(currInstrument, currDbiCoefficientsTableType);

            richTextBoxUsingCaliData.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            int dataCount = DbiCoefficientsTables.Default.PerDataCount(currDbiCoefficientsTableType);
            for (int i = 0; i < dataCount; i++)
                stringBuilder.AppendLine((i.ToString() + ".").PadRight(6, ' ') + DbiCoefficientsTables.Default[currDbiCoefficientsTableType, i, currBandMode, currChannelIndex, currSubBandIndex, currFilterBandMode].ToString().PadLeft(10, ' '));
            richTextBoxUsingCaliData.Text = stringBuilder.ToString();
            validSetting();
        }
        bool bScrolling = false;
        private int GetLineNoVscroll(RichTextBox rtb)
        {
            //获得当前坐标信息
            Point p = rtb.Location;
            int crntFirstIndex = rtb.GetCharIndexFromPosition(p);
            int crntFirstLine = rtb.GetLineFromCharIndex(crntFirstIndex);
            return crntFirstLine;
        }
        private void TrunRowsId(int iCodeRowsID, RichTextBox rtb)
        {
            try
            {
                rtb.SelectionStart = rtb.GetFirstCharIndexFromLine(iCodeRowsID);
                rtb.SelectionLength = 0;
                rtb.ScrollToCaret();
            }
            catch
            {

            }
        }
        private void buttonReadFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("请先选择正确的文件！");
                return;
            }
            if (comboBoxCaliType.SelectedIndex < 0)
            {
                MessageBox.Show("没有需要配置的系数类型！");
                return;
            }
            validSetting();
            Cursor= Cursors.WaitCursor;
            string[] fileContentLines = File.ReadAllLines(textBox1.Text);
            int dataCount = DbiCoefficientsTables.Default.PerDataCount(currDbiCoefficientsTableType);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < fileContentLines.Length && i < dataCount; i++)
                DbiCoefficientsTables.Default[currDbiCoefficientsTableType, i, currBandMode, currChannelIndex, currSubBandIndex, currFilterBandMode] = int.Parse(fileContentLines[i]);

            InstrumentInteract.DbiCoefficientsTable_SaveToFile(currInstrument, currDbiCoefficientsTableType,currBandMode,currChannelIndex,currSubBandIndex,currFilterBandMode);//在保存时，自动进行了数据传输

            OnSelectedItemChanged(null, null);
            Cursor = Cursors.Default;
            MessageBox.Show("OK");
        }

        private void richTextBoxFileContent_VScroll(object sender, EventArgs e)
        {
            if (bScrolling)
                return;
            int adjust = 1;
            int crntLastLine = GetLineNoVscroll(richTextBoxFileContent) - adjust;
            bScrolling = true;
            TrunRowsId(crntLastLine, richTextBoxUsingCaliData);
            bScrolling = false;
        }

        private void richTextBoxUsingCaliData_VScroll(object sender, EventArgs e)
        {
            if (bScrolling)
                return;
            int adjust = 1;
            bScrolling = true;
            int crntLastLine = GetLineNoVscroll(richTextBoxUsingCaliData) - adjust;
            TrunRowsId(crntLastLine, richTextBoxFileContent);
            bScrolling = false;
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                string[] fileContentLines = File.ReadAllLines(textBox1.Text);
                richTextBoxFileContent.Clear();
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < fileContentLines.Length; i++)
                    stringBuilder.AppendLine((i.ToString() + ".").PadRight(6, ' ') + fileContentLines[i].ToString().PadLeft(10, ' '));
                richTextBoxFileContent.Text = stringBuilder.ToString();
            }
        }

        private void buttonReSendAll_Click(object sender, EventArgs e)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " Message2AcquirerAnalogChannel,ResendAllCoefficientsTables";
            currInstrument!.WriteString(scpiCmd);
        }
    }
}
