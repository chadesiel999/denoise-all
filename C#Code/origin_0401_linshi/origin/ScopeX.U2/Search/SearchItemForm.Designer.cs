
using System;

namespace ScopeX.U2.Search
{
    partial class SearchItemForm
    {

        public static int Width = 530;
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            TlpOptions = new System.Windows.Forms.TableLayoutPanel();
            PnlTopBadge = new System.Windows.Forms.Panel();
            ChkShowEvent = new UserControls.ScopeXSwitchButton();
            LblActive = new UserControls.ScopeXLabel();
            ChkActive = new UserControls.ScopeXSwitchButton();
            LbType = new UserControls.ScopeXLabel();
            LBShowEvent = new UserControls.ScopeXLabel();
            CbxType = new ScopeX.UserControls.SelectComboBox();
            LblMarkerDisplay = new UserControls.ScopeXLabel();
            ChkMarkerDisplay = new UserControls.ScopeXSwitchButton();
            PnlBottomBadge = new System.Windows.Forms.Panel();
            BtnCopyFromTrigger = new UserControls.ScopeXIconButton();
            BtnCopyToTrigger = new UserControls.ScopeXIconButton();
            BtnCloseCurrentItem = new UserControls.ScopeXIconButton();
            BtnCloseAll = new UserControls.ScopeXIconButton();
            TlpOptions.SuspendLayout();
            PnlTopBadge.SuspendLayout();
            PnlBottomBadge.SuspendLayout();
            SuspendLayout();
            // 
            // TlpOptions
            // 
            TlpOptions.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            TlpOptions.ColumnCount = 1;
            TlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpOptions.Controls.Add(PnlTopBadge, 0, 0);
            TlpOptions.Controls.Add(PnlBottomBadge, 0, 2);
            TlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpOptions.Location = new System.Drawing.Point(1, 45);
            TlpOptions.Name = "TlpOptions";
            TlpOptions.RowCount = 3;
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            TlpOptions.Size = new System.Drawing.Size(498, 587);
            TlpOptions.TabIndex = 5;
            // 
            // PnlTopBadge
            // 
            PnlTopBadge.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            PnlTopBadge.Controls.Add(ChkShowEvent);
            PnlTopBadge.Controls.Add(ChkActive);
            PnlTopBadge.Controls.Add(ChkMarkerDisplay);
            PnlTopBadge.Controls.Add(LbType);
            PnlTopBadge.Controls.Add(LBShowEvent);
            PnlTopBadge.Controls.Add(CbxType);
            PnlTopBadge.Controls.Add(LblActive);
            PnlTopBadge.Controls.Add(LblMarkerDisplay);
            PnlTopBadge.Dock = System.Windows.Forms.DockStyle.Top;
            PnlTopBadge.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PnlTopBadge.Location = new System.Drawing.Point(0, 0);
            PnlTopBadge.Margin = new System.Windows.Forms.Padding(0);
            PnlTopBadge.Name = "PnlTopBadge";
            PnlTopBadge.Size = new System.Drawing.Size(498, 166);
            PnlTopBadge.TabIndex = 0;
            // 
            // ChkShowEvent
            // 
            ChkShowEvent.AnimationCount = 8;
            ChkShowEvent.AnimationFunc = null;
            ChkShowEvent.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkShowEvent.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkShowEvent.BorderThickness = 0;
            ChkShowEvent.Checked = false;
            ChkShowEvent.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkShowEvent.CheckedForeColor = System.Drawing.Color.Black;
            ChkShowEvent.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkShowEvent.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkShowEvent.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkShowEvent.DropKey = System.Windows.Forms.Keys.Space;
            ChkShowEvent.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkShowEvent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkShowEvent.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkShowEvent.Height = 30;
            ChkShowEvent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkShowEvent.Location = new System.Drawing.Point(274, 35);
            ChkShowEvent.Margin = new System.Windows.Forms.Padding(0);
            ChkShowEvent.Name = "ChkShowEvent";
            ChkShowEvent.Size = new System.Drawing.Size(80, 30);
            ChkShowEvent.SliderButtonWidth = 30;
            ChkShowEvent.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkShowEvent.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkShowEvent.StylizeFlag = true;
            ChkShowEvent.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkShowEvent.TabIndex = 0;
            ChkShowEvent.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkShowEvent.UseAnimation = true;
            ChkShowEvent.CheckedChangedEvent += ChkShowEvent_CheckedChangedEvent;
            // 
            // ChkActive
            // 
            ChkActive.AnimationCount = 8;
            ChkActive.AnimationFunc = null;
            ChkActive.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderThickness = 0;
            ChkActive.Checked = false;
            ChkActive.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkActive.CheckedForeColor = System.Drawing.Color.Black;
            ChkActive.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkActive.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkActive.DropKey = System.Windows.Forms.Keys.Space;
            ChkActive.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(10, 35);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(80, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 0;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // ChkMarkerDisplay
            // 
            ChkMarkerDisplay.AnimationCount = 8;
            ChkMarkerDisplay.AnimationFunc = null;
            ChkMarkerDisplay.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkMarkerDisplay.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkMarkerDisplay.BorderThickness = 0;
            ChkMarkerDisplay.Checked = false;
            ChkMarkerDisplay.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkMarkerDisplay.CheckedForeColor = System.Drawing.Color.Black;
            ChkMarkerDisplay.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkMarkerDisplay.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkMarkerDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkMarkerDisplay.DropKey = System.Windows.Forms.Keys.Space;
            ChkMarkerDisplay.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkMarkerDisplay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkMarkerDisplay.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkMarkerDisplay.Height = 30;
            ChkMarkerDisplay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkMarkerDisplay.Location = new System.Drawing.Point(410, 35);
            ChkMarkerDisplay.Margin = new System.Windows.Forms.Padding(0);
            ChkMarkerDisplay.Name = "ChkMarkerDisplay";
            ChkMarkerDisplay.Size = new System.Drawing.Size(80, 30);
            ChkMarkerDisplay.SliderButtonWidth = 30;
            ChkMarkerDisplay.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkMarkerDisplay.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkMarkerDisplay.StylizeFlag = true;
            ChkMarkerDisplay.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkMarkerDisplay.TabIndex = 0;
            ChkMarkerDisplay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkMarkerDisplay.UseAnimation = true;
            ChkMarkerDisplay.CheckedChangedEvent += ChkMarkerDisplay_CheckedChangedEvent;
            // 
            // LbType
            // 
            LbType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LbType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LbType.BorderThickness = 0;
            LbType.CornerRadius = 0;
            LbType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LbType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LbType.HighlightPrompt = defaultHighlightPrompt1;
            LbType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LbType.Location = new System.Drawing.Point(10, 92);
            LbType.MultyLineFlag = false;
            LbType.Name = "LbType";
            LbType.Size = new System.Drawing.Size(150, 21);
            LbType.StyleFlags = UserControls.Style.StyleFlag.None;
            LbType.StylizeFlag = true;
            LbType.TabIndex = 3;
            LbType.TabStop = false;
            LbType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoLeiXing");
            LbType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LbType.Token = null;
            // 
            // LBShowEvent
            // 
            LBShowEvent.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LBShowEvent.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LBShowEvent.BorderThickness = 0;
            LBShowEvent.CornerRadius = 0;
            LBShowEvent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LBShowEvent.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LBShowEvent.HighlightPrompt = defaultHighlightPrompt2;
            LBShowEvent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LBShowEvent.Location = new System.Drawing.Point(274, 7);
            LBShowEvent.MultyLineFlag = false;
            LBShowEvent.Name = "LBShowEvent";
            LBShowEvent.Size = new System.Drawing.Size(100, 21);
            LBShowEvent.StyleFlags = UserControls.Style.StyleFlag.None;
            LBShowEvent.StylizeFlag = true;
            LBShowEvent.TabIndex = 1;
            LBShowEvent.TabStop = false;
            LBShowEvent.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianLieBiao");
            LBShowEvent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LBShowEvent.Token = null;
            // 
            // CbxType
            // 
            CbxType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            CbxType.Height = 30;
            CbxType.Items = new string[]
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Edge"),
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MaiKuan")
            };
            CbxType.Location = new System.Drawing.Point(11, 120);
            CbxType.Name = "CbxType";
            CbxType.Size = new System.Drawing.Size(183, 30);
            CbxType.StyleFlags = UserControls.Style.StyleFlag.None;
            CbxType.StylizeFlag = true;
            CbxType.TabIndex = 1;
            CbxType.SelectedIndexChanged += CbxType_SelectedIndexChanged;
            // 
            // LblActive
            // 
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt1;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(10, 7);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(100, 21);
            LblActive.StyleFlags = UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 1;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuo");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
            // LblMarkerDisplay
            // 
            LblMarkerDisplay.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblMarkerDisplay.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblMarkerDisplay.BorderThickness = 0;
            LblMarkerDisplay.CornerRadius = 0;
            LblMarkerDisplay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMarkerDisplay.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMarkerDisplay.HighlightPrompt = defaultHighlightPrompt1;
            LblMarkerDisplay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMarkerDisplay.Location = new System.Drawing.Point(410, 7);
            LblMarkerDisplay.MultyLineFlag = false;
            LblMarkerDisplay.Name = "LblMarkerDisplay";
            LblMarkerDisplay.Size = new System.Drawing.Size(100, 21);
            LblMarkerDisplay.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMarkerDisplay.StylizeFlag = true;
            LblMarkerDisplay.TabIndex = 1;
            LblMarkerDisplay.TabStop = false;
            LblMarkerDisplay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoJIXianShi");
            LblMarkerDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMarkerDisplay.Token = null;
            // 
            // PnlBottomBadge
            // 
            PnlBottomBadge.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            PnlBottomBadge.Controls.Add(BtnCloseAll);
            PnlBottomBadge.Controls.Add(BtnCloseCurrentItem);
            PnlBottomBadge.Controls.Add(BtnCopyFromTrigger);
            PnlBottomBadge.Controls.Add(BtnCopyToTrigger);
            PnlBottomBadge.Dock = System.Windows.Forms.DockStyle.Bottom;
            PnlBottomBadge.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PnlBottomBadge.Location = new System.Drawing.Point(0, 541);
            PnlBottomBadge.Margin = new System.Windows.Forms.Padding(0);
            PnlBottomBadge.Name = "PnlBottomBadge";
            PnlBottomBadge.Size = new System.Drawing.Size(498, 120);
            PnlBottomBadge.TabIndex = 4;
            // 
            // BtnCopyFromTrigger
            // 
            BtnCopyFromTrigger.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyFromTrigger.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyFromTrigger.BorderThickness = 0;
            BtnCopyFromTrigger.CornerRadius = 0;
            BtnCopyFromTrigger.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCopyFromTrigger.DaskArray = null;
            BtnCopyFromTrigger.DropKey = System.Windows.Forms.Keys.Space;
            BtnCopyFromTrigger.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCopyFromTrigger.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyFromTrigger.Height = 30;
            BtnCopyFromTrigger.Icon = null;
            BtnCopyFromTrigger.IconOffset = 10;
            BtnCopyFromTrigger.IconSize = new System.Drawing.Size(24, 24);
            BtnCopyFromTrigger.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCopyFromTrigger.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCopyFromTrigger.IsIndicatorShow = false;
            BtnCopyFromTrigger.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCopyFromTrigger.Location = new System.Drawing.Point(274, 13);
            BtnCopyFromTrigger.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyFromTrigger.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyFromTrigger.MouseInBorderThickness = 0;
            BtnCopyFromTrigger.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyFromTrigger.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCopyFromTrigger.Name = "BtnCopyFromTrigger";
            BtnCopyFromTrigger.PressedBackColor = System.Drawing.Color.Gray;
            BtnCopyFromTrigger.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyFromTrigger.PressedBorderThickness = 0;
            BtnCopyFromTrigger.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyFromTrigger.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCopyFromTrigger.Size = new System.Drawing.Size(200, 40);
            BtnCopyFromTrigger.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCopyFromTrigger.StylizeFlag = true;
            BtnCopyFromTrigger.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyFromTrigger.SVGPath = "";
            BtnCopyFromTrigger.TabIndex = 3;
            BtnCopyFromTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCopyFromTrigger.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuZhiDaoSouSuo");
            BtnCopyFromTrigger.Click += BtnCopyFromTrigger_Click;
            // 
            // BtnCopyToTrigger
            // 
            BtnCopyToTrigger.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyToTrigger.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyToTrigger.BorderThickness = 0;
            BtnCopyToTrigger.CornerRadius = 0;
            BtnCopyToTrigger.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCopyToTrigger.DaskArray = null;
            BtnCopyToTrigger.DropKey = System.Windows.Forms.Keys.Space;
            BtnCopyToTrigger.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCopyToTrigger.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyToTrigger.Height = 30;
            BtnCopyToTrigger.Icon = null;
            BtnCopyToTrigger.IconOffset = 10;
            BtnCopyToTrigger.IconSize = new System.Drawing.Size(24, 24);
            BtnCopyToTrigger.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCopyToTrigger.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCopyToTrigger.IsIndicatorShow = false;
            BtnCopyToTrigger.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCopyToTrigger.Location = new System.Drawing.Point(10, 13);
            BtnCopyToTrigger.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyToTrigger.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyToTrigger.MouseInBorderThickness = 0;
            BtnCopyToTrigger.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyToTrigger.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCopyToTrigger.Name = "BtnCopyToTrigger";
            BtnCopyToTrigger.PressedBackColor = System.Drawing.Color.Gray;
            BtnCopyToTrigger.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCopyToTrigger.PressedBorderThickness = 0;
            BtnCopyToTrigger.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyToTrigger.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCopyToTrigger.Size = new System.Drawing.Size(200, 40);
            BtnCopyToTrigger.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCopyToTrigger.StylizeFlag = true;
            BtnCopyToTrigger.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCopyToTrigger.SVGPath = "";
            BtnCopyToTrigger.TabIndex = 2;
            BtnCopyToTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCopyToTrigger.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhiDaoChuFa");
            BtnCopyToTrigger.Click += BtnCopyToTrigger_Click;
            // 
            // BtnCloseCurrentItem
            // 
            BtnCloseCurrentItem.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseCurrentItem.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseCurrentItem.BorderThickness = 0;
            BtnCloseCurrentItem.CornerRadius = 0;
            BtnCloseCurrentItem.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCloseCurrentItem.DaskArray = null;
            BtnCloseCurrentItem.DropKey = System.Windows.Forms.Keys.Space;
            BtnCloseCurrentItem.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCloseCurrentItem.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseCurrentItem.Height = 30;
            BtnCloseCurrentItem.Icon = null;
            BtnCloseCurrentItem.IconOffset = 10;
            BtnCloseCurrentItem.IconSize = new System.Drawing.Size(24, 24);
            BtnCloseCurrentItem.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCloseCurrentItem.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseCurrentItem.IsIndicatorShow = false;
            BtnCloseCurrentItem.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCloseCurrentItem.Location = new System.Drawing.Point(10, 75);
            BtnCloseCurrentItem.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseCurrentItem.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseCurrentItem.MouseInBorderThickness = 0;
            BtnCloseCurrentItem.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseCurrentItem.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseCurrentItem.Name = "BtnCloseCurrentItem";
            BtnCloseCurrentItem.PressedBackColor = System.Drawing.Color.Gray;
            BtnCloseCurrentItem.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseCurrentItem.PressedBorderThickness = 0;
            BtnCloseCurrentItem.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseCurrentItem.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseCurrentItem.Size = new System.Drawing.Size(200, 40);
            BtnCloseCurrentItem.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCloseCurrentItem.StylizeFlag = true;
            BtnCloseCurrentItem.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseCurrentItem.SVGPath = "";
            BtnCloseCurrentItem.TabIndex = 3;
            BtnCloseCurrentItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCloseCurrentItem.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBiDangQianXiang");
            BtnCloseCurrentItem.Click += BtnCloseCurrentItem_Click;
            // 
            // BtnCloseAll
            // 
            BtnCloseAll.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseAll.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseAll.BorderThickness = 0;
            BtnCloseAll.CornerRadius = 0;
            BtnCloseAll.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCloseAll.DaskArray = null;
            BtnCloseAll.DropKey = System.Windows.Forms.Keys.Space;
            BtnCloseAll.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCloseAll.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseAll.Height = 30;
            BtnCloseAll.Icon = null;
            BtnCloseAll.IconOffset = 10;
            BtnCloseAll.IconSize = new System.Drawing.Size(24, 24);
            BtnCloseAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCloseAll.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseAll.IsIndicatorShow = false;
            BtnCloseAll.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCloseAll.Location = new System.Drawing.Point(274, 75);
            BtnCloseAll.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseAll.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseAll.MouseInBorderThickness = 0;
            BtnCloseAll.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseAll.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseAll.Name = "BtnCloseAll";
            BtnCloseAll.PressedBackColor = System.Drawing.Color.Gray;
            BtnCloseAll.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseAll.PressedBorderThickness = 0;
            BtnCloseAll.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseAll.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseAll.Size = new System.Drawing.Size(200, 40);
            BtnCloseAll.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCloseAll.StylizeFlag = true;
            BtnCloseAll.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseAll.SVGPath = "";
            BtnCloseAll.TabIndex = 3;
            BtnCloseAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCloseAll.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBiQuanBuXiang");
            BtnCloseAll.Click += BtnCloseAll_Click;
            // 
            // SearchItemForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 1;
            ClientSize = new System.Drawing.Size(Width, 633);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpOptions);
            DoubleBuffered = true;
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IconInterval = 21;
            IconWidth = 28;
            Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchItemForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            Controls.SetChildIndex(TlpOptions, 0);
            TlpOptions.ResumeLayout(false);
            PnlTopBadge.ResumeLayout(false);
            PnlBottomBadge.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpOptions;
        private System.Windows.Forms.Panel PnlTopBadge;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LbType;
        private ScopeX.UserControls.SelectComboBox CbxType;
        private ScopeX.UserControls.ScopeXLabel LblMarkerDisplay;
        private ScopeX.UserControls.ScopeXSwitchButton ChkMarkerDisplay;
        private ScopeX.UserControls.ScopeXIconButton BtnShowEventList;
        private UserControls.ScopeXSwitchButton ChkShowEvent;
        private UserControls.ScopeXLabel LBShowEvent;
        private System.Windows.Forms.Panel PnlBottomBadge;
        private ScopeX.UserControls.ScopeXIconButton BtnCopyToTrigger;
        private ScopeX.UserControls.ScopeXIconButton BtnCopyFromTrigger;
        private UserControls.ScopeXIconButton BtnCloseCurrentItem;
        private UserControls.ScopeXIconButton BtnCloseAll;
    }
}
