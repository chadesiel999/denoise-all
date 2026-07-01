using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ArticialIntelligenceSetInfoStrip : UserControl, IPanel, IArtificialIntelligenceView
    {
        private const int HEADER_HEIGHT = 36;  // 标题栏高度
        private const int ROW_HEIGHT = 24;     // 每行高度
        private const int MAX_VISIBLE_ROWS = 12; // 最多显示行数
        private const int MIN_VISIBLE_ROWS = 1;  // 最少显示行数
        private const int MARGIN_TOP = 40;      // 上边距
        private const int MARGIN_BOTTOM = 7;    // 下边距

        public ArticialIntelligenceSetInfoStrip()
        {
            base.Visible = false;
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            
            // 添加关闭按钮悬停效果
            LblClose.MouseEnter += (s, e) => 
            {
                LblClose.ForeColor = Color.White;
                LblClose.BackColor = Color.FromArgb(60, 60, 60);
            };
            LblClose.MouseLeave += (s, e) => 
            {
                LblClose.ForeColor = Color.FromArgb(180, 180, 180);
                LblClose.BackColor = Color.FromArgb(41, 42, 45);
            };
            
            // 添加尺寸改变事件处理
            this.SizeChanged += ArticialIntelligenceSetInfoStrip_SizeChanged;
            this.Load += (s, e) => ArticialIntelligenceSetInfoStrip_SizeChanged(s, e);
        }

        private void ArticialIntelligenceSetInfoStrip_SizeChanged(object sender, EventArgs e)
        {
            // 动态调整列宽以适应控件宽度
            if (LveMain.Columns.Count > 0)
            {
                // 减去左右边距（7px * 2 = 14px）
                LveMain.Columns[0].Width = LveMain.Width - 6;
            }
        }

        /// <summary>
        /// 根据信息条数动态调整窗口高度
        /// </summary>
        private void AdjustHeight(int itemCount)
        {
            if (itemCount < MIN_VISIBLE_ROWS)
                itemCount = MIN_VISIBLE_ROWS;

            // 计算显示的行数（限制最大值）
            int visibleRows = Math.Min(itemCount, MAX_VISIBLE_ROWS);
            
            // 计算控件总高度 = 标题栏高度 + 行数 * 行高 + 下边距
            int newHeight = HEADER_HEIGHT + MARGIN_TOP - HEADER_HEIGHT + (visibleRows * ROW_HEIGHT) + MARGIN_BOTTOM;
            
            // 设置控件高度
            this.Height = newHeight;
        }

        public ArtificialIntelligencePrsnt Presenter
        {
            get;
            set;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArtificialIntelligencePrsnt)value;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (IsDisposed || Disposing)
                return;
            if (!IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
                }
                catch (InvalidOperationException)
                {
                    // Handle may be unavailable during create/dispose window transitions.
                }
            }
            else
            {
                Update(prsnt, propertyName);
            }
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
                    case nameof(Presenter.AiSetEnable):
                        Visible = Presenter.AiSetEnable;
                        if (Visible)
                        {
                            TimerMain.Enabled = true;
                            TimerMain.Start();
                        }
                        else
                        {
                            TimerMain.Enabled = false;
                            TimerMain.Stop();
                        }
                        break;
                }
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Presenter == null)
                    return;
                Visible = Presenter.AiSetEnable;

                LveMain.BeginUpdate();
                String[] tipinfo = Presenter.AiSetInfo;
                
                // 清空现有项目
                LveMain.Items.Clear();
                
                // 添加新的提示信息，过滤掉空字符串
                int validCount = 0;
                for (Int32 row = 0; row < tipinfo.Length; row++)
                {
                    if (!String.IsNullOrWhiteSpace(tipinfo[row]))
                    {
                        LveMain.Items.Add(new ListViewItem(tipinfo[row]));
                        validCount++;
                    }
                }
                
                LveMain.EndUpdate();
                
                // 根据信息条数调整高度
                AdjustHeight(validCount);
                
                // 如果信息超过最大显示行数，自动滚动到最后一项
                if (validCount > MAX_VISIBLE_ROWS && LveMain.Items.Count > 0)
                {
                    LveMain.EnsureVisible(LveMain.Items.Count - 1);
                }
                
                // 确保列宽正确
                ArticialIntelligenceSetInfoStrip_SizeChanged(this, EventArgs.Empty);
            }
        }

        private volatile Boolean _IsUpdating = false;
        private void TimerMain_Tick(object sender, EventArgs e)
        {
            if (_IsUpdating)
                return;
            _IsUpdating = true;
            try
            {
                UpdateView();
            }
            finally
            {
                _IsUpdating = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void LblClose_Click(object sender, EventArgs e)
        {
            if (Presenter != null)
            {
                Presenter.AiSetEnable = false;
            }
        }

        public new Boolean Visible
        {
            get => base.Visible;
            set
            {
                _DesiredVisible = value;
                if (IsDisposed || Disposing)
                    return;

                DsoForm dsoForm = Program.Oscilloscope?.View as DsoForm;
                Control uiInvoker = dsoForm as Control ?? this;
                if (uiInvoker.IsDisposed)
                    return;
                if (!uiInvoker.IsHandleCreated)
                {
                    EventHandler? onHandleCreated = null;
                    onHandleCreated = (s, e) =>
                    {
                        uiInvoker.HandleCreated -= onHandleCreated;
                        if (!uiInvoker.IsDisposed && !IsDisposed && !Disposing)
                            ApplyVisibleState(_DesiredVisible);
                    };
                    uiInvoker.HandleCreated += onHandleCreated;
                    return;
                }

                if (uiInvoker.InvokeRequired)
                {
                    try
                    {
                        uiInvoker.BeginInvoke(new Action(() => ApplyVisibleState(_DesiredVisible)));
                    }
                    catch
                    { }
                    return;
                }

                ApplyVisibleState(_DesiredVisible);
            }
        }
        private volatile Boolean _DesiredVisible = false;
        private void ApplyVisibleState(Boolean value)
        {
            if (IsDisposed || Disposing)
                return;
            if (base.Visible != value)
                base.Visible = value;
            try
            {
                DsoForm dsoForm = Program.Oscilloscope?.View as DsoForm;
                if (dsoForm?.PanelManager == null)
                    return;
                Boolean inPanelManager = dsoForm.PanelManager.Contains(this);
                if (value && !inPanelManager)
                    dsoForm.PanelManager.Add(this);
                else if (!value && inPanelManager)
                    dsoForm.PanelManager.Remove(this);
            }
            catch
            { }
        }
        public Color HeaderBackColor { get => LveMain.BackColor; set => LveMain.BackColor = value; }
        public Color HeaderForeColor { get => LveMain.ForeColor; set => LveMain.ForeColor = value; }
        public Color ContentBackColor { get => LveMain.BackColor; set => LveMain.BackColor = value; }
        public Color ContentForeColor { get => LveMain.ForeColor; set => LveMain.ForeColor = value; }
        public Color BorderColor
        {
            get => LveMain.ForeColor; set => LveMain.ForeColor = value;
        }
        public int BorderThickness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
