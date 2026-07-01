using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using static ScopeX.Controls.Common.APIs.APIsStructs;

namespace ScopeX.U2
{
    public partial class FilterDesignForm :FlashBorderForm, IFilterView
    {
        private Boolean _ArgToCtrl;

        public FilterDesignForm()
        {
            InitializeComponent();
            InitFigure();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(FilterDesignForm)));
            };
        }

        private void InitFigure()
        {
            Figure.Plot.Style(figureBackground: Color.FromArgb(53, 54, 58), dataBackground: Color.Black, grid: Color.Gray, tick: Color.Gray, axisLabel: Color.White, titleLabel: Color.Gray);
            Figure.Plot.Grid(enable: true, color: Color.Gray, lineStyle: ScottPlot.LineStyle.Dot);
            Figure.Plot.GridCrosslineVisible(false);
            Figure.Plot.SetPlotMargin((25, 20, 25, 20));
        }

        private readonly String[] _FIRMethods = {
            //Properties.Resources.FilterDesign_FreqSampling,
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_FreqSampling"),
            //Properties.Resources.FilterDesign_Window,
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_Window"),
            //Properties.Resources.FilterDesign_Remez
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_Remez"),
        };

        private readonly String[] _IIRMethods = {
            //Properties.Resources.FilterDesign_Butterworth,
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_Butterworth"),
            //Properties.Resources.FilterDesign_ChebyshevI,
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_ChebyshevI"),
            //Properties.Resources.FilterDesign_ChebyshevII,
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_ChebyshevII"),
            //Properties.Resources.FilterDesign_Elliptic
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_Elliptic"),
        };

        private void CfgDesignForm()
        {

            if (CbxMethod.Tag is null || (FilterType)CbxMethod.Tag != Presenter.FilterType)
            {
                if (Presenter.FilterType == FilterType.FIRFilter)
                {
                    CbxMethod.Items = _FIRMethods;
                    CbxMethod.Tag = FilterType.FIRFilter;
                    //ScopeX.UserControls.SelectComboBox1.Text = ScopeX.UserControls.SelectComboBox1.Items[(Int32)Presenter.FIRMethod];
                    //CbxMethod.SelectIndex = (Int32)Presenter.FIRMethod;
                }
                else
                {
                    CbxMethod.Items = _IIRMethods;
                    CbxMethod.Tag = FilterType.IIRFilter;
                    //CbxMethod.SelectIndex= (Int32)Presenter.IIRMethod;
                }
            CbxMethod.SelectIndex = Presenter.FilterType == FilterType.FIRFilter ? (Int32)Presenter.FIRMethod : (Int32)Presenter.IIRMethod;

            }

            if (Presenter.CurrentDesignerConfig.IsShowMinOrder)
            {
                RdoFilterOrder.Enabled = true;
            }
            else
            {
                RdoFilterOrder.Enabled = false;
                NebFilterOrder.Visible = true;
                if(RdoFilterOrder.ChoosedButtonIndex == (Int32)FilterOrderMode.Minimum)
                {
                    RdoFilterOrder.ChoosedButtonIndex = (Int32)FilterOrderMode.UserDefined;
                }
            }

            var limit = Presenter.CurrentDesignerConfig[(Int32)Presenter.RespType, RdoFilterOrder.ChoosedButtonIndex];

            if(limit!=null)
            {
                NebLowPassFreq.Enabled = limit.IsShowFp1;
                NebLowStopFreq.Enabled = limit.IsShowFs1;
                NebHighPassFreq.Enabled = limit.IsShowFp2;
                NebHighStopFreq.Enabled = limit.IsShowFs2;
                if(limit.IsShowMag)
                {
                    LblPassMag.Visible= limit.IsShowPassMag;
                    NebPassMag.Visible = limit.IsShowPassMag;
                    NebPassMag.Enabled = limit.IsShowPassMag;
                    LblStopMag.Visible= limit.IsShowStopMag;
                    NebStopMag.Visible = limit.IsShowStopMag;
                    NebStopMag.Enabled = limit.IsShowStopMag;
                    LblMag.Visible = false;
                }
                else
                {
                    LblMag.Visible = true;
                    LblPassMag.Visible= false;
                    NebPassMag.Visible = false;
                    NebPassMag.Enabled = false;
                    LblStopMag.Visible= false;
                    NebStopMag.Visible = false;
                    NebStopMag.Enabled = false;
                }
            }

            GrpOption.Enabled = Presenter.FIRMethod == FIRType.Window && Presenter.FilterType == FilterType.FIRFilter;

            //EnableButtonByRespType();
        }

        private void InitOnLoad()
        {
            ControlsHotKnob.Default.InitHotKnob(NebFilterOrder);
            NebFilterOrder.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebFilterOrder, nameof(Presenter.Order));
            };
            NebFilterOrder.StringFormatFunc = (value) => Presenter.Order.ToString();
            NebFilterOrder.AddClicked = (a, b) => Presenter.Order++;
            NebFilterOrder.SubClicked = (a, b) => Presenter.Order--;
            NebFilterOrder.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFilterOrder);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.Order = (Int32)data);

                nkf.SetKeyBoardValue(LblHighStopFreq.Text, "", 3, onokclickeventaction,
                    Presenter.Order,
                    Presenter.FilterType== FilterType.FIRFilter?FilterPrsnt.FIRMaxOrder: FilterPrsnt.IIRMaxOrder,
                    FilterPrsnt.MinOrder);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebLowPassFreq);
            NebLowPassFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLowPassFreq, nameof(Presenter.LowPassFreq));
            };
            NebLowPassFreq.StringFormatFunc = (value) => FreqToString(Presenter.LowPassFreq);
            NebLowPassFreq.AddClicked = (a, b) => Presenter.LowPassFreq++;
            NebLowPassFreq.SubClicked = (a, b) => Presenter.LowPassFreq--;
            NebLowPassFreq.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowPassFreq);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.LowPassFreq = (Int32)Math.Round(data * Presenter.FreqFactor, MidpointRounding.AwayFromZero));

                var limit = Presenter.GetFreqLimitValue(Presenter.CurrentDesignerConfig[(Int32)Presenter.RespType, (Int32)Presenter.OrderMode].Fp1Limit);

                nkf.SetKeyBoardValue(LblLowPassFreq.Text, "", 3, onokclickeventaction,
                    Presenter.LowPassFreq / Presenter.FreqFactor,
                    limit.max,
                    limit.min);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebLowStopFreq);
            NebLowStopFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebLowStopFreq, nameof(Presenter.LowStopFreq));
            };
            NebLowStopFreq.StringFormatFunc = (value) => FreqToString(Presenter.LowStopFreq);
            NebLowStopFreq.AddClicked = (a, b) => Presenter.LowStopFreq++;
            NebLowStopFreq.SubClicked = (a, b) => Presenter.LowStopFreq--;
            NebLowStopFreq.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebLowStopFreq);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.LowStopFreq = (Int32)Math.Round(data * Presenter.FreqFactor, MidpointRounding.AwayFromZero));

                var limit = Presenter.GetFreqLimitValue(Presenter.CurrentDesignerConfig[(Int32)Presenter.RespType, (Int32)Presenter.OrderMode].Fs1Limit);
                nkf.SetKeyBoardValue(LblLowStopFreq.Text, "", 3, onokclickeventaction,
                    Presenter.LowStopFreq / Presenter.FreqFactor,
                    limit.max,
                    limit.min);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebHighPassFreq);
            NebHighPassFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHighPassFreq, nameof(Presenter.HighPassFreq));
            };
            NebHighPassFreq.StringFormatFunc = (value) => FreqToString(Presenter.HighPassFreq);
            NebHighPassFreq.AddClicked = (a, b) => Presenter.HighPassFreq++;
            NebHighPassFreq.SubClicked = (a, b) => Presenter.HighPassFreq--;
            NebHighPassFreq.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHighPassFreq);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.HighPassFreq = (Int32)Math.Round(data * Presenter.FreqFactor, MidpointRounding.AwayFromZero));

                var limit = Presenter.GetFreqLimitValue(Presenter.CurrentDesignerConfig[(Int32)Presenter.RespType, (Int32)Presenter.OrderMode].Fp2Limit);
                nkf.SetKeyBoardValue(LblHighPassFreq.Text, "", 3, onokclickeventaction,
                    Presenter.HighPassFreq / Presenter.FreqFactor,
                    limit.max,
                    limit.min);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebHighStopFreq);
            NebHighStopFreq.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHighStopFreq, nameof(Presenter.HighStopFreq));
            };
            NebHighStopFreq.StringFormatFunc = (value) => FreqToString(Presenter.HighStopFreq);
            NebHighStopFreq.AddClicked = (a, b) => Presenter.HighStopFreq++;
            NebHighStopFreq.SubClicked = (a, b) => Presenter.HighStopFreq--;
            NebHighStopFreq.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHighStopFreq);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.HighStopFreq = (Int32)Math.Round(data * Presenter.FreqFactor, MidpointRounding.AwayFromZero));

                var limit = Presenter.GetFreqLimitValue(Presenter.CurrentDesignerConfig[(Int32)Presenter.RespType, (Int32)Presenter.OrderMode].Fs2Limit);
                nkf.SetKeyBoardValue(LblHighStopFreq.Text, "", 3, onokclickeventaction,
                    Presenter.HighStopFreq / Presenter.FreqFactor,
                    limit.max,
                    limit.min);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebPassMag);
            NebPassMag.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPassMag, nameof(Presenter.PassMag));
            };
            NebPassMag.StringFormatFunc = (value) => MagnitudeToString(Presenter.PassMag);
            NebPassMag.AddClicked = (a, b) => Presenter.AdjPassMag(1);
            NebPassMag.SubClicked = (a, b) => Presenter.AdjPassMag(-1);
            NebPassMag.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPassMag);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.PassMag = (Int32)Math.Round(data * Presenter.MagFactor, MidpointRounding.AwayFromZero));

                nkf.SetKeyBoardValue(LblPassMag.Text, QuantityUnit.Decibel.ToUnitString(), 2, onokclickeventaction,
                    Presenter.PassMag / Presenter.MagFactor,
                    (Presenter.StopMag - 1) / Presenter.MagFactor,
                    Presenter.MinMagnitude);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebStopMag);
            NebStopMag.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebStopMag, nameof(Presenter.StopMag));
            };
            NebStopMag.StringFormatFunc = (value) => MagnitudeToString(Presenter.StopMag);
            NebStopMag.AddClicked = (a, b) => Presenter.AdjStopMag(1);
            NebStopMag.SubClicked = (a, b) => Presenter.AdjStopMag(1);
            NebStopMag.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebStopMag);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.StopMag = (Int32)Math.Round(data * Presenter.MagFactor, MidpointRounding.AwayFromZero));

                nkf.SetKeyBoardValue(LblStopMag.Text, QuantityUnit.Decibel.ToUnitString(), 2, onokclickeventaction,
                    Presenter.StopMag / Presenter.MagFactor,
                    Presenter.MaxMagnitude / Presenter.MagFactor,
                    (Presenter.PassMag + 1) / Presenter.MagFactor);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
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

        public FilterPrsnt Presenter
        {
            get;
            set;
        }

        IFilterPrsnt IView<IFilterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (FilterPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitOnLoad();
            Stylize();
            UpdateView();
            Render();
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);`
            ActiveBorderColor = Color.DeepSkyBlue;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
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

        public void UpdateView(Object prsnt, String propertyName = "")
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
                case nameof(Presenter.LowPassFreq):
                    NebLowPassFreq.UpdateValueString();
                    break;

                case nameof(Presenter.LowStopFreq):
                    NebLowStopFreq.UpdateValueString();
                    break;

                case nameof(Presenter.HighPassFreq):
                    NebHighPassFreq.UpdateValueString();
                    break;

                case nameof(Presenter.HighStopFreq):
                    NebHighStopFreq.UpdateValueString();
                    break;

                case nameof(Presenter.FilterType):
                case nameof(Presenter.FIRMethod):
                case nameof(Presenter.IIRMethod):
                    RdoMethod.ChoosedButtonIndex = (Int32)Presenter.FilterType;
                    if(Presenter.FilterType== FilterType.FIRFilter)
                    {
                        CbxMethod.SelectIndex = (Int32)Presenter.FIRMethod;
                    }
                    else
                    {
                        CbxMethod.SelectIndex = (Int32)Presenter.IIRMethod;
                    }
                    NebLowPassFreq.UpdateValueString();
                    NebLowStopFreq.UpdateValueString();
                    NebHighPassFreq.UpdateValueString();
                    NebHighStopFreq.UpdateValueString();
                    break;

                case nameof(Presenter.RespType):
                    //CbxRespType.SelectedIndex = (Int32)Presenter.RespType;
                    CbxRespType.SelectValue = (Int32)Presenter.RespType;
                    NebLowPassFreq.UpdateValueString();
                    NebLowStopFreq.UpdateValueString();
                    NebHighPassFreq.UpdateValueString();
                    NebHighStopFreq.UpdateValueString();
                    break;

                case nameof(Presenter.OrderMode):
                    RdoFilterOrder.ChoosedButtonIndex = (Int32)Presenter.OrderMode;
                    NebLowPassFreq.UpdateValueString();
                    NebLowStopFreq.UpdateValueString();
                    NebHighPassFreq.UpdateValueString();
                    NebHighStopFreq.UpdateValueString();
                    break;

                case nameof(Presenter.Order):
                    NebFilterOrder.UpdateValueString();
                    break;

                case nameof(Presenter.Window):
                    //CbxWindow.SelectedIndex = (Int32)Presenter.Window;
                    CbxWindow.SelectValue = (Int32)Presenter.Window;
                    break;

                case nameof(Presenter.PassMag):
                    NebPassMag.UpdateValueString();
                    break;

                case nameof(Presenter.StopMag):
                    NebStopMag.UpdateValueString();
                    break;
            }
            CfgDesignForm();
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                RdoMethod.ChoosedButtonIndex = (Int32)Presenter.FilterType;
                //CbxRespType.SelectedIndex = (Int32)Presenter.RespType;
                CbxRespType.SelectValue = (Int32)Presenter.RespType;
                RdoFilterOrder.ChoosedButtonIndex = (Int32)Presenter.OrderMode;
                //CbxWindow.SelectedIndex = (Int32)Presenter.Window;
                CbxWindow.SelectValue = (Int32)Presenter.Window;

                NebLowPassFreq.UpdateValueString();
                NebLowStopFreq.UpdateValueString();
                NebHighPassFreq.UpdateValueString();
                NebHighStopFreq.UpdateValueString();

                NebFilterOrder.UpdateValueString();

                NebPassMag.UpdateValueString();
                NebStopMag.UpdateValueString();

                CfgDesignForm();
            }
        }

        private String FreqToString(Double freq) => (freq / Presenter.FreqFactor).ToString("0.###");

        private String MagnitudeToString(Double mag) => (mag / Presenter.MagFactor).ToString("##0.## dB");

        private void RdoMethod_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FilterType = (FilterType)RdoMethod.ChoosedButtonIndex;
                RdoFilterOrder.ChoosedButtonIndex = (Int32)FilterOrderMode.UserDefined;
                NebFilterOrder.Visible = true;
            }
        }

        //private void CbxMethod_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        if (Presenter.FilterType == FilterType.FIRFilter)
        //        {
        //            Presenter.FIRMethod = (FIRType)CbxMethod.SelectedIndex;
        //        }
        //        else
        //        {
        //            Presenter.IIRMethod = (IIRType)CbxMethod.SelectedIndex;
        //        }
        //    }
        //}

        private void RdoFilterOrder_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.OrderMode = (FilterOrderMode)RdoFilterOrder.ChoosedButtonIndex;
                if(Presenter.OrderMode== FilterOrderMode.UserDefined)
                {
                    NebFilterOrder.Visible = true;
                }
                else
                {
                    NebFilterOrder.Visible = false;
                }
                //if (Presenter.OrderMode == FilterOrderMode.UserDefined)
                //{
                //    NebFilterOrder.Visible = true;
                //    LblPassMag.Visible = false;
                //    NebPassMag.Visible = false;
                //    LblStopMag.Visible = false;
                //    NebStopMag.Visible = false;
                //    LblMag.Visible = true;
                //}
                //else
                //{
                //    NebFilterOrder.Visible = false;
                //    LblPassMag.Visible = true;
                //    NebPassMag.Visible = true;
                //    LblStopMag.Visible = true;
                //    NebStopMag.Visible = true;
                //    LblMag.Visible = false;
                //}
            }
        }

        //private void CbxRespType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.RespType = (FilterResponseType)CbxRespType.SelectedIndex;
        //    }
        //}

        private void EnableButtonByRespType()
        {
            //Freq Button Enable
            switch (Presenter.RespType)
            {
                case FilterResponseType.LowPass:
                case FilterResponseType.HighPass:
                    NebLowPassFreq.Enabled = true;
                    var isfs = Presenter.FilterType == FilterType.FIRFilter && Presenter.FIRMethod == FIRType.FreqSampling;
                    NebHighPassFreq.Enabled = isfs;
                    NebLowStopFreq.Enabled = isfs;
                    NebHighStopFreq.Enabled = isfs;
                    break;
                case FilterResponseType.BandPass:
                case FilterResponseType.BandStop:
                    NebLowPassFreq.Enabled = true;
                    NebHighPassFreq.Enabled = true;
                    NebLowStopFreq.Enabled = false;
                    NebHighStopFreq.Enabled = false;
                    break;
            }

            //Mag Button Enable
            if (Presenter.FilterType == FilterType.IIRFilter)
            {
                switch (Presenter.IIRMethod)
                {
                    case IIRType.Butterworth:
                        NebPassMag.Enabled = false;
                        NebStopMag.Enabled = false;
                        break;
                    case IIRType.ChebyshevI:
                    case IIRType.ChebyshevII:
                        NebPassMag.Enabled = true;
                        NebStopMag.Enabled = false;
                        break;
                    case IIRType.Elliptic:
                        NebPassMag.Enabled = true;
                        NebStopMag.Enabled = true;
                        break;
                    case IIRType.Bessel:
                        NebPassMag.Enabled = false;
                        NebStopMag.Enabled = false;
                        break;
                }
            }
            else if (Presenter.FilterType == FilterType.FIRFilter)
            {
                NebPassMag.Enabled = false;
                NebStopMag.Enabled = false;
            }
        }

        //private void CbxWindow_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Window = (WindowType)CbxWindow.SelectedIndex;
        //    }
        //}

        private void RdoDiagram_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Render();
            }
        }

        private void BtnDesign_Click(object sender, EventArgs e)
        {
            Presenter.Design();

            RdoDiagram.ChoosedButtonIndex = 1;
            Render();
        }

        private void Render()
        {
            Figure.Plot.Clear();


            switch (RdoDiagram.ChoosedButtonIndex)
            {
                case 1:
                    RenderMagResp();
                    break;
                case 2:
                    RenderPhaResp();
                    break;
                case 3:
                    RenderImpResp();
                    break;
                case 4:
                    RenderZeroPoles();
                    break;

                default:
                    RenderTemplate();
                    break;
            }


            Figure.Render(true, true);
        }

        private void RenderEmpty()
        {
            Figure.Plot.SetAxisLimits(xMin: 0, xMax: 1, yMin: 0, yMax: 1);
            Figure.Plot.YAxis.MinorLogScale(false);
            Figure.Plot.ResetChannelParameter(0, 1, "", "", 1, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, 1, "", "", 1, true);
            Figure.Plot.AddText("NULL", 0.5, 0.5, 15, Color.Red);
        }

        private void RenderTemplate()
        {
            Figure.Plot.Clear();

            Figure.Plot.SetAxisLimits(xMin: 0, xMax: 1, yMin: -0.5, yMax: 1.5);
            Figure.Plot.YAxis.MinorLogScale(false);
            Figure.Plot.ResetChannelParameter(0, 1, "", "", 1, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, 0.1, "", "", 100, true);
            switch (Presenter.RespType)
            {
                case FilterResponseType.HighPass:
                    Figure.Plot.AddLine(0, 0, 0.3, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs1", 0.3, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.3, 0, 0.3, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.3, 0, 0.5, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp1", 0.5, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.5, 1, 0.5, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.5, 1, 1, 1, Color.Red, 1f);
                    break;
                case FilterResponseType.BandPass:
                    Figure.Plot.AddLine(0, 0, 0.3, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs1", 0.3, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.3, 0, 0.3, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.3, 0, 0.4, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp1", 0.4, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.4, 1, 0.4, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.4, 1, 0.7, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp2", 0.7, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.7, 1, 0.7, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.7, 1, 0.8, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs2", 0.8, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.8, 0, 0.8, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.8, 0, 1, 0, Color.Red, 1f);
                    break;
                case FilterResponseType.BandStop:
                    Figure.Plot.AddLine(0, 1, 0.3, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp1", 0.3, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.3, 1, 0.3, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.3, 1, 0.5, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs1", 0.5, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.5, 0, 0.5, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.5, 0, 0.6, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs2", 0.6, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.6, 0, 0.6, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.6, 0, 0.8, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp2", 0.8, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.8, 1, 0.8, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.8, 1, 1, 1, Color.Red, 1f);
                    break;

                default:
                    Figure.Plot.AddLine(0, 1, 0.3, 1, Color.Red, 1f);
                    Figure.Plot.AddText("fp1", 0.3, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.3, 1, 0.3, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.3, 1, 0.5, 0, Color.Red, 1f);
                    Figure.Plot.AddText("fs1", 0.5, -0.3, size: 13, Color.LightSkyBlue);
                    Figure.Plot.AddLine(0.5, 0, 0.5, -0.5, Color.Gray, 1f, ScottPlot.LineStyle.Dash);
                    Figure.Plot.AddLine(0.5, 0, 1, 0, Color.Red, 1f);
                    break;
            }

            Figure.Plot.YLabel("Magnitude (dB)");
            Figure.Plot.XLabel("Normalized Frequency");
        }

        private void RenderMagResp()
        {
            if (Presenter.Numerator is null)
            {
                RenderEmpty();
                return;
            }

            //IEnumerable<Complex> h;
            //IEnumerable<Double> w;
            Double[] mag;
            Int32 n = 1024;
            if (Presenter.Denominator is null)
            {
                var (h, _) = Presenter.Numerator.Freqz(n);
                var temp = Presenter.Numerator.FFT(WindowType.Rectangle, 2*n);
                mag = h.Select(o => 20.0 * Math.Log10(o.Magnitude)).Take(1024).ToArray();
            }
            else
            {
                var den = Presenter.Denominator;
                var (h, _) = Presenter.Numerator.Freqz(den, n);
                mag = h.Select(o => 20.0 * Math.Log10(o.Magnitude)).ToArray();
            }

            mag = mag.Where(x => double.IsFinite(x)).ToArray();
            var min = mag.Min();
            var max = mag.Max();
            if (!Double.IsFinite(min) || !Double.IsFinite(max)) 
            {
                return;
            }

            var vmax = GetMax(max);
            var vmin = GetMin(min);
            var yscale = Math.Ceiling((vmax - vmin) / 6.0);
            var amp = 50 / yscale;
            var offset =-(Math.Ceiling(vmax + vmin) / 2-0)*amp;

            Figure.Plot.SetAxisLimits(xMin: 0, xMax: 1000, yMin: -150, yMax: 150 );
            Figure.Plot.YAxis.MinorLogScale(false);
            //设置position=Y偏移值，scale=一个大格代表的数值，interval=一个大格代表的水平间隔
            Figure.Plot.ResetChannelParameter(offset, yscale, "", QuantityUnit.Decibel.ToUnitString(), 50, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, 0.1, "", "", 100, false);
            Figure.Plot.AddSignal(mag.Select(x=>x* amp+ offset).ToArray(), n / 1000.0, Color.Red);
            //var x = (Int32)Math.Round(1000 * Presenter.LowStopFreq / Presenter.FreqFactor, MidpointRounding.AwayFromZero);
            //Figure.Plot.AddCrosshair(x, mag[(Int32)Math.Round(1024 * Presenter.LowStopFreq / Presenter.FreqFactor, MidpointRounding.AwayFromZero)]);
            Figure.Plot.YLabel("Magnitude (dB)");
            Figure.Plot.XLabel("Normalized Frequency (×π rad/sample)");
        }

        private void RenderPhaResp()
        {
            if (Presenter.Numerator is null)
            {
                RenderEmpty();
                return;
            }

            Double[] pha;
            Int32 n = 1024;
            if (Presenter.Denominator is null)
            {
                var (h, _) = Presenter.Numerator.Freqz(n);
                pha = h.PhaseSpectrum().ToArray();
            }
            else
            {
                var (h, _) = Presenter.Numerator.Freqz(Presenter.Denominator, n);
                pha = h.PhaseSpectrum().ToArray();
            }

            Figure.Plot.SetAxisLimits(xMin: 0, xMax: 1000, yMin: -200, yMax: 200);
            Figure.Plot.YAxis.MinorLogScale(false);
            Figure.Plot.ResetChannelParameter(0, 90, "", QuantityUnit.Angle.ToUnitString(), 90, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, 0.1, "", "", 100, false);
            Figure.Plot.AddSignal(pha.ToArray(), n / 1000.0, Color.Red);
            Figure.Plot.YLabel("Phase (°)");
            Figure.Plot.XLabel("Normalized Frequency (×π rad/sample)");
        }

        private void RenderImpResp()
        {
            if (Presenter.Numerator is null)
            {
                RenderEmpty();
                return;
            }
            Double[] z;
            Int32 n = Presenter.Numerator.Length;
            if (Presenter.Denominator is null)
            {
                z = Presenter.Numerator.Impz(new Double[] { 1 }, n);
            }
            else
            {
                n = n >= 100 ? n : 100;
                z = Presenter.Numerator.Impz(Presenter.Denominator, n);
            }

            var min = z.Min();
            var max = z.Max();
            if (!Double.IsFinite(min) || !Double.IsFinite(max))
            {
                return;
            }

            var vmax = max * 1.2;
            var vmin = min < 0 ? min * 1.2 : 0;
            var temp = Math.Abs(min) > Math.Abs(max) ? Math.Abs(min) : Math.Abs(max);

            var yscalezoom = 1000.0;
            var xscalenum = 10.0;
            var yscalenum = 20.0;
            Int32 gain = 0;
            Double yscale = 1.0, xscale = 1.0;
            Int32 yinterval = 1, xinterval = 1;
            while (temp > yscalezoom)
            {
                temp /= yscalezoom;
                gain++;
            }
            while (temp < 1)
            {
                temp *= yscalezoom;
                gain--;
            }
            var prefix = (Prefix)((Int32)Prefix.Empty + gain);

            z.Multiply_(Math.Pow(yscalezoom, -gain));
            vmin *= Math.Pow(yscalezoom, -gain);
            vmax *= Math.Pow(yscalezoom, -gain);
            yscale = Math.Ceiling(Math.Abs(vmax - vmin) / yscalenum) * Math.Pow(yscalezoom, gain);
            yinterval = (Int32)Math.Ceiling(Math.Abs(vmax - vmin) / yscalenum);
            yinterval = yinterval == 0 ? 1 : yinterval;
            xscale = Math.Floor(n / xscalenum);
            xinterval = n / (Int32)xscalenum;
            xscale = xscale == 0 ? 1 : xscale;
            xinterval = xinterval == 0 ? 1 : xinterval;
            Figure.Plot.SetAxisLimits(xMin: 0, xMax: n, yMin: vmin, yMax: vmax);
            Figure.Plot.YAxis.MinorLogScale(false);
            Figure.Plot.ResetChannelParameter(0, yscale, "", "", yinterval, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, xscale, "", "", xinterval, false);
            Figure.Plot.AddSignal(z.ToArray(), 1, Color.Red);
            //Figure.Plot.AddLollipop(new Double[][] { z }, new Color[] { Color.Red });
            Figure.Plot.YLabel("Amplitude");
            Figure.Plot.XLabel("Samples");
        }

        private void RenderZeroPoles()
        {
            if (Presenter.Numerator is null)
            {
                RenderEmpty();
                return;
            }
            int n = Presenter.Order;
            Complex[] p = Presenter.Roots(Presenter.Denominator, n);
            Complex[] z = Presenter.Roots(Presenter.Numerator, n);

            var xmax = p.Concat(z).Where(x=>double.IsFinite(x.Real)&&Math.Abs(x.Real) <int.MaxValue).Max(x => x.Real);
            var xmin = p.Concat(z).Where(x => double.IsFinite(x.Real) && Math.Abs(x.Real) < int.MaxValue).Min(x => x.Real);

            var ratio = Figure.Size.Width / (Double)Figure.Size.Height;
            var xvmax = GetMax(xmax);
            var xvmin = GetMin(xmin);
            var xscale = GetMax(Math.Abs(xvmax - xvmin)/6.0);
            xscale = xscale == 0 ? 0.5:xscale;
            var amp = 50 / xscale;
            //var p = new Complex[] { 0.7143 + Complex.ImaginaryOne * 0.33, 0.6, 0.7143 - Complex.ImaginaryOne * 0.33 };
            //var z = new Complex[] { -1, 1, -0.3 };


            Figure.Plot.SetAxisLimits(xMin: -150 * ratio, xMax: 150 * ratio, yMin: -150, yMax: 150);
            Figure.Plot.YAxis.MinorLogScale(false);
            Figure.Plot.ResetChannelParameter(0, xscale, "", "",50, false);
            Figure.Plot.XAxis.MinorLogScale(false);
            Figure.Plot.ResetTimebaseParameter(0, xscale, "", "", 50, false);
            Figure.Plot.YLabel("Imaginary Part");
            Figure.Plot.XLabel("Real Part");

            Figure.Plot.AddPoint(0, 0, Color.White, 5, ScottPlot.MarkerShape.filledCircle);

            var thetas = new Double[400];
            var rs = new Double[400];
            for (Int32 i = 0; i < thetas.Length; i++)
            {
                rs[i] = amp;
                thetas[i] = i * 2 * Math.PI * 1/amp;
            }
            // convert polar data to Cartesian data
            var (real, imag) = ScottPlot.Tools.ConvertPolarCoordinates(rs, thetas);
            Figure.Plot.AddScatterLines(real, imag, Color.White, 1);

            real = p.Real().ToArray().Multiply_(amp);
            imag = p.Imaginary().ToArray().Multiply_(amp);
            Figure.Plot.AddScatterPoints(real, imag, Color.Red, 5, ScottPlot.MarkerShape.eks);

            real = z.Real().ToArray().Multiply_(amp);
            imag = z.Imaginary().ToArray().Multiply_(amp);

            Figure.Plot.AddScatterPoints(real, imag, Color.LightGreen, 5, ScottPlot.MarkerShape.openCircle);
        }

        double GetMax(double max,double @default=double.NaN)
        {
            if (max <= @default && @default != double.NaN)
                return @default;

            double roundedMax = Math.Ceiling(max);
            int magnitude = (int)Math.Floor(Math.Log10(Math.Abs(roundedMax)));
            double factor = Math.Pow(10, magnitude);

            // 获取基础倍数的首位数字
            double firstDigit = Math.Abs(roundedMax / factor);

            if (roundedMax > 0)
            {
                // 正数时，寻找更大的正数作为上限
                if (firstDigit <= 2)
                {
                    roundedMax = 2 * factor;
                }
                else if (firstDigit <= 5)
                {
                    roundedMax = 5 * factor;
                }
                else
                {
                    roundedMax = 10 * factor;
                }
            }
            else
            {
                // 负数时，寻找更小的负数作为上限
                if (firstDigit <= 2)
                {
                    roundedMax = -1 * factor;
                }
                else if (firstDigit <= 5)
                {
                    roundedMax = -2 * factor;
                }
                else
                {
                    roundedMax = -5 * factor;
                }
                // 确保我们的上限始终高于原始最大值
                if (roundedMax < max)
                {
                    if (firstDigit <= 5)
                    {
                        roundedMax = -1 * factor;
                    }
                    else
                    {
                        roundedMax = 0;
                    }
                }
            }

            return roundedMax;
        }

        double GetMin(double min, double @default = double.NaN)
        {
            if (min >= @default && @default != double.NaN)
                return @default;

            double roundedMin = Math.Floor(min);
            int magnitude = (int)Math.Floor(Math.Log10(Math.Abs(roundedMin)));
            double factor = Math.Pow(10, magnitude);

            // 获取基础倍数的首位数字
            double firstDigit = Math.Abs(roundedMin / factor);

            if (roundedMin < 0)
            {
                // 如果是负数，寻找更小的负数作为下限
                if (firstDigit >= 5)
                {
                    roundedMin = -10 * factor;
                }
                else if (firstDigit >= 2)
                {
                    roundedMin = -5 * factor;
                }
                else
                {
                    roundedMin = -2 * factor;
                }
            }
            else
            {
                // 如果是正数，寻找更小的正数作为下限
                if (firstDigit >= 5)
                {
                    roundedMin = 5 * factor;
                }
                else if (firstDigit >= 2)
                {
                    roundedMin = 2 * factor;
                }
                else
                {
                    roundedMin = factor;
                }
                // 确保我们的下限始终低于原始最小值
                if (roundedMin > min)
                {
                    if (firstDigit >= 5)
                    {
                        roundedMin = 2 * factor;
                    }
                    else if (firstDigit >= 2)
                    {
                        roundedMin = factor;
                    }
                    else
                    {
                        roundedMin = 0;
                    }
                }
            }



            return roundedMin;
        }


        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //FileBrowserForm fbf = FileBrowserForm.Instance;
            //fbf.CanEditFileName = true;

            //fbf.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.Text));
            //fbf.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //if (fbf.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                //if (Presenter.Save(fbf.ChoosedFolderPath, fbf.ChoosedFolderName))
                if (Presenter.Save(fileInfo.DirectoryName, fileInfo.FullName))
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingSuccess);
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingFailed);
                }

            }
            //fbf.CanEditFileName = false;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //FileBrowserForm fbf = FileBrowserForm.Instance;

            //fbf.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.Text));
            //fbf.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            //if (fbf.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //if (Presenter.Load(fbf.FullFileName))
                if (Presenter.Load(dialog.FileName))
                {
                    RdoDiagram.ChoosedButtonIndex = 1;
                    Render();
                    WeakTip.Default.Write("File", MsgTipId.ReadingSuccess);
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                    WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                }
            }
        }

        private void CbxMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (Presenter.FilterType == FilterType.FIRFilter)
                {
                    Presenter.FIRMethod = (FIRType)CbxMethod.SelectIndex;
                    if (Presenter.FIRMethod == FIRType.Remez)
                    {
                        RdoFilterOrder.Enabled = true;
                    }
                    else
                    {
                        RdoFilterOrder.Enabled = false;
                        RdoFilterOrder.ChoosedButtonIndex = (Int32)FilterOrderMode.UserDefined;
                        NebFilterOrder.Visible = true;
                        NebFilterOrder.Enabled = true;
                    }
                }
                else
                {
                    Presenter.IIRMethod = (IIRType)CbxMethod.SelectIndex;
                }
            }
        }

        private void CbxRespType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RespType = (FilterResponseType)CbxRespType.SelectValue;
                Render();
            }
        }

        private void CbxWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Window = (WindowType)CbxWindow.SelectValue;
            }
        }


        private void ClickStart(object sender, EventArgs e)
        {
            this.HasFlashChild = true;
        }

        private void ClickEnd(object sender, EventArgs e)
        {
            this.HasFlashChild = false;
        }
    }
}