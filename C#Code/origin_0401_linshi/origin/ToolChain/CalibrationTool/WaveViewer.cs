using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class WaveViewer : UserControl
    {
        public WaveViewer()
        {
            InitializeComponent();
        }
        private IInstrumentSession? currInstrument = null;
        public IInstrumentSession? CurrInstrument
        {
            set
            {
                currInstrument = value;
                if (currInstrument == null)
                {
                    timer1.Stop();
                    if (!bAtStopMode)
                        buttonADCRunStop_Click(buttonADCRunStop, new EventArgs());
                }
            }
        }

        public WaveDisplayParam[]? DisplayParam_Channel
        {
            get => displayParam_Channel;
        }
        public WaveDisplayParam[,]? DisplayParam_Adc
        {
            get => displayParam_Adc;
        }
        private string jsonFileName
        {
            get => Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".waveparam";
        }
        public void Run(bool bRun)
        {
            if (bRun)
                timer1.Start();
            else
            {
                timer1.Stop();
            }
        }

        public void SaveSetting()
        {
            if (DisplayParam_Channel == null)
                return;
            List<WaveDisplayParam> waveDisplayParamList = new List<WaveDisplayParam>();
            if (DisplayParam_Channel != null)
            {
                foreach (var param in DisplayParam_Channel)
                    waveDisplayParamList.Add(param);
            }
            if (DisplayParam_Adc != null)
            {
                for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                        waveDisplayParamList.Add(DisplayParam_Adc[adcIndex, coreIndex]);
                }
            }
            if (File.Exists(jsonFileName))
                File.Delete(jsonFileName);
            JsonSerializerOptions options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            File.WriteAllText(jsonFileName, JsonSerializer.Serialize<List<WaveDisplayParam>>(waveDisplayParamList, options), Encoding.UTF8);
        }
        WaveDisplayParam[]? displayParam_Channel;
        WaveDisplayParam[,]? displayParam_Adc;
        public void Wave_ViewInit()
        {
            AdcResolution = (int)(Math.Pow(2, ServerDomainConstants.AdcBits));
            List<WaveDisplayParam>? waveDisplayParamList = new List<WaveDisplayParam>();
            if (File.Exists(jsonFileName))
            {
                try
                {
                    waveDisplayParamList = JsonSerializer.Deserialize<List<WaveDisplayParam>>(File.ReadAllText(jsonFileName, Encoding.UTF8));
                }
                catch
                {
                    waveDisplayParamList = null;
                }
                if (waveDisplayParamList == null)
                    waveDisplayParamList = new List<WaveDisplayParam>();
            }
            #region 通道波形
            int needAddCount = CaliConstants.Fixed_MaxPhysicsChannelCount + CaliConstants.Fixed_PerChannelMergeAdcMaxCount * ServerDomainConstants.PerAdcCoreCount - waveDisplayParamList.Count;
            while (needAddCount > 0)
            {
                waveDisplayParamList.Add(new WaveDisplayParam());
                needAddCount--;
            }
            dataGridViewWaveDataView_Channel.RowCount = CaliConstants.Fixed_MaxPhysicsChannelCount;
            displayParam_Channel = new WaveDisplayParam[CaliConstants.Fixed_MaxPhysicsChannelCount];
            int displayParamIndex = 0;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                displayParam_Channel[channelIndex] = waveDisplayParamList[displayParamIndex++];
                dataGridViewWaveDataView_Channel.Rows[channelIndex].Cells[0].Value = $"CH{channelIndex + 1}";
                dataGridViewWaveDataView_Channel.Rows[channelIndex].Cells[1].Value = displayParam_Channel[channelIndex].bDisplay;
                dataGridViewWaveDataView_Channel.Rows[channelIndex].Cells[2].Style.BackColor = displayParam_Channel[channelIndex].GetColor();
            }
            #endregion
            comboBoxADC_Channel.Items.Clear();
            if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X || ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000A)
            {
                //JiHe_MSO7000X：采用交织模式。只有一块ADC，分4个核。与通道无关
                comboBoxADC_Channel.Items.Add("C1");
                comboBoxADC_Channel.Visible = false;
            }
            else
            {
                for (int chanelIndex = 0; chanelIndex < ServerDomainConstants.AnalogChannelCount; chanelIndex++)
                    comboBoxADC_Channel.Items.Add((ChannelId)chanelIndex);
                comboBoxADC_Channel.Visible = true;
            }
            #region ADC PerCore wave

            dataGridViewWaveDataView_Adc.RowCount = ServerDomainConstants.PerAnaChannelAdcCount * ServerDomainConstants.PerAdcCoreCount;
            int rowIndex = 0;
            displayParam_Adc = new WaveDisplayParam[CaliConstants.Fixed_PerChannelMergeAdcMaxCount, ServerDomainConstants.PerAdcCoreCount];
            for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
            {
                for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                {
                    displayParam_Adc[adcIndex, coreIndex] = waveDisplayParamList[displayParamIndex++];
                    dataGridViewWaveDataView_Adc.Rows[rowIndex].Cells[0].Value = $"Adc{adcIndex + 1}-Core[{coreIndex + 1}]";
                    dataGridViewWaveDataView_Adc.Rows[rowIndex].Cells[1].Value = displayParam_Adc[adcIndex, coreIndex].bDisplay;
                    dataGridViewWaveDataView_Adc.Rows[rowIndex].Cells[2].Style.BackColor = displayParam_Adc[adcIndex, coreIndex].GetColor();

                    rowIndex++;
                }
            }
            comboBoxADC_Channel.SelectedIndex = 0;
            #endregion

            CalcNeedDisplayCount();
            comboBoxAdcDotLineMode.SelectedIndex = 0;
            buttonADCRunStop.Tag = buttonADCRunStop.Text = "Stopped";
            timer1.Start();
        }

        private void dataGridViewWaveDataView_Channel_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (displayParam_Channel == null)
                return;
            if (dataGridViewWaveDataView_Channel.CurrentCell is DataGridViewCheckBoxCell)
            {
                for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
                {
                    DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridViewWaveDataView_Channel.Rows[channelIndex].Cells[1];
                    displayParam_Channel[channelIndex].bDisplay = (bool)cell.FormattedValue;
                    CalcNeedDisplayCount();
                }
                dataGridViewWaveDataView_Channel.CurrentCell = dataGridViewWaveDataView_Channel.Rows[dataGridViewWaveDataView_Channel.CurrentCell.RowIndex].Cells[2];
            }
        }
        int channelNeedDisplayCount = 0;
        int adcNeedDisplayCount = 0;
        private void CalcNeedDisplayCount()
        {
            bool bChanged = false;
            if (displayParam_Channel == null)
                return;
            int _channelNeedDisplayCount = displayParam_Channel.Sum<WaveDisplayParam>((o) => o.bDisplay ? 1 : 0);
            if (_channelNeedDisplayCount != channelNeedDisplayCount)
            {
                bChanged = true;
                channelNeedDisplayCount = _channelNeedDisplayCount;
            }
            int _adcNeedDisplayCount = 0;
            if (displayParam_Adc != null)
            {
                for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                    {
                        _adcNeedDisplayCount += displayParam_Adc[adcIndex, coreIndex].bDisplay ? 1 : 0;
                    }
                }
            }
            if (_adcNeedDisplayCount != adcNeedDisplayCount)
            {
                bChanged = true;
                adcNeedDisplayCount = _adcNeedDisplayCount;
            }
            if (bChanged && bAtStopMode)
                RedrawAtStopMode();
        }
        private void dataGridViewWaveDataView_Adc_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (displayParam_Adc == null)
                return;
            if (dataGridViewWaveDataView_Adc.CurrentCell is DataGridViewCheckBoxCell)
            {
                for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                    {
                        DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridViewWaveDataView_Adc.Rows[adcIndex * ServerDomainConstants.PerAdcCoreCount + coreIndex].Cells[1];
                        displayParam_Adc[adcIndex, coreIndex].bDisplay = (bool)cell.FormattedValue;
                        CalcNeedDisplayCount();
                    }
                }
                dataGridViewWaveDataView_Adc.CurrentCell = dataGridViewWaveDataView_Adc.Rows[dataGridViewWaveDataView_Adc.CurrentCell.RowIndex].Cells[2];
            }
        }

        int lastNeedDisplayCount = 0;
        List<ushort[]>? lastAllChannelData = null;
        List<ushort[]>? lastAllAdcCoreData = null;
        private void DrawCursor(Bitmap bitmap)
        {
            if (!checkBoxCursor.Checked)
                return;
            #region 画线
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.TranslateTransform(0, pictureBox1.Height / 2);
            Pen pen = new Pen(Color.Red);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pen.DashPattern = new float[] { 4.0f, 2.0f };
            g.DrawLine(pen, new PointF(labelCursorX1.Location.X + 7, 0 - rect.Height / 2), new PointF(labelCursorX1.Location.X + 7, rect.Height / 2));
            pen.Color = Color.Blue;
            g.DrawLine(pen, new PointF(labelCursorX2.Location.X + 7, 0 - rect.Height / 2), new PointF(labelCursorX2.Location.X + 7, rect.Height / 2));

            pen.Color = Color.Red;
            g.DrawLine(pen, new PointF(0, labelCursorY1.Location.Y - rect.Height / 2 + 7), new PointF(rect.Width, labelCursorY1.Location.Y - rect.Height / 2 + 7));
            pen.Color = Color.Blue;
            g.DrawLine(pen, new PointF(0, labelCursorY2.Location.Y - rect.Height / 2 + 7), new PointF(rect.Width, labelCursorY2.Location.Y - rect.Height / 2 + 7));
            #endregion

            string cursorResult = "";
            cursorResult = $"Y1At:{AdcResolution - (labelCursorY1.Location.Y + 7) * AdcResolution / rect.Height} , Y2At:{AdcResolution - (labelCursorY2.Location.Y + 7) * AdcResolution / rect.Height} , DeltaY={(labelCursorY1.Location.Y - labelCursorY2.Location.Y) * AdcResolution / rect.Height};";
            cursorResult += System.Environment.NewLine + $"X1At:{labelCursorX1.Location.X + 7} , X2At:{labelCursorX2.Location.X + 7} , DeltaX={labelCursorX1.Location.X - labelCursorX2.Location.X}";
            richTextBoxCursorResult.Text = cursorResult;
        }
        private static CancellationTokenSource? _Cts;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bAtStopMode || currInstrument == null)
                return;
            timer1.Stop();
            int totalSelectCount = channelNeedDisplayCount + adcNeedDisplayCount;
            if (totalSelectCount > 0)
            {
                lastNeedDisplayCount = totalSelectCount;

                Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.Black);

                if (channelNeedDisplayCount > 0)
                {
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000, _Cts?.Token);
                    if (allChannelData != null)
                    {
                        DrawChannelWaveMain(allChannelData, bitmap);
                        lastAllChannelData = allChannelData;
                    }
                }

                if (adcNeedDisplayCount > 0)
                {
                    int totalBytes = 0;
                    if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X || ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000A)
                        totalBytes = sizeof(ushort) * ServerDomainConstants.PerAnaChannelDataCount / ServerDomainConstants.PerAdcCoreCount * ServerDomainConstants.AnalogChannelCount;
                    List<ushort[]>? allAdcCoreData = InstrumentInteract.Factory_WaveData_Adc(currInstrument, 6_000, _Cts?.Token, totalBytes);
                    if (allAdcCoreData != null)
                    {
                        DrawAdcCoreWaveMain(allAdcCoreData, bitmap);
                        lastAllAdcCoreData = allAdcCoreData;
                    }
                }
                DrawCursor(bitmap);
                pictureBox1.Image = bitmap;
            }
            else if (lastNeedDisplayCount != totalSelectCount)
            {
                pictureBox1.Image = null;
                lastNeedDisplayCount = 0;
            }
            timer1.Start();
        }
        private int AdcResolution = (int)(Math.Pow(2, ServerDomainConstants.AdcBits));
        #region DrawWave
        private void DrawChannelWaveMain(List<ushort[]>? allChannelData, Bitmap bitmap)
        {
            if (allChannelData == null)
                return;
            Graphics g = Graphics.FromImage(bitmap);
            //g.TranslateTransform(0, pictureBox1.Height / 2);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            bool bTopNeedDisplay = false;
            if (displayParam_Channel != null)
            {
                for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
                {
                    if (displayParam_Channel[channelIndex].bDisplay)
                    {
                        if (channelIndex != currTopRowIndexofChannel)
                            DrawChannelWave(allChannelData, bitmap, rect, channelIndex);
                        else
                            bTopNeedDisplay = true;
                    }
                }
            }
            if (bTopNeedDisplay)
                DrawChannelWave(allChannelData, bitmap, rect, currTopRowIndexofChannel);
        }
        private void DrawAdcCoreWaveMain(List<ushort[]>? allAdcCoreData, Bitmap bitmap)
        {
            if (allAdcCoreData == null)
                return;
            Graphics g = Graphics.FromImage(bitmap);
            g.TranslateTransform(0, pictureBox1.Height / 2);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            int currChannelID = comboBoxADC_Channel.SelectedIndex;
            int topAdcIndex = currTopRowIndexofAdc / ServerDomainConstants.PerAdcCoreCount;
            int topCoreIndex = currTopRowIndexofAdc % ServerDomainConstants.PerAdcCoreCount;
            bool bTopAdcNeedDisplay = false;
            bool bDisplayByDotMode = comboBoxAdcDotLineMode.SelectedIndex != 0;
            for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount; adcIndex++)
            {
                for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                {
                    if (displayParam_Adc![adcIndex, coreIndex].bDisplay)
                    {
                        if (topAdcIndex == adcIndex && topCoreIndex == coreIndex)
                            bTopAdcNeedDisplay = true;
                        else
                        {
                            if (bDisplayByDotMode)
                                DrawAdcDataByDotMode(allAdcCoreData, bitmap, rect, currChannelID, adcIndex, coreIndex);
                            else
                                DrawAdcDataByLineMode(allAdcCoreData, g, rect, currChannelID, adcIndex, coreIndex);
                        }
                    }
                }
            }
            if (bTopAdcNeedDisplay)
            {
                if (bDisplayByDotMode)
                    DrawAdcDataByDotMode(allAdcCoreData, bitmap, rect, currChannelID, topAdcIndex, topCoreIndex);
                else
                    DrawAdcDataByLineMode(allAdcCoreData, g, rect, currChannelID, topAdcIndex, topCoreIndex);
            }
        }
        private void DrawChannelWave(List<ushort[]> dataList, Bitmap bitmap, Rectangle rect, int channelID)
        {
            Graphics g = Graphics.FromImage(bitmap);
            g.TranslateTransform(0, pictureBox1.Height / 2);
            Color waveColor = displayParam_Channel![channelID].GetColor();
            int extractNum = (int)numericUpDownWaveChannelExtractNum.Value;
            short[] currData = Array.ConvertAll<ushort, short>(dataList[channelID], (y) => (short)((AdcResolution / 2 - y) * rect.Height / AdcResolution));
            int yCenter = (AdcResolution / 2 - AdcResolution) * rect.Height / AdcResolution;
            g.DrawLine(new Pen(Color.Gray), new Point(0, 0), new Point(rect.Width, 0));
            if (extractNum <= 1)
            {
                List<Point> points = new List<Point>();
                Pen pen = new Pen(waveColor);
                int x = 0;
                foreach (short y in currData)
                {
                    points.Add(new Point(x, y));
                    x++;
                    if (x >= rect.Width)
                        break;
                }
                g.DrawLines(pen, points.ToArray());
            }
            else
            {
                Brush brush = new SolidBrush(waveColor);
                int dataIndex = 0;
                int x = 0;
                short max = short.MinValue;
                short min = short.MaxValue;
                for (; ; )
                {
                    max = short.MinValue;
                    min = short.MaxValue;
                    for (int i = 0; i < extractNum; i++)
                    {
                        if (currData[dataIndex + i] < min)
                            min = currData[dataIndex + i];
                        if (currData[dataIndex + i] > max)
                            max = currData[dataIndex + i];
                    }
                    dataIndex += extractNum;
                    g.FillRectangle(brush, x, min, 1, max - min);
                    if (dataIndex > currData.Length - extractNum)
                        break;
                    x++;
                    if (x >= rect.Width)
                        break;
                }
            }
        }
        private void DrawAdcDataByDotMode(List<ushort[]> dataList, Bitmap bitmap, Rectangle rect, int channelId, int adcIndex, int coreIndex)
        {
            Color color = displayParam_Adc![adcIndex, coreIndex].GetColor();
            ushort[] currData_source = dataList[((channelId * ServerDomainConstants.PerAnaChannelAdcCount) + adcIndex) * ServerDomainConstants.PerAdcCoreCount + coreIndex];
            List<Point> points = new List<Point>();
            int height = rect.Height;
            short[] currData = Array.ConvertAll<ushort, short>(currData_source, (y) => (short)((AdcResolution / 2 - y) * height / AdcResolution - 1));
            int x = 0;
            int step = (int)numericUpDownAdcZoomCount.Value;
            Pen pen = new Pen(color);
            foreach (short y in currData)
            {
                bitmap.SetPixel(x, y + height / 2, color);
                x += step;
                if (x >= bitmap.Width)
                    break;
            }
        }
        private void DrawAdcDataByLineMode(List<ushort[]> dataList, Graphics g, Rectangle rect, int channelId, int adcIndex, int coreIndex)
        {
            Color color = displayParam_Adc![adcIndex, coreIndex].GetColor();
            ushort[] currData_source = dataList[((channelId * ServerDomainConstants.PerAnaChannelAdcCount) + adcIndex) * ServerDomainConstants.PerAdcCoreCount + coreIndex];
            List<Point> points = new List<Point>();
            int height = rect.Height;
            short[] currData = Array.ConvertAll<ushort, short>(currData_source, (y) => (short)((AdcResolution / 2 - y) * height / AdcResolution - 1));
            int x = 0;
            int step = (int)numericUpDownAdcZoomCount.Value;
            foreach (short y in currData)
            {
                points.Add(new Point(x, y));
                x += step;
                if (x >= rect.Width)
                    break;
            }
            g.DrawLines(new Pen(color), points.ToArray());
        }
        private void RedrawAtStopMode()
        {
            if (!bAtStopMode)
                return;
            if (channelNeedDisplayCount + adcNeedDisplayCount == 0)
                return;
            if (pictureBox1.Width == 0 || pictureBox1.Height == 0)
                return;
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Black);
                DrawChannelWaveMain(lastAllChannelData, bitmap);
                DrawAdcCoreWaveMain(lastAllAdcCoreData, bitmap);
            }
            DrawCursor(bitmap);
            pictureBox1.Image = bitmap;
        }

        #endregion
        public class WaveDisplayParam
        {
            [JsonPropertyName("color")]
            public int IntColor
            {
                get;
                set;
            } = Color.White.ToArgb();
            public Color GetColor() => Color.FromArgb(IntColor);
            [JsonPropertyName("display")]
            public bool bDisplay
            {
                get; set;
            } = false;
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void onAdcSelectCtrl(object sender, EventArgs e)
        {
            bool bSelectAll = bool.Parse((sender as Button)?.Tag?.ToString() ?? "false");
            for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
            {
                for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                {
                    displayParam_Adc![adcIndex, coreIndex].bDisplay = bSelectAll;
                    int rowIndex = adcIndex * ServerDomainConstants.PerAdcCoreCount + coreIndex;
                    dataGridViewWaveDataView_Adc.Rows[rowIndex].Cells[1].Value = displayParam_Adc[adcIndex, coreIndex].bDisplay;
                }
            }
            CalcNeedDisplayCount();
        }

        private void onChannelSelectCtrl(object sender, EventArgs e)
        {
            bool bSelectAll = bool.Parse((sender as Button)?.Tag?.ToString() ?? "false");
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                displayParam_Channel![channelIndex].bDisplay = bSelectAll;
                dataGridViewWaveDataView_Channel.Rows[channelIndex].Cells[1].Value = displayParam_Channel[channelIndex].bDisplay;
            }
            CalcNeedDisplayCount();
        }

        private void dataGridViewWaveDataView_Channel_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                displayParam_Channel![e.RowIndex].IntColor = colorDialog1.Color.ToArgb();
                dataGridViewWaveDataView_Channel.Rows[e.RowIndex].Cells[2].Style.BackColor = displayParam_Channel[e.RowIndex].GetColor();
                dataGridViewWaveDataView_Channel.CurrentCell = dataGridViewWaveDataView_Channel.Rows[dataGridViewWaveDataView_Channel.CurrentCell.RowIndex].Cells[0];
            }
        }

        private void dataGridViewWaveDataView_Adc_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                int adcIndex = e.RowIndex / ServerDomainConstants.PerAdcCoreCount;
                int coreIndex = e.RowIndex % ServerDomainConstants.PerAdcCoreCount;
                displayParam_Adc![adcIndex, coreIndex].IntColor = colorDialog1.Color.ToArgb();
                dataGridViewWaveDataView_Adc.Rows[e.RowIndex].Cells[2].Style.BackColor = displayParam_Adc[adcIndex, coreIndex].GetColor();
                dataGridViewWaveDataView_Adc.CurrentCell = dataGridViewWaveDataView_Adc.Rows[dataGridViewWaveDataView_Adc.CurrentCell.RowIndex].Cells[0];
            }
        }
        private int currTopRowIndexofChannel = 0;
        private int currTopRowIndexofAdc = 0;
        private void dataGridViewWaveDataView_Adc_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            currTopRowIndexofAdc = e.RowIndex;
            RedrawAtStopMode();
        }
        private bool bAtStopMode => buttonADCRunStop.Tag.ToString() == "Stopped";

        private void dataGridViewWaveDataView_Channel_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            currTopRowIndexofChannel = e.RowIndex;
            RedrawAtStopMode();
        }

        private void buttonADCRunStop_Click(object sender, EventArgs e)
        {
            if (buttonADCRunStop.Tag.ToString() == "Stopped")
            {
                _Cts = new CancellationTokenSource();
                buttonADCRunStop.Tag = buttonADCRunStop.Text = "Running";
                buttonADCRunStop.BackColor = Color.Green;
            }
            else
            {
                _Cts?.Cancel();
                buttonADCRunStop.Tag = buttonADCRunStop.Text = "Stopped";
                buttonADCRunStop.BackColor = Color.Tomato;
                RedrawAtStopMode();
            }
        }
        private void ReadrawAtStopMode(object sender, EventArgs e)
        {
            RedrawAtStopMode();
        }

        private void buttonSaveChannelData_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接目标示波器！");
                return;
            }
            int selectCount = 0;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                if (displayParam_Channel![channelIndex].bDisplay)
                    selectCount++;
            }
            if (selectCount == 0)
            {
                MessageBox.Show("请先钩选需要保存的通道！");
                return;
            }
            if (this.folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            string nowTimeStr = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string path = this.folderBrowserDialog1.SelectedPath;
            if (path[path.Length - 1] != '\\')
                path = path + '\\';
            
            List<ushort[]>? allChannelData = new List<ushort[]>();
            if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X)
                allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000);
            else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X)
                allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrument, 20_000);

            if (allChannelData == null)
            {
                MessageBox.Show("数据读取错误！");
                return;
            }
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                if (displayParam_Channel![channelIndex].bDisplay)
                {
                    using (StreamWriter sw = new StreamWriter($"{path}CH{channelIndex + 1}_{nowTimeStr}.txt", false, Encoding.UTF8))
                    {
                        int len = allChannelData[channelIndex].Length;
                        for (int i = 0; i < len; i++)
                            sw.WriteLine(allChannelData[channelIndex][i]);
                    }
                }
            }
        }

        private void checkBoxCursor_CheckedChanged(object sender, EventArgs e)
        {
            labelCursorX1.Visible = checkBoxCursor.Checked;
            labelCursorX2.Visible = checkBoxCursor.Checked;
            labelCursorY1.Visible = checkBoxCursor.Checked;
            labelCursorY2.Visible = checkBoxCursor.Checked;

            richTextBoxCursorResult.Visible = checkBoxCursor.Checked;
            buttonCursorReset.Visible = checkBoxCursor.Checked;
        }
        private Point m_lastCursorXPoint;
        private Point m_lastCursorXMousePoint;
        private void onCursorXMouseDown(object sender, MouseEventArgs e)
        {
            Control control = (Label)sender;
            control.Cursor = Cursors.VSplit;
            m_lastCursorXMousePoint = Control.MousePosition;
            m_lastCursorXPoint = control.Location;
        }
        private void onCursorXMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Control control = (Label)sender;
                control.Location = new Point(m_lastCursorXPoint.X + Control.MousePosition.X - m_lastCursorXMousePoint.X, 0);
                if (bAtStopMode)
                    RedrawAtStopMode();
            }
        }

        private void onCursorXMouseUp(object sender, MouseEventArgs e)
        {
            Control control = (Label)sender;
            control.Cursor = Cursors.Default;
        }
        private Point m_lastCursorYPoint;
        private Point m_lastCursorYMousePoint;
        private void onCursorYMouseDown(object sender, MouseEventArgs e)
        {
            Control control = (Label)sender;
            control.Cursor = Cursors.HSplit;
            m_lastCursorYMousePoint = Control.MousePosition;
            m_lastCursorYPoint = control.Location;
        }

        private void onCursorYMouseUp(object sender, MouseEventArgs e)
        {
            Control control = (Label)sender;
            control.Cursor = Cursors.Default;
        }

        private void onCursorYMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Control control = (Label)sender;
                control.Location = new Point(0, m_lastCursorYPoint.Y + Control.MousePosition.Y - m_lastCursorYMousePoint.Y);
                if (bAtStopMode)
                    RedrawAtStopMode();
            }
        }

        private void buttonCursorReset_Click(object sender, EventArgs e)
        {
            labelCursorX1.Location = new Point(50, 0);
            labelCursorX2.Location = new Point(100, 0);
            labelCursorY1.Location = new Point(0, 50);
            labelCursorY2.Location = new Point(0, 100);
            if (bAtStopMode)
                RedrawAtStopMode();
        }

        private void BtnSaveCoreData_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接目标示波器！");
                return;
            }
            int selectCount = 0;
            for (int coreIndex = 0; coreIndex < CaliConstants.Fixed_PerAdcCoreMaxCount; coreIndex++)
            {
                if (displayParam_Adc![0, coreIndex].bDisplay)
                    selectCount++;
            }
            if (selectCount == 0)
            {
                MessageBox.Show("请先钩选需要保存的通道！");
                return;
            }
            if (this.folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            string nowTimeStr = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string path = this.folderBrowserDialog1.SelectedPath;
            if (path[path.Length - 1] != '\\')
                path = path + '\\';
            int totalBytes = 0;
            if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X || ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000A)
                totalBytes = sizeof(ushort) * ServerDomainConstants.PerAnaChannelDataCount / ServerDomainConstants.PerAdcCoreCount * ServerDomainConstants.AnalogChannelCount;
            List<ushort[]>? allAdcData = InstrumentInteract.Factory_WaveData_Adc(currInstrument, 6_000, _Cts?.Token, totalBytes);
            if (allAdcData == null)
            {
                MessageBox.Show("数据读取错误！");
                return;
            }
            for (int coreIndex = 0; coreIndex < CaliConstants.Fixed_PerAdcCoreMaxCount; coreIndex++)
            {
                if (displayParam_Adc![0, coreIndex].bDisplay)
                {
                    using (StreamWriter sw = new StreamWriter($"{path}Core{coreIndex + 1}_{nowTimeStr}.txt", false, Encoding.ASCII))
                    {
                        int len = allAdcData[coreIndex].Length;
                        for (int i = 0; i < len; i++)
                            sw.WriteLine(allAdcData[coreIndex][i]);
                    }
                }
            }
        }
    }
}
