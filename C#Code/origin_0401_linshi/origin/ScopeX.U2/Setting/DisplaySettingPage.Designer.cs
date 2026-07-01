
namespace ScopeX.U2
{
    partial class DisplaySettingPage
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
            LabelHide = new ScopeX.UserControls.ScopeXLabel();
            LblBrightness = new ScopeX.UserControls.ScopeXLabel();
            TrbBrigtness = new UserTrackBar();
            LblContrast = new ScopeX.UserControls.ScopeXLabel();
            TrbContrast = new UserTrackBar();
            BtnBrigtnessDefultValue = new ScopeX.UserControls.ScopeXIconButton();
            BtnContrastDefultValue = new ScopeX.UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LabelHide
            // 
            LabelHide.BackColor = System.Drawing.Color.Transparent;
            LabelHide.BorderColor = System.Drawing.Color.Black;
            LabelHide.BorderThickness = 0;
            LabelHide.CornerRadius = 0;
            LabelHide.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelHide.HighlightPrompt = defaultHighlightPrompt1;
            LabelHide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelHide.Location = new System.Drawing.Point(277, 95);
            LabelHide.MultyLineFlag = false;
            LabelHide.Name = "LabelHide";
            LabelHide.Size = new System.Drawing.Size(20, 16);
            LabelHide.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LabelHide.StylizeFlag = false;
            LabelHide.TabIndex = 14;
            LabelHide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LabelHide.Token = null;
            // 
            // LblBrightnessControl
            // 
            LblBrightness.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblBrightness.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblBrightness.BorderThickness = 0;
            LblBrightness.CornerRadius = 0;
            LblBrightness.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblBrightness.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblBrightness.HighlightPrompt = defaultHighlightPrompt2;
            LblBrightness.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblBrightness.Location = new System.Drawing.Point(15, 20);
            LblBrightness.MultyLineFlag = false;
            LblBrightness.Name = "LblBrightnessControl";
            LblBrightness.Size = new System.Drawing.Size(105, 30);
            LblBrightness.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblBrightness.StylizeFlag = true;
            LblBrightness.TabIndex = 18;
            LblBrightness.TabStop = false;
            LblBrightness.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.DisplaySettingPage.LblBrightnessControl");//"屏幕亮度：";
            LblBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblBrightness.Token = null;
            // 
            // TrbBrigtness
            // 
            TrbBrigtness.DcimalDigits = 0;
            TrbBrigtness.IsShowTips = true;
            TrbBrigtness.LineColor = System.Drawing.Color.FromArgb(80, 80, 80);
            TrbBrigtness.LineWidth = 10F;
            TrbBrigtness.Location = new System.Drawing.Point(121, 20);
            TrbBrigtness.MaxValue = 100F;
            TrbBrigtness.MinValue = 5F;
            TrbBrigtness.Name = "TrbBrigtness";
            TrbBrigtness.Size = new System.Drawing.Size(207, 30);
            TrbBrigtness.TabIndex = 19;
            TrbBrigtness.TipsFormat = "0";
            //TrbBrigtness.Value = 50F;
            TrbBrigtness.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            // 
            // LblContrast
            // 
            LblContrast.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblContrast.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblContrast.BorderThickness = 0;
            LblContrast.CornerRadius = 0;
            LblContrast.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblContrast.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblContrast.HighlightPrompt = defaultHighlightPrompt3;
            LblContrast.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblContrast.Location = new System.Drawing.Point(15, 94);
            LblContrast.MultyLineFlag = false;
            LblContrast.Name = "LblContrast";
            LblContrast.Size = new System.Drawing.Size(115, 30);
            LblContrast.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblContrast.StylizeFlag = true;
            LblContrast.TabIndex = 20;
            LblContrast.TabStop = false;
            LblContrast.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.DisplaySettingPage.LblContrast");//"屏幕对比度：";
            LblContrast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblContrast.Token = null;
            // 
            // TrbContrast
            // 
            TrbContrast.DcimalDigits = 0;
            TrbContrast.IsShowTips = true;
            TrbContrast.LineColor = System.Drawing.Color.FromArgb(80, 80, 80);
            TrbContrast.LineWidth = 10F;
            TrbContrast.Location = new System.Drawing.Point(121, 94);
            TrbContrast.MaxValue = 100F;
            TrbContrast.MinValue = 50F;
            TrbContrast.Name = "TrbContrast";
            TrbContrast.Size = new System.Drawing.Size(207, 30);
            TrbContrast.TabIndex = 21;
            TrbContrast.TipsFormat = "0";
            //TrbContrast.Value = 50F;
            TrbContrast.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            // 
            // BtnBrigtnessDefultValue
            // 
            BtnBrigtnessDefultValue.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBrigtnessDefultValue.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBrigtnessDefultValue.BorderThickness = 1;
            BtnBrigtnessDefultValue.CornerRadius = 0;
            BtnBrigtnessDefultValue.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnBrigtnessDefultValue.DaskArray = null;
            BtnBrigtnessDefultValue.DropKey = System.Windows.Forms.Keys.Space;
            BtnBrigtnessDefultValue.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnBrigtnessDefultValue.ForeColor = System.Drawing.Color.White;
            BtnBrigtnessDefultValue.Height = 30;
            BtnBrigtnessDefultValue.Icon = null;
            BtnBrigtnessDefultValue.IconOffset = 10;
            BtnBrigtnessDefultValue.IconSize = new System.Drawing.Size(24, 24);
            BtnBrigtnessDefultValue.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnBrigtnessDefultValue.IsIndicatorShow = false;
            BtnBrigtnessDefultValue.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnBrigtnessDefultValue.Location = new System.Drawing.Point(353, 20);
            BtnBrigtnessDefultValue.Margin = new System.Windows.Forms.Padding(2);
            BtnBrigtnessDefultValue.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBrigtnessDefultValue.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBrigtnessDefultValue.MouseInBorderThickness = 0;
            BtnBrigtnessDefultValue.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBrigtnessDefultValue.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnBrigtnessDefultValue.Name = "BtnBrigtnessDefultValue";
            BtnBrigtnessDefultValue.PressedBackColor = System.Drawing.Color.Gray;
            BtnBrigtnessDefultValue.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBrigtnessDefultValue.PressedBorderThickness = 0;
            BtnBrigtnessDefultValue.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBrigtnessDefultValue.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnBrigtnessDefultValue.Size = new System.Drawing.Size(80, 30);
            BtnBrigtnessDefultValue.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnBrigtnessDefultValue.StylizeFlag = true;
            BtnBrigtnessDefultValue.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBrigtnessDefultValue.SVGPath = "";
            BtnBrigtnessDefultValue.TabIndex = 43;
            BtnBrigtnessDefultValue.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.DisplaySettingPage.BtnBrigtnessDefultValue");//"默认值";
            BtnBrigtnessDefultValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnContrastDefultValue
            // 
            BtnContrastDefultValue.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnContrastDefultValue.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnContrastDefultValue.BorderThickness = 1;
            BtnContrastDefultValue.CornerRadius = 0;
            BtnContrastDefultValue.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnContrastDefultValue.DaskArray = null;
            BtnContrastDefultValue.DropKey = System.Windows.Forms.Keys.Space;
            BtnContrastDefultValue.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnContrastDefultValue.ForeColor = System.Drawing.Color.White;
            BtnContrastDefultValue.Height = 30;
            BtnContrastDefultValue.Icon = null;
            BtnContrastDefultValue.IconOffset = 10;
            BtnContrastDefultValue.IconSize = new System.Drawing.Size(24, 24);
            BtnContrastDefultValue.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnContrastDefultValue.IsIndicatorShow = false;
            BtnContrastDefultValue.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnContrastDefultValue.Location = new System.Drawing.Point(353, 94);
            BtnContrastDefultValue.Margin = new System.Windows.Forms.Padding(2);
            BtnContrastDefultValue.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnContrastDefultValue.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnContrastDefultValue.MouseInBorderThickness = 0;
            BtnContrastDefultValue.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnContrastDefultValue.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnContrastDefultValue.Name = "BtnContrastDefultValue";
            BtnContrastDefultValue.PressedBackColor = System.Drawing.Color.Gray;
            BtnContrastDefultValue.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnContrastDefultValue.PressedBorderThickness = 0;
            BtnContrastDefultValue.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnContrastDefultValue.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnContrastDefultValue.Size = new System.Drawing.Size(80, 30);
            BtnContrastDefultValue.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnContrastDefultValue.StylizeFlag = true;
            BtnContrastDefultValue.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnContrastDefultValue.SVGPath = "";
            BtnContrastDefultValue.TabIndex = 44;
            BtnContrastDefultValue.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.DisplaySettingPage.BtnContrastDefultValue");//"默认值";
            BtnContrastDefultValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DisplaySettingPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnContrastDefultValue);
            Controls.Add(BtnBrigtnessDefultValue);
            Controls.Add(TrbContrast);
            Controls.Add(LblContrast);
            Controls.Add(TrbBrigtness);
            Controls.Add(LblBrightness);
            Controls.Add(LabelHide);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "DisplaySettingPage";
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(459, 410);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LabelHide;
        private ScopeX.UserControls.ScopeXLabel LblBrightness;
        private UserTrackBar TrbBrigtness;
        private ScopeX.UserControls.ScopeXLabel LblContrast;
        private UserTrackBar TrbContrast;
        private ScopeX.UserControls.ScopeXIconButton BtnBrigtnessDefultValue;
        private ScopeX.UserControls.ScopeXIconButton BtnContrastDefultValue;
    }
}
