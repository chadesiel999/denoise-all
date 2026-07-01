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
    public partial class AVTPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public AVTPage()
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
            NebAmpScale.AddClicked = (a, b) => Presenter.AmpScale++;
            NebAmpScale.SubClicked = (a, b) => Presenter.AmpScale--;
            NebAmpScale.StringFormatFunc = (value) => VirticalScaleToString();
            NebAmpScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.AmpScale = data;

                //dBm
                nkf.SetKeyBoardValue(LblAmpScale.Text, Presenter.AmpVSTimeUnitV.ToString(), 9, onokclickeventaction,
                    Presenter.AmpScale,
                    Constants.RF_AMP_MAX_SCALE,
                    Constants.RF_AMP_MIN_SCALE);
                nkf.ShowDialogByPosition();
            };

            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource.Items.Add(ChannelId.AVT.ToString());
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

        public AmpVSTimePrsnt Presenter
        {
            get => (AmpVSTimePrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (AmpVSTimePrsnt)value;
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
                case nameof(Presenter.Label):
                    TbxLabel.Text = Presenter.Label;
                    break;
                case nameof(Presenter.AmpScale):
                    NebAmpScale.UpdateValueString();
                    break;
                case nameof(Presenter.UnitType):
                    CbxPowerUnit.SelectedIndex = (Int32)Presenter.UnitType;
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
                CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                TbxLabel.Text = Presenter.Label;
                CbxPowerUnit.SelectedIndex = (Int32)Presenter.UnitType;
                NebAmpScale.UpdateValueString();
                _ArgToCtrl = false;
            }

        }

        

        private String VirticalScaleToString() => new Quantity(Presenter.AmpScale, Prefix.Empty, Presenter.AmpVSTimeUnitV.ToString()).ToString("#0.#########", true);

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

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            Presenter.Label = TbxLabel.Text;
        }

        private void CbxPowerUnit_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.UnitType = (AmplitudeUnitType)CbxPowerUnit.SelectedIndex;
            }
        }
    }
}
