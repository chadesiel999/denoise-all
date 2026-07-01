// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
    using System;
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
    using static ScopeX.UserControls.ScopeXNumericEditBox;

    public partial class StateSettingForm : FlashBorderForm, ITriggerView, IChnlView
    {
        private readonly (ScopeXLabel Lbl, UIRadioButtonGroup Rdo, TouchNeb Neb)[] _PatSelections;

        private readonly String[] _TrigNames;

        private readonly ChannelId[] _TrigSources;

        private Boolean _ArgToCtrl;

        public StateSettingForm()
        {
            InitializeComponent();

            _TrigSources = PlatformUIManager.Default.Platform.GetTriggerSource(true, true, true, true).ToArray();

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
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(StateSettingForm)));
            };
        }

        public TrigStatePrsnt TrigPresenter { get; set; }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => TrigPresenter;
            set => TrigPresenter = (TrigStatePrsnt)value;
        }

        public DigitalPrsnt DigiPresenter { get; set; }

        IBadge IView<IBadge>.Presenter
        {
            get => DigiPresenter;
            set => DigiPresenter = (DigitalPrsnt)value;
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
            TrigPresenter.TryRemoveView(this);
            DigiPresenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
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
                case "CompCondition":
                case "CompPosIndex":
                case "UserThroldBymV":
                case "Family":
                    ConfigPatSeletions(4 * (int)(selectTouch1.SelectValue));
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                selectTouch1.SelectValue = 0;
                ConfigPatSeletions(0);
                ChangeClkState();

                NebInit(0);
                NebControlInit(ChannelId.Ext, NebThresholdExt);
                _ArgToCtrl = false;
            }
        }

        private void AllSettingAny()
        {
            for (Int32 i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + i], PatLevelCondition.Any);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.Any);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            ConfigPatSeletions((int)(selectTouch1.SelectValue) * 4);
        }

        private void AllSettingGreaterThan()
        {
            for (Int32 i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + i], PatLevelCondition.GreaterThan);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.GreaterThan);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            //刷新界面
            ConfigPatSeletions((int)(selectTouch1.SelectValue) * 4);
        }

        private void AllSettingLessThan()
        {
            for (int i = 0; i < 4; i++)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + i], PatLevelCondition.LessThan);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, i);
            }
            TrigPresenter.SetPosCompCondition(ChannelId.Ext, PatLevelCondition.LessThan);
            NebThresholdExt.Enabled = TrigPresenter.GetPosCompCondition(ChannelId.Ext) != PatLevelCondition.Any;

            ConfigPatSeletions((int)(selectTouch1.SelectValue) * 4);
        }

        private void ChangeTholdState(Int32 ofs, Int32 index)
        {
            var enabled = TrigPresenter.GetPosCompCondition(_TrigSources[ofs + index]) != PatLevelCondition.Any;
            _PatSelections[index].Item3.Enabled = enabled;
        }

        private void ChangeClkState()
        {
            for (Int32 i = 0; i < 4; i++)
            {
                _PatSelections[i].Item2.Enabled = _PatSelections[i].Item3.Enabled = true;
                RdoConditionExt.Enabled = NebThresholdExt.Enabled = true;
                //判断时钟源是否在当前界面上
                if (4 * (int)(selectTouch1.SelectValue) + i == (Int32)TrigPresenter.ClkSource)
                {
                    _PatSelections[i].Item2.Enabled = false;
                    _PatSelections[i].Item3.Enabled = false;
                    break;
                }
                else if (ChannelId.Ext == TrigPresenter.ClkSource)
                {
                    RdoConditionExt.Enabled = false;
                    NebThresholdExt.Enabled = false;
                }
            }
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
            CbxSelectGroup.Items.Clear();
            for (Int32 i = 0; i < _TrigSources.Length / 4; i++)
            {
                var name = _TrigSources[i * 4].ToString() + "-" + _TrigSources[i * 4 + 3].ToString();

                CbxSelectGroup.Items.Add(name);
            }
        }

        private void NebControlInit(ChannelId source, TouchNeb numEditor)
        {
            if (numEditor.AddClicked != null)
            {
                Delegate[] addInvokeList = numEditor.AddClicked.GetInvocationList(); ;
                if (addInvokeList != null)
                {
                    foreach (Delegate del in addInvokeList)
                    {
                        numEditor.AddClicked -= del as Action<object, NumericButtonEventData>;
                    }
                }
            }
            if (numEditor.SubClicked != null)
            {
                Delegate[] subInvokeList = numEditor.SubClicked.GetInvocationList(); ;
                if (subInvokeList != null)
                {
                    foreach (Delegate del in subInvokeList)
                    {
                        numEditor.SubClicked -= del as Action<object, NumericButtonEventData>;
                    }
                }
            }
            if (numEditor.EditValueChicked != null)
            {
                Delegate[] editInvokeList = numEditor.EditValueChicked.GetInvocationList(); ;
                if (editInvokeList != null)
                {
                    foreach (Delegate del in editInvokeList)
                    {
                        numEditor.EditValueChicked -= del as Action<object, double>;
                    }
                }
            }
            ControlsHotKnob.Default.InitHotKnob(numEditor);
            numEditor.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(TrigPresenter, numEditor);
            };
            numEditor.StringFormatFunc = (value) => CompThresholdToString(source);
            numEditor.AddClicked += (a, b) => TrigPresenter.SetPosIndex(source, TrigPresenter.GetPosIndex(source) + 1);
            numEditor.SubClicked += (a, b) => TrigPresenter.SetPosIndex(source, TrigPresenter.GetPosIndex(source) - 1);
            numEditor.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, numEditor);

                var onokclickeventaction = new Action<Double>((data) =>
                    TrigPresenter.SetCompPosition(source, Quantity.ConvertByPrefix(data, Prefix.Empty, TrigPresenter.GetPosPrefix(source))));

                nkf.SetKeyBoardValue("Threshold", TrigPresenter.GetPosUnit(source), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(TrigPresenter.GetCompPosition(source), TrigPresenter.GetPosPrefix(source)),
                    Quantity.ConvertByPrefix(TrigPresenter.GetMaxCompPosition(source), TrigPresenter.GetPosPrefix(source)),
                    Quantity.ConvertByPrefix(TrigPresenter.GetMinCompPosition(source), TrigPresenter.GetPosPrefix(source)));

                nkf.ShowDialogByPosition();
            };
        }

        private void NebInit(int ofs)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                NebControlInit(_TrigSources[ofs + i], _PatSelections[i].Item3);
            }
        }

        //private void (int)(selectTouch1.SelectValue)Changed(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        NebInit(4 * (int)(selectTouch1.SelectValue));
        //        ConfigPatSeletions(4 * (int)(selectTouch1.SelectValue));
        //        ChangeClkState();
        //    }
        //}

        private void RdoAllThrold_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                switch (RdoAllThrold.ChoosedButtonIndex)
                {
                    case 0://高于
                        AllSettingGreaterThan();
                        break;
                    case 1://低于
                        AllSettingLessThan();
                        break;
                    case 2://任意
                        AllSettingAny();
                        break;
                }
            }
        }

        private void RdoConditionCh1_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4], (PatLevelCondition)RdoCondition1.ChoosedButtonIndex);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, 0);
            }
        }

        private void RdoConditionCh2_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {

                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + 1], (PatLevelCondition)RdoCondition2.ChoosedButtonIndex);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, 1);
            }
        }

        private void RdoConditionCh3_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + 2], (PatLevelCondition)RdoCondition3.ChoosedButtonIndex);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, 2);
            }
        }

        private void RdoConditionCh4_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TrigPresenter.SetPosCompCondition(_TrigSources[(int)(selectTouch1.SelectValue) * 4 + 3], (PatLevelCondition)RdoCondition4.ChoosedButtonIndex);
                ChangeTholdState((int)(selectTouch1.SelectValue) * 4, 3);
            }
        }

        private void RdoConditionExt_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TrigPresenter.SetPosCompCondition(ChannelId.Ext, (PatLevelCondition)RdoConditionExt.ChoosedButtonIndex);
                ChangeTholdState(0, 4);
            }
        }

        private String CompThresholdToString(ChannelId source)
        {
            return new Quantity(TrigPresenter.GetCompPosition(source), TrigPresenter.GetPosPrefix(source), TrigPresenter.GetPosUnit(source)).ToString("##0.####", true, 7);
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        private void selectTouch1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                NebInit(4 * (int)(selectTouch1.SelectValue));
                ConfigPatSeletions(4 * (int)(selectTouch1.SelectValue));
                ChangeClkState();
            }
        }
    }
}
