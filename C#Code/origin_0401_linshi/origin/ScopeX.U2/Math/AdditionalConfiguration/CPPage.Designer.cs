namespace ScopeX.U2
{
    partial class CPPage
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
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            LblActive = new ScopeX.UserControls.ScopeXLabel();
            ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            LblChannelSpan = new ScopeX.UserControls.ScopeXLabel();
            NebChannelSpan = new ScopeX.UserControls.ScopeXNumericEditBox();
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
            LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(10, 1);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(133, 18);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 2;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiang");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
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
            ChkActive.Location = new System.Drawing.Point(10, 25);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 3;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // LblChannelSpan
            // 
            LblChannelSpan.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblChannelSpan.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblChannelSpan.BorderThickness = 0;
            LblChannelSpan.CornerRadius = 0;
            LblChannelSpan.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblChannelSpan.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblChannelSpan.HighlightPrompt = defaultHighlightPrompt2;
            LblChannelSpan.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblChannelSpan.Location = new System.Drawing.Point(10, 65);
            LblChannelSpan.MultyLineFlag = false;
            LblChannelSpan.Name = "LblChannelSpan";
            LblChannelSpan.Size = new System.Drawing.Size(115, 18);
            LblChannelSpan.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblChannelSpan.StylizeFlag = true;
            LblChannelSpan.TabIndex = 76;
            LblChannelSpan.TabStop = false;
            LblChannelSpan.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDaoKuanDu");
            LblChannelSpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblChannelSpan.Token = null;
            // 
            // NebChannelSpan
            // 
            NebChannelSpan.AddButtonImg = null;
            buttonBaseStyle1.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle1.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle1.BorderThickness = 0;
            buttonBaseStyle1.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseClickStyle = buttonBaseStyle1;
            buttonBaseStyle2.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle2.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle2.BorderThickness = 0;
            buttonBaseStyle2.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseInStyle = buttonBaseStyle2;
            buttonBaseStyle3.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle3.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle3.BorderThickness = 0;
            buttonBaseStyle3.ForeColor = System.Drawing.Color.White;
            buttonStyle1.NomalStyle = buttonBaseStyle3;
            NebChannelSpan.AddButtonStyle = buttonStyle1;
            NebChannelSpan.AddButtonVisibility = false;
            NebChannelSpan.AllwaysShowFocusImage = false;
            NebChannelSpan.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebChannelSpan.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebChannelSpan.BorderThickness = 0;
            NebChannelSpan.DisableHoldOnInput = false;
            NebChannelSpan.DropKey = System.Windows.Forms.Keys.Space;
            NebChannelSpan.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebChannelSpan.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebChannelSpan.FocusImage = null;
            NebChannelSpan.FocusImagePosition = ScopeX.UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebChannelSpan.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebChannelSpan.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebChannelSpan.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebChannelSpan.Height = 30;
            NebChannelSpan.HoldOnSpeedLevel = 10;
            NebChannelSpan.IconWidthProportion = 1F;
            NebChannelSpan.Interval = 0.1D;
            NebChannelSpan.LanguageKey = null;
            NebChannelSpan.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebChannelSpan.Location = new System.Drawing.Point(10, 89);
            NebChannelSpan.MaxValue = double.MaxValue;
            NebChannelSpan.MinValue = double.MinValue;
            NebChannelSpan.Name = "NebChannelSpan";
            NebChannelSpan.Size = new System.Drawing.Size(133, 30);
            NebChannelSpan.StringFormatFunc = null;
            NebChannelSpan.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NebChannelSpan.StylizeFlag = true;
            NebChannelSpan.SubButtonImg = null;
            buttonBaseStyle4.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle4.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle4.BorderThickness = 0;
            buttonBaseStyle4.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseClickStyle = buttonBaseStyle4;
            buttonBaseStyle5.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle5.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle5.BorderThickness = 0;
            buttonBaseStyle5.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseInStyle = buttonBaseStyle5;
            buttonBaseStyle6.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle6.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle6.BorderThickness = 0;
            buttonBaseStyle6.ForeColor = System.Drawing.Color.White;
            buttonStyle2.NomalStyle = buttonBaseStyle6;
            NebChannelSpan.SubButtonStyle = buttonStyle2;
            NebChannelSpan.SubButtonVisibility = false;
            NebChannelSpan.TabIndex = 78;
            NebChannelSpan.Value = 0D;
            // 
            // CPPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(NebChannelSpan);
            Controls.Add(LblChannelSpan);
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Name = "CPPage";
            Size = new System.Drawing.Size(445, 324);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblChannelSpan;
        private ScopeX.UserControls.ScopeXNumericEditBox NebChannelSpan;
    }
}
