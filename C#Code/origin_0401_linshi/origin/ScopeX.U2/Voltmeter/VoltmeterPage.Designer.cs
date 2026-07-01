
using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class VoltmeterPage
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem3 = new ScopeX.UserControls.RadioButtonItem();

            ScopeX.UserControls.RadioButtonItem radioButtonItem6 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem7 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem8 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem9 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            this.RdoMode = new ScopeX.UserControls.UIRadioButtonGroup();
            this.CbxSource = new ScopeX.UserControls.SelectComboBox();
            this.ChkAutoRange = new ScopeX.UserControls.ScopeXSwitchButton();
            this.LblActive = new ScopeX.UserControls.ScopeXLabel();
            this.LblAutorange = new ScopeX.UserControls.ScopeXLabel();
            this.LblSource = new ScopeX.UserControls.ScopeXLabel();
            this.LblMode = new ScopeX.UserControls.ScopeXLabel();
            this.LblFigure = new ScopeX.UserControls.ScopeXLabel();
            this.RdoFigure = new ScopeX.UserControls.UIRadioButtonGroup();
            this.LblStatistics = new ScopeX.UserControls.ScopeXLabel();
            this.ChkStatistics = new ScopeX.UserControls.ScopeXSwitchButton();
            this.BtnResetStat = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // ChkActive
            // 
            this.ChkActive.AnimationCount = 8;
            this.ChkActive.AnimationFunc = null;
            this.ChkActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkActive.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkActive.BorderThickness = 0;
            this.ChkActive.Checked = false;
            this.ChkActive.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.ChkActive.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkActive.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkActive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkActive.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkActive.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkActive.Height = 30;
            this.ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkActive.Location = new System.Drawing.Point(10, 28);
            this.ChkActive.Margin = new System.Windows.Forms.Padding(0);
            this.ChkActive.MinimumSize = new System.Drawing.Size(1, 1);
            this.ChkActive.Name = "ChkActive";
            this.ChkActive.Size = new System.Drawing.Size(85, 35);
            this.ChkActive.SliderButtonWidth = 30;
            this.ChkActive.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkActive.StylizeFlag = true;
            this.ChkActive.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkActive.TabIndex = 1;
            this.ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkActive.UseAnimation = true;
            this.ChkActive.CheckedChangedEvent += new System.EventHandler(this.ChkActive_CheckedChangedEvent);
            // 
            // RdoMode
            // 
            this.RdoMode.BackColor = System.Drawing.Color.AliceBlue;
            this.RdoMode.BorderColor = System.Drawing.Color.AliceBlue;
            this.RdoMode.BorderThickness = 0;
            this.RdoMode.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.RdoMode.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "DC";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "AC RMS";
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "DC+AC RMS";
            this.RdoMode.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2,
        radioButtonItem3};
            this.RdoMode.ButtonOffset = 10;
            this.RdoMode.ButtonTextColor = System.Drawing.Color.White;
            this.RdoMode.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            this.RdoMode.ChoosedButtonIndex = 0;
            this.RdoMode.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoMode.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(41)))), ((int)(((byte)(44)))));
            this.RdoMode.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoMode.FocusBorderColor = System.Drawing.Color.White;
            this.RdoMode.ForeColor = System.Drawing.Color.White;
            this.RdoMode.Height = 26;
            this.RdoMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoMode.Location = new System.Drawing.Point(10, 107);
            this.RdoMode.Name = "RdoMode";
            this.RdoMode.Size = new System.Drawing.Size(486,35);
            this.RdoMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.RdoMode.StylizeFlag = true;
            this.RdoMode.TabIndex = 7;
            this.RdoMode.IndexChanged += new System.EventHandler(this.RdoMode_IndexChanged);
            // 
            // ChkAutoRange
            // 
            this.ChkAutoRange.AnimationCount = 8;
            this.ChkAutoRange.AnimationFunc = null;
            this.ChkAutoRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkAutoRange.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkAutoRange.BorderThickness = 0;
            this.ChkAutoRange.Checked = false;
            this.ChkAutoRange.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.ChkAutoRange.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkAutoRange.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkAutoRange.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkAutoRange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkAutoRange.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkAutoRange.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkAutoRange.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkAutoRange.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkAutoRange.Height = 30;
            this.ChkAutoRange.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkAutoRange.Location = new System.Drawing.Point(240, 28);
            this.ChkAutoRange.Margin = new System.Windows.Forms.Padding(0);
            this.ChkAutoRange.MinimumSize = new System.Drawing.Size(1, 1);
            this.ChkAutoRange.Name = "ChkAutoRange";
            this.ChkAutoRange.Size = new System.Drawing.Size(85, 35);
            this.ChkAutoRange.SliderButtonWidth = 30;
            this.ChkAutoRange.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkAutoRange.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkAutoRange.StylizeFlag = true;
            this.ChkAutoRange.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkAutoRange.TabIndex = 5;
            this.ChkAutoRange.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkAutoRange.UseAnimation = true;
            this.ChkAutoRange.CheckedChangedEvent += new System.EventHandler(this.ChkAutoRange_CheckedChangedEvent);
            // 
            // LblActive
            // 
            this.LblActive.BackColor = System.Drawing.Color.Empty;
            this.LblActive.BorderColor = System.Drawing.Color.Black;
            this.LblActive.BorderThickness = 0;
            this.LblActive.CornerRadius = 0;
            this.LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblActive.ForeColor = System.Drawing.Color.White;
            this.LblActive.HighlightPrompt = defaultHighlightPrompt1;
            this.LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblActive.Location = new System.Drawing.Point(10, 0);
            this.LblActive.MultyLineFlag = false;
            this.LblActive.Name = "LblActive";
            this.LblActive.Size = new System.Drawing.Size(85, 21);
            this.LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblActive.StylizeFlag = true;
            this.LblActive.TabIndex = 0;
            this.LblActive.TabStop = false;
            this.LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            this.LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblActive.Token = null;
            // 
            // LblAutorange
            // 
            this.LblAutorange.BackColor = System.Drawing.Color.Empty;
            this.LblAutorange.BorderColor = System.Drawing.Color.Black;
            this.LblAutorange.BorderThickness = 0;
            this.LblAutorange.CornerRadius = 0;
            this.LblAutorange.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblAutorange.ForeColor = System.Drawing.Color.White;
            this.LblAutorange.HighlightPrompt = defaultHighlightPrompt2;
            this.LblAutorange.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblAutorange.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblAutorange.Location = new System.Drawing.Point(240, 0);
            this.LblAutorange.MultyLineFlag = false;
            this.LblAutorange.Name = "LblAutorange";
            this.LblAutorange.Size = new System.Drawing.Size(85, 21);
            this.LblAutorange.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblAutorange.StylizeFlag = true;
            this.LblAutorange.TabIndex = 4;
            this.LblAutorange.TabStop = false;
            this.LblAutorange.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDongLiangCheng");
            this.LblAutorange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblAutorange.Token = null;
            // 
            // LblSource
            // 
            this.LblSource.BackColor = System.Drawing.Color.Empty;
            this.LblSource.BorderColor = System.Drawing.Color.Black;
            this.LblSource.BorderThickness = 0;
            this.LblSource.CornerRadius = 0;
            this.LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSource.ForeColor = System.Drawing.Color.White;
            this.LblSource.HighlightPrompt = defaultHighlightPrompt3;
            this.LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSource.Location = new System.Drawing.Point(140, 0);
            this.LblSource.MultyLineFlag = false;
            this.LblSource.Name = "LblSource";
            this.LblSource.Size = new System.Drawing.Size(85, 21);
            this.LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblSource.StylizeFlag = true;
            this.LblSource.TabIndex = 2;
            this.LblSource.TabStop = false;
            this.LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            this.LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblSource.Token = null;
            // 
            // LblMode
            // 
            this.LblMode.BackColor = System.Drawing.Color.Empty;
            this.LblMode.BorderColor = System.Drawing.Color.Black;
            this.LblMode.BorderThickness = 0;
            this.LblMode.CornerRadius = 0;
            this.LblMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblMode.ForeColor = System.Drawing.Color.White;
            this.LblMode.HighlightPrompt = defaultHighlightPrompt4;
            this.LblMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblMode.Location = new System.Drawing.Point(10, 81);
            this.LblMode.MultyLineFlag = false;
            this.LblMode.Name = "LblMode";
            this.LblMode.Size = new System.Drawing.Size(275, 21);
            this.LblMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblMode.StylizeFlag = true;
            this.LblMode.TabIndex = 6;
            this.LblMode.TabStop = false;
            this.LblMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoShi");
            this.LblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblMode.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
            CbxSource.Location = new System.Drawing.Point(140, 28);
            CbxSource.Name = "CbxSource";
            CbxSource.SelectValue = 0;
            CbxSource.Size = new System.Drawing.Size(85, 35);
            CbxSource.TabIndex = 8;
            // 
            // LblFigure
            // 
            this.LblFigure.BackColor = System.Drawing.Color.Empty;
            this.LblFigure.BorderColor = System.Drawing.Color.Black;
            this.LblFigure.BorderThickness = 0;
            this.LblFigure.CornerRadius = 0;
            this.LblFigure.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblFigure.ForeColor = System.Drawing.Color.White;
            this.LblFigure.HighlightPrompt = defaultHighlightPrompt5;
            this.LblFigure.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblFigure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblFigure.Location = new System.Drawing.Point(10, 173);
            this.LblFigure.MultyLineFlag = false;
            this.LblFigure.Name = "LblFigure";
            this.LblFigure.Size = new System.Drawing.Size(90, 20);
            this.LblFigure.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblFigure.StylizeFlag = true;
            this.LblFigure.TabIndex = 17;
            this.LblFigure.TabStop = false;
            this.LblFigure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.LblFigure");// "副图";
            this.LblFigure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblFigure.Token = null;
            // 
            // RdoFigure
            // 
            this.RdoFigure.BackColor = System.Drawing.Color.AliceBlue;
            this.RdoFigure.BorderColor = System.Drawing.Color.AliceBlue;
            this.RdoFigure.BorderThickness = 0;
            this.RdoFigure.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.RdoFigure.ButtonFont = null;
            radioButtonItem6.Icon = null;
            radioButtonItem6.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem6.Tag = null;
            radioButtonItem6.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_0");// "关闭";
            radioButtonItem7.Icon = global::ScopeX.U2.Properties.Resources.Histogram;
            radioButtonItem7.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem7.Tag = null;
            radioButtonItem7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_1");// "直方图";
            radioButtonItem8.Icon = global::ScopeX.U2.Properties.Resources.Trend;
            radioButtonItem8.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem8.Tag = null;
            radioButtonItem8.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_2");// "趋势图";
            radioButtonItem9.Icon = global::ScopeX.U2.Properties.Resources.Track;
            radioButtonItem9.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem9.Tag = null;
            radioButtonItem9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasItemCfgForm.MeasItemCfgPage.RdoFigure.panel1.Btn_3");// "追踪";
            this.RdoFigure.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem6,
        radioButtonItem7,
        radioButtonItem8};
            this.RdoFigure.ButtonOffset = 10;
            this.RdoFigure.ButtonTextColor = System.Drawing.Color.White;
            this.RdoFigure.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            this.RdoFigure.ChoosedButtonIndex = 0;
            this.RdoFigure.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoFigure.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(41)))), ((int)(((byte)(44)))));
            this.RdoFigure.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoFigure.FocusBorderColor = System.Drawing.Color.White;
            this.RdoFigure.ForeColor = System.Drawing.Color.White;
            this.RdoFigure.Height = 35;
            this.RdoFigure.IsMutiSelect = true;
            this.RdoFigure.IsRemoveSelected = false;
            this.RdoFigure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoFigure.Location = new System.Drawing.Point(10, 201);
            this.RdoFigure.Name = "RdoFigure";
            this.RdoFigure.Size = new System.Drawing.Size(486, 35);
            this.RdoFigure.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.RdoFigure.StylizeFlag = true;
            this.RdoFigure.TabIndex = 18;
            this.RdoFigure.IndexChanged += RdoFigure_IndexChanged;
            // 
            // LblStatistics
            // 
            this.LblStatistics.BackColor = System.Drawing.Color.Empty;
            this.LblStatistics.BorderColor = System.Drawing.Color.Black;
            this.LblStatistics.BorderThickness = 0;
            this.LblStatistics.CornerRadius = 0;
            this.LblStatistics.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblStatistics.ForeColor = System.Drawing.Color.White;
            this.LblStatistics.HighlightPrompt = defaultHighlightPrompt5;
            this.LblStatistics.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblStatistics.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblStatistics.Location = new System.Drawing.Point(343, 0);
            this.LblStatistics.MultyLineFlag = false;
            this.LblStatistics.Name = "LblStatistics";
            this.LblStatistics.Size = new System.Drawing.Size(90, 20);
            this.LblStatistics.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblStatistics.StylizeFlag = true;
            this.LblStatistics.TabIndex = 19;
            this.LblStatistics.TabStop = false;
            this.LblStatistics.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeasureMenuForm.MeasureMenuPage.Tlp.BtnStatistic.Tlp.BtnMain");// "副图";
            this.LblStatistics.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblStatistics.Token = null;
            // 
            // ChkStatistics
            // 
            this.ChkStatistics.AnimationCount = 8;
            this.ChkStatistics.AnimationFunc = null;
            this.ChkStatistics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkStatistics.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkStatistics.BorderThickness = 0;
            this.ChkStatistics.Checked = false;
            this.ChkStatistics.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.ChkStatistics.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkStatistics.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkStatistics.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkStatistics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkStatistics.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkStatistics.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkStatistics.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkStatistics.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkStatistics.Height = 30;
            this.ChkStatistics.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkStatistics.Location = new System.Drawing.Point(343, 28);
            this.ChkStatistics.Margin = new System.Windows.Forms.Padding(0);
            this.ChkStatistics.MinimumSize = new System.Drawing.Size(1, 1);
            this.ChkStatistics.Name = "ChkStatistics";
            this.ChkStatistics.Size = new System.Drawing.Size(85,35);
            this.ChkStatistics.SliderButtonWidth = 30;
            this.ChkStatistics.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkStatistics.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkStatistics.StylizeFlag = true;
            this.ChkStatistics.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkStatistics.TabIndex = 20;
            this.ChkStatistics.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkStatistics.UseAnimation = true;
            this.ChkStatistics.CheckedChangedEvent += ChkStatistics_CheckedChangedEvent;
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
            this.BtnResetStat.Click += BtnResetStat_Click ;
            // 
            // VoltmeterPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.RdoMode);
            this.Controls.Add(this.LblMode);
            this.Controls.Add(this.LblSource);
            this.Controls.Add(this.LblAutorange);
            this.Controls.Add(this.LblActive);
            this.Controls.Add(this.ChkAutoRange);
            this.Controls.Add(this.CbxSource);
            this.Controls.Add(this.ChkActive);
            this.Controls.Add(this.LblFigure);
            this.Controls.Add(this.RdoFigure);
            this.Controls.Add(this.LblStatistics);
            this.Controls.Add(this.ChkStatistics); 
            this.Controls.Add(this.BtnResetStat); 
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.Name = "VoltmeterPage";
            this.Size = new System.Drawing.Size(544,249);
            this.ResumeLayout(false);

        }

        #endregion
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.UIRadioButtonGroup RdoMode;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAutoRange;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXLabel LblAutorange;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXLabel LblMode;
        private ScopeX.UserControls.SelectComboBox CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblFigure;
        private ScopeX.UserControls.UIRadioButtonGroup RdoFigure;
        private ScopeX.UserControls.ScopeXLabel LblStatistics;
        private ScopeX.UserControls.ScopeXSwitchButton ChkStatistics;
        private ScopeX.UserControls.ScopeXIconButton BtnResetStat;

    }
}
