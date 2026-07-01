using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
    public partial class TabPageTemperatureExperiment : UserControl, IMainFormTabPage
    {
        public TabPageTemperatureExperiment()
        {
            InitializeComponent();
            comboBoxChannelType.SelectedIndex = 0;
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>() 
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,

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
        public CaliDataType CaliDataType { get => CaliDataType.None; }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            buttonSave.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = false;
        }
        public void RefreshData() { }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            int CheckedItems = checkedListBox1.CheckedItems.Count;

            int index = 0;
            for (index = 0; index < CheckedItems; index++)
            {
                dataGridView1.Columns[index].Visible = true;
                dataGridView1.Columns[index].HeaderText = checkedListBox1.CheckedItems[index].ToString();
            }
            for (; index < 8; index++)
                dataGridView1.Columns[index].Visible = false;
            dataGridView1.Rows.Clear();
            buttonStart.Enabled = CheckedItems > 0 && (currInstrument != null);
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = false;
            TemperatureAmplRecord.Clear();
            int CheckedItems = checkedListBox1.CheckedItems.Count;

            for (int index = 1; index <= CheckedItems; index++)
            {
                currInstrument?.WriteString($":MEASure:ITEM{index}:SOURce C{checkedListBox1.CheckedItems[index - 1].ToString()!.Substring(2, 1)}");
                currInstrument?.WriteString($":MEASure:ITEM{index}:TYPe AMPL");
            }
            timer1.Interval = (int)(numericUpDown1.Value * 1000);
            buttonSave.Enabled = false;
            buttonStop.Enabled = true;
            buttonStart.Enabled = false;
            dataGridView1.RowCount = 0;
            bStoped = false;
            timer1.Start();
        }
        bool bStoped = false;
        record TemperatureAmplPair
        {
            public double Temperature;
            public double Ampl;
        }
        List<List<TemperatureAmplPair>> TemperatureAmplRecord = new List<List<TemperatureAmplPair>>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            int CheckedItems = checkedListBox1.CheckedItems.Count;
            double[] measureResilt = new double[CheckedItems];
            for (int index = 1; index <= CheckedItems; index++)
            {
                string measureResultStr = "";
                while (measureResultStr.Trim() == "")
                {
                    currInstrument!.WriteString($":MEASure:ITEM{index}:VAL?");
                    measureResultStr = currInstrument!.ReadShortString();
                }
                measureResilt[index - 1] = double.Parse(measureResultStr);
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $"? GetPhyChannelTemperatures{comboBoxChannelType.SelectedIndex+1}";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readBack = currInstrument.ReadString();
            if (readBack == "")
            {
                if (!bStoped)
                    timer1.Start();
                return;
            }
            double temperatureResult = double.Parse(readBack);
            dataGridView1.Rows.Add(1);
            List<TemperatureAmplPair> currTemperatureAmplPairs = new List<TemperatureAmplPair>();
            for (int index = 0; index < CheckedItems; index++)
            {
                currTemperatureAmplPairs.Add(new TemperatureAmplPair() { Temperature = temperatureResult, Ampl = measureResilt[index] });
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[index].Value = $"{temperatureResult.ToString("F2")}℃:{measureResilt[index].ToString("F4")}V";
            }
            TemperatureAmplRecord.Add(currTemperatureAmplPairs);

            if (!bStoped)
                timer1.Start();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string FileNamePrefix = textBoxFileNamePrefix.Text;
            int CheckedItems = checkedListBox1.CheckedItems.Count;
            List<FileStream> list_fs = new List<FileStream>();
            List<StreamWriter> list_sw = new List<StreamWriter>();
            for (int index = 0; index < CheckedItems; index++)
            {
                string channel_name = $"_C{checkedListBox1.CheckedItems[index].ToString()!.Substring(2, 1)}";
                string fileName = FileNamePrefix + channel_name + ".txt";
                list_fs.Add(new FileStream(fileName, FileMode.Create));
                list_sw.Add(new StreamWriter(list_fs[list_fs.Count - 1]));
                list_sw[list_sw.Count - 1].WriteLine($"//归一化温度为:{textBoxStandard.Text}℃");
            }
            double standardTemperature = double.Parse(textBoxStandard.Text);
            double IntervalTemperature = double.Parse(textBoxMinInterval.Text);

            Dictionary<double, (int, int)> normalization = new Dictionary<double, (int, int)>();

            for (int step = -100; step < 100; step++)
            {
                double currTemp = standardTemperature + IntervalTemperature * step;
                int currStart = -1;
                int currEnd = -1;

                for (int index = 0; index < TemperatureAmplRecord.Count; index++)
                {
                    if (TemperatureAmplRecord[index][0].Temperature > currTemp - IntervalTemperature / 2)
                    {
                        if (currStart == -1)
                            currStart = index;
                        currEnd = index;
                        if (TemperatureAmplRecord[index][0].Temperature < currTemp + IntervalTemperature / 2)
                        {
                            normalization.Add(currTemp, (currStart, currEnd));
                            break;
                        }
                    }
                }
            }
            List<double> normalAmpl = new List<double>();

            var start_endIndex = normalization[standardTemperature];
            for (int index = 0; index < CheckedItems; index++)
            {
                //取 平均
                normalAmpl.Add((TemperatureAmplRecord[start_endIndex.Item1][index].Ampl + TemperatureAmplRecord[start_endIndex.Item2][index].Ampl) / 2);
            }
            foreach (var v in normalization)
            {
                for (int index = 0; index < CheckedItems; index++)
                {
                    double avarage = (TemperatureAmplRecord[v.Value.Item1][index].Ampl + TemperatureAmplRecord[v.Value.Item2][index].Ampl) / 2;
                    list_sw[index].WriteLine($"{v.Key},{(avarage / normalAmpl[index]).ToString("F3")}");
                }
            }
            for (int index = 0; index < CheckedItems; index++)
            {
                list_sw[index].Close();
                list_fs[index].Close();
            }
            MessageBox.Show("保存成功！");
        }

        private void buttonBrower_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFileNamePrefix.Text = saveFileDialog1.FileName;
            }
        }

        private void buttonSendFanSpeed_Click(object sender, EventArgs e)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $" FanSpeed,{numericUpDownWhichFan.Value-1} {numericUpDownFanSpeed.Value}";
            currInstrument!.WriteString(scpiCmd);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            bStoped = true;
            timer1.Stop();
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            buttonSave.Enabled = dataGridView1.Rows.Count > 1;
            checkedListBox1.Enabled = true;
        }
    }
}
