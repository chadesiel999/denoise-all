using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.Protocol;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class PwrOnOffTimePage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrOnOffTimePage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxInVoltageSrc.DataSource = dss;

            CbxInVoltageSrc.SelectedIndexChanged -= CbxInVoltageSrc_SelectedIndexChanged;
            CbxInVoltageSrc.SelectedIndexChanged += CbxInVoltageSrc_SelectedIndexChanged;

            CbxOutVoltageSrc.DataSource = dss;

            CbxOutVoltageSrc.SelectedIndexChanged -= CbxOutVoltageSrc_SelectedIndexChanged;
            CbxOutVoltageSrc.SelectedIndexChanged += CbxOutVoltageSrc_SelectedIndexChanged;

            var types = Enum.GetValues<TurnOnOffType>().Select(x => new ComboBoxItem(EnumEx.GetDescription(x), x)).ToList();

            CbxConvertType.DataSource = types;

            CbxConvertType.SelectedIndexChanged -= CbxConvertType_SelectedIndexChanged;
            CbxConvertType.SelectedIndexChanged += CbxConvertType_SelectedIndexChanged;

            var testtypes= Enum.GetValues<TurnOnOffTestType>().Select(x => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(x)), x)).ToList();
            CbxTestType.DataSource = testtypes;
            CbxTestType.SelectedIndexChanged -= CbxTestType_SelectedIndexChanged;
            CbxTestType.SelectedIndexChanged += CbxTestType_SelectedIndexChanged;
        }

        private void CbxConvertType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Type = (TurnOnOffType)CbxConvertType.SelectValue;
            }
        }

        private void CbxTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TestType = (TurnOnOffTestType)CbxTestType.SelectValue;
            }
        }


        private void CbxInVoltageSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.InVoltageSrc = (ChannelId)CbxInVoltageSrc.SelectValue;
            }
        }

        private void CbxOutVoltageSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.OutVoltageSrc = (ChannelId)CbxOutVoltageSrc.SelectValue;
            }
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
        public PwrOnOffTimePrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrOnOffTimePrsnt)value;
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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.TurnOnOff;


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
                case nameof(Presenter.InVoltageSrc):
                    if((ChannelId)CbxInVoltageSrc.SelectValue != Presenter.InVoltageSrc)
                    {
                        CbxInVoltageSrc.SelectValue = Presenter.InVoltageSrc;
                    }
                    break;
                case nameof(Presenter.OutVoltageSrc):
                    if((ChannelId)CbxOutVoltageSrc.SelectValue != Presenter.OutVoltageSrc)
                    {
                        CbxOutVoltageSrc.SelectValue = Presenter.OutVoltageSrc;
                    }
                    break;
                case nameof(Presenter.InPeakVoltage):
                    SetInputPeakVoltage();
                    break;
                case nameof(Presenter.OutPeakVoltage):
                    SetOutputPeakVoltage();
                    break;
                case nameof(Presenter.AcquisitionTime):
                    SetAcquisitionTime();
                    break;
                case nameof(Presenter.Type):
                   if ((TurnOnOffType)CbxConvertType.SelectValue != Presenter.Type)
                    {
                        CbxConvertType.SelectValue = Presenter.Type;
                    }
                    break;
                case nameof(Presenter.TestType):
                   if((TurnOnOffTestType)CbxTestType.SelectValue != Presenter.TestType)
                    {
                        CbxTestType.SelectValue = Presenter.TestType;
                    }
                    break;
                case nameof(Presenter.RunFlag):
                    BtnRun.Enabled = !Presenter.RunFlag;
                    break;
                default:
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
                CbxInVoltageSrc.SelectValue = Presenter.InVoltageSrc;
                CbxOutVoltageSrc.SelectValue = Presenter.OutVoltageSrc;
                InitVoltage();
                InitAcquisitionTime();
                CbxConvertType.SelectValue = Presenter.Type;
                CbxTestType.SelectValue = Presenter.TestType;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            this.Refresh();
        }

        private void InitVoltage()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnInputPeakVoltage);
            BtnInputPeakVoltage.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnInputPeakVoltage, nameof(Presenter.InPeakVoltage), 0.02);
            };
            SetInputPeakVoltage();

            ControlsHotKnob.Default.InitHotKnob(BtnOutputPeakVoltage);
            BtnOutputPeakVoltage.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnOutputPeakVoltage, nameof(Presenter.OutPeakVoltage), 0.02);
            };
            SetOutputPeakVoltage();
        }

        private void SetInputPeakVoltage()
        {
            this.BtnInputPeakVoltage.Text = SIHelper.ValueChangeToSI(Presenter.InPeakVoltage, 3, Presenter.Unit);
        }

        private void SetOutputPeakVoltage()
        {
            this.BtnOutputPeakVoltage.Text = SIHelper.ValueChangeToSI(Presenter.OutPeakVoltage, 3, Presenter.Unit);
        }

        private void InitAcquisitionTime()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnAcquisitionTime);
            BtnAcquisitionTime.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnAcquisitionTime, nameof(Presenter.AcquisitionTime), 0.02);
            };
            SetAcquisitionTime();
        }

        private void SetAcquisitionTime()
        {
            this.BtnAcquisitionTime.Text = SIHelper.ValueChangeToSI(Presenter.AcquisitionTime, 3, QuantityUnit.Second.ToUnitString());
        }



        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void BtnAcquisitionTime_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AcquisitionTime = this.ShowNumberKeyboard(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhiCaiJiShiJian"), QuantityUnit.Second.ToUnitString(), Presenter.AcquisitionTime, Presenter.MaxAcquisitionTime, Presenter.MinAcquisitionTime);
            }
        }

        private void BtnInPeakVoltage_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.InPeakVoltage = this.ShowNumberKeyboard(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhiShuRuFengZhiDianYa"), Presenter.Unit, Presenter.InPeakVoltage, Presenter.MaxInPeakVoltage, Presenter.MinInPeakVoltage);
            }
        }

        private void BtnOutPeakVoltage_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.OutPeakVoltage = this.ShowNumberKeyboard(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhiShuChuFengZhiDianYa"), Presenter.Unit, Presenter.OutPeakVoltage, Presenter.MaxOutPeakVoltage, Presenter.MinOutPeakVoltage);
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RunFlag = true;
                BtnRun.Enabled = false;
                if (TriggerPrsnt.Mode == TriggerMode.OneShot || TriggerPrsnt.Mode == TriggerMode.Normal)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTO);
                }
                else if (TriggerPrsnt.State == SysState.Stop)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.RUNSTOP);
                }
            }
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowOnOffTimeGuideForm();
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
    }
}
