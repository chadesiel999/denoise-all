using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class RadioFrequencyPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public RadioFrequencyPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //NebAverageTimes

            ControlsHotKnob.Default.InitHotKnob(NebAverageTimes);
            NebAverageTimes.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAverageTimes);
            };
            NebAverageTimes.AddClicked = (a, b) => Presenter.AverageTimes++;
            NebAverageTimes.SubClicked = (a, b) => Presenter.AverageTimes--;
            NebAverageTimes.StringFormatFunc = (value) => Presenter.AverageTimes.ToString();

            NebAverageTimes.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAverageTimes);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.AverageTimes = (Int32)data;

                nkf.SetKeyBoardValue(LblAverageTimes.Text, QuantityUnit.Constant.ToUnitString(), 0, onokclickeventaction,
                    Presenter.AverageTimes,
                    10_000,
                    1);

                //numberkeybordform.Location = numberkeybordform.CalculateWindowPosition();
                //DialogResult dialogresult = numberkeybordform.ShowDialogByEvent();
                nkf.ShowDialogByPosition();
            };

            //NebAmpScale
            ControlsHotKnob.Default.InitHotKnob(NebAmpScale);
            NebAmpScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmpScale);
            };
            NebAmpScale.AddClicked = (a, b) => Presenter.AmpScale++;
            NebAmpScale.SubClicked = (a, b) => Presenter.AmpScale--;
            NebAmpScale.StringFormatFunc = (value) => AmpScaleToString();
            NebAmpScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.AmpScale = data;

                //dBm
                nkf.SetKeyBoardValue(LblAmpScale.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction,
                    Presenter.AmpScale,
                    Constants.RF_AMP_MAX_SCALE,
                    Constants.RF_AMP_MIN_SCALE);
                nkf.ShowDialogByPosition();
            };

            //NebAmpCenter
            ControlsHotKnob.Default.InitHotKnob(NebAmpCenter);
            NebAmpCenter.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmpCenter);
            };
            NebAmpCenter.AddClicked = (a, b) => Presenter.FigureCenterAmplitude++;
            NebAmpCenter.SubClicked = (a, b) => Presenter.FigureCenterAmplitude--;
            NebAmpCenter.StringFormatFunc = (value) => AmpCenterToString();
            NebAmpCenter.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpCenter);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FigureCenterAmplitude = data;

                nkf.SetKeyBoardValue(LblAmpCenter.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction,
                    Presenter.FigureCenterAmplitude,
                    Constants.RF_CENTER_AMPLITUDE_MAX,
                    Constants.RF_CENTER_AMPLITUDE_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebReferenceLevel
            ControlsHotKnob.Default.InitHotKnob(NebReferenceLevel);
            NebReferenceLevel.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebReferenceLevel);
            };
            NebReferenceLevel.AddClicked = (a, b) => Presenter.RefLevelValue++;
            NebReferenceLevel.SubClicked = (a, b) => Presenter.RefLevelValue--;
            NebReferenceLevel.StringFormatFunc = (value) => ReferenceLevelToString();
            NebReferenceLevel.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebReferenceLevel);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.RefLevelValue = (Int32)data;

                nkf.SetKeyBoardValue(LblReferenceLevel.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction,
                    Presenter.RefLevelValue,
                    Constants.RF_REF_LEVEL_MAX,
                    Constants.RF_REF_LEVEL_MIN);

                nkf.ShowDialogByPosition();
            };

            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource.Items.Add(ChannelId.RF.ToString());
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
                }
            };
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public RadioFrequencyPrsnt Presenter
        {
            get => (RadioFrequencyPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (RadioFrequencyPrsnt)value;
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
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
                    ChkActive.Checked = Presenter.Active;
                    ChangeCtrlState();
                    break;
                case nameof(Presenter.NormalLine):
                    ChkNormal.Checked = Presenter.NormalLine;
                    break;
                case nameof(Presenter.NormalLinePickMode):
                    CbxNormalPicker.SelectedIndex = (Int32)Presenter.NormalLinePickMode;
                    break;
                case nameof(Presenter.MaxHoldLine):
                    ChkMaxHold.Checked = Presenter.MaxHoldLine;
                    break;
                case nameof(Presenter.MaxHoldLinePickMode):
                    CbxMaxPicker.SelectedIndex = (Int32)Presenter.MaxHoldLinePickMode;
                    break;
                case nameof(Presenter.MinHoldLine):
                    ChkMinHold.Checked = Presenter.MinHoldLine;
                    break;
                case nameof(Presenter.MinHoldLinePickMode):
                    CbxMinPicker.SelectedIndex = (Int32)Presenter.MinHoldLinePickMode;
                    break;
                case nameof(Presenter.AverageLine):
                    ChkAverage.Checked = Presenter.AverageLine;
                    break;
                case nameof(Presenter.AverageLinePickMode):
                    CbxAvePicker.SelectedIndex = (Int32)Presenter.AverageLinePickMode;
                    break;
                case nameof(Presenter.AverageTimes):
                    NebAverageTimes.UpdateValueString();
                    break;
                case nameof(Presenter.PUnit):
                    CbxPowerUnit.SelectedIndex = (Int32)Presenter.PUnit;
                    NebAmpScale.UpdateValueString();
                    NebAmpCenter.UpdateValueString();
                    NebReferenceLevel.UpdateValueString();
                    break;
                case nameof(Presenter.Label):
                    TbxLabel.Text = Presenter.Label;
                    break;
                case nameof(Presenter.Window):
                    CbxWindowType.SelectedIndex = (Int32)Presenter.Window;
                    break;
                case nameof(Presenter.AmpScale):
                    NebAmpScale.UpdateValueString();
                    break;
                case nameof(Presenter.FigureCenterAmplitude):
                    NebAmpCenter.UpdateValueString();
                    break;
                case nameof(Presenter.RefLevelValue):
                    NebReferenceLevel.UpdateValueString();
                    break;
                case nameof(Presenter.Source):
                    CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    break;

            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                ChkActive.Checked = Presenter.Active;
                ChkNormal.Checked = Presenter.NormalLine;
                CbxNormalPicker.SelectedIndex = (Int32)Presenter.NormalLinePickMode;
                ChkMaxHold.Checked = Presenter.MaxHoldLine;
                CbxMaxPicker.SelectedIndex = (Int32)Presenter.MaxHoldLinePickMode;
                ChkMinHold.Checked = Presenter.MinHoldLine;
                CbxMinPicker.SelectedIndex = (Int32)Presenter.MinHoldLinePickMode;
                ChkAverage.Checked = Presenter.AverageLine;
                CbxAvePicker.SelectedIndex = (Int32)Presenter.AverageLinePickMode;
                CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                CbxWindowType.SelectedIndex = (Int32)Presenter.Window;
                CbxPowerUnit.SelectedIndex = (Int32)Presenter.PUnit;
                TbxLabel.Text = Presenter.Label;

                NebAverageTimes.UpdateValueString();
                NebAmpScale.UpdateValueString();
                NebAmpCenter.UpdateValueString();
                NebReferenceLevel.UpdateValueString();
                ChangeCtrlState();
                _ArgToCtrl = false;
            }

        }

        private void ChangeCtrlState()
        {
            ChkNormal.Enabled = ChkMaxHold.Enabled = ChkMinHold.Enabled = ChkAverage.Enabled = Presenter.Active;
        }

        private String AmpScaleToString() => new Quantity(Presenter.AmpScale, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);

        private String AmpCenterToString() => new Quantity(Presenter.FigureCenterAmplitude, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);

        private String ReferenceLevelToString() => new Quantity(Presenter.RefLevelValue, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AmpVSTime.Active = false;
                Presenter.PhaseVSTime.Active = false;
                Presenter.PhaseVSFrequency.Active = false;

                Presenter.Active = false;
            }
        }

        private void ChkAverage_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AverageLine = ChkAverage.Checked;
            }
        }

        private void ChkMinHold_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.MinHoldLine = ChkMinHold.Checked;
            }
        }

        private void ChkMaxHold_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.MaxHoldLine = ChkMaxHold.Checked;
            }
        }

        private void ChkNormal_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.NormalLine = ChkNormal.Checked;
            }
        }

        private void CbxPowerUnit_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PUnit = (LogarithmUnit)CbxPowerUnit.SelectedIndex;
            }
        }

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            Presenter.Label = TbxLabel.Text;
        }

        private void CbxNormalPicker_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.NormalLinePickMode = (PickMode)CbxNormalPicker.SelectedIndex;
            }
        }

        private void CbxMaxPicker_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.MaxHoldLinePickMode = (PickMode)CbxMaxPicker.SelectedIndex;
            }
        }

        private void CbxMinPicker_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.MinHoldLinePickMode = (PickMode)CbxMinPicker.SelectedIndex;
            }
        }

        private void CbxAvePicker_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AverageLinePickMode = (PickMode)CbxAvePicker.SelectedIndex;
            }
        }

        private void CbxWindowType_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Window = (RFWindowType)CbxWindowType.SelectedIndex;
            }
        }
    }
}
