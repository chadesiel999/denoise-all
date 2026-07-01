namespace ScopeX.U2
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Helper;

    public partial class AmplitudeSettingForm : FloatForm, IPwrOptionView, IStylize
    {

        private Boolean _ArgToCtrl;

        public AmplitudeSettingForm()
        {
            InitializeComponent();
            this.FixedToolIconInfos[2].IsShow = false;
        }

        public IPresenter<IPwrOptionView> Presenter { get; set; }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set => Presenter = value;
        }

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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.LoopAnalysis;
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case "AmpValue":
                    AmpValueRefresh();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                TbxAmplitudeOne.Text = ValueToString(GetAmpValue(0));
                TbxAmplitudeTwo.Text = ValueToString(GetAmpValue(1));
                TbxAmplitudeThree.Text = ValueToString(GetAmpValue(2));
                TbxAmplitudeFour.Text = ValueToString(GetAmpValue(3));
                TbxAmplitudeFive.Text = ValueToString(GetAmpValue(4));
                TbxAmplitudeSix.Text = ValueToString(GetAmpValue(5));
                TbxAmplitudeSeven.Text = ValueToString(GetAmpValue(6));
                TbxAmplitudeEight.Text = ValueToString(GetAmpValue(7));

                _ArgToCtrl = false;
            }
        }

        private Double GetAmpValue(Int32 index)
        {
            Double value = 0;
            if (Presenter is PwrLoopAnalysisPrsnt loop)
            {
                value = loop.AmpValue[index];
            }
            else if (Presenter is PwrPSRRPrsnt psrr)
            {
                value = psrr.AmpValue[index];
            }
            else
            {
                value = Double.NaN;
            }

            return value;
        }

        private (Double Max, Double Min) GetAmpRange()
        {
            (Double, Double) value = (Double.NaN, Double.NaN);
            if (Presenter is PwrLoopAnalysisPrsnt loop)
            {
                value = (loop.MaxAmplitude, loop.MinAmplitude);
            }
            else if (Presenter is PwrPSRRPrsnt psrr)
            {
                value = (psrr.MaxAmplitude, psrr.MinAmplitude);
            }

            return value;
        }

        private void SetAmplitudeValue(Int32 index, Double value)
        {
            if (Presenter is PwrLoopAnalysisPrsnt loop)
            {
                loop.SetAmplitudeValue(index, value);
            }
            else if (Presenter is PwrPSRRPrsnt psrr)
            {
                psrr.SetAmplitudeValue(index, value);
            }
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void AmpValueRefresh()
        {
            TbxAmplitudeOne.Text = ValueToString(GetAmpValue(0));
        }
        private String ValueToString(double value)
        {
            return new Quantity(value, Prefix.Milli, QuantityUnit.Voltage).ToString("##0.###", true);
        }

        private void TbxAmplitude_Click(object sender, EventArgs e)
        {
            ScopeXTextBox tbox = sender as ScopeXTextBox;
            Int32 index = 0;
            String title=String.Empty;

            switch (tbox.Name)
            {
                case nameof(TbxAmplitudeOne):
                    index = 0;
                    title = LblAmplitudeOne.Text;
                    break;
                case nameof(TbxAmplitudeTwo):
                    index = 1;
                    title = LblAmplitudeTwo.Text;
                    break;
                case nameof(TbxAmplitudeThree):
                    index = 2;
                    title = LblAmplitudeThree.Text;
                    break;
                case nameof(TbxAmplitudeFour):
                    index = 3;
                    title = LblAmplitudeFour.Text;
                    break;
                case nameof(TbxAmplitudeFive):
                    index = 4;
                    title = LblAmplitudeFive.Text;
                    break;
                case nameof(TbxAmplitudeSix):
                    index = 5;
                    title = LblAmplitudeSix.Text;
                    break;
                case nameof(TbxAmplitudeSeven):
                    index = 6;
                    title = LblAmplitudeSeven.Text;
                    break;
                case nameof(TbxAmplitudeEight):
                    index = 7;
                    title = LblAmplitudeEight.Text;
                    break;
            }


            var value = GetAmpValue(index);
            var range = GetAmpRange();
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, tbox);
            Action<Double> onokclickeventaction = new Action<Double>(
                (data) => value = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli)
                );

            nkf.SetKeyBoardValue(title, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
                Quantity.ConvertByPrefix(value, Prefix.Milli, Prefix.Empty),
                    Quantity.ConvertByPrefix(range.Max, Prefix.Milli, Prefix.Empty),
                        Quantity.ConvertByPrefix(range.Min, Prefix.Milli, Prefix.Empty));

            nkf.ShowDialogByPosition();

            //value = GetAmpValue(index);
            SetAmplitudeValue(index, value);
            tbox.Text = ValueToString(value);
        }

        //private void TbxAmplitudeOne_Click(object sender, EventArgs e)
        //{
        //    var value0 = GetAmpValue(0);
        //    var range = GetAmpRange();
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeOne);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => value0 = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeOne.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(value0, Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(range.Max, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(range.Min, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    SetAmplitudeValue(0, GetAmpValue(0));
        //    TbxAmplitudeOne.Text = ValueToString(GetAmpValue(0));
        //}

        //private void TbxAmplitudeTwo_Click(object sender, EventArgs e)
        //{
        //    var value1 = GetAmpValue(1);
        //    var range = GetAmpRange();
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeTwo);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => value1 = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeTwo.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(value1, Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(range.Max, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(range.Min, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    value1 = GetAmpValue(1);
        //    SetAmplitudeValue(1, value1);
        //    TbxAmplitudeTwo.Text = ValueToString(value1);
        //}


        //private void TbxAmplitudeThree_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeThree);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[2] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeThree.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[2], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(0, Presenter.AmpValue[2]);
        //    TbxAmplitudeThree.Text = ValueToString(Presenter.AmpValue[2]);
        //}

        //private void TbxAmplitudeFour_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeFour);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[3] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeFour.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[3], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(3, Presenter.AmpValue[3]);
        //    TbxAmplitudeFour.Text = ValueToString(Presenter.AmpValue[3]);
        //}

        //private void TbxAmplitudeFive_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeFive);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[4] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeFive.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[4], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(4, Presenter.AmpValue[4]);
        //    TbxAmplitudeFive.Text = ValueToString(Presenter.AmpValue[4]);
        //}

        //private void TbxAmplitudeSix_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeSix);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[5] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeSix.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[5], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(5, Presenter.AmpValue[5]);
        //    TbxAmplitudeSix.Text = ValueToString(Presenter.AmpValue[5]);
        //}

        //private void TbxAmplitudeSeven_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeSeven);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[6] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeSeven.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[6], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(6, Presenter.AmpValue[6]);
        //    TbxAmplitudeSeven.Text = ValueToString(Presenter.AmpValue[6]);
        //}

        //private void TbxAmplitudeEight_Click(object sender, EventArgs e)
        //{
        //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxAmplitudeEight);
        //    Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.AmpValue[7] = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

        //    nkf.SetKeyBoardValue(LblAmplitudeEight.Text, QuantityUnit.Voltage.ToUnitString(), 3, onokclickeventaction,
        //        Quantity.ConvertByPrefix(Presenter.AmpValue[7], Prefix.Milli, Prefix.Empty),
        //            Quantity.ConvertByPrefix(Presenter.MaxAmplitude, Prefix.Milli, Prefix.Empty),
        //                Quantity.ConvertByPrefix(Presenter.MinAmplitude, Prefix.Milli, Prefix.Empty));

        //    nkf.ShowDialogByPosition();

        //    Presenter.SetAmplitudeValue(7, Presenter.AmpValue[7]);
        //    TbxAmplitudeEight.Text = ValueToString(Presenter.AmpValue[7]);
        //}
    }
}
