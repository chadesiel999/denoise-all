using static ScopeX.UserControls.ScopeXNumericEditBox;

namespace ScopeX.U2
{
    partial class PwrSwitchingLossPage
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
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle11 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle31 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle32 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle33 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle12 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle34 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle35 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle36 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            LblCurrentSrc = new ScopeX.UserControls.ScopeXLabel();
            LblVoltageSrc = new ScopeX.UserControls.ScopeXLabel();
            CbxCurrentSrc = new ScopeX.UserControls.SelectComboBox();
            CbxVoltageSrc = new ScopeX.UserControls.SelectComboBox();
            ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            ScopeXLabel6 = new ScopeX.UserControls.ScopeXLabel();
            LblRdsOn = new ScopeX.UserControls.ScopeXLabel();
            NebRdsOn = new UserControls.TouchNeb();
            BtnPowerPic = new ScopeX.UserControls.ScopeXIconButton();
            BtnGuide = new ScopeX.UserControls.ScopeXIconButton();
            BtnResultTable = new ScopeX.UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LblCurrentSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(372, 16);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblCurrentSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(75, 18);
            LblCurrentSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 55;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
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
            LblVoltageSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVoltageSrc.Location = new System.Drawing.Point(206, 16);
            LblVoltageSrc.MultyLineFlag = false;
            LblVoltageSrc.Name = "LblVoltageSrc";
            LblVoltageSrc.Size = new System.Drawing.Size(75, 18);
            LblVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblVoltageSrc.StylizeFlag = true;
            LblVoltageSrc.TabIndex = 53;
            LblVoltageSrc.TabStop = false;
            LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            LblVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVoltageSrc.Token = null;
            // 
            // CbxVoltageSrc
            // 
            CbxVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxVoltageSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxVoltageSrc.ForeColor = System.Drawing.Color.White;
            CbxVoltageSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxVoltageSrc.Location = new System.Drawing.Point(206, 44);
            CbxVoltageSrc.Name = "CbxVoltageSrc";
            CbxVoltageSrc.Size = new System.Drawing.Size(100, 30);
            CbxVoltageSrc.TabIndex = 94;
            // 
            // CbxCurrentSrc
            // 
            CbxCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxCurrentSrc.Location = new System.Drawing.Point(372, 44);
            CbxCurrentSrc.Name = "CbxCurrentSrc";
            CbxCurrentSrc.Size = new System.Drawing.Size(121, 30);
            CbxCurrentSrc.TabIndex = 95;
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
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(19, 44);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 74;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // ScopeXLabel6
            // 
            ScopeXLabel6.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScopeXLabel6.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ScopeXLabel6.BorderThickness = 0;
            ScopeXLabel6.CornerRadius = 0;
            ScopeXLabel6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ScopeXLabel6.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ScopeXLabel6.HighlightPrompt = defaultHighlightPrompt3;
            ScopeXLabel6.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ScopeXLabel6.Location = new System.Drawing.Point(19, 16);
            ScopeXLabel6.MultyLineFlag = false;
            ScopeXLabel6.Name = "ScopeXLabel6";
            ScopeXLabel6.Size = new System.Drawing.Size(75, 18);
            ScopeXLabel6.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ScopeXLabel6.StylizeFlag = true;
            ScopeXLabel6.TabIndex = 75;
            ScopeXLabel6.TabStop = false;
            ScopeXLabel6.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            ScopeXLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            ScopeXLabel6.Token = null;
            // LblRdsOn
            // 
            LblRdsOn.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblRdsOn.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblRdsOn.BorderThickness = 0;
            LblRdsOn.CornerRadius = 0;
            LblRdsOn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblRdsOn.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblRdsOn.HighlightPrompt = defaultHighlightPrompt3;
            LblRdsOn.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblRdsOn.Location = new System.Drawing.Point(19, 270);
            LblRdsOn.MultyLineFlag = false;
            LblRdsOn.Name = "LblRdsOn";
            LblRdsOn.Size = new System.Drawing.Size(75, 18);
            LblRdsOn.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblRdsOn.StylizeFlag = true;
            LblRdsOn.TabIndex = 75;
            LblRdsOn.TabStop = false;
            LblRdsOn.Text = "Rds(on)";
            LblRdsOn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblRdsOn.Token = null;
            // 
            // NebRdsOn
            // 
            NebRdsOn.AddButtonImg = null;
            NebRdsOn.AllwaysShowFocusImage = false;
            NebRdsOn.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebRdsOn.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebRdsOn.BorderThickness = 0;
            NebRdsOn.DisableHoldOnInput = false;
            NebRdsOn.DropKey = System.Windows.Forms.Keys.Space;
            NebRdsOn.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebRdsOn.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebRdsOn.FocusImage = null;
            NebRdsOn.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebRdsOn.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebRdsOn.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebRdsOn.Height = 30;
            NebRdsOn.HoldOnSpeedLevel = 5;
            NebRdsOn.IconWidthProportion = 1F;
            NebRdsOn.Interval = 0.1D;
            NebRdsOn.LanguageKey = null;
            NebRdsOn.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebRdsOn.Location = new System.Drawing.Point(19, 300);
            NebRdsOn.Name = "NebRdsOn";
            NebRdsOn.AddButtonImg = null;
            buttonBaseStyle31.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle31.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle31.BorderThickness = 0;
            buttonBaseStyle31.ForeColor = System.Drawing.Color.White;
            buttonStyle11.MouseClickStyle = buttonBaseStyle31;
            buttonBaseStyle32.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle32.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle32.BorderThickness = 0;
            buttonBaseStyle32.ForeColor = System.Drawing.Color.White;
            buttonStyle11.MouseInStyle = buttonBaseStyle32;
            buttonBaseStyle33.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle33.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle33.BorderThickness = 0;
            buttonBaseStyle33.ForeColor = System.Drawing.Color.White;
            buttonStyle11.NomalStyle = buttonBaseStyle33;
            NebRdsOn.AddButtonStyle = buttonStyle11;
            NebRdsOn.SubButtonImg = null;
            buttonBaseStyle34.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle34.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle34.BorderThickness = 0;
            buttonBaseStyle34.ForeColor = System.Drawing.Color.White;
            buttonStyle12.MouseClickStyle = buttonBaseStyle34;
            buttonBaseStyle35.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle35.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle35.BorderThickness = 0;
            buttonBaseStyle35.ForeColor = System.Drawing.Color.White;
            buttonStyle12.MouseInStyle = buttonBaseStyle35;
            buttonBaseStyle36.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle36.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle36.BorderThickness = 0;
            buttonBaseStyle36.ForeColor = System.Drawing.Color.White;
            buttonStyle12.NomalStyle = buttonBaseStyle36;
            NebRdsOn.SubButtonStyle = buttonStyle12;
            NebRdsOn.Size = new System.Drawing.Size(180, 30);
            NebRdsOn.StringFormatFunc = null;
            NebRdsOn.StyleFlags = UserControls.Style.StyleFlag.None;
            NebRdsOn.StylizeFlag = true;
            NebRdsOn.SubButtonImg = null;
            NebRdsOn.TabIndex = 1;
            NebRdsOn.Value = 0D;
            // 
            // BtnPowerPic
            // 
            BtnPowerPic.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPowerPic.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPowerPic.BorderThickness = 0;
            BtnPowerPic.CornerRadius = 0;
            BtnPowerPic.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnPowerPic.DaskArray = null;
            BtnPowerPic.DropKey = System.Windows.Forms.Keys.Space;
            BtnPowerPic.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnPowerPic.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPowerPic.Height = 30;
            BtnPowerPic.Icon = null;
            BtnPowerPic.IconOffset = 10;
            BtnPowerPic.IconSize = new System.Drawing.Size(24, 24);
            BtnPowerPic.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnPowerPic.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnPowerPic.IsIndicatorShow = false;
            BtnPowerPic.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnPowerPic.Location = new System.Drawing.Point(19, 180);
            BtnPowerPic.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPowerPic.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPowerPic.MouseInBorderThickness = 0;
            BtnPowerPic.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPowerPic.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnPowerPic.Name = "BtnPowerPic";
            BtnPowerPic.PressedBackColor = System.Drawing.Color.Gray;
            BtnPowerPic.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPowerPic.PressedBorderThickness = 0;
            BtnPowerPic.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPowerPic.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnPowerPic.Size = new System.Drawing.Size(140, 30);
            BtnPowerPic.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnPowerPic.StylizeFlag = true;
            BtnPowerPic.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPowerPic.SVGPath = "";
            BtnPowerPic.TabIndex = 92;
            BtnPowerPic.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu");
            BtnPowerPic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnPowerPic.Click += BtnPowerPic_Click;
            // 
            // BtnGuide
            // 
            BtnGuide.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.BorderThickness = 0;
            BtnGuide.CornerRadius = 0;
            BtnGuide.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnGuide.DaskArray = null;
            BtnGuide.DropKey = System.Windows.Forms.Keys.Space;
            BtnGuide.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnGuide.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.Height = 30;
            BtnGuide.Icon = null;
            BtnGuide.IconOffset = 10;
            BtnGuide.IconSize = new System.Drawing.Size(24, 24);
            BtnGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnGuide.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.IsIndicatorShow = false;
            BtnGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnGuide.Location = new System.Drawing.Point(372, 180);
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
            BtnGuide.Size = new System.Drawing.Size(121, 30);
            BtnGuide.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnGuide.StylizeFlag = true;
            BtnGuide.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.SVGPath = "";
            BtnGuide.TabIndex = 52;
            BtnGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinHaoLianJieShiYi");
            BtnGuide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnGuide.Click += BtnGuide_Click;
            // 
            // BtnResultTable
            // 
            BtnResultTable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.BorderThickness = 0;
            BtnResultTable.CornerRadius = 0;
            BtnResultTable.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResultTable.DaskArray = null;
            BtnResultTable.DropKey = System.Windows.Forms.Keys.Space;
            BtnResultTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResultTable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.Height = 30;
            BtnResultTable.Icon = null;
            BtnResultTable.IconOffset = 10;
            BtnResultTable.IconSize = new System.Drawing.Size(24, 24);
            BtnResultTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResultTable.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.IsIndicatorShow = false;
            BtnResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResultTable.Location = new System.Drawing.Point(206, 180);
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
            BtnResultTable.Size = new System.Drawing.Size(100, 30);
            BtnResultTable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnResultTable.StylizeFlag = true;
            BtnResultTable.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.SVGPath = "";
            BtnResultTable.TabIndex = 93;
            BtnResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieGuoBiao");
            BtnResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResultTable.Click += BtnResultTable_Click;
            // 
            // PwrSwitchingLossPageEx
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnResultTable);
            Controls.Add(BtnPowerPic);
            Controls.Add(ScopeXLabel6);
            Controls.Add(LblRdsOn);
            Controls.Add(NebRdsOn);
            Controls.Add(ChkActive);
            Controls.Add(BtnGuide);
            Controls.Add(LblCurrentSrc);
            Controls.Add(LblVoltageSrc);
            Controls.Add(CbxCurrentSrc);
            Controls.Add(CbxVoltageSrc);
            Name = "PwrSwitchingLossPageEx";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXIconButton BtnEnergyPic;
        private ScopeX.UserControls.UIRadioButtonGroup SourceType;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.ScopeXLabel LblVoltageSrc;
        //private ScopeX.UserControls.ComboBoxEx CbxCurrentSrc;
        //private ScopeX.UserControls.ComboBoxEx CbxVoltageSrc;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel6;
        private ScopeX.UserControls.ScopeXLabel LblRdsOn;
        private ScopeX.UserControls.TouchNeb NebRdsOn;
        private ScopeX.UserControls.ComboBoxEx comboBoxEx1;
        private ScopeX.UserControls.ComboBoxEx comboBoxEx2;
        private ScopeX.UserControls.TouchNeb NebFrequencyValueStart;
        private ScopeX.UserControls.ScopeXIconButton BtnPowerPic;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
        private ScopeX.UserControls.SelectComboBox CbxVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
    }
}
