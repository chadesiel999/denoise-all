using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class PwrTransientPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PwrTransientPage()
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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Transient;

        public PwrTransientPrsnt Presenter
        {
            get;
            set;
        }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set => Presenter = (PwrTransientPrsnt)value;
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
                //  ShowCusorForm();
                _ArgToCtrl = false;
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

        private static String PosIndexToString(Double pos)
        {
            return new Quantity(pos, Prefix.Milli, QuantityUnit.Division.ToUnitString()).ToString("##0.###", true);
        }

        private void RdoSource_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (VIType)RdoTransientOpt.ChoosedButtonIndex;
            }
        }

    }
}