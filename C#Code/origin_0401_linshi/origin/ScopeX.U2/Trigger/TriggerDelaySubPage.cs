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
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using System.Collections.Generic;
    using static ScopeX.UserControls.SelectComboBox;
    

    public partial class TriggerDelaySubPage : UserControl, ITriggerView, IStylize
    {

        private Boolean _ArgToCtrl;

        public TriggerDelaySubPage()
        {
            InitializeComponent();
            InitLanguage();
            Init();
        }

        private void InitLanguage()
        {
            LblUpperPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan1DianPing");
            LblLowerPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan2DianPing");

            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            RdoSourceOneSlope.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 });

            LblSourceOneSlope.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan1BianYan");
            LblCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoJian");
            LblSourceOne.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan1");
            LblLowerWidth.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianXiaXian");
            LblSourceTwo.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan2");

            ScopeX.UserControls.RadioButtonItem radioButtonItem3 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem4 = new ScopeX.UserControls.RadioButtonItem();
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            RdoSourceTwoSlope.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem3, radioButtonItem4 });

            LblSourceTwoSlope.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan2BianYan");
            LblUpperWidth.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianShangXian");
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

        public TrigDelayPrsnt Presenter
        {
            get => (TrigDelayPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigDelayPrsnt)value;
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
            //DsoPrsnt.FocusId = Presenter.SourceOne;
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
                case "Source":
                    //SourceTwoItemsVisiable();
                    //SourceOneItemsVisiable();
                    CbxSourceOne.SelectValue = Presenter.SourceOne;
                    CbxSourceTwo.SelectValue = Presenter.SourceTwo;
                    break;
                case nameof(Presenter.SourceOne):
                    LoadSourceTwo();
                    if (Presenter.SourceOne != (ChannelId)CbxSourceOne.SelectValue)
                    {
                        CbxSourceOne.SelectValue = Presenter.SourceOne;
                        //SourceTwoItemsVisiable();
                        NebUpperPos.UpdateValueString();
                        NebBelowPos.UpdateValueString(); 
                    }
                    break;
                case nameof(Presenter.SourceTwo):
                    LoadSourceOne();
                    if(Presenter.SourceTwo!=(ChannelId)CbxSourceTwo.SelectValue)
                    {
                        CbxSourceTwo.SelectValue = Presenter.SourceTwo;
                        //SourceOneItemsVisiable();
                        NebUpperPos.UpdateValueString();
                        NebBelowPos.UpdateValueString();
                    }
                    break;
                case "PosIndex":
                case "CompPosIndex":
                    NebUpperPos.UpdateValueString();
                    NebBelowPos.UpdateValueString();
                    break;

                case "Condition":
                    //CbxCondition.SelectedIndex = (Int32)Presenter.WidthCompCondition;
                    CbxCondition.SelectValue = (Int32)Presenter.WidthCompCondition;
                    ChangeCtrlState();
                    NebLowerWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
                    break;
                case nameof(Presenter.WidthByps):
                case nameof(Presenter.UpperWidthByps):
                    NebLowerWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
                    break;
                case nameof(Presenter.SourceOneSlope):
                    RdoSourceOneSlope.ChoosedButtonIndex = (Int32)Presenter.SourceOneSlope;
                    break;
                case nameof(Presenter.SourceTwoSlope):
                    RdoSourceTwoSlope.ChoosedButtonIndex = (Int32)Presenter.SourceTwoSlope;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                LoadSourceOne();
                LoadSourceTwo();
                //SourceTwoItemsVisiable();
                //SourceOneItemsVisiable();
                RdoSourceOneSlope.ChoosedButtonIndex = (Int32)Presenter.SourceOneSlope;
                RdoSourceTwoSlope.ChoosedButtonIndex = (Int32)Presenter.SourceTwoSlope;
                //CbxCondition.SelectedIndex = (Int32)Presenter.WidthCompCondition;
                CbxCondition.SelectValue = (Int32)Presenter.WidthCompCondition;

                NebUpperPos.UpdateValueString();
                NebBelowPos.UpdateValueString();
                ChangeCtrlState();
                NebLowerWidth.UpdateValueString();
                NebUpperWidth.UpdateValueString();
                _ArgToCtrl = false;
            }
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

            //NebBelowPos
            ControlsHotKnob.Default.InitHotKnob(NebBelowPos);
            NebBelowPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebBelowPos);
            };
            NebBelowPos.StringFormatFunc = (_) => CompPosToString(Presenter.VuDataCompPosition);
            NebBelowPos.AddClicked = (_, e) => Presenter.DataCompPosIndex += e.Step;
            NebBelowPos.SubClicked = (_, e) => Presenter.DataCompPosIndex += e.Step;
            NebBelowPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBelowPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuDataCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.DataPrefix));

                nkf.SetKeyBoardValue(LblLowerPos.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuDataCompPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxDataCompPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinDataCompPosition, Presenter.DataPrefix));

                nkf.ShowDialogByPosition();
            };



            ControlsHotKnob.Default.InitHotKnob(NebLowerWidth);
            NebLowerWidth.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLowerWidth);
            };
            NebLowerWidth.AddClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebLowerWidth.SubClicked = (o, e) => Presenter.AdjWidth(e.Step);
            NebLowerWidth.StringFormatFunc = (_) => WidthToString(Presenter.WidthByps);
            NebLowerWidth.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowerWidth);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.WidthByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                var (min, max) = Presenter.GetWidthRange();
                nkf.SetKeyBoardValue(LblLowerWidth.Text, QuantityUnit.Second.ToUnitString(), 10, onokclickeventaction,
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

        private void LoadSourceOne()
        {
            //if(CbxSourceOne.DataSource==null|| CbxSourceOne.DataSource.Count==0)
            //{
                CbxSourceOne.DataSource = ChannelIdExt.GetAnalogs().Where(x=>x!=Presenter.SourceTwo).Select(o => new ComboBoxItem(o.ToString(), o)).ToList();
            //}
            if(Presenter.SourceOne!=(ChannelId)CbxSourceOne.SelectValue)
            {
                CbxSourceOne.SelectValue = Presenter.SourceOne;
            }
        }

        private void LoadSourceTwo()
        {
            //if(CbxSourceTwo.DataSource == null || CbxSourceTwo.DataSource.Count == 0)
            //{
                CbxSourceTwo.DataSource = ChannelIdExt.GetAnalogs().Where(x=>x!=Presenter.SourceOne).Select(o => new ComboBoxItem(o.ToString(), o)).ToList();
            //}
            if (Presenter.SourceTwo != (ChannelId)CbxSourceTwo.SelectValue)
            {
                CbxSourceTwo.SelectValue = Presenter.SourceTwo;
            }
        }

        //private void SourceOneItemsVisiable()
        //{
        //    var items = (CbxSourceOne.DataSource as List<ComboBoxItem>).Where(x => (Int32)x.Value == (Int32)Presenter.SourceTwo || !x.Visible);
        //    foreach (var item in items)
        //    {
        //        if ((Int32)item.Value!= (Int32)Presenter.SourceTwo)
        //        {
        //            item.Visible = true;
        //        }
        //        else
        //        {
        //            item.Visible = false;
        //        }
        //    }
        //}

        //private void SourceTwoItemsVisiable()
        //{
        //    var items = (CbxSourceTwo.DataSource as List<ComboBoxItem>).Where(x => (Int32)x.Value == (Int32)Presenter.SourceOne || !x.Visible);
        //    foreach (var item in items)
        //    {
        //        if ((Int32)item.Value != (Int32)Presenter.SourceOne)
        //        {
        //            item.Visible = true;
        //        }   
        //        else
        //        {
        //            item.Visible = false;
        //        }
        //    }
        //}

        private void ChangeCtrlState()
        {
            if (Presenter.WidthCompCondition == PulseCondition.GreaterThan)
            {
                LblLowerWidth.Visible = true;
                NebLowerWidth.Visible = true;
                LblUpperWidth.Visible = false;
                NebUpperWidth.Visible = false;
            }
            else if (Presenter.WidthCompCondition == PulseCondition.LessThan)
            {
                LblLowerWidth.Visible = false;
                NebLowerWidth.Visible = false;
                LblUpperWidth.Visible = true;
                NebUpperWidth.Visible = true;
            }
            else
            {
                LblLowerWidth.Visible = true;
                NebLowerWidth.Visible = true;
                LblUpperWidth.Visible = true;
                NebUpperWidth.Visible = true;
            }
        }

        //private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.WidthCompCondition = (PulseCondition)CbxCondition.SelectedIndex;
        //    }
        //}

        //private void CbxSourceOne_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.SourceOne = Enum.Parse<ChannelId>(CbxSourceOne.SelectedItem.ToString());
        //        _ArgToCtrl = true;
        //        LoadSourceTwo();
        //        DsoPrsnt.FocusId = Presenter.SourceOne;
        //        _ArgToCtrl = false;
        //    }
        //}

        private void RdoSourceOneSlope_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SourceOneSlope = (EdgeSlope)RdoSourceOneSlope.ChoosedButtonIndex;
            }
        }

        //private void CbxSourceTwo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.SourceTwo = Enum.Parse<ChannelId>(CbxSourceTwo.SelectedItem.ToString());
        //        _ArgToCtrl = true;
        //        LoadSourceOne();
        //        DsoPrsnt.FocusId = Presenter.SourceTwo;
        //        _ArgToCtrl = false;
        //    }
        //}

        private void RdoSourceTwoSlope_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SourceTwoSlope = (EdgeSlope)RdoSourceTwoSlope.ChoosedButtonIndex;
            }
        }

        private String CompPosToString(Double position)
        {
            //return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7);
            }
            else
            {
                return new Quantity(position, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7);
            }
        }

        private static String WidthToString(Int64 width)
        {
            return new Quantity(width, Prefix.Pico, "s").ToString("##0.##########", true, 14);
        }

        private  void CbxSourceOne_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SourceOne =(ChannelId)CbxSourceOne.SelectValue;
                _ArgToCtrl = true;
                LoadSourceTwo();
             //   BeginInvoke(new Action(() => { DsoPrsnt.FocusId = Presenter.SourceOne; }));
                _ArgToCtrl = false;
            }
        }

        private void CbxSourceTwo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SourceTwo = (ChannelId)CbxSourceTwo.SelectValue;
                _ArgToCtrl = true;
                LoadSourceOne();
              //  BeginInvoke(new Action(() => { DsoPrsnt.FocusId = Presenter.SourceTwo; }));
                _ArgToCtrl = false;
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
