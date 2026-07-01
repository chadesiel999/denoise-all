using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class HorizontalPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public HorizontalPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //NebRBW
            ControlsHotKnob.Default.InitHotKnob(NebRBW);
            NebRBW.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebRBW);
            };
            NebRBW.AddClicked = (a, b) => Presenter.RBW++;
            NebRBW.SubClicked = (a, b) => Presenter.RBW--;
            NebRBW.StringFormatFunc = (value) => RBWToString();
            NebRBW.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebRBW);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.RBW = (Int64)data;
                
                nkf.SetKeyBoardValue(LblRBW.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.RBW,
                    Presenter.GetRBWMax(),
                    Presenter.GetRBWMin());

                nkf.ShowDialogByPosition();
            };

            //NebHorizontalScale
            ControlsHotKnob.Default.InitHotKnob(NebHorizontalScale);
            NebHorizontalScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHorizontalScale);
            };
            NebHorizontalScale.AddClicked = (a, b) => Presenter.FrequencyScale++;
            NebHorizontalScale.SubClicked = (a, b) => Presenter.FrequencyScale--;
            NebHorizontalScale.StringFormatFunc = (value) => FreqScaleToString();
            NebHorizontalScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHorizontalScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FrequencyScale = (Int64)data;
                

                nkf.SetKeyBoardValue(LblHorizontalScale.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.FrequencyScale, 
                    Constants.RF_HSCALE_MAX * 2,
                    Constants.RF_HSCALE_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebHorizontalCenter
            ControlsHotKnob.Default.InitHotKnob(NebHorizontalCenter);
            NebHorizontalCenter.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHorizontalCenter);
            };
            NebHorizontalCenter.AddClicked = (a, b) => Presenter.FigureCenterFrequency++;
            NebHorizontalCenter.SubClicked = (a, b) => Presenter.FigureCenterFrequency--;
            NebHorizontalCenter.StringFormatFunc = (value) => HorizontalCenterFreqToString();
            NebHorizontalCenter.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHorizontalCenter);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FigureCenterFrequency = (Int64)data;

                nkf.SetKeyBoardValue(LblHorizontalCenter.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.FigureCenterFrequency,
                    Constants.RF_FREQUENCY_MAX,
                    Constants.RF_FREQUENCY_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebCenterFrequency
            ControlsHotKnob.Default.InitHotKnob(NebCenterFrequency);
            NebCenterFrequency.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebCenterFrequency);
            };
            NebCenterFrequency.AddClicked = (a, b) => Presenter.CenterFrequency++;
            NebCenterFrequency.SubClicked = (a, b) => Presenter.CenterFrequency--;
            NebCenterFrequency.StringFormatFunc = (value) => CenterFrequencyToString();
            NebCenterFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebCenterFrequency);
                Action<Double> onokclickeventaction = (data) =>
                   Presenter.CenterFrequency = (Int64)data;
               

                nkf.SetKeyBoardValue(LblCenterFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.CenterFrequency,
                    Constants.RF_CENTER_FREQUENCY_MAX,
                    Constants.RF_CENTER_FREQUENCY_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebSpan
            ControlsHotKnob.Default.InitHotKnob(NebSpan);
            NebSpan.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebSpan);
            };
            NebSpan.AddClicked = (a, b) => Presenter.Span++;
            NebSpan.SubClicked = (a, b) => Presenter.Span--;
            NebSpan.StringFormatFunc = (value) => SpanToString();
            NebSpan.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSpan);
                Action<Double> onokclickeventaction = (data) =>
                  Presenter.Span = (Int64)data;               

                nkf.SetKeyBoardValue(LblSpan.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.Span,
                    Constants.RF_SPAN_MAX,
                    Constants.RF_SPAN_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebStartFrequency
            ControlsHotKnob.Default.InitHotKnob(NebStartFrequency);
            NebStartFrequency.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebStartFrequency);
            };
            NebStartFrequency.AddClicked = (a, b) => Presenter.StartFrequency++;
            NebStartFrequency.SubClicked = (a, b) => Presenter.StartFrequency--;
            NebStartFrequency.StringFormatFunc = (value) => StartFrequencyToString();
            NebStartFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebStartFrequency);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.StartFrequency = (Int64)data;
                
                nkf.SetKeyBoardValue(LblStartFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.StartFrequency, 
                    Constants.RF_START_FREQUENCY_MAX,
                    Constants.RF_START_FREQUENCY_MIN);

                nkf.ShowDialogByPosition();
            };

            //NebEndFrequency
            ControlsHotKnob.Default.InitHotKnob(NebEndFrequency);
            NebEndFrequency.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebEndFrequency);
            };
            NebEndFrequency.AddClicked = (a, b) => Presenter.EndFrequency++;
            NebEndFrequency.SubClicked = (a, b) => Presenter.EndFrequency--;
            NebEndFrequency.StringFormatFunc = (value) => EndFrequencyToString();
            NebEndFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebEndFrequency);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.EndFrequency = (Int64)data;              

                nkf.SetKeyBoardValue(LblEndFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    Presenter.EndFrequency,
                    Constants.RF_END_FREQUENCY_MAX, 
                    Constants.RF_END_FREQUENCY_MIN);

                nkf.ShowDialogByPosition();
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

        public void UpdateView(Object prsnt, String propertyName)
        {
            UpdateView();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                NebRBW.UpdateValueString();
                NebSpan.UpdateValueString();
                NebStartFrequency.UpdateValueString();
                NebEndFrequency.UpdateValueString();
                NebCenterFrequency.UpdateValueString();
                NebHorizontalCenter.UpdateValueString();
                NebHorizontalScale.UpdateValueString();

                CbxFFTLength.Text = Presenter.FFTLength.ToString();

                _ArgToCtrl = false;
            }

        }

        private String FreqScaleToString() => new Quantity(Presenter.FrequencyScale, Prefix.Empty, "Hz").ToString("#0.#########", true);
        
        private String HorizontalCenterFreqToString() => new Quantity(Presenter.FigureCenterFrequency, Prefix.Empty, "Hz").ToString("#0.#########", true);
       
        private String CenterFrequencyToString() => new Quantity(Presenter.CenterFrequency, Prefix.Empty, "Hz").ToString("#0.#########", true);
        
        private String StartFrequencyToString() => new Quantity(Presenter.StartFrequency, Prefix.Empty, "Hz").ToString("#0.#########", true);
        
        private String EndFrequencyToString() => new Quantity(Presenter.EndFrequency, Prefix.Empty, "Hz").ToString("#0.#########", true);
        
        private String SpanToString() => new Quantity(Presenter.Span, Prefix.Empty, "Hz").ToString("#0.#########", true);
        
        private String RBWToString() => new Quantity(Presenter.RBW, Prefix.Empty, "").ToString("#0.#########", true);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void CbxFFTLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FFTLength = Convert.ToInt32(CbxFFTLength.Items[CbxFFTLength.SelectedIndex]);
            }
        }
    }
}
