
namespace ScopeX.U2
{
    using CyUSB;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Management;
    using System.Windows.Forms;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class KeyboardDetectionForm : FloatForm, ISystemCheckView, IStylize
    {

        public KeyboardDetectionForm()
        {
            InitializeComponent();
            InitControlLang();
            IsShowHelp = false;
            IsShowPin = false;
            IsShowClose = false;
            Program.Oscilloscope.SystemCheck.TryAddView(this);
            Program.Oscilloscope.SystemCheck.ExitCount = 0;
        }
        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private Boolean _ArgToCtrl = false;
        private IkeyBoardDetetionView _DetectionPage;

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
        public SystemCheckPrsnt Presenter
        {
            get;
            set;
        }

        ISystemCheckPrsnt IView<ISystemCheckPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (SystemCheckPrsnt)value;
        }

        private void InitControlLang()
        {
            LblHintOne.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KeyboardDetectionForm.LblHintOneText");
            LblHintTwo.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KeyboardDetectionForm.LblHintTwoText");
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
            //_ArgToCtrl = true;
            Boolean enable = Presenter.CheckEnable;
            CheckType type = Presenter.ScopeCheckType;
            if (enable && type == CheckType.KeyboardCheck)
            {
                switch (propertyName)
                {

                    case nameof(Presenter.ScreenColorDisplay):
                        UpdateScreenColor();
                        break;
                    case nameof(Presenter.ExitCount):
                        UpdateExitCount();
                        break;
                    default:
                        break;
                }
                ((IkeyBoardDetetionView)_DetectionPage).UpdateView(prsnt, propertyName);
            }

            //_ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            KeyboardLed.Default.ScopeStateControl(ScopeStateType.Close, Convert.ToInt32(false));
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //ChkTouch.Checked = Presenter.TouchLock;
                //ChkStopMeasure.Checked = DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure;
                _ArgToCtrl = false;
            }
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            LoadOptionPage();
            base.OnLoad(e);
            UpdateView();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            KeyboardLed.Default.LedStateControl(LedEnum.LedRunStop, true);
        }
        public override void Refresh()
        {
            base.Refresh();
        }
        public void InitMask()
        {
            Presenter.CheckEnable = true;
            UpdateScreenColor();
        }
        private void LoadOptionPage()
        {
            _DetectionPage = PlatformUIManager.Default.Platform.KeyboardDetectionView;
            TlpOptions.Controls.Add((UserControl)_DetectionPage, 1, 0);
        }

        #region  Update更新函数

        private void UpdateExitCount()
        {
            //退出模板
            Presenter.CheckEnable = false;
            this.Dispose();
            KeyboardLed.Default.ScopeStateControl(ScopeStateType.Close, Convert.ToInt32(false));
            KeyboardLed.Default.LedStateControl(LedEnum.LedRunStop, true);
        }

        private void UpdateScreenColor()
        {
            ScreenMaskColor type = Presenter.ScreenColorDisplay;
            //this.LblHint_1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.LblHint_2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            switch (type)
            {

                case ScreenMaskColor.Red:
                    this.BackColor = Color.Red;
                    this.ContentBackColor = Color.Red;
                    break;
                case ScreenMaskColor.Green:
                    this.BackColor = Color.Green;
                    this.ContentBackColor = Color.Green;
                    break;
                case ScreenMaskColor.Blue:
                    this.BackColor = Color.Blue;
                    this.ContentBackColor = Color.Blue;
                    break;
                case ScreenMaskColor.Black:
                    this.BackColor = Color.Black;
                    this.ContentBackColor = Color.Black;
                    break;
                case ScreenMaskColor.White:
                    this.BackColor = Color.White;
                    this.ContentBackColor = Color.White;
                    //this.LblHint_1.ForeColor = Color.Black;
                    //this.LblHint_2.ForeColor = Color.Black;
                    break;
                default:
                    break;
            }
        }

        #endregion
        private void LblHint_1_Click(object sender, EventArgs e)
        {
            if (Presenter.ScreenColorDisplay < ScreenMaskColor.White)
            {
                Presenter.ScreenColorDisplay++;
            }
            else
            {
                Presenter.ScreenColorDisplay = ScreenMaskColor.Red;
            }
        }



        private void LblHint_2_Click(object sender, EventArgs e)
        {
            Presenter.ExitCount++;

        }

    }
}
