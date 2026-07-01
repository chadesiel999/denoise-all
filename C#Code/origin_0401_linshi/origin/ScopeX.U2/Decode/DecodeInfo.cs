using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using static ScopeX.ComModel.ProtocolARINC429;

namespace ScopeX.U2
{

    [DefaultEvent("Click")]
    public partial class DecodeInfo : UserControl, IChnlView, IProtocolView, IChannelInfoStyle
    {
        public ChannelId Id { get; set; }

        public DecodeInfo(IChnlPrsnt prsnt)
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            DecodePresenter = (DecodePrsnt)prsnt;
            Presenter.TryAddView(this);
            DecodePresenter.TryAddView(this);
            (Program.Oscilloscope.View as DsoForm).TryAddDecodeView(DecodePresenter);
            (Program.Oscilloscope.View as DsoForm).TryAddProtocolView(Presenter);
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            this.Refresh();
        }

        public void ReLoadSource()
        {
            this.Refresh();
        }
        public void Reload()
        {
            this.Refresh();
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
                CreateParams parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }

        /// <summary>
        /// 显示参数窗口
        /// </summary>
        /// <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
        internal void ShowForm()
        {
            Clicked();
        }

        private void Clicked()
        {
            //if (DecodePresenter.Active)
            //    DsoPrsnt.FocusId = DecodePresenter.Id;
            DsoPrsnt.FocusId = DecodePresenter.Id;
            (ParentForm as DsoForm).MakeOperateForm(DecodePresenter.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
            {
                DecodeForm_ = new DecodeForm()
                {
                    Id = DecodePresenter.Id,
                    DecodePresenter = DecodePresenter,
                    Anchor = AnchorStyles.Bottom,
                    Text = DecodePresenter.Name,
                };
                _ = DecodeForm_.DecodePresenter.TryAddView(DecodeForm_);

                DecodeForm_.FormClosed += (s, e) =>
                {
                    DecodeForm_.Dispose();
                    Focus();
                };
                return DecodeForm_;
            });

        }

        private Rectangle GetRowRectangle(Int32 rowIndex = 0)
        {
            int rowscount = 3;  //内容行的数量

            if (DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.CAN_FD || DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.SPI || (DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.ARINC429 && (DecodePresenter.DecodeChPrsnt as ARINC429DecodePrsnt).InputMode == ProtocolARINC429.InputMode.Diff))
                rowscount = 4;
            int offset = 2;   //内容的偏移
            int titleheight = ChannelInfoStyleDefine.TitleHeight - 5;   //标题栏高度
            int contentheight = Height - titleheight - BottomBarBackThickness;   //内容区域高度

            rowIndex = Compare.CheckRange(rowIndex, rowscount - 1, 0);
            return new Rectangle(
                offset,
                titleheight + (contentheight / rowscount * rowIndex) + offset,
                Width - offset * 2,
                (contentheight / rowscount) - offset * 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //<Remark>更改人：彭博 创建日期：2024/2/23 9:26:00  原因：此设置将导致设备的绘制颜色不起效果，并且会一直触发重绘 </Remark>
            //DecodePresenter.DrawColor = DsoPrsnt.GetDrawColor(DecodePresenter.Id);
            TitleBackColor = DecodePresenter.DrawColor;
            if (DsoPrsnt.FocusId == DecodePresenter.Id)
                SetFocusedTitleStyle();
            else
                SetUnfocusedTitleStyle();
            int titleheight = ChannelInfoStyleDefine.TitleHeight;

            SetContentStyle();

            var bkc = ContentBackColor;
            if (Focused && ShowFocusCues)
            {
                bkc = ControlPaint.Light(bkc, 0.3F);
            }
            e.Graphics.Clear(bkc);

            //画标题栏内容
            e.Graphics.FillRectangle(new SolidBrush(TitleBackColor), new Rectangle(0, 0, Width, titleheight));
            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                ChannelInfoStyleDefine.BoldFont,
                new Rectangle(0, 0, Width, titleheight), Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            //画协议类型
            TextRenderer.DrawText(
                e.Graphics,
                $"{DecodePresenter.DecodeChPrsnt.ProtocolType.GetDisplay()}({DecodePresenter.Format.GetDisplay()})",
                ChannelInfoStyleDefine.BoldFont,
                new Rectangle(0, 0, Width, titleheight),
                ContentForeColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

            //画协议类型
            Protocol.DecodeView decodeView = DecodeApp.Default.DecodeViews.
                FirstOrDefault(x => x.ProtocolType == DecodePresenter.DecodeChPrsnt.ProtocolType);
            if (decodeView != null)
            {
                decodeView.GetRowRectangleDelegate = GetRowRectangle;
            }
            //小字体协议
            if (DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.CAN_FD || DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.SPI || (DecodePresenter.DecodeChPrsnt.ProtocolType == SerialProtocolType.ARINC429&& (DecodePresenter.DecodeChPrsnt as ARINC429DecodePrsnt).InputMode == ProtocolARINC429.InputMode.Diff))
            {
                decodeView?.DrawDecodeInfo(
                    e.Graphics,
                    e.ClipRectangle,
                    DecodePresenter.DecodeChPrsnt,
                    ContentForeColor,
                    ChannelInfoStyleDefine.Smallfont);
            }
            else
            {
                decodeView?.DrawDecodeInfo(
                    e.Graphics,
                    e.ClipRectangle,
                    DecodePresenter.DecodeChPrsnt,
                    ContentForeColor,
                    ChannelInfoStyleDefine.NormalFont);
            }

            //画BottomBar
            e.Graphics.FillRectangle(
                new SolidBrush(BottomBarBackColor),
                new Rectangle(0, Height - BottomBarBackThickness, Width, BottomBarBackThickness));
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


        public DecodePrsnt DecodePresenter { get; set; }
        IBadge IView<IBadge>.Presenter { get => DecodePresenter; set => DecodePresenter = value as DecodePrsnt; }
        public IProtocolPrsnt Presenter { get => DecodePresenter.DecodeChPrsnt; set => _ = value; }
        private DecodeForm DecodeForm_;

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
            if (!DesignMode)
            {

                switch (propertyName)
                {
                    case nameof(DsoPrsnt.FocusId):
                        //if (DsoPrsnt.FocusId == DecodePresenter.Id)
                        //    SetFocusedTitleStyle();
                        //else
                        //    SetUnfocusedTitleStyle();
                        break;
                    case nameof(DecodePresenter.Active):
                        Presenter.TryRemoveView(this);
                        DecodePresenter.TryRemoveView(this);
                        (Program.Oscilloscope.View as DsoForm).TryRemoveDecodeView(DecodePresenter);
                        (Program.Oscilloscope.View as DsoForm).TryRemoveProtocolView(Presenter);
                        (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI(DecodePresenter);
                        break;
                    case nameof(DecodePresenter.WindowId):
                        (Program.Oscilloscope.View as DsoForm).ChangeWaveformFig(DecodePresenter);
                        break;
                    //<Remark>更改人：彭博 创建日期：2024/2/23 9:26:00  原因：设置绘制颜色 </Remark>
                    case nameof(DecodePresenter.ProtocolType):
                    case nameof(DecodePresenter.DrawColor):
                        Refresh();
                        break;
                }

                //Fresh Binding window title
                //var busform = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetWindow(DecodePresenter.WindowId);

                //if (busform != null && busform.IsMainForm == false)
                //{
                //    busform.Title = DecodePresenter.Id.ToString() + " : " + DecodePresenter.DecodeChPrsnt.ProtocolType.ToString();
                //}
                //else
                //{

                //}
                if (propertyName == nameof(DecodePresenter.ProtocolType))
                {
                    if (DecodeForm_ != null && !DecodeForm_.IsDisposed)
                    {
                        DecodeForm_.SelectValueChangeUpdate(DecodePresenter.ProtocolType);
                    }
                }
                (Program.Oscilloscope.View as DsoForm).UpdateFigTitle(DecodePresenter, DecodePresenter.Id.ToString() + " : " + DecodePresenter.DecodeChPrsnt.ProtocolType.ToString());
                Refresh();

            }
        }

        public void UpdateThresholdUnit()
        {
            Refresh();
        }

        //protected override void OnMouseClick(MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //        return;
        //    Point point= this.PointToClient(MousePosition);

        //    if(point.Y <= ChannelInfoStyleDefine.TitleHeight)
        //        OnTitleClicked(EventArgs.Empty);
        //    else
        //        Clicked();
        //}

        protected override void OnClick(EventArgs e)
        {
            //!!!Disable UserControl's Click event when mouse right button is click.
            if (e is MouseEventArgs mea && mea.Button == MouseButtons.Right)
            {
                return;
            }

            if (PointToClient(MousePosition).Y <= ChannelInfoStyleDefine.TitleHeight)
                OnTitleClicked(EventArgs.Empty);
            else
                Clicked();
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

        private void OnTitleClicked(EventArgs e)
        {
            if (DsoPrsnt.FocusId != DecodePresenter.Id)
            {
                DsoPrsnt.FocusId = DecodePresenter.Id;
            }
            else
            {
                DecodePresenter.Active = false;
                //(Program.Oscilloscope.View as DsoForm).RemoveBadge(DecodePresenter);
            }

            TitleClicked?.Invoke(this, e);
        }
        public event EventHandler TitleClicked;


        #region IChanleInfoStyle的实现

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Red"), Category("CatAppearance"), Description("Channel Header BackColor")]
        public Color TitleBackColor
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Channel Header ForeColor")]
        public Color TitleForeColor
        {
            get;
            set;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Channel Content BackColor")]
        public Color ContentBackColor
        {
            get;
            set;
        } = Color.FromArgb(32, 32, 32);

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
        public int BottomBarBackThickness
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
            ((IChannelInfoStyle)this).DefaultSetFocusedTitleStyle(DecodePresenter.DrawColor);
        }

        public void SetUnfocusedTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetUnfocusedTitleStyle(DecodePresenter.DrawColor);
        }

        public void SetDeactiveTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetDeactiveTitleStyle();
        }


        #endregion IChanleInfoStyle的实现

        //public void SetToolTip(ToolTip toolTip)
        //{
        //    toolTip.SetToolTip(this, Properties.ToolTips.AboutBusBadge);
        //}
    }
}
