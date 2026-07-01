using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScottPlot.Drawing.Colormaps;
using System.Drawing;
using static ScopeX.UserControls.SelectComboBox;
using System.IO;

namespace ScopeX.U2.AWG
{
    public partial class AWGPage : UserControl, IWfmGenView, IStylize
    {
        private Boolean _ArgToCtrl;

        public AWGPage()
        {
            InitializeComponent();
            Init();
        }

        public void AWGPage_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Presenter.EnablePointByPoint && e.X >= 8 && e.X <= 180 && e.Y >= 54 && e.Y <= 184)
            {
                Int64 num = 1;
                if (Presenter.Frequency > 1_000)
                {
                    num = 1_000;
                }
                if (Presenter.Frequency > 1_000_000)
                {
                    num = 1_000_000;
                }
                if (Presenter.Frequency > 1_000_000_000)
                {
                    num = 1_000_000_000;
                }
                if (Presenter.Frequency > 1_000_000_000_000)
                {
                    num = 1_000_000_000_000;
                }
                Int32 count = Math.Abs(e.Delta / 120);
                if (e.Delta > 0)
                {
                    Presenter.Frequency += num * count;
                }
                else
                {
                    Presenter.Frequency -= num * count;
                }
            }
        }

        private void Init()
        {
            //<Remark>创建人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
            MouseWheel += new MouseEventHandler(AWGPage_MouseWheel);
            NebFreqControlInit();
            NebAmpControlInit();
            Neb1LevelControlInit();
            Neb0LevelControlInit();
            NebPeriodControlInit();
            NebOffsetControlInit();
            NebDutyControlInit();
            NebPulseRiseTimeControlInit();
            NebPulseFallTimeControlInit();

            NebAmpDepthControlInit();
            NebModFreqControlInit();
            NebFreqBiasControlInit();
            NebPhaseBiasControlInit();

            NebSweepStartFreqControlInit();
            NebSweepEndFreqControlInit();
            NebSweepDurationControlInit();

            NebNoiseControlInit();
            NebPhaseControlInit();
        }


        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public ArbWfmGenPrsnt Presenter
        {
            get => (ArbWfmGenPrsnt)(ParentForm as IWfmGenView).Presenter;
            set => (ParentForm as IWfmGenView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArbWfmGenPrsnt)value;
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
                case nameof(Presenter.Mode):
                    RdoMode.ChoosedButtonIndex = Presenter.Active ? (Int32)Presenter.Mode + 1 : 0;
                    ChangeCtrlState(true);
                    CbxSignal.SelectValue = Presenter.WfmType;
                    break;
                case nameof(Presenter.WfmType):
                    ChangeCtrlState();
                    CbxSignal.SelectValue = Presenter.WfmType;
                    NebFreq.UpdateValueString();
                    NebPeriod.UpdateValueString();
                    NebAmp.UpdateValueString();
                    NebOffset.UpdateValueString();
                    Neb0Level.UpdateValueString();
                    Neb1Level.UpdateValueString();
                    NebDuty.UpdateValueString();
                    break;
                case nameof(Presenter.Frequency):
                    NebFreq.UpdateValueString();
                    NebPeriod.UpdateValueString();
                    break;
                case nameof(Presenter.Amplitude):
                case nameof(Presenter.Offset):
                case nameof(Presenter.LowLevel):
                case nameof(Presenter.HighLevel):
                    NebAmp.UpdateValueString();
                    NebOffset.UpdateValueString();
                    Neb0Level.UpdateValueString();
                    Neb1Level.UpdateValueString();
                    break;
                case nameof(Presenter.Duty):
                    NebDuty.UpdateValueString();
                    break;


                case nameof(Presenter.ModMethod):
                    //CbxModMethod.SelectedIndex = (Int32)Presenter.ModMethod;
                    CbxModMethod.SelectValue = Presenter.ModMethod;
                    ChangeCtrlState();
                    break;
                case nameof(Presenter.ModulatedWfm):
                    //CbxModulatedSignal.SelectedValue = Presenter.ModulatedWfm;
                    CbxModulatedSignal.SelectValue = Presenter.ModulatedWfm;
                    ChangeCtrlState();

                    NebModFreq.UpdateValueString();
                    break;
                case nameof(Presenter.RampType):
                    RdoModRamp.ChoosedButtonIndex = (Int32)Presenter.RampType;
                    break;
                case nameof(Presenter.ModFreq):
                    NebModFreq.UpdateValueString();
                    break;
                case nameof(Presenter.AmpDepth):
                    NebAmpDepth.UpdateValueString();
                    break;
                case nameof(Presenter.FreqBias):
                    NebFreqBias.UpdateValueString();
                    break;
                case nameof(Presenter.PhaseBias):
                    NebPhaseBias.UpdateValueString();
                    break;

                case nameof(Presenter.SweepStartFreq):
                    NebSweepStartFreq.UpdateValueString();
                    break;
                case nameof(Presenter.SweepEndFreq):
                    NebSweepEndFreq.UpdateValueString();
                    break;
                case nameof(Presenter.SweepDuration):
                    NebSweepDuration.UpdateValueString();
                    break;
                case nameof(Presenter.SweepType):
                    //CbxSweepType.SelectedIndex = (Int32)Presenter.SweepType;
                    CbxSweepType.SelectValue = (Int32)Presenter.SweepType;
                    break;

                case nameof(Presenter.Phase):
                    NebPhase.UpdateValueString();
                    break;
                case nameof(Presenter.Noise):
                    NebNoise.UpdateValueString();
                    break;
                case nameof(Presenter.Impedance):
                    RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                    NebAmp.UpdateValueString();
                    NebOffset.UpdateValueString();
                    Neb0Level.UpdateValueString();
                    Neb1Level.UpdateValueString();
                    break;
                case nameof(Presenter.Opposition):
                    ChkOpposition.Checked = Presenter.Opposition;
                    break;
                case nameof(Presenter.Active):
                    RdoMode.ChoosedButtonIndex = Presenter.Active ? (Int32)Presenter.Mode + 1 : 0;
                    BtnTrig.Visible = Presenter.Mode == WfmGenMode.Sweep && Presenter.WfmGenTriger == TriggerSource.Manual;
                    break;
                case nameof(Presenter.WfmGenTriger):
                    CbTriggerSource.SelectValue = (int)Presenter.WfmGenTriger;
                    break;
                case nameof(Presenter.TirgerOutEnabel):
                    ChkGroupTrigOut.Checked = Presenter.TirgerOutEnabel;
                    break;
                case nameof(Presenter.PulseRiseTime):
                    NebPulseRiseTime.UpdateValueString();
                    break;
                case nameof(Presenter.PulseFallTime):
                    NebPulseFallTime.UpdateValueString();
                    break;
                case nameof(Presenter.EnablePointByPoint):
                    RdoPointbypoint.ChoosedButtonIndex = Presenter.EnablePointByPoint ? 1 : 0;
                    break;
                case nameof(Presenter.FilePath):
                    BtnArbitraryRefreshTxt();
                    break;
                case nameof(Presenter.ModFilePath):
                    BtnModArbRefreshTxt();
                    break;
            }
            _ArgToCtrl = false;
        }

        private void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChangeCtrlState(true);


                RdoMode.ChoosedButtonIndex = Presenter.Active ? (Int32)Presenter.Mode + 1 : 0;
                //CbxSignal.SelectedValue = Presenter.WfmType;
                CbxSignal.SelectValue = Presenter.WfmType;
                NebFreq.UpdateValueString();
                NebPeriod.UpdateValueString();
                NebAmp.UpdateValueString();
                NebOffset.UpdateValueString();
                Neb0Level.UpdateValueString();
                Neb1Level.UpdateValueString();
                NebDuty.UpdateValueString();

                //CbxModMethod.SelectedIndex = (Int32)Presenter.ModMethod;
                CbxModMethod.SelectValue = Presenter.ModMethod;
                // CbxModulatedSignal.SelectedValue = Presenter.ModulatedWfm;
                CbxModulatedSignal.SelectValue = Presenter.ModulatedWfm;
                RdoModRamp.ChoosedButtonIndex = (Int32)Presenter.RampType;
                NebModFreq.UpdateValueString();
                NebFreqBias.UpdateValueString();
                NebAmpDepth.UpdateValueString();
                NebPhaseBias.UpdateValueString();

                //CbxSweepType.SelectedIndex = (Int32)Presenter.WfmType;
                //CbxSweepType.SelectedIndex = (Int32)Presenter.SweepType;
                CbxSweepType.SelectValue = (Int32)Presenter.SweepType;
                //CbxSweepType.Text = CbxSweepType.Items[(Int32)Presenter.SweepType];
                NebSweepStartFreq.UpdateValueString();
                NebSweepEndFreq.UpdateValueString();
                NebSweepDuration.UpdateValueString();

                ChkOpposition.Checked = Presenter.Opposition;
                RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                NebNoise.UpdateValueString();
                NebPhase.UpdateValueString();
                if (Presenter.Mode != WfmGenMode.Sweep)
                {
                    BtnTrig.Visible = false;
                    CbTriggerSource.Visible = false;
                    LbTriggerSource.Visible = false;
                }
                else
                {
                    BtnTrig.Visible = Presenter.WfmGenTriger == TriggerSource.Manual;
                }
                CbTriggerSource.SelectValue = (int)Presenter.WfmGenTriger;
                ChkGroupTrigOut.Checked = Presenter.TirgerOutEnabel;
                NebPulseRiseTime.UpdateValueString();
                NebPulseFallTime.UpdateValueString();

                RdoPointbypoint.ChoosedButtonIndex = Presenter.EnablePointByPoint ? 1 : 0;

                BtnArbitraryRefreshTxt();

                BtnModArbRefreshTxt();
                _ArgToCtrl = false;
            }
        }
        private void InitControlLang()
        {
            LblSampling.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiYangLv");
            LbSymmet.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuiChengXing");
            LB_pointbypoint.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhuDianShuChu");
            LbPulseFallTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            LbPulseRiseTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            LblArbitrary.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYiBo");
            LblDuty.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhanKongBi");
            Lbl0Level.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiDianPing");
            LblOffset.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PianZhi");
            LblPeriod.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhouQi");
            Lbl1Level.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GaoDianPing");
            LblAmplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuDu");
            LblFreq.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv");
            LblSignal.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingLeiXing");
            LblResis.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChu");
            LblNoise.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuJiaZaoSheng");
            LblPhase.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiShiXiangWei");
            LblOpposition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FanXiang");
            ChkOpposition.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkOpposition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            LblImpedance.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuZaiZuKang");

            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKai");
            RdoPointbypoint.ButtonItems = new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 };

            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem5 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem6 = new UserControls.RadioButtonItem();
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LianXuBo");
            radioButtonItem5.Icon = null;
            radioButtonItem5.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem5.Tag = null;
            radioButtonItem5.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiBo");
            radioButtonItem6.Icon = null;
            radioButtonItem6.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem6.Tag = null;
            radioButtonItem6.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SaoPin");
            RdoMode.ButtonItems = new UserControls.RadioButtonItem[] { radioButtonItem3, radioButtonItem4, radioButtonItem5, radioButtonItem6 };


            UserControls.RadioButtonItem radioButtonItem9 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem10 = new UserControls.RadioButtonItem();
            radioButtonItem9.Icon = null;
            radioButtonItem9.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem9.Tag = null;
            radioButtonItem9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangSheng");
            radioButtonItem10.Icon = null;
            radioButtonItem10.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem10.Tag = null;
            radioButtonItem10.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiang");
            RdoModRamp.ButtonItems = new UserControls.RadioButtonItem[] { radioButtonItem9, radioButtonItem10 };

            LblModArb.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYiBo");
            LblModRamp.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XieBoLeiXing_1");
            LblModMethod.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiLeiXing");
            LblModulatedSignal.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiBoXing");
            LblModParameter.Tag = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinPian_DiaoZhiShenDu_XiangPian");
            LblModParameter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinPian");
            LblModFreq.Tag = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiPinLv_TiaoYuePinLv");
            LblModFreq.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiPinLv");
            BtnTrig.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShouDongChuFa");
            LbTriggerSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuFaYuan");
            CbTriggerSource.Items = PlatformUIManager.Default.Platform.GetAwgTriggerSource();
            CbTriggerSource.SelectedIndexChanged += CbTriggerSource_SelectedIndexChanged;


            //UserControls.RadioButtonItem radioButtonItem11 = new UserControls.RadioButtonItem();
            //UserControls.RadioButtonItem radioButtonItem12 = new UserControls.RadioButtonItem();
            //radioButtonItem11.Icon = null;
            //radioButtonItem11.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem11.Tag = null;
            //radioButtonItem11.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            //radioButtonItem12.Icon = null;
            //radioButtonItem12.Padding = new System.Windows.Forms.Padding(0);
            //radioButtonItem12.Tag = null;
            //radioButtonItem12.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            //ChkGroupTrigOut.ButtonItems = new UserControls.RadioButtonItem[] { radioButtonItem11, radioButtonItem12 };

            ChkGroupTrigOut.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkGroupTrigOut.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            LbTrigOut.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuFaShuChu");
            LblSweepType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SaoPinLeiXing");
            LblSweepDuration.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SaoPinShiJian");
            LblSweepEndFreq.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieShuPinLv");
            LblSweepStartFreq.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiShiPinLv");

            //CbxSweepType.Items.Clear();
            //CbxSweepType.Items.AddRange(new object[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianXing"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuiShu") });

            CbxSweepType.Items = new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianXing"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuiShu") };
            CbxSweepType.SelectedIndexChanged += CbxSweepType_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitControlLang();
            LoadModMethodList();
            LoadModulatedSignalList();
            UpdateView();
        }

        private void LoadSignalList(IEnumerable<ArbWfmType> types, ArbWfmType arbWfmType)
        {
            CbxSignal.SelectedIndexChanged -= CbxSignal_SelectedIndexChanged;
            CbxSignal.DataSource = types.Select(x => new ComboBoxItem(x.GetDescription_Lang()/*EnumEx.GetDescription(x)*/, x, null)).ToList();
            Presenter.WfmType = arbWfmType;
            CbxSignal.SelectValue = arbWfmType;
            CbxSignal.SelectedIndexChanged += CbxSignal_SelectedIndexChanged;
            //    (_, _) =>
            //{
            //    if (!_ArgToCtrl)
            //    {
            //        Presenter.WfmType = (ArbWfmType)CbxSignal.SelectedValue;
            //        if ((ArbWfmType)CbxSignal.SelectedValue != ArbWfmType.Arbitrary)
            //        {
            //            BtnArbitrary.Text = "...";
            //        }
            //    }
            //};
        }

        public void CbxSignal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.WfmType = (ArbWfmType)CbxSignal.SelectValue;
                if ((ArbWfmType)CbxSignal.SelectValue != ArbWfmType.Arbitrary)
                {
                    BtnArbitrary.Text = "...";
                }
                else
                {
                    BtnArbitraryRefreshTxt();
                }
            }
        }

        private void ChangeCtrlState(bool changeMode = false)
        {
            //!!!We donot think about the input impedence, so output impedence doesnot changed.
            RdoImpedance.Enabled = DsoPrsnt.DataSrcOpt == DataSourceOpt.PCIe;
            BtnArbitrary.Visible = LblArbitrary.Visible = Presenter.WfmType == ArbWfmType.Arbitrary;
            //RdoPointbypoint.Visible = LB_pointbypoint.Visible = Presenter.WfmType == ArbWfmType.Arbitrary && Presenter.Mode != WfmGenMode.Sweep;
            RdoPointbypoint.Visible = false;
            //<Remark>更改人：彭博 创建日期：2023/11/28 11:48:00  原因：当未选择任意波时，关闭逐点输出 </Remark>
            Presenter.EnablePointByPoint = Presenter.WfmType != ArbWfmType.Arbitrary ? false : RdoPointbypoint.ChoosedButtonIndex == 1 ? true : Presenter.EnablePointByPoint;

            //<Remark>更改人：彭博 创建日期：2023/11/30 11:39:00  原因：最新技术手册提出，方波也可设置占空比 </Remark>
            //NebDuty.Visible = Presenter.WfmType == ArbWfmType.Pulse || Presenter.WfmType == ArbWfmType.Ramp;
            NebDuty.Visible = Presenter.WfmType == ArbWfmType.Pulse || Presenter.WfmType == ArbWfmType.Ramp || Presenter.WfmType == ArbWfmType.Square;
            LbSymmet.Visible = Presenter.WfmType == ArbWfmType.Ramp;
            //LblDuty.Visible = Presenter.WfmType == ArbWfmType.Pulse;
            LblDuty.Visible = Presenter.WfmType == ArbWfmType.Pulse || Presenter.WfmType == ArbWfmType.Square;

            NebPulseRiseTime.Visible = NebPulseFallTime.Visible = LbPulseRiseTime.Visible = LbPulseFallTime.Visible = Presenter.WfmType == ArbWfmType.Pulse;
            BtnModArb.Visible = LblModArb.Visible = Presenter.ModulatedWfm == ArbWfmType.Arbitrary;

            LblModRamp.Visible = RdoModRamp.Visible = Presenter.ModulatedWfm == ArbWfmType.Ramp;

            NebFreq.Visible = LblPhase.Visible = NebPhase.Visible = Presenter.Mode != WfmGenMode.Sweep && Presenter.WfmType != ArbWfmType.Noise && Presenter.WfmType != ArbWfmType.DC;

            LblFreq.Visible = !Presenter.EnablePointByPoint && Presenter.Mode != WfmGenMode.Sweep && Presenter.WfmType != ArbWfmType.Noise && Presenter.WfmType != ArbWfmType.DC;
            //<Remark>更改人：彭博 创建日期：2023/11/28 10:22:00  原因：当选择直流和扫频时，采样率不应显示 </Remark>
            LblSampling.Visible = Presenter.EnablePointByPoint && Presenter.WfmType != ArbWfmType.DC && Presenter.Mode != WfmGenMode.Sweep;
            //LblSampling.Visible = Presenter.EnablePointByPoint;

            LblPeriod.Visible = NebPeriod.Visible = !Presenter.EnablePointByPoint && Presenter.Mode != WfmGenMode.Sweep && Presenter.WfmType != ArbWfmType.Noise && Presenter.WfmType != ArbWfmType.DC;

            //LblNoise.Visible = NebNoise.Visible = Presenter.WfmType != ArbWfmType.Noise;
            LblNoise.Visible = false;
            NebNoise.Visible = false;
            LblAmplitude.Visible = NebAmp.Visible
                = Lbl1Level.Visible = Neb1Level.Visible = Lbl0Level.Visible = Neb0Level.Visible = Presenter.WfmType != ArbWfmType.DC;

            if (changeMode)
            {
                switch (Presenter.Mode)
                {
                    case WfmGenMode.Continuous:
                        LoadSignalList(Enum.GetValues<ArbWfmType>(), Presenter.ContinuousArbWfmType);
                        break;
                    case WfmGenMode.Modulation:
                        LoadSignalList(Presenter.CarrierSignalList, Presenter.ModulationWfmType);
                        break;
                    case WfmGenMode.Sweep:
                        LoadSignalList(Presenter.SweepSignalList, Presenter.SweepWfmType);
                        BtnTrig.Visible = Presenter.WfmGenTriger == TriggerSource.Manual;
                        CbTriggerSource.Visible = LbTriggerSource.Visible = true;
                        break;
                }
            }

            switch (Presenter.Mode)
            {
                case WfmGenMode.Modulation:

                    PnlModOption.Visible = true;
                    PnlSweepOption.Visible = false;
                    TlpAwg.RowStyles[1].Height = 170;
                    TlpAwg.RowStyles[2].Height = 0;
                    break;
                case WfmGenMode.Sweep:

                    PnlModOption.Visible = false;
                    PnlSweepOption.Visible = true;
                    TlpAwg.RowStyles[2].Height = 170;
                    TlpAwg.RowStyles[1].Height = 0;
                    break;
                default:
                    TlpAwg.RowStyles[2].Height = 170;
                    TlpAwg.RowStyles[1].Height = 0;
                    PnlModOption.Visible = false;
                    PnlSweepOption.Visible = false;
                    break;
            }

            switch (Presenter.ModMethod)
            {
                case WfmModMethod.FSK:
                    LblModFreq.Text = (LblModFreq.Tag as String).Split('#')[1];
                    break;
                case WfmModMethod.FM:
                    LblModFreq.Text = (LblModFreq.Tag as String).Split('#')[0];
                    LblModParameter.Text = (LblModParameter.Tag as String).Split('#')[0];
                    NebFreqBias.Visible = true;
                    NebAmpDepth.Visible = false;
                    NebPhaseBias.Visible = false;
                    NebFreqBias.BringToFront();
                    NebFreqBias.UpdateValueString();
                    break;
                case WfmModMethod.AM:
                    LblModFreq.Text = (LblModFreq.Tag as String).Split('#')[0];
                    LblModParameter.Text = (LblModParameter.Tag as String).Split('#')[1];
                    NebFreqBias.Visible = false;
                    NebAmpDepth.Visible = true;
                    NebPhaseBias.Visible = false;
                    NebAmpDepth.BringToFront();
                    NebAmpDepth.UpdateValueString();
                    break;
                case WfmModMethod.PM:
                    LblModFreq.Text = (LblModFreq.Tag as String).Split('#')[0];
                    LblModParameter.Text = (LblModParameter.Tag as String).Split('#')[2];
                    NebFreqBias.Visible = false;
                    NebAmpDepth.Visible = false;
                    NebPhaseBias.Visible = true;
                    NebPhaseBias.BringToFront();
                    NebPhaseBias.UpdateValueString();
                    break;
            }
        }

        private void LoadModMethodList()
        {
            CbxModMethod.DataSource = Enum.GetValues<WfmModMethod>().Where(w => w != WfmModMethod.FSK).Select(w => new ComboBoxItem(w.ToString(), (object)w, null)).ToList();
            //selectTouch2.SelectValue = Enum.GetValues<WfmModMethod>().Where(w => w != WfmModMethod.FSK).Select(w => new ComboBoxItem(w.ToString(), (object)w)).ToList()[0].Value;
            CbxModMethod.SelectValue = Presenter.ModMethod;
            CbxModMethod.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.ModMethod = (WfmModMethod)CbxModMethod.SelectValue;
                }
            };
        }

        private void LoadModulatedSignalList()
        {
            CbxModulatedSignal.DataSource = Presenter.ModulatedSignalList.Select(x => new { Key = x.GetDescription_Lang(), Value = (object)x }).Select(x => new ComboBoxItem(x.Key, x.Value, null)).ToList();
            CbxModulatedSignal.SelectValue = Presenter.ModulatedWfm;
            CbxModulatedSignal.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.ModulatedWfm = (ArbWfmType)CbxModulatedSignal.SelectValue;
                    if ((ArbWfmType)CbxModulatedSignal.SelectValue != ArbWfmType.Arbitrary)
                    {
                        BtnModArb.Text = "...";
                    }
                }
            };
        }

        public void NebAmpDepthControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebAmpDepth);
            NebAmpDepth.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmpDepth, nameof(Presenter.AmpDepth));
            };
            NebAmpDepth.AddClicked = (_, e) => Presenter.AdjAmpDepth(e.Step);
            NebAmpDepth.SubClicked = (_, e) => Presenter.AdjAmpDepth(e.Step);
            NebAmpDepth.StringFormatFunc = (_) => AmpDepthToString();
            NebAmpDepth.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpDepth);
                nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                {
                    Presenter.AmpDepth = (Int32)Math.Round(data);
                });


                nkf.SetKeyBoardValue(LblModParameter.Text, "%", 3, onokclickeventaction,
                    Presenter.AmpDepth, Presenter.MaxAmpDepth, Presenter.MinAmpDepth);

                nkf.ShowDialogByPosition();
            };
        }

        public void NebPhaseBiasControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPhaseBias);
            NebPhaseBias.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPhaseBias, nameof(Presenter.PhaseBias));

            };
            NebPhaseBias.AddClicked = (_, e) => Presenter.AdjPhaseBias(e.Step);
            NebPhaseBias.SubClicked = (_, e) => Presenter.AdjPhaseBias(e.Step);
            NebPhaseBias.StringFormatFunc = (_) => PhaseBiasToString();
            NebPhaseBias.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPhaseBias);
                nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.PhaseBias = (Int32)Math.Round(data * 100));

                nkf.SetKeyBoardValue(LblModParameter.Text, "°", 3, onokclickeventaction,
                    Presenter.PhaseBias / 100.0, Presenter.MaxPhase / 100.0, Presenter.MinPhase / 100.0);

                nkf.ShowDialogByPosition();

            };
        }
        private void NebFreqBiasEditValue_Clicked(object sender, double e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFreqBias);
            //nkf.NumberKeyboard.UseSI = false;
            var onokclickeventaction = new Action<Double>((data) =>
                Presenter.FreqBias = (Int64)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro));

            long freqDiffer = Math.Abs(Presenter.MaxFrequency - Presenter.Frequency);

            long maxFreqBias = freqDiffer < Presenter.Frequency ? freqDiffer : Presenter.Frequency;
            long minFreqBias = Presenter.MinFreqBias;
            nkf.SetKeyBoardValue(LblModParameter.Text, "Hz", 3, onokclickeventaction,
                Quantity.ConvertByPrefix(Presenter.FreqBias, Prefix.Micro),
                Quantity.ConvertByPrefix(maxFreqBias, Prefix.Micro),
                Quantity.ConvertByPrefix(minFreqBias, Prefix.Micro));

            nkf.ShowDialogByPosition();
        }
        public void NebFreqBiasControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebFreqBias);
            NebFreqBias.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebFreqBias, nameof(Presenter.FreqBias));

            };
            NebFreqBias.AddClicked = (_, e) => Presenter.AdjFreqBias(e.Step);
            NebFreqBias.SubClicked = (_, e) => Presenter.AdjFreqBias(e.Step);
            NebFreqBias.StringFormatFunc = (_) => FreqBiasToString();
            NebFreqBias.EditValueChicked = (_, e) => NebFreqBiasEditValue_Clicked(_, e);
        }

        public void NebModFreqControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebModFreq);
            NebModFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebModFreq, nameof(Presenter.ModFreq));

            };
            NebModFreq.AddClicked = (_, e) => Presenter.AdjModFreq(e.Step);
            NebModFreq.SubClicked = (_, e) => Presenter.AdjModFreq(e.Step);
            NebModFreq.StringFormatFunc = (_) => ModFreqToString();
            NebModFreq.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebModFreq);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.ModFreq = (Int64)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro));

                nkf.SetKeyBoardValue(LblModFreq.Text, "Hz", 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.ModFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MaxModFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MinModFreq, Prefix.Micro));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebNoiseControlInit()
        {
            //此版本FPGA暂不支持基波噪声 ljw 5.29
            NebNoise.Enabled = false;
            NebNoise.Visible = false;
            LblNoise.Visible = false;
            return;
            NebNoise.AddClicked = (_, e) => Presenter.AdjNoise(e.Step);
            NebNoise.SubClicked = (_, e) => Presenter.AdjNoise(e.Step);
            NebNoise.StringFormatFunc = (_) => NoiseToString();
            NebNoise.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebNoise);
                nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.Noise = Convert.ToInt32(data * 100.0));

                nkf.SetKeyBoardValue(LblNoise.Text, "%", 3, onokclick,
                    Presenter.Noise / 100.0, Presenter.MaxNoise / 100.0, Presenter.MinNoise / 100.0);

                nkf.ShowDialogByPosition();
            };
        }

        private void NebPhaseControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPhase);
            NebPhase.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPhase, nameof(Presenter.Phase));

            };
            NebPhase.AddClicked = (_, e) => Presenter.AdjPhase(e.Step);
            NebPhase.SubClicked = (_, e) => Presenter.AdjPhase(e.Step);
            NebPhase.StringFormatFunc = (_) => PhaseToString();

            NebPhase.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPhase);
                nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.Phase = Convert.ToInt32(data * 100.0));

                nkf.SetKeyBoardValue(LblPhase.Text, "°", 3, onokclick,
                    (Presenter.Opposition ? (Presenter.Phase + Presenter.HalfPhase) % 36000 : Presenter.Phase) / 100.0,
                    Presenter.MaxPhase / 100.0,
                    Presenter.MinPhase / 100.0);

                nkf.ShowDialogByPosition();
            };
        }

        private void NebFreqControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebFreq);
            NebFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebFreq, nameof(Presenter.Frequency));

            };
            NebFreq.AddClicked = (_, e) => Presenter.AdjFrequency(e.Step);
            NebFreq.SubClicked = (_, e) => Presenter.AdjFrequency(e.Step);
            NebFreq.StringFormatFunc = (_) => FreqToString();
            NebFreq.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFreq);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>(
                    (data) =>
                    {
                        Presenter.Frequency = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro));
                        //<Remark>更改人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
                        //if (Presenter.ModMethod == WfmModMethod.FM && Presenter.FreqBias > Presenter.Frequency)
                        // <Remark>作者：彭博 创建日期：2024/1/29 10:26:00 创建原因：频偏不能大于当前设置频率，已经在Core层设置 </Remark>
                        //if (Presenter.EnablePointByPoint && Presenter.ModMethod == WfmModMethod.FM && Presenter.FreqBias > Presenter.Frequency)
                        //{
                        //    Presenter.FreqBias = Presenter.Frequency;
                        //}
                    }
                    );
                //<Remark>更改人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
                if (Presenter.EnablePointByPoint)
                {
                    nkf.SetKeyBoardValue(LblSampling.Text, "Sa/s", 3, onokclick,
                        Quantity.ConvertByPrefix(Presenter.Frequency, Prefix.Micro),
                        Quantity.ConvertByPrefix(Presenter.MaxFrequency, Prefix.Micro),
                        Quantity.ConvertByPrefix(Presenter.MinFrequency, Prefix.Micro));
                }
                else
                {
                    nkf.SetKeyBoardValue(LblFreq.Text, "Hz", 3, onokclick,
                        Quantity.ConvertByPrefix(Presenter.Frequency, Prefix.Micro),
                        Quantity.ConvertByPrefix(Presenter.MaxFrequency, Prefix.Micro),
                        Quantity.ConvertByPrefix(Presenter.MinFrequency, Prefix.Micro));
                }

                nkf.ShowDialogByPosition();
            };
        }

        private void NebPeriodControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPeriod);
            NebPeriod.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPeriod, nameof(Presenter.Frequency));
            };
            NebPeriod.AddClicked = (_, e) => Presenter.AdjPeriod(e.Step);
            NebPeriod.SubClicked = (_, e) => Presenter.AdjPeriod(e.Step);
            NebPeriod.StringFormatFunc = (_) => PeriodToString();
            NebPeriod.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPeriod);
                //nkf.NumberKeyboard.UseSI = false;
                var onokclick = new Action<Double>((data) =>
                    Presenter.Frequency = (Int64)Math.Round(Quantity.ConvertByPrefix(Math.Round(1000 / data), Prefix.Empty, Prefix.Milli)));
                double minPeriod = 1.0 / Presenter.MaxFrequency;
                if (Presenter.WfmType == ArbWfmType.Sinusoid)
                {
                    minPeriod = 1.6666 * 1e-14;
                }
                nkf.SetKeyBoardValue(LblPeriod.Text, "s", 3, onokclick,
                    Quantity.ConvertByPrefix(1.0 / Presenter.Frequency, Prefix.Mega),
                    Quantity.ConvertByPrefix(1.0 / Presenter.MinFrequency, Prefix.Mega),
                    Quantity.ConvertByPrefix(minPeriod, Prefix.Mega));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebDutyControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebDuty);
            NebDuty.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebDuty, nameof(Presenter.Duty));
            };
            NebDuty.AddClicked = (_, e) => Presenter.AdjDuty(e.Step);
            NebDuty.SubClicked = (_, e) => Presenter.AdjDuty(e.Step);
            NebDuty.StringFormatFunc = (_) => DutyToString();
            NebDuty.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebDuty);
                nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.Duty = Convert.ToInt32(data * 100.0));

                nkf.SetKeyBoardValue(LblDuty.Text, "%", 3, onokclick,
                    Presenter.Duty / 100.0, Presenter.MaxDuty / 100.0, Presenter.MinDuty / 100.0);

                nkf.ShowDialogByPosition();
            };
        }
        private void NebPulseRiseTimeControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPulseRiseTime);
            NebPulseRiseTime.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPulseRiseTime, nameof(Presenter.PulseRiseTime));
            };
            NebPulseRiseTime.AddClicked = (_, e) => Presenter.AdjPulseRiseTime(e.Step);
            NebPulseRiseTime.SubClicked = (_, e) => Presenter.AdjPulseRiseTime(e.Step);
            NebPulseRiseTime.StringFormatFunc = (_) => PulseRiseTimeToString();
            NebPulseRiseTime.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPulseRiseTime);
                nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.PulseRiseTime = (Int64)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico));

                nkf.SetKeyBoardValue(LblOffset.Text, "s", 3, onokclick
                    , Quantity.ConvertByPrefix(Presenter.PulseRiseTime, Prefix.Pico, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MaxPulseEdgeTime, Prefix.Pico, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MinPulseEdgeTime, Prefix.Pico, Prefix.Empty));

                nkf.ShowDialogByPosition();
            };
        }
        private void NebPulseFallTimeControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPulseFallTime);
            NebPulseFallTime.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPulseFallTime, nameof(Presenter.PulseFallTime));
            };
            NebPulseFallTime.AddClicked = (_, e) => Presenter.AdjPulseFallTime(e.Step);
            NebPulseFallTime.SubClicked = (_, e) => Presenter.AdjPulseFallTime(e.Step);
            NebPulseFallTime.StringFormatFunc = (_) => PulseFallTimeToString();
            NebPulseFallTime.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPulseFallTime);
                nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.PulseFallTime = (Int64)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico));

                nkf.SetKeyBoardValue(LblOffset.Text, "s", 3, onokclick
                    , Quantity.ConvertByPrefix(Presenter.PulseFallTime, Prefix.Pico, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MaxPulseEdgeTime, Prefix.Pico, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MinPulseEdgeTime, Prefix.Pico, Prefix.Empty));

                nkf.ShowDialogByPosition();
            };
        }


        private QuantityUnit QuantityUnitByAmp = QuantityUnit.VoltagePeakPeak;
        private void NebAmpControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebAmp);
            NebAmp.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmp, nameof(Presenter.AmpliteudeValue));
            };
            NebAmp.AddClicked = (_, e) => Presenter.AdjAmplitude(e.Step);
            NebAmp.SubClicked = (_, e) => Presenter.AdjAmplitude(e.Step);
            NebAmp.StringFormatFunc = (_) => AmpToString();
            NebAmp.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordAwgForm().UniformInitKeyBoard(this, NebAmp);
                var onokclick = new Action<Double>((data) =>
                {
                    string unit = nkf.NumberKeyboard.UnitStr;
                    switch (nkf.NumberKeyboard.UnitStr)
                    {
                        case "mVrms":
                            Presenter.QuantityUnitByAmp = QuantityUnit.Vrms;
                            Presenter.AmplitudeVrms = data;
                            break;
                        case "Vrms":
                            data *= 1000;
                            Presenter.QuantityUnitByAmp = QuantityUnit.Vrms;
                            Presenter.AmplitudeVrms = data;
                            break;
                        case "dBm":
                            Presenter.QuantityUnitByAmp = QuantityUnit.dBm;
                            Presenter.AmplitudedBm = data;
                            break;
                        case "mVpp":
                            Presenter.QuantityUnitByAmp = QuantityUnit.VoltagePeakPeak;
                            Presenter.Amplitude = (Int32)data;
                            break;
                        case "Vpp":
                        default:
                            Presenter.QuantityUnitByAmp = QuantityUnit.VoltagePeakPeak;
                            data *= 1000;
                            Presenter.Amplitude = (Int32)data;
                            break;
                    }
                    NebAmp.UpdateValueString();
                    NebOffset.UpdateValueString();
                    Neb0Level.UpdateValueString();
                    Neb1Level.UpdateValueString();
                });
                string units = "Vpp";
                switch (Presenter.QuantityUnitByAmp)
                {
                    case QuantityUnit.VoltagePeakPeak:
                        break;
                    case QuantityUnit.Vrms:
                        units = "Vrms";
                        break;
                    case QuantityUnit.dBm:
                        units = "dBm";
                        break;
                }
                Double ampliteudevalue = Presenter.QuantityUnitByAmp != QuantityUnit.dBm ? Quantity.ConvertByPrefix(Presenter.AmpliteudeValue, Prefix.Milli, Prefix.Empty) : Presenter.AmpliteudeValue;
                Double maxamplitudebydbm = Presenter.Impedance == WfmGenImpedance.Low50 ? Presenter.MaxAmplitude : Presenter.MaxAmplitude / 2;
                Double minamplitudebydbm = Presenter.Impedance == WfmGenImpedance.Low50 ? Presenter.MinAmplitude : Presenter.MinAmplitude / 2;
                nkf.SetKeyBoardValue(LblAmplitude.Text, units, 3, onokclick,
                    ampliteudevalue,
                    Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty),
                    Math.Round(Quantity.ConvertByPrefix(Presenter.UnitConversionVppToVrms(Presenter.MaxAmplitude), Prefix.Milli, Prefix.Empty), 3),
                    Math.Round(Quantity.ConvertByPrefix(Presenter.UnitConversionVppToVrms(Presenter.MinAmplitude), Prefix.Milli, Prefix.Empty), 3),
                    Math.Round(Presenter.UnitConversionVppTodBm(maxamplitudebydbm * 1.0 / 1000), 3),
                    Math.Round(Presenter.UnitConversionVppTodBm(minamplitudebydbm * 1.0 / 1000), 3), true);

                nkf.ShowDialogByPosition();
            };
        }

        private void Neb1LevelControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(Neb1Level);
            Neb1Level.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, Neb1Level, nameof(Presenter.HighLevel));
            };
            Neb1Level.AddClicked = (_, e) => Presenter.AdjHighLevel(e.Step);
            Neb1Level.SubClicked = (_, e) => Presenter.AdjHighLevel(e.Step);
            Neb1Level.StringFormatFunc = (_) => HighLevelToString();
            Neb1Level.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, Neb1Level);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.HighLevel = Convert.ToInt32(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli)));

                nkf.SetKeyBoardValue(Lbl1Level.Text, "Vpp", 3, onokclick,
                    Quantity.ConvertByPrefix(Presenter.HighLevel, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MaxOffset, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MinOffset, Prefix.Milli, Prefix.Empty));
                nkf.NumberKeyboard.AbsoluteMinValue = Quantity.ConvertByPrefix(1, Prefix.Micro, Prefix.Empty);
                nkf.ShowDialogByPosition();
            };
        }

        private void Neb0LevelControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(Neb0Level);
            Neb0Level.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, Neb0Level, nameof(Presenter.LowLevel));
            };
            Neb0Level.AddClicked = (_, e) => Presenter.AdjLowLevel(e.Step);
            Neb0Level.SubClicked = (_, e) => Presenter.AdjLowLevel(e.Step);
            Neb0Level.StringFormatFunc = (_) => LowLevelToString();
            Neb0Level.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, Neb0Level);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.LowLevel = Convert.ToInt32(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli)));

                nkf.SetKeyBoardValue(Lbl0Level.Text, "Vpp", 3, onokclick,
                    Quantity.ConvertByPrefix(Presenter.LowLevel, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MaxOffset, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(Presenter.MinOffset, Prefix.Milli, Prefix.Empty));
                nkf.NumberKeyboard.AbsoluteMinValue = Quantity.ConvertByPrefix(1, Prefix.Micro, Prefix.Empty);
                nkf.ShowDialogByPosition();
            };
        }

        private void NebOffsetControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebOffset);
            NebOffset.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebOffset, nameof(Presenter.Offset));
            };
            NebOffset.AddClicked = (_, e) => Presenter.AdjOffset(e.Step);
            NebOffset.SubClicked = (_, e) => Presenter.AdjOffset(e.Step);
            NebOffset.StringFormatFunc = (_) => OffsetToString();
            NebOffset.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebOffset);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclick = new Action<Double>((data) =>
                    Presenter.Offset = (Int32)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));
                nkf.SetKeyBoardValue(LblOffset.Text, "V", 3, onokclick
                    , Quantity.ConvertByPrefix(Presenter.Offset, Prefix.Milli, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MaxOffset, Prefix.Milli, Prefix.Empty)
                    , Quantity.ConvertByPrefix(Presenter.MinOffset, Prefix.Milli, Prefix.Empty));
                nkf.NumberKeyboard.AbsoluteMinValue = Quantity.ConvertByPrefix(1, Prefix.Micro, Prefix.Empty);
                nkf.ShowDialogByPosition();
            };
        }

        private void NebSweepStartFreqControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebSweepStartFreq);
            NebSweepStartFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebSweepStartFreq, nameof(Presenter.SweepStartFreq));
            };
            NebSweepStartFreq.AddClicked = (_, e) => Presenter.AdjSweepStartFreq(e.Step);
            NebSweepStartFreq.SubClicked = (_, e) => Presenter.AdjSweepStartFreq(e.Step);
            NebSweepStartFreq.StringFormatFunc = (_) => SweepStartFreqToString();
            NebSweepStartFreq.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSweepStartFreq);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SweepStartFreq = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro)));

                nkf.SetKeyBoardValue(LblSweepStartFreq.Text, "Hz", 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.SweepStartFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MaxSweepFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MinSweepFreq, Prefix.Micro));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebSweepEndFreqControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebSweepEndFreq);
            NebSweepEndFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebSweepEndFreq, nameof(Presenter.SweepEndFreq));
            };
            NebSweepEndFreq.AddClicked = (_, e) => Presenter.AdjSweepEndFreq(e.Step);
            NebSweepEndFreq.SubClicked = (_, e) => Presenter.AdjSweepEndFreq(e.Step);
            NebSweepEndFreq.StringFormatFunc = (_) => SweepEndFreqToString();
            NebSweepEndFreq.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSweepEndFreq);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SweepEndFreq = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro)));

                nkf.SetKeyBoardValue(LblSweepEndFreq.Text, "Hz", 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.SweepEndFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MaxSweepFreq, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MinSweepFreq, Prefix.Micro));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebSweepDurationControlInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebSweepDuration);
            NebSweepDuration.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebSweepDuration, nameof(Presenter.SweepDuration));
            };
            NebSweepDuration.AddClicked = (_, e) => Presenter.AdjSweepDuration(e.Step);
            NebSweepDuration.SubClicked = (_, e) => Presenter.AdjSweepDuration(e.Step);
            NebSweepDuration.StringFormatFunc = (_) => SweepDurationToString();
            NebSweepDuration.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSweepDuration);
                //nkf.NumberKeyboard.UseSI = false;

                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SweepDuration = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro)));

                nkf.SetKeyBoardValue(LblSweepDuration.Text, "s", 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.SweepDuration, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MaxSweepDuration, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MinSweepDuration, Prefix.Micro));

                nkf.ShowDialogByPosition();

            };
        }

        private String FreqToString()
        {
            //<Remark>更改人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
            if (Presenter.EnablePointByPoint)
            {
                return SIHelper.ValueChangeToSI(Presenter.Frequency / 1000_000d, 3, "Sa/s");
            }
            else
            {
                if (Presenter.Frequency < 1000)
                {
                    return $"{Presenter.Frequency:F3} μHz";
                }
                return SIHelper.ValueChangeToSI(Presenter.Frequency / 1000_000d, 3, "Hz");
                //return new Quantity(Presenter.Frequency, Prefix.Micro, QuantityUnit.Hertz).ToString("##0.########", true, 11);
                //return new Quantity(Presenter.Frequency, Prefix.Micro, QuantityUnit.Hertz).ToString(8, -1, true);
            }
        }
        private String PulseRiseTimeToString()
        {
            return SIHelper.ValueChangeToSI(Presenter.PulseRiseTime / 1000_000_000_000d, 3, "s");
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString("##0.########", true, 11);
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString(8, -1, true);
        }
        private String PulseFallTimeToString()
        {
            return SIHelper.ValueChangeToSI(Presenter.PulseFallTime / 1000_000_000_000d, 3, "s");
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString("##0.########", true, 11);
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString(8, -1, true);
        }

        private String PeriodToString()
        {
            return SIHelper.ValueChangeToSI(1E15 / Presenter.Frequency / 1000_000_000d, 3, "s");
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString("##0.########", true, 11);
            //return new Quantity(1.0 / Presenter.Frequency, Prefix.Mega, QuantityUnit.Second).ToString(8, -1, true);
        }

        private String AmpToString()
        {
            //return SIHelper.ValueChangeToSI(Presenter.Amplitude / 1000f, 3, "V");
            Prefix prefix = Prefix.Milli;
            prefix = Presenter.QuantityUnitByAmp == QuantityUnit.dBm ? Prefix.Empty : prefix;
            int maxnumm = Presenter.QuantityUnitByAmp == QuantityUnit.dBm ? 7 : 3;
            return Presenter.QuantityUnitByAmp != QuantityUnit.dBm ? new Quantity(Presenter.AmpliteudeValue, prefix, Presenter.QuantityUnitByAmp).ToString("##0.000", true, 7) : $"{string.Format("{0:##0.000}", Presenter.AmpliteudeValue)} dBm";
        }

        private String OffsetToString()
        {
            //return Presenter.Offset == 0 ? "0.00V" : SIHelper.ValueChangeToSI(Presenter.Offset / 1000f, 3, "V");
            return new Quantity(Presenter.Offset, Prefix.Milli, QuantityUnit.Voltage).ToString("##0.000", true, 7);
        }

        private String HighLevelToString()
        {
            //return Presenter.HighLevel == 0 ? "0.00V" : SIHelper.ValueChangeToSI(Presenter.HighLevel / 1000f, 3, "V");
            return new Quantity(Presenter.HighLevel, Prefix.Milli, QuantityUnit.VoltagePeakPeak).ToString("##0.000", true, 7);
        }

        private String LowLevelToString()
        {
            //return Presenter.LowLevel == 0 ? "0.00V" : SIHelper.ValueChangeToSI(Presenter.LowLevel / 1000f, 3, "V");
            return new Quantity(Presenter.LowLevel, Prefix.Milli, QuantityUnit.VoltagePeakPeak).ToString("##0.000", true, 7);
        }

        private String DutyToString()
        {
            return (Presenter.Duty / 100.0).ToString(@"#0.00\%");
        }

        private String PhaseToString()
        {
            //return $"{(Presenter.Opposition ? (Presenter.Phase + Presenter.HalfPhase) % 36000 : Presenter.Phase) / 100f:F2}°";
            return (Presenter.Phase / 100.0).ToString("##0.00°");
        }

        private String NoiseToString()
        {
            return (Presenter.Noise / 100.0).ToString(@"##0.00\%");
        }

        private String ModFreqToString()
        {
            return SIHelper.ValueChangeToSI(Presenter.ModFreq / 1000_000d, 3, "Hz");
            //return new Quantity(Presenter.ModFreq, Prefix.Micro, "Hz").ToString("###,###,##0.#", true, 11);
        }

        private String FreqBiasToString()
        {
            //return SIHelper.ValueChangeToSI(Presenter.FreqBias / 1000_000f, 3, "Hz");
            return new Quantity(Presenter.FreqBias, Prefix.Micro, "Hz").ToString("###,###,##0.###", true, 13);
        }

        private String PhaseBiasToString()
        {
            //return $"{Presenter.PhaseBias / 1000_000f:F2}°";
            return (Presenter.PhaseBias / 100.0).ToString("##0.00°");
        }

        private String AmpDepthToString()
        {
            //return $"{Presenter.AmpDepth:F2}%";
            return Presenter.AmpDepth.ToString(@"#0\%");
        }

        private String SweepStartFreqToString()
        {
            //return SIHelper.ValueChangeToSI(Presenter.SweepStartFreq / 1000_000f, 0, "Hz");
            return new Quantity(Presenter.SweepStartFreq, Prefix.Micro, QuantityUnit.Hertz).ToString("###,###,##0.#", true, 11);
        }

        private String SweepDurationToString()
        {
            //return SIHelper.ValueChangeToSI(Presenter.SweepDuration / 1000_000f, 0, "s");
            return new Quantity(Presenter.SweepDuration, Prefix.Micro, QuantityUnit.Second).ToString("###,###,##0.###", true, 13);
        }

        private String SweepEndFreqToString()
        {
            //return SIHelper.ValueChangeToSI(Presenter.SweepEndFreq / 1000_000f, 0, "Hz");
            return new Quantity(Presenter.SweepEndFreq, Prefix.Micro, QuantityUnit.Hertz).ToString("###,###,##0.#", true, 11);
        }

        private void RdoMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (RdoMode.ChoosedButtonIndex == 0)
                {
                    Presenter.Active = false;
                    //LbTriggerSource.Visible = false;
                }
                else
                {
                    switch (Presenter.Mode)
                    {
                        case WfmGenMode.Continuous:
                            Presenter.ContinuousArbWfmType = (ArbWfmType)CbxSignal.SelectValue;
                            break;
                        case WfmGenMode.Modulation:
                            Presenter.ModulationWfmType = (ArbWfmType)CbxSignal.SelectValue;
                            break;
                        case WfmGenMode.Sweep:
                            Presenter.SweepWfmType = (ArbWfmType)CbxSignal.SelectValue;
                            break;
                    }
                    int ChoosedButtonIndex = RdoMode.ChoosedButtonIndex;
                    Presenter.Mode = (WfmGenMode)ChoosedButtonIndex - 1;
                    //<Remark>更改人：彭博 创建日期：2023/12/4 13:48:00  原因：这类波形不支持调制或扫频时，不能选择调制和扫频 </Remark>
                    //if (Presenter.Mode != (WfmGenMode)ChoosedButtonIndex - 1)
                    //{
                    //    if (((WfmGenMode)ChoosedButtonIndex - 1) == WfmGenMode.Modulation)
                    //    {
                    //        WeakTip.Default.Write("AWG", MsgTipId.AWGModeModulation);
                    //    }
                    //    if (((WfmGenMode)ChoosedButtonIndex - 1) == WfmGenMode.Sweep)
                    //    {
                    //        WeakTip.Default.Write("AWG", MsgTipId.AWGModeSweep);
                    //    }
                    //    RdoMode.ChoosedButtonIndex = Presenter.Active ? (Int32)Presenter.Mode + 1 : 0;
                    //}
                    //else
                    {
                        if (!Presenter.Active)
                        {
                            Presenter.Active = true;
                        }
                        if (ChoosedButtonIndex > 1 && ((WfmGenMode)ChoosedButtonIndex - 1) == WfmGenMode.Sweep)
                        {
                            CbTriggerSource.SelectValue = (Int32)Presenter.WfmGenTriger;
                            ChkGroupTrigOut.Checked = Presenter.TirgerOutEnabel;
                            CbTriggerSource.Visible = true;
                            LbTriggerSource.Visible = true;
                        }
                        else
                        {
                            BtnTrig.Visible = false;
                            CbTriggerSource.Visible = false;
                            LbTriggerSource.Visible = false;
                        }
                    }
                }

            }
        }

        private void RdoImpedance_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Impedance = (WfmGenImpedance)RdoImpedance.ChoosedButtonIndex;
            }
        }

        private void ChkOpposition_CheckedChangedEvent_1(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Opposition = ChkOpposition.Checked;
            }
        }

        private void CbxSweepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SweepType = (SweepType)CbxSweepType.SelectValue;
            }
        }

        private void RdoModRamp_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RampType = (WfmRampType)RdoModRamp.ChoosedButtonIndex;
            }
        }

        private void BtnArbitrary_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BSV(*.bsv)|*.bsv|Binary(*.bin)|*.bin";

            //FileBrowserForm rdform = FileBrowserForm.Instance;
            //rdform.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.BSV || x == WfmFormat.Binary));
            var defultPath = Constants.WFM_DEF_PATH;
            if (!defultPath.EndsWith("\\"))
            {
                defultPath += "\\";
            }
            dialog.InitialDirectory = defultPath;

            //rdform.SetPath(System.IO.Path.GetDirectoryName(defultPath));

            //if (rdform.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Presenter.FilePath = dialog.FileName;//rdform.FullFileName;

                BtnArbitraryRefreshTxt();
            }
        }
        private void BtnModArbRefreshTxt()
        {
            if (!string.IsNullOrWhiteSpace(Presenter.ModFilePath))
            {
                var filePath = Presenter.ModFilePath;
                string firstName = Path.GetFileName(filePath);//filePath.Substring(filePath.LastIndexOf("\\") + 1, (filePath.LastIndexOf(".") - filePath.LastIndexOf("\\") - 1));  //文件名
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    if (firstName.Length > 9)
                    {
                        firstName = firstName.Substring(0, 9);
                    }
                    BtnModArb.Text = firstName;
                    return;
                }
            }
            BtnModArb.Text = "...";
        }
        private void BtnArbitraryRefreshTxt()
        {
            if (!string.IsNullOrWhiteSpace(Presenter.FilePath))
            {
                var filePath = Presenter.FilePath;
                string firstName = Path.GetFileName(filePath);// filePath.Substring(filePath.LastIndexOf("\\") + 1, (filePath.LastIndexOf(".") - filePath.LastIndexOf("\\") - 1));  //文件名
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    if (firstName.Length > 9)
                    {
                        firstName = firstName.Substring(0, 9);
                    }
                    BtnArbitrary.Text = firstName;
                    return;
                }
            }
            BtnArbitrary.Text = "...";
        }
        private void BtnModArb_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BSV(*.bsv)|*.bsv|Binary(*.bin)|*.bin";

            //FileBrowserForm rdform = FileBrowserForm.Instance;
            //rdform.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.BSV || x == WfmFormat.Binary));
            var defultPath = Constants.WFM_DEF_PATH;
            if (!defultPath.EndsWith("\\"))
            {
                defultPath += "\\";
            }
            dialog.InitialDirectory = defultPath;
            //rdform.SetPath(System.IO.Path.GetDirectoryName(defultPath));

            //if (rdform.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Presenter.ModFilePath = dialog.FileName;//rdform.FullFileName;

                BtnModArbRefreshTxt();
            }
        }


        private void BtnTrig_Click(object sender, EventArgs e)
        {
            Presenter.WfmGenDoTriger();
        }

        private void CbTriggerSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.WfmGenTriger = (TriggerSource)CbTriggerSource.SelectValue;
                BtnTrig.Visible = Presenter.WfmGenTriger == TriggerSource.Manual;
            }
        }

        private void ChkGroupTrigOut_CheckedChanged(object sender, EventArgs e)
        {
            Presenter.TirgerOutEnabel = ChkGroupTrigOut.Checked;
            if (ChkGroupTrigOut.Checked)
            {
                // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：触发输出选择建议关联辅助输出 </ Remark >
                DsoPrsnt.DefaultDsoPrsnt.Setting.AuxOutputSignal = AuxOutputType.Sync_AWG;
                WeakTip.Default.Write("AWG", MsgTipId.AUXOutIsRequired);
            }
            else
            {
                // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：触发输出选择建议关联辅助输出 </ Remark >
                DsoPrsnt.DefaultDsoPrsnt.Setting.AuxOutputSignal = DsoPrsnt.DefaultDsoPrsnt.Setting.AuxOutputSignal == AuxOutputType.Sync_AWG ? AuxOutputType.Close : DsoPrsnt.DefaultDsoPrsnt.Setting.AuxOutputSignal;
            }
        }

        private void RBG_pointbypoint_IndexChanged(object sender, EventArgs e)
        {
            Presenter.EnablePointByPoint = RdoPointbypoint.ChoosedButtonIndex == 1;
            //<Remark>更改人：彭博 创建日期：2023/11/28 11:48:00  原因：打开关闭逐点输出时，应当刷新界面显示 </Remark>
            ChangeCtrlState();
            NebFreq.UpdateValueString();
        }

    }
}
