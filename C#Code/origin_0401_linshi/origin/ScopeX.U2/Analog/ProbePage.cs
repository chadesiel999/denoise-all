
//#define  UseNewProbeCali 正式上线前关闭

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Threading.Tasks;
using EventBus;
using WebSocketSharp.NetCore;
using ScopeX.UserControls;
using System.Xml.Linq;
using PdfSharpCore.Drawing;


namespace ScopeX.U2
{

    public partial class ProbePage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;
        private String[] ProbeInfo;
        public ProbePage()
        {
            InitializeComponent();
            //Init();

        }

        private void Init()
        {
            InitCbxValue();
            InitCbxProbeMagValue();
            InitHotKnobValue();
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

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.ProbeIndex):
                    //CbxProbeMag.SelectedIndex = (Int32)Presenter.ProbeIndex;
                    CbxProbeMag.SelectIndex = (Int32)Presenter.ProbeIndex;
                    ChangeCtrlState();
                    break;
                case nameof(Presenter.ProbeGain):
                    //电流探头倍率单位细致化 LHJ
                    //if (Presenter.ProbeUnit == ProbeUnitType.A)
                    //{
                    //    double Val = (1D / Presenter.ProbeGain);
                    //    if (Val < 1)
                    //    {
                    //        BtnCustomGain.Text = (Val * 1000).ToString() + " mV/A";
                    //    }
                    //    else
                    //    {
                    //        int iVal = (int)Val;
                    //        BtnCustomGain.Text = (iVal).ToString() + " V/A";
                    //    }
                    //}
                    //else
                    {

                        var quantity = new Quantity(Presenter.ProbeGain, Prefix.Empty, "X").ToString("#0.###", true);

                        BtnCustomGain.Text = quantity;
                    }
                    break;
                //case nameof(Presenter.ProbeUnit):
                //    CbxProbeUnitType.SelectValue = Presenter.ProbeUnit;
                //    InitCbxProbeMagValue();
                //    CbxProbeMag.SelectIndex = (Int32)Presenter.ProbeIndex;
                //    ChangeCtrlState();
                //    break;
                case nameof(Presenter.ProbeUnitIsCustomized):
                    PrsntChangedWithUnit();
                    break;
                case "ConditioningScaleUnit":
                    //界面修改Presenter.Unit后，回调值此处
                    PrsntChangedWithUnit();
                    break;
                case nameof(Presenter.ProbeUnitRatio):
                    PrsntChangedWithUnitRatio();
                    break;
                case nameof(Presenter.ProbeBtnType):
                    //CbxProbeBtnType.SelectedIndex = (Int32)Presenter.ProbeBtnType;
                    CbxProbeBtnType.SelectValue = Presenter.ProbeBtnType;
                    //CbxProbeBtnType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(Presenter.ProbeBtnType));
                    break;
                case nameof(Presenter.ProbeHwVerion):
                    if (Presenter.ProbeHwVerion.IsNullOrEmpty())
                    {
                        LblOther.Text = "";
                    }
                    else
                    {
                        LblOther.Text = "Version " + Presenter.ProbeHwVerion;
                    }
                    break;
                case nameof(Presenter.SerailNumber):
                    //CbxProbeMag.Enabled = true;
                    SerailNumberAnalysis();
                    break;
                case nameof(Presenter.ProbeConnected):
                    //CbxProbeMag.Enabled = true;
                    ProbeConnectChanged();
                    if (!Presenter.ProbeConnected)
                    {
                        Presenter.Coupling = AnaChnlCoupling.DC1M;
                        Presenter.ProbeIndex = AnaChnlProbe.x1;
                    }
                    BtnDCBiasSetting.Text = ProbeDCBiasToString();
                    break;
                case nameof(Presenter.ProbeDCBiasBymV):
                    BtnDCBiasSetting.Text = ProbeDCBiasToString();
                    break;
            }
            _ArgToCtrl = false;

            if (Constants.BOARD_ATTACHED == false)
            {//如果处于仿真模式，默认显示探头校准按钮
                BtnProbeCali.Visible = true;
                LblDCBiasSetting.Visible = true;
                BtnDCBiasSetting.Visible = true;
                BtnProbeDefault.Visible = true;
            }

            //if (Constants.PROBE_FACT_CALIB)
            //{//如果开启了就启用示波器中探头的基础校准
            //    BtnProbeDefault.Visible = BtnProbeCali.Visible;
            //}
            //else
            //{
            //    BtnProbeDefault.Visible = false;
            //}
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkProbeUnitCustom.Checked = Presenter.ProbeUnitIsCustomized;
                TbxUnit.Text = Presenter.Unit;
                PrsntChangedWithUnit();

                //CbxProbeMag.SelectedIndex = (Int32)(AnaChnlProbe)Presenter.ProbeIndex;
                CbxProbeMag.SelectIndex = (Int32)Presenter.ProbeIndex;

                //CbxProbeUnitType.SelectedIndex = (Int32)Presenter.ProbeUnit;
                //CbxProbeUnitType.SelectValue = Presenter.ProbeUnit;

                //RdoInputSource.ChoosedButtonIndex = (Int32)(AnaChnlIpnutSource)Presenter.FlagInfo;
                CbxProbeBtnType.SelectValue = Presenter.ProbeBtnType;
                //CbxAttenuation.SelectedIndex = (Int32)Presenter.AttenuationType;
                //NebDeskew.UpdateValueString();
                BtnDCBiasSetting.Text = ProbeDCBiasToString();
                ChangeCtrlState();
                SerailNumberAnalysis();
                ProbeConnectChanged();
                _ArgToCtrl = false;
            }

            if (Constants.BOARD_ATTACHED == false)
            {//如果处于仿真模式，默认显示探头校准按钮
                BtnProbeCali.Visible = true;
                LblDCBiasSetting.Visible = true;
                BtnDCBiasSetting.Visible = true;
                BtnProbeDefault.Visible = true;
            }

            //if (Constants.PROBE_FACT_CALIB)
            //{//如果开启了就启用示波器中探头的基础校准
            //    BtnProbeDefault.Visible = BtnProbeCali.Visible;
            //}
            //else
            //{
            //    BtnProbeDefault.Visible = false;
            //}

        }

        private String DeskewToString() => new Quantity(Presenter.Deskew, Prefix.Femto, "s").ToString("##0.000", true, 5);
        private void ContorlTextRefresh()
        {
            //LblProbeUnit.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProbeUnit"); // "探头单位";
            LblProbeMag.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.LblProbeMag"); // "探头倍率";
            LblCustomGain.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.LblCustomGain"); // "自定义倍率";
            LblProbeBtn.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.LblProbeBtn"); // "探头按钮";
            LblInfo.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.LblInfo"); // "厂商：";
            LblManufacturerName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.PnlInfo.tableLayoutPanel1.ScopeXLabel1"); // "厂商：";
            LblMagName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.PnlInfo.tableLayoutPanel1.ScopeXLabel2"); // "倍率：";
            LblModelName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.PnlInfo.tableLayoutPanel1.ScopeXLabel4"); // "型号：";
            LblSNName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.PnlInfo.tableLayoutPanel1.ScopeXLabel5"); // "序列号：";
            LblOtherName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.PnlInfo.tableLayoutPanel1.ScopeXLabel6"); // "其它：";
            BtnProbeCali.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.AutoCalib"); // "自动校准：";
            BtnProbeDefault.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.ProbeDefault"); // "恢复默认";
            LblUnitSelection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BeiYongDanWei"); // "备用单位"
            LblUnitCvtText.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DanWeiBiLv"); // "单位比率"
            ChkProbeUnitCustom.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");//"开"
            ChkProbeUnitCustom.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");//"开"
            LblDCBiasSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.ProbePage.LblDCBiasSetting"); // "直流偏设置";
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ContorlTextRefresh();
            Init();
            UpdateView();
        }

        private void InitCbxValue()
        {
            _ArgToCtrl = true;
            CbxProbeBtnType.DataSource = Enum.GetValues<ProbeKeyType>().Select(o => new KeyValuePair<String, object>(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(o)), o)).ToList();

            CbxProbeBtnType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.ProbeBtnType = (ProbeKeyType)CbxProbeBtnType.SelectValue;
                }
            };
            //CbxAttenuation.DataSource = Enum.GetValues<AttenuationType>().Select(o => new KeyValuePair<String, AttenuationType>(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(o)), o)).ToList();
            //CbxAttenuation.DisplayMember = "Key";
            //CbxAttenuation.ValueMember = "Value";
            //CbxProbeUnitType.DataSource = Enum.GetValues<ProbeUnitType>().Select(o => new KeyValuePair<String, ProbeUnitType>(o.ToString(), o)).ToList();
            //CbxProbeUnitType.DisplayMember = "Key";
            //CbxProbeUnitType.ValueMember = "Value";

            //CbxProbeUnitType.DataSource = Enum.GetValues<ProbeUnitType>().Select(o => new KeyValuePair<String, object>(o.ToString(), o)).ToList();

            _ArgToCtrl = false;
        }
        private void InitCbxProbeMagValue()
        {
            _ArgToCtrl = true;
            //if (Presenter.ProbeUnit == ProbeUnitType.A)
            //{

            //    CbxProbeMag.DataSource = Enum.GetValues<AnaChnlProbeBymVA>().Select(o => new KeyValuePair<String, object>(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(o)), o)).ToList();

            //}
            //else
            {

                CbxProbeMag.DataSource = Enum.GetValues<AnaChnlProbe>().Select(o => new KeyValuePair<String, object>(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(o)), o)).ToList();

            }
            //Modify by LHJ 改变数据源后需要重设SelectIndex一次触发界面当前选项刷新
            CbxProbeMag.SelectIndex = -1;
            CbxProbeMag.SelectIndex = (Int32)Presenter.ProbeIndex;
            _ArgToCtrl = false;
        }
        private void InitHotKnobValue()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnCustomGain, BtnUnitRatio, BtnUnitRatioInverted);

            BtnCustomGain.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnCustomGain, nameof(Presenter.ProbeGain), Quantity.ConvertByPrefix(20, Prefix.Empty));
            };

            BtnUnitRatio.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnUnitRatio, nameof(Presenter.ProbeUnitRatio));
            };

            BtnUnitRatioInverted.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnUnitRatioInverted, nameof(Presenter.ProbeUnitRatio), Quantity.ConvertByPrefix(20, Prefix.Empty));
            };
            //BtnDCBiasSetting.Click += (_, _) =>
            //{
            //    ControlsHotKnob.Default.SetHotKnob(Presenter, BtnDCBiasSetting, nameof(Presenter.ProbeDCBiasBymV), Quantity.ConvertByPrefix(20, Prefix.Empty));
            //};
        }
        private void SerailNumberAnalysis()
        {
            CbxProbeMag.Enabled = true;
            if (Presenter.SerailNumber.Contains(AnaChnlProbe.x1.GetAlias().First()))
            {
                try
                {
                    BtnProbeCali.Visible = true;
                    BtnProbeDefault.Visible = true;
                    LblInfo.Visible = true;
                    PnlInfo.Visible = true;
                    LblDCBiasSetting.Visible = true;
                    BtnDCBiasSetting.Visible = true;
                    LblProbeBtn.Visible = true;
                    CbxProbeBtnType.Visible = true;
                    String str = Presenter.SerailNumber.Replace("\r", "");
                    ProbeInfo = str.Split(',');
                    LblManufacturer.Text = ProbeInfo[0];
                    LblModel.Text = ProbeInfo[1];
                    LblSN.Text = ProbeInfo[2];
                    LblMag.Text = ProbeInfo[3];
                    if (ProbeInfo[3] == AnaChnlProbe.x1.GetAlias())
                    {
                        //CbxProbeMag.SelectedIndex = 0;
                        //CbxProbeMag.Enabled = false;
                        CbxProbeMag.SelectIndex = (Int32)AnaChnlProbe.x1;
                        CbxProbeMag.Enabled = false;
                    }
                    else if (ProbeInfo[3] == AnaChnlProbe.x5.GetAlias())
                    {
                        //CbxProbeMag.SelectedIndex = 1;
                        //CbxProbeMag.Enabled = false;
                        CbxProbeMag.SelectIndex = (Int32)AnaChnlProbe.x5;
                        CbxProbeMag.Enabled = false;
                    }
                    else if (ProbeInfo[3] == AnaChnlProbe.x10.GetAlias())
                    {
                        //CbxProbeMag.SelectedIndex = 1;
                        //CbxProbeMag.Enabled = false;
                        CbxProbeMag.SelectIndex = (Int32)AnaChnlProbe.x10;
                        CbxProbeMag.Enabled = false;
                    }
                    else if (ProbeInfo[3] == AnaChnlProbe.x100.GetAlias())
                    {
                        //CbxProbeMag.SelectedIndex = 2;
                        //CbxProbeMag.Enabled = false;
                        CbxProbeMag.SelectIndex = (Int32)AnaChnlProbe.x100;
                        CbxProbeMag.Enabled = false;
                    }
                }
                catch
                {
                    LblDCBiasSetting.Visible = false;
                    BtnDCBiasSetting.Visible = false;
                    BtnProbeCali.Visible = false;
                    BtnProbeDefault.Visible = false;
                    LblInfo.Visible = false;
                    PnlInfo.Visible = false;
                }
            }
            else
            {
                if (Presenter.ProbeConnected && Presenter.SerailNumber != "")
                {
                    //CbxProbeMag.SelectedIndex = 1;
                    CbxProbeMag.SelectIndex = (Int32)AnaChnlProbe.x10;
                    Presenter.Coupling = AnaChnlCoupling.DC1M;
                    // CbxProbeMag.Enabled = false;
                    CbxProbeMag.Enabled = false;
                }
                else
                {
                    //CbxProbeMag.Enabled = true;
                    CbxProbeMag.Enabled = true;
                    //RdoProbeMag.ChoosedButtonIndex = 0;
                }
                BtnProbeCali.Visible = false;
                BtnProbeDefault.Visible = false;
                LblInfo.Visible = false;
                PnlInfo.Visible = false;
                LblDCBiasSetting.Visible = false;
                BtnDCBiasSetting.Visible = false;
                LblProbeBtn.Visible = false;
                CbxProbeBtnType.Visible = false;
                LblOther.Text = String.Empty;
                LblManufacturer.Text = String.Empty;
                LblModel.Text = String.Empty;
                LblSN.Text = String.Empty;
                LblMag.Text = String.Empty;
            }
        }
        private void ProbeConnectChanged()
        {
            if (Presenter.ProbeConnected && Presenter.SerailNumber == "")
            {
                if (!PlatformUIManager.Default.Platform.Attribute.SupportHighImpedance)
                    return;
                Presenter.Coupling = AnaChnlCoupling.DC1M;
                Presenter.ProbeIndex = AnaChnlProbe.x10;
                //CbxProbeMag.Enabled = false;
                CbxProbeMag.Enabled = false;
            }
            if (!Presenter.ProbeConnected)
            {
                CbxProbeMag.Enabled = true;
            }
        }
        private void ChangeCtrlState()
        {
            if (Presenter.ProbeIndex == AnaChnlProbe.custom)
            {
                LblCustomGain.Visible = true;
                BtnCustomGain.Visible = true;
                //电流探头倍率单位细致化 LHJ
                //if (Presenter.ProbeUnit == ProbeUnitType.A)
                //{
                //    double Val = (1D / Presenter.ProbeGain);
                //    if (Val < 1)
                //    {
                //        BtnCustomGain.Text = (Val * 1000).ToString() + " mV/A";
                //    }
                //    else
                //    {
                //        int iVal = (int)Val;
                //        BtnCustomGain.Text = (iVal).ToString() + " V/A";
                //    }
                //}
                //else
                {
                    var quantity = new Quantity(Presenter.ProbeGain, Prefix.Empty, "X").ToString("#0.###", true);

                    BtnCustomGain.Text = quantity;
                }
            }
            else
            {
                LblCustomGain.Visible = false;
                BtnCustomGain.Visible = false;
            }
        }

        private String ValueToString(double value, Prefix prefix, QuantityUnit unit)
        {
            return new Quantity(value, prefix, unit).ToString("##0.###", true);
        }
        //private void RdoProbeMag_IndexChanged(Object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.ProbeIndex = (AnaChnlProbe)RdoProbeMag.ChoosedButtonIndex;
        //    }
        //}

        //private void RdoInputSource_IndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.FlagInfo = (AnaChnlIpnutSource)RdoInputSource.ChoosedButtonIndex;
        //    }
        //}

        private void BtnResetDeskew_Click(Object sender, EventArgs e)
        {
            Presenter.Deskew = 0;
        }

        private void CbxProbeBtnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ProbeBtnType = (ProbeKeyType)CbxProbeBtnType.SelectValue;
            }
        }

        //private void CbxAttenuation_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.AttenuationType = (AttenuationType)CbxAttenuation.SelectedIndex;
        //    }
        //}

        private void BtnCustomGain_Click(object sender, EventArgs e)
        {
            //根据单位给数据面板的初始化值分别做处理
            //if (Presenter.ProbeUnit == ProbeUnitType.A)
            //{
            //    string unit = "V/A";
            //    var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnCustomGain);

            //    //Presenter.ProbeGain辅助回激发内部转换，转换后的至于新值相同，回导致新值设置失败
            //    //需要给 Presenter.ProbeGain.set 中修改
            //    var oncomfirm = new Action<Double>((data) =>
            //    {
            //        Presenter.ProbeGain = (Double)data;
            //    });

            //    nkf.SetKeyBoardValue(LblCustomGain.Text, unit, 3, oncomfirm, 1 / Presenter.ProbeGain, Presenter.MaxProbeGain, Presenter.MinProbeGain);
            //    nkf.ShowDialogByPosition();
            //}
            //else
            {
                string unit = "X";
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnCustomGain);
                var oncomfirm = new Action<Double>((data) =>
                {
                    Presenter.ProbeGain = (Double)data;
                });

                nkf.SetKeyBoardValue(LblCustomGain.Text, unit, 3, oncomfirm, Presenter.ProbeGain, Presenter.MaxProbeGain, Presenter.MinProbeGain);
                nkf.ShowDialogByPosition();
            }

        }

        /// <summary>
        /// 界面用户修改单位
        /// 
        /// 同步至于P层对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /// <remark>
        /// added by lihuijun
        /// </remark>
        private void TbxUnit_SelectedIndexChanged(Object sender, EventArgs e)
        {
            var txt = TbxUnit.Text.Trim();
            if (txt.IsNullOrEmpty())
            {
                txt = "V";
            }

            if (txt.Length > 5)
            {
                txt = txt.Substring(0, 5);
            }
            TbxUnit.Text = txt;
            Presenter.Unit = txt;
        }


        /// <summary>
        /// P层对象属性[UnitRatio]变化
        /// 更新界面层
        /// </summary>
        /// 
        /// <remark>
        /// added by lihuijun
        /// </remark>
        private void PrsntChangedWithUnitRatio()
        {
            {// x/V 格式

                string unit = " " + Presenter.Unit + "/V";
                double value = Presenter.ProbeUnitRatio;
                if (value < 1)
                {
                    value *= 1000;
                    unit = " m" + Presenter.Unit + "/V";
                }
                if (value >= 1000)
                {
                    value /= 1000;
                    unit = " k" + Presenter.Unit + "/V";
                }
                BtnUnitRatio.Text = value.ToString("G3") + unit;
            }

            {// V/x 格式
                string unitInverted = " V/" + Presenter.Unit;
                double valueInverted = 1 / Presenter.ProbeUnitRatio;
                if (valueInverted < 1)
                {
                    valueInverted *= 1000;
                    unitInverted = " mV/" + Presenter.Unit;
                }
                if (valueInverted >= 1000)
                {
                    valueInverted /= 1000;
                    unitInverted = " kV/" + Presenter.Unit;
                }


                BtnUnitRatioInverted.Text = valueInverted.ToString("G3") + unitInverted;
            }
        }

        /// <summary>
        /// P层对象属性[Unit]变化
        /// 更新界面层
        /// </summary>
        /// <remark>
        /// added by lihuijun
        /// </remark>
        private void PrsntChangedWithUnit()
        {
            ChkProbeUnitCustom.Checked = Presenter.ProbeUnitIsCustomized;
            TbxUnit.Text = Presenter.Unit;
            TbxUnit.Visible = Presenter.ProbeUnitIsCustomized;
            LblUnitCvtText.Visible = Presenter.ProbeUnitIsCustomized;
            BtnUnitRatio.Visible = Presenter.ProbeUnitIsCustomized;
            BtnUnitRatioInverted.Visible = Presenter.ProbeUnitIsCustomized;
            if (Presenter.ProbeUnitIsCustomized)
            {//单位不为V时，需要显示单位比率
                PrsntChangedWithUnitRatio();
            }
            else
            {//单位等于V时，需要重置单位比率
                Presenter.ProbeUnitRatio = 1;
            }
        }
        /// <summary>
        /// 探头单位定制开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkProbeUnitCustom_Click(object sender, EventArgs e)
        {
            if (ChkProbeUnitCustom.Checked)
            {//如果开启备用单位
                Presenter.Unit = "A";
                Presenter.ProbeUnitRatio = 1;
            }
            else
            {
                Presenter.Unit = "V";
                Presenter.ProbeUnitRatio = 1;
            }

            Presenter.ProbeUnitIsCustomized = ChkProbeUnitCustom.Checked;
        }

        /// <summary>
        /// 用户定制单位比率值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUnitRatio_Click(object sender, EventArgs e)
        {
            string unit = Presenter.Unit + "/V";
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnUnitRatio);
            var oncomfirm = new Action<Double>((data) =>
            {
                Presenter.ProbeUnitRatio = (Double)data;
            });

            nkf.SetKeyBoardValue(LblUnitCvtText.Text, unit, 3, oncomfirm, Presenter.ProbeUnitRatio, Presenter.MaxProbeUnitRatio, Presenter.MinProbeUnitRatio);
            nkf.ShowDialogByPosition();
        }

        /// <summary>
        /// 用户定制单位比率值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUnitRatioInverted_Click(object sender, EventArgs e)
        {
            string unit = "V/" + Presenter.Unit;
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnUnitRatioInverted);
            var oncomfirm = new Action<Double>((data) =>
            {
                Presenter.ProbeUnitRatio = (Double)(1 / data);
            });
            nkf.SetKeyBoardValue(LblUnitCvtText.Text, unit, 3, oncomfirm, 1 / Presenter.ProbeUnitRatio, Presenter.MaxProbeUnitRatio, Presenter.MinProbeUnitRatio);
            nkf.ShowDialogByPosition();
        }






        private void CbxProbeUnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl && Presenter != null)
            {
                //Presenter.ProbeUnit = (ProbeUnitType)CbxProbeUnitType.SelectValue;
            }
        }

        private void CbxProbeMag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl && Presenter != null)
            {
                Presenter.ProbeIndex = (AnaChnlProbe)CbxProbeMag.SelectIndex;
            }
        }

        /// <summary>
        /// 探头用户校准
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProbeCali_Click(object sender, EventArgs e)
        {

            (Program.Oscilloscope.View as DsoForm).Invoke(() => (Program.Oscilloscope.View as DsoForm).ProbeCalibrationWithUser(Presenter.Id));
            ParentForm?.Close();

        }

        /// <summary>
        /// 探头出厂校准功能暂时依附于示波器，待校准工具完成后移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProbeCaliFact_Click(object sender, EventArgs e)
        {
            var res = StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.AutoCalibration, MessageType.Warning);
            if (!res)
            {
                return;
            }

            var id = Presenter.Id;
            var vw = (Program.Oscilloscope.View as DsoForm);
            vw.Activate();//通道浮动被关闭
            vw.ProbeCalibrationWithFact(id);//弹出探头校准窗口
        }

        private void BtnDCBiasSetting_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnDCBiasSetting);

            Action<Double> onokclickeventaction = new Action<Double>((data) =>
                Presenter.ProbeDCBiasBymV = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

            nkf.SetKeyBoardValue(LblDCBiasSetting.Text, Presenter.Unit, 2, onokclickeventaction,
                Quantity.ConvertByPrefix(Presenter.ProbeDCBiasBymV, Prefix.Milli),
                Quantity.ConvertByPrefix(Presenter.MaxProbeDCBiasBymV, Prefix.Milli),
                Quantity.ConvertByPrefix(Presenter.MinProbeDCBiasBymV, Prefix.Milli));
            nkf.ShowDialogByPosition();
        }

        private String ProbeDCBiasToString()
        {
            return new Quantity(Presenter.ProbeDCBiasBymV, Prefix.Milli, Presenter.Unit).ToString("#0.###", true);
        }

        private void BtnProbeBtnProbeDefault_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ProbeUserCalibDefault();
            }
        }
    }
}
