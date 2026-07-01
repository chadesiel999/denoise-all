using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Core.Jitter;

namespace ScopeX.U2
{
    public partial class IntegrityAnalysisPage : UserControl, IJitterView, IStylize
    {
        private Boolean _ArgToCtrl = false;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public IntegrityAnalysisPage()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        public JitterPrsnt Presenter
        {
            get;//=> (JitterPrsnt) (ParentForm as IJitterView).Presenter;
            set;// => (ParentForm as IJitterView).Presenter = value;
        }

        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
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
                case nameof(Presenter.EnableChannelSim):
                    ChkChannelSimulation.Checked = Presenter.EnableChannelSim;
                    break;
                case nameof(Presenter.EnableRxFFE):
                    ChkReceiverEqualization.Checked = Presenter.EnableRxFFE;
                    break;
                default:
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                ChkChannelSimulation.Checked = Presenter.EnableChannelSim;
                ChkReceiverEqualization.Checked = Presenter.EnableRxFFE;
            }
        }

        private void BtnFileSelect_Click(object sender, EventArgs e)
        {
            FileBrowserForm rdform = FileBrowserForm.Instance;

            if (rdform.ShowDialogByEvent() == DialogResult.Yes)
            {
                Presenter.S2PPath = rdform.FullFileName;
            }
        }

        private void ChkChannelSimulation_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EnableChannelSim = ChkChannelSimulation.Checked;
            }
        }

        private void ChkReceiverEqualization_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EnableRxFFE = ChkReceiverEqualization.Checked;
            }
        }

        private void BtnReceiverEqualization_Click(object sender, EventArgs e)
        {
            FileBrowserForm rdform = FileBrowserForm.Instance;

            if (rdform.ShowDialogByEvent() == DialogResult.Yes)
            {
                Presenter.TapPath = rdform.FullFileName;
            }
        }
    }
}
