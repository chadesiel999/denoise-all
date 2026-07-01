using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using Veldrid.Common.Tools;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class PwrSOAPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrSOAPage()
        {
            InitializeComponent();
            InitControlLang();
            InitComboxList();
            InitHotKnob();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, ILanguage e) => InitControlLang();

        private void InitControlLang()
        {
            BtnEditMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnEditMaskText");
            BtnLoadMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnLoadMaskText");
            BtnSaveMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnSaveMaskText");
            BtnSOA.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BtnSOAText");
        }
        private void InitComboxList()
        {

            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxVoltageSrc.DataSource = dss;
            //selectTouch1.SelectValue = PowerPresenter.VoltageSrc;
            //selectTouch1.Text = PowerPresenter.VoltageSrc.ToString();

            CbxVoltageSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.VoltageSrc1 = (ChannelId)CbxVoltageSrc.SelectValue;
                }
            };

            CbxCurrentSrc.DataSource = dss;
            //selectTouch2.SelectValue = PowerPresenter.CurrentSrc;
            //selectTouch2.Text = PowerPresenter.CurrentSrc.ToString();

            CbxCurrentSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
                }
            };
        }

        private void InitHotKnob()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnAxisMaxX);
            BtnAxisMaxX.Click += (_, _) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMaxX, nameof(Presenter.MaxLinX), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
                else
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMaxX, nameof(Presenter.MaxLogX), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
            };
            ControlsHotKnob.Default.InitHotKnob(BtnAxisMinX);
            BtnAxisMinX.Click += (_, _) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMinX, nameof(Presenter.MinLinX), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
                else
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMinX, nameof(Presenter.MinLogX), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
            };
            ControlsHotKnob.Default.InitHotKnob(BtnAxisMaxY);
            BtnAxisMaxY.Click += (_, _) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMaxY, nameof(Presenter.MaxLinY), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
                else
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMaxY, nameof(Presenter.MaxLogY), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
            };
            ControlsHotKnob.Default.InitHotKnob(BtnAxisMinY);
            BtnAxisMinY.Click += (_, _) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMinY, nameof(Presenter.MinLinY), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
                else
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAxisMinY, nameof(Presenter.MinLogY), Quantity.ConvertByPrefix(1, Prefix.Empty));
                }
            };

        }
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }
        public PwrSOAPrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrSOAPrsnt)value;
            }
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Harmonic;


        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.MaxPower):
                    BtnMaxPower.Text = PowerToString();
                    break;
                case nameof(Presenter.MaxCurrent):
                    BtnMaxCurrent.Text = CurrentToString();
                    break;
                case nameof(Presenter.MaxVoltage):
                    BtnMaxVoltage.Text = VoltageToString();
                    break;
                case nameof(Presenter.StopOnFail):
                    ChkStopOnFail.Checked = Presenter.StopOnFail;
                    break;
                case nameof(PowerPresenter.Active):
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(PowerPresenter.VoltageSrc1):
                    CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                    break;
                case nameof(PowerPresenter.CurrentSrc1):
                    CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                    break;
                case "StopOnFault":
                    ChkStopOnFail.Checked = Presenter.StopOnFail;
                    break;
                default:
                    RdoAxisType.ChoosedButtonIndex = (Int32)Presenter.AxisType;
                    if (Presenter.AxisType == AxisType.Linear)
                    {
                        BtnAxisMaxX.Text = LinAxisMaxXToString();
                        BtnAxisMinX.Text = LinAxisMinXToString();
                        BtnAxisMaxY.Text = LinAxisMaxYToString();
                        BtnAxisMinY.Text = LinAxisMinYToString();
                    }
                    else
                    {
                        BtnAxisMaxX.Text = LogAxisMaxXToString();
                        BtnAxisMinX.Text = LogAxisMinXToString();
                        BtnAxisMaxY.Text = LogAxisMaxYToString();
                        BtnAxisMinY.Text = LogAxisMinYToString();
                    }
                    break;
            }
            _ArgToCtrl = false;
        }
        private String PowerToString()
        {
            return new Quantity(Presenter.MaxPower, Prefix.Empty, "W").ToString("#0.###", true);
        }

        private String CurrentToString()
        {
            return new Quantity(Presenter.MaxCurrent, Prefix.Empty, "A").ToString("#0.###", true);
        }

        private String VoltageToString()
        {
            return new Quantity(Presenter.MaxVoltage, Prefix.Empty, "V").ToString("#0.###", true);
        }

        private String LinAxisMaxXToString()
        {
            return new Quantity(Presenter.MaxLinX, Prefix.Empty, "V").ToString("#0.#####", true);
        }

        private String LinAxisMinXToString()
        {
            return new Quantity(Presenter.MinLinX, Prefix.Empty, "V").ToString("#0.#####", true);
        }

        private String LinAxisMaxYToString()
        {
            return new Quantity(Presenter.MaxLinY, Prefix.Empty, "A").ToString("#0.#####", true);
        }

        private String LinAxisMinYToString()
        {
            return new Quantity(Presenter.MinLinY, Prefix.Empty, "A").ToString("#0.#####", true);
        }

        private String LogAxisMaxXToString()
        {
            return new Quantity(Presenter.MaxLogX, Prefix.Empty, "V").ToString("#0.#####", true);
        }

        private String LogAxisMinXToString()
        {
            return new Quantity(Presenter.MinLogX, Prefix.Empty, "V").ToString("#0.#####", true);
        }

        private String LogAxisMaxYToString()
        {
            return new Quantity(Presenter.MaxLogY, Prefix.Empty, "A").ToString("#0.###", true);
        }

        private String LogAxisMinYToString()
        {
            return new Quantity(Presenter.MinLogY, Prefix.Empty, "A").ToString("#0.###", true);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = PowerPresenter.Active;
                CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;

                BtnMaxPower.Text = PowerToString();
                BtnMaxCurrent.Text = CurrentToString();
                BtnMaxVoltage.Text = VoltageToString();

                if (Presenter.AxisType == AxisType.Linear)
                {
                    BtnAxisMaxX.Text = LinAxisMaxXToString();
                    BtnAxisMinX.Text = LinAxisMinXToString();
                    BtnAxisMaxY.Text = LinAxisMaxYToString();
                    BtnAxisMinY.Text = LinAxisMinYToString();
                }
                else
                {
                    BtnAxisMaxX.Text = LogAxisMaxXToString();
                    BtnAxisMinX.Text = LogAxisMinXToString();
                    BtnAxisMaxY.Text = LogAxisMaxYToString();
                    BtnAxisMinY.Text = LogAxisMinYToString();
                }
                ChkStopOnFail.Checked = Presenter.StopOnFail;
                RdoAxisType.ChoosedButtonIndex = (Int32)Presenter.AxisType;
                _ArgToCtrl = false;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            LoadMaskData();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowSOAGuideForm();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }

        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }
        private void BtnPowerPic_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }


        private void BtnMaxVoltage_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnMaxVoltage);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.MaxVoltage = data);

            nkf.SetKeyBoardValue(LblMaxVoltage.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
                Presenter.MaxVoltage, Presenter.MaxMaxVoltage, Presenter.MinMaxVoltage);

            nkf.ShowDialogByPosition();
        }

        private void RdoAxisType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AxisType = (AxisType)RdoAxisType.ChoosedButtonIndex;
            }
        }

        private void BtnMaxCurrent_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnMaxCurrent);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.MaxCurrent = data);

            nkf.SetKeyBoardValue(LblMaxCurrent.Text, QuantityUnit.Ampere.ToUnitString(), 3, onokclickeventaction,
                Presenter.MaxCurrent, Presenter.MaxMaxCurrent, Presenter.MinMaxCurrent);

            nkf.ShowDialogByPosition();
        }

        private void BtnMaxPower_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnMaxPower);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.MaxPower = data);

            nkf.SetKeyBoardValue(LblMaxPower.Text, QuantityUnit.Watt.ToUnitString(), 3, onokclickeventaction,
                Presenter.MaxPower, Presenter.MaxMaxPower, Presenter.MinMaxPower);

            nkf.ShowDialogByPosition();
        }

        private void ChkStopOnFail_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.StopOnFail = ChkStopOnFail.Checked;
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            Presenter.Reset();
        }

        private void BtnAxisMaxX_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnAxisMaxX);
            Action<Double> onokclickeventaction = new Action<Double>((data) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    Presenter.MaxLinX = data;
                }
                else
                {
                    Presenter.MaxLogX = data;
                }
            });

            if (Presenter.AxisType == AxisType.Linear)
            {
                nkf.SetKeyBoardValue(LblAxisMaxX.Text, QuantityUnit.Voltage.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MaxLinX, Presenter.MaxMaxLinX, Presenter.MinMaxLinX);
            }
            else
            {
                nkf.SetKeyBoardValue(LblAxisMaxX.Text, QuantityUnit.Voltage.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MaxLogX, Presenter.MaxMaxLogX, Presenter.MinMaxLogX);
            }

            nkf.ShowDialogByPosition();
        }

        private void BtnAxisMinX_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnAxisMinX);
            Action<Double> onokclickeventaction = new Action<Double>((data) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    Presenter.MinLinX = data;
                }
                else
                {
                    Presenter.MinLogX = data;
                }
            });

            if (Presenter.AxisType == AxisType.Linear)
            {
                nkf.SetKeyBoardValue(LblAxisMinX.Text, QuantityUnit.Voltage.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MinLinX, Presenter.MaxMinLinX, Presenter.MinMinLinX);
            }
            else
            {
                nkf.SetKeyBoardValue(LblAxisMinX.Text, QuantityUnit.Voltage.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MinLogX, Presenter.MaxMinLogX, Presenter.MinMinLogX);
            }

            nkf.ShowDialogByPosition();
        }

        private void BtnAxisMaxY_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnAxisMaxY);
            Action<Double> onokclickeventaction = new Action<Double>((data) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    Presenter.MaxLinY = data;
                }
                else
                {
                    Presenter.MaxLogY = data;
                }
            });

            if (Presenter.AxisType == AxisType.Linear)
            {
                nkf.SetKeyBoardValue(LblAxisMaxY.Text, QuantityUnit.Ampere.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MaxLinY, Presenter.MaxMaxLinY, Presenter.MinMaxLinY);
            }
            else
            {
                nkf.SetKeyBoardValue(LblAxisMaxY.Text, QuantityUnit.Ampere.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MaxLogY, Presenter.MaxMaxLogY, Presenter.MinMaxLogY);
            }

            nkf.ShowDialogByPosition();
        }

        private void BtnAxisMinY_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnAxisMinY);
            Action<Double> onokclickeventaction = new Action<Double>((data) =>
            {
                if (Presenter.AxisType == AxisType.Linear)
                {
                    Presenter.MinLinY = data;
                }
                else
                {
                    Presenter.MinLogY = data;
                }
            });

            if (Presenter.AxisType == AxisType.Linear)
            {
                nkf.SetKeyBoardValue(LblAxisMinY.Text, QuantityUnit.Ampere.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MinLinY, Presenter.MaxMinLinY, Presenter.MinMinLinY);
            }
            else
            {
                nkf.SetKeyBoardValue(LblAxisMinY.Text, QuantityUnit.Ampere.ToUnitString(), 5, onokclickeventaction,
                    Presenter.MinLogY, Presenter.MaxMinLogY, Presenter.MinMinLogY);
            }

            nkf.ShowDialogByPosition();
        }

        private void BtnSOA_Click(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).TryAddSOAUI(PowerPresenter);
        }
        private void LoadMaskData()
        {
            if (Presenter.MaskDataChanged)
                return;
            String csvfilepath = Path.Combine(Constants.SET_DEF_PATH, "SOA_Mask_Linear.csv");
            if (!System.IO.File.Exists(csvfilepath))
            {
                WriteToCsv(csvfilepath);
            }
            // 检查文件是否存在
            if (System.IO.File.Exists(csvfilepath))
            {
                var maskdata = LoadData(csvfilepath);
                List<PointF> trianglespointlist = new List<PointF>();
                if (maskdata != null && SOAPointsChecked.Checked(maskdata, trianglespointlist) && PointInPolygonChecked(maskdata, trianglespointlist))
                {
                    Presenter.MaskData = maskdata;
                }
                else
                {
                    maskdata = new List<(Double x, Double y)>()
                    {
                        new (-4, 5.8), new ( -1, 5.6), new (0, 5.5), new ( 1, 5.5),new ( 2, 5.4),
                        new ( 3, 4.3), new ( 4, 3), new ( 5, 2), new (6, 1), new (  7, 0),
                        new( 8, 0), new (  8, 6),new (-4, 6),
                    };
                    Presenter.MaskData = maskdata;
                }

            }
        }
        private List<(double x, double y)> LoadData(string filePath)
        {
            try
            {
                List<(double x, double y)> csvData = new List<(double x, double y)>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] fields = line.Split(',');

                        (double x, double y) rowData = new(double.Parse(fields[1]), double.Parse(fields[2]));
                        csvData.Add(rowData);
                    }
                }
                return csvData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void WriteToCsv(string filePath)
        {
            #region 初始数据
            string[] data =
            {
                "Point, X(V), Y(A)",
                "1, -4, 5.8",
                "2, -1, 5.6",
                "3, 0, 5.5",
                "4, 1, 5.5",
                "5, 2, 5.4",
                "6, 3, 4.3",
                "7, 4, 3",
                "8, 5, 2",
                "9, 6, 1",
                "10, 7, 0",
                "11, 8, 0",
                "12, 8, 6",
                "13, -4, 6"
            };
            #endregion
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // 逐行写入数据
                foreach (string line in data)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private void BtnSaveMask_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "SOA_Mask_Linear.csv";
            // 设置保存对话框的默认文件类型和过滤器
            saveFileDialog.Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*";
            saveFileDialog.DefaultExt = "csv";

            // 弹出保存对话框
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        var data = Presenter.MaskData;
                        writer.WriteLine("Point,X(V),Y(A)");
                        for (int i = 0; i < data.Count; i++)
                        {
                            writer.WriteLine((i + 1).ToString() + "," + data[i].x.ToString() + "," + data[i].y.ToString());
                        }
                    }
                    WeakTip.Default.Write("SOA", MsgTipId.SavingSuccess, emergent: false, "", 3);
                }
                catch
                {
                    WeakTip.Default.Write("SOA", MsgTipId.SOASaveError, emergent: false, "", 3);
                }
            }
        }

        private void BtnLoadMask_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Constants.SET_DEF_PATH;
                openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 获取选择的文件路径
                    var maskdata = LoadData(openFileDialog.FileName);
                    List<PointF> trianglespointlist = new List<PointF>();
                    if (maskdata != null && SOAPointsChecked.Checked(maskdata, trianglespointlist) && PointInPolygonChecked(maskdata, trianglespointlist))
                    {
                        Presenter.MaskData = maskdata;
                        WeakTip.Default.Write("SOA", MsgTipId.SOALoadMaskSuccess, emergent: false, "", 3);
                    }
                    else
                    {
                        WeakTip.Default.Write("SOA", MsgTipId.SOALoadMaskError, emergent: false, "", 3);
                        return;
                    }
                }
            }
            Presenter.Reset();
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

        private void BtnEditMask_Click(object sender, EventArgs e)
        {
            DefineMaskForm defineMaskForm = new DefineMaskForm(Presenter)
            {
                StartPosition = FormStartPosition.CenterScreen,
            };
            defineMaskForm.ShowDialog();
        }
    }
}
