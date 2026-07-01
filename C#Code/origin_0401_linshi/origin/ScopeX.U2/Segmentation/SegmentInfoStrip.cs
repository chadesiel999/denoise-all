// <author>LJW</author>
// <date>2022/6/29</date>

using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class SegmentInfoStrip : UserControl, IPanel, ITimebaseView
    {
        #region 构造析构
        public SegmentInfoStrip()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //设置字体
            LblTitle.Font = ChannelInfoStyleDefine.BoldFont;
            UpcCollectState.Font = ChannelInfoStyleDefine.BoldFont;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            LblTitle.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunXuMoShi");
        }
        #endregion 构造析构

        #region 属性成员
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

        public TimebasePrsnt Presenter { get; set; }
        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter { get => Presenter; set => Presenter = (TimebasePrsnt)value; }

        #endregion 属性成员

        #region 接口实现
        public Color HeaderBackColor
        {
            get => LblTitle.BackColor;
            set
            {
                LblTitle.BackColor = value;
            }
        }
        public Color HeaderForeColor
        {
            get => LblTitle.ForeColor;
            set => LblTitle.ForeColor = value;
        }

        public Color ContentBackColor
        {
            get => TlpFragment.BackColor;
            set
            {
                TlpFragment.BackColor = value;
            }
        }

        public Color ContentForeColor
        {
            get => TlpFragment.ForeColor;
            set
            {
                TlpFragment.ForeColor = value;
                TlpFragment.ForeColor = value;
            }
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

        public void UpdateView(object presenter, string propertyName)
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
        #endregion 接口实现

        #region 业务逻辑
        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
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
                    case nameof(Presenter.CollectedFrameCount):
                        if ((Program.Oscilloscope.View as DsoForm).InvokeRequired)
                        {
                            (Program.Oscilloscope.View as DsoForm).Invoke(() =>
                            {
                                lblCount.Text = $"{Presenter.CollectedFrameCount} / {Presenter.FrameCount}";
                                UpcCollectState.Value = Presenter.CollectedFrameCount;
                                UpcCollectState.MaxValue = Presenter.FrameCount;
                            });
                        }
                        else
                        {
                            lblCount.Text = $"{Presenter.CollectedFrameCount} / {Presenter.FrameCount}";
                            UpcCollectState.MaxValue = Presenter.FrameCount;
                            UpcCollectState.Value = Presenter.CollectedFrameCount;
                        }

                        break;
                    case nameof(Presenter.FrameCount):
                        UpcCollectState.MaxValue = Presenter.FrameCount;
                        lblCount.Text = $"{Presenter.CollectedFrameCount} / {Presenter.FrameCount}";
                        break;
                    case nameof(Presenter.SegmentActive):
                        if ((Program.Oscilloscope.View as DsoForm).InvokeRequired)
                        {
                            (Program.Oscilloscope.View as DsoForm).Invoke(new Action(() => Visible = Presenter.SegmentActive));
                        }
                        else
                            Visible = Presenter.SegmentActive;
                        break;
                }
            }
        }

        private String GetFrameCount(Double value)
        {
            return new Quantity(value, Prefix.Empty, QuantityUnit.Count).ToString("#0.000", true);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {

                Visible = Presenter.SegmentActive;
                UpcCollectState.MaxValue = Presenter.FrameCount;
                UpcCollectState.Value = Presenter.CollectedFrameCount;
                lblCount.Text = $"{Presenter.CollectedFrameCount} / {Presenter.FrameCount}";
            }
        }

        private void UpcCollectState_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.ACQUIRE);
        }

        public new Boolean Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                try
                {
                    if (value)
                    {
                        Presenter.TryAddView(this);
                        (Program.Oscilloscope.View as DsoForm).PanelManager.Add(this);
                    }
                    else
                    {
                        UpcCollectState.Value = 0;
                        Presenter.TryRemoveView(this);
                        (Program.Oscilloscope.View as DsoForm).PanelManager.Remove(this);
                    }
                }
                catch
                { }

            }
        }
        #endregion  业务逻辑
    }
}
