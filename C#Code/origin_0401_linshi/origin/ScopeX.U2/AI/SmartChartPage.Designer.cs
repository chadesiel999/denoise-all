namespace ScopeX.U2
{
    partial class SmartChartPage
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ChkSignalRecognition = new ScopeX.UserControls.ScopeXSwitchButton();
            LblSignalRecognition = new ScopeX.UserControls.ScopeXLabel();
            LblSource = new ScopeX.UserControls.ScopeXLabel();
            CbxSource = new ScopeX.UserControls.ComboBoxEx();
            LblAiSetEnable = new ScopeX.UserControls.ScopeXLabel();
            ChkAiSetEnable = new ScopeX.UserControls.ScopeXSwitchButton();
            LblAiWindows = new ScopeX.UserControls.ScopeXLabel();
            ChkAiWindows = new ScopeX.UserControls.ScopeXSwitchButton();
            LblAiParams = new ScopeX.UserControls.ScopeXLabel();
            ChkAiParams = new ScopeX.UserControls.ScopeXSwitchButton();
            SuspendLayout();
            // 
            // ChkSignalRecognition
            // 
            ChkSignalRecognition.AnimationCount = 8;
            ChkSignalRecognition.AnimationFunc = null;
            ChkSignalRecognition.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSignalRecognition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSignalRecognition.BorderThickness = 0;
            ChkSignalRecognition.Checked = false;
            ChkSignalRecognition.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkSignalRecognition.CheckedForeColor = System.Drawing.Color.Black;
            ChkSignalRecognition.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSignalRecognition.CheckedText = "开";
            ChkSignalRecognition.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkSignalRecognition.DropKey = System.Windows.Forms.Keys.Space;
            ChkSignalRecognition.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSignalRecognition.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkSignalRecognition.Height = 30;
            ChkSignalRecognition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkSignalRecognition.Location = new System.Drawing.Point(9, 101);
            ChkSignalRecognition.Margin = new System.Windows.Forms.Padding(0);
            ChkSignalRecognition.Name = "ChkSignalRecognition";
            ChkSignalRecognition.Size = new System.Drawing.Size(75, 30);
            ChkSignalRecognition.SliderButtonWidth = 30;
            ChkSignalRecognition.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSignalRecognition.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkSignalRecognition.StylizeFlag = true;
            ChkSignalRecognition.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkSignalRecognition.TabIndex = 2;
            ChkSignalRecognition.Text = "关";
            ChkSignalRecognition.UseAnimation = true;
            ChkSignalRecognition.Click += ChkSignalRecognition_Click;
            // 
            // LblSignalRecognition
            // 
            LblSignalRecognition.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSignalRecognition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSignalRecognition.BorderThickness = 0;
            LblSignalRecognition.CornerRadius = 0;
            LblSignalRecognition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSignalRecognition.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSignalRecognition.HighlightPrompt = defaultHighlightPrompt1;
            LblSignalRecognition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSignalRecognition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSignalRecognition.Location = new System.Drawing.Point(9, 81);
            LblSignalRecognition.MultyLineFlag = false;
            LblSignalRecognition.Name = "LblSignalRecognition";
            LblSignalRecognition.Size = new System.Drawing.Size(84, 18);
            LblSignalRecognition.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblSignalRecognition.StylizeFlag = true;
            LblSignalRecognition.TabIndex = 4;
            LblSignalRecognition.TabStop = false;
            LblSignalRecognition.Text = "信号识别";
            LblSignalRecognition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSignalRecognition.Token = null;
            // 
            // LblSource
            // 
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt2;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(9, 3);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(42, 20);
            LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 7;
            LblSource.TabStop = false;
            LblSource.Text = "通道1";
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderThickness = 0;
            CbxSource.ContainerBackColor = null;
            CbxSource.ContainerForeColor = null;
            CbxSource.CornerRadius = 0;
            CbxSource.DataSource = null;
            CbxSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxSource.DropDownHeight = 200;
            CbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxSource.DropDownWidth = 90;
            CbxSource.DropKey = System.Windows.Forms.Keys.Space;
            CbxSource.DroppedDown = false;
            CbxSource.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxSource.FormattingEnabled = true;
            CbxSource.GetDisPlayName = null;
            CbxSource.Height = 26;
            CbxSource.ImageMode = false;
            CbxSource.ItemHeight = 28;
            CbxSource.Items.AddRange(new object[] { "C1", "C2", "C3", "C4" });
            CbxSource.KeyDropEnble = true;
            CbxSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxSource.Location = new System.Drawing.Point(9, 29);
            CbxSource.MaxDropDownItems = 8;
            CbxSource.Name = "CbxSource";
            CbxSource.RectBtnWidth = 20;
            CbxSource.SelectedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            CbxSource.SelectedIndex = -1;
            CbxSource.SelectedItem = null;
            CbxSource.SelectedText = "";
            CbxSource.Size = new System.Drawing.Size(90, 26);
            CbxSource.Soreted = false;
            CbxSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxSource.StylizeFlag = true;
            CbxSource.TabIndex = 53;
            CbxSource.Text = "";
            CbxSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblAiSetEnable
            // 
            LblAiSetEnable.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAiSetEnable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAiSetEnable.BorderThickness = 0;
            LblAiSetEnable.CornerRadius = 0;
            LblAiSetEnable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAiSetEnable.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAiSetEnable.HighlightPrompt = defaultHighlightPrompt3;
            LblAiSetEnable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAiSetEnable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAiSetEnable.Location = new System.Drawing.Point(136, 5);
            LblAiSetEnable.MultyLineFlag = false;
            LblAiSetEnable.Name = "LblAiSetEnable";
            LblAiSetEnable.Size = new System.Drawing.Size(84, 18);
            LblAiSetEnable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAiSetEnable.StylizeFlag = true;
            LblAiSetEnable.TabIndex = 55;
            LblAiSetEnable.TabStop = false;
            LblAiSetEnable.Text = "智能图表";
            LblAiSetEnable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAiSetEnable.Token = null;
            // 
            // ChkAiSetEnable
            // 
            ChkAiSetEnable.AnimationCount = 8;
            ChkAiSetEnable.AnimationFunc = null;
            ChkAiSetEnable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiSetEnable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiSetEnable.BorderThickness = 0;
            ChkAiSetEnable.Checked = false;
            ChkAiSetEnable.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkAiSetEnable.CheckedForeColor = System.Drawing.Color.Black;
            ChkAiSetEnable.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiSetEnable.CheckedText = "开";
            ChkAiSetEnable.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkAiSetEnable.DropKey = System.Windows.Forms.Keys.Space;
            ChkAiSetEnable.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiSetEnable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkAiSetEnable.Height = 30;
            ChkAiSetEnable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAiSetEnable.Location = new System.Drawing.Point(136, 25);
            ChkAiSetEnable.Margin = new System.Windows.Forms.Padding(0);
            ChkAiSetEnable.Name = "ChkAiSetEnable";
            ChkAiSetEnable.Size = new System.Drawing.Size(75, 30);
            ChkAiSetEnable.SliderButtonWidth = 30;
            ChkAiSetEnable.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiSetEnable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkAiSetEnable.StylizeFlag = true;
            ChkAiSetEnable.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAiSetEnable.TabIndex = 54;
            ChkAiSetEnable.Text = "关";
            ChkAiSetEnable.UseAnimation = true;
            ChkAiSetEnable.Click += ChkAiSetEnable_Click;
            // 
            // LblAiWindows
            // 
            LblAiWindows.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAiWindows.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAiWindows.BorderThickness = 0;
            LblAiWindows.CornerRadius = 0;
            LblAiWindows.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAiWindows.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAiWindows.HighlightPrompt = defaultHighlightPrompt4;
            LblAiWindows.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAiWindows.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAiWindows.Location = new System.Drawing.Point(127, 81);
            LblAiWindows.MultyLineFlag = false;
            LblAiWindows.Name = "LblAiWindows";
            LblAiWindows.Size = new System.Drawing.Size(84, 18);
            LblAiWindows.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAiWindows.StylizeFlag = true;
            LblAiWindows.TabIndex = 57;
            LblAiWindows.TabStop = false;
            LblAiWindows.Text = "智能图窗";
            LblAiWindows.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAiWindows.Token = null;
            // 
            // ChkAiWindows
            // 
            ChkAiWindows.AnimationCount = 8;
            ChkAiWindows.AnimationFunc = null;
            ChkAiWindows.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiWindows.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiWindows.BorderThickness = 0;
            ChkAiWindows.Checked = false;
            ChkAiWindows.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkAiWindows.CheckedForeColor = System.Drawing.Color.Black;
            ChkAiWindows.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiWindows.CheckedText = "开";
            ChkAiWindows.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkAiWindows.DropKey = System.Windows.Forms.Keys.Space;
            ChkAiWindows.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiWindows.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkAiWindows.Height = 30;
            ChkAiWindows.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAiWindows.Location = new System.Drawing.Point(127, 101);
            ChkAiWindows.Margin = new System.Windows.Forms.Padding(0);
            ChkAiWindows.Name = "ChkAiWindows";
            ChkAiWindows.Size = new System.Drawing.Size(75, 30);
            ChkAiWindows.SliderButtonWidth = 30;
            ChkAiWindows.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiWindows.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkAiWindows.StylizeFlag = true;
            ChkAiWindows.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAiWindows.TabIndex = 56;
            ChkAiWindows.Text = "关";
            ChkAiWindows.UseAnimation = true;
            ChkAiWindows.Click += ChkAiWindows_Click;
            // 
            // LblAiParams
            // 
            LblAiParams.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAiParams.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAiParams.BorderThickness = 0;
            LblAiParams.CornerRadius = 0;
            LblAiParams.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAiParams.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAiParams.HighlightPrompt = defaultHighlightPrompt5;
            LblAiParams.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAiParams.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAiParams.Location = new System.Drawing.Point(244, 81);
            LblAiParams.MultyLineFlag = false;
            LblAiParams.Name = "LblAiParams";
            LblAiParams.Size = new System.Drawing.Size(84, 18);
            LblAiParams.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAiParams.StylizeFlag = true;
            LblAiParams.TabIndex = 59;
            LblAiParams.TabStop = false;
            LblAiParams.Text = "智能参数";
            LblAiParams.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAiParams.Token = null;
            // 
            // ChkAiParams
            // 
            ChkAiParams.AnimationCount = 8;
            ChkAiParams.AnimationFunc = null;
            ChkAiParams.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiParams.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiParams.BorderThickness = 0;
            ChkAiParams.Checked = false;
            ChkAiParams.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkAiParams.CheckedForeColor = System.Drawing.Color.Black;
            ChkAiParams.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiParams.CheckedText = "开";
            ChkAiParams.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkAiParams.DropKey = System.Windows.Forms.Keys.Space;
            ChkAiParams.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAiParams.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkAiParams.Height = 30;
            ChkAiParams.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAiParams.Location = new System.Drawing.Point(244, 101);
            ChkAiParams.Margin = new System.Windows.Forms.Padding(0);
            ChkAiParams.Name = "ChkAiParams";
            ChkAiParams.Size = new System.Drawing.Size(75, 30);
            ChkAiParams.SliderButtonWidth = 30;
            ChkAiParams.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAiParams.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkAiParams.StylizeFlag = true;
            ChkAiParams.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAiParams.TabIndex = 58;
            ChkAiParams.Text = "关";
            ChkAiParams.UseAnimation = true;
            ChkAiParams.Click += ChkAiParams_Click;
            // 
            // SmartChartPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(LblAiParams);
            Controls.Add(ChkAiParams);
            Controls.Add(LblAiWindows);
            Controls.Add(ChkAiWindows);
            Controls.Add(LblAiSetEnable);
            Controls.Add(ChkAiSetEnable);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Controls.Add(LblSignalRecognition);
            Controls.Add(ChkSignalRecognition);
            Name = "SmartChartPage";
            Size = new System.Drawing.Size(374, 210);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXSwitchButton ChkSignalRecognition;
        private ScopeX.UserControls.ScopeXLabel LblSignalRecognition;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ComboBoxEx CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblAiSetEnable;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAiSetEnable;
        private ScopeX.UserControls.ScopeXLabel LblAiWindows;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAiWindows;
        private ScopeX.UserControls.ScopeXLabel LblAiParams;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAiParams;
    }
}
