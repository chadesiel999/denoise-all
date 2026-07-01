// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
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

    public partial class TriggerWindowSubPage : UserControl, ITriggerView, IStylize
    {

        private Boolean _ArgToCtrl;

        public TriggerWindowSubPage()
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

        public TrigWindowPrsnt Presenter
        {
            get => (TrigWindowPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigWindowPrsnt)value;
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
                    //CbxSource.Text = Presenter.Source.ToString();
                    CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    NebUpperPos.UpdateValueString();
                    NebBelowPos.UpdateValueString();
                    break;
                case "PosIndex":
                    NebUpperPos.UpdateValueString();
                    NebBelowPos.UpdateValueString();
                    break;

                case nameof(Presenter.TimeCondition):
                    RdoTimeCondition.ChoosedButtonIndex = (Int32)Presenter.TimeCondition;
                    ChangeWidthState();
                    break;
                case nameof(Presenter.WidthByps):
                    NebWidth.UpdateValueString();
                    break;
                case nameof(Presenter.PosCondition):
                    RdoPosCompCondition.ChoosedButtonIndex = (Int32)Presenter.PosCondition;
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
                RdoTimeCondition.ChoosedButtonIndex = (Int32)Presenter.TimeCondition;
                RdoPosCompCondition.ChoosedButtonIndex = (int)Presenter.PosCondition;

                NebUpperPos.UpdateValueString();
                NebBelowPos.UpdateValueString();
                ChangeWidthState();
                NebWidth.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void ChangeWidthState()
        {
            LblWidth.Visible = NebWidth.Visible = Presenter.TimeCondition != WindowTimeCondition.OnEnter;
        }

        private void Init()
        {
            //NebUpperPos
            ControlsHotKnob.Default.InitHotKnob(NebUpperPos);
            NebUpperPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebUpperPos, nameof(Presenter.VuUpperCompPosition));
            };
            NebUpperPos.StringFormatFunc = (_) => CompPosToString(Presenter.VuUpperCompPosition);
            NebUpperPos.AddClicked = (_, e) => Presenter.PosUpperIndex += e.Step;
            NebUpperPos.SubClicked = (_, e) => Presenter.PosUpperIndex += e.Step;
            NebUpperPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuUpperCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblUpperPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuUpperCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            //NeblowPos
            ControlsHotKnob.Default.InitHotKnob(NebBelowPos);
            NebBelowPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebBelowPos, nameof(Presenter.VuLowerCompPosition));
            };
            NebBelowPos.StringFormatFunc = (_) => CompPosToString(Presenter.VuLowerCompPosition);
            NebBelowPos.AddClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebBelowPos.SubClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebBelowPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBelowPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuLowerCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblLowerPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuLowerCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebWidth
            ControlsHotKnob.Default.InitHotKnob(NebWidth);
            NebWidth.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebWidth,nameof(Presenter.WidthByps));
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

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
            }
        }

        private void RdoTimeCondition_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TimeCondition = (WindowTimeCondition)RdoTimeCondition.ChoosedButtonIndex;
            }
        }

        private void RdoPosCompCondition_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PosCondition = (WindowRange)RdoPosCompCondition.ChoosedButtonIndex;
            }
        }

        private String WidthToString()
        {
            return new Quantity(Presenter.WidthByps, Prefix.Pico, "s").ToString("##0.###", true, 14);
        }

        private String CompPosToString(Double position)
        {
            //return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####", true, 7);
            }
            else
            {
                return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            }
        }

    }
}
