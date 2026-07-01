using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls.Style;
using ScopeX.Core.Tools;
using ScopeX.Controls.Common.Default;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.Content.Objects;
using ScopeX.Core;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class THDPage : UserControl, IMathView, IStylize
    {
        public THDPage()
        {
            InitializeComponent();
        }

        public MathType Mode => MathType.FFT;

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
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebChannelSpan, nameof(_FftArg.ChannelSpanTHD));
            };
            NebChannelSpan.AddClicked = (a, b) => _FftArg.ChannelSpanTHD++;
            NebChannelSpan.SubClicked = (a, b) => _FftArg.ChannelSpanTHD--;
            NebChannelSpan.StringFormatFunc = (_) => ChannelSpanToString();

            NebChannelSpan.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelSpan);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.ChannelSpanTHD = data * 1_000_000;

                nkf.SetKeyBoardValue(LblChannelSpan.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                    _FftArg.ChannelSpanTHD / 1_000_000,
                    Constants.RF_MEASURE_MAX_THD_CHANNEL_SPAN / 1_000_000,
                    Constants.RF_MEASURE_MIN_THD_CHANNEL_SPAN / 1_000_000);

                //nkf.ShowDialogByPosition();
            };

            //NebChannelSpacing
            ControlsHotKnob.Default.InitHotKnob(NebChannelSpacing);
            NebChannelSpacing.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebChannelSpacing, nameof(_FftArg.ChannelSpacingTHD));
            };
            NebChannelSpacing.AddClicked = (a, b) => _FftArg.ChannelSpacingTHD++;
            NebChannelSpacing.SubClicked = (a, b) => _FftArg.ChannelSpacingTHD--;
            NebChannelSpacing.StringFormatFunc = (value) => ChannelSpacingToString();

            NebChannelSpacing.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelSpacing);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.ChannelSpacingTHD = data * 1_000_000;

                nkf.SetKeyBoardValue(LblChannelSpacing.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                    _FftArg.ChannelSpacingTHD / 1_000_000,
                    Constants.RF_MEASURE_MAX_THD_CHANNEL_SPACING / 1_000_000,
                    Constants.RF_MEASURE_MIN_THD_CHANNEL_SPACING / 1_000_000);

                //nkf.ShowDialogByPosition();
            };

            //NebChannelCount
            ControlsHotKnob.Default.InitHotKnob(NebChannelCount);
            NebChannelCount.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebChannelCount, nameof(_FftArg.ChannelCountTHD));
            };
            NebChannelCount.AddClicked = (a, b) => _FftArg.ChannelCountTHD++;
            NebChannelCount.SubClicked = (a, b) => _FftArg.ChannelCountTHD--;
            NebChannelCount.StringFormatFunc = (value) => ChannelCountToString();

            NebChannelCount.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelCount);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.ChannelCountTHD = (Int32)data;

                nkf.SetKeyBoardValue(LblChannelCount.Text, QuantityUnit.Constant.ToUnitString(), 0, onokclickeventaction,
                    _FftArg.ChannelCountTHD,
                    Constants.RF_MEASURE_MAX_THD_CHANNEL_COUNT,
                    Constants.RF_MEASURE_MIN_THD_CHANNEL_COUNT);

                //nkf.ShowDialogByPosition();
            };
            ChkActive.Checked = _FftArg.EnableTHD;
        }

        private String ChannelSpanToString()
        {
            return new Quantity(_FftArg.ChannelSpanTHD, Prefix.Micro, QuantityUnit.Hertz).ToString("##0.000", true);
        }
        private String ChannelSpacingToString()
        {
            return new Quantity(_FftArg.ChannelSpacingTHD, Prefix.Micro, QuantityUnit.Hertz).ToString("##0.000", true);
        }
        private String ChannelCountToString()
        {
            return new Quantity(_FftArg.ChannelCountTHD, Prefix.Empty, QuantityUnit.Constant).ToString("##0", true);
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }
        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(Object presenter, String propertyName)
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
            {
                return;
            }

            NebChannelSpan.UpdateValueString();
            NebChannelSpacing.UpdateValueString();
            NebChannelCount.UpdateValueString();
            ChkActive.Checked = _FftArg.EnableTHD;
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
                NebChannelSpacing.UpdateValueString();
                NebChannelCount.UpdateValueString();
                ChkActive.Checked = _FftArg.EnableTHD;
                _ArgToCtrl = false;
            }
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                _FftArg.EnableTHD = ChkActive.Checked;
        }
    }
}
