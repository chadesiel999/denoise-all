using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Core.Jitter;

namespace ScopeX.U2
{
    public partial class ConstantFreqClockPage : UserControl, IJitterView, IStylize
    {
        private Boolean _ArgToCtrl = false;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public JitterPrsnt Presenter
        {
            get;//=> (JitterPrsnt)(ParentForm as IJitterView).Presenter;
            set;//=> (ParentForm as IJitterView).Presenter = value;
        }
        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
        }
        public ConstantFreqClockPage()
        {
            InitializeComponent();
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Presenter?.TryRemoveView(this);
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

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {

        }
    }
}
