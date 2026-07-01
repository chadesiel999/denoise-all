using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    [DefaultEvent("Click")]
    [DefaultValue("Text")]
    public partial class ExceptionCaptureInfo : UserControl, IExceptionCaptureView, IBadgeView, IChannelInfoStyle
    {
        public ExceptionCaptureInfo() : base()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //设置风格
            HeaderHeight = ChannelInfoStyleDefine.TitleHeight;
            ContentFont = ChannelInfoStyleDefine.NormalFont;
        }

        public ExceptionCapturePrsnt Presenter { get; set; }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (ExceptionCapturePrsnt)value;
        }

        IExceptionCapturePrsnt IView<IExceptionCapturePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ExceptionCapturePrsnt)value;
        }


        public void UpdateView(object prsnt, string propertyName)
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

        private void Update(Object prsnt, String propertyName)
        {
            switch (propertyName)
            {
                //!!!Remove itself from presnter
                case nameof(Presenter.Active):
                    if (!Presenter.Active)
                    {
                        Presenter.TryRemoveView(this);
                        (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI((IBadge)Presenter);
                    }
                    break;
                default:
                    this.Refresh();
                    break;
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



        [Editor(), Browsable(true), DefaultValue(50), Category("CatAppearance"), Description("The width of the row's name")]
        public Int32 NameWidth
        {
            get;
            set;
        } = 50;
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
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
                Text,
                ChannelInfoStyleDefine.BoldFont,
                new Rectangle(0, 0, Width, HeaderHeight),
                tfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            //TextRenderer.DrawText(e.Graphics,
            //    "异常捕获",
            //    ContentFont,
            //    new Rectangle(Width / 3, 0, Width / 3 * 2, HeaderHeight),
            //    tfc,
            //    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            #region 画具体信息
            //画具体信息
            Int32 h = (Height - HeaderHeight - BottomBarBackThickness) / 3;
            Int32 w = Width - NameWidth;
            Int32 y = HeaderHeight;

            //信息第一行
            TextRenderer.DrawText(
                e.Graphics,
                Presenter.ExceptionCaptureChnlId.ToString(),
                ContentFont,
                new Rectangle(0, y, Width / 2, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            //TextRenderer.DrawText(
            //    e.Graphics,
            //   new Quantity(Presenter.CaptureExceptionFrameLength, Prefix.Empty, QuantityUnit.Count).ToString("###0", true),
            //    ContentFont,
            //    new Rectangle(Width / 2, y, Width / 2, h),
            //    cfc,
            //    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            //信息第二行
            y += h;

            TextRenderer.DrawText(
                e.Graphics,
                new Quantity(Presenter.AnormalFrameCount, Prefix.Empty, QuantityUnit.Count).ToString("###0", true),
                ContentFont,
                new Rectangle(0, y, Width / 2, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(
                  e.Graphics,
                  new Quantity(Presenter.AnormalFrameID, Prefix.Empty, QuantityUnit.Count).ToString("###0", true),
                  ContentFont,
                  new Rectangle(Width / 2, y, Width / 2, h),
                  cfc,
                  TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //信息第三行
            y += h;
            TextRenderer.DrawText(
                e.Graphics,
                Presenter.ExceptionViewMode.GetDescription(),
                ContentFont,
                new Rectangle(0, y, Width * 2 / 5, h),
                cfc,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            //TextRenderer.DrawText(
            //    e.Graphics,
            //    Presenter.TemplateTriggerSource.GetDescription(),
            //    ContentFont,
            //    new Rectangle(Width * 2 / 5, y, Width * 3 / 5, h),
            //    cfc,
            //    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            #endregion

            //画BottomBar
            e.Graphics.FillRectangle(
                new SolidBrush(BottomBarBackColor),
                new Rectangle(0, Height - BottomBarBackThickness, Width, BottomBarBackThickness));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetContentStyle();
            SetUnfocusedTitleStyle();
        }

        protected override void OnClick(EventArgs e)
        {
            if (e is MouseEventArgs mea && mea.Button == MouseButtons.Right)
            {
                return;
            }
            if (PointToClient(MousePosition).Y < HeaderHeight)
            {
                Presenter.Active = false;
            }
            else
            {
                OnBodyClicked();
            }
            base.OnClick(e);
        }

        public void OnBodyClicked()
        {
            if (Program.Oscilloscope.View is DsoForm dso)
            {
                dso.MakeOperateForm(Presenter.Id.ToString(), PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
                {
                    ExceptionCaptureForm form = new(Presenter)
                    {
                        Anchor = AnchorStyles.Bottom,
                        Text ="异常捕获" ,//Presenter.Id.ToString(),
                        Presenter = Presenter
                    };
                    _ = form.Presenter.TryAddView(form);
                    form.FormClosed += (s, e) => Focus();
                    return form;
                });
            }
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
