using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;
using ScopeX.Core.Tools;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.Content.Objects;
using ScopeX.Core;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class OBPage : UserControl, IMathView, IStylize
    {
        public OBPage()
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

        private void Init()
        {
            //NebChannelSpan
            ControlsHotKnob.Default.InitHotKnob(NebChannelSpan);
            NebChannelSpan.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebChannelSpan, nameof(_FftArg.ChannelSpanOB));
            };
            NebChannelSpan.AddClicked = (a, b) => _FftArg.ChannelSpanOB++;
            NebChannelSpan.SubClicked = (a, b) => _FftArg.ChannelSpanOB--;
            NebChannelSpan.StringFormatFunc = (_) => ChannelSpanToString();

            NebChannelSpan.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelSpan);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.ChannelSpanOB = data * 1_000_000;

                nkf.SetKeyBoardValue(LblChannelSpan.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                    _FftArg.ChannelSpanOB / 1_000_000,
                    Constants.RF_MEASURE_MAX_OB_CHANNEL_SPAN / 1_000_000,
                    Constants.RF_MEASURE_MIN_OB_CHANNEL_SPAN / 1_000_000);

                //nkf.ShowDialogByPosition();
            };

            //NebPowerPercentage
            ControlsHotKnob.Default.InitHotKnob(NebPowerPercentage);
            NebPowerPercentage.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebPowerPercentage, nameof(_FftArg.PercentageOB));
            };
            NebPowerPercentage.AddClicked = (a, b) => _FftArg.PercentageOB++;
            NebPowerPercentage.SubClicked = (a, b) => _FftArg.PercentageOB--;
            NebPowerPercentage.StringFormatFunc = (value) => PercentageOBToString();

            NebPowerPercentage.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPowerPercentage);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.PercentageOB = data;

                nkf.SetKeyBoardValue(LblPowerPercentage.Text, QuantityUnit.Percent.ToUnitString(), 3, onokclickeventaction,
                    _FftArg.PercentageOB,
                    Constants.RF_MEASURE_MAX_OB_PERCENTAGE,
                    Constants.RF_MEASURE_MIN_OB_PERCENTAGE);

                //nkf.ShowDialogByPosition();
            };

            //NebdBDown
            ControlsHotKnob.Default.InitHotKnob(NebdBDown);
            NebdBDown.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(_FftArg, NebdBDown, nameof(_FftArg.dBDownOB));
            };
            NebdBDown.AddClicked = (a, b) => _FftArg.dBDownOB++;
            NebdBDown.SubClicked = (a, b) => _FftArg.dBDownOB--;
            NebdBDown.StringFormatFunc = (value) => dBDownToString();

            NebdBDown.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebdBDown);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.dBDownOB = (Int32)data;

                nkf.SetKeyBoardValue(LbldBDown.Text, QuantityUnit.Constant.ToUnitString(), 0, onokclickeventaction,
                    _FftArg.dBDownOB,
                    Constants.RF_MEASURE_MAX_OB_DB_DOWN,
                    Constants.RF_MEASURE_MIN_OB_DB_DOWN);

                //nkf.ShowDialogByPosition();
            };
            ChkActive.Checked = _FftArg.EnableOB;
        }

        private String ChannelSpanToString()
        {
            return new Quantity(_FftArg.ChannelSpanOB, Prefix.Micro, QuantityUnit.Hertz).ToString("##0.000", true);
        }
        private String PercentageOBToString()
        {
            return new Quantity(_FftArg.PercentageOB, Prefix.Empty, QuantityUnit.Percent).ToString("##0.000", true);
        }
        private String dBDownToString()
        {
            return new Quantity(_FftArg.dBDownOB, Prefix.Empty, QuantityUnit.Decibel).ToString("##0", true);
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _FftArg = (MathFftArg)Presenter.GetOrMakeArg(MathType.FFT);
            RdoAnalysisType.ChoosedButtonIndex = (Int32)_FftArg.AnalysisTypeOB;
            Init();
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
            NebPowerPercentage.UpdateValueString();
            NebdBDown.UpdateValueString();
            ChkActive.Checked = _FftArg.EnableOB;
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
                NebPowerPercentage.UpdateValueString();
                NebdBDown.UpdateValueString();
                ChkActive.Checked = _FftArg.EnableOB;
                _ArgToCtrl = false;
            }
        }
        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                _FftArg.EnableOB = ChkActive.Checked;
        }

        private void RdoAnalysisType_IndexChanged(object sender, EventArgs e)
        {
            _FftArg.AnalysisTypeOB = (OBWAnalysisType)RdoAnalysisType.ChoosedButtonIndex;
        }
    }
}
