namespace ScopeX.U2;

partial class MeasurePage
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
        components = new System.ComponentModel.Container();
        UUVerticalSplitLine uuVerticalSplitLine2;
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
        ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new ScopeX.UserControls.DefaultHighlightPrompt();
        ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new ScopeX.UserControls.DefaultHighlightPrompt();
        LblActive = new ScopeX.UserControls.ScopeXLabel();
        ChkMeasure = new ScopeX.UserControls.ScopeXSwitchButton();
        ChkSnapshot = new ScopeX.UserControls.ScopeXSwitchButton();
        LblSnapshot = new ScopeX.UserControls.ScopeXLabel();
        ChkStatistic = new ScopeX.UserControls.ScopeXSwitchButton();
        LblStatistic = new ScopeX.UserControls.ScopeXLabel();
        BtnResetStat = new ScopeX.UserControls.ScopeXIconButton();
        RdoGate = new ScopeX.UserControls.UIRadioButtonGroup();
        LblGate = new ScopeX.UserControls.ScopeXLabel();
        LblIndicator = new ScopeX.UserControls.ScopeXLabel();
        CbxIndicator = new ScopeX.UserControls.ComboBoxEx();
        ChkCymometer = new ScopeX.UserControls.ScopeXSwitchButton();
        LblCymometer = new ScopeX.UserControls.ScopeXLabel();
        ChkVoltmeter = new ScopeX.UserControls.ScopeXSwitchButton();
        LblVoltmeter = new ScopeX.UserControls.ScopeXLabel();
        LblCymometerIcon = new ScopeX.UserControls.ScopeXLabel();
        LblVoltmeterIcon = new ScopeX.UserControls.ScopeXLabel();
        LblSnapshotIcon = new ScopeX.UserControls.ScopeXLabel();
        uuVerticalSplitLine2 = new UUVerticalSplitLine(components);
        SuspendLayout();
        // 
        // uuVerticalSplitLine2
        // 
        uuVerticalSplitLine2.BackColor = System.Drawing.Color.Transparent;
        uuVerticalSplitLine2.DarkColor = System.Drawing.Color.FromArgb(53, 54, 58);
        uuVerticalSplitLine2.LightColor = System.Drawing.Color.White;
        uuVerticalSplitLine2.Location = new System.Drawing.Point(18, 195);
        uuVerticalSplitLine2.Name = "uuVerticalSplitLine2";
        uuVerticalSplitLine2.Size = new System.Drawing.Size(350, 1);
        uuVerticalSplitLine2.TabIndex = 9;
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
        LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblActive.Location = new System.Drawing.Point(10, 4);
        LblActive.MultyLineFlag = false;
        LblActive.Name = "LblActive";
        LblActive.Size = new System.Drawing.Size(75, 18);
        LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblActive.StylizeFlag = true;
        LblActive.TabIndex = 0;
        LblActive.TabStop = false;
        LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
        LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblActive.Token = null;
        // 
        // ChkMeasure
        // 
        ChkMeasure.AnimationCount = 8;
        ChkMeasure.AnimationFunc = null;
        ChkMeasure.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkMeasure.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkMeasure.BorderThickness = 0;
        ChkMeasure.Checked = false;
        ChkMeasure.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        ChkMeasure.CheckedForeColor = System.Drawing.Color.Black;
        ChkMeasure.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkMeasure.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
        ChkMeasure.Cursor = System.Windows.Forms.Cursors.Hand;
        ChkMeasure.DropKey = System.Windows.Forms.Keys.Space;
        ChkMeasure.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkMeasure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        ChkMeasure.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        ChkMeasure.Height = 30;
        ChkMeasure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        ChkMeasure.Location = new System.Drawing.Point(10, 28);
        ChkMeasure.Margin = new System.Windows.Forms.Padding(0);
        ChkMeasure.Name = "ChkMeasure";
        ChkMeasure.Size = new System.Drawing.Size(85, 30);
        ChkMeasure.SliderButtonWidth = 30;
        ChkMeasure.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkMeasure.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        ChkMeasure.StylizeFlag = true;
        ChkMeasure.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
        ChkMeasure.TabIndex = 1;
        ChkMeasure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        ChkMeasure.UseAnimation = true;
        ChkMeasure.CheckedChangedEvent += ChkMeasure_CheckedChangedEvent;
        // 
        // ChkSnapshot
        // 
        ChkSnapshot.AnimationCount = 8;
        ChkSnapshot.AnimationFunc = null;
        ChkSnapshot.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkSnapshot.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkSnapshot.BorderThickness = 0;
        ChkSnapshot.Checked = false;
        ChkSnapshot.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        ChkSnapshot.CheckedForeColor = System.Drawing.Color.Black;
        ChkSnapshot.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkSnapshot.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
        ChkSnapshot.Cursor = System.Windows.Forms.Cursors.Hand;
        ChkSnapshot.DropKey = System.Windows.Forms.Keys.Space;
        ChkSnapshot.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkSnapshot.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        ChkSnapshot.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        ChkSnapshot.Height = 30;
        ChkSnapshot.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        ChkSnapshot.Location = new System.Drawing.Point(39, 246);
        ChkSnapshot.Margin = new System.Windows.Forms.Padding(0);
        ChkSnapshot.Name = "ChkSnapshot";
        ChkSnapshot.Size = new System.Drawing.Size(75, 30);
        ChkSnapshot.SliderButtonWidth = 30;
        ChkSnapshot.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkSnapshot.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        ChkSnapshot.StylizeFlag = true;
        ChkSnapshot.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
        ChkSnapshot.TabIndex = 12;
        ChkSnapshot.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        ChkSnapshot.UseAnimation = true;
        ChkSnapshot.CheckedChangedEvent += ChkSnapshot_CheckedChangedEvent;
        // 
        // LblSnapshot
        // 
        LblSnapshot.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        LblSnapshot.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblSnapshot.BorderThickness = 0;
        LblSnapshot.CornerRadius = 0;
        LblSnapshot.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblSnapshot.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblSnapshot.HighlightPrompt = defaultHighlightPrompt2;
        LblSnapshot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblSnapshot.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblSnapshot.Location = new System.Drawing.Point(39, 222);
        LblSnapshot.MultyLineFlag = false;
        LblSnapshot.Name = "LblSnapshot";
        LblSnapshot.Size = new System.Drawing.Size(75, 18);
        LblSnapshot.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblSnapshot.StylizeFlag = true;
        LblSnapshot.TabIndex = 11;
        LblSnapshot.TabStop = false;
        LblSnapshot.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuKuaiZhao");
        LblSnapshot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblSnapshot.Token = null;
        // 
        // ChkStatistic
        // 
        ChkStatistic.AnimationCount = 8;
        ChkStatistic.AnimationFunc = null;
        ChkStatistic.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkStatistic.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkStatistic.BorderThickness = 0;
        ChkStatistic.Checked = false;
        ChkStatistic.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        ChkStatistic.CheckedForeColor = System.Drawing.Color.Black;
        ChkStatistic.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkStatistic.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
        ChkStatistic.Cursor = System.Windows.Forms.Cursors.Hand;
        ChkStatistic.DropKey = System.Windows.Forms.Keys.Space;
        ChkStatistic.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkStatistic.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        ChkStatistic.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        ChkStatistic.Height = 30;
        ChkStatistic.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        ChkStatistic.Location = new System.Drawing.Point(185, 28);
        ChkStatistic.Margin = new System.Windows.Forms.Padding(0);
        ChkStatistic.Name = "ChkStatistic";
        ChkStatistic.Size = new System.Drawing.Size(85, 30);
        ChkStatistic.SliderButtonWidth = 30;
        ChkStatistic.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkStatistic.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        ChkStatistic.StylizeFlag = true;
        ChkStatistic.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
        ChkStatistic.TabIndex = 3;
        ChkStatistic.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        ChkStatistic.UseAnimation = true;
        ChkStatistic.CheckedChangedEvent += ChkStatistic_CheckedChangedEvent;
        // 
        // LblStatistic
        // 
        LblStatistic.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        LblStatistic.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblStatistic.BorderThickness = 0;
        LblStatistic.CornerRadius = 0;
        LblStatistic.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblStatistic.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblStatistic.HighlightPrompt = defaultHighlightPrompt3;
        LblStatistic.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblStatistic.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblStatistic.Location = new System.Drawing.Point(185, 4);
        LblStatistic.MultyLineFlag = false;
        LblStatistic.Name = "LblStatistic";
        LblStatistic.Size = new System.Drawing.Size(85, 18);
        LblStatistic.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblStatistic.StylizeFlag = true;
        LblStatistic.TabIndex = 2;
        LblStatistic.TabStop = false;
        LblStatistic.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongJi");
        LblStatistic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblStatistic.Token = null;
        // 
        // BtnResetStat
        // 
        BtnResetStat.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        BtnResetStat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        BtnResetStat.BorderThickness = 0;
        BtnResetStat.CornerRadius = 0;
        BtnResetStat.Cursor = System.Windows.Forms.Cursors.Hand;
        BtnResetStat.DaskArray = null;
        BtnResetStat.DropKey = System.Windows.Forms.Keys.Space;
        BtnResetStat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        BtnResetStat.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        BtnResetStat.Height = 30;
        BtnResetStat.Icon = Properties.Resources.MeasureClear;
        BtnResetStat.IconOffset = 10;
        BtnResetStat.IconSize = new System.Drawing.Size(24, 24);
        BtnResetStat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        BtnResetStat.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
        BtnResetStat.IsIndicatorShow = false;
        BtnResetStat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        BtnResetStat.Location = new System.Drawing.Point(283, 28);
        BtnResetStat.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        BtnResetStat.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        BtnResetStat.MouseInBorderThickness = 0;
        BtnResetStat.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        BtnResetStat.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
        BtnResetStat.Name = "BtnResetStat";
        BtnResetStat.PressedBackColor = System.Drawing.Color.Gray;
        BtnResetStat.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        BtnResetStat.PressedBorderThickness = 0;
        BtnResetStat.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        BtnResetStat.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
        BtnResetStat.Size = new System.Drawing.Size(85, 30);
        BtnResetStat.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        BtnResetStat.StylizeFlag = true;
        BtnResetStat.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        BtnResetStat.SVGPath = "";
        BtnResetStat.TabIndex = 4;
        BtnResetStat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
        BtnResetStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        BtnResetStat.Click += BtnResetStat_Click_1;
        // 
        // RdoGate
        // 
        RdoGate.BackColor = System.Drawing.Color.AliceBlue;
        RdoGate.BorderColor = System.Drawing.Color.AliceBlue;
        RdoGate.BorderThickness = 0;
        RdoGate.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
        RdoGate.ButtonFont = null;
        radioButtonItem1.Icon = Properties.Resources.MeasuresGateScreen;
        radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
        radioButtonItem1.Tag = null;
        radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingMu");
        radioButtonItem2.Icon = Properties.Resources.MeasureGateCursor;
        radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
        radioButtonItem2.Tag = null;
        radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuangBiao");
        RdoGate.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 });
        RdoGate.ButtonOffset = 10;
        RdoGate.ButtonTextColor = System.Drawing.Color.White;
        RdoGate.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
        RdoGate.ChoosedButtonIndex = 0;
        RdoGate.ChoosedButtonTextColor = System.Drawing.Color.Black;
        RdoGate.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
        RdoGate.ContentPadding = new System.Windows.Forms.Padding(0);
        RdoGate.FocusBorderColor = System.Drawing.Color.White;
        RdoGate.ForeColor = System.Drawing.Color.White;
        RdoGate.Height = 30;
        RdoGate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        RdoGate.Location = new System.Drawing.Point(185, 126);
        RdoGate.Name = "RdoGate";
        RdoGate.Size = new System.Drawing.Size(180, 30);
        RdoGate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        RdoGate.StylizeFlag = true;
        RdoGate.TabIndex = 8;
        RdoGate.IndexChanged += RdoGate_IndexChanged;
        // 
        // LblGate
        // 
        LblGate.BackColor = System.Drawing.Color.Empty;
        LblGate.BorderColor = System.Drawing.Color.Black;
        LblGate.BorderThickness = 0;
        LblGate.CornerRadius = 0;
        LblGate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblGate.ForeColor = System.Drawing.Color.White;
        LblGate.HighlightPrompt = defaultHighlightPrompt4;
        LblGate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblGate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblGate.Location = new System.Drawing.Point(185, 102);
        LblGate.MultyLineFlag = false;
        LblGate.Name = "LblGate";
        LblGate.Size = new System.Drawing.Size(180, 18);
        LblGate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblGate.StylizeFlag = true;
        LblGate.TabIndex = 7;
        LblGate.TabStop = false;
        LblGate.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MenXian");
        LblGate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblGate.Token = null;
        // 
        // LblIndicator
        // 
        LblIndicator.BackColor = System.Drawing.Color.Empty;
        LblIndicator.BorderColor = System.Drawing.Color.Black;
        LblIndicator.BorderThickness = 0;
        LblIndicator.CornerRadius = 0;
        LblIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblIndicator.ForeColor = System.Drawing.Color.White;
        LblIndicator.HighlightPrompt = defaultHighlightPrompt5;
        LblIndicator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblIndicator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblIndicator.Location = new System.Drawing.Point(10, 102);
        LblIndicator.MultyLineFlag = false;
        LblIndicator.Name = "LblIndicator";
        LblIndicator.Size = new System.Drawing.Size(85, 18);
        LblIndicator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblIndicator.StylizeFlag = false;
        LblIndicator.TabIndex = 5;
        LblIndicator.TabStop = false;
        LblIndicator.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiShiQi");
        LblIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblIndicator.Token = null;
        // 
        // CbxIndicator
        // 
        CbxIndicator.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        CbxIndicator.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        CbxIndicator.BorderThickness = 0;
        CbxIndicator.CornerRadius = 0;
        CbxIndicator.DataSource = null;
        CbxIndicator.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        CbxIndicator.DropDownHeight = 200;
        CbxIndicator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        CbxIndicator.DropDownWidth = 85;
        CbxIndicator.DropKey = System.Windows.Forms.Keys.Space;
        CbxIndicator.DroppedDown = false;
        CbxIndicator.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
        CbxIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        CbxIndicator.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        CbxIndicator.FormattingEnabled = true;
        CbxIndicator.GetDisPlayName = null;
        CbxIndicator.Height = 30;
        CbxIndicator.ImageMode = false;
        CbxIndicator.ItemHeight = 28;
        CbxIndicator.Items.AddRange(new object[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "" });
        CbxIndicator.KeyDropEnble = true;
        CbxIndicator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        CbxIndicator.Location = new System.Drawing.Point(10, 126);
        CbxIndicator.MaxDropDownItems = 8;
        CbxIndicator.Name = "CbxIndicator";
        CbxIndicator.RectBtnWidth = 20;
        CbxIndicator.SelectedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        CbxIndicator.SelectedIndex = 0;
        CbxIndicator.SelectedItem = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
        CbxIndicator.SelectedText = "";
        CbxIndicator.Size = new System.Drawing.Size(85, 30);
        CbxIndicator.Soreted = false;
        CbxIndicator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        CbxIndicator.StylizeFlag = true;
        CbxIndicator.TabIndex = 6;
        CbxIndicator.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
        CbxIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        CbxIndicator.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
        CbxIndicator.SelectedIndexChanged += CbxIndicator_SelectedIndexChanged;
        // 
        // ChkCymometer
        // 
        ChkCymometer.AnimationCount = 8;
        ChkCymometer.AnimationFunc = null;
        ChkCymometer.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkCymometer.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkCymometer.BorderThickness = 0;
        ChkCymometer.Checked = false;
        ChkCymometer.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        ChkCymometer.CheckedForeColor = System.Drawing.Color.Black;
        ChkCymometer.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkCymometer.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
        ChkCymometer.Cursor = System.Windows.Forms.Cursors.Hand;
        ChkCymometer.DropKey = System.Windows.Forms.Keys.Space;
        ChkCymometer.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkCymometer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        ChkCymometer.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        ChkCymometer.Height = 30;
        ChkCymometer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        ChkCymometer.Location = new System.Drawing.Point(164, 246);
        ChkCymometer.Margin = new System.Windows.Forms.Padding(0);
        ChkCymometer.Name = "ChkCymometer";
        ChkCymometer.Size = new System.Drawing.Size(75, 30);
        ChkCymometer.SliderButtonWidth = 30;
        ChkCymometer.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkCymometer.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        ChkCymometer.StylizeFlag = true;
        ChkCymometer.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
        ChkCymometer.TabIndex = 15;
        ChkCymometer.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        ChkCymometer.UseAnimation = true;
        ChkCymometer.CheckedChangedEvent += ChkCymometer_CheckedChangedEvent;
        // 
        // LblCymometer
        // 
        LblCymometer.BackColor = System.Drawing.Color.Transparent;
        LblCymometer.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblCymometer.BorderThickness = 0;
        LblCymometer.CornerRadius = 0;
        LblCymometer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblCymometer.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblCymometer.HighlightPrompt = defaultHighlightPrompt6;
        LblCymometer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblCymometer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblCymometer.Location = new System.Drawing.Point(164, 222);
        LblCymometer.MultyLineFlag = false;
        LblCymometer.Name = "LblCymometer";
        LblCymometer.Size = new System.Drawing.Size(75, 18);
        LblCymometer.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblCymometer.StylizeFlag = true;
        LblCymometer.TabIndex = 14;
        LblCymometer.TabStop = false;
        LblCymometer.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLvJi");
        LblCymometer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblCymometer.Token = null;
        // 
        // ChkVoltmeter
        // 
        ChkVoltmeter.AnimationCount = 8;
        ChkVoltmeter.AnimationFunc = null;
        ChkVoltmeter.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkVoltmeter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkVoltmeter.BorderThickness = 0;
        ChkVoltmeter.Checked = false;
        ChkVoltmeter.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
        ChkVoltmeter.CheckedForeColor = System.Drawing.Color.Black;
        ChkVoltmeter.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkVoltmeter.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
        ChkVoltmeter.Cursor = System.Windows.Forms.Cursors.Hand;
        ChkVoltmeter.DropKey = System.Windows.Forms.Keys.Space;
        ChkVoltmeter.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        ChkVoltmeter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        ChkVoltmeter.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
        ChkVoltmeter.Height = 30;
        ChkVoltmeter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        ChkVoltmeter.Location = new System.Drawing.Point(290, 246);
        ChkVoltmeter.Margin = new System.Windows.Forms.Padding(0);
        ChkVoltmeter.Name = "ChkVoltmeter";
        ChkVoltmeter.Size = new System.Drawing.Size(75, 30);
        ChkVoltmeter.SliderButtonWidth = 30;
        ChkVoltmeter.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
        ChkVoltmeter.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        ChkVoltmeter.StylizeFlag = true;
        ChkVoltmeter.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
        ChkVoltmeter.TabIndex = 18;
        ChkVoltmeter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
        ChkVoltmeter.UseAnimation = true;
        ChkVoltmeter.CheckedChangedEvent += ChkVoltmeter_CheckedChangedEvent;
        // 
        // LblVoltmeter
        // 
        LblVoltmeter.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        LblVoltmeter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblVoltmeter.BorderThickness = 0;
        LblVoltmeter.CornerRadius = 0;
        LblVoltmeter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblVoltmeter.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblVoltmeter.HighlightPrompt = defaultHighlightPrompt7;
        LblVoltmeter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblVoltmeter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblVoltmeter.Location = new System.Drawing.Point(290, 222);
        LblVoltmeter.MultyLineFlag = false;
        LblVoltmeter.Name = "LblVoltmeter";
        LblVoltmeter.Size = new System.Drawing.Size(75, 18);
        LblVoltmeter.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblVoltmeter.StylizeFlag = true;
        LblVoltmeter.TabIndex = 17;
        LblVoltmeter.TabStop = false;
        LblVoltmeter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaBiao");
        LblVoltmeter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblVoltmeter.Token = null;
        // 
        // LblCymometerIcon
        // 
        LblCymometerIcon.BackColor = System.Drawing.Color.Transparent;
        LblCymometerIcon.BackgroundImage = Properties.Resources.Cymometer;
        LblCymometerIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        LblCymometerIcon.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblCymometerIcon.BorderThickness = 0;
        LblCymometerIcon.CornerRadius = 0;
        LblCymometerIcon.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblCymometerIcon.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblCymometerIcon.HighlightPrompt = defaultHighlightPrompt8;
        LblCymometerIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblCymometerIcon.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblCymometerIcon.Location = new System.Drawing.Point(135, 246);
        LblCymometerIcon.MultyLineFlag = false;
        LblCymometerIcon.Name = "LblCymometerIcon";
        LblCymometerIcon.Size = new System.Drawing.Size(26, 30);
        LblCymometerIcon.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblCymometerIcon.StylizeFlag = true;
        LblCymometerIcon.TabIndex = 13;
        LblCymometerIcon.TabStop = false;
        LblCymometerIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblCymometerIcon.Token = null;
        // 
        // LblVoltmeterIcon
        // 
        LblVoltmeterIcon.BackColor = System.Drawing.Color.Transparent;
        LblVoltmeterIcon.BackgroundImage = Properties.Resources.Dvm;
        LblVoltmeterIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        LblVoltmeterIcon.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblVoltmeterIcon.BorderThickness = 0;
        LblVoltmeterIcon.CornerRadius = 0;
        LblVoltmeterIcon.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblVoltmeterIcon.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblVoltmeterIcon.HighlightPrompt = defaultHighlightPrompt9;
        LblVoltmeterIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblVoltmeterIcon.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblVoltmeterIcon.Location = new System.Drawing.Point(261, 246);
        LblVoltmeterIcon.MultyLineFlag = false;
        LblVoltmeterIcon.Name = "LblVoltmeterIcon";
        LblVoltmeterIcon.Size = new System.Drawing.Size(26, 30);
        LblVoltmeterIcon.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblVoltmeterIcon.StylizeFlag = true;
        LblVoltmeterIcon.TabIndex = 16;
        LblVoltmeterIcon.TabStop = false;
        LblVoltmeterIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblVoltmeterIcon.Token = null;
        // 
        // LblSnapshotIcon
        // 
        LblSnapshotIcon.BackColor = System.Drawing.Color.Transparent;
        LblSnapshotIcon.BackgroundImage = Properties.Resources.MeasureSnapshot;
        LblSnapshotIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        LblSnapshotIcon.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
        LblSnapshotIcon.BorderThickness = 0;
        LblSnapshotIcon.CornerRadius = 0;
        LblSnapshotIcon.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LblSnapshotIcon.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
        LblSnapshotIcon.HighlightPrompt = defaultHighlightPrompt10;
        LblSnapshotIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        LblSnapshotIcon.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
        LblSnapshotIcon.Location = new System.Drawing.Point(10, 246);
        LblSnapshotIcon.MultyLineFlag = false;
        LblSnapshotIcon.Name = "LblSnapshotIcon";
        LblSnapshotIcon.Size = new System.Drawing.Size(26, 30);
        LblSnapshotIcon.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
        LblSnapshotIcon.StylizeFlag = true;
        LblSnapshotIcon.TabIndex = 10;
        LblSnapshotIcon.TabStop = false;
        LblSnapshotIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        LblSnapshotIcon.Token = null;
        // 
        // MeasurePage
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        Controls.Add(uuVerticalSplitLine2);
        Controls.Add(CbxIndicator);
        Controls.Add(LblIndicator);
        Controls.Add(RdoGate);
        Controls.Add(LblGate);
        Controls.Add(BtnResetStat);
        Controls.Add(LblStatistic);
        Controls.Add(LblVoltmeter);
        Controls.Add(LblVoltmeterIcon);
        Controls.Add(LblSnapshotIcon);
        Controls.Add(LblCymometerIcon);
        Controls.Add(LblCymometer);
        Controls.Add(LblSnapshot);
        Controls.Add(LblActive);
        Controls.Add(ChkStatistic);
        Controls.Add(ChkVoltmeter);
        Controls.Add(ChkCymometer);
        Controls.Add(ChkSnapshot);
        Controls.Add(ChkMeasure);
        DoubleBuffered = true;
        Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        Name = "MeasurePage";
        Size = new System.Drawing.Size(388, 293);
        ResumeLayout(false);
    }

    #endregion

    private ScopeX.UserControls.ScopeXLabel LblActive;
    private ScopeX.UserControls.ScopeXSwitchButton ChkMeasure;
    private ScopeX.UserControls.ScopeXSwitchButton ChkSnapshot;
    private ScopeX.UserControls.ScopeXLabel LblSnapshot;
    private ScopeX.UserControls.ScopeXSwitchButton ChkStatistic;
    private ScopeX.UserControls.ScopeXLabel LblStatistic;
    private ScopeX.UserControls.ScopeXIconButton BtnResetStat;
    private ScopeX.UserControls.UIRadioButtonGroup RdoGate;
    private ScopeX.UserControls.ScopeXLabel LblGate;
    private ScopeX.UserControls.ScopeXLabel LblIndicator;
    private ScopeX.UserControls.ComboBoxEx CbxIndicator;
    private ScopeX.UserControls.ScopeXSwitchButton ChkCymometer;
    private ScopeX.UserControls.ScopeXLabel LblCymometer;
    private ScopeX.UserControls.ScopeXSwitchButton ChkVoltmeter;
    private ScopeX.UserControls.ScopeXLabel LblVoltmeter;
    private ScopeX.UserControls.ScopeXLabel LblCymometerIcon;
    private ScopeX.UserControls.ScopeXLabel LblVoltmeterIcon;
    private ScopeX.UserControls.ScopeXLabel LblSnapshotIcon;
}
