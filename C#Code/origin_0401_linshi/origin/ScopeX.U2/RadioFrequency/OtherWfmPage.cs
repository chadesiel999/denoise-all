using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.ComModel;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class OtherWfmPage : UserControl, IChnlView, IStylize
    {       
        private Boolean _ArgToCtrl;

        public OtherWfmPage()
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

        public RadioFrequencyPrsnt Presenter
        {
            get => (RadioFrequencyPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (RadioFrequencyPrsnt)value;
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
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
                case "AmpVSTimeActive":
                    ChkAmpVSTime.Checked = Presenter.AmpVSTime.Active;
                    break;
                case "PhaseVSTimeActive":
                    ChkPhaseVSTime.Checked = Presenter.PhaseVSTime.Active;
                    break;
                case "PhaseVSFrequencyActive":
                    ChkPhaseSpec.Checked = Presenter.PhaseVSFrequency.Active;
                    break;
                case "FrequencyVSTimeActive":
                    ChkFrequencyVSTime.Checked = Presenter.FrequencyVSTime.Active;
                    break;
                case "TimeVSFrequencyActive":
                    ChkFrequencyVSTime.Checked = Presenter.TimeVSFrequency.Active;
                    break;
                case nameof(Presenter.ThreeD):
                    ChkWaterfall.Checked = Presenter.TimeVSFrequency.ThreeD;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkAmpVSTime.Checked = Presenter.AmpVSTime.Active;
                ChkPhaseVSTime.Checked = Presenter.PhaseVSTime.Active;
                ChkPhaseSpec.Checked = Presenter.PhaseVSFrequency.Active;
                ChkFrequencyVSTime.Checked = Presenter.FrequencyVSTime.Active;
                ChkWaterfall.Checked = Presenter.TimeVSFrequency.ThreeD;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void ChkAmpVSTime_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Boolean active = ChkAmpVSTime.Checked;
                Presenter.AmpVSTime.Active = ChkAmpVSTime.Checked;
                if (!active)
                {
                    //(Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveWindow(Presenter.AmpVSTime.WindowId);
                    (Program.Oscilloscope.View as DsoForm).TryAddAVTWaveform(Presenter.AmpVSTime);
                }
            }
        }

        private void ChkFrequencyVSTime_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Boolean active = ChkFrequencyVSTime.Checked;
                Presenter.FrequencyVSTime.Active = active;
                if (!active)
                {
                    //(Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveWindow(Presenter.TimeVSFrequency.WindowId);
                    (Program.Oscilloscope.View as DsoForm).TryAddFVTWaveform(Presenter.FrequencyVSTime);
                }
            }
        }

        private void ChkPhaseVSTime_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Boolean active = ChkPhaseVSTime.Checked;
                Presenter.PhaseVSTime.Active = active;
                if (!active)
                {
                    //(Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveWindow(Presenter.PhaseVSTime.WindowId);
                    (Program.Oscilloscope.View as DsoForm).TryAddPVTWaveform(Presenter.PhaseVSTime);
                }
            }
        }

        private void ChkPhaseVSFrequency_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Boolean active = ChkPhaseSpec.Checked;
                Presenter.PhaseVSFrequency.Active = active;
                
                if (!active)
                {
                    //(Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveWindow(Presenter.PhaseVSFrequency.WindowId);
                    (Program.Oscilloscope.View as DsoForm).TryAddPVFWaveform(Presenter.PhaseVSFrequency);
                }
            }
        }

        private void ChkThreeD_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Boolean active = ChkWaterfall.Checked;
                Presenter.ThreeD = active;
                if (active)
                {
                    (Program.Oscilloscope.View as DsoForm).TryAdd3dUI(Presenter);
                }
            }
        }

       
    }
}
