using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core;
using System.Windows.Forms;
using ScopeX.UserControls.Style;
using ScopeX.Core.Tools;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class ParameterPage : UserControl, IMultiDomainView, IStylize
    {
        private Boolean _ArgToCtrl;
        public ParameterPage()
        {
            InitializeComponent();
            Init();
            InitCbxWindowType();
        }

        private void Init()
        {
            # region NebRBW
            NebRBW.AddClicked = (a, b) => MultiDomainPresenter.RBWByHz++;
            NebRBW.SubClicked = (a, b) => MultiDomainPresenter.RBWByHz--;
            NebRBW.StringFormatFunc = (value) => RBWToString();
            NebRBW.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    MultiDomainPresenter.RBWByHz = NormalizeHzInput(data);

                nkf.SetKeyBoardValue(LblRBW.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.RBWByHz);
                //,
                //Presenter.GetRBWMax(),
                //Presenter.GetRBWMin());

                nkf.ShowDialogByPosition();
            };
            #endregion

            #region NebHorizontalScale
            //NebHorizontalScale.AddClicked = (a, b) => Presenter.FrequencyScale++;
            //NebHorizontalScale.SubClicked = (a, b) => Presenter.FrequencyScale--;
            //NebHorizontalScale.StringFormatFunc = (value) => FreqScaleToString();
            //NebHorizontalScale.EditValueChicked = (a, b) =>
            //{
            //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //    Action<Double> onokclickeventaction = (data) =>
            //        Presenter.FrequencyScale = (Int64)data;


            //    nkf.SetKeyBoardValue(LblHorizontalScale.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
            //        Presenter.FrequencyScale,
            //        Constants.RF_HSCALE_MAX * 2,
            //        Constants.RF_HSCALE_MIN);

            //    nkf.ShowDialogByPosition();
            //};
            #endregion

            #region NebHorizontalCenter
            //NebHorizontalCenter.AddClicked = (a, b) => Presenter.FigureCenterFrequency++;
            //NebHorizontalCenter.SubClicked = (a, b) => Presenter.FigureCenterFrequency--;
            //NebHorizontalCenter.StringFormatFunc = (value) => HorizontalCenterFreqToString();
            //NebHorizontalCenter.EditValueChicked = (a, b) =>
            //{
            //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //    Action<Double> onokclickeventaction = (data) =>
            //        Presenter.FigureCenterFrequency = (Int64)data;

            //    nkf.SetKeyBoardValue(LblHorizontalCenter.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
            //        Presenter.FigureCenterFrequency,
            //        Constants.RF_FREQUENCY_MAX,
            //        Constants.RF_FREQUENCY_MIN);

            //    nkf.ShowDialogByPosition();
            //};
            #endregion

            #region NebCenterFrequency
            NebCenterFrequency.AddClicked = (a, b) => MultiDomainPresenter.CenterFreqByHz++;
            NebCenterFrequency.SubClicked = (a, b) => MultiDomainPresenter.CenterFreqByHz--;
            NebCenterFrequency.StringFormatFunc = (value) => CenterFrequencyToString();
            NebCenterFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                   MultiDomainPresenter.CenterFreqByHz = (Int64)NormalizeHzInput(data);


                //nkf.SetKeyBoardValue(LblCenterFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                //    MultiDomainPresenter.CenterFreqByHz, Constants.RF_CENTER_FREQUENCY_MAX, Constants.RF_CENTER_FREQUENCY_MIN);
                nkf.SetKeyBoardValue(LblCenterFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.CenterFreqByHz, MultiDomainPresenter.MaxSampleRate - MultiDomainPresenter.SpanFreqByHz / 2, 1_000 / 2);

                nkf.ShowDialogByPosition();
            };
            #endregion

            #region NebSpan
            NebSpan.AddClicked = (a, b) => MultiDomainPresenter.SpanFreqByHz++;
            NebSpan.SubClicked = (a, b) => MultiDomainPresenter.SpanFreqByHz--;
            NebSpan.StringFormatFunc = (value) => SpanToString();
            NebSpan.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                  MultiDomainPresenter.SpanFreqByHz = (Int64)NormalizeHzInput(data);

                nkf.SetKeyBoardValue(LblSpan.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.SpanFreqByHz, MultiDomainPresenter.MaxSpanFreq, MultiDomainPresenter.MinSpanFreq);

                nkf.ShowDialogByPosition();
            };
            #endregion

            #region NebStartFrequency
            NebStartFrequency.AddClicked = (a, b) => MultiDomainPresenter.StartFreqByHz++;
            NebStartFrequency.SubClicked = (a, b) => MultiDomainPresenter.StartFreqByHz--;
            NebStartFrequency.StringFormatFunc = (value) => StartFrequencyToString();
            NebStartFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    MultiDomainPresenter.StartFreqByHz = (Int64)NormalizeHzInput(data);

                //nkf.SetKeyBoardValue(LblStartFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                //    MultiDomainPresenter.StartFreqByHz, Constants.RF_START_FREQUENCY_MAX, Constants.RF_START_FREQUENCY_MIN);
                nkf.SetKeyBoardValue(LblStartFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.StartFreqByHz, MultiDomainPresenter.MaxSampleRate - 1_000, 0);

                nkf.ShowDialogByPosition();
            };
            #endregion

            #region NebEndFrequency
            NebEndFrequency.AddClicked = (a, b) => MultiDomainPresenter.EndFreqByHz++;
            NebEndFrequency.SubClicked = (a, b) => MultiDomainPresenter.EndFreqByHz--;
            NebEndFrequency.StringFormatFunc = (value) => EndFrequencyToString();
            NebEndFrequency.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    MultiDomainPresenter.EndFreqByHz = (Int64)NormalizeHzInput(data);

                //nkf.SetKeyBoardValue(LblEndFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                //    MultiDomainPresenter.EndFreqByHz, Constants.RF_END_FREQUENCY_MAX, Constants.RF_END_FREQUENCY_MIN);
                nkf.SetKeyBoardValue(LblEndFrequency.Text, QuantityUnit.Hertz.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.EndFreqByHz, MultiDomainPresenter.MaxSampleRate, 1_000);

                nkf.ShowDialogByPosition();
            };
            #endregion

            InitNebTimeScale();

            InitNebTimeStep();
        }

        private void SpanForTimeFreqSelectedIndexChanged(Object sender, EventArgs args)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.SpanOptForTimeFreq = CbxTimeFreqSpan.SelectedIndex;
            }
        }

        private void InitCbxSpanForTimeFreq()
        {
            CbxTimeFreqSpan.SelectedIndexChanged -= SpanForTimeFreqSelectedIndexChanged;
            CbxTimeFreqSpan.DataSource = MultiDomainPresenter.SpanListForTimeFreq.Select(o => new KeyValuePair<String, Int64>(new Quantity(o, Prefix.Empty, "Hz").ToString("0.000", true), o)).ToList();
            CbxTimeFreqSpan.DisplayMember = "Key";
            CbxTimeFreqSpan.ValueMember = "Value";
            CbxTimeFreqSpan.SelectedIndexChanged += SpanForTimeFreqSelectedIndexChanged;
            CbxTimeFreqSpan.SelectedIndex = MultiDomainPresenter.SpanOptForTimeFreq;
        }

        private void InitNebTimeScale()
        {
            NebTimeScale.AddClicked = (a, b) => MultiDomainPresenter.TimeScaleForTimeFreq++;
            NebTimeScale.SubClicked = (a, b) => MultiDomainPresenter.TimeScaleForTimeFreq--;
            NebTimeScale.StringFormatFunc = (value) => TimeScaleToString();
            NebTimeScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    MultiDomainPresenter.TimeScaleForTimeFreq = (Double)data;

                nkf.SetKeyBoardValue(LblTimeScale.Text, QuantityUnit.Second.ToUnitString(), 9, onokclickeventaction,
                    MultiDomainPresenter.TimeScaleForTimeFreq, MultiDomainPresenter.MaxTimeScale, MultiDomainPresenter.MinTimeScale);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebTimeStep()
        {
            NebTimeStep.AddClicked = (a, b) => MultiDomainPresenter.TimeStep++;
            NebTimeStep.SubClicked = (a, b) => MultiDomainPresenter.TimeStep--;
            NebTimeStep.StringFormatFunc = (value) => TimeStepToString();
            NebTimeStep.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    MultiDomainPresenter.TimeStep = (Double)data;

                nkf.SetKeyBoardValue(LblTimeStep.Text, "", 9, onokclickeventaction,
                    MultiDomainPresenter.TimeStep, UInt16.MaxValue, 0);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitCbxWindowType()
        {
            CbxWindowType.DataSource = Enum.GetValues<RFWindowType>().Select(o => new KeyValuePair<RFWindowType, String>(o, o.GetAlias())).ToList();
            CbxWindowType.DisplayMember = "Value";
            CbxWindowType.ValueMember = "Key";

            CbxWindowType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    MultiDomainPresenter.WindowType = (RFWindowType)CbxWindowType.SelectedIndex;
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

        public MultiDomainPrsnt MultiDomainPresenter
        {
            get => (MultiDomainPrsnt)(ParentForm as IMultiDomainView).Presenter;
            set => (ParentForm as IMultiDomainView).Presenter = value;
        }

        IMultiDomainPrsnt IView<IMultiDomainPrsnt>.Presenter
        {
            get => MultiDomainPresenter;
            set => MultiDomainPresenter = (MultiDomainPrsnt)value;
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (ParentForm != null)
            {
                InitCbxSpanForTimeFreq();
                UpdateView();
            }
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
                NebTimeScale.UpdateValueString();
                NebTimeStep.UpdateValueString();

                CbxFFTLength.Text = MultiDomainPresenter.FFTLength.ToString();
                CbxSTFTDataLength.Text = (MultiDomainPresenter.STFTLength.ToString());
                CbxSTFTStep.Text = (MultiDomainPresenter.STFTStep.ToString());
                CbxWindowType.SelectedIndex = (Int32)MultiDomainPresenter.WindowType;
                CbxTimeFreqSpan.SelectedIndex = (Int32)MultiDomainPresenter.SpanOptForTimeFreq;

                _ArgToCtrl = false;
            }

        }

        //private String FreqScaleToString() => new Quantity(MultiDomainPresenter.FrequencyScale, Prefix.Empty, "Hz").ToString("#0.#########", true);

        //private String HorizontalCenterFreqToString() => new Quantity(Presenter.FigureCenterFrequency, Prefix.Empty, "Hz").ToString("#0.#########", true);

        private String CenterFrequencyToString() => new Quantity(MultiDomainPresenter.CenterFreqByHz, Prefix.Empty, "Hz").ToString("#0.#########", true);

        private String StartFrequencyToString() => new Quantity(MultiDomainPresenter.StartFreqByHz, Prefix.Empty, "Hz").ToString("#0.#########", true);

        private String EndFrequencyToString() => new Quantity(MultiDomainPresenter.EndFreqByHz, Prefix.Empty, "Hz").ToString("#0.#########", true);

        private String SpanToString() => new Quantity(MultiDomainPresenter.SpanFreqByHz, Prefix.Empty, "Hz").ToString("#0.#########", true);

        private String RBWToString() => new Quantity(MultiDomainPresenter.RBWByHz, Prefix.Empty, "").ToString("#0.#########", true);

        private static Double NormalizeHzInput(Double value) => Math.Round(value, 0, MidpointRounding.AwayFromZero);

        private String TimeScaleToString() => new Quantity(MultiDomainPresenter.TimeScaleForTimeFreq, Prefix.Empty, "s").ToString("#0.#########", true);

        private String TimeStepToString() => new Quantity(MultiDomainPresenter.TimeStep, Prefix.Empty, "").ToString("#0.#########", true);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitCbxSpanForTimeFreq();
            UpdateView();
        }

        private void CbxFFTLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.FFTLength = Convert.ToInt32(CbxFFTLength.Items[CbxFFTLength.SelectedIndex]);
                //if (Presenter.RoughSpec)
                //{
                //    Presenter.FFTLength = Convert.ToInt32(CbxFFTLength.Items[7]);
                //}
                //else
                //{
                //    Presenter.FFTLength = Convert.ToInt32(CbxFFTLength.Items[CbxFFTLength.SelectedIndex]);
                //}
            }
        }

        private void CbxSTFTDataLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                MultiDomainPresenter.STFTLength = Convert.ToInt32(CbxSTFTDataLength.Items[CbxSTFTDataLength.SelectedIndex]);
        }

        private void CbxSTFTStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                MultiDomainPresenter.STFTStep = Convert.ToInt32(CbxSTFTStep.Items[CbxSTFTStep.SelectedIndex]);
        }

        //private void CbxWindowType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //        MultiDomainPresenter.WindowType = (RFWindowType)CbxWindowType.SelectedIndex;
        //}
    }
}
