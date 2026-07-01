// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Controls.Common.Structs;
    

    public partial class TriggerPatSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        private PatSettingForm _PatternForm;

        public TriggerPatSubPage()
        {
            InitializeComponent();
            Init();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public TrigPatPrsnt Presenter
        {
            get => (TrigPatPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigPatPrsnt)value;
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

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
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
                case nameof(Presenter.Operator):
                    CbxOperator.SelectedIndex = (Int32)Presenter.Operator;
                    break;

                case nameof(Presenter.TimeCondition):
                    CbxTimeCondition.SelectedIndex = (Int32)Presenter.TimeCondition;
                    ChangeCtrlState();
                    NebDuration.UpdateValueString();
                    NebUpperDuration.UpdateValueString();
                    break;
                case nameof(Presenter.DurationByps):
                case nameof(Presenter.UpperDurationByps):
                    NebDuration.UpdateValueString();
                    NebUpperDuration.UpdateValueString();
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxOperator.SelectedIndex = (Int32)Presenter.Operator;
                CbxTimeCondition.SelectedIndex = (Int32)Presenter.TimeCondition;

                ChangeCtrlState();
                NebDuration.UpdateValueString();
                NebUpperDuration.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void Init()
        {
            //NebTime
            ControlsHotKnob.Default.InitHotKnob(NebDuration);
            NebDuration.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebDuration);
            };
            NebDuration.AddClicked = (o, e) => Presenter.AdjDuration(e.Step);
            NebDuration.SubClicked = (o, e) => Presenter.AdjDuration(e.Step);
            NebDuration.StringFormatFunc = (_) => DurationToString(Presenter.DurationByps);
            NebDuration.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebDuration);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.DurationByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetDurationRange();
                nkf.SetKeyBoardValue(LblDuration.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.DurationByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebUpperDuration);
            NebUpperDuration.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebUpperDuration);
            };
            NebUpperDuration.AddClicked = (o, e) => Presenter.AdjUpperDuration(e.Step);
            NebUpperDuration.SubClicked = (o, e) => Presenter.AdjUpperDuration(e.Step);
            NebUpperDuration.StringFormatFunc = (_) => DurationToString(Presenter.UpperDurationByps);
            NebUpperDuration.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperDuration);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.UpperDurationByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetUpperDurationRange();
                nkf.SetKeyBoardValue(LblUpperDuration.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperDurationByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };
        }

        private void ChangeCtrlState()
        {
            LblUpperDuration.Visible = NebUpperDuration.Visible = (Presenter.TimeCondition == PatTimeCondition.Inside) || (Presenter.TimeCondition == PatTimeCondition.Outside);
        }

        private void BtnDefine_Click(object sender, EventArgs e)
        {
            //if (_PatternForm == null) 
            {
                _PatternForm = new PatSettingForm
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    TrigPresenter = this.Presenter,
                    ActiveBorderColor = AppStyleConfig.DefaultFormActiveBorderColor,
                    ActiveBorderVisiable = true,
                };
                _PatternForm.TrigPresenter.TryAddView(_PatternForm);

                if (Program.Oscilloscope.TryGetChannel(ChannelId.D0, out var dch))
                {
                    _PatternForm.DigiPresenter = (DigitalPrsnt)dch;
                    _PatternForm.DigiPresenter.TryAddView(_PatternForm);
                }
            }

            (ParentForm as TriggerForm).CanClose = false;

            _PatternForm.ShowDialogByPosition();

            if (ParentForm is TriggerForm triggerform)
            {
                triggerform.CanClose = true;
            }
        }

        private void CbxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Operator = (PatOperator)CbxOperator.SelectedIndex;
            }
        }

        private void CbxTimeCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TimeCondition = (PatTimeCondition)CbxTimeCondition.SelectedIndex;
            }
        }

        private static String DurationToString(Int64 duration)
        {
            return new Quantity(duration, Prefix.Pico, "s").ToString("##0.###", true, 14);
        }
    }
}
