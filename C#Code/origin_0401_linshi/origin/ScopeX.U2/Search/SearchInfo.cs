using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using ScopeX.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2.Search
{
    internal record SearchInfoVisibleArgs(Boolean visible);

    public partial class SearchInfo : UserControl, ISearchItemView, IChannelInfoStyle, IPanel
    {

        #region Field&Property
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

        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
            }
            remove
            {
                base.Click -= value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Channel Name String")]
        public override String Text
        {
            get => Presenter?.Name;
        }

        public SearchItemPrsnt Presenter
        {
            get;
            set;
        }

        ISearchItemPrsnt IView<ISearchItemPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (SearchItemPrsnt)value;
        }

        SearchType ISearchItemView.Type => Presenter.Type;

        #endregion Field&Property
        private Boolean _ArgToCtrl;
        public SearchInfo(SearchItemPrsnt prsnt)
        {
            InitializeComponent();
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.AllowDrop = true;
            Presenter = prsnt;
            Presenter.TryAddView(this);
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            this.Invalidate();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
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


        public void Close()
        {
            Presenter.Active = false;
            Presenter.TryRemoveView(this);
        }

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            String[] property = propertyName.Split(":");
            if (property.Length == 2)
            {
                if (property[0] != Presenter.Name)
                {
                    UpdateView();
                    return;
                }
                else
                {
                    propertyName = property[1];
                }
            }

            _ArgToCtrl = true;
            if (!DesignMode)
            {
                switch (propertyName)
                {
                    default:
                    case nameof(Presenter.Type):
                    case "CompPosition":
                    case nameof(Presenter.Source):
                        this.Invalidate();
                        if (TriggerPrsnt.State == SysState.Stop)
                        {
                            Presenter.OnceSearch = true;
                            if (Presenter.Focused && Presenter.EventEnable && SearchApp.Default.InfoForm != null)
                            {
                                (SearchApp.Default.InfoForm as SearchInfoForm).NeedUpdate = true;
                            }
                        }
                        break;
                    case nameof(Presenter.Focused):
                        HeaderBackColor = Presenter.DrawColor;
                        this.Invalidate();
                      
                        break;
                    case nameof(Presenter.Visible):
                    case nameof(Presenter.ResultCount):
                        this.Invalidate();
                        break;
                    case nameof(Presenter.Active):
                        if (this.Visible!=Presenter.Active)
                        {
                            this.Visible = false;
                        }
                        break;
                    case nameof(Presenter.EventEnable):
                       if(SearchApp.Default.FoucsItem?.ID==Presenter.ID)
                       {
                            if (Presenter.EventEnable)
                            {
                                if (SearchApp.Default.InfoForm == null || SearchApp.Default.InfoForm.IsDisposed)
                                    SearchApp.Default.ShowInfoForm(Presenter);
                            }
                            else
                            {
                                SearchApp.Default.HideInfoForm();
                            }
                       }
                        break;
                }
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                SetContentStyle();
                HeaderBackColor = Presenter.Focused ? Presenter.DrawColor : Color.Gray;
                this.Invalidate();
                _ArgToCtrl = false;
            }
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
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            Presenter.TryRemoveView(this);
        }


        private Bitmap _Buffer;

        private Color BtnBorderColor = Color.FromArgb(185, 192, 199);

        private Int32 HeaderHeight = 30;

        private Bitmap VisiableImage
        {
            get
            {
                return Presenter?.Visible == false ? Properties.Resources.Hide : Properties.Resources.Show;
            }
        }

        private float VisiableBoderWidth = 0;

        private Bitmap PreviousImage
        {
            get
            {
                return TriggerPrsnt.State == SysState.Stop && Presenter?.Focused == true ? Properties.Resources.Navigation_Previous_Enable : null;
            }
        }

        private Bitmap NextImage
        {
            get
            {
                return TriggerPrsnt.State == SysState.Stop && Presenter?.Focused == true ? Properties.Resources.Navigation_Next_Enable : null;
            }
        }

        private Rectangle VisiableRect
        {
            get
            {
                return new Rectangle(new Point(180, 0), new Size(Width - 180 - 1, 30 - 1));
            }
        }

        private Rectangle VisiableRealRect
        {
            get
            {
                return new Rectangle(new Point(200, 0), new Size(30, 30));
            }
        }


        private Rectangle PreviousRect
        {
            get
            {
                return new Rectangle(new Point(0, HeaderHeight + (Height - HeaderHeight - BottomBarBackThickness) / 2), new Size(Width / 2, (Height - HeaderHeight - BottomBarBackThickness) / 2));
            }
        }

        private Rectangle PreviousDrawRect
        {
            get
            {
                return new Rectangle(new Point(PreviousRect.Width / 4, HeaderHeight + (Height - HeaderHeight - BottomBarBackThickness) / 2), new Size(PreviousRect.Width / 2, (Height - HeaderHeight - BottomBarBackThickness) / 2));
            }
        }

        private float PreviousBoderWidth = 0;

        private Rectangle NextRect
        {
            get
            {
                return new Rectangle(new Point(Width / 2, HeaderHeight + (Height - HeaderHeight - BottomBarBackThickness) / 2), new Size(Width / 2, (Height - HeaderHeight - BottomBarBackThickness) / 2));
            }
        }

        private Rectangle NextDrawRect
        {
            get
            {
                return new Rectangle(new Point(NextRect.X + NextRect.Width / 4, HeaderHeight + (Height - HeaderHeight - BottomBarBackThickness) / 2), new Size(NextRect.Width / 2, (Height - HeaderHeight - BottomBarBackThickness) / 2));
            }
        }

        private float NextBoderWidth = 0;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            SetStyle(ControlStyles.UserPaint, false);
            base.OnPaint(pevent);
            Rectangle o = pevent.ClipRectangle;
            if (Width > 0 && Height > 0)
            {
                DrawBuffer(pevent.Graphics);
            }
            SetStyle(ControlStyles.UserPaint, true);
        }

        private void DrawBuffer(Graphics g)
        {
            // 绘制 Header
            String title = string.Empty;
            String source = string.Empty;
            if (Presenter != null)
            {
                title = $"{Presenter?.Name}({SIHelper.ValueChangeToSI(Presenter.ResultCount, 0)})";
                source = Presenter?.Source.ToString();
            }
            using (Brush headerBrush = new SolidBrush(HeaderBackColor))
            {
                g.FillRectangle(headerBrush, 0, 0, this.Width, HeaderHeight);
                using (Brush textBrush = new SolidBrush(ChannelInfoStyleDefine.TitleDeactiveForeColor))
                {
                    g.DrawString(title, ChannelInfoStyleDefine.NormalFont, textBrush, new PointF(5, 5));
                    g.DrawString(source, ChannelInfoStyleDefine.NormalFont, textBrush, new PointF(120, 5));
                    using (var img = VisiableImage)
                    {
                        var rect = VisiableRealRect;
                        if (VisiableBoderWidth != 0)
                        {
                            using (Pen pen = new Pen(BtnBorderColor, VisiableBoderWidth))
                            {
                                g.DrawRectangle(pen, VisiableRect);
                            }
                            rect.Y--;
                        }
                        g.DrawImage(img, rect);
                    }
                }
            }

            // 绘制 Body
            using (Brush bodyBrush = new SolidBrush(ContentBackColor))
            {
                g.FillRectangle(bodyBrush, 0, 30, this.Width, this.Height - HeaderHeight - BottomBarBackThickness);
                if (Presenter != null)
                {
                    using (Brush textBrush = new SolidBrush(ChannelInfoStyleDefine.TitleDeactiveForeColor))//预留60的高度
                    {
                        float width = this.Width / Presenter.KeyInfos.Count;
                        float height = HeaderHeight + 20;
                        float x = width/2.0f;
                        SizeF textSize;
                        foreach (var info in Presenter.KeyInfos)
                        {
                            String txt = LanguageManger.Instance.GetIDMessage(info);
                            if(txt.Contains("\n")||txt.Contains("\t"))//存在换行符,暂时只会存在一个换行符
                            {
                                if(txt.Contains("\t"))//如果既有换行又有制表符则，制表符前后并列
                                {
                                    int tabIndex = txt.IndexOf('\t');
                                    String txt1 = txt.Substring(0, tabIndex);
                                    if(!String.IsNullOrEmpty(txt1))
                                    {
                                        txt1 = LanguageManger.Instance.GetIDMessage(txt1);
                                    }
                                    String txt2 = txt.Substring(tabIndex, txt.Length- txt.Substring(0, tabIndex).Length).Replace("\t","");
                                    if (!String.IsNullOrEmpty(txt2))
                                    {
                                        txt2 = LanguageManger.Instance.GetIDMessage(txt2);
                                    }
                                    textSize = g.MeasureString(txt.Replace("\t", ""), ChannelInfoStyleDefine.NormalFont);//计算文本尺寸
                                    var textSizetemp1 = g.MeasureString(txt1, AppStyleConfig.DefaultMeasureFont);//计算文本尺寸
                                    var textSizetemp2 = g.MeasureString(txt2, AppStyleConfig.DefaultMeasureFont);//计算文本尺寸
                                   if(textSizetemp1.Width+ textSizetemp2.Width<= width)
                                    {
                                        g.DrawString(txt1, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f , HeaderHeight+23));
                                        if (txt2.Contains("\n"))
                                        {
                                            g.DrawString(txt2, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f + textSizetemp1.Width, HeaderHeight + 12));
                                        }
                                    }
                                    else
                                    {
                                        g.DrawString(txt1, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f - textSizetemp1.Width / 3.0f, HeaderHeight + 23));
                                        if (txt2.Contains("\n"))
                                        {
                                            g.DrawString(txt2, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f + textSizetemp1.Width*0.6666f, HeaderHeight + 12));
                                        }
                                    }
                                }
                                else
                                {
                                    // 找到第一个换行符的索引
                                    int NewLineIndex = txt.IndexOf('\n');
                                    String txt1 = txt.Substring(0, NewLineIndex);
                                    if (!String.IsNullOrEmpty(txt1))
                                    {
                                        txt1 = LanguageManger.Instance.GetIDMessage(txt1);
                                    }
                                    String txt2 = txt.Substring(NewLineIndex, txt.Length- txt.Substring(0, NewLineIndex).Length).Replace("\n", "");
                                    if (!String.IsNullOrEmpty(txt2))
                                    {
                                        txt2 = LanguageManger.Instance.GetIDMessage(txt2);
                                    }
                                    textSize = g.MeasureString(txt1, AppStyleConfig.DefaultMeasureFont);//计算文本尺寸
                                    g.DrawString(txt1, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f, HeaderHeight+5));
                                    textSize = g.MeasureString(txt2, AppStyleConfig.DefaultMeasureFont);//计算文本尺寸
                                    g.DrawString(txt2, AppStyleConfig.DefaultMeasureFont, textBrush, new PointF(x - textSize.Width / 2.0f, HeaderHeight + 30));
                                }
                            }
                            else
                            {
                                Font font = ChannelInfoStyleDefine.NormalFont;
                                var h = height;
                                 textSize = g.MeasureString(txt, font);//计算文本尺寸
                                if(textSize.Width> width)
                                {
                                    font = AppStyleConfig.DefaultMeasureFont;
                                    textSize = g.MeasureString(txt, font);//计算文本尺寸
                                    height += 3;
                                }
                                g.DrawString(txt, font, textBrush, new PointF(x - textSize.Width / 2.0f, height));
                            }
                            x += width;
                        }
                    }
                    //导航区域（向前，向后）
                    using (var img = PreviousImage)
                    {
                        if (img != null)
                        {
                            var rect = PreviousDrawRect;
                            if (PreviousBoderWidth != 0)
                            {
                                using (Pen pen = new Pen(BtnBorderColor, PreviousBoderWidth))
                                {
                                    g.DrawRectangle(pen, PreviousRect);
                                }
                                rect.Y--;
                            }
                            g.DrawImage(img, rect);
                        }
                    }
                    using (var img = NextImage)
                    {
                        if (img != null)
                        {
                            var rect = NextDrawRect;
                            if (NextBoderWidth != 0)
                            {
                                using (Pen pen = new Pen(BtnBorderColor, NextBoderWidth))
                                {
                                    g.DrawRectangle(pen, NextRect);
                                }
                                rect.Y--;
                            }
                            g.DrawImage(img, rect);
                        }
                    }
                }
            }

            // 绘制 Footer
            using (Brush footerBrush = new SolidBrush(HeaderBackColor))
            {
                g.FillRectangle(footerBrush, 0, this.Height - BottomBarBackThickness, this.Width, BottomBarBackThickness);
            }

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _Buffer = new Bitmap(Width, Height);
        }

        public long ID { get => Presenter.ID; }

        #region 点击
        private Boolean IsMouseEnter = false;
        private Boolean IsMouseDown = false;
        private int IsOtherMouseDown = 0;//0代表非按钮，1代表Visiable,2代表上一个标记，3代表下一个标记
        private Point? MouseDownPoint = null;
        private Boolean IsMouseClick = false;

        protected override void OnClick(EventArgs e)
        {
            if (Presenter != null && !Presenter.Focused)
            {
                SearchApp.Default.Presenter.FoucsID = Presenter.ID;
                return;
            }
           
            if (IsOtherMouseDown == 0)
            {
                base.OnClick(e);
                _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_WAVESEARCH_ITEMSEETING);
            }
            IsMouseClick = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseEnter = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseEnter = false;
            if (VisiableBoderWidth == 1 || PreviousBoderWidth == 1 || NextBoderWidth == 1)
            {
                VisiableBoderWidth = 0;
                PreviousBoderWidth = 0;
                NextBoderWidth = 0;
                this.Invalidate();
                return;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var pm = this.PointToClient(MousePosition);
            if (IsMouseDown && IsMouseEnter && MouseDownPoint.HasValue)
            {
                if (Math.Abs(MouseDownPoint.Value.X - pm.X) >= 100)
                {
                    this.Close();
                    return;
                }
            }

            if (VisiableRect.Contains(pm))
            {
                VisiableBoderWidth = VisiableBoderWidth != 0 ? VisiableBoderWidth : 1;
                PreviousBoderWidth = PreviousBoderWidth == 0 ? PreviousBoderWidth : 0;
                NextBoderWidth = NextBoderWidth == 0 ? NextBoderWidth : 0;
                this.Invalidate();
                return;
            }

            if (PreviousRect.Contains(pm))
            {
                VisiableBoderWidth = VisiableBoderWidth == 0 ? VisiableBoderWidth : 0;
                PreviousBoderWidth = PreviousBoderWidth != 0 ? PreviousBoderWidth : 1;
                NextBoderWidth = NextBoderWidth == 0 ? NextBoderWidth : 0;
                this.Invalidate();
                return;
            }

            if (NextRect.Contains(pm))
            {
                VisiableBoderWidth = VisiableBoderWidth == 0 ? VisiableBoderWidth : 0;
                PreviousBoderWidth = PreviousBoderWidth == 0 ? PreviousBoderWidth : 0;
                NextBoderWidth = NextBoderWidth != 0 ? NextBoderWidth : 1;
                this.Invalidate();
                return;
            }

            if (VisiableBoderWidth != 0 || PreviousBoderWidth != 0 || NextBoderWidth != 0)
            {
                VisiableBoderWidth = 0;
                PreviousBoderWidth = 0;
                NextBoderWidth = 0;
                this.Invalidate();
                return;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            MouseDownPoint = this.PointToClient(MousePosition);
            if (e.Button == MouseButtons.Left && IsMouseEnter)
            {
                if (VisiableRect.Contains(MouseDownPoint.Value))
                {
                    IsOtherMouseDown = 1;
                    VisiableBoderWidth = 3;
                    this.Invalidate();
                }
                else if (PreviousImage != null && PreviousRect.Contains(MouseDownPoint.Value))
                {
                    IsOtherMouseDown = 2;
                    PreviousBoderWidth = 3;
                    this.Invalidate();
                }
                else if (NextImage != null && NextRect.Contains(MouseDownPoint.Value))
                {
                    IsOtherMouseDown = 3;
                    NextBoderWidth = 3;
                    this.Invalidate();
                }

                IsMouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Point pm = this.PointToClient(MousePosition);
            if (IsOtherMouseDown != 0)
            {
                if (IsOtherMouseDown == 1 && VisiableRealRect.Contains(pm))
                {
                    Presenter.Visible = !Presenter.Visible;
                    VisiableBoderWidth = 1;
                    this.Invalidate();
                }

                if (IsOtherMouseDown == 2 && PreviousRect.Contains(pm))
                {
                    SearchApp.Default.NavigationPreviousIndex();
                    PreviousBoderWidth = 1;
                    this.Invalidate();
                }

                if (IsOtherMouseDown == 3 && NextRect.Contains(pm))
                {
                    SearchApp.Default.NavigationNextIndex();
                    NextBoderWidth = 1;
                    this.Invalidate();
                }

            }
            IsOtherMouseDown = 0;
            IsMouseDown = false;
            MouseDownPoint = null;
        }

        #endregion

        #region IChanleInfoStyle的实现
        private Color _TitleBackColor;
        private Color _TitleForeColor;
        private Color _ContentBackColor;
        private Color _ContentForeColor;
        private Color _BottomBarBackColor;
        private Int32 _BottomBarBackThickness;


        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Red"), Category("CatAppearance"), Description("Search Header BackColor")]
        public Color TitleBackColor
        {
            get => _TitleBackColor;
            set => _TitleBackColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Search Header ForeColor")]
        public Color TitleForeColor
        {
            get => _TitleForeColor;
            set => _TitleForeColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Search Content BackColor")]
        public Color ContentBackColor
        {
            get => _ContentBackColor;
            set => _ContentBackColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Search Content ForeColor")]
        public Color ContentForeColor
        {
            get => _ContentForeColor;
            set => _ContentForeColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Search Bottom Bar Color")]
        public Color BottomBarBackColor
        {
            get => _BottomBarBackColor;
            set => _BottomBarBackColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Int32), "0"), Category("CatAppearance"), Description("Search Bottom Bar BorderThickness")]
        public Int32 BottomBarBackThickness
        {
            get => Presenter?.Focused == true ? 5 : 0;
            set => _BottomBarBackThickness = value;
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

        #region 接口IPanel实现

        private Color _HeaderBackColor;
        private Color _HeaderForeColor;

        public Color HeaderBackColor
        {
            get => _HeaderBackColor;
            set => _HeaderBackColor = value;
        }

        public Color HeaderForeColor
        {
            get => _HeaderForeColor;
            set => _HeaderForeColor = value;
        }


        public Color BorderColor
        {
            get => this.BackColor;
            set => this.BackColor = value;
        }

        private Int32 _BorderThickness = 2;
        public int BorderThickness
        {
            get => _BorderThickness;
            set
            {
                _BorderThickness = value;
                this.Padding = new Padding(value, value, value, value);
            }
        }


        #endregion 接口IPanel实现

        public new Boolean Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                EventBus.EventBroker.Instance.GetEvent<SearchInfoVisibleArgs>().Publish(this, new SearchInfoVisibleArgs(value));
            }
        }
    }
}
