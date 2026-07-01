// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.U2
{
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    public partial class TriggerEdgeSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;
        private Dictionary<ChannelId, double> _channel_set_pos = new Dictionary<ChannelId, double>();

        private void InitControlLang()
        {
            _ArgToCtrl = true;
            LblSlope.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BianYan");

            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();

            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            //radioButtonItem3.Icon = null;
            //radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem3.Tag = null;
            //radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYiYan");
            RdoSlope.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2/*, radioButtonItem3*/ });

            LblCoupling.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TriggerOuHe");
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianPing");

            CbxCoupling.Items = new string[] { "DC"};
            LblImpendance.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuZuKang");
            BtnResetPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWeiDao50Percent");
            LblEnableAtten.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WaiChuFaShuaiJian");
            ChkEnableAtten.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkEnableAtten.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            BtnResetLV.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            LblLevel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BuYong");
            _ArgToCtrl = false;
        }

        public TriggerEdgeSubPage()
        {
            InitializeComponent();
            InitSourceList();
            NebPosition.ValueChanged = new Action<object, (double oldValue, double newValue)>(NebPositionValueChangedHandler);
            InitControlLang();
            Init();
        }

        private void NebPositionValueChangedHandler(object arg1, (double oldValue, double newValue) tuple)
        {
            if (Presenter.Source == null)
                return;

            _channel_set_pos[Presenter.Source.Value] = Presenter.CompPosition;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public TrigEdgePrsnt Presenter
        {
            get => (TrigEdgePrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigEdgePrsnt)value;
        }

        private Boolean IsDigialSource => ((ChannelId)Presenter.Source).IsDigital();

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
            UpdateLocation();
            base.OnLoad(e);
            UpdateView();
        }

        private void UpdateLocation()
        {
            RdoImpedance.Location = new System.Drawing.Point(355, 101);
            LblImpendance.Location = new System.Drawing.Point(355, 73);
            BtnResetPosition.Location = new System.Drawing.Point(238, 177);
            LblPosition.Location = new System.Drawing.Point(10, 148);
            NebPosition.Location = new System.Drawing.Point(10, 177);
            LblCoupling.Location = new System.Drawing.Point(10, 267);
            CbxCoupling.Location = new System.Drawing.Point(10, 297);
            LblEnableAtten.Location = new System.Drawing.Point(376, 154);
            ChkEnableAtten.Location = new System.Drawing.Point(376, 177);
        }

        private void Init()
        {
            //NebPosition
            ControlsHotKnob.Default.InitHotKnob(NebPosition);
            NebPosition.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPosition);
            };
            NebPosition.StringFormatFunc = (_) => CompPosToString();
            NebPosition.AddClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPosition.SubClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPosition.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPosition);
                var onokclickeventaction = new Action<Double>((data) =>
                {
                    Presenter.VuCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix);
                    if (Presenter.Source != null)
                        _channel_set_pos[Presenter.Source.Value] = Presenter.VuCompPosition;
                });

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };
        }


        private void ChangeCtrlState()
        {
            if (Presenter.Source == ChannelId.Ext || Presenter.Source == ChannelId.Ext5)
            {
                //LblEnableAtten.Visible = true;
                //ChkEnableAtten.Visible = true;
                LblPosition.Visible = true;
                NebPosition.Visible = true;
                BtnResetPosition.Visible = true;

                LblSlope.Visible = true;
                RdoSlope.Visible = true;

                // 外触发在FPGA段不支持触发耦合功能，先屏蔽掉
                LblCoupling.Visible = true;
                CbxCoupling.Visible = true;
                LblImpendance.Visible = true;
                RdoImpedance.Visible = true;
                //ljw 23.11.1
                NebLevel.Visible = false;
                LblLevel.Visible = false;
                BtnResetLV.Visible = false;


                ControlsHotKnob.Default.InitHotKnob(NebLevel);
                NebLevel.EditValueOnceClicked += (a, b) =>
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, NebLevel);
                };
                NebLevel.StringFormatFunc = (_) => SensitivityToString();
                NebLevel.AddClicked = (_, e) => Presenter.SensitivityBymdiv += e.Step;
                NebLevel.SubClicked = (_, e) => Presenter.SensitivityBymdiv += e.Step;
                NebLevel.EditValueChicked = (a, b) =>
                {
                    var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLevel);

                    Action<double> onokclickeventaction = new Action<double>((data) =>
                    Presenter.SensitivityBymdiv = (int)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

                    nkf.SetKeyBoardValue(LblPosition.Text, QuantityUnit.Division.ToUnitString(), 2, onokclickeventaction,
                        Quantity.ConvertByPrefix(Presenter.SensitivityBymdiv, Prefix.Milli),
                        Quantity.ConvertByPrefix(Presenter.SensitivityMaxIndex, Prefix.Milli),
                        Quantity.ConvertByPrefix(Presenter.SensitivityMinIndex, Prefix.Milli));

                    nkf.ShowDialogByPosition();
                };
            }
            else if (Presenter.Source == ChannelId.AuxIn)
            {
                LblPosition.Visible = false;
                NebPosition.Visible = false;
                BtnResetPosition.Visible = false;
                LblImpendance.Visible = false;
                RdoImpedance.Visible = false;
                NebLevel.Visible = false;
                LblLevel.Visible = false;
                BtnResetLV.Visible = false;
                LblSlope.Visible = false;
                RdoSlope.Visible = false;
                LblCoupling.Visible = false;
                CbxCoupling.Visible = false;
            }
            else if (Presenter.Source == ChannelId.AC)
            {
                LblPosition.Visible = false;
                NebPosition.Visible = false;
                BtnResetPosition.Visible = false;
                LblImpendance.Visible = false;
                RdoImpedance.Visible = false;
                NebLevel.Visible = false;
                LblLevel.Visible = false;
                BtnResetLV.Visible = false;
                LblSlope.Visible = true;
                RdoSlope.Visible = true;
                LblCoupling.Visible = false;
                CbxCoupling.Visible = false;
                Presenter.Coupling = TriggerCoupling.DC;
            }
            else if (IsDigialSource)
            {
                LblPosition.Visible = false;
                NebPosition.Visible = false;
                BtnResetPosition.Visible = false;
                LblSlope.Visible = true;
                RdoSlope.Visible = true;
                LblCoupling.Visible = false;
                CbxCoupling.Visible = false;
                LblImpendance.Visible = false;
                RdoImpedance.Visible = false;
                NebLevel.Visible = false;
                LblLevel.Visible = false;
                BtnResetLV.Visible = false;
            }
            else
            {
                //LblEnableAtten.Visible = false;
                //ChkEnableAtten.Visible = false;
                LblPosition.Visible = true;
                NebPosition.Visible = true;
                BtnResetPosition.Visible = true;
                LblSlope.Visible = true;
                RdoSlope.Visible = true;
                LblCoupling.Visible = true;
                CbxCoupling.Visible = true;
                LblImpendance.Visible = false;
                RdoImpedance.Visible = false;
                NebLevel.Visible = false;
                LblLevel.Visible = false;
                BtnResetLV.Visible = false;
            }
            CbxCoupling.Enabled = false;
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
                    //selectComboxGroup1.SelectedIndex = selectComboxGroup1.FindString(Presenter.Source.ToString());
                    if (Presenter.Source != null && _channel_set_pos.ContainsKey(Presenter.Source.Value))
                        Presenter.CompPosition = _channel_set_pos[Presenter.Source.Value];
                    else
                        Presenter.CompPosition = 0;
                    if (Presenter.Source == ChannelId.AuxIn)
                    {
                        LblAuxInTip.Visible = true;
                    }
                    else
                    {
                        LblAuxInTip.Visible = false;
                    }
                    _ArgToCtrl = true;
                    if (Presenter.Source.ToString() == "Ext")
                    {
                        CbxSource.ChoosedButtonIndex =4;

                    }
                    else
                        CbxSource.ChoosedButtonIndex = (Int32)Presenter.Source;

                    //RbgSource.SelectedIndex = RbgSource.FindIndexInAllItem(Presenter.Source.ToString(), 0);
                    _ArgToCtrl = false;

                    //if (Presenter.Source == ChannelId.Ext5)
                    //{
                    //    if (RbgSource.SelectedIndex != 4)
                    //    {

                    //    }
                    //}
                    //else
                    //{
                    //    var index = RbgSource.FindIndexInAllItem(Presenter.Source.ToString());
                    //    if (RbgSource.SelectedIndex != index)
                    //    {
                    //        _ArgToCtrl = true;
                    //        RbgSource.SelectedIndex = index;
                    //        _ArgToCtrl = false;
                    //    }
                    //}
                    NebPosition.UpdateValueString();
                    NebLevel.UpdateValueString();
                    ChangeCtrlState();
                    break;
                case "CompPosIndex":
                    _channel_set_pos[Presenter.Source.Value] = Presenter.CompPosition;
                    NebPosition.UpdateValueString();
                    break;
                case nameof(Presenter.SensitivityBymdiv):
                    NebLevel.UpdateValueString();
                    break;
                case nameof(Presenter.Slope):
                    RdoSlope.ChoosedButtonIndex = (Int32)Presenter.Slope;
                    break;

                case nameof(Presenter.Coupling):
                    //CbxCoupling.SelectedIndex = (Int32)Presenter.Coupling;
                    CbxCoupling.SelectValue = (Int32)Presenter.Coupling;
                    //CbxCoupling.Text = Presenter.Coupling.ToString();
                    break;
                case nameof(Presenter.Impedance):
                    RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                    ChangeCtrlState();
                    break;
                case nameof(TriggerPrsnt.EnableExtAtten):
                    ChkEnableAtten.Checked = TriggerPrsnt.EnableExtAtten;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                // 避免多次注册事件，造成的事件多次触发。
                CbxSource.IndexChanged -= RbgSource_SelectedValueChanged;
                CbxSource.IndexChanged += RbgSource_SelectedValueChanged;

                if (Presenter.Source.ToString() == "Ext")
                {
                    CbxSource.ChoosedButtonIndex = 4;
                }
                else
                    CbxSource.ChoosedButtonIndex =(Int32)Presenter.Source;

                if (Presenter.Source == ChannelId.AuxIn)
                {
                    LblAuxInTip.Visible = true;
                }
                else
                {
                    LblAuxInTip.Visible = false;
                }

                //RbgSource.SelectedIndex = RbgSource.FindIndexInAllItem(Presenter.Source.ToString());

                RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                //CbxCoupling.SelectedIndex = (Int32)Presenter.Coupling;
                CbxCoupling.SelectValue = (Int32)Presenter.Coupling;
                //CbxCoupling.Text = Presenter.Coupling.ToString();
                ChkEnableAtten.Checked = TriggerPrsnt.EnableExtAtten;
                RdoSlope.ChoosedButtonIndex = (Int32)Presenter.Slope;

                ChangeCtrlState();
                NebPosition.UpdateValueString();
                NebLevel.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
            switch (Presenter.Coupling)
            {
                case TriggerCoupling.AC:
                case TriggerCoupling.LFR:
                    Presenter.SetPosIndexCenterZero();
                    break;
                case TriggerCoupling.HFR:
                default:
                    Presenter.SetPosIndexCenter();
                    break;
            }
        }

        //private void CbxCoupling_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Coupling = (TriggerCoupling)CbxCoupling.SelectedIndex;
        //    }
        //}

        /// <summary>
        /// The RbgSource_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        //private void RbgSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        if (RbgSource.SelectedIndex == 4)
        //        {
        //            Presenter.Source = ChannelId.Ext5;
        //        }
        //        else
        //        {
        //            Presenter.Source = Enum.Parse<ChannelId>((String)RbgSource.SelectedItem);
        //        }

        //    }
        //}

        private void RdoImpedance_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Impedance = (TriggerImpedance)RdoImpedance.ChoosedButtonIndex;
            }
        }

        private void RdoSlope_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Slope = (EdgeSlope)RdoSlope.ChoosedButtonIndex;
            }
        }

        private void InitSourceList()
        {
            ////这里需要两个source 一个在UI直接显示的 只有Analog，一个是在UI中折叠显示的 包括Digital、Ext、AC、Auxin
            //var analogsource = PlatformUIManager.Default.Platform.GetTriggerSource().ToList();
            //var othersource = PlatformUIManager.Default.Platform.GetTriggerSource(false, true, false, false).Where(s => !s.IsAnalog()).ToList();
            ////othersource.Insert(0, ChannelId.Ext5);
            //analogsource.Add( ChannelId.Ext);
            //CbxSource.ButtonItems = analogsource.Select(x => x.ToString()).ToArray();
            //CbxSource.CbbItem = othersource.Select(x =>
            //  {
            //      if (x.IsDigital())
            //      {
            //          return "D" + (x - ChannelId.D0);
            //      }
            //      if (x == ChannelId.Ext5)
            //      {
            //          return ChannelId.Ext5.GetAlias();
            //      }
            //      return x.ToString();
            //  }).ToArray();
        }

        private void ChkEnableAtten_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TriggerPrsnt.EnableExtAtten = ChkEnableAtten.Checked;
            }
        }
        private String SensitivityToString()
        {
            //return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);

            return new Quantity(Presenter.SensitivityBymdiv, Presenter.PosPrefix, QuantityUnit.Division.ToUnitString()).ToString("#0.###", true);

        }
        private String CompPosToString()
        {
            //return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7); // 格式化字符串主要为了修复Bug：4249
            }
            else
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.####;-##0.####;0", true, 7);// 格式化字符串主要为了修复Bug：4249
            }
        }

        private void BtnResetLV_Click(object sender, EventArgs e)
        {
            Presenter.SensitivityBymdiv = 0;
        }

        private void CbxCoupling_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Coupling = (TriggerCoupling)CbxCoupling.SelectValue;
            }
        }


        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!_ArgToCtrl)
            {
                string s = (String)btn.Text;
                Presenter.Source = Enum.Parse<ChannelId>(s);
            }
        }

        private void RbgSource_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (CbxSource.ChoosedButtonIndex == 4)
                {
                    Presenter.Source = ChannelId.Ext;
                }
                else
                {
                    Presenter.Source = (ChannelId)(CbxSource.ChoosedButtonIndex);
                }

                if (Presenter.Source == ChannelId.Ext || Presenter.Source == ChannelId.Ext5)
                {
                    BtnResetPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWeiDaoZero");
                }
                else
                {
                    BtnResetPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWeiDao50Percent");
                }
            }
        }
    }
}
