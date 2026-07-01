using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageSoftLA : UserControl, IMainFormTabPage
    {
        public TabPageSoftLA()
        {
            InitializeComponent();
            buttonCapture.Tag = "Capture";
            pictureBox1.MouseWheel += onMouseWhell_pictureBox1;
            comboBoxCursorAlign.SelectedIndex = 0;
            comboBoxDataDisplayFormat.SelectedIndex = 3;
        }
        IInstrumentSession? currInstrumentSession = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract) => currInstrumentSession = instrumentInteract;
        public void RefreshData() { }
        public CaliDataType CaliDataType { get => CaliDataType.None; }
        private bool bNeedRefresh = false;
        FpgaProjectLA? currFpgaProjectLA = null;
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_HD4G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private void onMouseWhell_pictureBox1(object? sender, MouseEventArgs? e)
        {
            if (Control.ModifierKeys != Keys.Control)
                return;
            if (e == null)
                return;
            if (e.Delta == 0)
                return;
            int zoomStep = 4;
            if (e.Delta > 0)    //放大
            {
                PerClockPixels += zoomStep;
                if (PerClockPixels > 128)
                    PerClockPixels = 128;
            }
            if (e.Delta < 0)    //缩小
            {
                PerClockPixels -= zoomStep;
                if (PerClockPixels < 8)
                    PerClockPixels = 8;
            }
            cursorAlignPosition = comboBoxCursorAlign.SelectedIndex == 0 ? 0 : PerClockPixels/2;
            DrawCapturedData();
        }
        private void buttonOpenProject_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            bNeedRefresh = false;
            textBoxProject.Text = openFileDialog1.FileName;

            currFpgaProjectLA = JsonSerializer.Deserialize<FpgaProjectLA>(File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8));
            if (currFpgaProjectLA==null)
                return ;
            comboBoxDefinedModule.Items.Clear();
            comboBoxGroup.Items.Clear();
            foreach (DefinedLAModule definedLAModule in currFpgaProjectLA.DefinedLAModules)
                comboBoxDefinedModule.Items.Add(definedLAModule.ModuleName);
            if (comboBoxDefinedModule.Items.Count > 0)
            {
                comboBoxDefinedModule.SelectedIndex = 0;
                foreach (GroupDefine groupDefine in currFpgaProjectLA.DefinedLAModules[0].GroupDefines)
                    comboBoxGroup.Items.Add(groupDefine.Name);

            }
            bNeedRefresh = true;
            if (comboBoxGroup.Items.Count > 0)
                comboBoxGroup.SelectedIndex = 0;
        }

        private void comboBoxDefinedModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bNeedRefresh)
                return;
            captureDataDecoder = null;
            DrawCapturedData();
            comboBoxGroup.Items.Clear();
            if (comboBoxDefinedModule.SelectedIndex < 0)
                return;
            bNeedRefresh = false;
            if (currFpgaProjectLA == null)
                return;
            foreach (GroupDefine groupDefine in currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines)
                comboBoxGroup.Items.Add(groupDefine.Name);
            bNeedRefresh = true;
            if (comboBoxGroup.Items.Count > 0)
                comboBoxGroup.SelectedIndex = 0;
        }

        private void comboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bNeedRefresh)
                return;
            if (comboBoxGroup.SelectedIndex < 0)
                return;
            if (currFpgaProjectLA == null)
                return;
            captureDataDecoder = null;
            DrawCapturedData();
            dataGridView1.RowCount = currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals.Count;
            int rowIndex = 0;
            foreach (Signal signal in currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals)
            {
                dataGridView1.Rows[rowIndex].Cells[0].Value = rowIndex == 0 ? "" : "↑";
                dataGridView1.Rows[rowIndex].Cells[1].Value = (rowIndex == dataGridView1.RowCount - 1) ? "" : "↓";
                if (signal.BitWidth > 1)
                {
                    dataGridView1.Rows[rowIndex].Cells[2].Value = "+";
                    dataGridView1.Rows[rowIndex].Cells[3].Value = $"{ signal.SignalName}[{ signal.BitWidth - 1}:0]";
                }
                else
                {
                    dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                    dataGridView1.Rows[rowIndex].Cells[3].Value = $"{ signal.SignalName}";
                }
                dataGridView1.Rows[rowIndex].Height = dataGridView1.RowTemplate.Height;
                dataGridView1.Rows[rowIndex].Cells[3].Tag = signal;
                dataGridView1.Rows[rowIndex].Cells[4].Value = "";
                rowIndex++;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.ColumnIndex != 1 && e.ColumnIndex != 2)
                return;
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
                return;
            if (e.ColumnIndex == 1 && e.RowIndex == dataGridView1.Rows.Count - 1)
                return;

            if (e.ColumnIndex == 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() == "")
                    return;
                //to->up
                Signal currRowSignal = (Signal)dataGridView1.Rows[e.RowIndex].Cells[3].Tag;
                Signal upRowSignal = (Signal)dataGridView1.Rows[e.RowIndex - 1].Cells[3].Tag;
                string currPluse = dataGridView1.Rows[e.RowIndex].Cells[2].Value?.ToString()??"";
                string upPluse = dataGridView1.Rows[e.RowIndex - 1].Cells[2].Value.ToString()??"";
                dataGridView1.Rows[e.RowIndex].Cells[3].Tag = upRowSignal;
                if (upRowSignal.BitWidth > 1)
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = $"{ upRowSignal.SignalName}[{ upRowSignal.BitWidth - 1}:0]";
                else
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = $"{ upRowSignal.SignalName}";

                dataGridView1.Rows[e.RowIndex - 1].Cells[3].Tag = currRowSignal;
                if (currRowSignal.BitWidth > 1)
                    dataGridView1.Rows[e.RowIndex - 1].Cells[3].Value = $"{ currRowSignal.SignalName}[{ currRowSignal.BitWidth - 1}:0]";
                else
                    dataGridView1.Rows[e.RowIndex - 1].Cells[3].Value = $"{ currRowSignal.SignalName}";
                dataGridView1.Rows[e.RowIndex - 1].Cells[3].Tag = currRowSignal;

                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex - 1].Cells[3];

                dataGridView1.Rows[e.RowIndex - 1].Cells[0].Value = ((e.RowIndex - 1) == 0) ? "" : "↑";
                dataGridView1.Rows[e.RowIndex].Cells[1].Value = (e.RowIndex == dataGridView1.RowCount - 1) ? "" : "↓";

                dataGridView1.Rows[e.RowIndex - 1].Cells[2].Value = currPluse;
                dataGridView1.Rows[e.RowIndex].Cells[2].Value = upPluse;

                for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[2].Value.ToString() == "-")
                    {
                        Signal signal = (Signal)dataGridView1.Rows[rowIndex].Cells[3].Tag;
                        dataGridView1.Rows[rowIndex].Height = (signal.BitWidth + 1) * dataGridView1.RowTemplate.Height;
                    }
                    else
                        dataGridView1.Rows[rowIndex].Height = dataGridView1.RowTemplate.Height;
                }
                lastDataGridView1_FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;

                DrawCapturedData();
            }
            else if (e.ColumnIndex == 1)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() == "")
                    return;
                //to->dn
                Signal currRowSignal = (Signal)dataGridView1.Rows[e.RowIndex].Cells[3].Tag;
                Signal dnRowSignal = (Signal)dataGridView1.Rows[e.RowIndex + 1].Cells[3].Tag;
                string currPluse = dataGridView1.Rows[e.RowIndex].Cells[2].Value?.ToString()??"";
                string dnPluse = dataGridView1.Rows[e.RowIndex + 1].Cells[2].Value?.ToString()??"";

                dataGridView1.Rows[e.RowIndex].Cells[3].Tag = dnRowSignal;
                if (dnRowSignal.BitWidth > 1)
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = $"{ dnRowSignal.SignalName}[{ dnRowSignal.BitWidth - 1}:0]";
                else
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value = $"{ dnRowSignal.SignalName}";

                dataGridView1.Rows[e.RowIndex + 1].Cells[3].Tag = currRowSignal;
                if (currRowSignal.BitWidth > 1)
                    dataGridView1.Rows[e.RowIndex + 1].Cells[3].Value = $"{ currRowSignal.SignalName}[{ currRowSignal.BitWidth - 1}:0]";
                else
                    dataGridView1.Rows[e.RowIndex + 1].Cells[3].Value = $"{ currRowSignal.SignalName}";

                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex + 1].Cells[3];

                dataGridView1.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex == 0) ? "" : "↑";
                dataGridView1.Rows[e.RowIndex + 1].Cells[1].Value = ((e.RowIndex + 1) == dataGridView1.RowCount - 1) ? "" : "↓";

                dataGridView1.Rows[e.RowIndex].Cells[2].Value = dnPluse;
                dataGridView1.Rows[e.RowIndex + 1].Cells[2].Value = currPluse;
                for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[2].Value.ToString() == "-")
                    {
                        Signal signal = (Signal)dataGridView1.Rows[rowIndex].Cells[3].Tag;
                        dataGridView1.Rows[rowIndex].Height = (signal.BitWidth + 1) * dataGridView1.RowTemplate.Height;
                    }
                    else
                        dataGridView1.Rows[rowIndex].Height = dataGridView1.RowTemplate.Height;
                }
                lastDataGridView1_FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;

                DrawCapturedData();
            }
            else if (e.ColumnIndex == 2)
            {
                Signal currRowSignal = (Signal)dataGridView1.Rows[e.RowIndex].Cells[3].Tag;
                if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "+")
                {
                    dataGridView1.Rows[e.RowIndex].Height = dataGridView1.RowTemplate.Height * (currRowSignal.BitWidth + 1);
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value = "-";
                }
                else if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "-")
                {
                    dataGridView1.Rows[e.RowIndex].Height = dataGridView1.RowTemplate.Height;
                    dataGridView1.Rows[e.RowIndex].Cells[2].Value = "+";
                }
                lastDataGridView1_FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;
                DrawCapturedData();
            }
        }
        private int DefaultRowHeight
        {
            get => dataGridView1.RowTemplate.Height;
        }
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex != 3 && e.ColumnIndex != 4) || e.RowIndex < 0 || currFpgaProjectLA==null)
                return;
            List<Signal> definedSignalList = currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals;
            Brush brush = new SolidBrush(dataGridView1.RowsDefaultCellStyle.ForeColor);
            int signalIndex = GetSignalDefineIndexByShowIndex(e.RowIndex);
            if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() != "-")
            {
                if (captureDataDecoder != null && e.ColumnIndex == 4)
                {
                    if (definedSignalList[signalIndex].BitWidth > 1)
                    {
                        string dataStr = FormatMultiBitData(captureDataDecoder.SplitResult[CursorAtClockIndex][signalIndex]);
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataStr;
                        e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = captureDataDecoder.SplitResult[CursorAtClockIndex][signalIndex] == 0 ? "0" : "1";
                        e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    }
                    //e.Handled = true;
                }
                return;
            }

            Brush backColorBrush = new SolidBrush(e.CellStyle.BackColor);
            Pen linePen = new Pen(dataGridView1.GridColor);
            Rectangle fillRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y + 1, e.CellBounds.Width - 1, e.CellBounds.Height - 3);
            e.Graphics.FillRectangle(backColorBrush, fillRect);
            //只有"-"才自行绘制
            Signal currRowSignal = (Signal)dataGridView1.Rows[e.RowIndex].Cells[3].Tag;
            if (e.ColumnIndex == 3)
            {
                e.Graphics.DrawString($"{currRowSignal.SignalName}[{currRowSignal.BitWidth - 1}:0]", e.CellStyle.Font, brush, new PointF(e.CellBounds.X + 2, e.CellBounds.Y));
                for (int bitIndex = 0; bitIndex < currRowSignal.BitWidth; bitIndex++)
                {
                    e.Graphics.DrawString(bitIndex.ToString(), e.CellStyle.Font, brush, new PointF(e.CellBounds.Right - 18, e.CellBounds.Y + (bitIndex + 1) * DefaultRowHeight));
                    e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left, e.CellBounds.Top + (bitIndex + 1) * DefaultRowHeight), new Point(e.CellBounds.Right, e.CellBounds.Top + (bitIndex + 1) * DefaultRowHeight));
                }
                e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Right - 1, e.CellBounds.Top - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1));
                e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1));
            }
            else if (e.ColumnIndex == 4)
            {
                //数据列
                if (captureDataDecoder != null)
                {
                    string dataStr = FormatMultiBitData(captureDataDecoder.SplitResult[CursorAtClockIndex][signalIndex]);
                    e.Graphics.DrawString(dataStr, e.CellStyle.Font, brush, new PointF(e.CellBounds.X + 2, e.CellBounds.Y));
                }
                for (int bitIndex = 0; bitIndex < currRowSignal.BitWidth; bitIndex++)
                {
                    if (captureDataDecoder != null)
                        e.Graphics.DrawString((captureDataDecoder.SplitResult[CursorAtClockIndex][signalIndex] & (1UL << bitIndex)) == 0 ? "0" : "1", e.CellStyle.Font, brush, new PointF(e.CellBounds.Right - 17, e.CellBounds.Y + (bitIndex + 1) * DefaultRowHeight));
                    //-
                    e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left, e.CellBounds.Top + (bitIndex + 1) * DefaultRowHeight), new Point(e.CellBounds.Right - 1, e.CellBounds.Top + (bitIndex + 1) * DefaultRowHeight));
                }
                //|
                e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Right - 1, e.CellBounds.Top - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1));
                //-
                e.Graphics.DrawLine(linePen, new Point(e.CellBounds.Left - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1), new Point(e.CellBounds.Right - 1, e.CellBounds.Top + (currRowSignal.BitWidth + 1) * DefaultRowHeight - 1));

            }
            e.Handled = true;
        }
        private int GetSignalDefineIndexByShowIndex(int showIndex)
        {
            if (currFpgaProjectLA == null)
                return 0;
            Signal showSignal = (Signal)dataGridView1.Rows[showIndex].Cells[3].Tag;
            for (int resultIndex = 0; resultIndex < currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals.Count; resultIndex++)
            {
                if (showSignal == currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals[resultIndex])
                    return resultIndex;
            }
            return 0;
        }
        private CaptureDataDecoder? captureDataDecoder = null;
        private int CursorAtClockIndex = 0;
        private int LeftClockIndex = 0;
        private int PerClockPixels = 8;
        private int dataDisplayFormat = 16;
        private int cursorAlignPosition = 0;
        private string FormatMultiBitData(ulong data)
        {
            string dataStr = dataDisplayFormat switch
            {
                2 => Convert.ToString((long)data, 2),
                8 => Convert.ToString((long)data, 8).ToUpper(),
                10 => data.ToString(),
                16 => Convert.ToString((long)data, 16).ToUpper(),
                _ => data.ToString(),
            };
            return dataStr;
        }
        private void DrawMultiBitData(Graphics g, Pen linePen, Brush dataBrush, Font dataFont, int left, int top, ulong data)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(left, top + 7));
            points.Add(new Point(left + 4, top));
            points.Add(new Point(left + PerClockPixels - 4, top));
            points.Add(new Point(left + PerClockPixels, top + 7));
            points.Add(new Point(left + PerClockPixels - 4, top + 14));
            points.Add(new Point(left + 4, top + 14));
            points.Add(new Point(left, top + 7));
            g.DrawLines(linePen, points.ToArray());
            string dataStr = FormatMultiBitData(data);
            SizeF size = g.MeasureString(dataStr, dataFont);
            if (size.Width < PerClockPixels - 8)
                g.DrawString(dataStr, dataFont, dataBrush, new PointF(left + (PerClockPixels - size.Width) / 2, top - 1));
            else
            {
                Pen dotPen = new Pen(linePen.Color, 1);
                dotPen.DashPattern = new float[] { 1, 1 };
                g.DrawLine(linePen, new Point(left + PerClockPixels / 2 - 2, top + 7), new Point(left + PerClockPixels / 2 + 2, top + 7));
            }

        }
        private void DrawCapturedData()
        {
            if (currFpgaProjectLA == null)
                return ;
            dataGridView1.Invalidate();
            Rectangle rect = new Rectangle(0, 0, pictureBox1.Width, dataGridView1.Height);
            Bitmap bitmap = new Bitmap(pictureBox1.Width, dataGridView1.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(new SolidBrush(Color.Black), rect);
            if (captureDataDecoder == null)
            {
                pictureBox1.Image = bitmap;
                return;
            }
            List<Signal> definedSignalList = currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals;
            Pen penSignalLine = new Pen(Color.Green);
            Pen penCursorLine = new Pen(Color.Yellow, 1);
            penCursorLine.DashPattern = new float[] { 1, 2, 1 };
            Font dataFont = dataGridView1.DefaultCellStyle.Font;
            int startRow = dataGridView1.FirstDisplayedScrollingRowIndex;
            int lastRow = startRow + dataGridView1.DisplayedRowCount(true);
            Brush dataBrush = new SolidBrush(Color.Yellow);
            int data_X = dataGridView1.GetCellDisplayRectangle(4, startRow, false).Left + 8;

            for (int showSignalIndex = startRow; showSignalIndex < lastRow; showSignalIndex++)
            {
                int signalIndex = GetSignalDefineIndexByShowIndex(showSignalIndex);
                int signalDrawTop = dataGridView1.GetCellDisplayRectangle(0, showSignalIndex, false).Top;
                for (int clockIndex = LeftClockIndex; clockIndex < captureDataDecoder.SplitResult.Count; clockIndex++)
                {
                    int left = clockIndex * PerClockPixels;
                    int right = (clockIndex + 1) * PerClockPixels;
                    if (dataGridView1.Rows[showSignalIndex].Cells[2].Value.ToString() == "-")
                    {
                        DrawMultiBitData(g, penSignalLine, dataBrush, dataFont, left, signalDrawTop, captureDataDecoder.SplitResult[clockIndex][signalIndex]);
                        for (int bitIndex = 0; bitIndex < definedSignalList[signalIndex].BitWidth; bitIndex++)
                        {
                            UInt64 currData = captureDataDecoder.SplitResult[clockIndex][signalIndex] & (1UL << bitIndex);
                            if (currData == 0)//高
                                g.DrawLine(penSignalLine, left, signalDrawTop + (bitIndex + 2) * DefaultRowHeight - 3, right, signalDrawTop + (bitIndex + 2) * DefaultRowHeight - 3);
                            else//低
                                g.DrawLine(penSignalLine, left, signalDrawTop + (bitIndex + 1) * DefaultRowHeight + 3, right, signalDrawTop + (bitIndex + 1) * DefaultRowHeight + 3);
                            //边沿
                            if (clockIndex > 0)
                            {
                                UInt64 lastData = captureDataDecoder.SplitResult[clockIndex - 1][signalIndex] & (1UL << bitIndex);
                                if (lastData != currData)
                                    g.DrawLine(penSignalLine, left, signalDrawTop + (bitIndex + 1) * DefaultRowHeight + 3, left, signalDrawTop + (bitIndex + 2) * DefaultRowHeight - 3);
                            }
                        }
                    }
                    else
                    {
                        if (definedSignalList[signalIndex].BitWidth > 1)
                        {
                            DrawMultiBitData(g, penSignalLine, dataBrush, dataFont, left, signalDrawTop, captureDataDecoder.SplitResult[clockIndex][signalIndex]);
                        }
                        else
                        {
                            if (captureDataDecoder.SplitResult[clockIndex][signalIndex] == 0)//高
                                g.DrawLine(penSignalLine, left, signalDrawTop + DefaultRowHeight - 3, right, signalDrawTop + DefaultRowHeight - 3);
                            else//低
                                g.DrawLine(penSignalLine, left, signalDrawTop + 3, right, signalDrawTop + 3);
                            //边沿
                            if (clockIndex > 0)
                            {
                                if (captureDataDecoder.SplitResult[clockIndex - 1][signalIndex] != captureDataDecoder.SplitResult[clockIndex][signalIndex])
                                    g.DrawLine(penSignalLine, left, signalDrawTop + 3, left, signalDrawTop + DefaultRowHeight - 3);
                            }
                        }
                    }
                }

            }
            #region 光标
            int top = dataGridView1.GetRowDisplayRectangle(startRow, false).Top;
            int bottom = dataGridView1.GetRowDisplayRectangle((lastRow - startRow) < 0 ? startRow : lastRow - 1, false).Bottom;
            if (CursorAtClockIndex >= LeftClockIndex)
                g.DrawLine(penCursorLine, new Point((CursorAtClockIndex - LeftClockIndex) * PerClockPixels + cursorAlignPosition, top),
                    new Point((CursorAtClockIndex - LeftClockIndex) * PerClockPixels + cursorAlignPosition, bottom));
            #endregion
            pictureBox1.Image = bitmap;
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (currInstrumentSession == null)
                return;
            if (currFpgaProjectLA == null)
                return;

            if (buttonCapture.Tag.ToString() == "Capture")
            {
                PackageToFPGA packageToFPGA = new PackageToFPGA();
                packageToFPGA.FPGAMark = "ACQ";
                packageToFPGA.GroupIndex = comboBoxGroup.SelectedIndex;
                packageToFPGA.ModuleIndex = comboBoxDefinedModule.SelectedIndex;
                packageToFPGA.PreTrigOfClkCount = 10;
                packageToFPGA.TrigConditionData = 0;
                JsonSerializerOptions options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                string dataStr = JsonSerializer.Serialize<PackageToFPGA>(packageToFPGA, options);

                string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
                scpiCmd += " SoftLAConfigData," + dataStr;
                currInstrumentSession.WriteString(scpiCmd);
                timer1.Start();
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            DrawCapturedData();
        }

        bool IsCursorMove = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currFpgaProjectLA == null || currInstrumentSession==null)
                return ;
            if (InstrumentInteract.Factory_ReadbackSoftLAIsFull(currInstrumentSession))
            {
                timer1.Stop();
                byte[] captureBinData = new byte[16 * 1024];
                int len = InstrumentInteract.Factory_ReadbackSoftLACaptureData(currInstrumentSession, ref captureBinData);
                if (len > 0)
                {
                    captureDataDecoder = new CaptureDataDecoder(captureBinData, currFpgaProjectLA.DefinedLAModules[comboBoxDefinedModule.SelectedIndex].GroupDefines[comboBoxGroup.SelectedIndex].SelectedSignals);
                    DrawCapturedData();
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int cursorX = CursorAtClockIndex * PerClockPixels + cursorAlignPosition;
            if (Math.Abs(e.X - cursorX) < 6)
            {
                Cursor = Cursors.VSplit;
                IsCursorMove = true;
            }
            else
                IsCursorMove = false;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsCursorMove)
            {
                CursorAtClockIndex = e.X / PerClockPixels;
                if (CursorAtClockIndex < 0)
                    CursorAtClockIndex = 0;
                if (captureDataDecoder == null)
                    CursorAtClockIndex = 0;
                if (CursorAtClockIndex > captureDataDecoder!.SplitResult.Count - 1)
                    CursorAtClockIndex = captureDataDecoder.SplitResult.Count - 1;
                DrawCapturedData();
            }
            IsCursorMove = false;
            Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsCursorMove)
            {
                CursorAtClockIndex = e.X / PerClockPixels;
                if (CursorAtClockIndex < 0)
                    CursorAtClockIndex = 0;
                if (captureDataDecoder == null)
                    CursorAtClockIndex = 0;
                else
                {
                    if (CursorAtClockIndex > captureDataDecoder.SplitResult.Count - 1)
                        CursorAtClockIndex = captureDataDecoder.SplitResult.Count - 1;
                }
                DrawCapturedData();
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            IsCursorMove = false;
            Cursor = Cursors.Default;
        }
        int lastDataGridView1_FirstDisplayedScrollingRowIndex = -1;
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridView1.FirstDisplayedScrollingRowIndex != lastDataGridView1_FirstDisplayedScrollingRowIndex)
            {
                lastDataGridView1_FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;
                DrawCapturedData();
            }
        }

        private void comboBoxDataDisplayFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataDisplayFormat = int.Parse(comboBoxDataDisplayFormat.SelectedItem.ToString()??"10");
            if (captureDataDecoder!=null)
            {
                DrawCapturedData();
                dataGridView1.Invalidate();
            }
        }

        private void comboBoxCursorAlign_SelectedIndexChanged(object sender, EventArgs e)
        {
            cursorAlignPosition = comboBoxCursorAlign.SelectedIndex == 0 ? 0 : PerClockPixels/2;
            if (captureDataDecoder != null)
            {
                DrawCapturedData();
            }
        }
    }
}
