// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/23</date>

using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class VerticalPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;
        private Boolean _ControlToCtrl = false;

        public VerticalPage()
        {
            InitializeComponent();
            TbxUnit.Visible = false;
            //LblUnitSelection.Visible = false;
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged += Timebase_PublisherChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            LangKeyInit();
            Init();
        }

        private void Timebase_PublisherChanged(object sender, CustomEventArg e)
        {
            if (e.Message == "SamplingScale")
            {
                LblChannelDelay.Enabled = NebChannelDelay.Enabled = Presenter.IsDelayable;
            }
        }


        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => LangKeyInit();

        private void LangKeyInit()
        {
            LblLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LabelVisiblity"); // "打开标签";
            ChkLabelVisiblity.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive", "CheckedText");
            ChkLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive"); //  "关";
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive", "CheckedText");
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive"); //  "关";
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblActive"); //  "显示";
            LblInvert.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblInvert"); //  "反相";
            LblbandLimit.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblbandLimit"); //  "带宽限制";
            ChkInvert.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkInvert", "CheckedText"); //  "开";
            ChkInvert.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkInvert"); //  "关";
            LblCoupling.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblCoupling"); // "耦合";
            LblScale.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblScale"); // "垂直刻度";
            LblLabel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblLabel"); // "标签";
            BtnResetPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.BtnResetPos"); // "设为0";
            LblPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblPosition"); // "位置";
            BtnResetBias.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.BtnResetBias"); // "设为0";
            LblBias.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblBias"); // "偏置电压";
            ChkIndependentWindow.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkIndependentWindow", "CheckedText"); // "开";
            ChkIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkIndependentWindow"); // "关";
            LblIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.LblIndependentWindow"); // "独立窗口";
            LblChannelDelay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDaoYanChi");
            LblAmplitudeSelection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.FuDuXiTiao"); // "幅度细调";
            ChkAmplitude.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive", "CheckedText");
            ChkAmplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive");
            BtnCopyToOtherChannel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.CopyTo");
        }

        NumberKeybordForm biasnkf = null;
        Object biasnkfLocker = new object();
        private void Init()
        {
            //NebScale
            ControlsHotKnob.Default.InitHotKnob(NebScale);
            ControlsHotKnob.Default.InitHotKnob(BtnBias);
            ControlsHotKnob.Default.InitHotKnob(BtnPosition);
            ControlsHotKnob.Default.InitHotKnob(NebChannelDelay);
            NebScale.AddClicked = (a, b) =>
            {
                if (Presenter.Ylevel_SelectStatus)
                {
                    Presenter.SetScaleValueBymV(1);
                }
                else
                {
                    Presenter.ScaleIndex++;
                }
            };
            NebScale.SubClicked = (a, b) =>
            {
                if (Presenter.Ylevel_SelectStatus)
                {
                    Presenter.SetScaleValueBymV(-1);
                }
                else
                {
                    Presenter.ScaleIndex--;
                }
            };
            NebScale.StringFormatFunc = (value) => ScaleToString();
            NebScale.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebScale, nameof(Presenter.ScaleIndex));

            };
            NebScale.EditValueChicked += (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.ScaleBymV = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.Prefix);

                nkf.SetKeyBoardValue(LblScale.Text, Presenter.Unit, 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.ScaleBymV, Presenter.Prefix),
                    Quantity.ConvertByPrefix(Presenter.MaxScale, Presenter.Prefix),
                    Quantity.ConvertByPrefix(Presenter.MinScale, Presenter.Prefix));

                nkf.ShowDialogByPosition();
            };
            //BtnBias
            BtnBias.Click += (_, _) =>
            {

                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnBias, nameof(Presenter.Bias), Quantity.ConvertByPrefix(20, Prefix.Empty, Prefix.Milli));
            };
            BtnBias.DoubleClick += (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnBias);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.Bias = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Micro);

                nkf.SetKeyBoardValue(LblBias.Text, Presenter.Unit, 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.Bias, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MaxBias, Prefix.Micro),
                    Quantity.ConvertByPrefix(Presenter.MinBias, Prefix.Micro));
                biasnkf = nkf;
                nkf.ShowDialogByPosition();
                lock (biasnkfLocker)
                {
                    biasnkf = null;
                }
            };
            BtnPosition.Click += (_, _) =>
            {

                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnPosition, nameof(Presenter.PosIndexBymDiv), Quantity.ConvertByPrefix(20, Prefix.Empty));
            };

            // 通道延迟初始化。
            NebChannelDelay.AddClicked = new Action<object, ScopeXNumericEditBox.NumericButtonEventData>(NebChannelDelayAddClickHandler);
            NebChannelDelay.SubClicked = new Action<object, ScopeXNumericEditBox.NumericButtonEventData>(NebChannelDelaySubClickHandler);
            NebChannelDelay.StringFormatFunc = a => DelayToString(a);
            NebChannelDelay.EditValueChicked = new Action<object, double>(NebChannelDelayEditValueClickHandler);
            NebChannelDelay.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebChannelDelay, nameof(Presenter.Delay));
            };
        }



        /// <summary>
        /// 通道延迟值编辑按钮点击事件。
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void NebChannelDelayEditValueClickHandler(object arg1, double arg2)
        {
            var maxposition = Quantity.ConvertByPrefix(Constants.MAX_CHANNEL_DELAY, Prefix.Empty);
            var minPosition = Quantity.ConvertByPrefix(Constants.MIN_CHANNEL_DELAY, Prefix.Empty);
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebChannelDelay);
            nkf.NumberKeyboard.MaxValue = maxposition;
            nkf.NumberKeyboard.Unit = QuantityUnit.Second.ToUnitString();
            var temp = (Prefix.Empty - Prefix.Empty) * 3 + (Int32)Math.Ceiling(Math.Abs(Math.Log10(Math.Abs(minPosition))));
            nkf.NumberKeyboard.MinValue = Math.Round(Quantity.ConvertByPrefix(minPosition, Prefix.Empty), temp > 15 ? 15 : temp);
            var current = Quantity.ConvertByPrefix(Presenter.Delay, Prefix.Empty);
            nkf.NumberKeyboard.DefaultValue = (current > maxposition) ? maxposition : current;
            nkf.NumberKeyboard.DecimalNumber = 3;
            nkf.Title = LblChannelDelay.Text;

            nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                Presenter.Delay = Quantity.ConvertByPrefix(args.Data, Prefix.Empty);
                nkf.Close();
            };

            _ = nkf.ShowDialogByPosition();
        }

        /// <summary>
        /// 通道延迟 - (左移)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="data"></param>
        private void NebChannelDelaySubClickHandler(object arg1, ScopeXNumericEditBox.NumericButtonEventData data)
        {
            // 左移，只需要移动当前通道
            Presenter.Delay -= Presenter.DelayStep;
        }

        /// <summary>
        /// 通道延迟 + (右移)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="data"></param>
        private void NebChannelDelayAddClickHandler(object arg1, ScopeXNumericEditBox.NumericButtonEventData data)
        {
            /*// 右移，移动所有通道，只是当前通道少移除指定点数，看起来就是右移。
            var annaChls = Program.Oscilloscope.GetAllChnls().Where(c => c.Id >= ChannelId.C1 && c.Id <= ChannelId.C4);

            // 目前以一级延时为准，右移时,所有通道都
            foreach (AnalogPrsnt chnlPresnt in annaChls)
            {
                if (chnlPresnt == Presenter)
                {
                    // 当前通道 +100ns - 右移time
                }
                else
                {
                    // 其它通道，确保已+100ns
                    //chnlPresnt.Delay += Quantity.ConvertByPrefix(100, Prefix.Nano);
                }

                //chnlPresnt.Delay += Presenter.DelayStep;
            }*/
            Presenter.Delay += Presenter.DelayStep;
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

        public AnalogPrsnt Presenter
        {
            get => (AnalogPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (AnalogPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        private void ResetCbxCoupling()
        {
            Int32 newindex = 0;
            if (!Presenter.SerailNumber.Contains("X"))
                newindex = (Int32)Presenter.Coupling;
            else
                newindex = (Int32)(Presenter.Coupling);// - 2);

            if ((Int32)CbxCoupling.SelectValue != newindex)
            {
                CbxCoupling.SelectValue = newindex;
            }
            //if (Presenter.ProbeConnected)
            //{
            //    if (PlatformUIManager.Default.Platform.Attribute.SupportHighImpedance || Presenter.SerailNumber.Contains(AnaChnlProbe.x1.GetAlias().First()))
            //        CbxCoupling.Enabled = false;
            //}
            //else
            //    CbxCoupling.Enabled = true;
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;
            switch (propertyName)
            {
                case "ConditioningScale":
                case nameof(Presenter.ScaleBymV):
                case nameof(Presenter.ProbeIndex):
                case nameof(Presenter.Bias):
                    if (PlatformUIManager.Default.Platform.Attribute.SupportHighImpedance)
                    {
                        //Modify by LHJ 带宽限制要根据真实值控制，而不是Presenter.ScaleBymV单一判断
                        var realval = Presenter.ScaleBymV / (Presenter.ProbeGain * Presenter.ProbeUnitRatio);
                        if (realval <= 5 && Constants.ENABLE_BANDWIDTH)
                        {
                            Presenter.Bandwidth = Presenter.BandWidthNames.Last().Index;
                            if (Presenter.Coupling == AnaChnlCoupling.DC50)
                                //{
                                //    Presenter.Bandwidth = Constants.LZBANDWIDTHNAMES.Last().Index;
                                //}
                                //else
                                //{
                                //    Presenter.Bandwidth = Constants.HZBANDWIDTHNAMES.Last().Index;
                                //}
                                ChkBandLimit.Enabled = false;
                        }
                        else
                        {
                            ChkBandLimit.Enabled = true;
                        }
                    }
                    NebScale.UpdateValueString();
                    BtnBias.Text = BiasToString();
                    break;
                case "ConditioningPosition":
                    BtnPosition.Text = PosToString();
                    break;
                case nameof(Presenter.Delay):
                    NebChannelDelay.Value = Presenter.Delay;
                    NebChannelDelay.UpdateValueString();
                    break;
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    break;
                case nameof(Presenter.LabelVisibility):
                    ChkLabelVisiblity.Checked = Presenter.LabelVisibility;
                    break;
                case nameof(Presenter.IsInverted):
                    ChkInvert.Checked = Presenter.IsInverted;
                    break;
                case nameof(Presenter.Coupling):
                    LoadCouplingList();
                    if (Presenter.IsUpdateCouplingBack)
                    {
                        // 高压警告反更新界面，这里必须先取消改变事件，防止通知递归
                        CbxCoupling.SelectedIndexChanged -= CbxCoupling_SelectedIndexChanged;
                        CbxCoupling.SelectValue = (Int32)Presenter.Coupling;
                        CbxCoupling.SelectedIndexChanged += CbxCoupling_SelectedIndexChanged;
                    }
                    else
                    {
                        ResetCbxCoupling();
                    }
                    LoadBandwidthList();
                    BtnBias.Text = BiasToString();
                    break;
                case nameof(Presenter.Bandwidth):
                    LoadBandwidthList();
                    //CbxBandLimit.SelectedValue = (Int32)Presenter.Bandwidth;
                    ChkBandLimit.SelectValue = Presenter.Bandwidth;
                    break;
                case nameof(Presenter.SerailNumber):
                    LoadCouplingList();
                    ResetCbxCoupling();
                    break;
                case "ConditioningScaleUnit":
                    TbxUnit.Text = Presenter.Unit;
                    NebScale.UpdateValueString();
                    ProbeUnitChanged();
                    BtnBias.Text = BiasToString();
                    break;
                case nameof(Presenter.Label):
                    TbxLabel.Text = Presenter.Label;
                    break;
                case "InterleaveMode":
                    TbxLabel.Text = Presenter.Label;
                    break;
                case nameof(Presenter.Ylevel_SelectStatus):
                    ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;
                    break;
                case nameof(Presenter.ProbeConnected):
                    //if (Presenter.ProbeConnected)
                    //{
                    //    if (PlatformUIManager.Default.Platform.Attribute.SupportHighImpedance || Presenter.SerailNumber.Contains(AnaChnlProbe.x1.GetAlias().First()))
                    //        CbxCoupling.Enabled = false;
                    //}
                    //else
                    //    CbxCoupling.Enabled = true;
                    break;
            }
            LoadBandwidthList();
            lock (biasnkfLocker)
            {
                if (biasnkf != null)
                {
                    biasnkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.MaxBias, Prefix.Micro);
                    biasnkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.MinBias, Prefix.Micro);
                }
            }
            _ArgToCtrl = false;
        }

        /// <summary>
        /// 延迟数字格式化程序
        /// </summary>
        /// <returns></returns>
        private string DelayToString(double v) => new Quantity(v, Prefix.Empty, "s").ToString("#0.###", true);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadOtherChannels();
            ChannelDelayInit();
            LoadCouplingList();
            LoadBandwidthList();
            UpdateView();

            var form = this.FindForm();
            if (form != null)
            {
                form.FormClosed += F_FormClosed;
            }
        }

        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged -= Timebase_PublisherChanged;
            var form = sender as Form;
            if (form != null)
            {
                form.FormClosed -= F_FormClosed;
            }
        }

        /// <summary>
        /// 通道延时功能状态初始化
        /// </summary>
        private void ChannelDelayInit()
        {
            NebChannelDelay.Interval = Presenter.DelayStep;

            var resultval = Presenter.Delay;
            if (resultval > 0 && resultval < Presenter.DelayStep)
            {
                resultval = Presenter.DelayStep;
                Presenter.Delay = resultval;
            }
            else if (resultval < 0 && Math.Abs(resultval) < Presenter.DelayStep)
            {
                resultval = -Presenter.DelayStep;
                Presenter.Delay = resultval;
            }

            NebChannelDelay.Value = resultval;
            NebChannelDelay.UpdateValueString();
            var chnneldelaystr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDaoYanChi");
            LblChannelDelay.Text = chnneldelaystr;//$"{chnnelDelayStr}({stepstr}：{new Quantity(Presenter.DelayStep, Prefix.Empty, QuantityUnit.Second).ToString("###", true)})";
            // 设置通道延迟按钮禁用状态
            LblChannelDelay.Enabled = NebChannelDelay.Enabled = Presenter.IsDelayable;
        }

        private void LoadCouplingList()
        {
            CbxCoupling.SelectedIndexChanged -= CbxCoupling_SelectedIndexChanged;
            CbxCoupling.DataSource = Presenter.SupportedCouplings.Select(o => new SelectComboBox.ComboBoxItem(o.GetAlias(), (object)o)).ToList();
            if (Presenter.SerailNumber.Contains("X"))
            {
                if (Presenter.Coupling == AnaChnlCoupling.DC1M || Presenter.Coupling == AnaChnlCoupling.AC1M)
                    Presenter.Coupling = AnaChnlCoupling.DC50;
            }

            //CbxCoupling.DisplayMember = "Key";
            //CbxCoupling.ValueMember = "Value";
            CbxCoupling.SelectValue = Presenter.Coupling;

            if (Presenter != null)
                CbxCoupling.SelectValue = Presenter.Coupling;

            CbxCoupling.SelectedIndexChanged += CbxCoupling_SelectedIndexChanged;
        }

        private void LoadOtherChannels()
        {
            ChkOtherChannelSelect.DataSource = ChannelIdExt.GetAnalogs().Where(x => !x.Equals(Presenter.Id)).Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
        }

        private void CbxCoupling_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AnaChnlCoupling newcoupling = AnaChnlCoupling.DC50;
                if (!Presenter.SerailNumber.Contains("X"))
                    newcoupling = (AnaChnlCoupling)CbxCoupling.SelectValue;
                else
                    newcoupling = (AnaChnlCoupling)((Int32)(AnaChnlCoupling)CbxCoupling.SelectValue);// + 2);

                // 防止反复触发属性更改。
                if (newcoupling != Presenter.Coupling)
                {
                    Presenter.Coupling = newcoupling;
                }
            }
        }

        private void LoadBandwidthList()
        {
            Int32 lastselectedindex = Presenter.Bandwidth;
            _ArgToCtrl = true;
            //var temps = (Presenter.Coupling == AnaChnlCoupling.DC50 ? ComModel.Constants.LZBANDWIDTHNAMES : Constants.HZBANDWIDTHNAMES).Select(o => new SelectComboBox.ComboBoxItem(o.Name, o.Index)).ToList();
            var temps = Presenter.BandWidthNames.Select(o => new SelectComboBox.ComboBoxItem(o.Name, o.Index)).ToList();
            ChkBandLimit.DataSource = temps;
            ChkBandLimit.SelectValue = (Int32)Presenter.Bandwidth;
            ChkBandLimit.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Bandwidth = (Int32)ChkBandLimit.SelectValue;
                }
            };
            if (temps.FindIndex(x => (Int32)x.Item2 == lastselectedindex) == -1)
            {
                ChkBandLimit.SelectValue = temps.First().Item2;
                Presenter.Bandwidth = (Int32)temps.First().Item2;
            }
            else
            {
                ChkBandLimit.SelectValue = lastselectedindex;
            }

            if (PlatformUIManager.Default.Platform.Attribute.SupportHighImpedance)
            {
                if (Presenter.Coupling == AnaChnlCoupling.DC50 && Constants.ENABLE_BANDWIDTH)
                {
                    if (Presenter.Sampling.InterleaveMode == AdcInterleaveMode.Mode1To1 || Presenter.Sampling.InterleaveMode == AdcInterleaveMode.Mode2To1)
                    {
                        BandLimit(false);
                    }
                    else
                    {
                        BandLimit(true);
                    }
                }
            }

            //Modify by LHJ 带宽限制要根据真实值控制，而不是Presenter.ScaleBymV单一判断
            var realval = Presenter.ScaleBymV / (Presenter.ProbeGain * Presenter.ProbeUnitRatio);
            if (realval <= 5 && Constants.ENABLE_BANDWIDTH)
            {
                Presenter.Bandwidth = Presenter.BandWidthNames.Last().Index;
                //if (Presenter.Coupling == AnaChnlCoupling.DC50)
                //{
                //    Presenter.Bandwidth = Constants.LZBANDWIDTHNAMES.Last().Index;
                //}
                //else
                //{
                //    Presenter.Bandwidth = Constants.HZBANDWIDTHNAMES.Last().Index;
                //}
                ChkBandLimit.Enabled = false;
            }
            else
            {
                ChkBandLimit.Enabled = true;
            }
            _ArgToCtrl = false;
        }

        private void BandLimit(Boolean visible)
        {
            if (this.ChkBandLimit.DataSource is List<SelectComboBox.ComboBoxItem> list)
            {
                var item = list.Find((a) => { return (Int32)a.Value == 0; });
                if (item != null && item.Value != null)
                {
                    item.Enabled = visible;
                }
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                NebScale.UpdateValueString();
                BtnPosition.Text = PosToString();
                NebChannelDelay.UpdateValueString();
                ChkActive.Checked = Presenter.Active;
                ChkLabelVisiblity.Checked = Presenter.LabelVisibility;
                ChkInvert.Checked = Presenter.IsInverted;
                //if (!Presenter.SerailNumber.Contains("X1"))
                //    CbxCoupling.SelectedIndex = (Int32)Presenter.Coupling;
                //else
                //    CbxCoupling.SelectedIndex = (Int32)Presenter.Coupling - 2;
                ResetCbxCoupling();
                ChkBandLimit.SelectValue = Presenter.Bandwidth;
                TbxUnit.Text = Presenter.Unit;
                TbxLabel.Text = Presenter.Label;
                ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;

                BtnBias.Text = BiasToString();
                ProbeUnitChanged();
                SetIndepentFigState();
                _ArgToCtrl = false;
            }
        }

        private void SetIndepentFigState()
        {
            var figmanager = (ParentForm.Owner as DsoForm).MultiWindowManager;
            if (Presenter.WindowId == figmanager.MainFigure.WindowId)
            {
                ChkIndependentWindow.Checked = false;
                Int32 count = figmanager.MainFigure.GetPlotWaveChannelIds().Where(x => x.IsAnalog()).Count();
                if (count < 2)
                {
                    ChkIndependentWindow.Enabled = false;
                }
            }
            else
            {
                ChkIndependentWindow.Checked = true;
            }
        }
        private void ProbeUnitChanged()
        {
            //探头单位删除
            //if (Presenter.Unit == Presenter.ProbeUnit.ToString())
            //    return;
            //if (Presenter.Unit == "V")
            //{
            //    Presenter.ProbeUnit = ProbeUnitType.V;
            //}
            //else if (Presenter.Unit == "A")
            //{
            //    Presenter.ProbeUnit = ProbeUnitType.A;
            //}
            //else if (Presenter.Unit == "W")
            //{
            //    Presenter.ProbeUnit = ProbeUnitType.W;
            //}
            //else if (Presenter.Unit == "U")
            //{
            //    Presenter.ProbeUnit = ProbeUnitType.U;
            //}
        }
        private String BiasToString()
        {
            return new Quantity(Presenter.Bias, Prefix.Micro, Presenter.Unit).ToString("#0.###", true);
        }

        private void BtnPosition_DoubleClick(Object sender, EventArgs e)
        {
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnPosition);
            Action<Double> onokclickeventaction = new Action<Double>((data) =>
                Presenter.PosIndexBymDiv = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

            nkf.SetKeyBoardValue(LblPosition.Text, QuantityUnit.Division.ToUnitString(), 3, onokclickeventaction,
                Quantity.ConvertByPrefix(Presenter.PosIndexBymDiv, Prefix.Milli),
                Quantity.ConvertByPrefix(Presenter.PosMaxIndex, Prefix.Milli),
                Quantity.ConvertByPrefix(Presenter.PosMinIndex, Prefix.Milli));

            nkf.ShowDialogByPosition();
        }

        private void BtnResetBias_Click(Object sender, EventArgs e)
        {
            Presenter.Bias = 0;
        }

        private void BtnResetPos_Click(Object sender, EventArgs e)
        {
            Presenter.PosIndexBymDiv = 0;
        }


        private void BtnCopyToOtherChannel_Click(object sender, System.EventArgs e)
        {
            if (ChkOtherChannelSelect.SelectValue is ChannelId ch && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ch, out var chp))
            {
                var prsnt = (AnalogPrsnt)chp;
                prsnt?.CopyParasFormOtherChannel(Presenter);
                //prst.ScaleIndex = Presenter.ScaleIndex;
                //prst.PosIndexBymDiv = Presenter.PosIndexBymDiv;
                //prst.Bias = Presenter.Bias;
                //prst.Label = Presenter.Label;
                //prst.Delay = Presenter.Delay;
                //prst.Coupling = Presenter.Coupling;
                //prst.Bandwidth = Presenter.Bandwidth;
                //prst.IsInverted = Presenter.IsInverted;
                //prst.Ylevel_SelectStatus = Presenter.Ylevel_SelectStatus;
                ////prst.Active = Presenter.Active;
            }
        }

        private void ChkLabelVisiblity_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.LabelVisibility = ChkLabelVisiblity.Checked;

            }
        }

        private void ChkActive_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                (ParentForm as FloatForm).CanClose = false;
                Presenter.Active = ChkActive.Checked;
                if (Presenter.Active)
                {
                    DsoPrsnt.FocusId = Presenter.Id;
                }
                (ParentForm as FloatForm).CanClose = true;
            }
        }

        private void ChkInvert_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IsInverted = ChkInvert.Checked;
            }
        }

        private String PosToString()
        {
            return new Quantity(Presenter.PosIndexBymDiv, Prefix.Milli, "div").ToString("#0.###", true);
        }

        private String ScaleToString()
        {
            return new Quantity(Presenter.ScaleBymV, Presenter.Prefix, Presenter.Unit).ToString();
        }

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
                (ParentForm as FloatForm).CanClose = false;
                if (ChkIndependentWindow.Checked)
                {
                    Presenter.WindowId = ChannelPrsnt.GetNewWindowId();
                }
                else
                {
                    Presenter.WindowId = (ParentForm.Owner as DsoForm).MultiWindowManager.MainFigure.WindowId;
                    DsoPrsnt.FocusId = Presenter.Id;
                }
                (ParentForm as FloatForm).CanClose = true;
                _ControlToCtrl = false;
                ChkIndependentWindow.Enabled = true;

            }
        }

        private void TbxUnit_SelectedIndexChanged(Object sender, EventArgs e)
        {
            Presenter.Unit = TbxUnit.Text;
        }

        private void TbxLabel_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (TbxLabel.Text.Length > 20)
            {
                TbxLabel.Text = TbxLabel.Text.Substring(0, 20);
            }
            Presenter.Label = TbxLabel.Text;
        }

        /// <summary>
        /// 幅度细调开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkAmplitude_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkAmplitude.Checked)
            {
                Presenter.Ylevel_SelectStatus = true;
            }
            else
            {
                Presenter.Ylevel_SelectStatus = false;
            }
        }

        /*
        /// <summary>
        /// 通道延迟置零按钮点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetChannelDelay_Click(object sender, EventArgs e)
        {
            Presenter.Delay = 0;
        }

        /// <summary>
        /// 通道延迟输入按钮点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnChannelDelay_Click(object sender, EventArgs e)
        {
            
        }*/
    }
}
