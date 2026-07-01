// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

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

    public partial class TriggerRuntSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerRuntSubPage()
        {
            InitializeComponent();
            Init();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public TrigRuntPrsnt Presenter
        {
            get => (TrigRuntPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigRuntPrsnt)value;
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
        private void ReloadLang()
        {
            _ArgToCtrl = true;
            LblUpperPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GaoDianPing");
            LblLowerPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiDianPing");
            LblCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoJian");

            CbxCondition.Items = new string[] { ">", "<", "[ ... ]", "] ... ["/*ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WuGuan")*/ };
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblWidth.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianXiaXian");


            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zheng");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Fu");
            RdoPolarity.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 });

            LblPolarity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MaiKuan");
            LblUpperWidth.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianShangXian");
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ReloadLang();
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
            ControlsHotKnob.Default.InitHotKnob(NebLowerPos);
            NebLowerPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLowerPos);
            };
            NebLowerPos.StringFormatFunc = (_) => CompPosToString(Presenter.VuLowerCompPosition);
            NebLowerPos.AddClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebLowerPos.SubClicked = (_, e) => Presenter.PosLowerIndex += e.Step;
            NebLowerPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowerPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuLowerCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblLowerPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuLowerCompPosition, Presenter.PosPrefix),
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
                nkf.SetKeyBoardValue(LblWidth.Text, QuantityUnit.Second.ToUnitString(), 10, onokclickeventaction,
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
                nkf.SetKeyBoardValue(LblUpperWidth.Text, QuantityUnit.Second.ToUnitString(), 10, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperWidthByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(max, Prefix.Pico),
                    Quantity.ConvertByPrefix(min, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };
        }

        private void ChangeCtrlState()
        {
            //LblUpperWidth.Visible = NebUpperWidth.Visible = (Presenter.WidthCompCondition == PulseCondition.Equal) || (Presenter.WidthCompCondition == PulseCondition.NotEqual);
            if (Presenter.WidthCompCondition == PulseCondition.GreaterThan)
            {
                LblWidth.Visible = true;
                NebWidth.Visible = true;
                LblUpperWidth.Visible = false;
                NebUpperWidth.Visible = false;
            }
            else if (Presenter.WidthCompCondition == PulseCondition.LessThan)
            {
                LblWidth.Visible = false;
                NebWidth.Visible = false;
                LblUpperWidth.Visible = true;
                NebUpperWidth.Visible = true;
            }
            else
            {
                LblWidth.Visible = true;
                NebWidth.Visible = true;
                LblUpperWidth.Visible = true;
                NebUpperWidth.Visible = true;
            }
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
                    //CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    CbxSource.SelectValue = (Int32)Presenter.Source;
                    NebUpperPos.UpdateValueString();
                    NebLowerPos.UpdateValueString();
                    break;
                case "PosIndex":
                    NebUpperPos.UpdateValueString();
                    NebLowerPos.UpdateValueString();
                    break;

                case "Condition":
                    //CbxCondition.SelectedIndex = (Int32)Presenter.WidthCompCondition;
                    CbxCondition.SelectValue = (Int32)Presenter.WidthCompCondition;
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
                CbxSource.Items = ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray();
                CbxSource.SelectValue = (Int32)Presenter.Source;
                CbxCondition.SelectValue = (Int32)Presenter.WidthCompCondition;
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;

                NebUpperPos.UpdateValueString();
                NebLowerPos.UpdateValueString();
                NebWidth.UpdateValueString();
                ChangeCtrlState();
                NebUpperWidth.UpdateValueString();

                _ArgToCtrl = false;
            }
        }

        //private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.WidthCompCondition = (PulseCondition)CbxCondition.SelectedIndex;
        //    }
        //}

        //private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
        //    }
        //}

        private void RdoPolarity_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (PulsePolarity)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private static String WidthToString(Int64 width)
        {
            return new Quantity(width, Prefix.Pico, "s").ToString("##0.##########", true, 14);
        }

        private String CompPosToString(Double pos)
        {
            //return new Quantity(pos, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(pos, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7);
            }
            else
            {
                return new Quantity(pos, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7);
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (ChannelId)CbxSource.SelectValue;
            }
        }

        private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.WidthCompCondition = (PulseCondition)CbxCondition.SelectValue;
            }
        }
    }
}
