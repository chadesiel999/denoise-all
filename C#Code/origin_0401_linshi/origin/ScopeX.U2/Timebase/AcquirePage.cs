using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Language;
using ScopeX.Controls.Common.Helper;
using System.Linq;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class AcquirePage : UserControl, ITimebaseView, IStylize
    {
        private Boolean _ArgToCtrl;

        public AcquirePage()
        {
            InitializeComponent();
            Init();
        }

        private void InitStorage()
        {
            CbxStorageDepth.SelectedIndexChanged -= CbxStorageDepth_SelectedIndexChanged;
            CbxStorageDepth.Items = Presenter.AnaChnlLengthSource.Select(x => x.Key.ToString()).ToArray();
            CbxStorageDepth.SelectValue = Presenter.StorageDepthOpt;
            CbxStorageDepth.SelectedIndexChanged += CbxStorageDepth_SelectedIndexChanged;
            //CbxStorageDepth.SelectedIndexChanged -= StorageSelectedIndexChanged;
            //CbxStorageDepth.DataSource = Presenter.AnaChnlLengthSource;
            //CbxStorageDepth.DisplayMember = "Key";
            //CbxStorageDepth.ValueMember = "Value";
            //CbxStorageDepth.SelectedIndexChanged += StorageSelectedIndexChanged;
            //CbxStorageDepth.SelectedIndex = Presenter.StorageDepthOpt;
        }



        //private void StorageSelectedIndexChanged(Object sender, EventArgs args)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.StorageDepthOpt =CbxStorageDepth.SelectedIndex;
        //    }
        //}
        private void Init()
        {

            CbxAcq.SelectedIndexChanged -= CbxAcq_SelectedIndexChanged;
            ControlsHotKnob.Default.InitHotKnob(NebAvgEnvpCnt);
            NebAvgEnvpCnt.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebAvgEnvpCnt);
            };
            NebAvgEnvpCnt.AddClicked = (_, _) =>
            {
                if (Presenter.Mode == AnaChnlAcqMode.Average)
                {
                    Presenter.AverageCnt++;
                }
                else
                {
                    Presenter.EnvelopeCnt++;
                }
            };
            NebAvgEnvpCnt.SubClicked = (_, _) =>
            {
                if (Presenter.Mode == AnaChnlAcqMode.Average)
                {
                    Presenter.AverageCnt--;
                }
                else
                {
                    Presenter.EnvelopeCnt--;
                }
            };
            NebAvgEnvpCnt.StringFormatFunc = (_) => Presenter.Mode == AnaChnlAcqMode.Average ? Presenter.AverageCnt.ToString() : Presenter.EnvelopeCnt.ToString();
            NebAvgEnvpCnt.EditValueChicked += (_, _) => InitNumKeyboard();

            void InitNumKeyboard()
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAvgEnvpCnt);

                nkf.NumberKeyboard.Unit = QuantityUnit.Count.ToUnitString();
                nkf.NumberKeyboard.UseSI = false;
                nkf.NumberKeyboard.DecimalNumber = 0;
                nkf.Title = LblAvgEnvpCnt.Text;
                if (Presenter.Mode == AnaChnlAcqMode.Average)
                {
                    nkf.NumberKeyboard.MaxValue = Presenter.AverageMaxCnt;
                    nkf.NumberKeyboard.MinValue = Presenter.AverageMinCnt;
                    nkf.NumberKeyboard.DefaultValue = Presenter.AverageCnt;
                    nkf.NumberKeyboard.OkClickEvent += (_, arg) =>
                    {
                        Presenter.AverageCnt = (Int32)arg.Data;
                        nkf.Close();
                    };
                }
                else
                {
                    nkf.NumberKeyboard.MaxValue = Presenter.EnvelopeMaxCnt;
                    nkf.NumberKeyboard.MinValue = Presenter.EnvelopeMinCnt;
                    nkf.NumberKeyboard.DefaultValue = Presenter.EnvelopeCnt;
                    nkf.NumberKeyboard.OkClickEvent += (_, arg) => { Presenter.EnvelopeCnt = (Int32)arg.Data; nkf.Close(); };
                }

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            }

            //ComboBoxItem
            Image GetTopInfoImg(String imgName)
            {
                Stream stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Namespace + ".Resources.TopInfoBar." + imgName);
                return new Bitmap(stream);
            }
            if (Program.Oscilloscope.Timebase.StorageMode != AnaChnlStorageMode.Fast)
            {
                CbxAcq.DataSource = new List<ComboBoxItem> {
                    new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Normal"),0,GetTopInfoImg("AcqModeNormal.png")),
                    new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Peak"),1,GetTopInfoImg("AcqModePeak.png")),
                    //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_HighRes"),2, GetTopInfoImg("AcqModeHighRes.png")),                
                    new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Average"),2, GetTopInfoImg("AcqModeAverage.png")){ Enabled=!(DsoPrsnt.DefaultDsoPrsnt.Jitter != null && DsoPrsnt.DefaultDsoPrsnt.Jitter.Active)},
                    //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Envelope"),4, GetTopInfoImg("AcqModeEnvelope.png")),
                };
            }
            else
            {
                CbxAcq.DataSource = new List<ComboBoxItem>()
                {
                    new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Normal"),0,GetTopInfoImg("AcqModeNormal.png")),
                    new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Peak"),1,GetTopInfoImg("AcqModePeak.png")),
                    //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_HighRes"),2, GetTopInfoImg("AcqModeHighRes.png")),
                };
            }
            CbxAcq.SelectedIndexChanged += CbxAcq_SelectedIndexChanged;


            ControlsHotKnob.Default.InitHotKnob(NebBits);
            NebBits.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebBits);
            };
            NebBits.AddClicked = (a, b) => Presenter.AdjEnhancedBits(1);
            NebBits.SubClicked = (a, b) => Presenter.AdjEnhancedBits(-1);
            NebBits.StringFormatFunc = (value) =>
            {
                return Presenter.EnhancedBits.ToString("0.0");
            };
            NebBits.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBits);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.EnhancedBits = data;

                nkf.SetKeyBoardValue(LBlBits.Text, String.Empty, 1, onokclickeventaction,
                    Presenter.EnhancedBits,
                    Constants.MaxEnhancedBits,
                    Constants.MinEnhancedBits);

                nkf.ShowDialogByPosition();
            };

        }

        private void CbxAcq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //if((Int32)selectTouch1.SelectValue>=selectTouch1.DataSource.Count)
                //    selectTouch1.SelectValue=0;

                Presenter.Mode = (AnaChnlAcqMode)CbxAcq.SelectValue;
                SelectMode(Presenter.Mode);
            }
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
            get => (TimebasePrsnt)(ParentForm as ITimebaseView).Presenter;
            set => (ParentForm as ITimebaseView).Presenter = value;
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TimebasePrsnt)value;
        }



        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Mode):
                    //CbxAcq.SelectedIndex = (Int32)Presenter.Mode;

                    if ((Int32)CbxAcq.SelectValue >= CbxAcq.DataSource.Count)
                        CbxAcq.SelectValue = 0;
                    else
                        CbxAcq.SelectValue = (Int32)Presenter.Mode;

                    SelectMode(Presenter.Mode);
                    break;
                case nameof(Presenter.InterplType):
                    RdoInterpolation.ChoosedButtonIndex = (Int32)Presenter.InterplType;
                    break;
                case nameof(Presenter.ClockSrc):
                case nameof(Presenter.Ext10MHzLocked):
                    RdoClkSource.ChoosedButtonIndex = (Int32)Presenter.ClockSrc;
                    if (Presenter.ClockSrc == AnaChnlClkSrc.Outter)
                    {
                        if (Presenter.Ext10MHzLocked)
                        {
                            LblLocked.Text = LanguageManger.Instance.GetIDMessage("MsgTipId.Ext10MHzLocked");
                            LblLocked.ForeColor = Color.DeepSkyBlue;
                        }
                        else
                        {
                            LblLocked.Text = LanguageManger.Instance.GetIDMessage("MsgTipId.Ext10MHzUnlocked");
                            LblLocked.ForeColor = Color.Red;
                        }
                        LblLocked.Visible = true;
                    }
                    else
                    {
                        LblLocked.Visible = false;
                    }
                    break;
                case nameof(Presenter.AverageCnt):
                case nameof(Presenter.EnvelopeCnt):
                    NebAvgEnvpCnt.UpdateValueString();
                    break;
                case nameof(Presenter.StorageDepthOpt):
                    //CbxStorageDepth.SelectedIndex = Presenter.StorageDepthOpt;
                    CbxStorageDepth.SelectValue = Presenter.StorageDepthOpt;
                    if (Presenter.StorageDepthOpt==0)
                    {
                        LblConstAutoStorage.Visible = false;
                        ChkConstAutoStorage.Visible = false;
                        //LblConstAutoStorage.Visible = true;
                        //ChkConstAutoStorage.Visible = true;
                    }
                    else
                    {
                        LblConstAutoStorage.Visible = false;
                        ChkConstAutoStorage.Visible = false;
                    }
                    break;
                case nameof(TimebasePrsnt.AnaChnlLengthSource):
                    InitStorage();
                    break;
                case nameof(Presenter.StorageMode):
                    //ComboBoxItem
                    Image GetTopInfoImg(String imgName)
                    {
                        Stream stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Namespace + ".Resources.TopInfoBar." + imgName);
                        return new Bitmap(stream);
                    }

                    Int32 selectindex = (Int32)CbxAcq.SelectValue;
                    if (Presenter.StorageMode != AnaChnlStorageMode.Fast)
                    {
                        CbxAcq.DataSource = new List<ComboBoxItem>()
                        {
                            new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Normal"),0,GetTopInfoImg("AcqModeNormal.png")),
                            new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Peak"),1,GetTopInfoImg("AcqModePeak.png")),
                            //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_HighRes"),2, GetTopInfoImg("AcqModeHighRes.png")),
                            new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Average"),2, GetTopInfoImg("AcqModeAverage.png")),
                            //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Envelope"),4, GetTopInfoImg("AcqModeEnvelope.png")),
                        };
                    }
                    else
                    {

                        CbxAcq.DataSource = new List<ComboBoxItem>()
                        {
                            new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Normal"), 0, GetTopInfoImg("AcqModeNormal.png")),
                            new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_Peak"), 1, GetTopInfoImg("AcqModePeak.png")),
                            //new ComboBoxItem(LanguageManger.Instance.GetIDMessage("AcqMode_HighRes"), 2, GetTopInfoImg("AcqModeHighRes.png")),
                        };

                       
                        _ArgToCtrl = false;
                     
                    }
                    if (selectindex >= CbxAcq.DataSource.Count)
                        CbxAcq.SelectValue = 0;
                    else
                        CbxAcq.SelectValue = Presenter.Mode;

                    ChkFast.Checked = Presenter.StorageMode == AnaChnlStorageMode.Fast;
                    if (Presenter.StorageMode == AnaChnlStorageMode.Long)
                    {
                        RdoStorageMode.ChoosedButtonIndex = 0;
                    }
                    else if (Presenter.StorageMode == AnaChnlStorageMode.Fast)
                    {
                        RdoStorageMode.ChoosedButtonIndex = 1;
                    }
                    RdoStorageMode.ChoosedButtonIndex = Presenter.StorageMode == AnaChnlStorageMode.Fast ? 1 : 0;
                    break;
                case nameof(Presenter.EnvelopOpt):
                    CbxEvlpOpt.SelectValue = (Int32)Presenter.EnvelopOpt;
                    break;
                case nameof(Presenter.EnableRIS):
                    ChkEnableRIS.Checked = Presenter.EnableRIS;
                    break;
                case nameof(Presenter.EnhancedBits):
                    NebBits.UpdateValueString();
                    break;
                case nameof(Presenter.ConstAutoStorageActive):
                    ChkConstAutoStorage.Checked = Presenter.ConstAutoStorageActive;
                    break;
                case nameof(Presenter.EnhancedBitsActive):
                    ChkERes.Checked = Presenter.EnhancedBitsActive;
                    LBlBits.Visible = Presenter.EnhancedBitsActive;
                    NebBits.Visible = Presenter.EnhancedBitsActive;
                    break;

            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                InitStorage();
                CbxAcq.SelectValue = (Int32)CbxAcq.SelectValue >= CbxAcq.DataSource.Count ? 0 : (Int32)Presenter.Mode;

                RdoInterpolation.ChoosedButtonIndex = (Int32)Presenter.InterplType;
                RdoClkSource.ChoosedButtonIndex = (Int32)Presenter.ClockSrc;
                if (Presenter.ClockSrc == AnaChnlClkSrc.Outter)
                {
                    if (Presenter.Ext10MHzLocked)
                    {
                        LblLocked.Text = LanguageManger.Instance.GetIDMessage("YiSuoDing");
                        LblLocked.ForeColor = Color.DeepSkyBlue;
                    }
                    else
                    {
                        LblLocked.Text = LanguageManger.Instance.GetIDMessage("WeiSuoDing");
                        LblLocked.ForeColor = Color.Red;
                    }
                    LblLocked.Visible = true;
                }
                else
                {
                    LblLocked.Visible = false;
                }
                NebAvgEnvpCnt.UpdateValueString();
                ChkERes.Checked = Presenter.EnhancedBitsActive;
                LBlBits.Visible = Presenter.EnhancedBitsActive;
                NebBits.Visible = Presenter.EnhancedBitsActive;
                NebBits.UpdateValueString();
                CbxStorageDepth.SelectValue = Presenter.StorageDepthOpt;
                if (Presenter.StorageDepthOpt == 0)
                {
                    //LblConstAutoStorage.Visible = true;
                    //ChkConstAutoStorage.Visible = true;
                    LblConstAutoStorage.Visible = false;
                    ChkConstAutoStorage.Visible = false;
                }
                else
                {
                    LblConstAutoStorage.Visible = false;
                    ChkConstAutoStorage.Visible = false;
                }
                RdoStorageMode.ChoosedButtonIndex = Presenter.StorageMode == AnaChnlStorageMode.Fast ? 1 : 0;
                CbxEvlpOpt.SelectValue = (Int32)Presenter.EnvelopOpt;
                ChkEnableRIS.Checked = Presenter.EnableRIS;
                ChkFast.Checked = Presenter.StorageMode == AnaChnlStorageMode.Fast;
                ChkConstAutoStorage.Checked = Presenter.ConstAutoStorageActive;

                _ArgToCtrl = false;

                SelectMode(Presenter.Mode);
            }
        }

        private void SelectMode(AnaChnlAcqMode mode)
        {
            ChkFast.Visible = LblFast.Visible = mode < AnaChnlAcqMode.Average;
   
            switch (mode)
            {
                case AnaChnlAcqMode.Average:
                    LblAvgEnvpCnt.Visible = true;
                    NebAvgEnvpCnt.Visible = true;
                    NebAvgEnvpCnt.UpdateValueString();
                    LblEvlpOpt.Visible = false;
                    CbxEvlpOpt.Visible = false;
                    ChkERes.Visible = true;
                    LBlERes.Visible = true;
                    break;
                case AnaChnlAcqMode.Envelope:
                    LblAvgEnvpCnt.Visible = true;
                    NebAvgEnvpCnt.Visible = true;
                    NebAvgEnvpCnt.UpdateValueString();
                    LblEvlpOpt.Visible = true;
                    CbxEvlpOpt.Visible = true;
                    ChkERes.Visible = true;
                    LBlERes.Visible = true;
                    break;
                case AnaChnlAcqMode.HighRes:
                    if(Presenter.EnhancedBitsActive)
                    {
                        Presenter.EnhancedBitsActive = false;
                    }
                    LblAvgEnvpCnt.Visible = false;
                    NebAvgEnvpCnt.Visible = false;
                    LblEvlpOpt.Visible = false;
                    CbxEvlpOpt.Visible = false;
                    ChkERes.Visible = false;
                    LBlERes.Visible = false;
                    break;
                default:
                    LblAvgEnvpCnt.Visible = false;
                    NebAvgEnvpCnt.Visible = false;
                    LblEvlpOpt.Visible = false;
                    CbxEvlpOpt.Visible = false;
                    ChkERes.Visible = true;
                    LBlERes.Visible = true;
                    break;
            }
            ChkERes.Visible = false;
            LBlERes.Visible = false;
            ChkFast.Visible = LblFast.Visible = false;
            
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
            InitStorage();
            RdoStorageMode.ScopeXIconButtons[1].Enabled = false;
        }

        private void RdoStorageMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (RdoStorageMode.ChoosedButtonIndex == 0)
                {
                    Presenter.StorageMode = AnaChnlStorageMode.Long;
                }
                else if (RdoStorageMode.ChoosedButtonIndex == 1)
                {
                    //WeakTip.Default.Write("FastAcq", MsgTipId.FunctionUnused, false, "", 5);
                    // Presenter.StorageMode = AnaChnlStorageMode.Fast;
                }
                //Presenter.StorageMode = (AnaChnlStorageMode)RdoStorageMode.ChoosedButtonIndex;
                //LblStorageDepth.Visible = CbxStorageDepth.Visible = Presenter.StorageMode == AnaChnlStorageMode.Long;
            }
        }

        private void RdoInterpolation_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.InterplType = (AnaChnlItplType)RdoInterpolation.ChoosedButtonIndex;
            }
        }

        private void RdoClkSource_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClockSrc = (AnaChnlClkSrc)RdoClkSource.ChoosedButtonIndex;
            }
        }

        //private void CbxEvlpOpt_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.EnvelopOpt = (EvlpOpt)CbxEvlpOpt.SelectedIndex;
        //    }
        //}

        private void ChkEnableRIS_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EnableRIS = ChkEnableRIS.Checked;
            }
        }

        private void ChkFast_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.StorageMode = ChkFast.Checked ? AnaChnlStorageMode.Fast : AnaChnlStorageMode.Long;
                ChkFast.Checked = Presenter.StorageMode == AnaChnlStorageMode.Fast;
            }
        }

        private void ChkConstAutoStorage_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ConstAutoStorageActive = ChkConstAutoStorage.Checked;
            }
        }

        private void ChkERes_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EnhancedBitsActive = ChkERes.Checked;
            }
        }

        private void CbxStorageDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.StorageDepthOpt = (Int32)CbxStorageDepth.SelectValue;
            }
        }

        private void CbxEvlpOpt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EnvelopOpt = (EvlpOpt)CbxEvlpOpt.SelectValue;
            }
        }

    }
}
