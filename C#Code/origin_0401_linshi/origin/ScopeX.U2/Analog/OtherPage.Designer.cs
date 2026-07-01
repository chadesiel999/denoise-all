
namespace ScopeX.U2
{
    partial class OtherPage
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
            TbxDelay = new ScopeX.UserControls.ScopeXTextBox();
            LblPhaseCheck = new ScopeX.UserControls.ScopeXLabel();
            LblFigure = new ScopeX.UserControls.ScopeXLabel();
            BtnHistgram = new ScopeX.UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // TbxDelay
            // 
            TbxDelay.AcceptsTab = false;
            TbxDelay.AutoShowKeyBoard = true;
            TbxDelay.AutoSize = false;
            TbxDelay.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxDelay.BorderColor = System.Drawing.Color.Black;
            TbxDelay.BorderThickness = 0;
            TbxDelay.CornerRadius = 0;
            TbxDelay.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxDelay.Enabled = true;
            TbxDelay.EnbleSelectBorder = true;
            TbxDelay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxDelay.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxDelay.Height = 30;
            TbxDelay.HideSelection = true;  
            TbxDelay.KeyboardVerify = null;
            TbxDelay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxDelay.Location = new System.Drawing.Point(48, 147);
            TbxDelay.MaxLength = 32767;
            TbxDelay.Modified = false;
            TbxDelay.MouseEnterState = false;
            TbxDelay.Multiline = false;
            TbxDelay.Name = "TbxDelay";
            TbxDelay.ProcessCmdKeyFunc = null;
            TbxDelay.ReadOnly = false;
            TbxDelay.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxDelay.SelectedText = "";
            TbxDelay.SelectionLength = 0;
            TbxDelay.SelectionStart = 0;
            TbxDelay.ShortcutsEnabled = true;
            TbxDelay.Size = new System.Drawing.Size(116, 30);
            TbxDelay.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            TbxDelay.StylizeFlag = true;
            TbxDelay.TabIndex = 1;
            TbxDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxDelay.UseSystemPasswordChar = false;
            TbxDelay.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxDelay.Visible = false;
            TbxDelay.WordWrap = true;
            // 
            // LblPhaseCheck
            // 
            LblPhaseCheck.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPhaseCheck.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPhaseCheck.BorderThickness = 0;
            LblPhaseCheck.CornerRadius = 0;
            LblPhaseCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPhaseCheck.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPhaseCheck.HighlightPrompt = defaultHighlightPrompt1;
            LblPhaseCheck.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPhaseCheck.Location = new System.Drawing.Point(19, 109);
            LblPhaseCheck.MultyLineFlag = false;
            LblPhaseCheck.Name = "LblPhaseCheck";
            LblPhaseCheck.Size = new System.Drawing.Size(116, 18);
            LblPhaseCheck.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblPhaseCheck.StylizeFlag = true;
            LblPhaseCheck.TabIndex = 0;
            LblPhaseCheck.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangChaXiaoZheng");
            LblPhaseCheck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPhaseCheck.Token = null;
            LblPhaseCheck.Visible = false;
            // 
            // LblFigure
            // 
            LblFigure.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblFigure.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblFigure.BorderThickness = 0;
            LblFigure.CornerRadius = 0;
            LblFigure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFigure.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblFigure.HighlightPrompt = defaultHighlightPrompt2;
            LblFigure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFigure.Location = new System.Drawing.Point(19, 18);
            LblFigure.MultyLineFlag = false;
            LblFigure.Name = "LblFigure";
            LblFigure.Size = new System.Drawing.Size(57, 30);
            LblFigure.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblFigure.StylizeFlag = true;
            LblFigure.TabIndex = 2;
            LblFigure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuTu_");
            LblFigure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblFigure.Token = null;
            LblFigure.Visible = false;
            // 
            // BtnHistgram
            // 
            BtnHistgram.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistgram.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistgram.BorderThickness = 0;
            BtnHistgram.CornerRadius = 0;
            BtnHistgram.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnHistgram.DaskArray = null;
            BtnHistgram.DropKey = System.Windows.Forms.Keys.Space;
            BtnHistgram.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistgram.Height = 30;
            BtnHistgram.Icon = null;
            BtnHistgram.IconOffset = 10;
            BtnHistgram.IconSize = new System.Drawing.Size(24, 24);
            BtnHistgram.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnHistgram.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnHistgram.IsIndicatorShow = false;
            BtnHistgram.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnHistgram.Location = new System.Drawing.Point(48, 54);
            BtnHistgram.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistgram.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistgram.MouseInBorderThickness = 0;
            BtnHistgram.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistgram.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnHistgram.Name = "BtnHistgram";
            BtnHistgram.PressedBackColor = System.Drawing.Color.Gray;
            BtnHistgram.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistgram.PressedBorderThickness = 0;
            BtnHistgram.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistgram.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnHistgram.Size = new System.Drawing.Size(77, 30);
            BtnHistgram.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnHistgram.StylizeFlag = true;
            BtnHistgram.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistgram.SVGPath = "";
            BtnHistgram.TabIndex = 15;
            BtnHistgram.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiFangTu");
            BtnHistgram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnHistgram.Visible = false;
            BtnHistgram.Click += BtnHistgram_Click;
            // 
            // OtherPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnHistgram);
            Controls.Add(LblFigure);
            Controls.Add(TbxDelay);
            Controls.Add(LblPhaseCheck);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "OtherPage";
            Size = new System.Drawing.Size(375, 206);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXTextBox TbxDelay;
        private ScopeX.UserControls.ScopeXLabel LblPhaseCheck;
        private ScopeX.UserControls.ScopeXLabel LblFigure;
        private ScopeX.UserControls.ScopeXIconButton BtnHistgram;
    }
}
