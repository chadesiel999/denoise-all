// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    
    using ScopeX.UserControls.Style;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class TriggerVideoSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerVideoSubPage()
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

        public TrigVideoPrsnt Presenter
        {
            get => (TrigVideoPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigVideoPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        private void LoadLangText()
        {
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblPolarity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiXing");
            LblStandard.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhun");
            LblLineNumber.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XingShu");
            LblFieldNumber.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChangShu");

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
            LblAnyLine.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYiXingChuFa");
            BtnResetLine.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            BtnResetField.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            ChkAnyLine.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkAnyLine.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            LblSync.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongBu");
			CbxSync.Items= new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_VideoSync_Odd")/*"偶数场"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_VideoSync_Even")/*"奇数场"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_VideoSync_All")/*"所有行"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_VideoSync_Specified")/*"指定行"*/ };
            BtnResetPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            LblPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianPing");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadLangText();
            UpdateView();
        }

        public void UpdateView(Object prnst, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Line):
                    NebLine.UpdateValueString();
                    break;
                case nameof(Presenter.Field):
                    NebField.UpdateValueString();
                    break;
                case nameof(Presenter.Standard):
                    //CbxStandard.SelectedIndex = (Int32)Presenter.Standard;
                    CbxStandard.SelectValue = (Int32)Presenter.Standard;
                    break;
                case nameof(Presenter.Polarity):
                    RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                    break;
                case nameof(Presenter.Source):
                    //CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    CbxSource.SelectValue =Presenter.Source;
                    NebPositon.UpdateValueString();
                    break;
                case "CompPosIndex":
                    NebPositon.UpdateValueString();
                    break;
                case nameof(Presenter.Sync):
                    //ChkAnyLine.Checked = Presenter.Sync == VideoSync.All;
                    //CbxSync.SelectedIndex = (Int32)Presenter.Sync;
                    CbxSync.SelectValue = (Int32)Presenter.Sync;
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
                InitSourceList();
                CbxSource.SelectValue = Presenter.Source;
                CbxStandard.SelectValue = (Int32)Presenter.Standard;
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                CbxSync.SelectValue = (Int32)Presenter.Sync;
                ChangeCtrlState();
                NebPositon.UpdateValueString();
                NebLine.UpdateValueString();
                NebField.UpdateValueString();

                _ArgToCtrl = false;
            }
        }

        private void Init()
        {
            //NebLine
            ControlsHotKnob.Default.InitHotKnob(NebLine);
            NebLine.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLine);
            };
            NebLine.AddClicked = (o, e) => Presenter.Line++;
            NebLine.SubClicked = (o, e) => Presenter.Line--;
            NebLine.StringFormatFunc = (_) => Presenter.Line.ToString() + "#";
            NebLine.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLine);
                var onokclickeventaction = new Action<Double>((date) => Presenter.Line = Convert.ToInt16(date));
                nkf.SetKeyBoardValue(LblLineNumber.Text, "#", 3, onokclickeventaction, Presenter.Line, Presenter.MaxLine, Presenter.MinLine);

                nkf.ShowDialogByPosition();
            };

            //NebField
            ControlsHotKnob.Default.InitHotKnob(NebField);
            NebField.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebField);
            };
            NebField.AddClicked = (o, e) => Presenter.Field++;
            NebField.SubClicked = (o, e) => Presenter.Field--;
            NebField.StringFormatFunc = (_) => Presenter.Field.ToString() + "#";
            NebField.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebField);
                var onokclickeventaction = new Action<double>((date) => Presenter.Field = Convert.ToInt16(date));
                nkf.SetKeyBoardValue(LblFieldNumber.Text, "#", 0, onokclickeventaction, Presenter.Field, Presenter.MaxField, Presenter.MinField);

                nkf.ShowDialogByPosition();
            };
            //NebPositon
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

        }

        private void InitSourceList()
        {
            //CbxSource.Items.Clear();
            //CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            //CbxSource.Items.Add(ChannelId.Ext);
            List<ComboBoxItem> arrs =new List<ComboBoxItem>();
            arrs.AddRange(ChannelIdExt.GetAnalogs().Select(o => new ComboBoxItem( o.ToString(),o)).ToArray());
            // arrs.Add(new ComboBoxItem(ChannelId.Ext.ToString(), ChannelId.Ext));
            CbxSource.DataSource = arrs;
        }

        private void ChangeCtrlState()
        {
            //NebField.Enabled = NebLine.Enabled = BtnResetField.Enabled = BtnResetLine.Enabled = Presenter.Sync == VideoSync.Specified;
            if (Presenter.Sync == VideoSync.Specified)
            {
                LblLineNumber.Visible = true;
                NebLine.Visible = true;
                BtnResetLine.Visible = true;
            }
            else
            {
                LblLineNumber.Visible = false;
                NebLine.Visible = false;
                BtnResetLine.Visible = false;
            }
        }

        private void BtnFieldReset_Click(object sender, EventArgs e)
        {
            Presenter.Field = Presenter.MinField;
        }

        private void BtnLineReset_Click(object sender, EventArgs e)
        {
            Presenter.Line = Presenter.MinLine;
        }

        //private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
        //    }
        //}

        //private void CbxStandard_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Standard = (VideoStandard)CbxStandard.SelectedIndex;
        //    }
        //}

        private void ChkAnyLine_CheckedChanged(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.Sync = ChkAnyLine.Checked ? VideoSync.All : VideoSync.Specified;
            //}
        }

        //private void CbxSync_SelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Sync = (VideoSync)CbxSync.SelectedIndex;
        //    }
        //}

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (VideoPolarity)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private String CompPosToString()
        {
            //return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.000", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.######", true, 7);
            }
            else
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
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
                Presenter.Source = Enum.Parse< ChannelId >( CbxSource.SelectKey.ToString());
            }
        }

        private void CbxStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Standard = (VideoStandard)CbxStandard.SelectValue;
            }
        }

        private void CbxSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Sync = (VideoSync)CbxSync.SelectValue;
            }
        }
    }
}
