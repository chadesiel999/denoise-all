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

    public partial class SearchPulseSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public SearchPulseSubPage(SearchItemPrsnt item)
        {
            InitializeComponent();
            Item = item;
            Presenter = item.SearchTypePrsnt as SearchPulsePrsnt;
            Init();
        }

        private SearchItemPrsnt Item
        {
            get; set;
        }

        public SearchPulsePrsnt Presenter
        {
            get;
            set;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

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

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchPulsePrsnt)value; }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        protected void Update(Object presenter, String propertyName)
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
                    CbxSource.SelectValue = Presenter.Source.ToString();
                    ChangeWidthStatu();
                    break;
                case "CompPosIndex":
                    NebPositon.UpdateValueString();
                    break;
                case nameof(Presenter.WidthByps):
                    NebWidth.UpdateValueString();
                    break;
                case nameof(Presenter.UpperWidthByps):
                    NebWidthUpper.UpdateValueString();
                    break;
                case nameof(Presenter.Polarity):
                    if(RdoPolarity.ChoosedButtonIndex != (Int32)Presenter.Polarity)
                    {
                        RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                        Item.SetModelValue(nameof(Presenter.Polarity), Presenter.Polarity);
                    }
                    break;
                case nameof(Presenter.Condition):
                    if(CbxCondition.SelectIndex != (Int32)Presenter.Condition)
                    {
                        CbxCondition.SelectIndex = (Int32)Presenter.Condition;
                        Item.SetModelValue(nameof(Presenter.Condition), Presenter.Condition);
                    }
                    ChangeWidthStatu();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Presenter != null)
            {
                Presenter.TryAddView(this);
            }
            UpdateView();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSource.SelectValue = Presenter.Source.ToString();
                CbxCondition.SelectIndex = (Int32)Presenter.Condition;
                NebPositon.UpdateValueString();
                NebWidth.UpdateValueString();
                NebWidthUpper.UpdateValueString();
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                ChangeWidthStatu();
                _ArgToCtrl = false;
            }
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
            Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
        }

        private void BtnWidthReset_Click(object sender, EventArgs e)
        {
            Presenter.WidthByps = Presenter.MinWidth;
            Item.SetModelValue(nameof(Presenter.WidthByps), Presenter.WidthByps);
        }

        private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Condition = (PulseCondition)CbxCondition.SelectIndex;
                Item.SetModelValue(nameof(Presenter.Condition), Presenter.Condition);
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>(CbxSource.Items[(int)CbxSource.SelectValue]);
                Item.Source = Presenter.Source;
            }
        }

        private String CompPosToString()
        {
            return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString(5, true);
        }

        private void Init()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPositon);
            NebPositon.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPositon);
            };
            NebPositon.StringFormatFunc = (_) => CompPosToString();
            NebPositon.AddClicked = (_, e) =>
            { 
                Presenter.PosIndex += e.Step;
                Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
            };
            NebPositon.SubClicked = (_, e) =>
            {
                Presenter.PosIndex += e.Step;
                Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
            };
            NebPositon.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPositon);
                var onokclickeventaction = new Action<Double>((data) =>
                    {
                        Presenter.CompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix);
                        Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
                    });

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.CompPosition, Presenter.PosPrefix),
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
            NebWidth.StringFormatFunc = (_) => WidthToString(Presenter.WidthByps);
            NebWidth.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebWidth);
                var onokclickeventaction = new Action<Double>((data) =>
                    {
                        Presenter.WidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico));
                        Item.SetModelValue(nameof(Presenter.WidthByps), Presenter.WidthByps);
                    });

                var (min, max) = Presenter.GetWidthRange();
                nkf.SetKeyBoardValue(LblWidth.Text, QuantityUnit.Second.ToUnitString(), 4, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.WidthByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebWidthUpper);
            NebWidthUpper.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebWidthUpper);
            };
            NebWidthUpper.AddClicked = (o, e) => Presenter.AdjUpperWidth(e.Step);
            NebWidthUpper.SubClicked = (o, e) => Presenter.AdjUpperWidth(e.Step);
            NebWidthUpper.StringFormatFunc = (_) => WidthToString(Presenter.UpperWidthByps);
            NebWidthUpper.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebWidthUpper);
                var onokclickeventaction = new Action<Double>((data) =>
                    {
                        Presenter.UpperWidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico));
                        Item.SetModelValue(nameof(Presenter.UpperWidthByps), Presenter.UpperWidthByps);
                    });

                var (min, max) = Presenter.GetUpperWidthRange();
                nkf.SetKeyBoardValue(LblWidthUpper.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperWidthByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };

            InitSourceList();
        }

        private void InitSourceList()
        {
            //CbxSource.Items = ChannelIdExt.GetAnalogs().Concat(ChannelIdExt.GetDigitals()).Select(o => o.ToString()).ToArray();
            CbxSource.Items = ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray();
        }

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (PulsePolarity)RdoPolarity.ChoosedButtonIndex;
                Item.SetModelValue(nameof(Presenter.Polarity), Presenter.Polarity);
            }
        }

        private static String WidthToString(Int64 width)
        {
            return new Quantity(width, Prefix.Pico, QuantityUnit.Second).ToString("##0.###########", true, 14);
        }

        private void ChangeWidthStatu()
        {
            if (Presenter.Condition == PulseCondition.GreaterThan)
            {
                LblWidth.Visible = true;
                NebWidth.Visible = true;
                LblWidthUpper.Visible = false;
                NebWidthUpper.Visible = false;
                NebWidth.UpdateValueString();
            }
            else if (Presenter.Condition == PulseCondition.LessThan)
            {
                LblWidth.Visible = false;
                NebWidth.Visible = false;
                LblWidthUpper.Visible = true;
                NebWidthUpper.Visible = true;
                NebWidthUpper.UpdateValueString();
            }
            else
            {
                LblWidth.Visible = true;
                NebWidth.Visible = true;
                LblWidthUpper.Visible = true;
                NebWidthUpper.Visible = true;
                NebWidth.UpdateValueString();
                NebWidthUpper.UpdateValueString();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Presenter != null)
            {
                Presenter.TryRemoveView(this);
            }
        }
    }
}
