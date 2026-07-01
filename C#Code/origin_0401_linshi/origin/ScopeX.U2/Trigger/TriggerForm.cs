// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.U2.LanguageSupoort;
    using ScopeX.U2.Search;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class TriggerForm : FloatForm, ITriggerView, ILocationAssistedView, IVisualTriggerView, ITriggerAssistedView
    {
        #region 属性及字段定义
        private Boolean _ArgToCtrl;

        private Control _OptionSubPage;

        private Lazy<Control> _EdgePage;
        private Lazy<Control> _PulseWidthPage;
        private Lazy<Control> _VideoPage;
        private Lazy<Control> _TransitionPage;
        private Lazy<Control> _RuntPage;
        private Lazy<Control> _DelayPage;
        private Lazy<Control> _TimeOutPage;
        private Lazy<Control> _SustainTimePage;
        private Lazy<Control> _SetupHoldPage;
        private Lazy<Control> _NEdgePage;
        private Lazy<Control> _PatternPage;
        private Lazy<Control> _Serial;
        #endregion
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;
        public TriggerForm()
        {
            InitializeComponent();
            InitLang_Control();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(TriggerForm)));
            };
        }

        private void InitLang_Control()
        {
            BtnForce.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiangZhi");
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DanCi");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengChang");
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDong");
            RdoMode.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2, radioButtonItem3 });
            LblMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoShi");
            CbxTriggerType.Items = new string[] {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Edge"),// "边沿",
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_PulseWidth"),// "脉宽",
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Video"),// "视频", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Transition"),// "斜率", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Runt"),// "欠幅",
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Delay"),// "延迟", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_TimeOut"),// "超时", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_SustainTime"),// "持续时间", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_SetupHold"),// "建立保持", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_NEdge"),// "N边沿", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Pattern"),// "码型", 
                //ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Serial"),// "串行"
            };
            LblTriggerType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            /*LblTriggerAssist.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuZhuChuFa");
            BtnViusalTrigger.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PeiZhi");
            ChkVisualTrigger.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KeShiChuFa");
            ChkTriggerAssisted.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkTriggerAssisted.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            BtnLocationAssisted.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PeiZhi");
            ChkLocationAssisted.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiangChuFa");*/
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem5 = new UserControls.RadioButtonItem();
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJian");
            radioButtonItem5.Icon = null;
            radioButtonItem5.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem5.Tag = null;
            radioButtonItem5.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJian");
            RdoHoldoffType.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem4, radioButtonItem5 });
            LblHoldoff.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiYiShiJian");
            BtnResetHoldff.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuFaSheZhi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuFaSheZhi");
        }


        public TriggerAssistedPrsnt TriggerAssistedPresenter { get; set; }
        public LocationAssistedPrsnt LocationAssistedPresenter
        {
            get;
            set;
        }

        public VisualTriggerPrsnt VisualTriggerPresenter
        {
            get;
            set;
        }

        public TriggerPrsnt Presenter
        {
            get;
            set;
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

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TriggerPrsnt)value;
        }
        ITriggerAssistedPrsnt IView<ITriggerAssistedPrsnt>.Presenter
        {
            get => TriggerAssistedPresenter;
            set => TriggerAssistedPresenter = (TriggerAssistedPrsnt)value;
        }
        IVisualTriggerPrsnt IView<IVisualTriggerPrsnt>.Presenter
        {
            get => VisualTriggerPresenter;
            set => VisualTriggerPresenter = (VisualTriggerPrsnt)value;
        }
        ILocationAssistedPrsnt IView<ILocationAssistedPrsnt>.Presenter
        {
            get => LocationAssistedPresenter;
            set => LocationAssistedPresenter = (LocationAssistedPrsnt)value;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TriggerPrsnt.TryRemoveTriggerView(this);
            Presenter.TryRemoveView(this);
            TriggerAssistedPresenter.TryRemoveView(this);
            LocationAssistedPresenter.TryRemoveView(this);
            VisualTriggerPresenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TlpTrigger.RowStyles[0] = new RowStyle(SizeType.Absolute, 70);
            InitOptionSubPage();
            Stylize();
            UpdateView();
            BtnForce.Visible = false;   
            // LanguageFactory.CacheFormLanguageControls(this);
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
                case nameof(TriggerPrsnt.Mode):
                    RdoMode.ChoosedButtonIndex = (Int32)TriggerPrsnt.Mode;
                    break;
                case nameof(TriggerPrsnt.HoldoffType):
                case nameof(TriggerPrsnt.HoldoffByps):
                case nameof(TriggerPrsnt.HoldoffByCnt):
                    RdoHoldoffType.ChoosedButtonIndex = (Int32)TriggerPrsnt.HoldoffType;
                    NebHoldoff.UpdateValueString();
                    break;
                case "AdjHoldoffTime":
                    NebHoldoff.UpdateValueString();
                    break;
                case nameof(TriggerPrsnt.Type):
                    //CbxTriggerType.SelectedIndex = TriggerPrsnt.Type == TriggerType.Serial ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type; ;
                    //CbxTriggerType.SelectedIndex = (Int32)TriggerPrsnt.Type > (Int32)TriggerType.BeyondVol ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type;
                    CbxTriggerType.SelectIndex = (Int32)TriggerPrsnt.Type > (Int32)TriggerType.BeyondVol ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type;
                    ChangeOptionPage(TriggerPrsnt.Type);
                    break;
                default:
                   if(_OptionSubPage?.Parent?.Parent!=null)
                    {
                        (_OptionSubPage as ITriggerView)?.UpdateView(prsnt, propertyName);
                    }
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoMode.ChoosedButtonIndex = (Int32)TriggerPrsnt.Mode;
                //CbxTriggerType.SelectedIndex = TriggerPrsnt.Type == TriggerType.Serial ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type;
                //CbxTriggerType.SelectedIndex = (Int32)TriggerPrsnt.Type > (Int32)TriggerType.BeyondVol ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type;
                CbxTriggerType.SelectIndex = (Int32)TriggerPrsnt.Type > (Int32)TriggerType.BeyondVol ? ((Int32)TriggerPrsnt.Type - 1) : (Int32)TriggerPrsnt.Type;
                ChangeOptionPage(TriggerPrsnt.Type);
                RdoHoldoffType.ChoosedButtonIndex = (Int32)TriggerPrsnt.HoldoffType;
                NebHoldoff.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void ChangeOptionPage(TriggerType tp)
        {
            var optionPage = GetOptionPage(tp); 
            if (optionPage == null) return;
            if (optionPage != _OptionSubPage)
            {
                TlpTrigger.Controls.Remove(_OptionSubPage);
                //_OptionSubPage.Dispose();
                _OptionSubPage = optionPage;
                //_OptionSubPage.BackColor = Color.Transparent;
                if (_OptionSubPage is IStylize stylepage)
                {
                    stylepage.StylizeFlag = true;
                    DefaultStyleManager.Instance.RegisterControlRecursion(_OptionSubPage, StyleFlag.FontSize);
                }
                
                TlpTrigger.Controls.Add(_OptionSubPage, 0, 1);
            }
        }

        private void InitSubPage()
        {
            _EdgePage = new Lazy<Control>(()=> new TriggerEdgeSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _PulseWidthPage = new Lazy<Control>(() => new TriggerWidthSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _VideoPage = new Lazy<Control>(() => new TriggerVideoSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _TransitionPage = new Lazy<Control>(() => new TriggerTransSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _RuntPage = new Lazy<Control>(() => new TriggerRuntSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _DelayPage = new Lazy<Control>(() => new TriggerDelaySubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _TimeOutPage = new Lazy<Control>(() => new TriggerTimeOutSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _SustainTimePage = new Lazy<Control>(() => new TriggerSustainTimeSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _SetupHoldPage = new Lazy<Control>(() => new TriggerSetupHoldSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _NEdgePage = new Lazy<Control>(() => new TriggerNEdgeSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _PatternPage = new Lazy<Control>(() => new TriggerPatSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
            _Serial = new Lazy<Control>(() => new TriggerSerialSubPage()
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent
            });
        }
        private Control GetOptionPage(TriggerType tt)
        {
            switch (tt)
            {
                case TriggerType.Edge:
                    return _EdgePage.Value;
                case TriggerType.PulseWidth: 
                    return _PulseWidthPage.Value;
                case TriggerType.Video:
                    return _VideoPage.Value;
                case TriggerType.Transition:
                    return _TransitionPage.Value;
                case TriggerType.Runt:
                    return _RuntPage.Value;
                case TriggerType.Delay:
                    return _DelayPage.Value;
                case TriggerType.TimeOut:
                    return _TimeOutPage.Value;
                case TriggerType.SustainTime:
                    return _SustainTimePage.Value;
                case TriggerType.SetupHold:
                    return _SetupHoldPage.Value;
                case TriggerType.NEdge:
                    return _NEdgePage.Value;
                case TriggerType.Pattern:
                    return _PatternPage.Value;
                case TriggerType.Serial:
                    return _Serial.Value;
                default: return _EdgePage.Value;

            }
        }

        private void InitNebHoldoff()
        {
            ControlsHotKnob.Default.InitHotKnob(NebHoldoff);
            NebHoldoff.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHoldoff);
            };
            NebHoldoff.AddClicked = (_, e) =>
            {
                if (TriggerPrsnt.HoldoffType == DelayOpt.Time)
                {
                    TriggerPrsnt.AdjHoldoff(e.Step);
                }
                else
                {
                    TriggerPrsnt.HoldoffByCnt += e.Step;
                }
            };
            NebHoldoff.SubClicked = (_, e) =>
            {
                if (TriggerPrsnt.HoldoffType == DelayOpt.Time)
                {
                    TriggerPrsnt.AdjHoldoff(e.Step);
                }
                else
                {
                    TriggerPrsnt.HoldoffByCnt += e.Step;
                }
            };
            NebHoldoff.StringFormatFunc = new Func<double, string>((d) => HoldoffToString());
            NebHoldoff.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHoldoff);


                if (TriggerPrsnt.HoldoffType == DelayOpt.Time)
                {
                    nkf.NumberKeyboard.UseSI = true;
                    var onokclickeventaction = new Action<Double>((data) =>
                        TriggerPrsnt.HoldoffByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                    nkf.SetKeyBoardValue(LblHoldoff.Text, "s", 3, onokclickeventaction,
                        Quantity.ConvertByPrefix(TriggerPrsnt.HoldoffByps, Prefix.Pico),
                        Quantity.ConvertByPrefix(TriggerPrsnt.MaxHoldoffTime, Prefix.Pico),
                        Quantity.ConvertByPrefix(TriggerPrsnt.MinHoldoffTime, Prefix.Pico));
                }
                else
                {
                    nkf.NumberKeyboard.UseSI = false;
                    var onokclickeventaction = new Action<Double>((data) =>
                        TriggerPrsnt.HoldoffByCnt = Convert.ToInt32(data));

                    nkf.SetKeyBoardValue(LblHoldoff.Text, "#", 0, onokclickeventaction,
                        TriggerPrsnt.HoldoffByCnt,
                        TriggerPrsnt.MaxHoldoffCnt,
                        TriggerPrsnt.MinHoldoffCnt, false);
                }
                nkf.ShowDialogByPosition();
            };
        }

        private void InitOptionSubPage()
        {
            InitNebHoldoff();
            InitSubPage();
            _OptionSubPage = GetOptionPage(TriggerPrsnt.Type);

            TlpTrigger.Controls.Add(_OptionSubPage, 0, 1);
        }

        private void BtnTriggerAssist_Click(object sender, EventArgs e)
        {
            var laf = new LocAssistedSettingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                Presenter = LocationAssistedPresenter,
            };
            laf.Presenter.TryAddView(laf);
            laf.ShowDialogByPosition();
        }

        private void BtnViusalTrigger_Click(object sender, EventArgs e)
        {
            var vtf = new VisualTriggerSettingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                Presenter = VisualTriggerPresenter,
            };
            vtf.Presenter.TryAddView(vtf);
            vtf.ShowDialogByPosition();
        }

        //private void CbxTriggerType_SelectedIndexChanged(object sender, EventArgs e)
        //{
          //  if (!_ArgToCtrl)
           // {
            //    var tt = (TriggerType)CbxTriggerType.SelectedIndex;
             //   if (CbxTriggerType.SelectedIndex >= 5)
             //   {
             //       tt = (TriggerType)(CbxTriggerType.SelectedIndex + 1);
             //   }
              //  tt = tt == TriggerType.MultiQulified ? TriggerType.Serial : tt;
              //  if (TriggerPrsnt.Type == tt)
              //  {
             //       return;
              //  }
               // TriggerPrsnt.GetOrMakeTrigger(Presenter.Dso, tt);
                //TlpTrigger.Controls.Remove(_OptionSubPage);
                //_OptionSubPage.Dispose();

                //_OptionSubPage = GetOptionPage(tt);

                #region 注释的代码
                //var p = TriggerPrsnt.GetTrigger(tt, Program.Oscilloscope.View.TriggerVu);
                //var itv = FindForm() as ITriggerView;
                //itv.Presenter = p;
                //p.TryAddView(itv);

                //Presenter = TriggerPrsnt.GetTrigger(tt, /*Program.Oscilloscope.View.TriggerVu*/);
                //Presenter.TryAddView(ParentForm as ITriggerView);

                //var tv = Presenter.GetViewList().ToList();
                //var tp = TriggerPrsnt.GetOrMakeTrigger(tt);
                //tp.AddViewList(tv);
                //foreach (var itv in tv)
                //    itv.Presenter = tp;

                //TriggerPrsnt.GetOrMakeTrigger(tt, Presenter.GetViewList().ToList());
                //TriggerPrsnt.GetOrMakeTrigger(Presenter.Dso, tt);
                #endregion

                //_OptionSubPage.BackColor = Color.Transparent;
                //if (_OptionSubPage is IStylize stylepage)
                //{
                //    stylepage.StylizeFlag = true;
                //    DefaultStyleManager.Instance.RegisterControlRecursion(_OptionSubPage, StyleFlag.FontSize);
                //}
                //TlpTrigger.Controls.Add(_OptionSubPage, 0, 1);
            //}
        //}

        private void BtnResetHoldff_Click(object sender, EventArgs e)
        {
            if (TriggerPrsnt.HoldoffType == DelayOpt.Time)
            {
                TriggerPrsnt.HoldoffByps = TriggerPrsnt.MinHoldoffTime;
            }
            else
            {
                TriggerPrsnt.HoldoffByCnt = TriggerPrsnt.MinHoldoffCnt;
            }
        }

        private void RdoMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TriggerPrsnt.Mode = (TriggerMode)RdoMode.ChoosedButtonIndex;
            }
        }

        private void RdoHoldoffType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TriggerPrsnt.HoldoffType = (DelayOpt)RdoHoldoffType.ChoosedButtonIndex;
            }
        }

        private static String HoldoffToString()
        {
            if (TriggerPrsnt.HoldoffType == DelayOpt.Time)
            {
                return new Quantity(TriggerPrsnt.HoldoffByps, Prefix.Pico, QuantityUnit.Second).ToString("##0.000000000", true, 5);/*ToString("##0.#########", true, 13);*/
            }
            return TriggerPrsnt.HoldoffByCnt.ToString() + " #";
        }
        
        private void Stylize()
        {
            if (_OptionSubPage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
            }
            IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }

        private void BtnForce_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                TriggerPrsnt.Force();
            }
        }

        private void CbxTriggerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                var tt = (TriggerType)CbxTriggerType.SelectIndex;
                if ((Int32)CbxTriggerType.SelectIndex >= 5)
                {
                    tt = (TriggerType)((Int32)CbxTriggerType.SelectIndex + 1);
                }
                tt = tt == TriggerType.MultiQulified ? TriggerType.Serial : tt;
                if (TriggerPrsnt.Type == tt)
                {
                    return;
                }
                Presenter = TriggerPrsnt.GetOrMakeTrigger(Presenter.Dso, tt);
            }
        }
    }
}
