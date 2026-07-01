// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using System.Linq;
    using ScopeX.U2.LanguageSupoort;
    using static ScopeX.UserControls.SelectComboBox;

    /// <summary>
    /// Defines the <see cref="MathPage" />.
    /// </summary>
    public partial class MathPage : UserControl, IChnlView, IStylize
    {
        /// <summary>
        /// The index of optionSubPage in TlpMath rows.
        /// </summary>
        private const Int32 OptionSubPageIndex = 1;

        /// <summary>
        /// Defines the _ArgToCtrl.
        /// </summary>
        private Boolean _ArgToCtrl;
        private Boolean _ControlToCtrl = false;

        public Boolean NeedPrsnt;

        /// <summary>
        /// Defines the _OptionSubPage.
        /// </summary>
        private Control _OptionSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathPage"/> class.
        /// </summary>
        public MathPage()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        public MathPrsnt Presenter
        {
            get =>
                (MathPrsnt)(ParentForm as IChnlView).Presenter;
            set =>
                (ParentForm as IChnlView).Presenter = value;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        IBadge IView<IBadge>.Presenter { get => Presenter; set => Presenter = (MathPrsnt)value; }

        /// <summary>
        /// The Refresh.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        /// <summary>
        /// The UpdateView.
        /// </summary>
        /// <param name="prsnt">The prsnt<see cref="Object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="String"/>.</param>
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
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    break;
                case "ConditioningScale":
                case "ConditioningPosition":
                case "ConditioningScaleUnit":
                    NebVScale.UpdateValueString();
                    NebVPos.UpdateValueString();
                    break;
                case "SamplingScale":
                case "SamplingPosition":
                case "SamplingScaleUnit":
                    //if (CbxCalcType.SelectedIndex == (Int32)MathType.FFT && propertyName == "SamplingScale")
                    //    (_OptionSubPage as IMathView)?.UpdateView(prsnt, propertyName);
                    if (CbxCalcType.SelectValue.ToString() == UpdateCbxCalcTypeIndex() && propertyName == "SamplingScale")
                        (_OptionSubPage as IMathView)?.UpdateView(prsnt, propertyName);
                    NebHScale.UpdateValueString();
                    NebHPos.UpdateValueString();
                    break;
                case nameof(Presenter.Label):
                    if (TbxLabel.Text != Presenter.Label)
                    {
                        TbxLabel.Text = Presenter.Id.IsJitterMath() ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Presenter.Label) : Presenter.Label;
                    }
                    break;
                case nameof(Presenter.IsAutoUnit):
                    if (ChkUnit.Checked == Presenter.IsAutoUnit)
                    {
                        ChkUnit.Checked = !Presenter.IsAutoUnit;
                        TbxUnit.Enabled = ChkUnit.Checked;
                    }
                    break;
                case nameof(Presenter.CustomUnit):
                    if (Presenter.CustomUnit != TbxUnit.Text)
                    {
                        TbxUnit.Text = Presenter.CustomUnit;
                    }
                    break;
                case nameof(MathPrsnt.AutoScale):
                    if (Presenter.Args.Type == MathType.Histgram)
                    {
                        //NebVPos.Enabled = false;
                        //BtnResetVPos.Enabled = false;
                        NebVScale.Enabled = !Presenter.AutoScale;
                        NebHPos.Enabled = !Presenter.AutoScale;
                        BtnResetHPos.Enabled = !Presenter.AutoScale;
                        NebHScale.Enabled = !Presenter.AutoScale;
                    }
                    break;
                case nameof(Presenter.LabelVisibility):
                    ChkLabelVisiblity.Checked = Presenter.LabelVisibility;
                    break;
                default:
                    (_OptionSubPage as IMathView)?.UpdateView(prsnt, propertyName);
                    if (CbxCalcType.SelectValue.ToString() != UpdateCbxCalcTypeIndex())
                        CbxCalcType.SelectValue = UpdateCbxCalcTypeIndex();
                    break;
            }
            _ArgToCtrl = false;

            //(_OptionSubPage as IMathView)?.UpdateView(prsnt, propertyName);
            ChangeOptionSubPage(Presenter.Args.Type);
            ChangeOptState(Presenter.Args.Occupier is null);
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            InitOptionSubPageOnLoad();
            ChangeOptionSubPage(Presenter.Args.Type);
            base.OnLoad(e);
            UpdateView();
        }
        public void ToAutoScale(Boolean auto)
        {
            NebVScale.Enabled = !Presenter.AutoScale;
            NebHPos.Enabled = !Presenter.AutoScale;
            BtnResetHPos.Enabled = !Presenter.AutoScale;
            NebHScale.Enabled = !Presenter.AutoScale;
            NebVPos.Enabled = !Presenter.AutoScale;
            BtnResetVPos.Enabled = !Presenter.AutoScale;
            ChkUnit.Enabled = !Presenter.AutoScale;
        }
        /// <summary>
        /// The UpdateView.
        /// </summary>
        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = Presenter.Active;
                ChkLabelVisiblity.Checked = Presenter.LabelVisibility;
                //CbxCalcType.SelectedIndex = Presenter.Args.Type == MathType.ERes ? (Int32)Presenter.Args.Type - 1 : (Int32)Presenter.Args.Type;
                var ffts = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (MathPrsnt)p).Where(m => m.Args.Type == MathType.FFT).ToList();
                var fftvisiable = ffts.Count == 2 ? ffts.Any(x => x.Id == Presenter.Id) : true;
                if (!fftvisiable)
                {
                    var fft = (CbxCalcType.DataSource as List<SelectComboBox.ComboBoxItem>)?.FirstOrDefault(x => x.Value?.Equals(nameof(Properties.Resources.MathCalcTypeFFT)) == true);
                    fft.Visible = false;
                }
                CbxCalcType.SelectValue = UpdateCbxCalcTypeIndex();
                NebVScale.UpdateValueString();
                NebVPos.UpdateValueString();

                //FFT type special setting
                if (CbxCalcType.SelectValue?.ToString() == nameof(Properties.Resources.MathCalcTypeFFT))
                {
                    PnlHorizon.Visible = false;
                    SetHVisible(PnlHorizon.Visible);
                }
                else
                {
                    PnlHorizon.Visible = true;
                    NebHScale.UpdateValueString();
                    NebHPos.UpdateValueString();
                }

                TbxLabel.Text = Presenter.Id.IsJitterMath() ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Presenter.Label) : Presenter.Label;

                if (string.IsNullOrEmpty(TbxUnit.Text))
                {
                    TbxUnit.Text = String.IsNullOrEmpty(Presenter.CustomUnit) ? Presenter.Unit : Presenter.CustomUnit;
                }

                ChkUnit.Checked = !Presenter.IsAutoUnit;
                TbxUnit.Enabled = ChkUnit.Checked;

                ChangeOptState(Presenter.Args.Occupier is null);
                if (CbxCalcType.SelectValue?.ToString() == nameof(Properties.Resources.MathCalcTypeHistogram))
                {
                    //NebVPos.Enabled = false;
                    //BtnResetVPos.Enabled = false;
                    NebVPos.Enabled = !Presenter.AutoScale;
                    BtnResetVPos.Enabled = !Presenter.AutoScale;
                    NebVScale.Enabled = !Presenter.AutoScale;
                    NebHPos.Enabled = !Presenter.AutoScale;
                    BtnResetHPos.Enabled = !Presenter.AutoScale;
                    NebHScale.Enabled = !Presenter.AutoScale;
                }
                if (CbxCalcType.SelectValue?.ToString() == nameof(Properties.Resources.MathCalcTypeTrack))
                {
                    NebVPos.Enabled = true;
                    NebVScale.Enabled = true;
                    NebHPos.Enabled = false;
                    NebHScale.Enabled = false;
                    BtnResetHPos.Enabled = false;
                }
                ChkUnit.Visible = !Presenter.Id.IsAdvancedMath();
                TbxUnit.Visible = !Presenter.Id.IsAdvancedMath();
                LblLabel.Enabled = !Presenter.Id.IsAdvancedMath();
                TbxLabel.Enabled = !Presenter.Id.IsAdvancedMath();
                NebVPos.Visible = Presenter.Args.Type != MathType.Histgram;
                BtnResetVPos.Visible = Presenter.Args.Type != MathType.Histgram;
                LblVPos.Visible = Presenter.Args.Type != MathType.Histgram;

                ChkUnit.Visible = !Presenter.Id.IsAdvancedMath();
                TbxUnit.Visible = !Presenter.Id.IsAdvancedMath();

                ChkUnit.Visible = false;
                TbxUnit.Visible = false;

                _ArgToCtrl = false;
            }
        }

        private String UpdateCbxCalcTypeIndex()
        {
            var value = Presenter.Args.Type switch
            {
                MathType.Binary => nameof(Properties.Resources.MathCalcTypeBinary),
                MathType.FFT => nameof(Properties.Resources.MathCalcTypeFFT),
                MathType.Zoom => nameof(Properties.Resources.MathCalcTypeZoom),
                MathType.Filter => nameof(Properties.Resources.MathCalcTypeFilter),
                MathType.ERes => nameof(Properties.Resources.MathCalcTypeERes),
                MathType.Histgram => nameof(Properties.Resources.MathCalcTypeHistogram),
                MathType.Track => nameof(Properties.Resources.MathCalcTypeTrack),
                MathType.Trend => nameof(Properties.Resources.MathCalcTypeTrend),
                MathType.Custom => nameof(Properties.Resources.MathCalcTypeCustom),
                MathType.UserProgram => nameof(Properties.Resources.MathCalcTypeUserProgram),
                _ => nameof(Properties.Resources.MathCalcTypeBinary)
            };
            //var index = cbsource.FindIndex(x => x.Value == value);
            //CbxCalcType.SelectedIndex = index == -1 ? 0 : index;
            //CbxCalcType.SelectedIndex = Presenter.Args.Type == MathType.ERes ? (Int32)Presenter.Args.Type - 1 : (Int32)Presenter.Args.Type;
            return value;
        }

        private static Control FindOptionSubPage(ControlCollection collection, MathType mt)
        {
            var subtype = SubPageBase.GetMathChildren(mt.ToString());
            if (subtype == null)
            {
                throw new NotImplementedException();
            }
            foreach (var item in collection)
            {
                if (item.GetType() == subtype)
                    return (Control)item;
            }

            return null;
        }

        /// <summary>
        /// The GetOptionSubPage.
        /// </summary>
        /// <param name="mt">The mt<see cref="MathType"/>.</param>
        /// <returns>The <see cref="Control"/>.</returns>
        private static Control GetOptionSubPage(MathType mt, Control parentForm)
        {
            var subpage = SubPageBase.GetMathChildrenInstantce(mt.ToString());
            if (subpage == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                subpage.Parent = parentForm;
            }

            subpage.Dock = DockStyle.Fill;
            subpage.TabIndex = 1;
            subpage.BackColor = Color.Transparent;
            if (subpage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
                DefaultStyleManager.Instance.RegisterControlRecursion(subpage, StyleFlag.FontSize);
            }
            return subpage;

            //static Control MakeFilterSubPage()
            //{
            //    var mfsp = new MathFilterSubPage()
            //    {
            //        FilterPresenter = Program.Oscilloscope.Filter,
            //    };
            //    mfsp.FilterPresenter.TryAddView(mfsp);
            //    mfsp.UpdateView(mfsp.FilterPresenter);
            //    return mfsp;
            //}
        }

        /// <summary>
        /// The OnEnterKeyPressed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyPressEventArgs"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        private static Boolean OnEnterKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The BtnResetHPos_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnResetHPos_Click(object sender, EventArgs e)
        {
            Presenter.Sampling.ResetPosIndex();
        }

        /// <summary>
        /// The BtnResetVPos_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnResetVPos_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
        }

        /// <summary>
        /// The CbxCalcType_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void CbxCalcType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MathType mathType = CbxCalcType.SelectValue.ToString() switch
                {
                    nameof(Properties.Resources.MathCalcTypeBinary) => mathType = MathType.Binary,
                    nameof(Properties.Resources.MathCalcTypeFFT) => mathType = MathType.FFT,
                    nameof(Properties.Resources.MathCalcTypeFilter) => mathType = MathType.Filter,
                    nameof(Properties.Resources.MathCalcTypeERes) => mathType = MathType.ERes,
                    nameof(Properties.Resources.MathCalcTypeCustom) => mathType = MathType.Custom,
                    nameof(Properties.Resources.MathCalcTypeUserProgram) => mathType = MathType.UserProgram,
                    _ => MathType.Binary
                };

                //if (index == (Int32)MathType.Filter)
                //{
                //    index = (Int32)MathType.ERes;
                //}
                //else
                //{
                //    if (index > (Int32)MathType.ERes - 1)
                //    {
                //        index += (MathType.Custom - MathType.Histgram) + 1;
                //    }
                //}
                if (mathType == MathType.FFT)
                {
                    //if (!PlatformUIManager.Default.Platform.FFTFunctionLimitWithJitter())
                    //{
                    //    CbxCalcType.SelectValue = UpdateCbxCalcTypeIndex();
                    //    return;
                    //}
                    var fftmath = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Id != Presenter.Id && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (MathPrsnt)p).Where(m => m.Args.Type == MathType.FFT).ToList();
                    if (fftmath != null && fftmath.Count >= 2)
                    {
                        WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
                        return;
                    }
                }
                ChangeOptionSubPage(mathType);
            }
        }

        /// <summary>
        /// The CbxUnit_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void CbxUnit_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IsAutoUnit = !ChkUnit.Checked;
                if (ChkUnit.Checked)
                {
                    Presenter.CustomUnit = TbxUnit.Text;
                    Presenter.Unit = Presenter.CustomUnit;
                }
                TbxUnit.Enabled = ChkUnit.Checked;
            }
        }

        private Boolean _IsChangingPage = false;
        /// <summary>
        /// The ChangeOptionSubPage.
        /// </summary>
        /// <param name="mt">The mt<see cref="MathType"/>.</param>
        private void ChangeOptionSubPage(MathType mt)
        {
            if (_OptionSubPage == null)
            {
                return;
            }
            if ((_OptionSubPage as IMathView).Mode == mt)
            {
                return;
            }
            if (_IsChangingPage == true)
            {
                return;
            }
            _IsChangingPage = true;
            //FFT type special setting
            PnlHorizon.Visible = !(mt == MathType.FFT);
            if (mt == MathType.FFT || mt == MathType.Zoom)
            {
                ChkIndependentWindow.Visible = false;
                ChkIndependentWindow.Checked = true;
            }
            _OptionSubPage.Visible = false;
            //TlpMath.Controls.Remove(_OptionSubPage);
            //_OptionSubPage.Dispose();
            _OptionSubPage = FindOptionSubPage(TlpMath.Controls, mt);
            if (_OptionSubPage == null)
            {
                _OptionSubPage = GetOptionSubPage(mt, this);
                TlpMath.Controls.Add(_OptionSubPage, 0, OptionSubPageIndex);
            }
            else
            {
                _OptionSubPage.Visible = true;
            }

            if (mt == MathType.Binary || mt == MathType.Filter || mt == MathType.ERes || mt == MathType.Track || mt == MathType.Custom || mt == MathType.UserProgram)
            {
                ChkIndependentWindow.Visible = true;
                LblIndependentWindow.Visible = true;
            }
            else
            {
                ChkIndependentWindow.Visible = false;
                LblIndependentWindow.Visible = false;
            }
            var form = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetWindow(Presenter.WindowId);
            LanguageFactory.UpdateFormCache(this.ParentForm);
            if (form?.IsMainForm == false)
            {
                SetHVisible(true);
            }
            else
            {
                SetHVisible(false);
            }
            _IsChangingPage = false;
        }

        /// <summary>
        /// The ChangeOptState.
        /// </summary>
        /// <param name="state">The state<see cref="Boolean"/>.</param>
        private void ChangeOptState(Boolean state)
        {
            if (_OptionSubPage == null)
                return;
            _OptionSubPage.Enabled = CbxCalcType.Enabled = state;
            if (Presenter.IsJitterMath())
            {
                this.LblIndependentWindow.Visible = false;
                this.ChkIndependentWindow.Visible = false;
                this.NebVScale.Enabled = false;
                this.NebHScale.Enabled = false;
                this.NebVPos.Enabled = false;
                this.NebHPos.Enabled = false;
            }
        }

        /// <summary>
        /// The ChkActive_CheckedChangedEvent.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = false;

                //(Program.Oscilloscope.View as DsoForm).RemoveBadge(Presenter);
            }
        }

        /// <summary>
        /// The HPosToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String HPosToString()
        {
            return new Quantity(Presenter.Sampling.PosIndexBymDiv, Prefix.Milli, QuantityUnit.Division).ToString("#0.###", true);
        }

        /// <summary>
        /// The HScaleToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String HScaleToString()
        {
            return new Quantity(Presenter.Sampling.Scale, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString();
        }

        /// <summary>
        /// The Init.
        /// </summary>
        private void Init()
        {
            ControlsHotKnob.Default.InitHotKnob(NebVScale);
            NebVScale.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVScale, nameof(Presenter.Scale));
            };
            //NebVScale
            NebVScale.AddClicked += (_, _) => Presenter.ScaleIndex++;
            NebVScale.SubClicked += (_, _) => Presenter.ScaleIndex--;
            NebVScale.StringFormatFunc = (value) => VScaleToString();
            NebVScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVScale);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.MaxScale, Presenter.Prefix);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.MinScale, Presenter.Prefix);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.Scale, Presenter.Prefix);
                nkf.NumberKeyboard.Unit = Presenter.Unit;
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.Title = LblVScale.Text;
                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.Scale = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Prefix);

                    nkf.Close();
                };
                nkf.ShowDialogByPosition();
            };
            //NebVPos
            ControlsHotKnob.Default.InitHotKnob(NebVPos);
            NebVPos.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVPos, nameof(Presenter.PosIndexBymDiv));
            };
            NebVPos.AddClicked += (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebVPos.SubClicked += (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebVPos.StringFormatFunc = (value) => VPosToString();
            NebVPos.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVPos);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.PosMaxIndex, Prefix.Milli);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.PosMinIndex, Prefix.Milli);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.PosIndexBymDiv, Prefix.Milli);
                nkf.NumberKeyboard.Unit = QuantityUnit.Division.ToUnitString();
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.Title = LblVPos.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.PosIndexBymDiv = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Prefix.Milli);

                    nkf.Close();
                };

                nkf.ShowDialogByPosition();
            };

            //NebHScale
            ControlsHotKnob.Default.InitHotKnob(NebHScale);
            NebHScale.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHScale, nameof(Presenter.Sampling.Scale));
            };
            NebHScale.AddClicked += (_, _) =>
            Presenter.Sampling.ScaleIndex++;
            NebHScale.SubClicked += (_, _) => Presenter.Sampling.ScaleIndex--;
            NebHScale.StringFormatFunc = (value) => HScaleToString();
            NebHScale.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.Sampling.Scale = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.Sampling.Prefix);

                nkf.SetKeyBoardValue(LblHScale.Text, Presenter.Sampling.Unit, 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.Sampling.Scale, Presenter.Sampling.Prefix),
                    Quantity.ConvertByPrefix(Presenter.Sampling.MaxScale, Presenter.Sampling.Prefix),
                    Quantity.ConvertByPrefix(Presenter.Sampling.MinScale, Presenter.Sampling.Prefix));

                nkf.ShowDialogByPosition();
            };

            //NebHPos
            ControlsHotKnob.Default.InitHotKnob(NebHPos);
            NebHPos.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHPos, nameof(Presenter.Sampling.PosIndexBymDiv));
            };
            NebHPos.AddClicked += (_, e) => Presenter.Sampling.PosIndexBymDiv += e.Step;
            NebHPos.SubClicked += (_, e) => Presenter.Sampling.PosIndexBymDiv += e.Step;
            NebHPos.StringFormatFunc = (value) => HPosToString();
            NebHPos.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHPos);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosMaxIndex, Prefix.Milli);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosMinIndex, Prefix.Milli);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosIndexBymDiv, Prefix.Milli);
                nkf.NumberKeyboard.Unit = QuantityUnit.Division.ToUnitString();
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.Title = LblHPos.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.Sampling.PosIndexBymDiv = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Prefix.Milli);

                    nkf.Close();
                };

                nkf.ShowDialogByPosition();
            };

            //CbxCalcType
            List<SelectComboBox.ComboBoxItem> data = new List<SelectComboBox.ComboBoxItem>();
            data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathType_Binary"), nameof(Properties.Resources.MathCalcTypeBinary), null));
            data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathType_FFT"), nameof(Properties.Resources.MathCalcTypeFFT), null/*new Bitmap(Properties.Resources.Fft, new Size(32, 28))*/));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcType_Zoom, nameof(Properties.Resources.MathCalcType_Zoom), true, Properties.Resources.Zoom));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcTypeFilter, nameof(Properties.Resources.MathCalcTypeFilter), true, null));
            //data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LvBo"), nameof(Properties.Resources.MathCalcTypeFilter), null));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcTypeERes, nameof(Properties.Resources.MathCalcTypeERes), true, null));
            //data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ERes"), nameof(Properties.Resources.MathCalcTypeERes), null));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcType_Histogram, nameof(Properties.Resources.MathCalcType_Histogram), true, Properties.Resources.Histogram));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcType_Track, nameof(Properties.Resources.MathCalcType_Track), true, Properties.Resources.Track));
            //cbsource.Add(new ComboBoxExItem(Properties.Resources.MathCalcType_Trend, nameof(Properties.Resources.MathCalcType_Trend), true, Properties.Resources.Trend));
            //data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathCalcType_Custom"), nameof(Properties.Resources.MathCalcTypeCustom), null));
            //data.Add(new SelectComboBox.ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathCalcType_UserProgram"), nameof(Properties.Resources.MathCalcTypeUserProgram), null));
            CbxCalcType.DataSource = data;
            //CbxCalcType.SelectValue = 0;
            //CbxCalcType.Text = data[0].Key;
            //CbxCalcType.UpdateImage(true,(MathType)CbxCalcType.SelectValue);
            this.CbxCalcType.SelectedIndexChanged += this.CbxCalcType_SelectedIndexChanged;
        }

        /// <summary>
        /// The InitOptionSubPageOnLoad.
        /// </summary>
        private void InitOptionSubPageOnLoad()
        {
            _OptionSubPage = GetOptionSubPage(Presenter.Args.Type, this);
            _ArgToCtrl = true;
            var form = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetWindow(Presenter.WindowId);
            if (form?.IsMainForm == false)
            {
                ChkIndependentWindow.Checked = true;
                SetHVisible(true);
            }
            else
            {
                ChkIndependentWindow.Checked = false;
                SetHVisible(false);
            }
            _ArgToCtrl = false;

            if (Presenter.Args.Type == MathType.Binary || Presenter.Args.Type == MathType.Filter ||
                Presenter.Args.Type == MathType.ERes || Presenter.Args.Type == MathType.Track || Presenter.Args.Type == MathType.Custom || Presenter.Args.Type == MathType.UserProgram)
            {
                ChkIndependentWindow.Visible = true;
                LblIndependentWindow.Visible = true;
            }
            else
            {
                ChkIndependentWindow.Visible = false;
                LblIndependentWindow.Visible = false;
            }

            if (Presenter.Args.Type == MathType.Histgram || Presenter.Args.Type == MathType.Track || Presenter.Args.Type == MathType.Trend)
            {
                CbxCalcType.Visible = false;
                LblCalcType.Visible = false;
            }
            else
            {
                CbxCalcType.Visible = true;
                LblCalcType.Visible = true;
            }

            TlpMath.Controls.Add(_OptionSubPage, 0, OptionSubPageIndex);
            //PnlHorizon.Visible = !(Presenter.Args.Type == MathType.FFT);
            //SetHVisible(PnlHorizon.Visible);
            TbxUnit.Text = Presenter.CustomUnit;
        }

        /// <summary>
        /// The TbxLabel_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.EventArgs"/>.</param>
        private void TbxLabel_Click(object sender, System.EventArgs e)
        {
            KeyboardForm kf = KeyboardForm.CreateInstance(TbxLabel.Text);
            kf.ShowDialogByEvent();
            if (kf.DialogResult == DialogResult.OK)
            {
                TbxLabel.Text = kf.Text;
            }
        }
        private void ChkLabelVisiblity_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.LabelVisibility = ChkLabelVisiblity.Checked;

            }
        }
        /// <summary>
        /// The TbxLabel_KeyPress.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyPressEventArgs"/>.</param>
        private void TbxLabel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (OnEnterKeyPressed(sender, e))
            {
                Presenter.Label = TbxLabel.Text;
            }
        }

        /// <summary>
        /// The TbxLabel_Leave.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void TbxLabel_Leave(object sender, EventArgs e)
        {
            Presenter.Label = TbxLabel.Text;
        }

        /// <summary>
        /// The TbxUnit_KeyPress.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyPressEventArgs"/>.</param>
        private void TbxUnit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (OnEnterKeyPressed(sender, e))
            {
                Presenter.CustomUnit = TbxUnit.Text;
                if (!Presenter.IsAutoUnit)
                {
                    Presenter.Unit = TbxUnit.Text;
                }
            }
        }

        /// <summary>
        /// The TbxUnit_Leave.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void TbxUnit_Leave(object sender, EventArgs e)
        {
            Presenter.Unit = TbxUnit.Text;
        }

        /// <summary>
        /// The VPosToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String VPosToString()
        {
            return new Quantity(Presenter.PosIndexBymDiv, Prefix.Milli, QuantityUnit.Division).ToString("#0.###", true);
        }

        /// <summary>
        /// The VScaleToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String VScaleToString()
        {
            return new Quantity(Presenter.Scale, Presenter.Prefix, Presenter.Unit).ToString();
        }
        Object _Locker = new Object();
        private void ChkIndependentWindow_CheckedChangedEvent(object sender, EventArgs e)
        {

            if (!_ArgToCtrl)
            {
                if (_ControlToCtrl)
                {
                    return;
                }
                ChkIndependentWindow.Enabled = false;
                _ControlToCtrl = true;
                NeedPrsnt = true;
                if (ChkIndependentWindow.Checked)
                {
                    SetHVisible(true);
                    Presenter.WindowId = IdFactory.NextId;
                }
                else
                {
                    var form = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetMainWindow();
                    if (form != null)
                    {
                        SetHVisible(false);

                        Presenter.WindowSwtich(form.WindowId, !ChkIndependentWindow.Checked);
                        //Presenter.WindowId = form.WindowId;
                    }
                }
                NeedPrsnt = false;
                _ControlToCtrl = false;
                ChkIndependentWindow.Enabled = true;
            }
        }

        private void SetHVisible(Boolean visible)
        {
            LblHScale.Visible = visible;
            NebHScale.Visible = visible;
            LblHPos.Visible = visible;
            NebHPos.Visible = visible;
            BtnResetHPos.Visible = visible;
        }

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (TbxLabel.Text.Length > 20)
                {
                    TbxLabel.Text = TbxLabel.Text.Substring(0, 20);
                }
                Presenter.Label = TbxLabel.Text;
            }
        }

        private void TbxUnit_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (TbxUnit.Text.Length > 6)
                {
                    TbxUnit.Text = TbxUnit.Text.Substring(0, 6);
                }
                else if (TbxUnit.Text.Length == 0)
                {
                    TbxUnit.Text = "V";
                }
                Presenter.CustomUnit = TbxUnit.Text;
            }
        }

        protected override void DestroyHandle()
        {
            if (TlpMath.Controls != null && TlpMath.Controls.Count > 0)
            {
                foreach (Control subpag in TlpMath.Controls)
                {
                    subpag.Dispose();
                }

                TlpMath.Controls.Clear();
            }
            base.DestroyHandle();
        }
    }
}
