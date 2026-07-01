using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.UserControls;


namespace ScopeX.U2
{
    public partial class PwrPSRRPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        private AmplitudeSettingForm _AmplitudeSettingForm;
        public PwrPSRRPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer| ControlStyles.AllPaintingInWmPaint| ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
            InitHotKnob();
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new SelectComboBox.ComboBoxItem(x.GetDescription(), x, null)).ToList();
            CbxInputVolSrc.DataSource = dss;
            //selectTouch1.SelectValue = PowerPresenter.VoltageSrc;
            //selectTouch1.Text = PowerPresenter.VoltageSrc.ToString();

            CbxInputVolSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.VoltageSrc1 = (ChannelId)CbxInputVolSrc.SelectValue;
                }
            };

            CbxInputCurSrc.DataSource = dss;
            //selectTouch2.SelectValue = PowerPresenter.CurrentSrc;
            //selectTouch2.Text = PowerPresenter.CurrentSrc.ToString();

            CbxInputCurSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.CurrentSrc1 = (ChannelId)CbxInputCurSrc.SelectValue;
                }
            };
            var asource = Enum.GetValues<AWGId>().Where(id => (Int32)id < ChannelIdExt.MaxAwgId - ChannelIdExt.MinAwgId + 1);
            var awgSrc = asource.
                Select(x => new KeyValuePair<String, object>(x.GetDescription(), x)).ToList();
            CbxAWGSrc.DataSource = awgSrc;

            CbxAWGSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    //PowerPresenter.CurrentSrc = (ChannelId)CbxCurrentSrc.SelectedIndex;
                    Presenter.AWGSource = (AWGId)CbxAWGSrc.SelectIndex;
                }
            };
        }
        private void InitHotKnob()
        {
            ControlsHotKnob.Default.InitHotKnob(TbxStartFreq);
            TbxStartFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxStartFreq, nameof(Presenter.StartFreq),Quantity.ConvertByPrefix(20,Prefix.Empty));
            };

            ControlsHotKnob.Default.InitHotKnob(TbxEndFreq);
            TbxEndFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxEndFreq, nameof(Presenter.EndFreq),Quantity.ConvertByPrefix(20, Prefix.Empty));
            };

            ControlsHotKnob.Default.InitHotKnob(TbxScanNumber);
            TbxScanNumber.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxScanNumber, nameof(Presenter.ScanNum));
            };

            ControlsHotKnob.Default.InitHotKnob(TbxAmplitudeSetting);
            TbxAmplitudeSetting.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxAmplitudeSetting, nameof(Presenter.Amplitude),Quantity.ConvertByPrefix(20, Prefix.Empty));
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
        public PwrPSRRPrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrPSRRPrsnt)value;
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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.PSRR;


        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(PowerPresenter.Active):
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(PowerPresenter.VoltageSrc1):
                    //CbxVoltageSrc.SelectedIndex = (Int32)Presenter.InputSource;
                    CbxInputVolSrc.SelectValue = PowerPresenter.VoltageSrc1;
                    break;
                case nameof(PowerPresenter.CurrentSrc1):
                    //CbxCurrentSrc.SelectedIndex = (Int32)Presenter.OutputSource;
                    CbxInputCurSrc.SelectValue = PowerPresenter.CurrentSrc1;
                    break;
                case nameof(Presenter.AWGSource):
                    CbxAWGSrc.SelectIndex = (Int32)Presenter.AWGSource;
                    break;
                case nameof(Presenter.Scan):
                    //RdoScanMode.ChoosedButtonIndex = (Int32)Presenter.Scan;
                    break;
                case nameof(Presenter.CheckTriggerStatus):
                    var value = Presenter.CheckTriggerStatus ? 1 : 0;
                    if (RdoCheckTriggerStatus.ChoosedButtonIndex != value)
                    {
                        RdoCheckTriggerStatus.ChoosedButtonIndex = value;
                    }
                    
                    break;
                case nameof(Presenter.Impedance):
                    RdoImpedance.ChoosedButtonIndex = (Int32)(Presenter.Impedance);
                    break;
                case nameof(Presenter.StartFreq):
                    TbxStartFreq.Text = ValueToString(Presenter.StartFreq, Prefix.Empty, QuantityUnit.Hertz);
                    break;
                case nameof(Presenter.EndFreq):
                    TbxEndFreq.Text = ValueToString(Presenter.EndFreq, Prefix.Empty, QuantityUnit.Hertz);
                    break;
                case nameof(Presenter.ScanNum):
                    TbxScanNumber.Text = Presenter.ScanNum.ToString();
                    break;
                case nameof(Presenter.AmplitudeMode):
                    RdoAmplitudeMode.ChoosedButtonIndex = (Int32)Presenter.AmplitudeMode;
                    if (Presenter.AmplitudeMode == AmplitudeMode.Constant)
                    {
                        TbxAmplitudeSetting.Visible = true;
                        BtnAmplitudeSetting.Visible = false;
                    }
                    else
                    {
                        TbxAmplitudeSetting.Visible = false;
                        BtnAmplitudeSetting.Visible = true;
                    }
                    break;
                case nameof(Presenter.Amplitude):
                    TbxAmplitudeSetting.Text = ValueToString(Presenter.Amplitude, Prefix.Milli, QuantityUnit.Voltage);
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = PowerPresenter.Active;
                //CbxVoltageSrc.SelectedIndex = (Int32)Presenter.InputSource;
                CbxInputVolSrc.SelectValue = PowerPresenter.VoltageSrc1;
                //CbxCurrentSrc.SelectedIndex = (Int32)Presenter.OutputSource;
                CbxInputCurSrc.SelectValue = PowerPresenter.CurrentSrc1;
                CbxAWGSrc.SelectIndex = (Int32)Presenter.AWGSource;
                //RdoScanMode.ChoosedButtonIndex = (Int32)Presenter.Scan;
                RdoCheckTriggerStatus.ChoosedButtonIndex = Presenter.CheckTriggerStatus ? 1 : 0;
                RdoImpedance.ChoosedButtonIndex = (Int32)(Presenter.Impedance);
                TbxStartFreq.Text = ValueToString(Presenter.StartFreq, Prefix.Empty, QuantityUnit.Hertz);
                TbxEndFreq.Text = ValueToString(Presenter.EndFreq, Prefix.Empty, QuantityUnit.Hertz);
                TbxScanNumber.Text = Presenter.ScanNum.ToString();
                RdoAmplitudeMode.ChoosedButtonIndex = (Int32)Presenter.AmplitudeMode;
                if (Presenter.AmplitudeMode == AmplitudeMode.Constant)
                {
                    TbxAmplitudeSetting.Visible = true;
                    BtnAmplitudeSetting.Visible = false;
                }
                else
                {
                    TbxAmplitudeSetting.Visible = false;
                    BtnAmplitudeSetting.Visible = true;
                }
                TbxAmplitudeSetting.Text = ValueToString(Presenter.Amplitude, Prefix.Milli, QuantityUnit.Voltage);
                _ArgToCtrl = false;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            Presenter.VuPsrrRun = PsrrRun;
            Presenter.VuTryAddPsrrBode = TryAddBode;
            UpdateView();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
        private String ValueToString(Double value, Prefix prefix, QuantityUnit unit)
        {
            return new Quantity(value, prefix, unit).ToString("##0.###", true);
        }
        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowPSRRGuideForm();
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
        private void BtnBodePic_Click(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(PowerPresenter);
        }

        private void BtnAmplitudeSetting_Click(object sender, EventArgs e)
        {
            _AmplitudeSettingForm = new AmplitudeSettingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                Presenter = this.Presenter,
                ActiveBorderColor = AppStyleConfig.DefaultFormActiveBorderColor,
                ActiveBorderVisiable = true,
            };
            Presenter.TryAddView(_AmplitudeSettingForm);
            _AmplitudeSettingForm.FormClosed += _AmplitudeSettingForm_FormClosed;
            (ParentForm as PwrPSRRForm).CanClose = false;
            _AmplitudeSettingForm.ShowDialogByPosition();
            (ParentForm as PwrPSRRForm).CanClose = true;
        }

        private void _AmplitudeSettingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(_AmplitudeSettingForm);
        }

        private void TbxStartFreq_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxStartFreq);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.StartFreq = Convert.ToInt64(data));

            nkf.SetKeyBoardValue(LblStartFreq.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                Presenter.StartFreq, Presenter.MaxFreq, Presenter.MinFreq);

            nkf.ShowDialogByPosition();
        }

        private void TbxEndFreq_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxEndFreq);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.EndFreq = Convert.ToInt64(data));

            nkf.SetKeyBoardValue(LblEndFreq.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                Presenter.EndFreq, Presenter.MaxFreq, Presenter.MinFreq);

            nkf.ShowDialogByPosition();
        }

        private void TbxScanNumber_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxScanNumber);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.ScanNum = Convert.ToInt32(data));

            nkf.SetKeyBoardValue(LblScanNumber.Text, "", 3, onokclickeventaction,
                Presenter.ScanNum, Presenter.MaxScanNum, Presenter.MinScanNum);

            nkf.ShowDialogByPosition();
        }

        private void TbxAmplitudeSetting_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeSetting);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.Amplitude = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

            nkf.SetKeyBoardValue(LblAmplitudeSetting.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
                Quantity.ConvertByPrefix(Presenter.Amplitude, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
                        Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

            nkf.ShowDialogByPosition();
        }

        private void RdoScanMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //Presenter.Scan = (ScanMode)RdoScanMode.ChoosedButtonIndex;
            }
        }


        private void RdoCheckTriggerStatus_IndexChanged(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                var value = RdoCheckTriggerStatus.ChoosedButtonIndex != 0;
                if (Presenter.CheckTriggerStatus != value)
                {
                    Presenter.CheckTriggerStatus = value;
                }
            }
        }


        private void RdoImpedance_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Impedance = (ImpedanceType)RdoImpedance.ChoosedButtonIndex;
            }
        }

        private void RdoAmplitudeMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AmplitudeMode = (AmplitudeMode)RdoAmplitudeMode.ChoosedButtonIndex;
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(PowerPresenter);
            //PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
            if(!Presenter.RunFlag)
            {
                Presenter.RunFlag = true;
                ParentForm?.Close();
            }
        }
        private void TryAddBode()
        {
            (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(PowerPresenter);
        }

        private void PsrrRun()
        {
            (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(PowerPresenter);
            //PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
            if (!Presenter.RunFlag)
            {
                Presenter.RunFlag = true;
                ParentForm?.Close();
            }
        }
    }
}
