// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;

    public partial class PwrDifferPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PwrDifferPage()
        {
            InitializeComponent();
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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Differ;

        public PwrDifferPrsnt Presenter { get; set; }
        
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter 
        { 
            get => Presenter; 
            set => Presenter = (PwrDifferPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh(); 
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
                    RdoDifferSrc.ChoosedButtonIndex = (Int32)Presenter.Source;
                    break;
            }
            _ArgToCtrl = false;
        }

        //!!!Notice: Dispose Presenter
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Presenter.TryRemoveView(this);

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoDifferSrc.ChoosedButtonIndex = (Int32)Presenter.Source;
                _ArgToCtrl = false;
            }
        }

        private void BtnShowDifferWfm_Click(object sender, EventArgs e)
        {
            if (Program.Oscilloscope.TryGetChannel(ComModel.ChannelId.M1, out var mprsnt))
            {
                Presenter.TryShowDifferWfm((MathPrsnt)mprsnt);
            }
        }

        private void RdoDifferSrc_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (VIType)RdoDifferSrc.ChoosedButtonIndex;
            }
        }
    }
}
