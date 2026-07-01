using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    [DefaultEvent("Click")]
    public partial class VoltmeterInfo : UserControl, IVoltmeterView, IChannelInfoStyle
    {
        public VoltmeterInfo(IVoltmeterPrsnt prsnt)
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            Presenter = (VoltmeterPrsnt)prsnt;
            Presenter.TryAddView(this);
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

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
                LblInfo.Click += value;
                LblCoupling.Click += value;
            }
            remove
            {
                base.Click -= value;
                LblInfo.Click -= value;
                LblCoupling.Click -= value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Channel Name String")]
        public override String Text
        {
            get => LblTitle.Text;
            set => LblTitle.Text = value;
        }

        public VoltmeterPrsnt Presenter
        {
            get;
            set;
        }

        //IVoltmeterPrsnt IVoltmeterView.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (VoltmeterPrsnt)value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (VoltmeterPrsnt)value;
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            else
                Update(presenter, propertyName);
        }

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            if (!DesignMode)
            {
                switch (propertyName)
                {
                    case nameof(Presenter.Source):
                        LblSource.Text = Presenter.Source.ToString();
                        //LblSource.ForeColor = Program.Oscilloscope.TryGetChannel(Presenter.Source, out var ap) ? ap.DrawColor : SystemColors.ControlText;
                        LblCoupling.Text = Presenter.Mode.ToString();
                        break;
                    case nameof(Presenter.Mode):
                        LblCoupling.Text = Presenter.Mode.ToString();
                        break;
                    case nameof(Presenter.DrawColor):
                        UpdateView();
                        break;
                }
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                SetContentStyle();
                SetUnfocusedTitleStyle();

                LblSource.Text = Presenter.Source.ToString();
                //LblSource.ForeColor = Program.Oscilloscope.TryGetChannel(Presenter.Source, out var ap) ? ap.DrawColor : SystemColors.ControlText;
                //if (Presenter.Active)
                LblInfo.Text = new Quantity(Presenter.StaBuffer.Current, Prefix.Milli, "V").ToString("##0.###", true);
                //else
                //{
                //    LblInfo.Text = "***";
                //}
                LblCoupling.Text = Presenter.Mode.ToString();
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

        private Bitmap _Buffer;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            SetStyle(ControlStyles.UserPaint, false);
            base.OnPaint(pevent);
            Rectangle o = pevent.ClipRectangle;
            Graphics.FromImage(_Buffer).Clear(BackColor);
            if (o.Width > 0 && o.Height > 0)
                DrawToBitmap(_Buffer, new Rectangle(0, 0, o.Width, o.Height));
            pevent.Graphics.DrawImageUnscaled(_Buffer, 0, 0);
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _Buffer = new Bitmap(Width, Height);
        }

        private void LblHeader_Click(object sender, EventArgs e)
        {
            Presenter.Active = false;
            (ParentForm as DsoForm).RemoveWaveformUI(Presenter);
        }

        private void DVMInfo_Click(object sender, EventArgs e)
        {
            //(ParentForm as DsoForm).CreateDvmForm(Presenter);
        }

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }

            void TickEvent()
            {
                LblInfo.Text = new Quantity(Presenter.StaBuffer.Current, Prefix.Milli, "V").ToString("##0.###", true);
            }
        }

        #region IChanleInfoStyle的实现

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Red"), Category("CatAppearance"), Description("Channel Header BackColor")]
        public Color TitleBackColor
        {
            get => LblTitle.BackColor;
            set
            {
                LblTitle.BackColor = value;
                LblSource.BackColor = value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Channel Header ForeColor")]
        public Color TitleForeColor
        {
            get => LblTitle.ForeColor;
            set
            {
                LblTitle.ForeColor = value;
                LblSource.ForeColor = value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Channel Content BackColor")]
        public Color ContentBackColor
        {
            get => LblInfo.BackColor;
            set
            {
                LblInfo.BackColor = value;
                LblCoupling.BackColor = value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("CatAppearance"), Description("Channel Content ForeColor")]
        public Color ContentForeColor
        {
            get => LblInfo.ForeColor;
            set
            {
                LblInfo.ForeColor = value;
                LblCoupling.ForeColor = value;
            }
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Color), "Gray"), Category("CatAppearance"), Description("Channel Bottom Bar Color")]
        public Color BottomBarBackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(typeof(Int32), "0"), Category("CatAppearance"), Description("Channel Bottom Bar BorderThickness")]
        public int BottomBarBackThickness
        {
            get => base.Padding.Bottom;
            set => base.Padding = new Padding(0, 0, 0, value);
        }

        public void SetContentStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetContentStyle();
        }

        public void SetFocusedTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetFocusedTitleStyle(Presenter.DrawColor);
        }

        public void SetUnfocusedTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetUnfocusedTitleStyle(Presenter.DrawColor);
        }

        public void SetDeactiveTitleStyle()
        {
            ((IChannelInfoStyle)this).DefaultSetDeactiveTitleStyle();
        }


        #endregion IChanleInfoStyle的实现
    }
}
