namespace ScopeX.U2
{
    partial class KeyboardDetectionForm
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
            LblScreen = new UserControls.ScopeXLabel();
            scopexLabel1 = new UserControls.ScopeXLabel();
            TlpOptions = new System.Windows.Forms.TableLayoutPanel();
            PlShow = new System.Windows.Forms.Panel();
            LblHintTwo = new UserControls.ScopeXLabel();
            LblHintOne = new UserControls.ScopeXLabel();
            TlpOptions.SuspendLayout();
            PlShow.SuspendLayout();
            SuspendLayout();
            // 
            // LblScreen
            // 
            LblScreen.BackColor = System.Drawing.Color.Empty;
            LblScreen.BorderColor = System.Drawing.Color.Black;
            LblScreen.BorderThickness = 0;
            LblScreen.CornerRadius = 0;
            LblScreen.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblScreen.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblScreen.HighlightPrompt = defaultHighlightPrompt1;
            LblScreen.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblScreen.IsOmittext = true;
            LblScreen.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblScreen.Location = new System.Drawing.Point(918, 625);
            LblScreen.MultyLineFlag = false;
            LblScreen.Name = "LblScreen";
            LblScreen.Size = new System.Drawing.Size(85, 30);
            LblScreen.StyleFlags = UserControls.Style.StyleFlag.None;
            LblScreen.StylizeFlag = true;
            LblScreen.TabIndex = 24;
            LblScreen.TabStop = false;
            LblScreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblScreen.Token = null;
            // 
            // scopexLabel1
            // 
            scopexLabel1.BackColor = System.Drawing.Color.Empty;
            scopexLabel1.BorderColor = System.Drawing.Color.Black;
            scopexLabel1.BorderThickness = 0;
            scopexLabel1.CornerRadius = 0;
            scopexLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            scopexLabel1.HighlightPrompt = defaultHighlightPrompt2;
            scopexLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel1.IsOmittext = true;
            scopexLabel1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel1.Location = new System.Drawing.Point(926, 633);
            scopexLabel1.MultyLineFlag = false;
            scopexLabel1.Name = "scopexLabel1";
            scopexLabel1.Size = new System.Drawing.Size(85, 30);
            scopexLabel1.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel1.StylizeFlag = true;
            scopexLabel1.TabIndex = 25;
            scopexLabel1.TabStop = false;
            scopexLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel1.Token = null;
            // 
            // TlpOptions
            // 
            TlpOptions.ColumnCount = 2;
            TlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            TlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            TlpOptions.Controls.Add(PlShow, 0, 0);
            TlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpOptions.Location = new System.Drawing.Point(0, 0);
            TlpOptions.Name = "TlpOptions";
            TlpOptions.RowCount = 1;
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpOptions.Size = new System.Drawing.Size(1960, 1100);
            TlpOptions.TabIndex = 26;
            // 
            // PlShow
            // 
            PlShow.Controls.Add(LblHintTwo);
            PlShow.Controls.Add(LblHintOne);
            PlShow.Dock = System.Windows.Forms.DockStyle.Fill;
            PlShow.Location = new System.Drawing.Point(3, 3);
            PlShow.Name = "PlShow";
            PlShow.Size = new System.Drawing.Size(1464, 1094);
            PlShow.TabIndex = 0;
            // 
            // LblHintTwo
            // 
            LblHintTwo.BackColor = System.Drawing.Color.Transparent;
            LblHintTwo.BorderColor = System.Drawing.Color.Black;
            LblHintTwo.BorderThickness = 0;
            LblHintTwo.CornerRadius = 0;
            LblHintTwo.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHintTwo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblHintTwo.HighlightPrompt = defaultHighlightPrompt3;
            LblHintTwo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblHintTwo.IsOmittext = true;
            LblHintTwo.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHintTwo.Location = new System.Drawing.Point(139, 150);
            LblHintTwo.MultyLineFlag = false;
            LblHintTwo.Name = "LblHintTwo";
            LblHintTwo.Size = new System.Drawing.Size(1012, 41);
            LblHintTwo.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHintTwo.StylizeFlag = true;
            LblHintTwo.TabIndex = 28;
            LblHintTwo.TabStop = false;
            LblHintTwo.Text = "请按下按键板上的按钮进行测试。";
            LblHintTwo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHintTwo.Token = null;
            // 
            // LblHintOne
            // 
            LblHintOne.BackColor = System.Drawing.Color.Transparent;
            LblHintOne.BorderColor = System.Drawing.Color.Black;
            LblHintOne.BorderThickness = 0;
            LblHintOne.CornerRadius = 0;
            LblHintOne.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHintOne.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblHintOne.HighlightPrompt = defaultHighlightPrompt4;
            LblHintOne.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblHintOne.IsOmittext = true;
            LblHintOne.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHintOne.Location = new System.Drawing.Point(139, 103);
            LblHintOne.MultyLineFlag = false;
            LblHintOne.Name = "LblHintOne";
            LblHintOne.Size = new System.Drawing.Size(1012, 41);
            LblHintOne.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHintOne.StylizeFlag = true;
            LblHintOne.TabIndex = 27;
            LblHintOne.TabStop = false;
            LblHintOne.Text = "连续按3下RUN/STOP退出自检模式!!!";
            LblHintOne.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHintOne.Token = null;
            // 
            // KeyboardDetectionForm
            // 
            ActiveBorderColor = System.Drawing.Color.Black;
            ActiveBorderVisiable = false;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            BorderBackColor = System.Drawing.Color.DimGray;
            ClientSize = new System.Drawing.Size(1960, 1100);
            ContentBackColor = System.Drawing.Color.Black;
            Controls.Add(TlpOptions);
            Controls.Add(scopexLabel1);
            Controls.Add(LblScreen);
            HeadHeight = 0;
            Name = "KeyboardDetectionForm";
            Padding = new System.Windows.Forms.Padding(0);
            ShowHead = true;
            Controls.SetChildIndex(LblScreen, 0);
            Controls.SetChildIndex(scopexLabel1, 0);
            Controls.SetChildIndex(TlpOptions, 0);
            TlpOptions.ResumeLayout(false);
            PlShow.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private UserControls.ScopeXLabel LblScreen;
        private UserControls.ScopeXLabel scopexLabel1;
        private System.Windows.Forms.TableLayoutPanel TlpOptions;
        private System.Windows.Forms.Panel PlShow;
        private UserControls.ScopeXLabel LblHintOne;
        private UserControls.ScopeXLabel LblHintTwo;
    }
}
