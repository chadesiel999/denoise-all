
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class DsoForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DsoForm));
            TlpMain = new TableLayoutPanel();
            DsoInfoStrip = new DsoBtmStrip();
            DsoTopStrip = new DsoTopStrip();
            DsoResultStrip = new DsoResultStrip();
            WindowDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            MainToolTip = new ToolTip(components);
            VS2015DarkTheme = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            VS2015BlueTheme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            VS2005Theme = new WeifenLuo.WinFormsUI.Docking.VS2005Theme();
            VS2003Theme = new WeifenLuo.WinFormsUI.Docking.VS2003Theme();
            VS2015LightTheme = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            VS2013LightTheme = new WeifenLuo.WinFormsUI.Docking.VS2013LightTheme();
            VS2013BlueTheme = new WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme();
            VS2013DarkTheme = new WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme();
            VS2012LightTheme = new WeifenLuo.WinFormsUI.Docking.VS2012LightTheme();
            VS2012BlueTheme = new WeifenLuo.WinFormsUI.Docking.VS2012BlueTheme();
            VS2012DarkTheme = new WeifenLuo.WinFormsUI.Docking.VS2012DarkTheme();
            TlpMain.SuspendLayout();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.BackColor = Color.FromArgb(15, 15, 15);
            resources.ApplyResources(TlpMain, "TlpMain");
            TlpMain.Controls.Add(DsoInfoStrip, 0, 3);
            TlpMain.Controls.Add(DsoTopStrip, 0, 0);
            TlpMain.Controls.Add(DsoResultStrip, 0, 2);
            TlpMain.Controls.Add(WindowDockPanel, 0, 1);
            TlpMain.Name = "TlpMain";
            // 
            // DsoInfoStrip
            // 
            DsoInfoStrip.AllowDrop = true;
            DsoInfoStrip.BackColor = Color.FromArgb(41, 42, 45);
            TlpMain.SetColumnSpan(DsoInfoStrip, 3);
            resources.ApplyResources(DsoInfoStrip, "DsoInfoStrip");
            DsoInfoStrip.ForeColor = Color.White;
            DsoInfoStrip.MaxCount = 32;
            DsoInfoStrip.Name = "DsoInfoStrip";
            DsoInfoStrip.Load += DsoInfoStrip_Load;
            DsoInfoStrip.ControlAdded += DsoInfoStrip_ControlAdded;
            DsoInfoStrip.ControlRemoved += DsoInfoStrip_ControlRemoved;
            DsoInfoStrip.DragDrop += DsoInfoStrip_DragDrop;
            DsoInfoStrip.DragEnter += ItemTarget_DragEnter;
            // 
            // DsoTopStrip
            // 
            DsoTopStrip.artificialIntelligence = null;
            DsoTopStrip.BackColor = Color.FromArgb(15, 15, 15);
            DsoTopStrip.ChnlPresenter = null;
            TlpMain.SetColumnSpan(DsoTopStrip, 3);
            resources.ApplyResources(DsoTopStrip, "DsoTopStrip");
            DsoTopStrip.ForeColor = Color.White;
            DsoTopStrip.Id = ComModel.ChannelId.C1;
            DsoTopStrip.IsAppsFormShow = false;
            DsoTopStrip.IsSettingFormShow = false;
            DsoTopStrip.IsTriggerFormShow = false;
            DsoTopStrip.Name = "DsoTopStrip";
            DsoTopStrip.Presenter = null;
            DsoTopStrip.TmbPresenter = null;
            DsoTopStrip.TrgPresenter = null;
            // 
            // DsoResultStrip
            // 
            DsoResultStrip.BackColor = Color.FromArgb(33, 33, 40);
            TlpMain.SetColumnSpan(DsoResultStrip, 3);
            DsoResultStrip.CymometerPresenter = null;
            resources.ApplyResources(DsoResultStrip, "DsoResultStrip");
            DsoResultStrip.MeasPresenter = null;
            DsoResultStrip.Name = "DsoResultStrip";
            DsoResultStrip.VoltmeterPresenter = null;
            // 
            // WindowDockPanel
            // 
            WindowDockPanel.AllowDrop = true;
            WindowDockPanel.BackColor = Color.Black;
            TlpMain.SetColumnSpan(WindowDockPanel, 3);
            resources.ApplyResources(WindowDockPanel, "WindowDockPanel");
            WindowDockPanel.DockBottomPortion = 0.5D;
            WindowDockPanel.DockLeftPortion = 0.5D;
            WindowDockPanel.DockRightPortion = 0.5D;
            WindowDockPanel.DockTopPortion = 0.5D;
            WindowDockPanel.IsMaxWindow = true;
            WindowDockPanel.MaxWindowFlag = false;
            WindowDockPanel.Name = "WindowDockPanel";
            WindowDockPanel.DragDrop += WindowDockPanel_DragDrop;
            WindowDockPanel.DragEnter += ItemTarget_DragEnter;
            // 
            // MainToolTip
            // 
            MainToolTip.BackColor = Color.FromArgb(41, 42, 45);
            MainToolTip.ForeColor = Color.FromArgb(210, 210, 210);
            MainToolTip.OwnerDraw = true;
            MainToolTip.ShowAlways = true;
            // 
            // DsoForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 25);
            ControlBox = false;
            Controls.Add(TlpMain);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DsoForm";
            SizeGripStyle = SizeGripStyle.Hide;
            KeyDown += DsoForm_KeyDown;
            TlpMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel TlpMain;
        private DsoBtmStrip DsoInfoStrip;
        private DsoTopStrip DsoTopStrip;
        private ToolTip MainToolTip;
        private DsoResultStrip DsoResultStrip;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme VS2015DarkTheme;
        public WeifenLuo.WinFormsUI.Docking.DockPanel WindowDockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme VS2015BlueTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2005Theme VS2005Theme;
        private WeifenLuo.WinFormsUI.Docking.VS2003Theme VS2003Theme;
        private WeifenLuo.WinFormsUI.Docking.VS2015LightTheme VS2015LightTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2013LightTheme VS2013LightTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme VS2013BlueTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme VS2013DarkTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2012LightTheme VS2012LightTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2012BlueTheme VS2012BlueTheme;
        private WeifenLuo.WinFormsUI.Docking.VS2012DarkTheme VS2012DarkTheme;
    }
}

