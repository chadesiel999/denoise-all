// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;

    public partial class TriggerCascadePathSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerCascadePathSubPage()
        {
            InitializeComponent();
            Init();
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

        //public TrigPathwayPrsnt Presenter
        //{
        //get => (TrigPathwayPrsnt)(ParentForm as ITriggerView).Presenter;
        //set => (ParentForm as ITriggerView).Presenter = value;
        //}

        public TrigPathwayPrsnt Presenter 
        { 
            get => (ParentForm as CascadeSettingForm).PathPresenter; 
            set => (ParentForm as CascadeSettingForm).PathPresenter = value; 
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter 
        { 
            get => Presenter; 
            set => Presenter = (TrigPathwayPrsnt)value; 
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

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.EventCounts):
                    NebCount.UpdateValueString();
                    break;
                case nameof(Presenter.DurationByps):
                    NebDuration.UpdateValueString();
                    break;
                case nameof(Presenter.DelayType):
                    RdoDelayType.ChoosedButtonIndex = (Int32)Presenter.DelayType;
                    ChangeCtrlState();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoDelayType.ChoosedButtonIndex = (Int32)Presenter.DelayType;

                ChangeCtrlState();
                NebCount.UpdateValueString();
                NebDuration.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void Init()
        {
            ControlsHotKnob.Default.InitHotKnob(NebDuration);
            NebDuration.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebDuration);
            };
            NebDuration.AddClicked = (_, e) => Presenter.DurationByps += e.Step;
            NebDuration.SubClicked = (_, e) => Presenter.DurationByps += e.Step;
            NebDuration.StringFormatFunc = (_) => DurationToString();
            NebDuration.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebDuration);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.DurationByps = (Int64)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico));

                nkf.SetKeyBoardValue(LblDuration.Text, QuantityUnit.Second.ToUnitString(), 9, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.DurationByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MaxDuration, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MinDuration, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebCount);
            NebCount.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebCount);
            };
            NebCount.AddClicked = (_, e) => Presenter.EventCounts += e.Step;
            NebCount.SubClicked = (_, e) => Presenter.EventCounts += e.Step;
            NebCount.StringFormatFunc = (_) => CountToString();
            NebCount.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebCount);
                nkf.NumberKeyboard.UseSI = false;
                var onokclickeventaction = new Action<Double>((data) => Presenter.EventCounts = (Int32)data);

                nkf.SetKeyBoardValue(LblCount.Text, QuantityUnit.Count.ToUnitString(), 0, onokclickeventaction,
                   Presenter.EventCounts,
                   Presenter.MaxEventCounts,
                   Presenter.MinEventCounts);        

                nkf.ShowDialogByPosition();
            };
        }

        private void ChangeCtrlState()
        {
            var flag = Presenter.DelayType == DelayOpt.Event;
            LblCount.Visible = NebCount.Visible = flag;
            LblDuration.Visible = NebDuration.Visible = !flag;
        }

        private void RdoDelayType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.DelayType = (DelayOpt)RdoDelayType.ChoosedButtonIndex;
            }
        }

        private String DurationToString()
        {
            return new Quantity(Presenter.DurationByps, Prefix.Pico, QuantityUnit.Second).ToString("##0.#########", true, 13);
        }

        private String CountToString()
        {
            return Presenter.EventCounts.ToString() + " #";
        }
    }
}
