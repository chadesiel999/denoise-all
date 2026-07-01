// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;

    public partial class TriggerCascadeSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl; 
        
        private readonly ComboBoxEx[] _EventType;

        public TriggerCascadeSubPage()
        {
            InitializeComponent();
            _EventType = new[] { CbxEventA, CbxEventB, CbxEventC, CbxEventD };
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public TrigMultiQualifiedPrsnt Presenter 
        { 
            get => (TrigMultiQualifiedPrsnt)(ParentForm as ITriggerView).Presenter; 
            set => (ParentForm as ITriggerView).Presenter = value; 
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter 
        { 
            get => Presenter; 
            set => Presenter = (TrigMultiQualifiedPrsnt)value; 
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
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                InitEventTypeOpt();
                _ArgToCtrl = false;
            }
        }

        private void CfgEvent(Int32 index)
        {
            if (Presenter.Count <= index)
            {
                Presenter.AddEvent(Presenter.QualifiedType[_EventType[index].SelectedIndex]);
            }

            var mqsf = new CascadeSettingForm(GetContentOpt(_EventType[index].SelectedIndex), _EventType[index].Text)
            {
                StartPosition = FormStartPosition.CenterScreen,
            };
            mqsf.Presenter = Presenter.GetEvent(index, mqsf);
            mqsf.PathPresenter = Presenter.GetPathway(index, mqsf);
            mqsf.PathPresenter.TryAddView(mqsf);

            (ParentForm as TriggerForm).CanClose = false;
            mqsf.ShowDialogByPosition();
            (ParentForm as TriggerForm).CanClose = true;
        }

        private ITriggerView GetContentOpt(Int32 index)
        {
            return Presenter.QualifiedType[index] switch
            {
                ComModel.TriggerType.Edge => new TriggerEdgeSubPage(),
                ComModel.TriggerType.PulseWidth => new TriggerWidthSubPage(),

                ComModel.TriggerType.TimeOut => new TriggerTimeOutSubPage(),

                ComModel.TriggerType.Runt => new TriggerRuntSubPage(),
                ComModel.TriggerType.Transition => new TriggerTransSubPage(),
                ComModel.TriggerType.Glitch => new TriggerGlitchSubPage(),
                ComModel.TriggerType.Window => new TriggerWindowSubPage(),
                ComModel.TriggerType.MultiQulified => new TriggerCascadeSubPage(),
                _ => throw new NotImplementedException(),
            };
        }

        private void CfgEventType(Int32 index)
        {
            if (!_ArgToCtrl)
            {
                if (Presenter.Count > index)
                {
                    Presenter.SetEvent(index, Presenter.QualifiedType[_EventType[index].SelectedIndex]);
                }
                else
                {
                    Presenter.AddEvent(Presenter.QualifiedType[_EventType[index].SelectedIndex]);
                }
            }
        }

        private void InitEventTypeOpt()
        {
            if (Presenter.Count > 0)
            {
                _EventType[0].SelectedIndex = Presenter.GetEventTypeIndex(0);
            }
            else
            {
                Presenter.AddEvent(Presenter.QualifiedType[_EventType[0].SelectedIndex]);
            }

            if (Presenter.Count > 1)
            {
                _EventType[1].SelectedIndex = Presenter.GetEventTypeIndex(1);
            }
            else
            {
                Presenter.AddEvent(Presenter.QualifiedType[_EventType[1].SelectedIndex]);
            }

            if (Presenter.Count > 2)
            {
                _EventType[2].SelectedIndex = Presenter.GetEventTypeIndex(2);
                ChkEnableEventC.Enabled = true;
            }

            if (Presenter.Count > 3)
            {
                _EventType[3].SelectedIndex = Presenter.GetEventTypeIndex(3);
                ChkEnableEventD.Enabled = true;
            }
        }

        private void BtnEventA_Click(object sender, EventArgs e)
        {
            CfgEvent(0);
        }

        private void BtnEventB_Click(object sender, EventArgs e)
        {
            CfgEvent(1);
        }

        private void BtnEventC_Click(object sender, EventArgs e)
        {
            CfgEvent(2);
        }

        private void BtnEventD_Click(object sender, EventArgs e)
        {
            CfgEvent(3);
        }

        private void CbxEventA_SelectedIndexChanged(object sender, EventArgs e)
        {
            CfgEventType(0);
        }

        private void CbxEventB_SelectedIndexChanged(object sender, EventArgs e)
        {
            CfgEventType(1);
        }

        private void CbxEventC_SelectedIndexChanged(object sender, EventArgs e)
        {
            CfgEventType(2);
        }

        private void CbxEventD_SelectedIndexChanged(object sender, EventArgs e)
        {
            CfgEventType(3);
        }
    
        private void ChkEnableEventC_Click(object sender, EventArgs e)
        {
            CbxEventC.Enabled = BtnEventC.Enabled = GrpEventD.Enabled = ChkEnableEventC.Checked;
        }
    
        private void ChkEnableEventD_Click(object sender, EventArgs e)
        {
            CbxEventD.Enabled = BtnEventD.Enabled = ChkEnableEventD.Checked;
        }
    }
}
