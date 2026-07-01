using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public enum ReactType
    {
        /// <summary>
        /// 未选择
        /// </summary>
        None,

        /// <summary>
        /// Zoom缩放
        /// </summary>
        Zoom,

        /// <summary>
        /// 区域触发--相交
        /// </summary>
        VisualTrigIntersect,

        /// <summary>
        /// 区域触发--不相交
        /// </summary>
        VisualTrigNotIntersect,

        /// <summary>
        /// 区域直方图
        /// </summary>
        Histogram,

        /// <summary>
        /// 保存截图数据
        /// </summary>
        SaveData

    }


    public partial class DrawRectangleConfirmDialog : FloatForm, IView
    {
        public DrawRectangleConfirmDialog()
        {
            InitializeComponent();
            InitLanguage();
            BtnHistogram.Font = AppStyleConfig.DefaultButtonFont;
            BtnHistogram.ForeColor = Color.White;
            BtnHistogram.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveScreenData.Font = AppStyleConfig.DefaultButtonFont;
            BtnSaveScreenData.ForeColor = Color.White;
            BtnSaveScreenData.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrigger.Font = AppStyleConfig.DefaultButtonFont;
            BtnTrigger.ForeColor = Color.White;
            BtnTrigger.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTriggerNotInterSect.Font = AppStyleConfig.DefaultButtonFont;
            BtnTriggerNotInterSect.ForeColor = Color.White;
            BtnTriggerNotInterSect.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnZoom.Font = AppStyleConfig.DefaultButtonFont;
            BtnZoom.ForeColor = Color.White;
            BtnZoom.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged -= Timebase_PublisherChanged;
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged += Timebase_PublisherChanged;
            IsShowHelp = false;
            ShowHead = false;
            this.HelpClick += (_, _) => HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(DrawRectangleConfirmDialog)));
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void Timebase_PublisherChanged(object sender, CustomEventArg e)
        {
            if (e != null && (e.Message == "StorageMode" || e.Message == "SamplingScale"))
            {
                InitTriggerBtn();
            }
        }

        private void InitTriggerBtn() => BtnTriggerNotInterSect.Enabled = BtnTrigger.Enabled = DsoPrsnt.DefaultDsoPrsnt?.Timebase?.StorageMode == AnaChnlStorageMode.Fast && (DsoPrsnt.DefaultDsoPrsnt!.Timebase!.ScaleIndex < AnaChnlTimebaseIndex.Lv50m);

        private void InitLanguage()
        {
            //var trigMsg = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("VisualTrigger");
            //var msgintersect = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuNei");
            //var msgNotIntersect = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuWai");
            BtnZoom.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AreaZoom");
            BtnTrigger.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AreaTriggerIntersect");
            BtnTriggerNotInterSect.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AreaTriggerNotIntersect");
            BtnHistogram.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutNormalMani");
            BtnSaveScreenData.Text= ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunJieQuShuJu");
            // BtnZoom.TextInfo = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("");
        }

        public ReactType Result { get; set; } = ReactType.None;

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
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
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
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged -= Timebase_PublisherChanged;
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            Style();
            base.OnLoad(e);
            InitTriggerBtn();
        }

        private void Style()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            IsShowPin = false;
            IsShowClose = false;
            HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        private void BtnZoom_Click(object sender, EventArgs e)
        {
            Result = ReactType.Zoom;
            this.Close();
        }

        private void BtnTrigger_Click(object sender, EventArgs e)
        {
            Result = ReactType.VisualTrigIntersect;
            this.Close();
        }

        private void BtnHistogram_Click(object sender, EventArgs e)
        {
            Result = ReactType.Histogram;
            this.Close();
        }
        private void BtnSaveScreenData_Click(object sender, EventArgs e)
        {
            Result = ReactType.SaveData;
            this.Close();
        }

        private void BtnTriggerNotInterSect_Click(object sender, EventArgs e)
        {
            Result = ReactType.VisualTrigNotIntersect;
            this.Close();
        }
    }
}
