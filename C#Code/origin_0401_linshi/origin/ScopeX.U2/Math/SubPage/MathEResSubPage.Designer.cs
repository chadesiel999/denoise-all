
namespace ScopeX.U2
{
    partial class MathEResSubPage
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
            CbxSource = new ScopeX.UserControls.SelectComboBox();
            LblSource = new ScopeX.UserControls.ScopeXLabel();
            LblBits = new ScopeX.UserControls.ScopeXLabel();
            NebBits = new ScopeX.UserControls.TouchNeb();
            SuspendLayout();
            // 
            // CbxSource
            // 
            CbxSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
    //        CbxSource.Items = new string[]
    //{
    //"M1",
    //"M2",
    //"M3",
    //"M4",
    //"M5",
    //"M6",
    //"M7",
    //"M8",
    //"C1",
    //"C2",
    //"C3",
    //"C4",
    //"R1",
    //"R2",
    //"R3",
    //"R4"
    //};
            CbxSource.Location = new System.Drawing.Point(10, 31);
            CbxSource.Name = "CbxSource";
            CbxSource.SelectValue = 0;
            CbxSource.Size = new System.Drawing.Size(100, 30);
            CbxSource.TabIndex = 4;
            // 
            // LblSource
            // 
            LblSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt1;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 3);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(100, 18);
            LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TabStop = false;
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathEResSubPage.LblSource");// "源";
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // LblBits
            // 
            LblBits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblBits.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblBits.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblBits.BorderThickness = 0;
            LblBits.CornerRadius = 0;
            LblBits.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblBits.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblBits.HighlightPrompt = defaultHighlightPrompt2;
            LblBits.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblBits.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblBits.Location = new System.Drawing.Point(155, 3);
            LblBits.MultyLineFlag = false;
            LblBits.Name = "LblBits";
            LblBits.Size = new System.Drawing.Size(170, 18);
            LblBits.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblBits.StylizeFlag = true;
            LblBits.TabIndex = 2;
            LblBits.TabStop = false;
            LblBits.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathEResSubPage.LblBits");// "增强位数";
            LblBits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblBits.Token = null;
            // 
            // NebBits
            // 
            NebBits.AddButtonImg = null;
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
            NebBits.AddButtonStyle = buttonStyle1;
            NebBits.AllwaysShowFocusImage = false;
            NebBits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NebBits.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebBits.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebBits.BorderThickness = 0;
            NebBits.DisableHoldOnInput = false;
            NebBits.DropKey = System.Windows.Forms.Keys.Space;
            NebBits.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebBits.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebBits.FocusImage = null;
            NebBits.FocusImagePosition = ScopeX.UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebBits.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebBits.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebBits.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebBits.Height = 30;
            NebBits.HoldOnSpeedLevel = 5;
            NebBits.IconWidthProportion = 1F;
            NebBits.Interval = 1D;
            NebBits.LanguageKey = null;
            NebBits.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebBits.Location = new System.Drawing.Point(155, 31);
            NebBits.Name = "NebBits";
            NebBits.Size = new System.Drawing.Size(170, 30);
            NebBits.StringFormatFunc = null;
            NebBits.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NebBits.StylizeFlag = true;
            NebBits.SubButtonImg = null;
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
            NebBits.SubButtonStyle = buttonStyle2;
            NebBits.TabIndex = 3;
            NebBits.Value = 0D;
            // 
            // MathEResSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(NebBits);
            Controls.Add(LblBits);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathEResSubPage";
            Size = new System.Drawing.Size(400, 75);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXLabel LblBits;
        private ScopeX.UserControls.TouchNeb NebBits;
        private ScopeX.UserControls.SelectComboBox CbxSource;
    }
}
