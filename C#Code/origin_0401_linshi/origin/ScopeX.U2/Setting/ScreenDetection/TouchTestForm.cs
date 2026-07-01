namespace ScopeX.U2
{
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
    using System.Reflection.Emit;
    using System.Windows.Forms;
    public partial class TouchTestForm : FloatForm, ISystemCheckView, IStylize
    {
        public TouchTestForm()
        {
            InitializeComponent();
            IsShowHelp = false;
            IsShowPin = false;
            IsShowClose = false;
            this.TouchColor_4_7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_4_7");
            this.TouchColor_4_8.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_4_8");
            this.TouchColor_4_9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_4_9");
            this.TouchColor_5_7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_5_7");
            this.TouchColor_5_8.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_5_8");
            this.TouchColor_5_9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TouchTestForm.TouchColor_5_9");
            Program.Oscilloscope.SystemCheck.TryAddView(this);
            Program.Oscilloscope.SystemCheck.ExitCount = 0;

        }

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private Boolean _ArgToCtrl = false;

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
            if (enable && type == CheckType.TouchCheck)
            {
                switch (propertyName)
                {
                    case nameof(Presenter.TextColorDisplay):
                        UpdateTouchTextColor();
                        break;
                    case nameof(Presenter.ExitCount):
                        UpdateExitCount();
                        break;
                    default:
                        break;
                }

            }

            //_ArgToCtrl = false;
        }

        protected void UpdateView()
        {
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
            base.OnLoad(e);
            UpdateView();
        }

        public override void Refresh()
        {
            base.Refresh();
        }
        public void InitTouch()
        {
            Presenter.CheckEnable = true;
            UpdateTouchTextColor();
        }

        #region Update函数
        private void UpdateExitCount()
        {
            //退出模板
            Presenter.CheckEnable = false;
            this.Dispose();
        }


        public void UpdateTouchTextColor()
        {
            TouchTestTextColor type = Presenter.TextColorDisplay;
            switch (type)
            {
                case TouchTestTextColor.Red:
                    this.TouchColor_4_7.ForeColor = Color.Red;
                    this.TouchColor_4_8.ForeColor = Color.Red;
                    this.TouchColor_4_9.ForeColor = Color.Red;
                    this.TouchColor_5_7.ForeColor = Color.Red;
                    this.TouchColor_5_8.ForeColor = Color.Red;
                    this.TouchColor_5_9.ForeColor = Color.Red;
                    break;
                case TouchTestTextColor.White:
                    this.TouchColor_4_7.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    this.TouchColor_4_8.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    this.TouchColor_4_9.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    this.TouchColor_5_7.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    this.TouchColor_5_8.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    this.TouchColor_5_9.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    break;
                case TouchTestTextColor.Black:
                    this.TouchColor_4_7.ForeColor = Color.Black;
                    this.TouchColor_4_8.ForeColor = Color.Black;
                    this.TouchColor_4_9.ForeColor = Color.Black;
                    this.TouchColor_5_7.ForeColor = Color.Black;
                    this.TouchColor_5_8.ForeColor = Color.Black;
                    this.TouchColor_5_9.ForeColor = Color.Black;
                    break;
                default:
                    break;
            }
        }

        private Boolean _IsPress = false;

        
        public void UpdateMousePositionTouchColor(Point mousePosition)
        {
            

            Int32 y = mousePosition.Y;
            Int32 areaheight = 120;
            if (y < areaheight * 1 && y >= areaheight * 0)
            {
                //第一行 label刷新颜色
                UpdateTouchLabelColor1(mousePosition);
            }
            else if (y < areaheight * 2 && y >= areaheight * 1)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor2(mousePosition);
            }
            else if (y < areaheight * 3 && y >= areaheight * 2)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor3(mousePosition);
            }
            else if (y < areaheight * 4 && y >= areaheight * 3)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor4(mousePosition);
            }
            else if (y < areaheight * 5 && y >= areaheight * 4)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor5(mousePosition);
            }
            else if (y < areaheight * 6 && y >= areaheight * 5)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor6(mousePosition);
            }
            else if (y < areaheight * 7 && y >= areaheight * 6)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor7(mousePosition);
            }
            else if (y < areaheight * 8 && y >= areaheight * 7)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor8(mousePosition);
            }
            else if (y < areaheight * 9 && y >= areaheight * 8)
            {
                //第二行 label刷新颜色
                UpdateTouchLabelColor9(mousePosition);
            }
        }

        private void GetEnterPoint(object sender, EventArgs e)
        {
            
            if (!_IsPress)
            {
                if (e is MouseEventArgs mouseArgs)
                {
                    if (mouseArgs.Button == MouseButtons.Left)
                    {
                        _IsPress = true;
                    }
                    else 
                    {
                        //如果触屏未按下，则不进行触摸检测
                        return;
                    }
                }

            }

            // 获取鼠标的全局坐标
            Point globalMousePosition = Control.MousePosition;

            // 显示全局坐标
            // TouchColor_4_7.Text = $"{globalMousePosition.X}, {globalMousePosition.Y}";

            //根据坐标更新TouchLabel颜色
            UpdateMousePositionTouchColor(globalMousePosition);
        }

        #endregion


        #region  第1行 label刷新颜色
        public void UpdateTouchLabelColor1(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_1_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_1_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_1_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_1_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_1_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_1_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_1_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_1_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_1_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_1_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_1_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_1_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_1_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_1_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_1_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_1_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第2行 label刷新颜色
        public void UpdateTouchLabelColor2(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_2_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_2_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_2_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_2_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_2_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_2_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_2_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_2_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_2_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_2_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_2_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_2_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_2_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_2_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_2_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_2_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第3行 label刷新颜色
        public void UpdateTouchLabelColor3(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_3_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_3_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_3_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_3_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_3_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_3_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_3_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_3_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_3_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_3_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_3_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_3_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_3_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_3_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_3_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_3_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第4行 label刷新颜色
        public void UpdateTouchLabelColor4(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_4_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_4_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_4_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_4_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_4_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_4_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_4_7.ForeColor = System.Drawing.SystemColors.ButtonFace;
                this.TouchColor_4_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_4_8.ForeColor = System.Drawing.SystemColors.ButtonFace;
                this.TouchColor_4_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_4_9.ForeColor = System.Drawing.SystemColors.ButtonFace;
                this.TouchColor_4_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_4_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_4_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_4_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_4_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_4_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_4_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_4_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第5行 label刷新颜色
        public void UpdateTouchLabelColor5(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_5_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_5_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_5_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_5_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_5_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_5_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_5_7.BackColor = System.Drawing.Color.MediumBlue;
                this.TouchColor_5_7.ForeColor = System.Drawing.SystemColors.ButtonFace;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_5_8.BackColor = System.Drawing.Color.MediumBlue;
                this.TouchColor_5_8.ForeColor = System.Drawing.SystemColors.ButtonFace;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_5_9.BackColor = System.Drawing.Color.MediumBlue;
                this.TouchColor_5_9.ForeColor = System.Drawing.SystemColors.ButtonFace;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_5_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_5_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_5_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_5_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_5_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_5_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_5_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第6行 label刷新颜色
        public void UpdateTouchLabelColor6(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_6_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_6_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_6_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_6_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_6_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_6_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_6_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_6_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_6_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_6_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_6_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_6_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_6_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_6_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_6_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_6_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第7行 label刷新颜色
        public void UpdateTouchLabelColor7(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_7_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_7_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_7_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_7_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_7_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_7_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_7_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_7_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_7_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_7_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_7_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_7_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_7_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_7_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_7_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_7_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第8行 label刷新颜色
        public void UpdateTouchLabelColor8(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_8_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_8_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_8_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_8_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_8_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_8_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_8_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_8_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_8_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_8_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_8_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_8_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_8_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_8_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_8_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_8_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion

        #region 第9行 label刷新颜色
        public void UpdateTouchLabelColor9(Point mousePosition)
        {
            Int32 x = mousePosition.X;
            Int32 areawidth = 120;
            if (x < areawidth * 1 && x >= areawidth * 0)
            {
                this.TouchColor_9_1.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 2 && x >= areawidth * 1))
            {
                this.TouchColor_9_2.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 3 && x >= areawidth * 2))
            {
                this.TouchColor_9_3.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 4 && x >= areawidth * 3))
            {
                this.TouchColor_9_4.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 5 && x >= areawidth * 4))
            {
                this.TouchColor_9_5.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 6 && x >= areawidth * 5))
            {
                this.TouchColor_9_6.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 7 && x >= areawidth * 6))
            {
                this.TouchColor_9_7.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 8 && x >= areawidth * 7))
            {
                this.TouchColor_9_8.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 9 && x >= areawidth * 8))
            {
                this.TouchColor_9_9.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 10 && x >= areawidth * 9))
            {
                this.TouchColor_9_10.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 11 && x >= areawidth * 10))
            {
                this.TouchColor_9_11.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 12 && x >= areawidth * 11))
            {
                this.TouchColor_9_12.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 13 && x >= areawidth * 12))
            {
                this.TouchColor_9_13.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 14 && x >= areawidth * 13))
            {
                this.TouchColor_9_14.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 15 && x >= areawidth * 14))
            {
                this.TouchColor_9_15.BackColor = System.Drawing.Color.MediumBlue;
            }
            else if ((x < areawidth * 16 && x >= areawidth * 15))
            {
                this.TouchColor_9_16.BackColor = System.Drawing.Color.MediumBlue;
            }
        }
        #endregion


    }
}
