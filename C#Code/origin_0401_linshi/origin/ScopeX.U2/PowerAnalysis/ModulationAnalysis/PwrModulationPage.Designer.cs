namespace ScopeX.U2
{
    partial class PwrModulationPage
    {
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
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PwrModulationPage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt12 = new UserControls.DefaultHighlightPrompt();
            BtnResultTable = new UserControls.ScopeXIconButton();
            SourceType = new UserControls.UIRadioButtonGroup();
            LblSourceSelection = new UserControls.ScopeXLabel();
            BtnGuide = new UserControls.ScopeXIconButton();
            LblCurrentSrc = new UserControls.ScopeXLabel();
            LblVoltageSrc = new UserControls.ScopeXLabel();
            CbxCurrentSrc = new UserControls.SelectComboBox();
            CbxVoltageSrc = new UserControls.SelectComboBox();
            ChkActive = new UserControls.ScopeXSwitchButton();
            LblDisplay = new UserControls.ScopeXLabel();
            CbxHistogramSource = new UserControls.SelectComboBox();
            LblHistogram = new UserControls.ScopeXLabel();
            BtnHistogramAdd = new UserControls.ScopeXIconButton();
            BtnTrendAdd = new UserControls.ScopeXIconButton();
            LblTrend = new UserControls.ScopeXLabel();
            CbxTrendSource = new UserControls.SelectComboBox();
            SuspendLayout();
            // 
            // BtnResultTable
            // 
            BtnResultTable.Adjustable = false;
            BtnResultTable.BackColor = System.Drawing.Color.Transparent;
            BtnResultTable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.BorderThickness = 0;
            BtnResultTable.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnResultTable.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.CornerRadius = 0;
            BtnResultTable.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResultTable.DaskArray = null;
            BtnResultTable.DoubleClickEnable = true;
            BtnResultTable.DropKey = System.Windows.Forms.Keys.Space;
            BtnResultTable.FineEnable = false;
            BtnResultTable.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnResultTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResultTable.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnResultTable.Height = 30;
            BtnResultTable.Icon = null;
            BtnResultTable.IconOffset = 10;
            BtnResultTable.IconSize = new System.Drawing.Size(24, 24);
            BtnResultTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResultTable.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.IsChoosed = false;
            BtnResultTable.IsIndicatorShow = false;
            BtnResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResultTable.Location = new System.Drawing.Point(386, 154);
            BtnResultTable.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.MouseInBorderThickness = 0;
            BtnResultTable.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResultTable.Name = "BtnResultTable";
            BtnResultTable.PressedBackColor = System.Drawing.Color.Gray;
            BtnResultTable.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.PressedBorderThickness = 0;
            BtnResultTable.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResultTable.Size = new System.Drawing.Size(120, 30);
            BtnResultTable.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResultTable.StylizeFlag = true;
            BtnResultTable.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.SVGPath = "";
            BtnResultTable.TabIndex = 70;
            BtnResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResultTable.Click += BtnResultTable_Click;
            // 
            // FrequencyReference
            // 
            SourceType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            SourceType.BackColor = System.Drawing.Color.Black;
            SourceType.BorderColor = System.Drawing.Color.Black;
            SourceType.BorderThickness = 0;
            SourceType.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            SourceType.ButtonFont = null;
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            SourceType.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem3,
    radioButtonItem4
    };
            SourceType.ButtonOffset = 0;
            SourceType.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            SourceType.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            SourceType.ChoosedButtonIndex = 0;
            SourceType.ChoosedButtonTextColor = System.Drawing.Color.Black;
            SourceType.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            SourceType.ContentPadding = new System.Windows.Forms.Padding(0);
            SourceType.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            SourceType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            SourceType.Height = 30;
            SourceType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            SourceType.Location = new System.Drawing.Point(19, 154);
            SourceType.Margin = new System.Windows.Forms.Padding(0);
            SourceType.Name = "SourceType";
            SourceType.Size = new System.Drawing.Size(140, 30);
            SourceType.StyleFlags = UserControls.Style.StyleFlag.None;
            SourceType.StylizeFlag = true;
            SourceType.TabIndex = 62;
            SourceType.IndexChanged += SourceType_IndexChanged;
            // 
            // LblSourceSelection
            // 
            LblSourceSelection.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSourceSelection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSourceSelection.BorderThickness = 0;
            LblSourceSelection.CornerRadius = 0;
            LblSourceSelection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSourceSelection.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSourceSelection.HighlightPrompt = defaultHighlightPrompt7;
            LblSourceSelection.IsOmittext = true;
            LblSourceSelection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSourceSelection.Location = new System.Drawing.Point(19, 126);
            LblSourceSelection.MultyLineFlag = false;
            LblSourceSelection.Name = "LblSourceSelection";
            LblSourceSelection.Size = new System.Drawing.Size(108, 18);
            LblSourceSelection.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSourceSelection.StylizeFlag = true;
            LblSourceSelection.TabIndex = 57;
            LblSourceSelection.TabStop = false;
            LblSourceSelection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSourceSelection.Token = null;
            // 
            // BtnGuide
            // 
            BtnGuide.Adjustable = false;
            BtnGuide.BackColor = System.Drawing.Color.Transparent;
            BtnGuide.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.BorderThickness = 0;
            BtnGuide.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnGuide.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.CornerRadius = 0;
            BtnGuide.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnGuide.DaskArray = null;
            BtnGuide.DoubleClickEnable = true;
            BtnGuide.DropKey = System.Windows.Forms.Keys.Space;
            BtnGuide.FineEnable = false;
            BtnGuide.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnGuide.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnGuide.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnGuide.Height = 30;
            BtnGuide.Icon = null;
            BtnGuide.IconOffset = 10;
            BtnGuide.IconSize = new System.Drawing.Size(24, 24);
            BtnGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnGuide.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.IsChoosed = false;
            BtnGuide.IsIndicatorShow = false;
            BtnGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnGuide.Location = new System.Drawing.Point(196, 154);
            BtnGuide.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.MouseInBorderThickness = 0;
            BtnGuide.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnGuide.Name = "BtnGuide";
            BtnGuide.PressedBackColor = System.Drawing.Color.Gray;
            BtnGuide.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.PressedBorderThickness = 0;
            BtnGuide.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnGuide.Size = new System.Drawing.Size(140, 30);
            BtnGuide.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnGuide.StylizeFlag = true;
            BtnGuide.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.SVGPath = "";
            BtnGuide.TabIndex = 52;
            BtnGuide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnGuide.Click += BtnGuide_Click;
            // 
            // LblCurrentSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt8;
            LblCurrentSrc.IsOmittext = true;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(386, 16);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblCurrentSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(100, 18);
            LblCurrentSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 55;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCurrentSrc.Token = null;
            // 
            // LblVoltageSrc
            // 
            LblVoltageSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVoltageSrc.BorderThickness = 0;
            LblVoltageSrc.CornerRadius = 0;
            LblVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVoltageSrc.HighlightPrompt = defaultHighlightPrompt9;
            LblVoltageSrc.IsOmittext = true;
            LblVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVoltageSrc.Location = new System.Drawing.Point(196, 16);
            LblVoltageSrc.MultyLineFlag = false;
            LblVoltageSrc.Name = "LblVoltageSrc";
            LblVoltageSrc.Size = new System.Drawing.Size(100, 18);
            LblVoltageSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVoltageSrc.StylizeFlag = true;
            LblVoltageSrc.TabIndex = 53;
            LblVoltageSrc.TabStop = false;
            LblVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVoltageSrc.Token = null;
            // 
            // CbxCurrentSrc
            // 
            CbxCurrentSrc.AutoSize = true;
            CbxCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.ComBorderColor = System.Drawing.Color.Blue;
            CbxCurrentSrc.DataSource = (System.Collections.IList)resources.GetObject("CbxCurrentSrc.DataSource");
            CbxCurrentSrc.ExtText = "";
            CbxCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxCurrentSrc.Location = new System.Drawing.Point(386, 44);
            CbxCurrentSrc.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxCurrentSrc.Name = "CbxCurrentSrc";
            CbxCurrentSrc.SelectIndex = 0;
            CbxCurrentSrc.SelectValue = 0;
            CbxCurrentSrc.Size = new System.Drawing.Size(100, 30);
            CbxCurrentSrc.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxCurrentSrc.StylizeFlag = true;
            CbxCurrentSrc.TabIndex = 77;
            // 
            // CbxVoltageSrc
            // 
            CbxVoltageSrc.AutoSize = true;
            CbxVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.ComBorderColor = System.Drawing.Color.Blue;
            CbxVoltageSrc.DataSource = null;
            CbxVoltageSrc.ExtText = "";
            CbxVoltageSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxVoltageSrc.ForeColor = System.Drawing.Color.White;
            CbxVoltageSrc.Location = new System.Drawing.Point(196, 44);
            CbxVoltageSrc.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxVoltageSrc.Name = "CbxVoltageSrc";
            CbxVoltageSrc.SelectIndex = 0;
            CbxVoltageSrc.SelectValue = null;
            CbxVoltageSrc.Size = new System.Drawing.Size(100, 30);
            CbxVoltageSrc.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxVoltageSrc.StylizeFlag = true;
            CbxVoltageSrc.TabIndex = 76;
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
            ChkActive.CheckedText = "开";
            ChkActive.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkActive.DropKey = System.Windows.Forms.Keys.Space;
            ChkActive.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(19, 44);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 74;
            ChkActive.Text = "关";
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // LblDisplay
            // 
            LblDisplay.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblDisplay.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblDisplay.BorderThickness = 0;
            LblDisplay.CornerRadius = 0;
            LblDisplay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDisplay.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblDisplay.HighlightPrompt = defaultHighlightPrompt10;
            LblDisplay.IsOmittext = true;
            LblDisplay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDisplay.Location = new System.Drawing.Point(19, 16);
            LblDisplay.MultyLineFlag = false;
            LblDisplay.Name = "LblDisplay";
            LblDisplay.Size = new System.Drawing.Size(75, 18);
            LblDisplay.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDisplay.StylizeFlag = true;
            LblDisplay.TabIndex = 75;
            LblDisplay.TabStop = false;
            LblDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDisplay.Token = null;
            // 
            // CbxHistogramSource
            // 
            CbxHistogramSource.AutoSize = true;
            CbxHistogramSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxHistogramSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxHistogramSource.ComBorderColor = System.Drawing.Color.Blue;
            CbxHistogramSource.DataSource = null;
            CbxHistogramSource.ExtText = "";
            CbxHistogramSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxHistogramSource.ForeColor = System.Drawing.Color.White;
            CbxHistogramSource.Location = new System.Drawing.Point(18, 273);
            CbxHistogramSource.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxHistogramSource.Name = "CbxHistogramSource";
            CbxHistogramSource.SelectIndex = 0;
            CbxHistogramSource.SelectValue = null;
            CbxHistogramSource.Size = new System.Drawing.Size(140, 30);
            CbxHistogramSource.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxHistogramSource.StylizeFlag = true;
            CbxHistogramSource.TabIndex = 79;
            // 
            // LblHistogram
            // 
            LblHistogram.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHistogram.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHistogram.BorderThickness = 0;
            LblHistogram.CornerRadius = 0;
            LblHistogram.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHistogram.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHistogram.HighlightPrompt = defaultHighlightPrompt11;
            LblHistogram.IsOmittext = true;
            LblHistogram.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHistogram.Location = new System.Drawing.Point(18, 245);
            LblHistogram.MultyLineFlag = false;
            LblHistogram.Name = "LblHistogram";
            LblHistogram.Size = new System.Drawing.Size(100, 18);
            LblHistogram.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHistogram.StylizeFlag = true;
            LblHistogram.TabIndex = 78;
            LblHistogram.TabStop = false;
            LblHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHistogram.Token = null;
            // 
            // BtnHistogramAdd
            // 
            BtnHistogramAdd.Adjustable = false;
            BtnHistogramAdd.BackColor = System.Drawing.Color.Transparent;
            BtnHistogramAdd.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogramAdd.BorderThickness = 0;
            BtnHistogramAdd.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnHistogramAdd.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnHistogramAdd.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnHistogramAdd.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnHistogramAdd.CornerRadius = 0;
            BtnHistogramAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnHistogramAdd.DaskArray = null;
            BtnHistogramAdd.DoubleClickEnable = true;
            BtnHistogramAdd.DropKey = System.Windows.Forms.Keys.Space;
            BtnHistogramAdd.FineEnable = false;
            BtnHistogramAdd.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnHistogramAdd.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnHistogramAdd.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnHistogramAdd.Height = 30;
            BtnHistogramAdd.Icon = null;
            BtnHistogramAdd.IconOffset = 10;
            BtnHistogramAdd.IconSize = new System.Drawing.Size(24, 24);
            BtnHistogramAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnHistogramAdd.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnHistogramAdd.IsChoosed = false;
            BtnHistogramAdd.IsIndicatorShow = false;
            BtnHistogramAdd.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnHistogramAdd.Location = new System.Drawing.Point(164, 273);
            BtnHistogramAdd.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogramAdd.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogramAdd.MouseInBorderThickness = 0;
            BtnHistogramAdd.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistogramAdd.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnHistogramAdd.Name = "BtnHistogramAdd";
            BtnHistogramAdd.PressedBackColor = System.Drawing.Color.Gray;
            BtnHistogramAdd.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogramAdd.PressedBorderThickness = 0;
            BtnHistogramAdd.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistogramAdd.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnHistogramAdd.Size = new System.Drawing.Size(50, 30);
            BtnHistogramAdd.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnHistogramAdd.StylizeFlag = true;
            BtnHistogramAdd.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistogramAdd.SVGPath = "";
            BtnHistogramAdd.TabIndex = 80;
            BtnHistogramAdd.Text = "添加";
            BtnHistogramAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnHistogramAdd.Click += BtnHistogramAdd_Click;
            // 
            // BtnTrendAdd
            // 
            BtnTrendAdd.Adjustable = false;
            BtnTrendAdd.BackColor = System.Drawing.Color.Transparent;
            BtnTrendAdd.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrendAdd.BorderThickness = 0;
            BtnTrendAdd.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTrendAdd.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnTrendAdd.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTrendAdd.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTrendAdd.CornerRadius = 0;
            BtnTrendAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnTrendAdd.DaskArray = null;
            BtnTrendAdd.DoubleClickEnable = true;
            BtnTrendAdd.DropKey = System.Windows.Forms.Keys.Space;
            BtnTrendAdd.FineEnable = false;
            BtnTrendAdd.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnTrendAdd.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnTrendAdd.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnTrendAdd.Height = 30;
            BtnTrendAdd.Icon = null;
            BtnTrendAdd.IconOffset = 10;
            BtnTrendAdd.IconSize = new System.Drawing.Size(24, 24);
            BtnTrendAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnTrendAdd.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTrendAdd.IsChoosed = false;
            BtnTrendAdd.IsIndicatorShow = false;
            BtnTrendAdd.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnTrendAdd.Location = new System.Drawing.Point(455, 273);
            BtnTrendAdd.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrendAdd.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrendAdd.MouseInBorderThickness = 0;
            BtnTrendAdd.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTrendAdd.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnTrendAdd.Name = "BtnTrendAdd";
            BtnTrendAdd.PressedBackColor = System.Drawing.Color.Gray;
            BtnTrendAdd.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrendAdd.PressedBorderThickness = 0;
            BtnTrendAdd.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTrendAdd.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnTrendAdd.Size = new System.Drawing.Size(50, 30);
            BtnTrendAdd.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnTrendAdd.StylizeFlag = true;
            BtnTrendAdd.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTrendAdd.SVGPath = "";
            BtnTrendAdd.TabIndex = 83;
            BtnTrendAdd.Text = "添加";
            BtnTrendAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnTrendAdd.Click += BtnTrendAdd_Click;
            // 
            // LblTrend
            // 
            LblTrend.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTrend.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTrend.BorderThickness = 0;
            LblTrend.CornerRadius = 0;
            LblTrend.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTrend.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTrend.HighlightPrompt = defaultHighlightPrompt12;
            LblTrend.IsOmittext = true;
            LblTrend.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTrend.Location = new System.Drawing.Point(309, 245);
            LblTrend.MultyLineFlag = false;
            LblTrend.Name = "LblTrend";
            LblTrend.Size = new System.Drawing.Size(100, 18);
            LblTrend.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTrend.StylizeFlag = true;
            LblTrend.TabIndex = 81;
            LblTrend.TabStop = false;
            LblTrend.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTrend.Token = null;
            // 
            // CbxTrendSource
            // 
            CbxTrendSource.AutoSize = true;
            CbxTrendSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTrendSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTrendSource.ComBorderColor = System.Drawing.Color.Blue;
            CbxTrendSource.DataSource = (System.Collections.IList)resources.GetObject("CbxTrendSource.DataSource");
            CbxTrendSource.ExtText = "";
            CbxTrendSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxTrendSource.ForeColor = System.Drawing.Color.White;
            CbxTrendSource.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxTrendSource.Location = new System.Drawing.Point(309, 273);
            CbxTrendSource.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxTrendSource.Name = "CbxTrendSource";
            CbxTrendSource.SelectIndex = 0;
            CbxTrendSource.SelectValue = 0;
            CbxTrendSource.Size = new System.Drawing.Size(140, 30);
            CbxTrendSource.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxTrendSource.StylizeFlag = true;
            CbxTrendSource.TabIndex = 82;
            // 
            // PwrModulationPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            Controls.Add(BtnTrendAdd);
            Controls.Add(LblTrend);
            Controls.Add(CbxTrendSource);
            Controls.Add(BtnHistogramAdd);
            Controls.Add(LblHistogram);
            Controls.Add(CbxHistogramSource);
            Controls.Add(LblDisplay);
            Controls.Add(ChkActive);
            Controls.Add(BtnResultTable);
            Controls.Add(SourceType);
            Controls.Add(LblSourceSelection);
            Controls.Add(BtnGuide);
            Controls.Add(LblCurrentSrc);
            Controls.Add(LblVoltageSrc);
            Controls.Add(CbxCurrentSrc);
            Controls.Add(CbxVoltageSrc);
            Name = "PwrModulationPage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
        private ScopeX.UserControls.UIRadioButtonGroup SourceType;
        private ScopeX.UserControls.ScopeXLabel LblSourceSelection;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.ScopeXLabel LblVoltageSrc;
        // private ScopeX.UserControls.ComboBoxEx CbxCurrentSrc;
        // private ScopeX.UserControls.ComboBoxEx CbxVoltageSrc;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblDisplay;
        private ScopeX.UserControls.ScopeXSwitchButton ScopeXSwitchButton1;
        private ScopeX.UserControls.SelectComboBox CbxVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
        private UserControls.SelectComboBox CbxHistogramSource;
        private UserControls.ScopeXLabel LblHistogram;
        private UserControls.ScopeXIconButton BtnHistogramAdd;
        private UserControls.ScopeXIconButton BtnTrendAdd;
        private UserControls.ScopeXLabel LblTrend;
        private UserControls.SelectComboBox CbxTrendSource;
    }
}
