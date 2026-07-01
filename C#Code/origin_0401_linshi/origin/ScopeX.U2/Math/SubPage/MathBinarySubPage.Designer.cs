
namespace ScopeX.U2
{
    partial class MathBinarySubPage
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem3 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem4 = new ScopeX.UserControls.RadioButtonItem();
            CbxRightSource = new ScopeX.UserControls.SelectComboBox();
            LblRightSource = new ScopeX.UserControls.ScopeXLabel();
            CbxLeftSource = new ScopeX.UserControls.SelectComboBox();
            LblLeftSource = new ScopeX.UserControls.ScopeXLabel();
            LblOperator = new ScopeX.UserControls.ScopeXLabel();
            RdoOperator = new ScopeX.UserControls.UIRadioButtonGroup();
            SuspendLayout();
            // 
            // CbxRightSource
            // 
            CbxRightSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxRightSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxRightSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxRightSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxRightSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxRightSource.ForeColor = System.Drawing.Color.White;
            CbxRightSource.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4",
    "M1",
    "M2",
    "M3",
    "M4",
    "M5",
    "M6",
    "M7",
    "M8"
    };
            CbxRightSource.Location = new System.Drawing.Point(10, 106);
            CbxRightSource.Name = "CbxRightSource";
            CbxRightSource.Size = new System.Drawing.Size(100, 26);
            CbxRightSource.TabIndex = 7;
            // 
            // LblRightSource
            // 
            LblRightSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblRightSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblRightSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblRightSource.BorderThickness = 0;
            LblRightSource.CornerRadius = 0;
            LblRightSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblRightSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblRightSource.HighlightPrompt = defaultHighlightPrompt1;
            LblRightSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblRightSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblRightSource.Location = new System.Drawing.Point(10, 78);
            LblRightSource.MultyLineFlag = false;
            LblRightSource.Name = "LblRightSource";
            LblRightSource.Size = new System.Drawing.Size(100, 18);
            LblRightSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblRightSource.StylizeFlag = true;
            LblRightSource.TabIndex = 2;
            LblRightSource.TabStop = false;
            LblRightSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathBinarySubPage.LblRightSource"); // "源2";
            LblRightSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblRightSource.Token = null;
             // 
            // CbxLeftSource
            // 
            CbxLeftSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxLeftSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxLeftSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxLeftSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxLeftSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxLeftSource.ForeColor = System.Drawing.Color.White;
            CbxLeftSource.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4",
    "M1",
    "M2",
    "M3",
    "M4",
    "M5",
    "M6",
    "M7",
    "M8"
    };
            CbxLeftSource.Location = new System.Drawing.Point(10, 31);
            CbxLeftSource.Name = "CbxLeftSource";
            CbxLeftSource.Size = new System.Drawing.Size(100, 26);
            CbxLeftSource.TabIndex = 6;
            // 
            // LblLeftSource
            // 
            LblLeftSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblLeftSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblLeftSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblLeftSource.BorderThickness = 0;
            LblLeftSource.CornerRadius = 0;
            LblLeftSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLeftSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblLeftSource.HighlightPrompt = defaultHighlightPrompt2;
            LblLeftSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblLeftSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLeftSource.Location = new System.Drawing.Point(10, 3);
            LblLeftSource.MultyLineFlag = false;
            LblLeftSource.Name = "LblLeftSource";
            LblLeftSource.Size = new System.Drawing.Size(100, 18);
            LblLeftSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblLeftSource.StylizeFlag = true;
            LblLeftSource.TabIndex = 0;
            LblLeftSource.TabStop = false;
            LblLeftSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathBinarySubPage.LblLeftSource"); // "源1";
            LblLeftSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblLeftSource.Token = null;
            // 
            // LblOperator
            // 
            LblOperator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblOperator.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOperator.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOperator.BorderThickness = 0;
            LblOperator.CornerRadius = 0;
            LblOperator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOperator.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOperator.HighlightPrompt = defaultHighlightPrompt3;
            LblOperator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblOperator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOperator.Location = new System.Drawing.Point(155, 45);
            LblOperator.MultyLineFlag = false;
            LblOperator.Name = "LblOperator";
            LblOperator.Size = new System.Drawing.Size(220, 18);
            LblOperator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblOperator.StylizeFlag = true;
            LblOperator.TabIndex = 4;
            LblOperator.TabStop = false;
            LblOperator.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathBinarySubPage.LblOperator"); // "运算";
            LblOperator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOperator.Token = null;
            // 
            // RdoOperator
            // 
            RdoOperator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RdoOperator.BackColor = System.Drawing.Color.Black;
            RdoOperator.BorderColor = System.Drawing.Color.Black;
            RdoOperator.BorderThickness = 0;
            RdoOperator.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoOperator.ButtonFont = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "+";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "-";
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "×";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = "÷";
            RdoOperator.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2, radioButtonItem3, radioButtonItem4 };
            RdoOperator.ButtonOffset = 10;
            RdoOperator.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoOperator.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoOperator.ChoosedButtonIndex = 0;
            RdoOperator.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoOperator.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoOperator.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoOperator.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoOperator.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoOperator.Height = 30;
            RdoOperator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoOperator.Location = new System.Drawing.Point(155, 73);
            RdoOperator.Margin = new System.Windows.Forms.Padding(4);
            RdoOperator.Name = "RdoOperator";
            RdoOperator.Size = new System.Drawing.Size(220, 30);
            RdoOperator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.FontSize;
            RdoOperator.StylizeFlag = true;
            RdoOperator.TabIndex = 5;
            RdoOperator.IndexChanged += RdoOperator_SelectedIndexChanged;
            // 
            // MathBinarySubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(RdoOperator);
            Controls.Add(LblOperator);
            Controls.Add(CbxRightSource);
            Controls.Add(LblRightSource);
            Controls.Add(CbxLeftSource);
            Controls.Add(LblLeftSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathBinarySubPage";
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(389, 150);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LblRightSource;
        private ScopeX.UserControls.ScopeXLabel LblLeftSource;
        private ScopeX.UserControls.ScopeXLabel LblOperator;
        private ScopeX.UserControls.UIRadioButtonGroup RdoOperator;
        private ScopeX.UserControls.SelectComboBox CbxLeftSource;
        private ScopeX.UserControls.SelectComboBox CbxRightSource;
    }
}
