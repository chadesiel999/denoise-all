namespace ScopeX.U2
{
    partial class DataExportPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            ChkMeasure = new System.Windows.Forms.CheckBox();
            BtnSave = new UserControls.ScopeXIconButton();
            ChkSuffix = new UserControls.ScopeXSwitchButton();
            TbxFileName = new UserControls.ScopeXTextBox();
            LblTime = new UserControls.ScopeXLabel();
            LabelFileName = new UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // ChkMeasure
            // 
            ChkMeasure.AutoSize = true;
            ChkMeasure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkMeasure.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkMeasure.Location = new System.Drawing.Point(13, 14);
            ChkMeasure.Name = "ChkMeasure";
            ChkMeasure.Size = new System.Drawing.Size(94, 21);
            ChkMeasure.TabIndex = 34;
            ChkMeasure.Text = "参数快照";
            ChkMeasure.UseVisualStyleBackColor = true;
            ChkMeasure.CheckStateChanged += ChkMeasure_CheckStateChanged;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = System.Drawing.Color.Transparent;
            BtnSave.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderThickness = 0;
            BtnSave.CornerRadius = 0;
            BtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSave.DaskArray = null;
            BtnSave.DropKey = System.Windows.Forms.Keys.Space;
            BtnSave.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSave.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSave.Height = 30;
            BtnSave.Icon = null;
            BtnSave.IconOffset = 10;
            BtnSave.IconSize = new System.Drawing.Size(24, 24);
            BtnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnSave.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSave.IsChoosed = false;
            BtnSave.IsIndicatorShow = false;
            BtnSave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSave.Location = new System.Drawing.Point(13, 176);
            BtnSave.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseInBorderThickness = 0;
            BtnSave.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Name = "BtnSave";
            BtnSave.PressedBackColor = System.Drawing.Color.Gray;
            BtnSave.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.PressedBorderThickness = 0;
            BtnSave.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Size = new System.Drawing.Size(75, 30);
            BtnSave.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSave.StylizeFlag = true;
            BtnSave.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.SVGPath = "";
            BtnSave.TabIndex = 35;
            BtnSave.Text = "保 存";
            BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSave.Click += BtnSave_Click;
            // 
            // ChkSuffix
            // 
            ChkSuffix.AnimationCount = 8;
            ChkSuffix.AnimationFunc = null;
            ChkSuffix.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.BorderThickness = 0;
            ChkSuffix.Checked = false;
            ChkSuffix.CheckedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ChkSuffix.CheckedForeColor = System.Drawing.Color.Black;
            ChkSuffix.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.CheckedText = "开";
            ChkSuffix.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkSuffix.DropKey = System.Windows.Forms.Keys.Space;
            ChkSuffix.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkSuffix.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkSuffix.Height = 30;
            ChkSuffix.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkSuffix.Location = new System.Drawing.Point(450, 112);
            ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            ChkSuffix.Name = "ChkSuffix";
            ChkSuffix.Size = new System.Drawing.Size(120, 30);
            ChkSuffix.SliderButtonWidth = 30;
            ChkSuffix.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkSuffix.StylizeFlag = true;
            ChkSuffix.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkSuffix.TabIndex = 39;
            ChkSuffix.Text = "关";
            ChkSuffix.UseAnimation = true;
            // 
            // TbxFileName
            // 
            TbxFileName.AcceptsTab = false;
            TbxFileName.AutoShowKeyBoard = true;
            TbxFileName.AutoSize = false;
            TbxFileName.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderThickness = 0;
            TbxFileName.CornerRadius = 0;
            TbxFileName.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxFileName.Enabled = true;
            TbxFileName.EnbleSelectBorder = true;
            TbxFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxFileName.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxFileName.Height = 30;
            TbxFileName.HideSelection = true;
            TbxFileName.KeyboardVerify = null;
            TbxFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxFileName.Location = new System.Drawing.Point(13, 115);
            TbxFileName.Margin = new System.Windows.Forms.Padding(0);
            TbxFileName.MaxLength = 32767;
            TbxFileName.Modified = false;
            TbxFileName.MouseEnterState = false;
            TbxFileName.Multiline = false;
            TbxFileName.Name = "TbxFileName";
            TbxFileName.ProcessCmdKeyFunc = null;
            TbxFileName.ReadOnly = false;
            TbxFileName.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxFileName.SelectedText = "";
            TbxFileName.SelectionLength = 0;
            TbxFileName.SelectionStart = 0;
            TbxFileName.ShortcutsEnabled = true;
            TbxFileName.Size = new System.Drawing.Size(423, 30);
            TbxFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxFileName.StylizeFlag = true;
            TbxFileName.TabIndex = 37;
            TbxFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxFileName.UseSystemPasswordChar = false;
            TbxFileName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxFileName.WordWrap = true;
            // 
            // LblTime
            // 
            LblTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTime.BorderThickness = 0;
            LblTime.CornerRadius = 0;
            LblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTime.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTime.HighlightPrompt = defaultHighlightPrompt7;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(450, 69);
            LblTime.Margin = new System.Windows.Forms.Padding(0);
            LblTime.MultyLineFlag = false;
            LblTime.Name = "LblTime";
            LblTime.Size = new System.Drawing.Size(150, 18);
            LblTime.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTime.StylizeFlag = true;
            LblTime.TabIndex = 38;
            LblTime.TabStop = false;
            LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTime.Token = null;
            // 
            // LabelFileName
            // 
            LabelFileName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileName.BorderThickness = 0;
            LabelFileName.CornerRadius = 0;
            LabelFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileName.HighlightPrompt = defaultHighlightPrompt8;
            LabelFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileName.Location = new System.Drawing.Point(13, 69);
            LabelFileName.Margin = new System.Windows.Forms.Padding(0);
            LabelFileName.MultyLineFlag = false;
            LabelFileName.Name = "LabelFileName";
            LabelFileName.Size = new System.Drawing.Size(343, 18);
            LabelFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelFileName.StylizeFlag = true;
            LabelFileName.TabIndex = 36;
            LabelFileName.TabStop = false;
            LabelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelFileName.Token = null;
            // 
            // DataExportPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(ChkSuffix);
            Controls.Add(TbxFileName);
            Controls.Add(LblTime);
            Controls.Add(LabelFileName);
            Controls.Add(BtnSave);
            Controls.Add(ChkMeasure);
            ForeColor = System.Drawing.Color.Black;
            Name = "DataExportPage";
            Size = new System.Drawing.Size(606, 291);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox ChkMeasure;
        private UserControls.ScopeXIconButton BtnSave;
        private UserControls.ScopeXSwitchButton ChkSuffix;
        private UserControls.ScopeXTextBox TbxFileName;
        private UserControls.ScopeXLabel LblTime;
        private UserControls.ScopeXLabel LabelFileName;
    }
}
