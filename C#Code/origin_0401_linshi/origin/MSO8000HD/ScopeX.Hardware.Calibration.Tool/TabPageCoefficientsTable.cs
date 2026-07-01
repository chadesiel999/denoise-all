using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.Collections.Generic;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageCoefficientsTable : UserControl, IMainFormTabPage
    {
        public TabPageCoefficientsTable()
        {
            InitializeComponent();
            Init();
        }
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
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private void Init()
        {
            comboBoxChannel.Items.Clear();
            comboBoxCaliType.Items.Clear();
            for (int id = (int)ChannelId.C1; id < ServerDomainConstants.AnalogChannelCount; id++)
                comboBoxChannel.Items.Add((ChannelId)id);
            comboBoxChannel.SelectedIndex = 0;
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            buttonReadFile.Enabled = (currInstrument != null);
            if (currInstrument != null)
                Init();
        }
        public void RefreshData()
        {
            comboBoxCaliType.Items.Clear();
            foreach (var kvp in ServerDomainConstants.ProductCoefficientsTableTypeDefine)
            {
                ComboxItemObject item = new ComboxItemObject() { Text = kvp.Value, Value = kvp.Key };
                comboBoxCaliType.Items.Add(item);
            }
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
            ComboxItemObject comboxItemObject = (ComboxItemObject)comboBoxCaliType.SelectedItem;
            CoefficientsTableType coefficientsTableType = (CoefficientsTableType)comboxItemObject.Value!;

            ChannelId channelID = (ChannelId)comboBoxChannel.SelectedItem;
            if (currInstrument == null)
                return;
            Cursor = Cursors.WaitCursor;
            InstrumentInteract.CaliData_Get(currInstrument, CaliDataType.CoefficientsTables);

            string[] fileContentLines = File.ReadAllLines(textBox1.Text);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < fileContentLines.Length && i < CoefficientsTables.Fixed_PerChannelDataCount; i++)
                CoefficientsTables.Default[coefficientsTableType, (int)channelID, i] = int.Parse(fileContentLines[i]);

            InstrumentInteract.CaliData_SaveData(currInstrument, CaliDataType.CoefficientsTables);//在保存时，自动进行了数据传输
            //当前的系数类型
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " CoefficientsTableType," + (int)coefficientsTableType;
            currInstrument.WriteString(scpiCmd);

            OnSelectedItemChanged(null, null);
            Cursor = Cursors.Default;
            MessageBox.Show("OK");
        }

        private void OnSelectedItemChanged(object? sender, EventArgs? e)
        {
            if (comboBoxChannel.SelectedIndex == -1 || comboBoxCaliType.SelectedIndex == -1)
                return;
            if (currInstrument == null)
                return;
            ComboxItemObject comboxItemObject = (ComboxItemObject)comboBoxCaliType.SelectedItem;
            CoefficientsTableType coefficientsTableType = (CoefficientsTableType)comboxItemObject.Value!;
            ChannelId channelID = (ChannelId)comboBoxChannel.SelectedItem;
            InstrumentInteract.CaliData_Get(currInstrument, CaliDataType.CoefficientsTables);

            richTextBoxUsingCaliData.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < CoefficientsTables.Fixed_PerChannelDataCount; i++)
                stringBuilder.AppendLine((i.ToString() + ".").PadRight(6, ' ') + CoefficientsTables.Default[coefficientsTableType, (int)channelID, i].ToString().PadLeft(10, ' '));
            richTextBoxUsingCaliData.Text = stringBuilder.ToString();
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
    }
}
