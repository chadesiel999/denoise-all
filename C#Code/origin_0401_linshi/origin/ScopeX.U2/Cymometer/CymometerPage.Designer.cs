
namespace ScopeX.U2
{
    partial class CymometerPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            CbxSource = new UserControls.SelectComboBox();
            LblSource = new UserControls.ScopeXLabel();
            ChkActive = new UserControls.ScopeXSwitchButton();
            LblActive = new UserControls.ScopeXLabel();
            RdoSwitch = new UserControls.UIRadioButtonGroup();
            LblStatistics = new UserControls.ScopeXLabel();
            ChkStatistics = new UserControls.ScopeXSwitchButton();
            BtnResetStat = new ScopeX.UserControls.ScopeXIconButton();
            LblFigure = new UserControls.ScopeXLabel();
            RdoFigure = new UserControls.UIRadioButtonGroup();
            SuspendLayout();
            // 
            // CbxSource
            // 
            CbxSource.AutoSize = true;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.ComBorderColor = System.Drawing.Color.Blue;
            CbxSource.DataSource = null;
            CbxSource.ExtText = "";
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
            CbxSource.Location = new System.Drawing.Point(140, 28);
            CbxSource.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxSource.Name = "CbxSource";
            CbxSource.SelectIndex = -1;
            CbxSource.SelectValue = null;
            CbxSource.Size = new System.Drawing.Size(85, 35);
            CbxSource.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxSource.StylizeFlag = true;
            CbxSource.TabIndex = 4;
            // 
            // LblSource
            // 
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt1;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.IsOmittext = true;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(140, 0);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(85, 21);
            LblSource.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 2;
            LblSource.TabStop = false;
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            LblSource.Visible = false;
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
            ChkActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 35;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(10, 28);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.MinimumSize = new System.Drawing.Size(1, 1);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(85, 35);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 1;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // LblActive
            // 
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt2;
            LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblActive.IsOmittext = true;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(10, 0);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(85, 21);
            LblActive.StyleFlags = UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 0;
            LblActive.TabStop = false;
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            LblActive.Text= ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            // 
            // ChkSwitch
            // 
            RdoSwitch.BackColor = System.Drawing.Color.AliceBlue;
            RdoSwitch.BorderColor = System.Drawing.Color.AliceBlue;
            RdoSwitch.BorderThickness = 0;
            RdoSwitch.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RdoSwitch.ButtonFont = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv");// "频率";
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhouQi");// "周期";
            RdoSwitch.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem1,
    radioButtonItem2,
    };
            RdoSwitch.ButtonOffset = 10;
            RdoSwitch.ButtonTextColor = System.Drawing.Color.White;
            RdoSwitch.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RdoSwitch.ChoosedButtonIndex = 0;
            RdoSwitch.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoSwitch.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RdoSwitch.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoSwitch.FocusBorderColor = System.Drawing.Color.White;
            RdoSwitch.ForeColor = System.Drawing.Color.White;
            RdoSwitch.Height = 35;
            RdoSwitch.IsMutiSelect = true;
            RdoSwitch.IsRemoveSelected = false;
            RdoSwitch.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoSwitch.Location = new System.Drawing.Point(10, 72);
            RdoSwitch.Name = "RdoSwitch";
            RdoSwitch.Size = new System.Drawing.Size(240, 35);
            RdoSwitch.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoSwitch.StylizeFlag = true;
            RdoSwitch.TabIndex = 24;
            RdoSwitch.IndexChanged += RdoSwitch_IndexChanged;
            // 
            // LblStatistics
            // 
            LblStatistics.BackColor = System.Drawing.Color.Empty;
            LblStatistics.BorderColor = System.Drawing.Color.Black;
            LblStatistics.BorderThickness = 0;
            LblStatistics.CornerRadius = 0;
            LblStatistics.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblStatistics.ForeColor = System.Drawing.Color.White;
            LblStatistics.HighlightPrompt = defaultHighlightPrompt3;
            LblStatistics.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblStatistics.IsOmittext = true;
            LblStatistics.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblStatistics.Location = new System.Drawing.Point(343, 0);
            LblStatistics.MultyLineFlag = false;
            LblStatistics.Name = "LblStatistics";
            LblStatistics.Size = new System.Drawing.Size(90, 20);
            LblStatistics.StyleFlags = UserControls.Style.StyleFlag.None;
            LblStatistics.StylizeFlag = true;
            LblStatistics.TabIndex = 21;
            LblStatistics.TabStop = false;
            LblStatistics.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblStatistics.Token = null;
            LblStatistics.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasureMenuForm.MeasureMenuPage.Tlp.BtnStatistic.Tlp.BtnMain");// "副图";
            // 
            // ChkStatistics
            // 
            ChkStatistics.AnimationCount = 8;
            ChkStatistics.AnimationFunc = null;
            ChkStatistics.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStatistics.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStatistics.BorderThickness = 0;
            ChkStatistics.Checked = false;
            ChkStatistics.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkStatistics.CheckedForeColor = System.Drawing.Color.Black;
            ChkStatistics.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkStatistics.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkStatistics.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkStatistics.DropKey = System.Windows.Forms.Keys.Space;
            ChkStatistics.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStatistics.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkStatistics.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkStatistics.Height = 35;
            ChkStatistics.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkStatistics.Location = new System.Drawing.Point(343, 28);
            ChkStatistics.Margin = new System.Windows.Forms.Padding(0);
            ChkStatistics.MinimumSize = new System.Drawing.Size(1, 1);
            ChkStatistics.Name = "ChkStatistics";
            ChkStatistics.Size = new System.Drawing.Size(85, 35);
            ChkStatistics.SliderButtonWidth = 30;
            ChkStatistics.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkStatistics.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkStatistics.StylizeFlag = true;
            ChkStatistics.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkStatistics.TabIndex = 22;
            ChkStatistics.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkStatistics.UseAnimation = true;
            ChkStatistics.Click += ChkStatistics_CheckedChangedEvent;
            // 
            // BtnResetStat
            // 
            this.BtnResetStat.BackColor = System.Drawing.Color.Transparent;
            this.BtnResetStat.BorderColor = System.Drawing.Color.Black;
            this.BtnResetStat.BorderThickness = 1;
            this.BtnResetStat.CornerRadius = 0;
            this.BtnResetStat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnResetStat.DaskArray = null;
            this.BtnResetStat.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnResetStat.ForeColor = System.Drawing.Color.White;
            this.BtnResetStat.Height = 35;
            this.BtnResetStat.Icon = null;
            this.BtnResetStat.IconOffset = 10;
            this.BtnResetStat.IconSize = new System.Drawing.Size(24, 24);
            this.BtnResetStat.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnResetStat.IsIndicatorShow = false;
            this.BtnResetStat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnResetStat.Location = new System.Drawing.Point(435, 28);
            this.BtnResetStat.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnResetStat.MouseinBorderColor = System.Drawing.SystemColors.Control;
            this.BtnResetStat.MouseInBorderThickness = 1;
            this.BtnResetStat.MouseinForeColor = System.Drawing.Color.Black;
            this.BtnResetStat.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnResetStat.Name = "BtnResetStat";
            this.BtnResetStat.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnResetStat.PressedBorderColor = System.Drawing.SystemColors.Control;
            this.BtnResetStat.PressedBorderThickness = 1;
            this.BtnResetStat.PressedForeColor = System.Drawing.Color.Black;
            this.BtnResetStat.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnResetStat.Size = new System.Drawing.Size(55, 35);
            this.BtnResetStat.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnResetStat.StylizeFlag = true;
            this.BtnResetStat.SVGForeColor = System.Drawing.Color.Black;
            this.BtnResetStat.SVGPath = "";
            this.BtnResetStat.TabIndex = 12;
            this.BtnResetStat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");// "统计复位";
            this.BtnResetStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnResetStat.Click += BtnResetStat_Click;
            // 
            // LblFigure
            // 
            LblFigure.BackColor = System.Drawing.Color.Empty;
            LblFigure.BorderColor = System.Drawing.Color.Black;
            LblFigure.BorderThickness = 0;
            LblFigure.CornerRadius = 0;
            LblFigure.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFigure.ForeColor = System.Drawing.Color.White;
            LblFigure.HighlightPrompt = defaultHighlightPrompt4;
            LblFigure.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblFigure.IsOmittext = true;
            LblFigure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFigure.Location = new System.Drawing.Point(10, 173);
            LblFigure.MultyLineFlag = false;
            LblFigure.Name = "LblFigure";
            LblFigure.Size = new System.Drawing.Size(90, 20);
            LblFigure.StyleFlags = UserControls.Style.StyleFlag.None;
            LblFigure.StylizeFlag = true;
            LblFigure.TabIndex = 23;
            LblFigure.TabStop = false;
            LblFigure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblFigure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.LblFigure");// "副图";
            LblFigure.Token = null;
            // 
            // RdoFigure
            // 
            RdoFigure.BackColor = System.Drawing.Color.AliceBlue;
            RdoFigure.BorderColor = System.Drawing.Color.AliceBlue;
            RdoFigure.BorderThickness = 0;
            RdoFigure.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RdoFigure.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_0");// "关闭";
            radioButtonItem2.Icon = Properties.Resources.Histogram;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_1");// "直方图";
            radioButtonItem3.Icon = Properties.Resources.Trend;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_2");// "趋势图";
            radioButtonItem4.Icon = Properties.Resources.Track;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_3");// "追踪";
            RdoFigure.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem1,
    radioButtonItem2,
    radioButtonItem3
    };
            RdoFigure.ButtonOffset = 10;
            RdoFigure.ButtonTextColor = System.Drawing.Color.White;
            RdoFigure.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RdoFigure.ChoosedButtonIndex = 0;
            RdoFigure.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoFigure.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RdoFigure.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoFigure.Font = new System.Drawing.Font("MiSans", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoFigure.FocusBorderColor = System.Drawing.Color.White;
            RdoFigure.ForeColor = System.Drawing.Color.White;
            RdoFigure.Height = 35;
            RdoFigure.IsMutiSelect = true;
            RdoFigure.IsRemoveSelected = false;
            RdoFigure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoFigure.Location = new System.Drawing.Point(10, 201);
            RdoFigure.Name = "RdoFigure";
            RdoFigure.Size = new System.Drawing.Size(486, 35);
            RdoFigure.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoFigure.StylizeFlag = false;
            RdoFigure.TabIndex = 24;
            RdoFigure.IndexChanged += RdoFigure_IndexChanged;
            // 
            // CymometerPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(LblFigure);
            Controls.Add(RdoFigure);
            Controls.Add(RdoSwitch);
            Controls.Add(LblStatistics);
            Controls.Add(ChkStatistics);
            Controls.Add(BtnResetStat);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Controls.Add(ChkActive);
            Controls.Add(LblActive);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "CymometerPage";
            Size = new System.Drawing.Size(544, 249);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScopeX.UserControls.SelectComboBox CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private UserControls.UIRadioButtonGroup RdoSwitch;
        private UserControls.ScopeXLabel LblStatistics;
        private UserControls.ScopeXSwitchButton ChkStatistics;
        private ScopeX.UserControls.ScopeXIconButton BtnResetStat;
        private UserControls.ScopeXLabel LblFigure;
        private UserControls.UIRadioButtonGroup RdoFigure;
    }
}
