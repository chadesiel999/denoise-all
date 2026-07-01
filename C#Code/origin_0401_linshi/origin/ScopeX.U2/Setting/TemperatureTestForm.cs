using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class TemperatureTestForm : FloatForm
    {
        #region 温度 风扇

        private Object _Locker = new();

        private Double ChnlTargetTemperature { get; set; } = 50D;

        private Double HardTargetTemperature { get; set; } = 50D;

        #endregion

        #region Render

        private volatile Boolean _IsRunning = true;

        private Thread _RenderThread = null;
        private Int32 RenderInterval { get; set; } = 2_000;

        /// <summary>
        /// 通道目标温度曲线
        /// </summary>
        private ScatterPlot _ChnlTargetTempPlot;

        /// <summary>
        /// 赢哦温度目前曲线
        /// </summary>
        private ScatterPlot _HardTargetTempPlot;

        /// <summary>
        /// 转速曲线
        /// </summary>
        private List<(String Description, WavePlot Polt)> _FanSpeedPlots = new List<(String Description, WavePlot Polt)>();
        /// <summary>
        /// 温度曲线
        /// </summary>
        private List<(String Description, WavePlot Polt)> _TemperaturePlots = new List<(String Description, WavePlot Polt)>();

        private List<(String Description, List<Double> Temperatures)> _AllTemperatures = new();

        private List<(String Description, List<Double> FanSpeeds)> _AllFanSpeeds = new();

        private List<ScopeXTextBox> _AllTempControls = new();

        private List<CheckBox> _AllCheckBoxControls = new();


        #endregion

        private DsoPrsnt _Oscilloscope;

        private PIDContoller _ChannelPID = new PIDContoller()
        {
            Kd = 1000
        };
        private PIDContoller _CpuPID = new PIDContoller()
        {
            Kp = 500,
            Ki = 30,
            Kd = 0
        };

        public TemperatureTestForm(DsoPrsnt dso)
        {
            ChnlTargetTemperature = Constants.ANALOGCHANNEL_WORKING_TEMPERATURE;
            HardTargetTemperature = Constants.HARDDISK_WORKING_TEMPERATURE;
            TopMost = true;
            CheckForIllegalCrossThreadCalls = false;
            _Oscilloscope = dso;
            _Oscilloscope.AutoCalibration.UsingUIParam = true;
            InitializeComponent();
            InitFigure();
        }

        private void InitFigure()
        {
            ScottPlotFormControl.Plot.Style(Color.FromArgb(53, 54, 58), Color.Black, Color.Gray, Color.Gray, Color.White, Color.Gray);

            ScottPlotFormControl.Plot.SetAxisLimits(0, 2000, 30, PlatformUIManager.Default.Platform.GetFanControlParams().MaxTemperature, 0);
            ScottPlotFormControl.Plot.AddAxis(ScottPlot.Renderable.Edge.Left, 2);
            ScottPlotFormControl.Plot.SetAxisLimits(0, 2000, 0, 4500, 0, 1);
            ScottPlotFormControl.Plot.ResetChannelParameter(0, 10, "", QuantityUnit.Celsius.ToUnitString(), 10, false);
            ScottPlotFormControl.Plot.ResetTimebaseParameter(0, 200, "", QuantityUnit.Count.ToUnitString(), 200, false);

            ScottPlotFormControl.Plot.SetAxisLimits(0, 2000, 0, PlatformUIManager.Default.Platform.GetFanControlParams().MaxFanSpeed, 0, 1);
            ScottPlotFormControl.Plot.YAxis2.Dims.Update(0, PlatformUIManager.Default.Platform.GetFanControlParams().Scale, "", "r", 1000, false);

            ScottPlotFormControl.Plot.YAxis2.IsVisible = true;
            //图形颜色设置
            ScottPlotFormControl.Plot.Style(Color.Transparent, Color.Transparent, Color.Gray, Color.Gray, Color.Gray, Color.Gray);
            //网格属性配置
            ScottPlotFormControl.Plot.Grid(true, Color.Gray, LineStyle.Dot);
            ScottPlotFormControl.BackColor = AppStyleConfig.DefaultAreaBackColor;
            ScottPlotFormControl.Plot.GridCrosslineVisible(false);
            ScottPlotFormControl.Dock = DockStyle.Fill;
            //网格刻度设置
            ScottPlotFormControl.Plot.SetTickLabelStyle(Color.White, AppStyleConfig.DefaultContextFont.Name, null, null);
            ScottPlotFormControl.Plot.SetPlotMargin((1, 1, 1, 1));
        }

        private void InitAllPlot()
        {
            //确定有多少根温度曲线
            var alltemps = _Oscilloscope.AutoCalibration.GetAllTemperature();
            Int32 index = 0;
            foreach (var temp in alltemps)
            {
                //创建曲线
                var plot = ScottPlotFormControl.Plot.AddWave(new Double[0, 0], 1, label: temp.Description);
                plot.Color = _Oscilloscope.AutoCalibration.GetColor(index);
                plot.LineStyle = LineStyle.Solid;
                plot.LineWidth = 1;
                plot.Label = temp.Description;
                plot.LabelFont = AppStyleConfig.DefaultMeasureFont;
                plot.ZIndex = index;
                _TemperaturePlots.Add((temp.Description, plot));

                AddControls(temp.Description, plot.Color, index);

                index++;
            }

            //确定有多少根转速曲线曲线
            var allFanSpeeds = _Oscilloscope.AutoCalibration.GetAllFanSpeed();
            foreach (var speed in allFanSpeeds)
            {
                //创建曲线
                var plot = ScottPlotFormControl.Plot.AddWave(new Double[0, 0], 1, label: speed.Description);
                plot.Color = _Oscilloscope.AutoCalibration.GetColor(index);
                plot.LineStyle = LineStyle.Solid;
                plot.LineWidth = 1;
                plot.Label = speed.Description;
                plot.LabelFont = AppStyleConfig.DefaultMeasureFont;
                plot.ZIndex = index;
                plot.YAxisIndex = 1;
                _FanSpeedPlots.Add((speed.Description, plot));

                AddControls(speed.Description, plot.Color, index);

                index++;
            }

            //创建目标温度曲线
            _ChnlTargetTempPlot = ScottPlotFormControl.Plot.AddLine(0, ChnlTargetTemperature, 2000, ChnlTargetTemperature, _Oscilloscope.AutoCalibration.GetColor(index), 1, LineStyle.Solid);
            _ChnlTargetTempPlot.ZIndex = index;
            BtnTargetTemperature.BackColor = _ChnlTargetTempPlot.Color;

            index++;
            _HardTargetTempPlot = ScottPlotFormControl.Plot.AddLine(0, HardTargetTemperature, 2000, HardTargetTemperature, _Oscilloscope.AutoCalibration.GetColor(index), 1, LineStyle.Solid);
            _HardTargetTempPlot.ZIndex = index;
            BtnHardDiskTargetTemperature.BackColor = _HardTargetTempPlot.Color;
        }

        private const Int32 INTERVAL = 21;
        private const Int32 WIDTH = 90;
        private const Int32 LABEL_HEIGHT = 22;
        private const Int32 TEXTBOX_HEIGHT = 30;

        private void AddControls(String text, Color backcolor, Int32 index)
        {
            var label = new ScopeXLabel();
            label.BackColor = Color.FromArgb(41, 42, 45);
            label.BorderColor = Color.FromArgb(53, 54, 58);
            label.BorderThickness = 0;
            label.CornerRadius = 0;
            label.Font = new Font("Microsoft Sans Serif", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            label.ForeColor = Color.FromArgb(232, 234, 237);
            label.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            label.Location = new Point(6 + (index > 14 ? index - 15 : index) * (INTERVAL + WIDTH), index > 14 ? 138 : 77);
            label.MultyLineFlag = false;
            label.Name = $"Lbl{text}";
            label.Size = new Size(105, LABEL_HEIGHT);
            label.StyleFlags = StyleFlag.None;
            label.StylizeFlag = true;
            label.TabStop = false;
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Token = null;
            panelHead.Controls.Add(label);

            var textbox = new ScopeXTextBox();
            textbox.AcceptsTab = false;
            textbox.AutoShowKeyBoard = false;
            textbox.AutoSize = false;
            textbox.BackColor = backcolor;
            textbox.BorderColor = Color.FromArgb(53, 54, 58);
            textbox.BorderThickness = 0;
            textbox.CornerRadius = 0;
            textbox.Cursor = Cursors.Hand;
            textbox.Enabled = false;
            textbox.EnbleSelectBorder = true;
            textbox.Font = new Font("Microsoft Sans Serif", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            textbox.ForeColor = Color.Black;
            textbox.Height = 30;
            textbox.HideSelection = true;
            textbox.KeyboardVerify = null;
            textbox.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            textbox.Location = new Point(6 + (index > 14 ? index - 15 : index) * (INTERVAL + WIDTH), index > 14 ? 162 : 101);
            textbox.MaxLength = 32767;
            textbox.Modified = false;
            textbox.MouseEnterState = false;
            textbox.Multiline = false;
            textbox.Name = $"Stb{text}";
            textbox.ProcessCmdKeyFunc = null;
            textbox.ReadOnly = true;
            textbox.SelectedColor = Color.FromArgb(0, 157, 255);
            textbox.SelectedText = "";
            textbox.SelectionLength = 0;
            textbox.SelectionStart = 0;
            textbox.ShortcutsEnabled = true;
            textbox.Size = new Size(WIDTH, TEXTBOX_HEIGHT);
            textbox.StyleFlags = StyleFlag.None;
            textbox.StyleFlags = StyleFlag.None;
            textbox.StylizeFlag = false;
            textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textbox.UseSystemPasswordChar = false;
            textbox.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            textbox.WordWrap = true;
            panelHead.Controls.Add(textbox);
            _AllTempControls.Add(textbox);

            var checkBox = new CheckBox();
            checkBox.Size = new Size(110, 25);
            checkBox.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox.AutoSize = true;
            checkBox.ForeColor = Color.White;
            if (index >= 0 && index < 6)//第一行
            {
                checkBox.Location = new Point(1100 + index * 110, 1);
            }
            else
            {
                checkBox.Location = index >= 6 && index < 12
                    ? new Point(1100 + (index - 6) * 110, 16)
                    : index >= 12 && index < 18
                                    ? new Point(1100 + (index - 12) * 110, 31)
                                    : index >= 18 && index < 24 ? new Point(1100 + (index - 18) * 110, 46) : new Point(1100 + (index - 24) * 110, 61);
            }

            checkBox.Name = $"Chk{text}";
            checkBox.Text = text;
            checkBox.Tag = index;
            checkBox.Checked = true;
            checkBox.CheckedChanged += CheckBox_CheckedChanged;
            panelHead.Controls.Add(checkBox);
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                var index = Int32.Parse(checkbox.Tag.ToString());
                if (index < _TemperaturePlots.Count)
                {
                    _TemperaturePlots[index].Polt.IsVisible = checkbox.Checked;
                }
                else
                {
                    index = index - _TemperaturePlots.Count;
                    _FanSpeedPlots[index].Polt.IsVisible = checkbox.Checked;
                }
            }
        }

        /// <summary>
        /// Plot刷新线程，每隔2s刷新一次
        /// </summary>
        private void StartUpdaterThread()
        {
            Task.Run(() =>
            {
                while (_IsRunning)
                {
                    UpdateView();
                    UpdatePlot();
                    Thread.Sleep(2_000);
                }
            });
        }

        /// <summary>
        /// PIDcontroller线程 每隔5s刷新一次
        /// </summary>
        private void StartPIDControllerThread()
        {
            Task.Run(() =>
            {
                while (_IsRunning)
                {
                    #region Get Temperature

                    #endregion
                    //var temperature = _Oscilloscope.AutoCalibration.GetAllTemperature().Where(ch => ch.Description.Contains("CH")).Select(ch => ch
                    //.Temperature).Average();
                    //var speed = _ChannelPID.Caculate(temperature, ChnlTargetTemperature, MinFanSpeed, MaxFanSpeed, IntegralMinValue, IntegralMaxValue);
                    //_Oscilloscope.AutoCalibration.ChannelFanSpeed = (Int32)speed;

                    ////Cup最低转速1000r
                    //var cpuspeed = _CpuPID.Caculate(_Oscilloscope.AutoCalibration.CtrollerPcieFanSpeedTempertuare, HardTargetTemperature, MinFanSpeed, MaxFanSpeed, IntegralMinValue, IntegralMaxValue, 4000);
                    ////var cpuspeed = _CpuPID.Caculate(AdcTempByCelsius, HardTargetTemperature, MinFanSpeed, MaxFanSpeed, IntegralMinValue, IntegralMaxValue, 1500);
                    //_Oscilloscope.AutoCalibration.CpuFanSpeed = (Int32)cpuspeed;

                    //_Oscilloscope.AutoCalibration.PcieFanSpeed = (Int32)(cpuspeed);

                    lock (_Locker)
                    {
                        UpdateData();
                    }
                    Thread.Sleep(5_000);//每隔5秒读一次
                }
            });
        }

        /// <summary>
        /// Plot绘制主线程 2s刷新一次
        /// </summary>
        private void StartReanderThread()
        {
            if (_RenderThread == null)
            {
                _RenderThread = new Thread(Render)
                {
                    IsBackground = true,
                    Name = "DrawWaveThread",
                    Priority = ThreadPriority.Highest
                };
                _RenderThread.Start();
            }
        }

        private void Render()
        {
            while (_IsRunning)
            {
                DateTime dt = DateTime.Now;
                try
                {
                    ScottPlotFormControl.Render(true, false);
                    Double elapse = DateTime.Now.Subtract(dt).TotalMilliseconds;
                    if (elapse < RenderInterval)
                    {
                        Task.Delay((Int32)(RenderInterval - elapse)).Wait();
                    }
                }
                catch
                {

                }
            }
        }

        private void UpdateView()
        {
            BtnTargetTemperature.Text = GetTemperature(ChnlTargetTemperature);
            BtnRadioParam.Text = _ChannelPID.Kp.ToString();
            BtnIntegralParam.Text = _ChannelPID.Ki.ToString();
            BtnDiffParam.Text = _ChannelPID.Kd.ToString();
            BtnHardDiskTargetTemperature.Text = GetTemperature(HardTargetTemperature);

            var alltemps = _Oscilloscope.AutoCalibration.GetAllTemperature();
            for (int index = 0; index < alltemps.Count; index++)
            {
                _AllTempControls[index].Text = GetTemperature(alltemps[index].Temperature);
            }

            var allspeed = _Oscilloscope.AutoCalibration.GetAllFanSpeed();
            for (int index = alltemps.Count; index < alltemps.Count + allspeed.Count; index++)
            {
                _AllTempControls[index].Text = GetFanSpeed(allspeed[index - alltemps.Count].Speed);
            }
        }

        private void InitAllTemperaturesAndSpeeds()
        {
            var alltemps = _Oscilloscope.AutoCalibration.GetAllTemperature();
            foreach (var temp in alltemps)
            {
                _AllTemperatures.Add((temp.Description, new List<double>()));
            }

            var allspeeds = _Oscilloscope.AutoCalibration.GetAllFanSpeed();
            foreach (var speed in allspeeds)
            {
                _AllFanSpeeds.Add((speed.Description, new List<double>()));
            }
        }

        private void UpdateData()
        {
            var alltemps = _Oscilloscope.AutoCalibration.GetAllTemperature();
            foreach (var temp in alltemps)
            {
                var current = _AllTemperatures.FirstOrDefault(element => element.Description == temp.Description);
                current.Temperatures.Add(temp.Temperature);
                if (current.Temperatures.Count > 2001)
                    current.Temperatures.RemoveAt(0);
            }
            var allspeeds = _Oscilloscope.AutoCalibration.GetAllFanSpeed();
            foreach (var speed in allspeeds)
            {
                var current = _AllFanSpeeds.FirstOrDefault(element => element.Description == speed.Description);
                current.FanSpeeds.Add(speed.Speed);
                if (current.FanSpeeds.Count > 2001)
                    current.FanSpeeds.RemoveAt(0);
            }
        }

        /// <summary>
        /// 配置波形亮度
        /// </summary>
        /// <param name="wfmIntensity"></param>
        private void ConfigWfmIntensity(Int32 wfmIntensity)
        {
            ScottPlotFormControl.Plot.WavePlotStyle(wfmIntensity);

            ScottPlotFormControl.Plot.Grid(color: Color.FromArgb((Int32)(wfmIntensity / 100d * 255), Color.White));
            ScottPlotFormControl.Plot.SetTickLabelColor(Color.FromArgb((Int32)(wfmIntensity / 100d * 255), Color.White));
        }

        protected void UpdatePlot()
        {
            UpdateTargetTempPlot();
            UpdateAllPlot();
        }

        #region Update Temperature And FanSpeed Plot

        private void UpdateTargetTempPlot()
        {
            if (_ChnlTargetTempPlot == null || _HardTargetTempPlot == null)
            {
                return;
            }
            _ChnlTargetTempPlot.Xs = new Double[] { 0, 2000 };
            _ChnlTargetTempPlot.Ys = new Double[] { ChnlTargetTemperature, ChnlTargetTemperature };

            _HardTargetTempPlot.Xs = new Double[] { 0, 2000 };
            _HardTargetTempPlot.Ys = new Double[] { HardTargetTemperature, HardTargetTemperature };
        }

        private void UpdateAllPlot()
        {
            if (_AllTemperatures[0].Temperatures.Count <= 0)
            {
                return;
            }
            for (int index = 0; index < _TemperaturePlots.Count; index++)
            {
                var plot = _TemperaturePlots[index].Polt;
                lock (plot.YTs.SyncRoot)
                {
                    lock (_Locker)
                    {
                        var buffer = new Double[1, _AllTemperatures[index].Temperatures.Count];
                        var data = _AllTemperatures[index].Temperatures.ToArray();
                        Unsafe.CopyBlock(ref Unsafe.As<Double, byte>(ref buffer[0, 0]), ref Unsafe.As<Double, byte>(ref data[0]), (UInt32)(Unsafe.SizeOf<Double>() * data.Length));
                        plot.YTs = buffer;
                    }
                }
            }

            for (int index = 0; index < _FanSpeedPlots.Count; index++)
            {
                var plot = _FanSpeedPlots[index].Polt;
                lock (plot.YTs.SyncRoot)
                {
                    lock (_Locker)
                    {
                        var buffer = new Double[1, _AllFanSpeeds[index].FanSpeeds.Count];
                        var data = _AllFanSpeeds[index].FanSpeeds.ToArray();
                        Unsafe.CopyBlock(ref Unsafe.As<Double, byte>(ref buffer[0, 0]), ref Unsafe.As<Double, byte>(ref data[0]), (UInt32)(Unsafe.SizeOf<Double>() * data.Length));
                        plot.YTs = buffer;
                    }
                }
            }
        }


        #endregion Update Temperature And FanSpeed Plot


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            ConfigWfmIntensity(100);
            InitAllTemperaturesAndSpeeds();
            InitAllPlot();

            StartPIDControllerThread();
            StartUpdaterThread();
            StartReanderThread();
            UpdateView();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _IsRunning = false;
            _Oscilloscope.AutoCalibration.UsingUIParam = false;
            if (_RenderThread != null && _RenderThread.IsAlive)
            {
                _RenderThread.Join(50);
                _RenderThread = null;
            }
            ScottPlotFormControl.Plot.Remove(_ChnlTargetTempPlot);
            ScottPlotFormControl.Plot.Remove(_HardTargetTempPlot);

            _TemperaturePlots.ForEach(tp =>
            {
                ScottPlotFormControl.Plot.Remove(tp.Polt);
            });

            _FanSpeedPlots.ForEach(tp =>
            {
                ScottPlotFormControl.Plot.Remove(tp.Polt);
            });

            ScottPlotFormControl.Dispose();
            base.OnFormClosing(e);
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            TitleColor = AppStyleConfig.DefaultTitleForeColor;
            ActiveBorderColor = Color.FromArgb(255, 150, 14, 75);
            ActiveBorderVisiable = true;
            IsIndicatorShow = true;
        }

        private void BtnTargetTemperature_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnTargetTemperature);
            var oncomfirm = new Action<Double>((data) =>
            {
                ChnlTargetTemperature = data;
                lock (_ChnlTargetTempPlot.Ys.SyncRoot)
                {
                    _ChnlTargetTempPlot.Ys = new Double[] { data, data };
                }
            });

            nkf.SetKeyBoardValue(LblChnnlTargetTemp.Text, QuantityUnit.Celsius.ToUnitString(), 2, oncomfirm,
                ChnlTargetTemperature, 80, 30);
            nkf.ShowDialogByPosition();
        }

        private void BtnHardDiskTargetTemperature_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnHardDiskTargetTemperature);
            var oncomfirm = new Action<Double>((data) =>
            {
                HardTargetTemperature = data;
                lock (_HardTargetTempPlot.Ys.SyncRoot)
                {
                    _HardTargetTempPlot.Ys = new Double[] { data, data };
                }
            });

            nkf.SetKeyBoardValue(LblHardDiskTargetTemp.Text, QuantityUnit.Celsius.ToUnitString(), 2, oncomfirm,
                HardTargetTemperature, 80, 30);
            nkf.ShowDialogByPosition();
        }

        private String GetTemperature(Double temperature)
        {
            return new Core.Tools.Quantity(temperature, Core.Tools.Prefix.Empty, QuantityUnit.Celsius).ToString("#0.000", true);
        }

        private String GetFanSpeed(Int32 speed)
        {
            return new Core.Tools.Quantity(speed, Core.Tools.Prefix.Empty, "r").ToString("#0.00", true);
        }

        private void BtnRadioParam_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnRadioParam);
            var oncomfirm = new Action<Double>((data) =>
            {
                _ChannelPID.Kp = data;
            });

            nkf.SetKeyBoardValue(ScopeXLabel1.Text, "", 2, oncomfirm,
                _ChannelPID.Kp, 1_000_000, 0);
            nkf.ShowDialogByPosition();
        }

        private void BtnIntegralParam_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnIntegralParam);
            var oncomfirm = new Action<Double>((data) =>
            {
                _ChannelPID.Ki = data;
            });

            nkf.SetKeyBoardValue(ScopeXLabel2.Text, "", 2, oncomfirm,
                _ChannelPID.Ki, 1_000_000, 0);
            nkf.ShowDialogByPosition();
        }

        private void BtnDiffParam_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnDiffParam);
            var oncomfirm = new Action<Double>((data) =>
            {
                _ChannelPID.Kd = data;
            });

            nkf.SetKeyBoardValue(ScopeXLabel3.Text, "", 2, oncomfirm,
                _ChannelPID.Kd, 1_000_000, 0);
            nkf.ShowDialogByPosition();
        }

        private void BtnFanSpeed_Click(Object sender, EventArgs e)
        {
            //var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //var oncomfirm = new Action<Double>((data) =>
            //{
            //    _Oscilloscope.AutoCalibration.FanSpeed = (Int32)data;
            //});

            //nkf.SetKeyBoardValue(ScopeXLabel5.Text, "", 2, oncomfirm, _Oscilloscope.AutoCalibration.FanSpeed, 4000, 400);
            //nkf.ShowDialogByPosition();
        }

        private void BtnPcieRatio_Click(object sender, EventArgs e)
        {
            //var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //var oncomfirm = new Action<Double>((data) =>
            //{
            //    _Oscilloscope.AutoCalibration.PcieSpeedOffset = (Int32)data;
            //});

            //nkf.SetKeyBoardValue(scopexLabel16.Text, "", 0, oncomfirm, _Oscilloscope.AutoCalibration.PcieSpeedOffset, 0, MaxFanSpeed);
            //nkf.ShowDialogByPosition();
        }

        private void BtnCpuRatio_Click(object sender, EventArgs e)
        {
            //var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //var oncomfirm = new Action<Double>((data) =>
            //{
            //    _Oscilloscope.AutoCalibration.CpuSpeedOffset = (Int32)data;
            //});

            //nkf.SetKeyBoardValue(scopexLabel17.Text, "", 0, oncomfirm, _Oscilloscope.AutoCalibration.CpuSpeedOffset, 0, MaxFanSpeed);
            //nkf.ShowDialogByPosition();
        }
    }

    internal class PIDContoller
    {
        /// <summary>
        /// 比例系数Proportional
        /// </summary>
        public Double Kp { get; set; } = 1000;

        /// <summary>
        /// 积分系数Integral
        /// </summary>
        public Double Ki { get; set; } = 30;

        /// <summary>
        ///微分系数Derivative
        /// </summary>
        public Double Kd { get; set; } = 0;

        private Double P, I, D;

        /// <summary>
        /// 当前误差
        /// </summary>
        private Double Error { get; set; } = 0D;

        /// <summary>
        /// 上一次误差
        /// </summary>
        private Double Lasterror { get; set; } = 0D;

        //internal static PIDContoller Default = new PIDContoller();

        public PIDContoller()
        {
            Kd = Kd == 0 ? 3 * Kp : Kd;
        }

        /// <summary>
        /// 计算函数
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="targetValue">目标值</param>
        /// <param name="targetMinValue">目标最小值</param>
        /// <param name="targetMaxValue">目标最大值</param>
        /// <param name="integralMinValue">积分增量最小值</param>
        /// <param name="integralMaxValue">积分增量最大值</param>
        /// <param name="defaultValue">默认值（目标温度与实际温度一直时）</param>
        /// <returns>期望值</returns>
        public Double Caculate(Double currentValue, Double targetValue, Double targetMinValue, Double targetMaxValue, Double integralMinValue, Double integralMaxValue, Double defaultValue = 0)
        {
            Error = targetValue - currentValue;
            P = Kp * Error;
            //I += Ki * Error;
            if (Error < 0)
            {
                I -= Ki * Math.Sqrt(-Error);
            }
            else
            {
                I += Ki * Math.Sqrt(Error);
            }
            I = Math.Clamp(I, integralMinValue, integralMaxValue);
            D = Kd * (Error - Lasterror);
            Lasterror = Error;
            defaultValue = defaultValue == 0 ? (targetMaxValue + targetMinValue) / 2 : defaultValue;
            var result = defaultValue - P - I - D;
            result = Math.Clamp(result, targetMinValue, targetMaxValue);

            return result;
        }
    }
}
