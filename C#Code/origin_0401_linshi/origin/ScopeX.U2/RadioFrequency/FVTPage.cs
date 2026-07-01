using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class FVTPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public FVTPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //NebVirticalScale
            ControlsHotKnob.Default.InitHotKnob(NebVirticalScale);
            NebVirticalScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVirticalScale);
            };
            NebVirticalScale.AddClicked = (a, b) => Presenter.FrequencyScale++;
            NebVirticalScale.SubClicked = (a, b) => Presenter.FrequencyScale--;
            NebVirticalScale.StringFormatFunc = (value) => VirticalScaleToString();
            NebVirticalScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVirticalScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FrequencyScale = (Int64)data;

                //dBm
                nkf.SetKeyBoardValue(LblVirticalScale.Text, Presenter.FrequencyVSTimeUnitV.ToString(), 9, onokclickeventaction,
                    Presenter.FrequencyScale,
                    Constants.RF_FREQUENCY_SCALE_MAX,
                    Constants.RF_FREQUENCY_SCALE_MIN);
                nkf.ShowDialogByPosition();
            };

            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource.Items.Add(ChannelId.FVT.ToString());
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

        public FrequencyVSTimePrsnt Presenter
        {
            get => (FrequencyVSTimePrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (FrequencyVSTimePrsnt)value;
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
                case nameof(Presenter.FrequencyScale):
                    NebVirticalScale.UpdateValueString();
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
                NebVirticalScale.UpdateValueString();
                _ArgToCtrl = false;
            }

        }

        

        private String VirticalScaleToString() => new Quantity(Presenter.FrequencyScale, Prefix.Empty, Presenter.FrequencyVSTimeUnitV.ToString()).ToString("#0.#########", true);

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

    }
}
