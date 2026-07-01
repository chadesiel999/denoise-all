using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using ScopeX.UserControls.Style;
using System.IO;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;


namespace ScopeX.U2 
{ 
    public partial class RFVerticalPage : UserControl, IChnlView, IStylize
    {
        

        #region Field&Property

        private Boolean _ArgToCtrl;

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
        #endregion Field&Property

        public RFVerticalPage()
        {
            InitializeComponent();
            Init();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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
            NebAmpScale.StringFormatFunc = (value) => AmpScaleToString();
            NebAmpScale.EditValueChicked = (a, b) =>
            {
                var numberkeybordform = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpScale);
                var onokclickeventaction = new Action<double>((data) =>
                {
                    Presenter.AmpScale = SIHelper.SIUnitConversion(data, (Int32)Prefix.Empty, (Int32)Presenter.Prefix);
                });

                numberkeybordform.SetKeyBoardValue(LblAmpScale.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction
                    , SIHelper.SIUnitConversion(Presenter.AmpScale, (Int32)Presenter.Prefix, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_AMP_MAX_SCALE, (Int32)Presenter.Prefix, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_AMP_MIN_SCALE, (Int32)Presenter.Prefix, (Int32)Prefix.Empty));
                numberkeybordform.Location = numberkeybordform.CalculateWindowPosition();
                numberkeybordform.ShowDialogByEvent();
            };

            //NebAmpCenter
            ControlsHotKnob.Default.InitHotKnob(NebAmpCenter);
            NebAmpCenter.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAmpCenter);
            };
            NebAmpCenter.AddClicked = (a, b) => Presenter.FigureCenterAmplitude++;
            NebAmpCenter.SubClicked = (a, b) => Presenter.FigureCenterAmplitude--;
            NebAmpCenter.StringFormatFunc = (value) => AmpCenterToString();
            NebAmpCenter.EditValueChicked = (a, b) =>
            {
                var numberkeybordform = new NumberKeybordForm().UniformInitKeyBoard(this, NebAmpCenter);
                var onokclickeventaction = new Action<double>((data) =>
                {
                    Presenter.FigureCenterAmplitude = SIHelper.SIUnitConversion(data, (Int32)Prefix.Empty, (Int32)Prefix.Milli);
                });

                numberkeybordform.SetKeyBoardValue(LblAmpCenter.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction
                    , SIHelper.SIUnitConversion(Presenter.FigureCenterAmplitude, (Int32)Prefix.Empty, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_CENTER_AMPLITUDE_MAX, (Int32)Prefix.Empty, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_CENTER_AMPLITUDE_MIN, (Int32)Prefix.Empty, (Int32)Prefix.Empty));

                numberkeybordform.Location = numberkeybordform.CalculateWindowPosition();
                DialogResult dialogresult = numberkeybordform.ShowDialogByEvent();
            };

            //NebReferenceLevel
            ControlsHotKnob.Default.InitHotKnob(NebReferenceLevel);
            NebReferenceLevel.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebReferenceLevel);
            };
            NebReferenceLevel.AddClicked = (a, b) => Presenter.RefLevelValue++;
            NebReferenceLevel.SubClicked = (a, b) => Presenter.RefLevelValue--;
            NebReferenceLevel.StringFormatFunc = (value) => ReferenceLevelToString();
            NebReferenceLevel.EditValueChicked = (a, b) =>
            {
                var numberkeybordform = new NumberKeybordForm().UniformInitKeyBoard(this, NebReferenceLevel);
                var onokclickeventaction = new Action<double>((data) =>
                {
                    Presenter.RefLevelValue = SIHelper.SIUnitConversion(data, (Int32)Prefix.Empty, (Int32)Prefix.Empty);
                });

                numberkeybordform.SetKeyBoardValue(LblReferenceLevel.Text, Presenter.PUnit.ToString(), 9, onokclickeventaction
                    , SIHelper.SIUnitConversion(Presenter.RefLevelValue, (Int32)Prefix.Empty, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_REF_LEVEL_MAX, (Int32)Prefix.Empty, (Int32)Prefix.Empty)
                    , SIHelper.SIUnitConversion(Constants.RF_REF_LEVEL_MIN, (Int32)Prefix.Empty, (Int32)Prefix.Empty));

                numberkeybordform.Location = numberkeybordform.CalculateWindowPosition();
                DialogResult dialogresult = numberkeybordform.ShowDialogByEvent();
            };
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
            //switch (propertyName)
            //{
                
            //}
            NebAmpScale.UpdateValueString();
            NebAmpCenter.UpdateValueString();
            NebReferenceLevel.UpdateValueString();
            CbxWindowType.SelectedIndex = (Int32)Presenter.Window;
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                NebAmpScale.UpdateValueString();
                NebAmpCenter.UpdateValueString();
                NebReferenceLevel.UpdateValueString();
                CbxWindowType.SelectedIndex = (Int32)Presenter.Window;
                _ArgToCtrl = false;
            }

        }


        private String AmpScaleToString() => new Quantity(Presenter.AmpScale, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);

        private String AmpCenterToString() => new Quantity(Presenter.FigureCenterAmplitude, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);

        private String ReferenceLevelToString() => new Quantity(Presenter.RefLevelValue, Prefix.Empty, Presenter.PUnit.ToString()).ToString("#0.#########", true);
        #region 事件处理相关
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }



        #endregion 事件处理相关

        private void CbxWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                Presenter.Window = (RFWindowType)CbxWindowType.SelectedIndex;
        }

        //private void BtnExportWindowCoe_Click(object sender, EventArgs e)
        //{
        //    //RFWindowType windowType = Presenter.Window;
        //    //Int32 length = Presenter.FFTLength;
        //    //List<double> coefficient = AbstractAcquirer_RadioFrequency.GetWindowCoefficient(length, windowType).ToList();
        //    //double[] SaveData = coefficient.ToArray();
        //    //SaveDataToFile(SaveData);

        //}
        private void SaveDataToFile(double[] data)
        {
            StreamWriter sw = new StreamWriter("C:Users\\Administrator\\Desktop\\WindowCoefficient.txt", false);
            for (int i = 0; i < data.Length; i++)
            {
                sw.WriteLine(data[i]);
            }
            sw.Close();
        }
    }
}
