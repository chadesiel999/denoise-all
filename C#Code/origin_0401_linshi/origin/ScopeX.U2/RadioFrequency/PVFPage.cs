using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PVFPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PVFPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //NebAmpScale
            ControlsHotKnob.Default.InitHotKnob(NebAmpScale);
            NebAmpScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmpScale);
            };
            NebAmpScale.AddClicked = (a, b) => Presenter.PhaseScale++;
            NebAmpScale.SubClicked = (a, b) => Presenter.PhaseScale--;
            NebAmpScale.StringFormatFunc = (value) => VirticalScaleToString();
            NebAmpScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.PhaseScale = data;

                //dBm
                nkf.SetKeyBoardValue(LblAmpScale.Text, Presenter.PhaseUnit.ToString(), 9, onokclickeventaction,
                    Presenter.PhaseScale,
                    Constants.RF_PHASE_SCALE_MAX,
                    Constants.RF_PHASE_SCALE_MIN);
                nkf.ShowDialogByPosition();
            };

            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource.Items.Add(ChannelId.PVF.ToString());
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
                }
            };
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

        public PhaseVSFrequencyPrsnt Presenter
        {
            get => (PhaseVSFrequencyPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (PhaseVSFrequencyPrsnt)value;
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
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
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    break;
                case nameof(Presenter.PhaseUnit):
                    NebAmpScale.UpdateValueString();
                    CbxPhaseUnit.SelectedIndex = (Int32)Presenter.PhaseUnit;
                    break;
                case nameof(Presenter.Label):
                    TbxLabel.Text = Presenter.Label;
                    break;
                case nameof(Presenter.PhaseScale):
                    NebAmpScale.UpdateValueString();
                    break;
                case nameof(Presenter.Source):
                    CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    break;

            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                ChkActive.Checked = Presenter.Active;
                NebAmpScale.UpdateValueString();
                CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                CbxPhaseUnit.SelectedIndex = (Int32)Presenter.PhaseUnit;
                TbxLabel.Text = Presenter.Label;
                _ArgToCtrl = false;
            }

        }

        

        private String VirticalScaleToString() => new Quantity(Presenter.PhaseScale, Prefix.Empty, Presenter.PhaseVSFrequencyUnitV.ToString()).ToString("#0.#########", true);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = false;
            }
        }


        private void CbxPowerUnit_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PhaseUnit = (PhaseUnitType)CbxPhaseUnit.SelectedIndex;
                NebAmpScale.UpdateValueString();
            }
        }

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            Presenter.Label = TbxLabel.Text;
        }

    }
}
