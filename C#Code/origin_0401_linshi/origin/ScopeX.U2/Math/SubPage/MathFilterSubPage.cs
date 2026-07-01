using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Drawing;
using ScopeX.UserControls;


namespace ScopeX.U2
{
    [Description(nameof(MathType.Filter))]
    public partial class MathFilterSubPage : MathSubPageBase, IFilterView, ITimebaseView
    {
        MathFilterArg Arg;

        public MathFilterSubPage()
        {
            InitializeComponent();
            if (FilterPresenter == null)
            {
                FilterPresenter = Program.Oscilloscope.Filter;
                FilterPresenter.TryAddView(this);
                UpdateView(FilterPresenter);
            }
            DefaultStyleManager.Instance.RegisterControlRecursion(ChkCustomDesign, StyleFlag.FontSize);
            ViewInit();
            DsoPrsnt.DefaultDsoPrsnt.Timebase.TryAddView(this);
        }

        #region Base Method
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!this.Visible)
            {
                if (Presenter.Args is MathFilterArg arg && arg != null && arg.Numerator == null)
                {
                    arg.Numerator = FilterPresenter.Numerator;
                    arg.Denominator = FilterPresenter.Denominator;
                }
            }
        }

        protected override void ViewInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebFreq1);
            NebFreq1.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Arg, NebFreq1, nameof(Arg.Freq1));
            };
            NebFreq1.AddClicked = (_, e) => Arg.AdjFreq1(e.Step);
            NebFreq1.SubClicked = (_, e) => Arg.AdjFreq1(e.Step);
            NebFreq1.StringFormatFunc = (_) => FreqToString(Arg.Freq1);
            NebFreq1.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFreq1);
                var onokclickeventaction = new Action<Double>((data) =>
                    Arg.Freq1 = (Int32)Math.Round(data, MidpointRounding.AwayFromZero));

                nkf.SetKeyBoardValue(LblFreq1.Text, "", 3, onokclickeventaction,
                    Arg.Freq1,
                    MathFilterArg.MaxFreq,
                    MathFilterArg.MinFreq);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };

            ControlsHotKnob.Default.InitHotKnob(NebFreq2);
            NebFreq2.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Arg, NebFreq2, nameof(Arg.Freq2));
            };
            NebFreq2.AddClicked = (_, e) => Arg.AdjFreq2(e.Step);
            NebFreq2.SubClicked = (_, e) => Arg.AdjFreq2(e.Step);
            NebFreq2.StringFormatFunc = (_) => FreqToString(Arg.Freq2);
            NebFreq2.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFreq2);
                var onokclickeventaction = new Action<Double>((data) =>
                    Arg.Freq2 = (Int32)Math.Round(data, MidpointRounding.AwayFromZero));

                nkf.SetKeyBoardValue(LblFreq2.Text, "", 3, onokclickeventaction,
                    Arg.Freq2,
                    MathFilterArg.MaxFreq,
                    MathFilterArg.MinFreq);

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
        }

        protected override void SubPageUpdateView(Object prsnt, String propertyName = "")
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            var presenter = Presenter.GetOrMakeArg(MathType.Filter);
            if (presenter is MathFilterArg filterarg)
            {
                Arg = filterarg;
            }

            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsReference()));

            if (Presenter.PreMathChannels.Count > 0)
            {
                srcs.Add(Presenter.PreMathChannels[0]);
            }
            LoadSourceList(srcs);
            UpdateView();
        }
        #endregion

        public FilterPrsnt FilterPresenter
        {
            get;
            set;
        }

        IFilterPrsnt IView<IFilterPrsnt>.Presenter
        {
            get => FilterPresenter;
            set => FilterPresenter = (FilterPrsnt)value;
        }
        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter { get => DsoPrsnt.DefaultDsoPrsnt.Timebase; set => throw new NotImplementedException(); }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Arg is null || FilterPresenter is null)
                {
                    return;
                }

                _ArgToCtrl = true;
                CbxSource.SelectValue = Arg.Source;
                CbxResp.SelectValue = (Int32)(Arg.IsQuickDesign ? Arg.RespType : FilterPresenter.RespType);
                NebFreq1.UpdateValueString();
                NebFreq2.UpdateValueString();
                ChkCustomDesign.Checked = !Arg.IsQuickDesign;
                ChangeCtrlState();
                UpdateSample();
                _ArgToCtrl = false;
            }
        }


        private void UpdateSample()
        {
            Double? si = null;
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Arg.Source, out var cp))
            {

                if (cp is AnalogPrsnt ap && cp.Active)
                {
                    si = ap.Pack?.Properties?.SampInterval;
                }
                else if (cp is ReferencePrsnt rp && rp.Active)
                {
                    si = rp.Pack?.Properties?.SampInterval;
                }
                if (Arg.SampleInterval != si)
                {
                    Arg.SampleInterval = si;
                }
            }
            LblCurrentSample.Text = $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangQianShuJuCaiYangLv")}: {(Arg.SampleInterval != null ? FreqToString(1 / Arg.SampleInterval.Value) : "---")}";
        }

        private void ChangeCtrlState()
        {
            if (!Arg.IsQuickDesign)
            {
                LblResp.Enabled = false;
                CbxResp.Enabled = false;
                LblFreq1.Enabled = false;
                NebFreq1.Enabled = false;
                LblFreq2.Enabled = false;
                NebFreq2.Enabled = false;
                if (Arg.RespType == FilterResponseType.LowPass || Arg.RespType == FilterResponseType.HighPass)
                {
                    LblFreq2.Visible = false;
                    NebFreq2.Visible = false;
                }
                else
                {
                    LblFreq2.Visible = true;
                    NebFreq2.Visible = true;
                }
                BtnConfig.Enabled = true;
            }
            else
            {
                LblResp.Enabled = true;
                CbxResp.Enabled = true;
                LblFreq1.Enabled = true;
                NebFreq1.Enabled = true;
                if (Arg.RespType == FilterResponseType.LowPass || Arg.RespType == FilterResponseType.HighPass)
                {
                    LblFreq2.Enabled = false;
                    NebFreq2.Enabled = false;
                    LblFreq2.Visible = false;
                    NebFreq2.Visible = false;
                }
                else
                {
                    LblFreq2.Enabled = true;
                    NebFreq2.Enabled = true;
                    LblFreq2.Visible = true;
                    NebFreq2.Visible = true;
                }

                BtnConfig.Enabled = false;
            }
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (CbxSource.DataSource != null)
            {
                if (((CbxSource.DataSource is IEnumerable<KeyValuePair<String, ChannelId>> channels) && channels.Select(x => x.Value.ToString()).ToArray().SequenceEqual(sources.Select(x => x.ToString()))))
                {
                    return;
                }
            }
            CbxSource.DataSource = sources.Select(x => new SelectComboBox.ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxSource.SelectValue, out var chnlPrsnt))
                    {
                        //if (!chnlPrsnt.Active)
                        //{
                        //    chnlPrsnt.Active = true;
                        //}
                    }
                    //_FilterArg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedValue);
                    Arg.Source = (ChannelId)CbxSource.SelectValue;
                }
            };

        }

        private static String FreqToString(Double freq)
        {
            if (Double.IsFinite(freq))
            {
                return new Quantity(freq, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.###", true);
            }
            return String.Empty;
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            var fdf = new FilterDesignForm()
            {
                //StartPosition = FormStartPosition.CenterScreen,
                //Owner =(Form) this.Parent.Parent.Parent,
                Presenter = FilterPresenter,
            };

            FilterPresenter.TryAddView(fdf);
            if (fdf.ShowDialogByEvent() == DialogResult.OK && Presenter.Args is MathFilterArg arg && arg != null)
            {
                arg.Numerator = FilterPresenter.Numerator;
                arg.Denominator = FilterPresenter.Denominator;
                //MathVecBuffer.Default.Provide(MathFilterArg.GetCoeffKey(MathPresenter.Name), new Vector(FilterPresenter.Numerator));
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //_FilterArg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
            }
        }

        //private void CbxResp_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        if (_FilterArg.IsQuickDesign)
        //        {
        //            _FilterArg.RespType = (FilterResponseType)CbxResp.SelectedIndex;
        //        }
        //        else
        //        {
        //            FilterPresenter.RespType = (FilterResponseType)CbxResp.SelectedIndex;
        //        }
        //    }
        //}

        private void ChkCustomDesign_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.IsQuickDesign = !ChkCustomDesign.Checked;
            }
        }

        private void CbxSource_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {

            }
        }
        private void CbxResp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (Arg.IsQuickDesign)
                {
                    Arg.RespType = (FilterResponseType)CbxResp.SelectValue;
                }
                else
                {
                    FilterPresenter.RespType = (FilterResponseType)CbxResp.SelectValue;
                }
            }
        }

        //!!!Notice: Dispose FilterPresenter
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            DsoPrsnt.DefaultDsoPrsnt.Timebase.TryRemoveView(this);
            FilterPresenter.TryRemoveView(this);

            base.Dispose(disposing);
        }
    }
}
