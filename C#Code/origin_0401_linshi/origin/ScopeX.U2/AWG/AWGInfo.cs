using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2.AWG
{
    [DefaultEvent("Click")]
    [DefaultValue("Text")]
    public partial class AWGInfo : UserControl, IWfmGenView, IChannelInfoStyle
    {
        #region 多语言切换支持

        internal string Row1stLangName { get; set; }
        internal string Row2stLangName { get; set; }
        internal string Row3stLangName { get; set; }
        #endregion

        public AWGInfo(IWfmGenPrsnt prsnt)
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Presenter = (ArbWfmGenPrsnt)prsnt;
            Presenter.TryAddView(this);

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
            Row1stName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Row1stLangName);
            Row2ndName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Row2stLangName);
            Row3rdName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Row3stLangName);
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

        [Editor(), Browsable(true), DefaultValue(35), Category("CatAppearance"), Description("The width of the row's name")]
        public Int32 NameWidth
        {
            get;
            set;
        } = 35;

        public ArbWfmGenPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArbWfmGenPrsnt)value;
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
                if (propertyName is nameof(Presenter.Frequency)
                    or nameof(Presenter.Amplitude)
                    or nameof(Presenter.Offset)
                    or nameof(Presenter.WfmType)
                    or nameof(Presenter.DrawColor)
                    or nameof(Presenter.Active)
                    or nameof(Presenter.IsShow)
                    or nameof(Presenter.Mode)
                    or nameof(Presenter.ModMethod)
                    or nameof(Presenter.EnablePointByPoint))
                {
                    Refresh();
                    if (Presenter.IsShow)
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddAwgInfo(Presenter.Id);
                    }
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
            Color newColor;
            if (Presenter.IsShow)
            {
                newColor = ControlPaint.Light(Color.Blue, 0.5F);
            }
            else
                newColor = Presenter.DrawColor;

            //画标题栏内容
            e.Graphics.FillRectangle(new SolidBrush(newColor), new Rectangle(0, 0, Width, HeaderHeight));

            var tfc = TitleForeColor;
            var cfc = ContentForeColor;
            //if (!Presenter.Active)
            //{
            //	tfc = ControlPaint.Dark(tfc, 0.2F);
            //	cfc = ControlPaint.Dark(cfc, 0.2F);
            //}
            TextRenderer.DrawText(e.Graphics,
                Text,
                ChannelInfoStyleDefine.BoldFont,
                new Rectangle(0, 0, Width / 4, HeaderHeight),
                Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            var wt = Presenter.Mode switch
            {
                WfmGenMode.Continuous => Presenter.WfmType.GetDescription_Lang(),// Presenter.WfmType.GetDescription(),
                WfmGenMode.Modulation => Presenter.ModMethod.GetDescription_Lang(),// .GetDescription(),
                WfmGenMode.Sweep => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SaoPin"),// Presenter.Mode.GetDescription_Lang(),// .GetDescription(),
                _ => ""
            };
            if (!Presenter.IsShow)
            {
                wt = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            }
            TextRenderer.DrawText(e.Graphics,
                wt,
                ContentFont,
                new Rectangle(Width / 4, 0, Width / 4 * 3, HeaderHeight),
                tfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //画具体信息
            Int32 h = (Height - HeaderHeight - BottomBarBackThickness) / 3;
            Int32 w = Width - NameWidth;
            Int32 y = HeaderHeight;

            //<Remark>创建人：彭博 创建日期：2024/2/27 10:21:00  原因：当选择直流或噪音时不显示频率参数 </Remark>
            if (Presenter.Mode != WfmGenMode.Sweep && Presenter.WfmType != ArbWfmType.Noise && Presenter.WfmType != ArbWfmType.DC)
            {
                //<Remark>创建人：彭博 创建日期：2023/11/29 13:41:00  原因：测试需求，新增采样率参数 </Remark>
                //Row1stName = Presenter.EnablePointByPoint ? Properties.Resources.FilterDesign_FreqSampling : Properties.Resources.AWGInfo_Frequency;
                Row1stName = Presenter.EnablePointByPoint ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FilterDesign_FreqSampling") : ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AWGInfo_Frequency"); //Properties.Resources.AWGInfo_Frequency;
                //信息第一行
                TextRenderer.DrawText(
                    e.Graphics,
                    Row1stName,
                    ContentFont,
                    new Rectangle(0, y, NameWidth + 10, h),
                    cfc,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    FreqToString(),
                    ContentFont,
                    new Rectangle(NameWidth, y, w, h),
                    cfc,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                y += h;
            }
            if (Presenter.WfmType != ArbWfmType.DC)
            {
                //信息第二行
                TextRenderer.DrawText(
                    e.Graphics,
                    ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Row2ndName),
                    ContentFont,
                    new Rectangle(0, y, NameWidth, h),
                    cfc,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(
                    e.Graphics,
                    AmpToString(),
                    ContentFont,
                    new Rectangle(NameWidth + 5, y, w, h),
                    cfc,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                y += h;
            }
            //信息第三行
            TextRenderer.DrawText(
                e.Graphics,
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(Row3rdName),
                ContentFont,
                new Rectangle(0, y, NameWidth, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(
                e.Graphics,
                OffsetToString(),
                ContentFont,
                new Rectangle(NameWidth, y, w, h),
                cfc,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //画BottomBar
            e.Graphics.FillRectangle(
                new SolidBrush(Presenter.DrawColor),
                new Rectangle(0, Height - BottomBarBackThickness, Width, BottomBarBackThickness));
        }
        /// <summary>
        /// 显示参数窗口
        /// </summary>
        /// <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
        internal void ShowForm()
        {
            OnBodyClicked();
        }

        private void OnBodyClicked()
        {
            if (null != ParentForm)
            {
                (ParentForm as DsoForm).MakeOperateForm(Presenter.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
                {
                    var form = new AWGForm()
                    {
                        Presenter = Presenter,
                        Anchor = AnchorStyles.Bottom,
                        Text = Presenter.Name
                    };
                    _ = form.Presenter.TryAddView(form);

                    form.FormClosed += (s, e) => Focus();
                    return form;
                });
            }
        }

        private void OnHeaderClicked()
        {
            Presenter.Active = false;
            Presenter.IsShow = false;
            (ParentForm as DsoForm).RemoveWaveformUI(Presenter);
        }

        //protected override void OnMouseClick(MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        return;
        //    }

        //    if (PointToClient(MousePosition).Y < HeaderHeight)
        //    {
        //        OnHeaderClicked();
        //    }
        //    else
        //    {
        //        OnBodyClicked();
        //    }
        //}

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
        }

        private String FreqToString()
        {
            if (!Presenter.EnablePointByPoint)
            {
                var res = new Quantity(Presenter.Frequency, Prefix.Micro, "Hz").ToString(4, false);
                if (Char.IsLetter(res, res.Length - 1))
                {
                    return res + "Hz";
                }

                return res + " Hz";
            }
            else
            {
                var res = new Quantity(Presenter.Frequency, Prefix.Micro, "Sa/s").ToString(4, false);
                if (Char.IsLetter(res, res.Length - 1))
                {
                    return res + "Sa/s";
                }

                return res + " Sa/s";
            }
        }

        private String AmpToString()
        {
            var res = string.Empty;
            Prefix prefix = Prefix.Milli;
            prefix = Presenter.QuantityUnitByAmp == QuantityUnit.dBm ? Prefix.Empty : prefix;
            res = new Quantity(Presenter.AmpliteudeValue, prefix, QuantityUnitExt.UnitTable[Presenter.QuantityUnitByAmp]).ToString(4, false);
            res = Presenter.QuantityUnitByAmp != QuantityUnit.dBm ? new Quantity(Presenter.AmpliteudeValue, prefix, QuantityUnitExt.UnitTable[Presenter.QuantityUnitByAmp]).ToString(4, false) : $"{string.Format("{0:##0.000}", Presenter.AmpliteudeValue)} ";
            if (Char.IsLetter(res, res.Length - 1))
            {
                return res + QuantityUnitExt.UnitTable[Presenter.QuantityUnitByAmp];
            }
            return res + QuantityUnitExt.UnitTable[Presenter.QuantityUnitByAmp];

        }

        private String OffsetToString()
        {
            var res = new Quantity(Presenter.Offset, Prefix.Milli, "V").ToString(4, false);
            if (Char.IsLetter(res, res.Length - 1))
            {
                return res + "V";
            }

            return res + " V";
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
            ((IChannelInfoStyle)this).DefaultSetFocusedTitleStyle(Color.FromArgb(72, 77, 85));
        }

        public void SetUnfocusedTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetUnfocusedTitleStyle(Color.FromArgb(72, 77, 85));
        }

        public void SetDeactiveTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetDeactiveTitleStyle();
        }


        #endregion IChanleInfoStyle的实现

        //public void SetToolTip(ToolTip toolTip)
        //{
        //    toolTip.SetToolTip(this, Properties.ToolTips.AboutAwgBadge);
        //}
    }
}
