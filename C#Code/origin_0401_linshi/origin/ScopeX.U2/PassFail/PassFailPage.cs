// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/23</date>

using ScopeX.Core;

namespace ScopeX.U2.PassFail
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Controls.Common.Structs;
    using ScopeX.UserControls;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class PassFailPage : UserControl, IPassFailView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PassFailPage()
        {
            InitializeComponent();

            DefaultStyleManager.Instance.RegisterControlRecursion(ChkMask, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(BtnRun, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(PanelStdTest, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ScopeXLabelStdTest, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(CbxStdMaskType, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(BtnReadMask, StyleFlag.FontSize);


            DefaultStyleManager.Instance.RegisterControlRecursion(ChkLockMask, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LbMaskInfo, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblStandard, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(PanelLimitTest, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(BtnCreateMask, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(CbxMaskSource, StyleFlag.FontSize);


            DefaultStyleManager.Instance.RegisterControlRecursion(NebHoriTolerance, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblHorzTolerance, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblMaskSource, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblVertTolerance, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(NebVertTolerance, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ScopeXLabelLimitTest, StyleFlag.FontSize);

            DefaultStyleManager.Instance.RegisterControlRecursion(RdoMode, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblMode, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(CbxSource, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblSource, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkActive, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblActive, StyleFlag.FontSize);

            DefaultStyleManager.Instance.RegisterControlRecursion(PanelFinishCondition, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(NebTestDuration, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(NebViolation, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(RdoTestStopType, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblLimit, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblViolation, StyleFlag.FontSize);


            DefaultStyleManager.Instance.RegisterControlRecursion(ScopeXLabelFinishCondition, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(PanelFailAction, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkHardCopy, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkPulse, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkBeep, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkStore, StyleFlag.FontSize);

            DefaultStyleManager.Instance.RegisterControlRecursion(ScopeXLabelFailAction, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(NebTestWfms, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(SplitLine, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(SplitLineFinishCondition, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(SplitLineFailAction, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(SplitLineLimitTest, StyleFlag.FontSize);

            // DefaultStyleManager.Instance.RegisterControlRecursion(SplitLineRun, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkLabNoteBook, StyleFlag.FontSize);
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

        public PassFailPrsnt Presenter
        {
            get => Program.Oscilloscope.PassFail;
            set => (ParentForm as IPassFailView).Presenter = value;
        }

        IPassFailPrsnt IView<IPassFailPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PassFailPrsnt)value;
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    if (Presenter.Active)
                    {
                        PassFailApp.Default.ShowInfoForm();
                    }
                    BtnRun.Enabled = Presenter.Active;
                    break;
                case nameof(Presenter.Source):
                    //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                    CbxSource.SelectValue = Presenter.Source;
                    break;
                case nameof(Presenter.Mode):
                    RdoMode.ChoosedButtonIndex = (Int32)Presenter.Mode;
                    break;
                case nameof(Presenter.VertTolerance):
                    NebVertTolerance.UpdateValueString();
                    break;
                case nameof(Presenter.HorzTolerance):
                    NebHoriTolerance.UpdateValueString();
                    break;
                case nameof(Presenter.MaskSource):
                    //CbxMaskSource.SelectedIndex = (Int32)Presenter.MaskSource;
                    CbxMaskSource.SelectValue = Presenter.MaskSource;
                    break;
                case nameof(Presenter.StdMaskType):
                    CbxStdMaskType.SelectedIndex = (Int32)Presenter.StdMaskType;
                    ShowMaskList();
                    Presenter.ReadStdMask();
                    break;
                case nameof(Presenter.MaskLocked):
                    ChkLockMask.Checked = Presenter.MaskLocked;
                    break;
                case nameof(Presenter.Violations):
                    NebViolation.UpdateValueString();
                    break;
                case nameof(Presenter.TestStopType):
                    RdoTestStopType.ChoosedButtonIndex = (Int32)Presenter.TestStopType;
                    break;
                case nameof(Presenter.TestWfms):
                    NebTestWfms.UpdateValueString();
                    break;
                case nameof(Presenter.TestDurationByms):
                    NebTestDuration.UpdateValueString();
                    break;
                case nameof(Presenter.Store):
                    ChkStore.Checked = Presenter.Store;
                    break;
                case nameof(Presenter.Beep):
                    ChkBeep.Checked = Presenter.Beep;
                    break;
                case nameof(Presenter.Pulse):
                    ChkPulse.Checked = Presenter.Pulse;
                    break;
                case nameof(Presenter.HardCopy):
                    ChkHardCopy.Checked = Presenter.HardCopy;
                    break;
                case nameof(Presenter.LabNoteBook):
                    ChkLabNoteBook.Checked = Presenter.LabNoteBook;
                    break;
                //case nameof(Presenter.VisibleInfo):
                //    ChkResult.Checked = Presenter.VisibleInfo;
                //    break;
                case nameof(Presenter.VisibleMask):
                    ChkMask.Checked = Presenter.VisibleMask;
                    break;
                case nameof(Presenter.Running):
                    ChangeRunButtonState(Presenter.Running);
                    break;
                case nameof(Presenter.StdMaskIndex):
                    LbMaskInfo.SelectedIndex = Presenter.StdMaskIndex;
                    ShowMaskList();
                    Presenter.ReadStdMask();
                    break;
                default:
                    break;
            }
            ChangeCtrlState();
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = Presenter.Active;
                //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                CbxSource.SelectValue = Presenter.Source;
                RdoMode.ChoosedButtonIndex = (Int32)Presenter.Mode;
                //CbxMaskSource.SelectedIndex = (Int32)Presenter.MaskSource;
                CbxMaskSource.SelectValue = (Int32)Presenter.MaskSource;

                CbxStdMaskType.SelectedIndex = (Int32)Presenter.StdMaskType;
                NebVertTolerance.UpdateValueString();
                NebHoriTolerance.UpdateValueString();

                RdoTestStopType.ChoosedButtonIndex = (Int32)Presenter.TestStopType;
                ChkStore.Checked = Presenter.Store;
                ChkBeep.Checked = Presenter.Beep;
                ChkPulse.Checked = Presenter.Pulse;
                ChkHardCopy.Checked = Presenter.HardCopy;
                ChkLabNoteBook.Checked = Presenter.LabNoteBook;
                ChkMask.Checked = Presenter.VisibleMask;
                ChangeRunButtonState(Presenter.Running);
                BtnRun.Enabled = Presenter.Active;
                ChkLockMask.Checked = Presenter.MaskLocked;

                NebTestDuration.UpdateValueString();
                NebTestWfms.UpdateValueString();
                NebViolation.UpdateValueString();

                ShowMaskList();

                ChangeCtrlState();

                if (Presenter.Active)
                {
                    PassFailApp.Default.ShowInfoForm();
                }
                _ArgToCtrl = false;
            }
        }

        private void ChangeRunButtonState(Boolean running)
        {
            string passfailStop = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PassFail_Stop");
            string passfailRun = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PassFail_Run");
            //BtnRun.Text = running ? Properties.Resources.PassFail_Stop : Properties.Resources.PassFail_Run;
            BtnRun.Text = running ? passfailStop : passfailRun;
            var color = running ? Color.FromArgb(142, 11, 11) : Color.FromArgb(14, 139, 28);
            BtnRun.BackColor = color;
            BtnRun.MouseinBackColor = color;
            BtnRun.PressedBackColor = color;
            BtnRun.Invalidate();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }
        private void InitControlLang()
        {
            ChkMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiCeShiMoBan");
            BtnRun.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YunXing");
            ScopeXLabelStdTest.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhunCeShiMoBan");
            BtnReadMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuQu");
            ChkLockMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SuoDingMoBan");
            LblStandard.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhun");
            BtnCreateMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuangJian");
            LblHorzTolerance.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuiPingRongXian");
            LblMaskSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanKaoYuan");
            LblVertTolerance.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuiZhiRongXian");
            ScopeXLabelLimitTest.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiXianCeShiMoBan");

            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiXianCeShi");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhunCeShi");
            RdoMode.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 });

            LblMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShi");
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");

            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingZongShu");
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongShiJian");
            RdoTestStopType.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem3, radioButtonItem4 });

            LblViolation.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongWeiLiCiShu");
            ScopeXLabelFinishCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiJieShuTiaoJian");
            ChkHardCopy.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiePing");
            ChkPulse.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MaiChong");
            ChkBeep.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoJing");
            ChkStore.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunBoXing");
            ScopeXLabelFailAction.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiShiBaiCaoZuo");
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitControlLang();
            InitOnLoad();
            UpdateView();
        }

        private void InitOnLoad()
        {
            //SplitLine.DarkColor = AppStyleConfig.DefaultTitleBackColor;
            //SplitLine.LightColor = AppStyleConfig.DefaultTitleBackColor;

            //SplitLineFinishCondition.DarkColor = AppStyleConfig.DefaultTitleBackColor;
            //SplitLineFinishCondition.LightColor = AppStyleConfig.DefaultTitleBackColor;

            //SplitLineFailAction.DarkColor = AppStyleConfig.DefaultTitleBackColor;
            //SplitLineFailAction.LightColor = AppStyleConfig.DefaultTitleBackColor;

            //SplitLineLimitTest.DarkColor = AppStyleConfig.DefaultTitleBackColor;
            //SplitLineLimitTest.LightColor = AppStyleConfig.DefaultTitleBackColor;

            //SplitLineRun.DarkColor = AppStyleConfig.DefaultTitleBackColor;
            //SplitLineRun.LightColor = AppStyleConfig.DefaultTitleBackColor;

            ControlsHotKnob.Default.InitHotKnob(NebHoriTolerance);
            NebHoriTolerance.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHoriTolerance, nameof(Presenter.HorzTolerance));
            };
            NebHoriTolerance.AddClicked = (a, b) => Presenter.HorzTolerance++;
            NebHoriTolerance.SubClicked = (a, b) => Presenter.HorzTolerance--;
            NebHoriTolerance.StringFormatFunc = (value) => ToleranceToString(Presenter.HorzTolerance);
            NebHoriTolerance.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHoriTolerance);
                var onokclick = new Action<double>((data) =>
                {
                    Presenter.HorzTolerance = (Int32)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli);
                });

                nkf.SetKeyBoardValue(LblHorzTolerance.Text, QuantityUnit.Division.ToUnitString(), 3, onokclick,
                    Quantity.ConvertByPrefix(Presenter.HorzTolerance, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MaxHorzTolerance, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MinHorzTolerance, Prefix.Milli));

                nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebVertTolerance);
            NebVertTolerance.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVertTolerance, nameof(Presenter.VertTolerance));
            };
            NebVertTolerance.AddClicked = (a, b) => Presenter.VertTolerance++;
            NebVertTolerance.SubClicked = (a, b) => Presenter.VertTolerance--;
            NebVertTolerance.StringFormatFunc = (value) => ToleranceToString(Presenter.VertTolerance);
            NebVertTolerance.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVertTolerance);
                var onokclick = new Action<double>((data) =>
                {
                    Presenter.VertTolerance = (Int32)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli);
                });

                nkf.SetKeyBoardValue(LblVertTolerance.Text, QuantityUnit.Division.ToUnitString(), 3, onokclick,
                    Quantity.ConvertByPrefix(Presenter.VertTolerance, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MaxVertTolerance, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MinVertTolerance, Prefix.Milli));

                nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebViolation);
            NebViolation.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebViolation, nameof(Presenter.Violations));
            };
            NebViolation.AddClicked = (a, b) => Presenter.Violations++;
            NebViolation.SubClicked = (a, b) => Presenter.Violations--;
            NebViolation.StringFormatFunc = (value) => ViolationToString();
            NebViolation.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebViolation);
                var onokclick = new Action<Double>((data) => Presenter.Violations = (Int32)data);
                Int32 maxviolations = Presenter.TestStopType == PFTestStopOpt.TestWfms ? Presenter.TestWfms : Presenter.MaxViolations;

                nkf.SetKeyBoardValue(LblViolation.Text, QuantityUnit.Count.ToUnitString(), 0, onokclick,
                    Presenter.Violations,
                    maxviolations,
                    Presenter.MinViolations);

                nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebTestWfms);
            NebTestWfms.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebTestWfms, nameof(Presenter.TestWfms));
            };
            NebTestWfms.AddClicked = (a, b) => Presenter.TestWfms++;
            NebTestWfms.SubClicked = (a, b) => Presenter.TestWfms--;
            NebTestWfms.StringFormatFunc = (value) => TestWfmsToString();
            NebTestWfms.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebTestWfms);
                var onokclick = new Action<Double>((data) => Presenter.TestWfms = (Int32)data);

                nkf.SetKeyBoardValue(RdoTestStopType.ButtonItems[0].Text,
                    QuantityUnit.Count.ToUnitString(),
                    2,
                    onokclick,
                    Presenter.TestWfms,
                    Presenter.MaxTestWfms,
                    Presenter.MinTestWfms);

                nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebTestDuration);
            NebTestDuration.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebTestDuration, nameof(Presenter.TestDurationByms));
            };
            NebTestDuration.AddClicked = (a, b) => Presenter.AdjTestDuration(1);
            NebTestDuration.SubClicked = (a, b) => Presenter.AdjTestDuration(-1);
            NebTestDuration.StringFormatFunc = (value) => TestDurationToString();
            NebTestDuration.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebTestDuration);
                var onokclick = new Action<Double>((data) => Presenter.TestDurationByms = (Int32)Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

                nkf.SetKeyBoardValue(RdoTestStopType.ButtonItems[1].Text,
                    QuantityUnit.Second.ToUnitString(),
                    2,
                    onokclick,
                    Quantity.ConvertByPrefix(Presenter.TestDurationByms, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MaxTestDuration, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MinTestDuration, Prefix.Milli));

                nkf.ShowDialogByPosition();
            };

            LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
            LoadMaskSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));

            //if (Presenter.MaskCreated == false && Presenter.Active)
            //{
            //    if (Presenter.Mode == PFTestMode.LimitMode)
            //    {
            //        BtnCreateMask_Click(null, null);
            //    }
            //    else
            //    {
            //        BtnReadMask_Click(null, null);
            //    }
            //}
        }

        private void LoadMaskSourceList(IEnumerable<ChannelId> sources)
        {
            CbxMaskSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxMaskSource.SelectValue = Presenter.MaskSource;
            CbxMaskSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.MaskSource = (ChannelId)CbxMaskSource.SelectValue;
                }
            };
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            CbxSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxSource.SelectValue = Presenter.Source;
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = (ChannelId)CbxSource.SelectValue;
                }
            };
        }

        private void ChangeCtrlState()
        {
            PanelLimitTest.Visible = Presenter.Mode == PFTestMode.LimitMode;
            PanelStdTest.Visible = !PanelLimitTest.Visible;

            PanelStdTest.Enabled = Presenter.Active;
            PanelLimitTest.Enabled = Presenter.Active;
            ChkMask.Enabled = Presenter.Active; ;

            NebTestWfms.Visible = Presenter.TestStopType == PFTestStopOpt.TestWfms;
            NebTestDuration.Visible = !NebTestWfms.Visible;

            var enabled = !Presenter.Running;
            BtnCreateMask.Enabled = RdoMode.Enabled = CbxSource.Enabled = enabled && Presenter.Active;
            BtnReadMask.Enabled = enabled && Presenter.Active && !ChkLockMask.Checked;
            PanelFinishCondition.Enabled = PanelFailAction.Enabled = enabled && Presenter.Active;
            LbMaskInfo.Enabled = enabled && Presenter.Active;
            CbxStdMaskType.Enabled = enabled && Presenter.Active;
            ChkLockMask.Enabled = enabled && Presenter.Active;
            NebHoriTolerance.Enabled = enabled && Presenter.Active;
            NebVertTolerance.Enabled = enabled && Presenter.Active;
            CbxMaskSource.Enabled = enabled && Presenter.Active;
        }

        private static String ToleranceToString(Double value)
        {
            return new Quantity(value, Prefix.Milli, QuantityUnit.Division).ToString("##0", true);
        }

        private String TestDurationToString()
        {
            return new Quantity(Presenter.TestDurationByms, Prefix.Milli, QuantityUnit.Second).ToString("##0.######", true);
        }

        private String TestWfmsToString()
        {
            return new Quantity(Presenter.TestWfms, Prefix.Empty, QuantityUnit.Count).ToString("##0.###", true);
        }

        private String ViolationToString()
        {
            return new Quantity(Presenter.Violations, Prefix.Empty, QuantityUnit.Count).ToString("##0", true);
        }

        private void BtnCreateMask_Click(object sender, EventArgs e)
        {
            if (Program.Oscilloscope.TryGetChannel(Presenter.MaskSource, out IChnlPrsnt channel))
            {
                if (!channel.Active)
                {
                    WeakTip.Default.Write("PassFail", MsgTipId.ChannelClosed);
                    return;
                }
            }

            Presenter.MakeMask();
        }

        private void LbMaskInfo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (Presenter.StdMaskIndex == LbMaskInfo.SelectedIndex)
                    return;

                Presenter.StdMaskIndex = LbMaskInfo.SelectedIndex;
                Presenter.ReadStdMask();
            }
        }

        private void BtnReadMask_Click(object sender, EventArgs e)
        {
            if (this.ParentForm != null && this.ParentForm is FloatForm floatform)
            {
                floatform.CanClose = false;
            }

            //Presenter.StdMaskName = name;
            //Presenter.StdMaskIndex = LbMaskInfo.SelectedIndex;
            Presenter.ReadStdMask();

            //Int32 idx = LbMaskInfo.SelectedIndex;
            ShowMaskList();
            //LbMaskInfo.SelectedIndex = idx;


            if (this.ParentForm != null && this.ParentForm is FloatForm form)
            {
                form.CanClose = true;
            }
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = ChkActive.Checked;
                ChkActive.Checked = Presenter.Active;
                Presenter.Running = false;
            }
        }

        private void ChkLockMask_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.MaskLocked = ChkLockMask.Checked;
                BtnReadMask.Enabled = !ChkLockMask.Checked;
            }
        }

        private void RdoMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Mode = (PFTestMode)RdoMode.ChoosedButtonIndex;
                // if (Presenter.MaskCreated == false)
                {
                    if (Presenter.Mode == PFTestMode.LimitMode)
                    {
                        BtnCreateMask_Click(null, null);
                    }
                    else
                    {
                        BtnReadMask_Click(null, null);
                    }
                }
            }
        }

        private void ShowMaskList()
        {

            var topindex = LbMaskInfo.TopIndex;
            LbMaskInfo.Items.Clear();

            var smt = (Int32)Presenter.StdMaskType;

            foreach (var name in Presenter.StdMaskNames[smt])
            {
                var temp = name.Substring(0, name.LastIndexOf('.'));
                if (temp == Presenter.CurrentStdMaskName)
                {
                    temp = @"●  " + temp;
                }
                LbMaskInfo.Items.Add(temp);
            }

            LbMaskInfo.SelectedIndex = Presenter.StdMaskIndex;
            LbMaskInfo.TopIndex = topindex;
        }

        //public static Control InfoControl
        //{
        //    get;
        //    set;
        //}

        //private void ShowInfoForm()
        //{
        //    //if (active)
        //    //{
        //    //    PassFailApp.Default.ShowInfoForm();
        //    //}
        //    //else
        //    //{
        //    //    PassFailApp.Default.CloseInfoForm();
        //    //}

        //    if (InfoControl is null)
        //    {
        //        var pfif = new PassFailInfoForm(Presenter)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = pfif.GetDataView;

        //        EventBus.EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = pfif, Type = FormType.InfoForm });
        //    }


        //}

        private void BtnRun_Click(object sender, EventArgs e)
        {
            Presenter.Running ^= true;
        }

        private void ChkStore_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Store = ChkStore.Checked;
            }
        }

        private void ChkBeep_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Beep = ChkBeep.Checked;
            }
        }

        private void ChkPulse_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Pulse = ChkPulse.Checked;
            }
        }

        private void ChkHardCopy_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.HardCopy = ChkHardCopy.Checked;
            }
        }

        private void ChkMask_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.VisibleMask = ChkMask.Checked;
            }
        }

        private void RdoTestStopType_IndexChanged(object sender, EventArgs e)
        {
            Presenter.TestStopType = (PFTestStopOpt)RdoTestStopType.ChoosedButtonIndex;
        }

        private void CbxStdMaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Presenter.StdMaskType = (PFStdMaskType)CbxStdMaskType.SelectedIndex;
        }

        private void ChkLabNoteBook_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.LabNoteBook = ChkLabNoteBook.Checked;
            }
        }
    }
}
