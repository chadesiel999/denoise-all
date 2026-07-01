using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.UserControls;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class PwrHarmonicPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrHarmonicPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer| ControlStyles.AllPaintingInWmPaint| ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
            InitHotKnob();
        }

        private void InitComboxList()
        {
            NebCustomFreq.AddClicked = (a, b) => Presenter.AdjRefFreq(1);
            NebCustomFreq.SubClicked = (a, b) => Presenter.AdjRefFreq(-1);
            NebCustomFreq.StringFormatFunc = (value) => FreqToString();
            NebCustomFreq.EditValueChicked = (a, b) =>
            {
                var numberkeybordform = new NumberKeybordForm().UniformInitKeyBoard(this, NebCustomFreq);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.CustomRefFreq = (Int64)Math.Round(data, MidpointRounding.AwayFromZero));

                numberkeybordform.SetKeyBoardValue(LblRefFreq.Text, QuantityUnit.Hertz.ToUnitString(), 2, onokclickeventaction,
                    Presenter.CustomRefFreq,
                    Presenter.MaxCustomFreq,
                    Presenter.MinCustomFreq);

                numberkeybordform.ShowDialogByPosition();
            };

            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();
            CbxVoltageSrc.DataSource = dss;
            //selectTouch1.SelectValue = PowerPresenter.VoltageSrc;
            //selectTouch1.Text = PowerPresenter.VoltageSrc.ToString();

            CbxVoltageSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.VoltageSrc1 = (ChannelId)CbxVoltageSrc.SelectValue;
                }
            };

            CbxCurrentSrc.DataSource = dss;
            //selectTouch2.SelectValue = PowerPresenter.CurrentSrc;
            //selectTouch2.Text = PowerPresenter.CurrentSrc.ToString();

            CbxCurrentSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
                }
            };

            //    var ds = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
            //        Select(x => new KeyValuePair<String, ChannelId>(x.GetDescription(), x)).ToList();
            //    CbxVoltageSrc.DataSource = ds;
            //    CbxVoltageSrc.DisplayMember = "Key";
            //    CbxVoltageSrc.ValueMember = "Value";

            //    CbxVoltageSrc.SelectedIndexChanged += (_, _) =>
            //    {
            //        if (!_ArgToCtrl)
            //        {
            //            PowerPresenter.VoltageSrc = (ChannelId)CbxVoltageSrc.SelectedIndex;
            //        }
            //    };

            //    CbxCurrentSrc.DataSource = ds;
            //    CbxCurrentSrc.DisplayMember = "Key";
            //    CbxCurrentSrc.ValueMember = "Value";

            //    CbxCurrentSrc.SelectedIndexChanged += (_, _) =>
            //    {
            //        if (!_ArgToCtrl)
            //        {
            //            PowerPresenter.CurrentSrc = (ChannelId)CbxCurrentSrc.SelectedIndex;
            //        }
            //    };
        }
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }
        public PwrHarmonicPrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrHarmonicPrsnt)value;
            }
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Harmonic;


        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Source):
                    RdoSource.ChoosedButtonIndex = (Int32)Presenter.Source;
                    break;
                case nameof(Presenter.Unit):
                    RdoUnit.ChoosedButtonIndex = (Int32)Presenter.Unit;
                    break;
                case nameof(Presenter.RefFreqSrc):
                    CbxRefFreq.SelectIndex =(Int32) Presenter.RefFreqSrc;
                    CfgHarmonicPage();
                    break;
                case nameof(Presenter.HarmonicNumIdx):
                    CbxHarmonicNum.SelectValue = Presenter.HarmonicNumIdx;
                    break;
                case nameof(Presenter.HarmonicNum):
                    TbxHarmonicNum.Text = Presenter.HarmonicNum.ToString();
                    break;
                case nameof(Presenter.HarmonicOpt):
                    RdoHarmonicOpt.ChoosedButtonIndex = (Int32)Presenter.HarmonicOpt;
                    break;
                case nameof(Presenter.CustomRefFreq):
                    //NebCustomFreq.UpdateValueString();
                    TbxCustomFreq.Text = FreqToString();
                    break;
                case nameof(PowerPresenter.Active):
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(PowerPresenter.VoltageSrc1):
                    //CbxVoltageSrc.SelectedIndex = (Int32)PowerPresenter.VoltageSrc;
                    CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                    break;
                case nameof(PowerPresenter.CurrentSrc1):
                    //CbxCurrentSrc.SelectedIndex = (Int32)PowerPresenter.CurrentSrc;
                    CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                    break;
            }
            //CfgHarmonicPage();
            _ArgToCtrl = false;
            
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = PowerPresenter.Active;
                //CbxVoltageSrc.SelectedIndex = (Int32)PowerPresenter.VoltageSrc;
                CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                //CbxCurrentSrc.SelectedIndex = (Int32)PowerPresenter.CurrentSrc;
                CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                RdoHarmonicOpt.ChoosedButtonIndex = (Int32)Presenter.HarmonicOpt;
                RdoSource.ChoosedButtonIndex = (Int32)Presenter.Source;
                RdoUnit.ChoosedButtonIndex = (Int32)Presenter.Unit;
                //CbxRefFreq.SelectedIndex = (Int32)Presenter.RefFreqSrc;
                CbxRefFreq.SelectIndex =(Int32) Presenter.RefFreqSrc;
                //CbxHarmonicNum.SelectedIndex = Presenter.HarmonicNumIdx;
                CbxHarmonicNum.SelectValue = Presenter.HarmonicNumIdx;
                NebCustomFreq.UpdateValueString();
                TbxHarmonicNum.Text = Presenter.HarmonicNum.ToString();
                TbxCustomFreq.Text = FreqToString();
                CfgHarmonicPage();
                _ArgToCtrl = false;

            }
        }
        private void InitHotKnob()
        {
            ControlsHotKnob.Default.InitHotKnob(TbxHarmonicNum);
            TbxHarmonicNum.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxHarmonicNum, nameof(Presenter.HarmonicNum));
            };

            ControlsHotKnob.Default.InitHotKnob(TbxCustomFreq);
            TbxCustomFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, TbxCustomFreq, nameof(Presenter.CustomRefFreq));
            };
        }
        private void CfgHarmonicPage()
        {
            //NebCustomFreq.Visible = Presenter.RefFreqSrc == HarmonicRefFreqSrc.Constant;
            LblCutomFreq.Visible = Presenter.RefFreqSrc == HarmonicRefFreqSrc.Constant;
            TbxCustomFreq.Visible = LblCutomFreq.Visible;
            this.Refresh();
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
        private String FreqToString()
        {
            return new Quantity(Presenter.CustomRefFreq, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.#", true);
        }
        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowHarmonicGuideForm();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }

        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }
        private void BtnPowerPic_Click(object sender, EventArgs e)
        {
            //PowerPresenter.BoundMathPrsnt.Active = true;
            //Presenter.TryShowHarmonicWfm(PowerPresenter.BoundMathPrsnt);
            //DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt.Id;
            //var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt.Id);
            //if (fig is BaseDisplayForm form)
            //{
            //    form.Activate();
            //}
            (Program.Oscilloscope.View as DsoForm).TryAddPwrHarmonicUI(PowerPresenter);
        }

        private void RdoHarmonicOpt_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.HarmonicOpt = (HarmonicDisplayOpt)RdoHarmonicOpt.ChoosedButtonIndex;
            }
        }

        private void RdoSource_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (VIType)RdoSource.ChoosedButtonIndex;
            }
        }

        private void RdoUnit_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Unit = (SweepType)RdoUnit.ChoosedButtonIndex;
            }
        }

        //private void CbxHarmonicNum_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.HarmonicNumIdx = CbxHarmonicNum.SelectedIndex;
        //    }
        //}

        //private void CbxRefFreq_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.RefFreqSrc = (HarmonicRefFreqSrc)CbxRefFreq.SelectedIndex;
        //    }
        //}

        private void CbxHarmonicNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.HarmonicNumIdx = (Int32)CbxHarmonicNum.SelectValue;
            }
        }

        private void CbxRefFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RefFreqSrc = (HarmonicRefFreqSrc)CbxRefFreq.SelectIndex;
            }
        }

        private void TbxHarmonicNum_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxHarmonicNum);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.HarmonicNum = Convert.ToInt32(data));

            nkf.SetKeyBoardValue(LblHarmonicNum.Text, "", 3, onokclickeventaction,
                Presenter.HarmonicNum, Presenter.MaxHarmonicNum, Presenter.MinHarmonicNum);

            nkf.ShowDialogByPosition();
        }

        private void TbxCustomFreq_Click(object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, TbxCustomFreq);
            Action<Double> onokclickeventaction = new Action<Double>((data) => Presenter.CustomRefFreq = Convert.ToInt64(data));

            nkf.SetKeyBoardValue(LblCutomFreq.Text, QuantityUnit.Hertz.ToUnitString(), 3, onokclickeventaction,
                Presenter.CustomRefFreq, Presenter.MaxCustomFreq, Presenter.MinCustomFreq);

            nkf.ShowDialogByPosition();
        }
    }
}
