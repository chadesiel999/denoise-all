namespace ScopeX.U2
{
    partial class PwrSlewRatePage
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
            LblActive = new ScopeX.UserControls.ScopeXLabel();
            BtnShowVoltageRate = new ScopeX.UserControls.ScopeXIconButton();
            BtnShowCurrentRate = new ScopeX.UserControls.ScopeXIconButton();
            BtnGuide = new ScopeX.UserControls.ScopeXIconButton();
            BtnResultTable = new ScopeX.UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LblOutVoltageSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(373, 16);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblOutVoltageSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(140, 18);
            LblCurrentSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 55;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            LblCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCurrentSrc.Token = null;
            // 
            // LblInVoltageSrc
            // 
            LblVoltageSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVoltageSrc.BorderThickness = 0;
            LblVoltageSrc.CornerRadius = 0;
            LblVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVoltageSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVoltageSrc.Location = new System.Drawing.Point(196, 16);
            LblVoltageSrc.MultyLineFlag = false;
            LblVoltageSrc.Name = "LblInVoltageSrc";
            LblVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblVoltageSrc.StylizeFlag = true;
            LblVoltageSrc.TabIndex = 53;
            LblVoltageSrc.TabStop = false;
            LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            LblVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVoltageSrc.Token = null;
            // 
            // CbxInVoltageSrc
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
            CbxVoltageSrc.Location = new System.Drawing.Point(196, 44);
            CbxVoltageSrc.Name = "CbxInVoltageSrc";
            CbxVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxVoltageSrc.TabIndex = 94;
            // 
            // CbxOutVoltageSrc
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
            CbxCurrentSrc.Location = new System.Drawing.Point(373, 44);
            CbxCurrentSrc.Name = "CbxOutVoltageSrc";
            CbxCurrentSrc.Size = new System.Drawing.Size(140, 30);
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
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt3;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(19, 16);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "ScopeXLabel6";
            LblActive.Size = new System.Drawing.Size(140, 18);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 75;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
            // BtnShowVoltageRate
            // 
            BtnShowVoltageRate.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowVoltageRate.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowVoltageRate.BorderThickness = 0;
            BtnShowVoltageRate.CornerRadius = 0;
            BtnShowVoltageRate.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowVoltageRate.DaskArray = null;
            BtnShowVoltageRate.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowVoltageRate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowVoltageRate.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowVoltageRate.Height = 30;
            BtnShowVoltageRate.Icon = null;
            BtnShowVoltageRate.IconOffset = 10;
            BtnShowVoltageRate.IconSize = new System.Drawing.Size(24, 24);
            BtnShowVoltageRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnShowVoltageRate.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowVoltageRate.IsIndicatorShow = false;
            BtnShowVoltageRate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowVoltageRate.Location = new System.Drawing.Point(19, 224);
            BtnShowVoltageRate.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowVoltageRate.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowVoltageRate.MouseInBorderThickness = 0;
            BtnShowVoltageRate.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowVoltageRate.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowVoltageRate.Name = "BtnShowVoltageRate";
            BtnShowVoltageRate.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowVoltageRate.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowVoltageRate.PressedBorderThickness = 0;
            BtnShowVoltageRate.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowVoltageRate.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowVoltageRate.Size = new System.Drawing.Size(140, 30);
            BtnShowVoltageRate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnShowVoltageRate.StylizeFlag = true;
            BtnShowVoltageRate.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowVoltageRate.SVGPath = "";
            BtnShowVoltageRate.TabIndex = 52;
            BtnShowVoltageRate.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaZhuanHuanSuLvTu");
            BtnShowVoltageRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowVoltageRate.Click += BtnShowVoltageRate_Click;
            // 
            // BtnShowCurrentRate
            // 
            BtnShowCurrentRate.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowCurrentRate.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowCurrentRate.BorderThickness = 0;
            BtnShowCurrentRate.CornerRadius = 0;
            BtnShowCurrentRate.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowCurrentRate.DaskArray = null;
            BtnShowCurrentRate.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowCurrentRate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowCurrentRate.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowCurrentRate.Height = 30;
            BtnShowCurrentRate.Icon = null;
            BtnShowCurrentRate.IconOffset = 10;
            BtnShowCurrentRate.IconSize = new System.Drawing.Size(24, 24);
            BtnShowCurrentRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnShowCurrentRate.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowCurrentRate.IsIndicatorShow = false;
            BtnShowCurrentRate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowCurrentRate.Location = new System.Drawing.Point(196, 224);
            BtnShowCurrentRate.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowCurrentRate.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowCurrentRate.MouseInBorderThickness = 0;
            BtnShowCurrentRate.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowCurrentRate.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowCurrentRate.Name = "BtnShowCurrentRate";
            BtnShowCurrentRate.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowCurrentRate.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowCurrentRate.PressedBorderThickness = 0;
            BtnShowCurrentRate.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowCurrentRate.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowCurrentRate.Size = new System.Drawing.Size(140, 30);
            BtnShowCurrentRate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnShowCurrentRate.StylizeFlag = true;
            BtnShowCurrentRate.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowCurrentRate.SVGPath = "";
            BtnShowCurrentRate.TabIndex = 52;
            BtnShowCurrentRate.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuZhuanHuanSuLvTu");
            BtnShowCurrentRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowCurrentRate.Click += BtnShowCurrentRate_Click;
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
            BtnGuide.Location = new System.Drawing.Point(19, 134);
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
            BtnResultTable.Location = new System.Drawing.Point(373, 224);
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
            BtnResultTable.Size = new System.Drawing.Size(140, 30);
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
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Controls.Add(LblCurrentSrc);
            Controls.Add(LblVoltageSrc);
            Controls.Add(CbxCurrentSrc);
            Controls.Add(CbxVoltageSrc);
            Controls.Add(BtnShowVoltageRate);
            Controls.Add(BtnShowCurrentRate);
            Controls.Add(BtnGuide);
            Controls.Add(BtnResultTable);
            Name = "PwrSlewRatePage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
        private ScopeX.UserControls.ScopeXIconButton BtnShowVoltageRate;
        private ScopeX.UserControls.ScopeXIconButton BtnShowCurrentRate;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
    }
}
