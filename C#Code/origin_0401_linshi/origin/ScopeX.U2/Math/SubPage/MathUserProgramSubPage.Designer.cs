
namespace ScopeX.U2
{
    partial class MathUserProgramSubPage
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
            CbxSource = new ScopeX.UserControls.SelectComboBox();
            LblSource = new ScopeX.UserControls.ScopeXLabel();
            BtnEditCode = new ScopeX.UserControls.ScopeXIconButton();
            LblEngineType = new ScopeX.UserControls.ScopeXLabel();
            CbxEngineType = new ScopeX.UserControls.SelectComboBox();
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
            CbxSource.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4",
    "P1",
    "P2",
    "P3",
    "P4",
    "P5",
    "P6",
    "P7",
    "P8",
    "M1",
    "M2",
    "M3",
    "M4",
    "M5",
    "M6",
    "M7",
    "M8"
    };
            CbxSource.Location = new System.Drawing.Point(10, 32);
            CbxSource.Name = "CbxSource";
            CbxSource.Size = new System.Drawing.Size(100, 30);
            CbxSource.TabIndex = 5;
            // 
            // LblSource
            // 
            LblSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("MiSans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt1;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 6);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(100, 21);
            LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.FontSize;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TabStop = false;
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathUserProgramSubPage.LblSource");// "源";
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // BtnEditCode
            // 
            BtnEditCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnEditCode.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnEditCode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnEditCode.BorderThickness = 1;
            BtnEditCode.CornerRadius = 0;
            BtnEditCode.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnEditCode.DaskArray = null;
            BtnEditCode.DropKey = System.Windows.Forms.Keys.Space;
            BtnEditCode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnEditCode.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnEditCode.Height = 30;
            BtnEditCode.Icon = null;
            BtnEditCode.IconOffset = 10;
            BtnEditCode.IconSize = new System.Drawing.Size(24, 24);
            BtnEditCode.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnEditCode.IsIndicatorShow = false;
            BtnEditCode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnEditCode.Location = new System.Drawing.Point(239, 32);
            BtnEditCode.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnEditCode.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnEditCode.MouseInBorderThickness = 1;
            BtnEditCode.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnEditCode.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnEditCode.Name = "BtnEditCode";
            BtnEditCode.PressedBackColor = System.Drawing.Color.Gray;
            BtnEditCode.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnEditCode.PressedBorderThickness = 1;
            BtnEditCode.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnEditCode.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnEditCode.Size = new System.Drawing.Size(136, 30);
            BtnEditCode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnEditCode.StylizeFlag = true;
            BtnEditCode.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnEditCode.SVGPath = "";
            BtnEditCode.TabIndex = 4;
            BtnEditCode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathUserProgramSubPage.BtnEditCode");// "代码编辑";
            BtnEditCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnEditCode.Click += BtnEditCode_Click;
            // 
            // LblEngineType
            // 
            LblEngineType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblEngineType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblEngineType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblEngineType.BorderThickness = 0;
            LblEngineType.CornerRadius = 0;
            LblEngineType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblEngineType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblEngineType.HighlightPrompt = defaultHighlightPrompt2;
            LblEngineType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblEngineType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblEngineType.Location = new System.Drawing.Point(133, 6);
            LblEngineType.MultyLineFlag = false;
            LblEngineType.Name = "LblEngineType";
            LblEngineType.Size = new System.Drawing.Size(120, 21);
            LblEngineType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.FontSize;
            LblEngineType.StylizeFlag = true;
            LblEngineType.TabIndex = 2;
            LblEngineType.TabStop = false;
            LblEngineType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathUserProgramSubPage.LblEngineType");// "引擎库类型";
            LblEngineType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblEngineType.Token = null;
            // 
            // CbxEngineType
            // 
            CbxEngineType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxEngineType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxEngineType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxEngineType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxEngineType.DataSource = null;
            CbxEngineType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxEngineType.ForeColor = System.Drawing.Color.White;
            CbxEngineType.Items = new string[]
    {
    "Matlab",
    "JavaScript",
    "Python"
    };
            CbxEngineType.Location = new System.Drawing.Point(133, 32);
            CbxEngineType.Name = "CbxEngineType";
            CbxEngineType.Size = new System.Drawing.Size(100, 30);
            CbxEngineType.TabIndex = 6;
            // 
            // MathUserProgramSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(LblEngineType);
            Controls.Add(CbxEngineType);
            Controls.Add(BtnEditCode);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathUserProgramSubPage";
            Size = new System.Drawing.Size(400, 85);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXIconButton BtnEditCode;
        private ScopeX.UserControls.ScopeXLabel LblEngineType;
        private ScopeX.UserControls.SelectComboBox CbxSource;
        private ScopeX.UserControls.SelectComboBox CbxEngineType;
    }
}
