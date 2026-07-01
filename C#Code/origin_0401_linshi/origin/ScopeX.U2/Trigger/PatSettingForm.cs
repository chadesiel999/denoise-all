// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.LanguageSupoort;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class PatSettingForm : FlashBorderForm, ITriggerView, IChnlView
    {
        private readonly (ScopeXLabel, UIRadioButtonGroup, TouchNeb)[] _PatSelections;

        private readonly String[] _TrigNames;

        private readonly ChannelId[] _TrigSources;

        private Boolean _ArgToCtrl;

        private bool _HasEdgeSelected;

        public PatSettingForm()
        {
            InitializeComponent();
            LangKeyInit();
            _TrigSources = PlatformUIManager.Default.Platform.GetTriggerSource().ToArray();
            _TrigNames = _TrigSources.Select((ts) => ts.ToString()).ToArray();
            _PatSelections = new[]
            {
                (LblTrigger1, RdoCondition1, NebThreshold1),
                (LblTrigger2, RdoCondition2, NebThreshold2),
                (LblTrigger3, RdoCondition3, NebThreshold3),
                (LblTrigger4, RdoCondition4, NebThreshold4),
                (LblExt, RdoConditionExt, NebThresholdExt),
            };
            InitSrcGroupList();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PatSettingForm)));
            };
        }
        private void LangKeyInit()
        {

            LblGreater.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Gao");
            LblLess.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Di");
            LblAny.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYi");
            LblPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MenXian");
            LblSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuanBuSheZhi");
            LblSelection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDaoFenZuXuanZe");
            LblR.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            LblF.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuSheZhi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuSheZhi");
        }

        public DigitalPrsnt DigiPresenter { get; set; }

        IBadge IView<IBadge>.Presenter
        {
            get => DigiPresenter;
            set => DigiPresenter = (DigitalPrsnt)value;
        }

        public TrigPatPrsnt TrigPresenter { get; set; }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => TrigPresenter;
            set => TrigPresenter = (TrigPatPrsnt)value;
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TrigPresenter?.TryRemoveView(this);
            DigiPresenter?.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
            LblError.ForeColor = System.Drawing.Color.FromArgb(255, 115, 48);
            ChannelOpenCheck_All();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case "CompCondition":
                case "CompPosIndex":
                case "UserThroldBymV":
                case "Family":
                    ConfigPatSeletions(4 * ((Int32)CbxSelectGroup.SelectValue));
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                CbxSelectGroup.SelectValue = 0;
                NebInit(0);
                NebControlInit(ChannelId.Ext, NebThresholdExt);
                ConfigPatSeletions(0);
                _ArgToCtrl = false;
            }
        }

        private void AllSettingAny()
        {
            for (Int32 i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + i], PatLevelCondition.Any);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.Any);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            ConfigPatSeletions(((Int32)CbxSelectGroup.SelectValue) * 4);
        }

        private void AllSettingGreaterThan()
        {
            for (Int32 i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + i], PatLevelCondition.GreaterThan);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.GreaterThan);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            //刷新界面
            ConfigPatSeletions(((Int32)CbxSelectGroup.SelectValue) * 4);
        }

        private void AllSettingLessThan()
        {
            for (int i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + i], PatLevelCondition.LessThan);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.LessThan);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            ConfigPatSeletions(((Int32)CbxSelectGroup.SelectValue) * 4);
        }

        private void AllSettingRisingEdge()
        {
            for (int i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + i], PatLevelCondition.Rise);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.Rise);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Rise;

            ConfigPatSeletions(((Int32)CbxSelectGroup.SelectValue) * 4);
        }
        private void AllSettingFallingEdge()
        {
            for (int i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + i], PatLevelCondition.Fall);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.Fall);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Fall;

            ConfigPatSeletions(((Int32)CbxSelectGroup.SelectValue) * 4);
        }

        // 判断是否禁用阈值选择.
        private void ChangeTholdState(Int32 ofs, Int32 index)
        {
            // 处理Bug：1762
            /*var enabled = TrigPresenter.GetPosCompCondition(_TrigSources[ofs + index]) != PatLevelCondition.Any;
            _PatSelections[index].Item3.Enabled = enabled;*/
        }

        private void ConfigPatSeletions(Int32 ofs)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                _PatSelections[i].Item1.Text = _TrigNames[ofs + i];
                _PatSelections[i].Item2.ChoosedButtonIndex = (Int32)TrigPresenter.GetPosCompCondition(_TrigSources[ofs + i]);
                //Update Value
                _PatSelections[i].Item3.UpdateValueString();
            }

            RdoConditionExt.ChoosedButtonIndex = (Int32)TrigPresenter.GetPosCompCondition(ChannelId.Ext);
            NebThresholdExt.UpdateValueString();
        }

        private void InitSrcGroupList()
        {
            //CbxSelectGroup.Items.Clear();
            //for (Int32 i = 0; i < _TrigSources.Length / 4; i++)
            //{
            //    var name = _TrigSources[i * 4].ToString() + "-" + _TrigSources[i * 4 + 3].ToString();

            //    CbxSelectGroup.Items.Add(name);
            //}

            var arrs = new List<String>();
            for (Int32 i = 0; i < _TrigSources.Length / 4; i++)
            {
                var name = _TrigSources[i * 4].ToString() + "-" + _TrigSources[i * 4 + 3].ToString();

                arrs.Add(name);
            }
            CbxSelectGroup.Items = arrs.ToArray();

        }

        private void NebControlInit(ChannelId source, TouchNeb numEditor)
        {
            ControlsHotKnob.Default.InitHotKnob(numEditor);
            numEditor.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(TrigPresenter, numEditor);
            };

            numEditor.AddClicked = (a, b) => TrigPresenter.SetPosIndex(source, TrigPresenter.GetPosIndex(source) + 1);
            numEditor.SubClicked = (a, b) => TrigPresenter.SetPosIndex(source, TrigPresenter.GetPosIndex(source) - 1);
            numEditor.StringFormatFunc = (value) => CompThresholdToString(source);
            numEditor.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, numEditor);

                var onokclickeventaction = new Action<Double>((data) =>
                    TrigPresenter.SetVuCompPosition(source, Quantity.ConvertByPrefix(data, Prefix.Empty, TrigPresenter.GetPosPrefix(source))));

                nkf.SetKeyBoardValue(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MenXian"), TrigPresenter.GetPosUnit(source), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(TrigPresenter.GetVuCompPosition(source), TrigPresenter.GetPosPrefix(source)),
                    Quantity.ConvertByPrefix(TrigPresenter.GetMaxCompPosition(source), TrigPresenter.GetPosPrefix(source)),
                    Quantity.ConvertByPrefix(TrigPresenter.GetMinCompPosition(source), TrigPresenter.GetPosPrefix(source)));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebInit(Int32 ofs)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                NebControlInit(_TrigSources[ofs + i], _PatSelections[i].Item3);
            }
        }

        //private void CbxSelectGroup_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        NebInit(4 * CbxSelectGroup.SelectedIndex);
        //        ConfigPatSeletions(4 * CbxSelectGroup.SelectedIndex);
        //    }
        //}


        private void RdoAllThrold_IndexChanged(object sender, EventArgs e)
        {
            var haserror = ChannelOpenCheck(ChannelId.C1, RdoAllThrold);
            if (haserror)
                return;
            haserror = ChannelOpenCheck(ChannelId.C2, RdoAllThrold);
            if (haserror)
                return;
            haserror = ChannelOpenCheck(ChannelId.C3, RdoAllThrold);
            if (haserror)
                return;
            ChannelOpenCheck(ChannelId.C4, RdoAllThrold);
        }

        private void ChannelOpenCheck_All()
        {
            var haserror = ChannelOpenCheck(ChannelId.C1, RdoCondition1);
            if (haserror)
                return;
            haserror = ChannelOpenCheck(ChannelId.C2, RdoCondition2);
            if (haserror)
                return;
            haserror = ChannelOpenCheck(ChannelId.C3, RdoCondition3);
            if (haserror)
                return;
            ChannelOpenCheck(ChannelId.C4, RdoCondition4);
        }


        /// <summary>
        /// 通道是否打开的检测，如果通道未打开，却在切换则给出提示
        /// </summary>
        /// <param name="chnnelid"></param>
        private bool ChannelOpenCheck(ChannelId chnnelid, UIRadioButtonGroup uIRadioButton)
        {
            LblError.Visible = false;

            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(chnnelid, out IChnlPrsnt cprsnt))
                return false;

            if (cprsnt is not ChannelPrsnt channelPrsnt)
                return false;

            if (channelPrsnt.Active)
                return false;

            if (uIRadioButton.ChoosedButtonIndex == 1 || uIRadioButton.ChoosedButtonIndex == 2)
                return false;

            var fomart = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChannelNotOpenErrorMsg");
            LblError.Text = string.Format(fomart, chnnelid.ToString());
            LblError.Visible = true;
            return true;
        }

        private void RdoConditionCh1_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                OnlyOneEdgeHandler(RdoCondition1);
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4], (PatLevelCondition)RdoCondition1.ChoosedButtonIndex);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, 0);
                ChannelOpenCheck_All();
            }
        }

        private void RdoConditionCh2_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                OnlyOneEdgeHandler(RdoCondition2);
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + 1], (PatLevelCondition)RdoCondition2.ChoosedButtonIndex);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, 1);
                ChannelOpenCheck_All();
            }
        }

        private void RdoConditionCh3_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                OnlyOneEdgeHandler(RdoCondition3);
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + 2], (PatLevelCondition)RdoCondition3.ChoosedButtonIndex);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, 2);
                ChannelOpenCheck_All();
            }
        }

        private void RdoConditionCh4_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                OnlyOneEdgeHandler(RdoCondition4);
                TrigPresenter.SetPosCompCondition(_TrigSources[((Int32)CbxSelectGroup.SelectValue) * 4 + 3], (PatLevelCondition)RdoCondition4.ChoosedButtonIndex);
                ChangeTholdState(((Int32)CbxSelectGroup.SelectValue) * 4, 3);
                ChannelOpenCheck_All();
            }
        }

        private void EnabledAllEdgeBtn(bool enabled)
        {
            RdoCondition1.SetButtonEnable(3, enabled);
            RdoCondition1.SetButtonEnable(4, enabled);
            RdoCondition2.SetButtonEnable(3, enabled);
            RdoCondition2.SetButtonEnable(4, enabled);
            RdoCondition3.SetButtonEnable(3, enabled);
            RdoCondition3.SetButtonEnable(4, enabled);
            RdoCondition4.SetButtonEnable(3, enabled);
            RdoCondition4.SetButtonEnable(4, enabled);
        }

        private bool AllBtnNotEdgeConfig() => RdoCondition1.ChoosedButtonIndex < 3 && RdoCondition2.ChoosedButtonIndex < 3 && RdoCondition3.ChoosedButtonIndex < 3 && RdoCondition4.ChoosedButtonIndex < 3;

        /// <summary>
        /// 实现只能配置一个边沿（上升沿或者下降沿）Bug：1803
        /// </summary>
        /// <param name="uIRadioButton"></param>
        private void OnlyOneEdgeHandler(UIRadioButtonGroup uIRadioButton)
        {
            if (uIRadioButton.ChoosedButtonIndex > 2)
            {
                if (!_HasEdgeSelected)
                {
                    EnabledAllEdgeBtn(false);
                    uIRadioButton.SetButtonEnable(3, true);
                    uIRadioButton.SetButtonEnable(4, true);
                    _HasEdgeSelected = true;
                }
            }
            else
            {
                if (_HasEdgeSelected && AllBtnNotEdgeConfig())
                {
                    EnabledAllEdgeBtn(true);
                    _HasEdgeSelected = false;
                }
            }
        }

        private void RdoConditionExt_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TrigPresenter.SetPosCompCondition(ChannelId.Ext, (PatLevelCondition)RdoConditionExt.ChoosedButtonIndex);
                NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;
            }
        }

        private String CompThresholdToString(ChannelId source)
        {
            return new Quantity(TrigPresenter.GetVuCompPosition(source), TrigPresenter.GetPosPrefix(source), TrigPresenter.GetPosUnit(source)).ToString("##0.####", true, 7);
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        private void CbxSelectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                NebInit(4 * ((Int32)CbxSelectGroup.SelectValue));
                ConfigPatSeletions(4 * ((Int32)CbxSelectGroup.SelectValue));
            }
        }

        private void RdoAllThrold_OnItemClicked(int obj)
        {
            if (!_ArgToCtrl)
            {
                switch (RdoAllThrold.ChoosedButtonIndex)
                {
                    case 0://高于
                        AllSettingGreaterThan();
                        OnlyOneEdgeHandler(RdoAllThrold);
                        break;
                    case 1://低于
                        AllSettingLessThan();
                        OnlyOneEdgeHandler(RdoAllThrold);
                        break;
                    case 2://任意
                        AllSettingAny();
                        OnlyOneEdgeHandler(RdoAllThrold);
                        break;
                        /*case 3:
                            AllSettingRisingEdge();
                            break;
                        case 4:
                            AllSettingFallingEdge();
                            break;*/
                }
            }
        }
    }
}
