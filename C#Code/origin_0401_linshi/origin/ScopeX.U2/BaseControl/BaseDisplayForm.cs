using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using WeifenLuo.WinFormsUI.Docking;

namespace ScopeX.U2
{
    public partial class BaseDisplayForm : BorderColorForm, IDockContent
    {
        //显示窗口只有一个关闭按钮
        //显示窗口为非顶级控件
        //显示窗口可以显示波形、表格、（可作为TabControl的容器）
        //“波形视图”为常在视图(唯一)，该窗体不可关闭，需禁用关闭按钮
        //“绘图”为动态视图(数量可变)，显示窗口默认有一个关联的配置弹窗，在单击窗口时显示
        //
        //
        //
        //
        //
        //
        //
        //
        //

        public BaseDisplayForm()
        {
            m_dockHandler = new DockContentHandler(this, new GetPersistStringCallback(GetPersistString));
            m_dockHandler.DockStateChanged += new EventHandler(DockHandler_DockStateChanged);
            if (PatchController.EnableFontInheritanceFix != true)
            {
                //Suggested as a fix by bensty regarding form resize
                this.ParentChanged += new EventHandler(DockContent_ParentChanged);
            }

            InitializeComponent();
            this.TopLevel = false;
            WindowId = IdFactory.NextId;

            SizeChanged += BaseDisplayForm_SizeChanged;
        }

        private void BaseDisplayForm_SizeChanged(object sender, EventArgs e)
        {
            if ((Program.Oscilloscope.View as DsoForm).WindowDockPanel.Panes.Count  == 1 && (Program.Oscilloscope.View as DsoForm).WindowDockPanel.Panes[0].Contents.Count == 1)
            {
                if ((Program.Oscilloscope.View as DsoForm).WindowDockPanel.MaxWindowFlag)
                    return;
                //(Program.Oscilloscope.View as DsoForm)
                (Program.Oscilloscope.View as DsoForm).WindowDockPanel.Theme.ColorPalette.TabSelectedActive.Background = AppStyleConfig.TabSelectedInactiveBackground;
                (Program.Oscilloscope.View as DsoForm).WindowDockPanel.Theme.ColorPalette.TabSelectedActive.Text = AppStyleConfig.TabSelectedInactiveForeColor;
                this.BorderBackColor = AppStyleConfig.TabSelectedInactiveBackground;
            }
            else
            {
                (Program.Oscilloscope.View as DsoForm).WindowDockPanel.Theme.ColorPalette.TabSelectedActive.Background = AppStyleConfig.TabSelectedActiveBackground; //Color.FromArgb(0, 209, 255);
                (Program.Oscilloscope.View as DsoForm).WindowDockPanel.Theme.ColorPalette.TabSelectedActive.Text = AppStyleConfig.TabSelectedActiveForeColor; //Color.Black;
            }

        }

        public Int64? WindowId { get; set; } = null;
        public Boolean IsMaximize { get; set; } = false;
        public Boolean IsRemoveBadge { get; set; } = true;
        private Boolean _IsMainForm = false;

        public String ExtTitle { get; init; }


        /// <summary>
        /// 窗体的多语言Key
        /// </summary>
        public List<String> TitleLanugageIDs { get; set; }

        /// <summary>
        /// true为主窗口
        /// </summary>
        public Boolean IsMainForm
        {
            get
            {
                return _IsMainForm;
            }
            set
            {
                _IsMainForm = value;
                if (value)
                {
                    CanClosed = false;
                }
            }
        }
        public Boolean CanClosed { get; set; } = true;

        #region Dock
        private void DockContent_ParentChanged(object Sender, EventArgs e)
        {
            if (this.Parent != null)
                this.Font = this.Parent.Font;
        }

        private DockContentHandler m_dockHandler = null;
        [Browsable(false)]
        public DockContentHandler DockHandler
        {
            get { return m_dockHandler; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserDocking
        {
            get { return DockHandler.AllowEndUserDocking; }
            set { DockHandler.AllowEndUserDocking = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_DockAreas_Description")]
        [DefaultValue(DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float)]
        public DockAreas DockAreas
        {
            get { return DockHandler.DockAreas; }
            set { DockHandler.DockAreas = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AutoHidePortion_Description")]
        [DefaultValue(0.25)]
        public double AutoHidePortion
        {
            get { return DockHandler.AutoHidePortion; }
            set { DockHandler.AutoHidePortion = value; }
        }

        private string m_tabText = null;
        [Localizable(true)]
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabText_Description")]
        [DefaultValue(null)]
        public string TabText
        {
            get { return m_tabText; }
            set { DockHandler.TabText = m_tabText = value; }
        }

        private bool ShouldSerializeTabText()
        {
            return (m_tabText != null);
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_CloseButton_Description")]
        [DefaultValue(true)]
        public bool CloseButton
        {
            get { return DockHandler.CloseButton; }
            set { DockHandler.CloseButton = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_CloseButtonVisible_Description")]
        [DefaultValue(true)]
        public bool CloseButtonVisible
        {
            get { return DockHandler.CloseButtonVisible; }
            set { DockHandler.CloseButtonVisible = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPanel DockPanel
        {
            get { return DockHandler.DockPanel; }
            set { DockHandler.DockPanel = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockState DockState
        {
            get { return DockHandler.DockState; }
            set { DockHandler.DockState = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPane Pane
        {
            get { return DockHandler.Pane; }
            set { DockHandler.Pane = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHidden
        {
            get { return DockHandler.IsHidden; }
            set { DockHandler.IsHidden = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockState VisibleState
        {
            get { return DockHandler.VisibleState; }
            set { DockHandler.VisibleState = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFloat
        {
            get { return DockHandler.IsFloat; }
            set { DockHandler.IsFloat = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPane PanelPane
        {
            get { return DockHandler.PanelPane; }
            set { DockHandler.PanelPane = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPane FloatPane
        {
            get { return DockHandler.FloatPane; }
            set { DockHandler.FloatPane = value; }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual string GetPersistString()
        {
            return GetType().ToString();
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_HideOnClose_Description")]
        [DefaultValue(false)]
        public bool HideOnClose
        {
            get { return DockHandler.HideOnClose; }
            set { DockHandler.HideOnClose = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_ShowHint_Description")]
        [DefaultValue(DockState.Unknown)]
        public DockState ShowHint
        {
            get { return DockHandler.ShowHint; }
            set { DockHandler.ShowHint = value; }
        }

        [Browsable(false)]
        public bool IsActivated
        {
            get { return DockHandler.IsActivated; }
        }

        public bool IsDockStateValid(DockState dockState)
        {
            return DockHandler.IsDockStateValid(dockState);
        }

        /// <summary>
        /// Context menu strip.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabPageContextMenuStrip_Description")]
        [DefaultValue(null)]
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return DockHandler.TabPageContextMenuStrip; }
            set { DockHandler.TabPageContextMenuStrip = value; }
        }

        void IContextMenuStripHost.ApplyTheme()
        {
            DockHandler.ApplyTheme();

            if (DockPanel != null)
            {
                if (MainMenuStrip != null)
                    DockPanel.Theme.ApplyTo(MainMenuStrip);
                if (ContextMenuStrip != null)
                    DockPanel.Theme.ApplyTo(ContextMenuStrip);
            }
        }

        [Localizable(true)]
        [Category("Appearance")]
        [LocalizedDescription("DockContent_ToolTipText_Description")]
        [DefaultValue(null)]
        public string ToolTipText
        {
            get { return DockHandler.ToolTipText; }
            set { DockHandler.ToolTipText = value; }
        }
        private List<TitleButtonInfo> _ButtonSource = new List<TitleButtonInfo>();

        public List<TitleButtonInfo> ButtonSource
        {
            get
            {
                return _ButtonSource;
            }
            set
            {
                _ButtonSource = value;
            }
        }

        public Boolean IsRender { get; set; } = true;

        public new void Activate()
        {
            DockHandler.Activate();
        }

        public new void Hide()
        {
            DockHandler.Hide();
        }

        public new void Show()
        {
            DockHandler.Show();
        }

        public void Show(DockPanel dockPanel)
        {
            DockHandler.Show(dockPanel);
        }

        public void Show(DockPanel dockPanel, DockState dockState)
        {
            DockHandler.Show(dockPanel, dockState);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
        {
            DockHandler.Show(dockPanel, floatWindowBounds);
        }

        public void Show(DockPane pane, IDockContent beforeContent)
        {
            DockHandler.Show(pane, beforeContent);
        }

        public void Show(DockPane previousPane, DockAlignment alignment, double proportion)
        {
            DockHandler.Show(previousPane, alignment, proportion);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void FloatAt(Rectangle floatWindowBounds)
        {
            DockHandler.FloatAt(floatWindowBounds);
        }

        public void DockTo(DockPane paneTo, DockStyle dockStyle, int contentIndex)
        {
            DockHandler.DockTo(paneTo, dockStyle, contentIndex);
        }

        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            DockHandler.DockTo(panel, dockStyle);
        }

        #region IDockContent Members
        void IDockContent.OnActivated(EventArgs e)
        {
            if ((Program.Oscilloscope.View as DsoForm).WindowDockPanel.Panes.Count > 1)
            {
                this.BorderBackColor = AppStyleConfig.SelectedActiveBorderBackColor;//Color.FromArgb(0, 85, 104);
            }
            this.OnActivated(e);
        }

        void IDockContent.OnDeactivate(EventArgs e)
        {
            this.BorderBackColor = AppStyleConfig.SelectedInactiveBorderBackColor;//Color.FromArgb(38, 38, 46);
            this.OnDeactivate(e);
        }

        #endregion IDockContent Members

        #region Events

        private void DockHandler_DockStateChanged(object sender, EventArgs e)
        {
            OnDockStateChanged(e);
        }

        private static readonly object DockStateChangedEvent = new object();

        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("Pane_DockStateChanged_Description")]
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(DockStateChangedEvent, value); }
            remove { Events.RemoveHandler(DockStateChangedEvent, value); }
        }

        protected virtual void OnDockStateChanged(EventArgs e)
        {
            ((EventHandler)Events[DockStateChangedEvent])?.Invoke(this, e);
        }

        #endregion Events

        ///// <summary>
        ///// Overridden to avoid resize issues with nested controls
        ///// </summary>
        ///// <remarks>
        ///// http://blogs.msdn.com/b/alejacma/archive/2008/11/20/controls-won-t-get-resized-once-the-nesting-hierarchy-of-windows-exceeds-a-certain-depth-x64.aspx
        ///// http://support.microsoft.com/kb/953934
        ///// </remarks>
        //protected override void OnSizeChanged(EventArgs e)
        //{
        //    if (DockPanel != null && DockPanel.SupportDeeplyNestedContent && IsHandleCreated)
        //    {
        //        BeginInvoke((MethodInvoker)delegate
        //        {
        //            base.OnSizeChanged(e);
        //        });
        //    }
        //    else
        //    {
        //        base.OnSizeChanged(e);
        //    }
        //}

        #endregion Dock
    }
}
