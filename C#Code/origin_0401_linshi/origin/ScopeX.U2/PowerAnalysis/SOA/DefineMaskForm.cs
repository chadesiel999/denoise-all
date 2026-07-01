// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/31</date>

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Controls.Language;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using Svg;
    using Veldrid.Common.Tools;

    public partial class DefineMaskForm :FloatForm, IStylize
    {
        private Size _IndependentSize;

        public Size LastSize { get; set; }
        private List<DisPlayPointData> _DisPlayData = new();
        public DataTableFigure TableForm { get; set; } = null;

        private List<PointData> _PointData = new List<PointData>();
        public DefineMaskForm(PwrSOAPrsnt pslp)
        {
            InitializeComponent();
            //FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            SOAPresenter = pslp;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            Init();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PwrSOAForm)));
            };
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, ILanguage e) => InitControlLang();



        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_COMPOSITED
                cp.ExStyle |= 0x02000000;
                //Turn off ALT+F4
                cp.ClassStyle |= 0x200;
                return cp;
            }
        }

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        private PwrSOAPrsnt SOAPresenter { get; set; }

        public override void Refresh()
        {
            base.Refresh();
            //UpdateView();
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        //}

        private void InitControlLang()
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DefineMask");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DefineMask");
            BtnAdd.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnAddText");
            BtnDelete.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnDeleteText");
            BtnSave.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnSaveText");

        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            InitControlLang();
            //UpdateView();
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {

            }
        }

        private void Init()
        {
            //string folderPath = AppDomain.CurrentDomain.BaseDirectory;

            //// 构建CSV文件的完整路径
            //string csvFilePath = Path.Combine(folderPath, "SOA_Mask_Linear.csv");

            //// 检查文件是否存在
            //if (File.Exists(csvFilePath))
            //{
            //    _PointData = LoadData(csvFilePath);
            //}
            _PointData = GetMaskData();
            BindingData();
        }

        private void GetDisPlayData()
        {
            _DisPlayData.Clear();
            foreach (var item in _PointData)
            {
                var data = new DisPlayPointData(Quantity.ConverByQuintWithUnit(item.X, QuantityUnit.Voltage, 3), Quantity.ConverByQuintWithUnit(item.Y, QuantityUnit.Ampere, 3));
                _DisPlayData.Add(data);
            }
        }

        private List<PointData> LoadData(string filePath)
        {
            List<PointData> csvData = new List<PointData>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] fields = line.Split(',');

                        //(double x, double y) rowData = new(fields[0],fields[1], fields[2]);
                        csvData.Add(new() {/* Point = int.Parse(fields[0]), */X = double.Parse(fields[1]), Y = double.Parse(fields[2]) });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return csvData;
        }

        public void BindingData()
        {

            GetDisPlayData();

            // 初始化DataGridView
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = null;

            // 绑定数据
            dataGridView1.DataSource = _DisPlayData;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.Columns[0].HeaderText = "X(V)";
            dataGridView1.Columns[1].HeaderText = "Y(A)";
            VScrollBar vscrollbar = dataGridView1.Controls[0] as VScrollBar;
            HScrollBar hscrollbar = dataGridView1.Controls[1] as HScrollBar;

            if (vscrollbar != null)
            {
                // 设置垂直滚动条的背景颜色为红色（示例）
                vscrollbar.BackColor = Color.Gray;
            }

            if (hscrollbar != null)
            {
                // 设置水平滚动条的背景颜色为蓝色（示例）
                hscrollbar.BackColor = Color.Gray;
            }

            dataGridView1.Refresh();
        }
        public List<PointData> GetMaskData()
        {
            var data = new List<PointData>();
            foreach (var item in SOAPresenter.MaskData)
            {
                data.Add(new() { X = item.x, Y = item.y });
            }

            return data;
        }
        public List<(Double x, Double y)> ConvertMaskData()
        {
            var data = new List<(Double x, Double y)>();
            foreach (var item in _PointData)
            {
                data.Add((item.X, item.Y));
            }

            return data;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //_PointData = GetMaskData();
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            string unit = QuantityUnit.Voltage.ToUnitString();
            Double max = SOAPresenter != null ? SOAPresenter.MaxLinX : 100;
            Double min = SOAPresenter != null ? SOAPresenter.MinLinX : -100;

            if (e.ColumnIndex == 1)
            {
                unit = QuantityUnit.Ampere.ToUnitString();
                max = SOAPresenter != null ? SOAPresenter.MaxLinY : 100;
                min = SOAPresenter != null ? SOAPresenter.MinLinY : -100;
            }
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            Action<Double> onokclickeventaction = new Action<Double>((data) => {
                _PointData[e.RowIndex][e.ColumnIndex] = Quantity.ConverByQuint(data);
            });

            nkf.SetKeyBoardValue("", unit, 6, onokclickeventaction,
                _PointData[e.RowIndex][e.ColumnIndex], max, min);

            nkf.ShowDialogByPosition();

            BindingData();
        }

        private void DefineMaskForm_SettingClick(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            control.Size = _IndependentSize;
            TableForm = null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                _PointData.Clear();
                _PointData.Add(new() { X = 0, Y = 0 });
            }
            else
            {
                _PointData.Insert(dataGridView1.CurrentRow.Index + 1, new() { X = _PointData[dataGridView1.CurrentRow.Index].X, Y = _PointData[dataGridView1.CurrentRow.Index].Y });
            }
            BindingData();

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                _PointData.RemoveAt(dataGridView1.CurrentRow.Index);
                BindingData();
            }

        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            #region 注释的代码
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.FileName = "SOA_Mask_Linear.csv";
            //// 设置保存对话框的默认文件类型和过滤器
            //saveFileDialog.Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*";
            //saveFileDialog.DefaultExt = "csv";

            //// 弹出保存对话框
            //DialogResult result = saveFileDialog.ShowDialog();

            //if (result == DialogResult.OK)
            //{
            //    try
            //    {
            //        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
            //        {
            //            var data = SOAPresenter.MaskData;
            //            writer.WriteLine("Point,X(V),Y(A)");
            //            for (int i = 0; i < data.Count; i++)
            //            {
            //                writer.WriteLine((i + 1).ToString() + "," + data[i].x.ToString() + "," + data[i].y.ToString());
            //            }
            //        }
            //    }
            //    catch { }
            //}
            #endregion
            var maskdatalist = ConvertMaskData();
            List<PointF> trianglespointlist = new List<PointF>();
            if (SOAPointsChecked.Checked(maskdatalist, trianglespointlist) && PointInPolygonChecked(maskdatalist, trianglespointlist))
            {
                SOAPresenter.MaskData = maskdatalist;// ConvertMaskData();
                SOAPresenter.Reset();
                SaveMask(maskdatalist);
                WeakTip.Default.Write("SOA", MsgTipId.SavingSuccess, emergent: false, "", 5);
            }
            else
            {
                WeakTip.Default.Write("SOA", MsgTipId.SOASaveError, emergent: false, "", 5);
            }

        }
        private Boolean PointInPolygonChecked(List<(Double x, Double y)> maskdata, List<PointF> trianglesPointList)
        {
            if (trianglesPointList.Count > 0)
            {
                var graphicspath = new System.Drawing.Drawing2D.GraphicsPath();
                graphicspath.Reset();
                graphicspath.AddPolygon(trianglesPointList.ToArray());
                var region = new Region();
                region.MakeEmpty();
                region.Union(graphicspath);

                foreach (var point in maskdata)
                {
                    if (region.IsVisible(new PointF((Single)point.x, (Single)point.y)) == false && trianglesPointList.Contains(new PointF((Single)point.x, (Single)point.y)) == false)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public void SaveMask(List<(double x, double y)> pointList)
        {
            string csvFilePath = Path.Combine(Constants.SET_DEF_PATH, "SOA_Mask_Linear.csv");
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                writer.WriteLine("Point,X(V),Y(A)");
                for (int i = 0; i < pointList.Count; i++)
                {
                    writer.WriteLine((i + 1).ToString() + "," + pointList[i].x.ToString() + "," + pointList[i].y.ToString());
                }
            }
        }
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }

    public class PointData
    {
        //public int Point { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    //_ => Point,
                };
            }
            set
            {
                switch (index)
                {
                    default:
                        //Point = (int)value;
                        break;
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;

                };
            }
        }
    }

    public class DisPlayPointData
    {
        //public int Point { get; set; }
        public String X { get; set; }
        public String Y { get; set; }

        public DisPlayPointData(String _X, String _Y)
        {
            X = _X;
            Y = _Y;
        }
        public String this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    //_ => Point,
                };
            }
            set
            {
                switch (index)
                {
                    default:
                        //Point = (int)value;
                        break;
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;

                };
            }
        }
    }

}
