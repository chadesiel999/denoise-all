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
    using ScopeX.Controls.Common.Structs;
    

    public partial class TriggerSustainTimeSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;
        private SustainTimeSettingForm _SustainTimeSettingForm;

        public TriggerSustainTimeSubPage()
        {
            InitializeComponent();
            InitLanguage();
            Init();
        }

        private void InitLanguage()
        {

            LblCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoJian");
            BtnSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuSheZhi");
            LblSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuanXuanZeJiDianPingSheZhi");
            LblLowerWidth.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianXiaXian");
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

        public TrigSustainTimePrsnt Presenter
        {
            get => (TrigSustainTimePrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigSustainTimePrsnt)value;
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
                    //CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    //NebPositon.UpdateValueString();
                    break;
                case nameof(Presenter.Condition):
                    //CbxCondition.SelectedIndex = (Int32)Presenter.Condition;
                    CbxCondition.SelectValue = (Int32)Presenter.Condition;
                    ChangeCtrlState();
                    NebLowerWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
                    break;
                case nameof(Presenter.WidthByps):
                case nameof(Presenter.UpperWidthByps):
                    NebLowerWidth.UpdateValueString();
                    NebUpperWidth.UpdateValueString();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                //CbxCondition.SelectedIndex = (Int32)Presenter.Condition;
                CbxCondition.SelectValue = (Int32)Presenter.Condition;
                ChangeCtrlState();
                NebLowerWidth.UpdateValueString();
                NebUpperWidth.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void Init()
        {
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

        //private void InitSourceList()
        //{
        //    //CbxSource.Items.Clear();
        //    //CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
        //}

        private void ChangeCtrlState()
        {
            if (Presenter.Condition == PulseCondition.GreaterThan)
            {
                LblLowerWidth.Visible = true;
                NebLowerWidth.Visible = true;
                LblUpperWidth.Visible = false;
                NebUpperWidth.Visible = false;
            }
            else if (Presenter.Condition == PulseCondition.LessThan)
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
        //        Presenter.Condition = (PulseCondition)CbxCondition.SelectedIndex;
        //    }
        //}

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            _SustainTimeSettingForm = new SustainTimeSettingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                TrigPresenter = this.Presenter,
                ActiveBorderColor = AppStyleConfig.DefaultFormActiveBorderColor,
                ActiveBorderVisiable = true,
            };
            _SustainTimeSettingForm.TrigPresenter.TryAddView(_SustainTimeSettingForm);

            if (Program.Oscilloscope.TryGetChannel(ChannelId.D0, out var dch))
            {
                _SustainTimeSettingForm.DigiPresenter = (DigitalPrsnt)dch;
                _SustainTimeSettingForm.DigiPresenter.TryAddView(_SustainTimeSettingForm);
            }

            (ParentForm as TriggerForm).CanClose = false;

            //EventBus.EventBroker.Instance.GetEvent<FormShowDialogEventArgs>().Publish(this, new FormShowDialogEventArgs() { Current = _SustainTimeSettingForm });

            _SustainTimeSettingForm.ShowDialogByPosition();
            if(ParentForm is TriggerForm triggerform)
            {
                triggerform.CanClose = true;
            }
        }


        private static String WidthToString(Int64 width)
        {
            return new Quantity(width, Prefix.Pico, "s").ToString("##0.##########", true, 14);
        }

        private void CbxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Condition = (PulseCondition)CbxCondition.SelectValue;
            }
        }
    }
}
