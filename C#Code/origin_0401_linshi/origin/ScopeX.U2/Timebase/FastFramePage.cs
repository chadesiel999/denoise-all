using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class FastFramePage : UserControl, ITimebaseView, IStylize
    {
        private Boolean _ArgToCtrl;

        //private Control _OptionSubPage;

        public FastFramePage()
        {
            InitializeComponent();
            InitControlsText();
            Init();
            Size = new(PnlMain.Width, PnlMain.Height + PnlSingle.Height);
            InitControlEnable();
        }

        private void InitControlsText()
        {
            ChkFastFrame.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkFastFrame.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            FastFrameLabel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunXuMoShi");
            LabFrameStorage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengChangDu");
            LblFrameCount.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongZhengShu");
            LblViewMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiMoShi");
            LblBlankTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengChuFaJianGeShiJian");
            BtnAcqReset.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhong__Cai");
            BtnCallBack.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Hui__Fang");
            LblRefActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiCanKaoZheng");
            ChkRefActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkRefActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            LblRefFrame.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanKaoZheng");
            LblSelectedFrame.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanDingZheng");
            BtnResetAcq.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhong__Cai");
            LblFramesSelected.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengXuanZe");
            ScopeXLabel1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiLeiXing");
            LblEndFrame.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieShuZheng");
            LblStartFrame.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiShiZheng");
            LblCallBackInterval.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CallBackInterval");

        }

        private void InitControlEnable()
        {
            var enable = Constants.ENABLE_Segement;
            NebEndFrame.Enabled = enable;
            //NebRefFrame.Enabled = enable;
            //NebSelectedFrame.Enabled = enable;
            BtnEnable(!Presenter.CallBack);
            NebStartFrame.Enabled = enable;
            ChkRefActive.Enabled = enable;
            BtnResetAcq.Enabled = enable;
            BtnAcqReset.Enabled = enable;
            BtnFrameCount.Enabled = enable;
            BtnCallBack.Enabled = enable;
            BtnFramesSelected.Enabled = enable;

        }

        private void Init()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnFrameCount);

            ControlsHotKnob.Default.InitHotKnob(NebBlankTime);
            NebBlankTime.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebBlankTime);
            };
            NebBlankTime.AddClicked = (o, e) => Presenter.BlankTime = (UInt32)(Presenter.BlankTime + e.Step);
            NebBlankTime.SubClicked = (o, e) => Presenter.BlankTime = (UInt32)(Presenter.BlankTime + e.Step);
            NebBlankTime.StringFormatFunc = (value) => BlankTimeString();
            NebBlankTime.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBlankTime);
                Action<Double> onokclickeventaction = new((data) =>
                    Presenter.BlankTime = (UInt32)data);

                nkf.SetKeyBoardValue(LblFrameCount.Text, "", 12, onokclickeventaction,
                    Presenter.BlankTime, UInt32.MaxValue, UInt32.MinValue);

                nkf.ShowDialogByPosition();
            };

            CbxFrameStorage.DataSource = Enum.GetValues<AnaChnlLengthOpt>().Select(o => new KeyValuePair<AnaChnlLengthOpt, String>(o, o.GetAlias())).ToList();
            CbxFrameStorage.DisplayMember = "Value";
            CbxFrameStorage.ValueMember = "Key";

            CbxFrameStorage.SelectedIndexChanged += (_, _) => Presenter.LengthOpt = (AnaChnlLengthOpt)CbxFrameStorage.SelectedIndex;


            ControlsHotKnob.Default.InitHotKnob(NebSelectedFrame);
            NebSelectedFrame.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebSelectedFrame);
            };
            NebSelectedFrame.AddClicked = (o, e) => Presenter.CurFrameId += e.Step;
            NebSelectedFrame.SubClicked = (o, e) => Presenter.CurFrameId += e.Step;
            NebSelectedFrame.StringFormatFunc = (value) => GetFrameCountOrFrameID(Presenter.CurFrameId);// / {Presenter.FrameCount}#";
            NebSelectedFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSelectedFrame);
                nkf.NumberKeyboard.UseSI = false;
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.CurFrameId = (Int32)data);

                nkf.SetKeyBoardValue(LblSelectedFrame.Text, QuantityUnit.Count.ToUnitString(), 3, onokclickeventaction,
                    Presenter.CurFrameId, Presenter.FrameCount, 1);

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebRefFrame);
            NebRefFrame.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebRefFrame);
            };
            NebRefFrame.AddClicked = (o, e) => Presenter.ReferFrameIds += e.Step;
            NebRefFrame.SubClicked = (o, e) => Presenter.ReferFrameIds += e.Step;
            NebRefFrame.StringFormatFunc = (value) => GetFrameCountOrFrameID(Presenter.ReferFrameIds);// / {Presenter.FrameCount}#";
            NebRefFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebRefFrame);
                nkf.NumberKeyboard.UseSI = false;
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.ReferFrameIds = (Int32)data);

                nkf.SetKeyBoardValue(LblRefFrame.Text, QuantityUnit.Count.ToUnitString(), 3, onokclickeventaction,
                    Presenter.ReferFrameIds, Presenter.FrameCount, 1);

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebStartFrame);
            NebStartFrame.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebStartFrame);
            };
            NebStartFrame.AddClicked = (o, e) => Presenter.SequentStartFrame += e.Step;
            NebStartFrame.SubClicked = (o, e) => Presenter.SequentStartFrame += e.Step;
            NebStartFrame.StringFormatFunc = (value) => GetFrameCountOrFrameID(Presenter.SequentStartFrame);// / {Presenter.FrameCount}#";
            NebStartFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebStartFrame);
                nkf.NumberKeyboard.UseSI = false;
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SequentStartFrame = (Int32)data);

                nkf.SetKeyBoardValue(LblStartFrame.Text, QuantityUnit.Count.ToUnitString(), 3, onokclickeventaction,
                    Presenter.SequentStartFrame, Presenter.FrameCount - 1, 1);

                nkf.ShowDialogByPosition();
            };


            ControlsHotKnob.Default.InitHotKnob(NebEndFrame);
            NebEndFrame.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebEndFrame);
            };
            NebEndFrame.AddClicked = (o, e) => Presenter.SequentEndFrame += e.Step;
            NebEndFrame.SubClicked = (o, e) => Presenter.SequentEndFrame += e.Step;
            NebEndFrame.StringFormatFunc = (value) => GetFrameCountOrFrameID(Presenter.SequentEndFrame);// / {Presenter.FrameCount}";
            NebEndFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebEndFrame);
                nkf.NumberKeyboard.UseSI = false;
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SequentEndFrame = (Int32)data);

                nkf.SetKeyBoardValue(LblEndFrame.Text, QuantityUnit.Count.ToUnitString(), 3, onokclickeventaction,
                    Presenter.SequentEndFrame, Presenter.FrameCount, 2);

                nkf.ShowDialogByPosition();
            };
			
			ControlsHotKnob.Default.InitHotKnob(NebCallBcakInterval);
			NebCallBcakInterval.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebCallBcakInterval);
            };
            NebCallBcakInterval.AddClicked = (s, e) => Presenter.CallBackIntervalByms++;
            NebCallBcakInterval.SubClicked = (s, e) => Presenter.CallBackIntervalByms--;
            NebCallBcakInterval.StringFormatFunc = (d) => GetCallBackInterval();
            NebCallBcakInterval.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebCallBcakInterval);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.MaxCallBackIntervalByms, Prefix.Milli);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.MinCallBackIntervalByms, Prefix.Milli);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.CallBackIntervalByms, Prefix.Milli);
                nkf.NumberKeyboard.DecimalNumber = 0;
                nkf.NumberKeyboard.Unit = QuantityUnit.Second.ToUnitString();
                nkf.Title = LblCallBackInterval.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Double interval = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Prefix.Milli);
                    Presenter.CallBackIntervalByms = (Int32)interval; //Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Prefix);
                    nkf.Close();
                };

                DialogResult dialogresult = nkf.ShowDialogByPosition();
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

        public TimebasePrsnt Presenter
        {
            get
            {
                return Program.Oscilloscope.Timebase;
            }
            set => (ParentForm as ITimebaseView).Presenter = value;
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TimebasePrsnt)value;
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

            if (!DesignMode)
            {
                _ArgToCtrl = true;
                switch (propertyName)
                {
                    case nameof(Presenter.SegmentActive):
                        ChkFastFrame.Checked = Presenter.SegmentActive;
                        break;
                    case nameof(Presenter.LengthOpt):
                        CbxFrameStorage.SelectedIndex = (Int32)Presenter.LengthOpt;
                        break;
                    case nameof(Presenter.FrameCount):
                        BtnFrameCount.Text = GetFrameCountOrFrameID(Presenter.FrameCount);
                        break;
                    case nameof(Presenter.WorkMode):
                        RdoViewMode.ChoosedButtonIndex = (Int32)Presenter.WorkMode;
                        ChangeCtrlState(Presenter.WorkMode);
                        break;
                    case nameof(Presenter.BlankTime):
                        NebBlankTime.UpdateValueString();
                        break;
                    case nameof(Presenter.CurFrameId):
                        NebSelectedFrame.UpdateValueString();
                        break;
                    case nameof(Presenter.ReferFrameIds):
                        NebRefFrame.UpdateValueString();
                        break;
                    case nameof(Presenter.RefActive):
                        ChkRefActive.Checked = Presenter.RefActive;
                        break;
                    case nameof(Presenter.CurFrameSecond):
                        LblSelectedSeconds.Text = CurFrameSecondString();
                        break;

                    case nameof(Presenter.SequentStartFrame):
                        NebStartFrame.UpdateValueString();
                        break;
                    case nameof(Presenter.SequentEndFrame):
                        NebEndFrame.UpdateValueString();
                        break;
                    case nameof(Presenter.RenderType):
                        //CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
                        CbxViewMode.SelectValue = (Int32)Presenter.RenderType;
                        break;
                    case nameof(Presenter.CallBack):
                        BtnEnable(!Presenter.CallBack);
                        //if (Presenter.CallBack)
                        //{
                        //    SignModeRePlay();
                        //}
                        break;
                    case nameof(Presenter.CallBackIntervalByms):
                        NebCallBcakInterval.UpdateValueString();
                        break;
                    default:
                        //(_OptionSubPage as ITimebaseView)?.UpdateView(prsnt, propertyName);
                        break;
                }
                _ArgToCtrl = false;
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkFastFrame.Checked = Presenter.SegmentActive;
                CbxFrameStorage.SelectedIndex = (Int32)Presenter.LengthOpt;
                RdoViewMode.ChoosedButtonIndex = (Int32)Presenter.WorkMode;
                BtnFrameCount.Text = GetFrameCountOrFrameID(Presenter.FrameCount);
                NebBlankTime.UpdateValueString();
                ChkRefActive.Checked = Presenter.RefActive;
                LblSelectedSeconds.Text = CurFrameSecondString();
                NebSelectedFrame.UpdateValueString();
                NebRefFrame.UpdateValueString();
                //CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
                CbxViewMode.SelectValue = (Int32)Presenter.RenderType;
                NebStartFrame.UpdateValueString();
                NebEndFrame.UpdateValueString();
                NebCallBcakInterval.UpdateValueString();
                ChangeCtrlState(Presenter.WorkMode);
                _ArgToCtrl = false;
            }
        }

        private String BlankTimeString()
        {
            return new Quantity(Presenter.BlankTime, Prefix.Milli, QuantityUnit.Second).ToString("##0.000000", true);
        }

        private String CurFrameSecondString()
        {
            return new Quantity(Presenter.CurFrameSecond, Prefix.Nano, QuantityUnit.Second).ToString("##0.000000", true);
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void ChangeCtrlState(SegmentWorkMode workMode)
        {
            //if (_OptionSubPage is not null && (SegmentWorkMode)_OptionSubPage.Tag != workMode)
            //{
            //    TlpMain.Controls.Remove(_OptionSubPage);
            //    _OptionSubPage.Dispose();
            //}

            //_OptionSubPage = GetOptionSubPage(workMode);
            //TlpMain.Controls.Add(_OptionSubPage, 0, 1);

            PnlSingle.Visible = workMode == SegmentWorkMode.Single;
            PnlSequence.Visible = !PnlSingle.Visible;
            if (PnlSequence.Visible)
            {
                var enable = Constants.ENABLE_Segement;
                Presenter.RenderType = (PlotRenderType)CbxViewMode.SelectValue;
                LblFramesSelected.Enabled = BtnFramesSelected.Enabled = workMode == SegmentWorkMode.Select && enable;
                LblStartFrame.Enabled = NebStartFrame.Enabled = LblEndFrame.Enabled = NebEndFrame.Enabled = workMode == SegmentWorkMode.Sequent && enable;
            }
        }

        //private static Control GetOptionSubPage(SegmentWorkMode workMode)
        //{
        //    Control subpage = workMode switch
        //    {
        //        SegmentWorkMode.Single => new SegmentSinglePage(),
        //        SegmentWorkMode.Sequent => new SegmentSequentPage(),
        //        SegmentWorkMode.Select => new SegmentSelectPage(),
        //        _ => throw new NotImplementedException(),
        //    };
        //    subpage.Dock = DockStyle.Fill;
        //    subpage.TabIndex = 1;
        //    subpage.BackColor = Color.Transparent;
        //    if (subpage is IStylize stylepage)
        //    {
        //        stylepage.StylizeFlag = true;
        //        DefaultStyleManager.Instance.RegisterControlRecursion(subpage);
        //    }
        //    return subpage;
        //}

        private void ChkFastFrame_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SegmentActive = ChkFastFrame.Checked;

                ChkFastFrame.Checked = Presenter.SegmentActive;//重新刷新一次
                SegmentApp.Default.InfoStrip.Visible = Presenter.SegmentActive;
            }
        }

        private void RdoViewMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.WorkMode = (SegmentWorkMode)RdoViewMode.ChoosedButtonIndex;
            }
        }

        private void ChkRefActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RefActive = ChkRefActive.Checked;
            }
        }

        //private void CbxViewMode_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.RenderType = (PlotRenderType)CbxViewMode.SelectedIndex;
        //    }
        //}

        private void BtnFramesSelected_Click(object sender, EventArgs e)
        {
            SegmentChooseForm form = new(Presenter);
            form.ShowDialog();
        }

        private void BtnCallBack_Click(object sender, EventArgs e)
        {
            Presenter.CallBack = !Presenter.CallBack;
           
            BtnEnable(!Presenter.CallBack);
            //if (Presenter.CallBack)
            //{
            //    SignModeRePlay();
            //}
        }

        private void BtnAcqReset_Click(object sender, EventArgs e)
        {
            Presenter.CallBack = false;
            Presenter.ResetAcq();
        }

        //private async void SignModeRePlay()
        //{
        //    await Task.Factory.StartNew(() =>
        //    {
        //        BtnEnable(false);
        //        for (int i = SegmentApp.Default.Presenter.CurFrameId; i <= SegmentApp.Default.Presenter.FrameCount; i++)
        //        {
        //            SegmentApp.Default.Presenter.CurFrameId = i;
        //            if (!SegmentApp.Default.Presenter.CallBack || !SegmentApp.Default.Presenter.SegmentActive)
        //            {
        //                break;
        //            }
        //            Thread.Sleep(5);
        //        }
        //        SegmentApp.Default.Presenter.CurFrameId = SegmentApp.Default.Presenter.CurFrameId == SegmentApp.Default.Presenter.FrameCount ? 1 : SegmentApp.Default.Presenter.CurFrameId;
        //        SegmentApp.Default.Presenter.CallBack = false;
        //        BtnEnable(true);
        //    });
        //    return;
        //}

        private void BtnEnable(Boolean enable)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => BtnEnable(enable));
                return;
            }
            BtnCallBack.Text = enable
                ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Hui__Fang")
                : ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Ting__Zhi");
            NebSelectedFrame.Enabled = enable;
            NebRefFrame.Enabled = enable;
        }
        private void BtnFrameCount_Click(object sender, EventArgs e)
        {
            ControlsHotKnob.Default.SetHotKnob(Presenter, BtnFrameCount, nameof(Presenter.FrameCount));
        }
        private void BtnFrameCount_DoubleClick(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnFrameCount);
            var oncomfirm = new Action<Double>((data) => Presenter.FrameCount = (Int32)data);

            nkf.SetKeyBoardValue(LblFrameCount.Text, QuantityUnit.Count.ToUnitString(), 3, oncomfirm,
                Presenter.FrameCount, Presenter.MaxFrameCount, Presenter.MinFrameCount);
            nkf.ShowDialogByPosition();
        }

        private string GetCallBackInterval()
        {
            return new Quantity(Presenter.CallBackIntervalByms, Prefix.Milli, QuantityUnit.Second.ToUnitString()).ToString("##0.###", true);
        }

        private String GetFrameCountOrFrameID(Int32 countOrId)
        {
            return $"{countOrId}{QuantityUnit.Count.ToUnitString()}";
            //return new Quantity(countOrId, Prefix.Empty, QuantityUnit.Count).ToString("#0.###", true);
        }

        private void CbxViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RenderType = (PlotRenderType)CbxViewMode.SelectValue;
            }
        }
    }
}
