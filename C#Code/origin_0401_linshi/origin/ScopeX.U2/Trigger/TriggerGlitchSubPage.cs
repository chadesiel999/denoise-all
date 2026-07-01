// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;

    public partial class TriggerGlitchSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerGlitchSubPage()
        {
            InitializeComponent();
            Init();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public TrigGlitchPrsnt Presenter
        {
            get => (TrigGlitchPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigGlitchPrsnt)value;
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
                case nameof(Presenter.Source):
                    CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    NebPositon.UpdateValueString();
                    break;
                case "CompPosIndex":
                    NebPositon.UpdateValueString();
                    break;

                case nameof(Presenter.WidthByps):
                    NebWidth.UpdateValueString();
                    break;
                case nameof(Presenter.Polarity):
                    RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;

                NebPositon.UpdateValueString();
                NebWidth.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void Init()
        {

            ControlsHotKnob.Default.InitHotKnob(NebPositon);
            NebPositon.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPositon);
            };
            NebPositon.StringFormatFunc = (_) => CompPosToString();
            NebPositon.AddClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPositon.SubClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPositon.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPositon);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebWidth);
            NebWidth.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebWidth);
            };
            NebWidth.AddClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebWidth.SubClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebWidth.StringFormatFunc = (_) => WidthToString();
            NebWidth.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebWidth);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.WidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetWidthRange();
                nkf.SetKeyBoardValue(LblWidth.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.WidthByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };

            InitSourceList();
        }

        private void InitSourceList()
        {
            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
        }

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (PulsePolarity)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.SetPosIndexCenter();
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
            }
        }

        private String CompPosToString()
        {
            //return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####", true, 7);
            }
            else
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            }
        }

        private String WidthToString()
        {
            return new Quantity(Presenter.WidthByps, Prefix.Pico, "s").ToString("##0.###", true, 14);
        }
    }
}
