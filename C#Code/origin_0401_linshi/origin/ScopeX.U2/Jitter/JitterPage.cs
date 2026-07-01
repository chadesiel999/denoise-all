using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using static System.Windows.Forms.AxHost;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Threading;
using static ScopeX.UserControls.SelectComboBox;
using ScopeX.Core.Jitter;
using System.Windows.Forms.VisualStyles;

namespace ScopeX.U2
{
    public partial class JitterPage : UserControl, IJitterView, IStylize
    {
        private Boolean _ArgToCtrl;
        public Boolean NeedPrsnt;

        private Int32 _Distance = 0;
        private IJitterView _OptionPage;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        //图形使能加载慢需要添加防颤功能(5s)
        private System.Timers.Timer _DebounceTimer = new System.Timers.Timer() { Interval = 4500 };
        public JitterPage()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            InitSourceList();
            InitSourceType();
            InitBinNumList();
            InitHotKnobValue();
            //_Distance = ChkGraphEnable.Location.X - CbxBinNum.Location.X;
            _DebounceTimer.Elapsed += DebounceTimer_Tick;
            LblPatternLength.DoubleClick += LblPatternLength_DoubleClick;
        }

        private Int32 needrecorddata = 0;


        private void LblPatternLength_DoubleClick(object sender, EventArgs e)
        {
            needrecorddata++;
            if (needrecorddata == 10)
            {
                Presenter.NeedRecordData = !Presenter.NeedRecordData;
                WeakTip.Default.Write("Jitter", Presenter.NeedRecordData ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKai") : ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), emergent: false, "", 2);
                needrecorddata = 0;
            }
        }

        private void InitSourceList()
        {
            CbxSource.DataSource = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsReference())).OrderBy(x => x).Select(o => new ComboBoxItem(o.ToString(), o, null)).ToList();

            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = (ChannelId)CbxSource.SelectValue;
                }
            };
        }

        private void InitBinNumList()
        {
            CbxBinNum.DataSource = Enum.GetValues<MaxBinNum>().Select(o => new ComboBoxItem(EnumEx.GetDescription(o), o, null)).ToList();
            CbxBinNum.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.CurrentBinNum = (MaxBinNum)CbxBinNum.SelectValue;
                }
            };

        }

        private void InitSourceType()
        {
            CbxSourceType.DataSource = Enum.GetValues<JitterSignalType>().Select(o => new ComboBoxItem(o.GetAliaLangString(), o, null)).ToList();
            CbxSourceType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.SignalType = (JitterSignalType)CbxSourceType.SelectValue;
                    if (Presenter.SignalType == JitterSignalType.Clock)
                    {
                        LblPatternLength.Visible = false;
                        BtnPatternLength.Visible = false;
                    }
                    else
                    {
                        LblPatternLength.Visible = true;
                        BtnPatternLength.Visible = true;
                    }
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
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

        public JitterPrsnt Presenter
        {
            get;//=> (JitterPrsnt)(ParentForm as IJitterView).Presenter;
            set;//=> (ParentForm as IJitterView).Presenter = value;
        }

        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
        }
        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    if (ChkEnable.Checked != Presenter.Active)
                    {
                        ChkEnable.Checked = Presenter.Active;
                    }
                    break;
                case nameof(Presenter.Source):
                    //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                    if (Enum.Parse<ChannelId>(CbxSource.SelectValue.ToString()) != Presenter.Source)
                    {
                        CbxSource.SelectValue = Presenter.Source;
                    }
                    break;
                case nameof(Presenter.Threshold):
                    BtnThreshold.Text = ThresholdToString();
                    break;
                case nameof(Presenter.Hysteresis):
                    BtnHysteresis.Text = HysteresisToString();
                    break;
                case nameof(Presenter.PatternLength):
                    BtnPatternLength.Text = PatternLengthToString();
                    break;
                case nameof(Presenter.SignalType):
                    if (Enum.Parse<JitterSignalType>(CbxSourceType.SelectValue.ToString()) != Presenter.SignalType)
                    {
                        CbxSourceType.SelectValue = Presenter.SignalType;
                        BtnBitRate.Text = BitRateToString();
                    }
                    break;
                case nameof(Presenter.BitRate):
                    BtnBitRate.Text = BitRateToString();
                    break;
                case nameof(Presenter.EnableBitRateSearch):
                    BtnBitRate.Text = BitRateToString();
                    break;
                case nameof(Presenter.ClockType):
                    if (RdoClockType.ChoosedButtonIndex != (Int32)Presenter.ClockType)
                    {
                        RdoClockType.ChoosedButtonIndex = (Int32)Presenter.ClockType;
                        InitOptionPage();
                    }
                    break;
                case nameof(Presenter.CurGraphType):
                    //CbxDemod.SelectedIndex = (Int32)Presenter.CurGraphType;
                    if (Int32.Parse(CbxDemod.SelectValue.ToString()) != (Int32)Presenter.CurGraphType)
                    {
                        CbxDemod.SelectValue = (Int32)Presenter.CurGraphType;
                        ChkGraphEnable.Checked = Presenter.GetCurGraphPrsnt(Presenter.CurGraphType).Enabled;
                    }
                    break;
                case nameof(Presenter.CurrentBinNum):
                    //CbxBinNum.SelectedIndex = (Int32)Presenter.CurrentBinNum;
                    if (Enum.Parse<MaxBinNum>(CbxBinNum.SelectValue.ToString()) != Presenter.CurrentBinNum)
                    {
                        CbxBinNum.SelectValue = Presenter.CurrentBinNum;
                    }
                    break;
                case nameof(Presenter.ThresholdFreq):
                    BtnThresholdFreq.Text = ThresholdFreqToString();
                    break;
                case nameof(Presenter.EyeSaturation):
                    BtnEyeSaturation.Text = EyeSaturationToString();
                    break;
                case nameof(Presenter.EyeEnable):
                    if (ChkEyeEnable.Checked != Presenter.EyeEnable)
                    {
                        ChkEyeEnable.Checked = Presenter.EyeEnable;
                        LblFastEye.Visible = ChkFastEye.Visible = LblEyeParameters.Visible = ChkEyeParameters.Visible = ChkEyeEnable.Checked;
                    }
                    break;
                case nameof(Presenter.FastEye):
                    if (ChkFastEye.Checked != Presenter.FastEye)
                    {
                        ChkFastEye.Checked = Presenter.FastEye;
                    }
                    break;
                case nameof(Presenter.EyeParamEnable):
                    if (ChkEyeParameters.Checked != Presenter.EyeParamEnable)
                    {
                        ChkEyeParameters.Checked = Presenter.EyeParamEnable;
                    }
                    break;
                case nameof(Presenter.JitterParamEnable):
                    if (ChkJitterParam.Checked != Presenter.JitterParamEnable)
                    {
                        ChkJitterParam.Checked = Presenter.JitterParamEnable;
                    }
                    break;
                default:
                    _OptionPage?.UpdateView(prsnt, propertyName);
                    break;
            }
            //SetControlsState(CbxDemod.SelectedItem.ToString() == ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing"));
            SetControlsState();
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                ChkEnable.Checked = Presenter.Active;
                //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                CbxSource.SelectValue = Presenter.Source;
                BtnThreshold.Text = ThresholdToString();
                BtnHysteresis.Text = HysteresisToString();
                BtnPatternLength.Text = PatternLengthToString();
                CbxSourceType.SelectValue = Presenter.SignalType;
                LblPatternLength.Visible = Presenter.SignalType == JitterSignalType.Custom;
                BtnPatternLength.Visible = Presenter.SignalType == JitterSignalType.Custom;

                BtnBitRate.Text = BitRateToString();
                RdoClockType.ChoosedButtonIndex = (Int32)Presenter.ClockType;
                InitOptionPage();
                CbxDemod.SelectValue = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing");
                if (CbxDemod.SelectKey?.ToString() != ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing"))
                {
                    ChkGraphEnable.Checked = Presenter.GetCurGraphPrsnt(Presenter.CurGraphType).Enabled;
                }
                else
                {
                    ChkGraphEnable.Checked = Presenter.IsAllEnableGraph(exclusion: JitterGraphType.QFactor);
                }

                //CbxBinNum.Text = BinNumToString();
                //CbxBinNum.SelectedIndex = (Int32)Presenter.CurrentBinNum;
                CbxBinNum.SelectValue = Presenter.CurrentBinNum;
                BtnThresholdFreq.Text = ThresholdFreqToString();
                BtnEyeSaturation.Text = EyeSaturationToString();
                ChkJitterParam.Checked = Presenter.JitterParamEnable;
                ChkEyeEnable.Checked = Presenter.EyeEnable;
                ChkEyeParameters.Checked = Presenter.EyeParamEnable;
                ChkFastEye.Checked = Presenter.FastEye;
                ChkJitterParam.Enabled = Presenter.Active;
                ChkGraphEnable.Visible = LblGraphEnable.Visible = Presenter.Active;
                LblFastEye.Visible = ChkFastEye.Visible = LblEyeParameters.Visible = ChkEyeParameters.Visible = ChkEyeEnable.Checked;
                SetControlsState();

                _ArgToCtrl = false;
            }
        }

        private Boolean IsAll => CbxDemod.SelectKey?.ToString() == ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing");

        private void SetControlsState()
        {
            ChkEyeParameters.Enabled = Presenter.EyeEnable;
            //LblBinNum.Visible = CbxBinNum.Visible = Presenter.CurGraphType == JitterGraphType.Histogram && !IsAll;
            //LblThrehold.Visible = BtnThresholdFreq.Visible = Presenter.CurGraphType == JitterGraphType.Spectrum && !IsAll;
            if ((Presenter.CurGraphType == JitterGraphType.Histogram || Presenter.CurGraphType == JitterGraphType.Spectrum) && !IsAll)
            {
                if (ChkGraphEnable.Location.X != CbxBinNum.Location.X + _Distance)
                {
                    LblGraphEnable.Location = new Point(LblGraphEnable.Location.X + _Distance, LblGraphEnable.Location.Y);
                    ChkGraphEnable.Location = new Point(ChkGraphEnable.Location.X + _Distance, ChkGraphEnable.Location.Y);
                }
            }
            else
            {
                if (ChkGraphEnable.Location.X != CbxBinNum.Location.X)
                {
                    LblGraphEnable.Location = new Point(LblGraphEnable.Location.X - _Distance, LblGraphEnable.Location.Y);
                    ChkGraphEnable.Location = new Point(ChkGraphEnable.Location.X - _Distance, ChkGraphEnable.Location.Y);
                }

            }
        }

        private void ChkEnable_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = ChkEnable.Checked;
                ChkEnable.Checked = Presenter.Active;

                ChkGraphEnable.Visible = LblGraphEnable.Visible = Presenter.Active;
                ChkJitterParam.Enabled = Presenter.Active;
                BtnBitRate.Text = BitRateToString();
            }
        }

        private void BtnThreshold_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnThreshold);
            var oncomfirm = new Action<Double>((data) => Presenter.Threshold = data);

            nkf.SetKeyBoardValue(LblThreshold.Text, QuantityUnit.Percent.ToUnitString(), 3, oncomfirm,
                Presenter.Threshold, Presenter.MaxThreshold, Presenter.MinThreshold);

            nkf.ShowDialogByPosition();
        }

        private void BtnHysteresis_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnHysteresis);
            var oncomfirm = new Action<Double>((data) => Presenter.Hysteresis = data);

            nkf.SetKeyBoardValue(LblHysteresis.Text, QuantityUnit.Percent.ToUnitString(), 0, oncomfirm,
                Presenter.Hysteresis,
                 Presenter.MaxHysteresis,
                 Presenter.MinHysteresis, false);

            nkf.ShowDialogByPosition();
        }

        private void BtnPatternLength_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnPatternLength);
            var oncomfirm = new Action<Double>((data) => Presenter.PatternLength = (Int32)data);

            nkf.SetKeyBoardValue(LblPatternLength.Text, QuantityUnit.Bit.ToUnitString(), 0, oncomfirm,
                Presenter.PatternLength, Int32.MaxValue, 0, true, true);

            nkf.ShowDialogByPosition();
        }

        private String ThresholdToString() => new Quantity(Presenter.Threshold, Prefix.Empty, QuantityUnit.Percent).ToString("#0.0", true);

        private String HysteresisToString() => new Quantity(Presenter.Hysteresis, Prefix.Empty, QuantityUnit.Percent).ToString("##0.###", true);

        private String PatternLengthToString() => new Quantity(Presenter.PatternLength, Prefix.Empty, QuantityUnit.Bit).ToString("##0.###", true);

        private void BtnBitRate_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnBitRate);
            var oncomfirm = new Action<Double>((data) => Presenter.BitRate = data);

            nkf.SetKeyBoardValue(LblBitRate.Text, QuantityUnit.BitPerSecond.ToUnitString(), 7, oncomfirm,
                Presenter.BitRate,
                Presenter.MaxBitRate,
                Presenter.MinBitRate, true, true);

            nkf.ShowDialogByPosition();
        }

        private String BitRateToString()
        {
            if (Presenter.SignalType == JitterSignalType.Clock /*|| !Presenter.EnableBitRateSearch*/)
            {
                BtnBitRate.Enabled = false;
                return MeasureHelper.MeasureEmpty;
            }
            BtnBitRate.Enabled = true;
            return new Quantity(Presenter.BitRate, Prefix.Empty, QuantityUnit.BitPerSecond).ToString("##0.###", true);
        }

        private void RdoClockType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClockType = (ClockTypeOpt)RdoClockType.ChoosedButtonIndex;
                InitOptionPage();
            }
        }

        private void InitHotKnobValue()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnThreshold);
            BtnThreshold.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnThreshold, nameof(Presenter.Threshold));

            };
            ControlsHotKnob.Default.InitHotKnob(BtnHysteresis);
            BtnHysteresis.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnHysteresis, nameof(Presenter.Hysteresis));
            };
            ControlsHotKnob.Default.InitHotKnob(BtnPatternLength);
            BtnPatternLength.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnPatternLength, nameof(Presenter.PatternLength));
            };
            ControlsHotKnob.Default.InitHotKnob(BtnBitRate);
            BtnBitRate.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnBitRate, nameof(Presenter.BitRate));
            };
            ControlsHotKnob.Default.InitHotKnob(BtnThresholdFreq);
            BtnThresholdFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnThresholdFreq, nameof(Presenter.ThresholdFreq));
            };

        }
        private void InitOptionPage()
        {
            _OptionPage = GetOptionPage(RdoClockType.ChoosedButtonIndex, this);
            if (CrmOptions.Controls.Count > 0)
            {
                ((IJitterView)CrmOptions.Controls[0]).Presenter.TryRemoveView((IJitterView)CrmOptions.Controls[0]);
                CrmOptions.Controls.RemoveAt(0);
            }

            BeginInvoke(() => CrmOptions.Controls.Add(_OptionPage as Control));
        }

        private IJitterView GetOptionPage(Int32 index, Control parentForm)
        {
            Control subpage = index switch
            {
                0 => new ConstantFreqClockPage() { Presenter = Presenter },
                1 => new PllPage() { Presenter = Presenter },
                2 => new ExternalClockPage() { Presenter = Presenter },
                _ => throw new NotImplementedException(),
            };
            ((IJitterView)subpage).Presenter.TryAddView((IJitterView)subpage);
            subpage.Dock = DockStyle.Fill;
            //subpage.TabIndex = 1;
            subpage.BackColor = Color.Transparent;

            if (subpage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
                DefaultStyleManager.Instance.RegisterControlRecursion(subpage, StyleFlag.FontSize);
            }
            return (IJitterView)subpage;
        }



        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                ChkGraphEnable.Enabled = true;
                _DebounceTimer.Stop();
            }
        }

        private void ChkGraphEnable_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                NeedPrsnt = true;
                ChkGraphEnable.Enabled = false;
                this.Enabled = false;
                _DebounceTimer.Start();
                if (CbxDemod.SelectKey?.ToString() == ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing"))
                {
                    SelectGraphAll(ChkGraphEnable.Checked);
                }
                else
                {
                    SelectGraph((JitterGraphType)CbxDemod.SelectValue, ChkGraphEnable.Checked);
                }
                this.Enabled = true;
                NeedPrsnt = false;
            }
        }

        private void UpdateGraphMathState(AdvancedMathPrsnt mathPrsnt)
        {
            if (CbxDemod.SelectValue.ToString() == ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing"))
            {
                ChkGraphEnable.Checked = Presenter.IsAllEnableGraph(JitterGraphType.QFactor);
            }
            else
            {
                ChkGraphEnable.Checked = mathPrsnt.Enabled;
            }
        }

        private void SelectGraphAll(Boolean state)
        {
            var types = Enum.GetValues(typeof(JitterGraphType));


            foreach (JitterGraphType type in types)
            {
                if (type == JitterGraphType.QFactor || type == JitterGraphType.Eye)
                    continue;

                Presenter.CurGraphType = type;
                if (Presenter.GetCurGraphPrsnt(type).Enabled != state)
                {
                    Presenter.SetGraphEnable(type,state);
                    UpdateGraphMathState(Presenter.GetCurGraphPrsnt(Presenter.CurGraphType));
                    Thread.Sleep(100);
                }
            }


        }

        private void SelectGraph(JitterGraphType graphType, Boolean state)
        {
            Presenter.SetGraphEnable(graphType, state);
            UpdateGraphMathState(Presenter.GetCurGraphPrsnt(Presenter.CurGraphType));
        }

        private String BinNumToString() => Presenter.CurrentBinNum.ToString();

        private void BtnThresholdFreq_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnThresholdFreq);
            var oncomfirm = new Action<Double>((data) => Presenter.ThresholdFreq = data);

            nkf.SetKeyBoardValue(LblThrehold.Text, QuantityUnit.Percent.ToUnitString(), 5, oncomfirm,
                Presenter.ThresholdFreq, 100, 20);

            nkf.ShowDialogByPosition();
        }

        private String ThresholdFreqToString() => new Quantity(Presenter.ThresholdFreq, Prefix.Empty, QuantityUnit.Percent).ToString("#0.00", true);

        private void BtnEyeSaturation_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnEyeSaturation);
            var oncomfirm = new Action<Double>((data) => Presenter.EyeSaturation = data);

            nkf.SetKeyBoardValue(LblEyeSaturation.Text, QuantityUnit.Percent.ToUnitString(), 3, oncomfirm,
                Presenter.EyeSaturation, 100, 0.1);

            nkf.ShowDialogByPosition();
        }

        private String EyeSaturationToString() => new Quantity(Presenter.EyeSaturation, Prefix.Empty, QuantityUnit.Percent).ToString("#0.00", true);

        private void ChkEyeEnable_CheckedChangedEvent(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EyeEnable = ChkEyeEnable.Checked;
                LblFastEye.Visible=ChkFastEye.Visible=LblEyeParameters.Visible=ChkEyeParameters.Visible= ChkEyeEnable.Checked;
            }
        }

        private void ChkEyeParameters_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EyeParamEnable = ChkEyeParameters.Checked;
            }
        }
        private void ChkFastEye_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FastEye = ChkFastEye.Checked;                
            }
        }
        private void ChkJitterParam_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.JitterParamEnable = ChkJitterParam.Checked;
            }
        }

        private void BtnFindBitRate_Click(object sender, EventArgs e)
        {
            Presenter.EnableBitRateSearch = true;
        }

        private void CbxDemod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _ArgToCtrl = true;
                if (CbxDemod.SelectKey.ToString() == ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuTuXing"))
                {
                    ChkGraphEnable.Checked = Presenter.IsAllEnableGraph(JitterGraphType.QFactor);
                }
                else
                {
                    Presenter.CurGraphType = (JitterGraphType)CbxDemod.SelectValue;
                    UpdateGraphMathState(Presenter.GetCurGraphPrsnt(Presenter.CurGraphType));
                }
                SetControlsState();
                _ArgToCtrl = false;
            }
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            Presenter?.TryRemoveView(_OptionPage);
            Presenter?.TryRemoveView(this);
        }
    }
}
