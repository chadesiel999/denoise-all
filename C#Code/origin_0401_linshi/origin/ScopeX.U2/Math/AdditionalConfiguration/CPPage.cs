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
using ScopeX.Core.Tools;
using ScopeX.Controls.Common.Default;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class CPPage : UserControl, IMathView, IStylize
    {
        public CPPage()
        {
            InitializeComponent();
        }
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public MathType Mode => MathType.FFT;
        public MathPrsnt Presenter
        {
            get => (MathPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (MathPrsnt)value;
        }

        private MathFftArg _FftArg;

        private Boolean _ArgToCtrl;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _FftArg = (MathFftArg)Presenter.GetOrMakeArg(MathType.FFT);
            Init();
            UpdateView();
        }
        private void Init()
        {
            //NebChannelSpan
            ControlsHotKnob.Default.InitHotKnob(NebChannelSpan);
            NebChannelSpan.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebChannelSpan, nameof(_FftArg.ChannelSpanCP));
            };
            NebChannelSpan.AddClicked = (a, b) => _FftArg.ChannelSpanCP++;
            NebChannelSpan.SubClicked = (a, b) => _FftArg.ChannelSpanCP--;
            NebChannelSpan.StringFormatFunc = (_) => ChannelSpanToString();

            NebChannelSpan.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelSpan);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.ChannelSpanCP = data * 1_000_000;

                nkf.SetKeyBoardValue(LblChannelSpan.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                    _FftArg.ChannelSpanCP / 1_000_000,
                    Constants.RF_MEASURE_MAX_CP_CHANNEL_SPAN / 1_000_000,
                    Constants.RF_MEASURE_MIN_CP_CHANNEL_SPAN / 1_000_000);

                //nkf.ShowDialogByPosition();
            };

            ChkActive.Checked = _FftArg.EnableCP;
        }

        private String ChannelSpanToString()
        {
            return new Quantity(_FftArg.ChannelSpanCP, Prefix.Micro, QuantityUnit.Hertz).ToString("##0.000", true);
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

            }
            if (_FftArg is null)
                return;
            NebChannelSpan.UpdateValueString();
            ChkActive.Checked = _FftArg.EnableCP;
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (_FftArg is null)
                    return;

                _ArgToCtrl = true;
                NebChannelSpan.UpdateValueString();
                ChkActive.Checked = _FftArg.EnableCP;
                _ArgToCtrl = false;
            }
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                _FftArg.EnableCP = ChkActive.Checked;
        }
    }
}
