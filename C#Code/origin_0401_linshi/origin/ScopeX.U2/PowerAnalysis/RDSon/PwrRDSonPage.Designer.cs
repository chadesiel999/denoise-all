using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class PwrRDSonPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PwrRDSonPage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            LblActive = new UserControls.ScopeXLabel();
            ChkActive = new UserControls.ScopeXSwitchButton();
            LblVoltageSrc = new UserControls.ScopeXLabel();
            CbxVoltageSrc = new UserControls.SelectComboBox();
            LblCurrentSrc = new UserControls.ScopeXLabel();
            CbxCurrentSrc = new UserControls.SelectComboBox();
            BtnShowRdsonWave = new UserControls.ScopeXIconButton();
            BtnResultTable = new UserControls.ScopeXIconButton();
            BtnGuide = new UserControls.ScopeXIconButton();
            SuspendLayout();
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
            LblActive.Location = new System.Drawing.Point(19, 16);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(75, 18);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 75;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
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
            // LblCurrentSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblCurrentSrc.IsOmittext = true;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(373, 16);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblCurrentSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(140, 18);
            LblCurrentSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 55;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.Text= ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
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
            LblVoltageSrc.IsOmittext = true;
            LblVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVoltageSrc.Location = new System.Drawing.Point(196, 16);
            LblVoltageSrc.MultyLineFlag = false;
            LblVoltageSrc.Name = "LblVoltageSrc";
            LblVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblVoltageSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVoltageSrc.StylizeFlag = true;
            LblVoltageSrc.TabIndex = 53;
            LblVoltageSrc.TabStop = false;
            LblVoltageSrc.Text= ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
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
            CbxCurrentSrc.Location = new System.Drawing.Point(373, 44);
            CbxCurrentSrc.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxCurrentSrc.Name = "CbxCurrentSrc";
            CbxCurrentSrc.SelectIndex = 0;
            CbxCurrentSrc.SelectValue = 0;
            CbxCurrentSrc.Size = new System.Drawing.Size(140, 30);
            CbxCurrentSrc.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxCurrentSrc.StylizeFlag = true;
            CbxCurrentSrc.TabIndex = 83;
            // 
            // CbxVoltageSrc
            // 
            CbxVoltageSrc.AutoSize = true;
            CbxVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.ComBorderColor = System.Drawing.Color.Blue;
            CbxVoltageSrc.DataSource = (System.Collections.IList)resources.GetObject("CbxVoltageSrc.DataSource");
            CbxVoltageSrc.ExtText = "";
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
            CbxVoltageSrc.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxVoltageSrc.Name = "CbxVoltageSrc";
            CbxVoltageSrc.SelectIndex = 0;
            CbxVoltageSrc.SelectValue = 0;
            CbxVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxVoltageSrc.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxVoltageSrc.StylizeFlag = true;
            CbxVoltageSrc.TabIndex = 82;
            // 
            // BtnShowRdsonWave
            // 
            BtnShowRdsonWave.Adjustable = false;
            BtnShowRdsonWave.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowRdsonWave.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowRdsonWave.BorderThickness = 0;
            BtnShowRdsonWave.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowRdsonWave.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnShowRdsonWave.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowRdsonWave.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowRdsonWave.CornerRadius = 0;
            BtnShowRdsonWave.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowRdsonWave.DaskArray = null;
            BtnShowRdsonWave.DoubleClickEnable = true;
            BtnShowRdsonWave.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowRdsonWave.FineEnable = false;
            BtnShowRdsonWave.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnShowRdsonWave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowRdsonWave.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowRdsonWave.Height = 30;
            BtnShowRdsonWave.Icon = null;
            BtnShowRdsonWave.IconOffset = 10;
            BtnShowRdsonWave.IconSize = new System.Drawing.Size(24, 24);
            BtnShowRdsonWave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnShowRdsonWave.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowRdsonWave.IsChoosed = false;
            BtnShowRdsonWave.IsIndicatorShow = false;
            BtnShowRdsonWave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowRdsonWave.Location = new System.Drawing.Point(19, 164);
            BtnShowRdsonWave.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowRdsonWave.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowRdsonWave.MouseInBorderThickness = 0;
            BtnShowRdsonWave.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowRdsonWave.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowRdsonWave.Name = "BtnShowRdsonWave";
            BtnShowRdsonWave.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowRdsonWave.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowRdsonWave.PressedBorderThickness = 0;
            BtnShowRdsonWave.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowRdsonWave.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowRdsonWave.Size = new System.Drawing.Size(140, 30);
            BtnShowRdsonWave.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnShowRdsonWave.StylizeFlag = true;
            BtnShowRdsonWave.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowRdsonWave.SVGPath = "";
            BtnShowRdsonWave.TabIndex = 70;
            BtnShowRdsonWave.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Rds(on)BoXingTu");
            BtnShowRdsonWave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowRdsonWave.Click += BtnShowRdsonWave_Click;
            // 
            // BtnResultTable
            // 
            BtnResultTable.Adjustable = false;
            BtnResultTable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
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
            BtnResultTable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.Height = 30;
            BtnResultTable.Icon = null;
            BtnResultTable.IconOffset = 10;
            BtnResultTable.IconSize = new System.Drawing.Size(24, 24);
            BtnResultTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResultTable.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.IsChoosed = false;
            BtnResultTable.IsIndicatorShow = false;
            BtnResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResultTable.Location = new System.Drawing.Point(196, 164);
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
            BtnResultTable.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResultTable.StylizeFlag = true;
            BtnResultTable.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.SVGPath = "";
            BtnResultTable.TabIndex = 70;
            BtnResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieGuoBiao");
            BtnResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResultTable.Click += BtnResultTable_Click;
            // 
            // BtnGuide
            // 
            BtnGuide.Adjustable = false;
            BtnGuide.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
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
            BtnGuide.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.Height = 30;
            BtnGuide.Icon = null;
            BtnGuide.IconOffset = 10;
            BtnGuide.IconSize = new System.Drawing.Size(24, 24);
            BtnGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnGuide.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.IsChoosed = false;
            BtnGuide.IsIndicatorShow = false;
            BtnGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnGuide.Location = new System.Drawing.Point(373, 164);
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
            BtnGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinHaoLianJieShiYi");
            BtnGuide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnGuide.Click += BtnGuide_Click;
            // 
            // PwrRDSonPage
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
            Controls.Add(BtnShowRdsonWave);
            Controls.Add(BtnResultTable);
            Controls.Add(BtnGuide);
            Name = "PwrRDSonPage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
        private ScopeX.UserControls.ScopeXIconButton BtnShowRdsonWave;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
    }
}
