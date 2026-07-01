namespace ScopeX.U2.Search
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
    public partial class SearchRuntSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;
        public SearchRuntSubPage(SearchRuntPrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
            Init();
        }

        public SearchRuntPrsnt Presenter
        {
            get;
            set;
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

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchRuntPrsnt)value; }

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

        private void Init()
        {
            //NebUpperPos
            ControlsHotKnob.Default.InitHotKnob(NebUpperPos);
            NebUpperPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebUpperPos);
            };
            NebUpperPos.StringFormatFunc = (_) => CompPosToString(Presenter.UpperCompPosition);
            NebUpperPos.AddClicked = (_, e) => Presenter.PosUpperIndex += e.Step;
            NebUpperPos.SubClicked = (_, e) => Presenter.PosUpperIndex += e.Step;
            NebUpperPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.UpperCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblUpperPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            //NeblowPos
            ControlsHotKnob.Default.InitHotKnob(NebLowerPos);
            NebLowerPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLowerPos);
            };
            NebLowerPos.StringFormatFunc = (_) => CompPosToString(Presenter.LowerCompPosition);
            NebLowerPos.AddClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebLowerPos.SubClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebLowerPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowerPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.LowerCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblLowerPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.LowerCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebTime
            ControlsHotKnob.Default.InitHotKnob(NebWidth);
            NebWidth.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebWidth);
            };
            NebWidth.AddClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebWidth.SubClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebWidth.StringFormatFunc = (_) => WidthToString(Presenter.WidthByps);
            NebWidth.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebWidth);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.WidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetWidthRange();
                nkf.SetKeyBoardValue(LblWidth.Text, QuantityUnit.Second.ToUnitString(), 12, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.WidthByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebUpperWidth);
            NebUpperWidth.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebUpperWidth);
            };
            NebUpperWidth.AddClicked = (o, e) => Presenter.AdjUpperWidth(e.Step);
            NebUpperWidth.SubClicked = (o, e) => Presenter.AdjUpperWidth(e.Step);
            NebUpperWidth.StringFormatFunc = (_) => WidthToString(Presenter.UpperWidthByps);
            NebUpperWidth.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperWidth);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.UpperWidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetUpperWidthRange();
                nkf.SetKeyBoardValue(LblUpperWidth.Text, QuantityUnit.Second.ToUnitString(), 12, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperWidthByps, Prefix.Pico),
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

        private void ChangeCtrlState()
        {
            LblUpperWidth.Visible = NebUpperWidth.Visible = (Presenter.WidthCompCondition == PulseCondition.Equal) || (Presenter.WidthCompCondition == PulseCondition.NotEqual);
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
                    NebUpperPos.UpdateValueString();
                    NebLowerPos.UpdateValueString();
                    break;
                case "PosIndex":
                    NebUpperPos.UpdateValueString();
                    NebLowerPos.UpdateValueString();
                    break;

                case "Condition":
                    CbxCondition.SelectedIndex = (Int32)Presenter.WidthCompCondition;
                    ChangeCtrlState();
                    NebWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
                    break;
                case nameof(Presenter.WidthByps):
                case nameof(Presenter.UpperWidthByps):
                    NebWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
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
                CbxCondition.SelectedIndex = (Int32)Presenter.WidthCompCondition;
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;

                NebUpperPos.UpdateValueString();
                NebLowerPos.UpdateValueString();
                NebWidth.UpdateValueString();
                ChangeCtrlState();
                NebUpperWidth.UpdateValueString();

                _ArgToCtrl = false;
            }
        }

        private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.WidthCompCondition = (PulseCondition)CbxCondition.SelectedIndex;
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
            }
        }

        private void RdoPolarity_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (PulsePolarity)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private static String WidthToString(Int64 width)
        {
            return new Quantity(width, Prefix.Pico, "s").ToString("##0.############", true, 14);
        }

        private String CompPosToString(Double pos)
        {
            return new Quantity(pos, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
        }

    }
}
