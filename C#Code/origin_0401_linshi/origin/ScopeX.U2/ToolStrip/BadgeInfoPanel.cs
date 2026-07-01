using Svg;
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
using ScopeX.ComModel;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class BadgeInfoPanel : UserControl
    {
        private Point _MousePreviousPoint;

        private Int32 _ScrollVal = 1;

        public BadgeInfoPanel()
        {
            InitializeComponent();

            PnlBadge.VerticalScroll.Enabled = false;

            SetScrollArrowSvg(BtnLeftArrow, true);
            SetScrollArrowState(BtnLeftArrow, false);
            SetScrollArrowSvg(BtnRightArrow, false);
            SetScrollArrowState(BtnRightArrow, false);

            ShowArrowButton(false);

            Application.AddMessageFilter(new MouseDragMsgFilter(PnlBadge, PnlBadge_MouseUp, PnlBadge_MouseDown, PnlBadge_MouseMove, PnlBadge_MouseWheel));

            PnlBadge.SizeChanged += (_, _) =>
            {
                if (PnlBadge.Controls.Count > 0)
                {
                    PnlBadge.SuspendLayout();
                    var dist = PnlBadge.Controls.Cast<Control>().Max(o => o.Right) - PnlBadge.Controls.Cast<Control>().Min(o => o.Left) + ChildWidthInterval;
                    if ((Program.Oscilloscope.View as DsoForm).WindowState != FormWindowState.Minimized)
                        ShowArrowButton(dist > Width);
                    PnlBadge.ResumeLayout();

                    //var width = ((TableLayoutPanel)Parent).GetColumnWidths().Skip(1).Sum();
                    ChangeHScrollRange(dist);
                }
            };
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
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private Int32 _ChildWidthInterval = 5;

        [Bindable(true), DefaultValue(5), Category("CatControl"), Description("子控件宽度间隔")]
        public Int32 ChildWidthInterval
        {
            get => _ChildWidthInterval;
            set
            {
                if (value >= 0 && _ChildWidthInterval != value)
                {
                    _ChildWidthInterval = value;
                }
            }
        }

        [Bindable(true), DefaultValue(typeof(Color), "red"), Category("CatControl"), Description("控件背景色")]
        public new Color BackColor
        {
            get => TlpContainer.BackColor;
            set
            {
                TlpContainer.BackColor = value;
                PnlBadge.BackColor = value;
                base.BackColor = value;
            }
        }

        [Bindable(true), DefaultValue(0), Category("CatControl"), Description("左右箭头按钮宽度")]
        public int ScrollArrowWidth
        {
            get => BtnLeftArrow.Width;
            set
            {
                if (value >= 0 && BtnLeftArrow.Width != value)
                    BtnLeftArrow.Width = BtnRightArrow.Width = value;
            }
        }

        [Bindable(true), DefaultValue(typeof(Color), "red"), Category("CatControl"), Description("左右箭头按钮背景色")]
        public Color ScrollArrowBackColor
        {
            get => BtnRightArrow.BackColor;
            set
            {
                BtnRightArrow.BackColor = value;
                BtnLeftArrow.BackColor = value;
            }
        }

        [Bindable(true), DefaultValue(typeof(Color), "red"), Category("CatControl"), Description("左右箭头按钮鼠标进入时背景色")]
        public Color ScrollArrowMouseInBackColor
        {
            get => BtnRightArrow.MouseinBackColor;
            set
            {
                BtnRightArrow.MouseinBackColor = value;
                BtnLeftArrow.MouseinBackColor = value;
                BtnRightArrow.PressedBackColor = value;
                BtnLeftArrow.PressedBackColor = value;
            }
        }

        public IBadgeView GetBadge(ChannelId id)
        {
            return PnlBadge.Controls.Cast<IBadgeView>().FirstOrDefault(ctrl => ctrl.Presenter.Id == id);
        }

        public IBadgeView GetBadge(IBadge badge)
        {
            return PnlBadge.Controls.Cast<IBadgeView>().FirstOrDefault(ctrl => ctrl.Presenter == badge);
        }

        public void AddBadge(Control badge)
        {
            if (badge is not IBadgeView bv)
                return;

            AddBadge(bv);
        }

        public IEnumerable<Control> GetBadgeControls()
        {
            if (PnlBadge.Controls == null)
                yield return null;

            foreach (Control item in PnlBadge.Controls)
                yield return item;
        }

        public void AddBadge(IBadgeView badge)
        {
            var c = badge as Control;
            Int32 dist = c.Width + ChildWidthInterval;

            Int32 pos = ChildWidthInterval;

            PnlBadge.SuspendLayout();
            if (PnlBadge.Controls.Count > 0)
            {
                pos = PnlBadge.Controls.Cast<Control>().Max(b => b.Right) + ChildWidthInterval;

                if (badge.Presenter is not null)
                {
                    var badges = PnlBadge.Controls.Cast<Control>().Where(b => (b as IBadgeView).Presenter.Id > badge.Presenter.Id);
                    if (badges.Any())
                    {
                        pos = badges.Min(b => b.Left);
                        foreach (var b in badges)
                            b.Left += dist;
                    }
                }
            }

            c.Left = pos;
            c.Top = 3;
            c.Height = PnlBadge.Height - 6;
            c.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            PnlBadge.Controls.Add(c);

            dist = PnlBadge.Controls.Cast<Control>().Max(o => o.Right) - PnlBadge.Controls.Cast<Control>().Min(o => o.Left) + ChildWidthInterval;

            if (dist > Width)
                ShowArrowButton(true);

            PnlBadge.ResumeLayout();
            ChangeHScrollRange(dist);
            SetScrollArrowState(BtnRightArrow, false);
        }

        public void RemoveBadge(ChannelId id)
        {
            var c = GetBadge(id);
            if (c is not null)
                RemoveBadge(c);
        }

        public void RemoveBadge(Control badge)
        {
            if (badge is not IBadgeView bv)
                return;

            RemoveBadge(bv);
        }

        public void RemoveBadge(IBadgeView badge)
        {
            var c = badge as Control;

            Int32 dist = c.Width + ChildWidthInterval;

            PnlBadge.SuspendLayout();
            var badges = PnlBadge.Controls.Cast<Control>().Where(o => (o as IBadgeView).Presenter.Id > badge.Presenter.Id);
            foreach (var b in badges)
                b.Left -= dist;

            PnlBadge.Controls.Remove(c);

            dist = PnlBadge.Controls.Cast<Control>().Max(o => o.Right) - PnlBadge.Controls.Cast<Control>().Min(o => o.Left) + ChildWidthInterval;

            if (dist <= Width)
                ShowArrowButton(false);

            PnlBadge.ResumeLayout();
            ChangeHScrollRange(dist);

            c.Dispose();
        }

        public void SrcollToShowCurrent()
        {
            if (BtnRightArrow.Visible&& _ScrollVal<= PnlBadge.HorizontalScroll.Maximum)
            {
                _ScrollVal= PnlBadge.HorizontalScroll.Maximum;
                PnlBadge.HorizontalScroll.Value = _ScrollVal;
                SetScrollArrowState(BtnRightArrow, true);
                SetScrollArrowState(BtnLeftArrow, false);
            }
        }

        private void ChangeHScrollRange(Int32 dist)
        {
            var width = dist - PnlBadge.Width;
            PnlBadge.HorizontalScroll.Maximum = width > 0 ? width : 100;
            PnlBadge.HorizontalScroll.Minimum = 0;
            if (_ScrollVal > PnlBadge.HorizontalScroll.Maximum)
            {
                _ScrollVal = PnlBadge.HorizontalScroll.Maximum;
                PnlBadge.HorizontalScroll.Value = _ScrollVal;
            }
        }

        private void ShowArrowButton(Boolean value)
        {
            TlpContainer.SuspendLayout();
            if (!value)
            {
                _ScrollVal = 1;
                PnlBadge.HorizontalScroll.Value = _ScrollVal;
                PnlBadge.Left = 0;
            }

            BtnLeftArrow.Visible = value;
            BtnRightArrow.Visible = value;
            TlpContainer.ResumeLayout();
        }

        private void BtnLeftArrow_Click(object sender, EventArgs e)
        {
            if (_ScrollVal > 1)
            {
                var orderbadges = PnlBadge.Controls.Cast<IBadgeView>().OrderBy(b => b.Presenter.Id);

                var badge = orderbadges.LastOrDefault(b => (b as Control).Left < ChildWidthInterval - 5) as Control;

                if (badge is not null)
                {
                    _ScrollVal += badge.Left - ChildWidthInterval;
                    if (_ScrollVal < 1)
                        _ScrollVal = 1;
                }
                else
                    _ScrollVal = 1;

                PnlBadge.HorizontalScroll.Value = _ScrollVal;
            }

            SetScrollArrowState(BtnLeftArrow, _ScrollVal <= 1);
            SetScrollArrowState(BtnRightArrow, false);
        }

        private void BtnRightArrow_Click(object sender, EventArgs e)
        {
            if (_ScrollVal < PnlBadge.HorizontalScroll.Maximum)
            {
                var orderbadges = PnlBadge.Controls.Cast<IBadgeView>().OrderBy(b => b.Presenter.Id);

                var badge = orderbadges.First(b => (b as Control).Left + 1 >= ChildWidthInterval) as Control;

                _ScrollVal += badge.Width + ChildWidthInterval;

                if (_ScrollVal > PnlBadge.HorizontalScroll.Maximum)
                    _ScrollVal = PnlBadge.HorizontalScroll.Maximum;

                PnlBadge.HorizontalScroll.Value = _ScrollVal;
            }

            SetScrollArrowState(BtnRightArrow, _ScrollVal >= PnlBadge.HorizontalScroll.Maximum);
            SetScrollArrowState(BtnLeftArrow, false);
        }

        private Rectangle _DragBox;

        private Int64 _DragDropStamp;

        private void PnlBadge_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left /*|| !BtnLeftArrow.Visible*/)
                return;

            //!!!Change focus to DsoBtmStrip
            if (ActiveControl is null)
            {
                SelectNextControl(ActiveControl, true, true, true, true);
            }

            //_MousePreviousPoint = Cursor.Position;
            _MousePreviousPoint = e.Location;

            Size ds = SystemInformation.DragSize;

            _DragBox = new Rectangle(new Point(e.X - (ds.Width / 2), e.Y - (ds.Height / 2)), ds);

            _DragDropStamp = DateTime.Now.Ticks + 10_000_000;
        }

        private void PnlBadge_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_DragDropStamp < DateTime.Now.Ticks && _DragBox.Contains(e.X, e.Y))
                {
                    _DragBox = Rectangle.Empty;
                    return;
                }

                //Int32 px = Cursor.Position.X - _MousePreviousPoint.X;
                Int32 px = e.X - _MousePreviousPoint.X;

                if (px == 0)
                    return;

                _ScrollVal -= px;
                if (_ScrollVal < 1)
                    _ScrollVal = 1;
                else if (_ScrollVal > PnlBadge.HorizontalScroll.Maximum)
                    _ScrollVal = PnlBadge.HorizontalScroll.Maximum;

                PnlBadge.HorizontalScroll.Value = _ScrollVal;
                _MousePreviousPoint = e.Location;// Cursor.Position;

                SetScrollArrowState(BtnRightArrow, _ScrollVal >= PnlBadge.HorizontalScroll.Maximum);
                SetScrollArrowState(BtnLeftArrow, _ScrollVal <= 1);
            }
        }

        private void PnlBadge_MouseUp(object sender, MouseEventArgs e)
        {
            //if (IsDragging)
            //    IsDragging = false;

            _DragBox = Rectangle.Empty;
        }

        private void PnlBadge_MouseWheel(object sender, MouseEventArgs e)
        {
            Int32 px = e.Delta / 3;

            if (px == 0)
                return;

            _ScrollVal -= px;
            if (_ScrollVal < 1)
                _ScrollVal = 1;
            else if (_ScrollVal > PnlBadge.HorizontalScroll.Maximum)
                _ScrollVal = PnlBadge.HorizontalScroll.Maximum;

            PnlBadge.HorizontalScroll.Value = _ScrollVal;
            _MousePreviousPoint = Cursor.Position;

            SetScrollArrowState(BtnRightArrow, _ScrollVal >= PnlBadge.HorizontalScroll.Maximum);
            SetScrollArrowState(BtnLeftArrow, _ScrollVal <= 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PnlBadge.HorizontalScroll.Maximum = PnlBadge.Width;
        }

        private static void SetScrollArrowSvg(ScopeXIconButton btn, Boolean isLeft)
        {
            Single widthscale = 0.6F;
            btn.IconSize = new Size((Int32)(btn.Width * widthscale), btn.Height);
            btn.IconOffset = (Int32)(btn.Width * (1 - widthscale) / 2);

            String svgstring;

            if (isLeft)
                svgstring = Properties.Resources.ChnlInfoPanelleftArrowSvg;
            else
                svgstring = Properties.Resources.ChnlInfoPanelrightArrowSvg;

            btn.SVGPath = svgstring;
        }

        private static void SetScrollArrowState(ScopeXIconButton arrow, Boolean isEnd)
        {
            var color = isEnd ? Color.Gray : Color.White;
            if (arrow.SVGForeColor != color)
            {
                arrow.SVGForeColor = color;
                arrow.MouseinSvgForeColor = color;
                arrow.PressedSvgForeColor = color;
            }
        }

    }
}
