using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;

namespace ScopeX.U2
{
    [DefaultEvent("Click")]
    [DefaultValue("Text")]
    public partial class PowerQualityInfo : UserControl, IPwrAnalysisView, IChannelInfoStyle
    {
        #region 多语言资源Key名称
        internal string Row1LangKey { get; set; }

        internal string Row2LangKey { get; set; }

        internal string Row3LangKey { get; set; }
        #endregion

        public PowerQualityInfo()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //设置风格
            HeaderHeight = ChannelInfoStyleDefine.TitleHeight;
            ContentFont = ChannelInfoStyleDefine.NormalFont;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Row1stName = LanguageHelper.GetPowerAnalysisString(Row1LangKey);
            Row2ndName = LanguageHelper.GetPowerAnalysisString(Row2LangKey);
            Row3rdName = LanguageHelper.GetPowerAnalysisString(Row3LangKey);
            this.Invalidate();
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
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }
        #region
        [Editor(), Browsable(true), DefaultValue(28), Category("CatAppearance"), Description("Header Height")]
        public Int32 HeaderHeight
        {
            get;
            set;
        } = 28;

        [Editor(), Browsable(true), DefaultValue(typeof(Font), "Consolas, 10.5pt"), Category("CatAppearance"), Description("Content Font")]
        public Font ContentFont
        {
            get;
            set;
        } = new Font(new FontFamily("Consolas"), 10.5f, FontStyle.Regular);


        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("The name of Row 1st")]
        public String Row1stName
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("The name of Row 2nd")]
        public String Row2ndName
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("The name of Row 3rd")]
        public String Row3rdName
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(50), Category("CatAppearance"), Description("The width of the row's name")]
        public Int32 NameWidth
        {
            get;
            set;
        } = 50;
        #endregion

        public PowerAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => (IBadge)Presenter;
            set => Presenter = (PowerAnalysisPrsnt)value;
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
                Refresh();
                return;
            }
            if (!DesignMode)
            {
                switch (propertyName)
                {
                    //!!!Remove itself from presnter
                    case nameof(Presenter.Active):
                        {
                            if (!Presenter.Active)
                            {
                                Presenter.TryRemoveView(this);
                                Program.Oscilloscope.PwrAnalysisDictionary.Remove(Presenter.Id);
                                (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI((IBadge)Presenter);
                                Presenter.BoundMathPrsnt1.Active = false;
                                PowerAnalysisApp.Default.CloseDataTableForm(Presenter);
                            }
                        }
                        break;
                    case nameof(Presenter.VoltageSrc1):
                    case nameof(Presenter.CurrentSrc1):
                        {
                            this.Refresh();
                        }
                        break;
                    default: break;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var bkc = ContentBackColor;
            if (Focused /*&& ShowFocusCues*/)
            {
                bkc = ControlPaint.Light(bkc, 0.3F);
            }

            e.Graphics.Clear(bkc);
            TitleBackColor = Color.FromArgb(255, 77, 77, 77);
            //画标题栏内容
            e.Graphics.FillRectangle(new SolidBrush(TitleBackColor), new Rectangle(0, 0, Width, HeaderHeight));
            var tfc = TitleForeColor = Color.FromArgb(255, 234, 234, 234);
            var cfc = ContentForeColor;
            //if (!Presenter.Active)
            //{
            //    tfc = ControlPaint.Dark(tfc, 0.2F);
            //    cfc = ControlPaint.Dark(cfc, 0.2F);
            //}
            TextRenderer.DrawText(e.Graphics,
                $"{Text}",
                ChannelInfoStyleDefine.BoldFont,
                new Rectangle(0, 0, Width / 2, HeaderHeight),
                tfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            string str = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanZhiLiang");
            TextRenderer.DrawText(e.Graphics,
                str,
                ContentFont,
                new Rectangle(Width / 3, 0, Width / 3 * 2, HeaderHeight),
                tfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            #region 画具体信息
            //画具体信息
            Int32 h = (Height - HeaderHeight - BottomBarBackThickness) / 3;
            Int32 w = Width - NameWidth;
            Int32 y = HeaderHeight;

            //信息第一行
            TextRenderer.DrawText(
                e.Graphics,
                Row1stName,
                ContentFont,
                new Rectangle(0, y, NameWidth, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(
                e.Graphics,
                Presenter.VoltageSrc1.ToString(),
                ContentFont,
                new Rectangle(NameWidth - 2, y, w, h),
                cfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //信息第二行
            y += h;

            TextRenderer.DrawText(
                e.Graphics,
                Row2ndName,
                ContentFont,
                new Rectangle(0, y, NameWidth, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(
                e.Graphics,
                Presenter.CurrentSrc1.ToString(),
                ContentFont,
                new Rectangle(NameWidth, y, w, h),
                cfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //信息第二行
            y += h;

            TextRenderer.DrawText(
                e.Graphics,
                Row3rdName,
                ContentFont,
                new Rectangle(0, y, NameWidth, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(
                e.Graphics,
               Presenter.BoundMathPrsnt1?.Id.ToString(),
                ContentFont,
                new Rectangle(NameWidth, y, w, h),
                cfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            #endregion

            //画BottomBar
            e.Graphics.FillRectangle(
                new SolidBrush(BottomBarBackColor),
                new Rectangle(0, Height - BottomBarBackThickness, Width, BottomBarBackThickness));
        }

        private void OnBodyClicked()
        {
            var name = Presenter.Mode.GetDescription_Lang();
            (Program.Oscilloscope.View as DsoForm).MakeOperateForm(Presenter.Id.ToString() + "(" + name + ")", PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
            {
                PowerAnalysisPrsnt poweranalysisprsnt = null;
                Program.Oscilloscope.PwrAnalysisDictionary.TryGetValue(Presenter.Id, out poweranalysisprsnt);
                var form = new PowerQualityForm(poweranalysisprsnt)
                {
                    Anchor = AnchorStyles.Bottom,
                    Text = Presenter.Id.ToString() + "(" + name + ")"
                };
                _ = form.Presenter.TryAddView(form);
                form.FormClosed += (s, e) => Focus();
                return form;
            });
        }

        private void OnHeaderClicked()
        {
            //Presenter.Active = false;
            //(ParentForm as DsoForm).RemoveWaveformUI(Presenter);
        }

        protected override void OnClick(EventArgs e)
        {
            if (e is MouseEventArgs mea && mea.Button == MouseButtons.Right)
            {
                return;
            }

            if (PointToClient(MousePosition).Y < HeaderHeight)
            {
                OnHeaderClicked();
            }
            else
            {
                OnBodyClicked();
            }
            base.OnClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                OnClick(e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate(true);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetContentStyle();
            SetUnfocusedTitleStyle();
            OnBodyClicked();
        }

        #region IChanleInfoStyle的实现

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Red"), Category("CatAppearance"), Description("Channel Header BackColor")]
        public Color TitleBackColor
        {
            get;
            set;
        } = Color.FromArgb(32, 32, 32);

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Channel Header ForeColor")]
        public Color TitleForeColor
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Channel Content BackColor")]
        public Color ContentBackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Channel Content ForeColor")]
        public Color ContentForeColor
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Channel Bottom Bar Color")]
        public Color BottomBarBackColor
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Int32), "0"), Category("CatAppearance"), Description("Channel Bottom Bar BorderThickness")]
        public Int32 BottomBarBackThickness
        {
            get;
            set;
        }

        public void SetContentStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetContentStyle();
        }

        public void SetFocusedTitleStyle()
        {
            //((IChannelInfoStyle)this).DefaultSetFocusedTitleStyle(Presenter.DrawColor);
        }

        public void SetUnfocusedTitleStyle()
        {
            //((IChannelInfoStyle)this).DefaultSetUnfocusedTitleStyle(Presenter.DrawColor);
        }

        public void SetDeactiveTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetDeactiveTitleStyle();
        }


        #endregion IChanleInfoStyle的实现
    }
}
